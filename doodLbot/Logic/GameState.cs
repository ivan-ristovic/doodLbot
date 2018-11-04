using doodLbot.Entities;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace doodLbot.Logic
{
    // used to send game data to client
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


        public GameState(Hero hero, IEnumerable<Enemy> enemies)
        {
            this.Hero = hero;
            this.enemies = enemies.ToList();
            this.projectiles.AddRange(hero.Projectiles);
        }
    }
}
