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

            for (var i = 0; i < 1000; i++) {
                var radius = rnd.NextDouble() * maxValue + 1d;
                (double Xpos, double Ypos) hero = (rnd.NextDouble() * maxValue, rnd.NextDouble() * maxValue);

                var enemy = Enemy.Spawn<Kamikaze>(hero.Xpos, hero.Ypos, radius, 0);
                Assert.IsNotNull(enemy);
                
                Assert.LessOrEqual(Math.Abs(enemy.Xpos - hero.Xpos), radius);
                Assert.LessOrEqual(Math.Abs(enemy.Ypos - hero.Ypos), radius);
            }
        }
    }
}
