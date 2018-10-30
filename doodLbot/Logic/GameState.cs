using doodLbot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doodLbot.Logic
{
    public sealed class GameState
    {
        public Hero Hero { get; }
        public IReadOnlyList<Enemy> Enemies => this.enemies.AsReadOnly();

        private readonly List<Enemy> enemies;


        public GameState(Hero hero, IEnumerable<Enemy> enemies)
        {
            this.Hero = hero;
            this.enemies = enemies.ToList();
        }
    }
}
