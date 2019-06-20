using doodLbot.Logic;

namespace doodLbot.Entities.CodeElements
{
    /// <summary>
    /// Represents an idle action.
    /// </summary>
    public class IdleElement : BaseCodeElement
    {
        private static readonly int _limit = 50;

        private int counter;


        public IdleElement()
        {
            Cost = Design.CostIdle;
        }


        protected override bool OnExecute(GameState state, Hero hero)
        {
            counter++;
            if (counter > _limit)
            {
                counter = 0;
                return true;
            }
            return false;
        }
    }
}
