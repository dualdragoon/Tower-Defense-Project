using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Duality;

namespace Tower_Defense_Project
{
    public enum CustomCursor { BL_Cursor, BLL_Cursor, GL_Cursor, IT_Cursor, OL_Cursor, RL_Cursor, SS_Cursor, YL_Cursor }

    public enum GameState { Menu, Play, LevelDesigner }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Game
    {
        bool keyPressed, keyDidSomething;
        GameState currentState = GameState.LevelDesigner;
        int offset;
        Level level;
        List<Texture2D> textures = new List<Texture2D>();
        ParticleEngine generator;
        SpriteBatch spriteBatch;
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

        public GameState CurrentState
        {
            get { return currentState; }
            set 
            { 
                currentState = value;
                StateChanged();
            }
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
        #endregion

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            mouseManager = new MouseManager(this);
            keyboardManager = new KeyboardManager(this);
            content = Content;
            //graphics.PreferredBackBufferHeight = 480;
            //graphics.PreferredBackBufferWidth = 800;
            /*graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1440;
            graphics.IsFullScreen = true;
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

            emptyCursor = Content.Load<Texture2D>(@"Textures/Null");

            cursors[0] = Content.Load<Texture2D>(@"Textures/Cursors/BLL Cursor");
            cursors[1] = Content.Load<Texture2D>(@"Textures/Cursors/BL Cursor");
            cursors[2] = Content.Load<Texture2D>(@"Textures/Cursors/GL Cursor");
            cursors[3] = Content.Load<Texture2D>(@"Textures/Cursors/IT Cursor");
            cursors[4] = Content.Load<Texture2D>(@"Textures/Cursors/OL Cursor");
            cursors[5] = Content.Load<Texture2D>(@"Textures/Cursors/RL Cursor");
            cursors[6] = Content.Load<Texture2D>(@"Textures/Cursors/SS Cursor");
            cursors[7] = Content.Load<Texture2D>(@"Textures/Cursors/YL Cursor");

            textures.Add(Content.Load<Texture2D>("Textures/Cursors/Particles/Drop1"));
            textures.Add(Content.Load<Texture2D>("Textures/Cursors/Particles/Drop2"));
            generator = new ParticleEngine(textures, new Vector2(-10, -10), EngineType.Dripping);

            CurrentState = GameState.Play;
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
                    offset = 2;
                }
                else if (currentCursor == cursors[5])
                {
                    generator.EmitterLocation = new Vector2(CurrentMouse.X, CurrentMouse.Y);
                    generator.Update();
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
                    break;

                case GameState.Play:
                    level.Update(gameTime);
                    break;

                case GameState.LevelDesigner:
                    break;

                default:
                    break;
            }

            base.Update(gameTime);
        }

        private void Input()
        {
            keyDidSomething = keyPressed && keyDidSomething;

            if (keyboard.IsKeyPressed(Keys.E))
            {
                if ((int)CursorType + 1 > 7)
                {
                    CursorType = (CustomCursor)0;
                }
                else
                {
                    CursorType = (CustomCursor)((int)CursorType + 1);
                }
                keyDidSomething = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Q))
            {
                if ((int)CursorType - 1 < 0)
                {
                    CursorType = (CustomCursor)7;
                }
                else
                {
                    CursorType = (CustomCursor)((int)CursorType - 1);
                }
                keyDidSomething = true;
            }
        }

        private void StateChanged()
        {
            switch(currentState)
            {
                case GameState.Play:
                    level = new Level();
                    level.LoadLevel(1);
                    break;

                case GameState.Menu:
                    break;

                case GameState.LevelDesigner:
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
                    break;

                case GameState.Play:
                    level.Draw(gameTime, spriteBatch);
                    break;

                case GameState.LevelDesigner:
                    break;

                default:
                    break;
            }

            generator.Draw(spriteBatch);
            spriteBatch.Draw(currentCursor, new Vector2((CurrentMouse.X * graphics.GraphicsDevice.Viewport.Width) - offset, (CurrentMouse.Y * graphics.GraphicsDevice.Viewport.Height) - offset), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
