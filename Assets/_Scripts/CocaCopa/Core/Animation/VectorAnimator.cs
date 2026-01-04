namespace CocaCopa.Core.Animation {
    public sealed class VectorAnimator {
        public float EvaluateUnclamped(float deltaTime) => Step(deltaTime, true);
        /// <summary>
        /// Advance the animation and get the current value.
        /// </summary>
        /// <param name="deltaTime">how much time passed since last step</param>
        /// <returns>The interpolated value between from..to using easing.</returns>
        public float Evaluate(float deltaTime) => Step(deltaTime, false);

        private float Step(float deltaTime, bool unclamped) {
            throw new System.NotImplementedException();
        }
    }
}