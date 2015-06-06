using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Duality;

namespace Tower_Defense_Project
{
    [Serializable]
    class Path
    {
        public List<Vector2> points = new List<Vector2>();
        public float[] lengths;
        public Vector2[] directions;

        public List<FloatingRectangle> pathSet = new List<FloatingRectangle>();

        public Path()
        {
            
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

        public void Build()
        {
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

        public bool Intersects(FloatingRectangle rect)
        {
            bool intersects = false;
            foreach (FloatingRectangle pathRectangle in pathSet)
            {
                if (!intersects)
                {
                    intersects = rect.Left < pathRectangle.Right && pathRectangle.Left < rect.Right && rect.Top < pathRectangle.Bottom && pathRectangle.Top < rect.Bottom; 
                }
            }
            return intersects;
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