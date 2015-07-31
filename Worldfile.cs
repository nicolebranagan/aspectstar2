using Microsoft.Xna.Framework;
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

        public Adventure Clone()
        {
            Adventure newAdv = new Adventure();
            newAdv.tileset = tileset;
            newAdv.key = new int[key.Length];
            key.CopyTo(newAdv.key, 0);

            for (int i = 0; i < 16; i++)
                for (int j = 0; j < 16; j++)
                {
                    if (rooms[i,j] != null)
                        newAdv.rooms[i,j] = rooms[i,j].Clone();
                }

            return newAdv;
        }
    }

    public class Room
    {
        public int[] tileMap;
        public List<StoredObject> storedObjects;
        public List<AdventureObject> adventureObjects; 

        public Room Clone()
        {
            Room newRoom = new Room();

            newRoom.tileMap = new int[tileMap.Length];
            tileMap.CopyTo(newRoom.tileMap, 0);

            newRoom.adventureObjects = new List<AdventureObject>();
            foreach (StoredObject sO in storedObjects)
                newRoom.adventureObjects.Add(sO.getAdventureObject());

            return newRoom;
        }
    }

    public class StoredObject
    {
        public float x, y;
        public ObjectType type;

        public enum ObjectType
        {
            key = 0,
        }

        public AdventureObject getAdventureObject()
        {
            switch (type)
            {
                default:
                    AdventureKey key = new AdventureKey();
                    key.location = new Vector2(x, y);
                    return key;
            }
        }
    }
}
