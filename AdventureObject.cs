using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    abstract class AdventureObject
    {
        protected Texture2D texture;
        protected int currentFrame = 0;
        int stallCount = 0;
        protected Vector2 offset = new Vector2(16,32);

        public Master.Directions faceDir;
        public bool moving;
        public Vector2 location;
        int z = 0;
        int vz = 0;

        public virtual void Update()
        {
            if (stallCount == 10)
            {
                stallCount = 0;
                currentFrame = currentFrame + 1;
                if (currentFrame == 2)
                    currentFrame = 0;
            }
            else
                stallCount++;

            if ((stallCount % 4 == 0))
            {
                z = z + vz;
                if (z > 0)
                    vz = vz - 1;
                else
                {
                    z = 0;
                    vz = 0;
                }
            }

            if ((stallCount % 4 != 0) && (moving))
            {
                Vector2 move_dist = new Vector2(0, 0);
                switch (faceDir)
                {
                    case Master.Directions.Down:
                        move_dist = new Vector2(0, 2);
                        break;
                    case Master.Directions.Up:
                        move_dist = new Vector2(0, -2);
                        break;
                    case Master.Directions.Left:
                        move_dist = new Vector2(-2, 0);
                        break;
                    case Master.Directions.Right:
                        move_dist = new Vector2(2, 0);
                        break;
                    default:
                        break; // Something has gone wrong
                }
                this.Move(move_dist);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Color mask)
        {
            int dim_x = 32;
            int dim_y = 48;
            int column = ((int)faceDir * 2) + currentFrame;
            Vector2 screen_loc = location - offset;

            Rectangle sourceRectangle = new Rectangle(dim_x * column, 0, dim_x, dim_y);
            Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y - (z * 2), dim_x, dim_y);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, mask);
            spriteBatch.End();
        }

        public virtual void Move(Vector2 move_dist)
        {
            this.location = this.location + move_dist;
        }

        public void Jump()
        {
            if (this.z == 0)
                this.vz = +5;
        }
    }

    class AdventurePlayer : AdventureObject
    {
        public AdventurePlayer()
        {
            this.texture = Master.texCollection.texAdvPlayer;
            this.location = new Vector2(100, 100);
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {

            int dim_x = 32;
            int dim_y = 48;
            int column = ((int)faceDir * 2) + currentFrame;
            Vector2 screen_loc = location - offset;

            Rectangle sourceRectangle = new Rectangle(dim_x * column, 0, dim_x, dim_y);
            Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y, dim_x, dim_y);

            spriteBatch.Begin();
            spriteBatch.Draw(Master.texCollection.texShadows, destinationRectangle, sourceRectangle, mask);
            spriteBatch.End();

            base.Draw(spriteBatch, mask);
        }

        public override void Update()
        {
            base.Update();

            if (!this.moving)
                this.currentFrame = 0;
        }
    }
}
