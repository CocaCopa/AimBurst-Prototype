using System;
using AimBurst.Core.Contracts;
using AimBurst.LevelLayout.Contracts;
using CocaCopa.Primitives;

namespace AimBurst.LevelLayout.API {
    public static class LevelLayoutAPI {
        private static ILevelLayout layout;

        private static ILevelLayout Layout => layout ?? throw new NullReferenceException($"<color=white>[{nameof(LevelLayoutAPI)}]</color> {nameof(ILevelLayout)}");

        internal static void Wire(ILevelLayout impl) => layout = impl;

        public static bool TryGetClosestCube(C_Vector3 yourPos, CubeColor targetColor, out ICube cube) => Layout.TryGetClosestCube(yourPos, targetColor, out cube);
        public static void ReportHit(ICube hitTarget, ICube intendedTarget, C_Vector3 yourMoveDir) => Layout.ReportHit(hitTarget, intendedTarget, yourMoveDir);
    }
}
