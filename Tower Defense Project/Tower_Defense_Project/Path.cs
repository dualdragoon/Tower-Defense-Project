using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Duality;

namespace Tower_Defense_Project
{
    class Path
    {
        public List<FloatingRectangle> pathSet = new List<FloatingRectangle>();

        public Path(Vector2[] locations, Vector2[] w_h)
        {
            try
            {
                if (locations.Length != w_h.Length)
                    throw new PathException("A path array is longer than the other.");

                for (int i = 0; i < locations.Length; i++)
                {
                    pathSet.Add(new FloatingRectangle(locations[i].X, locations[i].Y, w_h[i].X, w_h[i].Y));
                }

                SinglePath();
            }
            catch (PathException ex)
            {
                //Todo: add exception record.
            }
        }

        private void SinglePath()
        {
            for (int i = 0; i < pathSet.Count - 2; i++)
            {
                try
                {
                    if (!pathSet[i].Intersects(pathSet[i + 1]))
                        throw new PathException("Path is not connected.");
                }
                catch (PathException ex)
                {
                    Console.WriteLine(ex.Message);
                    ErrorHandler.RecordError(3, 103, "This is my fault.", ex.Message);
                }
            }
        }

        public void Clear()
        {
            pathSet.Clear();
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