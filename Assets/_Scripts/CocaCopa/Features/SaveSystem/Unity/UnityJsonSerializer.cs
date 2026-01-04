using CocaCopa.SaveSystem.SPI;
using UnityEngine;

namespace CocaCopa.SaveSystem.Unity {
    /// <summary>
    /// IJsonSerializer implementation using UnityEngine.JsonUtility.
    /// Lives in the Unity layer so Runtime stays engine-agnostic.
    /// </summary>
    internal sealed class UnityJsonSerializer : IJsonSerializer {
        private readonly bool _prettyPrint;

        public UnityJsonSerializer(bool prettyPrint = true) {
            _prettyPrint = prettyPrint;
        }

        public string ToJson<T>(T value) {
            return JsonUtility.ToJson(value, _prettyPrint);
        }

        public T FromJson<T>(string json) {
            return JsonUtility.FromJson<T>(json);
        }
    }
}
