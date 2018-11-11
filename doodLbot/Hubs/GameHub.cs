using doodLbot.Common;
using doodLbot.Entities.CodeElements;
using doodLbot.Logic;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System;
using System.Threading.Tasks;

namespace doodLbot.Hubs
{
    public class GameHub : Hub
    {
        private readonly Game game;

        public GameHub(Game game)
        {
            this.game = game;
        }

        public Task UpdateGameState(GameStateUpdate update)
        {
            this.game.UpdateControls(update);
            return Task.CompletedTask; 
        }

        // TODO remove, this is a communication test
        public Task SendMessage(string user, string message)
        {
            game.SpawnEnemy(doodLbot.Logic.Design.SpawnRange);
            return this.Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public Task SendCodeUpdate(BehaviourAlgorithm alg)
        {
            return this.Clients.All.SendAsync("UpdateCodeBlocks", alg);
        }

        // TODO well these aren't used because Hub interface doesnt know about them.
        // we just manually send them from Game.cs
        public Task SendUpdatesToClient(GameState update)
            => this.Clients.All.SendAsync("GameStateUpdateRecieved", update);

        public Task AlgorithmUpdated(BehaviourAlgorithm algorithm)
        {
            this.game.GameState.Hero.Algorithm = algorithm;
            return Task.CompletedTask;
        }
    }
}
