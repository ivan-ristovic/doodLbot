using doodLbot.Entities.CodeElements.ConditionElements;
using doodLbot.Logic;

namespace doodLbot.Entities.CodeElements
{
    public class BranchingElement : BaseCodeElement
    {
        private readonly BaseConditionElement condition;
        private readonly CodeBlockElement thenBlock;
        private readonly CodeBlockElement elseBlock;


        public BranchingElement(BaseConditionElement condition, CodeBlockElement thenBlock, CodeBlockElement elseBlock)
        {
            this.condition = condition;
            this.thenBlock = thenBlock;
            this.elseBlock = elseBlock;
        }


        public override void Execute(GameState state)
        {
            if (this.condition.Evaluate(state))
                this.thenBlock.Execute(state);
            else
                this.elseBlock.Execute(state);
        }
    }
}
