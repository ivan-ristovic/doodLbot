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


        public override bool Execute(Hero hero)
        {
            bool finished = this.condition.Evaluate() ? this.thenBlock.Execute(hero) : this.elseBlock.Execute(hero);
            if (finished)
                this.Reset();
            return finished;
        }

        public override void Reset()
        {
            this.thenBlock.Reset();
            this.elseBlock.Reset();
        }
    }
}
