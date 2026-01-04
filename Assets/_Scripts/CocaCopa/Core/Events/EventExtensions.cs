using System;

namespace CocaCopa.Core {
    public static class EventExtensions {
        public static void SafeInvoke<T>(this Action<T> evt, T arg) => SafeInvoke(evt, arg, "");
        public static void SafeInvoke<T>(this Action<T> evt, T arg, string evtName) {
            if (evt == null) return;

            foreach (var d in evt.GetInvocationList()) {
                var handler = (Action<T>)d;
                try {
                    handler(arg);
                }
                catch (Exception ex) {
                    var targetType = handler.Target?.GetType().Name ?? "<static>";
                    var methodName = handler.Method.Name;
                    throw new Exception($"[{evtName}] listener threw in {targetType}.{methodName}: {ex}");
                }
            }
        }

        public static void SafeInvoke(this Action evt, string evtName) {
            if (evt == null) return;
            foreach (var d in evt.GetInvocationList()) {
                var handler = (Action)d;
                try { handler(); }
                catch (Exception ex) {
                    var targetType = handler.Target?.GetType().Name ?? "<static>";
                    var methodName = handler.Method.Name;
                    throw new Exception($"[{evtName}] listener threw in {targetType}.{methodName}: {ex}");
                }
            }
        }

        public static void SafeInvoke(Delegate evt, string evtName, params object[] args) {
            if (evt == null) return;
            var list = evt.GetInvocationList();        // alloc
            for (int i = 0; i < list.Length; i++) {
                var h = list[i];
                try { h.DynamicInvoke(args); }         // reflection + boxing + allocs
                catch (Exception ex) {
                    var tgt = h.Target?.GetType().Name ?? "<static>";
                    var method = h.Method.Name;
                    throw new Exception($"[{evtName}] {tgt}.{method} threw: {ex}");
                }
            }
        }
    }
}
