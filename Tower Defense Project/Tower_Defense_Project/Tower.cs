using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Duality;

namespace Tower_Defense_Project
{
    enum TowerType { Small, Medium, Large, };

    class Tower
    {
        public Level Level
        {
            get { return level; }
        }
        private Level level;

        public bool isPlaced = false;
        private bool canPlace;
        float attackTimer = 0, minAttackTimer;
        int dummy;
        public Circle range;
        private Color rangeColor = Color.Gray;
        public FloatingRectangle collision;
        private Texture2D tex, rangeTex;
        public TowerType type;

        private void LoadContent()
        {
            switch (type)
            {
                case TowerType.Small:
                    tex = Level.Content.Load<Texture2D>(@"Towers/Small");
                    rangeTex = Level.Content.Load<Texture2D>(@"Towers/Small Range");
                    break;

                case TowerType.Medium:
                    tex = Level.Content.Load<Texture2D>(@"Towers/Medium");
                    rangeTex = Level.Content.Load<Texture2D>(@"Towers/Test");
                    break;

                case TowerType.Large:
                    tex = Level.Content.Load<Texture2D>(@"Towers/Large");
                    rangeTex = Level.Content.Load<Texture2D>(@"Towers/Large Range");
                    break;

                default:
                    break;
            }
        }

        public Tower(Level level, TowerType type, MouseState mouse)
        {
            this.level = level;
            this.type = type;

            switch (type)
            {
                case TowerType.Small:
                    collision = new FloatingRectangle(mouse.X, mouse.Y, 10, 10);
                    range = new Circle(new Vector2(mouse.X + (collision.Width / 2), mouse.Y + (collision.Height / 2)), 40);
                    minAttackTimer = 1f;
                    break;

                case TowerType.Medium:
                    collision = new FloatingRectangle(mouse.X, mouse.Y, 20, 20);
                    range = new Circle(new Vector2(mouse.X + (collision.Width / 2), mouse.Y + (collision.Height / 2)), 120);
                    minAttackTimer = .3f;
                    break;

                case TowerType.Large:
                    collision = new FloatingRectangle(mouse.X, mouse.Y, 30, 30);
                    range = new Circle(new Vector2(mouse.X + (collision.Width / 2), mouse.Y + (collision.Height / 2)), 60);
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
            }
        }

        private void Fire()
        {
            //if (attackTimer > minAttackTimer)
            {
                attackTimer = 0f;
                for (int i = 0; i < Level.enemies.Count; i++)
                {
                    if (range.Contains(Level.enemies[i].position))
                    {
                        Level.projectiles.Add(new Projectile(collision.Location + new Vector2(collision.Width / 2), Level.enemies[i], ProjectileType.Small, Level));
                        break;
                    } 
                }
            }
        }

        private bool Placed(MouseState mouse)
        {
            bool placed = false;
            collision.X = mouse.X;
            collision.Y = mouse.Y;
            range.Center = new Vector2(mouse.X + (collision.Width / 2), mouse.Y + (collision.Height / 2));
            if (CanPlace())
            {
                rangeColor = Color.Gray;
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    placed = true;
                }
            }
            else
            {
                rangeColor = Color.Red;
                placed = false;
            }
            return placed;
        }

        private bool CanPlace()
        {
            return !Level.Path.Intersects(collision) && TowerCheck();
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
            if (!isPlaced)
            {
                spriteBatch.Draw(rangeTex, range.Location, rangeColor);
            }
            spriteBatch.Draw(tex, collision.Draw, Color.White);
        }
    }
}
