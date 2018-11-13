using doodLbot.Logic;

using Newtonsoft.Json;

using System.Collections.Generic;

namespace doodLbot.Entities.CodeElements
{
    /// <summary>
    /// Represents a block of code elements.
    /// </summary>
    public class CodeBlockElement : BaseCodeElement
    {
        /// <summary>
        /// Get code blocks that are contained in this block.
        /// </summary>
        [JsonProperty("elements")]
        public IReadOnlyCollection<BaseCodeElement> CodeElements { get; }


        /// <summary>
        /// Constructs a new CodeElementBlock from a collection of code elements.
        /// </summary>
        /// <param name="elements"></param>
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
