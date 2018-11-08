using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doodLbot.Logic
{
    public class Controls
    {
        public bool IsFire { get; private set; } = false;
        public bool IsForward { get; private set; } = false;
        public bool IsBackward { get; private set; } = false;
        public bool IsLeft { get; private set; } = false;
        public bool IsRight { get; private set; } = false;

        public void OnKey(ConsoleKey key, bool isDown)
        {
            switch (key)
            {
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
