using Newtonsoft.Json;

namespace doodLbot.Equipment
{
    public class Armor : Gear
    {
        [JsonProperty("speed")]
        public double Speed { get; protected set; }

        [JsonProperty("hp")]
        public double Hp { get; protected set; }

        [JsonProperty("def")]
        public double Defense { get; protected set; }

        public Armor(string name, int cost, double speed) : base(name, cost)
        {
            Speed = speed;
        }
    }
}
