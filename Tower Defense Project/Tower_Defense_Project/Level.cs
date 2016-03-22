using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Duality;
using Duality.Encrypting;
using Duality.Interaction;
using Duality.Records;
using SharpDX.Serialization;
using System.Reflection;

namespace Tower_Defense_Project
{
    class Level
    {
        public Button temp1, start;
        private bool keyPressed, keyDidSomething, pause = false, waveRunning = false;
        private float escapeTimer = 0, minEscapeTimer = .05f;
        private int pointsNum, pathSetNum, waveNum;
        private Path path;
        public RectangleF storeSection;
        private SpriteFont font;
        private StreamWriter sw;
        private StreamReader tempFile, read;
        private Texture2D tex, background, tempButton1, tempButton2, startWave, startWavePressed;
        private uint currency;
        private WaveManager waves;

        public List<Enemy> enemies = new List<Enemy>();
        public List<Tower> towers = new List<Tower>();
        public List<Projectile> projectiles = new List<Projectile>();

        public uint Currency
        {
            get { return currency; }
        }

        public Path Path
        {
            get { return path; }
        }

        public SpriteFont Font
        {
            get { return font; }
        }

        public Level()
        {

        }

        private void LoadContent(int levelIndex)
        {
            background = Main.GameContent.Load<Texture2D>(@"Levels/Level" + levelIndex);
            tempButton1 = Main.GameContent.Load<Texture2D>(@"Buttons/Temp Button 1");
            tempButton2 = Main.GameContent.Load<Texture2D>(@"Buttons/Temp Button 2");
            startWave = Main.GameContent.Load<Texture2D>(@"Buttons/Start Wave");
            startWavePressed = Main.GameContent.Load<Texture2D>(@"Buttons/Start Wave Pressed");
            tex = Main.GameContent.Load<Texture2D>(@"Textures/SQUARE");
            font = Main.GameContent.Load<SpriteFont>(@"Fonts/Font");

            storeSection = new RectangleF(.75f * Main.Graphics.PreferredBackBufferWidth, 0f * Main.Graphics.PreferredBackBufferHeight, (.25f * Main.Graphics.PreferredBackBufferWidth) + 1, (Main.Graphics.PreferredBackBufferHeight) + 1);
            waves = new WaveManager(this);
        }

        public void LoadLevel(int levelIndex)
        {
            LoadContent(levelIndex);

            waves.LoadEnemies(string.Format("Level{0}", levelIndex));
            waves.WaveFinished += WaveEnd;

            tempFile = new StreamReader(@"Content/Levels/Level" + levelIndex + ".path");
            sw = new StreamWriter("temp2.temp");
            sw.Write(StringCipher.Decrypt(tempFile.ReadLine(), "temp2"));
            sw.Close();
            read = new StreamReader("temp2.temp");
            pointsNum = int.Parse(read.ReadLine());
            pathSetNum = int.Parse(read.ReadLine());
            path = new Path();
            for (int i = 0; i < pointsNum; i++)
            {
                path.points.Add(new Vector2(float.Parse(read.ReadLine()), float.Parse(read.ReadLine())));
            }
            for (int i = 0; i < pathSetNum; i++)
            {
                path.pathSet.Add(new RectangleF(float.Parse(read.ReadLine()), float.Parse(read.ReadLine()), float.Parse(read.ReadLine()), float.Parse(read.ReadLine())));
            }
            read.Close();
            File.Delete("temp2.temp");
            path.Build(true);
            currency = 1000;

            temp1 = new Button(new Vector2(610, 10), 180, 80, 1, Main.CurrentMouse, tempButton1, tempButton2, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
            temp1.ButtonPressed += ButtonHandling;

            start = new Button(new Vector2(610, 380), 180, 90, 2, Main.CurrentMouse, startWave, startWavePressed, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
            start.ButtonPressed += ButtonHandling;
        }

        public void Update(GameTime gameTime)
        {
            Main.CurrentMouse = Main.Mouse.GetState();

            escapeTimer = Main.CurrentKeyboard.IsKeyPressed(Keys.Escape) ? escapeTimer + (float)gameTime.ElapsedGameTime.TotalSeconds : escapeTimer;

            if (Main.CurrentKeyboard.IsKeyReleased(Keys.Escape) && escapeTimer > minEscapeTimer)
            {
                escapeTimer = 0;
                pause = !pause;
            }

            if (!pause)
            {
                temp1.Update(Main.CurrentMouse);
                
                if (waveRunning)
                {
                    waves.UpdateWave(gameTime);
                }
                else
                {
                    start.Update(Main.CurrentMouse);
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

        private void ButtonHandling(object sender, EventArgs e)
        {
            switch (((Button)sender).ButtonNum)
            {
                case 1:
                    if (Currency >= 500)
                    {
                        towers.Add(new Tower(this, TowerType.GL, Main.CurrentMouse));
                        currency -= towers[towers.Count - 1].Cost;
                    }
                    break;

                case 2:
                    waves.WaveFinished += WaveEnd;
                    start.ButtonPressed -= ButtonHandling;
                    waveRunning = true;
                    break;

                default:
                    break;
            }
        }

        private void WaveEnd(object sender, EventArgs e)
        {
            waves.WaveFinished -= WaveEnd;
            start.ButtonPressed += ButtonHandling;
            waveRunning = false;
        }

        private void Input()
        {
            if (towers.Count > 0 && towers[towers.Count - 1].isPlaced)
            {
                keyPressed = false;
            }

            keyDidSomething = keyPressed && keyDidSomething;

            if (Main.CurrentKeyboard.IsKeyPressed(Keys.D1) && Currency >= 500)
            {
                keyPressed = true;
                if (!keyDidSomething)
                {
                    towers.Add(new Tower(this, TowerType.GL, Main.CurrentMouse));
                    currency -= towers[towers.Count - 1].Cost;
                    keyDidSomething = true;
                }
            }
            else if (Main.CurrentKeyboard.IsKeyPressed(Keys.D2))
            {
                keyPressed = true;
                if (!keyDidSomething)
                {
                    towers.Add(new Tower(this, TowerType.RL, Main.CurrentMouse));
                    keyDidSomething = true;
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            spritebatch.Draw(background, new RectangleF(0, 0, Main.Graphics.GraphicsDevice.Viewport.Width, Main.Graphics.GraphicsDevice.Viewport.Height), Color.White);

            spritebatch.Draw(tex, storeSection, Color.Black);

            #region ButtonDrawing
            try
            {
                spritebatch.Draw(temp1.Texture, temp1.Collision, Color.White);
                if (!waveRunning) spritebatch.Draw(start.Texture, start.Collision, Color.White);
            }
            catch
            { }
            #endregion

            foreach (RectangleF i in Path.pathSet)
            {
                spritebatch.Draw(tex, i, Color.Green);
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