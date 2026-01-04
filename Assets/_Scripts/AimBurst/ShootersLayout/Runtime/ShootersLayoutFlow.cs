using System.Diagnostics;
using System.Threading.Tasks;
using AimBurst.ShootersLayout.Runtime.Abstractions;

namespace AimBurst.ShootersLayout.Runtime {
    internal sealed class ShootersLayoutFlow : IShootersLayoutFlow {
        private readonly ISlotManager slotManager;
        private readonly int clickGateMs;
        private readonly object clickLock = new object();

        private bool canAcceptClicks;

        internal ShootersLayoutFlow(int clickGateMs, SlotsManager slotManager) {
            this.slotManager = slotManager;
            this.clickGateMs = clickGateMs;
        }

        internal void Init() {
            canAcceptClicks = true;
        }

        public void HandleClick(ILaneController clickedName) {
            lock (clickLock) {
                HandleClickAsync(clickedName);
            }
        }

        public async void HandleClickAsync(ILaneController clickedLane) {
            if (!canAcceptClicks) return;

            canAcceptClicks = false;
            try {
                var front = clickedLane.Peek();
                if (front == null) { return; }
                if (front is not IShooterCombat frontCombat) { return; }

                if (front.CombatFriend != null) {
                    await TryHandlePairAsync(clickedLane, frontCombat, front.CombatFriend);
                    return;
                }
                await TryHandleSingleAsync(clickedLane, frontCombat);
            }
            finally {
                var sw = Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < clickGateMs) {
                    await Task.Yield();
                }
                canAcceptClicks = true;
            }
        }

        private async Task TryHandleSingleAsync(ILaneController clickedLane, IShooterCombat frontShooter) {
            if (!slotManager.TryReserveFor(frontShooter)) { return; }

            clickedLane.Dequeue();
            _ = slotManager.AcceptShooterAsync(frontShooter);
            await clickedLane.AdvanceLaneAsync();
        }

        private async Task TryHandlePairAsync(ILaneController clickedLane, IShooterCombat frontShooter, IShooterCombat combatFriend) {
            if (!IsPairAllowed(frontShooter, combatFriend)) { return; }
            if (!slotManager.TryReserveFor(frontShooter, combatFriend)) { return; }


            if (frontShooter.LaneIndex == combatFriend.LaneIndex) {
                clickedLane.Dequeue();
                _ = slotManager.AcceptShooterAsync(frontShooter);
                await clickedLane.AdvanceLaneAsync();
                clickedLane.Dequeue();
                _ = slotManager.AcceptShooterAsync(combatFriend);
                await clickedLane.AdvanceLaneAsync();
            }
            else {
                clickedLane.Dequeue();
                _ = slotManager.AcceptShooterAsync(frontShooter);
                ShooterLaneController.Dequeue(combatFriend.LaneIndex);
                _ = slotManager.AcceptShooterAsync(combatFriend);

                await Task.WhenAll(
                    clickedLane.AdvanceLaneAsync(),
                    ShooterLaneController.AdvanceLane(combatFriend.LaneIndex)
                );
            }
        }

        private static bool IsPairAllowed(IShooterCombat front, IShooterCombat combatFriend) {
            bool differentLane = front.LaneIndex != combatFriend.LaneIndex;
            bool friendNotFront = combatFriend.Positioning != ShooterPositioning.Front;

            if (differentLane && friendNotFront) { return false; }
            else { return true; }
        }
    }
}
