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
            // TODO: Add your initialization logic here
            // Set default controls
            Master.controls = new Controls();
            /*controls.Up = Keys.Up;
            controls.Down = Keys.Down;
            controls.Left = Keys.Left;
            controls.Right = Keys.Right;
            controls.A = Keys.X;
            controls.B = Keys.Z;
            controls.Start = Keys.Enter;*/

            globalRandom = new Random();

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
            texCollection.texPlosion = Content.Load<Texture2D>("explosion");

            texCollection.adventureTiles = new Texture2D[6];
            texCollection.adventureTiles[0] = Content.Load<Texture2D>("dungeon1");
            texCollection.adventureTiles[1] = Content.Load<Texture2D>("town");
            texCollection.adventureTiles[2] = Content.Load<Texture2D>("dungeon2");
            texCollection.adventureTiles[3] = Content.Load<Texture2D>("dungeon3");
            texCollection.adventureTiles[4] = Content.Load<Texture2D>("dungeon4");
            texCollection.adventureTiles[5] = Content.Load<Texture2D>("dungeon5");

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
            if ((currentScreen == null || currentScreen is TitleScreen) &&
                (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)))
                Exit();

            Master.controls.Update();
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
                UpdateScreen(new TextScreen(currentGame, currentFile.opening, delegate(bool x) { currentGame.Begin(); }));
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

        public bool LoadGame()
        {
            try
            {
                XmlSerializer xS = new XmlSerializer(typeof(SavedGame));
                FileStream fS = new FileStream("saved.xml", FileMode.Open);
                XmlTextReader xTR = new XmlTextReader(fS);
                SavedGame sG;
                sG = (SavedGame)xS.Deserialize(xTR);

                currentGame = new Game(this);
                currentScreen = currentGame.BeginFromSaved(sG);
                fS.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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
    }

    public struct Textures
    {
        // Game textures

        public Texture2D title;
        public Texture2D controls;
        public Texture2D arcadeFont;
        public Texture2D blank;

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

        public Texture2D texShadows;
        public Texture2D texPlosion;
    }

    public class Controls
    {
        // First step towards gamepad support? Though I need a gamepad to actually add that

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
            // Default controls are now here

            definitions.Add(Key.Up, Keys.Up);
            definitions.Add(Key.Down, Keys.Down);
            definitions.Add(Key.Left, Keys.Left);
            definitions.Add(Key.Right, Keys.Right);
            definitions.Add(Key.A, Keys.X);
            definitions.Add(Key.B, Keys.Z);
            definitions.Add(Key.Start, Keys.Enter);
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
        }
    }
    
}
