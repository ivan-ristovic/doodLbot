using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using doodLbot.Common;
using doodLbot.Entities.CodeElements;
using doodLbot.Equipment;
using doodLbot.Logic;
using Newtonsoft.Json;

namespace doodLbot.Entities
{
    public class Hero : Entity
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("projectiles")]
        public IReadOnlyCollection<Projectile> Projectiles => projectiles;

        [JsonProperty("pts")]
        public int Points;

        [JsonProperty("id")]
        public int Id;

        [JsonIgnore]
        public bool IsAlive;

        [JsonIgnore]
        public DateTime TimeOfLastHeartbeat { get; set; }

        [JsonIgnore]
        public bool HasCodeChanged { get; set; }

        [JsonIgnore]
        public bool IsControlledByAlgorithm { get; set; }

        [JsonIgnore]
        public bool HasGearChanged { get; set; }

        [JsonIgnore]
        public IReadOnlyCollection<Gear> Gear => gear;

        [JsonProperty("gear")]
        public IReadOnlyCollection<Gear> VisibleGear => gear.Where(x => x.IsVisible).ToList();

        [JsonProperty("algorithm")]
        public BehaviourAlgorithm Algorithm { get; set; }

        private readonly List<Gear> gear = new List<Gear>();

        private readonly ConcurrentHashSet<Projectile> projectiles = new ConcurrentHashSet<Projectile>();

        [JsonProperty("codeInventory")]
        public CodeStorage CodeInventory { get; private set; }

        [JsonProperty("equipmentInventory")]
        public EquipmentStorage EquipmentInventory { get; private set; }

        private readonly RateLimiter ShootRateLimiter;

        private readonly Controls controls;
        private readonly Controls syntheticControls;


        private double baseHp;
        private readonly double baseSpeed;
        private readonly double baseDamage;
        private readonly Timer heartbeatTimer;

        public Hero(int id, double x, double y, CodeStorage codeInventory, EquipmentStorage equipmentInventory)
            : base(x: x, y: y)
        {
            HasCodeChanged = false;
            HasGearChanged = false;
            Algorithm = new BehaviourAlgorithm(this);
            Id = id;
            CodeInventory = codeInventory;
            EquipmentInventory = equipmentInventory;
            Points = 0;
            baseSpeed = Design.HeroSpeed;
            baseDamage = Damage;
            baseHp = Hp;
            ShootRateLimiter = new RateLimiter(Design.FireCooldown);
            controls = new Controls(ConsoleKey.Spacebar, ConsoleKey.W, ConsoleKey.S, ConsoleKey.A, ConsoleKey.D);
            syntheticControls = new Controls(ConsoleKey.Spacebar, ConsoleKey.W, ConsoleKey.S, ConsoleKey.A, ConsoleKey.D);
            CalculateStatsFromGear();

            // Configure a Timer for use
            heartbeatTimer = new Timer
            {
                Interval = 2000
            };
            heartbeatTimer.Elapsed += new ElapsedEventHandler(CheckAliveness);
            heartbeatTimer.Enabled = true;
            IsAlive = true;
            TimeOfLastHeartbeat = DateTime.Now;
        }

        public bool IsMoving => controls.IsMoving;

        public void WipeSyntheticControls()
        {
            syntheticControls.Wipe();
        }

        public void CheckAliveness(Object sender, ElapsedEventArgs eventArgs)
        {
            if (DateTime.Now - TimeOfLastHeartbeat > TimeSpan.FromSeconds(10))
            {
                IsAlive = false;
            }
        }

        public void CalculateStatsFromGear()
        {
            Speed = baseSpeed;
            Damage = baseDamage;
            Hp = baseHp;
            foreach (var g in gear)
            {
                switch (g)
                {
                    case Armor armor:
                        Speed += armor.Speed;
                        baseHp += armor.Hp;
                        break;
                    case Weapon weapon:
                        Damage += weapon.Damage;
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
            if (EquipmentInventory.ItemExists(name, out var cost))
            {
                if (Points < cost)
                {
                    return;
                }
                Points -= cost;
                var item = EquipmentInventory.BuyItem(name);
                AddGear(item);
            }
        }

        public void SellGear(string name)
        {
            if (EquipmentInventory.ItemExists(name, out var cost))
            {
                Points += cost;
                var item = EquipmentInventory.SellItem(name);
                RemoveGear(item);
            }
        }

        public void BuyCode(string name)
        {
            HasCodeChanged = true;
            if (CodeInventory.ItemExists(name, out var cost))
            {
                if (Points < cost)
                {
                    return;
                }
                Points -= cost;
                _ = CodeInventory.BuyItem(name);
            }
        }

        public void SellCode(string name)
        {
            HasCodeChanged = true;
            if (CodeInventory.ItemExists(name, out var cost))
            {
                Points += cost;
                CodeInventory.SellItem(name);
            }
        }

        public void EquipCode(string name)
        {
            HasCodeChanged = true;
            if (CodeInventory.ItemExists(name, out _))
            {
                var item = CodeInventory.BuyItem(name);
                CodeInventory.SellItem(name);
                Algorithm.Insert(item);
            }
        }

        protected override void OnMove()
        {
            var map = Logic.Design.MapSize;
            if (IsOutsideBounds(map))
            {
                Xpos = Math.Max(0, Xpos);
                Ypos = Math.Max(0, Ypos);
                Xpos = Math.Min(map.X, Xpos);
                Ypos = Math.Min(map.Y, Ypos);
            }
        }

        public bool TryFire(double speed, double damage)
        {
            if (!ShootRateLimiter.IsCooldownActive())
            {
                Fire(speed, damage);
                return true;
            }
            return false;
        }

        private void Fire(double speed, double damage)
        {
            projectiles.Add(new Projectile(Xpos, Ypos, Rotation, speed, damage));
        }

        public bool TryRemoveProjectile(Projectile p)
        {
            return projectiles.TryRemove(p);
        }

        public override void DecreaseHealthPoints(double value)
        {
            base.DecreaseHealthPoints(value);

            if (Hp == 0)
            {
                // TODO when hero's hp == 0 - GAME OVER
                // For now, we reset hp.
                Hp = 100;
            }
        }

        internal void UpdateControls(ConsoleKey key, bool isDown)
        {
            controls.OnKey(key, isDown);
        }

        internal void UpdateSyntheticControls(ConsoleKey key, bool isDown)
        {
            syntheticControls.OnKey(key, isDown);
        }

        internal void UpdateStateWithControls(double delta)
        {
            var controlWith = controls.IsMoving ? controls : syntheticControls;

            var rotationAmount = Design.RotateAmount * delta;

            if (controls.IsFire)
            {
                TryFire(Design.ProjectileSpeed * delta, Design.ProjectileDamage);
            }

            var velocity = Speed * delta;
            if (controlWith.IsForward)
            {
                Xvel = Math.Cos(Rotation) * velocity;
                Yvel = Math.Sin(Rotation) * velocity;
            }
            if (controlWith.IsBackward)
            {
                velocity *= Design.BackwardsSpeedRatio;
                Xvel = -Math.Cos(Rotation) * velocity;
                Yvel = -Math.Sin(Rotation) * velocity;
            }
            if (!controlWith.IsForward && !controlWith.IsBackward)
            {
                Xvel = 0;
                Yvel = 0;
            }
            if (controlWith.IsLeft)
            {
                Rotation -= rotationAmount;
            }
            if (controlWith.IsRight)
            {
                Rotation += rotationAmount;
            }
        }
    }
}
