using System;

namespace doodLbot.Entities
{
    /// <summary>
    /// Represents a projectile that can be fired by an Entity.
    /// </summary>
    public class Projectile : Entity
    {
        /// <summary>
        /// Constructs a new projectile.
        /// </summary>
        /// <param name="x">Origin X coodrinate.</param>
        /// <param name="y">Origin Y coodrinate.</param>
        /// <param name="angle">Projectile angle (in radians).</param>
        /// <param name="speed">Projectile speed.</param>
        /// <param name="damage">Projectile damage</param>
        public Projectile(double x, double y, double angle, double speed, double damage) : base(x, y)
        {
            this.Damage = damage;
            this.Xvel = Math.Cos(angle) * speed;
            this.Yvel = Math.Sin(angle) * speed;
        }
    }
}
