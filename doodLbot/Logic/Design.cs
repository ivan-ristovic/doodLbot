using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doodLbot.Logic
{
    // holds all game design parameters
    public static class Design
    {
        public static double ProjectileSpeed { get; private set; }
        public static double HeroSpeed { get; private set; }
        public static double EnemySpeed { get; private set; }
        public static double FireCooldown { get; private set; }
        public static double RotateAmount { get; private set; }


        public static double HeroStartX { get; private set; }
        public static double HeroStartY { get; private set; }

        public static double SpawnRange { get; private set; }
        public static double ProjectileDamage { get; private set; }

        public static double MapWidth { get; private set; }
        public static double MapHeight { get; private set; }

        public static double TickRate { get; private set; }

        static Design()
        {
            ProjectileSpeed = 6;
            HeroSpeed = 5;
            EnemySpeed = 2;
            FireCooldown = 0.2;
            RotateAmount = 0.05;

            HeroStartX = 300;
            HeroStartY = 300;

            SpawnRange = 300;
            ProjectileDamage = 30;

            MapWidth = 1300;
            MapHeight = 1000;

            TickRate = 50;
        }
    }
}
