using doodLbot.Entities.CodeElements;
using doodLbot.Entities.CodeElements.ConditionElements;

using Newtonsoft.Json.Linq;

using System.Collections.Generic;

namespace doodLbot.Logic
{
    /// <summary>
    /// Deserializes JSON strings to classes needed for game logic.
    /// </summary>
    static public class DynamicJsonDeserializer
    {
        /// <summary>
        /// Deserializes JSON string to BehaviourAlgorithm.
        /// </summary>
        /// <param name="json"></param>
        /// <returns>Deserialized BehaviourAlgorithm object.</returns>
        static public BehaviourAlgorithm ToBehaviourAlgorithm(string json)
        {
            var jsonVal = JArray.Parse(json) as JArray;
            dynamic elements = jsonVal;

            var algorithm = new BehaviourAlgorithm();
            foreach (dynamic element in elements)
                algorithm.Insert(DeserializeCodeElementInternal(element));

            return algorithm;


            BaseCodeElement DeserializeCodeElementInternal(dynamic element)
            {
                switch ((string)element["type"]) {
                    case "ShootElement":
                        return new ShootElement();
                    case "IdleElement":
                        return new IdleElement();
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
