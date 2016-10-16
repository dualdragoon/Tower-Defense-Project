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
        bool isSelected, isSecondSelected, isFirstSelected;
        Button point1, point2, point3, point4;
        Texture2D pressed, unpressed;

        #region Properties
        public bool Selected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        public bool SecondSelected
        {
            get { return isSecondSelected; }
            set { isSecondSelected = value; }
        }

        public bool FirstSelected
        {
            get { return isFirstSelected; }
            set { isFirstSelected = value; }
        }

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
        #endregion

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

            point1 = new Button(Points[0], 24, 1, Main.CurrentMouse, unpressed, pressed, true, Main.Scale.X, Main.Scale.Y);
            point2 = new Button(Points[1], 24, 2, Main.CurrentMouse, unpressed, pressed, true, Main.Scale.X, Main.Scale.Y);
            point3 = new Button(Points[2], 24, 3, Main.CurrentMouse, unpressed, pressed, true, Main.Scale.X, Main.Scale.Y);
            point4 = new Button(Points[3], 24, 4, Main.CurrentMouse, unpressed, pressed, true, Main.Scale.X, Main.Scale.Y);
        }

        public void Update()
        {
            if (Selected)
            {
                point1.Update(Main.CurrentMouse);
                point4.Update(Main.CurrentMouse);
            }
            if (Selected || SecondSelected)
            {
                point3.Update(Main.CurrentMouse); 
            }
            if (Selected || FirstSelected)
            {
                point2.Update(Main.CurrentMouse);
            }

            if (point1.LeftHeld && Selected)
            {
                Button1 = new Vector2(Main.CurrentMouse.X * Main.Scale.X, Main.CurrentMouse.Y * Main.Scale.Y);
                point2.Clickable = false;
                point3.Clickable = false;
                point4.Clickable = false;
            }
            else if (point2.LeftHeld && (Selected || FirstSelected))
            {
                Button2 = new Vector2(Main.CurrentMouse.X * Main.Scale.X, Main.CurrentMouse.Y * Main.Scale.Y);
                point1.Clickable = false;
                point3.Clickable = false;
                point4.Clickable = false;
            }
            else if (point3.LeftHeld && (Selected || SecondSelected))
            {
                Button3 = new Vector2(Main.CurrentMouse.X * Main.Scale.X, Main.CurrentMouse.Y * Main.Scale.Y);
                point1.Clickable = false;
                point2.Clickable = false;
                point4.Clickable = false;
            }
            else if (point4.LeftHeld && Selected)
            {
                Button4 = new Vector2(Main.CurrentMouse.X * Main.Scale.X, Main.CurrentMouse.Y * Main.Scale.Y);
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

            if (Selected)
            {
                spriteBatch.Draw(point1.Texture, point1.Position, Color.White);
                spriteBatch.Draw(point4.Texture, point4.Position, Color.White);
            }
            if (Selected || SecondSelected)
            {
                spriteBatch.Draw(point3.Texture, point3.Position, Color.White); 
            }
            if (Selected || FirstSelected)
            {
                spriteBatch.Draw(point2.Texture, point2.Position, Color.White);
            }
        }
    }
}