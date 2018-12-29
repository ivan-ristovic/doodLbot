using System;
using Serilog;

namespace doodLbot.Entities
{
    /// <summary>
    /// Represents a projectile that can be fired by an Entity.
    /// </summary>
    public class Projectile : Entity
    {
        private readonly double angleRandom = Math.PI * 2 / 30;
        private readonly double speedRandom = .1;
        private static Random random = new Random();

        /// <summary>
        /// Constructs a new projectile.
        /// </summary>
        /// <param name="x">Origin X coodrinate.</param>
        /// <param name="y">Origin Y coodrinate.</param>
        /// <param name="angle">Projectile angle (in radians).</param>
        /// <param name="speed">Projectile speed.</param>
        /// <param name="damage">Projectile damage</param>
        public Projectile(double x, double y, double angle, double speed, double damage) : base(x: x, y: y, speed: speed, damage: damage, rotation: angle)
        {
            angle += random.NextDouble() * angleRandom - angleRandom / 2;
            speed += random.NextDouble() * speedRandom - speedRandom / 2;
            Log.Debug(random.NextDouble().ToString());
            this.Xvel = Math.Cos(angle) * speed;
            this.Yvel = Math.Sin(angle) * speed;
        }
    }
}
