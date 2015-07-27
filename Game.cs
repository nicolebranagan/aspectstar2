using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    class Game
    {
        // The game class. Maintains all information about the current game.

        public Master master { get; private set; }

        public Game(Master master)
        {
            this.master = master;
        }

        public Screen Begin()
        {
            return new MapScreen(this);
        }
    }
}
