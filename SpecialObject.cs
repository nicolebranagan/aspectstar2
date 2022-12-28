using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aspectstar2
{
    public abstract class SpecialObject
    {
        protected SpecialScreen parent;

        protected Vector2 next_loc;
        public Vector2 location;
        protected int graphicsRow;
        protected int height = 32;
        protected int width = 32;

        public bool active = true;
        public int radius = 12;

        protected int stallCount;

        public virtual void Draw(SpriteBatch spriteBatch, Color mask)
        {
            Vector2 offset = new Vector2(16, 16);
            Rectangle source;

            source = graphicsRow == 0 ? new Rectangle(0, 0, width, height) : new Rectangle(0, 48 + (graphicsRow - 1) * 32, width, height);
            Rectangle dest = new Rectangle((int)location.X - 16, (int)location.Y - 16, width, height);

            spriteBatch.Begin();
            spriteBatch.Draw(Master.texCollection.texSpecial, dest, source, Color.White);
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

        public virtual bool Hurt()
        {
            active = false;
            return false; // Return true if points should be gained
        }

        public bool Move(Vector2 move)
        {
            Vector2 test = next_loc + move;
            if ((next_loc - location != move) && (test.X > 0) && (test.X < SpecialScreen.width) && (test.Y > 0) && (test.Y < SpecialScreen.height))
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
            location = new Vector2(SpecialScreen.width / 2, SpecialScreen.height - 48);
            next_loc = location;
        }

        public override bool Hurt()
        {
            parent.addObject(new SpecialExplosion(location));
            PlaySound.Play(PlaySound.SoundEffectName.Die);
            base.Hurt();
            return false;
        }
    }

    public class SpecialEnemy : SpecialObject
    {
        //  Behavior
        readonly int shootingRate;
        readonly int speed;
        readonly int amplitude;
        readonly int time;
        readonly bool track;
        readonly Behaviors currentBehavior;

        // Internal tracking
        int animCount;
        int countRate;
        readonly bool down = true;

        enum Behaviors
        {
            Stationary = 0,
            SpaceInvaders = 1,
            Boustrophedon = 2,
            Sinusoidal = 3,
        }

        public SpecialEnemy(SpecialScreen parent, int row, int x, int y, int shootingrate, int behavior, int speed, int amplitude, int time, bool track)
        {
            this.parent = parent;
            graphicsRow = row;
            location = new Vector2(x, y);
            next_loc = location;

            shootingRate = shootingrate;
            currentBehavior = (Behaviors)behavior;
            this.speed = speed;
            this.amplitude = amplitude;
            this.time = time;
            this.track = track;
        }

        public override void Update()
        {
            base.Update();

            switch (currentBehavior)
            {
                case Behaviors.SpaceInvaders:
                    if (stallCount % speed == 0)
                    {
                        if (animCount < 9)
                        {
                            Move(new Vector2(2, 0));
                            animCount++;
                        }
                        else if (animCount < 18)
                            animCount++;
                        else if (animCount < 27)
                        {
                            Move(new Vector2(0, 2));
                            animCount++;
                        }
                        else if (animCount < 36)
                            animCount++;
                        else if (animCount < 45)
                        {
                            Move(new Vector2(-2, 0));
                            animCount++;
                        }
                        else if (animCount < 54)
                            animCount++;
                        else if (animCount < 63)
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

            if (location.Y >= SpecialScreen.height - 4)
            {
                this.active = false;
                parent.HitBottom();
            }

            if (shootingRate != 0)
            {
                if (countRate == 0)
                {
                    parent.addObject(track ? new SpecialProjectile(parent, location, new Vector2(0, 2), false, true) : new SpecialProjectile(parent, location, new Vector2(0, 2), false, false));
                    countRate = 100 - shootingRate;
                }
                else
                    countRate--;
            }
        }

        public override bool Hurt()
        {
            parent.addObject(new SpecialExplosion(location));
            PlaySound.Play(PlaySound.SoundEffectName.Boom);
            base.Hurt();
            parent.killCount = parent.killCount - 1;
            return true;
        }
    }

    class SpecialProjectile : SpecialObject
    {
        public bool friendly, fiery, ghostly;
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

    class SpecialExplosion : SpecialObject
    {
        readonly AdventureExplosion exp;

        public SpecialExplosion(Vector2 location)
        {
            exp = new AdventureExplosion(location);
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            exp.Draw(spriteBatch, mask);
        }

        public override void Update()
        {
            exp.Update();
        }

        public override bool Hurt()
        {
            return false;
        }
    }
}
