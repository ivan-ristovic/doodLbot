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


        public override bool Execute(Hero hero)
        {
            bool finished = this.codeElements[this.index].Execute(hero);

            if (!finished)
                return false;

            this.index++;
            if (this.index == this.codeElements.Count) {
                this.Reset();
                return true;
            } else {
                return false;
            }
        }

        public override void Reset()
        {
            foreach (BaseCodeElement element in this.codeElements)
                element.Reset();
            this.index = 0;
        }
    }
}
