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
        public List<BestiaryEntry> bestiary;
        public List<EntityData> stockEntities;
        public List<SpecialStage> specialStages;
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
        public int music = -1;
        public int[] key;
        public Room templateRoom;
        public string name = "DUNGEON";

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
            newAdv.music = music;
            newAdv.name = name.ToUpper();
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
        public int music;
        public int[] tileMap;
        public List<StoredObject> storedObjects;
        public List<AdventureObject> adventureObjects;
        public string code;
        public bool dark;

        public Room Clone()
        {
            Room newRoom = new Room();

            newRoom.music = music;
            newRoom.tileMap = new int[tileMap.Length];
            tileMap.CopyTo(newRoom.tileMap, 0);

            newRoom.adventureObjects = new List<AdventureObject>();
            foreach (StoredObject sO in storedObjects)
                newRoom.adventureObjects.Add(sO.getAdventureObject());

            newRoom.code = code;
            newRoom.dark = dark;

            return newRoom;
        }
    }

    public class StoredObject
    {
        public float x, y;
        public ObjectType type;
        public int enemyType;
        public EntityData data;

        public int dest, destx, desty, destroomX, destroomY;
        public int screen, key;

        public enum ObjectType
        {
            key = 0,
            heart = 1,
            goldkey = 2,
            enemy = 3,
            shooter = 4,
            boss = 5,
            entity = 6,
            teleporter = 7,
            stock = 8,
            special = 9,
        }

        public AdventureObject getAdventureObject()
        {
            switch (type)
            {
                case ObjectType.special:
                    AdventureSpecial special = new AdventureSpecial(screen, key - 1);
                    special.location = new Vector2(x, y);
                    return special;
                case ObjectType.stock:
                    AdventureEntity entityFromStock = new AdventureEntity(Master.currentFile.stockEntities[enemyType]);
                    entityFromStock.location = new Vector2(x, y);
                    return entityFromStock;
                case ObjectType.teleporter:
                    AdventureTeleporter exit = new AdventureTeleporter(dest, destx, desty, destroomX, destroomY);
                    exit.location = new Vector2(x, y);
                    return exit;
                case ObjectType.entity:
                    AdventureEntity entity = new AdventureEntity(data);
                    entity.location = new Vector2(x, y);
                    return entity;
                case ObjectType.boss:
                    if (enemyType == 2)
                    {
                        AdventureBoss2 boss2 = new AdventureBoss2();
                        boss2.location = new Vector2(x, y);
                        return boss2;
                    }
                    else if (enemyType == 3)
                    {
                        AdventureBoss3 boss3 = new AdventureBoss3();
                        boss3.location = new Vector2(x, y);
                        return boss3;
                    }
                    else if (enemyType == 4)
                    {
                        AdventureBoss4 boss4 = new AdventureBoss4();
                        boss4.location = new Vector2(x, y);
                        return boss4;
                    }
                    else if (enemyType == 5)
                    {
                        AdventureBoss5 boss5 = new AdventureBoss5();
                        boss5.location = new Vector2(x, y);
                        return boss5;
                    }
                    else
                    {
                        AdventureBoss1 boss1 = new AdventureBoss1();
                        boss1.location = new Vector2(x, y);
                        return boss1;
                    }
                case ObjectType.shooter:
                    AdventureShooter shooter = new AdventureShooter();
                    shooter.location = new Vector2(x, y);
                    return shooter;
                case ObjectType.enemy:
                    AdventureEnemy enemy = new AdventureEnemy(Master.currentFile.bestiary[enemyType]);
                    enemy.location = new Vector2(x, y);
                    return enemy;
                case ObjectType.goldkey:
                    AdventureGoldKey goldkey = new AdventureGoldKey();
                    goldkey.location = new Vector2(x, y);
                    return goldkey;
                case ObjectType.heart:
                    AdventureHeart heart = new AdventureHeart();
                    heart.location = new Vector2(x, y);
                    return heart;
                default:
                    AdventureKey key_ = new AdventureKey();
                    key_.location = new Vector2(x, y);
                    return key_;
            }
        }
    }


    public struct EntityData
    {
        public string name;
        public string code;
        public int graphics;
        public GraphicsType gfxtype;

        public enum GraphicsType
        {
            Maptile = 0,
            Enemies = 1,
            Characters = 2,
            Null = 3,
        }
    }

    public class SpecialStage
    {
        public int height;
        public int[] tileMap;
        public List<StoredSpecial> objects;
        public WinCondition winCondition = WinCondition.ReachEnd;

        public enum WinCondition
        {
            ReachEnd = 0,
            KillEnemies = 1,
        }
    }

    public class StoredSpecial
    {
        public int row, x, y, shootingrate, behavior, speed, amplitude, time;
        public bool track, used;

        public SpecialEnemy getEnemy(SpecialScreen parent)
        {
            return new SpecialEnemy(parent, row, x, 0, shootingrate, behavior, speed, amplitude, time, track);
        }

        public StoredSpecial Clone()
        {
            StoredSpecial sS = new StoredSpecial();
            sS.row = row;
            sS.x = x;
            sS.y = y;
            sS.shootingrate = shootingrate;
            sS.behavior = behavior;
            sS.speed = speed;
            sS.amplitude = amplitude;
            sS.time = time;
            sS.track = track;
            sS.used = false;
            return sS;
        }
    }
}
