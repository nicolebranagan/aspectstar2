using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    public class BestiaryEntry
    {
        public string name = "";
        public int graphicsRow;
        public int speed;
        public int decisiveness;
        public int intelligence;
        public int health = 1;

        public int xOffset = 16;
        public int yOffset = 40;

        public int width = 10;
        public int height = 6;

        public bool ghost;
        public string dependent = "";

        public MovementTypes movementType = MovementTypes.random;
        public bool wanderer;

        public enum MovementTypes
        {
            stationary = 0,
            random = 1,
            intelligent = 2
        }
    }
}
