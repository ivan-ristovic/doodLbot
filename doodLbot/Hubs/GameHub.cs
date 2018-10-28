using doodLbot.Entities;
using doodLbot.Logic;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace doodLbot.Hubs
{
    public class GameHub : Hub
    {
        static public readonly int TickRate = 50;
        static public TimeSpan RefreshTimeSpan => TimeSpan.FromMilliseconds(1000 / TickRate);

        static private void UpdateCallback(object _)
        {
            var game = _ as GameHub;

            game.hero.Move();
            foreach (Enemy enemy in game.enemies)
                enemy.Move();

            // Ugly solution, leave it for now until I create an AsyncExecutor class
            game.Clients.All.SendAsync("StateUpdated", game.GameState).GetAwaiter().GetResult();
        }


        public GameState GameState => new GameState(this.hero, this.enemies);

        private readonly Hero hero;
        private readonly List<Enemy> enemies;
        private readonly Timer ticker;


        public GameHub()
        {
            this.hero = new Hero();
            this.enemies = new List<Enemy>();
            this.ticker = new Timer(UpdateCallback, this, RefreshTimeSpan, RefreshTimeSpan);
        }
        

        public Task GameStateUpdated(GameStateUpdate update)
        {
            // TODO 
            return Task.CompletedTask;
        }
    }
}
