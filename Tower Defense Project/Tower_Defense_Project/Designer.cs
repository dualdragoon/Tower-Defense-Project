using System;
using System.Collections.Generic;
using System.IO;
using Duality.Encrypting;
using Duality.Interaction;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Tower_Defense_Project
{
    enum DesignerForm { Path, Enemies }

    class Designer
    {
        public RectangleF storeSection;
        bool fin, x1Selected, y1Selected, x2Selected, y2Selected, x3Selected, y3Selected, x4Selected, y4Selected, nameSelected, anythingSelected;
        string location1X, location1Y, location2X, location2Y, location3X, location3Y, location4X, location4Y, name;
        Button build, previous, next;
        Color color1X, color1Y, color2X, color2Y, color3X, color3Y, color4X, color4Y, colorName;
        DesignerForm form = DesignerForm.Path;
        DesignPath path = new DesignPath();
        RectangleF x1, y1, x2, y2, x3, y3, x4, y4, nameRect;
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
                else if (value == DesignerForm.Path) fin = true;
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
            color1X = Color.LightGray;
            color1Y = Color.LightGray;
            color2X = Color.LightGray;
            color2Y = Color.LightGray;
            color3X = Color.LightGray;
            color3Y = Color.LightGray;
            color4X = Color.LightGray;
            color4Y = Color.LightGray;
            tex = Main.GameContent.Load<Texture2D>("Textures/SQUARE");
            font = Main.GameContent.Load<SpriteFont>("Fonts/Font");

            buildUnpressed = Main.GameContent.Load<Texture2D>("Buttons/Build Path");
            buildPressed = Main.GameContent.Load<Texture2D>("Buttons/Build Path Pressed");
            build = new Button(new Vector2(.7625f * Main.Scale.X, (420f / 480f) * Main.Scale.Y), (int)(.225f * Main.Scale.X), (int)(.1125f * Main.Scale.Y), 2, Main.CurrentMouse, buildUnpressed, buildPressed, true, Main.Scale.X, Main.Scale.Y);
            build.LeftClicked += Build;

            previousNormal = Main.GameContent.Load<Texture2D>("Buttons/Previous Normal");
            previousHovered = Main.GameContent.Load<Texture2D>("Buttons/Previous Hovered");
            previous = new Button(new Vector2(.7625f * Main.Scale.X, .71f * Main.Scale.Y), (int)((106f / 1440f) * Main.Scale.X), (int)((122f / 900f) * Main.Scale.Y), 3, Main.CurrentMouse, previousNormal, previousHovered, true, Main.Scale.X, Main.Scale.Y);
            previous.LeftClicked += PreviousCurve;

            nextNormal = Main.GameContent.Load<Texture2D>("Buttons/Next Normal");
            nextHovered = Main.GameContent.Load<Texture2D>("Buttons/Next Hovered");
            next = new Button(new Vector2(.9155f * Main.Scale.X, .71f * Main.Scale.Y), (int)((106f / 1440f) * Main.Scale.X), (int)((122f / 900f) * Main.Scale.Y), 4, Main.CurrentMouse, nextNormal, nextHovered, true, Main.Scale.X, Main.Scale.Y);
            next.LeftClicked += NextCurve;

            storeSection = new RectangleF(.75f * Main.Scale.X, 0f, (.25f * Main.Scale.X) + 1, (Main.Scale.Y) + 1);
            x1 = new RectangleF(.7625f * Main.Scale.X, .625f * Main.Scale.Y, .09375f * Main.Scale.X, .0625f * Main.Scale.Y);
            y1 = new RectangleF(.89375f * Main.Scale.X, .625f * Main.Scale.Y, .09375f * Main.Scale.X, .0625f * Main.Scale.Y);
            x2 = new RectangleF(.7625f * Main.Scale.X, (260f / 480f) * Main.Scale.Y, .09375f * Main.Scale.X, .0625f * Main.Scale.Y);
            y2 = new RectangleF(.89375f * Main.Scale.X, (260f / 480f) * Main.Scale.Y, .09375f * Main.Scale.X, .0625f * Main.Scale.Y);
            x3 = new RectangleF(.7625f * Main.Scale.X, (220f / 480f) * Main.Scale.Y, .09375f * Main.Scale.X, .0625f * Main.Scale.Y);
            y3 = new RectangleF(.89375f * Main.Scale.X, (220f / 480f) * Main.Scale.Y, .09375f * Main.Scale.X, .0625f * Main.Scale.Y);
            x4 = new RectangleF(.7625f * Main.Scale.X, .375f * Main.Scale.Y, .09375f * Main.Scale.X, .0625f * Main.Scale.Y);
            y4 = new RectangleF(.89375f * Main.Scale.X, .375f * Main.Scale.Y, .09375f * Main.Scale.X, .0625f * Main.Scale.Y);
            nameRect = new RectangleF(.7625f * Main.Scale.X, (10f / 480f) * Main.Scale.Y, .225f * Main.Scale.X, .0625f * Main.Scale.Y);

            location1X = "0";
            location1Y = "0";
            location2X = "0";
            location2Y = "0";
            location3X = "0";
            location3Y = "0";
            location4X = "0";
            location4Y = "0";
            name = "";
        }

        public void Update(GameTime gameTime)
        {
            Main.CurrentMouse = Main.Mouse.GetState();
            mousePos = new Vector2(Main.CurrentMouse.X * Main.Scale.X, Main.CurrentMouse.Y * Main.Scale.Y);

            switch (form)
            {
                case DesignerForm.Path:
                    color1X = (x1Selected) ? Color.Aqua : Color.LightGray;
                    color1Y = (y1Selected) ? Color.Aqua : Color.LightGray;
                    color2X = (x2Selected) ? Color.Aqua : Color.LightGray;
                    color2Y = (y2Selected) ? Color.Aqua : Color.LightGray;
                    color3X = (x3Selected) ? Color.Aqua : Color.LightGray;
                    color3Y = (y3Selected) ? Color.Aqua : Color.LightGray;
                    color4X = (x4Selected) ? Color.Aqua : Color.LightGray;
                    color4Y = (y4Selected) ? Color.Aqua : Color.LightGray;

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

                    if (fin)
                    {
                        build.Update(Main.CurrentMouse);
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
            StreamWriter temp = new StreamWriter("temp1.temp");
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

            temp.Close();

            StreamReader read = new StreamReader("temp1.temp");

            write.Write(StringCipher.Encrypt(read.ReadToEnd(), "temp2"));
            write.Close();
            read.Close();
            File.Delete("temp1.temp");

            path.Clear();

            fin = false;
        }

        private void Input()
        {
            if (Main.CurrentKeyboard.IsKeyPressed(Keys.P) && !anythingSelected)
            {
                path.AddCurve();
                path.Selected = path.Curves.Count - 1;
                fin = true;
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
                if ((x1Selected || y1Selected || x2Selected || y2Selected || x3Selected || y3Selected || x4Selected || y4Selected))
                {
                    char? c;
                    InputParser.TryConvertNumberInput(Main.CurrentKeyboard, out c);
                    if (x1Selected) StringInput(ref location1X, c);
                    else if (y1Selected) StringInput(ref location1Y, c);
                    else if (x2Selected) StringInput(ref location2X, c);
                    else if (y2Selected) StringInput(ref location2Y, c);
                    else if (x3Selected) StringInput(ref location3X, c);
                    else if (y3Selected) StringInput(ref location3Y, c);
                    else if (x4Selected) StringInput(ref location4X, c);
                    else if (y4Selected) StringInput(ref location4Y, c);

                    if (Main.CurrentKeyboard.IsKeyPressed(Keys.Enter))
                    {

                    }
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
                x1Selected = x1.Contains(mousePos);
                y1Selected = y1.Contains(mousePos);
                x2Selected = x2.Contains(mousePos);
                y2Selected = y2.Contains(mousePos);
                x3Selected = x3.Contains(mousePos);
                y3Selected = y3.Contains(mousePos);
                x4Selected = x4.Contains(mousePos);
                y4Selected = y4.Contains(mousePos);
                nameSelected = nameRect.Contains(mousePos);
            }
            anythingSelected = x1Selected || y1Selected || x2Selected || y2Selected || x3Selected || y3Selected || x4Selected || y4Selected || nameSelected;
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
                    spriteBatch.Draw(tex, x1, color1X);
                    spriteBatch.Draw(tex, y1, color1Y);
                    spriteBatch.Draw(tex, x2, color2X);
                    spriteBatch.Draw(tex, y2, color2Y);
                    spriteBatch.Draw(tex, x3, color3X);
                    spriteBatch.Draw(tex, y3, color3Y);
                    spriteBatch.Draw(tex, x4, color4X);
                    spriteBatch.Draw(tex, y4, color4Y);
                    spriteBatch.Draw(tex, nameRect, colorName);
                    spriteBatch.DrawString(font, location1X, new Vector2(.76875f * Main.Scale.X, (298f / 480f) * Main.Scale.Y), Color.Black);
                    spriteBatch.DrawString(font, location1Y, new Vector2(.9f * Main.Scale.X, (298f / 480f) * Main.Scale.Y), Color.Black);
                    spriteBatch.DrawString(font, location2X, new Vector2(.76875f * Main.Scale.X, .5375f * Main.Scale.Y), Color.Black);
                    spriteBatch.DrawString(font, location2Y, new Vector2(.9f * Main.Scale.X, .5375f * Main.Scale.Y), Color.Black);
                    spriteBatch.DrawString(font, location3X, new Vector2(.76875f * Main.Scale.X, (218f / 480f) * Main.Scale.Y), Color.Black);
                    spriteBatch.DrawString(font, location3Y, new Vector2(.9f * Main.Scale.X, (218f / 480f) * Main.Scale.Y), Color.Black);
                    spriteBatch.DrawString(font, location4X, new Vector2(.76875f * Main.Scale.X, (178f / 480f) * Main.Scale.Y), Color.Black);
                    spriteBatch.DrawString(font, location4Y, new Vector2(.9f * Main.Scale.X, (178f / 480f) * Main.Scale.Y), Color.Black);
                    spriteBatch.DrawString(font, name, new Vector2(textLocation.X - (font.MeasureString(name).X / 2), textLocation.Y), Color.Black);

                    try
                    {
                        if (fin)
                        {
                            spriteBatch.Draw(build.Texture, build.Collision, Color.White);
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