using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
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
        SpriteFont font;
        Texture2D addWaveNormal, addWaveHovered, addEnemyNormal, addEnemyHovered,
            removeWaveNormal, removeWaveHovered, removeEnemyNormal, removeEnemyHovered, backNormal, backHovered, tex, viewBack, background;
        Vector2 enemyTypeLocation, mousePos;

        public List<List<EnemyType>> Waves
        {
            get { return waves; }
        }

        public WaveBuilder(Designer designer)
        {
            this.designer = designer;
            AddWave(this, EventArgs.Empty);
        }

        public void LoadContent()
        {
            typeColor = Color.LightGray;
            tex = Main.GameContent.Load<Texture2D>("Textures/SQUARE");
            font = Main.GameContent.Load<SpriteFont>("Fonts/Font");
            background = Main.GameContent.Load<Texture2D>("Textures/Wave Designer Background");

            addWaveNormal = Main.GameContent.Load<Texture2D>("Buttons/Add Wave Normal");
            addWaveHovered = Main.GameContent.Load<Texture2D>("Buttons/Add Wave Hovered");
            addWave = new Button(new Vector2(0, (49f / 64f) * Main.Scale.Y), (188f / 1440f) * Main.Scale.X, (1f / 9f) * Main.Scale.Y, 1, Main.CurrentMouse, addWaveNormal, addWaveHovered, true, Main.Scale.X, Main.Scale.Y);
            addWave.LeftClicked += AddWave;

            removeWaveNormal = Main.GameContent.Load<Texture2D>("Buttons/Remove Wave Normal");
            removeWaveHovered = Main.GameContent.Load<Texture2D>("Buttons/Remove Wave Hovered");
            removeWave = new Button(new Vector2((94f / 683f) * Main.Scale.X, (49f / 64f) * Main.Scale.Y), (188f / 1440f) * Main.Scale.X, (1f / 9f) * Main.Scale.Y, 3, Main.CurrentMouse, removeWaveNormal, removeWaveHovered, true, Main.Scale.X, Main.Scale.Y);
            removeWave.LeftClicked += RemoveWave;

            addEnemyNormal = Main.GameContent.Load<Texture2D>("Buttons/Add Enemy Normal");
            addEnemyHovered = Main.GameContent.Load<Texture2D>("Buttons/Add Enemy Hovered");
            addEnemy = new Button(new Vector2(0, (683f / 768f) * Main.Scale.Y), (188f / 1440f) * Main.Scale.X, (1f / 9f) * Main.Scale.Y, 2, Main.CurrentMouse, addEnemyNormal, addEnemyHovered, true, Main.Scale.X, Main.Scale.Y);
            addEnemy.LeftClicked += AddEnemy;

            removeEnemyNormal = Main.GameContent.Load<Texture2D>("Buttons/Remove Enemy Normal");
            removeEnemyHovered = Main.GameContent.Load<Texture2D>("Buttons/Remove Enemy Hovered");
            removeEnemy = new Button(new Vector2((94f / 683f) * Main.Scale.X, (683f / 768f) * Main.Scale.Y), (188f / 1440f) * Main.Scale.X, (1f / 9f) * Main.Scale.Y, 4, Main.CurrentMouse, removeEnemyNormal, removeEnemyHovered, true, Main.Scale.X, Main.Scale.Y);
            removeEnemy.LeftClicked += RemoveEnemy;

            backNormal = Main.GameContent.Load<Texture2D>("Buttons/Back Normal");
            backHovered = Main.GameContent.Load<Texture2D>("Buttons/Back Hovered");
            back = new Button(Vector2.Zero, (188f / 1440f) * Main.Scale.X, (1f / 9f) * Main.Scale.Y, 5, Main.CurrentMouse, backNormal, backHovered, true, Main.Scale.X, Main.Scale.Y);
            back.LeftClicked += Back;

            typeRect = new RectangleF((54f / 683f) * Main.Scale.X, (125f / 192f) * Main.Scale.Y, (75f / 683) * Main.Scale.X, (1f / 16f) * Main.Scale.Y);
            enemyTypeLocation = new Vector2((183f / 1366f) * Main.Scale.X, (169f / 256f) * Main.Scale.Y);

            viewBack = Main.GameContent.Load<Texture2D>("Textures/Wave Designer View Back");
            viewRect = new RectangleF(756, 98, 570, 630);
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
        }

        public void AddEnemy(object sender, EventArgs e)
        {
            if (waves.Count != 0)
            {
                waves[(int)selectedWave].Add((EnemyType)int.Parse(enemyType));
                selectedEnemy = waves[(int)selectedWave].Count - 1; 
            }
        }

        public void RemoveWave(object sender, EventArgs e)
        {
            if (waves.Count != 0)
            {
                waves.Remove(waves[(int)selectedWave]);
                if (selectedWave == waves.Count) selectedWave--; 
            }
        }

        public void RemoveEnemy(object sender, EventArgs e)
        {
            if (waves.Count != 0)
            {
                if (waves[(int)selectedWave].Count != 0)
                {
                    waves[(int)selectedWave].Remove(waves[(int)selectedWave][(int)selectedEnemy]);
                    if (selectedEnemy == waves[(int)selectedWave].Count) selectedEnemy--;
                } 
            }
        }

        public void Update(GameTime gameTime)
        {
            Main.CurrentMouse = Main.Mouse.GetState();
            mousePos = new Vector2(Main.CurrentMouse.X * Main.Scale.X, Main.CurrentMouse.Y * Main.Scale.Y);

            typeColor = (enemyTypeSelected) ? Color.Aqua : Color.LightGray;

            addWave.Update(Main.CurrentMouse);
            addEnemy.Update(Main.CurrentMouse);
            removeWave.Update(Main.CurrentMouse);
            removeEnemy.Update(Main.CurrentMouse);
            back.Update(Main.CurrentMouse);

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
            spriteBatch.Draw(background, new RectangleF(0, 0, Main.Scale.X, Main.Scale.Y), Color.White);

            spriteBatch.Draw(addWave.Texture, addWave.Collision, Color.White);
            spriteBatch.Draw(addEnemy.Texture, addEnemy.Collision, Color.White);
            spriteBatch.Draw(removeWave.Texture, removeWave.Collision, Color.White);
            spriteBatch.Draw(removeEnemy.Texture, removeEnemy.Collision, Color.White);
            spriteBatch.Draw(back.Texture, back.Collision, Color.White);
            spriteBatch.Draw(tex, typeRect, typeColor);
            spriteBatch.DrawString(font, enemyType, new Vector2(enemyTypeLocation.X - (font.MeasureString(enemyType).X / 2), enemyTypeLocation.Y), Color.Black);

            spriteBatch.DrawString(font, string.Format(numOfWaves, waves.Count), new Vector2(300, 200), Color.Gold);
            spriteBatch.DrawString(font, string.Format(numOfEnemies, totalEnemies), new Vector2(300, 250), Color.Gold);
            spriteBatch.DrawString(font, string.Format("Selected Wave: {0}", selectedWave + 1), new Vector2(756, 70), Color.Gold);
            spriteBatch.DrawString(font, "Waves", new Vector2((375f / 1366f) * Main.Scale.X, (205f / 256f) * Main.Scale.Y), Color.Gold);
            spriteBatch.DrawString(font, "Enemies", new Vector2((375f / 1366f) * Main.Scale.X, (59f / 64f) * Main.Scale.Y), Color.Gold);
        }
    }
}