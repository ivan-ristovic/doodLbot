using doodLbot.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doodLbot.Logic
{
    // used to send game data to client
    public sealed class GameState
    {
        // maybe these AsReadOnly conversions are unnecessary overhead?
        [JsonProperty("hero")]
        public Hero Hero { get; }

        [JsonProperty("enemies")]
        public IReadOnlyList<Enemy> Enemies => this.enemies.AsReadOnly();

        [JsonProperty("projectiles")]
        public IReadOnlyList<Projectile> Projectiles => this.projectiles.AsReadOnly();

        private readonly List<Enemy> enemies;

        private readonly List<Projectile> projectiles;

        public GameState(Hero hero, IEnumerable<Enemy> enemies, IEnumerable<Projectile> projectiles)
        {
            this.Hero = hero;
            this.enemies = enemies.ToList();
            // System.ArgumentException: 'Destination array was not long enough. 
            // Check the destination index, length, and the array's lower bounds.'
            // It happens to us beacuse its not thread safe.
            this.projectiles = projectiles.ToList();
        }
    }
}
