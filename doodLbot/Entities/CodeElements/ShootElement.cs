namespace doodLbot.Entities.CodeElements
{
    public class ShootElement : BaseCodeElement
    {
        public ShootElement()
        {

        }

        public override void Execute(Hero hero)
            => hero.Fire();
    }
}
