using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    class MapObject
    {
        // The only thing that can move on the map is the player
    }

    class MapPlayer
    {
        MapScreen parent;
         
        protected Texture2D texture;
        protected int currentFrame = 0;
        int stallCount = 0;

        Master.Directions _faceDir;
        public Master.Directions faceDir
        {
            get { return _faceDir; }
            set { if (!moving)
                {
                    _faceDir = value;
                }
            }
        }
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
                        Move();
                    else
                        renewMove = true;
                }
                else
                    renewMove = false;
            }
        }

        public MapPlayer(MapScreen parent)
        {
            this.parent = parent;
            this.texture = Master.texCollection.texMapPlayer;
        }

        public void Update()
        {
            if (stallCount == 10)
            {
                stallCount = 0;
                currentFrame = currentFrame + 1;
                if (currentFrame == 2)
                    currentFrame = 0;

                if (!this.moving)
                    this.currentFrame = 0;
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
                this.location = this.location + move_dist;
                this.moveCount--;
                if (this.moveCount == 0 & this.renewMove)
                    this.Move();
            }
            else if (moveCount == 0)
            {
                this.location.X = (float)Math.Floor(this.location.X / 32) * 32;
                this.location.Y = (float)Math.Floor(this.location.Y / 32) * 32;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Color mask, Vector2 offset)
        {
            int dim = 32;
            int column = ((int)faceDir * 2) + currentFrame;
            Vector2 screen_loc = location; // Map objects MUST be 16x16!

            Rectangle sourceRectangle = new Rectangle(dim * column, 0, dim, dim);
            Rectangle destinationRectangle = new Rectangle((int)(screen_loc.X - offset.X), (int)(screen_loc.Y - offset.Y), dim, dim);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, mask);
            spriteBatch.End();
        }
        
        public void Move()
        {
            int x = (int)Math.Floor(location.X / 32);
            int y = (int)Math.Floor(location.Y / 32);

            switch (faceDir)
            {
                case Master.Directions.Down:
                    y = y + 1;
                    break;
                case Master.Directions.Up:
                    y = y - 1;
                    break;
                case Master.Directions.Left:
                    x = x - 1;
                    break;
                case Master.Directions.Right:
                    x = x + 1;
                    break;
                default:
                    break; // Something has gone wrong
            }

            if (!parent.tileSolid(x, y))
            {
                this.moveCount = 16;
            }
            else
                this.moveCount = 0;
        }
    }
}
