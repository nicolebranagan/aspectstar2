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
    public abstract class MapObject
    {
        public int x, y;
    }

    public class MapTeleporter : MapObject
    {
        public int dest, destroomX, destroomY, destx, desty; // if dest = -1, stay on map
    }
}
