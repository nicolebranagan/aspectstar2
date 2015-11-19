using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Drawing;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Xml;

namespace aspectstar2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Master : Microsoft.Xna.Framework.Game
    {
        public static Textures texCollection = new Textures();
        public static Controls controls = new Controls();

        public static Worldfile currentFile;

        public const int width = 800;
        public const int height = 480;

        public static Random globalRandom;
        public Options options;

        SavedGame _sG;
        public SavedGame savedGame
        {
            get
            {
                if (_sG == null)
                    return LoadGame();
                else
                    return _sG;
            }
            set
            {
                _sG = value;
            }
        }

        public enum Directions
        {
            Up = 1,
            Down = 0,
            Left = 2,
            Right = 3
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Screen currentScreen;

        Game currentGame = null;

        public Master()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // NES resolution: 256x240;
            //graphics.PreferredBackBufferWidth = (256*2);
            //graphics.PreferredBackBufferHeight = (240*2);

            Window.Title = "Aspect Star 2";
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            controls = new Controls();
            globalRandom = new Random();
            savedGame = LoadGame();

            options = LoadOptions();
            ActivateOptions(options);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load Worldfile XML
            XmlSerializer xS = new XmlSerializer(typeof(Worldfile));
            FileStream fS = new FileStream("worldfile.xml", FileMode.Open);
            Worldfile wF;
            wF = (Worldfile)xS.Deserialize(fS);
            Master.currentFile = wF;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Draw textures
            texCollection.blank = new Texture2D(GraphicsDevice, 1, 1);
            texCollection.blank.SetData(new Color[] { Color.White });

            // Load in-game content
            texCollection.title = Content.Load<Texture2D>("title");
            texCollection.arcadeFont = Content.Load<Texture2D>("arcadefont");
            texCollection.controls = Content.Load<Texture2D>("menu");
            texCollection.credits = Content.Load<Texture2D>("credits");
            texCollection.controller = Content.Load<Texture2D>("controller");
            texCollection.worldTiles = Content.Load<Texture2D>("protoworld");
            texCollection.texMapPlayer = Content.Load<Texture2D>("mapplayer");
            texCollection.specialTiles = Content.Load<Texture2D>("specialstage");
            texCollection.texSpecial = Content.Load<Texture2D>("special");
            texCollection.texAdvPlayer = Content.Load<Texture2D>("advplayer");
            texCollection.texShadows = Content.Load<Texture2D>("shadows");
            texCollection.texEnemies = Content.Load<Texture2D>("enemies");
            texCollection.texCharacters = Content.Load<Texture2D>("characters");
            texCollection.texProjectile = Content.Load<Texture2D>("projectile");
            texCollection.texBosses = Content.Load<Texture2D>("bosses");
            texCollection.texFinal = Content.Load<Texture2D>("darkmarion");
            texCollection.texPlosion = Content.Load<Texture2D>("explosion");

            texCollection.adventureTiles = new Texture2D[8];
            texCollection.adventureTiles[0] = Content.Load<Texture2D>("dungeon1");
            texCollection.adventureTiles[1] = Content.Load<Texture2D>("town");
            texCollection.adventureTiles[2] = Content.Load<Texture2D>("dungeon2");
            texCollection.adventureTiles[3] = Content.Load<Texture2D>("dungeon3");
            texCollection.adventureTiles[4] = Content.Load<Texture2D>("dungeon4");
            texCollection.adventureTiles[5] = Content.Load<Texture2D>("dungeon5");
            texCollection.adventureTiles[6] = Content.Load<Texture2D>("dungeon6");
            texCollection.adventureTiles[7] = Content.Load<Texture2D>("town2");

            // Load sound effects (made in sfxr) and songs
            PlaySound.Initialize(Content);
            PlaySong.Initialize(Content);

            // Load title screen
            currentScreen = new TitleScreen(this);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if ((currentScreen == null || currentScreen is TitleScreen) &&
                (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || state.IsKeyDown(Keys.Escape)))
                Exit();
            else if ((state.IsKeyDown(Keys.LeftAlt) || state.IsKeyDown(Keys.RightAlt)) && (state.IsKeyDown(Keys.Enter)))
                ToggleFullScreen();

            controls.Update();
            currentScreen.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            currentScreen.Draw(spriteBatch, gameTime);

            base.Draw(gameTime);
        }

        public void NewGame()
        {
            currentGame = new Game(this);
            if (currentFile.opening != null)
            {
                UpdateScreen(new TextScreen(currentGame, currentFile.opening, delegate(bool x) { currentGame.Begin(); }, false));
            }
            else
                currentGame.Begin();
        }

        public void UpdateScreen(Screen screen)
        {
            this.currentScreen = screen;
        }

        public void SaveGame(SavedGame game)
        {
            savedGame = game;
            XmlSerializer xS = new XmlSerializer(typeof(SavedGame));
            try
            {
                StreamWriter sW = new StreamWriter("saved.xml");
                xS.Serialize(sW, game);
                sW.Close();
            }
            catch
            {

            }
        }

        SavedGame LoadGame()
        {
            try
            {
                XmlSerializer xS = new XmlSerializer(typeof(SavedGame));
                FileStream fS = new FileStream("saved.xml", FileMode.Open);
                XmlTextReader xTR = new XmlTextReader(fS);
                SavedGame sG;
                sG = (SavedGame)xS.Deserialize(xTR);

                //currentGame = new Game(this);
                //currentScreen = currentGame.BeginFromSaved(sG);
                fS.Close();
                return sG;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void LoadGameFromSaved(SavedGame sG)
        {
            currentGame = new Game(this);
            currentScreen = currentGame.BeginFromSaved(sG);
        }

        public void ActivateOptions(Options opt)
        {
            PlaySound.enabled = opt.sound;
            PlaySong.enabled = opt.music;
            controls.ChangeDefinition(opt);
            ToggleFullScreen(opt.fullscreen);

            options = opt;
        }

        public void SaveOptions(Options opt)
        {
            XmlSerializer xS = new XmlSerializer(typeof(Options));
            try
            {
                StreamWriter sW = new StreamWriter("options.xml");
                xS.Serialize(sW, opt);
                sW.Close();
            }
            catch
            {
                
            }
        }
        
        public void ToggleFullScreen()
        {
            graphics.IsFullScreen = !graphics.IsFullScreen;
            graphics.ApplyChanges();
        }

        public void ToggleFullScreen(bool fullScreen)
        {
            graphics.IsFullScreen = fullScreen;
            graphics.ApplyChanges();
        }

        Options LoadOptions()
        {
            Options opti = new Options();
            try
            {
                XmlSerializer xS = new XmlSerializer(typeof(Options));
                FileStream fS = new FileStream("options.xml", FileMode.Open);
                XmlTextReader xTR = new XmlTextReader(fS);
                opti = (Options)xS.Deserialize(xTR);
                fS.Close();
            }
            catch (Exception)
            {
            }

            return opti;
        }

        public static Vector2 getMapTile(int index, Texture2D map)
        {
            int width = map.Width / 32;
            int height = map.Height / 32;

            return new Vector2(index % width * 32, index / width * 32);
        }

        public static int[] sumRooms(int[] roomleft, int[] roomright, int roomwidth, int roomheight)
        {
            int[] summed = new int[roomwidth * roomheight * 2];
            for (int i = 0; i < roomheight; i++)
            {
                Array.Copy(roomleft, i * roomwidth, summed, 2 * i * roomwidth, roomwidth);
                Array.Copy(roomright, i * roomwidth, summed, (2 * i + 1) * roomwidth, roomwidth);
            }
            return summed;
        }

        public static float absoluteMin(float var1, float var2, float var3)
        {
            float a1 = Math.Abs(var1);
            float a2 = Math.Abs(var2);
            float a3 = Math.Abs(var3);

            if (a1 > a2)
            {
                if (a2 > a3)
                    return var3;
                else
                    return var2;
            }
            else
            {
                if (a1 > a3)
                    return var3;
                else
                    return var1;
            }
        }
    }

    public struct Textures
    {
        // Game textures

        public Texture2D title;
        public Texture2D controls;
        public Texture2D arcadeFont;
        public Texture2D blank;
        public Texture2D credits;
        public Texture2D controller;

        public Texture2D worldTiles;
        public Texture2D[] adventureTiles;
        public Texture2D specialTiles;

        public Texture2D texMapPlayer;
        public Texture2D texAdvPlayer;
        public Texture2D texSpecial;
        public Texture2D texEnemies;
        public Texture2D texCharacters;
        public Texture2D texProjectile;
        public Texture2D texBosses;
        public Texture2D texFinal;

        public Texture2D texShadows;
        public Texture2D texPlosion;
    }

    public class Options
    {
        public bool sound = true;
        public bool music = true;
        public bool fullscreen = false;

        public Keys Up = Keys.Up;
        public Keys Down = Keys.Down;
        public Keys Left = Keys.Left;
        public Keys Right = Keys.Right;
        public Keys A = Keys.X;
        public Keys B = Keys.Z;
        public Keys Start = Keys.Enter;

        public Options Clone()
        {
            Options newOptions = new Options();
            newOptions.sound = sound;
            newOptions.music = music;
            newOptions.fullscreen = fullscreen;
            newOptions.Up = Up;
            newOptions.Down = Down;
            newOptions.Left = Left;
            newOptions.Right = Right;
            newOptions.A = A;
            newOptions.B = B;
            newOptions.Start = Start;
            return newOptions;
        }
    }

    public class Controls
    {
        public bool Up, Down, Left, Right, A, B, Start;

        Dictionary<Key, Keys> definitions = new Dictionary<Key, Keys>();

        public enum Key
        {
            Up,
            Down,
            Left,
            Right,
            A,
            B,
            Start,
        }

        public Controls()
        {

        }

        public void Update()
        {
            KeyboardState state = Keyboard.GetState();

            Up = state.IsKeyDown(definitions[Key.Up]);
            Down = state.IsKeyDown(definitions[Key.Down]);
            Left = state.IsKeyDown(definitions[Key.Left]);
            Right = state.IsKeyDown(definitions[Key.Right]);
            A = state.IsKeyDown(definitions[Key.A]);
            B = state.IsKeyDown(definitions[Key.B]);
            Start = state.IsKeyDown(definitions[Key.Start]);

            // Rudimentary gamepad support, untested
            GamePadState gpState = GamePad.GetState(PlayerIndex.One);
            if (gpState.IsConnected)
            {
                Up = Up || gpState.IsButtonDown(Buttons.DPadUp);
                Down = Down || gpState.IsButtonDown(Buttons.DPadDown);
                Left = Left || gpState.IsButtonDown(Buttons.DPadLeft);
                Right = Right || gpState.IsButtonDown(Buttons.DPadRight);
                A = A || gpState.IsButtonDown(Buttons.A);
                B = B || gpState.IsButtonDown(Buttons.B);
                Start = Start || gpState.IsButtonDown(Buttons.Start);
            }
        }

        public void ChangeDefinition(Options opti)
        {
            definitions = new Dictionary<Key, Keys>();
            definitions.Add(Key.Up, opti.Up);
            definitions.Add(Key.Down, opti.Down);
            definitions.Add(Key.Left, opti.Left);
            definitions.Add(Key.Right, opti.Right);
            definitions.Add(Key.A, opti.A);
            definitions.Add(Key.B, opti.B);
            definitions.Add(Key.Start, opti.Start);
        }
    }
    
}
