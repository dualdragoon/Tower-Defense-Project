using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Duality;
using Duality.Interaction;

namespace Tower_Defense_Project
{
    class WaveBuilder
    {
        bool enemyTypeSelected = false;
        int totalEnemies = 0;
        int? selectedEnemy, selectedWave;
        string enemyType = "101", numOfWaves = "Waves: {0}", numOfEnemies = "Enemies: {0}";
        Button addWave, addEnemy, removeWave, removeEnemy, back;
        Color typeColor;
        Designer designer;
        List<List<EnemyType>> waves = new List<List<EnemyType>>();
        RectangleF typeRect, viewRect;
        ScrollingWindow window;
        SpriteFont font;
        Texture2D addWaveNormal, addWaveHovered, addEnemyNormal, addEnemyHovered,
            removeWaveNormal, removeWaveHovered, removeEnemyNormal, removeEnemyHovered, backNormal, backHovered, tex, viewBack, background;
        Texture2D[] scrollBar = new Texture2D[6];
        Vector2 enemyTypeLocation, mousePos;
        Vector2[] scrollBarSizes = new Vector2[3];

        public List<List<EnemyType>> Waves
        {
            get { return waves; }
        }

        public WaveBuilder(Designer designer)
        {
            this.designer = designer;
            LoadContent();
            AddWave(this, EventArgs.Empty);
        }

        private void LoadContent()
        {
            typeColor = Color.LightGray;
            tex = Main.GameContent.Load<Texture2D>("Textures/SQUARE");
            font = Main.GameContent.Load<SpriteFont>("Fonts/Font");
            background = Main.GameContent.Load<Texture2D>("Textures/Wave Designer Background");

            addWaveNormal = Main.GameContent.Load<Texture2D>("Buttons/Wave Builder/Add Wave Normal");
            addWaveHovered = Main.GameContent.Load<Texture2D>("Buttons/Wave Builder/Add Wave Hovered");
            addWave = new Button(new Vector2(0, (49f / 64f) * Main.Scale.Y), (188f / 1440f) * Main.Scale.X, (1f / 9f) * Main.Scale.Y, 1, Main.CurrentMouse, addWaveNormal, addWaveHovered, true, Main.Scale.X, Main.Scale.Y);
            addWave.LeftClicked += AddWave;

            removeWaveNormal = Main.GameContent.Load<Texture2D>("Buttons/Wave Builder/Remove Wave Normal");
            removeWaveHovered = Main.GameContent.Load<Texture2D>("Buttons/Wave Builder/Remove Wave Hovered");
            removeWave = new Button(new Vector2((94f / 683f) * Main.Scale.X, (49f / 64f) * Main.Scale.Y), (188f / 1440f) * Main.Scale.X, (1f / 9f) * Main.Scale.Y, 3, Main.CurrentMouse, removeWaveNormal, removeWaveHovered, true, Main.Scale.X, Main.Scale.Y);
            removeWave.LeftClicked += RemoveWave;

            addEnemyNormal = Main.GameContent.Load<Texture2D>("Buttons/Wave Builder/Add Enemy Normal");
            addEnemyHovered = Main.GameContent.Load<Texture2D>("Buttons/Wave Builder/Add Enemy Hovered");
            addEnemy = new Button(new Vector2(0, (683f / 768f) * Main.Scale.Y), (188f / 1440f) * Main.Scale.X, (1f / 9f) * Main.Scale.Y, 2, Main.CurrentMouse, addEnemyNormal, addEnemyHovered, true, Main.Scale.X, Main.Scale.Y);
            addEnemy.LeftClicked += AddEnemy;

            removeEnemyNormal = Main.GameContent.Load<Texture2D>("Buttons/Wave Builder/Remove Enemy Normal");
            removeEnemyHovered = Main.GameContent.Load<Texture2D>("Buttons/Wave Builder/Remove Enemy Hovered");
            removeEnemy = new Button(new Vector2((94f / 683f) * Main.Scale.X, (683f / 768f) * Main.Scale.Y), (188f / 1440f) * Main.Scale.X, (1f / 9f) * Main.Scale.Y, 4, Main.CurrentMouse, removeEnemyNormal, removeEnemyHovered, true, Main.Scale.X, Main.Scale.Y);
            removeEnemy.LeftClicked += RemoveEnemy;

            backNormal = Main.GameContent.Load<Texture2D>("Buttons/Wave Builder/Back Normal");
            backHovered = Main.GameContent.Load<Texture2D>("Buttons/Wave Builder/Back Hovered");
            back = new Button(Vector2.Zero, (188f / 1440f) * Main.Scale.X, (1f / 9f) * Main.Scale.Y, 5, Main.CurrentMouse, backNormal, backHovered, true, Main.Scale.X, Main.Scale.Y);
            back.LeftClicked += Back;

            typeRect = new RectangleF((54f / 683f) * Main.Scale.X, (125f / 192f) * Main.Scale.Y, (75f / 683) * Main.Scale.X, (1f / 16f) * Main.Scale.Y);
            enemyTypeLocation = new Vector2((183f / 1366f) * Main.Scale.X, (169f / 256f) * Main.Scale.Y);

            viewBack = Main.GameContent.Load<Texture2D>("Textures/Wave Designer View Back");
            viewRect = new RectangleF((378f / 683f) * Main.Scale.X, (49f / 384f) * Main.Scale.Y, (285f / 683f) * Main.Scale.X, (105f / 128f) * Main.Scale.Y);

            scrollBar[0] = Main.GameContent.Load<Texture2D>("Buttons/Scroll Up Normal");
            scrollBar[1] = Main.GameContent.Load<Texture2D>("Buttons/Scroll Up Hovered");
            scrollBar[2] = Main.GameContent.Load<Texture2D>("Buttons/Scroll Normal");
            scrollBar[3] = Main.GameContent.Load<Texture2D>("Buttons/Scroll Hovered");
            scrollBar[4] = Main.GameContent.Load<Texture2D>("Buttons/Scroll Down Normal");
            scrollBar[5] = Main.GameContent.Load<Texture2D>("Buttons/Scroll Down Hovered");

            scrollBarSizes[0] = new Vector2((5f / 288f) * Main.Scale.X, (1f / 30f) * Main.Scale.Y);
            scrollBarSizes[1] = new Vector2((5f / 288f) * Main.Scale.X, (1f / 30f) * Main.Scale.Y);
            scrollBarSizes[2] = new Vector2((5f / 288f) * Main.Scale.X, (1f / 30f) * Main.Scale.Y);

            window = new ScrollingWindow(new List<string>(), scrollBar, scrollBarSizes, viewRect, Main.Scale, font, Main.CurrentMouse);
        }

        private List<string> StringsFromTypes(List<EnemyType> enemies)
        {
            List<string> t = new List<string>();
            for (int i = 0; i < enemies.Count; i++)
            {
                t.Add(((int)enemies[i]).ToString());
            }
            return t;
        }

        public void Clear()
        {
            Waves.Clear();
            AddWave(this, EventArgs.Empty);
        }

        public void Back(object sender, EventArgs e)
        {
            designer.Form = DesignerForm.Path;
        }

        public void AddWave(object sender, EventArgs e)
        {
            waves.Add(new List<EnemyType>());
            selectedWave = waves.Count - 1;
            selectedEnemy = null;
            window.Strings = StringsFromTypes(waves[(int)selectedWave]);
        }

        public void AddEnemy(object sender, EventArgs e)
        {
            if (waves.Count != 0)
            {
                waves[(int)selectedWave].Add((EnemyType)int.Parse(enemyType));
                selectedEnemy = waves[(int)selectedWave].Count - 1;
                window.Strings = StringsFromTypes(waves[(int)selectedWave]);
            }
        }

        public void RemoveWave(object sender, EventArgs e)
        {
            if (waves.Count != 0)
            {
                waves.Remove(waves[(int)selectedWave]);
                if (selectedWave == waves.Count && selectedWave != 0) selectedWave--;
                if (selectedEnemy != null && waves.Count != 0) selectedEnemy = (selectedEnemy != 0) ? waves[(int)selectedWave].Count - 1 : waves[(int)selectedWave].Count;
                if (waves.Count != 0) window.Strings = StringsFromTypes(waves[(int)selectedWave]);
                if (waves.Count == 0) selectedWave = -1;
                else window.Strings = new List<string>();
            }
        }

        public void RemoveEnemy(object sender, EventArgs e)
        {
            if (waves.Count != 0)
            {
                if (waves[(int)selectedWave].Count != 0 && selectedEnemy != null)
                {
                    waves[(int)selectedWave].Remove(waves[(int)selectedWave][(int)selectedEnemy]);
                    if (selectedEnemy == waves[(int)selectedWave].Count && selectedEnemy != 0) selectedEnemy--;
                    window.Strings = StringsFromTypes(waves[(int)selectedWave]);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            mousePos = new Vector2(Main.CurrentMouse.X * Main.Scale.X, Main.CurrentMouse.Y * Main.Scale.Y);

            typeColor = (enemyTypeSelected) ? Color.Aqua : Color.LightGray;

            addWave.Update(Main.CurrentMouse);
            addEnemy.Update(Main.CurrentMouse);
            removeWave.Update(Main.CurrentMouse);
            removeEnemy.Update(Main.CurrentMouse);
            back.Update(Main.CurrentMouse);

            window.Update(Main.CurrentMouse);

            if (Main.CurrentMouse.LeftButton.Pressed) enemyTypeSelected = typeRect.Contains(mousePos);

            if (enemyTypeSelected)
            {
                char? c;
                InputParser.TryConvertNumberInput(Main.CurrentKeyboard, out c);
                StringInput(ref enemyType, c);
            }

            totalEnemies = 0;
            foreach (List<EnemyType> i in waves)
            {
                totalEnemies += i.Count;
            }
        }

        private void StringInput(ref string source, char? c)
        {
            if (c == '\b') source = source.Remove(source.Length - 1);
            else source += c;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(viewBack, viewRect, Color.White);
            window.Draw(spriteBatch);
            spriteBatch.Draw(background, new RectangleF(0, 0, Main.Scale.X, Main.Scale.Y), Color.White);

            spriteBatch.Draw(addWave.Texture, addWave.Collision, Color.White);
            spriteBatch.Draw(addEnemy.Texture, addEnemy.Collision, Color.White);
            spriteBatch.Draw(removeWave.Texture, removeWave.Collision, Color.White);
            spriteBatch.Draw(removeEnemy.Texture, removeEnemy.Collision, Color.White);
            spriteBatch.Draw(back.Texture, back.Collision, Color.White);
            spriteBatch.Draw(tex, typeRect, typeColor);
            spriteBatch.DrawString(font, enemyType, new Vector2(enemyTypeLocation.X - (font.MeasureString(enemyType).X / 2), enemyTypeLocation.Y), Color.Black);

            spriteBatch.DrawString(font, string.Format("Selected Wave: {0}", selectedWave + 1), new Vector2((378f / 683f) * Main.Scale.X, (35f / 384f) * Main.Scale.Y), Color.Gold);
            spriteBatch.DrawString(font, string.Format(numOfWaves, waves.Count), new Vector2((375f / 1366f) * Main.Scale.X, (205f / 256f) * Main.Scale.Y), Color.Gold);
            spriteBatch.DrawString(font, string.Format(numOfEnemies, totalEnemies), new Vector2((375f / 1366f) * Main.Scale.X, (59f / 64f) * Main.Scale.Y), Color.Gold);
        }
    }
}