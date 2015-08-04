﻿using System;
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
            Rectangle sourceRectangle = new Rectangle((128 + 64), 0, 16, 16);
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
                parent.Keys = parent.Keys + 1;
                PlaySound.Key();
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
            Rectangle sourceRectangle = new Rectangle(128, 0, 16, 16);
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
                game.life = game.life + 2;
                if (game.life > game.possibleLife)
                    game.life = game.possibleLife;
                PlaySound.Aspect();
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
                PlaySound.Key();
            }
        }
    }
}