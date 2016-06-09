using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Duality.Interaction;

namespace Tower_Defense_Project
{
    class ExperimentalPath
    {
        private List<Button> controls = new List<Button>();
        private List<Curve> curves = new List<Curve>();
        private Texture2D tex, pressed, unPressed;

        public List<Curve> Curves
        {
            get { return curves; }
            set { curves = value; }
        }

        public ExperimentalPath()
        {
            LoadContent();
        }

        private void LoadContent()
        {
            tex = Main.GameContent.Load<Texture2D>("Textures/Null");
            pressed = Main.GameContent.Load<Texture2D>("Textures/Point Pressed");
            unPressed = Main.GameContent.Load<Texture2D>("Textures/Point");
        }

        public void AddCurve()
        {
            if (curves.Count > 0)
            {
                Vector2 i = curves.Last().End;
                List<Vector2> points = new List<Vector2>() { i - new Vector2(50, 25), i, i + new Vector2(25, 75), i + new Vector2(100, 75) };
                curves.Add(new Curve(points));
            }
            else curves.Add(new Curve());
            Curve o = curves.Last();
            for (int l = 0; l < o.Points.Count; l++)
            {
                controls.Add(new Button(o.Points[l], 16, controls.Count + 1, Main.CurrentMouse, unPressed, pressed, true, Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight));
            }
        }

        public void Update()
        {
            foreach (Button i in controls)
            {
                i.Update(Main.CurrentMouse);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Button i in controls)
            {
                spriteBatch.Draw(i.Texture, i.Position, Color.White);
            }

            foreach (Curve i in curves)
            {
                i.Draw(spriteBatch, tex);
            }
        }
    }
}