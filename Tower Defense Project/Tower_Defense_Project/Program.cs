using System;
using System.IO;
using Microsoft.Xna.Framework;
using Duality;
using Duality.Records;

namespace Tower_Defense_Project
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //Uncomment for making new level serializations.
            /*Path path = new Path();

            path.points.Add(new Vector2(0, 5));
            path.points.Add(new Vector2(100, 5));
            path.points.Add(new Vector2(103.5f, 6.5f));
            path.points.Add(new Vector2(105, 10));
            path.points.Add(new Vector2(105, 100));
            path.points.Add(new Vector2(106.5f, 103.5f));
            path.points.Add(new Vector2(110, 105));
            path.points.Add(new Vector2(200, 105));

            path.pathSet.Add(new FloatingRectangle(0, 0, 110, 10));
            path.pathSet.Add(new FloatingRectangle(100, 0, 10, 110));
            path.pathSet.Add(new FloatingRectangle(100, 100, 100, 10));

            StreamWriter write = new StreamWriter("Level1.path");
            write.Write(Serialization.SerializeToString<Path>(path));
            write.Close();*/

            using (Main game = new Main())
            {
                game.Run();
            }
        }
    }
#endif
}