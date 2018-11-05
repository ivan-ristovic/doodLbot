using System;
using System.Collections.Generic;
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
        private static double distanceSquared(double x1, double y1, double x2, double y2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        }

        private static bool isColliding(double x1, double y1, double x2, double y2, double r1, double r2)
        {
            //Log.Debug($"Collision: ({distanceSquared(x1, y1, x2, y2)}) ; ({(r1 + r2) * (r1 + r2)})");
            return distanceSquared(x1,y1,x2,y2) < (r1+r2)*(r1+r2);
        }

        public static Collision[] getCollisions(IEnumerable<Entity> projectiles, IEnumerable<Entity> enemies)
        {

            List<Collision> collides = new List<Collision>();
            const double radius1 = 20;
            const double radius2 = 10;

            foreach (var projectile in projectiles)
            {
                foreach (var enemy in enemies)
                {
                    if (isColliding(enemy.Xpos, enemy.Ypos, projectile.Xpos, projectile.Ypos, radius1, radius2))
                    {
                        collides.Append<Collision>(new Collision(enemy, projectile));
                        Log.Debug($"Collision: ({enemy.Xpos}, {enemy.Xpos}) ; ({projectile.Xpos}, {projectile.Ypos})");
                    }
                }
            }

            return collides.ToArray(); ;
        }
    }
}
