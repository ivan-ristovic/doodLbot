using System.Collections.Generic;
using System.Linq;
using doodLbot.Logic;
using Newtonsoft.Json;

namespace doodLbot.Entities.CodeElements
{
    /// <summary>
    /// Represents a hero behaviour algorithm made of code elements.
    /// </summary>
    public class BehaviourAlgorithm
    {
        /// <summary>
        /// Gets a collection of code elements that this algorithm is made of.
        /// </summary>
        [JsonProperty("elements")]
        public IReadOnlyList<BaseCodeElement> CodeElements => codeElements.AsReadOnly();

        private readonly Hero hero;
        private readonly List<BaseCodeElement> codeElements;
        private readonly object codeElementsLock;


        /// <summary>
        /// Constructs a new empty BehaviourAlgorithm.
        /// </summary>
        public BehaviourAlgorithm(Hero hero)
        {
            this.hero = hero;
            codeElements = new List<BaseCodeElement>();
            codeElementsLock = new object();
        }
        

        /// <summary>
        /// Insert given code element to this algorithm.
        /// </summary>
        /// <param name="element">Code element to insert</param>
        /// <param name="index">Position in the algorithm element list.</param>
        public void Insert(BaseCodeElement element, int? index = null)
        {
            lock (codeElementsLock)
            {
                if (index is null)
                    codeElements.Add(element);
                else
                    codeElements.Insert(index.Value, element);
            }
        }

        /// <summary>
        /// Remove an element from the algorithm code element list.
        /// </summary>
        /// <param name="index">Position of the element to remove.</param>
        public void RemoveAt(int index)
        {
            lock (codeElementsLock)
                codeElements.RemoveAt(index);
        }

        /// <summary>
        /// Executes this algorithm. All code elements are executed at once.
        /// </summary>
        /// <param name="state"></param>
        public void Execute(GameState state)
        {
            lock (codeElementsLock)
            {
                if (!codeElements.Any())
                    return;
                
                foreach (var element in codeElements)
                {
                    element.Execute(state, hero);
                }
            }
        }
    }
}
