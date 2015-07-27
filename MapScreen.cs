using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace aspectstar2
{
    class MapScreen : Screen
    {
        Game game;

        MapPlayer player;
        List<MapObject> objects = new List<MapObject>();

        Vector2 screenOffset = new Vector2(0, 0);

        int[] tileMap; int[] key;

        public MapScreen(Game game)
        {
            this.game = game;
            this.master = game.master;
            this.player = new MapPlayer(this);
            tileMap = Master.currentFile.map.tileMap;
            key = Master.currentFile.map.key;

            player.location = new Vector2(Master.currentFile.map.startX * 32, Master.currentFile.map.startY * 32);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Rectangle source;
            Rectangle dest;
            Vector2 sourceTile, limitOffset;
            int x, y;
            spriteBatch.Begin();

            if (player.location.X > (Master.width / 2))
            {
                screenOffset.X = player.location.X - (Master.width / 2);
            }
            
            if (player.location.Y > (Master.height / 2))
            {
                screenOffset.Y = player.location.Y - (Master.height / 2);
            }

            for (int i = 0; i < (Mapfile.width * Mapfile.height); i++)
            {
                x = i % Mapfile.width;
                y = i / Mapfile.width;
                limitOffset = new Vector2((float)Math.Floor(screenOffset.X / 32), (float)Math.Floor(screenOffset.Y / 32));
                if ((x >= limitOffset.X) && (x <= limitOffset.X + Master.width) &&
                    (y >= limitOffset.Y) && (y <= limitOffset.Y + Master.height))
                {
                    sourceTile = Master.getMapTile(tileMap[i]);
                    source = new Rectangle((int)sourceTile.X, (int)sourceTile.Y, 32, 32);
                    dest = new Rectangle(x * 32 - (int)screenOffset.X, y * 32 - (int)screenOffset.Y, 32, 32);
                    spriteBatch.Draw(Master.texCollection.worldTiles, dest, source, Color.White);
                }
            }
            spriteBatch.End();

            player.Draw(spriteBatch, Color.White, screenOffset);
        }

        public override void Update(GameTime gameTime)
        {
            // Update
            player.Update();

            // Get keyboard input
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Master.controls.Up))
            {
                player.faceDir = Master.Directions.Up;
                player.moving = true;
            }
            else if (state.IsKeyDown(Master.controls.Down))
            {
                player.faceDir = Master.Directions.Down;
                player.moving = true;
            }
            else if (state.IsKeyDown(Master.controls.Left))
            {
                player.faceDir = Master.Directions.Left;
                player.moving = true;
            }
            else if (state.IsKeyDown(Master.controls.Right))
            {
                player.faceDir = Master.Directions.Right;
                player.moving = true;
            }
            else
            {
                player.moving = false;
            }
        }

        public bool tileSolid(int x, int y)
        {
            int i = x + (y * Mapfile.width);
            if (i < 0)
                return false;
            else
                return key[tileMap[i]] == 1;
        }
    }
}
