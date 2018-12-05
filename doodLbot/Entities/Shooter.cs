using doodLbot.Logic;

namespace doodLbot.Entities
{
    /// <summary>
    /// Represents an Enemy with extra HP that moves towards a hero and tries to collide with him.
    /// </summary>
    public class Shooter : Enemy
    {

        public Shooter() : base(hp: 150, damage: 10, speed: Design.EnemySpeed)
        {

        }

    }
}
