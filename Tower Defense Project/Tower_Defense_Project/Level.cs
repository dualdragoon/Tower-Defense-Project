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
using Duality.Encrypting;

namespace Tower_Defense_Project
{
    class Level
    {
        private bool keyPressed, keyDidSomething;
        private float timer = 0, minTimer = 1f;
        private MouseState mouse;
        private Texture2D tex;
        private StreamReader reader;

        public List<Enemy> enemies = new List<Enemy>();
        public List<Tower> towers = new List<Tower>();
        public List<Projectile> projectiles = new List<Projectile>();

        public Path Path
        {
            get { return path; }
        }
        Path path;

        public SpriteFont Font
        {
            get { return font; }
        }
        SpriteFont font;

        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        private void LoadContent()
        {
            tex = Content.Load<Texture2D>(@"Textures/SQUARE");
            font = Content.Load<SpriteFont>(@"Fonts/Font");
        }

        public void LoadLevel(int levelIndex)
        {
            reader = new StreamReader(@"Content/Levels/Level" + levelIndex + ".path");
            path = Serialization.DeserializeFromString<Path>(StringCipher.Decrypt(reader.ReadLine(), "temp2"));
            path.Build();
        }

        public Level(IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");

            LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            mouse = Mouse.GetState();

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer > minTimer)
            {
                enemies.Add(new Enemy(this, EnemyType.Scout));
                timer = 0;
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Update(gameTime);

                if (enemies[i].position == path.points[path.points.Count - 1] || enemies[i].Health == 0)
                {
                    enemies.Remove(enemies[i]);
                }
            }

            foreach (Tower tower in towers)
            {
                tower.Update(gameTime, mouse);
            }

            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Update(gameTime);

                if (projectiles[i].StageIndex == 1)
                {
                    projectiles[i].target.Health -= projectiles[i].damage;
                    projectiles.Remove(projectiles[i]);
                }
            }

            if (towers.Count > 0 && towers[towers.Count-1].isPlaced)
            {
                keyPressed = false;
            }

            keyDidSomething = keyPressed && keyDidSomething;

            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                keyPressed = true;
                if (!keyDidSomething)
                {
                    towers.Add(new Tower(this, TowerType.Small, mouse));
                    keyDidSomething = true;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                keyPressed = true;
                if (!keyDidSomething)
                {
                    towers.Add(new Tower(this, TowerType.Medium, mouse));
                    keyDidSomething = true;
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

            foreach (Tower tower in towers)
            {
                tower.Draw(spritebatch);
            }

            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spritebatch);
            }

            spritebatch.DrawString(Font, keyDidSomething.ToString(), new Vector2(200, 5), Color.White);
            spritebatch.DrawString(Font, keyPressed.ToString(), new Vector2(200, 200), Color.White);
        }
    }
}