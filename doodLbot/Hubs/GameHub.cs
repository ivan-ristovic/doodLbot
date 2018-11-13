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
        /// Updates player's controls. Received from the frontend.
        /// </summary>
        /// <param name="update"></param>
        public Task UpdateGameState(GameStateUpdate update)
        {
            this.game.UpdateControls(update);
            return Task.CompletedTask;
        }

        // TODO remove, this is a communication test
        /// <summary>
        /// DO NOT USE THIS.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        public Task SendMessage(string user, string message)
        {
            this.game.SpawnEnemy(Design.SpawnRange);
            return this.Clients.All.SendAsync("ReceiveMessage", user, message);
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
                mapSize = Design.MapSize
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
