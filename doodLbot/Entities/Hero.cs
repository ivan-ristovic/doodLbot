using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace doodLbot.Entities
{
    public class Hero : Entity
    {
        // These will remain lists of objects until the classes are made
        [JsonProperty("gear")]
        public IReadOnlyList<object> Gear => this.gear.AsReadOnly();
        [JsonProperty("modules")]
        public IReadOnlyList<object> Modules => this.modules.AsReadOnly();

        private readonly List<object> gear;
        private readonly List<object> modules;


        public Hero() : base()
        {
            this.gear = new List<object>();
            this.modules = new List<object>();
        }
    }
}
