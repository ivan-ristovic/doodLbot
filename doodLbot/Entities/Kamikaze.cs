using doodLbot.Logic;

namespace doodLbot.Entities
{
    /// <summary>
    /// Represents an Enemy that moves towards a hero and tries to collide with him.
    /// </summary>
    public class Kamikaze : Enemy
    {
        public Kamikaze() : base(max_hp: 60, hp: 60, damage: 20, speed: 2 * Design.EnemySpeed, reward: 10)
        {

        }
    }
}
