using Newtonsoft.Json;

namespace doodLbot.Equipment
{
    public class Weapon : Gear
    {
        [JsonProperty("damage")]
        public double Damage { get; protected set; }


        public Weapon(string name, int cost) : base(name, cost)
        {

        }
    }
}
