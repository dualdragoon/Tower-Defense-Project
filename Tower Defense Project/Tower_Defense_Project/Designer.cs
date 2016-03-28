using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Tower_Defense_Project
{
    class Designer
    {
        public List<Tower> towers = new List<Tower>();

        public Designer() { }

        public void LoadContent()
        {

        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tower i in towers)
            {
                i.Draw(spriteBatch);
            }
        }
    }
}