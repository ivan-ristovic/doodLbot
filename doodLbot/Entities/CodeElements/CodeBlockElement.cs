using doodLbot.Logic;
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


        public override void Execute(GameState state)
        {
            foreach (BaseCodeElement element in this.codeElements)
                element.Execute(state);
        }
    }
}
