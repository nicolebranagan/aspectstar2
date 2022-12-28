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
        int waterTimer;

        public AdventureBoss2()
        {
            this.texture = Master.texCollection.texBosses;
            this.offset = new Vector2(32, 32);
            this.width = 30;
            this.height = 30;
            this.health = 7;
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

            if (stallCount % 4 != 0)
            {
                if (waterTimer % 3 != 0)
                    location = new Vector2(location.X + (left ? -2 : 2), location.Y);

                if (aboveWater)
                {
                    waterTimer = Master.globalRandom.Next(60) < 1 ? waterTimer - 40 : waterTimer - 1;

                    if (waterTimer % 50 == 0)
                    {
                        PlaySound.Play(PlaySound.SoundEffectName.Pew);
                        var aP = new SeekerProjectile(parent, new Vector2(location.X, location.Y), 500);
                        parent.addObject(aP);
                    }

                    if (waterTimer < 0)
                    {
                        aboveWater = false;
                        waterTimer = 200;
                    }
                }
                else if (!aboveWater)
                {
                    waterTimer--;

                    if (waterTimer == 0)
                    {
                        if (Master.globalRandom.Next(60) < 1)
                            waterTimer = waterTimer - 30;
                        aboveWater = true;
                        waterTimer = 400;
                    }
                }
            }

            // Move
            if (location.X + width + 32 * 7 >= Master.width)
                left = true;
            else if (location.X - width - 32 * 7 <= 0)
                left = false;
        }

        public override void Hurt(bool ghost, int damage)
        {
            if (aboveWater)
            {
                base.Hurt(ghost, damage);
                aboveWater = false;
                waterTimer = 100;
                left = !left;
            }
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
            if (stallCount == 7 && Master.globalRandom.Next(0, 9) > 7 && (flickerCount == 0))
            {
                AdventureEnemy bunny = new AdventureEnemy(Master.currentFile.bestiary[6], 6);
                bunny.location = new Vector2(location.X, location.Y - 2);
                bunny.faceDir = faceDir;
                parent.addObject(bunny);
                PlaySound.Play(PlaySound.SoundEffectName.Aspect);
            }

            base.Update();
        }
    }

    public class AdventureBoss4 : AdventureEnemy
    {
        int visibleCounter = 25;
        int randomCounter;

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
            health = 9;
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

            if (randomCounter == 0)
            {
                definition.movementType = BestiaryEntry.MovementTypes.intelligent;
            }
            else
                randomCounter--;

            base.Update();
        }

        public override bool inRange(AdventurePlayer player)
        {
            if (base.inRange(player))
            {
                randomCounter = 250;
                definition.movementType = BestiaryEntry.MovementTypes.random;
            }
            return false;
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
            {
                currentAspect = Aspects.Blue;
                PlaySound.Play(PlaySound.SoundEffectName.Aspect);
            }
            else if (x == 12 && y == 4 && currentAspect != Aspects.Red)
            {
                currentAspect = Aspects.Red;
                PlaySound.Play(PlaySound.SoundEffectName.Aspect);
            }
            else if (x == 19 && y == 7 && currentAspect != Aspects.Green)
            {
                currentAspect = Aspects.Green;
                PlaySound.Play(PlaySound.SoundEffectName.Aspect);
            }
        }

        public override void Touch()
        {
            //
        }
    }

    public class AdventureBoss6 : AdventureEnemy
    {
        int frameCount;

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
                switch (frame)
                {
                    case 1:
                        flickerCount--;
                        break;
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
        Master.Directions stableDir = Master.Directions.Down;

        public AdventureBoss7()
        {
            this.texture = Master.texCollection.texBosses;
            this.offset = new Vector2(32, 32);
            this.width = 30;
            this.height = 30;
            this.flickerTime = 55;

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
            int column = (int)stableDir;
            Vector2 screen_loc = location - offset;

            Rectangle sourceRectangle = new Rectangle(dim_x * column, 512, dim_x, dim_y);
            Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y - (z * 2), dim_x, dim_y);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, mask);
            spriteBatch.End();
        }

        public override void Update()
        {
            int del_x = (int)(parent.player.location.X - location.X);
            int del_y = (int)(parent.player.location.Y - location.Y);

            // TODO: https://rules.sonarsource.com/csharp/RSPEC-3358
            stableDir = Math.Abs(del_x) > Math.Abs(del_y) ?
                del_x > 0 ?
                    Master.Directions.Right :
                    Master.Directions.Left :
                        del_y > 0 ?
                            Master.Directions.Down :
                            Master.Directions.Up;

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

    public class AdventureBoss8 : AdventureEnemy
    {
        bool _active;
        bool left;

        public AdventureBoss8()
        {
            this.texture = Master.texCollection.texBosses;
            this.offset = new Vector2(32, 32);
            this.width = 30;
            this.height = 30;

            BestiaryEntry bossEntry = new BestiaryEntry();
            bossEntry.movementType = BestiaryEntry.MovementTypes.stationary;
            definition = bossEntry;
            health = 15;
        }

        public override void Initialize(AdventureScreen parent, Game game)
        {
            base.Initialize(parent, game);

            parent.addObject(new AdventureBoss8Helper(new Vector2(2 * 32 + 16, 3 * 32 + 16), true));

            parent.addObject(new AdventureBoss8Helper(new Vector2(22 * 32 + 16, 11 * 32 + 16), false));
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            if (flickerCount > 0)
            {
                flickerCount--;
                if (flickerCount % 7 == 0)
                    mask = Color.Black;
            }

            int dim_x = 64;
            int dim_y = 64;
            Vector2 screen_loc = location - offset;

            Rectangle sourceRectangle = new Rectangle(_active ? 192 : 64, 576, dim_x, dim_y);
            Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y - (z * 2), dim_x, dim_y);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, mask);
            spriteBatch.End();
        }

        int fireCount;
        public override void Update()
        {
            if (!_active && parent.GetFlag("_active"))
                fireCount = 15;
            if (fireCount > 0)
            {
                fireCount--;
                if (fireCount % 5 == 0)
                    parent.addObject(AdventureProjectile.getFieryProjectile(false, Master.Directions.Down, new Vector2(location.X, location.Y + 32), 400));
            }

            _active = parent.GetFlag("_active");
            if (!_active) { flickerCount = 0; fireCount = 0; }

            if (left && location.X == 5 * 32)
                left = false;
            else if (!left && location.X == 20 * 32)
                left = true;

            if (_active || health < 4)
            {
                location.X = left ? location.X - 2 : location.X + 2;
            }

            if (flickerCount == 1)
            {
                parent.SetCounter("_angrycount", 1);
            }

            base.Update();
        }

        public override void Hurt(bool ghostly, int damage)
        {
            if (_active)
            {
                fireCount = 0;
                base.Hurt(ghostly, damage);
            }
            else
                PlaySound.Play(PlaySound.SoundEffectName.Key);
        }
    }

    public class AdventureBoss8Helper : AdventureObject
    {
        bool _active;
        private readonly bool left;
        private bool down;
        int handOpen;

        public AdventureBoss8Helper(Vector2 location, bool left)
        {
            this.texture = Master.texCollection.texBosses;
            this.offset = new Vector2(16, 16);
            this.width = 14;
            this.height = 14;
            this.location = location;

            this.left = left;
            down = left;
            if (!left) handOpen = 200;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            int dim_x = 32;
            int dim_y = 32;
            int column = (_active ? 4 : 0) + (left ? 0 : 1);
            int row = (handOpen < 16 ? 0 : 1);
            Vector2 screen_loc = location - offset;

            Rectangle sourceRectangle = new Rectangle(dim_x * column, 576 + dim_y * row, dim_x, dim_y);
            Rectangle destinationRectangle = new Rectangle((int)screen_loc.X, (int)screen_loc.Y - (z * 2), dim_x, dim_y);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, mask);
            spriteBatch.End();
        }

        public override bool inRange(AdventurePlayer player)
        {
            return false;
        }

        public override void Touch()
        {
            return;
        }

        public override void Update()
        {
            _active = parent.GetFlag("_active");

            handOpen--;
            if (handOpen < 0)
                handOpen = 400;
            if (_active && handOpen > 100)
                handOpen = 100;
            if (_active)
            {
                if (handOpen < 16 && handOpen % 4 == 0)
                    parent.addObject(new SeekerProjectile(parent, location, 300));
            }
            else if (handOpen == 12 || handOpen == 8 || handOpen == 4)
            {
                parent.addObject(AdventureProjectile.getIntangibleProjectile(false, (left ? Master.Directions.Right : Master.Directions.Left), location, 200));
                PlaySound.Play(PlaySound.SoundEffectName.Laser);
            }

            if (down && location.Y == 11 * 32 + 16)
                down = false;
            else if (!down && location.Y == 3 * 32 + 16)
                down = true;

            if (handOpen > 16 || _active)
            {
                location.Y = down ? location.Y + 2 : location.Y - 2;
            }

            base.Update();
        }
    }

    public class AdventureBoss9 : AdventureEnemy
    {
        Aspect _aspect;

        Aspect aspect
        {
            get
            {
                return _aspect;
            }
            set
            {
                _aspect = value;
                switch (_aspect)
                {
                    case Aspect.None:
                        definition.dependent = "";
                        break;
                    case Aspect.Blue:
                        definition.dependent = "_blue";
                        break;
                    case Aspect.Green:
                        definition.dependent = "_green";
                        break;
                    case Aspect.Red:
                        definition.dependent = "_red";
                        break;
                }
            }
        }

        int timerCount;

        enum Aspect
        {
            None = 0,
            Blue = 1,
            Green = 2,
            Red = 3,
        }

        public AdventureBoss9()
        {
            this.texture = Master.texCollection.texFinal;

            BestiaryEntry bossEntry = new BestiaryEntry();
            bossEntry.movementType = BestiaryEntry.MovementTypes.intelligent;
            bossEntry.speed = 7;
            bossEntry.intelligence = 7;
            bossEntry.decisiveness = 6;
            bossEntry.wanderer = false;
            definition = bossEntry;
            defense = true;
            health = 10;

            aspect = (Aspect)Master.globalRandom.Next(1, 4);
            graphicsRow = (int)aspect;
        }

        public override void Update()
        {
            graphicsRow = (int)aspect;
            timerCount++;
            if (timerCount > 500)
            {
                ShuffleAspect();
                timerCount = 0;
            }
            base.Update();
        }

        void ShuffleAspect()
        {
            PlaySound.Play(PlaySound.SoundEffectName.Aspect);
            if (parent.GetFlag("_blue"))
            {
                switch (aspect)
                {
                    case Aspect.Blue:
                        aspect = (Aspect)Master.globalRandom.Next(2, 4);
                        break;
                    case Aspect.Green:
                        aspect = Aspect.Red;
                        break;
                    case Aspect.Red:
                        aspect = Aspect.Green;
                        break;
                }
            }
            else if (parent.GetFlag("_green"))
            {
                switch (aspect)
                {
                    case Aspect.Green:
                        aspect = (Aspect)Master.globalRandom.Next(2, 4);
                        if (aspect == Aspect.Green)
                            aspect = Aspect.Blue;
                        break;
                    case Aspect.Blue:
                        aspect = Aspect.Red;
                        break;
                    case Aspect.Red:
                        aspect = Aspect.Blue;
                        break;
                }
            }
            else if (parent.GetFlag("_red"))
            {
                switch (aspect)
                {
                    case Aspect.Red:
                        aspect = (Aspect)Master.globalRandom.Next(1, 3);
                        break;
                    case Aspect.Green:
                        aspect = Aspect.Blue;
                        break;
                    case Aspect.Blue:
                        aspect = Aspect.Green;
                        break;
                }
            }
            else
            {
                Aspect newAspect = (Aspect)Master.globalRandom.Next(1, 3);
                if ((aspect == Aspect.Blue && newAspect == Aspect.Blue) ||
                        (aspect == Aspect.Green && newAspect == Aspect.Green))
                    newAspect = Aspect.Red;
                aspect = newAspect;
            }
            graphicsRow = (int)aspect;
        }

        public override void Hurt(bool ghostly, int damage)
        {
            int a = health;
            base.Hurt(ghostly, damage);
            if (a != health)
                ShuffleAspect();
            if (health == 1)
                enterPhase2();
        }

        void enterPhase2()
        {
            PlaySound.Play(PlaySound.SoundEffectName.Enter);
            parent.player.row = 0;
            parent.SetName("");
            parent.EnterNewRoom(4, 3, 12, 11);
        }
    }


    public class AdventureBoss9Phase2 : AdventureObject
    {
        bool begin;
        int health = 9;
        int flickerCount;
        int bulletCount = 10;
        int laughCount;
        bool left;

        public AdventureBoss9Phase2()
        {
            this.texture = Master.texCollection.texFinal;
            graphicsRow = 0;
            left = Master.globalRandom.Next(2) == 0;
        }

        public override void Update()
        {
            if (!begin)
            {
                parent.TextBox("WHEN WILL YOU LEARN THAT YOUR BULLETS ARE     WORTHLESS AGAINST ME", true);
                begin = true;
            }

            if (left && location.X == 4 * 32 + 16)
                left = false;
            else if (!left && location.X == 21 * 32 - 16)
                left = true;

            if (stallCount % 2 == 0)
            {
                location.X = left ? location.X - 2 : location.X + 2;
            }
            else if (bulletCount == 0)
            {
                parent.addObject(
                    new AdventureProjectile(false, Master.Directions.Down, new Vector2(location.X, location.Y + 16), 300, 1));
                bulletCount = 40;
            }

            if (bulletCount > 0)
                bulletCount--;

            if (flickerCount > 0)
                flickerCount--;
            if (laughCount > 1)
                laughCount--;
            if (laughCount == 1)
            {
                texture = Master.texCollection.texFinal;
                graphicsRow = 0;
                faceDir = Master.Directions.Down;
            }

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            if (flickerCount > 0)
                mask = Color.Red;
            base.Draw(spriteBatch, mask);
        }

        public override bool inRange(AdventurePlayer player)
        {
            return false;
        }

        public override void Touch()
        {

        }

        public void Thud()
        {
            PlaySound.Play(PlaySound.SoundEffectName.Key);
            texture = Master.texCollection.texCharacters;
            graphicsRow = 6;
            faceDir = Master.Directions.Up;
            laughCount = 99;
        }

        public void Hurt()
        {
            if (flickerCount == 0)
            {
                PlaySound.Play(PlaySound.SoundEffectName.Boom);
                health--;
                flickerCount = 18;
                if (laughCount > 0)
                    laughCount = 1;
            }
            if (health == 0)
            {
                parent.TextBox("LOOKS LIKE YOU DID IT KITTEN                  TIME TO COLLECT YOUR REWARD", true);
                parent.ClearObjects();
                PlaySound.Play(PlaySound.SoundEffectName.Aspect);
                PlaySong.Play(PlaySong.SongName.Item);
                parent.OverwriteTile(10, 0, 2);
                parent.OverwriteTile(11, 0, 0);
                parent.OverwriteTile(12, 0, 0);
                parent.OverwriteTile(13, 0, 0);
                parent.OverwriteTile(14, 0, 2);
                parent.OverwriteTile(11, 1, 0);
                parent.OverwriteTile(12, 1, 0);
                parent.OverwriteTile(13, 1, 0);
                parent.OverwriteTile(11, 2, 0);
                parent.OverwriteTile(12, 2, 0);
                parent.OverwriteTile(13, 2, 0);

                parent.OverwriteTile(11, 4, 0);
                parent.OverwriteTile(11, 5, 0);
                parent.OverwriteTile(12, 4, 0);
                parent.OverwriteTile(12, 5, 0);
                parent.OverwriteTile(13, 4, 0);
                parent.OverwriteTile(13, 5, 0);
            }
        }
    }
}
