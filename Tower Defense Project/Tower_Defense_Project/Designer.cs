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
        private bool startPlaced;
        private Path path;
        public RectangleF storeSection;

        public List<Tower> towers = new List<Tower>();

        public Designer() { }

        public void LoadContent()
        {

        }

        public void Update(GameTime gameTime)
        {
            Main.CurrentMouse = Main.Mouse.GetState();

            if (!startPlaced)
            {
                startPlaced = towers[0].isPlaced;
            }

            Input();
        }

        private void Input()
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
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tower i in towers)
            {
                i.Draw(spriteBatch);
            }
        }
    }
}