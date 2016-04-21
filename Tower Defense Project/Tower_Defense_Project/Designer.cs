using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private bool startPlaced, fin, xSelected, ySelected, xSelectedRect, ySelectedRect, widthSelected, heightSelected, nameSelected, anythingSelected;
        private Button build;
        private Color colorX, colorY, colorXRect, colorYRect, colorWidth, colorHeight, colorName;
        private DesignerForm form = DesignerForm.Path;
        private Path path = new Path();
        private RectangleF x, y, xRect, yRect, width, height, nameRect;
        public RectangleF storeSection;
        private RectangleSelection selectedRectangle;
        private SpriteFont font;
        private string selectedLocationX, selectedLocationY, rectangleX, rectangleY, rectangleWidth, rectangleHeight, name;
        private Texture2D tex, buildUnpressed, buildPressed;
        private Tower selected;
        private Vector2 mousePos, textLocation = new Vector2(690, 8);

        Random p = new Random();

        public List<Tower> towers = new List<Tower>();
        private List<RectangleSelection> pieces = new List<RectangleSelection>();

        private static Dictionary<int, string[]> towerStats = new Dictionary<int, string[]>();

        public static Dictionary<int, string[]> TowerStats
        {
            get { return towerStats; }
            set { towerStats = value; }
        }

        public Path Path
        {
            get { return path; }
        }

        private string Name
        {
            get { return name; }
            set
            {
                bool backspaced = false;
                try { backspaced = (value == name.Remove(name.Length - 1)); }
                catch { }
                name = value;
                if (name.Length <= 1) textLocation.X = 690;
                else if (!backspaced) textLocation.X -= 8.5f;
                else textLocation.X += 8.5f;
            }
        }

        public Designer() { }

        public void LoadContent()
        {
            string[] s = new string[] { "Start", "32", "100" };
            towerStats.Add(0, s);

            s = new string[] { "Point", "32", "100" };
            towerStats.Add(1, s);

            s = new string[] { "Stop", "32", "100" };
            towerStats.Add(2, s);

            colorX = Color.LightGray;
            colorY = Color.LightGray;
            colorXRect = Color.LightGray;
            colorYRect = Color.LightGray;
            colorWidth = Color.LightGray;
            colorHeight = Color.LightGray;
            tex = Main.GameContent.Load<Texture2D>(@"Textures/SQUARE");
            font = Main.GameContent.Load<SpriteFont>(@"Fonts/Font");

            selected = Selected();

            buildUnpressed = Main.GameContent.Load<Texture2D>(@"Buttons/Build Path");
            buildPressed = Main.GameContent.Load<Texture2D>(@"Buttons/Build Path Pressed");
            build = new Button(new Vector2(610, 380), 180, 90, 2, Main.CurrentMouse, buildUnpressed, buildPressed, true, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
            build.LeftClicked += Build;

            storeSection = new RectangleF(.75f * Main.Graphics.PreferredBackBufferWidth, 0f * Main.Graphics.PreferredBackBufferHeight, (.25f * Main.Graphics.PreferredBackBufferWidth) + 1, (Main.Graphics.PreferredBackBufferHeight) + 1);
            x = new RectangleF(610, 300, 75, 30);
            y = new RectangleF(715, 300, 75, 30);
            xRect = new RectangleF(610, 50, 75, 30);
            yRect = new RectangleF(715, 50, 75, 30);
            width = new RectangleF(610, 100, 75, 30);
            height = new RectangleF(715, 100, 75, 30);
            nameRect = new RectangleF(610, 10, 180, 30);

            selectedLocationX = "0";
            selectedLocationY = "0";
            rectangleX = "0";
            rectangleY = "0";
            rectangleWidth = "0";
            rectangleHeight = "0";
            name = "";
        }

        public void Update(GameTime gameTime)
        {
            Main.CurrentMouse = Main.Mouse.GetState();
            mousePos = new Vector2(Main.CurrentMouse.X * Main.Graphics.PreferredBackBufferWidth, Main.CurrentMouse.Y * Main.Graphics.PreferredBackBufferHeight);

            switch (form)
            {
                case DesignerForm.Path:
                    if (!startPlaced)
                    {
                        try { startPlaced = towers[0].isPlaced; }
                        catch { }
                    }

                    colorX = (xSelected && towers.Contains(selected)) ? Color.Aqua : Color.LightGray;
                    colorY = (ySelected && towers.Contains(selected)) ? Color.Aqua : Color.LightGray;

                    colorXRect = (xSelectedRect && pieces.Contains(selectedRectangle)) ? Color.Aqua : Color.LightGray;
                    colorYRect = (ySelectedRect && pieces.Contains(selectedRectangle)) ? Color.Aqua : Color.LightGray;
                    colorWidth = (widthSelected && pieces.Contains(selectedRectangle)) ? Color.Aqua : Color.LightGray;
                    colorHeight = (heightSelected && pieces.Contains(selectedRectangle)) ? Color.Aqua : Color.LightGray;

                    colorName = (nameSelected) ? Color.Aqua : Color.LightGray;

                    foreach (Tower i in towers)
                    {
                        i.UpdateDesigner(gameTime, Main.CurrentMouse);
                    }

                    foreach (RectangleSelection i in pieces)
                    {
                        i.Update();
                    }

                    if (Main.CurrentMouse.RightButton.Pressed)
                    {
                        selected = Selected();
                        selectedRectangle = SelectedRectangle();
                    }

                    if (fin && pieces.Count > 0) build.Update(Main.CurrentMouse);

                    Input();
                    break;

                case DesignerForm.Enemies:
                    break;

                default:
                    break;
            }
        }

        private void Build(object sender, EventArgs e)
        {
            path = new Path();

            foreach (Tower i in towers)
            {
                path.points.Add(new Vector2(i.Center.X / Main.Graphics.PreferredBackBufferWidth, i.Center.Y / Main.Graphics.PreferredBackBufferHeight));
            }

            foreach (RectangleSelection i in pieces)
            {
                path.pathSet.Add(new RectangleF(i.SelectedRectangle.X / Main.Graphics.PreferredBackBufferWidth, i.SelectedRectangle.Y / Main.Graphics.PreferredBackBufferHeight, i.SelectedRectangle.Width / Main.Graphics.PreferredBackBufferWidth, i.SelectedRectangle.Height / Main.Graphics.PreferredBackBufferHeight));
            }

            StreamWriter temp = new StreamWriter("temp1.temp");
            StreamWriter write = new StreamWriter(string.Format("{0}.path", name));

            temp.WriteLine(path.points.Count);
            temp.WriteLine(path.pathSet.Count);

            for (int i = 0; i < path.points.Count; i++)
            {
                temp.WriteLine(path.points[i].X);
                temp.WriteLine(path.points[i].Y);
            }

            for (int i = 0; i < path.pathSet.Count; i++)
            {
                temp.WriteLine(path.pathSet[i].X);
                temp.WriteLine(path.pathSet[i].Y);
                temp.WriteLine(path.pathSet[i].Width);
                temp.WriteLine(path.pathSet[i].Height);
            }

            temp.Close();

            StreamReader read = new StreamReader("temp1.temp");

            write.Write(StringCipher.Encrypt(read.ReadToEnd(), "temp2"));
            write.Close();
            read.Close();
            File.Delete("temp1.temp");

            towers.Clear();
            pieces.Clear();
            fin = false;
        }

        private Tower Selected()
        {
            Tower t;
            foreach (Tower i in towers)
            {
                if (i.isSelected)
                {
                    if (i != selected) selectedLocationX = i.Position.X.ToString(); selectedLocationY = i.Position.Y.ToString();
                    return i;
                }
            }
            t = new Tower(this, TowerType.Start, Main.CurrentMouse);
            t.isPlaced = true;
            t.Position = Vector2.Zero;
            selectedLocationX = "0";
            selectedLocationY = "0";
            xSelected = false;
            ySelected = false;
            return t;
        }

        private RectangleSelection SelectedRectangle()
        {
            RectangleSelection t;
            foreach (RectangleSelection i in pieces)
            {
                if (i.isSelected)
                {
                    if (i != selectedRectangle)
                    {
                        rectangleX = i.SelectedRectangle.X.ToString();
                        rectangleY = i.SelectedRectangle.Y.ToString();
                        rectangleWidth = i.SelectedRectangle.Width.ToString();
                        rectangleHeight = i.SelectedRectangle.Height.ToString();
                    }
                    return i;
                }
            }
            t = new RectangleSelection(0, 0, 0, 0);
            rectangleX = "0";
            rectangleY = "0";
            rectangleWidth = "0";
            rectangleHeight = "0";
            xSelectedRect = false;
            ySelectedRect = false;
            widthSelected = false;
            heightSelected = false;
            return t;
        }

        private void Input()
        {
            if (!fin)
            {
                if (Main.CurrentKeyboard.IsKeyPressed(Keys.D1) && !startPlaced && !anythingSelected)
                {
                    towers.Add(new Tower(this, TowerType.Start, Main.CurrentMouse));
                }
                else if (Main.CurrentKeyboard.IsKeyPressed(Keys.D1) && startPlaced && towers[towers.Count - 1].isPlaced && !anythingSelected)
                {
                    towers.Add(new Tower(this, TowerType.Point, Main.CurrentMouse));
                }
                else if (Main.CurrentKeyboard.IsKeyPressed(Keys.D2) && startPlaced && towers[towers.Count - 1].isPlaced && !anythingSelected)
                {
                    towers.Add(new Tower(this, TowerType.Stop, Main.CurrentMouse));
                    fin = true;
                }
            }

            if (Main.CurrentKeyboard.IsKeyPressed(Keys.D3) && !anythingSelected) pieces.Add(new RectangleSelection(10, 10, 40, 40));

            try
            {
                if ((xSelected || ySelected || xSelectedRect || ySelectedRect || widthSelected || heightSelected))
                {
                    char? c;
                    InputParser.TryConvertNumberInput(Main.CurrentKeyboard, out c);
                    if (xSelected) StringInput(ref selectedLocationX, c);
                    else if (ySelected) StringInput(ref selectedLocationY, c);
                    else if (xSelectedRect) StringInput(ref rectangleX, c);
                    else if (ySelectedRect) StringInput(ref rectangleY, c);
                    else if (widthSelected) StringInput(ref rectangleWidth, c);
                    else if (heightSelected) StringInput(ref rectangleHeight, c);

                    if (Main.CurrentKeyboard.IsKeyPressed(Keys.Enter))
                    {
                        selected.Position = new Vector2(float.Parse(selectedLocationX), float.Parse(selectedLocationY));
                        selectedRectangle.Point1 = new Vector2(float.Parse(rectangleX), float.Parse(rectangleY));
                        selectedRectangle.Point4 = new Vector2(float.Parse(rectangleX) + float.Parse(rectangleWidth), float.Parse(rectangleY) + float.Parse(rectangleHeight));
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
                xSelected = x.Contains(mousePos);
                ySelected = y.Contains(mousePos);
                xSelectedRect = xRect.Contains(mousePos);
                ySelectedRect = yRect.Contains(mousePos);
                widthSelected = width.Contains(mousePos);
                heightSelected = height.Contains(mousePos);
                nameSelected = nameRect.Contains(mousePos);
            }
            anythingSelected = xSelected || ySelected || xSelectedRect || ySelectedRect || widthSelected || heightSelected || nameSelected;
        }

        private void StringInput(ref string source, char? c)
        {
            if (source != name)
            {
                if (c == '\b') source = source.Remove(source.Length - 1);
                else source += c;
            }
            else
            {
                if (c == '\b') Name = Name.Remove(Name.Length - 1);
                else if (c != null)
                {
                    Name += c;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (form)
            {
                case DesignerForm.Path:
                    spriteBatch.Draw(tex, storeSection, Color.Black);
                    spriteBatch.Draw(tex, x, colorX);
                    spriteBatch.Draw(tex, y, colorY);
                    spriteBatch.Draw(tex, xRect, colorXRect);
                    spriteBatch.Draw(tex, yRect, colorYRect);
                    spriteBatch.Draw(tex, width, colorWidth);
                    spriteBatch.Draw(tex, height, colorHeight);
                    spriteBatch.Draw(tex, nameRect, colorName);
                    spriteBatch.DrawString(font, selectedLocationX, new Vector2(615, 298), Color.Black);
                    spriteBatch.DrawString(font, selectedLocationY, new Vector2(720, 298), Color.Black);
                    spriteBatch.DrawString(font, rectangleX, new Vector2(615, 48), Color.Black);
                    spriteBatch.DrawString(font, rectangleY, new Vector2(720, 48), Color.Black);
                    spriteBatch.DrawString(font, rectangleWidth, new Vector2(615, 98), Color.Black);
                    spriteBatch.DrawString(font, rectangleHeight, new Vector2(720, 98), Color.Black);
                    spriteBatch.DrawString(font, name, textLocation, Color.Black);

                    try { if (fin) spriteBatch.Draw(build.Texture, build.Collision, Color.White); }
                    catch { }

                    foreach (Tower i in towers)
                    {
                        i.Draw(spriteBatch);
                    }

                    foreach (RectangleSelection i in pieces)
                    {
                        i.Draw(spriteBatch);
                    }
                    break;

                case DesignerForm.Enemies:
                    spriteBatch.Draw(tex, new RectangleF(0, 0, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight), Color.Black);
                    break;

                default:
                    break;
            }
        }
    }
}