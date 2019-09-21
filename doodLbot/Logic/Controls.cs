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

        private readonly ConsoleKey fireKey;
        private readonly ConsoleKey forwardKey;
        private readonly ConsoleKey backwardKey;
        private readonly ConsoleKey leftKey;
        private readonly ConsoleKey rightKey;

        public Controls(ConsoleKey fireKey, ConsoleKey forwardKey, ConsoleKey backwardKey,
            ConsoleKey leftKey, ConsoleKey rightKey)
        {
            this.fireKey = fireKey;
            this.forwardKey = forwardKey;
            this.backwardKey = backwardKey;
            this.leftKey = leftKey;
            this.rightKey = rightKey;
        }

        public void Wipe()
        {
            IsFire = false;
            IsForward = false;
            IsBackward = false;
            IsLeft = false;
            IsRight = false;
        }

        public bool IsMoving => IsLeft || IsRight || IsBackward || IsForward;

        /// <summary>
        /// Action executed when the key is pressed or released.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isDown"></param>
        public void OnKey(ConsoleKey key, bool isDown)
        {
            if (key == leftKey)
                IsLeft = isDown;
            else if (key == backwardKey)
                IsBackward = isDown;
            else if (key == rightKey)
                IsRight = isDown;
            else if (key == forwardKey)
                IsForward = isDown;
            else if (key == fireKey)
                IsFire = isDown;
        }
    }
}
