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

        MapObject player;
        List<MapObject> objects = new List<MapObject>();

        public MapScreen(Game game)
        {
            this.game = game;
            this.master = game.master;
            this.player = new MapPlayer();

            objects.Add(this.player);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (MapObject obj in this.objects)
            {
                obj.Draw(spriteBatch, Color.White);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Update
            foreach (MapObject obj in this.objects)
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
        }
    }
}
