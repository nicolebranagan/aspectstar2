using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace aspectstar2
{
    // AdventureItem is defined as items that inherit from AdventureObject
    // but do not require any actions to be performed during an update call.
    //
    // Their actions should be done as a Touch() call.

    public abstract class AdventureItem : AdventureObject
    {
        public override void Update()
        {
            // Do nothing
        }

        public override bool inRange(AdventurePlayer player)
        {
            return false;
            // Do nothing
        }
    }

    public class AdventureKey : AdventureItem
    {
        public AdventureKey()
        {
            this.texture = Master.texCollection.controls;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            if (!parent.isSolid(location, 0, 0, 0, 0))
            {
                Rectangle sourceRectangle = new Rectangle((128 + 64), 0, 16, 16);
                Rectangle destinationRectangle = new Rectangle((int)(this.location.X - 8), (int)(this.location.Y - 8), 16, 16);

                spriteBatch.Begin();
                spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);
                spriteBatch.End();
            }
        }

        public override void Touch()
        {
            if (active && !parent.isSolid(location, 0, 0, 0, 0))
            {
                active = false;
                parent.Keys = parent.Keys + 1;
                PlaySound.Play(PlaySound.SoundEffectName.Key);
            }
        }
    }

    public class AdventureHeart : AdventureItem
    {
        public AdventureHeart()
        {
            this.texture = Master.texCollection.controls;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            if (!parent.isSolid(location, 0, 0, 0, 0))
            {
                Rectangle sourceRectangle = new Rectangle(128, 0, 16, 16);
                Rectangle destinationRectangle = new Rectangle((int)(this.location.X - 8), (int)(this.location.Y - 8), 16, 16);

                spriteBatch.Begin();
                spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);
                spriteBatch.End();
            }
        }

        public override void Touch()
        {
            if (active && !parent.isSolid(location, 0, 0, 0, 0))
            {
                active = false;
                game.life = game.life + 2;
                if (game.life > game.possibleLife)
                    game.life = game.possibleLife;
                PlaySound.Play(PlaySound.SoundEffectName.Heal);
            }
        }
    }

    public class AdventureGoldKey : AdventureItem
    {
        public AdventureGoldKey()
        {
            this.texture = Master.texCollection.controls;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            Rectangle sourceRectangle = new Rectangle((128 + 48), 0, 16, 16);
            Rectangle destinationRectangle = new Rectangle((int)(this.location.X - 8), (int)(this.location.Y - 8), 16, 16);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);
            spriteBatch.End();
        }

        public override void Update()
        {
            if (parent.beaten)
                active = false; // Gold keys can not be collected from beaten dungeons!!
        }

        public override void Touch()
        {
            if (active)
            {
                active = false;
                game.goldKeys = game.goldKeys + 1;
                parent.beaten = true;
                PlaySound.Play(PlaySound.SoundEffectName.Key);
            }
        }
    }

    public class AdventureBell : AdventureItem
    {
        public AdventureBell()
        {
            this.texture = Master.texCollection.controls;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            Rectangle sourceRectangle = new Rectangle((128 + 64 + 16), 0, 16, 16);
            Rectangle destinationRectangle = new Rectangle((int)(this.location.X - 8), (int)(this.location.Y - 8), 16, 16);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);
            spriteBatch.End();
        }

        public override void Touch()
        {
            if (active)
            {
                active = false;
                game.bells++;
                PlaySound.Play(PlaySound.SoundEffectName.Coin);
            }
        }
    }

    public class AdventureTeleporter : AdventureItem
    {
        int dest, destx, desty, destroomX, destroomY;
        
        public AdventureTeleporter(int dest, int destx, int desty, int destroomX, int destroomY)
        {
            this.dest = dest;
            this.destx = destx;
            this.desty = desty;
            this.destroomX = destroomX;
            this.destroomY = destroomY;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            // Do nothing
        }

        public override void Touch()
        {
            //game.exitAdventure(parent.beaten, dest, destroomX, destroomY, destx, desty);
            if (parent.player.z == 0)
                parent.leaveAdventure(dest, destx, desty, destroomX, destroomY);
        }
    }
    public class AdventureSpecial : AdventureItem
    {
        int screen, key;
        Vector2 lastloc;
        bool works = true;

        public AdventureSpecial(int screen, int key)
        {
            this.screen = screen;
            this.key = key;

            texture = Master.texCollection.controls;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            Rectangle sourceRectangle = new Rectangle(160, 32, 32, 32);
            Rectangle destinationRectangle = new Rectangle((int)(this.location.X - 16), (int)(this.location.Y - 16), 32, 32);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);
            spriteBatch.End();
        }

        public override void Touch()
        {
            if (works)
            {
                parent.enterSpecialStage(screen, key);
                works = false;
                lastloc = parent.player.location;
            }
        }

        public override void Update()
        {
            if (!works && Vector2.Distance(lastloc, parent.player.location) > 32)
                works = true;
        }
    }

    public class AdventureTorch : AdventureItem
    {
        public AdventureTorch()
        {
            this.texture = Master.texCollection.controls;
        }

        public override void Draw(SpriteBatch spriteBatch, Color mask)
        {
            Rectangle sourceRectangle = new Rectangle(224, 0, 16, 16);
            Rectangle destinationRectangle = new Rectangle((int)(this.location.X - 8), (int)(this.location.Y - 8), 16, 16);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);
            spriteBatch.End();
        }

        public override void Touch()
        {
            if (active)
            {
                active = false;
                game.GetWeapon(new TorchWeapon(1));
                PlaySound.Play(PlaySound.SoundEffectName.Aspect);
            }
        }
    }

}
