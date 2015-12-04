using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    public abstract class AdventureObject
    {
        protected Game game;
        protected AdventureScreen parent;
        protected Texture2D texture;
        
        protected int currentFrame = 0;
        protected int stallCount = 0;
        protected Vector2 offset = new Vector2(16,40);
        protected int width = 10;
        protected int height = 6;
        protected int graphicsRow = 0;

        public Master.Directions faceDir;
        public bool moving;
        public Vector2 location;
        public int z = 0;
        int vz = 0;

        public bool active = true;

        public virtual void Initialize(AdventureScreen parent, Game game)
        {
            this.parent = parent;
            this.game = game;
        }

        public virtual void Update()
        {
            if (stallCount == 10)
            {
                stallCount = 0;
                currentFrame = currentFrame + 1;
                if (currentFrame > 1)
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
            int dim_y;
            if (parent.isInjury(location, 0, 0) && z == 0)
                dim_y = 40;
            else
                dim_y = 48;

            int column = ((int)faceDir * 2) + currentFrame;
            int row = graphicsRow;
            Vector2 screen_loc = location - offset;

            Rectangle sourceRectangle = new Rectangle(dim_x * column, row * 48, dim_x, dim_y);
            Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y - (z * 2), dim_x, dim_y);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, mask);
            //spriteBatch.Draw(Master.texCollection.blank, new Rectangle((int)location.X - width, (int)location.Y - height, width * 2, height * 2), Color.Red);
            spriteBatch.End();
        }

        public virtual void Move(Vector2 move_dist)
        {
            Vector2 test = move_dist + location;
            if ((test.X - width) >= 0 && (test.Y - height) >= 0 && (test.X + width) < (25 * 32) && (test.Y + height) < (13 * 32))
            {
                if (!this.parent.isSolid(test, z, width, height, faceDir))
                    this.location = test;
                return;
            }

            if (parent.hloop && test.X - width < 0)
                location.X = 25 * 32 - width - 2;
            else if (parent.hloop && test.X + width >= (25 * 32))
                location.X = width + 2;
            else if (parent.vloop && test.Y - height < 0)
                location.Y = 13 * 32 - height - 2;
            else if (parent.vloop && test.Y + height >= (13 * 32))
                location.Y = height + 2;
        }

        public virtual void Jump()
        {
            if (this.z == 0)
                this.vz = +5;
        }

        public abstract void Touch() ;

        public abstract bool inRange(AdventurePlayer player) ;

        protected bool doesOverlap(AdventureObject obj)
        {
            return Math.Abs(location.X - obj.location.X) < (width + obj.width) && Math.Abs(location.Y - obj.location.Y) < (height + obj.height);
        }
    }

    public class AdventureShooter : AdventureObject
    {
        bool track;
        bool active = false;

        public AdventureShooter(bool track)
        {
            this.track = track;
            stallCount = Master.globalRandom.Next(0, 80);
        }

        public override bool inRange(AdventurePlayer player)
        {
            return false;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            //
        }

        public override void Update()
        {
            if (!track)
            {
                if (stallCount <= 0)
                {
                    SeekerProjectile seeker = new SeekerProjectile(parent, location, 300);
                    parent.addObject(seeker);
                    stallCount = 90;
                }
                else
                    stallCount = stallCount - 1;
            }
            else
            {
                if (((Math.Floor(location.Y / 32) == Math.Floor(parent.player.location.Y / 32)) ||
                    (Math.Floor(location.X / 32) == Math.Floor(parent.player.location.X / 32))) &&
                   stallCount == 0)
                {
                    Master.Directions closeDir;
                    int del_x = (int)(parent.player.location.X - location.X);
                    int del_y = (int)(parent.player.location.Y - location.Y);

                    if (Math.Abs(del_x) > Math.Abs(del_y))
                    {
                        if (del_x > 0)
                            closeDir = Master.Directions.Right;
                        else
                            closeDir = Master.Directions.Left;
                    }
                    else
                    {
                        if (del_y > 0)
                            closeDir = Master.Directions.Down;
                        else
                            closeDir = Master.Directions.Up;
                    }
                    AdventureProjectile aP = AdventureProjectile.getIntangibleProjectile(false, closeDir, location, 300);
                    parent.addObject(aP);
                    stallCount = 4;

                    if (active == false)
                        PlaySound.Play(PlaySound.SoundEffectName.Laser);
                    active = true;
                }
                else if (stallCount == 0)
                    active = false;
                else if (stallCount > 0)
                    stallCount = stallCount - 1;
            }
        }

        public override void Touch()
        {
            //
        }
    }

    public class AdventureExplosion : AdventureObject
    {
        const int frameCount = 10;
        int animCountMax = 4 * frameCount;
        int animCount = 0;

        public AdventureExplosion(Vector2 location)
        {
            this.location = location;
            this.texture = Master.texCollection.texPlosion;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            int dim_x = 32;
            int dim_y = 32;
            Vector2 screen_loc = Master.getMapTile(animCount / 4, Master.texCollection.texPlosion);

            Rectangle sourceRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y, dim_x, dim_y);
            Rectangle destinationRectangle = new Rectangle((int)location.X - 16, (int)location.Y - 16, dim_x, dim_y);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);
            spriteBatch.End();
        }

        public override void Update()
        {
            animCount++;
            if (animCount > animCountMax)
                this.active = false;
        }

        public override bool inRange(AdventurePlayer player)
        {
            return false;
        }

        public override void Touch()
        {
            //
        }
    }

    public class AdventureHidden : AdventureObject
    {
        int enemy;

        public AdventureHidden(int enemy)
        {
            this.enemy = enemy;
        }

        public override void Update()
        {
            if (Vector2.Distance(location, parent.player.location) < 96 && !parent.isSolid(location, 0, 0, 0, Master.Directions.Down))
            {
                AdventureEnemy aE = new AdventureEnemy(Master.currentFile.bestiary[enemy], enemy);
                aE.location = location;
                parent.addObject(aE);
                active = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            // Do nothing
        }

        public override bool inRange(AdventurePlayer player)
        {
            return false;
        }

        public override void Touch()
        {
            // Do nothing
        }
    }
}
