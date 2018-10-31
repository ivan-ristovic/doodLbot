using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doodLbot.Entities
{
    public class Kamikaze : Enemy
    {
        public Kamikaze() : base()
        {
            // for testing
            this.Xpos = 300;
            this.Ypos = 300;
            this.Xvel = 1;
            this.Yvel = 0.5;
        }
    }
}
