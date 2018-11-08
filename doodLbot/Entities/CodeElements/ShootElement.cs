using doodLbot.Logic;

namespace doodLbot.Entities.CodeElements
{
    public class ShootElement : BaseCodeElement
    {
        public ShootElement()
        {
        }

        public override bool Execute(Hero hero)
        {
            hero.Fire(Design.ProjectileSpeed, Design.ProjectileDamage);
            return true;
        }
    }
}
