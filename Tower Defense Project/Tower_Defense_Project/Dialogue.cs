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
		public string ChoiceType { get; private set; }
		public string Name { get; private set; }
		public string Result { get; private set; }

		private event EventHandler choiceSelected;

		public event EventHandler ChoiceSelected
		{
			add { choiceSelected += value; }
			remove { choiceSelected -= value; }
		}

		public DialogueChoice(string type, string name, string result)
		{
			ChoiceType = type;
			Name = name;
			Result = result;
		}
	}

	class DialogueNode
	{
		public string Name { get; set; }
		public string Line { get; private set; }

		public DialogueNode Node { get; private set; }
		public List<DialogueChoice> Choices { get; private set; }
		public Texture2D Portrait { get; private set; }

		public DialogueNode(string line, Texture2D portrait, DialogueNode node)
		{
			Line = line;
			Portrait = portrait;
			Node = node;
			Name = "???";
		}

		public DialogueNode(string line, Texture2D portrait, DialogueNode node, params DialogueChoice[] choices)
		{
			Line = line;
			Portrait = portrait;
			Choices = choices.ToList();
			Name = "???";

			if (choices[0].ChoiceType != "Node")
			{
				Node = node;
			}
		}

		public void Clear()
		{
			if (Choices != null) Choices.Clear();
			Node.Clear();
		}
	}

	class Dialogue
	{
		private DialogueNode node;
		SpriteFont font;

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
				string name = i.Attributes["name"].InnerText;
			}
		}

		private void LoadLine(string line, DialogueNode nod)
		{

		}

		public string WrapText(string text, float maxLineWidth, float maxLineHeight)
		{
			text.Replace("\n", "");
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