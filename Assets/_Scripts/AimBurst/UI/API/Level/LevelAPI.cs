using System;
using AimBurst.UI.Contracts.Level;

namespace AimBurst.UI.API.Level {
    public static class LevelAPI {
        private static ILevel level;

        private static ILevel LevelProgression => level ?? throw new NullReferenceException($"<color=white>[{nameof(LevelAPI)}]</color> {nameof(ILevel)}");

        internal static void Wire(ILevel impl) => level = impl;

        public static void SetLevelIndex(int index) => LevelProgression.SetLevelIndex(index);
        public static void SetProgress(float progressPercentage) => LevelProgression.SetProgress(progressPercentage);
    }
}
