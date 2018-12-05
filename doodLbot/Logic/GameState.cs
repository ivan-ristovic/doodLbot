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
        public ConcurrentHashSet<Hero> Heroes { get; }

        [JsonProperty("enemies")]
        public IReadOnlyCollection<Enemy> Enemies { get; private set; }

        [JsonProperty("projectiles")]
        public IReadOnlyCollection<Projectile> Projectiles => this.Hero.Projectiles;
//       [JsonProperty("heroProjectiles")]
//       public IReadOnlyCollection<Projectile> HeroProjectiles => this.Heroes.SelectMany(h => h.Projectiles).ToList().AsReadOnly();

        [JsonProperty("enemyProjectiles")]
        public IReadOnlyCollection<Projectile> EnemyProjectiles { get; private set; }

        /// <summary>
        /// Constructs a new GameState object containing the hero and spawned enemies.
        /// </summary>
        /// <param name="heroes"></param>
        /// <param name="enemies"></param>
        public GameState(IReadOnlyCollection<Hero> heroes, IReadOnlyCollection<Enemy> enemies, IReadOnlyCollection<Projectile> enemyProjectiles)
        {
            // TODO: milana: Change to ConcurrentHashSet
            Hero playerOne = heroes.Single(hero => hero.id == 1);
            this.Hero = playerOne;

            this.Enemies = enemies;
            this.EnemyProjectiles = enemyProjectiles;
        }
    }
}
