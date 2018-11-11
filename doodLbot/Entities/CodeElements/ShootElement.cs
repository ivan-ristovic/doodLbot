using doodLbot.Logic;

namespace doodLbot.Entities.CodeElements
{
    public class ShootElement : BaseCodeElement
    {
        public ShootElement()
        {

        }


        public override void Execute(GameState state)
            =>state.Hero.Fire(Design.ProjectileSpeed, Design.ProjectileDamage);
    }
}
