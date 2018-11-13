using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using doodLbot.Entities;
using Serilog;

namespace doodLbot.Logic
{
    public class Collision
    {
        public Entity collider1;
        public Entity collider2;

        public Collision(Entity collider1, Entity collider2)
        {
            this.collider1 = collider1;
            this.collider2 = collider2;
        }
    }

    public static class CollisionCheck
    {
        private static double DistanceSquared(double x1, double y1, double x2, double y2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        }

        private static bool IsColliding(Entity collider1, Entity collider2, double r1, double r2)
        {
            return DistanceSquared(collider2.Xpos, collider2.Ypos, collider1.Xpos, collider1.Ypos) < (r1 + r2) * (r1 + r2);
        }

        public static IReadOnlyList<Collision> GetCollisions(IEnumerable<Entity> colliders1, IEnumerable<Entity> colliders2)
        {
            var collides = new List<Collision>();
            // TODO thees radiuses are sometimes hero, figure out how to assign them correctly
            // (example: see if entity is hero, then hero radius, etc..)
            double radius1 = Design.EnemyRadiusSize;
            double radius2 = Design.ProjectileRadiusSize;

            foreach (var collider1 in colliders1)
            {
                foreach (var collider2 in colliders2)
                {
                    if (IsColliding(collider2, collider1, radius1, radius2))
                    {
                        collides.Add(new Collision(collider1, collider2));
                    }
                }
            }

            if (collides.Count > 0)
                Log.Debug($"Collisions: ({collides.Count})");

            return collides.AsReadOnly();
        }
    }
}
