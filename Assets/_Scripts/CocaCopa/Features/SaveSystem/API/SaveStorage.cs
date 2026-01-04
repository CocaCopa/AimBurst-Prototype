using System;

namespace CocaCopa.SaveSystem.API {
    /// <summary>
    /// Public static facade over the active save storage implementation.
    /// 
    /// <para>Consumers:</para>
    /// <para>- Call Initialize(...) once at startup, providing an ISaveStorage implementation.</para>
    /// <para>- Use Save/Load from anywhere after that.</para>
    /// 
    /// Implementations are provided by hidden modules (Runtime, Unity) or by the user.
    /// </summary>
    public static class SaveStorage {
        private static ISaveStorage implementation;
        private static bool initialized;

        /// <summary>
        /// Configure the save system with a concrete implementation.
        /// Must be called exactly once at startup.
        /// </summary>
        internal static void Initialize(ISaveStorage implementation) {
            if (initialized) { throw new InvalidOperationException("[SaveStorage] Already initialized. Avoid re-initializing at runtime."); }

            SaveStorage.implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));
            initialized = true;
        }

        public static void Save<T>(T data, string filePath) {
            EnsureInitialized();
            implementation.Save(data, filePath);
        }

        public static bool Load<T>(string filePath, out T result) {
            EnsureInitialized();
            return implementation.TryLoad(filePath, out result);
        }

        private static void EnsureInitialized() {
            if (!initialized || implementation == null) {
                throw new InvalidOperationException($"[SaveStorage] Not initialized. Make sure you have created a SaveSystemConfig at Resources");
            }
        }
    }
}
