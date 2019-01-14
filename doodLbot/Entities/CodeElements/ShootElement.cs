using doodLbot.Common;
using doodLbot.Logic;
using System.Linq;

namespace doodLbot.Entities.CodeElements
{
    /// <summary>
    /// Represents a firing action.
    /// </summary>
    public class ShootElement : BaseCodeElement
    {
        private RateLimiter shootLimit;
        public ShootElement(RateLimiter limiter = null)
        {
            this.Cost = Design.CostShoot;
            shootLimit = limiter;
        }

        protected override void OnExecute(GameState state, Hero hero)
        {
            if (!shootLimit?.IsCooldownActive() ?? false)
            {
                hero.TryFire(Design.ProjectileSpeed, Design.ProjectileDamage);
            }
        }
    }
}
