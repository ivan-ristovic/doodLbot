using Serilog;

using System;

namespace doodLbot.Entities
{
    /// <summary>
    /// Represents an enemy entity.
    /// </summary>
    public abstract class Enemy : Entity
    {
        /// <summary>
        /// Spawn enemies in a rectangle around the hero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="heroX">Hero origin X coordinate.</param>
        /// <param name="heroY">Hero origin Y coordinate.</param>
        /// <param name="radius">Half-side of a square around player in which the enemy will spawn.</param>
        /// <returns>A newly spawned Enemy.</returns>
        static public T Spawn<T>(double heroX, double heroY, double radius) where T : Enemy, new()
        {
            var rand = new Random();
            double xpos, ypos;
            const double centerRandomVar = 0.5;
            double r = radius * 2;
            xpos = heroX + (rand.NextDouble() - centerRandomVar) * r;
            ypos = heroY + (rand.NextDouble() - centerRandomVar) * r;

            Log.Debug($"Spawning: {typeof(T)} at ({xpos}, {ypos}) ; player position ({heroX}, {heroY})");

            return new T() {
                Xpos = xpos,
                Ypos = ypos
            };
        }
    }
}
