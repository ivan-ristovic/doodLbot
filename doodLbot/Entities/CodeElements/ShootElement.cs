using doodLbot.Common;
using doodLbot.Logic;

namespace doodLbot.Entities.CodeElements
{
    /// <summary>
    /// Represents a firing action.
    /// </summary>
    public class ShootElement : BaseCodeElement
    {
        private readonly RateLimiter shootLimit;
        public ShootElement(RateLimiter limiter = null)
        {
            Cost = Design.CostShoot;
            shootLimit = limiter;
        }

        protected override bool OnExecute(GameState state, Hero hero)
        {
            if (!shootLimit?.IsCooldownActive() ?? false)
            {
                hero.TryFire(Design.ProjectileSpeed, Design.ProjectileDamage);
            }

            return true;
        }
    }
}
