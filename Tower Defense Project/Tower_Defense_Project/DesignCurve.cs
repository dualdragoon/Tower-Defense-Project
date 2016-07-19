using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit.Input;
using SharpDX.Toolkit.Graphics;
using Duality;
using Duality.Interaction;

namespace Tower_Defense_Project
{
    class DesignCurve : Curve
    {
        Button point1, point2, point3, point4;
        Texture2D pressed, unpressed;

        Vector2 Button1
        {
            get { return point1.Center; }
            set
            {
                point1.Center = value;
                Points[0] = value;
            }
        }

        Vector2 Button2
        {
            get { return point2.Center; }
            set
            {
                point2.Center = value;
                Points[1] = value;
            }
        }

        Vector2 Button3
        {
            get { return point3.Center; }
            set
            {
                point3.Center = value;
                Points[2] = value;
            }
        }

        Vector2 Button4
        {
            get { return point4.Center; }
            set
            {
                point4.Center = value;
                Points[3] = value;
            }
        }

        public DesignCurve() : base()
        {
            LoadContent();
        }

        public DesignCurve(List<Vector2> points) : base(points)
        {
            LoadContent();
        }

        private void LoadContent()
        {
            pressed = Main.GameContent.Load<Texture2D>("Textures/Point Pressed");
            unpressed = Main.GameContent.Load<Texture2D>("Textures/Point");

            point1 = new Button(Points[0], 16, 1, Main.CurrentMouse, unpressed, pressed, true, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
            point2 = new Button(Points[1], 16, 2, Main.CurrentMouse, unpressed, pressed, true, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
            point3 = new Button(Points[2], 16, 3, Main.CurrentMouse, unpressed, pressed, true, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
            point4 = new Button(Points[3], 16, 4, Main.CurrentMouse, unpressed, pressed, true, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight);
        }

        public void Update()
        {
            point1.Update(Main.CurrentMouse);
            point2.Update(Main.CurrentMouse);
            point3.Update(Main.CurrentMouse);
            point4.Update(Main.CurrentMouse);

            if (point1.LeftHeld)
            {
                Button1 = new Vector2(Main.CurrentMouse.X * Main.Graphics.PreferredBackBufferWidth, Main.CurrentMouse.Y * Main.Graphics.PreferredBackBufferHeight);
                point2.Clickable = false;
                point3.Clickable = false;
                point4.Clickable = false;
            }

            if (point2.LeftHeld)
            {
                Button2 = new Vector2(Main.CurrentMouse.X * Main.Graphics.PreferredBackBufferWidth, Main.CurrentMouse.Y * Main.Graphics.PreferredBackBufferHeight);
                point1.Clickable = false;
                point3.Clickable = false;
                point4.Clickable = false;
            }

            if (point3.LeftHeld)
            {
                Button3 = new Vector2(Main.CurrentMouse.X * Main.Graphics.PreferredBackBufferWidth, Main.CurrentMouse.Y * Main.Graphics.PreferredBackBufferHeight);
                point1.Clickable = false;
                point2.Clickable = false;
                point4.Clickable = false;
            }

            if (point4.LeftHeld)
            {
                Button4 = new Vector2(Main.CurrentMouse.X * Main.Graphics.PreferredBackBufferWidth, Main.CurrentMouse.Y * Main.Graphics.PreferredBackBufferHeight);
                point1.Clickable = false;
                point2.Clickable = false;
                point3.Clickable = false;
            }
            else
            {
                point1.Clickable = true;
                point2.Clickable = true;
                point3.Clickable = true;
                point4.Clickable = true;
            }
        }

        new public void Draw(SpriteBatch spriteBatch, Texture2D tex)
        {
            base.Draw(spriteBatch, tex);

            spriteBatch.Draw(point1.Texture, point1.Position, Color.White);
            spriteBatch.Draw(point2.Texture, point2.Position, Color.White);
            spriteBatch.Draw(point3.Texture, point3.Position, Color.White);
            spriteBatch.Draw(point4.Texture, point4.Position, Color.White);
        }
    }
}
