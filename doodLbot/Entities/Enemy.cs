using doodLbot.Hubs;

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
                xpos = (rand.Next() % 2 == 0) ? 0 : GameHub.CanvasSize.X;
                ypos = rand.NextDouble() * GameHub.CanvasSize.Y;
            } else {
                xpos = rand.NextDouble() * GameHub.CanvasSize.X;
                ypos = (rand.Next() % 2 == 0) ? 0 : GameHub.CanvasSize.Y;
            }

            // TODO set velocity

            return new T() {
                Xpos = xpos,
                Ypos = ypos
            };
        }
    }
}
