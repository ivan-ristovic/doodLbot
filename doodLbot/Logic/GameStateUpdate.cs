using System;
using System.Collections.Generic;
using System.Linq;

namespace doodLbot.Logic
{
    public class GameStateUpdate
    {
        public TimeSpan TimeSinceLastUpdate { get; }
        public IReadOnlyList<ConsoleKey> KeyPresses => this.keyPresses.AsReadOnly();
        public IReadOnlyList<object> ActionsPerformed => this.actionsPerformed.AsReadOnly();

        private readonly List<ConsoleKey> keyPresses;
        private readonly List<object> actionsPerformed;


        public GameStateUpdate(IEnumerable<ConsoleKey> keys, IEnumerable<object> actions)
        {
            this.keyPresses = keys.ToList();
            this.actionsPerformed = actions .ToList();
        }
    }
}
