using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace aspectstar2
{
    public class OptionsScreen : Screen
    {
        Selections selection = Selections.SaveAndExit;
        Options opti;
        int lag = 20;
        int timer = 0;

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
            Rectangle source, dest;

            WriteText(spriteBatch, "ASPECT STAR 2 OPTIONS", 2, Color.White);

            WriteText(spriteBatch, "SET KEYBOARD CONTROLS", 5, 6, Color.White);

            /*WriteText(spriteBatch, "UP", 7, 8, selection == Selections.Up ? Color.Red : Color.White);
            WriteText(spriteBatch, "DOWN", 9, 8, selection == Selections.Down ? Color.Red : Color.White);
            WriteText(spriteBatch, "LEFT", 11, 8, selection == Selections.Left ? Color.Red : Color.White);
            WriteText(spriteBatch, "RIGHT", 13, 8, selection == Selections.Right ? Color.Red : Color.White);

            WriteText(spriteBatch, "A", 7, 20, selection == Selections.A ? Color.Red : Color.White);
            WriteText(spriteBatch, "B", 9, 20, selection == Selections.B ? Color.Red : Color.White);
            WriteText(spriteBatch, "START", 13, 20, selection == Selections.Start ? Color.Red : Color.White);*/

            if (opti.music)
                WriteText(spriteBatch, "MUSIC VOLUME", 14, 6, Color.White);
            else
                WriteText(spriteBatch, "GAME MUSIC DISABLED", 14, 6, Color.White);

            if (opti.sound)
                WriteText(spriteBatch, "SOUND EFFECTS ENABLED", 16, 6, Color.White);
            else
                WriteText(spriteBatch, "SOUND EFFECTS DISABLED", 16, 6, Color.White);

            if (opti.fullscreen)
                WriteText(spriteBatch, "ENTER WINDOWED MODE", 18, 6, Color.White);
            else
                WriteText(spriteBatch, "ENTER FULL SCREEN MODE", 18, 6, Color.White);

            WriteText(spriteBatch, "RETURN TO DEFAULT SETTINGS", 22, 6, Color.White);
            WriteText(spriteBatch, "RETURN TO PREVIOUS OPTIONS", 24, 6, Color.White);
            WriteText(spriteBatch, "EXIT OPTIONS AND SAVE CHANGES", 26, 6, Color.White);

            spriteBatch.Begin();

            if (opti.music)
            {
                spriteBatch.Draw(Master.texCollection.blank, new Rectangle(19 * 16 + opti.volume * 16 + 2, 14 * 16, (10 - opti.volume) * 16 - 2, 14), Color.FromNonPremultiplied(120, 120, 120, 255));
                spriteBatch.Draw(Master.texCollection.blank, new Rectangle(19 * 16, 14 * 16, opti.volume * 16, 14),
                    opti.volume > 8 ?
                    Color.FromNonPremultiplied(216, 64, 96, 255) :
                    Color.FromNonPremultiplied(80, 128, 255, 255)
                    );
            }

            source = new Rectangle(0, 0, 192, 80);
            dest = new Rectangle(7 * 16, 7 * 16, 192, 80);
            spriteBatch.Draw(Master.texCollection.controller, dest, source, Color.White);

            // Draw arrow
            if ((int)selection < 100)
            {
                source = new Rectangle(128, 16, 16, 16);
                dest = new Rectangle(4 * 16, 0, 16, 16);
                
                switch (selection)
                {
                    case Selections.Controls:
                        dest.Y = 16 * 5;
                        break;
                    case Selections.Music:
                        dest.Y = 14 * 16;
                        break;
                    case Selections.Sound:
                        dest.Y = 16 * 16;
                        break;
                    case Selections.FullScreen:
                        dest.Y = 18 * 16;
                        break;
                    case Selections.Default:
                        dest.Y = 22 * 16;
                        break;
                    case Selections.ExitWithoutChanging:
                        dest.Y = 24 * 16;
                        break;
                    case Selections.SaveAndExit:
                        dest.Y = 26 * 16;
                        break;
                }
                spriteBatch.Draw(Master.texCollection.controls, dest, source, Color.MonoGameOrange);
            }
            else if (timer % 8 == 0)
            {
                switch (selection)
                {
                    case Selections.Up:
                        source = new Rectangle(32, 96, 16, 16);
                        break;
                    case Selections.Down:
                        source = new Rectangle(32, 128, 16, 16);
                        break;
                    case Selections.Left:
                        source = new Rectangle(16, 112, 16, 16);
                        break;
                    case Selections.Right:
                        source = new Rectangle(48, 112, 16, 16);
                        break;
                    case Selections.A:
                        source = new Rectangle(128, 120, 20, 20);
                        break;
                    case Selections.B:
                        source = new Rectangle(156, 120, 20, 20);
                        break;
                    case Selections.Start:
                        source = new Rectangle(84, 130, 24, 10);
                        break;
                    default:
                        spriteBatch.End();
                        return;
                }
                dest = new Rectangle(source.X + 7 * 16, source.Y - 80 + 7 * 16, source.Width, source.Height);
                spriteBatch.Draw(Master.texCollection.controller, dest, source, Color.White);
            }

            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            timer++;
            if (timer > 10)
                timer = 0;

            if (lag > 0)
            {
                lag--;
                return;
            }

            KeyboardState state = Keyboard.GetState();
            Keys[] downKeys = state.GetPressedKeys();

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
                            if (opti.music == false)
                                PlaySong.Play(PlaySong.SongName.Silent);
                            else
                                PlaySong.Play(PlaySong.SongName.Title);
                            break;
                        case Selections.Sound:
                            opti.sound = !opti.sound;
                            PlaySound.enabled = opti.sound;
                            PlaySound.Play(PlaySound.SoundEffectName.Pause);
                            break;
                        case Selections.FullScreen:
                            opti.fullscreen = !opti.fullscreen;
                            PlaySound.Play(PlaySound.SoundEffectName.Pause);
                            master.ToggleFullScreen(opti.fullscreen);
                            break;
                        case Selections.Default:
                            opti = new Options();
                            PlaySound.Play(PlaySound.SoundEffectName.Key);
                            Exit(true);
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

                if (selection == Selections.Music && opti.music)
                {
                    if (lag == 0)
                    {
                        PlaySong.Play(PlaySong.SongName.Title);
                        if (Master.controls.Left && opti.volume > 0)
                        {
                            opti.volume--;
                            PlaySong.SetVolume(opti.volume);
                            lag = 20;
                        }
                        else if (Master.controls.Right && opti.volume < 10)
                        {
                            opti.volume++;
                            PlaySong.SetVolume(opti.volume);
                            lag = 20;
                        }
                    }
                }
                else
                    PlaySong.Play(PlaySong.SongName.Silent);
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
