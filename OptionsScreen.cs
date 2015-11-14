using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace aspectstar2
{
    public class OptionsScreen : Screen
    {
        Selections selection = Selections.Exit;
        Options opti;
        int lag = 20;

        enum Selections
        {
            Controls = 0,
            Music = 1,
            Sound = 2,
            Exit = 3,

            Up = 101,
            Down = 102,
            Left = 103,
            Right = 104,
            A = 105,
            B = 106,
            Start = 107,
        }

        public OptionsScreen(Master master)
        {
            this.master = master;
            opti = master.options;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            WriteText(spriteBatch, "ASPECT STAR 2", 1, Color.White);
            WriteText(spriteBatch, "OPTIONS", 2, Color.White);

            WriteText(spriteBatch, "CHOOSE CONTROLS", 6, 6, Color.White);

            WriteText(spriteBatch, "UP", 8, 8, selection == Selections.Up ? Color.Red : Color.White);
            WriteText(spriteBatch, "DOWN", 10, 8, selection == Selections.Down ? Color.Red : Color.White);
            WriteText(spriteBatch, "LEFT", 12, 8, selection == Selections.Left ? Color.Red : Color.White);
            WriteText(spriteBatch, "RIGHT", 14, 8, selection == Selections.Right ? Color.Red : Color.White);

            WriteText(spriteBatch, "A", 8, 20, selection == Selections.A ? Color.Red : Color.White);
            WriteText(spriteBatch, "B", 10, 20, selection == Selections.B ? Color.Red : Color.White);
            WriteText(spriteBatch, "START", 14, 20, selection == Selections.Start ? Color.Red : Color.White);

            if (opti.music)
                WriteText(spriteBatch, "GAME MUSIC ENABLED", 18, 6, Color.White);
            else
                WriteText(spriteBatch, "GAME MUSIC DISABLE", 18, 6, Color.White);

            if (opti.sound)
                WriteText(spriteBatch, "SOUND EFFECTS ENABLED", 20, 6, Color.White);
            else
                WriteText(spriteBatch, "SOUND EFFECTS DISABLED", 20, 6, Color.White);


            WriteText(spriteBatch, "EXIT OPTIONS AND SAVE CHANGES", 26, 6, Color.White);

            if ((int)selection < 100)
            {
                Rectangle source = new Rectangle(128, 16, 16, 16);
                Rectangle dest = new Rectangle(4 * 16, 0, 16, 16);
                
                switch (selection)
                {
                    case Selections.Controls:
                        dest.Y = 16 * 6;
                        break;
                    case Selections.Music:
                        dest.Y = 16 * 18;
                        break;
                    case Selections.Sound:
                        dest.Y = 20 * 16;
                        break;
                    case Selections.Exit:
                        dest.Y = 26 * 16;
                        break;
                }

                spriteBatch.Begin();
                spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.Cyan);
                spriteBatch.End();
            }

        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            Keys[] downKeys = state.GetPressedKeys();
            if (lag > 0)
            {
                lag--;
                return;
            }

            if ((int)selection < 100)
            {
                if (Master.controls.Up && selection != 0)
                {
                    selection = (Selections)((int)selection - 1);
                    lag = 20;
                }
                else if (Master.controls.Down && (int)selection != 3)
                {
                    selection = (Selections)((int)selection + 1);
                    lag = 20;
                }

                if ((Master.controls.Start) || (Master.controls.A))
                {
                    lag = 20;
                    switch (selection)
                    {
                        case Selections.Controls:
                            PlaySound.Play(PlaySound.SoundEffectName.Key);
                            selection = Selections.Up;
                            break;
                        case Selections.Music:
                            opti.music = !opti.music;
                            PlaySound.Play(PlaySound.SoundEffectName.Pause);
                            break;
                        case Selections.Sound:
                            opti.sound = !opti.sound;
                            PlaySound.Play(PlaySound.SoundEffectName.Pause);
                            break;
                        case Selections.Exit:
                            PlaySound.Play(PlaySound.SoundEffectName.Pause);
                            Exit();
                            break;
                    }
                }
            }
            else if (downKeys.Count() > 0)
            {
                Keys testKey = downKeys[0];
                switch (selection)
                {
                    case Selections.Up:
                        opti.Up = testKey;
                        break;
                    case Selections.Down:
                        opti.Down = testKey;
                        break;
                    case Selections.Left:
                        opti.Left = testKey;
                        break;
                    case Selections.Right:
                        opti.Right = testKey;
                        break;
                    case Selections.A:
                        opti.A = testKey;
                        break;
                    case Selections.B:
                        opti.B = testKey;
                        break;
                    case Selections.Start:
                        opti.Start = testKey;
                        break;
                }
                PlaySound.Play(PlaySound.SoundEffectName.Pause);
                lag = 20;
                if (selection == Selections.Start)
                    selection = Selections.Controls;
                else
                    selection++;
            }
        }

        void Exit()
        {
            master.SaveOptions(opti);
            master.ActivateOptions(opti);
            master.UpdateScreen(new TitleScreen(master));
        }
    }
}
