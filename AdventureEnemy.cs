using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace aspectstar2
{
    public class AdventureEnemy : AdventureObject
    {
        BestiaryEntry definition;

        public AdventureEnemy(BestiaryEntry definition)
        {
            this.definition = definition;
            this.texture = Master.texCollection.texEnemies;
        }

        public override void Update()
        {
            switch (definition.movementType)
            {
                default:
                    //Stationary
                    break;
            }

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            int dim_x = 32;
            int dim_y = 48;
            int column = ((int)faceDir * 2) + currentFrame;
            int row = definition.graphicsRow;
            Vector2 screen_loc = location - offset;

            Rectangle sourceRectangle = new Rectangle(dim_x * column, row * dim_y, dim_x, dim_y);
            Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y - (z * 2), dim_x, dim_y);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, mask);
            spriteBatch.End();
        }

        public override bool inRange(AdventurePlayer player)
        {
            Vector2 playerloc = player.location;

            if (Math.Sqrt( Math.Pow(location.X - playerloc.X, 2) + Math.Pow(location.Y - playerloc.Y, 2) ) <= Math.Max(width, height))
            {
                player.Hurt();
                player.Recoil();
                return true;
            }
            return false;
        }

        public override void Touch()
        {
            // Enemies are defined by their dependence on inRange
        }
    }
}
