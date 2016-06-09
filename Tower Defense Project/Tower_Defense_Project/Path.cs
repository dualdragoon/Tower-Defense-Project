using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using SharpDX;
using SharpDX.Serialization;
using Duality;

namespace Tower_Defense_Project
{
    [Serializable]
    class Path : IDataSerializable
    {
        public List<Vector2> points = new List<Vector2>();
        public float[] lengths;
        public Vector2[] directions;

        public List<RectangleF> pathSet = new List<RectangleF>();

        public Path()
        {
            
        }

        public Path(List<Vector2> points)
        {
            this.points = points;
        }

        private void SinglePath()
        {
            for (int i = 0; i < pathSet.Count - 2; i++)
            {
                try
                {
                    if (!pathSet[i].Intersects(pathSet[i + 1]))
                        throw new PathException("PathException: Path is not connected.");
                }
                catch (PathException ex)
                {
                    Console.WriteLine(ex.Message);
                    ErrorHandler.RecordError(3, 103, "Solution: replace levels file with newest version.", ex.Message);
                }
            }
        }

        public void Build(bool fromFile)
        {
            if (fromFile)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    points[i] = new Vector2(Main.Graphics.PreferredBackBufferWidth, Main.Graphics.PreferredBackBufferHeight) * points[i];
                }

                for (int i = 0; i < pathSet.Count; i++)
                {
                    pathSet[i] = new RectangleF(Main.Graphics.PreferredBackBufferWidth * pathSet[i].X, Main.Graphics.PreferredBackBufferHeight * pathSet[i].Y, Main.Graphics.PreferredBackBufferWidth * pathSet[i].Width, Main.Graphics.PreferredBackBufferHeight * pathSet[i].Height);
                } 
            }

            lengths = new float[points.Count - 1];
            directions = new Vector2[points.Count - 1];
            for (int i = 0; i < points.Count - 1; i++)
            {
                directions[i] = points[i + 1] - points[i];
                lengths[i] = directions[i].Length();
                directions[i].Normalize();
            }
        }

        public void Clear()
        {
            points.Clear();
            pathSet.Clear();
        }

        public bool Intersects(RectangleF rect)
        {
            try
            {
                bool intersects = false;
                foreach (RectangleF pathRectangle in pathSet)
                {
                    if (!intersects)
                    {
                        intersects = rect.Left < pathRectangle.Right && pathRectangle.Left < rect.Right && rect.Top < pathRectangle.Bottom && pathRectangle.Top < rect.Bottom;
                    }
                }
                return intersects;
            }
            catch { return false; }
        }

        public void Serialize(BinarySerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    class PathException : Exception
    {
        public PathException()
            : base() { }

        public PathException(string message)
            : base(message) { }

        public PathException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public PathException(string message, Exception innerException)
            : base(message, innerException) { }

        public PathException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }
}