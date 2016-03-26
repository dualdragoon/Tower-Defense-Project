using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Tower_Defense_Project
{
    public class Particle
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Angle { get; set; }
        public float AngularVelocity { get; set; }
        public Color Color { get; set; }
        public float Size { get; set; }
        /// <summary>
        /// Time To Live.
        /// </summary>
        public int TTL { get; set; }

        public Particle() { }

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity, float angle, float angularVelocity, Color color, float size, int ttl)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            Size = size;
            TTL = ttl;
        }

        public virtual void Update()
        {
            TTL--;
            Position += Velocity;
            Angle += AngularVelocity;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            spriteBatch.Draw(Texture, Position, null, Color, Angle, origin, Size, SpriteEffects.None, 0f);
        }
    }

    class Pulse : Particle
    {
        RectangleF placement;
        float Speed { get; set; }

        public Pulse(Texture2D texture, Vector2 position, float speed, float angle, float angularVelocity, Color color, float size, int ttl)
        {
            Texture = texture;
            Position = position;
            Speed = speed;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            TTL = ttl;
            placement = new RectangleF(position.X, position.Y, 16 * size, 23 * size);
        }

        public override void Update()
        {
            TTL--;
            Angle += AngularVelocity;
            placement.X -= Speed / 4;
            placement.Y -= Speed / 4;
            placement.Width += Speed / 2;
            placement.Height += Speed / 2;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(placement.Width / 2, placement.Height / 2);

            spriteBatch.Draw(Texture, placement, null, Color, Angle, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }
}
