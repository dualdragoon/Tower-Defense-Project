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
        protected List<Vector2> mid = new List<Vector2>(),
            side1 = new List<Vector2>(),
            side2 = new List<Vector2>();

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

            BuildPoints();

            lengths = new float[Points.Count - 1];
            directions = new Vector2[Points.Count - 1];
            for (int i = 0; i < Points.Count - 1; i++)
            {
                directions[i] = Points[i + 1] - Points[i];
                lengths[i] = directions[i].Length();
                directions[i].Normalize();
            }
        }

        private void BuildPoints()
        {
            for (int i = 0; i < curves.Count; i++)
            {
                for (float j = 0; j < 1; j += .001f)
                {
                    Vector2 temp = Vector2.CatmullRom(curves[i].Points[0], curves[i].Points[1], curves[i].Points[2], curves[i].Points[3], j);
                    if (mid.Count != 0)
                    {
                        if (temp != mid[mid.Count - 1])
                        {
                            mid.Add(temp);
                        }
                        else Console.WriteLine(temp == mid[mid.Count - 1]);
                    }
                    else mid.Add(temp);
                }
            }

            for (int i = 0; i < mid.Count; i++)
            {
                if (i == 0)
                {
                    Vector2 slope = mid[i] - mid[i + 1];
                    double x = mid[i].X + (12 * Math.Cos(Math.Atan(-(slope.X / slope.Y)))),
                        y = mid[i].Y + (12 * Math.Sin(Math.Atan(-(slope.X / slope.Y))));
                    side1.Add(new Vector2((float)x, (float)y));

                    x = mid[i].X - (12 * Math.Cos(Math.Atan(-(slope.X / slope.Y))));
                    y = mid[i].Y - (12 * Math.Sin(Math.Atan(-(slope.X / slope.Y))));
                    side2.Add(new Vector2((float)x, (float)y));
                }
                else if (i < mid.Count - 1)
                {
                    Vector2 slope1 = mid[i] - mid[i - 1],
                        slope2 = mid[i + 1] - mid[i];

                    Vector2 slope = (slope1 + slope2) / 2;

                    double x = mid[i].X + (12 * Math.Cos(Math.Atan(-(slope.X / slope.Y)))),
                        y = mid[i].Y + (12 * Math.Sin(Math.Atan(-(slope.X / slope.Y))));
                    side1.Add(new Vector2((float)x, (float)y));

                    x = mid[i].X - (12 * Math.Cos(Math.Atan(-(slope.X / slope.Y))));
                    y = mid[i].Y - (12 * Math.Sin(Math.Atan(-(slope.X / slope.Y))));
                    side2.Add(new Vector2((float)x, (float)y));
                }
                else
                {
                    Vector2 slope = mid[i] - mid[i - 1];

                    double x = mid[i].X + (12 * Math.Cos(Math.Atan(-(slope.X / slope.Y)))),
                        y = mid[i].Y + (12 * Math.Sin(Math.Atan(-(slope.X / slope.Y))));
                    side1.Add(new Vector2((float)x, (float)y));

                    x = mid[i].X - (12 * Math.Cos(Math.Atan(-(slope.X / slope.Y))));
                    y = mid[i].Y - (12 * Math.Sin(Math.Atan(-(slope.X / slope.Y))));
                    side2.Add(new Vector2((float)x, (float)y));
                }
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
            for (int i = 0; i < mid.Count; i++)
            {
                Vector2 t = side1[i];
                RectangleF rect = new RectangleF(t.X, t.Y, 2, 2);
                spriteBatch.Draw(tex, rect, Color.Black);

                t = side2[i];
                rect = new RectangleF(t.X, t.Y, 2, 2);
                spriteBatch.Draw(tex, rect, Color.Black);
            }

            foreach (Curve i in curves)
            {
                i.Draw(spriteBatch, tex);
            }
        }
    }
}