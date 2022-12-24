using System;
using System.Collections.Generic;
using System.IO;
using Dust.HUD;
using Dust.Particles;
using Dust.PCClasses;
using Lotus.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.NavClasses
{
	public class NavManager
	{
		private static object syncObject = new object();

		public float navScale;

		public int colX = 20;

		public int colY = 20;

		private SpriteBatch sprite;

		private Texture2D navTex;

		private Texture2D nullTex;

		public WorldMap worldMap;

		public int playerX;

		public int playerY;

		public int scrollX;

		public int scrollY;

		public int mouseX;

		public int mouseY;

		public Vector2 navScroll;

		public int flagAnimFrame;

		public List<string> destMaps = new List<string>();

		private List<Vector2> destMapVectors = new List<Vector2>();

		private static string navPath = "";

		private static Dictionary<string, RevealMap> revealMap = new Dictionary<string, RevealMap>();

		private static List<string> navFiles = new List<string>();

		public Texture2D NavTex => this.navTex;

		public string NavPath => NavManager.navPath;

		public Dictionary<string, RevealMap> RevealMap => NavManager.revealMap;

		public NavManager(SpriteBatch _sprite, Texture2D _nullTex)
		{
			this.sprite = _sprite;
			this.nullTex = _nullTex;
			this.navScale = 0.5f * MathHelper.Clamp(Game1.hud.hudScale, 0.8f, 1.5f);
			this.PopulateNavs();
			this.Reset();
		}

		public void LoadContent(ContentManager content)
		{
			this.navTex = content.Load<Texture2D>("gfx/ui/nav_elements");
		}

		public void PopulateNavs()
		{
			NavManager.revealMap.Clear();
			NavManager.navFiles.Clear();
			NavManager.navFiles.Add("trial");
			NavManager.navFiles.Add("glade");
			NavManager.navFiles.Add("aurora");
			NavManager.navFiles.Add("smith");
			NavManager.navFiles.Add("forest");
			NavManager.navFiles.Add("cave");
			NavManager.navFiles.Add("grave");
			NavManager.navFiles.Add("snow");
			NavManager.navFiles.Add("lava");
			NavManager.navFiles.Add("ivy");
			NavManager.navFiles.Add("farm");
			NavManager.navFiles.Add("cove");
			NavManager.navFiles.Add("sanc");
			NavManager.navFiles.Add("castle");
			NavManager.navFiles.Add("dream");
			NavManager.navFiles.Add("z1");
			NavManager.navFiles.Add("z2");
			NavManager.navFiles.Add("z3");
			string text = ((!Game1.Xbox360 && !Game1.isPCBuild) ? "../../../../../navs/" : "data/navs/");
			string[] files = Directory.GetFiles(text);
			for (int i = 0; i < files.Length; i++)
			{
				string text2 = files[i].Remove(0, text.Length);
				string[] array = text2.Split('.');
				for (int j = 0; j < NavManager.navFiles.Count; j++)
				{
					if (!(NavManager.navFiles[j] == array[0]))
					{
						continue;
					}
					BinaryReader binaryReader = new BinaryReader(File.Open(text + array[0] + ".nav", FileMode.Open, FileAccess.Read));
					byte totalExplorable = (byte)binaryReader.ReadInt32();
					byte b = (byte)binaryReader.ReadInt32();
					byte b2 = (byte)binaryReader.ReadInt32();
					NavManager.revealMap.Add(array[0], new RevealMap(array[0], b, b2, totalExplorable));
					for (int k = 0; k < b; k++)
					{
						for (int l = 0; l < b2; l++)
						{
							int num = binaryReader.ReadInt32();
							if (num <= -1)
							{
								continue;
							}
							bool save = binaryReader.ReadBoolean();
							bool shop = binaryReader.ReadBoolean();
							int challenge = ((!binaryReader.ReadBoolean()) ? (-1) : 0);
							int treasure = binaryReader.ReadInt32();
							int entrance = binaryReader.ReadInt32();
							List<string> list = new List<string>();
							for (int m = 0; m < 3; m++)
							{
								string text3 = binaryReader.ReadString();
								if (text3 != "")
								{
									list.Add(text3);
								}
							}
							string[,] array2 = new string[3, 128];
							for (int n = 0; n < 3; n++)
							{
								for (int num2 = 0; num2 < 128; num2++)
								{
									array2[n, num2] = binaryReader.ReadString();
								}
							}
							if (list.Count > 0)
							{
								NavManager.revealMap[array[0]].Cell[k, l] = new Cell(this, list, array2, num, save, shop, challenge, treasure, entrance);
							}
							binaryReader.ReadInt32();
							binaryReader.ReadInt32();
						}
					}
					binaryReader.Close();
					this.InitMapItems(NavManager.revealMap[array[0]]);
					break;
				}
			}
			Game1.stats.GetWorldExplored();
		}

		public void Reset()
		{
			this.scrollX = (this.scrollY = 0);
			foreach (KeyValuePair<string, RevealMap> item in NavManager.revealMap)
			{
				item.Value.Reset();
			}
		}

		public void BeginReadThread(string _path, bool forcePopulate)
		{
			lock (NavManager.syncObject)
			{
				if (NavManager.navPath != _path || forcePopulate)
				{
					NavManager.navPath = _path;
					this.Read(NavManager.navPath);
				}
				else
				{
					this.SetPlayerCell();
				}
			}
		}

		public void Read(string _path)
		{
			NavManager.navPath = _path;
			if (NavManager.navPath == string.Empty)
			{
				return;
			}
			NavManager.revealMap[NavManager.navPath].Destination = false;
			for (int i = 0; i < NavManager.revealMap[NavManager.navPath].Width; i++)
			{
				for (int j = 0; j < NavManager.revealMap[NavManager.navPath].Height; j++)
				{
					if (NavManager.revealMap[NavManager.navPath].Cell[i, j] == null)
					{
						continue;
					}
					for (int k = 0; k < NavManager.revealMap[NavManager.navPath].Cell[i, j].CellMapName.Length; k++)
					{
						for (int l = 0; l < this.destMaps.Count; l++)
						{
							if (this.destMaps[l] == NavManager.revealMap[NavManager.navPath].Cell[i, j].CellMapName[k])
							{
								NavManager.revealMap[NavManager.navPath].Destination = true;
							}
						}
					}
				}
			}
			this.PopulateCells();
			if (Game1.gameMode != Game1.GameModes.WorldMap)
			{
				this.SetPlayerCell();
			}
		}

		public void WriteSaveFile(BinaryWriter writer)
		{
			foreach (KeyValuePair<string, RevealMap> item in NavManager.revealMap)
			{
				if (!(item.Key != "trial"))
				{
					continue;
				}
				writer.Write(item.Value.Explored);
				writer.Write((int)item.Value.LastVisitPos.X);
				writer.Write((int)item.Value.LastVisitPos.Y);
				writer.Write(item.Value.Updated);
				for (int i = 0; i < item.Value.Width; i++)
				{
					for (int j = 0; j < item.Value.Height; j++)
					{
						if (item.Value.Cell[i, j] != null)
						{
							writer.Write(item.Value.Cell[i, j].RevealState);
						}
					}
				}
				for (int k = 0; k < item.Value.GameItemList.Count; k++)
				{
					writer.Write(item.Value.GameItemList[k].Stage);
				}
				for (int l = 0; l < item.Value.KeyList.Count; l++)
				{
					writer.Write(item.Value.KeyList[l].Stage);
				}
				for (int m = 0; m < item.Value.ChestList.Count; m++)
				{
					writer.Write(item.Value.ChestList[m].Stage);
				}
				for (int n = 0; n < item.Value.CageList.Count; n++)
				{
					writer.Write(item.Value.CageList[n].Stage);
				}
				for (int num = 0; num < item.Value.DestructableList.Count; num++)
				{
					writer.Write(item.Value.DestructableList[num].Stage);
				}
			}
		}

		public void ReadSaveFile(BinaryReader reader)
		{
			foreach (KeyValuePair<string, RevealMap> item in NavManager.revealMap)
			{
				if (!(item.Key != "trial"))
				{
					continue;
				}
				item.Value.Explored = reader.ReadByte();
				item.Value.LastVisitPos = new Vector2(reader.ReadInt32(), reader.ReadInt32());
				item.Value.Updated = reader.ReadBoolean();
				for (int i = 0; i < item.Value.Width; i++)
				{
					for (int j = 0; j < item.Value.Height; j++)
					{
						if (item.Value.Cell[i, j] != null)
						{
							item.Value.Cell[i, j].RevealState = reader.ReadByte();
						}
					}
				}
				for (int k = 0; k < item.Value.GameItemList.Count; k++)
				{
					item.Value.GameItemList[k].Stage = reader.ReadByte();
				}
				for (int l = 0; l < item.Value.KeyList.Count; l++)
				{
					item.Value.KeyList[l].Stage = reader.ReadByte();
				}
				for (int m = 0; m < item.Value.ChestList.Count; m++)
				{
					item.Value.ChestList[m].Stage = reader.ReadByte();
				}
				for (int n = 0; n < item.Value.CageList.Count; n++)
				{
					item.Value.CageList[n].Stage = reader.ReadByte();
				}
				for (int num = 0; num < item.Value.DestructableList.Count; num++)
				{
					item.Value.DestructableList[num].Stage = reader.ReadByte();
				}
			}
			this.SetPlayerCell();
			this.ForceScroll(this.navScale, this.playerX, this.playerY);
		}

		private void PopulateCells()
		{
			this.destMapVectors.Clear();
			for (int i = 0; i < NavManager.revealMap[NavManager.navPath].Width; i++)
			{
				for (int j = 0; j < NavManager.revealMap[NavManager.navPath].Height; j++)
				{
					if (NavManager.revealMap[NavManager.navPath].Cell[i, j] == null)
					{
						continue;
					}
					int num = -1;
					for (int k = 0; k < NavManager.revealMap[NavManager.navPath].Cell[i, j].CellMapName.Length; k++)
					{
						int val = NavManager.revealMap[NavManager.navPath].Cell[i, j].StoredScripts[k].DoScript(i, j, NavManager.revealMap[NavManager.navPath].Cell[i, j].CellMapName[k], NavManager.navPath);
						num = Math.Max(val, num);
						for (int l = 0; l < this.destMaps.Count; l++)
						{
							if (NavManager.revealMap[NavManager.navPath].Cell[i, j].CellMapName[k].EndsWith(this.destMaps[l]))
							{
								this.destMapVectors.Add(new Vector2(i, j));
							}
						}
					}
					NavManager.revealMap[NavManager.navPath].Cell[i, j].Treasure = num;
				}
			}
		}

		public void SetPlayerCell()
		{
			if (NavManager.navPath == string.Empty)
			{
				return;
			}
			for (int i = 0; i < NavManager.revealMap[NavManager.navPath].Width; i++)
			{
				for (int j = 0; j < NavManager.revealMap[NavManager.navPath].Height; j++)
				{
					if (NavManager.revealMap[NavManager.navPath].Cell[i, j] == null)
					{
						continue;
					}
					for (int k = 0; k < NavManager.revealMap[NavManager.navPath].Cell[i, j].CellMapName.Length; k++)
					{
						if (NavManager.revealMap[NavManager.navPath].Cell[i, j].CellMapName[k] == Game1.map.path)
						{
							this.playerX = i;
							this.playerY = j;
							NavManager.revealMap[NavManager.navPath].Cell[i, j].RevealState = 1;
							if (NavManager.revealMap[NavManager.navPath].LastVisitPos == new Vector2(-1f, -1f))
							{
								NavManager.revealMap[NavManager.navPath].LastVisitPos = new Vector2(0f, 0f);
							}
							if (NavManager.revealMap[NavManager.navPath].Cell[i, j].Entrance > -1)
							{
								NavManager.revealMap[NavManager.navPath].Visiting = true;
								NavManager.revealMap[NavManager.navPath].LastVisitPos = new Vector2(i, j);
								this.RevealExits(NavManager.navPath, NavManager.revealMap[NavManager.navPath].Cell, i - 1, j);
								this.RevealExits(NavManager.navPath, NavManager.revealMap[NavManager.navPath].Cell, i + 1, j);
								this.RevealExits(NavManager.navPath, NavManager.revealMap[NavManager.navPath].Cell, i, j - 1);
								this.RevealExits(NavManager.navPath, NavManager.revealMap[NavManager.navPath].Cell, i, j + 1);
							}
							this.UpdateExplored(NavManager.navPath, NavManager.revealMap[NavManager.navPath].Cell);
							break;
						}
					}
				}
			}
		}

		public void RevealCell(string mapName)
		{
			int retX = 0;
			int retY = 0;
			string retNav = string.Empty;
			this.RevealCell(mapName, ref retX, ref retY, ref retNav);
			if (Game1.gameMode != Game1.GameModes.WorldMap)
			{
				return;
			}
			if (this.worldMap != null)
			{
				int num = 0;
				while (Game1.questManager.updatingQuests)
				{
					num++;
					if (num > 1000)
					{
						break;
					}
				}
			}
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(Game1.stats.GetWorldExplored)));
			this.worldMap.PopulateAvailable(moveCursor: false);
			this.worldMap.PopulateControls();
		}

		public void RevealCell(string mapName, ref int retX, ref int retY, ref string retNav)
		{
			try
			{
				lock (NavManager.syncObject)
				{
					foreach (KeyValuePair<string, RevealMap> item in NavManager.revealMap)
					{
						for (int i = 0; i < item.Value.Width; i++)
						{
							for (int j = 0; j < item.Value.Height; j++)
							{
								if (item.Value.Cell[i, j] == null)
								{
									continue;
								}
								for (int k = 0; k < item.Value.Cell[i, j].CellMapName.Length; k++)
								{
									if (!(mapName == item.Value.Cell[i, j].CellMapName[k]))
									{
										continue;
									}
									if (NavManager.revealMap[item.Key].Cell[i, j] != null && NavManager.revealMap[item.Key].Cell[i, j].RevealState == 0)
									{
										NavManager.revealMap[item.Key].Updated = true;
										if (NavManager.revealMap[item.Key].LastVisitPos == new Vector2(-1f, -1f))
										{
											Game1.hud.InitMiniPrompt(MiniPromptType.RegionAdded, 0, blueprint: false);
										}
										NavManager.revealMap[item.Key].Cell[i, j].RevealState = 1;
									}
									retNav = item.Key;
									retX = i;
									retY = j;
									if (item.Value.Cell[i, j].Entrance > -1)
									{
										this.RevealExits(item.Key, item.Value.Cell, i - 1, j);
										this.RevealExits(item.Key, item.Value.Cell, i + 1, j);
										this.RevealExits(item.Key, item.Value.Cell, i, j - 1);
										this.RevealExits(item.Key, item.Value.Cell, i, j + 1);
									}
									this.UpdateExplored(item.Key, item.Value.Cell);
									break;
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void UpdateExplored(string path, Cell[,] cell)
		{
			NavManager.revealMap[path].Explored = 0;
			for (int i = 0; i < NavManager.revealMap[path].Width; i++)
			{
				for (int j = 0; j < NavManager.revealMap[path].Height; j++)
				{
					if (NavManager.revealMap[path].Cell[i, j] != null && NavManager.revealMap[path].Cell[i, j].RevealState > 0 && cell[i, j] != null && cell[i, j].CellType < 16)
					{
						NavManager.revealMap[path].Explored++;
					}
				}
			}
		}

		private void RevealExits(string path, Cell[,] cells, int x, int y)
		{
			if (x > -1 && y > -1 && x < NavManager.revealMap[path].Width && y < NavManager.revealMap[path].Height && cells[x, y] != null && cells[x, y].CellType >= 16)
			{
				NavManager.revealMap[path].Cell[x, y].RevealState = 1;
			}
		}

		public List<Vector3> PopulateEntrances(List<Vector3> entranceList)
		{
			entranceList.Clear();
			for (int i = 0; i < NavManager.revealMap[NavManager.navPath].Width; i++)
			{
				for (int j = 0; j < NavManager.revealMap[NavManager.navPath].Height; j++)
				{
					if (NavManager.revealMap[NavManager.navPath].Cell[i, j] != null && NavManager.revealMap[NavManager.navPath].Cell[i, j].CellType < 16 && NavManager.revealMap[NavManager.navPath].Cell[i, j].Entrance > -1 && NavManager.revealMap[NavManager.navPath].Cell[i, j].RevealState > 0)
					{
						entranceList.Add(new Vector3(i, j, NavManager.revealMap[NavManager.navPath].Cell[i, j].Entrance));
					}
				}
			}
			return entranceList;
		}

		public void CheckRegionTreasure(ParticleManager pMan)
		{
			bool flag = false;
			for (int i = 0; i < NavManager.revealMap[NavManager.navPath].Cell[this.playerX, this.playerY].CellMapName.Length; i++)
			{
				if (NavManager.revealMap[NavManager.navPath].Cell[this.playerX, this.playerY].CellMapName[i].Contains("alt"))
				{
					continue;
				}
				for (int j = 0; j < NavManager.revealMap[NavManager.navPath].GameItemList.Count; j++)
				{
					if (!NavManager.revealMap[NavManager.navPath].GameItemList[j].UniqueID.Contains("alt") && NavManager.revealMap[NavManager.navPath].GameItemList[j].UniqueID.Split(' ')[0] == NavManager.revealMap[NavManager.navPath].Cell[this.playerX, this.playerY].CellMapName[i] && NavManager.revealMap[NavManager.navPath].GameItemList[j].Stage == 0)
					{
						flag = true;
						break;
					}
				}
				for (int k = 0; k < NavManager.revealMap[NavManager.navPath].KeyList.Count; k++)
				{
					if (!NavManager.revealMap[NavManager.navPath].KeyList[k].UniqueID.Contains("alt") && NavManager.revealMap[NavManager.navPath].KeyList[k].UniqueID.Split(' ')[0] == NavManager.revealMap[NavManager.navPath].Cell[this.playerX, this.playerY].CellMapName[i] && NavManager.revealMap[NavManager.navPath].KeyList[k].Stage == 0)
					{
						flag = true;
						break;
					}
				}
				for (int l = 0; l < NavManager.revealMap[NavManager.navPath].ChestList.Count; l++)
				{
					if (!NavManager.revealMap[NavManager.navPath].ChestList[l].UniqueID.Contains("alt") && NavManager.revealMap[NavManager.navPath].ChestList[l].UniqueID.Split(' ')[0] == NavManager.revealMap[NavManager.navPath].Cell[this.playerX, this.playerY].CellMapName[i] && NavManager.revealMap[NavManager.navPath].ChestList[l].Stage == 0)
					{
						flag = true;
						break;
					}
				}
				for (int l = 0; l < NavManager.revealMap[NavManager.navPath].DestructableList.Count; l++)
				{
					if (!NavManager.revealMap[NavManager.navPath].DestructableList[l].UniqueID.Contains("alt") && NavManager.revealMap[NavManager.navPath].DestructableList[l].UniqueID.Split(' ')[0] == NavManager.revealMap[NavManager.navPath].Cell[this.playerX, this.playerY].CellMapName[i] && NavManager.revealMap[NavManager.navPath].DestructableList[l].Stage == 0)
					{
						flag = true;
						break;
					}
				}
				for (int m = 0; m < NavManager.revealMap[NavManager.navPath].CageList.Count; m++)
				{
					if (!NavManager.revealMap[NavManager.navPath].CageList[m].UniqueID.Contains("alt") && NavManager.revealMap[NavManager.navPath].CageList[m].UniqueID.Split(' ')[0] == NavManager.revealMap[NavManager.navPath].Cell[this.playerX, this.playerY].CellMapName[i] && NavManager.revealMap[NavManager.navPath].CageList[m].Stage == 0)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				NavManager.revealMap[NavManager.navPath].Cell[this.playerX, this.playerY].Treasure = 1;
			}
			else
			{
				NavManager.revealMap[NavManager.navPath].Cell[this.playerX, this.playerY].Treasure = 0;
			}
		}

		public bool CheckLocalTreasure()
		{
			return false;
		}

		public void AddDestination(List<string> maps)
		{
			this.destMaps.Clear();
			for (int i = 0; i < maps.Count; i++)
			{
				this.destMaps.Add(maps[i]);
			}
			this.BeginReadThread(NavManager.navPath, forcePopulate: true);
		}

		public void RemoveDestination(string[] maps)
		{
			for (int i = 0; i < maps.Length; i++)
			{
				this.destMaps.Remove(maps[i]);
			}
			this.BeginReadThread(NavManager.navPath, forcePopulate: true);
		}

		public void InitMapItems(RevealMap revealMap)
		{
			revealMap.GameItemList.Clear();
			revealMap.KeyList.Clear();
			revealMap.ChestList.Clear();
			revealMap.CageList.Clear();
			revealMap.DestructableList.Clear();
			for (int i = 0; i < revealMap.Width; i++)
			{
				for (int j = 0; j < revealMap.Height; j++)
				{
					if (revealMap.Cell[i, j] == null)
					{
						continue;
					}
					revealMap.Cell[i, j].Treasure = -1;
					for (int k = 0; k < revealMap.Cell[i, j].CellMapName.Length; k++)
					{
						if (revealMap.Cell[i, j].CellMapName[k].Contains("alt"))
						{
							continue;
						}
						TreasureScript treasureScript = new TreasureScript();
						for (int l = 0; l < revealMap.Cell[i, j].StoredScripts[k].Lines.Length; l++)
						{
							NavScriptLine navScriptLine = revealMap.Cell[i, j].StoredScripts[k].Lines[l];
							if (navScriptLine != null)
							{
								treasureScript.Lines[l] = navScriptLine;
							}
							else
							{
								treasureScript.Lines[l] = null;
							}
						}
						treasureScript.DoScript(revealMap.Cell[i, j].CellMapName[k], revealMap);
					}
				}
			}
			for (int m = 0; m < revealMap.KeyList.Count; m++)
			{
				revealMap.KeyList[m].ID = m;
			}
			for (int n = 0; n < revealMap.ChestList.Count; n++)
			{
				revealMap.ChestList[n].ID = n;
			}
			for (int num = 0; num < revealMap.CageList.Count; num++)
			{
				revealMap.CageList[num].ID = num;
			}
			for (int num2 = 0; num2 < revealMap.DestructableList.Count; num2++)
			{
				revealMap.DestructableList[num2].ID = num2;
			}
		}

		public void InitMapItems2(List<string> mapNameList, RevealMap revealMap)
		{
			revealMap.GameItemList.Clear();
			revealMap.KeyList.Clear();
			revealMap.ChestList.Clear();
			revealMap.CageList.Clear();
			revealMap.DestructableList.Clear();
			for (int i = 0; i < mapNameList.Count; i++)
			{
				if (mapNameList[i].Contains("alt"))
				{
					mapNameList[i] = mapNameList[i].Replace("alt", ".");
					mapNameList[i] = mapNameList[i].Split('.')[0];
				}
			}
			for (int j = 0; j < mapNameList.Count; j++)
			{
				for (int k = 0; k < mapNameList.Count; k++)
				{
					if (mapNameList[j] == mapNameList[k] && j != k)
					{
						mapNameList.Remove(mapNameList[j]);
						break;
					}
				}
			}
			string text = ((!Game1.Xbox360 && !Game1.isPCBuild) ? "../../../../../maps/" : "data/maps/");
			string[] files = Directory.GetFiles(text);
			for (int l = 0; l < files.Length; l++)
			{
				bool flag = false;
				string[] array = files[l].Remove(0, text.Length).Split('.');
				for (int m = 0; m < mapNameList.Count; m++)
				{
					if (array[0] == mapNameList[m])
					{
						flag = true;
					}
				}
				if (!flag)
				{
					continue;
				}
				try
				{
					BinaryReader binaryReader = new BinaryReader(File.Open(files[l], FileMode.Open, FileAccess.Read));
					int num = binaryReader.ReadInt32();
					binaryReader.ReadInt32();
					for (int n = 0; n < num; n++)
					{
						int num2 = binaryReader.ReadInt32();
						for (int num3 = 0; num3 < num2; num3++)
						{
							binaryReader.ReadSingle();
							binaryReader.ReadSingle();
						}
						binaryReader.ReadInt32();
					}
					TreasureScript treasureScript = new TreasureScript();
					for (int num4 = 0; num4 < treasureScript.Lines.Length; num4++)
					{
						string text2 = binaryReader.ReadString();
						if (text2.Length > 0)
						{
							treasureScript.Lines[num4] = new NavScriptLine(text2);
						}
						else
						{
							treasureScript.Lines[num4] = null;
						}
					}
					binaryReader.Close();
					array[0] = array[0].Replace("alt", ".");
					treasureScript.DoScript(array[0].Split('.')[0], revealMap);
				}
				catch (Exception)
				{
				}
			}
			for (int num5 = 0; num5 < revealMap.KeyList.Count; num5++)
			{
				revealMap.KeyList[num5].ID = num5;
			}
			for (int num6 = 0; num6 < revealMap.ChestList.Count; num6++)
			{
				revealMap.ChestList[num6].ID = num6;
			}
			for (int num7 = 0; num7 < revealMap.CageList.Count; num7++)
			{
				revealMap.CageList[num7].ID = num7;
			}
			for (int num8 = 0; num8 < revealMap.DestructableList.Count; num8++)
			{
				revealMap.DestructableList[num8].ID = num8;
			}
		}

		public void ForceScroll(float scale, int selectX, int selectY)
		{
			int num = (int)(64f * scale);
			this.navScroll = new Vector2(selectX * num, selectY * num);
		}

		public void LimitScroll(float scale, int selectX, int selectY, int xLimit, int yLimit)
		{
			int num = (int)(64f * scale);
			this.navScroll = new Vector2(MathHelper.Clamp(this.navScroll.X, (selectX - xLimit) * num, (selectX + xLimit) * num), MathHelper.Clamp(this.navScroll.Y, (selectY - yLimit) * num, (selectY + yLimit) * num));
		}

		public void InitWorldMap(string _curPath, bool _canReturn)
		{
			if (this.worldMap == null)
			{
				this.worldMap = new WorldMap(this.sprite, this.nullTex, Game1.character, Game1.map);
			}
			this.worldMap.InitWorldMap(Game1.pManager, _curPath, _canReturn);
		}

		public void ExitWorldMap()
		{
			this.worldMap = null;
		}

		public void DrawNavTarget()
		{
			try
			{
				this.Draw(new Vector2(88f, 56f), 0.5f, this.playerX, this.playerY, 9, 7, Color.White, 0.4f, 0.75f, background: false, entrances: false);
			}
			catch (Exception)
			{
			}
		}

		public void Draw(Vector2 pos, float scale, int selectX, int selectY, int width, int height, Color color, float blinkAlpha, float flagScale, bool background, bool entrances)
		{
			if (NavManager.navPath == string.Empty)
			{
				return;
			}
			if (Game1.skipFrame > 1)
			{
				this.flagAnimFrame++;
				if (this.flagAnimFrame > 23)
				{
					this.flagAnimFrame = 0;
				}
			}
			float num = (float)(int)color.A / 255f;
			if (num <= 0f)
			{
				return;
			}
			int num2 = (int)(64f * scale);
			if (background)
			{
				Rectangle destinationRectangle = new Rectangle((int)(pos.X - (float)(width * num2 / 2)), (int)(pos.Y - (float)(height * num2 / 2)), width * num2, height * num2);
				this.sprite.Draw(this.nullTex, destinationRectangle, new Color(0f, 0f, 0f, 0.5f * num));
			}
			Vector2 vector = new Vector2(selectX * num2, selectY * num2);
			if (Game1.map.GetTransVal() == 0f)
			{
				byte b = (byte)((Game1.hud.inventoryState == InventoryState.None) ? 8u : 12u);
				this.navScroll += (vector - this.navScroll) * Game1.HudTime * (int)b;
			}
			Vector2 vector2 = pos - this.navScroll - new Vector2(num2, num2) / 2f;
			int num3 = selectX - width / 2 - (int)(vector.X - this.navScroll.X) / num2;
			int num4 = selectY - height / 2 - (int)(vector.Y - this.navScroll.Y) / num2;
			int num5 = num3 + width;
			int num6 = num4 + height;
			int num7 = (int)MathHelper.Clamp(num3, 0f, (int)NavManager.revealMap[NavManager.navPath].Width);
			int num8 = (int)MathHelper.Clamp(num4, 0f, (int)NavManager.revealMap[NavManager.navPath].Height);
			int num9 = (int)MathHelper.Clamp(num5, 0f, (int)NavManager.revealMap[NavManager.navPath].Width);
			int num10 = (int)MathHelper.Clamp(num6, 0f, (int)NavManager.revealMap[NavManager.navPath].Height);
			Color color2 = new Color(1f, 1f, 1f, 0.1f * num);
			Vector2 vector3 = pos - new Vector2(width, height) / 2f * num2;
			for (int i = num3 + 1; i < num3 + width; i++)
			{
				this.sprite.Draw(this.nullTex, new Rectangle((int)((float)(i * num2) + vector2.X), (int)vector3.Y, 1, num2 * height), color2);
			}
			for (int j = num4 + 1; j < num4 + height; j++)
			{
				this.sprite.Draw(this.nullTex, new Rectangle((int)vector3.X, (int)((float)(j * num2) + vector2.Y), num2 * width, 1), color2);
			}
			bool flag = new Vector2((float)Math.Round(this.navScroll.X), (float)Math.Round(this.navScroll.Y)) != vector;
			this.mouseX = (this.mouseY = -1);
			for (int k = num7; k < num9; k++)
			{
				for (int l = num8; l < num10; l++)
				{
					if (k < 0 || k >= this.colX || l < 0 || l >= this.colY || NavManager.revealMap[NavManager.navPath].Cell[k, l] == null)
					{
						continue;
					}
					bool flag2 = NavManager.revealMap[NavManager.navPath].Cell[k, l].RevealState > 0;
					bool flag3 = false;
					for (int m = 0; m < this.destMapVectors.Count; m++)
					{
						if ((float)k == this.destMapVectors[m].X && (float)l == this.destMapVectors[m].Y)
						{
							flag3 = true;
						}
					}
					if (!flag2 && !flag3)
					{
						continue;
					}
					Rectangle rectangle = new Rectangle((int)((float)(k * num2) + vector2.X), (int)((float)(l * num2) + vector2.Y), num2, num2);
					float num11 = num;
					Color color3;
					if (flag)
					{
						if (this.navScroll.X > vector.X && k == num3)
						{
							num11 -= MathHelper.Clamp(1f - 0.5f / (this.navScroll.X - vector.X), 0f, 1f);
						}
						else if (this.navScroll.X < vector.X && k == num3 + width - 1)
						{
							num11 -= MathHelper.Clamp(1f - 0.5f / Math.Abs(this.navScroll.X - vector.X), 0f, 1f);
						}
						if (this.navScroll.Y > vector.Y && l == num4)
						{
							num11 -= MathHelper.Clamp(1f - 0.2f / (this.navScroll.Y - vector.Y), 0f, 1f);
						}
						else if (this.navScroll.Y < vector.Y && l == num4 + height - 1)
						{
							num11 -= MathHelper.Clamp(1f - 0.2f / Math.Abs(this.navScroll.Y - vector.Y), 0f, 1f);
						}
						color3 = new Color((int)color.R, (int)color.G, (int)color.B, num11);
					}
					else
					{
						color3 = color;
					}
					if (flag2)
					{
						this.DrawCell(NavManager.revealMap[NavManager.navPath].Cell[k, l].CellType, new Vector2(rectangle.X, rectangle.Y), scale, color3);
						if (NavManager.revealMap[NavManager.navPath].Cell[k, l].Save)
						{
							if (NavManager.navPath != "trial")
							{
								this.sprite.Draw(this.navTex, new Vector2(rectangle.X, rectangle.Y) + new Vector2(34f, 34f) * scale, new Rectangle(688, 16, 58, 58), new Color(0f, 1f, 1f, 0.6f * num11), 0f, new Vector2(35f, 35f), scale, SpriteEffects.None, 0f);
							}
							if (NavManager.revealMap[NavManager.navPath].Cell[k, l].Shop)
							{
								this.sprite.Draw(this.navTex, new Vector2(rectangle.X, rectangle.Y) + new Vector2(34f, 34f) * scale, new Rectangle(688, 16, 58, 32), new Color(1f, 1f, 0f, 0.6f * num11), 0f, new Vector2(35f, 35f), scale, SpriteEffects.None, 0f);
							}
						}
						else if (NavManager.revealMap[NavManager.navPath].Cell[k, l].Shop)
						{
							this.sprite.Draw(this.navTex, new Vector2(rectangle.X, rectangle.Y) + new Vector2(34f, 34f) * scale, new Rectangle(688, 16, 58, 58), new Color(1f, 1f, 0f, 0.6f * num11), 0f, new Vector2(35f, 35f), scale, SpriteEffects.None, 0f);
						}
						if (NavManager.revealMap[NavManager.navPath].Cell[k, l].Challenge > -2)
						{
							this.sprite.Draw(this.navTex, new Vector2(rectangle.X, rectangle.Y) + new Vector2(34f, 34f) * scale, new Rectangle(688, 16, 58, 58), new Color(1f, 0.4f, 0.4f, 0.6f * num11), 0f, new Vector2(35f, 35f), scale, SpriteEffects.None, 0f);
						}
						if (NavManager.revealMap[NavManager.navPath].Cell[k, l].Treasure == 0)
						{
							this.sprite.Draw(this.navTex, new Vector2((float)rectangle.X + 32f * scale, (float)rectangle.Y + 32f * scale), new Rectangle(656, 32, 32, 32), color3, 0f, new Vector2(17f, 17f), 1f, SpriteEffects.None, 0f);
						}
						else if (NavManager.revealMap[NavManager.navPath].Cell[k, l].Treasure == 1)
						{
							this.sprite.Draw(this.navTex, new Vector2((float)rectangle.X + 32f * scale, (float)rectangle.Y + 32f * scale), new Rectangle(656, 0, 32, 32), color3, 0f, new Vector2(16f, 16f), scale, SpriteEffects.None, 0f);
						}
						if (NavManager.revealMap[NavManager.navPath].Cell[k, l].Challenge > -1)
						{
							float num12 = Math.Min((float)Game1.cManager.challengeArenas[NavManager.revealMap[NavManager.navPath].Cell[k, l].Challenge].HighScore / (float)(Game1.cManager.challengeArenas[NavManager.revealMap[NavManager.navPath].Cell[k, l].Challenge].RankScore - 1) * 3f, 4f);
							int num13 = 0;
							for (int n = 0; (float)n < num12; n++)
							{
								num13++;
							}
							for (int num14 = 0; num14 < num13; num14++)
							{
								int num15 = 0;
								if (num14 == 0 || num14 == num13 - 1)
								{
									num15 = 4;
								}
								this.sprite.Draw(this.navTex, new Vector2((float)rectangle.X + (float)((num14 - 1) * 12 + 38 - (num13 - 2) * 6) * scale, (float)rectangle.Y + (float)(56 - num15) * scale), new Rectangle(656, 64, 32, 32), color3, 0f, new Vector2(16f, 16f), scale, SpriteEffects.None, 0f);
							}
						}
						if (entrances)
						{
							if (k == selectX && l == selectY && Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
							{
								this.sprite.Draw(this.navTex, new Vector2(rectangle.X, rectangle.Y) + new Vector2(34f, 34f) * scale, new Rectangle(688, 16, 58, 58), new Color(1f, 1f, 1f, (float)Math.Cos(Game1.hud.pulse * 8f) * num11 * blinkAlpha * 4f), 0f, new Vector2(35f, 35f), scale, SpriteEffects.None, 0f);
							}
							else if (NavManager.revealMap[NavManager.navPath].Cell[k, l].Entrance > -1)
							{
								if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && Game1.gameMode == Game1.GameModes.WorldMap && new Rectangle(rectangle.X, rectangle.Y, (int)(64f * scale), (int)(64f * scale)).Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y))
								{
									this.mouseX = k;
									this.mouseY = l;
									if (Game1.pcManager.leftMouseClicked)
									{
										Game1.pcManager.leftMouseClicked = false;
										Game1.navManager.worldMap.KeySelect = true;
									}
								}
								if (this.mouseX == k && this.mouseY == l)
								{
									this.sprite.Draw(this.navTex, new Vector2(rectangle.X, rectangle.Y) + new Vector2(34f, 34f) * scale, new Rectangle(688, 16, 58, 58), new Color(1f, 1f, 1f, (float)Math.Cos(Game1.hud.pulse * 8f) * num11 * blinkAlpha * 4f), 0f, new Vector2(35f, 35f), scale * 1.5f, SpriteEffects.None, 0f);
								}
								else
								{
									this.sprite.Draw(this.navTex, new Vector2(rectangle.X, rectangle.Y) + new Vector2(34f, 34f) * scale, new Rectangle(688, 16, 58, 58), new Color(1f, 1f, 1f, (float)Math.Cos(Game1.hud.pulse * 8f) * num11 * blinkAlpha), 0f, new Vector2(35f, 35f), scale, SpriteEffects.None, 0f);
								}
							}
						}
						else if (this.playerX == k && this.playerY == l)
						{
							this.sprite.Draw(this.navTex, new Vector2(rectangle.X, rectangle.Y) + new Vector2(34f, 34f) * scale, new Rectangle(688, 16, 58, 58), new Color(1f, 1f, 1f, (float)Math.Cos(Game1.hud.pulse * 4f) * num11 * blinkAlpha), 0f, new Vector2(35f, 35f), scale, SpriteEffects.None, 0f);
						}
					}
					if (flag3)
					{
						this.sprite.Draw(this.navTex, new Vector2(rectangle.X, rectangle.Y) + new Vector2(32f, 32f) * scale, new Rectangle(64 * this.flagAnimFrame - 768 * (this.flagAnimFrame / 12), 96 + 48 * (this.flagAnimFrame / 12), 64, 48), color3, 0f, new Vector2(55f, 45f), flagScale, SpriteEffects.FlipHorizontally, 0f);
					}
				}
			}
		}

		private void DrawCell(int type, Vector2 pos, float scale, Color color)
		{
			float rotation = 0f;
			byte b = 0;
			switch (type)
			{
			case 1:
				rotation = 1.57f;
				break;
			case 2:
				rotation = 3.14f;
				break;
			case 3:
				rotation = 4.71f;
				break;
			case 4:
				b = 1;
				break;
			case 5:
				b = 1;
				rotation = 1.57f;
				break;
			case 6:
				b = 2;
				break;
			case 7:
				b = 2;
				rotation = 1.57f;
				break;
			case 8:
				b = 2;
				rotation = 3.14f;
				break;
			case 9:
				b = 2;
				rotation = 4.71f;
				break;
			case 10:
				b = 3;
				break;
			case 11:
				b = 3;
				rotation = 1.57f;
				break;
			case 12:
				b = 3;
				rotation = 3.14f;
				break;
			case 13:
				b = 3;
				rotation = 4.71f;
				break;
			case 14:
				b = 4;
				break;
			case 15:
				b = 6;
				break;
			case 16:
				b = 5;
				break;
			case 17:
				b = 5;
				rotation = 1.57f;
				break;
			case 18:
				b = 5;
				rotation = 3.14f;
				break;
			case 19:
				b = 5;
				rotation = 4.71f;
				break;
			}
			this.sprite.Draw(this.navTex, pos + new Vector2(32f, 32f) * scale, new Rectangle(b * 96 + 13, 13, 70, 70), color, rotation, new Vector2(35f, 35f), scale, SpriteEffects.None, 0f);
		}
	}
}
