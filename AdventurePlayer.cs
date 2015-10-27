using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    public class AdventurePlayer : AdventureObject
    {
        int flickerCount = 1;

        public AdventurePlayer(AdventureScreen parent, Game game)
        {
            this.texture = Master.texCollection.texAdvPlayer;
            this.parent = parent;
            this.game = game;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            if (!parent.isInjury(location, width, height) || z != 0)
            {
                int dim_x = 32;
                int dim_y = 48;
                int column = ((int)faceDir * 2) + currentFrame;
                Vector2 screen_loc = location - offset;

                Rectangle sourceRectangle = new Rectangle(dim_x * column, 0, dim_x, dim_y);
                Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y, dim_x, dim_y);

                spriteBatch.Begin();
                spriteBatch.Draw(Master.texCollection.texShadows, destinationRectangle, sourceRectangle, Color.White);
                spriteBatch.End();
            }

            if (flickerCount > 0)
            {
                flickerCount--;
                if (flickerCount % 7 == 0)
                    mask = Color.Red;
            }
            base.Draw(spriteBatch, mask);
        }

        public void Drown(SpriteBatch spriteBatch, int animCount)
        {
            currentFrame++;
            int dim_x = 32;
            int dim_y = 48;
            int column = (currentFrame % 16 > 8) ? 0 : 1;
            Vector2 screen_loc = location - offset;

            Rectangle sourceRectangle = new Rectangle(dim_x * column, 48, dim_x, dim_y - ((24 - animCount) * 2));
            Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y + (24 - animCount) * 2, dim_x, dim_y - ((24 - animCount) * 2));

            spriteBatch.Begin();
            spriteBatch.Draw(Master.texCollection.texAdvPlayer, destinationRectangle, sourceRectangle, Color.White);
            spriteBatch.End();
        }

        public override void Jump()
        {
            if (z == 0)
                PlaySound.Play(PlaySound.SoundEffectName.Jump);
            base.Jump();
        }

        public void Hurt()
        {
            if (flickerCount == 0)
            {
                game.life = game.life - 1;
                Flicker();
                PlaySound.Play(PlaySound.SoundEffectName.Hurt);
            }

            if (game.life <= 0)
                parent.Die();
        }

        public void Flicker()
        {
            this.flickerCount = 80;
        }

        public void Recoil(Vector2 far_location, AdventureObject recoiler)
        {
            int del_x = (int)(location.X - far_location.X);
            int del_y = (int)(location.Y - far_location.Y);

            if (Math.Abs(del_x) > Math.Abs(del_y))
            {
                if (del_x > 0)
                    faceDir = Master.Directions.Right;
                else
                    faceDir = Master.Directions.Left;
            }
            else
            {
                if (del_y > 0)
                    faceDir = Master.Directions.Down;
                else
                    faceDir = Master.Directions.Up;
            }

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
            Vector2 test = location + move_dist;
            if (!parent.hblock && !parent.hloop && ((test.X - width) <= 0))
                parent.enterNewRoom(-1, 0);
            else if (!parent.vblock && !parent.vloop && ((test.Y - height) <= 0))
                parent.enterNewRoom(0, -1);
            else if (!parent.hblock && !parent.hloop && ((test.X + width) >= (25 * 32)))
                parent.enterNewRoom(1, 0);
            else if (!parent.hblock && !parent.hloop && ((test.Y + height) >= (13 * 32)))
                parent.enterNewRoom(0, 1);
            else if (!parent.hblock && !parent.hloop && (!parent.isSolid(test, 1, width, height, faceDir)))
                location = test;
            else if (flickerCount <= 0 && !(recoiler is AdventureEntity))
                recoiler.Move(-move_dist);
        }

        public override void Update()
        {
            base.Update();

            if ((z == 0) && (parent.isSolid(this.location, 0, 0, 0, faceDir)))
            {
                moving = false;
                Flicker();
                parent.Drown();
            }

            if (!this.moving)
                this.currentFrame = 0;

            if (z != 0)
                this.currentFrame = 1;
        }

        public override void Move(Vector2 move_dist)
        {
            Vector2 test = move_dist + location;
            if (!parent.hblock && !parent.hloop && ((test.X - width) <= 0))
                parent.enterNewRoom(-1, 0);
            else if (!parent.vblock && !parent.vloop && ((test.Y - height) <= 0))
                parent.enterNewRoom(0, -1);
            else if (!parent.hblock && !parent.hloop && ((test.X + width) >= (25 * 32)))
                parent.enterNewRoom(1, 0);
            else if (!parent.vblock && !parent.vloop && ((test.Y + height) >= (13 * 32)))
                parent.enterNewRoom(0, 1);
            else if (parent.Collide(flickerCount))
                ; // Deliberately left blank
            else
            {
                base.Move(move_dist);
                if ((z == 0) && (parent.isInjury(location, width, height)))
                {
                    Hurt();
                }
                parent.tileAction(test, width, height);
            }
        }

        public override void Touch()
        {
            // Do nothing
        }

        public override bool inRange(AdventurePlayer player)
        {
            // Do nothing
            return false;
        }
    }
}
