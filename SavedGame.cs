using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    public class SavedGame
    {
        public int x, y, possibleLife, life, goldKeys, bells;
        public List<mapChange> mapChanges;
        public List<storedWeapon> weapons;
        public List<storedDictionaryEntry> globalFlags;
        public bool[] beaten;
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
