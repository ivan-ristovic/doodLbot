using doodLbot.Logic;
using Serilog;
using System;

namespace doodLbot.Entities
{
    public abstract class Enemy : Entity
    {
        // spawns enemies around (spawnX, spawnY) in radius range
        static public T Spawn<T>(double spawnX, double spawnY, double radius) where T : Enemy, new()
        {
            var rand = new Random();
            double xpos, ypos;
            const double centerRandomVar = 0.5;
            double r = radius * 2;
            xpos = spawnX + (rand.NextDouble() - centerRandomVar) * r;
            ypos = spawnY + (rand.NextDouble() - centerRandomVar) * r;

            Log.Debug($"Spawning: {typeof(T)} at ({xpos}, {ypos}) ; player position ({spawnX}, {spawnY})");

            return new T() {
                Xpos = xpos,
                Ypos = ypos
            };
        }
    }
}
