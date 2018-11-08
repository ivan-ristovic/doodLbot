using Newtonsoft.Json;

using System;

namespace doodLbot.Entities
{
    public class Entity
    {
        [JsonProperty("x")]
        public double Xpos { get; protected set; }
        [JsonProperty("y")]
        public double Ypos { get; protected set; }
        [JsonProperty("rotation")]
        public double Rotation { get; set; }
        [JsonProperty("vy")]
        public double Xvel { get; set; }
        [JsonProperty("vx")]
        public double Yvel { get; set; }
        [JsonProperty("hp")]
        public double Hp { get; protected set; }
        [JsonProperty("damage")]
        public double Damage { get; protected set; }


        public Entity()
        {
            this.Hp = 100;
            this.Damage = 1;
        }

        public Entity(double x, double y, double rotation = 0) : this()
        {
            this.Xpos = x;
            this.Ypos = y;
            this.Rotation = rotation;
        }

        public void Move()
        {
            this.Xpos += this.Xvel;
            this.Ypos += this.Yvel;
        }

        public void VelocityTowards(Entity goal, double withSpeed)
        {
            double xvel = goal.Xpos - this.Xpos;
            double yvel = goal.Ypos - this.Ypos;
            double norm = Math.Sqrt( xvel * xvel + yvel * yvel );
            // todo: handle case when norm is 0
            this.Xvel = xvel / norm * withSpeed;
            this.Yvel = yvel / norm * withSpeed;
        }

        public virtual void DecreaseHelthPoints(double value)
        {
            // TODO: make Hp atomic
            double newHp = this.Hp - value;
            this.Hp = newHp > 0 ? newHp : 0;
        }
    }
}
