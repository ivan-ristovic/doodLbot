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

        private readonly List<object> gear = new List<object>();
        private readonly List<object> modules = new List<object>();


        public Hero() : base()
        {
        }

        public Hero(double x, double y) : base(x, y)
        {

        }
    }
}
