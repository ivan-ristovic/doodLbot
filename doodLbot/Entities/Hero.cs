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

        public CodeStorage CodeInventory { get; set; }

        public EquipmentStorage EquipmentInventory { get; set; }

        private RateLimiter ShootRateLimiter;

        private readonly Controls controls;

        public Hero(double x, double y, CodeStorage codeInventory, EquipmentStorage equipmentInventory) 
        : base(x, y)
        {
            this.CodeInventory = codeInventory;
            this.EquipmentInventory = equipmentInventory;
            this.Points = 0;
            this.Speed = Design.HeroSpeed;
            this.ShootRateLimiter = new RateLimiter(Design.FireCooldown);
            this.controls = new Controls(ConsoleKey.Spacebar, ConsoleKey.W, ConsoleKey.S, ConsoleKey.A, ConsoleKey.D);
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

        public bool TryFire(double speed, double damage)
        {
            if (!this.ShootRateLimiter.IsCooldownActive()) {
                this.Fire(speed, damage);
                return true;
            }
            return false;
        }

        private void Fire(double speed, double damage)
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

        internal void UpdateControls(ConsoleKey key, bool isDown)
        {
            this.controls.OnKey(key, isDown);
        }

        internal void UpdateStateWithControls(double delta)
        {
            double rotationAmount = Design.RotateAmount * delta;

            if (this.controls.IsFire)
            {
                this.TryFire(Design.ProjectileSpeed, Design.ProjectileDamage);
            }
            double velocity = this.Speed * delta;
            if (this.controls.IsForward)
            {
                this.Xvel = Math.Cos(this.Rotation) * velocity;
                this.Yvel = Math.Sin(this.Rotation) * velocity;
            }
            if (this.controls.IsBackward)
            {
                velocity *= Design.BackwardsSpeedRatio;
                this.Xvel = -Math.Cos(this.Rotation) * velocity;
                this.Yvel = -Math.Sin(this.Rotation) * velocity;
            }
            if (!this.controls.IsForward && !this.controls.IsBackward)
            {
                this.Xvel = 0;
                this.Yvel = 0;
            }
            if (this.controls.IsLeft)
            {
                this.Rotation -= rotationAmount;
            }
            if (this.controls.IsRight)
            {
                this.Rotation += rotationAmount;
            }
        }
    }
}
