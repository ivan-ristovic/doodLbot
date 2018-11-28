using doodLbot.Entities.CodeElements;
using doodLbot.Logic;

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

            // TODO change when multiplayer is done
            return this.Clients.All.SendAsync("InitClient", data);
        }

        public Task SendCodeUpdate(BehaviourAlgorithm alg)
        {
            // TODO this should be changed when multiplayer happens, because each hero
            // will have a different algorithm
            return this.Clients.All.SendAsync("UpdateCodeBlocks", alg);
        }

        public Task AlgorithmUpdated(string json)
        {
            this.game.GameState.Hero.Algorithm = DynamicJsonDeserializer.ToBehaviourAlgorithm(json);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Internal game state update action. Forces the clients to update their game state on
        /// each tick.
        /// </summary>
        /// <param name="update"></param>
        internal Task SendUpdatesToClient(GameState update)
            => this.Clients.All.SendAsync("GameStateUpdateRecieved", update);
    }
}
