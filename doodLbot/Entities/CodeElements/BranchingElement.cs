using doodLbot.Entities.CodeElements.ConditionElements;

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


        public override void Execute(Hero hero)
        {
            if (this.condition.Evaluate())
                this.thenBlock.Execute(hero);
            else
                this.elseBlock.Execute(hero);
        }
    }
}
