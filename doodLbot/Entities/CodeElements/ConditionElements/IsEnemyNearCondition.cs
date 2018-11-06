using System.Collections.Generic;
using System.Linq;

namespace doodLbot.Entities.CodeElements.ConditionElements
{
    public class IsEnemyNearCondition : BaseConditionElement
    {
        public IsEnemyNearCondition()
        {

        }


        public override bool Evaluate(Hero hero = null, IEnumerable<Enemy> enemies = null, IEnumerable<Projectile> projectiles = null)
        {
            if (!enemies?.Any() ?? false)
                return false;

            foreach (Enemy enemy in enemies)
                return true; // TODO

            return false;
        }
    }
}
