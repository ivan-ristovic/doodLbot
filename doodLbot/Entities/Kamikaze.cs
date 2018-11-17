using doodLbot.Logic;

namespace doodLbot.Entities
{
    /// <summary>
    /// Represents an Enemy that moves towards a hero and tries to collide with him.
    /// </summary>
    public class Kamikaze : Enemy
    {
        public Kamikaze() : base()
        {
            this.Speed = Design.EnemySpeed;
            this.Damage = 20;
        }
    }
}
