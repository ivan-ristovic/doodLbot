using System;
using doodLbot.Logic;

namespace doodLbot.Entities.CodeElements
{
    /// <summary>
    /// When executed, moves the hero in the opposite direction relative to the nearest enemy.
    /// </summary>
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
