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
        protected int count = 1;

        public abstract void Activate(AdventurePlayer player, AdventureScreen screen, Game game);
        public abstract void Draw(SpriteBatch spriteBatch, int x, int y);
        public abstract string getLabel();

        public virtual void Update()
        {

        }

        public virtual void Extra(Weapon weapon)
        {
            
        }

        enum WeaponType
        {
            NullWeapon = 0,
            JumpWeapon = 1,
            ProjectileWeapon = 2,
            FishWeapon = 3,
            TorchWeapon = 4,
            FireballWeapon = 5,
            GhostlyWeapon = 6,
        }

        public storedWeapon packWeapon()
        {
            int type = 0;
            if (this is JumpWeapon)
                type = (int)WeaponType.JumpWeapon;
            else if (this is FireballWeapon)
                type = (int)WeaponType.FireballWeapon;
            else if (this is GhostlyWeapon)
                type = (int)WeaponType.GhostlyWeapon;
            else if (this is ProjectileWeapon)
                type = (int)WeaponType.ProjectileWeapon;
            else if (this is FishWeapon)
                type = (int)WeaponType.FishWeapon;
            else if (this is TorchWeapon)
                type = (int)WeaponType.TorchWeapon;

            storedWeapon sW = new storedWeapon();
            sW.type = type;
            sW.count = count;
            return sW;
        }

        public static Weapon unpackWeapon(storedWeapon packedWeapon)
        {
            WeaponType type = (WeaponType)packedWeapon.type;
            switch (type)
            {
                case WeaponType.JumpWeapon:
                    return new JumpWeapon();
                case WeaponType.ProjectileWeapon:
                    return new ProjectileWeapon();
                case WeaponType.FishWeapon:
                    return new FishWeapon(packedWeapon.count);
                case WeaponType.TorchWeapon:
                    return new TorchWeapon(packedWeapon.count);
                case WeaponType.FireballWeapon:
                    return new FireballWeapon();
                case WeaponType.GhostlyWeapon:
                    return new GhostlyWeapon();
                default:
                    return new NullWeapon();
            }
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
        protected int cooldown = 0;

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
        int lag = 0;

        public FishWeapon()
        {

        }

        public FishWeapon(int count)
        {
            this.count = count;
        }

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
            //if (count != 9)
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

    class TorchWeapon : Weapon
    {
        AdventureScreen currentScreen;

        public TorchWeapon()
        {
            count = 6;
        }

        public TorchWeapon(int count)
        {
            this.count = count;
        }

        public override void Extra(Weapon weapon)
        {
            count = count + 6;
        }

        public override void Activate(AdventurePlayer player, AdventureScreen screen, Game game)
        {
            currentScreen = screen;

            if (count > 0 && !screen.lit)
            {
                screen.lit = true;
                count = count - 1;
                if (!screen.lit)
                    PlaySound.Aspect();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            Rectangle sourceRectangle = new Rectangle(96, 64, 32, 32);
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

        public override string getLabel()
        {
            return "TORCHES";
        }
    }

    class FireballWeapon : ProjectileWeapon
    {
        public override void Activate(AdventurePlayer player, AdventureScreen screen, Game game)
        {
            if ((cooldown == 0) && (player.z == 0))
            {
                Vector2 location = new Vector2(player.location.X, player.location.Y - 16);
                AdventureProjectile proj = AdventureProjectile.getFieryProjectile(true, player.faceDir, location, 20);
                screen.addObject(proj);
                cooldown = 30;
                PlaySound.Pew();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            Rectangle source = new Rectangle(128, 64, 32, 32);
            Rectangle dest = new Rectangle(x, y, 32, 32);
            spriteBatch.Begin();
            spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.White);
            spriteBatch.End();
        }

        public override string getLabel()
        {
            return "FIREBALL";
        }
    }

    class GhostlyWeapon : ProjectileWeapon
    {
        public override void Activate(AdventurePlayer player, AdventureScreen screen, Game game)
        {
            if ((cooldown == 0) && (player.z == 0))
            {
                Vector2 location = new Vector2(player.location.X, player.location.Y - 16);
                AdventureProjectile proj = AdventureProjectile.getGhostlyProjectile(true, player.faceDir, location, 20);
                screen.addObject(proj);
                cooldown = 30;
                PlaySound.Pew();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            Rectangle source = new Rectangle(128 + 32, 64, 32, 32);
            Rectangle dest = new Rectangle(x, y, 32, 32);
            spriteBatch.Begin();
            spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.White);
            spriteBatch.End();
        }

        public override string getLabel()
        {
            return "ECTO-SHOT";
        }
    }
}
