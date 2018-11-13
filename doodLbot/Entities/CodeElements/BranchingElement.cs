using doodLbot.Entities.CodeElements.ConditionElements;
using doodLbot.Logic;

using Newtonsoft.Json;

namespace doodLbot.Entities.CodeElements
{
    /// <summary>
    /// Represents a branching statement made of condition, then and else blocks.
    /// </summary>
    public class BranchingElement : BaseCodeElement
    {
        /// <summary>
        /// Get this element's condition element.
        /// </summary>
        [JsonProperty("cond")]
        public BaseConditionElement Condition { get; }
        /// <summary>
        /// Get this element's "then" block.
        /// </summary>
        [JsonProperty("then")]
        public CodeBlockElement ThenBlock { get; }
        /// <summary>
        /// Get this element's "else" block.
        /// </summary>
        [JsonProperty("else")]
        public CodeBlockElement ElseBlock { get; }


        /// <summary>
        /// Constructs a new branching element from a condition, "then" and "else blocks.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="thenBlock"></param>
        /// <param name="elseBlock"></param>
        public BranchingElement(BaseConditionElement condition, CodeBlockElement thenBlock, CodeBlockElement elseBlock)
        {
            this.Condition = condition;
            this.ThenBlock = thenBlock;
            this.ElseBlock = elseBlock;
        }


        public override void Execute(GameState state)
        {
            if (this.Condition.Evaluate(state))
                this.ThenBlock.Execute(state);
            else
                this.ElseBlock.Execute(state);
        }
    }
}
