using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Duality;
using Duality.Encrypting;
using Duality.Interaction;
using Duality.Records;

namespace Tower_Defense_Project
{
    class Level
    {
        public Button temp1;
        private bool keyPressed, keyDidSomething, pause = false;
        private float timer = 0, minTimer = 1f, escapeTimer = 0, minEscapeTimer = .05f;
        public FloatingRectangle storeSection;
        private Texture2D tex, background, tempButton1, tempButton2;
        private StreamReader reader;

        public List<Enemy> enemies = new List<Enemy>();
        public List<Tower> towers = new List<Tower>();
        public List<Projectile> projectiles = new List<Projectile>();

        public uint Currency
        {
            get { return currency; }
        }
        uint currency;

        public Path Path
        {
            get { return path; }
        }
        Path path;

        public SpriteFont Font
        {
            get { return font; }
        }
        SpriteFont font;

        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        public static GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }
        private static GraphicsDeviceManager graphics;

        private void LoadContent(int levelIndex)
        {
            background = Content.Load<Texture2D>(@"Levels/Level" + levelIndex);
            tempButton1 = Content.Load<Texture2D>(@"Buttons/Temp Button 1");
            tempButton2 = Content.Load<Texture2D>(@"Buttons/Temp Button 2");
            tex = Content.Load<Texture2D>(@"Textures/SQUARE");
            font = Content.Load<SpriteFont>(@"Fonts/Font");
        }

        public void LoadLevel(int levelIndex)
        {
            reader = new StreamReader(@"Content/Levels/Level" + levelIndex + ".path");
            path = Serialization.DeserializeFromString<Path>(StringCipher.Decrypt(reader.ReadLine(), "temp2"));
            path.Build(true);
            currency = 1000;

            LoadContent(levelIndex);
        }

        public Level(IServiceProvider serviceProvider, GraphicsDeviceManager graphics)
        {
            content = new ContentManager(serviceProvider, "Content");
            Level.graphics = graphics;
            storeSection = new FloatingRectangle(.75f * Graphics.PreferredBackBufferWidth, 0f * Graphics.PreferredBackBufferHeight, (.25f * Graphics.PreferredBackBufferWidth) + 1, (Graphics.PreferredBackBufferHeight) + 1);
        }

        public void Update(GameTime gameTime)
        {
            Main.CurrentMouse = Mouse.GetState();

            escapeTimer = Keyboard.GetState().IsKeyDown(Keys.Escape) ? escapeTimer + (float)gameTime.ElapsedGameTime.TotalSeconds : escapeTimer;

            if (Keyboard.GetState().IsKeyUp(Keys.Escape) && escapeTimer > minEscapeTimer)
            {
                escapeTimer = 0;
                pause = !pause;
            }

            if (!pause)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                temp1 = new Button(new Vector2(610, 10), 180, 80, 1, Main.CurrentMouse, tempButton1, tempButton2);

                if (timer > minTimer)
                {
                    enemies.Add(new Enemy(this, (EnemyType)101));
                    timer = 0;
                }

                if (temp1.getButtonState() && Currency >= 500)
                {
                    keyPressed = true;
                    if (!keyDidSomething)
                    {
                        towers.Add(new Tower(this, TowerType.Small, Main.CurrentMouse));
                        currency -= towers[towers.Count - 1].Cost;
                        keyDidSomething = true;
                    }
                }

                try
                {
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.Update(gameTime);

                        if (enemy.position == path.points[path.points.Count - 1])
                        {
                            enemies.Remove(enemy);
                        }
                        else if (enemy.Health <= 0)
                        {
                            currency += enemy.Worth;
                            enemies.Remove(enemy);
                        }
                    }

                    foreach (Tower tower in towers)
                    {
                        tower.Update(gameTime, Main.CurrentMouse);
                    }

                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        projectiles[i].Update(gameTime);

                        if (projectiles[i].StageIndex == 1)
                        {
                            projectiles[i].target.Health -= projectiles[i].damage;
                            projectiles.Remove(projectiles[i]);
                        }
                        else if (!projectiles[i].Origin.range.Contains(projectiles[i].Position))
                        {
                            projectiles.Remove(projectiles[i]);
                        }
                    }

                }
                catch
                {
                    //ErrorHandler.RecordError(3, 100, "*shrugs*", ex.Message);
                }

                Input();

                /*try
                {
                    Console.WriteLine(enemies[0].stagePos);
                }
                catch
                { }*/
            }
        }

        private void Input()
        {
            if (towers.Count > 0 && towers[towers.Count - 1].isPlaced)
            {
                keyPressed = false;
            }

            keyDidSomething = keyPressed && keyDidSomething;

            if (Keyboard.GetState().IsKeyDown(Keys.D1) && Currency >= 500)
            {
                keyPressed = true;
                if (!keyDidSomething)
                {
                    towers.Add(new Tower(this, TowerType.Small, Main.CurrentMouse));
                    currency -= towers[towers.Count - 1].Cost;
                    keyDidSomething = true;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                keyPressed = true;
                if (!keyDidSomething)
                {
                    towers.Add(new Tower(this, TowerType.Medium, Main.CurrentMouse));
                    keyDidSomething = true;
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            spritebatch.Draw(background, new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight), Color.White);

            spritebatch.Draw(tex, storeSection.Draw, Color.Black);

            #region ButtonDrawing
            try
            {
                spritebatch.Draw(temp1.getTexture(), temp1.Collision, Color.White);
            }
            catch
            { } 
            #endregion

            foreach (FloatingRectangle i in Path.pathSet)
            {
                spritebatch.Draw(tex, i.Draw, Color.Green);
            }

            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(gameTime, spritebatch);
            }

            foreach (Tower tower in towers)
            {
                tower.Draw(spritebatch);
            }

            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spritebatch);
            }

            spritebatch.DrawString(Font, Currency.ToString(), Vector2.Zero, Color.Black);
        }
    }
}