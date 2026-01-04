using System;
using System.Threading;
using System.Threading.Tasks;
using AimBurst.LevelLayout.API;
using AimBurst.SceneControls.API;
using AimBurst.ShootersLayout.API;
using AimBurst.UI.API.EndScreen;
using AimBurst.UI.API.Level;
using AimBurst.UI.Contracts.EndScreen;
using CocaCopa.Logger.API;

namespace AimBurst.GameFlow.Runtime {
    internal sealed class GameFlowCoordinator {
        private readonly CancellationToken ct;

        internal GameFlowCoordinator(CancellationToken ct) {
            this.ct = ct;
        }

        internal void Init() {
            _ = GameObserver();
        }

        private async Task GameObserver() {
            try {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                CancellationToken token = linkedCts.Token;

                LevelAPI.SetLevelIndex(SceneControllerAPI.CurrentIndex);
                LevelProgressionAsync(ct).Forget();

                Task<State> winTask = ObserveWinAsync(token);
                Task<State> loseTask = ObserveLoseAsync(token);

                Task<State> finished = await Task.WhenAny(winTask, loseTask);
                State result = await finished;

                linkedCts.Cancel();

                bool hideNextLvlBtn = !SceneControllerAPI.NextIndex.HasValue;
                bool nextLevelClicked = await EndScreenAPI.Trigger(result, hideNextLvlBtn);
                if (nextLevelClicked) {
                    if (result == State.Win) { SceneControllerAPI.LoadNextLevel(); }
                    else { SceneControllerAPI.RestartCurrentLevel(); }
                }
                else { throw new Exception($"<color=white>[{nameof(GameFlowCoordinator)}]</color> End screen flow did not complete as expected"); }

            }
            catch (OperationCanceledException) { /* Expected - ignore */ }
            catch (Exception ex) {
                Log.Error(ex);
                throw;
            }
        }

        private async Task LevelProgressionAsync(CancellationToken token) {
            int lastCurrTargets = int.MinValue;
            int lastTotalTargets = int.MinValue;
            float lastProgress = -1f;

            while (true) {
                token.ThrowIfCancellationRequested();

                int totalTargets = LevelLayoutStateAPI.TotalTargetsCount;
                int currTargets = LevelLayoutStateAPI.CurrentTargetsCount;

                if (currTargets != lastCurrTargets || totalTargets != lastTotalTargets) {
                    lastCurrTargets = currTargets;
                    lastTotalTargets = totalTargets;

                    float progress = CalculateProgressSafe(currTargets, totalTargets);

                    if (!NearlyEqual(progress, lastProgress)) {
                        lastProgress = progress;
                        LevelAPI.SetProgress(progress);
                    }
                }

                await Task.Yield();
            }
        }

        private static float CalculateProgressSafe(int currTargets, int totalTargets) {
            if (totalTargets <= 0) return 1f;
            if (currTargets <= 0) return 1f;
            if (currTargets >= totalTargets) return 0f;

            float remaining = (float)currTargets / totalTargets;
            float progress = 1f - remaining;
            return Clamp01(progress);
        }

        private static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);

        private static bool NearlyEqual(float a, float b, float eps = 0.0001f) {
            float d = a - b;
            if (d < 0f) d = -d;
            return d < eps;
        }

        private async Task<State> ObserveWinAsync(CancellationToken token) {
            while (true) {
                token.ThrowIfCancellationRequested();

                if (LevelLayoutStateAPI.CurrentTargetsCount == 0) {
                    return State.Win;
                }
                await Task.Yield();
            }
        }

        private async Task<State> ObserveLoseAsync(CancellationToken token) {
            while (true) {
                token.ThrowIfCancellationRequested();

                bool allSlotsInactive = await SlotStateAPI.ObserveSlotsInactive();
                if (allSlotsInactive) {
                    return State.Lose;
                }

                await Task.Yield();
            }
        }
    }
}
