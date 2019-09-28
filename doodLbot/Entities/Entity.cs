using System;
using Newtonsoft.Json;

namespace doodLbot.Entities
{
    /// <summary>
    /// Represents an entity present in the game.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Get the X position of this entity.
        /// </summary>
        [JsonProperty("x")]
        public double Xpos { get; protected set; }

        /// <summary>
        /// Get the Y position of this entity.
        /// </summary>
        [JsonProperty("y")]
        public double Ypos { get; protected set; }

        /// <summary>
        /// Get the rotation angle of this entity.
        /// </summary>
        [JsonProperty("rotation")]
        public double Rotation { get; set; }

        /// <summary>
        /// Get the velocity X component of this entity.
        /// </summary>
        [JsonProperty("vx")]
        public double Xvel { get; set; }

        /// <summary>
        /// Get the velocity Y component of this entity.
        /// </summary>
        [JsonProperty("vy")]
        public double Yvel { get; set; }

        /// <summary>
        /// Get this entity's health.
        /// </summary>
        [JsonProperty("hp")]
        public double Hp { get; protected set; }

        /// <summary>
        /// Get this entity's health.
        /// </summary>
        [JsonProperty("max_hp")]
        public double MaxHp { get; protected set; }

        /// <summary>
        /// Get this entity's damage.
        /// </summary>
        [JsonProperty("damage")]
        public double Damage { get; protected set; }

        /// <summary>
        /// Get this entity's speed.
        /// </summary>
        [JsonProperty("speed")]
        public double Speed { get; protected set; }

        /// <summary>
        /// Constructs a new Entity with default health and damage points.
        /// </summary>
        public Entity(double hp = 100, double max_hp = 100, double damage = 1, double speed = 0, double rotation = 0)
        {
            Speed = speed;
            Hp = hp;
            MaxHp = max_hp;
            Damage = damage;
            Rotation = rotation;
        }

        /// <summary>
        /// Constructs a new Entity at a given location.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rotation">Entity's rotation amount (in radians)</param>
        public Entity(double x, double y, double speed = 0, double hp = 100, double max_hp = 100, double damage = 1, double rotation = 0, int reward = 10) 
            : this(hp, max_hp, damage, speed, rotation)
        {
            Xpos = x;
            Ypos = y;
        }


        /// <summary>
        /// Move this entity along the velocity vector.
        /// </summary>
        /// <param name="delta">elpased time, 1 if it's exact as tick rate,
        /// less if faster, more if slower</param>
        public void Move(double delta)
        {
            Xpos += Xvel * delta;
            Ypos += Yvel * delta;
            OnMove();
        }

        /// <summary>
        /// Action that is executed whenever the entity is moved, i.e whenever <code>Move()</code> is called.
        /// </summary>
        protected virtual void OnMove()
        {

        }

        /// <summary>
        /// Direct this entity to move towards another entity using given speed.
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="withSpeed"></param>
        public void VelocityTowards(Entity goal, double? withSpeed = null)
        {
            var xvel = goal.Xpos - Xpos;
            var yvel = goal.Ypos - Ypos;
            var norm = Math.Sqrt(xvel * xvel + yvel * yvel);
            if (Math.Abs(norm) < 0.001)
            {
                Xvel = 0;
                Yvel = 0;
                return;
            }
            var speed = withSpeed ?? Speed;
            Xvel = xvel / norm * speed;
            Yvel = yvel / norm * speed;
        }

        /// <summary>
        ///  TODO
        /// </summary>
        /// <param name="value"></param>
        public virtual void DecreaseHealthPoints(double value)
        {
            var newHp = Hp - value;
            Hp = newHp > 0 ? newHp : 0;
        }

        /// <summary>
        /// Checks if this Entity is out of bounds specified by a Cartessian rectangle [(0, 0), (X, Y)].
        /// </summary>
        /// <param name="bounds">Bounding rectangle diagonal ending point.</param>
        /// <returns>Indicator if entity is out of bounds.</returns>
        public bool IsOutsideBounds((double X, double Y) bounds)
        {
            return Xpos < 0 || Xpos > bounds.X || Ypos < 0 || Ypos > bounds.Y;
        }

        /// <summary>
        /// calculates squared distance between entities
        /// </summary>
        /// <param name="e"></param>
        /// <returns>squared distance</returns>
        public double SquaredDist(Entity e)
        {
            var x = e.Xpos - Xpos;
            var y = e.Ypos - Ypos;
            return x * x + y * y;
        }
    }
}
