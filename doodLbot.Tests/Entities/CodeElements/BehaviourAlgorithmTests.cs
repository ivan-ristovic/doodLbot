using System.Threading.Tasks;
using doodLbot.Entities;
using doodLbot.Entities.CodeElements;
using NUnit.Framework;

namespace doodLbot.Tests.Entities.CodeElements
{
    [TestFixture]
    public class BehaviourAlgorithmTests
    {
        [Test]
        public async Task InsertRaceConditionTestAsync()
        {
            var h = new Hero(0, 0, 0, null, null);
            var algorithm = new BehaviourAlgorithm(h);

            const int count = 100;

            var tasks = new Task[count];
            for (int i = 0; i < tasks.Length; i++)
                tasks[i] = Task.Run(() => algorithm.Insert(new ShootElement()));

            await Task.WhenAll(tasks);

            Assert.AreEqual(count, algorithm.CodeElements.Count);
        }

        [Test]
        public async Task RemoveRaceConditionTestAsync()
        {
            var h = new Hero(0, 0, 0, null, null);
            const int count = 100;

            var algorithm = new BehaviourAlgorithm(h);
            for (int i = 0; i < count; i++)
                algorithm.Insert(new ShootElement());

            var tasks = new Task[count];
            for (int i = 0; i < tasks.Length; i++)
            {
                var element = new ShootElement();
                tasks[i] = Task.Run(() => algorithm.RemoveAt(0));
            }

            await Task.WhenAll(tasks);

            CollectionAssert.IsEmpty(algorithm.CodeElements);
        }
    }
}
