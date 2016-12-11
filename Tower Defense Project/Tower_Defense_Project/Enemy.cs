using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Duality.Graphics;

namespace Tower_Defense_Project
{
    enum EnemyType
    {
        Peon = 101,
        Scout = 102,
        Brute = 103,
    }

    class Enemy
    {
        private Animation moveAnimation;
        private AnimationPlayer sprite;
        private Designer designer;
        public float stagePos;
        private float speed = 0, seconds = 0;
        private int stageIndex, frameWidth, health;
        private Level level;
        private Path path;
        private string spriteSet;
        private uint worth;
        public Vector2 position;

        public Level Level
        {
            get { return level; }
        }

        public Designer Designer
        {
            get { return designer; }
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public Path Path
        {
            get { return path; }
        }

        public uint Worth
        {
            get { return worth; }
        }

        /// <summary>
        /// Constructs a new Enemy.
        /// </summary>
        public Enemy(Level level, EnemyType type)
        {
            this.level = level;
            path = this.level.Path;

            spriteSet = Level.EnemyStats[(int)type][0];
            frameWidth = int.Parse(Level.EnemyStats[(int)type][1]);
            speed = float.Parse(Level.EnemyStats[(int)type][2]);
            seconds = float.Parse(Level.EnemyStats[(int)type][3]);
            health = int.Parse(Level.EnemyStats[(int)type][4]);
            worth = uint.Parse(Level.EnemyStats[(int)type][5]);

            LoadContent();
        }

        public Enemy(Designer designer, Path path)
        {
            this.designer = designer;
            this.path = path;

            spriteSet = "Brute";
            frameWidth = 30;
            speed = .5f;
            seconds = 1;

            LoadContent();
        }

        /// <summary>
        /// Loads a particular enemy sprite sheet and sounds.
        /// </summary>
        public void LoadContent()
        {
            moveAnimation = new Animation(Main.GameContent.Load<Texture2D>(string.Format("Enemies/{0}", spriteSet)), 0.1f, true, frameWidth);

            sprite.PlayAnimation(moveAnimation);
        }

        public void Update(GameTime gameTime)
        {
            if (stageIndex != path.Points.Count - 1)
            {
                stagePos += speed * seconds;
                while (stagePos > path.lengths[stageIndex])
                {
                    stagePos -= path.lengths[stageIndex];
                    stageIndex++;
                    if (stageIndex == path.Points.Count - 1)
                    {
                        position = path.Points[stageIndex];
                        return;
                    }
                }
                position = path.Points[stageIndex] + path.directions[stageIndex] * stagePos;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sprite.PlayAnimation(moveAnimation);

            sprite.Draw(gameTime, spriteBatch, new RectangleF(position.X, position.Y + 5, (int)((frameWidth / 800f) * Main.Scale.X), (int)((moveAnimation.Texture.Height / 480f) * Main.Scale.Y)), SpriteEffects.None);
        }
    }
}