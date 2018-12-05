using doodLbot.Logic;

namespace doodLbot.Entities
{
    /// <summary>
    /// Represents an Enemy with extra HP that moves towards a hero and tries to collide with him.
    /// </summary>
    public class Tank : Enemy
    {
        public Tank() : base(speed: Design.EnemySpeed, hp: 1000, damage: 5)
        {

        }
    }
}
