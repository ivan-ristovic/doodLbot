using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace doodLbot.Logic
{
    /// <summary>
    /// Represent a game state update that is sent from the clients to the backend.
    /// </summary>
    public sealed class GameStateUpdate
    {
        public TimeSpan TimeSinceLastUpdate { get; }
        public IReadOnlyList<(ConsoleKey key, bool isDown)> KeyPresses => keyPresses.AsReadOnly();
        public IReadOnlyList<object> ActionsPerformed => actionsPerformed.AsReadOnly();
        public int Id { get; }

        private readonly List<(ConsoleKey key, bool isDown)> keyPresses;
        private readonly List<object> actionsPerformed;

        [JsonConstructor]
        public GameStateUpdate(int timeSinceLastSend, int[] keyPresses, object [] actions, int id)
        {
            this.keyPresses = keyPresses.Select((key) => ((ConsoleKey)Math.Abs(key), key >= 0)).ToList();
            actionsPerformed = actions.ToList();
            Id = id;
        }
    }
}
