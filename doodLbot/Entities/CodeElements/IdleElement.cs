using doodLbot.Logic;

namespace doodLbot.Entities.CodeElements
{
    /// <summary>
    /// Represents an idle action.
    /// </summary>
    public class IdleElement : BaseCodeElement
    {
        public IdleElement()
        {
            this.Cost = Design.CostIdle;
        }

        protected override bool OnExecute(GameState state, Hero hero)
        {
            return true;
        }
    }
}