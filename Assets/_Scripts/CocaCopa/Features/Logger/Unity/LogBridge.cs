using CocaCopa.Logger.SPI;
using UnityEngine;

namespace CocaCopa.Logger.Unity {
    public class LogBridge : ILogBridge {
        public void LogInfo(object message) => Debug.Log(message);
        public void LogWarning(object message) => Debug.LogWarning(message);
        public void LogError(object message) => Debug.LogError(message);
    }
}
