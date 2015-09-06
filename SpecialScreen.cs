using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace aspectstar2
{
    public class SpecialScreen : Screen
    {
        public const int width = Master.width - (32 * 6);
        public const int height = Master.height;

        SpecialPlayer player;
        List<SpecialObject> objects = new List<SpecialObject>();
        List<SpecialObject> newobjects = new List<SpecialObject>();
        List<StoredSpecial> potential = new List<StoredSpecial>();

        float yoffset = 0;

        int[] currentMap;
        int levelheight;

        public Vector2 playerloc
        {
            get { return player.location; }
            private set {; }
        }

        int lag = 0;

        public SpecialScreen(int stage)
        {
            currentMap = Master.currentFile.specialStages[stage].tileMap;
            levelheight = Master.currentFile.specialStages[stage].height;

            player = new SpecialPlayer(this);
            yoffset = levelheight * 32;
            
            objects.Add(player);
            foreach (StoredSpecial sS in Master.currentFile.specialStages[stage].objects)
            {
                potential.Add(sS.Clone());
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Rectangle source, dest;
            int x, y, screenOffset, limitOffset;
            Vector2 sourceTile;
            source = new Rectangle(0, 0, 32, 32);
            int stageheight = levelheight * 32;
            
            screenOffset = (int)yoffset - (Master.height);

            spriteBatch.Begin();
            for (int i = 0; i < currentMap.Length; i++)
            {
                x = i % (width / 32);
                y = i / (width / 32) % stageheight;
                limitOffset = (int)Math.Floor((decimal)screenOffset / 32);
                if ((y >= limitOffset) && (y <= limitOffset + Master.height))
                {
                    sourceTile = Master.getMapTile(currentMap[x + y * (width / 32)], Master.texCollection.worldTiles);
                    source = new Rectangle((int)sourceTile.X, (int)sourceTile.Y, 32, 32);
                    dest = new Rectangle(x * 32, (y * 32 - (int)screenOffset), 32, 32);
                    spriteBatch.Draw(Master.texCollection.specialTiles, dest, source, Color.White);
                }
            }
            spriteBatch.End();

            List<SpecialObject> sortedList = objects.OrderBy(o => o.location.Y).ToList();

            foreach (SpecialObject obj in sortedList)
                obj.Draw(spriteBatch, Color.White);
        }

        public override void Update(GameTime gameTime)
        {

            objects.AddRange(newobjects);
            newobjects = new List<SpecialObject>();

            foreach (SpecialObject obj in objects)
                obj.Update();

            objects.RemoveAll(x => x.active == false);

            foreach (SpecialProjectile sP in objects.Where(x => x is SpecialProjectile))
            {
                foreach (SpecialObject sO in objects)
                {
                    if (sO != sP && (!sP.friendly || !(sO is SpecialPlayer)) && (sP.friendly || !(sO is SpecialEnemy)) &&
                        (Math.Pow(sO.location.X - sP.location.X, 2) + Math.Pow(sO.location.Y - sP.location.Y, 2) < sO.radius))
                    {
                        sP.active = false;
                        sO.Hurt();
                    }
                }
            }

            int movedist = 4;
            if (Master.controls.B)
                movedist = 2;

            if (Master.controls.Up)
                player.Move(new Vector2(0, -movedist));
            else if (Master.controls.Down)
                player.Move(new Vector2(0, movedist));
            else if (Master.controls.Left)
                player.Move(new Vector2(-movedist, 0));
            else if (Master.controls.Right)
                player.Move(new Vector2(movedist, 0));

            if (Master.controls.A && lag == 0)
            {
                newobjects.Add(new SpecialProjectile(this, player.location, new Vector2(0, -4), true));
                PlaySound.Pew();
                lag = 10;
            }

            if (lag > 0)
                lag = lag - 1;

            if (yoffset > Master.height)
                yoffset = yoffset - (float)0.5;

            if (potential.Count > 0)
            {
                foreach (StoredSpecial sS in potential.Where(o => o.y > (yoffset - Master.height - 32)))
                {
                    objects.Add(sS.getEnemy(this));
                    sS.used = true;
                }
                potential.RemoveAll(o => o.used == true);
            }
        }

        public void addObject(SpecialObject obj)
        {
            newobjects.Add(obj);
        }
    }
}
