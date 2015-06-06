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
        private Color rangeColor = Color.Gray;
        public FloatingRectangle collision;
        private Texture2D tex;
        public TowerType type;

        private void LoadContent()
        {
            switch (type)
            {
                case TowerType.Small:
                    tex = Level.Content.Load<Texture2D>(@"Towers/Small");
                    break;

                case TowerType.Medium:
                    tex = Level.Content.Load<Texture2D>(@"Towers/Medium");
                    break;

                case TowerType.Large:
                    tex = Level.Content.Load<Texture2D>(@"Towers/Large");
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
                    break;

                case TowerType.Medium:
                    collision = new FloatingRectangle(mouse.X, mouse.Y, 20, 20);
                    break;

                case TowerType.Large:
                    collision = new FloatingRectangle(mouse.X, mouse.Y, 30, 30);
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
        }

        private bool Placed (MouseState mouse)
        {
            bool placed = false;
            collision.X = mouse.X;
            collision.Y = mouse.Y;
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
            if (Level.Path.Intersects(collision))
            {
                canPlace = false;
            }
            else
            {
                canPlace = true;
            }
            return canPlace;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, collision.Draw, Color.White);
        }
    }
}
