using System;
using System.Threading.Tasks;
using AimBurst.UI.Contracts.EndScreen;

namespace AimBurst.UI.API.EndScreen {
    public static class EndScreenAPI {
        private static IEndScreen endScreen;

        private static IEndScreen Screen => endScreen ?? throw new NullReferenceException($"<color=white>[{nameof(EndScreenAPI)}]</color> {nameof(IEndScreen)}");

        internal static void Wire(IEndScreen impl) => endScreen = impl;

        public static async Task<bool> Trigger(State state, bool hideNextLvlBtn) => await Screen.Trigger(state, hideNextLvlBtn);
    }
}
