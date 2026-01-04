using CocaCopa.Primitives;
using UnityEngine;

namespace CocaCopa.Unity.Numerics {
    public static class Vector3UnityConversions {
        public static Vector3 ToUnity(this C_Vector3 v) => new(v.x, v.y, v.z);
        public static C_Vector3 ToCore(this Vector3 v) => new(v.x, v.y, v.z);
    }
}
