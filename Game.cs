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
        public int life = 5;
        public int possibleLife = 10;
        public int goldKeys = 0;
        AdventureScreen currentAdventure;
        MapScreen currentMap;

        public Game(Master master)
        {
            this.master = master;
        }

        public Screen Begin()
        {
            currentMap = new MapScreen(this);
            return currentMap;
        }

        public void enterAdventureFromMap(int dest, int destroomX, int destroomY, int destx, int desty)
        {
            AdventureScreen aS = new AdventureScreen(this, dest, destroomX, destroomY, destx, desty);
            aS.fromMap = true;
            currentAdventure = aS;
            master.UpdateScreen(aS);
            PlaySound.Enter();
        }

        public void exitAdventure()
        {
            if (currentAdventure.fromMap)
            {
                master.UpdateScreen(currentMap);
                currentAdventure = null;
            }
            if (life == 0)
                life = 5;
        }
    }
}
