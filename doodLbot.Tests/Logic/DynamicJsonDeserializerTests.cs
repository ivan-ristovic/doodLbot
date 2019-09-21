using System.Linq;
using doodLbot.Entities;
using doodLbot.Entities.CodeElements;
using doodLbot.Entities.CodeElements.ConditionElements;
using doodLbot.Logic;
using NUnit.Framework;

namespace doodLbot.Tests.Logic
{
    [TestFixture]
    public class DynamicJsonDeserializerTests
    {
        [Test]
        public void DynamicDeserializationTest()
        {
            var h = new Hero(0, 0, 0, null, null);
            string json;
            BehaviourAlgorithm algorithm;

            json = "[{\"type\":\"ShootElement\", \"isActive\":\"true\"}]";
            algorithm = DynamicJsonDeserializer.ToBehaviourAlgorithm(json, h);
            Assert.NotNull(algorithm);
            CollectionAssert.IsNotEmpty(algorithm.CodeElements);
            CollectionAssert.AllItemsAreNotNull(algorithm.CodeElements);
            Assert.AreEqual(1, algorithm.CodeElements.Count);
            Assert.IsInstanceOf<ShootElement>(algorithm.CodeElements[0]);

            json = "[{\"type\":\"CodeBlockElement\", \"isActive\":\"true\", \"elements\":[{\"type\":\"ShootElement\", \"isActive\":\"true\"}, {\"type\":\"IdleElement\", \"isActive\":\"true\"}]}]";
            algorithm = DynamicJsonDeserializer.ToBehaviourAlgorithm(json, h);
            Assert.NotNull(algorithm);
            CollectionAssert.IsNotEmpty(algorithm.CodeElements);
            CollectionAssert.AllItemsAreNotNull(algorithm.CodeElements);
            Assert.AreEqual(1, algorithm.CodeElements.Count);
            Assert.IsInstanceOf<CodeBlockElement>(algorithm.CodeElements[0]);
            {
                var block = algorithm.CodeElements[0] as CodeBlockElement;
                Assert.NotNull(block);
                Assert.NotNull(block.CodeElements);
                CollectionAssert.IsNotEmpty(block.CodeElements);
                CollectionAssert.AllItemsAreNotNull(block.CodeElements);
                Assert.AreEqual(2, block.CodeElements.Count);
                Assert.IsInstanceOf<ShootElement>(block.CodeElements.ElementAt(0));
                Assert.IsInstanceOf<IdleElement>(block.CodeElements.ElementAt(1));
            }

            json = "["
                   + "{\"type\":\"ShootElement\", \"isActive\":\"true\"}, "
                   + "{\"type\":\"CodeBlockElement\", \"isActive\":\"true\", \"elements\":[{\"type\":\"ShootElement\", \"isActive\":\"true\"}]},"
                   + "{\"type\":\"BranchingElement\", \"isActive\":\"true\", "
                   + "\"cond\":{\"type\":\"IsEnemyNearCondition\", \"isActive\":\"true\"}, "
                   + "\"then\":{\"type\":\"CodeBlockElement\", \"isActive\":\"true\", \"elements\":[{\"type\":\"ShootElement\", \"isActive\":\"true\"}, {\"type\":\"IdleElement\", \"isActive\":\"true\"}]}, "
                   + "\"else\":{\"type\":\"CodeBlockElement\", \"isActive\":\"true\", \"elements\":[{\"type\":\"ShootElement\", \"isActive\":\"true\"}, {\"type\":\"IdleElement\", \"isActive\":\"true\"}]}"
                   + "}"
                   + "]";
            algorithm = DynamicJsonDeserializer.ToBehaviourAlgorithm(json, null);
            Assert.NotNull(algorithm);
            CollectionAssert.IsNotEmpty(algorithm.CodeElements);
            CollectionAssert.AllItemsAreNotNull(algorithm.CodeElements);
            Assert.AreEqual(3, algorithm.CodeElements.Count);
            Assert.IsInstanceOf<ShootElement>(algorithm.CodeElements[0]);
            Assert.IsInstanceOf<CodeBlockElement>(algorithm.CodeElements[1]);
            {
                var block = algorithm.CodeElements[1] as CodeBlockElement;
                Assert.NotNull(block);
                Assert.NotNull(block.CodeElements);
                CollectionAssert.IsNotEmpty(block.CodeElements);
                CollectionAssert.AllItemsAreNotNull(block.CodeElements);
                Assert.AreEqual(1, block.CodeElements.Count);
                Assert.IsInstanceOf<ShootElement>(block.CodeElements.First());
            }
            Assert.IsInstanceOf<BranchingElement>(algorithm.CodeElements[2]);
            {
                var branching = algorithm.CodeElements[2] as BranchingElement;
                Assert.NotNull(branching);
                Assert.NotNull(branching.Condition);
                Assert.NotNull(branching.ThenBlock);
                Assert.NotNull(branching.ElseBlock);
                CollectionAssert.IsNotEmpty(branching.ThenBlock.CodeElements);
                CollectionAssert.IsNotEmpty(branching.ElseBlock.CodeElements);
                CollectionAssert.AllItemsAreNotNull(branching.ThenBlock.CodeElements);
                CollectionAssert.AllItemsAreNotNull(branching.ElseBlock.CodeElements);
                Assert.AreEqual(2, branching.ThenBlock.CodeElements.Count);
                Assert.AreEqual(2, branching.ElseBlock.CodeElements.Count);
                Assert.IsInstanceOf<IsEnemyNearCondition>(branching.Condition);
                Assert.IsInstanceOf<ShootElement>(branching.ThenBlock.CodeElements.ElementAt(0));
                Assert.IsInstanceOf<IdleElement>(branching.ThenBlock.CodeElements.ElementAt(1));
                Assert.IsInstanceOf<ShootElement>(branching.ElseBlock.CodeElements.ElementAt(0));
                Assert.IsInstanceOf<IdleElement>(branching.ElseBlock.CodeElements.ElementAt(1));
            }

        }
    }
}
