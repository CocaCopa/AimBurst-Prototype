using System.Collections.Generic;
using CocaCopa.Logger.Runtime.Internal;
using UnityEngine;

namespace CocaCopa.Logger.Unity {
    [CreateAssetMenu(fileName = "LoggerSettings", menuName = "CocaCopa/Logger Settings", order = 10)]
    internal class LoggerSettings : ScriptableObject {
        public LogFiltering filter = LogFiltering.Messages | LogFiltering.Warnings | LogFiltering.Errors;
        public KeywordMode mode = KeywordMode.Include;
        public List<string> keywords = new List<string>();
        public static LoggerSettings Default => new LoggerSettings(
            LogFiltering.Messages | LogFiltering.Warnings | LogFiltering.Errors,
            KeywordMode.Include,
            new List<string>()
        );

        private LoggerSettings(LogFiltering filter, KeywordMode mode, List<string> keywords) {
            this.filter = filter;
            this.mode = mode;
            this.keywords = keywords;
        }
    }
}
