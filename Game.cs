﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    class Game
    {
        // The game class. Maintains all information about the current game.

        public Master master { get; private set; }
        public int life = 5;
        public int possibleLife = 10;
        AdventureScreen currentAdventure;

        public Game(Master master)
        {
            this.master = master;
        }

        public Screen Begin()
        {
            return new MapScreen(this);
        }

        public void enterAdventureFromMap(int dest, int destroomX, int destroomY, int destx, int desty)
        {
            AdventureScreen aS = new AdventureScreen(this, dest, destroomX, destroomY, destx, desty);
            currentAdventure = aS;
            master.UpdateScreen(aS);
        }
    }
}
