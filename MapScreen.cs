using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace aspectstar2
{
    public class MapScreen : Screen
    {
        readonly Game game;
        readonly MapPlayer player;

        public Vector2 playerloc
        {
            get { return player.location; }
            private set {; }
        }

        Vector2 screenOffset = new Vector2(0, 0);
        readonly int[] tileMap;
        readonly int[] key;
        readonly List<MapObject> objects;

        int controlLag = 20;
        Selections selection = Selections.Continue;
        bool saved;

        mapModes currentMode = mapModes.runMode;

        public List<mapChange> mapChanges = new List<mapChange>();

        enum mapModes
        {
            runMode = 0,
            menuMode = 1,
        }

        enum Selections
        {
            Continue = 0,
            Save = 1,
            Quit = 2,
        }

        public MapScreen(Game game)
        {
            this.game = game;
            this.master = game.master;
            this.player = new MapPlayer(this);
            //tileMap = Master.currentFile.map.tileMap;
            tileMap = new int[Master.currentFile.map.tileMap.Length];
            Master.currentFile.map.tileMap.CopyTo(tileMap, 0);
            key = Master.currentFile.map.key;
            objects = Master.currentFile.map.objects;
            foreach (MapObject obj in objects)
                obj.Initialize(this, game);
            PlaySong.Play(PlaySong.SongName.WorldMap);

            player.location = new Vector2(Master.currentFile.map.startX * 32, Master.currentFile.map.startY * 32);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Rectangle source;
            Rectangle dest;
            Vector2 sourceTile, limitOffset;
            int x, y;

            Color color = Color.White;
            if (currentMode == mapModes.menuMode)
                color = Color.FromNonPremultiplied(255, 255, 255, 100);

            spriteBatch.Begin();

            if (player.location.X > (Master.width / 2))
                screenOffset.X = player.location.X - (Master.width / 2);
            else
                screenOffset.X = 0;

            if (player.location.Y > (Master.height / 2))
                screenOffset.Y = player.location.Y - (Master.height / 2);
            else
                screenOffset.Y = 0;

            for (int i = 0; i < (Mapfile.width * Mapfile.height); i++)
            {
                x = i % Mapfile.width;
                y = i / Mapfile.width;
                limitOffset = new Vector2((float)Math.Floor(screenOffset.X / 32), (float)Math.Floor(screenOffset.Y / 32));
                if ((x >= limitOffset.X) && (x <= limitOffset.X + Master.width) &&
                    (y >= limitOffset.Y) && (y <= limitOffset.Y + Master.height))
                {
                    sourceTile = Master.getMapTile(tileMap[i], Master.texCollection.worldTiles);
                    source = new Rectangle((int)sourceTile.X, (int)sourceTile.Y, 32, 32);
                    dest = new Rectangle(x * 32 - (int)screenOffset.X, y * 32 - (int)screenOffset.Y, 32, 32);
                    spriteBatch.Draw(Master.texCollection.worldTiles, dest, source, color);
                }
            }
            spriteBatch.End();

            if (currentMode == mapModes.runMode)
                player.Draw(spriteBatch, Color.White, screenOffset);
            else if (currentMode == mapModes.menuMode)
            {
                Vector2 line = new Vector2((Master.width / 2) - (16 * 3), (Master.height / 2) - (16 * 3));
                WriteText(spriteBatch, "CONTINUE", line, Color.White);
                line = new Vector2(line.X, line.Y + 32);
                WriteText(spriteBatch, saved ? "SAVED" : "SAVE", line, Color.White);
                line = new Vector2(line.X, line.Y + 32);
                WriteText(spriteBatch, "QUIT", line, Color.White);

                spriteBatch.Begin();
                source = new Rectangle(128, 16, 16, 16);

                dest = new Rectangle((Master.width / 2) - (16 * 5), (Master.height / 2) - (16 * 3) + (32 * (int)selection), 16, 16);

                spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.Red);
                spriteBatch.End();
            }

            DrawOverlay(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (controlLag > 0)
                controlLag = controlLag - 1;

            switch (currentMode)
            {
                case mapModes.menuMode:
                    if (controlLag == 0)
                    {
                        if (Master.controls.Up && selection != 0)
                        {
                            selection = (Selections)((int)selection - 1);
                            controlLag = 20;
                        }
                        else if (Master.controls.Down && (int)selection != 2)
                        {
                            selection = (Selections)((int)selection + 1);
                            controlLag = 20;
                        }
                        else if (Master.controls.Start || Master.controls.A || Master.controls.B)
                        {
                            switch (selection)
                            {
                                case Selections.Continue:
                                    currentMode = mapModes.runMode;
                                    controlLag = 20;
                                    break;
                                case Selections.Save:
                                    game.saveGame();
                                    controlLag = 20;
                                    saved = true;
                                    break;
                                case Selections.Quit:
                                    game.master.UpdateScreen(new TitleScreen(game.master));
                                    break;
                            }
                        }
                    }
                    break;
                default:
                    // Update
                    player.Update();

                    // Get keyboard input
                    //KeyboardState state = Keyboard.GetState();

                    if (controlLag == 0)
                    {
                        if (Master.controls.Up)
                        {
                            player.faceDir = Master.Directions.Up;
                            player.moving = true;
                        }
                        else if (Master.controls.Down)
                        {
                            player.faceDir = Master.Directions.Down;
                            player.moving = true;
                        }
                        else if (Master.controls.Left)
                        {
                            player.faceDir = Master.Directions.Left;
                            player.moving = true;
                        }
                        else if (Master.controls.Right)
                        {
                            player.faceDir = Master.Directions.Right;
                            player.moving = true;
                        }
                        else
                        {
                            player.moving = false;
                        }

                        if (Master.controls.Start)
                        {
                            currentMode = mapModes.menuMode;
                            saved = false;
                            controlLag = 20;
                            PlaySound.Play(PlaySound.SoundEffectName.Pause);
                        }
                    }
                    break;
            }
        }

        void DrawOverlay(SpriteBatch spriteBatch)
        {
            Rectangle source, dest;

            if (game.goldKeys != 0)
            {
                spriteBatch.Begin();

                source = new Rectangle((128 + 48), 0, 16, 16);
                dest = new Rectangle(Master.width - 64, 32, 16, 16);
                spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.White);

                spriteBatch.End();

                WriteText(spriteBatch, game.goldKeys.ToString(), new Vector2(Master.width - 48, 32), Color.White);
            }
        }

        public bool tileSolid(int x, int y)
        {
            int i = x + (y * Mapfile.width);
            return !(i < 0)&&key[tileMap[i]] == 1;
        }

        public void checkObjects(int x, int y)
        {
            foreach (MapObject m in objects)
            {
                if ((m.x == x) && (m.y == y))
                {
                    m.Activate();
                    player.moving = false;
                }
            }
        }

        public void LocalTeleport(int x, int y)
        {
            player.location = new Vector2(x * 32, y * 32);
            player.faceDir = Master.Directions.Down;
        }

        public void ChangeTile(int x, int y, int tile)
        {
            int i = x + (y * Mapfile.width);
            tileMap[i] = tile;

            mapChange mC = new mapChange();
            mC.i = i;
            mC.tile = tile;
            mapChanges.Add(mC);
        }

        public void ApplyChanges(List<mapChange> mapChanges)
        {
            foreach (var change in mapChanges)
            {
                tileMap[change.i] = change.tile;
                this.mapChanges.Add(change);
            }
        }
    }
}
