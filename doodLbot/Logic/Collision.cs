using System.Collections.Generic;
using doodLbot.Entities;

namespace doodLbot.Logic
{
    /// <summary>
    /// Contains methods which help with collision checking.
    /// </summary>
    public static class CollisionCheck
    {
        /// <summary>
        /// Gets all the collisions between the two entity collections.
        /// </summary>
        /// <param name="colliders1"></param>
        /// <param name="colliders2"></param>
        /// <returns>A collection of collision objects.</returns>
        public static IReadOnlyList<(Entity, Entity)> GetCollisions(IEnumerable<Entity> colliders1, IEnumerable<Entity> colliders2)
        {
            var collides = new List<(Entity Collider1, Entity Collider2)>();
            // TODO these radiuses are sometimes hero, figure out how to assign them correctly
            // (example: see if entity is hero, then hero radius, etc..)
            var radius1 = Design.EnemyRadiusSize;
            var radius2 = Design.ProjectileRadiusSize;

            foreach (var collider1 in colliders1) {
                foreach (var collider2 in colliders2) {
                    if (IsColliding(collider2, collider1, radius1, radius2)) {
                        collides.Add((collider1, collider2));
                    }
                }
            }

            /*
            if (collides.Count > 0)
            {
                Log.Debug("Collisions: {CollisionCount}", collides.Count);
            }
            */

            return collides.AsReadOnly();


            bool IsColliding(Entity collider1, Entity collider2, double r1, double r2)
            {
                return DistanceSquared(collider2.Xpos, collider2.Ypos, collider1.Xpos, collider1.Ypos) < (r1 + r2) * (r1 + r2);
            }
        }

        public static double DistanceSquared(double x1, double y1, double x2, double y2)
            => (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
    }
}
