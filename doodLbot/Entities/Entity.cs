using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace doodLbot.Entities
{
    public class Entity
    {
        [JsonProperty("x")]
        public double Xpos { get; protected set; }
        [JsonProperty("y")]
        public double Ypos { get; protected set; }
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


        public void Move()
        {
            this.Xpos += this.Xvel;
            this.Ypos += this.Yvel;
        }

        public void VelocityTowards(Entity goal, double withSpeed = 1)
        {
            double xvel = goal.Xpos - this.Xpos;
            double yvel = goal.Ypos - this.Ypos;
            double norm = xvel * xvel + yvel * yvel;
            // todo: handle case when norm is 0
            this.Xvel = xvel / norm;
            this.Yvel = yvel / norm;
        }
    }
}
