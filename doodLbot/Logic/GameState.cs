using doodLbot.Common;
using doodLbot.Entities;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace doodLbot.Logic
{
    /// <summary>
    /// Represent a game state that will be sent to clients so they can update the game view.
    /// </summary>
    public sealed class GameState
    {
        [JsonProperty("hero")]
        public Hero Hero { get; }

        [JsonProperty("enemies")]
        public IReadOnlyList<Enemy> Enemies => this.enemies.AsReadOnly();

        [JsonProperty("projectiles")]
        public IReadOnlyList<Projectile> Projectiles => this.projectiles.AsReadOnly();

        private readonly List<Enemy> enemies;
        private readonly List<Projectile> projectiles = new List<Projectile>();


        /// <summary>
        /// Constructs a new GameState object containing the hero and spawned enemies.
        /// </summary>
        /// <param name="heroes"></param>
        /// <param name="enemies"></param>
        public GameState(ConcurrentHashSet<Hero> heroes, IEnumerable<Enemy> enemies)
        {
            Hero playerOne = heroes.Single(hero => hero.id == 1);
            // TODO: milana: Add playerTwo
            this.Hero = playerOne;
            this.enemies = enemies.ToList();
            this.projectiles.AddRange(playerOne.Projectiles);
        }
    }
}
