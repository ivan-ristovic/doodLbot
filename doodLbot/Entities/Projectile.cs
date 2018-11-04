using System;

namespace doodLbot.Entities
{
    public class Projectile : Entity
    {
        public Projectile(double x, double y, double angle) : base(x, y, angle)
        {
            this.Xvel = Math.Sin(angle);
            this.Yvel = Math.Cos(angle);
        }
    }
}
