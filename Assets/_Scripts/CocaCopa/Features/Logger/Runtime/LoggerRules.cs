#if !UNITY_EDITOR
using System.Text.RegularExpressions;
#endif
using System;
using System.Collections.Generic;
using CocaCopa.Logger.API;
using CocaCopa.Logger.Runtime.Internal;
using CocaCopa.Logger.SPI;

namespace CocaCopa.Logger.Runtime {
    internal class LoggerRules : ILogger {
        // Strip color tags in player builds to avoid log junk
#if !UNITY_EDITOR
        static readonly Regex Strip = new Regex(@"<\/?color.*?>", RegexOptions.Compiled);
#endif
        private readonly ILogBridge logBridge;
        private readonly LogFiltering filter = LogFiltering.Messages | LogFiltering.Warnings | LogFiltering.Errors;
        private readonly KeywordMode mode = KeywordMode.Include;
        private readonly List<string> keywords;

        public LoggerRules(ILogBridge logBridge, LogFiltering filter, KeywordMode mode, List<string> keywords = null) {
            this.logBridge = logBridge;
            this.filter = filter;
            this.mode = mode;
            this.keywords = keywords ?? new List<string>(0);
        }

        public void Log(object message, LogColor color) {
            Print(LogType.Info, message, color);
        }

        public void LogWarning(object message, LogColor color) {
            Print(LogType.Warning, message, color);
        }

        public void LogError(object message, LogColor color) {
            Print(LogType.Error, message, color);
        }

        private void Print(LogType t, object message, LogColor color) {
            if (!TypeAllowed(t) || !KeywordAllowed(message))
                return;

            var text = Colorize(message, color);

            switch (t) {
                case LogType.Info: logBridge.LogInfo(text); break;
                case LogType.Warning: logBridge.LogWarning(text); break;
                case LogType.Error: logBridge.LogError(text); break;
            }
        }

        private bool TypeAllowed(LogType t) => t switch {
            LogType.Info => (filter & LogFiltering.Messages) != 0,
            LogType.Warning => (filter & LogFiltering.Warnings) != 0,
            LogType.Error => (filter & LogFiltering.Errors) != 0,
            _ => false
        };

        private bool KeywordAllowed(object msgObj) {
            if (keywords == null || keywords.Count == 0)
                return true;

            var msg = msgObj?.ToString() ?? string.Empty;
            if (msg.Length == 0) return true;

            bool any = false;
            for (int i = 0; i < keywords.Count; i++) {
                var k = keywords[i];
                if (string.IsNullOrWhiteSpace(k)) continue;
                if (msg.IndexOf(k.Trim(), StringComparison.OrdinalIgnoreCase) >= 0) {
                    any = true; break;
                }
            }
            return mode == KeywordMode.Include ? any : !any;
        }

        static string Colorize(object m, LogColor c) {
            var text = m?.ToString() ?? "null";
#if UNITY_EDITOR
            string open = c switch {
                LogColor.White => "<color=white>",
                LogColor.Red => "<color=red>",
                LogColor.Green => "<color=green>",
                LogColor.Yellow => "<color=yellow>",
                LogColor.Blue => "<color=cyan>",
                LogColor.Magenta => "<color=#FF00FF>",
                LogColor.Orange => "<color=orange>",
                _ => ""
            };
            return open.Length > 0 ? open + text + "</color>" : text;
#else
            return Strip.Replace(text, "");
#endif
        }
    }
}
