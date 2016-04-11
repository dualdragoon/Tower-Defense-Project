using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Duality.Interaction;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Tower_Defense_Project
{
    struct RectangleSelection
    {
        private Button point1, point2, point3, point4;
        private Texture2D unpressed, pressed;

        #region Properties
        private Vector2 Point1
        {
            get { return point1.Center; }
            set
            {
                point1.Center = value;
                point2.Center = new Vector2(point2.Center.X, value.Y);
                point3.Center = new Vector2(value.X, point3.Center.Y);
            }
        }

        private Vector2 Point2
        {
            get { return point2.Center; }
            set
            {
                point2.Center = value;
                point1.Center = new Vector2(point1.Center.X, value.Y);
                point4.Center = new Vector2(value.X, point4.Center.Y);
            }
        }

        private Vector2 Point3
        {
            get { return point3.Center; }
            set
            {
                point3.Center = value;
                point4.Center = new Vector2(point4.Center.X, value.Y);
                point1.Center = new Vector2(value.X, point1.Center.Y);
            }
        }

        private Vector2 Point4
        {
            get { return point4.Center; }
            set
            {
                point4.Center = value;
                point3.Center = new Vector2(point3.Center.X, value.Y);
                point2.Center = new Vector2(value.X, point2.Center.Y);
            }
        }
        #endregion

        public RectangleSelection(float x, float y, float width, float height)
        {
            unpressed = Main.GameContent.Load<Texture2D>(@"Textures/Point");
            pressed = Main.GameContent.Load<Texture2D>(@"Textures/Point Pressed");

            point1 = new Button(new Vector2(x, y), 0, Main.CurrentMouse, unpressed, pressed, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
            point2 = new Button(new Vector2(x + width, y), 0, Main.CurrentMouse, unpressed, pressed, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
            point3 = new Button(new Vector2(x, y + height), 0, Main.CurrentMouse, unpressed, pressed, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
            point4 = new Button(new Vector2(x + width, y + height), 0, Main.CurrentMouse, unpressed, pressed, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
        }

        public void Update()
        {
            point1.Update(Main.CurrentMouse);
            point2.Update(Main.CurrentMouse);
            point3.Update(Main.CurrentMouse);
            point4.Update(Main.CurrentMouse);

            if (point1.Grabbed) Point1 = new Vector2(Main.CurrentMouse.X * Main.Graphics.PreferredBackBufferWidth, Main.CurrentMouse.Y * Main.Graphics.PreferredBackBufferHeight);
            if (point2.Grabbed) Point2 = new Vector2(Main.CurrentMouse.X * Main.Graphics.PreferredBackBufferWidth, Main.CurrentMouse.Y * Main.Graphics.PreferredBackBufferHeight);
            if (point3.Grabbed) Point3 = new Vector2(Main.CurrentMouse.X * Main.Graphics.PreferredBackBufferWidth, Main.CurrentMouse.Y * Main.Graphics.PreferredBackBufferHeight);
            if (point4.Grabbed) Point4 = new Vector2(Main.CurrentMouse.X * Main.Graphics.PreferredBackBufferWidth, Main.CurrentMouse.Y * Main.Graphics.PreferredBackBufferHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            try
            {
                spriteBatch.Draw(point1.Texture, point1.Position, Color.White);
                spriteBatch.Draw(point2.Texture, point2.Position, Color.White);
                spriteBatch.Draw(point3.Texture, point3.Position, Color.White);
                spriteBatch.Draw(point4.Texture, point4.Position, Color.White);
            }
            catch { }
        }
    }
}
