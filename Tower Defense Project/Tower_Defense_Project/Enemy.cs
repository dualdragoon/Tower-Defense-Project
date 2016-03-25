using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Duality.Graphics;

namespace Tower_Defense_Project
{
    enum EnemyType
    {
        Peon = 101,
        Scout = 102,
        Brute = 103,
    }

    class Enemy
    {
        private Animation moveAnimation;
        private AnimationPlayer sprite;
        public float stagePos;
        private float speed = 0, seconds = 0;
        private int stageIndex, frameWidth, health;
        private Level level;
        private string spriteSet;
        private uint worth;
        public Vector2 position;

        public Level Level
        {
            get { return level; }
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public uint Worth
        {
            get { return worth; }
        }

        /// <summary>
        /// Constructs a new Enemy.
        /// </summary>
        public Enemy(Level level, EnemyType type)
        {
            this.level = level;

            spriteSet = Level.EnemyStats[(int)type][0];
            frameWidth = int.Parse(Level.EnemyStats[(int)type][1]);
            speed = float.Parse(Level.EnemyStats[(int)type][2]);
            seconds = float.Parse(Level.EnemyStats[(int)type][3]);
            health = int.Parse(Level.EnemyStats[(int)type][4]);
            worth = uint.Parse(Level.EnemyStats[(int)type][5]);

            LoadContent();
        }

        /// <summary>
        /// Loads a particular enemy sprite sheet and sounds.
        /// </summary>
        public void LoadContent()
        {
            moveAnimation = new Animation(Main.GameContent.Load<Texture2D>(string.Format("Enemies/{0}", spriteSet)), 0.1f, true, frameWidth);

            sprite.PlayAnimation(moveAnimation);
        }

        public void Update(GameTime gameTime)
        {
            if (stageIndex != Level.Path.points.Count - 1)
            {
                stagePos += speed * seconds;
                while (stagePos > Level.Path.lengths[stageIndex])
                {
                    stagePos -= Level.Path.lengths[stageIndex];
                    stageIndex++;
                    if (stageIndex == Level.Path.points.Count - 1)
                    {
                        position = Level.Path.points[stageIndex];
                        return;
                    }
                }
                position = Level.Path.points[stageIndex] + Level.Path.directions[stageIndex] * stagePos;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sprite.PlayAnimation(moveAnimation);

            sprite.Draw(gameTime, spriteBatch, position + new Vector2(0, 5), SpriteEffects.None);
        }
    }
}