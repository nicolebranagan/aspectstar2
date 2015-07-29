using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace aspectstar2
{
    class AdventureScreen : Screen
    {
        Game game;

        AdventureObject player;
        List<AdventureObject> objects = new List<AdventureObject>();
        int adventure, roomX, roomY;
        int[] tileMap;

        public AdventureScreen(Game game, int dest, int destroomX, int destroomY, int x, int y)
        {
            this.game = game;
            this.master = game.master;
            this.player = new AdventurePlayer();

            this.adventure = dest;
            this.roomX = destroomX;
            this.roomY = destroomY;
            this.tileMap = Master.currentFile.adventures[dest].rooms[roomX, roomY].tileMap;

            objects.Add(this.player);
            player.location = new Vector2(x * 32, y * 32);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawRoom(spriteBatch);
            foreach (AdventureObject obj in this.objects)
            {
                obj.Draw(spriteBatch, Color.White);
            }
        }

        void DrawRoom(SpriteBatch spriteBatch)
        {
            int x, y;
            Rectangle source, dest;
            Vector2 sourceTile;
            spriteBatch.Begin();
            for (int i = 0; i < (tileMap.Length); i++)
            {
                x = i % 25;
                y = i / 25;
                sourceTile = Master.getMapTile(tileMap[i], Master.texCollection.dungeonTiles);
                source = new Rectangle((int)sourceTile.X, (int)sourceTile.Y, 32, 32);
                dest = new Rectangle(x * 32, y * 32, 32, 32);
                spriteBatch.Draw(Master.texCollection.dungeonTiles, dest, source, Color.White);
            }
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            // Update
            foreach (AdventureObject obj in this.objects)
            {
                obj.Update();
            }

            // Get keyboard input
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Master.controls.Up))
            {
                player.moving = true;
                player.faceDir = Master.Directions.Up;
            }
            else if (state.IsKeyDown(Master.controls.Down))
            {
                player.moving = true;
                player.faceDir = Master.Directions.Down;
            }
            else if (state.IsKeyDown(Master.controls.Left))
            {
                player.moving = true;
                player.faceDir = Master.Directions.Left;
            }
            else if (state.IsKeyDown(Master.controls.Right))
            {
                player.moving = true;
                player.faceDir = Master.Directions.Right;
            }
            else
            {
                player.moving = false;
            }
            
            if (state.IsKeyDown(Master.controls.A))
                player.Jump();
        }
    }
}
