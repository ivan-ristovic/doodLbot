using doodLbot.Common;
using doodLbot.Logic;

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

        protected override void OnExecute(GameState state)
        {
            if (!shootLimit?.IsCooldownActive() ?? false)
            {
                state.Hero.TryFire(Design.ProjectileSpeed, Design.ProjectileDamage);
            }
        }
    }
}
