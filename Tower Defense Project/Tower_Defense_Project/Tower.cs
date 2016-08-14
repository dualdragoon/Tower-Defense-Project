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
        public bool isPlaced = false, isSelected;
        public Ellipse range, tower;
        private Color rangeColor = Color.Gray;
        private float attackTimer = 0, minAttackTimer, diameter;
        private int size;
        private Level level;
        private ProjectileType projectileType;
        private RectangleF collision;
        private string spriteSet;
        private Texture2D texture, rangeTex;
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

        public Vector2 Position
        {
            get { return collision.Location; }
            set
            {
                Vector2 offset = value - collision.Location;
                range.Offset(offset);
                collision.Location = value;
            }
        }

        public Vector2 Center
        {
            get { return range.Center; }
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

            collision = new RectangleF((mouse.X * Main.Scale.X) - ((size / 800f) * Main.Scale.X), (mouse.Y * Main.Scale.Y) - ((size / 480f) * Main.Scale.Y), (size / 800f) * Main.Scale.X, (size / 480f) * Main.Scale.Y);
            range = new Ellipse((diameter / 800f) * Main.Scale.X, (diameter / 480f) * Main.Scale.Y, new Vector2(mouse.X * Main.Scale.X, mouse.Y * Main.Scale.Y));
            tower = new Ellipse((size / 800f) * Main.Scale.X, (size / 480f) * Main.Scale.Y, new Vector2(mouse.X * Main.Scale.X, mouse.Y * Main.Scale.Y));

            LoadContent();
        }

        private void LoadContent()
        {
            texture = Main.GameContent.Load<Texture2D>(@"Towers/" + spriteSet);
            rangeTex = Main.GameContent.Load<Texture2D>(@"Towers/Range");
        }

        public void Update(GameTime gameTime, MouseState mouse)
        {
            if (!isPlaced) isPlaced = Placed(mouse);
            else
            {
                attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (attackTimer > minAttackTimer)
                {
                    Fire();
                }

                if (mouse.RightButton.Pressed)
                {
                    if (tower.Contains(new Vector2(mouse.X * Main.Scale.X, mouse.Y * Main.Scale.Y)))
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
            if (mouse.X * Main.Scale.X >= 0 && mouse.X * Main.Scale.X <= Main.Scale.X && mouse.Y * Main.Scale.Y >= 0 && mouse.Y * Main.Scale.Y <= Main.Scale.Y)
            {
                collision.Location = new Vector2(mouse.X * Main.Scale.X - (collision.Width / 2), mouse.Y * Main.Scale.Y - (collision.Height / 2));
                range.Center = new Vector2(mouse.X * Main.Scale.X, mouse.Y * Main.Scale.Y);
                tower.Center = new Vector2(mouse.X * Main.Scale.X, mouse.Y * Main.Scale.Y);
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
            else if (mouse.X * Main.Scale.X <= 0 && mouse.Y * Main.Scale.Y >= 0 && mouse.Y * Main.Scale.Y <= Main.Scale.Y)
            {
                collision.X = 0;
                collision.Y = mouse.Y * Main.Scale.Y;
                range.Center = new Vector2(0 + (collision.Width / 2), mouse.Y * Main.Scale.Y + (collision.Height / 2));
                tower.Center = new Vector2(0 + (collision.Width / 2), mouse.Y * Main.Scale.Y + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Main.Scale.X >= Main.Scale.X && mouse.Y * Main.Scale.Y >= 0 && mouse.Y * Main.Scale.Y <= Main.Scale.Y)
            {
                collision.X = Main.Scale.X;
                collision.Y = mouse.Y * Main.Scale.Y;
                range.Center = new Vector2(Main.Scale.X + (collision.Width / 2), mouse.Y * Main.Scale.Y + (collision.Height / 2));
                tower.Center = new Vector2(Main.Scale.X + (collision.Width / 2), mouse.Y * Main.Scale.Y + (collision.Height / 2));
                return placed;
            }
            else if (mouse.Y * Main.Scale.Y <= 0 && mouse.X * Main.Scale.X >= 0 && mouse.X * Main.Scale.X <= Main.Scale.X)
            {
                collision.X = mouse.X * Main.Scale.X;
                collision.Y = 0;
                range.Center = new Vector2(mouse.X * Main.Scale.X + (collision.Width / 2), 0 + (collision.Height / 2));
                tower.Center = new Vector2(mouse.X * Main.Scale.X + (collision.Width / 2), 0 + (collision.Height / 2));
                return placed;
            }
            else if (mouse.Y * Main.Scale.Y >= Main.Scale.Y && mouse.X * Main.Scale.X >= 0 && mouse.X * Main.Scale.X <= Main.Scale.X)
            {
                collision.X = mouse.X * Main.Scale.X;
                collision.Y = Main.Scale.Y;
                range.Center = new Vector2(mouse.X * Main.Scale.X + (collision.Width / 2), Main.Scale.Y + (collision.Height / 2));
                tower.Center = new Vector2(mouse.X * Main.Scale.X + (collision.Width / 2), Main.Scale.Y + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Main.Scale.X <= 0 && mouse.Y * Main.Scale.Y <= 0)
            {
                collision.X = 0;
                collision.Y = 0;
                range.Center = new Vector2(0 + (collision.Width / 2), 0 + (collision.Height / 2));
                tower.Center = new Vector2(0 + (collision.Width / 2), 0 + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Main.Scale.X <= 0 && mouse.Y * Main.Scale.Y >= Main.Scale.Y)
            {
                collision.X = 0;
                collision.Y = Main.Scale.Y;
                range.Center = new Vector2(0 + (collision.Width / 2), Main.Scale.Y + (collision.Height / 2));
                tower.Center = new Vector2(0 + (collision.Width / 2), Main.Scale.Y + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Main.Scale.X >= Main.Scale.X && mouse.Y * Main.Scale.Y <= 0)
            {
                collision.X = Main.Scale.X;
                collision.Y = 0;
                range.Center = new Vector2(Main.Scale.X + (collision.Width / 2), 0 + (collision.Height / 2));
                tower.Center = new Vector2(Main.Scale.X + (collision.Width / 2), 0 + (collision.Height / 2));
                return placed;
            }
            else if (mouse.X * Main.Scale.X >= Main.Scale.X && mouse.Y * Main.Scale.Y >= Main.Scale.Y)
            {
                collision.X = Main.Scale.X;
                collision.Y = Main.Scale.Y;
                range.Center = new Vector2(Main.Scale.X + (collision.Width / 2), Main.Scale.Y + (collision.Height / 2));
                tower.Center = new Vector2(Main.Scale.X + (collision.Width / 2), Main.Scale.Y + (collision.Height / 2));
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
            try { return !Level.Path.Intersects(tower) && TowerCheck() && !tower.Intersects(Level.storeSection); }
            catch { return false; }
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
                spriteBatch.Draw(rangeTex, range.RectangleF, rangeColor);
            }
            spriteBatch.Draw(texture, collision, Color.White);
        }
    }
}