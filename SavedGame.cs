using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    public class SavedGame
    {
        public int x, y, possibleLife, life, goldKeys, bells, deaths;
        public List<mapChange> mapChanges;
        public List<storedWeapon> weapons;
        public List<storedDictionaryEntry> globalFlags;
        public bool[] beaten;
        public bool[] crystalKeys;
        public int[] top;
    }

    public struct mapChange
    {
        public int i, tile;
    }

    public struct storedWeapon
    {
        public int type, count;
    }

    public struct storedDictionaryEntry
    {
        public string flag;
        public bool value;
    }
}
