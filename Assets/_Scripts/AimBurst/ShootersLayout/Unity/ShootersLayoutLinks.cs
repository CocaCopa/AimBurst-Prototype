using System;
using AimBurst.ShootersLayout.Unity.Actor;
using UnityEngine;

namespace AimBurst.ShootersLayout.Unity {
    internal sealed class ShootersLayoutLinks : MonoBehaviour {
        [SerializeField] private ShootersLayoutSpawner[] spawners;
        [SerializeField] private LinkInfo[] linkInfo;

        private void OnValidate() {
            for (int i = 0; i < linkInfo.Length; i++) {
                linkInfo[i].name = $"Link {i + 1}";
            }
        }

        private void Start() {
            for (int i = 0; i < linkInfo.Length; i++) {
                var info = linkInfo[i];
                var author = spawners[info.author.spawner].SpawnedShooters<Shooter>()[info.author.shooter];
                var attached = spawners[info.attached.spawner].SpawnedShooters<Shooter>()[info.attached.shooter];
                author.SetCombatFriend(attached);
                attached.SetCombatFriend(author);
            }
        }

        [Serializable]
        private struct LinkInfo {
            [HideInInspector] public string name;
            public LinkId author;
            public LinkId attached;
        }

        [Serializable]
        private struct LinkId {
            public int spawner;
            public int shooter;
        }
    }
}
