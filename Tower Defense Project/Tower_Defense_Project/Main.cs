using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Duality;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;

namespace Tower_Defense_Project
{
    public enum CustomCursor { BLL_Cursor, BL_Cursor, GL_Cursor, IT_Cursor, OL_Cursor, RL_Cursor, SS_Cursor, YL_Cursor }

    public enum GameState { Menu, Play, LevelDesigner }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Game
    {
        Designer designer;
        GameState currentState;
        int offset;
        Level level;
        List<Texture2D> bloodDrops = new List<Texture2D>();
        List<Texture2D> bluePulse = new List<Texture2D>();
        Menu menu;
        ParticleEngine generator = new ParticleEngine();
        SpriteBatch spriteBatch;
        string levelName = "Level1";
        Texture2D[] cursors = new Texture2D[8];
        static bool isCustomMouseVisible = true;
        static ContentManager content;
        static CustomCursor cursorType = CustomCursor.GL_Cursor;
        static GraphicsDeviceManager graphics;
        static KeyboardManager keyboardManager;
        static KeyboardState keyboard;
        static MouseManager mouseManager;
        static MouseState mouse;
        static Texture2D currentCursor, emptyCursor, selectedCursor;

        #region Properties
        public static bool IsCustomMouseVisible
        {
            get { return isCustomMouseVisible; }
            set { isCustomMouseVisible = value; }
        }

        public static ContentManager GameContent
        {
            get { return content; }
        }

        public static CustomCursor CursorType
        {
            get { return cursorType; }
            set { cursorType = value; }
        }

        public static GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        public static KeyboardManager Keyboard
        {
            get { return keyboardManager; }
        }

        public static KeyboardState CurrentKeyboard
        {
            get { return keyboard; }
        }

        public static MouseManager Mouse
        {
            get { return mouseManager; }
        }

        public static MouseState CurrentMouse
        {
            get { return mouse; }
            set { mouse = value; }
        }

        private static Texture2D SelectedCursor
        {
            get { return selectedCursor; }
            set { selectedCursor = value; }
        }

        /// <summary>
        /// Vector containing screen sizes for scaling.
        /// </summary>
        public static Vector2 Scale
        {
            get { return new Vector2(Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight); }
        }

        public GameState CurrentState
        {
            get { return currentState; }
            set 
            { 
                currentState = value;
                StateChanged();
            }
        }

        public string LevelName
        {
            get { return levelName; }
            set { levelName = value; }
        }
        #endregion

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            mouseManager = new MouseManager(this);
            keyboardManager = new KeyboardManager(this);
            content = Content;
            //graphics.PreferredBackBufferHeight = 768;
            //graphics.PreferredBackBufferWidth = 1366;
            //graphics.PreferredBackBufferHeight = 900;
            //graphics.PreferredBackBufferWidth = 1440;
            /*graphics.IsFullScreen = true;
            IsMouseVisible = true;*/
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ErrorHandler.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            emptyCursor = Content.Load<Texture2D>("Textures/help");

            cursors[0] = Content.Load<Texture2D>("Textures/Cursors/BLL Cursor");
            cursors[1] = Content.Load<Texture2D>("Textures/Cursors/BL Cursor");
            cursors[2] = Content.Load<Texture2D>("Textures/Cursors/GL Cursor");
            cursors[3] = Content.Load<Texture2D>("Textures/Cursors/IT Cursor");
            cursors[4] = Content.Load<Texture2D>("Textures/Cursors/OL Cursor");
            cursors[5] = Content.Load<Texture2D>("Textures/Cursors/RL Cursor");
            cursors[6] = Content.Load<Texture2D>("Textures/Cursors/SS Cursor");
            cursors[7] = Content.Load<Texture2D>("Textures/Cursors/YL Cursor");

            bloodDrops.Add(Content.Load<Texture2D>("Textures/Cursors/Particles/Drop1"));
            bloodDrops.Add(Content.Load<Texture2D>("Textures/Cursors/Particles/Drop2"));

            bluePulse.Add(Content.Load<Texture2D>("Textures/Cursors/Particles/Pulse"));

			CurrentState = GameState.Menu;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            mouse = Mouse.GetState();
            keyboard = Keyboard.GetState();
            if (IsCustomMouseVisible)
            {
                currentCursor = cursors[(int)CursorType];
                if (currentCursor == cursors[1])
                {
                    generator.EmitterLocation = new Vector2(CurrentMouse.X, CurrentMouse.Y);
                    generator.Update();
                    offset = 2;
                }
                else if (currentCursor == cursors[5])
                {
                    generator.EmitterLocation = new Vector2(CurrentMouse.X, CurrentMouse.Y);
                    generator.Update();
                    offset = 1;
                }
                else
                {
                    generator.ClearParticles();
                    offset = 1;
                }
            }
            else
            {
                currentCursor = emptyCursor;
            }

            Input();

            switch (CurrentState)
            {
                case GameState.Menu:
                    menu.Update(gameTime);
                    break;

                case GameState.Play:
                    level.Update(gameTime);
                    break;

                case GameState.LevelDesigner:
                    designer.Update(gameTime);
                    break;

                default:
                    break;
            }

            base.Update(gameTime);
        }

        private void Input()
        {
            if (keyboard.IsKeyPressed(Keys.E))
            {
                if ((int)CursorType + 1 > 7) CursorType = (CustomCursor)0;
                else CursorType = (CustomCursor)((int)CursorType + 1);
            }
            else if (keyboard.IsKeyPressed(Keys.Q))
            {
                if ((int)CursorType - 1 < 0) CursorType = (CustomCursor)7;
                else CursorType = (CustomCursor)((int)CursorType - 1);
            }

            if (keyboard.IsKeyPressed(Keys.E) || keyboard.IsKeyPressed(Keys.Q))
            {
                if (CursorType == (CustomCursor)5) generator = new ParticleEngine(bloodDrops, new Vector2(-10, -10), EngineType.Dripping);
                else if (CursorType == (CustomCursor)1) generator = new ParticleEngine(bluePulse, new Vector2(-10, -10), EngineType.Pulsing); 
            }
        }

        private void StateChanged()
        {
            switch(currentState)
            {
                case GameState.Play:
					if (level == null) level = new Level();
					else level.Clear();
                    level.LoadLevel(LevelName);
                    break;

                case GameState.Menu:
                    if (menu == null) { menu = new Menu(this); }
                    else menu.LoadMenu(menu.LastMenu);
                    break;

                case GameState.LevelDesigner:
                    designer = new Designer();
                    designer.LoadContent();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, graphics.GraphicsDevice.BlendStates.NonPremultiplied);

            Window.AllowUserResizing = false;

            switch (CurrentState)
            {
                case GameState.Menu:
                    menu.Draw(gameTime, spriteBatch);
                    break;

                case GameState.Play:
                    level.Draw(gameTime, spriteBatch);
                    break;

                case GameState.LevelDesigner:
                    designer.Draw(gameTime, spriteBatch);
                    break;

                default:
                    break;
            }

            generator.Draw(spriteBatch);
            spriteBatch.Draw(currentCursor, new Vector2((CurrentMouse.X * Scale.X) - offset, (CurrentMouse.Y * Scale.Y) - offset), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}