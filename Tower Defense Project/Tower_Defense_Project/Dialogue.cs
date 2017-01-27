using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Tower_Defense_Project
{
    class DialogueChoice
    {
        public string ChoiceType { get; }
        public string Name { get; }
        public string Result { get; }

        private Dialogue ParentDialogue { get; }

        private event EventHandler choiceSelected;

        public event EventHandler ChoiceSelected
        {
            add { choiceSelected += value; }
            remove { choiceSelected -= value; }
        }

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
                    ParentDialogue.LoadNode("Result");
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
    }

    class DialogueNode
    {
        public string Name { get; set; }
        public string Line { get; }

        public string DestinationNode { get; }
        public List<DialogueChoice> Choices { get; }
        public List<RectangleF> Rectangles { get; }
        public Texture2D Portrait { get; }

        public DialogueNode(string line, Texture2D portrait, string node)
        {
            Line = line;
            Portrait = portrait;
            DestinationNode = node;
            Name = "???";
        }

        public DialogueNode(string line, Texture2D portrait, string node, params DialogueChoice[] choices)
        {
            Line = line;
            Portrait = portrait;
            Choices = choices.ToList();
            Name = "???";

            if (choices[0].ChoiceType != "Node")
            {
                DestinationNode = node;
            }

            for (int i = Choices.Count; i > 0; i--)
            {
                
            }
        }

        public void Clear()
        {
            if (Choices != null) Choices.Clear();
        }
    }

    class Dialogue
    {
        private DialogueNode node;
        SpriteFont font;

        Dictionary<string, XmlNode> Nodes { get; set; }

        public DialogueNode Node
        {
            get { return node; }
            set { node = value; }
        }

        public Dialogue()
        {
            font = Main.GameContent.Load<SpriteFont>("Fonts/Font");
        }

        public void LoadDialogue(string dialogueName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(string.Format("{0}.xml", dialogueName));
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
            string line = "";

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

            Texture2D portrait = Main.GameContent.Load<Texture2D>(string.Format("Dialogues/{0}/{1}_{2}", "DialogueTitle", root.Attributes["name"].InnerText, root.Attributes["image"].InnerText));

            if (dialogueChoices.Count == 0)
            {
                Node = new DialogueNode(line, portrait, root.Attributes["destination"].InnerText);
            }
            else if (dialogueChoices[0].ChoiceType == "Node")
            {
                Node = new DialogueNode(line, portrait, "", dialogueChoices.ToArray());
            }
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
    }
}