using doodLbot.Logic;

using System;

namespace doodLbot.Entities
{
    public abstract class Enemy : Entity
    {
        static public T Spawn<T>() where T : Enemy, new()
        {
            var rand = new Random();
            double xpos, ypos;
            
            if (rand.Next() % 2 == 0) {
                xpos = (rand.Next() % 2 == 0) ? 0 : Game.MapSize.X;
                ypos = rand.NextDouble() * Game.MapSize.Y;
            } else {
                xpos = rand.NextDouble() * Game.MapSize.X;
                ypos = (rand.Next() % 2 == 0) ? 0 : Game.MapSize.Y;
            }

            // TODO set velocity

            return new T() {
                Xpos = xpos,
                Ypos = ypos
            };
        }
    }
}
