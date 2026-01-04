using AimBurst.ShootersLayout.Runtime.Abstractions;
using AimBurst.ShootersLayout.Unity.Actor;
using TMPro;
using UnityEngine;

namespace AimBurst.ShootersLayout.Unity.UI {
    internal class ShooterUI : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Shooter shooter;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI bulletTxt;
        [SerializeField] private float bulletTxtFadedAlpha;

        private void Awake() {
            shooter.OnAmmoChanged += Shooter_OnAmmoChanged;
            shooter.OnPositioningChanged += Shooter_OnPositioningChanged;
            shooter.OnLocked += Shooter_OnLocked;
        }

        private void Shooter_OnAmmoChanged(int remainingAmmo) {
            string ammo = remainingAmmo.ToString();
            bulletTxt.SetText(ammo);
        }

        private void Shooter_OnPositioningChanged() {
            if (shooter.IsHidden) { return; }

            if (shooter.Positioning == ShooterPositioning.Lane) {
                SetBulletTxtAlpha(bulletTxtFadedAlpha);
            }
            else { SetBulletTxtAlpha(1f); }
        }

        private void SetBulletTxtAlpha(float alpha) {
            Color txtColor = bulletTxt.color;
            txtColor.a = alpha;
            bulletTxt.color = txtColor;
        }

        private void Shooter_OnLocked(bool locked) {
            if (locked) { bulletTxt.alpha = 0f; }
            else { bulletTxt.alpha = 1f; }
        }
    }
}
