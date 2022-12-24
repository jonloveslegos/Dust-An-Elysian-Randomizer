using Dust.MapClasses;
using Dust.Storage;

namespace Dust.NavClasses
{
	public class TreasureScript
	{
		public NavScriptLine[] Lines;

		private static int curLine;

		public TreasureScript()
		{
			this.Lines = new NavScriptLine[Game1.map.mapScript.Lines.Length];
		}

		public void DoScript(string path, RevealMap revealMap)
		{
			bool flag = false;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			TreasureScript.curLine = 0;
			while (!flag)
			{
				if (TreasureScript.curLine < this.Lines.Length - 1)
				{
					TreasureScript.curLine++;
					if (this.Lines[TreasureScript.curLine] != null)
					{
						switch (this.Lines[TreasureScript.curLine].Command)
						{
						case MapCommands.Key:
							revealMap.KeyList.Add(new SaveItem(0, path + " " + num2, 0));
							num2++;
							break;
						case MapCommands.QuestItem:
							revealMap.GameItemList.Add(new SaveItem(this.Lines[TreasureScript.curLine].IParam, path + " " + num, 0));
							num++;
							break;
						case MapCommands.QuestCreature:
							revealMap.GameItemList.Add(new SaveItem(this.Lines[TreasureScript.curLine].IParam + 1000, path + " " + num, 0));
							num++;
							break;
						case MapCommands.Upgrade:
							revealMap.GameItemList.Add(new SaveItem(this.Lines[TreasureScript.curLine].IParam + 2000, path + " " + num, 0));
							num++;
							break;
						case MapCommands.Note:
							revealMap.GameItemList.Add(new SaveItem(this.Lines[TreasureScript.curLine].IParam + 3000, path + " " + num, 0));
							num++;
							break;
						case MapCommands.Reward:
						{
							string text3 = "challenge";
							string text4 = path.Remove(0, text3.Length);
							text4 = text4.Remove(2, text4.Length - 2);
							revealMap.GameItemList.Add(new SaveItem(int.Parse(text4) + 4000, path + " " + num, 0));
							num++;
							break;
						}
						case MapCommands.Chest:
							revealMap.ChestList.Add(new SaveItem(0, path + " " + num3, 0));
							num3++;
							break;
						case MapCommands.Cage:
							revealMap.CageList.Add(new SaveItem(0, path + " " + num4, 0));
							num4++;
							break;
						case MapCommands.Destructable:
							revealMap.DestructableList.Add(new SaveItem(0, path + " " + num5, 0));
							num5++;
							break;
						case MapCommands.DestructLamp:
						{
							string text = "challenge";
							string text2 = path.Remove(0, text.Length);
							text2 = text2.Remove(2, text2.Length - 2);
							Game1.cManager.challengeArenas[int.Parse(text2)].lampList.Add(new DestructLamp(path, num6, this.Lines[TreasureScript.curLine].VParam * 2f));
							num6++;
							break;
						}
						case MapCommands.Stop:
							flag = true;
							break;
						}
					}
					continue;
				}
				flag = true;
				break;
			}
		}
	}
}
