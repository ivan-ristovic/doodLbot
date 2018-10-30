using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doodLbot.Entities
{
    public class Entity
    {
        public double Xpos { get; protected set; }
        public double Ypos { get; protected set; }
        public double Xvel { get; set; }
        public double Yvel { get; set; }
        public double Hp { get; protected set; }
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
    }
}
