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
    enum TowerType
    {
        GL = 101,
        RL = 102,
        BLL = 103,
    }

    class Tower
    {
        public bool isPlaced = false;
        private bool isSelected;
        public Circle range;
        private Color rangeColor = Color.Gray;
        private float attackTimer = 0, minAttackTimer, diameter;
        private int size;
        private Level level;
        private ProjectileType projectileType;
        public RectangleF collision;
        private string spriteSet;
        private Texture2D tex, rangeTex;
        public TowerType type;
        private uint cost;

        public Level Level
        {
            get { return level; }
        }

        public uint Cost
        {
            get { return cost; }
        }

        public Tower(Level level, TowerType type, MouseState mouse)
        {
            this.level = level;
            this.type = type;

            spriteSet = Level.TowerStats[(int)type][0];
            size = int.Parse(Level.TowerStats[(int)type][1]);
            diameter = float.Parse(Level.TowerStats[(int)type][2]);
            minAttackTimer = float.Parse(Level.TowerStats[(int)type][3]);
            projectileType = (ProjectileType)int.Parse(Level.TowerStats[(int)type][4]);
            cost = uint.Parse(Level.TowerStats[(int)type][5]);

            collision = new RectangleF(mouse.X * Main.Graphics.PreferredBackBufferWidth, mouse.Y * Main.Graphics.PreferredBackBufferHeight, size, size);
            range = new Circle(new Vector2(mouse.X * Main.Graphics.PreferredBackBufferWidth + (collision.Width / 2), mouse.Y * Main.Graphics.PreferredBackBufferHeight + (collision.Height / 2)), diameter);

            LoadContent();
        }

        private void LoadContent()
        {
            tex = Main.GameContent.Load<Texture2D>(@"Towers/" + spriteSet);
            rangeTex = Main.GameContent.Load<Texture2D>(@"Towers/" + spriteSet + " Range");
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
                    if (collision.Contains(new Vector2(mouse.X * Main.Graphics.PreferredBackBufferWidth, mouse.Y * Main.Graphics.PreferredBackBufferHeight)))
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
            if (mouse.X * Main.Graphics.PreferredBackBufferWidth >= 0 && mouse.X * Main.Graphics.PreferredBackBufferWidth <= Main.Graphics.PreferredBackBufferWidth && mouse.Y * Main.Graphics.PreferredBackBufferHeight >= 0 && mouse.Y * Main.Graphics.PreferredBackBufferHeight <= Main.Graphics.PreferredBackBufferHeight)
            {
                collision.X = mouse.X * Main.Graphics.PreferredBackBufferWidth;
                collision.Y = mouse.Y * Main.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(mouse.X * Main.Graphics.PreferredBackBufferWidth + (collision.Width / 2), mouse.Y * Main.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
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
            #region Mouse Off-Screen Handling
            else if (mouse.X * Main.Graphics.PreferredBackBufferWidth <= 0 && mouse.Y * Main.Graphics.PreferredBackBufferHeight >= 0 && mouse.Y * Main.Graphics.PreferredBackBufferHeight <= Main.Graphics.PreferredBackBufferHeight)
            {
                collision.X = 0;
                collision.Y = mouse.Y * Main.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(0 + (collision.Width / 2), mouse.Y * Main.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Main.Graphics.PreferredBackBufferWidth >= Main.Graphics.PreferredBackBufferWidth && mouse.Y * Main.Graphics.PreferredBackBufferHeight >= 0 && mouse.Y * Main.Graphics.PreferredBackBufferHeight <= Main.Graphics.PreferredBackBufferHeight)
            {
                collision.X = Main.Graphics.PreferredBackBufferWidth;
                collision.Y = mouse.Y * Main.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(Main.Graphics.PreferredBackBufferWidth + (collision.Width / 2), mouse.Y * Main.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                return placed;
            }
            else if (mouse.Y * Main.Graphics.PreferredBackBufferHeight <= 0 && mouse.X * Main.Graphics.PreferredBackBufferWidth >= 0 && mouse.X * Main.Graphics.PreferredBackBufferWidth <= Main.Graphics.PreferredBackBufferWidth)
            {
                collision.X = mouse.X * Main.Graphics.PreferredBackBufferWidth;
                collision.Y = 0;
                range.Center = new Vector2(mouse.X * Main.Graphics.PreferredBackBufferWidth + (collision.Width / 2), 0 + (collision.Height / 2));
                return placed;
            }
            else if (mouse.Y * Main.Graphics.PreferredBackBufferHeight >= Main.Graphics.PreferredBackBufferHeight && mouse.X * Main.Graphics.PreferredBackBufferWidth >= 0 && mouse.X * Main.Graphics.PreferredBackBufferWidth <= Main.Graphics.PreferredBackBufferWidth)
            {
                collision.X = mouse.X * Main.Graphics.PreferredBackBufferWidth;
                collision.Y = Main.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(mouse.X * Main.Graphics.PreferredBackBufferWidth + (collision.Width / 2), Main.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Main.Graphics.PreferredBackBufferWidth <= 0 && mouse.Y * Main.Graphics.PreferredBackBufferHeight <= 0)
            {
                collision.X = 0;
                collision.Y = 0;
                range.Center = new Vector2(0 + (collision.Width / 2), 0 + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Main.Graphics.PreferredBackBufferWidth <= 0 && mouse.Y * Main.Graphics.PreferredBackBufferHeight >= Main.Graphics.PreferredBackBufferHeight)
            {
                collision.X = 0;
                collision.Y = Main.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(0 + (collision.Width / 2), Main.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Main.Graphics.PreferredBackBufferWidth >= Main.Graphics.PreferredBackBufferWidth && mouse.Y * Main.Graphics.PreferredBackBufferHeight <= 0)
            {
                collision.X = Main.Graphics.PreferredBackBufferWidth;
                collision.Y = 0;
                range.Center = new Vector2(Main.Graphics.PreferredBackBufferWidth + (collision.Width / 2), 0 + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Main.Graphics.PreferredBackBufferWidth >= Main.Graphics.PreferredBackBufferWidth && mouse.Y * Main.Graphics.PreferredBackBufferHeight >= Main.Graphics.PreferredBackBufferHeight)
            {
                collision.X = Main.Graphics.PreferredBackBufferWidth;
                collision.Y = Main.Graphics.PreferredBackBufferHeight;
                range.Center = new Vector2(Main.Graphics.PreferredBackBufferWidth + (collision.Width / 2), Main.Graphics.PreferredBackBufferHeight + (collision.Height / 2));
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
            spriteBatch.Draw(tex, collision, Color.White);
        }
    }
}