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
        private event EventHandler waveFinished, levelComplete;
        float timer = 0, minTimer = 1f;
        Level level;
        Queue<Queue<EnemyType>> waves = new Queue<Queue<EnemyType>>();
        XmlDocument doc;
        XmlNode node;

        public event EventHandler WaveFinished
        {
            add { waveFinished += value; }
            remove { waveFinished -= value; }
        }

        public event EventHandler LevelComplete
        {
            add { levelComplete += value; }
            remove { levelComplete -= value; }
        }

        public WaveManager(Level level)
        {
            this.level = level;
        }

        public void LoadEnemies(string level)
        {
            doc = new XmlDocument();
            doc.Load(string.Format("Content/Levels/{0}.enm", level));
            node = doc.SelectSingleNode("/Enemies");

            foreach (XmlNode t in node.ChildNodes)
            {
                waves.Enqueue(new Queue<EnemyType>());
                foreach (XmlNode i in t.ChildNodes)
                {
                    waves.ElementAt(waves.Count - 1).Enqueue((EnemyType)int.Parse(i.InnerText));
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
                else if (waveFinished != null && level.enemies.Count == 0 && waves.Count > 1)
                {
                    waves.Dequeue();
                    waveFinished(this, EventArgs.Empty);
                }
                else if (levelComplete != null && level.enemies.Count == 0 && waves.Count == 1)
                {
                    waves.Dequeue();
                    levelComplete(this, EventArgs.Empty);
                }
                timer = 0;
            }
        }
    }
}