using AimBurst.LevelLayout.Contracts;
using CocaCopa.Primitives;

namespace AimBurst.LevelLayout.Runtime.Abstractions {
    internal interface ICubeInternal : ICube {
        void PlayDodgeBullet(C_Vector3 dodgeToward);
        void PlayDestroy();
    }
}
