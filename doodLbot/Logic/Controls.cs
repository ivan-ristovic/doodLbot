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

        private ConsoleKey fireKey;
        private ConsoleKey forwardKey;
        private ConsoleKey backwardKey;
        private ConsoleKey leftKey;
        private ConsoleKey rightKey;

        public Controls(ConsoleKey fireKey, ConsoleKey forwardKey, ConsoleKey backwardKey, 
            ConsoleKey leftKey, ConsoleKey rightKey)
        {
            this.fireKey = fireKey;
            this.forwardKey = forwardKey;
            this.backwardKey = backwardKey;
            this.leftKey = leftKey;
            this.rightKey = rightKey;
        }

        /// <summary>
        /// Action executed when the key is pressed or released.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isDown"></param>
        public void OnKey(ConsoleKey key, bool isDown)
        {
            if (key == this.leftKey)
                this.IsLeft = isDown;
            else if (key == this.backwardKey)
                this.IsBackward = isDown;
            else if (key == this.rightKey)
                this.IsRight = isDown;
            else if (key == this.forwardKey)
                this.IsForward = isDown;
            else if (key == this.fireKey)
                this.IsFire = isDown;
        }
    }
}
