using System;

namespace doodLbot.Common
{
    public class RateLimiter
    {
        private static readonly TimeSpan _cooldownTimeout = TimeSpan.FromSeconds(0.2);

        private bool cooldown;
        private DateTimeOffset resetTime;

        public RateLimiter()
        {
            this.resetTime = DateTimeOffset.UtcNow + _cooldownTimeout;
            this.cooldown = false;
        }

        public bool IsCooldownActive()
        {
            bool success = false;

            var now = DateTimeOffset.UtcNow;
            if (now >= this.resetTime)
            {
                this.cooldown = false;
                this.resetTime = now + _cooldownTimeout;
            }

            if (!this.cooldown)
            {
                this.cooldown = true;
                success = true;
            }

            return !success;
        }
    }
}
