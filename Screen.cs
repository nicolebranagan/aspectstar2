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

        protected void WriteText(SpriteBatch spriteBatch, string text, int row, Color textColor)
        {
            WriteText(spriteBatch, text, new Vector2((Master.width / 2) - (8 * text.Length), row * 16), textColor);
        }

        protected void WriteText(SpriteBatch spriteBatch, string text, int row, int column, Color textColor)
        {
            WriteText(spriteBatch, text, new Vector2(column * 16, row * 16), textColor);
        }

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
                if (asc == 42)
                {
                    sourceRect = new Rectangle((128), 0, 16, 16);
                    spriteBatch.Draw(Master.texCollection.controls, destRect, sourceRect, Color.White);
                }
                else if (asc == 46)
                {
                    sourceRect = new Rectangle(592, 8, 16, 8);
                    destRect.Y = destRect.Y + 8;
                    destRect.Height = 8;
                    spriteBatch.Draw(font, destRect, sourceRect, textColor);
                }
                else if ((asc >= 48) && (asc <= 57))
                {
                    asc = asc - 48;
                    sourceRect = new Rectangle(16 * asc, 0, 16, 16);
                    spriteBatch.Draw(font, destRect, sourceRect, textColor);
                }
                else if (asc == 58)
                {
                    sourceRect = new Rectangle(592, 0, 16, 16);
                    spriteBatch.Draw(font, destRect, sourceRect, textColor);
                }
                else if (asc == 60)
                {
                    sourceRect = new Rectangle(128, 16, 16, 16);
                    spriteBatch.Draw(Master.texCollection.controls, new Vector2(destRect.X - 2, destRect.Y - 2), sourceRect, textColor, (float)Math.PI, new Vector2(16,16), 1, SpriteEffects.None, 0);
                }
                else if (asc == 62)
                {
                    sourceRect = new Rectangle(128, 16, 16, 16);
                    spriteBatch.Draw(Master.texCollection.controls, destRect, sourceRect, textColor);
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
        SavedGame savedGame;

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
            this.savedGame = master.savedGame;
            if (savedGame == null)
                saveFailed = true;
            PlaySong.Play(PlaySong.SongName.Title);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //WriteText(spriteBatch, "ASPECT STAR 2", new Vector2((Master.width / 2) - (8 * 13), (Master.height / 2) - 128), Color.White);
            //WriteText(spriteBatch, "PRE RELEASE DEMO", new Vector2((Master.width / 2) - (8 * 16), (Master.height / 2) - 96), Color.White);

            WriteText(spriteBatch, "NEW GAME", new Vector2((Master.width / 2) - (2 * 16), (Master.height / 2) - 40), Color.White);
            if (!saveFailed)
                WriteText(spriteBatch, "CONTINUE", new Vector2((Master.width / 2) - (2 * 16), (Master.height / 2) - 8), Color.White);
            WriteText(spriteBatch, "OPTIONS", new Vector2((Master.width / 2) - (2 * 16), (Master.height / 2) + 24), Color.White);

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
                    if (saveFailed && selection == Selections.Continue)
                        selection = Selections.NewGame;
                    lag = 20;
                }
                else if (Master.controls.Down && (int)selection != 2)
                {
                    selection = (Selections)((int)selection + 1);
                    if (saveFailed && selection == Selections.Continue)
                        selection = Selections.Options;
                    lag = 20;
                }
                else if (Master.controls.Start || Master.controls.A || Master.controls.B)
                {
                    switch (selection)
                    {
                        case Selections.NewGame:
                            // begin game
                            master.NewGame();
                            PlaySound.Play(PlaySound.SoundEffectName.Aspect);
                            break;
                        case Selections.Continue:
                            if (!saveFailed)
                            {
                                PlaySound.Play(PlaySound.SoundEffectName.Pause);
                                master.LoadGameFromSaved(savedGame);
                            }
                            break;
                        case Selections.Options:
                            master.UpdateScreen(new OptionsScreen(master));
                            PlaySound.Play(PlaySound.SoundEffectName.Pause);
                            break;
                    }
                }
            }
        }
    }

    public class TextScreen : Screen
    {
        Game game;
        Action<bool> activator;
        bool credits;
        bool creditsDone = false;

        int lag = 20;

        int timeCount;
        int timeLag = 1;
        int floatCount = 0;
        bool up = true;
        string[] text;

        public TextScreen(Game game, string text, Action<bool> activator, bool credits)
        {
            this.text = text.Split(Environment.NewLine.ToCharArray());
            this.game = game;
            this.activator = activator;
            this.credits = credits;
            timeCount = Master.height;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (credits)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(Master.texCollection.credits, new Rectangle(Master.width - 128 - 32, Master.height - 127 + (floatCount - 5), 128, 96), Color.White);
                spriteBatch.End();
            }
            for(int i = 0; i < text.Length; i++)
            {   
                WriteText(spriteBatch, text[i], new Vector2(Master.width / 2 - text[i].Length * 8, timeCount + 32 * i), Color.White);
            }
            if (creditsDone)
            {
                string deathCount;
                if (game.deaths > 0)
                    deathCount = string.Concat("YOU DIED ", game.deaths.ToString(), " TIMES");
                else
                    deathCount = "";
                WriteText(spriteBatch, "CONGRATULATION", new Vector2(Master.width / 2 - 14 * 8, Master.height / 2 - 16), Color.White);
                WriteText(spriteBatch, deathCount, new Vector2(Master.width / 2 - deathCount.Length * 8, Master.height / 2 + 16), Color.White);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (timeCount > Math.Min(-text.Length * 32, 0))
            {
                if (credits)
                {
                    timeLag = timeLag - 1;
                    if (timeLag == 0)
                    {
                        timeLag = 1;
                        timeCount = timeCount - 1;
                    }
                }
                else
                    timeCount = timeCount - 1;
            }
            else
            {
                activator(true);
                if (credits) creditsDone = true;
            }

            if (floatCount <= 0)
                up = true;
            else if (floatCount >= 10)
                up = false;

            if (timeCount % 20 == 0)
            {
                if (up)
                    floatCount = floatCount + 2;
                else
                    floatCount = floatCount - 2;
            }

            if (lag > 0)
                lag--;
            else if (Master.controls.Start)
                activator(false); //game.Begin();
        }
    }
}
