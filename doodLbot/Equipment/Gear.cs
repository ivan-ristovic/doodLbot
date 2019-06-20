using Newtonsoft.Json;

namespace doodLbot.Equipment
{
    public abstract class Gear : IStorageItem
    {
        [JsonProperty("cost")]
        public int Cost { get; protected set; }

        /// <summary>
        /// Gets the type of this code element.
        /// </summary>
        [JsonProperty("type")]
        public string Type => GetType().Name;

        [JsonProperty("name")]
        public string Name { get; protected set; }

        /// <summary>
        /// If it's not visible, then no need to send it 
        /// to client in gear list
        /// </summary>
        [JsonIgnore]
        public bool IsVisible { get; set; }

        public Gear(string name, int cost)
        {
            Name = name;
            Cost = cost;
        }
    }
}
