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
        Game game;

        public const int width = Master.width - (32 * 6);
        public const int height = Master.height;

        SpecialPlayer player;
        List<SpecialObject> objects = new List<SpecialObject>();
        List<SpecialObject> newobjects = new List<SpecialObject>();
        List<StoredSpecial> potential = new List<StoredSpecial>();

        float yoffset = 0;

        int[] currentMap;
        int levelheight;
        int stage, key;
        int top;

        SpecialStage.WinCondition winCondition;
        int _killCount;
        public int killCount
        {
            get
            {
                return _killCount;
            }
            set
            {
                _killCount = value;
                if (value <= 0 && winCondition == SpecialStage.WinCondition.KillEnemies)
                {
                    Win();
                }
            }
        }

        int animCount = 200;

        int lag = 0;
        Action<bool> leaver;

        int modeLag = 20;
        int introLag = 400;
        string[] introText =
        {
            "PRESS A TO FIRE  B TO SLOW DOWN",
            "COMPLETE LEVEL",
            "TO EARN CRYSTAL KEY"
        };

        int _score;
        int score
        {
            get { return _score; }
            set
            {
                if (value > top)
                    top = value;
                _score = value;
            }
        }

        SpecialModes _currentMode = SpecialModes.runMode;
        SpecialModes currentMode
        {
            get { return _currentMode; }
            set { if (modeLag == 0)
                {
                    if (value == SpecialModes.Paused)
                    {
                        PlaySound.Pause();
                    }
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
            WinAnim,
        }

        public Vector2 playerloc
        {
            get { return player.location; }
            private set {; }
        }

        public SpecialScreen(Game game, int stage, int key, Action<bool> leaver)
        {
            this.game = game;
            this.leaver = leaver;
            this.stage = stage;
            this.key = key;

            currentMap = Master.currentFile.specialStages[stage].tileMap;
            levelheight = Master.currentFile.specialStages[stage].height;
            winCondition = Master.currentFile.specialStages[stage].winCondition;

            switch (winCondition)
            {
                case SpecialStage.WinCondition.KillEnemies:
                    introText[1] = "KILL ALL ENEMIES";
                    break;
            }

            player = new SpecialPlayer(this);
            yoffset = levelheight * 32;
            
            objects.Add(player);

            foreach (StoredSpecial sS in Master.currentFile.specialStages[stage].objects)
            {
                potential.Add(sS.Clone());
            }
            killCount = potential.Count;
            if (game.top[stage] == 0)
                top = potential.Count * 50;
            else
                top = game.top[stage];
            
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

            if ((currentMode == SpecialModes.runMode && introLag > 0 && (introLag / 35) % 3 != 1) || 
                currentMode == SpecialModes.DeathAnim || currentMode == SpecialModes.WinAnim)
                DrawText(spriteBatch, introText, Color.Black);
            else if (currentMode == SpecialModes.Paused)
                DrawText(spriteBatch, introText, Color.White);

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

            if (currentMode == SpecialModes.Paused && animCount < 100)
                WriteText(spriteBatch, "PAUSED", new Vector2(width + 32, 32), Color.White);
            else
                WriteText(spriteBatch, "TOP", new Vector2(width + 32, 32), Color.White);
            WriteText(spriteBatch, top.ToString("000000"), new Vector2(Master.width - 8 * 16, 32 + 16), Color.White);

            WriteText(spriteBatch, "SCORE", new Vector2(width + 32, 64), Color.White);
            WriteText(spriteBatch, score.ToString("000000"), new Vector2(Master.width - 8 * 16, 64 + 16), Color.White);

        }

        void DrawText(SpriteBatch spriteBatch, string[] text, Color textColor)
        {
            for (int i = 0; i < text.Length; i++)
            {
                WriteText(spriteBatch, text[i], new Vector2(width / 2 - (8 * text[i].Length), 32 + i * 32), Color.DarkGray);
                WriteText(spriteBatch, text[i], new Vector2(width / 2 - (8 * text[i].Length) + 2, 32 + i * 32 + 2), textColor);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (modeLag > 0)
                modeLag = modeLag - 1;

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
                        LeaveStage(false);

                    foreach (SpecialObject obj in objects)
                        obj.Update();

                    break;
                case SpecialModes.WinAnim:
                    if (animCount > 0)
                        animCount = animCount - 1;
                    else
                        LeaveStage(true);
                    break;
                case SpecialModes.runMode:
                    if (introLag > 0)
                        introLag--;

                    foreach (SpecialObject obj in objects)
                        obj.Update();

                    foreach (SpecialProjectile sP in objects.Where(x => x is SpecialProjectile))
                    {
                        foreach (SpecialObject sO in objects)
                        {
                            double distance = Vector2.Distance(sO.location, sP.location); // Math.Pow(sO.location.X - sP.location.X, 2) + Math.Pow(sO.location.Y - sP.location.Y, 2);
                            if (sO != sP && (!sP.friendly || !(sO is SpecialPlayer)) && (sP.friendly || !(sO is SpecialEnemy)) &&
                                (distance < sO.radius))
                            {
                                sP.active = false;
                                int y_dist = (int)Math.Abs(sO.location.Y - sP.location.Y);
                                if (sO.Hurt())
                                {
                                    if (y_dist < (sO.radius / 8))
                                        score += 100;
                                    else if (y_dist < (sO.radius / 2))
                                        score += 75;
                                    else
                                        score += 50;
                                }
                            }
                        }
                    }

                    if (player.active == false)
                    {
                        Die();
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

        void LeaveStage(bool win)
        {
            if (key >= 0 && key < game.crystalKeys.Length && !game.crystalKeys[key])
                game.crystalKeys[key] = win;
            game.top[stage] = top;

            leaver(win);
        }

        void Win()
        {
            _currentMode = SpecialModes.WinAnim;
            animCount = 80;
            introLag = 80;
            introText = new string[2];
            introText[0] = "CONGRATULATIONS";
            if (key >= 0)
            {
                introText[1] = "YOU GOT A CRYSTAL KEY";
            }
            else
                introText[1] = "YOU HAVE WON";

            PlaySound.Aspect();
        }

        void Die()
        {
            _currentMode = SpecialModes.DeathAnim;
            animCount = 80;
            introLag = 80;
            introText = new string[1];
            introText[0] = "NICE TRY";
        }

        public void HitBottom()
        {
            if (winCondition == SpecialStage.WinCondition.KillEnemies)
            {
                player.Hurt();
                Die();
            }
        }
    }
}
