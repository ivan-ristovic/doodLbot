using doodLbot.Logic;

using Newtonsoft.Json;

using System.Collections.Generic;

namespace doodLbot.Entities.CodeElements
{
    public class CodeBlockElement : BaseCodeElement
    {
        [JsonProperty("elements")]
        public IReadOnlyCollection<BaseCodeElement> CodeElements { get; }


        public CodeBlockElement(ICollection<BaseCodeElement> elements)
        {
            this.CodeElements = elements as IReadOnlyCollection<BaseCodeElement>;
        }


        public override void Execute(GameState state)
        {
            foreach (BaseCodeElement element in this.CodeElements)
                element.Execute(state);
        }
    }
}
