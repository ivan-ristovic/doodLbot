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
        [JsonProperty("heroes")]
        public IReadOnlyCollection<Hero> Heroes { get; }

        [JsonProperty("enemies")]
        public IReadOnlyCollection<Enemy> Enemies { get; private set; }

        [JsonProperty("projectiles")]
        public IReadOnlyCollection<Projectile> Projectiles { get; private set; }

        [JsonProperty("enemyProjectiles")]
        public IReadOnlyCollection<Projectile> EnemyProjectiles { get; private set; }

        /// <summary>
        /// Constructs a new GameState object containing the hero and spawned enemies.
        /// </summary>
        /// <param name="heroes"></param>
        /// <param name="enemies"></param>
        public GameState(IReadOnlyCollection<Hero> heroes, IReadOnlyCollection<Enemy> enemies, IReadOnlyCollection<Projectile> enemyProjectiles)
        {
            this.Heroes = heroes.OrderBy(x=>x.Id).ToList();
            this.Projectiles = heroes.SelectMany(h => h.Projectiles).ToList().AsReadOnly();
            this.Enemies = enemies;
            this.EnemyProjectiles = enemyProjectiles;
        }
    }
}
