using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    public abstract class Weapon
    {
        public abstract void Activate(AdventurePlayer player, AdventureScreen screen, Game game);
        public abstract void Draw(SpriteBatch spriteBatch, int x, int y);
        public abstract string getLabel();

        public virtual void Update()
        {

        }

        public virtual void Extra(Weapon weapon)
        {
            
        }
    }

    class NullWeapon : Weapon
    {
        public override void Activate(AdventurePlayer player, AdventureScreen screen, Game game)
        {
            //
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            //
        }

        public override string getLabel()
        {
            return " ";
        }
    }

    class JumpWeapon : Weapon
    {
        public override void Activate(AdventurePlayer player, AdventureScreen screen, Game game)
        {
            player.Jump();
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            Rectangle source = new Rectangle(0, 64, 32, 32);
            Rectangle dest = new Rectangle(x, y, 32, 32);
            spriteBatch.Begin();
            spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.White);
            spriteBatch.End();
        }

        public override string getLabel()
        {
            return "JUMP";
        }
    }

    class ProjectileWeapon : Weapon
    {
        int cooldown = 0;

        public override void Activate(AdventurePlayer player, AdventureScreen screen, Game game)
        {
            if ((cooldown == 0) && (player.z == 0))
            {
                Vector2 location = new Vector2(player.location.X, player.location.Y - 16);
                AdventureProjectile proj = new AdventureProjectile(true, player.faceDir, location, 30);
                screen.addObject(proj);
                cooldown = 30;
                PlaySound.Pew();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            Rectangle source = new Rectangle(32, 64, 32, 32);
            Rectangle dest = new Rectangle(x, y, 32, 32);
            spriteBatch.Begin();
            spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.White);
            spriteBatch.End();
        }

        public override void Update()
        {
            if (cooldown >= 0)
            {
                cooldown = cooldown - 1;
            }
            else
                cooldown = 0;
        }

        public override string getLabel()
        {
            return "PROJECTILE";
        }
    }

    class FishWeapon : Weapon
    {
        int count = 1;
        int lag = 0;

        public override void Activate(AdventurePlayer player, AdventureScreen screen, Game game)
        {
            if (lag < 1 && count > 0)
            {
                game.life = game.life + 2;
                if (game.life > game.possibleLife)
                    game.life = game.possibleLife;
                PlaySound.Aspect();
                count--;
                lag = 20;

                if (count == 0)
                    game.RemoveWeapon(this);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            Rectangle sourceRectangle = new Rectangle(64, 64, 32, 32);
            Rectangle destinationRectangle = new Rectangle(x, y, 32, 32);

            if (count > 0)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(Master.texCollection.controls, destinationRectangle, sourceRectangle, Color.White);

                sourceRectangle = new Rectangle(16 * count, 0, 16, 16);
                destinationRectangle = new Rectangle(x + 16, y, 16, 16);
                spriteBatch.Draw(Master.texCollection.arcadeFont, destinationRectangle, sourceRectangle, Color.White);
                spriteBatch.End();
            }
        }

        public override void Extra(Weapon weapon)
        {
            if (count != 9)
                count = count + 1;
        }

        public override void Update()
        {
            lag = lag - 1;
        }

        public override string getLabel()
        {
            return "FISH RESTORES 1 HEART";
        }
    }
}
