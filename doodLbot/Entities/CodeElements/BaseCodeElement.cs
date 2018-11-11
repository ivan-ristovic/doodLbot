using doodLbot.Logic;
using Newtonsoft.Json;

namespace doodLbot.Entities.CodeElements
{
    abstract public class BaseCodeElement
    {
        [JsonProperty("type")]
        public string Type => this.GetType().Name;


        abstract public void Execute(GameState state);
    }
}
