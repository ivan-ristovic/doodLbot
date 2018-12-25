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

        [JsonProperty("id")]
        public int Id;

        [JsonIgnore]
        public bool HasCodeChanged { get; set; }

        [JsonIgnore]
        public bool HasGearChanged { get; set; }

        public IReadOnlyCollection<Gear> Gear => this.gear;

        public BehaviourAlgorithm Algorithm { get; set; } = new BehaviourAlgorithm();

        private readonly List<Gear> gear = new List<Gear>();
        
        private ConcurrentHashSet<Projectile> projectiles = new ConcurrentHashSet<Projectile>();

        public CodeStorage CodeInventory { get; private set; }

        public EquipmentStorage EquipmentInventory { get; private set; }

        private RateLimiter ShootRateLimiter;

        private readonly Controls controls;

        private double baseHp;
        private double baseSpeed;
        private double baseDamage;

        public Hero(int id, double x, double y, CodeStorage codeInventory, EquipmentStorage equipmentInventory) 
            : base(x: x, y: y)
        {
            HasCodeChanged = true; // TODO tmp, should be false when shop works
            HasGearChanged = true;
            this.Id = id;
            this.CodeInventory = codeInventory;
            this.EquipmentInventory = equipmentInventory;
            this.Points = 0;
            this.baseSpeed = Design.HeroSpeed;
            this.baseDamage = this.Damage;
            this.baseHp = this.Hp;
            this.ShootRateLimiter = new RateLimiter(Design.FireCooldown);
            this.controls = new Controls(ConsoleKey.Spacebar, ConsoleKey.W, ConsoleKey.S, ConsoleKey.A, ConsoleKey.D);
            CalculateStatsFromGear();
        }

        public void CalculateStatsFromGear()
        {
            this.Speed = this.baseSpeed;
            this.Damage = this.baseDamage;
            this.Hp = this.baseHp;
            foreach(var g in gear){
                switch(g)
                {
                    case Armor armor:
                        this.Speed += armor.Speed;
                        this.baseHp += armor.Hp;
                        break;
                    case Weapon weapon:
                        this.Damage += weapon.Damage;
                        break;
                }    
            }
        }

        /// <summary>
        /// TODO: this shouldn't be public when 
        /// we build a store
        /// </summary>
        /// <param name="g"></param>
        private void AddGear(Gear g)
        {
            // HasGearChanged = true;
            gear.Add(g);
            CalculateStatsFromGear();
        }

        /// <summary>
        /// TODO: this shouldn't be public when finished,
        /// now it's public because we don't have a shop
        /// </summary>
        /// <param name="g"></param>
        private void RemoveGear(Gear g)
        {
            // HasGearChanged = true;
            gear.Remove(g);
            CalculateStatsFromGear();
        }

        public void BuyGear(string name)
        {
            if (EquipmentInventory.ItemExists(name, out int cost)){
                var item = EquipmentInventory.BuyItem(name);
                AddGear(item);
            }
        }

        public void SellGear(string name)
        {
            if (EquipmentInventory.ItemExists(name, out int cost)){
                this.Points += cost;
                var item = EquipmentInventory.SellItem(name);
                RemoveGear(item);
            }
        }

        public void BuyCode(string name)
        {
            HasCodeChanged = true;
            if (CodeInventory.ItemExists(name, out int cost)){
                this.Points -= cost;
                var item = CodeInventory.BuyItem(name);
            }
        }

        public void SellCode(string name)
        {
            HasCodeChanged = true;
            if (CodeInventory.ItemExists(name, out int cost)){
                this.Points += cost;
                CodeInventory.SellItem(name);
            }
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
