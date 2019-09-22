using System.Collections.Generic;
using doodLbot.Equipment;

namespace doodLbot.Logic
{
    /// <summary>
    /// Represents game design parameters.
    /// </summary>
    public static class Design
    {
        public static double ProjectileSpeed { get; private set; }
        public static double HeroSpeed { get; private set; }
        public static double BackwardsSpeedRatio { get; private set; }
        public static double RotateAmount { get; private set; }
        public static double HeroRadiusSize { get; private set; }
        public static double ProjectileRadiusSize { get; private set; }
        public static double FireCooldown { get; private set; }
        public static double ShootElementCooldown { get; private set; }
        public static double ProjectileDamage { get; private set; }

        public static double HeroStartX { get; private set; }
        public static double HeroStartY { get; private set; }

        public static double EnemyRadiusSize { get; private set; }
        public static double EnemySpeed { get; private set; }
        public static double SpawnRange { get; private set; }
        public static double SpawnInterval { get; private set; }

        public static double MapWidth { get; private set; }
        public static double MapHeight { get; private set; }
        public static double TickRate { get; private set; }

        public static (double X, double Y) MapSize => (MapWidth, MapHeight);

        public static IDictionary<string, Gear> GearDict { get; private set; }

        public static int CostShoot;
        public static int CostBranching;
        public static int CostIdle;
        public static int CostTarget;
        public static int CostIsNear;

        public static double Delta { get; set; }

        static Design()
        {
            TickRate = 30;

            // All speeds are calculated relative to the 50 tickrate
            var adjust = 50 / TickRate;
            Delta = 1;

            ProjectileSpeed = 6 * adjust;
            HeroSpeed = 5 * adjust;
            BackwardsSpeedRatio = 1.0 / 3.0;
            EnemySpeed = 2 * adjust;
            FireCooldown = 0.2;
            ShootElementCooldown = 0.4;
            RotateAmount = 0.065 * adjust;

            SpawnRange = 600;
            SpawnInterval = 5;

            ProjectileDamage = 30;

            MapWidth = 1900;
            MapHeight = 1000;

            HeroStartX = MapWidth / 2;
            HeroStartY = MapHeight / 2;

            EnemyRadiusSize = 20;
            HeroRadiusSize = 30;
            ProjectileRadiusSize = 10;

            CostShoot = 50;
            CostBranching = 100;
            CostIdle = 5;
            CostTarget = 80;
            CostIsNear = 70;

            PopulateGearList();
        }

        private static void PopulateGearList()
        {
            GearDict = new Dictionary<string, Gear>();
            var item = new Armor("hoverboard", 40, 5)
            {
                IsVisible = true
            };
            GearDict.Add("hoverboard", item);
            GearDict.Add("hoverboard2", new Armor("hoverboard2", 1, 2));
            GearDict.Add("hoverboard3", new Armor("hoverboard3", 4, 5));
        }
    }
}
