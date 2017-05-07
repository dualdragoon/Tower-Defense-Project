using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Duality.Encrypting;
using Duality.Interaction;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Tower_Defense_Project
{
    public enum DesignerForm { Path, Enemies }

    public class Designer
    {
        public RectangleF storeSection;
        bool pathExists, enemiesExist, healthSelected, nameSelected, anythingSelected;
        string health, name;
        Button build, previous, next;
        Color colorHealth, colorName;
        DesignerForm form = DesignerForm.Path;
        DesignPath path = new DesignPath();
        RectangleF healthRect, nameRect;
        SpriteFont font;
        Texture2D tex, buildUnpressed, buildPressed, previousNormal, previousHovered, nextNormal, nextHovered;
        Vector2 mousePos, textLocation = new Vector2(.88f * Main.Scale.X, (14f / 480f) * Main.Scale.Y);
        WaveBuilder waveBuilder;

        List<Enemy> enemies = new List<Enemy>();

        public DesignerForm Form
        {
            get { return form; }
            set
            {
                if (value == DesignerForm.Enemies && waveBuilder == null)
                {
                    waveBuilder = new WaveBuilder(this);
                }
                else if (value == DesignerForm.Path) pathExists = true;
                form = value;
            }
        }

        public DesignPath Path
        {
            get { return path; }
        }

        private string Name
        {
            get { return name; }
            set { name = value; }
        }

        public void LoadContent()
        {
            colorHealth = Color.LightGray;
            tex = Main.GameContent.Load<Texture2D>("Textures/SQUARE");
            font = Main.GameContent.Load<SpriteFont>("Fonts/Font");

            buildUnpressed = Main.GameContent.Load<Texture2D>("Buttons/Designer/Build Path");
            buildPressed = Main.GameContent.Load<Texture2D>("Buttons/Designer/Build Path Pressed");
            build = new Button(new Vector2(.7625f * Main.Scale.X, (420f / 480f) * Main.Scale.Y), (int)(.225f * Main.Scale.X), (int)(.1125f * Main.Scale.Y), 2, Main.CurrentMouse, buildUnpressed, buildPressed, true, Main.Scale.X, Main.Scale.Y);
            build.LeftClicked += Build;

            previousNormal = Main.GameContent.Load<Texture2D>("Buttons/Designer/Previous Normal");
            previousHovered = Main.GameContent.Load<Texture2D>("Buttons/Designer/Previous Hovered");
            previous = new Button(new Vector2(.7625f * Main.Scale.X, .71f * Main.Scale.Y), (int)((106f / 1440f) * Main.Scale.X), (int)((122f / 900f) * Main.Scale.Y), 3, Main.CurrentMouse, previousNormal, previousHovered, true, Main.Scale.X, Main.Scale.Y);
            previous.LeftClicked += PreviousCurve;

            nextNormal = Main.GameContent.Load<Texture2D>("Buttons/Designer/Next Normal");
            nextHovered = Main.GameContent.Load<Texture2D>("Buttons/Designer/Next Hovered");
            next = new Button(new Vector2(.9155f * Main.Scale.X, .71f * Main.Scale.Y), (int)((106f / 1440f) * Main.Scale.X), (int)((122f / 900f) * Main.Scale.Y), 4, Main.CurrentMouse, nextNormal, nextHovered, true, Main.Scale.X, Main.Scale.Y);
            next.LeftClicked += NextCurve;

            storeSection = new RectangleF(.75f * Main.Scale.X, 0f, (.25f * Main.Scale.X) + 1, (Main.Scale.Y) + 1);
            healthRect = new RectangleF(.7625f * Main.Scale.X, .625f * Main.Scale.Y, .09375f * Main.Scale.X, .0625f * Main.Scale.Y);
            nameRect = new RectangleF(.7625f * Main.Scale.X, (10f / 480f) * Main.Scale.Y, .225f * Main.Scale.X, .0625f * Main.Scale.Y);

            health = "0";
            name = "";
        }

        public void Update(GameTime gameTime)
        {
            Main.CurrentMouse = Main.Mouse.GetState();
            mousePos = new Vector2(Main.CurrentMouse.X * Main.Scale.X, Main.CurrentMouse.Y * Main.Scale.Y);

			if (waveBuilder != null)
			{
				enemiesExist = waveBuilder.TotalEnemies > 0;
			}

            switch (form)
            {
                case DesignerForm.Path:
                    colorHealth = (healthSelected) ? Color.Aqua : Color.LightGray;

                    colorName = (nameSelected) ? Color.Aqua : Color.LightGray;

                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].Update(gameTime);

                        if (enemies[i].position == enemies[i].Path.Points[enemies[i].Path.Points.Count - 1])
                        {
                            enemies.Remove(enemies[i]);
                        }
                    }

                    path.Update();

                    if (pathExists)
                    {
                        if (enemiesExist) build.Update(Main.CurrentMouse);
                        previous.Update(Main.CurrentMouse);
                        next.Update(Main.CurrentMouse);
                    }

                    Input();
                    break;

                case DesignerForm.Enemies:
                    waveBuilder.Update(gameTime);
                    break;

                default:
                    break;
            }
        }

        private void PreviousCurve(object sender, EventArgs e)
        {
            Path.Selected = (Path.Selected != 0) ? Path.Selected - 1 : Path.Curves.Count - 1;
        }

        private void NextCurve(object sender, EventArgs e)
        {
            Path.Selected = (Path.Selected != Path.Curves.Count - 1) ? Path.Selected + 1 : 0;
        }

        private void Build(object sender, EventArgs e)
        {
			MemoryStream stream = new MemoryStream();
			StreamWriter temp = new StreamWriter(stream);
            StreamWriter write = new StreamWriter(string.Format("{0}.path", name));

            temp.WriteLine(path.Curves.Count);

            for (int i = 0; i < path.Curves.Count; i++)
            {
                for (int l = 0; l < path.Curves[i].Points.Count; l++)
                {
                    temp.WriteLine(path.Curves[i].Points[l].X / Main.Scale.X);
                    temp.WriteLine(path.Curves[i].Points[l].Y / Main.Scale.Y);
                }
            }
			
			temp.Flush();
			stream.Position = 0;
			
			StreamReader read = new StreamReader(stream);

            write.Write(StringCipher.Encrypt(read.ReadToEnd(), "temp2"));
            write.Close();
            read.Close();

            path.Clear();

            XmlDocument xml = new XmlDocument();
            XmlNode rootNode = xml.CreateElement("Enemies");
            xml.AppendChild(rootNode);

            for (int i = 0; i < waveBuilder.Waves.Count; i++)
            {
                XmlNode userNode = xml.CreateElement(string.Format("Wave{0}", i + 1));
                for (int j = 0; j < waveBuilder.Waves[i].Count; j++)
                {
                    XmlNode enemyNode = xml.CreateElement("E");
                    enemyNode.InnerText = (int)waveBuilder.Waves[i][j] + "";
                    userNode.AppendChild(enemyNode);
                    rootNode.AppendChild(userNode);
                }
            }
            xml.Save(string.Format("{0}.enm", name));
            //write = new StreamWriter(string.Format("{0}.enm", name));

            waveBuilder.Clear();

            pathExists = false;
        }

        private void Input()
        {
            if (Main.CurrentKeyboard.IsKeyPressed(Keys.P) && !anythingSelected)
            {
                path.AddCurve();
                path.Selected = path.Curves.Count - 1;
                pathExists = true;
            }

            if (Main.CurrentKeyboard.IsKeyPressed(Keys.Left)) PreviousCurve(this, EventArgs.Empty);
            if (Main.CurrentKeyboard.IsKeyPressed(Keys.Right)) NextCurve(this, EventArgs.Empty);

            if (Main.CurrentKeyboard.IsKeyPressed(Keys.D) && !anythingSelected && path.Curves.Count != 0)
            {
                path.Build();
                enemies.Add(new Enemy(this, path));
            }

            if (Main.CurrentKeyboard.IsKeyPressed(Keys.E) && !anythingSelected) Form = DesignerForm.Enemies;

            try
            {
                if (healthSelected)
                {
                    char? c;
                    InputParser.TryConvertNumberInput(Main.CurrentKeyboard, out c);
                    StringInput(ref health, c);
                }
                else if (nameSelected)
                {
                    char? c;
                    InputParser.TryConvertKeyboardInput(Main.CurrentKeyboard, out c);
                    StringInput(ref name, c);
                }
            }
            catch (Exception e)
            { Console.WriteLine(e.Message); }

            if (Main.CurrentMouse.LeftButton.Pressed)
            {
                healthSelected = healthRect.Contains(mousePos);
                nameSelected = nameRect.Contains(mousePos);
            }
            anythingSelected = healthSelected || nameSelected;
        }

        private void StringInput(ref string source, char? c)
        {
            if (c == '\b') source = source.Remove(source.Length - 1);
            else source += c;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            switch (form)
            {
                case DesignerForm.Path:
                    spriteBatch.Draw(tex, storeSection, Color.Black);
                    spriteBatch.Draw(tex, healthRect, colorHealth);
                    spriteBatch.Draw(tex, nameRect, colorName);
                    spriteBatch.DrawString(font, health, new Vector2(.76875f * Main.Scale.X, (298f / 480f) * Main.Scale.Y), Color.Black);
                    spriteBatch.DrawString(font, name, new Vector2(textLocation.X - (font.MeasureString(name).X / 2), textLocation.Y), Color.Black);

                    try
                    {
                        if (pathExists)
                        {
                            if (enemiesExist) spriteBatch.Draw(build.Texture, build.Collision, Color.White);
                            spriteBatch.Draw(previous.Texture, previous.Collision, Color.White);
                            spriteBatch.Draw(next.Texture, next.Collision, Color.White);
                        }
                    }
                    catch { }

                    foreach (Enemy i in enemies)
                    {
                        i.Draw(gameTime, spriteBatch);
                    }

                    path.Draw(spriteBatch);
                    break;

                case DesignerForm.Enemies:
                    waveBuilder.Draw(gameTime, spriteBatch);
                    break;

                default:
                    break;
            }
        }
    }
}