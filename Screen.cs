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
        Selections selection = Selections.NewGame;
        bool saveFailed = false;

        enum Selections
        {
            NewGame = 0,
            Continue = 1,
            Options = 2,
        }

        int lag = 20;

        public TitleScreen(Master master)
        {
            this.master = master;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //WriteText(spriteBatch, "ASPECT STAR 2", new Vector2((Master.width / 2) - (8 * 13), (Master.height / 2) - 128), Color.White);
            //WriteText(spriteBatch, "PRE RELEASE DEMO", new Vector2((Master.width / 2) - (8 * 16), (Master.height / 2) - 96), Color.White);

            WriteText(spriteBatch, "NEW GAME", new Vector2((Master.width / 2) - (2 * 16), (Master.height / 2) - 40), Color.White);
            if (saveFailed)
                WriteText(spriteBatch, "NO SAVED GAME", new Vector2((Master.width / 2) - (2 * 16), (Master.height / 2) - 8), Color.White);
            else
                WriteText(spriteBatch, "CONTINUE", new Vector2((Master.width / 2) - (2 * 16), (Master.height / 2) - 8), Color.White);
            WriteText(spriteBatch, "TEST SPECIAL SCREEN", new Vector2((Master.width / 2) - (2 * 16), (Master.height / 2) + 24), Color.White);

            WriteText(spriteBatch, "PRE RELEASE DEMO", new Vector2((Master.width / 2) - (8 * 16), (Master.height / 2) + 96 - 16), Color.White);
            WriteText(spriteBatch, "NICOLE 2015", new Vector2((Master.width / 2) - (8 * 10), (Master.height / 2) + 96 ), Color.White);

            spriteBatch.Begin();
            Rectangle source = new Rectangle(128, 16, 16, 16);
            Rectangle dest = new Rectangle((Master.width / 2) - Master.texCollection.title.Width / 2, (Master.height / 2) - 192,
                Master.texCollection.title.Width, Master.texCollection.title.Height);

            spriteBatch.Draw(Master.texCollection.title, dest, Color.White);

            dest = new Rectangle((Master.width / 2) - (4 * 16), (Master.height / 2) - 40 + (32 * (int)selection), 16, 16);

            spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.Cyan);
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (lag > 0)
                lag = lag - 1;

            //KeyboardState state = Keyboard.GetState();

            if (lag == 0)
            {
                if (Master.controls.Up && selection != 0)
                {
                    selection = (Selections)((int)selection - 1);
                    lag = 20;
                }
                else if (Master.controls.Down && (int)selection != 2)
                {
                    selection = (Selections)((int)selection + 1);
                    lag = 20;
                }
                else if (Master.controls.Start || Master.controls.A || Master.controls.B)
                {
                    switch (selection)
                    {
                        case Selections.NewGame:
                            // begin game
                            master.NewGame();
                            PlaySound.Aspect();
                            break;
                        case Selections.Continue:
                            PlaySound.Pause();
                            if (!saveFailed)
                                saveFailed = !(master.LoadGame());
                            break;
                        case Selections.Options:
                            master.UpdateScreen(new SpecialScreen(new Game(master), 0, 0, x => master.UpdateScreen(this)));
                            break;
                    }
                }
            }
        }
    }
}
