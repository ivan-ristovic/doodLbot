using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doodLbot.Entities
{
    public class Entity
    {
        public double Xpos { get; private set; }
        public double Ypos { get; private set; }
        public double Xvel { get; private set; }
        public double Yvel { get; private set; }
        public double Hp { get; private set; }
        public double Damage { get; private set; }


        public Entity()
        {
            this.Hp = 100;
            this.Damage = 1;
        }
    }
}
