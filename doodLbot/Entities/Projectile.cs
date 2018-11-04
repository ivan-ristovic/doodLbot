using System;

namespace doodLbot.Entities
{
    public class Projectile : Entity
    {
        public Projectile(double x, double y, double angle) : base(x, y)
        {
            this.Xvel = Math.Cos(angle);
            this.Yvel = Math.Sin(angle);
        }
    }
}
