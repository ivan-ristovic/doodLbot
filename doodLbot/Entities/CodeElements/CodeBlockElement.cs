using doodLbot.Logic;

using System.Collections.Generic;

namespace doodLbot.Entities.CodeElements
{
    public class CodeBlockElement : BaseCodeElement
    {
        private readonly ICollection<BaseCodeElement> codeElements;


        public CodeBlockElement(ICollection<BaseCodeElement> elements)
        {
            this.codeElements = elements;
        }


        public override void Execute(GameState state)
        {
            foreach (BaseCodeElement element in this.codeElements)
                element.Execute(state);
        }
    }
}
