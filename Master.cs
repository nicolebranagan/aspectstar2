using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Drawing;
using Microsoft.Xna.Framework.Audio;

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
            controls.Up = Keys.Up;
            controls.Down = Keys.Down;
            controls.Left = Keys.Left;
            controls.Right = Keys.Right;
            controls.A = Keys.X;
            controls.B = Keys.Z;

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
            texCollection.arcadeFont = Content.Load<Texture2D>("arcadefont");
            texCollection.controls = Content.Load<Texture2D>("menu");
            texCollection.worldTiles = Content.Load<Texture2D>("protoworld");
            texCollection.texMapPlayer = Content.Load<Texture2D>("mapplayer");
            texCollection.texAdvPlayer = Content.Load<Texture2D>("advplayer");
            texCollection.texShadows = Content.Load<Texture2D>("shadows");
            texCollection.texEnemies = Content.Load<Texture2D>("enemies");
            texCollection.texProjectile = Content.Load<Texture2D>("projectile");
            texCollection.bigMouse = Content.Load<Texture2D>("bigmouse");
            texCollection.texPlosion = Content.Load<Texture2D>("explosion");

            texCollection.adventureTiles = new Texture2D[2];
            texCollection.adventureTiles[0] = Content.Load<Texture2D>("dungeon1");
            texCollection.adventureTiles[1] = Content.Load<Texture2D>("town");

            // Load sound effects (made in sfxr)
            PlaySound.die = Content.Load<SoundEffect>("die");
            PlaySound.aspect = Content.Load<SoundEffect>("aspect");
            PlaySound.jump = Content.Load<SoundEffect>("jump");
            PlaySound.enter = Content.Load<SoundEffect>("enter");
            PlaySound.hurt = Content.Load<SoundEffect>("hurt");
            PlaySound.drown = Content.Load<SoundEffect>("drown");
            PlaySound.key = Content.Load<SoundEffect>("key"); 
            PlaySound.pew = Content.Load<SoundEffect>("pew");
            PlaySound.boom = Content.Load<SoundEffect>("boom");
            PlaySound.leave = Content.Load<SoundEffect>("leave");
            PlaySound.Initialize();
                
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
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
            currentScreen = currentGame.Begin();
        }

        public void UpdateScreen(Screen screen)
        {
            this.currentScreen = screen;
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

        public Texture2D controls;
        public Texture2D arcadeFont;
        public Texture2D blank;

        public Texture2D worldTiles;
        public Texture2D[] adventureTiles;

        public Texture2D texMapPlayer;
        public Texture2D texAdvPlayer;
        public Texture2D texEnemies;
        public Texture2D texProjectile;

        public Texture2D bigMouse;

        public Texture2D texShadows;
        public Texture2D texPlosion;
    }

    public struct Controls
    {
        public Keys Up, Down, Left, Right, A, B;
    }
    
}
