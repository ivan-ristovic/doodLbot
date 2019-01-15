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


        protected override bool OnExecute(GameState state, Hero hero)
        {
            return true;
        }

        public override bool Evaluate(GameState state, Hero hero)
        {
            IReadOnlyCollection<Enemy> enemies = state.Enemies;

            if (!enemies?.Any() ?? false)
                return false;

            foreach (Enemy enemy in enemies)
                if (enemy.SquaredDist(hero) < Design.SpawnRange / 10)
                    return true;

            return false;
        }
    }
}
