using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Duality;

namespace Tower_Defense_Project
{
    class Level
    {
        private Texture2D tex;

        public Path Path
        {
            get { return path; }
        }
        Path path;

        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        private void LoadContent()
        {
            tex = Content.Load<Texture2D>(@"Textures/SQUARE");
        }

        private void LoadLevel()
        {
            Vector2[] locations = new Vector2[] { Vector2.Zero, new Vector2(100, 0), new Vector2(100, 100) };
            Vector2[] w_h = new Vector2[] { new Vector2(100, 10), new Vector2(10, 110), new Vector2(100, 10) };

            path = new Path(locations, w_h);
        }

        public Level(IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");

            LoadContent();

            LoadLevel();
        }

        public void Draw(SpriteBatch spritebatch)
        {
            for (int i = 0; i < path.pathSet.Count; i++)
			{
                spritebatch.Draw(tex, new Rectangle((int)path.pathSet[i].X, (int)path.pathSet[i].Y, (int)path.pathSet[i].Width, (int)path.pathSet[i].Height), Color.White); 
			}
        }
    }
}
