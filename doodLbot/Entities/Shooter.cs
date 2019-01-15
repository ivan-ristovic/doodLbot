using doodLbot.Common;
using doodLbot.Logic;
using System;

namespace doodLbot.Entities
{
    /// <summary>
    /// Represents an Enemy with extra HP that moves towards a hero and tries to collide with him.
    /// </summary>
    public class Shooter : Enemy
    {
        private readonly RateLimiter shootingLimiter;


        public Shooter() : base(max_hp: 150, hp: 150, damage: 10, speed: Design.EnemySpeed)
        {
            this.shootingLimiter = new RateLimiter(TimeSpan.FromSeconds(3));
        }


        /// <summary>
        /// Try to fire a new projectile.
        /// </summary>
        /// <returns>The fired projectile or null if the firing cooldown is active.</returns>
        public Projectile TryFire()
        {
            if (!this.shootingLimiter.IsCooldownActive())
            {
                return new Projectile(this.Xpos, this.Ypos, Math.Atan2(this.Yvel, this.Xvel), Design.ProjectileSpeed, this.Damage);
            }
            else
                return null;
        }

    }
}
