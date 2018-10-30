using doodLbot.Entities;
using doodLbot.Hubs;
using NUnit.Framework;

namespace doodLbotTests.Entities
{
    [TestFixture]
    public class EntitySpawnerTests
    {
        [Test]
        public void KamikazeSpawnerTest()
        {
            for (int i = 0; i < 1000; i++) {
                var enemy = Enemy.Spawn<Kamikaze>();
                Assert.IsNotNull(enemy);

                Assert.GreaterOrEqual(enemy.Xpos, 0d);
                Assert.LessOrEqual(enemy.Xpos, GameHub.CanvasSize.X);
                Assert.GreaterOrEqual(enemy.Ypos, 0d);
                Assert.LessOrEqual(enemy.Ypos, GameHub.CanvasSize.Y);

                Assert.That(
                    enemy.Xpos == 0d || 
                    enemy.Xpos == GameHub.CanvasSize.X || 
                    enemy.Ypos == 0d || 
                    enemy.Ypos == GameHub.CanvasSize.Y
                );
            }
        }
    }
}
