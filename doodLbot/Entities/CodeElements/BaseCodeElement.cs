﻿using doodLbot.Logic;
using Newtonsoft.Json;

namespace doodLbot.Entities.CodeElements
{
    /// <summary>
    /// Represents an abstract code element.
    /// </summary>
    abstract public class BaseCodeElement
    {
        /// <summary>
        /// Gets the type of this code element.
        /// </summary>
        [JsonProperty("type")]
        public string Type => this.GetType().Name;
        /// <summary>
        /// Gets or sets the active flag for this code element.
        /// </summary>
        [JsonProperty("isActive")]
        public virtual bool IsActive { get; set; }


        /// <summary>
        /// Executes this code element.
        /// </summary>
        /// <param name="state"></param>
        public void Execute(GameState state)
        {
            if (IsActive)
            {
                OnExecute(state);
            }
        }

        abstract protected void OnExecute(GameState state);
    }
}
