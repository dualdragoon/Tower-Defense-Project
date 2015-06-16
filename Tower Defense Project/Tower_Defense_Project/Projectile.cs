using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tower_Defense_Project
{
    enum ProjectileType { Small, Medium, Large, }

    class Projectile
    {
        public Level Level
        {
            get { return level; }
        }
        Level level;

        public int StageIndex
        {
            get { return stageIndex; }
        }

        public Tower Origin
        {
            get { return origin; }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        public int damage;
        int stageIndex;
        float stagePos, speed = 0, seconds = 0;
        public Enemy target;
        Path path;
        ProjectileType type;
        Texture2D tex;
        Tower origin;
        Vector2 position;

        private void LoadContent()
        {
            switch (type)
            {
                case ProjectileType.Small:
                    tex = Level.Content.Load<Texture2D>(@"Projectiles/Small Projectile");
                    break;
                
                case ProjectileType.Medium:
                    tex = Level.Content.Load<Texture2D>(@"Projectiles/Medium Projectile");
                    break;

                case ProjectileType.Large:
                    tex = Level.Content.Load<Texture2D>(@"Projectiles/Large Projectile");
                    break;

                default:
                    break;
            }
        }

        public Projectile(Tower origin, Vector2 position, Enemy target, ProjectileType type, Level level)
        {
            this.level = level;
            this.position = position;
            this.target = target;
            this.type = type;
            this.origin = origin;

            path = new Path();
            path.points.Add(position);
            path.points.Add(target.position);
            path.Build();

            switch (type)
            {
                case ProjectileType.Small:
                    speed = .1f;
                    seconds = 10f;
                    damage = 1;
                    break;

                case ProjectileType.Medium:
                    speed = .1f;
                    seconds = .6f;
                    damage = 2;
                    break;

                case ProjectileType.Large:
                    speed = .1f;
                    seconds = 1.5f;
                    damage = 5;
                    break;

                default:
                    break;
            }

            LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            path.Clear();
            path.points.Add(position);
            path.points.Add(target.position);
            path.Build();

            if (stageIndex != path.points.Count - 1)
            {
                stagePos += speed * seconds;
                while (stagePos > path.lengths[stageIndex])
                {
                    stagePos -= path.lengths[stageIndex];
                    stageIndex++;
                    if (stageIndex == path.points.Count - 1)
                    {
                        position = path.points[stageIndex];
                        return;
                    }
                }
                position = path.points[stageIndex] + path.directions[stageIndex] * stagePos;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            try
            {
                spriteBatch.Draw(tex, position, Color.White);
            }
            catch
            { }
        }
    }
}
