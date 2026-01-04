using UnityEngine;
using UnityEngine.Pool;

namespace AimBurst.ShootersLayout.Unity.Pool {
    internal class BulletPool : MonoBehaviour {
        [SerializeField] private GameObject bulletPrefab;

        private ObjectPool<GameObject> bulletPool;

        private void Awake() {
            bulletPool = new ObjectPool<GameObject>(
                createFunc: CreateFunc,
                actionOnGet: b => ActionOnGet(),
                actionOnRelease: b => ActionOnRelease(),
                actionOnDestroy: b => ActionOnDestroy(),
                collectionCheck: true,
                defaultCapacity: 10,
                maxSize: 10
            );
        }

        private GameObject CreateFunc() {
            return null;
        }

        private void ActionOnGet() {

        }

        private void ActionOnRelease() {

        }

        private void ActionOnDestroy() {

        }
    }
}
