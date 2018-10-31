using doodLbot.Logic;
using Microsoft.AspNetCore.SignalR;
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

        public Task GameStateUpdated(GameStateUpdate update)
        {
            this.game.UpdateState(update);
            return Task.CompletedTask;
        }

        // TODO remove, this is a communication test
        public Task SendMessage(string user, string message)
            => this.Clients.All.SendAsync("ReceiveMessage", user, message);

        public Task SendUpdatesToClient(GameState update)
            => this.Clients.All.SendAsync("GameStateUpdateRecieved", update);
    }
}
