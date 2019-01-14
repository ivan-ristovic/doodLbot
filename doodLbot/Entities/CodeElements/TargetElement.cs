using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doodLbot.Logic;

namespace doodLbot.Entities.CodeElements
{
    /// <summary>
    /// Represents a targeting action.
    /// </summary>
    public class TargetElement : BaseCodeElement
    {
        public TargetElement()
        {
            this.Cost = Design.CostTarget;
        }

        protected override void OnExecute(GameState state, Hero hero)
        {
            if (!state.Enemies.Any())
            {
                return;
            }

            var closest = state.Enemies.OrderBy(e => e.SquaredDist(hero)).First();
            hero.Rotation = Math.Atan2(closest.Ypos - hero.Ypos, closest.Xpos - hero.Xpos);
        }
    }
}
