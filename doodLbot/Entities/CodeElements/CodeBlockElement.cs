using System.Collections.Generic;
using doodLbot.Logic;
using Newtonsoft.Json;

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

        public override bool IsActive => true;


        /// <summary>
        /// Constructs a new CodeElementBlock from a collection of code elements.
        /// </summary>
        /// <param name="elements"></param>
        public CodeBlockElement(ICollection<BaseCodeElement> elements = null)
        {
            if (elements is null)
            {
                elements = new List<BaseCodeElement>();
            }
            CodeElements = elements as IReadOnlyCollection<BaseCodeElement>;
        }

        protected override bool OnExecute(GameState state, Hero hero)
        {
            foreach (var element in CodeElements)
            {
                if (!element.Execute(state, hero))
                    return false;
            }

            return true;
        }
    }
}
