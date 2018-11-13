namespace doodLbot.Logic
{
    /// <summary>
    /// Represents game design parameters.
    /// </summary>
    public static class Design
    {
        public static double ProjectileSpeed { get; private set; }
        public static double HeroSpeed { get; private set; }
        public static double BackwardsSpeed { get; private set; }
        public static double RotateAmount { get; private set; }
        public static double HeroRadiusSize { get; private set; }
        public static double ProjectileRadiusSize { get; private set; }
        public static double FireCooldown { get; private set; }
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


        static Design()
        {
            TickRate = 30;

            // all speeds are calculated relative to the 50 tickrate
            double adjust = 50 / TickRate;

            ProjectileSpeed = 6 * adjust;
            HeroSpeed = 5 * adjust;
            BackwardsSpeed = 2 * adjust;
            EnemySpeed = 2 * adjust;
            FireCooldown = 0.2;
            RotateAmount = 0.05 * adjust;

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
        }
    }
}
