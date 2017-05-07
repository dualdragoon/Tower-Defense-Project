using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Duality;
using Duality.Encrypting;
using Duality.Interaction;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;

namespace Tower_Defense_Project
{
	public class Level
	{
		public Button start;
		private bool pause = false, waveRunning = false;
		private float escapeTimer = 0, minEscapeTimer = .05f;
		private int pointsNum, waveNum, health;
		private Main baseMain;
		private Path path;
		public RectangleF storeSection;
		private SpriteFont font;
		private StreamWriter sw;
		private StreamReader tempFile, read;
		private Texture2D tex, /*background,*/ towerBackground, hoveredBackground, startWave, startWavePressed;
		private uint currency;
		private WaveManager waves;
		XmlDocument doc;
		XmlNode node;

		private List<Texture2D> sprites = new List<Texture2D>();
		private List<Button> buttons = new List<Button>();
		internal List<Enemy> enemies = new List<Enemy>();
		internal List<Tower> towers = new List<Tower>();
		public List<dynamic> projectiles = new List<dynamic>();
		internal Dictionary<int, dynamic> projectileTypes = new Dictionary<int, dynamic>();

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

		public int Health
		{
			get { return health; }
			set { health = value; }
		}

		internal Path Path
		{
			get { return path; }
		}

		public SpriteFont Font
		{
			get { return font; }
		}

		public Level(Main main)
		{
			baseMain = main;

			ScriptEngine engine = Python.CreateEngine();
			ScriptSource source = engine.CreateScriptSourceFromFile("Content/Projectiles/small.py");
			ScriptScope scope = engine.CreateScope();
			scope.Engine.Runtime.LoadAssembly(typeof(Program).Assembly);
			try
			{
				source.Execute(scope);
			}
			catch (Exception e)
			{
				ErrorHandler.RecordError(2, 102, "Shrug", e.Message);
			}

			projectileTypes.Add(scope.GetVariable("idNum"), scope.GetVariable("Projectile"));

			source = engine.CreateScriptSourceFromFile("Content/Projectiles/medium.py");
			scope = engine.CreateScope();
			scope.Engine.Runtime.LoadAssembly(typeof(Program).Assembly);

			try
			{
				source.Execute(scope);
			}
			catch (Exception e)
			{
				ErrorHandler.RecordError(2, 102, "Shrug", e.Message);
			}

			projectileTypes.Add(scope.GetVariable("idNum"), scope.GetVariable("Projectile"));
		}

		private void LoadContent(string levelName)
		{
			doc = new XmlDocument();
			doc.Load(@"Content/Towers/Stats/Tower Data.twd");
			node = doc.SelectSingleNode("/Towers");

			for (int i = 0; i < node.ChildNodes.Count; i++)
			{
				if (node.ChildNodes[i].Name == "#comment") continue;
				string[] stats = new string[6];
				for (int t = 0; t < node.ChildNodes[i].ChildNodes.Count; t++)
				{
					stats[t] = node.ChildNodes[i].ChildNodes[t].InnerText;
				}
				towerStats.Add(101 + towerStats.Count, stats);
			}

			doc.Load(@"Content/Enemies/Stats/Enemy Data.emd");
			node = doc.SelectSingleNode("/Enemies");

			for (int i = 0; i < node.ChildNodes.Count; i++)
			{
				if (node.ChildNodes[i].Name == "#comment") continue;
				string[] stats = new string[7];
				for (int t = 0; t < node.ChildNodes[i].ChildNodes.Count; t++)
				{
					stats[t] = node.ChildNodes[i].ChildNodes[t].InnerText;
				}
				enemyStats.Add(101 + enemyStats.Count, stats);
			}

			//background = Main.GameContent.Load<Texture2D>(@"Levels/" + levelName);
			towerBackground = Main.GameContent.Load<Texture2D>("Buttons/Button Background");
			hoveredBackground = Main.GameContent.Load<Texture2D>("Buttons/Hovered Background");
			startWave = Main.GameContent.Load<Texture2D>(@"Buttons/Start Wave");
			startWavePressed = Main.GameContent.Load<Texture2D>(@"Buttons/Start Wave Pressed");
			tex = Main.GameContent.Load<Texture2D>(@"Textures/SQUARE");
			font = Main.GameContent.Load<SpriteFont>(@"Fonts/Font");

			storeSection = new RectangleF(.85f * Main.Scale.X, 0f * Main.Scale.Y, (.15f * Main.Scale.X) + 1, (Main.Scale.Y) + 1);
			waves = new WaveManager(this);
		}

		public void LoadLevel(string levelName)
		{
			LoadContent(levelName);

			waves.LoadEnemies(levelName);
			waves.WaveFinished += WaveEnd;
			waves.LevelComplete += LevelEnd;

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
			Currency = 1000;

			try
			{
				for (int i = 0; i < TowerStats.Keys.Count; i++)
				{
					int[] keys = new int[TowerStats.Keys.Count];
					TowerStats.Keys.CopyTo(keys, 0);
					float x = (i - 1 % 2 == 0) ? 1264 : 1174,
						y = (buttons.Count <= 1) ? 112 : buttons[buttons.Count - 2].Collision.Bottom + 12;

					sprites.Add(Main.GameContent.Load<Texture2D>("Towers/" + TowerStats[keys[i]][0]));

					buttons.Add(new Button(new Vector2(x, y), (40f / 683f) * Main.Scale.X, (5f / 48f) * Main.Scale.Y, buttons.Count, Main.CurrentMouse, towerBackground, hoveredBackground, true, Main.Scale.X, Main.Scale.Y));
					int l = i;
					buttons[buttons.Count - 1].LeftClicked += (object sender, EventArgs e) =>
					{
						bool previousPlaced = (towers.Count > 0) ? towers[towers.Count - 1].isPlaced : true;
						if (Currency >= uint.Parse(TowerStats[keys[l]][5]) && previousPlaced)
						{
							towers.Add(new Tower(this, (TowerType)keys[l], Main.CurrentMouse));
							Currency -= towers[towers.Count - 1].Cost;
						}
					};
				}
			}
			catch (Exception e)
			{
				ErrorHandler.RecordError(3, 104, e.Message, e.StackTrace);
			}

			start = new Button(new Vector2(.866f * Main.Scale.X, (650f / 768f) * Main.Scale.Y), (160f / 1366f) * Main.Scale.X, (90f / 768f) * Main.Scale.Y, 2, Main.CurrentMouse, startWave, startWavePressed, true, Main.Scale.X, Main.Scale.Y);
			start.LeftClicked += ButtonHandling;
		}

		public void Clear()
		{
			enemies.Clear();
			towers.Clear();
			projectiles.Clear();
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
				foreach (Button button in buttons)
				{
					button.Update(Main.CurrentMouse);
				}

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
							Health -= enemy.Damage;
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
			}
		}

		private void ButtonHandling(object sender, EventArgs e)
		{
			waves.WaveFinished += WaveEnd;
			start.LeftClicked -= ButtonHandling;
			waveRunning = true;
		}

		private void WaveEnd(object sender, EventArgs e)
		{
			waves.WaveFinished -= WaveEnd;
			start.LeftClicked += ButtonHandling;
			waveRunning = false;
		}

		private void LevelEnd(object sender, EventArgs e)
		{
			waves.LevelComplete -= LevelEnd;
			baseMain.CurrentState = GameState.Menu;
		}

		private void Input()
		{
			bool previousPlaced = (towers.Count > 0) ? towers[towers.Count - 1].isPlaced : true;

			if (Main.CurrentKeyboard.IsKeyPressed(Keys.D1) && Currency >= 500 && previousPlaced)
			{
				towers.Add(new Tower(this, TowerType.GL, Main.CurrentMouse));
				Currency -= towers[towers.Count - 1].Cost;
			}
			else if (Main.CurrentKeyboard.IsKeyPressed(Keys.D2) && Currency >= 1000 && previousPlaced)
			{
				towers.Add(new Tower(this, TowerType.RL, Main.CurrentMouse));
				Currency -= towers[towers.Count - 1].Cost;
			}
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			//spriteBatch.Draw(background, new RectangleF(0, 0, Main.Scale.X, Main.Scale.Y), Color.White);

			spriteBatch.Draw(tex, storeSection, Color.Black);

			#region ButtonDrawing
			try
			{
				for (int i = 0; i < buttons.Count; i++)
				{
					spriteBatch.Draw(buttons[i].Texture, buttons[i].Collision, Color.White);
					spriteBatch.Draw(sprites[i], buttons[i].Collision, Color.White);
				}
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

			foreach (dynamic projectile in projectiles)
			{
				projectile.Draw(spriteBatch);
			}

			spriteBatch.DrawString(Font, Currency.ToString(), Vector2.Zero, Color.Black);
		}

		public void DrawProjectile(SpriteBatch spriteBatch, Texture2D texture, Vector2 pos, Color color)
		{
			spriteBatch.Draw(texture, pos, color);
		}
	}
}