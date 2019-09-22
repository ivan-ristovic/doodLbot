using doodLbot.Equipment;
using doodLbot.Logic;
using Newtonsoft.Json;

namespace doodLbot.Entities.CodeElements
{
    /// <summary>
    /// Represents a base for all code elements.
    /// </summary>
    public abstract class BaseCodeElement : IStorageItem
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


        protected abstract bool OnExecute(GameState state, Hero hero);
    }
}
