using System;

namespace doodLbot.Entities
{
    public class Projectile : Entity
    {
        public double strength { get; private set; }

        public Projectile(double x, double y, double angle, double strenght = 50) : base(x, y)
        {
            this.strength = strenght;
            this.Xvel = Math.Cos(angle);
            this.Yvel = Math.Sin(angle);
        }
    }
}
