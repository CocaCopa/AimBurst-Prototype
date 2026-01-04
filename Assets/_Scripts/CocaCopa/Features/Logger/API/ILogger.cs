namespace CocaCopa.Logger.API {
    public interface ILogger {
        void Log(object message, LogColor color);
        void LogWarning(object message, LogColor color);
        void LogError(object message, LogColor color);
    }
}
