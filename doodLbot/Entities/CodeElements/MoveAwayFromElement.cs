using System;
using doodLbot.Logic;

namespace doodLbot.Entities.CodeElements
{
    public class MoveAwayFromElement : TargetElement
    {

        public MoveAwayFromElement()
        {
            Cost = Design.CostTarget;
        }


        protected override bool OnExecute(GameState state, Hero hero)
        {
            if (!Target(state, hero))
            {
                hero.IsControlledByAlgorithm = false;
                hero.UpdateSyntheticControls(ConsoleKey.S, false);
                return false;
            }
            hero.UpdateSyntheticControls(ConsoleKey.S, true);
            hero.IsControlledByAlgorithm = true;
            return true;
        }
    }
}
