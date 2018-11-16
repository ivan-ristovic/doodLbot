using doodLbot.Common;
using doodLbot.Entities;
using doodLbot.Entities.CodeElements;
using doodLbot.Entities.CodeElements.ConditionElements;
using doodLbot.Hubs;

using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;

namespace doodLbot.Logic
{
    /// <summary>
    /// Represents a doodLbot game.
    /// </summary>
    public sealed class Game
    {
        public static readonly double TickRate = Design.TickRate;
        public static TimeSpan RefreshTimeSpan => TimeSpan.FromMilliseconds(1000d / TickRate);

        private static readonly AsyncExecutor _async = new AsyncExecutor();

        // TODO track if code blocks have changed
        private static bool codeBlocksChanged = false;

        // TODO track if gear has changed
        private static bool gearChanged = true;

        static private System.Diagnostics.Stopwatch Watch = 
            System.Diagnostics.Stopwatch.StartNew();

        /// <summary>
        /// Callback executed on each game tick.
        /// </summary>
        /// <param name="_"></param>
        private static void UpdateCallback(object _)
        {
            var ExecWatch = System.Diagnostics.Stopwatch.StartNew();
            Watch.Stop();
            var mss = Watch.ElapsedMilliseconds;
            Watch = System.Diagnostics.Stopwatch.StartNew();

            var game = _ as Game;

            if (!game.enemySpawnLimiter.IsCooldownActive()) {
                game.SpawnEnemy(Design.SpawnRange);
            }
            game.UpdateStateWithControls();

            game.hero.Move();
            game.hero.Algorithm.Execute(game.GameState);

            foreach (Enemy enemy in game.enemies) {
                enemy.VelocityTowards(game.hero);
                enemy.Move();
            }

            foreach (Projectile projectile in game.hero.Projectiles) {
                projectile.Move();
            }

            game.CheckForCollisionsAndUpdateGame();

            game.RemoveProjectilesOutsideOfMap();

            _async.Execute(game.hubContext.Clients.All.SendAsync("StateUpdate", game.GameState));

            // begin test
            if (game.hero.Points >= 40)
            {
                if (gearChanged)
                {
                    gearChanged = false;
                    game.hero.AddGear(new Equipment.Armor("hoverboard", 40, 5));
                    game.hero.Points -= 40;
                }
            }
            if (codeBlocksChanged) {
                codeBlocksChanged = false;
                _async.Execute(game.hubContext.Clients.All.SendAsync("UpdateCodeBlocks", game.GameState.Hero.Algorithm));
            }
            ExecWatch.Stop();
            var ms = ExecWatch.ElapsedMilliseconds;
            if (false)
            {
                Log.Debug($"exec ms = {ms}, between calls = {mss}");
            }
            // end test
        }

        public GameState GameState => new GameState(this.hero, this.enemies);

        private readonly Hero hero;     // Change this to ConcurrentHashSet for the multiplayer
        private readonly ConcurrentHashSet<Enemy> enemies;
        private readonly Timer ticker;
        private readonly IHubContext<GameHub> hubContext;
        private readonly Controls controls;
        private readonly RateLimiter shootRateLimiter;
        private readonly RateLimiter enemySpawnLimiter;

        /// <summary>
        /// Constructs a new Game which uses a HubContext interface to send data to clients.
        /// </summary>
        /// <param name="hctx"></param>
        public Game(IHubContext<GameHub> hctx)
        {
            this.hero = new Hero(Design.HeroStartX, Design.HeroStartY);
            this.enemies = new ConcurrentHashSet<Enemy>();
            this.SpawnEnemy(Design.SpawnRange);
            this.ticker = new Timer(UpdateCallback, this, RefreshTimeSpan, RefreshTimeSpan);
            this.hubContext = hctx;
            this.controls = new Controls();
            this.shootRateLimiter = new RateLimiter(Design.FireCooldown);
            this.enemySpawnLimiter = new RateLimiter(Design.SpawnInterval);

            // begin test
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

            this.hero.Algorithm.Insert(branchingElement);
            this.hero.Algorithm.Insert(new IdleElement());
            this.hero.Algorithm.Insert(new ShootElement(
                new RateLimiter(Design.ShootElementCooldown)));
            // end test
        }


        /// <summary>
        /// Spawns an enemy in the given square radius around the hero.
        /// </summary>
        /// <param name="inRange"></param>
        public void SpawnEnemy(double inRange) 
            => this.enemies.Add(Enemy.Spawn<Kamikaze>(this.hero.Xpos, this.hero.Ypos, inRange));

        /// <summary>
        /// Updates the current hero controls based on the update received from the frontend.
        /// </summary>
        /// <param name="update"></param>
        public void UpdateControls(GameStateUpdate update)
        {
            foreach ((ConsoleKey key, bool isDown) in update.KeyPresses)
                this.controls.OnKey(key, isDown);
        }

        /// <summary>
        /// Updates the hero movement based on the controls pressed/released.
        /// </summary>
        public void UpdateStateWithControls()
        {
            double rotationAmount = Design.RotateAmount;

            if (this.controls.IsFire) {
                if (!this.shootRateLimiter.IsCooldownActive()) {
                    this.hero.Fire(Design.ProjectileSpeed, Design.ProjectileDamage);
                }
            }
            if (this.controls.IsForward) {
                double velocity = this.hero.Speed;
                this.hero.Xvel = Math.Cos(this.hero.Rotation) * velocity;
                this.hero.Yvel = Math.Sin(this.hero.Rotation) * velocity;
            }
            if (this.controls.IsBackward) {
                double velocity = Design.BackwardsSpeed;
                this.hero.Xvel = -Math.Cos(this.hero.Rotation) * velocity;
                this.hero.Yvel = -Math.Sin(this.hero.Rotation) * velocity;
            }
            if (!this.controls.IsForward && !this.controls.IsBackward) {
                this.hero.Xvel = 0;
                this.hero.Yvel = 0;
            }
            if (this.controls.IsLeft) {
                this.hero.Rotation -= rotationAmount;
            }
            if (this.controls.IsRight) {
                this.hero.Rotation += rotationAmount;
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
            IReadOnlyList<Collision> collisions = CollisionCheck.GetCollisions(this.enemies, this.hero.Projectiles);

            foreach (Collision c in collisions) {
                Entity enemy = c.Collider1;
                Entity projectile = c.Collider2;

                enemy.DecreaseHealthPoints(projectile.Damage);

                // Removing projectile and enemy (if it's dead)
                if (enemy.Hp <= 0)
                { 
                    this.enemies.TryRemove((Enemy)enemy);
                    this.hero.Points += (int)Math.Ceiling(enemy.Damage);
                }

                this.hero.TryRemoveProjectile((Projectile)projectile);
            }
        }

        private void CheckCollisionEnemyHero()
        {
            var heros = new List<Entity> {
                this.hero
            };
            IReadOnlyList<Collision> collisionsWithHero = CollisionCheck.GetCollisions(heros, this.enemies);

            foreach (Collision c in collisionsWithHero) {
                Entity hero = c.Collider1;
                Entity kamikaze = c.Collider2;
                kamikaze.DecreaseHealthPoints(hero.Damage);

                // Remove kamikaze from the game
                this.enemies.TryRemove((Enemy)kamikaze);

                this.hero.DecreaseHealthPoints(kamikaze.Damage);
            }
        }

        private void RemoveProjectilesOutsideOfMap()
        {
            IReadOnlyCollection<Projectile> projectiles = this.hero.Projectiles;
            foreach (Projectile p in projectiles) {
                if (p.IsOutsideBounds(Design.MapSize)) {
                    if (this.hero.TryRemoveProjectile(p)) {
                        //Log.Debug($"Removed projectile on location: " +
                        //    $"({ p.Xpos}, { p.Ypos}) because it's outside of the map.");
                    } else {
                        Log.Debug($"Failed to remove projectile on location:" +
                            $" ({p.Xpos}, {p.Ypos}) because it's outside of the map.");
                    }
                }
            }
        }

        #endregion
    }
}
