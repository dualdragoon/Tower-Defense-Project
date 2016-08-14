using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Duality;
using Duality.Interaction;

namespace Tower_Defense_Project
{
    class DesignPath : Path
    {
        private List<DesignCurve> curves = new List<DesignCurve>();
        private Texture2D tex, pressed, unPressed;

        public new List<DesignCurve> Curves
        {
            get { return curves; }
            set { curves = value; }
        }

        public DesignPath()
        {
            LoadContent();
        }

        private void LoadContent()
        {
            tex = Main.GameContent.Load<Texture2D>("Textures/help");
            pressed = Main.GameContent.Load<Texture2D>("Textures/Point Pressed");
            unPressed = Main.GameContent.Load<Texture2D>("Textures/Point");
        }

        public void AddCurve()
        {
            if (curves.Count > 0)
            {
                Vector2 i = curves.Last().End;
                List<Vector2> points = new List<Vector2>() { i - new Vector2(50, 25), i, i + new Vector2(25, 75), i + new Vector2(100, 75) };
                curves.Add(new DesignCurve(points));
            }
            else curves.Add(new DesignCurve());
            DesignCurve o = curves.Last();
        }

        public new void Clear()
        {
            base.Clear();
            Curves.Clear();
        }

        public new void Build()
        {
            Points.Clear();

            foreach (DesignCurve i in Curves)
            {
                i.Build();
                Points.AddRange(i.Reference);
            }

            lengths = new float[Points.Count - 1];
            directions = new Vector2[Points.Count - 1];
            for (int i = 0; i < Points.Count - 1; i++)
            {
                directions[i] = Points[i + 1] - Points[i];
                lengths[i] = directions[i].Length();
                directions[i].Normalize();
            }
        }

        public void Update()
        {
            for (int i = 0; i < curves.Count; i++)
            {
                curves[i].Update();
            }
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            foreach (DesignCurve i in curves)
            {
                i.Draw(spriteBatch, tex);
            }
        }
    }
}