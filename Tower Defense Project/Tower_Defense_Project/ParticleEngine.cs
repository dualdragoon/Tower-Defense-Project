using System;
using System.Collections.Generic;
using System.Timers;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Tower_Defense_Project
{
    public enum EngineType { Dripping, Pulsing }

    public class ParticleEngine
    {
        private EngineType type;
        private List<Particle> particles = new List<Particle>();
        private List<Texture2D> textures;
        private Random random;
        private Timer timer;
        private Vector2 location;

        public Vector2 EmitterLocation
        {
            get { return location; }
            set
            {
                switch (type)
                {
                    case EngineType.Dripping:
                        location = new Vector2((value.X * Main.Scale.X) + random.Next(13), (value.Y * Main.Scale.Y) + 13);
                        break;

                    case EngineType.Pulsing:
                        location = new Vector2((value.X * Main.Scale.X) - 2, (value.Y * Main.Scale.Y) - 2);
                        break;

                    default: break;
                }
            }
        }

        public ParticleEngine() { }

        public ParticleEngine(List<Texture2D> textures, Vector2 location, EngineType type)
        {
            this.location = location;
            this.textures = textures;
            this.type = type;
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

                case EngineType.Pulsing:
                    timer = new Timer(750);
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

        private Pulse GeneratePulse()
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            float speed = 1;
            Color color = Color.White;
            int ttl = 20 + random.Next(40);

            return new Pulse(texture, position, speed, 0, 0, color, 1, ttl);
        }

        private void ParticleTimed(object sender, EventArgs e)
        {
            switch (type)
            {
                case EngineType.Dripping:
                    particles.Add(GenerateDrippingParticle());
                    break;

                case EngineType.Pulsing:
                    particles.Add(GeneratePulse());
                    break;

                default: break;
            }
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