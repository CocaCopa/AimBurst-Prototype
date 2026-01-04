using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AimBurst.Core.Contracts;
using AimBurst.LevelLayout.API;
using AimBurst.LevelLayout.Contracts;
using AimBurst.ShootersLayout.Contracts;
using AimBurst.ShootersLayout.Runtime.Abstractions;
using CocaCopa.Logger.API;

namespace AimBurst.ShootersLayout.Runtime {
    internal class SlotsManager : ISlotManager, ISlotsState {
        private readonly ISlotsAnimator slotsAnimator;
        private readonly int totalSlots;
        private readonly object reservationLock = new object();
        private readonly int releaseDelayMs;
        private readonly CancellationToken ct;

        /// <summary>
        /// Returns the slot that the given shooter belongs to.
        /// <list type="bullet">
        ///     <item><description><b>Key</b>: Shooter reference</description></item>
        ///     <item><description><b>Value</b>: Slot that owns the shooter</description></item>
        /// </list>
        /// </summary>
        private readonly Dictionary<IShooterCombat, ShooterSlot> slotsMap = new Dictionary<IShooterCombat, ShooterSlot>();

        private ShooterSlot[] shooterSlots;
        private int middleSlotIndex;

        public int TotalSlots => totalSlots;

        internal SlotsManager(ISlotsAnimator slotsAnimator, int totalSlots, int releaseDelayMs, CancellationToken ct) {
            this.slotsAnimator = slotsAnimator;
            this.totalSlots = totalSlots;
            this.releaseDelayMs = releaseDelayMs;
            this.ct = ct;
        }

        internal void Init() {
            shooterSlots = new ShooterSlot[totalSlots];
            middleSlotIndex = (int)Math.Ceiling(totalSlots * 0.5f) - 1;
            _ = ObserveSlotsInactive();
        }

        public async Task<bool> ObserveSlotsInactive() {
            const int confirmMs = 1000;
            const int pollMs = 50;
            int inactiveTime = 0;

            while (true) {
                if (AreAllInactive()) {
                    inactiveTime += pollMs;
                    if (inactiveTime >= confirmMs) {
                        return true;
                    }
                }
                else { inactiveTime = 0; }

                var sw = Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < pollMs) {
                    await Task.Yield();
                }
            }
        }

        private bool AreAllInactive() {
            for (int i = 0; i < shooterSlots.Length; i++) {
                var shooter = shooterSlots[i];
                if (shooter == null || !shooter.active.HasValue || shooter.active.Value) {
                    return false;
                }
            }
            return true;
        }

        public async Task AcceptShooterAsync(IShooterCombat shooter) {
            try {
                ShooterSlot slot = slotsMap[shooter];
                if (slot == null) { return; }

                await slot.shooter.MoveToSlot(slot.index);
                slot.onPosition = true;

                if (TryFindTripleColor(shooterSlots, out var triple)) {
                    IShooterCombat mergedShooter = await MergeShootersAsync(triple);
                    ReleaseSlot(triple[0], triple[2]);
                    slot = slotsMap[mergedShooter];
                    slot.forceStop = false;
                }
                await ActiveShooterUnload(slot);
                if (!slot.forceStop) {
                    if (slot.shooter.CombatFriend != null) {
                        while (slot.shooter.CombatFriend.HasAmmo && !slot.forceStop) {
                            await Task.Yield();
                        }
                    }

                    var sw = Stopwatch.StartNew();
                    while (sw.ElapsedMilliseconds < releaseDelayMs && !slot.forceStop) {
                        await Task.Yield();
                    }

                    _ = MoveShooterOutOfBounds(slot.index, slot);
                }
            }
            catch (Exception ex) {
                Log.Error($"<color=white>[{nameof(SlotsManager)}]</color> {nameof(AcceptShooterAsync)} failed: {ex}");
                throw;
            }
        }

        private bool TryFindTripleColor(ShooterSlot[] currentShooters, out IShooterCombat[] result) {
            lock (reservationLock) {
                var byColor = new Dictionary<CubeColor, (IShooterCombat[] arr, int count)>();

                for (int i = 0; i < currentShooters.Length; i++) {
                    var slot = currentShooters[i];
                    if (slot == null) { continue; }
                    var shooter = slot.shooter;
                    if (shooter == null) { continue; }

                    var color = shooter.Color;

                    if (!byColor.TryGetValue(color, out var entry)) {
                        entry = (new IShooterCombat[3], 0);
                    }

                    if (shooter.CombatFriend != null) { continue; }
                    if (shooter.RemainingAmmo < 2 && slot.active == true) { continue; }
                    if (!slot.onPosition || slot.forceStop) { continue; }

                    entry.arr[entry.count] = shooter;
                    entry.count++;

                    if (entry.count == 3) {
                        result = entry.arr;
                        return true;
                    }

                    byColor[color] = entry;
                }

                result = null;
                return false;
            }
        }

        private async Task<IShooterCombat> MergeShootersAsync(IShooterCombat[] triple) {
            for (int i = 0; i < triple.Length; i++) {
                IShooterCombat shooter = triple[i];
                var slot = slotsMap[shooter];
                slot.forceStop = true;
            }
            IShooterCombat mergedShooter = await slotsAnimator.MergeShootersAsync(triple);
            int totalAmmo = 0;
            for (int i = 0; i < triple.Length; i++) {
                totalAmmo += triple[i].RemainingAmmo;
            }
            mergedShooter.Merge(totalAmmo);
            return mergedShooter;
        }

        private async Task ActiveShooterUnload(ShooterSlot slot) {
            // Track every cube reference this shooter has received
            var receivedList = new List<ICube>(64);
            var receivedSet = new HashSet<ICube>(ReferenceComparer<ICube>.Instance);

            var shooter = slot.shooter;

            try {
                while (shooter.HasAmmo) {
                    ct.ThrowIfCancellationRequested(); // Unity life-cycle
                    if (slot.forceStop) { return; }

                    if (shooter.OnCooldown) { await Task.Yield(); continue; }
                    if (shooter.HasAmmo && LevelLayoutAPI.TryGetClosestCube(shooter.Position, shooter.Color, out var cube)) {
                        // Detect duplicate reference
                        if (!receivedSet.Add(cube)) {
                            throw new InvalidOperationException(
                                $"Duplicate cube reference returned by TryGetClosestCube. " +
                                $"Shooter={shooter} CubeRefHash={RuntimeHelpers.GetHashCode(cube)} " +
                                $"ReceivedCount={receivedList.Count}"
                            );
                        }

                        receivedList.Add(cube);
                        shooter.Shoot(cube);
                        slot.active = true;
                    }
                    else { slot.active = false; }

                    await Task.Yield();
                }
            }
            catch (OperationCanceledException) { /* Expected – ignore */ }
            catch (Exception ex) {
                Log.Error($"[{nameof(ActiveShooterUnload)}] ERROR\n{ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        private async Task MoveShooterOutOfBounds(int slotIndex, ShooterSlot shooterSlot) {
            ReleaseSlot(shooterSlot.shooter);
            bool curved = false;
            int startIndex = 0;
            int endIndex = middleSlotIndex;
            if (slotIndex > middleSlotIndex) {
                startIndex = slotIndex + 1;
                endIndex = shooterSlots.Length;
            }
            else if (slotIndex < middleSlotIndex) {
                startIndex = 0;
                endIndex = slotIndex;
            }
            for (int i = startIndex; i < endIndex; i++) {
                if (shooterSlots[i] != null) {
                    curved = true;
                    break;
                }
            }
            await shooterSlot.shooter.MoveOut(slotIndex, curved);
        }

        public bool TryReserveFor(IShooterCombat shooter) {
            if (shooter == null) throw new ArgumentNullException($"<color=white>[{nameof(SlotsManager)}] {nameof(shooter)}");

            lock (reservationLock) {
                if (slotsMap.ContainsKey(shooter)) { throw new InvalidOperationException($"<color=white>{nameof(SlotsManager)}</color> Shooter already reserved."); }

                bool reserved = false;
                for (int i = 0; i < shooterSlots.Length; i++) {
                    if (shooterSlots[i] != null) { continue; }
                    reserved = true;
                    var createdSlot = new ShooterSlot(shooter, onPosition: false, forceStop: false, i);
                    shooterSlots[i] = createdSlot;
                    slotsMap[shooter] = createdSlot;
                    break;
                }
                return reserved;
            }
        }

        public bool TryReserveFor(IShooterCombat shooter, IShooterCombat attachedShooter) {
            if (shooter == null) throw new ArgumentNullException($"<color=white>[{nameof(SlotsManager)}] {nameof(shooter)}");
            if (attachedShooter == null) throw new ArgumentNullException($"<color=white>[{nameof(SlotsManager)}] {nameof(attachedShooter)}");

            lock (reservationLock) {
                if (slotsMap.ContainsKey(shooter) || slotsMap.ContainsKey(attachedShooter)) { throw new InvalidOperationException($"<color=white>{nameof(SlotsManager)}</color> Shooter already reserved."); }

                int firstIndex = -1;

                for (int i = 0; i < shooterSlots.Length - 1; i++) {
                    if (shooterSlots[i] != null) { continue; }
                    if (shooterSlots[i + 1] != null) { continue; }

                    firstIndex = i;
                    break;
                }

                if (firstIndex == -1) { return false; }

                var mainSlot = new ShooterSlot(shooter, onPosition: false, forceStop: false, firstIndex);
                var attachedSlot = new ShooterSlot(attachedShooter, onPosition: false, forceStop: false, firstIndex + 1);

                shooterSlots[firstIndex] = mainSlot;
                shooterSlots[firstIndex + 1] = attachedSlot;

                slotsMap[shooter] = mainSlot;
                slotsMap[attachedShooter] = attachedSlot;

                return true;
            }
        }

        private void ReleaseSlot(params IShooterCombat[] shooters) {
            lock (reservationLock) {
                for (int i = 0; i < shooterSlots.Length; i++) {
                    var slot = shooterSlots[i];
                    if (slot == null) { continue; }
                    var slotShooter = slot.shooter;

                    for (int j = 0; j < shooters.Length; j++) {
                        if (slotShooter == shooters[j]) {
                            shooterSlots[i] = null;
                            if (slotsMap.ContainsKey(shooters[j])) {
                                slotsMap.Remove(shooters[j]);
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>Reference identity comparer (not Equals/GetHashCode).</summary>
        private sealed class ReferenceComparer<T> : IEqualityComparer<T> where T : class {
            public static readonly ReferenceComparer<T> Instance = new ReferenceComparer<T>();
            public bool Equals(T x, T y) => ReferenceEquals(x, y);
            public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
        }

        private class ShooterSlot {
            public IShooterCombat shooter;
            public int index;
            public bool? active;
            public bool onPosition;
            public bool forceStop;
            public ShooterSlot(IShooterCombat shooter, bool onPosition, bool forceStop, int index) {
                this.active = null;
                this.onPosition = onPosition;
                this.shooter = shooter;
                this.forceStop = forceStop;
                this.index = index;
            }
        }
    }
}
