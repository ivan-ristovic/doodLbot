using System;

namespace doodLbot.Entities
{
    public class Projectile : Entity
    {
        public Projectile(double x, double y, double angle, double speed, double damage) : base(x, y)
        {
            this.Damage = damage;
            this.Xvel = Math.Cos(angle) * speed;
            this.Yvel = Math.Sin(angle) * speed;
        }
    }
}
