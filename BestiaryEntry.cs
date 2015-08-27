using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    public class BestiaryEntry
    {
        public string name = "";
        public int graphicsRow = 0;
        public int speed = 0;
        public int decisiveness = 0;
        public int intelligence = 0;
        public int health = 1;

        public int xOffset = 16;
        public int yOffset = 40;

        public int width = 10;
        public int height = 6;

        public bool ghost = false;

        public MovementTypes movementType = MovementTypes.stationary;

        public enum MovementTypes
        {
            stationary = 0,
            random = 1,
            intelligent = 2
        }
    }
}
