using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace aspectstar2
{
    public class Worldfile
    {
        public Mapfile map;
        public List<Adventure> adventures;
    }

    public class Mapfile
    {
        public const int width = 100;
        public const int height = 100;
        public int[] tileMap;
        public int[] key;
        public int startX;
        public int startY;
        public List<MapObject> objects;
    }

    public class Adventure
    {
        public int tileset;
        public int[] key;
        public Room templateRoom;

        [XmlIgnore]
        public Room[,] rooms = new Room[16,16];

        // XmlSerializer does not support multidimensional arrays, this is the next best thing
        public Room[] xmlRooms
        {
            get
            {
                Room[] xmlList = new Room[16 * 16];
                int x, y;
                for (int i = 0; i < (16 * 16); i++)
                {
                    x = i % 16;
                    y = i / 16;
                    xmlList[i] = rooms[x, y];
                }
                return xmlList;
            }

            set
            {
                int x, y;
                for (int i = 0; i < (16 * 16); i++)
                {
                    x = i % 16;
                    y = i / 16;
                    if (value[i] != null)
                        rooms[x, y] = value[i];
                }
            }
        }
    }

    public class Room
    {
        public int[] tileMap;
    }
}
