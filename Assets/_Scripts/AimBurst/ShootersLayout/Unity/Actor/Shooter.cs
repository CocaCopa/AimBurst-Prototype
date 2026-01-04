using System;
using System.Collections;
using System.Threading.Tasks;
using AimBurst.ColorMap;
using AimBurst.Core.Contracts;
using AimBurst.LevelLayout.Contracts;
using AimBurst.PrefabRegistry;
using AimBurst.ShootersLayout.Runtime.Abstractions;
using AimBurst.ShootersLayout.Unity.Abstractions;
using AimBurst.ShootersLayout.Unity.Pathing;
using CocaCopa.Core.Animation;
using CocaCopa.Primitives;
using CocaCopa.Unity.Numerics;
using UnityEngine;

namespace AimBurst.ShootersLayout.Unity.Actor {
    [RequireComponent(typeof(Collider))]
    internal class Shooter : MonoBehaviour, IShooterInit, IShooterState, IShooterLane, IShooterCombat, IClickable {
        // Combat Friend
        [SerializeField] private Shooter combatFriend;

        [Header("Lane Movement")]
        [SerializeField] private AnimationCurve moveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField, Min(0f)] private float speed;

        [Header("Shooting")]
        [SerializeField] private float idleTime;
        [SerializeField] private CubeColor targetCubes;
        [SerializeField, Min(0f)] private int totalBullets;
        [SerializeField, Min(0f)] private float fireRate;
        [SerializeField] private RotationSpeed rotationSpeed;
        [SerializeField] private float maxRotationAngle;

        #region Private State
        private ValueAnimator movementAnimator;
        private ShooterVisuals visuals;
        private ShootersBezier bezierPaths;
        private ShooterPositioning positioning;
        private Collider col;

        private Vector3 lookDir;
        private float idleTimer;
        private int ownerIndex;
        private int remainingAmmo;
        private bool isHidden;
        private bool aimTowardsTarget;
        private bool onCooldown;
        #endregion

        #region Private Data
        private class MovementEase : IEasing {
            private readonly AnimationCurve curve;
            public MovementEase(AnimationCurve curve) => this.curve = curve;
            public float Evaluate(float t) => curve.Evaluate(t);
        }

        [Serializable]
        private struct RotationSpeed { public float idle; public float shoot; }
        #endregion

        #region Events
        internal event Action<int> OnAmmoChanged;
        internal event Action OnAdvanced;
        internal event Action OnPositioningChanged;
        internal event Action<bool> OnLocked;
        #endregion

        #region Unity Lifecycle
        private void Awake() {
            visuals = GetComponentInChildren<ShooterVisuals>(true);
            col = GetComponent<Collider>();
            var movementEase = new MovementEase(moveCurve);
            movementAnimator = ValueAnimator.BySpeed(0f, 1f, speed, movementEase);
            aimTowardsTarget = false;
            onCooldown = false;
            idleTimer = idleTime;
            remainingAmmo = totalBullets;
        }

        private void Start() {
            if (isHidden) {
                visuals.Lock();
                OnLocked?.Invoke(true);
            }
        }

        private void Update() {
            HandleCombatFriendConnection();
            Aim();
        }
        #endregion

        #region IClickable
        public void HandleClick() {
            visuals.Compress();
        }
        #endregion

        #region IShooterInit
        public void SetOwnerIndex(int index) => ownerIndex = index;
        public void WireShootersBezierRef(ShootersBezier reference) => bezierPaths = reference;
        public void SetCombatFriend(Shooter shooter) {
            combatFriend = shooter;
            UnityEngine.Color color = ShooterMaterialsAPI.Get(Color).color;
            visuals.EnableFriendConnection(color);
        }
        public void SetAmmo(int amount) { remainingAmmo = amount; OnAmmoChanged?.Invoke(remainingAmmo); }
        public void SetHidden(bool hidden) => isHidden = hidden;
        #endregion

        #region IShooterState
        public IShooterCombat CombatFriend => combatFriend;
        public C_Vector3 Position => transform.position.ToCore();
        public CubeColor Color => targetCubes;
        public bool HasAmmo => remainingAmmo > 0;
        public bool OnCooldown => onCooldown;
        public int RemainingAmmo => remainingAmmo;
        public bool IsHidden => isHidden;
        public int LaneIndex => ownerIndex;
        public ShooterPositioning Positioning => positioning;
        #endregion

        #region IShooterLane
        public void SetPositioning(ShooterPositioning positioning) {
            if (positioning != ShooterPositioning.Lane) {
                col.enabled = false;
            }
            this.positioning = positioning;
            OnPositioningChanged?.Invoke();
        }

        public Task Advance(float dist) {
            Vector3 currentPos = transform.position;
            Vector3 targetPos = Vector3.back * dist;
            return AdvanceRoutine(currentPos, targetPos);
        }
        #endregion

        #region IShooterCombat
        public void Shoot(ICube targetCube) {
            aimTowardsTarget = true;
            if (remainingAmmo == 0) { return; }
            visuals.RecoilEffect();
            visuals.Shoot();
            lookDir = (targetCube.Position.ToUnity() - transform.position).normalized;
            lookDir.y = 0;
            idleTimer = idleTime;
            remainingAmmo--;
            if (remainingAmmo == 0) { visuals.Highlight(); }
            var bulletObj = PrefabAPI.InstantiateBullet();
            var bulletComp = bulletObj.GetComponent<Bullet>();
            bulletComp.Fire(transform.position, targetCube);
            OnAmmoChanged?.Invoke(remainingAmmo);

            StartCoroutine(OnCooldownRoutine());
        }

        public void Kill() {
            visuals.Disable();
        }

        public async Task MoveToSlot(int toSlot) {
            visuals.EnableFootsteps(true);
            var path = bezierPaths.GetSlotPathInstance(ownerIndex, toSlot);
            path.Reset();
            path.SetTarget(transform);
            path.Play();
            while (path.IsActive) { await Task.Yield(); }
            visuals.EnableFootsteps(false);
            aimTowardsTarget = true;
        }

        public async Task MoveOut(int slot, bool curved) {
            visuals.EnableFootsteps(true);
            visuals.DisableFriendConnection();
            var path = bezierPaths.GetMoveOutPathInstance(slot, curved);
            path.Reset();
            path.SetTarget(transform);
            path.Play();
            while (path.IsActive) { await Task.Yield(); }
            visuals.EnableFootsteps(false);
        }

        public void Merge(int newAmmo) {
            remainingAmmo = newAmmo;
            visuals.Highlight();
            OnAmmoChanged?.Invoke(remainingAmmo);
        }
        #endregion

        #region Private
        private void HandleCombatFriendConnection() {
            if (combatFriend == null) { return; }
            Vector3 startPos = transform.position;
            Vector3 endPos = (transform.position + combatFriend.transform.position) / 2f;
            visuals.FriendConnectionPosition(startPos, endPos);
        }

        private void Aim() {
            if (!aimTowardsTarget) { return; }

            float speed = SwitchToIdleDir() ? rotationSpeed.idle : rotationSpeed.shoot;
            Vector3 dir = SwitchToIdleDir() ? Vector3.back : lookDir;

            if (dir.sqrMagnitude < 0.0001f) { return; }

            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, speed * Time.deltaTime);

            if (idleTimer.Equals(0)) { return; }

            Vector3 euler = transform.localEulerAngles;
            if (euler.y > 180f) { euler.y -= 360f; }
            euler.y = Mathf.Clamp(euler.y, -25f, 25f);
            transform.localEulerAngles = euler;
        }

        private bool SwitchToIdleDir() {
            if (transform.forward != Vector3.back) {
                idleTimer -= Time.deltaTime;
                idleTimer = Mathf.Clamp01(idleTimer);
                if (idleTimer.Equals(0)) { return true; }
            }
            return false;
        }

        private async Task AdvanceRoutine(Vector3 curr, Vector3 target) {
            movementAnimator.ResetAnimator();
            visuals.EnableFootsteps(true);
            while (!movementAnimator.IsComplete) {
                float t = movementAnimator.Evaluate(Time.deltaTime);
                transform.position = curr + target * t;
                await Task.Yield();
            }
            visuals.EnableFootsteps(false);
            if (positioning == ShooterPositioning.Front) {
                if (isHidden) {
                    isHidden = false;
                    visuals.Unlock();
                    OnLocked?.Invoke(false);
                }
                visuals.Highlight();
            }
            OnAdvanced?.Invoke();
        }

        private IEnumerator OnCooldownRoutine() {
            var cooldown = Time.time + (1f / fireRate);
            onCooldown = true;
            while (Time.time < cooldown) { yield return null; }
            onCooldown = false;
        }
        #endregion
    }
}
