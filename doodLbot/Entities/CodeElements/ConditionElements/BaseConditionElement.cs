using System.Collections.Generic;

namespace doodLbot.Entities.CodeElements.ConditionElements
{
    abstract public class BaseConditionElement
    {
        abstract public bool Evaluate(Hero hero = null, IEnumerable<Enemy> enemies = null, 
            IEnumerable<Projectile> projectiles = null);
    }
}
