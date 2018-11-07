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
        

        public Game(IHubContext<GameHub> hctx)
        {
            this.hero = new Hero(300, 300);
            this.enemies = new ConcurrentHashSet<Enemy>();
            this.SpawnEnemy();
            this.ticker = new Timer(UpdateCallback, this, RefreshTimeSpan, RefreshTimeSpan);
            this.hubContext = hctx;
        }

        public void SpawnEnemy()
        {
            const double spawnRange = 300;
            enemies.Add(Enemy.Spawn<Kamikaze>(hero.Xpos, hero.Ypos, spawnRange));
        }


        public void UpdateState(GameStateUpdate update)
        {
            foreach(var (key, isDown) in update.KeyPresses)
            {
                OnKey(key, isDown);
            }
        }

        public void OnKey(ConsoleKey key, bool isDown)
        {
            double velMultiplier = isDown ? 5 : 0;
            switch (key)
            {
                case ConsoleKey.A:
                    this.hero.Rotation -= 0.1;
                    break;
                case ConsoleKey.S:
                    this.hero.Xvel = -Math.Cos(this.hero.Rotation) * velMultiplier;
                    this.hero.Yvel = -Math.Sin(this.hero.Rotation) * velMultiplier; 
                    break;
                case ConsoleKey.D:
                    this.hero.Rotation += 0.1;
                    break;
                case ConsoleKey.W:
                    this.hero.Xvel = Math.Cos(this.hero.Rotation) * velMultiplier;
                    this.hero.Yvel = Math.Sin(this.hero.Rotation) * velMultiplier; 
                    break;
                case ConsoleKey.Spacebar:
                    this.hero.Fire();
                    break;
            }
        }



        public void CheckForCollisionsAndUpdateGame()
        {
            CheckForCollisionsEnemiesProjectiles();
            CheckForCollisionsEnemiesHero();
        }

        private void CheckForCollisionsEnemiesHero()
        {
            IReadOnlyList<Collision> collisions = CollisionCheck.GetCollisions(this.enemies, this.hero.Projectiles);

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
            IReadOnlyList<Collision> collisionsWithHero = CollisionCheck.GetCollisions(heros, this.enemies);

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
    }
}
