using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using doodLbot.Common;
using doodLbot.Entities;
using doodLbot.Extensions;
using doodLbot.Hubs;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace doodLbot.Logic
{
    /// <summary>
    /// Represents a doodLbot game.
    /// </summary>
    public sealed class Game : IDisposable
    {
        public static readonly double TickRate = Design.TickRate;

        public static TimeSpan RefreshTimeSpan => TimeSpan.FromMilliseconds(1000d / TickRate);

        private static System.Diagnostics.Stopwatch Watch = System.Diagnostics.Stopwatch.StartNew();

        public GameState GameState => new GameState(heroes, enemies, EnemyProjectiles);

        public IReadOnlyCollection<Projectile> EnemyProjectiles => enemyProjectiles;

        private Task gameLoopTask;
        private readonly ConcurrentHashSet<Projectile> enemyProjectiles = new ConcurrentHashSet<Projectile>();
        private readonly ConcurrentHashSet<Hero> heroes;
        private readonly HashSet<Enemy> enemies;
        private readonly IHubContext<GameHub> hubContext;
        private readonly RateLimiter enemySpawnLimiter;
        private readonly CancellationTokenSource gameLoopCTS;
        private int currentHeroId = 1;

        /// <summary>
        /// Constructs a new Game which uses a HubContext interface to send data to clients.
        /// </summary>
        /// <param name="hctx"></param>
        public Game(IHubContext<GameHub> hctx)
        {
            heroes = new ConcurrentHashSet<Hero>();
            enemies = new HashSet<Enemy>();
            hubContext = hctx;
            enemySpawnLimiter = new RateLimiter(Design.SpawnInterval);
            gameLoopCTS = new CancellationTokenSource();
        }

        ~Game()
        {
            Dispose();
        }

        public Hero AddNewHero()
        {
            var hero = new Hero(currentHeroId, Design.HeroStartX, Design.HeroStartY,
                new Equipment.CodeStorage(), new Equipment.EquipmentStorage()
            );

            // Update hero Id for the next hero that comes to the game.
            Interlocked.Increment(ref currentHeroId);

            heroes.Add(hero);

            // this.SpawnEnemy(Design.SpawnRange);

            if (heroes.Count == 1)
            {
                gameLoopTask = Task.Run(async () =>
                {
                    while (!gameLoopCTS.IsCancellationRequested)
                    {
                        var ExecWatch = System.Diagnostics.Stopwatch.StartNew();
                        Watch.Stop();
                        var mss = Watch.ElapsedMilliseconds;
                        Watch = System.Diagnostics.Stopwatch.StartNew();
                        var delta = mss / RefreshTimeSpan.TotalMilliseconds;

                        await GameTick(delta);
                        ExecWatch.Stop();
                        var ms = ExecWatch.ElapsedMilliseconds;
                        await Task.Delay(RefreshTimeSpan);
                        //Log.Debug($"exec ms = {ms}, between calls = {mss}, delta = {delta}");
                    }
                }, gameLoopCTS.Token);
            }

            return hero;
        }

        public void Dispose()
        {
            gameLoopCTS.Cancel();
            gameLoopCTS.Dispose();
            gameLoopTask.Dispose();
        }

        /// <summary>
        /// Callback executed on each game tick.
        /// </summary>
        /// <param name="_">game object that is passed by the timer</param>
        private async Task GameTick(double delta)
        {
            Design.Delta = delta < 0.0001 ? 1 : delta;
            if (!enemySpawnLimiter.IsCooldownActive())
            {
                SpawnEnemy(Design.SpawnRange);
            }

            RemoveDeadHeroes();

            foreach (var h in heroes)
            {
                h.IsControlledByAlgorithm = false;
                h.Algorithm.Execute(GameState);
            }

            UpdateStateWithControls(delta);

            foreach (var h in heroes)
            {
                h.Move(delta);
            }
            foreach (var enemy in enemies)
            {
                enemy.VelocityTowardsClosestEntity(heroes);
                enemy.Move(delta);
                if (enemy is Shooter shooter)
                {
                    TryAddEnemyProjectile(shooter);
                }
            }

            foreach (var projectile in enemyProjectiles)
            {
                projectile.Move(delta);
            }

            foreach (var h in heroes)
            {
                foreach (var projectile in h.Projectiles)
                {
                    projectile.Move(delta);
                }
            }

            CheckForCollisionsAndUpdateGame();
            RemoveProjectilesOutsideOfMap();

            await hubContext.SendUpdatesToClients(GameState);
            foreach (var h in heroes)
            {
                h.WipeSyntheticControls();
                if (h.HasCodeChanged)
                {
                    h.HasCodeChanged = false;
                    await hubContext.SendCodeUpdate(h.Algorithm, h.Id);
                }
            }
        }

        /// <summary>
        /// Removes dead heroes from the backend. We pronounce hero dead when client doesn't send a heartbeat for N seconds
        /// </summary>
        /// <param name="inRange"></param>
        public void RemoveDeadHeroes()
        {
            foreach (var h in heroes)
            {
                if (!h.IsAlive)
                {
                    heroes.TryRemove(h);
                }
            }
        }

        /// <summary>
        /// Spawns an enemy in the given square radius around the hero.
        /// </summary>
        /// <param name="inRange"></param>
        public void SpawnEnemy(double inRange)
        {
            // TODO make this to work nicely whith multiplayer - create only one enemy
            foreach (var h in heroes)
            {
                enemies.Add(Enemy.Spawn<Kamikaze>(h.Xpos, h.Ypos, inRange, inRange / 2));
                enemies.Add(Enemy.Spawn<Shooter>(h.Xpos, h.Ypos, inRange, inRange / 2));
            }
        }

        /// <summary>
        /// Updates the current hero controls based on the update received from the frontend.
        /// </summary>
        /// <param name="update"></param>
        public void UpdateControls(GameStateUpdate update)
        {
            var h = GetHeroById(update.Id);
            if (h != null)
            {
                foreach ((var key, var isDown) in update.KeyPresses)
                    h.UpdateControls(key, isDown);
            }
        }

        /// <summary>
        /// Finds hero with the given id.
        /// </summary>
        /// <param name="id">Id of the Hero</param>
        public Hero GetHeroById(int id)
        {
            return heroes.SingleOrDefault(h => h.Id == id);
        }

        /// <summary>
        /// Updates the hero movement based on the controls pressed/released.
        /// </summary>
        /// <param name="delta">relative delta time</param>
        public void UpdateStateWithControls(double delta)
        {
            foreach (var h in heroes)
            {
                h.UpdateStateWithControls(delta);
            }
        }

        private void CheckForCollisionsAndUpdateGame()
        {
            CheckCollisionEnemyHero();
            CheckCollisionWithHeroProjectile();
            CheckCollisionWithEnemyProjectile();
        }


        #region Helper functions
        private void CheckCollisionWithHeroProjectile()
        {
            foreach (var h in heroes)
            {
                IReadOnlyList<(Entity Collider1, Entity Collider2)> collisions = CollisionCheck.GetCollisions(enemies, h.Projectiles);

                foreach ((var Collider1, var Collider2) in collisions)
                {
                    var enemy = Collider1 as Enemy;
                    var projectile = Collider2 as Projectile;

                    enemy.DecreaseHealthPoints(projectile.Damage);

                    // Removing projectile and enemy (if it's dead)
                    if (enemy.Hp <= 0)
                    {
                        enemies.Remove(enemy);
                        h.Points += (int)Math.Ceiling(enemy.Damage);
                    }

                    h.TryRemoveProjectile(projectile);
                }
            }
        }

        private void CheckCollisionWithEnemyProjectile()
        {
            IReadOnlyList<(Entity Collider1, Entity Collider2)> collisions = CollisionCheck.GetCollisions(heroes, enemyProjectiles);
            foreach ((var Collider1, var Collider2) in collisions)
            {
                var hero = Collider1 as Hero;
                var projectile = Collider2 as Projectile;

                hero.DecreaseHealthPoints(projectile.Damage);
                enemyProjectiles.TryRemove(projectile);
            }
        }

        private void CheckCollisionEnemyHero()
        {
            foreach (var h in heroes)
            {
                var heroList = new List<Entity> { h };
                var collisionsWithHero = CollisionCheck.GetCollisions(heroList, enemies);

                foreach ((var Collider1, var Collider2) in collisionsWithHero)
                {
                    var hero = Collider1 as Hero;
                    var enemy = Collider2 as Enemy;
                    enemy.DecreaseHealthPoints(hero.Damage);

                    // Remove kamikaze from the game
                    enemies.Remove(enemy);

                    h.DecreaseHealthPoints(enemy.Damage);
                }
            }
        }

        private void RemoveProjectilesOutsideOfMap()
        {
            foreach (var h in heroes)
            {
                var projectiles = h.Projectiles;
                foreach (var p in projectiles)
                {
                    if (p.IsOutsideBounds(Design.MapSize))
                    {
                        if (h.TryRemoveProjectile(p))
                        {
                            //Log.Debug("Failed to remove projectile on location: " +
                            //    "({Xpos}, {Ypos}) because it's outside of the map.", p.Xpos, p.Ypos);
                        }
                        else
                        {
                            Log.Debug("Failed to remove projectile on location: " +
                                "({Xpos}, {Ypos}) because it's outside of the map.", p.Xpos, p.Ypos);
                        }
                    }
                }
            }

            foreach (var p in EnemyProjectiles)
            {
                if (p.IsOutsideBounds(Design.MapSize))
                {
                    if (TryRemoveEnemyProjectile(p))
                    {
                        //Log.Debug("Failed to remove projectile on location: " +
                        //    "({Xpos}, {Ypos}) because it's outside of the map.", p.Xpos, p.Ypos);
                    }
                    else
                    {
                        Log.Debug("Failed to remove projectile on location: " +
                            "({Xpos}, {Ypos}) because it's outside of the map.", p.Xpos, p.Ypos);
                    }
                }
            }
        }

        public bool TryAddEnemyProjectile(Shooter shooter)
        {
            var p = shooter.TryFire();
            return p != null && enemyProjectiles.Add(p);
        }

        public bool TryRemoveEnemyProjectile(Projectile p)
            => enemyProjectiles.TryRemove(p);
        #endregion
    }
}
