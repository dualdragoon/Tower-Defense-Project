using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Duality;
using SharpDX;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;

namespace Tower_Defense_Project
{
    class DialogueChoice
    {
        public string ChoiceType { get; }
        public string Name { get; }
        public string Result { get; }

        private Dialogue ParentDialogue { get; }

        public event EventHandler ChoiceSelected;

        public DialogueChoice(Dialogue dialogue, string type, string name, string result)
        {
            ParentDialogue = dialogue;
            ChoiceType = type;
            Name = name;
            Result = result;

            if (type == "Node")
            {
                ChoiceSelected += (object sender, EventArgs e) =>
                {
                    ParentDialogue.LoadNode(Result);
                };
            }
            else if (type == "Console")
            {
                ChoiceSelected += (object sender, EventArgs e) =>
                {
                    Console.WriteLine(Result);
                };
            }
        }

        public void Invoke() => ChoiceSelected?.Invoke(this, EventArgs.Empty);
    }

    class DialogueNode
    {
        public string Name;
        public string Line;

        public string DestinationNode;
        private Color TextColor = new Color(208, 112, 41);
        public List<DialogueChoice> Choices;
        public List<RectangleF> Rectangles = new List<RectangleF>();
		public RectangleF Side;
        public Texture2D Portrait, Background, Backdrop;

        public DialogueNode(string line, string name, string side, Texture2D portrait, Texture2D backdrop, Texture2D background, string node)
        {
            Init(line, name, side, portrait, backdrop, background, node);
        }

        public DialogueNode(string line, string name, string side, Texture2D portrait, Texture2D backdrop, Texture2D background, string node, params DialogueChoice[] choices)
        {
            SpriteFont tempFont = Main.GameContent.Load<SpriteFont>("Fonts/Font");
            Init(line, name, side, portrait, backdrop, background, node);
            Choices = choices.ToList();

            if (choices[0].ChoiceType != "Node")
            {
                DestinationNode = node;
            }

            Rectangles.Add(new RectangleF(0, 0, 0, 0));

            for (int i = 0; i < Choices.Count; i++)
            {
                Rectangles.Add(new RectangleF(Rectangles.Last().X + Rectangles.Last().Width + (10f / 1366f) * Main.Scale.X, .625f * Main.Scale.Y, ((tempFont.MeasureString(Choices[i].Name).X + 20) / 1366f) * Main.Scale.X, .078125f * Main.Scale.Y));
            }

            Rectangles.RemoveAt(0);
        }

        private void Init(string line, string name, string side, Texture2D portrait, Texture2D backdrop, Texture2D background, string node)
        {
            Line = line;
            Portrait = portrait;
            Backdrop = backdrop;
            Background = background;
            DestinationNode = node;
            Name = name;

			float x = (side == "Right") ? (1066f / 1363f) * Main.Scale.X : 0;
			Side = new RectangleF(x, (185f / 768f) * Main.Scale.Y, (300f / 1366f) * Main.Scale.X, (300f / 768f) * Main.Scale.Y);
		}

        public void Update()
        {
            for (int i = 0; i < Choices.Count; i++)
            {
                if (Rectangles[i].Contains(Main.CurrentMouse.X * Main.Scale.X, Main.CurrentMouse.Y * Main.Scale.Y) && Main.CurrentMouse.LeftButton.Pressed)
                {
                    Choices[i].Invoke();
                }
            }
        }

        public void Clear()
        {
            if (Choices != null) Choices.Clear();
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.Draw(Background, new RectangleF(0, 0, Main.Scale.X, Main.Scale.Y), Color.White);
            spriteBatch.Draw(Backdrop, new RectangleF(0, 0, Main.Scale.X, Main.Scale.Y), Color.White);

            spriteBatch.DrawString(font, Line, new Vector2((10f / 1366f) * Main.Scale.X, (530f / 768f) * Main.Scale.Y), TextColor);

			spriteBatch.Draw(Portrait, Side, Color.White);

			Vector2 nameSize = font.MeasureString(Name);
			spriteBatch.DrawString(font, Name, new Vector2(((683f - nameSize.X / 2) / 1366f) * Main.Scale.X, (450f / 768f) * Main.Scale.Y), Color.Black);

            for (int i = 0; i < Rectangles.Count; i++)
            {
                spriteBatch.DrawString(font, Choices[i].Name, new Vector2(Rectangles[i].X + (10f / 1366f) * Main.Scale.X, Rectangles[i].Y + (10f / 768f) * Main.Scale.Y), TextColor);
            }
        }
    }

    class Dialogue
    {
        string currentName;
        private DialogueNode node;
        SpriteFont font;
        Texture2D backDrop, background;

        Dictionary<string, XmlNode> Nodes { get; set; }

        public DialogueNode Node
        {
            get { return node; }
            set { node = value; }
        }

        public Dialogue()
        {
            LoadContent();
        }

        public void LoadContent()
        {
            backDrop = Main.GameContent.Load<Texture2D>("Dialogues/Dialogue Text Backdrop");
            font = Main.GameContent.Load<SpriteFont>("Fonts/Font");
            Nodes = new Dictionary<string, XmlNode>();
        }

        public void LoadDialogue(string dialogueName)
        {
            currentName = dialogueName;
            XmlDocument doc = new XmlDocument();
            doc.Load(string.Format("Content/Dialogues/{0}.xml", dialogueName));
            XmlNode root = doc.SelectSingleNode("/Dialogue");

            foreach (XmlNode i in root)
            {
                Nodes.Add(i.Attributes["idNum"].InnerText, i);
            }

            LoadNode("Start");
        }

		public void LoadNode(string idNum)
		{
			XmlNode root = Nodes[idNum];
			List<DialogueChoice> dialogueChoices = new List<DialogueChoice>();
			List<XmlNode> choices = new List<XmlNode>();
			string line = "", charName = root.Attributes["name"].InnerText, side = root.Attributes["side"].InnerText;

            foreach (XmlNode i in root)
            {
                string type = i.Attributes["type"].InnerText;
                if (type == "Speech")
                {
                    line = i.InnerText;
                }
                else if (type == "Choice")
                {
                    foreach (XmlNode l in i)
                    {
                        choices.Add(l);
                    }
                }
            }

            foreach (XmlNode i in choices)
            {
                string type = i.Attributes["type"].InnerText, name = i.Attributes["name"].InnerText, result = i.InnerText;
                dialogueChoices.Add(new DialogueChoice(this, type, name, result));
            }

            line = WrapText(line, Main.Scale.X, 150);

            Texture2D portrait = Main.GameContent.Load<Texture2D>(string.Format("Dialogues/{0}/{1}_{2}", currentName, root.Attributes["name"].InnerText, root.Attributes["image"].InnerText));

            try
            {
                background = Main.GameContent.Load<Texture2D>(string.Format("Dialogues/{0}", root.Attributes["background"].InnerText));
            }
            catch (AssetNotFoundException e)
            {
                ErrorHandler.RecordError(2, 103, string.Format("Background missing - name: {0}", root.Attributes["background"].InnerText), e.StackTrace);
            }
            catch { }

            if (dialogueChoices.Count == 0)
            {
                Node = new DialogueNode(line, charName, side, portrait, backDrop, background, root.Attributes["destination"].InnerText);
            }
            else if (dialogueChoices[0].ChoiceType == "Node")
            {
                Node = new DialogueNode(line, charName, side, portrait, backDrop, background, "", dialogueChoices.ToArray());
            }
            else
            {
                Node = new DialogueNode(line, charName, side, portrait, backDrop, background, "", dialogueChoices.ToArray());
            }
        }

        public void Update()
        {
            Node.Update();
        }

        public string WrapText(string text, float maxLineWidth, float maxLineHeight)
        {
            text.Replace("\n", " ");
            text.Replace("\r", "");
            string[] words = text.Split(' ');
            string newText = "";
            float lineWidth = 0;
            float lineHeight = font.MeasureString("A").Y;
            float spaceWidth = font.MeasureString(" ").X;

            foreach (string word in words)
            {
                Console.WriteLine(word);
                Vector2 size = font.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    newText += (word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    if (lineHeight + size.Y < maxLineHeight)
                    {
                        newText += ("\n" + word + " ");
                        lineWidth = size.X + spaceWidth;
                        lineHeight += size.Y;
                    }
                    else
                    {
                        newText += ("\t" + word + " ");
                        lineWidth = size.X + spaceWidth;
                        lineHeight = size.Y;
                    }
                }
            }

            return newText;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Node.Draw(spriteBatch, font);
        }
    }
}