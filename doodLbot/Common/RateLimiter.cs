using System;

namespace doodLbot.Common
{
    public class RateLimiter
    {
        private static readonly TimeSpan _cooldownTimeout = TimeSpan.FromSeconds(1);

        private bool cooldown;
        private DateTimeOffset resetTime;
        private readonly object cooldownLock;

        public RateLimiter()
        {
            this.resetTime = DateTimeOffset.UtcNow + _cooldownTimeout;
            this.cooldownLock = new object();
            this.cooldown = false;
        }

        public bool IsCooldownActive()
        {
            bool success = false;

            lock (this.cooldownLock)
            {
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
            }

            return !success;
        }
    }
}
