using doodLbot.Common;
using doodLbot.Entities.CodeElements;
using doodLbot.Equipment;
using doodLbot.Logic;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System;

namespace doodLbot.Entities
{
    public class Hero : Entity
    {
        [JsonProperty("projectiles")]
        public IReadOnlyCollection<Projectile> Projectiles => this.projectiles;

        [JsonProperty("pts")]
        public int Points;

        public IReadOnlyCollection<Gear> Gear => this.gear;

        public BehaviourAlgorithm Algorithm { get; set; } = new BehaviourAlgorithm();

        private readonly List<Gear> gear = new List<Gear>();
        private ConcurrentHashSet<Projectile> projectiles = new ConcurrentHashSet<Projectile>();

        public Hero(double x, double y) : base(x, y)
        {
            Points = 0;
            this.Speed = Design.HeroSpeed;
        }

        public void AddGear(Gear g)
        {
            gear.Add(g);
            if (g is Armor)
            {
                var armor = g as Armor;
                this.Speed += armor.Speed;
            }
            else
            {

            }
        }

        public bool RemoveGear(Gear g)
        {
            return gear.Remove(g);
        }

        protected override void OnMove()
        {
            var map = Logic.Design.MapSize;
            if (this.IsOutsideBounds(map))
            {
                this.Xpos = Math.Max(0, this.Xpos);
                this.Ypos = Math.Max(0, this.Ypos);
                this.Xpos = Math.Min(map.X, this.Xpos);
                this.Ypos = Math.Min(map.Y, this.Ypos);
            }
        }

        public void Fire(double speed, double damage)
            => this.projectiles.Add(new Projectile(this.Xpos, this.Ypos, this.Rotation, speed, damage));
        
        public bool TryRemoveProjectile(Projectile p)
        {
            return this.projectiles.TryRemove(p);
        }

        public override void DecreaseHealthPoints(double value)
        {
            base.DecreaseHealthPoints(value);

            if (this.Hp == 0)
            {
                // TODO when hero's hp == 0 - GAME OVER
                // For now, we reset hp.
                this.Hp = 100;
            }
        }
    }
}
