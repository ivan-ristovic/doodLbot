using doodLbot.Common;
using doodLbot.Entities;
using doodLbot.Hubs;

using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;

namespace doodLbot.Logic
{
    public sealed class Game
    {
        static public readonly double TickRate = Design.TickRate;
        static public TimeSpan RefreshTimeSpan => TimeSpan.FromMilliseconds(1000d / TickRate);

        static private readonly AsyncExecutor _async = new AsyncExecutor();

        // TODO track if code blocks have changed
        static bool codeBlocksChanged = false;

        // executes one tick of the game
        static private void UpdateCallback(object _)
        {
            var game = _ as Game;

            if (!game.enemySpawnLimiter.IsCooldownActive())
            {
                game.SpawnEnemy(Design.SpawnRange);
            }
            game.UpdateStateWithControls();

            game.hero.Move();
            game.hero.Algorithm.Execute(game.GameState);

            foreach (Enemy enemy in game.enemies)
            {
                enemy.VelocityTowards(game.hero, Design.EnemySpeed);
                enemy.Move();
            }

            foreach (var projectile in game.hero.Projectiles)
            {
                projectile.Move();
                // Remove projectiles 
            }

            game.CheckForCollisionsAndUpdateGame();

            game.RemoveProjectilesOutsideOfMap();

            _async.Execute(game.hubContext.Clients.All.SendAsync("StateUpdate", game.GameState));

            // test

            if (codeBlocksChanged)
            {
                codeBlocksChanged = false;
                _async.Execute(game.hubContext.Clients.All.SendAsync("UpdateCodeBlocks", game.GameState.Hero.Algorithm));
            }
            // test
        }

        public GameState GameState => new GameState(this.hero, this.enemies);

        private readonly Hero hero;     // Change this to ConcurrentHashSet for the multiplayer
        private readonly ConcurrentHashSet<Enemy> enemies;
        private readonly Timer ticker;
        private readonly IHubContext<GameHub> hubContext;
        private readonly Controls controls;
        private readonly RateLimiter shootRateLimiter;
        private readonly RateLimiter enemySpawnLimiter;

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

            // testing begin
            var isEnemyNear = new Entities.CodeElements.ConditionElements.IsEnemyNearCondition();

            var shootElement = new Entities.CodeElements.ShootElement();
            var shootElementList = new List<Entities.CodeElements.BaseCodeElement>();
            shootElementList.Add(shootElement);
            shootElementList.Add(shootElement);
            shootElementList.Add(shootElement);

            var idleElement = new Entities.CodeElements.IdleElement();
            var idleElementList = new List<Entities.CodeElements.BaseCodeElement>();
            idleElementList.Add(idleElement);
            idleElementList.Add(shootElement);
            idleElementList.Add(idleElement);

            var branchingElement = new Entities.CodeElements.BranchingElement(
                isEnemyNear,
                new Entities.CodeElements.CodeBlockElement(shootElementList),
                new Entities.CodeElements.CodeBlockElement(idleElementList));

            hero.Algorithm.Insert(branchingElement);
            hero.Algorithm.Insert(idleElement);
            hero.Algorithm.Insert(shootElement);
            // testing end
        }

        public void SpawnEnemy(double inRange)
        {
            enemies.Add(Enemy.Spawn<Kamikaze>(hero.Xpos, hero.Ypos, inRange));
        }

        public void UpdateControls(GameStateUpdate update)
        {
            foreach (var (key, isDown) in update.KeyPresses)
            {
                this.controls.OnKey(key, isDown);
            }
        }

        public void UpdateStateWithControls()
        {
            double rotationAmount = Design.RotateAmount;

            if (controls.IsFire)
            {
                if (!shootRateLimiter.IsCooldownActive())
                {
                    this.hero.Fire(Design.ProjectileSpeed, Design.ProjectileDamage);
                }
            }
            if (controls.IsForward)
            {
                double velocity = Design.HeroSpeed;
                this.hero.Xvel = Math.Cos(this.hero.Rotation) * velocity;
                this.hero.Yvel = Math.Sin(this.hero.Rotation) * velocity;
            }
            if (controls.IsBackward)
            {
                double velocity = Design.BackwardsSpeed;
                this.hero.Xvel = -Math.Cos(this.hero.Rotation) * velocity;
                this.hero.Yvel = -Math.Sin(this.hero.Rotation) * velocity;
            }
            if (!controls.IsForward && !controls.IsBackward)
            {
                this.hero.Xvel = 0;
                this.hero.Yvel = 0;
            }
            if (controls.IsLeft)
            {
                this.hero.Rotation -= rotationAmount;
            }
            if (controls.IsRight)
            {
                this.hero.Rotation += rotationAmount;
            }
        }

        private void CheckForCollisionsAndUpdateGame()
        {
            CheckForCollisionsEnemiesProjectiles();
            CheckForCollisionsEnemiesHero();
        }

        #region Helper functions

        private void CheckForCollisionsEnemiesHero()
        {
            var collisions = CollisionCheck.GetCollisions(this.enemies, this.hero.Projectiles);

            foreach (Collision c in collisions)
            {
                Entity enemy = c.collider1;
                Entity projectile = c.collider2;

                enemy.DecreaseHelthPoints(projectile.Damage);

                // Removing projectile and enemy (if it's dead)
                if (enemy.Hp <= 0)
                {
                    this.enemies.TryRemove((Enemy)enemy);
                }

                this.hero.TryRemoveProjectile((Projectile)projectile);
            }
        }

        private void CheckForCollisionsEnemiesProjectiles()
        {
            List<Entity> heros = new List<Entity>();
            heros.Add(this.hero);
            var collisionsWithHero = CollisionCheck.GetCollisions(heros, this.enemies);

            foreach (Collision c in collisionsWithHero)
            {
                Entity hero = c.collider1;
                Entity kamikaze = c.collider2;
                kamikaze.DecreaseHelthPoints(hero.Damage);

                // Remove kamikaze from the game
                // TODO: add animation for the death of kamikaze
                this.enemies.TryRemove((Enemy)kamikaze);

                this.hero.DecreaseHelthPoints(kamikaze.Damage);
            }
        }

        private void RemoveProjectilesOutsideOfMap()
        {
            var projectiles = this.hero.Projectiles;
            foreach (Projectile p in projectiles)
            {
                if (p.IsOutsideBounds(Design.MapSize))
                {
                    if (this.hero.TryRemoveProjectile(p))
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

        #endregion
    }
}
