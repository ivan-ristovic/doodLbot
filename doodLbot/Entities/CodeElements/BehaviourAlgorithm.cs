using doodLbot.Logic;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace doodLbot.Entities.CodeElements
{
    /// <summary>
    /// Represents a hero behaviour algorithm made of code elements.
    /// </summary>
    public class BehaviourAlgorithm
    {
        /// <summary>
        /// Get a collection of code elements that this algorithm is made of.
        /// </summary>
        [JsonProperty("elements")]
        public IReadOnlyList<BaseCodeElement> CodeElements => this.codeElements.AsReadOnly();

        private readonly List<BaseCodeElement> codeElements;
        private readonly object codeElementsLock;


        /// <summary>
        /// Constructs a new blank BehaviourAlgorithm.
        /// </summary>
        public BehaviourAlgorithm()
        {
            this.codeElements = new List<BaseCodeElement>();
            this.codeElementsLock = new object();
        }
        

        /// <summary>
        /// Insert a code element in this algorithm.
        /// </summary>
        /// <param name="element">Code element to insert</param>
        /// <param name="index">Position in the algorithm element list.</param>
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

        /// <summary>
        /// Remove an element from the algorithm code element list.
        /// </summary>
        /// <param name="index">Position of the element to remove.</param>
        public void RemoveAt(int index)
        {
            lock (this.codeElementsLock)
                this.codeElements.RemoveAt(index);
        }

        /// <summary>
        /// Executes this algorithm. All code elements are executed at once.
        /// </summary>
        /// <param name="state"></param>
        public void Execute(GameState state)
        {
            lock (this.codeElementsLock)
            {
                if (!this.codeElements.Any())
                    return;

                // TODO elements don't share any state for now,
                // think about how to pipe them, for example: target->shoot
                // without this dirty fix, hero can't rotate while targeting
                var hero = state.Heroes.First();
                var saveRotation = hero.Rotation;
                foreach (BaseCodeElement element in this.codeElements)
                {
                    element.Execute(state);
                }
                hero.Rotation = saveRotation;
            }
        }
    }
}
