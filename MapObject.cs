using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace aspectstar2
{
    [XmlInclude(typeof(MapTeleporter))]
    [XmlInclude(typeof(MapLock))]
    public abstract class MapObject
    {
        protected Game game;
        protected MapScreen parent;

        public int x, y;

        public void Initialize(MapScreen parent, Game game)
        {
            this.parent = parent;
            this.game = game;
        }

        public abstract void Activate()
            ;
    }

    public class MapTeleporter : MapObject
    {
        public int dest, destroomX, destroomY, destx, desty; // if dest = -1, stay on map

        public override void Activate()
        {
            if (dest == -1)
                parent.LocalTeleport(destx, desty);
            else
                game.enterAdventureFromMap(dest, destroomX, destroomY, destx, desty);
        }
    }

    public class MapLock : MapObject
    {
        public int tile;
        public bool active = true;

        public override void Activate()
        {
            if (parent.tileSolid(x, y))
            {
                if (game.goldKeys > 0)
                {
                    PlaySound.Play(PlaySound.SoundEffectName.Aspect);
                    game.goldKeys = game.goldKeys - 1;
                    active = false;
                    parent.ChangeTile(x, y, tile);
                }
            }
            else
            {
                active = false;
            }
        }
    }
}
