using doodLbot.Logic;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace doodLbot.Entities.CodeElements
{
    public class BehaviourAlgorithm
    {
        [JsonProperty("elements")]
        public IReadOnlyList<BaseCodeElement> CodeElements => this.codeElements.AsReadOnly();

        private readonly List<BaseCodeElement> codeElements;
        private readonly object codeElementsLock;

        public BehaviourAlgorithm()
        {
            this.codeElements = new List<BaseCodeElement>();
            this.codeElementsLock = new object();
        }

        public void Insert(BaseCodeElement element, int? index = null)
        {
            lock (this.codeElementsLock)
            {
                if (index is null)
                    this.codeElements.Add(element);
                else
                    this.codeElements.Insert(index.Value, element);
            }
        }

        public void RemoveAt(int index)
        {
            lock (this.codeElementsLock)
                this.codeElements.RemoveAt(index);
        }

        public void Execute(GameState state)
        {
            lock (this.codeElementsLock)
            {
                if (!this.codeElements.Any())
                    return;

                foreach (BaseCodeElement element in this.codeElements)
                    element.Execute(state);
            }
        }
    }
}
