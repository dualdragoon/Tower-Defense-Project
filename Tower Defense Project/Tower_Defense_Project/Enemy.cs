using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public Vector2 position;
        public float stagePos;
        int stageIndex, frameWidth, health;
        float speed = 0, seconds = 0;

        private Animation moveAnimation;
        private AnimationPlayer sprite;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        /// <summary>
        /// Loads a particular enemy sprite sheet and sounds.
        /// </summary>
        public void LoadContent(EnemyType type)
        {
            // Load animations.
            switch (type)
            {
                case EnemyType.Peon:
                    frameWidth = 10;
                    moveAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Peon"), 0.1f, true, frameWidth);
                    speed = .5f;
                    seconds = 1f;
                    Health = 1;
                    break;

                case EnemyType.Scout:
                    frameWidth = 20;
                    moveAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Scout"), 0.1f, true, frameWidth);
                    speed = .5f;
                    seconds = 2f;
                    Health = 6;
                    break;

                case EnemyType.Brute:
                    frameWidth = 30;
                    moveAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Brute"), 0.1f, true, frameWidth);
                    speed = .2f;
                    seconds = 1f;
                    Health = 10;
                    break;

                default:
                    break;
            }

            sprite.PlayAnimation(moveAnimation);

        }

        /// <summary>
        /// Constructs a new Enemy.
        /// </summary>
        public Enemy(Level level, EnemyType type)
        {
            this.level = level;

            LoadContent(type);
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