using System;
using AimBurst.Core.Contracts;

namespace AimBurst.Input {
    public static class GameInputAPI {
        private static IInputPublisher publisher;

        private static IInputPublisher Publisher {
            get {
                return publisher ?? throw new InvalidOperationException(
                    "GameInput not initialized. Ensure GameInputAdapter exists in the scene."
                );
            }
        }

        internal static void OverrideImplementation(IInputPublisher impl) {
            // if (publisher != null) { throw new InvalidOperationException("GameInput already wired. Ensure only one GameInputAdapter exists."); }
            publisher = impl ?? throw new ArgumentNullException($"<color=white>[{nameof(GameInputAPI)}]</color> {nameof(IInputPublisher)}");
        }

        public static event Action<IClickable> OnLeftClick {
            add => Publisher.OnLeftClick += value;
            remove => Publisher.OnLeftClick -= value;
        }
    }
}
