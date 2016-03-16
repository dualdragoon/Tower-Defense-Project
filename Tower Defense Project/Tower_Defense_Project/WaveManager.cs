using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit;

namespace Tower_Defense_Project
{
    class WaveManager
    {
        public EventHandler WaveFinished;
        float timer = 0, minTimer = 1f;
        Level level;
        Queue<Queue<EnemyType>> waves = new Queue<Queue<EnemyType>>();
        XmlDocument doc;
        XmlNode node;

        public WaveManager(Level level)
        {
            this.level = level;
        }

        public void LoadEnemies(string level)
        {
            doc.Load(string.Format("Content/Levels/{0}.enm", level));
            node = doc.SelectSingleNode("/Enemies");

            foreach (XmlNode t in node.ChildNodes)
            {
                waves.Enqueue(new Queue<EnemyType>());
                foreach (XmlNode i in t.ChildNodes)
                {
                    waves.ElementAt(waves.Count-1).Enqueue((EnemyType)int.Parse(i.InnerText));
                }
            }
        }

        public void UpdateWave(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (timer > minTimer)
            {
                if (waves.Peek().Count > 0)
                {
                    level.enemies.Add(new Enemy(level, waves.Peek().Dequeue()));
                }
                else if (WaveFinished != null)
                {
                    waves.Dequeue();
                    WaveFinished(this, EventArgs.Empty);
                }
                timer = 0;
            }
        }
    }
}