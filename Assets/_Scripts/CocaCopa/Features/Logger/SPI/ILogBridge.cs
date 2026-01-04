namespace CocaCopa.Logger.SPI {
    internal interface ILogBridge {
        void LogInfo(object message);
        void LogWarning(object message);
        void LogError(object message);
    }
}
