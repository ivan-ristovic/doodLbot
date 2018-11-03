using doodLbot.Entities;

using NUnit.Framework;

using System;

namespace doodLbotTests.Entities
{
    [TestFixture]
    public class EntitySpawnerTests
    {
        [Test]
        public void KamikazeSpawnerTest()
        {
            var rnd = new Random();
            const double maxValue = 100d;

            for (int i = 0; i < 1000; i++) {
                double radius = rnd.NextDouble() * maxValue + 1d;
                (double Xpos, double Ypos) hero = (rnd.NextDouble() * maxValue, rnd.NextDouble() * maxValue);

                var enemy = Enemy.Spawn<Kamikaze>(hero.Xpos, hero.Ypos, radius);
                Assert.IsNotNull(enemy);
                
                Assert.LessOrEqual(Math.Abs(enemy.Xpos - hero.Xpos), radius);
                Assert.LessOrEqual(Math.Abs(enemy.Ypos - hero.Ypos), radius);
            }
        }
    }
}
