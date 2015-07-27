using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Xml.Serialization;

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

            // Load in-game content
            texCollection.arcadeFont = Content.Load<Texture2D>("arcadefont");
            texCollection.worldTiles = Content.Load<Texture2D>("protoworld");
            texCollection.texPlayer = Content.Load<Texture2D>("nadine_3col");

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

        public static Vector2 getMapTile(int index)
        {
            int width = texCollection.worldTiles.Width / 32;
            int height = texCollection.worldTiles.Height / 32;

            return new Vector2(index % width * 32, index / width * 32);
        }
    }

    public struct Textures
    {
        // Game textures

        public Texture2D arcadeFont;

        public Texture2D worldTiles;

        public Texture2D texPlayer;

    }

    public struct Controls
    {
        public Keys Up, Down, Left, Right;
    }
}
