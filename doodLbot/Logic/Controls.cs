using System;

namespace doodLbot.Logic
{
    /// <summary>
    /// Represent a set of player controls.
    /// </summary>
    public class Controls
    {
        public bool IsFire { get; private set; }
        public bool IsForward { get; private set; }
        public bool IsBackward { get; private set; }
        public bool IsLeft { get; private set; }
        public bool IsRight { get; private set; }


        /// <summary>
        /// Action executed when the key is pressed or released.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isDown"></param>
        public void OnKey(ConsoleKey key, bool isDown)
        {
            switch (key) {
                case ConsoleKey.A:
                    this.IsLeft = isDown;
                    break;
                case ConsoleKey.S:
                    this.IsBackward = isDown;
                    break;
                case ConsoleKey.D:
                    this.IsRight = isDown;
                    break;
                case ConsoleKey.W:
                    this.IsForward = isDown;
                    break;
                case ConsoleKey.Spacebar:
                    this.IsFire = isDown;
                    break;
            }
        }
    }
}
