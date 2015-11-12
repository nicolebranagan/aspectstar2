using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace aspectstar2
{
    public class AdventureProjectile : AdventureObject
    {
        protected bool friendly;
        protected int deathTimer;

        bool fiery;
        bool ghostly;

        bool intangible;

        int damage = 1;

        int frameCounter = 0;

        public AdventureProjectile()
        { }

        public AdventureProjectile(bool friendly, Master.Directions faceDir, Vector2 location, int deathTimer, int damage)
        {
            this.friendly = friendly;
            this.faceDir = faceDir;
            this.location = location;
            this.deathTimer = deathTimer;
            this.texture = Master.texCollection.texProjectile;
            this.moving = true;
            this.damage = damage;
            //z = 1;
            width = 4;
            height = 4;
        }

        public static AdventureProjectile getFieryProjectile(bool friendly, Master.Directions faceDir, Vector2 location, int deathTimer)
        {
            AdventureProjectile aP = new AdventureProjectile(friendly, faceDir, location, deathTimer, 1);
            aP.fiery = true;
            return aP;
        }

        public static AdventureProjectile getGhostlyProjectile(bool friendly, Master.Directions faceDir, Vector2 location, int deathTimer)
        {
            AdventureProjectile aP = new AdventureProjectile(friendly, faceDir, location, deathTimer, 1);
            aP.ghostly = true;
            return aP;
        }

        public static AdventureProjectile getIntangibleProjectile(bool friendly, Master.Directions faceDir, Vector2 location, int deathTimer)
        {
            AdventureProjectile aP = new AdventureProjectile(friendly, faceDir, location, deathTimer, 1);
            aP.intangible = true;
            return aP;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            frameCounter++;
            if (frameCounter >= 20)
                frameCounter = 0;

            int frame = frameCounter / 5;

            spriteBatch.Begin();
            Rectangle source = new Rectangle(0, 0, 16, 16);
            if (!friendly)
            {
                source.Y = 16;
                frame = 0;
            }
            if (fiery)
                source.Y = 32;
            if (ghostly)
                source.Y = 48;
            if (damage > 2)
            {
                source.Y = 64;
                frame = 0;
            }
            if (intangible)
            {
                if (faceDir == Master.Directions.Up || faceDir == Master.Directions.Down)
                {
                    source.Y = 96;
                    frame = 0;
                }
                else
                {
                    source.Y = 80;
                    frame = 0;
                }
            }
            spriteBatch.Draw(texture, location, source, Color.White, (float)Math.PI * frame / 2, new Vector2(8,8), 1, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        public void projectileUpdate(List<AdventureObject> objects)
        {
            foreach (AdventureObject obj in objects)
            {
                if (obj != this)
                    if ( Math.Sqrt( Math.Pow(location.X - obj.location.X, 2) + Math.Pow(location.Y - obj.location.Y + (obj.z * 2), 2) ) < 16)
                    {
                        if (obj is AdventurePlayer)
                        {
                            if (!friendly)
                            {
                                AdventurePlayer player = (AdventurePlayer)obj;
                                player.Hurt();
                                active = false;
                            }
                        }
                        else if (friendly && obj is AdventureEnemy)
                        {
                            AdventureEnemy enemy = (AdventureEnemy)obj;
                            enemy.Hurt(ghostly, damage);
                            active = false;
                        }
                        else if (obj is AdventureEntity)
                        {
                            AdventureEntity entity = (AdventureEntity)obj;
                            if (entity.solid)
                            {
                                entity.Hurt(damage);
                                active = false;
                            }
                        }
                        else if (obj is AdventureBoss9Phase2)
                        {
                            AdventureBoss9Phase2 boss9 = (AdventureBoss9Phase2)obj;
                            if (friendly)
                                boss9.Thud();
                            else
                                boss9.Hurt();

                            active = false;
                        }
                        else if (obj is AdventureProjectile)
                        {
                            AdventureProjectile proj = (AdventureProjectile)obj;
                            if (proj.damage > damage)
                                active = false;
                            else if (damage > proj.damage)
                                proj.active = false;
                            else if (proj.fiery && !fiery)
                                active = false;
                            else if (fiery && !proj.fiery)
                                proj.active = false;
                            else if (friendly)
                                active = false;
                        }
                    }
            }
        }

        public override void Update()
        {
            Vector2 move_dist = new Vector2(0, 0);
            switch (faceDir)
            {
                case Master.Directions.Down:
                    move_dist = new Vector2(0, 4);
                    break;
                case Master.Directions.Up:
                    move_dist = new Vector2(0, -4);
                    break;
                case Master.Directions.Left:
                    move_dist = new Vector2(-4, 0);
                    break;
                case Master.Directions.Right:
                    move_dist = new Vector2(4, 0);
                    break;
                default:
                    break; // Something has gone wrong
            }
            if (fiery)
                parent.Burn(move_dist + location);

            if (deathTimer % 4 != 0)
                Move(move_dist);

            this.deathTimer--;
            if (deathTimer <= 0)
                active = false;
        }

        public override void Move(Vector2 move_dist)
        {
            Vector2 test = move_dist + location;
            int z = (fiery) ? 0 : 1;
            if ((test.X - width) >= 0 && (test.Y - height) >= 0 && (test.X + width) < (25 * 32) && (test.Y + height) < (13 * 32))
            {
                if (!this.parent.isSolid(test, z, 0, 0, faceDir) || ghostly || intangible)
                    this.location = test;
                else
                    this.active = false;
            }
            else
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

    public class SeekerProjectile : AdventureProjectile
    {
        Vector2 move_dist;
        int speed = 4;

        public SeekerProjectile(AdventureScreen parent, Vector2 location, int deathTimer)
        {
            this.friendly = false;
            this.location = location;
            this.deathTimer = deathTimer;
            this.texture = Master.texCollection.texProjectile;
            this.moving = true;
            this.faceDir = Master.Directions.Down;
            z = 1;
            width = 4;
            height = 4;

            Vector2 playerloc = parent.player.location;

            int del_x = (int)(playerloc.X - location.X);
            int del_y = (int)(playerloc.Y - location.Y);
            float norm = (float)Math.Sqrt(Math.Pow(del_x, 2) + Math.Pow(del_y, 2));

            move_dist = new Vector2(2 * del_x / norm, 2 * del_y / norm);
        }

        public override void Update()
        {   
            if (deathTimer % speed != 0)
                Move(move_dist);

            this.deathTimer--;
            if (deathTimer <= 0)
                active = false;
        }

        public override void Move(Vector2 move_dist)
        {
            Vector2 test = move_dist + location;
            if ((test.X - width) >= 0 && (test.Y - height) >= 0 && (test.X + width) < (25 * 32) && (test.Y + height) < (13 * 32))
                location = test;
            else
                active = false;
        }
    }
}
