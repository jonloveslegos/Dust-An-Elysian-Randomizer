using System;
using Dust.MapClasses;

namespace Dust.NavClasses
{
	public class NavScript
	{
		public NavScriptLine[] Lines;

		private NavManager navMap;

		private static int curLine;

		public NavScript(NavManager _navMap)
		{
			this.navMap = _navMap;
			this.Lines = new NavScriptLine[128];
		}

		public int DoScript(int x, int y, string path, string curNav)
		{
			int num = -1;
			int num2 = -1;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num8 = 0;
			bool flag = false;
			NavScript.curLine = 0;
			int num7 = -1;
			if (path.StartsWith("challenge"))
			{
				string text = "challenge";
				string text2 = path.Remove(0, text.Length);
				num7 = int.Parse(text2.Remove(2, text2.Length - 2));
			}
			while (!flag)
			{
				if (NavScript.curLine < this.Lines.Length - 1)
				{
					NavScript.curLine++;
					try
					{
						if (this.Lines[NavScript.curLine] == null)
						{
							continue;
						}
						switch (this.Lines[NavScript.curLine].Command)
						{
						case MapCommands.Key:
							num2 = ((Game1.savegame.GetItemID(this.navMap.RevealMap[curNav].KeyList, path + " " + num4).Stage == 0) ? 1 : 0);
							num4++;
							break;
						case MapCommands.Upgrade:
						case MapCommands.Note:
						case MapCommands.QuestItem:
						case MapCommands.QuestCreature:
							num2 = ((Game1.savegame.GetItemID(this.navMap.RevealMap[curNav].GameItemList, path + " " + num3).Stage == 0) ? 1 : 0);
							num3++;
							break;
						case MapCommands.Destructable:
							num2 = ((Game1.savegame.GetItemID(this.navMap.RevealMap[curNav].DestructableList, path + " " + num8).Stage == 0) ? 1 : 0);
							num8++; 
							break;
						case MapCommands.Chest:
							num2 = ((Game1.savegame.GetItemID(this.navMap.RevealMap[curNav].ChestList, path + " " + num5).Stage == 0) ? 1 : 0);
							num5++;
							break;
						case MapCommands.Cage:
							num2 = ((Game1.savegame.GetItemID(this.navMap.RevealMap[curNav].CageList, path + " " + num6).Stage == 0) ? 1 : 0);
							num6++;
							break;
						case MapCommands.Reward:
							if (num7 > -1)
							{
								num2 = ((Game1.savegame.GetItemID(this.navMap.RevealMap[curNav].GameItemList, path + " " + num6).Stage == 0) ? 1 : 0);
							}
							num6++;
							break;
						case MapCommands.Stop:
							flag = true;
							break;
						}
						if (num2 > num)
						{
							num = num2;
						}
					}
					catch (Exception)
					{
					}
					continue;
				}
				flag = true;
				break;
			}
			return num;
		}
	}
}
