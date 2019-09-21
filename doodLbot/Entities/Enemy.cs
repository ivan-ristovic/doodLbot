using System;
using System.Collections.Generic;
using doodLbot.Logic;

namespace doodLbot.Entities
{
    /// <summary>
    /// Represents an enemy entity.
    /// </summary>
    public abstract class Enemy : Entity
    {

        protected Enemy(double max_hp = 100, double hp = 100, double damage = 1, double speed = 0, double rotation = 0) : base(hp, max_hp, damage, speed, rotation)
        {

        }

        /// <summary>
        /// Spawn enemies in a rectangle around the hero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="heroX">Hero origin X coordinate.</param>
        /// <param name="heroY">Hero origin Y coordinate.</param>
        /// <param name="maxRadius">Half-side of a square around player in which the enemy will spawn.</param>
        /// <returns>A newly spawned Enemy.</returns>
        static public T Spawn<T>(double heroX, double heroY, double maxRadius, double minRadius) where T : Enemy, new()
        {
            var rand = new Random();
            double xpos, ypos;
            var r = maxRadius * 2;
            var deg = rand.NextDouble() * 2 * Math.PI;
            var dist = minRadius + (maxRadius - minRadius) * rand.NextDouble();
            var dx = Math.Cos(deg) * dist;
            var dy = Math.Sin(deg) * dist;

            xpos = heroX + dx;
            ypos = heroY + dy;

            xpos = Math.Min(xpos, Design.MapWidth);
            xpos = Math.Max(xpos, 0);
            ypos = Math.Min(ypos, Design.MapHeight);
            ypos = Math.Max(ypos, 0);
            // Log.Debug($"Spawning: {typeof(T)} at ({xpos}, {ypos}) ; player position ({heroX}, {heroY})");

            return new T()
            {
                Xpos = xpos,
                Ypos = ypos
            };
        }

        /// <summary>
        /// Direct this entity to move towards another entity using given speed.
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="withSpeed"></param>
        public void VelocityTowardsClosestEntity(IEnumerable<Entity> entities, double? withSpeed = null)
        {
            Entity goal = null;
            var minDistanceSquared = double.MaxValue;
            foreach (var e in entities)
            {
                var currentDistanceSquared = CollisionCheck.DistanceSquared(e.Xpos, e.Ypos, Xpos, Ypos);
                if (currentDistanceSquared < minDistanceSquared)
                {
                    goal = e;
                    minDistanceSquared = currentDistanceSquared;
                }
            }
            VelocityTowards(goal, withSpeed);
        }
    }
}
