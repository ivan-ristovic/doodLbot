using doodLbot.Logic;
using System.Collections.Generic;
using System.Linq;

namespace doodLbot.Entities.CodeElements.ConditionElements
{
    public class IsEnemyNearCondition : BaseConditionElement
    {
        public IsEnemyNearCondition()
        {

        }


        public override bool Evaluate(GameState state)
        {
            IReadOnlyList<Enemy> enemies = state.Enemies;

            if (!enemies?.Any() ?? false)
                return false;

            foreach (Enemy enemy in enemies)
                return true; // TODO

            return false;
        }
    }
}
