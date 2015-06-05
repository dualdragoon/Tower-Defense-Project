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
        peon,
        scout,
        brute,
    }

    class Enemy
    {
        public Vector2 position;
        public float stagePos;
        int stageIndex, frameWidth;
        float speed = 0, seconds = 0;

        private Animation moveAnimation;
        private AnimationPlayer sprite;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Loads a particular enemy sprite sheet and sounds.
        /// </summary>
        public void LoadContent(EnemyType type)
        {
            string spriteSet = "";

            // Load animations.
            switch (type)
            {
                case EnemyType.peon:
                    frameWidth = 10;
                    spriteSet = "Peon";
                    speed = .5f;
                    break;

                case EnemyType.scout:
                    frameWidth = 20;
                    spriteSet = "Scout";
                    speed = .9f;
                    break;

                case EnemyType.brute:
                    frameWidth = 30;
                    spriteSet = "Brute";
                    speed = .2f;
                    break;

                default:
                    break;
            }

            spriteSet = "Sprites/" + spriteSet;

            moveAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet), 0.1f, true, frameWidth);
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
            seconds += (float)gameTime.ElapsedGameTime.TotalSeconds;

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