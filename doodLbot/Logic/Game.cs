using doodLbot.Common;
using doodLbot.Entities;
using doodLbot.Entities.CodeElements;
using doodLbot.Entities.CodeElements.ConditionElements;
using doodLbot.Extensions;
using doodLbot.Hubs;

using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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


        public GameState GameState => new GameState(this.heroes, this.enemies, this.EnemyProjectiles);

        public IReadOnlyCollection<Projectile> EnemyProjectiles => this.enemyProjectiles;

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
            this.heroes = new ConcurrentHashSet<Hero>();
            this.enemies = new HashSet<Enemy>();
            this.hubContext = hctx;
            this.enemySpawnLimiter = new RateLimiter(Design.SpawnInterval);
            this.gameLoopCTS = new CancellationTokenSource();
        }

        ~Game()
        {
            this.Dispose();
        }

        public Hero AddNewHero()
        {
            // begin hardcoded test
            // todo remove this
            Hero hero = new Hero(currentHeroId, Design.HeroStartX, Design.HeroStartY,
                new Equipment.CodeStorage(), new Equipment.EquipmentStorage()
            );

            // Update hero Id for the next hero that comes to the game.
            Interlocked.Increment(ref currentHeroId);

            var shootElementList = new List<BaseCodeElement> {
                new TargetElement(),
                new ShootElement(new RateLimiter(Design.ShootElementCooldown)),
                new ShootElement(new RateLimiter(Design.ShootElementCooldown))
            };

            var idleElementList = new List<BaseCodeElement> {
                new IdleElement(),
                new IdleElement(),
                new IdleElement(),
            };

            var branchingElement = new BranchingElement(
                new IsEnemyNearCondition(),
                new CodeBlockElement(shootElementList),
                new CodeBlockElement(idleElementList)
            );

            hero.Algorithm.Insert(branchingElement);
            hero.Algorithm.Insert(new IdleElement());
            hero.Algorithm.Insert(new ShootElement(
                new RateLimiter(Design.ShootElementCooldown)));

            this.heroes.Add(hero);

            //this.SpawnEnemy(Design.SpawnRange);
            // end hardcoded test

            if (this.heroes.Count == 1)
            {
                this.gameLoopTask = Task.Run(async () => {
                    while (!this.gameLoopCTS.IsCancellationRequested)
                    {
                        var ExecWatch = System.Diagnostics.Stopwatch.StartNew();
                        Watch.Stop();
                        var mss = Watch.ElapsedMilliseconds;
                        Watch = System.Diagnostics.Stopwatch.StartNew();
                        double delta = mss / RefreshTimeSpan.TotalMilliseconds;

                        await this.GameTick(delta);
                        ExecWatch.Stop();
                        var ms = ExecWatch.ElapsedMilliseconds;
                        //Thread.Sleep(RefreshTimeSpan);
                        await Task.Delay(RefreshTimeSpan);
                        //Log.Debug($"exec ms = {ms}, between calls = {mss}, delta = {delta}");
                    }
                }, gameLoopCTS.Token);
            }

            return hero;
        }

        public void Dispose()
        {
            this.gameLoopCTS.Cancel();
            this.gameLoopCTS.Dispose();
            this.gameLoopTask.Dispose();
        }

        /// <summary>
        /// Callback executed on each game tick.
        /// </summary>
        /// <param name="_">game object that is passed by the timer</param>
        private async Task GameTick(double delta)
        {
            if (!this.enemySpawnLimiter.IsCooldownActive()) {
                this.SpawnEnemy(Design.SpawnRange);
            }
            this.UpdateStateWithControls(delta);

            foreach (Hero h in this.heroes) {
                h.Move(delta);
                h.Algorithm.Execute(this.GameState);
            }

            foreach (Enemy enemy in this.enemies) {
                enemy.VelocityTowardsClosestEntity(this.heroes);
                enemy.Move(delta);
                if (enemy is Shooter shooter)
                    this.TryAddEnemyProjectile(shooter);
            }

            foreach (Hero h in this.heroes) {
                foreach (Projectile projectile in h.Projectiles) {
                    projectile.Move(delta);
                }
            }

            this.CheckForCollisionsAndUpdateGame();
            this.RemoveProjectilesOutsideOfMap();

            await this.hubContext.SendUpdatesToClients(this.GameState);
            foreach (Hero h in this.heroes) {
                if (h.Points >= 40) {
                    if (h.HasGearChanged) {
                        h.BuyGear("hoverboard");
                        h.HasGearChanged = false;
                    }
                }
                if (h.HasCodeChanged)
                {
                    h.HasCodeChanged = false;
                    await this.hubContext.SendCodeUpdate(h.Algorithm);
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
            foreach (Hero h in this.heroes)
            {
                this.enemies.Add(Enemy.Spawn<Kamikaze>(h.Xpos, h.Ypos, inRange, inRange/2));
            }
        }

        /// <summary>
        /// Updates the current hero controls based on the update received from the frontend.
        /// </summary>
        /// <param name="update"></param>
        public void UpdateControls(GameStateUpdate update)
        {
            foreach (Hero h in this.heroes)
            {
                foreach ((ConsoleKey key, bool isDown) in update.KeyPresses)
                    h.UpdateControls(key, isDown);
            }
        }

        /// <summary>
        /// Updates the hero movement based on the controls pressed/released.
        /// </summary>
        /// <param name="delta">relative delta time</param>
        public void UpdateStateWithControls(double delta)
        {
            foreach (Hero h in this.heroes)
            {
                h.UpdateStateWithControls(delta);
            }           
        }

        private void CheckForCollisionsAndUpdateGame()
        {
            this.CheckCollisionEnemyHero();
            this.CheckCollisionEnemyProjectile();
        }

        #region Helper functions

        private void CheckCollisionEnemyProjectile()
        {
            foreach (Hero h in this.heroes)
            {
                IReadOnlyList<(Entity Collider1, Entity Collider2)> collisions = CollisionCheck.GetCollisions(this.enemies, h.Projectiles);

                foreach ((Entity Collider1, Entity Collider2) in collisions)
                {
                    var enemy = Collider1 as Enemy;
                    var projectile = Collider2 as Projectile;

                    enemy.DecreaseHealthPoints(projectile.Damage);

                    // Removing projectile and enemy (if it's dead)
                    if (enemy.Hp <= 0)
                    {
                        this.enemies.Remove(enemy);
                        h.Points += (int)Math.Ceiling(enemy.Damage);
                    }

                    h.TryRemoveProjectile(projectile);
                }
            }
        }

        private void CheckCollisionEnemyHero()
        {
            foreach (Hero h in this.heroes)
            {
                var heroList = new List<Entity> { h };
                IReadOnlyList<(Entity, Entity)> collisionsWithHero = CollisionCheck.GetCollisions(heroList, this.enemies);

                foreach ((Entity Collider1, Entity Collider2) in collisionsWithHero) {
                    var hero = Collider1 as Hero;
                    var enemy = Collider2 as Enemy;
                    enemy.DecreaseHealthPoints(hero.Damage);

                    // Remove kamikaze from the game
                    this.enemies.Remove(enemy);

                    h.DecreaseHealthPoints(enemy.Damage);
                }
            }
        }

        private void RemoveProjectilesOutsideOfMap()
        {
            foreach (Hero h in this.heroes)
            {
                IReadOnlyCollection<Projectile> projectiles = h.Projectiles;
                foreach (Projectile p in projectiles)
                {
                    if (p.IsOutsideBounds(Design.MapSize))
                    {
                        if (h.TryRemoveProjectile(p))
                        {
                            //Log.Debug($"Removed projectile on location: " +
                            //    $"({ p.Xpos}, { p.Ypos}) because it's outside of the map.");
                        }
                        else
                        {
                            Log.Debug($"Failed to remove projectile on location:" +
                                $" ({p.Xpos}, {p.Ypos}) because it's outside of the map.");
                        }
                    }
                }
            }

            foreach (Projectile p in this.EnemyProjectiles) {
                if (p.IsOutsideBounds(Design.MapSize)) {
                    if (this.TryRemoveEnemyProjectile(p)) {
                        //Log.Debug($"Removed projectile on location: " +
                        //    $"({ p.Xpos}, { p.Ypos}) because it's outside of the map.");
                    } else {
                        Log.Debug($"Failed to remove projectile on location:" +
                            $" ({p.Xpos}, {p.Ypos}) because it's outside of the map.");
                    }
                }
            }
        }

        public bool TryAddEnemyProjectile(Shooter shooter)
        {
            Projectile p = shooter.TryFire();
            return p != null && this.enemyProjectiles.Add(p);
        }

        public bool TryRemoveEnemyProjectile(Projectile p)
            =>  this.enemyProjectiles.TryRemove(p);

        #endregion
    }
}
