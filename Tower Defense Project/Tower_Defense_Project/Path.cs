using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Duality;

namespace Tower_Defense_Project
{
    public class Path
    {
        public float[] lengths;
        private Texture2D tex;
        public Vector2[] directions;

        private List<Curve> curves = new List<Curve>();
        private List<Vector2> points = new List<Vector2>();

        public List<Curve> Curves
        {
            get { return curves; }
            set { curves = value; }
        }

        public List<Vector2> Points
        {
            get { return points; }
            set { points = value; }
        }

        public Path()
        {
            LoadContent();
        }

        private void LoadContent()
        {
            tex = Main.GameContent.Load<Texture2D>("Textures/help");
        }

        public void AddCurve(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            List<Vector2> points = new List<Vector2>() { p1, p2, p3, p4 };
            Curves.Add(new Curve(points));
        }

        public void Build()
        {
            Points.Clear();

            foreach (Curve i in Curves)
            {
                i.Build();
                Points.AddRange(i.Reference);
            }

            lengths = new float[Points.Count - 1];
            directions = new Vector2[Points.Count - 1];
            for (int i = 0; i < Points.Count - 1; i++)
            {
                directions[i] = Points[i + 1] - Points[i];
                lengths[i] = directions[i].Length();
                directions[i].Normalize();
            }
        }

        public void Clear()
        {
            Curves.Clear();
            Points.Clear();
        }

        public bool Intersects(RectangleF rect)
        {
            try
            {
                bool intersects = false;
                foreach (Vector2 i in Points)
                {
                    if (!intersects)
                    {
                        intersects = rect.Contains(i);
                    }
                }
                return intersects;
            }
            catch { return false; }
        }

        public bool Intersects(Ellipse ellipse)
        {
            try
            {
                bool intersects = false;
                foreach (Vector2 i in Points)
                {
                    if (!intersects)
                    {
                        intersects = ellipse.Contains(i);
                    }
                }
                return intersects;
            }
            catch { return false; }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Curve i in Curves)
            {
                i.Draw(spriteBatch, tex);
            }
        }
    }

    class PathException : Exception
    {
        public PathException()
            : base()
        { }

        public PathException(string message)
            : base(message)
        { }

        public PathException(string format, params object[] args)
            : base(string.Format(format, args))
        { }

        public PathException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public PathException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException)
        { }
    }
}