namespace CocaCopa.Core.Animation {
    /// <summary>
    /// Provides smooth value interpolation between two scalar values using a configurable easing function.
    /// </summary>
    public sealed class ValueAnimator {
        private readonly IEasing easing;
        private readonly float from;
        private readonly float to;
        private readonly float defaultSpeed;
        private float speed;
        private float t;
        private bool paused;

        /// <summary>
        /// Indicates whether the animation has reached its end (progress ≥ 1.0).
        /// </summary>
        public bool IsComplete => t >= 1f;

        /// <summary>
        /// The current normalized progress of the animation (0.0–1.0).
        /// </summary>
        public float Progress => t;

        /// <summary>
        /// Private constructor. Use <see cref="BySpeed"/> or <see cref="ByDuration"/> to create instances.
        /// </summary>
        private ValueAnimator(float from, float to, float speed, IEasing easing) {
            this.from = from;
            this.to = to;
            this.speed = defaultSpeed = speed; // normalized progress per second
            this.easing = easing;
            t = 0f;
            paused = false;
        }

        /// <summary>
        /// Creates a new <see cref="ValueAnimator"/> configured to advance at a constant speed.
        /// </summary>
        /// <param name="from">The starting value of the animation.</param>
        /// <param name="to">The target value of the animation.</param>
        /// <param name="speed">
        /// The rate of progress increase per second, where 1.0 equals one full animation per second.
        /// For example, <c>0.5f</c> completes the animation in 2 seconds.
        /// </param>
        /// <param name="easing">The easing function used to shape the interpolation curve.</param>
        /// <returns>A new <see cref="ValueAnimator"/> instance configured with the specified speed.</returns>
        public static ValueAnimator BySpeed(float from, float to, float speed, IEasing easing) {
            return new ValueAnimator(from, to, speed, easing);
        }

        /// <summary>
        /// Creates a new <see cref="ValueAnimator"/> configured to complete over a specific duration,
        /// instead of using a manual speed value.
        /// </summary>
        /// <param name="from">The starting value of the animation.</param>
        /// <param name="to">The target value of the animation.</param>
        /// <param name="durationSeconds">The total duration, in seconds, the animation should take to reach completion.</param>
        /// <param name="easing">The easing function used to shape the animation curve.</param>
        /// <returns>
        /// A new <see cref="ValueAnimator"/> instance whose speed is automatically calculated so that
        /// progress reaches 1.0 exactly after <paramref name="durationSeconds"/> seconds.
        /// </returns>
        public static ValueAnimator ByDuration(float from, float to, float durationSeconds, IEasing easing) {
            float spd = durationSeconds <= 0f ? 1f : 1f / durationSeconds;
            return new ValueAnimator(from, to, spd, easing);
        }

        public void OverrideSpeed(float newSpeed) => speed = newSpeed;
        public void ResetSpeedToDefault() => speed = defaultSpeed;

        public void ResetAnimator() { t = 0f; paused = false; }
        public void SetProgress(float normalized) => t = MathUtils.Clamp01(normalized);

        public void Pause() => paused = true;
        public void Resume() => paused = false;

        /// <summary>
        /// Advances the animation state by the given delta time and returns the current interpolated value.
        /// The internal progress <c>t</c> is clamped to the [0..1] range, meaning the animation stops once it reaches the end.
        /// </summary>
        /// <param name="deltaTime">The elapsed time since the last update, typically <c>Time.deltaTime</c>.</param>
        /// <returns>
        /// The interpolated value between <c>from</c> and <c>to</c> according to the easing function,
        /// with progress clamped between 0 and 1.
        /// </returns>
        public float Evaluate(float deltaTime) => Step(deltaTime, clamp: true);

        /// <summary>
        /// Advances the animation state by the given delta time and returns the current interpolated value.
        /// Unlike <see cref="Evaluate"/>, the internal progress <c>t</c> is not clamped,
        /// allowing the animation to overshoot its target if time continues to advance.
        /// </summary>
        /// <param name="deltaTime">The elapsed time since the last update, typically <c>Time.deltaTime</c>.</param>
        /// <returns>
        /// The interpolated value between <c>from</c> and <c>to</c> according to the easing function,
        /// with progress unbounded (can exceed 0..1 range).
        /// </returns>
        public float EvaluateUnclamped(float deltaTime) => Step(deltaTime, clamp: false);

        private float Step(float deltaTime, bool clamp) {
            if (!paused && deltaTime > 0f) {
                t += speed * deltaTime;
                if (clamp) t = MathUtils.Clamp01(t);
            }

            float curvedT = easing.Evaluate(clamp ? MathUtils.Clamp01(t) : t);
            return clamp
                ? MathUtils.Lerp(from, to, curvedT)
                : MathUtils.LerpUnclamped(from, to, curvedT);
        }
    }
}
