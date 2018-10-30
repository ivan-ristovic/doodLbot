using doodLbot.Common;
using doodLbot.Entities;
using doodLbot.Logic;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// https://docs.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/hubs-api-guide-server#how-to-handle-connection-lifetime-events-in-the-hub-class
namespace doodLbot.Hubs
{
    public class GameHub : Hub
    {
        static public readonly (int X, int Y) CanvasSize = (800, 600);
        static public readonly int TickRate = 50;
        static public TimeSpan RefreshTimeSpan => TimeSpan.FromMilliseconds(1000.0 / TickRate);

        static private readonly AsyncExecutor _async = new AsyncExecutor();
        /*You should not use async void for Hub methods, as it create a race condition.Without a Task return 
         * from the Hub method we don't know when the actual async operation completes. 
         * As soon as it hits the first await, the function "returns" (having scheduled the async operation) and we think the 
         * Hub method has completed and dispose it. Change the function to be async Task and the issue should be resolved.*/
        static private void UpdateCallback(object _)
        {
            var game = _ as GameHub;

            game.hero.Move();
            foreach (Enemy enemy in game.enemies)
                enemy.Move();
            _async.Execute(game.Clients.All.SendAsync("StateUpdated", game.GameState));
        }

        // testing communication
        // Currently only calling this method from client.
        public Task SendMessage(string user, string message)
            => this.Clients.All.SendAsync("ReceiveMessage", user, message);

        // This is how we should use these methods, i think.
        public Task ClientToServer(string message)
        {
            return this.Clients.Caller.SendAsync("ServerToClientTemplate", "This is server, hi! I've heared from you = " + message);
        }

        public GameState GameState => new GameState(this.hero, this.enemies);

        private readonly Hero hero;     // Change this to ConcurrentHashSet for the multiplayer
        private readonly ConcurrentHashSet<Enemy> enemies;
        private readonly Timer ticker;

        // I don't think we should look at the hub object as something that lives for a long time.
        // So, lets not forward 'this' to the Timer. 
        // Instead it should be something else, maybe a static class with needed fields.
        public GameHub()
        {
            this.hero = new Hero();
            this.enemies = new ConcurrentHashSet<Enemy>();
            // TODO UNCOMMENT when its all connected, now its crashing because of this being disposed.
            //this.ticker = new Timer(UpdateCallback, this, RefreshTimeSpan, RefreshTimeSpan);
        }
        

        public Task GameStateUpdated(GameStateUpdate update)
        {
            // TODO 
            return Task.CompletedTask;
        }
    }
}
