using doodLbot.Entities;
using doodLbot.Entities.CodeElements;

using NUnit.Framework;

using System.Collections.Generic;
using System.Linq;

namespace doodLbot.Tests.Entities.CodeElements
{
    [TestFixture]
    public class CodeBlockElementTests
    {
        [Test]
        public void CodeBlockExecuteTest()
        {
            const int count = 5;
            var block = new CodeBlockElement(new List<BaseCodeElement>(Enumerable.Repeat(new ShootElement(), count)));

            var hero = new Hero();
            for (int i = 1; i < 20; i++)
                Assert.That(block.Execute(hero) == (i % count == 0));
        }

        [Test]
        public void CodeBlockRecursiveExecuteTest()
        {
            const int count = 5;
            var block = new CodeBlockElement(Enumerable.Repeat(new CodeBlockElement(Enumerable.Repeat(new ShootElement(), count)), count));

            var hero = new Hero();
            for (int i = 0; i < count * count - 1; i++)
                Assert.That(block.Execute(hero) == false);
            Assert.That(block.Execute(hero) == true);
        }
    }
}
