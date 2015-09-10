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
        int score;

        int animCount = 200;

        int lag = 0;
        Action<bool> leaver;

        SpecialModes _currentMode = SpecialModes.runMode;
        int modeLag = 20;

        SpecialModes currentMode
        {
            get { return _currentMode; }
            set { if (modeLag == 0)
                {
                    if (value == SpecialModes.Paused)
                        PlaySound.Pause();
                    _currentMode = value;
                    modeLag = 20;
                }
            }
        }

        enum SpecialModes
        {
            runMode,
            Paused,
            DeathAnim,
        }

        public Vector2 playerloc
        {
            get { return player.location; }
            private set {; }
        }

        public SpecialScreen(int stage, Action<bool> leaver)
        {
            this.leaver = leaver;

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
            Color levelMask = Color.White;

            if (currentMode == SpecialModes.Paused)
                levelMask = Color.DarkOrchid;

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
                    spriteBatch.Draw(Master.texCollection.specialTiles, dest, source, levelMask);
                }
            }
            spriteBatch.End();

            if (currentMode == SpecialModes.runMode || (currentMode == SpecialModes.DeathAnim))
            {
                List<SpecialObject> sortedList = objects.OrderBy(o => o.location.Y).ToList();

                foreach (SpecialObject obj in sortedList)
                    obj.Draw(spriteBatch, Color.White);
            }

            DrawStatusBar(spriteBatch);
        }

        void DrawStatusBar(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Master.texCollection.blank, new Rectangle(width, 0, Master.width - width, height), Color.Black);
            for (int i = 0; i < (Master.height / 16); i++)
            {
                spriteBatch.Draw(Master.texCollection.controls, new Rectangle(width, i * 16, 16, 16),
                    new Rectangle(0, 16, 16, 16), Color.White);
            }
            spriteBatch.End();

            if (currentMode == SpecialModes.runMode || (currentMode == SpecialModes.Paused && animCount > 100))
                WriteText(spriteBatch, "SCORE", new Vector2(width + 32, 32), Color.White);
            else
                WriteText(spriteBatch, "PAUSED", new Vector2(width + 32, 32), Color.White);
            WriteText(spriteBatch, score.ToString("000000"), new Vector2(Master.width - 8 * 16, 32 + 16), Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            if (modeLag > 0)
                modeLag = modeLag - 1;

            System.Diagnostics.Debug.WriteLine(animCount);

            switch (currentMode)
            {
                case SpecialModes.Paused:
                    if (Master.controls.Start)
                    {
                        currentMode = SpecialModes.runMode;
                    }
                    if (animCount > 0)
                        animCount = animCount - 1;
                    else
                        animCount = 200;
                    break;
                case SpecialModes.DeathAnim:
                    if (animCount > 0)
                        animCount = animCount - 1;
                    else
                        leaver(false);

                    foreach (SpecialObject obj in objects)
                        obj.Update();

                    break;
                case SpecialModes.runMode:
                    foreach (SpecialObject obj in objects)
                        obj.Update();

                    foreach (SpecialProjectile sP in objects.Where(x => x is SpecialProjectile))
                    {
                        foreach (SpecialObject sO in objects)
                        {
                            double distance = Math.Pow(sO.location.X - sP.location.X, 2) + Math.Pow(sO.location.Y - sP.location.Y, 2);
                            if (sO != sP && (!sP.friendly || !(sO is SpecialPlayer)) && (sP.friendly || !(sO is SpecialEnemy)) &&
                                (distance < Math.Pow(sO.radius, 2)))
                            {
                                sP.active = false;
                                if (sO.Hurt())
                                {
                                    if (distance < Math.Pow(sO.radius / 4, 2))
                                        score += 100;
                                    else if (distance < Math.Pow(sO.radius / 2, 2))
                                        score += 75;
                                    else
                                        score += 50;
                                }
                            }
                        }
                    }

                    if (player.active == false)
                    {
                        _currentMode = SpecialModes.DeathAnim;
                        animCount = 50;
                    }

                    if (Master.controls.Start)
                        currentMode = SpecialModes.Paused;

                    int movedist = 4;
                    if (Master.controls.B)
                        movedist = 2;

                    if (Master.controls.Up)
                        player.Move(new Vector2(0, -movedist));
                    if (Master.controls.Down)
                        player.Move(new Vector2(0, movedist));
                    if (Master.controls.Left)
                        player.Move(new Vector2(-movedist, 0));
                    if (Master.controls.Right)
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
                        yoffset = yoffset - (float)0.4;

                    objects.RemoveAll(x => x.active == false);
                    objects.AddRange(newobjects);
                    newobjects = new List<SpecialObject>();

                    if (potential.Count > 0)
                    {
                        foreach (StoredSpecial sS in potential.Where(o => o.y > (yoffset - Master.height)))
                        {
                            SpecialEnemy enemy = sS.getEnemy(this);
                            enemy.location.Y = sS.y - yoffset + Master.height;
                            objects.Add(enemy);
                            sS.used = true;
                        }
                        potential.RemoveAll(o => o.used == true);
                    }
                    break;
            }
        }

        public void addObject(SpecialObject obj)
        {
            newobjects.Add(obj);
        }
    }
}
