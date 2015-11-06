using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace aspectstar2
{
    public class AdventureBoss1 : AdventureEnemy
    {
        public AdventureBoss1()
        {
            this.texture = Master.texCollection.texBosses;
            this.offset = new Vector2(32, 32);
            this.width = 30;
            this.height = 30;

            BestiaryEntry bossEntry = new BestiaryEntry();
            bossEntry.movementType = BestiaryEntry.MovementTypes.intelligent;
            bossEntry.speed = 5;
            bossEntry.intelligence = 7;
            bossEntry.decisiveness = 6;
            definition = bossEntry;
            health = 10;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            if (flickerCount > 0)
            {
                flickerCount--;
                if (flickerCount % 7 == 0)
                    mask = Color.Red;
            }

            int dim_x = 64;
            int dim_y = 64;
            int column = ((int)faceDir * 2) + currentFrame;
            Vector2 screen_loc = location - offset;

            Rectangle sourceRectangle = new Rectangle(dim_x * column, 0, dim_x, dim_y);
            Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y - (z * 2), dim_x, dim_y);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, mask);
            spriteBatch.End();
        }

        public override void Touch()
        {
            //
        }
    }


    public class AdventureBoss2 : AdventureEnemy
    {
        bool left = true;
        bool aboveWater = true;

        public AdventureBoss2()
        {
            this.texture = Master.texCollection.texBosses;
            this.offset = new Vector2(32, 32);
            this.width = 30;
            this.height = 30;
            this.health = 6;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            if (flickerCount > 0)
            {
                flickerCount--;
                if (currentFrame == 1)
                    mask = Color.Red;
            }

            int dim_x = 64;
            int dim_y = 64;

            int column = 0;
            if (aboveWater)
                column += 4;
            if (!left)
                column += 2;
            column = column + currentFrame;

            Vector2 screen_loc = location - offset;

            Rectangle sourceRectangle = new Rectangle(dim_x * column, 64, dim_x, dim_y);
            Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y - (z * 2), dim_x, dim_y);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, mask);
            spriteBatch.End();
        }

        public override void Touch()
        {
            //
        }

        public override void Update()
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

            // Move
            if (location.X + width + 32 >= Master.width)
                left = true;
            else if (location.X - width - 32 <= 0)
                left = false;

            if (stallCount % 4 != 0)
            {
                location = new Vector2(location.X + (left ? -2 : 2), location.Y);

                if (aboveWater && Master.globalRandom.Next(30) < 1)
                {
                    aboveWater = false;
                }
                else if (!aboveWater && Master.globalRandom.Next(30) < 5)
                {
                    aboveWater = true;
                    PlaySound.Play(PlaySound.SoundEffectName.Pew);
                }

                /*if (aboveWater && Master.globalRandom.Next(60) < 2)
                {
                    var aP = new AdventureProjectile(false, Master.Directions.Down, location, 300, 1);
                    parent.addObject(aP);
                    PlaySound.Play(PlaySound.SoundEffectName.Pew);
                }*/
            }
        }

        public override void Hurt(bool ghost, int damage)
        {
            if (aboveWater)
                base.Hurt(ghost, damage);
        }
    }
    public class AdventureBoss3 : AdventureEnemy
    {
        public AdventureBoss3()
        {
            this.texture = Master.texCollection.texBosses;
            this.offset = new Vector2(32, 32);
            this.width = 25;
            this.height = 25;
            this.interenemycollide = true;

            BestiaryEntry bossEntry = new BestiaryEntry();
            bossEntry.movementType = BestiaryEntry.MovementTypes.intelligent;
            bossEntry.speed = 5;
            bossEntry.intelligence = 7;
            bossEntry.decisiveness = 6;
            definition = bossEntry;
            health = 10;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            if (flickerCount > 0)
            {
                flickerCount--;
                if (flickerCount % 7 == 0)
                    mask = Color.Red;
            }

            int dim_x = 64;
            int dim_y = 64;
            int column = ((int)faceDir * 2) + currentFrame;
            Vector2 screen_loc = location - offset;

            Rectangle sourceRectangle = new Rectangle(dim_x * column, 128, dim_x, dim_y);
            Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y - (z * 2), dim_x, dim_y);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, mask);
            spriteBatch.End();
        }

        public override void Touch()
        {
            //
        }

        public override void Update()
        {
            if (stallCount == 7)
            {
                if (Master.globalRandom.Next(0, 9) > 7 && (flickerCount == 0))
                {
                    AdventureEnemy bunny = new AdventureEnemy(Master.currentFile.bestiary[6]);
                    bunny.location = new Vector2(location.X, location.Y - 2);
                    bunny.faceDir = faceDir;
                    parent.addObject(bunny);
                    PlaySound.Play(PlaySound.SoundEffectName.Aspect);
                }
            }

            base.Update();
        }
    }

    public class AdventureBoss4 : AdventureEnemy
    {
        int visibleCounter = 25;

        public AdventureBoss4()
        {
            this.texture = Master.texCollection.texBosses;
            this.offset = new Vector2(32, 32);
            this.width = 30;
            this.height = 30;

            BestiaryEntry bossEntry = new BestiaryEntry();
            bossEntry.movementType = BestiaryEntry.MovementTypes.intelligent;
            bossEntry.speed = 5;
            bossEntry.intelligence = 7;
            bossEntry.decisiveness = 6;
            definition = bossEntry;
            health = 12;
            ghost = true;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            mask = Color.Black;
            if (flickerCount > 0)
            {
                flickerCount--;
                if (flickerCount % 7 == 0)
                    mask = Color.Red;
            }
            else
                if (visibleCounter % 6 != 1) return;

            int dim_x = 64;
            int dim_y = 64;
            int column = ((int)faceDir * 2) + currentFrame;
            Vector2 screen_loc = location - offset;

            Rectangle sourceRectangle = new Rectangle(dim_x * column, 192, dim_x, dim_y);
            Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y - (z * 2), dim_x, dim_y);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, mask);
            spriteBatch.End();
        }

        public override void Touch()
        {
            //
        }

        public override void Update()
        {
            if (visibleCounter == 0)
            {
                if (Master.globalRandom.Next(0, 30) == 19)
                    visibleCounter = 25;
            }
            else
                visibleCounter--;

            base.Update();
        }
    }

    public class AdventureBoss5 : AdventureEnemy
    {
        Rectangle bluetangle = new Rectangle(0, 576, 32, 16);
        Rectangle greentangle = new Rectangle(0, 624, 32, 16);
        Rectangle redtangle = new Rectangle(0, 672, 32, 16);

        Aspects _aspect;
        Aspects currentAspect
        {
            get { return _aspect; }
            set
            {
                PlaySound.Play(PlaySound.SoundEffectName.Aspect);
                _aspect = value;
                switch (value)
                {
                    case Aspects.Blue:
                        definition.dependent = "_blue";
                        break;
                    case Aspects.Green:
                        definition.dependent = "_green";
                        break;
                    case Aspects.Red:
                        definition.dependent = "_red";
                        break;
                }
            }
        }

        enum Aspects
        {
            Blue = 0,
            Green = 1,
            Red = 2
        }

        public AdventureBoss5()
        {
            this.texture = Master.texCollection.texBosses;
            this.offset = new Vector2(32, 32);
            this.width = 30;
            this.height = 30;
            this.radius = 32;

            defense = true;

            BestiaryEntry bossEntry = new BestiaryEntry();
            bossEntry.movementType = BestiaryEntry.MovementTypes.intelligent;
            bossEntry.speed = 5;
            bossEntry.intelligence = 8;
            bossEntry.decisiveness = 7;
            bossEntry.wanderer = true;
            definition = bossEntry;
            health = 5;
            currentAspect = (Aspects)Master.globalRandom.Next(0, 3);
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            if (flickerCount > 0)
            {
                flickerCount--;
                if (flickerCount % 7 == 0)
                    mask = Color.Red;
            }

            int dim_x = 64;
            int dim_y = 64;
            int column = ((int)faceDir * 2) + currentFrame;
            int row;
            Rectangle signifierRect;

            switch (currentAspect)
            {
                case Aspects.Green:
                    row = 4;
                    signifierRect = greentangle;
                    break;
                case Aspects.Red:
                    row = 5;
                    signifierRect = redtangle;
                    break;
                default:
                    row = 0;
                    signifierRect = bluetangle;
                    break;
            }

            Vector2 screen_loc = location - offset;

            Rectangle sourceRectangle = new Rectangle(dim_x * column, row * dim_y, dim_x, dim_y);
            Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y - (z * 2), dim_x, dim_y);
            Rectangle signifierDestination = new Rectangle((int)screen_loc.X + 16, (int)screen_loc.Y - (z * 2) - 16, 32, 16);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, mask);
            spriteBatch.Draw(Master.texCollection.texEnemies, signifierDestination, signifierRect, Color.White);
            spriteBatch.End();
        }

        public override void Update()
        {
            base.Update();

            int x = (int)Math.Floor(location.X / 32);
            int y = (int)Math.Floor(location.Y / 32);

            if (x == 5 && y == 7 && currentAspect != Aspects.Blue)
                currentAspect = Aspects.Blue;
            else if (x == 12 && y == 4 && currentAspect != Aspects.Red)
                currentAspect = Aspects.Red;
            else if (x == 19 && y == 7 && currentAspect != Aspects.Green)
                currentAspect = Aspects.Green;
        }

        public override void Touch()
        {
            //
        }
    }

    public class AdventureBoss6 : AdventureEnemy
    {
            int frameCount = 0;

            public AdventureBoss6()
            {
                this.texture = Master.texCollection.texBosses;
                this.offset = new Vector2(32, 96);
                this.width = 32;
                this.height = 32;
                this.radius = 32;

                BestiaryEntry bossEntry = new BestiaryEntry();
                bossEntry.movementType = BestiaryEntry.MovementTypes.stationary;
                definition = bossEntry;
                health = 5;

                defense = true;
            }

            public override void Draw(SpriteBatch spriteBatch, Color mask)
            {
                frameCount++;
                if (frameCount == 30)
                    frameCount = 0;
                int frame = frameCount / 10;
                if (flickerCount > 0)
                {
                    if (frame == 1)
                        flickerCount--;
                    switch (frame)
                    {
                        case 0:
                            frame = 2;
                            break;
                        case 2:
                            frame = 0;
                            break;
                    }

                    frame = frame + 3;

                }

                Vector2 screen_loc = location - offset;

                Rectangle sourceRectangle = new Rectangle(frame * 64, 384, 64, 128);
                Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y - (z * 2), 64, 128);

                spriteBatch.Begin();
                spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, mask);
                spriteBatch.End();
            }

            public override void Update()
            {
                parent.SetFlag("_redlight", flickerCount > 0);
                base.Update();
            }

            public override void Die()
            {
                parent.SetFlag("_die", true);
                base.Die();
            }
        }

    public class AdventureBoss7 : AdventureEnemy
    {
        bool forward = true;

        public AdventureBoss7()
        {
            this.texture = Master.texCollection.texBosses;
            this.offset = new Vector2(32, 32);
            this.width = 30;
            this.height = 30;

            BestiaryEntry bossEntry = new BestiaryEntry();
            bossEntry.movementType = BestiaryEntry.MovementTypes.intelligent;
            bossEntry.speed = 4;
            bossEntry.intelligence = 7;
            bossEntry.decisiveness = 6;
            bossEntry.wanderer = true;
            definition = bossEntry;
            health = 12;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            if (flickerCount > 0)
            {
                flickerCount--;
                if (flickerCount % 7 == 0)
                    return;
            }

            int dim_x = 64;
            int dim_y = 64;
            int column = forward ? 0 : 1;
            Vector2 screen_loc = location - offset;

            Rectangle sourceRectangle = new Rectangle(dim_x * column, 512, dim_x, dim_y);
            Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y - (z * 2), dim_x, dim_y);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, mask);
            spriteBatch.End();
        }

        public override void Update()
        {
            if (!forward && faceDir == Master.Directions.Down)
                forward = true;
            else if (forward && faceDir == Master.Directions.Up)
                forward = false;

            base.Update();
        }

        public override void Move(Vector2 move_dist)
        {
            Vector2 test = move_dist + location;
            if ((test.X - width) >= 0 && (test.Y - height) >= 0 && (test.X + width) < (25 * 32) && (test.Y + height) < (13 * 32))
                location = test;
            base.Move(move_dist);
        }

        public override void Touch()
        {
            //
        }
    }
}
