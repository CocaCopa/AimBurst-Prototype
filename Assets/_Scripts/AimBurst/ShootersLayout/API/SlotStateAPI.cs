using System;
using System.Threading.Tasks;
using AimBurst.ShootersLayout.Contracts;

namespace AimBurst.ShootersLayout.API {
    public static class SlotStateAPI {
        private static ISlotsState slotsState;

        private static ISlotsState Slots => slotsState ?? throw new NullReferenceException($"<color=white>[{nameof(SlotStateAPI)}]</color> {nameof(ISlotsState)}");

        internal static void Wire(ISlotsState impl) => slotsState = impl;

        public static int TotalSlots => Slots.TotalSlots;
        public static async Task<bool> ObserveSlotsInactive() => await Slots.ObserveSlotsInactive();
    }
}
