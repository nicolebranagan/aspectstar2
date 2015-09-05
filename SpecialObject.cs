using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aspectstar2
{
    abstract class SpecialObject
    {
        protected SpecialScreen parent;

        protected Vector2 next_loc;
        public Vector2 location;
        protected int graphicsRow;
        protected int height = 32;
        protected int width = 32;

        public bool active = true;

        protected int stallCount = 0;

        public virtual void Draw(SpriteBatch spriteBatch, Color mask)
        {
            Vector2 offset = new Vector2(16, 16);
            Rectangle source;

            if (graphicsRow == 0)
                source = new Rectangle(0, 0, width, height);
            else
                source = new Rectangle(0, 48 + (graphicsRow - 1) * 32, width, height);
            Rectangle dest = new Rectangle((int)location.X - 16, (int)location.Y - 16, width, height);

            spriteBatch.Begin();
            spriteBatch.Draw(Master.texCollection.specialTiles, dest, source, Color.White);
            spriteBatch.End();
        }

        public virtual void Update()
        {
            if (next_loc != location)
                location = next_loc;

            stallCount = stallCount + 1;
            if (stallCount > 10)
                stallCount = 0;
        }

        public bool Move(Vector2 move)
        {
            Vector2 test = location + move;
            if ((test.X > 0) && (test.X < Master.width) && (test.Y > 0) && (test.Y < Master.height))
            {
                next_loc = test;
                return true;
            }
            else
                return false;
        }
    }

    class SpecialPlayer : SpecialObject
    {
        public SpecialPlayer(SpecialScreen parent)
        {
            this.parent = parent;
            graphicsRow = 0;
            height = 48;
            location = new Vector2(Master.width / 2, Master.height - 48);
            next_loc = location;
        }
    }

    class SpecialEnemy : SpecialObject
    {
        //  Behavior
        int shootingRate = 30;
        int speed = 1;
        int amplitude = 10;
        int time = 5;
        Behaviors currentBehavior;

        // Internal tracking
        int animCount = 0;
        int countRate;
        bool down = true;

        enum Behaviors
        {
            Stationary = 0,
            SpaceInvaders = 1,
            Boustrophedon = 2,
            Sinusoidal = 3,
        }

        public SpecialEnemy(SpecialScreen parent, int row, Vector2 loc)
        {
            this.parent = parent;
            graphicsRow = row;
            location = loc;
            next_loc = location;
            currentBehavior = Behaviors.Boustrophedon;
        }

        public override void Update()
        {
            base.Update();

            switch (currentBehavior)
            {
                case Behaviors.SpaceInvaders:
                    if (stallCount % speed == 0)
                    {
                        if (animCount < 3)
                        {
                            Move(new Vector2(2, 0));
                            animCount++;
                        }
                        else if (animCount < 6)
                            animCount++;
                        else if (animCount < 9)
                        {
                            Move(new Vector2(0, 2));
                            animCount++;
                        }
                        else if (animCount < 12)
                            animCount++;
                        else if (animCount < 15)
                        {
                            Move(new Vector2(-2, 0));
                            animCount++;
                        }
                        else if (animCount < 18)
                            animCount++;
                        else if (animCount < 21)
                        {
                            Move(new Vector2(0, 2));
                            animCount++;
                        }
                        else
                            animCount = 0;
                    }
                    break;
                case Behaviors.Boustrophedon:
                    if (stallCount % speed == 0)
                    {
                        if (animCount == 0)
                        {
                            if (!Move(new Vector2(4, 0)))
                                animCount = 1;
                        }
                        else if (animCount < 7)
                        {
                            Move(new Vector2(0, 4));
                            animCount++;
                        }
                        else if (animCount == 7)
                        {
                            if (!Move(new Vector2(-4, 0)))
                                animCount = 8;
                        }
                        else if (animCount < 14)
                        {
                            Move(new Vector2(0, 4));
                            animCount++;
                            if (animCount == 14)
                                animCount = 0;
                        }
                    }
                    break;
                case Behaviors.Sinusoidal:
                    if (stallCount % speed == 0)
                    {
                        Move(new Vector2(amplitude * (float)Math.Sin(animCount / time), down ? 2 : -2));
                        animCount++;
                    }
                    break;
            }

            if (shootingRate != 0)
            {
                if (countRate == 0)
                {
                    parent.addObject(new SpecialProjectile(parent, location, new Vector2(0, 2), false, true));
                    countRate = 100 - shootingRate;
                }
                else
                    countRate--;
            }
        }
    }

    class SpecialProjectile : SpecialObject
    {
        bool friendly, fiery, ghostly;
        Vector2 direction;

        public SpecialProjectile(SpecialScreen parent, Vector2 location, Vector2 direction, 
            bool friendly = false, bool tracking = false, bool fiery = false, bool ghostly = false)
        {
            this.location = location; next_loc = location;
            if (!tracking)
                this.direction = direction;
            else
            {
                int del_x = (int)(parent.playerloc.X - location.X);
                int del_y = (int)(parent.playerloc.Y - location.Y);
                float norm = (float)Math.Sqrt(Math.Pow(del_x, 2) + Math.Pow(del_y, 2));

                this.direction = new Vector2(4 * del_x / norm, 4 * del_y / norm);
            }
            this.friendly = friendly;
            this.fiery = fiery;
            this.ghostly = ghostly;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            spriteBatch.Begin();
            Rectangle source = new Rectangle(0, 0, 16, 16);
            if (!friendly)
                source.Y = 16;
            if (fiery)
                source.Y = 32;
            if (ghostly)
                source.Y = 48;
            Rectangle dest = new Rectangle((int)location.X - 8, (int)location.Y - 8, 16, 16);
            spriteBatch.Draw(Master.texCollection.texProjectile, dest, source, Color.White);
            spriteBatch.End();
        }

        public override void Update()
        {
            location = location + direction;
            if (!Move(direction))
                active = false;
        }
    }
}
