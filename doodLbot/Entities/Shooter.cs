using doodLbot.Common;
using doodLbot.Logic;

using Newtonsoft.Json;

using System.Collections.Generic;

namespace doodLbot.Entities
{
    /// <summary>
    /// Represents an Enemy with extra HP that moves towards a hero and tries to collide with him.
    /// </summary>
    public class Shooter : Enemy
    {
        [JsonProperty("projectiles")]
        public IReadOnlyCollection<Projectile> Projectiles => this.projectiles;

        private readonly ConcurrentHashSet<Projectile> projectiles = new ConcurrentHashSet<Projectile>();


        public Shooter() : base(speed: Design.EnemySpeed, hp: 50, damage: 25)
        {

        }


        public void Fire(double speed, double damage)
        {
            this.projectiles.Add(new Projectile(this.Xpos, this.Ypos, this.Rotation, speed, damage));
        }

        public bool TryRemoveProjectile(Projectile p)
        {
            return this.projectiles.TryRemove(p);
        }
    }
}
