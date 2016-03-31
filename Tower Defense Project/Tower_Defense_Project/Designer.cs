using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Tower_Defense_Project
{
    class Designer
    {
        private bool startPlaced, fin = false;
        private Path path;
        private RectangleF x, y;
        public RectangleF storeSection;
        private SpriteFont font;
        private Texture2D tex;
        private Tower selected;
        private Vector2 selectedLocation;

        public List<Tower> towers = new List<Tower>();

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

            tex = Main.GameContent.Load<Texture2D>(@"Textures/SQUARE");
            font = Main.GameContent.Load<SpriteFont>(@"Fonts/Font");

            path = new Path();
            selected = new Tower(this, TowerType.Start, Main.CurrentMouse);
            
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

            foreach (Tower i in towers)
            {
                i.UpdateDesigner(gameTime, Main.CurrentMouse);
            }

            selected = Selected();

            Input();
        }
        
        private Tower Selected()
        {
            Tower t;
            foreach (Tower i in towers)
            {
                if (i.isSelected)
                {
                    if (i != selected) selectedLocation = i.Position;
                    return i;
                }
            }
            t = new Tower(this, TowerType.Start, Main.CurrentMouse);
            t.isPlaced = true;
            t.Position = Vector2.Zero;
            selectedLocation = t.Position;
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
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, storeSection, Color.Black);
            spriteBatch.Draw(tex, x, Color.LightGray);
            spriteBatch.Draw(tex, y, Color.LightGray);
            spriteBatch.DrawString(font, selectedLocation.X.ToString(), new Vector2(615, 298), Color.Black);
            spriteBatch.DrawString(font, selectedLocation.Y.ToString(), new Vector2(720, 298), Color.Black);

            foreach (Tower i in towers)
            {
                i.Draw(spriteBatch);
            }
        }
    }
}