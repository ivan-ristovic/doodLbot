using doodLbot.Entities;
using doodLbot.Entities.CodeElements;

using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace doodLbot.Tests.Entities.CodeElements
{
    [TestFixture]
    public class BehaviourAlgorithmTests
    {
        [Test]
        public async Task InsertRaceConditionTestAsync()
        {
            var algorithm = new BehaviourAlgorithm();

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
            const int count = 100;

            var algorithm = new BehaviourAlgorithm();
            for (int i = 0; i < count; i++)
                algorithm.Insert(new ShootElement());

            var tasks = new Task[count];
            for (int i = 0; i < tasks.Length; i++) {
                var element = new ShootElement();
                tasks[i] = Task.Run(() => algorithm.RemoveAt(0));
            }

            await Task.WhenAll(tasks);

            CollectionAssert.IsEmpty(algorithm.CodeElements);
        }
    }
}
