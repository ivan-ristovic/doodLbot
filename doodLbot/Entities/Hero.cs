using doodLbot.Common;
using doodLbot.Entities.CodeElements;
using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace doodLbot.Entities
{
    public class Hero : Entity
    {
        // These will remain lists of objects until the classes are made
        [JsonProperty("gear")]
        public IReadOnlyList<object> Gear => this.gear.AsReadOnly();
        [JsonProperty("modules")]
        public IReadOnlyList<object> Modules => this.modules.AsReadOnly();
        [JsonProperty("projectiles")]
        public IReadOnlyList<Projectile> Projectiles => this.projectiles.ToList().AsReadOnly();

        //public BehaviourAlgorithm Algorithm { get; } = new BehaviourAlgorithm();

        private readonly List<object> gear = new List<object>();
        private readonly List<object> modules = new List<object>();
        private ConcurrentHashSet<Projectile> projectiles = new ConcurrentHashSet<Projectile>();


        public Hero() : base()
        {

        }

        public Hero(double x, double y) : base(x, y)
        {

        }

        public void Fire()
            => this.projectiles.Add(new Projectile(this.Xpos, this.Ypos, this.Rotation));
        
        public bool TryRemoveProjectile(Projectile p)
        {
            return projectiles.TryRemove(p);
        }
    }
}
