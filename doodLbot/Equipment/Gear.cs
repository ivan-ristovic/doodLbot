using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public string Type => this.GetType().Name;

        public string Name { get; protected set; }

        public Gear(string name, int cost)
        {
            this.Name = name;
            this.Cost = cost;
        }
    }
}
