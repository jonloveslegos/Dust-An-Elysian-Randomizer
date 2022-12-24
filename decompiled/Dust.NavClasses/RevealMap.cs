using System.Collections.Generic;
using Dust.Storage;
using Microsoft.Xna.Framework;

namespace Dust.NavClasses
{
	public class RevealMap
	{
		private string navName;

		private Cell[,] cell;

		private byte width;

		private byte height;

		private Vector2 lastVisitPos;

		private bool visiting;

		private bool updated;

		private bool destination;

		private byte explored;

		private byte totalExplorable;

		private List<SaveItem> gameItemList = new List<SaveItem>();

		private List<SaveItem> keyList = new List<SaveItem>();

		private List<SaveItem> chestList = new List<SaveItem>();

		private List<SaveItem> cageList = new List<SaveItem>();

		private List<SaveItem> destructableList = new List<SaveItem>();

		public string NavName
		{
			get
			{
				return this.navName;
			}
			set
			{
				this.navName = value;
			}
		}

		public Cell[,] Cell
		{
			get
			{
				return this.cell;
			}
			set
			{
				this.cell = value;
			}
		}

		public byte Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}

		public byte Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
			}
		}

		public Vector2 LastVisitPos
		{
			get
			{
				return this.lastVisitPos;
			}
			set
			{
				this.lastVisitPos = value;
			}
		}

		public bool Visiting
		{
			get
			{
				return this.visiting;
			}
			set
			{
				this.visiting = value;
			}
		}

		public bool Updated
		{
			get
			{
				return this.updated;
			}
			set
			{
				this.updated = value;
			}
		}

		public bool Destination
		{
			get
			{
				return this.destination;
			}
			set
			{
				this.destination = value;
			}
		}

		public byte Explored
		{
			get
			{
				return this.explored;
			}
			set
			{
				this.explored = value;
			}
		}

		public List<SaveItem> GameItemList
		{
			get
			{
				return this.gameItemList;
			}
			set
			{
				this.gameItemList = value;
			}
		}

		public List<SaveItem> KeyList
		{
			get
			{
				return this.keyList;
			}
			set
			{
				this.keyList = value;
			}
		}

		public List<SaveItem> ChestList
		{
			get
			{
				return this.chestList;
			}
			set
			{
				this.chestList = value;
			}
		}

		public List<SaveItem> CageList
		{
			get
			{
				return this.cageList;
			}
			set
			{
				this.cageList = value;
			}
		}

		public List<SaveItem> DestructableList
		{
			get
			{
				return this.destructableList;
			}
			set
			{
				this.destructableList = value;
			}
		}

		public RevealMap(string name, byte _width, byte _height, byte _totalExplorable)
		{
			this.navName = name;
			this.width = _width;
			this.height = _height;
			this.totalExplorable = _totalExplorable;
			this.cell = new Cell[this.width, this.height];
			this.Reset();
		}

		public void Reset()
		{
			for (int i = 0; i < this.width; i++)
			{
				for (int j = 0; j < this.height; j++)
				{
					if (this.cell[i, j] != null)
					{
						this.cell[i, j].RevealState = 0;
					}
				}
			}
			this.lastVisitPos = new Vector2(-1f, -1f);
			this.visiting = false;
			this.updated = false;
			this.explored = 0;
			for (int k = 0; k < this.gameItemList.Count; k++)
			{
				this.gameItemList[k].Stage = 0;
			}
			for (int l = 0; l < this.keyList.Count; l++)
			{
				this.keyList[l].Stage = 0;
			}
			for (int m = 0; m < this.chestList.Count; m++)
			{
				this.chestList[m].Stage = 0;
			}
			for (int n = 0; n < this.cageList.Count; n++)
			{
				this.cageList[n].Stage = 0;
			}
			for (int num = 0; num < this.destructableList.Count; num++)
			{
				this.destructableList[num].Stage = 0;
			}
		}

		public void Unlock()
		{
			for (int i = 0; i < this.Width; i++)
			{
				for (int j = 0; j < this.Height; j++)
				{
					if (this.cell[i, j] != null)
					{
						this.cell[i, j].RevealState = 1;
					}
				}
			}
			this.LastVisitPos = new Vector2(1f, 1f);
			this.Explored = this.totalExplorable;
			this.updated = true;
		}

		public float GetTotalExplored()
		{
			if (this.lastVisitPos == new Vector2(-1f, -1f))
			{
				return 0f;
			}
			return (float)(int)this.explored / (float)(int)this.totalExplorable * 100f;
		}

		public float GetTreasureFound()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			for (int i = 0; i < this.gameItemList.Count; i++)
			{
				num2++;
				if (this.gameItemList[i].Stage > 0)
				{
					num3++;
				}
			}
			for (int j = 0; j < this.keyList.Count; j++)
			{
				num2++;
				if (this.keyList[j].Stage > 0)
				{
					num4++;
				}
			}
			for (int k = 0; k < this.chestList.Count; k++)
			{
				num2++;
				if (this.chestList[k].Stage > 0)
				{
					num5++;
				}
			}
			for (int l = 0; l < this.cageList.Count; l++)
			{
				num2++;
				if (this.cageList[l].Stage > 0)
				{
					num6++;
				}
			}
			num = num3 + num4 + num5 + num6;
			if (num2 == 0)
			{
				return 100f;
			}
			return (float)num / (float)num2 * 100f;
		}
	}
}
