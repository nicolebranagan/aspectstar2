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
        Selections selection = Selections.SaveAndExit;
        Options opti;
        int lag = 20;

        enum Selections
        {
            Controls = 0,
            Music = 1,
            Sound = 2,
            FullScreen = 3,
            Default = 4,
            ExitWithoutChanging = 5,
            SaveAndExit = 6,

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
            opti = master.options.Clone();
            PlaySong.Play(PlaySong.SongName.Silent);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            WriteText(spriteBatch, "ASPECT STAR 2 OPTIONS", 2, Color.White);

            WriteText(spriteBatch, "SET KEYBOARD CONTROLS", 5, 6, Color.White);

            WriteText(spriteBatch, "UP", 7, 8, selection == Selections.Up ? Color.Red : Color.White);
            WriteText(spriteBatch, "DOWN", 9, 8, selection == Selections.Down ? Color.Red : Color.White);
            WriteText(spriteBatch, "LEFT", 11, 8, selection == Selections.Left ? Color.Red : Color.White);
            WriteText(spriteBatch, "RIGHT", 13, 8, selection == Selections.Right ? Color.Red : Color.White);

            WriteText(spriteBatch, "A", 7, 20, selection == Selections.A ? Color.Red : Color.White);
            WriteText(spriteBatch, "B", 9, 20, selection == Selections.B ? Color.Red : Color.White);
            WriteText(spriteBatch, "START", 13, 20, selection == Selections.Start ? Color.Red : Color.White);

            if (opti.music)
                WriteText(spriteBatch, "GAME MUSIC ENABLED", 16, 6, Color.White);
            else
                WriteText(spriteBatch, "GAME MUSIC DISABLED", 16, 6, Color.White);

            if (opti.sound)
                WriteText(spriteBatch, "SOUND EFFECTS ENABLED", 18, 6, Color.White);
            else
                WriteText(spriteBatch, "SOUND EFFECTS DISABLED", 18, 6, Color.White);

            if (opti.fullscreen)
                WriteText(spriteBatch, "ENTER WINDOWED MODE", 20, 6, Color.White);
            else
                WriteText(spriteBatch, "ENTER FULL SCREEN MODE", 20, 6, Color.White);

            WriteText(spriteBatch, "RETURN TO DEFAULT SETTINGS", 23, 6, Color.White);
            WriteText(spriteBatch, "RETURN TO PREVIOUS OPTIONS", 25, 6, Color.White);
            WriteText(spriteBatch, "EXIT OPTIONS AND SAVE CHANGES", 27, 6, Color.White);

            if ((int)selection < 100)
            {
                Rectangle source = new Rectangle(128, 16, 16, 16);
                Rectangle dest = new Rectangle(4 * 16, 0, 16, 16);
                
                switch (selection)
                {
                    case Selections.Controls:
                        dest.Y = 16 * 5;
                        break;
                    case Selections.Music:
                        dest.Y = 16 * 16;
                        break;
                    case Selections.Sound:
                        dest.Y = 18 * 16;
                        break;
                    case Selections.FullScreen:
                        dest.Y = 20 * 16;
                        break;
                    case Selections.Default:
                        dest.Y = 23 * 16;
                        break;
                    case Selections.ExitWithoutChanging:
                        dest.Y = 25 * 16;
                        break;
                    case Selections.SaveAndExit:
                        dest.Y = 27 * 16;
                        break;
                }

                spriteBatch.Begin();
                spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.MonoGameOrange);
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
                if (Master.controls.Up && selection != Selections.Controls)
                {
                    selection = (Selections)((int)selection - 1);
                    lag = 20;
                }
                else if (Master.controls.Down && selection != Selections.SaveAndExit)
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
                        case Selections.FullScreen:
                            opti.fullscreen = !opti.fullscreen;
                            PlaySound.Play(PlaySound.SoundEffectName.Pause);
                            master.ToggleFullScreen(opti.fullscreen);
                            break;
                        case Selections.Default:
                            opti = new Options();
                            master.ActivateOptions(opti);
                            PlaySound.Play(PlaySound.SoundEffectName.Pause);
                            break;
                        case Selections.ExitWithoutChanging:
                            PlaySound.Play(PlaySound.SoundEffectName.Boom);
                            Exit(false);
                            break;
                        case Selections.SaveAndExit:
                            PlaySound.Play(PlaySound.SoundEffectName.Pause);
                            Exit(true);
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

        void Exit(bool useNewOptions)
        {
            if (useNewOptions)
            {
                master.SaveOptions(opti);
                master.ActivateOptions(opti);
            }
            else
                master.ActivateOptions(master.options);

            master.UpdateScreen(new TitleScreen(master));
        }
    }
}
