using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Duality;

namespace Tower_Defense_Project
{
    enum TowerType { GL, RL, BLL, };

    class Tower
    {
        public Level Level
        {
            get { return level; }
        }
        private Level level;

        public uint Cost
        {
            get { return cost; }
        }
        private uint cost;

        public bool isPlaced = false;
        private bool isSelected;
        private float attackTimer = 0, minAttackTimer;
        public Circle range;
        private Color rangeColor = Color.Gray;
        public FloatingRectangle collision;
        private ProjectileType projectileType;
        private string spriteSet;
        private Texture2D tex, rangeTex;
        public TowerType type;

        private void LoadContent()
        {
            tex = Level.Content.Load<Texture2D>(@"Towers/" + spriteSet);
            rangeTex = Level.Content.Load<Texture2D>(@"Towers/" + spriteSet + " Range");
        }

        public Tower(Level level, TowerType type, MouseState mouse)
        {
            this.level = level;
            this.type = type;

            switch (type)
            {
                case TowerType.GL:
                    collision = new FloatingRectangle(mouse.X * Level.Graphics.PreferredBackBufferWidth, mouse.Y * Level.Graphics.PreferredBackBufferHeight, 32, 32);
                    range = new Circle(new Vector2(mouse.X * Level.Graphics.PreferredBackBufferWidth + (collision.Width / 2), mouse.Y * Level.Graphics.PreferredBackBufferHeight + (collision.Height / 2)), 100);
                    minAttackTimer = 1f;
                    projectileType = ProjectileType.Small;
                    spriteSet = "GL";
                    cost = 500;
                    break;

                case TowerType.RL:
                    collision = new FloatingRectangle(mouse.X * Level.Graphics.PreferredBackBufferWidth, mouse.Y * Level.Graphics.PreferredBackBufferHeight, 32, 32);
                    range = new Circle(new Vector2(mouse.X * Level.Graphics.PreferredBackBufferWidth + (collision.Width / 2), mouse.Y * Level.Graphics.PreferredBackBufferHeight + (collision.Height / 2)), 120);
                    minAttackTimer = .3f;
                    projectileType = ProjectileType.Medium;
                    spriteSet = "RL";
                    break;

                case TowerType.BLL:
                    collision = new FloatingRectangle(mouse.X * Level.Graphics.PreferredBackBufferWidth, mouse.Y * Level.Graphics.PreferredBackBufferHeight, 30, 30);
                    range = new Circle(new Vector2(mouse.X * Level.Graphics.PreferredBackBufferWidth + (collision.Width / 2), mouse.Y * Level.Graphics.PreferredBackBufferHeight + (collision.Height / 2)), 60);
                    projectileType = ProjectileType.Large;
                    spriteSet = "BLL";
                    break;

                default:
                    break;
            }

            LoadContent();
        }

        public void Update(GameTime gameTime, MouseState mouse)
        {
            if (!isPlaced)
            {
                isPlaced = Placed(mouse);
            }
            else
            {
                attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (attackTimer > minAttackTimer)
                {
                    Fire();
                }

                if (mouse.RightButton.Pressed)
                {
                    if (collision.Contains(new Vector2(mouse.X * Level.Graphics.PreferredBackBufferWidth, mouse.Y * Level.Graphics.PreferredBackBufferHeight)))
                    {
                        isSelected = true;
                    }
                    else
                    {
                        isSelected = false;
                    }
                }
            }
        }

        private void Fire()
        {
            attackTimer = 0f;
            for (int i = 0; i < Level.enemies.Count; i++)
            {
                if (range.Contains(Level.enemies[i].position))
                {
                    Level.projectiles.Add(new Projectile(this, collision.Location + new Vector2(collision.Width / 2), Level.enemies[i], projectileType, Level));
                    break;
                }
            }
        }

        private bool Placed(MouseState mouse)
        {
            Main.IsCustomMouseVisible = false;
            bool placed = false;
            if (mouse.X * Level.Graphics.PreferredBackBufferWidth >= 0 && mouse.X * Level.Graphics.PreferredBackBufferWidth <= Level.Graphics.PreferredBackBufferWidth && mouse.Y * Level.Graphics.PreferredBackBufferHeight >= 0 && mouse.Y * Level.Graphics.PreferredBackBufferHeight <= Level.Graphics.PreferredBackBufferHeight)
            {
                collision.X = mouse.X * Level.Graphics.PreferredBackBufferWidth;
                collision.Y = mouse.Y * Level.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(mouse.X * Level.Graphics.PreferredBackBufferWidth + (collision.Width / 2), mouse.Y * Level.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                if (CanPlace())
                {
                    rangeColor = Color.Gray;
                    if (mouse.LeftButton.Pressed)
                    {
                        placed = true;
                        isSelected = false;
                        Main.IsCustomMouseVisible = true;
                    }
                }
                else
                {
                    rangeColor = Color.Red;
                    placed = false;
                }
                return placed; 
            }
            #region OutOfBoundsHandling
            else if (mouse.X * Level.Graphics.PreferredBackBufferWidth <= 0 && mouse.Y * Level.Graphics.PreferredBackBufferHeight >= 0 && mouse.Y * Level.Graphics.PreferredBackBufferHeight <= Level.Graphics.PreferredBackBufferHeight)
            {
                collision.X = 0;
                collision.Y = mouse.Y * Level.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(0 + (collision.Width / 2), mouse.Y * Level.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Level.Graphics.PreferredBackBufferWidth >= Level.Graphics.PreferredBackBufferWidth && mouse.Y * Level.Graphics.PreferredBackBufferHeight >= 0 && mouse.Y * Level.Graphics.PreferredBackBufferHeight <= Level.Graphics.PreferredBackBufferHeight)
            {
                collision.X = Level.Graphics.PreferredBackBufferWidth;
                collision.Y = mouse.Y * Level.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(Level.Graphics.PreferredBackBufferWidth + (collision.Width / 2), mouse.Y * Level.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                return placed;
            }
            else if (mouse.Y * Level.Graphics.PreferredBackBufferHeight <= 0 && mouse.X * Level.Graphics.PreferredBackBufferWidth >= 0 && mouse.X * Level.Graphics.PreferredBackBufferWidth <= Level.Graphics.PreferredBackBufferWidth)
            {
                collision.X = mouse.X * Level.Graphics.PreferredBackBufferWidth;
                collision.Y = 0;
                range.Center = new Vector2(mouse.X * Level.Graphics.PreferredBackBufferWidth + (collision.Width / 2), 0 + (collision.Height / 2));
                return placed;
            }
            else if (mouse.Y * Level.Graphics.PreferredBackBufferHeight >= Level.Graphics.PreferredBackBufferHeight && mouse.X * Level.Graphics.PreferredBackBufferWidth >= 0 && mouse.X * Level.Graphics.PreferredBackBufferWidth <= Level.Graphics.PreferredBackBufferWidth)
            {
                collision.X = mouse.X * Level.Graphics.PreferredBackBufferWidth;
                collision.Y = Level.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(mouse.X * Level.Graphics.PreferredBackBufferWidth + (collision.Width / 2), Level.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Level.Graphics.PreferredBackBufferWidth <= 0 && mouse.Y * Level.Graphics.PreferredBackBufferHeight <= 0)
            {
                collision.X = 0;
                collision.Y = 0;
                range.Center = new Vector2(0 + (collision.Width / 2), 0 + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Level.Graphics.PreferredBackBufferWidth <= 0 && mouse.Y * Level.Graphics.PreferredBackBufferHeight >= Level.Graphics.PreferredBackBufferHeight)
            {
                collision.X = 0;
                collision.Y = Level.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(0 + (collision.Width / 2), Level.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Level.Graphics.PreferredBackBufferWidth >= Level.Graphics.PreferredBackBufferWidth && mouse.Y * Level.Graphics.PreferredBackBufferHeight <= 0)
            {
                collision.X = Level.Graphics.PreferredBackBufferWidth;
                collision.Y = 0;
                range.Center = new Vector2(Level.Graphics.PreferredBackBufferWidth + (collision.Width / 2), 0 + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Level.Graphics.PreferredBackBufferWidth >= Level.Graphics.PreferredBackBufferWidth && mouse.Y * Level.Graphics.PreferredBackBufferHeight >= Level.Graphics.PreferredBackBufferHeight)
            {
                collision.X = Level.Graphics.PreferredBackBufferWidth;
                collision.Y = Level.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(Level.Graphics.PreferredBackBufferWidth + (collision.Width / 2), Level.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                return placed;
            } 
            #endregion
            else
            {
                return placed;
            }
        }

        private bool CanPlace()
        {
            return !Level.Path.Intersects(collision) && TowerCheck() && !Level.storeSection.Intersects(collision);
        }

        private bool TowerCheck()
        {
            bool check = true;
            for (int i = 0; i < Level.towers.Count - 1; i++)
            {
                if (collision.Intersects(Level.towers[i].collision))
                {
                    check = false;
                }
            }
            return check;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isPlaced || isSelected)
            {
                spriteBatch.Draw(rangeTex, range.Location, rangeColor);
            }
            spriteBatch.Draw(tex, collision.Draw, Color.White);
        }
    }
}
