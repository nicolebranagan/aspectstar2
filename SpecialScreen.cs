using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace aspectstar2
{
    class SpecialScreen : Screen
    {
        SpecialPlayer player;
        List<SpecialObject> objects = new List<SpecialObject>();
        List<SpecialObject> newobjects = new List<SpecialObject>();
        int yoffset = 0;

        public Vector2 playerloc
        {
            get { return player.location; }
            private set {; }
        }

        int lag = 0;

        public SpecialScreen()
        {
            player = new SpecialPlayer(this);

            objects.Add(new SpecialEnemy(this, Master.globalRandom.Next(1,4), new Vector2(100,32)));
            objects.Add(new SpecialEnemy(this, Master.globalRandom.Next(1, 4), new Vector2(500, 32)));
            objects.Add(player);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Rectangle source, dest;
            int x, y;

            source = new Rectangle(0, 0, 32, 32);

            spriteBatch.Begin();
            for (int i = 0; i < ((Master.height + 32) * Master.width / (32 * 32)); i++)
            {
                x = i % (Master.width / 32);
                y = i / (Master.width / 32);
                dest = new Rectangle(x * 32, y * 32 + yoffset, 32, 32);
                spriteBatch.Draw(Master.texCollection.adventureTiles[0], dest, source, Color.White);
            }
            spriteBatch.End();

            foreach (SpecialObject obj in objects)
                obj.Draw(spriteBatch, Color.White);

            player.Draw(spriteBatch, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            objects.AddRange(newobjects);
            newobjects = new List<SpecialObject>();

            foreach (SpecialObject obj in objects)
                obj.Update();

            if (Master.controls.Up)
                player.Move(new Vector2(0, -4));
            else if (Master.controls.Down)
                player.Move(new Vector2(0, 4));
            else if (Master.controls.Left)
                player.Move(new Vector2(-4, 0));
            else if (Master.controls.Right)
                player.Move(new Vector2(4, 0));

            if (Master.controls.A && lag == 0)
            {
                newobjects.Add(new SpecialProjectile(this, player.location, new Vector2(0, -4), true));
                lag = 10;
            }

            if (lag > 0)
                lag = lag - 1;
        }

        public void addObject(SpecialObject obj)
        {
            newobjects.Add(obj);
        }
    }
}
