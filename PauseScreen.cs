using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace aspectstar2
{
    class PauseScreen : Screen
    {
        AdventureScreen screen;
        Game game;
        int selection = 0;
        int lag = 10;

        public PauseScreen(AdventureScreen screen, Game game)
        {
            this.screen = screen;
            this.game = game;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            screen.DrawRoom(spriteBatch, Color.FromNonPremultiplied(255,255,255,100));

            WriteText(spriteBatch, "ABILITIES AND WEAPONS", new Vector2(32, 32), Color.White);

            int i = 0;
            foreach (Weapon w in game.weapons)
            {

                spriteBatch.Begin();
                spriteBatch.Draw(Master.texCollection.blank, new Rectangle(32 + (i * (32 + 16)), 64 + 16, 32, 32), (i == selection) ? Color.DarkRed : Color.Black);
                spriteBatch.End();

                w.Draw(spriteBatch, 32 + (i * (32 + 16)), 64 + 16);

                if (w == game.weaponA)
                    WriteText(spriteBatch, "A", new Vector2(32 + (i * (32 + 16)), 64), Color.White);
                else if (w == game.weaponB)
                    WriteText(spriteBatch, "B", new Vector2(32 + (i * (32 + 16)), 64), Color.White);
                i++;
            }

            WriteText(spriteBatch, game.weapons[selection].getLabel(), new Vector2(32, 128), Color.White);

            screen.drawStatus(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (lag > 0)
                lag = lag - 1;
            else
            {
                KeyboardState state = Keyboard.GetState();
                if (state.IsKeyDown(Master.controls.Left))
                {
                    selection = selection - 1;
                    if (selection < 0)
                        selection = 0;
                    lag = 15;
                }
                else if (state.IsKeyDown(Master.controls.Right))
                {
                    selection = selection + 1;
                    if (selection == game.weapons.Count)
                        selection = selection - 1;
                    lag = 15;
                }
                else if (state.IsKeyDown(Master.controls.A))
                {
                    game.weaponA = game.weapons[selection];
                    if (game.weaponA == game.weaponB)
                        game.weaponB = new NullWeapon();
                    lag = 15;
                }
                else if (state.IsKeyDown(Master.controls.B))
                {
                    game.weaponB = game.weapons[selection];
                    if (game.weaponB == game.weaponA)
                        game.weaponA = new NullWeapon();
                    lag = 15;
                }
                else if (state.IsKeyDown(Master.controls.Start))
                {
                    game.Unpause();
                }
            }
        }
    }
}
