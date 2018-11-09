using doodLbot.Logic;

namespace doodLbot.Entities.CodeElements.ConditionElements
{
    abstract public class BaseConditionElement
    {
        abstract public bool Evaluate(GameState state);
    }
}
