using doodLbot.Logic;

namespace doodLbot.Entities.CodeElements.ConditionElements
{
    /// <summary>
    /// Represents an abstract condition code element.
    /// </summary>
    abstract public class BaseConditionElement : BaseCodeElement
    {
        /// <summary>
        /// Evaluate this condition element and return the boolean condition evaluation.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        abstract public bool Evaluate(GameState state, Hero hero);


        protected override bool OnExecute(GameState state, Hero hero)
        {
            return true;
        }
    }
}
