using doodLbot.Common;
using doodLbot.Entities;
using doodLbot.Hubs;

using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.Threading;

namespace doodLbot.Logic
{
    public sealed class Game 
    {
        static public readonly (int X, int Y) MapSize = (2000, 2000);
        static public readonly int TickRate = 50;
        static public TimeSpan RefreshTimeSpan => TimeSpan.FromMilliseconds(1000d / TickRate);

        static private readonly AsyncExecutor _async = new AsyncExecutor();

        // executes one tick of the game
        static private void UpdateCallback(object _)
        {
            var game = _ as Game;
            
            game.hero.Move();
            //game.hero.Algorithm.ExecuteStep(game.hero);

            foreach (Enemy enemy in game.enemies)
            {
                enemy.VelocityTowards(game.hero, 5);
                enemy.Move();
            }            

            foreach (var projectile in game.hero.Projectiles)
            {
                projectile.Move();
                // Remove projectiles 
            }

            game.CheckForCollisionsAndUpdateGame();

            _async.Execute(game.hubContext.Clients.All.SendAsync("StateUpdate", game.GameState));
        }

        public GameState GameState => new GameState(this.hero, this.enemies);

        private readonly Hero hero;     // Change this to ConcurrentHashSet for the multiplayer
        private readonly ConcurrentHashSet<Enemy> enemies;
        private readonly Timer ticker;
        private readonly IHubContext<GameHub> hubContext;
        private readonly Controls controls;

        public Game(IHubContext<GameHub> hctx)
        {
            this.hero = new Hero(300, 300);
            this.enemies = new ConcurrentHashSet<Enemy>();
            this.SpawnEnemy();
            this.ticker = new Timer(UpdateCallback, this, RefreshTimeSpan, RefreshTimeSpan);
            this.hubContext = hctx;
            this.controls = new Controls();
        }

        public void SpawnEnemy()
        {
            const double spawnRange = 300;
            enemies.Add(Enemy.Spawn<Kamikaze>(hero.Xpos, hero.Ypos, spawnRange));
        }


        public void UpdateControls(GameStateUpdate update)
        {
            foreach(var (key, isDown) in update.KeyPresses)
            {
                this.controls.OnKey(key, isDown);
            }
        }

        public void UpdateStateWithControls()
        {
            const double velMultiplier = 5;
            const double rotationAmount = 0.1;

            if (controls.IsFire)
            {
                this.hero.Fire();
            }
            if (controls.IsForward)
            {
                this.hero.Xvel = Math.Cos(this.hero.Rotation) * velMultiplier;
                this.hero.Yvel = Math.Sin(this.hero.Rotation) * velMultiplier;
            }
            if (controls.IsBackward)
            {
                this.hero.Xvel = -Math.Cos(this.hero.Rotation) * velMultiplier;
                this.hero.Yvel = -Math.Sin(this.hero.Rotation) * velMultiplier;
            }
            if (!controls.IsForward && controls.IsBackward)
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

        public void CheckForCollisionsAndUpdateGame()
        {
            Collision[] collisions = CollisionCheck.getCollisions(this.enemies, this.hero.Projectiles);

            foreach (Collision c in collisions)
            {
                Enemy e = (Enemy)c.collider1;
                Projectile p = (Projectile)c.collider2;
                e.DecreaseHelthPoints(p.Damage);

                // Removing projectile and enemy (if it's dead)
                if (e.Hp <= 0)
                {
                    this.enemies.TryRemove(e);
                }

                this.hero.TryRemoveProjectile(p);
            }
        }
    }
}
