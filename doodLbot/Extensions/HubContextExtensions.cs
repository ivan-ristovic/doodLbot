using System.Threading.Tasks;
using doodLbot.Common;
using doodLbot.Entities.CodeElements;
using doodLbot.Hubs;
using doodLbot.Logic;
using Microsoft.AspNetCore.SignalR;

namespace doodLbot.Extensions
{
    /// <summary>
    /// Extensions for gamehub.
    /// Used for C# => client direction.
    /// </summary>
    public static class HubContextExtensions
    {
        /// <summary>
        /// Internal game state update action. Forces the clients to update their game state on
        /// each tick.
        /// </summary>
        /// <param name="update"></param>
        public static void SendUpdatesToClients(this IHubContext<GameHub> hub, GameState update)
        {
            //async.Execute(hub.Clients.All.SendAsync("StateUpdate", update));
            hub.Clients.All.SendAsync("StateUpdate", update);

        }

        // Sends 
        public static Task SendCodeUpdate(this IHubContext<GameHub> hub, BehaviourAlgorithm alg)
        {
            // TODO this should be changed when multiplayer happens, because each hero
            // will have a different algorithm
            return hub.Clients.All.SendAsync("UpdateCodeBlocks", alg);
        }
    }
}
