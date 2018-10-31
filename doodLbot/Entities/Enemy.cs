using doodLbot.Logic;
using System;

namespace doodLbot.Entities
{
    public abstract class Enemy : Entity
    {
        // spawns enemies around (spawnX, spawnY) in radius range
        static public T Spawn<T>(double spawnX, double spawnY, double radius) where T : Enemy, new()
        {
            var rand = new Random();
            // TODO change all doubles into floats, not really life-changing,
            // but games rarely need the precision of double.
            // On the other hand, JS doesn't have any 32bit floats,
            // so there would be conversions from float to double
            // which is probably more of a slowdown than double is to float
            double xpos, ypos;
            const double centerRandomVar = 0.5;
            double r = radius * 2;
            xpos = spawnX + (rand.NextDouble() - centerRandomVar) * r;
            ypos = spawnY + (rand.NextDouble() - centerRandomVar) * r;

            return new T() {
                Xpos = xpos,
                Ypos = ypos
            };
        }
    }
}
