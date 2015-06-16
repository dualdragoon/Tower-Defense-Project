using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private bool keyPressed, keyDidSomething, pause = false;
        private float timer = 0, minTimer = 1f, escapeTimer = 0, minEscapeTimer = .05f;
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

            escapeTimer = Keyboard.GetState().IsKeyDown(Keys.Escape) ? escapeTimer + (float)gameTime.ElapsedGameTime.TotalSeconds : escapeTimer;

            if (Keyboard.GetState().IsKeyUp(Keys.Escape) && escapeTimer > minEscapeTimer)
            {
                escapeTimer = 0;
                pause = !pause;
            }

            if (!pause)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (timer > minTimer)
                {
                    enemies.Add(new Enemy(this, (EnemyType)101));
                    timer = 0;
                }

                try
                {
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.Update(gameTime);

                        if (enemy.position == path.points[path.points.Count - 1] || enemy.Health <= 0)
                        {
                            enemies.Remove(enemy);
                        }
                    }

                    foreach (Tower tower in towers)
                    {
                        tower.Update(gameTime, mouse);
                    }

                    foreach (Projectile projectile in projectiles)
                    {
                        projectile.Update(gameTime);

                        if (projectile.StageIndex == 1)
                        {
                            projectile.target.Health -= projectile.damage;
                            projectiles.Remove(projectile);
                        }
                        else if (!projectile.Origin.range.Contains(projectile.Position))
                        {
                            projectiles.Remove(projectile);
                        }
                    }
                }
                catch
                { }

                Input();

                try
                {
                    Console.WriteLine(enemies[0].stagePos);
                }
                catch
                { }
            }
        }

        private void Input()
        {
            if (towers.Count > 0 && towers[towers.Count - 1].isPlaced)
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
        }

        public void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            foreach (FloatingRectangle i in Path.pathSet)
            {
                spritebatch.Draw(tex, i.Draw, Color.White);
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

            spritebatch.DrawString(Font, keyDidSomething.ToString(), new Vector2(300, 5), Color.White);
            spritebatch.DrawString(Font, keyPressed.ToString(), new Vector2(300, 200), Color.White);
        }
    }
}