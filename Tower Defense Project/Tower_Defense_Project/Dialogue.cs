using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Tower_Defense_Project
{
	private class DialogueChoice
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
	private class DialogueNode
	{
		public string Line { get; private set; }

		public DialogueNode Node { get; private set; }
		public List<DialogueChoice> Choices = new List<DialogueChoice>();

		public DialogueNode(string line, DialogueNode node)
		{
			Line = line;
			Node = node;
		}

		public DialogueNode(string line, DialogueNode node, params DialogueChoice[] choices)
		{
			Line = line;
			Choices = choices.ToList();

			if (choices[0].ChoiceType != "Node")
			{
				Node = node;
			}
		}
	}
	class Dialogue
	{

	}
}
