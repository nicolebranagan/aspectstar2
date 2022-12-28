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
        public int goldKeys;
        public int bells;
        public int deaths;
        AdventureScreen currentAdventure;
        MapScreen currentMap;

        public bool hasTorch;
        public bool[] beaten;
        public bool[] crystalKeys = { false, false, false, false, false, false, false, false };
        public int crystalKeyCount
        {
            get
            {
                return crystalKeys.Count(o => o);
            }
            private set {; }
        }
        public int[] top;
        int dest;

        public Weapon weaponA;
        public Weapon weaponB;
        public List<Weapon> weapons;

        public Dictionary<string, bool> globalFlags = new Dictionary<string, bool>();

        public Game(Master master)
        {
            this.master = master;

            top = new int[Master.currentFile.specialStages.Count];

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

        public void Begin()
        {
            currentMap = new MapScreen(this);
            master.UpdateScreen(currentMap);
        }

        public Screen BeginFromSaved(SavedGame sG)
        {
            MapScreen mS = new MapScreen(this);

            mS.ApplyChanges(sG.mapChanges);
            mS.LocalTeleport(sG.x, sG.y);

            foreach (var stored in sG.globalFlags)
                globalFlags.Add(stored.flag, stored.value);

            weapons = new List<Weapon>();
            foreach (var stored in sG.weapons)
            {
                Weapon w = Weapon.unpackWeapon(stored);
                weapons.Add(w);
                if (w is TorchWeapon)
                    hasTorch = true;
            }

            if (weapons.Count > 0)
                weaponA = weapons[0];
            else
                weaponA = new NullWeapon();

            if (weapons.Count > 1)
                weaponB = weapons[1];
            else
                weaponB = new NullWeapon();

            if (sG.weaponA != -1)
                weaponA = weapons[sG.weaponA];
            if (sG.weaponB != -1)
                weaponB = weapons[sG.weaponB];

            if (sG.beaten.Length < Master.currentFile.adventures.Count)
            {
                beaten = new bool[Master.currentFile.adventures.Count];
                for (int i = 0; i < beaten.Length; i++)
                {
                    beaten[i] = i < sG.beaten.Length &&sG.beaten[i];
                }
            }
            else
                beaten = sG.beaten;

            if (sG.top.Length < Master.currentFile.specialStages.Count)
            {
                top = new int[Master.currentFile.specialStages.Count];
                for (int i = 0; i < sG.top.Length; i++)
                    top[i] = sG.top[i];
            }
            else
                top = sG.top;

            goldKeys = sG.goldKeys;
            bells = sG.bells;
            crystalKeys = sG.crystalKeys;
            deaths = sG.deaths;

            if (sG.possibleLife > possibleLife)
                possibleLife = sG.possibleLife;
            if (sG.life > life)
                life = sG.life;

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
            PlaySound.Play(PlaySound.SoundEffectName.Enter);
        }

        public void exitAdventure(bool beat, int dest, int destroomX, int destroomY, int destx, int desty)
        {
            if (dest != this.dest)
            {
                beaten[this.dest] = beat;

                if (dest == -1)
                {
                    if (destx != -1 && desty != -1)
                        currentMap.LocalTeleport(destx, desty);
                    master.UpdateScreen(currentMap);
                    PlaySong.Play(PlaySong.SongName.WorldMap);
                    currentAdventure = null;
                }
                else
                {
                    AdventureScreen aS = new AdventureScreen(this, dest, destroomX, destroomY, destx, desty, beaten[dest]);
                    this.dest = dest;
                    aS.fromMap = false;
                    currentAdventure = aS;
                    master.UpdateScreen(aS);
                    PlaySound.Play(PlaySound.SoundEffectName.Enter);
                }

                if (life == 0)
                {
                    deaths = deaths + 1;
                    life = (possibleLife - 5);
                }
            }
            else
            {
                // same adventure
                currentAdventure.EnterNewRoom(destroomX, destroomY, destx, desty);
            }
        }

        public void warpAdventure(bool beat)
        {
            exitAdventure(beat, -1, 0, 0, -1, -1);
        }

        public void enterSpecialStageFromAdventure(int stage, int key)
        {
            master.UpdateScreen(new SpecialScreen(this, stage, key,
                x =>
                {
                    currentAdventure.FadeIn();
                    master.UpdateScreen(currentAdventure);
                }));
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

                if (newWeapon is TorchWeapon)
                    hasTorch = true;
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
            items.Add(new AdventureBell());
            if (hasTorch && currentAdventure.adventure.hasDarkRooms)
                items.Add(new AdventureTorch());
            else
                items.Add(new AdventureHeart());
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

            game.globalFlags = new List<storedDictionaryEntry>();
            foreach (var f in globalFlags)
            {
                storedDictionaryEntry sD = new storedDictionaryEntry();
                sD.flag = f.Key;
                sD.value = f.Value;
                game.globalFlags.Add(sD);
            }

            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i] == weaponA)
                    game.weaponA = i;
                if (weapons[i] == weaponB)
                    game.weaponB = i;
            }

            game.beaten = beaten;

            game.goldKeys = goldKeys;
            game.crystalKeys = crystalKeys;
            game.top = top;
            game.bells = bells;
            game.life = life;
            game.possibleLife = possibleLife;
            game.deaths = deaths;

            master.SaveGame(game);
        }

        public void enterCredits()
        {
            PlaySong.Play(PlaySong.SongName.Credits);
            master.UpdateScreen(new TextScreen(this, Master.currentFile.credits,
                delegate (bool x)
                {
                    if (!x)
                        master.UpdateScreen(new TitleScreen(master));
                },
                true));
        }
    }
}
