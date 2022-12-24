using System.Collections.Generic;
using System.IO;

namespace Dust
{
	public class GetText
	{
		private static object syncObject = new object();

		public bool readingLine;

		public void PopulateDictionary(Dictionary<string, string> dictionary, string file, bool buttons)
		{
			dictionary.Clear();
			StreamReader @string = this.GetString(file);
			string text = string.Empty;
			string empty = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			while (!@string.EndOfStream)
			{
				empty = text;
				text = @string.ReadLine();
				if (text.StartsWith("#"))
				{
					if (!text.StartsWith("#Description"))
					{
						if (text2 != string.Empty)
						{
							dictionary.Add(text2, text3);
							text2 = string.Empty;
						}
						string[] array = text.Split('#');
						if (array.Length > 1)
						{
							text2 = array[1];
							text3 = string.Empty;
						}
					}
				}
				else
				{
					string text4 = ((!buttons) ? "\n" : "[N]");
					if (empty != string.Empty && text != string.Empty)
					{
						text3 = (empty.StartsWith("#") ? (text3 + text) : (text3 + text4 + text));
					}
					else if (empty == string.Empty && text != string.Empty)
					{
						text3 = text3 + text4 + text4 + text;
					}
				}
			}
			if (text2 != string.Empty)
			{
				dictionary.Add(text2, text3);
				text2 = string.Empty;
			}
		}

		public List<string> PopulateList(List<string> list, string name, string file, bool buttons)
		{
			list.Clear();
			StreamReader @string = this.GetString(file);
			string empty = string.Empty;
			bool flag = false;
			while (!@string.EndOfStream)
			{
				empty = @string.ReadLine();
				if (empty == "#" + name)
				{
					empty = @string.ReadLine();
					flag = true;
				}
				if (flag)
				{
					if (empty.StartsWith("#") || !(empty != string.Empty))
					{
						break;
					}
					list.Add(empty);
				}
			}
			return list;
		}

		public string PopulateString(string name, string file, bool buttons)
		{
			StreamReader @string = this.GetString(file);
			string empty = string.Empty;
			string text = string.Empty;
			bool flag = false;
			while (!@string.EndOfStream)
			{
				empty = @string.ReadLine();
				if (empty == "#" + name)
				{
					empty = @string.ReadLine();
					flag = true;
				}
				if (flag)
				{
					if (empty.StartsWith("#"))
					{
						break;
					}
					string text2 = ((!buttons) ? "\n" : "[N]");
					text = text + empty + text2;
				}
			}
			return text;
		}

		private StreamReader GetString(string file)
		{
			if (!Game1.Xbox360)
			{
				return new StreamReader("../../../../../text/" + this.GetLocalizedFolder() + file + ".txt");
			}
			string text = ".txt";
			if (Game1.XBLABuild)
			{
				text = ".dst";
			}
			return new StreamReader("data/text/" + this.GetLocalizedFolder() + file + text);
		}

		public string GetLocalizedFolder()
		{
			return string.Empty;
		}
	}
}
