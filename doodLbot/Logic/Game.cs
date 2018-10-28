using doodLbot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace doodLbot.Logic
{
    public class Game
    {
        static public readonly int TickRate = 60;
        static public TimeSpan RefreshTimeSpan 
            => TimeSpan.FromMilliseconds(1 / TickRate);

        static private void UpdateCallback(object _)
        {
            var game = _ as Game;

            // Update game state
        }


        private readonly Hero hero;
        private readonly List<Enemy> enemies;
        private readonly Timer ticker;


        public Game()
        {
            this.hero = new Hero();
            this.enemies = new List<Enemy>();
            this.ticker = new Timer(UpdateCallback, this, RefreshTimeSpan, RefreshTimeSpan);
        }


        public GameState GetState() 
            => new GameState(hero, enemies);
    }
}
