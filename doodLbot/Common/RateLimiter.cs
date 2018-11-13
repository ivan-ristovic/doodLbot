using System;

namespace doodLbot.Common
{
    /// <summary>
    /// Used to prevent an action from being executed more than once in a given time span.
    /// </summary>
    public class RateLimiter
    {
        private readonly TimeSpan cooldownTimeout = TimeSpan.FromSeconds(0.2);
        private bool cooldown;
        private DateTimeOffset resetTime;


        /// <summary>
        /// Constructs a new RateLimiter object that resets the cooldown after givens time span.
        /// </summary>
        /// <param name="cooldown">Time span after the cooldown will be reset.</param>
        public RateLimiter(TimeSpan timespan)
        {
            this.cooldownTimeout = timespan;
            this.resetTime = DateTimeOffset.UtcNow + this.cooldownTimeout;
            this.cooldown = false;
        }

        /// <summary>
        /// Constructs a new RateLimiter object that resets the cooldown after given time span in seconds.
        /// </summary>
        /// <param name="seconds">Amount of seconds after the cooldown will be reset.</param>
        public RateLimiter(double seconds)
            : this(TimeSpan.FromSeconds(seconds))
        {

        }


        /// <summary>
        /// Checks if the cooldown is active for this instance.
        /// </summary>
        /// <returns></returns>
        public bool IsCooldownActive()
        {
            bool success = false;

            var now = DateTimeOffset.UtcNow;
            if (now >= this.resetTime) {
                this.cooldown = false;
                this.resetTime = now + this.cooldownTimeout;
            }

            if (!this.cooldown) {
                this.cooldown = true;
                success = true;
            }

            return !success;
        }
    }
}
