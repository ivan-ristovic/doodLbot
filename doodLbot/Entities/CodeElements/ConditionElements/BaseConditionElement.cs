using doodLbot.Logic;

namespace doodLbot.Entities.CodeElements.ConditionElements
{
    abstract public class BaseConditionElement : BaseCodeElement
    {
        abstract public bool Evaluate(GameState state);


        public override void Execute(GameState state)
        {

        }
    }
}
