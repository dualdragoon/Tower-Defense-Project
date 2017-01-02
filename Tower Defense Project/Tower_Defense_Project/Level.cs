using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Duality.Encrypting;
using Duality.Interaction;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;

namespace Tower_Defense_Project
{
    public class Level
    {
        public Button temp1, start;
        private bool pause = false, waveRunning = false;
        private float escapeTimer = 0, minEscapeTimer = .05f;
        private int pointsNum, waveNum;
        private Path path;
        public RectangleF storeSection;
        private SpriteFont font;
        private StreamWriter sw;
        private StreamReader tempFile, read;
        private Texture2D tex, background, tempButton1, tempButton2, startWave, startWavePressed;
        private uint currency;
        private WaveManager waves;
        XmlDocument doc;
        XmlNode node;

        internal List<Enemy> enemies = new List<Enemy>();
        internal List<Tower> towers = new List<Tower>();
        internal List<Projectile> projectiles = new List<Projectile>();

        private static Dictionary<int, string[]> towerStats = new Dictionary<int, string[]>();
        private static Dictionary<int, string[]> enemyStats = new Dictionary<int, string[]>();

        public static Dictionary<int, string[]> TowerStats
        {
            get { return towerStats; }
            set { towerStats = value; }
        }

        public static Dictionary<int, string[]> EnemyStats
        {
            get { return enemyStats; }
            set { enemyStats = value; }
        }

        public uint Currency
        {
            get { return currency; }
            set { currency = value; }
        }

        internal Path Path
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

        private void LoadContent(string levelName)
        {
            doc = new XmlDocument();
            doc.Load(@"Content/Towers/Stats/Tower Data.twd");
            node = doc.SelectSingleNode("/Towers");

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                string[] stats = new string[6];
                for (int t = 0; t < node.ChildNodes[i].ChildNodes.Count; t++)
                {
                    stats[t] = node.ChildNodes[i].ChildNodes[t].InnerText;
                }
                towerStats.Add(101 + i, stats);
            }

            doc.Load(@"Content/Enemies/Stats/Enemy Data.emd");
            node = doc.SelectSingleNode("/Enemies");

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                string[] stats = new string[6];
                for (int t = 0; t < node.ChildNodes[i].ChildNodes.Count; t++)
                {
                    stats[t] = node.ChildNodes[i].ChildNodes[t].InnerText;
                }
                enemyStats.Add(101 + i, stats);
            }

            background = Main.GameContent.Load<Texture2D>(@"Levels/" + levelName);
            tempButton1 = Main.GameContent.Load<Texture2D>(@"Buttons/Temp Button 1");
            tempButton2 = Main.GameContent.Load<Texture2D>(@"Buttons/Temp Button 2");
            startWave = Main.GameContent.Load<Texture2D>(@"Buttons/Start Wave");
            startWavePressed = Main.GameContent.Load<Texture2D>(@"Buttons/Start Wave Pressed");
            tex = Main.GameContent.Load<Texture2D>(@"Textures/SQUARE");
            font = Main.GameContent.Load<SpriteFont>(@"Fonts/Font");

            storeSection = new RectangleF(.75f * Main.Scale.X, 0f * Main.Scale.Y, (.25f * Main.Scale.X) + 1, (Main.Scale.Y) + 1);
			waves = new WaveManager(this);
        }

        public void LoadLevel(string levelName)
        {
            LoadContent(levelName);

            waves.LoadEnemies(levelName);
            waves.WaveFinished += WaveEnd;

            tempFile = new StreamReader(@"Content/Levels/" + levelName + ".path");
            sw = new StreamWriter("temp2.temp");
            sw.Write(StringCipher.Decrypt(tempFile.ReadLine(), "temp2"));
            sw.Close();
            read = new StreamReader("temp2.temp");
            pointsNum = int.Parse(read.ReadLine());
            path = new Path();
            for (int i = 0; i < pointsNum; i++)
            {
                path.AddCurve((new Vector2(float.Parse(read.ReadLine()) * Main.Scale.X, float.Parse(read.ReadLine()) * Main.Scale.Y)),
                    (new Vector2(float.Parse(read.ReadLine()) * Main.Scale.X, float.Parse(read.ReadLine()) * Main.Scale.Y)),
                    (new Vector2(float.Parse(read.ReadLine()) * Main.Scale.X, float.Parse(read.ReadLine()) * Main.Scale.Y)),
                    (new Vector2(float.Parse(read.ReadLine()) * Main.Scale.X, float.Parse(read.ReadLine()) * Main.Scale.Y)));
            }
            read.Close();
            File.Delete("temp2.temp");
            path.Build();
            currency = 1000;

            temp1 = new Button(new Vector2(.7625f * Main.Scale.X, (10f / 480f) * Main.Scale.Y), (int)(.225f * Main.Scale.X), (int)(.1875f * Main.Scale.Y), 1, Main.CurrentMouse, tempButton1, tempButton2, true, Main.Scale.X, Main.Scale.Y);
            temp1.LeftClicked += ButtonHandling;

            start = new Button(new Vector2(.7625f * Main.Scale.X, (380f / 480f) * Main.Scale.Y), (int)(.225f * Main.Scale.X), (int)(.1875f * Main.Scale.Y), 2, Main.CurrentMouse, startWave, startWavePressed, true, Main.Scale.X, Main.Scale.Y);
			start.LeftClicked += ButtonHandling;

			ScriptEngine engine = Python.CreateEngine();
			ScriptSource source = engine.CreateScriptSourceFromFile("Content/Projectiles/Small.py");
			ScriptScope scope = engine.CreateScope();
			scope.Engine.Runtime.LoadAssembly(typeof(Program).Assembly);
			source.Execute(scope);

			dynamic Test2 = scope.GetVariable("Projectile");
			dynamic test = Test2(new Tower(this, TowerType.GL, Main.CurrentMouse), Vector2.Zero, new Enemy(this, EnemyType.Peon), ProjectileType.Small, this);
			//test2.go();
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

                        if (enemy.position == path.Points[path.Points.Count - 1])
                        {
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
                    start.LeftClicked -= ButtonHandling;
                    waveRunning = true;
                    break;

                default:
                    break;
            }
        }

        private void WaveEnd(object sender, EventArgs e)
        {
            waves.WaveFinished -= WaveEnd;
            start.LeftClicked += ButtonHandling;
            waveRunning = false;
        }

        private void Input()
        {
            bool temp = (towers.Count > 0);

            if (!temp)
            {
                if (Main.CurrentKeyboard.IsKeyPressed(Keys.D1) && Currency >= 500)
                {
                    towers.Add(new Tower(this, TowerType.GL, Main.CurrentMouse));
                    currency -= towers[towers.Count - 1].Cost;
                }
                else if (Main.CurrentKeyboard.IsKeyPressed(Keys.D2))
                {
                    towers.Add(new Tower(this, TowerType.RL, Main.CurrentMouse));
                }
            }
            else
            {
                if (Main.CurrentKeyboard.IsKeyPressed(Keys.D1) && Currency >= 500 && towers[towers.Count - 1].isPlaced)
                {
                    towers.Add(new Tower(this, TowerType.GL, Main.CurrentMouse));
                    currency -= towers[towers.Count - 1].Cost;
                }
                else if (Main.CurrentKeyboard.IsKeyPressed(Keys.D2) && towers[towers.Count - 1].isPlaced)
                {
                    towers.Add(new Tower(this, TowerType.RL, Main.CurrentMouse));
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new RectangleF(0, 0, Main.Scale.X, Main.Scale.Y), Color.White);

            spriteBatch.Draw(tex, storeSection, Color.Black);

            #region ButtonDrawing
            try
            {
                spriteBatch.Draw(temp1.Texture, temp1.Collision, Color.White);
                if (!waveRunning) spriteBatch.Draw(start.Texture, start.Collision, Color.White);
            }
            catch
            { }
            #endregion

            path.Draw(spriteBatch);

            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(gameTime, spriteBatch);
            }

            foreach (Tower tower in towers)
            {
                tower.Draw(spriteBatch);
            }

            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }

            spriteBatch.DrawString(Font, Currency.ToString(), Vector2.Zero, Color.Black);
        }
    }
}