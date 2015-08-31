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

            this.health = 4;
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
                    PlaySound.Pew();
                }

                if (aboveWater && Master.globalRandom.Next(60) < 2)
                {
                    var aP = new AdventureProjectile(false, Master.Directions.Down, location, 300);
                    parent.addObject(aP);
                    PlaySound.Pew();
                }
            }
        }

        public override void Hurt(bool ghost)
        {
            if (aboveWater)
                base.Hurt(ghost);
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
                    PlaySound.Aspect();
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
}
