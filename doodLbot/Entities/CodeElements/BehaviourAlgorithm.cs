using System;
using System.Collections.Generic;
using System.Linq;

namespace doodLbot.Entities.CodeElements
{
    public class BehaviourAlgorithm
    {
        public IReadOnlyList<BaseCodeElement> CodeElements => this.codeElements.AsReadOnly();

        private readonly List<BaseCodeElement> codeElements;
        private readonly object codeElementsLock;
        private int index;


        public BehaviourAlgorithm()
        {
            this.codeElements = new List<BaseCodeElement>();
            this.codeElementsLock = new object();
            this.index = 0;
        }
        

        public void Insert(BaseCodeElement element, int? index = null)
        {
            lock (this.codeElementsLock) {
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

        public void ExecuteStep(Hero hero)
        {
            lock (this.codeElementsLock) {
                if (!this.codeElements.Any())
                    return;

                if (this.index >= this.codeElements.Count)
                    this.Reset();

                if (this.codeElements[this.index].Execute(hero))
                    this.index++;
            }
        }


        private void Reset()
        {
            lock (this.codeElementsLock) {
                foreach (BaseCodeElement element in this.codeElements)
                    element.Reset();
                this.index = 0;
            }
        }
    }
}
