using doodLbot.Logic;

namespace doodLbot.Entities.CodeElements
{
    /// <summary>
    /// Represents an idle action.
    /// </summary>
    public class IdleElement : BaseCodeElement
    {
        private static int _limit = 50;
        private int counter;


        public IdleElement()
        {
            this.Cost = Design.CostIdle;
        }

        protected override bool OnExecute(GameState state, Hero hero)
        {
            this.counter++;
            if (this.counter > _limit) {
                this.counter = 0;
                return true;
            }
            return false;
        }
    }
}