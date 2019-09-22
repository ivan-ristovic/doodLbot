using Newtonsoft.Json;

namespace doodLbot.Equipment
{
    public abstract class Gear : IStorageItem
    {
        [JsonProperty("cost")]
        public int Cost { get; protected set; }

        [JsonProperty("type")]
        public string Type => GetType().Name;

        [JsonProperty("name")]
        public string Name { get; protected set; }

        [JsonIgnore]
        public bool IsVisible { get; set; }

        public Gear(string name, int cost)
        {
            Name = name;
            Cost = cost;
        }
    }
}
