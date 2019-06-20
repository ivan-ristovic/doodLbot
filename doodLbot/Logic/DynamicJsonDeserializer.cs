using System.Collections.Generic;
using doodLbot.Common;
using doodLbot.Entities;
using doodLbot.Entities.CodeElements;
using doodLbot.Entities.CodeElements.ConditionElements;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;

namespace doodLbot.Logic
{
    /// <summary>
    /// Deserializes JSON strings to classes needed for game logic.
    /// </summary>
    public static class DynamicJsonDeserializer
    {
        /// <summary>
        /// Deserializes JSON string to BehaviourAlgorithm.
        /// </summary>
        /// <param name="json"></param>
        /// <returns>Deserialized BehaviourAlgorithm object.</returns>
        public static BehaviourAlgorithm ToBehaviourAlgorithm(string json, Hero hero)
        {
            var jsonVal = JArray.Parse(json) as JArray;
            dynamic elements = jsonVal;

            var algorithm = new BehaviourAlgorithm(hero);
            foreach (var element in elements)
                algorithm.Insert(DeserializeCodeElementInternal(element));

            return algorithm;


            BaseCodeElement DeserializeCodeElementInternal(dynamic element)
            {
                BaseCodeElement ret;
                switch ((string)element["type"])
                {
                    case "ShootElement":
                        ret = new ShootElement(new RateLimiter(Design.ShootElementCooldown));
                        break;
                    case "IdleElement":
                        ret = new IdleElement();
                        break;
                    case "CodeBlockElement":
                        var children = new List<BaseCodeElement>();
                        foreach (var child in element.elements)
                            children.Add(DeserializeCodeElementInternal(child));
                        ret = new CodeBlockElement(children);
                        break;
                    case "BranchingElement":
                        var thenBlock = new List<BaseCodeElement>();
                        var elseBlock = new List<BaseCodeElement>();
                        try
                        {
                            foreach (var child in element.@then.elements)
                                thenBlock.Add(DeserializeCodeElementInternal(child));
                        }
                        catch (RuntimeBinderException)
                        {
                        }

                        try
                        {
                            foreach (var child in element.@else.elements)
                                elseBlock.Add(DeserializeCodeElementInternal(child));
                        }
                        catch (RuntimeBinderException)
                        {
                        }

                        ret = new BranchingElement(
                            DeserializeConditionElementInternal(element["cond"]),
                            new CodeBlockElement(thenBlock),
                            new CodeBlockElement(elseBlock)
                        );
                        break;
                    case "TargetElement":
                        ret = new TargetElement();
                        break;
                    case "MoveAwayFromElement":
                        ret = new MoveAwayFromElement();
                        break;
                    default:
                        ret = null;
                        break;
                }
                ret.IsActive = element["isActive"];
                return ret;
            }

            BaseConditionElement DeserializeConditionElementInternal(dynamic element)
            {
                if (element is null)
                {
                    return null;
                }
                switch ((string)element["type"])
                {
                    case "IsEnemyNearCondition":
                        return new IsEnemyNearCondition();
                    default:
                        return null;
                }
            }
        }
    }
}
