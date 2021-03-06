﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using doodLbot.Logic;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace doodLbot.Hubs
{
    /// <summary>
    /// Represents a game hub.
    /// </summary>
    public class GameHub : Hub
    {
        private readonly Game game;

        /// <summary>
        /// Constructs a new GameHub using a given Game.
        /// </summary>
        /// <param name="game"></param>
        public GameHub(Game game)
        {
            this.game = game;
        }

        /// <summary>
        /// Updates player's controls. Receieved from the frontend.
        /// </summary>
        /// <param name="update"></param>
        public Task UpdateGameState(GameStateUpdate update)
        {
            game.UpdateControls(update);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Heartbeat from player. Receieved from the frontend.
        /// </summary>
        /// <param name="update"></param>
        public Task Heartbeat(int id)
        {
            var hero = game.GetHeroById(id);
            if (hero != null)
            {
                hero.TimeOfLastHeartbeat = DateTime.Now;
            }

            return Task.CompletedTask;
        }

        public Task UpdateName(string newName, int id)
        {
            var hero = game.GetHeroById(id);
            if (hero == null){
                // TODO think: is it necessary to kill the server for this?
                throw new KeyNotFoundException("hero not found");
            }
            hero.Name = newName;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Used for quick client->server testing..
        /// </summary>
        public Task TestingCallback()
        {
            game.SpawnEnemy(Design.SpawnRange);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Fired when client is ready for the initial sync.
        /// </summary>
        /// <returns></returns>
        public Task ClientIsReady()
        {
            var h = game.AddNewHero();

            var data = new
            {
                id = h.Id,
                algorithm = h.Algorithm,
                mapWidth = Design.MapSize.X,
                mapHeight = Design.MapSize.Y,
                tickRate = Design.TickRate,
                codeInventory = h.CodeInventory,
                equipmentInventory = h.EquipmentInventory
            };
            return Clients.Caller.SendAsync("InitClient", data);
        }
        
        public Task BuyGear(string name, int id)
        {
            var hero = game.GetHeroById(id);
            Log.Debug("server: buy gear");
            
            hero.BuyGear(name);
            return Task.CompletedTask;
        }

        public Task SellGear(string name, int id)
        {
            var hero = game.GetHeroById(id);
            Log.Debug("server: sell gear");
            hero.SellGear(name);
            return Task.CompletedTask;
        }

        public Task BuyCode(string name, int id)
        {
            var hero = game.GetHeroById(id);
            Log.Debug("server: buy code");
            hero.BuyCode(name);
            return Task.CompletedTask;
        }

        public Task SellCode(string name, int id)
        {
            var hero = game.GetHeroById(id);
            Log.Debug("server: sell code");
            hero.SellCode(name);
            return Task.CompletedTask;
        }

        public Task EquipItem(string name, int id)
        {
            var hero = game.GetHeroById(id);
            Log.Debug("server: equip code");
            hero.EquipCode(name);
            return Task.CompletedTask;
        }

        public Task AlgorithmUpdated(string json, int id)
        {
            var hero = game.GetHeroById(id);
            hero.Algorithm = DynamicJsonDeserializer.ToBehaviourAlgorithm(json, hero);
            return Task.CompletedTask;
        }
    }
}
