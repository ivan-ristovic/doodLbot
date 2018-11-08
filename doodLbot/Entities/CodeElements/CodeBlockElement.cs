using System.Collections.Generic;
using System.Linq;

namespace doodLbot.Entities.CodeElements
{
    public class CodeBlockElement : BaseCodeElement
    {
        private readonly List<BaseCodeElement> codeElements;


        public CodeBlockElement(IEnumerable<BaseCodeElement> elements)
        {
            this.codeElements = elements.ToList();
        }


        public override void Execute(Hero hero)
        {
            foreach (BaseCodeElement element in this.codeElements)
                element.Execute(hero);
        }
    }
}
