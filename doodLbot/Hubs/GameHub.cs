using doodLbot.Common;
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
        static public readonly (int X, int Y) CanvasSize = (800, 600);
        static public readonly int TickRate = 50;
        static public TimeSpan RefreshTimeSpan => TimeSpan.FromMilliseconds(1000 / TickRate);

        static private readonly AsyncExecutor _async = new AsyncExecutor();

        static private void UpdateCallback(object _)
        {
            var game = _ as GameHub;

            game.hero.Move();
            foreach (Enemy enemy in game.enemies)
                enemy.Move();

            _async.Execute(game.Clients.All.SendAsync("StateUpdated", game.GameState));
        }


        public GameState GameState => new GameState(this.hero, this.enemies);

        private readonly Hero hero;     // Change this to ConcurrentHashSet for the multiplayer
        private readonly ConcurrentHashSet<Enemy> enemies;
        private readonly Timer ticker;


        public GameHub()
        {
            this.hero = new Hero();
            this.enemies = new ConcurrentHashSet<Enemy>();
            this.ticker = new Timer(UpdateCallback, this, RefreshTimeSpan, RefreshTimeSpan);
        }
        

        public Task GameStateUpdated(GameStateUpdate update)
        {
            // TODO 
            return Task.CompletedTask;
        }
    }
}
