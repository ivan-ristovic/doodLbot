using doodLbot.Logic;
using Newtonsoft.Json;
using doodLbot.Equipment;

namespace doodLbot.Entities.CodeElements
{
    /// <summary>
    /// Represents an abstract code element.
    /// </summary>
    abstract public class BaseCodeElement : IStorageItem
    {
        /// <summary>
        /// Gets the type of this code element.
        /// </summary>
        [JsonProperty("type")]
        public string Type => GetType().Name;

        /// <summary>
        /// Gets or sets the active flag for this code element.
        /// </summary>
        [JsonProperty("isActive")]
        public virtual bool IsActive { get; set; }

        public string Name => Type;

        public int Cost { get; protected set; }

        /// <summary>
        /// Executes this code element.
        /// </summary>
        /// <param name="state"></param>
        public bool Execute(GameState state, Hero hero)
        {
            return IsActive ? OnExecute(state, hero) : false;
        }

        abstract protected bool OnExecute(GameState state, Hero hero);
    }
}
