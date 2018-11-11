using doodLbot.Entities.CodeElements.ConditionElements;
using doodLbot.Logic;

using Newtonsoft.Json;

namespace doodLbot.Entities.CodeElements
{
    public class BranchingElement : BaseCodeElement
    {
        [JsonProperty("cond")]
        public BaseConditionElement Condition { get; }
        [JsonProperty("then")]
        public CodeBlockElement ThenBlock { get; }
        [JsonProperty("else")]
        public CodeBlockElement ElseBlock { get; }


        public BranchingElement(BaseConditionElement condition, CodeBlockElement thenBlock, CodeBlockElement elseBlock)
        {
            this.Condition = condition;
            this.ThenBlock = ThenBlock;
            this.ElseBlock = ElseBlock;
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
