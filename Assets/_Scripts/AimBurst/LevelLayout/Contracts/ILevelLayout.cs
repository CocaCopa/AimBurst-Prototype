using AimBurst.Core.Contracts;
using CocaCopa.Primitives;

namespace AimBurst.LevelLayout.Contracts {
    public interface ILevelLayout {
        bool TryGetClosestCube(C_Vector3 yourPos, CubeColor targetColor, out ICube cube);
        void ReportHit(ICube hitTarget, ICube intendedTarget, C_Vector3 yourMoveDir);
    }
}
