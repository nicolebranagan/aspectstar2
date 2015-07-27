using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    public class Worldfile
    {
        public Mapfile map;
        public List<Adventurefile> adventures;
    }

    public class Mapfile
    {
        public const int width = 100;
        public const int height = 100;
        public int[] tileMap;
        public int[] key;
    }

    public class Adventurefile
    {
        public int id;
        public List<Roomfile> rooms;
    }

    public class Roomfile
    {
        public int id;
    }
}
