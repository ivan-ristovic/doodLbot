using doodLbot.Logic;

namespace doodLbot.Entities.CodeElements
{
    /// <summary>
    /// Represents a firing action.
    /// </summary>
    public class ShootElement : BaseCodeElement
    {
        public ShootElement()
        {
            
        }


        public override void Execute(GameState state)
        {
            if (!this.IsActive)
                return;
            state.Hero.Fire(Design.ProjectileSpeed, Design.ProjectileDamage);
        }
    }
}
