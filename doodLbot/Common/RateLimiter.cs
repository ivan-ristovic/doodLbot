using System;

namespace doodLbot.Common
{
    public class RateLimiter
    {
        private readonly TimeSpan cooldownTimeout = TimeSpan.FromSeconds(0.2);
        private bool cooldown;
        private DateTimeOffset resetTime;


        public RateLimiter(double cooldown)
        {
            this.cooldownTimeout = TimeSpan.FromSeconds(cooldown);
            this.resetTime = DateTimeOffset.UtcNow + this.cooldownTimeout;
            this.cooldown = false;
        }


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
