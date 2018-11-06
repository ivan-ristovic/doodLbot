using System.Collections.Generic;
using System.Linq;

namespace doodLbot.Entities.CodeElements
{
    public class CodeBlockElement : BaseCodeElement
    {
        private readonly List<BaseCodeElement> codeElements;
        private int index;


        public CodeBlockElement(IEnumerable<BaseCodeElement> elements)
        {
            this.codeElements = elements.ToList();
            this.index = 0;
        }


        public override void Execute(Hero hero)
        {
            if (this.index >= this.codeElements.Count)
                this.Reset();

            this.codeElements[this.index].Execute(hero);

            this.index++;
        }

        public override void Reset()
        {
            foreach (CodeBlockElement element in this.codeElements)
                element.Reset();
            this.index = 0;
        }
    }
}
