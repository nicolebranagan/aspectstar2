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
        public abstract void Activate(AdventurePlayer player, AdventureScreen screen);
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
        public override void Activate(AdventurePlayer player, AdventureScreen screen)
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
        public override void Activate(AdventurePlayer player, AdventureScreen screen)
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

        public override void Activate(AdventurePlayer player, AdventureScreen screen)
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
}
