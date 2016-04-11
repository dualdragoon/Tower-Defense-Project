using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Duality.Interaction;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Tower_Defense_Project
{
    class Designer
    {
        private bool startPlaced, fin = false, xSelected, ySelected;
        private Button build;
        private Color colorX, colorY;
        private Path path;
        private RectangleF x, y;
        public RectangleF storeSection;
        private SpriteFont font;
        private string selectedLocationX, selectedLocationY;
        private Texture2D tex, buildUnpressed, buildPressed;
        private Tower selected;

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
            tex = Main.GameContent.Load<Texture2D>(@"Textures/SQUARE");
            font = Main.GameContent.Load<SpriteFont>(@"Fonts/Font");

            path = new Path();
            selected = Selected();

            buildUnpressed = Main.GameContent.Load<Texture2D>(@"Buttons/Build Path");
            buildPressed = Main.GameContent.Load<Texture2D>(@"Buttons/Build Path Pressed");
            build = new Button(new Vector2(610, 380), 180, 90, 2, Main.CurrentMouse, buildUnpressed, buildPressed, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
            build.ButtonPressed += Build;

            storeSection = new RectangleF(.75f * Main.Graphics.PreferredBackBufferWidth, 0f * Main.Graphics.PreferredBackBufferHeight, (.25f * Main.Graphics.PreferredBackBufferWidth) + 1, (Main.Graphics.PreferredBackBufferHeight) + 1);
            x = new RectangleF(610, 300, 75, 30);
            y = new RectangleF(715, 300, 75, 30);
        }

        public void Update(GameTime gameTime)
        {
            Main.CurrentMouse = Main.Mouse.GetState();

            if (!startPlaced)
            {
                try { startPlaced = towers[0].isPlaced; }
                catch { }
            }

            if (Main.CurrentMouse.LeftButton.Pressed && towers.Contains(selected))
            {
                if (x.Contains(Main.CurrentMouse.X * Main.Graphics.PreferredBackBufferWidth, Main.CurrentMouse.Y * Main.Graphics.PreferredBackBufferHeight))
                {
                    xSelected = true;
                    colorX = Color.Aqua;
                }
                else
                {
                    xSelected = false;
                    colorX = Color.LightGray;
                }

                if (y.Contains(Main.CurrentMouse.X * Main.Graphics.PreferredBackBufferWidth, Main.CurrentMouse.Y * Main.Graphics.PreferredBackBufferHeight))
                {
                    ySelected = true;
                    colorY = Color.Aqua;
                }
                else
                {
                    ySelected = false;
                    colorY = Color.LightGray;
                }
            }

            foreach (Tower i in towers)
            {
                i.UpdateDesigner(gameTime, Main.CurrentMouse);
            }

            foreach (RectangleSelection i in pieces)
            {
                i.Update();
            }

            if (Main.CurrentMouse.RightButton.Pressed) selected = Selected();

            if (fin) build.Update(Main.CurrentMouse);

            Input();
        }

        private void Build(object sender, EventArgs e)
        {

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

        private void Input()
        {
            if (!fin)
            {
                if (Main.CurrentKeyboard.IsKeyPressed(Keys.D1) && !startPlaced)
                {
                    towers.Add(new Tower(this, TowerType.Start, Main.CurrentMouse));
                }
                else if (Main.CurrentKeyboard.IsKeyPressed(Keys.D1) && startPlaced)
                {
                    towers.Add(new Tower(this, TowerType.Point, Main.CurrentMouse));
                }
                else if (Main.CurrentKeyboard.IsKeyPressed(Keys.D2) && startPlaced)
                {
                    towers.Add(new Tower(this, TowerType.Stop, Main.CurrentMouse));
                    fin = true;
                }

                if (Main.CurrentKeyboard.IsKeyPressed(Keys.D3)) pieces.Add(new RectangleSelection(10, 10, 40, 40));
            }

            try
            {
                if ((xSelected || ySelected))
                {
                    char? c;
                    InputParser.TryConvertNumberInput(Main.CurrentKeyboard, out c);
                    if (xSelected)
                    {
                        if (c == '\b') selectedLocationX = selectedLocationX.Remove(selectedLocationX.Length - 1);
                        else selectedLocationX += c;
                    }
                    else
                    {
                        if (c == '\b') selectedLocationY = selectedLocationY.Remove(selectedLocationY.Length - 1);
                        else selectedLocationY += c;
                    }

                    if (Main.CurrentKeyboard.IsKeyPressed(Keys.Enter))
                    {
                        selected.Position = new Vector2(float.Parse(selectedLocationX), float.Parse(selectedLocationY));
                    }
                }
            }
            catch { }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, storeSection, Color.Black);
            spriteBatch.Draw(tex, x, colorX);
            spriteBatch.Draw(tex, y, colorY);
            spriteBatch.DrawString(font, selectedLocationX, new Vector2(615, 298), Color.Black);
            spriteBatch.DrawString(font, selectedLocationY, new Vector2(720, 298), Color.Black);

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
        }
    }
}