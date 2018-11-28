using doodLbot.Equipment;
using System.Collections.Generic;

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

        static Design()
        {
            TickRate = 30;

            // all speeds are calculated relative to the 50 tickrate
            double adjust = 50 / TickRate;

            ProjectileSpeed = 6 * adjust;
            HeroSpeed = 5 * adjust;
            BackwardsSpeedRatio = 1.0/3.0;
            EnemySpeed = 2 * adjust;
            FireCooldown = 0.2;
            ShootElementCooldown = 0.4;
            RotateAmount = 0.065 * adjust;

            HeroStartX = 300;
            HeroStartY = 300;

            SpawnRange = 300;
            SpawnInterval = 5;

            ProjectileDamage = 30;

            MapWidth = 1900;
            MapHeight = 1000;

            EnemyRadiusSize = 20;
            HeroRadiusSize = 30;
            ProjectileRadiusSize = 10;

            PopulateGearList();
        }

        private static void PopulateGearList()
        {
            GearDict = new Dictionary<string, Gear>();
            GearDict.Add("hoverboard", new Armor("hoverboard", 40, 5));
            GearDict.Add("hoverboard2", new Armor("hoverboard", 1, 2));
            GearDict.Add("hoverboard3", new Armor("hoverboard", 4, 5));
        }
    }
}
