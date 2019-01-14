using doodLbot.Logic;
using System.Collections.Generic;
using System.Linq;

namespace doodLbot.Entities.CodeElements.ConditionElements
{
    /// <summary>
    /// Represents a condition element that evaluates to true if an enemy is near the hero.
    /// </summary>
    public class IsEnemyNearCondition : BaseConditionElement
    {
        /// <summary>
        /// Constructs a new IsEnemyNearCondition element.
        /// </summary>
        public IsEnemyNearCondition()
        {
            this.Cost = Design.CostIsNear;
        }


        protected override void OnExecute(GameState state)
        {

        }

        public override bool Evaluate(GameState state)
        {
            IReadOnlyCollection<Enemy> enemies = state.Enemies;

            if (!enemies?.Any() ?? false)
                return false;

            foreach (Enemy enemy in enemies)
                return true; // TODO

            return false;
        }
    }
}
