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
        public static async Task SendUpdatesToClients(this IHubContext<GameHub> hub, GameState update)
            => await hub.Clients.All.SendAsync("StateUpdate", update);

        /// <summary>
        ///                                 TODO
        /// </summary>
        /// <param name="hub"></param>
        /// <param name="alg"></param>
        /// <returns></returns>
        // TODO this should be changed when multiplayer happens, because each hero
        // will have a different algorithm
        public static async Task SendCodeUpdate(this IHubContext<GameHub> hub, BehaviourAlgorithm alg)
            => await hub.Clients.All.SendAsync("UpdateCodeBlocks", alg);
    }
}
