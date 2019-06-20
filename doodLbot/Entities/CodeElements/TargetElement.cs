using System;
using System.Linq;
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
            Cost = Design.CostTarget;
        }

        protected override bool OnExecute(GameState state, Hero hero)
        {
            return Target(state, hero);
        }

        protected bool Target(GameState state, Hero hero)
        {

            if (!state.Enemies.Any())
            {
                return false;
            }

            var closest = state.Enemies.OrderBy(e => e.SquaredDist(hero)).First();
            var rotationToClosest = Math.Atan2(closest.Ypos - hero.Ypos, closest.Xpos - hero.Xpos);
            var rotAmount = Math.Abs(rotationToClosest - hero.Rotation) % (2 * Math.PI);
            if (rotAmount > Design.RotateAmount * Design.Delta)
            {
                // convert to [-Pi, Pi] so that rotation direction can be known
                var rotMinusPiToPi = (rotationToClosest - hero.Rotation) % (2 * Math.PI);
                rotMinusPiToPi = rotMinusPiToPi > Math.PI ? -rotMinusPiToPi + Math.PI :
                    rotMinusPiToPi < -Math.PI ? -rotMinusPiToPi - Math.PI : rotMinusPiToPi;
                var side = rotMinusPiToPi > 0 ? ConsoleKey.D : ConsoleKey.A;
                hero.UpdateSyntheticControls(side, true);
                return false;
            }
            else
            {
                hero.Rotation = rotationToClosest;
                return true;
            }
        }
    }
}
