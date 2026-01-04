using System;
using AimBurst.Core.Contracts;
using AimBurst.ShootersLayout.Contracts;

namespace AimBurst.ShootersLayout.API {
    public static class LaneStateAPI {
        private static ILaneState slotsState;

        private static ILaneState Slots => slotsState ?? throw new NullReferenceException($"<color=white>[{nameof(LaneStateAPI)}]</color> {nameof(ILaneState)}");

        internal static void Wire(ILaneState impl) => slotsState = impl;

        public static int CurrentShootersCount => Slots.CurrentShootersCount;
        public static CubeColor? NextShooterColor => Slots.NextShooterColor;
    }
}
