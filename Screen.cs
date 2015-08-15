using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aspectstar2
{
    // Ported from Aspect Star, because why not?

    public abstract class Screen
    {
        protected Master master;

        abstract public void Update(GameTime gameTime);
        abstract public void Draw(SpriteBatch spriteBatch, GameTime gameTime);


        protected void WriteText(SpriteBatch spriteBatch, string text, Vector2 pos, Color textColor)
        {
            Rectangle destRect;
            Rectangle sourceRect;
            int asc;

            Texture2D font = Master.texCollection.arcadeFont;

            spriteBatch.Begin();
            for (int i = 0; i < text.Length; i++)
            {
                destRect = new Rectangle((int)pos.X + i * 16, (int)pos.Y, 16, 16);

                asc = (int)(text[i]);
                if ((asc >= 48) && (asc <= 57))
                {
                    asc = asc - 48;
                    sourceRect = new Rectangle(16 * asc, 0, 16, 16);
                    spriteBatch.Draw(font, destRect, sourceRect, textColor);
                }
                else if ((asc >= 65) && (asc <= 90))
                {
                    asc = (asc - 65) + 10;
                    sourceRect = new Rectangle(16 * asc, 0, 16, 16);
                    spriteBatch.Draw(font, destRect, sourceRect, textColor);
                }
            }
            spriteBatch.End();
        }
    }

    public class TitleScreen : Screen
    {
        public TitleScreen(Master master)
        {
            this.master = master;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            WriteText(spriteBatch, "ASPECT STAR 2", new Vector2((Master.width / 2) - (8 * 13), (Master.height / 2) - 24), Color.White);
            WriteText(spriteBatch, "PRE RELEASE DEMO", new Vector2((Master.width / 2) - (8 * 16), (Master.height / 2) - 8), Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Enter))
            {
                // begin game
                this.master.NewGame();
            }
        }
    }
}
