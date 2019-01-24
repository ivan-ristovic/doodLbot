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

        protected override bool OnExecute(GameState state, Hero hero)
        {
            return Target(state, hero);
        }

        protected bool Target(GameState state, Hero hero)
        {

            if (!state.Enemies.Any()) {
                return false;
            }

            var closest = state.Enemies.OrderBy(e => e.SquaredDist(hero)).First();
            double rotationToClosest = Math.Atan2(closest.Ypos - hero.Ypos, closest.Xpos - hero.Xpos);
            double rotAmount = Math.Abs(-rotationToClosest - hero.Rotation) % (2*Math.PI);
            if (rotAmount > Design.RotateAmount * Design.Delta) {
                //hero.Rotation += (rotationToClosest > hero.Rotation) ? Design.RotateAmount : -Design.RotateAmount;
                var side = rotationToClosest > 0 ? ConsoleKey.D : ConsoleKey.A;
                hero.UpdateSyntheticControls(side, true);
                return false;
            } else {
                hero.Rotation = rotationToClosest;
                return true;
            }
        }
    }
}
