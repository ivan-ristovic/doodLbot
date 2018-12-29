using doodLbot.Entities.CodeElements;
using doodLbot.Logic;

using Serilog;
using Microsoft.AspNetCore.SignalR;

using System.Threading.Tasks;

namespace doodLbot.Hubs
{
    /// <summary>
    /// Represents a game hub.
    /// </summary>
    public class GameHub : Hub
    {
        private readonly Game game;

        /// <summary>
        /// Constructs a new GameHub using a given Game.
        /// </summary>
        /// <param name="game"></param>
        public GameHub(Game game)
        {
            this.game = game;
        }

        /// <summary>
        /// Updates player's controls. Receieved from the frontend.
        /// </summary>
        /// <param name="update"></param>
        public Task UpdateGameState(GameStateUpdate update)
        {
            this.game.UpdateControls(update);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Used for quick client->server testing..
        /// </summary>
        public Task TestingCallback()
        {
            this.game.SpawnEnemy(Design.SpawnRange);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Fired when client is ready for initial sync.
        /// </summary>
        /// <returns></returns>
        public Task ClientIsReady()
        {
            var data = new
            {
                algorithm = game.GameState.Hero.Algorithm,
                mapWidth = Design.MapSize.X,
                mapHeight = Design.MapSize.Y,
                tickRate = Design.TickRate,
                codeInventory = game.GameState.Hero.CodeInventory,
                equipmentInventory = game.GameState.Hero.EquipmentInventory
            };

            return this.Clients.Caller.SendAsync("InitClient", data);
        }

        public Task BuyGear(string name)
        {
            Log.Debug("server: buy gear");
            this.game.GameState.Hero.BuyGear(name);
            return Task.CompletedTask;
        }

        public Task SellGear(string name)
        {
            Log.Debug("server: sell gear");
            this.game.GameState.Hero.SellGear(name);
            return Task.CompletedTask;
        }

        public Task BuyCode(string name)
        {
            Log.Debug("server: buy code");
            this.game.GameState.Hero.BuyCode(name);
            return Task.CompletedTask;
        }

        public Task SellCode(string name)
        {
            Log.Debug("server: sell code");
            this.game.GameState.Hero.SellCode(name);
            return Task.CompletedTask;
        }

        public Task EquipItem(string name)
        {
            Log.Debug("server: equip code");
            this.game.GameState.Hero.EquipCode(name);
            return Task.CompletedTask;
        }

        public Task AlgorithmUpdated(string json)
        {
            this.game.GameState.Hero.Algorithm = DynamicJsonDeserializer.ToBehaviourAlgorithm(json);
            return Task.CompletedTask;
        }
    }
}
