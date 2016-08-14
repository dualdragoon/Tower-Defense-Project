using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Tower_Defense_Project
{
    enum ProjectileType
    { 
        Small = 101, 
        Medium = 102, 
        Large = 103,
    }

    class Projectile
    {
        public int damage;
        int stageIndex;
        float stagePos, speed = 0, seconds = 0;
        public Enemy target;
        ProjectileType type;
        Texture2D tex;
        Tower origin;
        Vector2 position;

        private float[] lengths;
        private Vector2[] directions;
        private List<Vector2> Points = new List<Vector2>();

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

        public Projectile(Tower origin, Vector2 position, Enemy target, ProjectileType type, Level level)
        {
            this.level = level;
            this.position = position;
            this.target = target;
            this.type = type;
            this.origin = origin;

            Build();

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

        private void LoadContent()
        {
            switch (type)
            {
                case ProjectileType.Small:
                    tex = Main.GameContent.Load<Texture2D>(@"Projectiles/Small Projectile");
                    break;
                
                case ProjectileType.Medium:
                    tex = Main.GameContent.Load<Texture2D>(@"Projectiles/Medium Projectile");
                    break;

                case ProjectileType.Large:
                    tex = Main.GameContent.Load<Texture2D>(@"Projectiles/Large Projectile");
                    break;

                default:
                    break;
            }
        }

        private void Build()
        {
            Points.Clear();

            Points.Add(position);
            Points.Add(target.position);

            lengths = new float[Points.Count - 1];
            directions = new Vector2[Points.Count - 1];
            for (int i = 0; i < Points.Count - 1; i++)
            {
                directions[i] = Points[i + 1] - Points[i];
                lengths[i] = directions[i].Length();
                directions[i].Normalize();
            }
        }

        public void Update(GameTime gameTime)
        {
            Build();

            if (stageIndex != Points.Count - 1)
            {
                stagePos += speed * seconds;
                while (stagePos > lengths[stageIndex])
                {
                    stagePos -= lengths[stageIndex];
                    stageIndex++;
                    if (stageIndex == Points.Count - 1)
                    {
                        position = Points[stageIndex];
                        return;
                    }
                }
                position = Points[stageIndex] + directions[stageIndex] * stagePos;
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