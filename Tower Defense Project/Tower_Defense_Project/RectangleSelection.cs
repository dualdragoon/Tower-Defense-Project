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
    class RectangleSelection
    {
        private Button point1, point2, point3, point4;
        private Texture2D unpressed, pressed, tex, back;

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

        public RectangleF SelectedRectangle
        {
            get
            {
                if (Point1.Y <= Point3.Y)
                {
                    if (Point1.X <= Point2.X) return new RectangleF(Point1.X, Point1.Y, Point2.X - Point1.X, Point3.Y - Point1.Y);
                    else return new RectangleF(Point2.X, Point2.Y, Point1.X - Point2.X, Point4.Y - Point2.Y);
                }
                else
                {
                    if (Point1.X <= Point2.X) return new RectangleF(Point3.X, Point3.Y, Point4.X - Point3.X, Point1.Y - Point3.Y);
                    else return new RectangleF(Point4.X, Point4.Y, Point3.X - Point4.X, Point2.Y - Point4.Y);
                }
            }
        }
        #endregion

        public RectangleSelection(float x, float y, float width, float height)
        {
            unpressed = Main.GameContent.Load<Texture2D>(@"Textures/Point");
            pressed = Main.GameContent.Load<Texture2D>(@"Textures/Point Pressed");
            tex = Main.GameContent.Load<Texture2D>(@"Textures/SQUARE");
            back = Main.GameContent.Load<Texture2D>(@"Textures/Transparent Pixel");

            point1 = new Button(new Vector2(x, y), 16, 0, Main.CurrentMouse, unpressed, pressed, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
            point2 = new Button(new Vector2(x + width, y), 16, 1, Main.CurrentMouse, unpressed, pressed, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
            point3 = new Button(new Vector2(x, y + height), 16, 2, Main.CurrentMouse, unpressed, pressed, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
            point4 = new Button(new Vector2(x + width, y + height), 16, 3, Main.CurrentMouse, unpressed, pressed, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
        }

        public void Update()
        {
            point1.Update(Main.CurrentMouse);
            point2.Update(Main.CurrentMouse);
            point3.Update(Main.CurrentMouse);
            point4.Update(Main.CurrentMouse);

            if (point1.Grabbed) Point1 = new Vector2(Main.CurrentMouse.X * Main.Graphics.PreferredBackBufferWidth, Main.CurrentMouse.Y * Main.Graphics.PreferredBackBufferHeight);
            else if (point2.Grabbed) Point2 = new Vector2(Main.CurrentMouse.X * Main.Graphics.PreferredBackBufferWidth, Main.CurrentMouse.Y * Main.Graphics.PreferredBackBufferHeight);
            else if (point3.Grabbed) Point3 = new Vector2(Main.CurrentMouse.X * Main.Graphics.PreferredBackBufferWidth, Main.CurrentMouse.Y * Main.Graphics.PreferredBackBufferHeight);
            else if (point4.Grabbed) Point4 = new Vector2(Main.CurrentMouse.X * Main.Graphics.PreferredBackBufferWidth, Main.CurrentMouse.Y * Main.Graphics.PreferredBackBufferHeight);
        }

        private void DrawLine(SpriteBatch spriteBatch, Vector2 begin, Vector2 end, int width = 1)
        {
            RectangleF r = new RectangleF(begin.X, begin.Y, (end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathUtil.TwoPi - angle;
            spriteBatch.Draw(tex, r, null, Color.Black, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(back, SelectedRectangle, Color.White);

            DrawLine(spriteBatch, point1.Center, point2.Center);
            DrawLine(spriteBatch, point2.Center, point4.Center);
            DrawLine(spriteBatch, point4.Center, point3.Center);
            DrawLine(spriteBatch, point3.Center, point1.Center);

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
