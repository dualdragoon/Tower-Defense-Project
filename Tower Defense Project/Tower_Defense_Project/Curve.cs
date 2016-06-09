using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Tower_Defense_Project
{
    class Curve
    {
        private List<Vector2> points, reference;

        public List<Vector2> Points
        {
            get { return points; }
            set { points = value; }
        }

        public Vector2 Start
        {
            get { return points[1]; }
            set { points[1] = value; }
        }

        public Vector2 End
        {
            get { return points[2]; }
            set { points[2] = value; }
        }

        public List<Vector2> Reference
        {
            get { return reference; }
            set { reference = value; }
        }

        public Curve()
        {
            points = new List<Vector2>() { Vector2.Zero, new Vector2(50, 25), new Vector2(75, 100), new Vector2(150, 100) };
            Build();
        }

        public Curve(List<Vector2> points)
        {
            this.points = points;
            Build();
        }

        public void Build()
        {
            for (float i = 0; i < 1; i = i + .1f)
            {
                reference.Add(Vector2.CatmullRom(points[0], points[1], points[2], points[3], i));
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D tex)
        {
            for (float i = 0; i < 1; i = i + .01f)
            {
                Vector2 t = Vector2.CatmullRom(points[0], points[1], points[2], points[3], i);
                RectangleF rect = new RectangleF(t.X, t.Y, 2, 2);
                spriteBatch.Draw(tex, rect, Color.Black);
            }
        }
    }
}
