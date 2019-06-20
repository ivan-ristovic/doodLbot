using System.Linq;
using doodLbot.Logic;

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
            Cost = Design.CostIsNear;
        }


        protected override bool OnExecute(GameState state, Hero hero) => true;

        public override bool Evaluate(GameState state, Hero hero)
        {
            var enemies = state.Enemies;

            if (!enemies?.Any() ?? false)
                return false;

            foreach (var enemy in enemies)
                if (enemy.SquaredDist(hero) < Design.SpawnRange * Design.SpawnRange)
                    return true;

            return false;
        }
    }
}
