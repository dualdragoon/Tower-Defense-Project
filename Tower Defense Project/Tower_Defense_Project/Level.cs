using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Duality;
using Duality.Records;

namespace Tower_Defense_Project
{
    class Level
    {
        private float timer = 0, minTimer = 1f;
        private Texture2D tex;
        private StreamReader reader;

        private List<Enemy> enemies = new List<Enemy>();

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

        public void LoadLevel(int levelIndex)
        {
            reader = new StreamReader(@"Content/Levels/Level" + levelIndex + ".path");
            path = Serialization.DeserializeFromString<Path>(reader.ReadLine());
            path.Build();
        }

        public Level(IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");

            LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer > minTimer)
            {
                enemies.Add(new Enemy(this, EnemyType.peon));
                timer = 0;
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Update(gameTime);

                if (enemies[i].position.X == 200)
                {
                    enemies.Remove(enemies[i]);
                }
            }

            try
            {
                Console.WriteLine(enemies[0].stagePos);
            }
            catch
            { }
        }

        public void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            for (int i = 0; i < Path.pathSet.Count(); i++)
            {
                spritebatch.Draw(tex, Path.pathSet[i].Draw, Color.White);
            }

            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(gameTime, spritebatch);
            }
        }
    }
}