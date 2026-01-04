namespace CocaCopa.Logger.API {
    public static class Log {
        private static ILogger Logger { get; set; }
        internal static void WireLogger(ILogger logger) => Logger ??= logger;
        public static void Info(object message, LogColor color = LogColor.Default) => Logger.Log(message, color);
        public static void Warning(object message, LogColor color = LogColor.Default) => Logger.LogWarning(message, color);
        public static void Error(object message, LogColor color = LogColor.Default) => Logger.LogError(message, color);
    }
}
