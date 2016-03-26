using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Tower_Defense_Project
{
    public enum EngineType { Dripping }

    public class ParticleEngine
    {
        private List<Particle> particles;
        private List<Texture2D> textures;
        private Random random;
        private Timer timer;
        private Vector2 location;

        public Vector2 EmitterLocation
        {
            get { return location; }
            set { location = new Vector2((value.X * 800) + random.Next(13), (value.Y * 480) + 13); }
        }

        public ParticleEngine(List<Texture2D> textures, Vector2 location, EngineType type)
        {
            this.location = location;
            this.textures = textures;
            particles = new List<Particle>();
            random = new Random();
            switch (type)
            {
                case EngineType.Dripping:
                    timer = new Timer(500);
                    timer.Elapsed += ParticleTimed;
                    timer.AutoReset = true;
                    timer.Start();
                    break;

                default: break;
            }
        }

        //Come back to this and change it to better suit certain things.
        private Particle GenerateNormalParticle()
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = new Vector2(1f * (float)(random.NextDouble() * 2 - 1), 1f * (float)(random.NextDouble() * 2 - 1));
            float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
            Color color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            float size = (float)random.NextDouble();
            int ttl = 20 + random.Next(40);

            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }

        private Particle GenerateDrippingParticle()
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = new Vector2(0, .5f);
            float angle = 0;
            float angularVelocity = 0;
            Color color = Color.White;
            float size = (float)random.NextDouble(.5, 1.5);
            int ttl = 20 + random.Next(40);

            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }

        private void ParticleTimed(object sender, EventArgs e)
        {
            particles.Add(GenerateDrippingParticle());
        }

        public void ClearParticles()
        {
            particles.Clear();
        }

        public void Update()
        {
            /*int total = 10;

            for (int i = 0; i < total; i++)
            {
                particles.Add(GenerateDrippingParticle());
            }*/

            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].TTL <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(spriteBatch);
            }
        }
    }
}
