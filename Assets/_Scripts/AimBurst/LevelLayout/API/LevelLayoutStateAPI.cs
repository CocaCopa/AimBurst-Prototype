using System;
using AimBurst.LevelLayout.Contracts;

namespace AimBurst.LevelLayout.API {
    public static class LevelLayoutStateAPI {
        private static ILevelState layout;

        private static ILevelState Layout => layout ?? throw new NullReferenceException($"<color=white>[{nameof(LevelLayoutStateAPI)}]</color> {nameof(ILevelState)}");

        internal static void Wire(ILevelState impl) => layout = impl;

        public static int TotalTargetsCount => Layout.TotalTargetsCount;
        public static int CurrentTargetsCount => Layout.CurrentTargetsCount;
    }
}
