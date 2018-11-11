using doodLbot.Logic;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace doodLbot.Entities.CodeElements
{
    public class CodeBlockElement : BaseCodeElement
    {
        [JsonProperty("elements")]
        private readonly ICollection<BaseCodeElement> CodeElements;


        public CodeBlockElement(ICollection<BaseCodeElement> elements)
        {
            this.CodeElements = elements;
        }


        public override void Execute(GameState state)
        {
            foreach (BaseCodeElement element in this.CodeElements)
                element.Execute(state);
        }
    }
}
