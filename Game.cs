using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    public class Game
    {
        // The game class. Maintains all information about the current game.

        public Master master { get; private set; }
        public int life = 5;
        public int possibleLife = 10;
        public int goldKeys = 0;
        public int bells = 0;
        AdventureScreen currentAdventure;
        MapScreen currentMap;

        public bool[] beaten;
        int dest;

        public Weapon weaponA;
        public Weapon weaponB;
        public List<Weapon> weapons;

        public Game(Master master)
        {
            this.master = master;

            beaten = new bool[Master.currentFile.adventures.Count];
            for (int i = 0; i < beaten.Length; i++)
            {
                beaten[i] = false;
            }
            weapons = new List<Weapon>();
            weapons.Add(new JumpWeapon());

            weaponA = weapons[0];
            weaponB = new NullWeapon();
        }

        public Screen Begin()
        {
            currentMap = new MapScreen(this);
            return currentMap;
        }

        public Screen BeginFromSaved(SavedGame sG)
        {
            MapScreen mS = new MapScreen(this);

            mS.ApplyChanges(sG.mapChanges);
            mS.LocalTeleport(sG.x, sG.y);

            weapons = new List<Weapon>();
            foreach (var stored in sG.weapons)
                weapons.Add(Weapon.unpackWeapon(stored));

            if (weapons.Count > 0)
                weaponA = weapons[0];
            else
                weaponA = new NullWeapon();

            if (weapons.Count > 1)
                weaponB = weapons[1];
            else
                weaponB = new NullWeapon();

            if (sG.beaten.Length < Master.currentFile.adventures.Count)
            {
                beaten = new bool[Master.currentFile.adventures.Count];
                for (int i = 0; i < beaten.Length; i++)
                {
                    if (i < sG.beaten.Length)
                        beaten[i] = sG.beaten[i];
                    else
                        beaten[i] = false;
                }
            }
            else
                beaten = sG.beaten;

            goldKeys = sG.goldKeys;
            bells = sG.bells;

            if (sG.possibleLife > possibleLife)
                possibleLife = sG.possibleLife;
            if (sG.life > life)
                sG.life = life;

            currentMap = mS;
            return mS;
        }


        public void enterAdventureFromMap(int dest, int destroomX, int destroomY, int destx, int desty)
        {
            AdventureScreen aS = new AdventureScreen(this, dest, destroomX, destroomY, destx, desty, beaten[dest]);
            this.dest = dest;
            aS.fromMap = true;
            currentAdventure = aS;
            master.UpdateScreen(aS);
            PlaySound.Enter();
        }

        public void exitAdventure(bool beat, int dest, int destroomX, int destroomY, int destx, int desty)
        {
            beaten[this.dest] = beat;

            if (dest == -1)
            {
                if (destx != -1 && desty != -1)
                    currentMap.LocalTeleport(destx, desty);
                master.UpdateScreen(currentMap);
                currentAdventure = null;
            }
            else
            {
                AdventureScreen aS = new AdventureScreen(this, dest, destroomX, destroomY, destx, desty, beaten[dest]);
                this.dest = dest;
                aS.fromMap = false;
                currentAdventure = aS;
                master.UpdateScreen(aS);
                PlaySound.Enter();
            }

            if (life == 0)
                life = 5;
        }

        public void warpAdventure(bool beat)
        {
            exitAdventure(beat, -1, 0, 0, -1, -1);
        }

        public void Pause()
        {
            master.UpdateScreen(new PauseScreen(currentAdventure, this));
        }

        public void Unpause()
        {
            master.UpdateScreen(currentAdventure);
        }

        public void GetWeapon(Weapon newWeapon)
        {
            bool condition = false;
            foreach (Weapon w in weapons)
                if (w.GetType() == newWeapon.GetType())
                {
                    condition = true;
                    w.Extra(newWeapon);
                }
            
            if (!condition)
            {
                weapons.Add(newWeapon);
                if (weaponA is NullWeapon)
                    weaponA = newWeapon;
                else if (weaponB is NullWeapon)
                    weaponB = newWeapon;
            }
        }

        public void RemoveWeapon(Weapon oldWeapon)
        {
            weapons.RemoveAll(o => o.GetType() == oldWeapon.GetType());

            if (weaponA == oldWeapon)
                weaponA = new NullWeapon();
            else if (weaponB == oldWeapon)
                weaponB = new NullWeapon();
        }

        public AdventureItem getRandomItem()
        {
            List<AdventureItem> items = new List<AdventureItem>();
            items.Add(new AdventureHeart());
            items.Add(new AdventureHeart());
            items.Add(new AdventureBell());
            return items[Master.globalRandom.Next(items.Count)];
        }

        public void saveGame()
        {
            SavedGame game = new SavedGame();
            game.x = (int)(Math.Floor(currentMap.playerloc.X / 32));
            game.y = (int)(Math.Floor(currentMap.playerloc.Y / 32));
            game.mapChanges = new List<mapChange>();
            foreach (var mC in currentMap.mapChanges)
                game.mapChanges.Add(mC);

            game.weapons = new List<storedWeapon>();
            foreach (var w in weapons)
                game.weapons.Add(w.packWeapon());

            game.beaten = beaten;

            game.goldKeys = goldKeys;
            game.bells = bells;
            game.life = life;
            game.possibleLife = possibleLife;

            master.SaveGame(game);
        }
    }
}
