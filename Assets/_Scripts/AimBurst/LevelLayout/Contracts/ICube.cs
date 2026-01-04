using AimBurst.Core.Contracts;
using CocaCopa.Primitives;

namespace AimBurst.LevelLayout.Contracts {
    public interface ICube {
        C_Vector3 Position { get; }
        CubeColor Color { get; }
        int InstanceID { get; }
        void Kill();
        void DodgeBullet(C_Vector3 dodgeToward);
    }
}
