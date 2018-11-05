using System.Collections.Generic;
using System.Linq;

namespace doodLbot.Entities.CodeBlocks
{
    public class BehaviourAlgorithm
    {
        public IReadOnlyList<BaseCodeElement> CodeElements => this.codeElements.AsReadOnly();

        private readonly List<BaseCodeElement> codeElements;
        private readonly object codeBlocksLock;
        private int index;


        public BehaviourAlgorithm()
        {
            this.codeElements = new List<BaseCodeElement>();
            this.codeBlocksLock = new object();
            this.index = 0;
        }
        

        public void InsertAt(BaseCodeElement element)
        {
            lock (this.codeBlocksLock)
                this.codeElements.Insert(this.index, element);
        }

        public void RemoveAt(int index)
        {
            lock (this.codeBlocksLock)
                this.codeElements.RemoveAt(this.index);
        }

        public void ExecuteStep(Hero hero)
        {
            lock (this.codeBlocksLock) {
                if (!this.codeElements.Any())
                    return;

                if (this.index >= this.codeElements.Count)
                    this.index = 0;

                switch (this.codeElements[this.index]) {
                    case ShootElement _: hero.Fire(); break;
                    default: break;
                }

                this.index++;
            }
        }
    }
}
