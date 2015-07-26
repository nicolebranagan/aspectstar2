using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    abstract class MapObject
    {
        protected Texture2D texture;
        protected int currentFrame = 0;
        int stallCount = 0;

        public Master.Directions faceDir;
        public Vector2 location;

        int moveCount = 0;
        bool renewMove = false;
        public bool moving
        {
            get { return moveCount > 0; }
            set
            {
                if (value)
                {
                    if (moveCount == 0)
                        moveCount = 16;
                    else
                        renewMove = true;
                }
                else
                    renewMove = false;
            }
        }

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
                this.moveCount--;
                if (this.moveCount == 0 & this.renewMove)
                    this.moveCount = 16;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Color mask)
        {
            int dim = 32;
            int column = ((int)faceDir * 2) + currentFrame;
            Vector2 screen_loc = location; // Map objects MUST be 16x16!

            Rectangle sourceRectangle = new Rectangle(dim * column, 0, dim, dim);
            Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y, dim, dim);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, mask);
            spriteBatch.End();
        }

        public virtual void Move(Vector2 move_dist)
        {
            this.location = this.location + move_dist;
        }
    }

    class MapPlayer : MapObject
    {
        public MapPlayer()
        {
            this.texture = Master.texCollection.texPlayer;
            this.location = new Vector2(100, 100);
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
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
