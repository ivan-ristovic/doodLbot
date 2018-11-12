using doodLbot.Common;
using doodLbot.Entities.CodeElements;
using doodLbot.Entities.CodeElements.ConditionElements;
using doodLbot.Logic;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace doodLbot.Hubs
{
    public class GameHub : Hub
    {
        private readonly Game game;


        public GameHub(Game game)
        {
            this.game = game;
        }


        public Task UpdateGameState(GameStateUpdate update)
        {
            this.game.UpdateControls(update);
            return Task.CompletedTask; 
        }

        // TODO remove, this is a communication test
        public Task SendMessage(string user, string message)
        {

            // TODO remove
            this.AlgorithmUpdated(null);

            game.SpawnEnemy(doodLbot.Logic.Design.SpawnRange);
            return this.Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public Task SendUpdatesToClient(GameState update)
            => this.Clients.All.SendAsync("GameStateUpdateRecieved", update);

        public Task AlgorithmUpdated(string json)
        {
            // TODO dummy JSON for testing - remove when unit tests get created for this function
            json = "[" +
                "{\"type\":\"ShootElement\"}, " +
                "{\"type\":\"CodeBlockElement\", \"elements\":[{\"type\":\"ShootElement\"}]}," +
                "{\"type\":\"BranchingElement\", \"cond\":{\"type\":\"IsEnemyNearCondition\"}, \"then\":[{\"type\":\"ShootElement\"}], \"else\":[{\"type\":\"ShootElement\"}]}" +
                "]";
            var jsonVal = JArray.Parse(json) as JArray;
            dynamic elements = jsonVal;

            var algorithm = new BehaviourAlgorithm();
            foreach (dynamic element in elements) 
                algorithm.Insert(DeserializeCodeElementInternal(element));

            this.game.GameState.Hero.Algorithm = algorithm;
            return Task.CompletedTask;


            BaseCodeElement DeserializeCodeElementInternal(dynamic element)
            {
                switch ((string)element["type"]) {
                    case "ShootElement":
                        return new ShootElement();
                    case "CodeBlockElement":
                        var children = new List<BaseCodeElement>();
                        foreach (dynamic child in element.elements)
                            children.Add(DeserializeCodeElementInternal(child));
                        return new CodeBlockElement(children);
                    case "BranchingElement":
                        var thenBlock = new List<BaseCodeElement>();
                        var elseBlock = new List<BaseCodeElement>();
                        foreach (dynamic child in element.@then)
                            thenBlock.Add(DeserializeCodeElementInternal(child));
                        foreach (dynamic child in element.@else)
                            elseBlock.Add(DeserializeCodeElementInternal(child));
                        return new BranchingElement(
                            DeserializeConditionElementInternal(element["cond"]), 
                            new CodeBlockElement(thenBlock), 
                            new CodeBlockElement(elseBlock)
                        );
                    default:
                        return null;
                }
            }

            BaseConditionElement DeserializeConditionElementInternal(dynamic element)
            {
                switch ((string)element["type"]) {
                    case "IsEnemyNearCondition":
                        return new IsEnemyNearCondition();
                    default:
                        return null;
                }
            }
        }
    }
}
