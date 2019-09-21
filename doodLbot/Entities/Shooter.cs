using System;
using doodLbot.Common;
using doodLbot.Logic;

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
            shootingLimiter = new RateLimiter(TimeSpan.FromSeconds(3));
        }


        /// <summary>
        /// Try to fire a new projectile.
        /// </summary>
        /// <returns>The fired projectile or null if the firing cooldown is active.</returns>
        public Projectile TryFire()
        {
            if (!shootingLimiter.IsCooldownActive())
            {
                return new Projectile(Xpos, Ypos, Math.Atan2(Yvel, Xvel), Design.ProjectileSpeed * Design.Delta, Damage);
            }
            else
                return null;
        }

    }
}
