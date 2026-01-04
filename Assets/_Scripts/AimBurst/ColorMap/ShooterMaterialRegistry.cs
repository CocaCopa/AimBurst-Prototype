using System;
using System.Collections.Generic;
using AimBurst.Core.Contracts;
using UnityEngine;

namespace AimBurst.ColorMap {
    [CreateAssetMenu(fileName = "MaterialMap", menuName = "Game/Material Registry")]
    public sealed class ShooterMaterialRegistry : ScriptableObject {
        [Serializable]
        private struct Entry {
            public CubeColor color;
            public Material material;
        }

        [SerializeField] private Entry[] entries;

        private Dictionary<CubeColor, Material> map;

        public void Init() {
            map = new Dictionary<CubeColor, Material>(entries.Length);
            foreach (var e in entries)
                map[e.color] = e.material;
        }

        public Material Get(CubeColor color) {
            if (!map.TryGetValue(color, out var mat) || mat == null)
                throw new KeyNotFoundException($"Missing material for {color}");
            return mat;
        }
    }
}
