using doodLbot.Logic;

namespace doodLbot.Entities
{
    /// <summary>
    /// Represents an Enemy with extra HP that moves towards a hero and tries to collide with him.
    /// </summary>
    public class Tank : Enemy
    {
        public Tank() : base(max_hp: 300, hp: 300, damage: 10, speed: Design.EnemySpeed)
        {

        }
    }
}
