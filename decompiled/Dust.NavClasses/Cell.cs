using System.Collections.Generic;

namespace Dust.NavClasses
{
	public class Cell
	{
		private string[] cellMapName;

		private NavScript[] storedScripts;

		private int cellType;

		private byte revealState;

		private int treasure;

		private bool save;

		private bool shop;

		private int challenge;

		private int entrance;

		public string[] CellMapName => this.cellMapName;

		public NavScript[] StoredScripts => this.storedScripts;

		public int CellType => this.cellType;

		public byte RevealState
		{
			get
			{
				return this.revealState;
			}
			set
			{
				this.revealState = value;
			}
		}

		public int Treasure
		{
			get
			{
				return this.treasure;
			}
			set
			{
				this.treasure = value;
			}
		}

		public bool Save => this.save;

		public bool Shop => this.shop;

		public int Challenge
		{
			get
			{
				return this.challenge;
			}
			set
			{
				this.challenge = value;
			}
		}

		public int Entrance => this.entrance;

		public Cell(NavManager navMap, List<string> mapNames, string[,] scripts, int _cellType, bool _save, bool _shop, int _challenge, int _treasure, int _entrance)
		{
			this.cellMapName = new string[mapNames.Count];
			this.storedScripts = new NavScript[this.cellMapName.Length];
			for (int i = 0; i < this.cellMapName.Length; i++)
			{
				this.cellMapName[i] = mapNames[i];
				this.storedScripts[i] = new NavScript(navMap);
				for (int j = 0; j < this.storedScripts[i].Lines.Length; j++)
				{
					this.storedScripts[i].Lines[j] = new NavScriptLine(scripts[i, j]);
				}
			}
			this.cellType = _cellType;
			this.treasure = _treasure;
			this.save = _save;
			this.shop = _shop;
			this.entrance = _entrance;
			this.challenge = -2;
			if (this.cellMapName[0].StartsWith("challenge"))
			{
				string text = "challenge";
				string text2 = this.cellMapName[0].Remove(0, text.Length);
				this.challenge = int.Parse(text2.Remove(2, text2.Length - 2));
				if (!text2.EndsWith(this.challenge.ToString()))
				{
					this.challenge = -1;
				}
			}
		}
	}
}
