using doodLbot.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doodLbot.Entities.CodeElements
{
    public class MoveAwayFromElement : TargetElement
    {

        public MoveAwayFromElement()
        {
            this.Cost = Design.CostTarget;
        }


        protected override bool OnExecute(GameState state, Hero hero)
        {
            if (!Target(state, hero)) {
                hero.UpdateControls(ConsoleKey.S, false);
                return false;
            }
            hero.UpdateControls(ConsoleKey.S, true);
            return true;
        }
    }
}
