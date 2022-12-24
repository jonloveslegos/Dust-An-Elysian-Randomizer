using System.Collections.Generic;
using Dust.CharClasses;

namespace Dust.Quests.SideQuests
{
	internal class _000_Main : Quest
	{
		public _000_Main()
		{
			base.SetName(this);
		}

		public override void Update()
		{
			int currentEvent = Game1.events.currentEvent;
			base.stage = 0;
			if (currentEvent >= 25)
			{
				base.stage++;
			}
			if (currentEvent >= 50)
			{
				base.stage++;
			}
			if (currentEvent >= 70)
			{
				base.stage++;
			}
			if (currentEvent >= 100)
			{
				base.stage++;
			}
			if (currentEvent >= 120)
			{
				base.stage++;
			}
			if (currentEvent >= 130)
			{
				base.stage++;
			}
			if (currentEvent >= 145)
			{
				base.stage++;
			}
			if (currentEvent >= 152)
			{
				base.stage++;
			}
			if (currentEvent >= 185)
			{
				base.stage++;
			}
			if (currentEvent >= 200)
			{
				base.stage++;
			}
			if (currentEvent >= 210)
			{
				base.stage++;
			}
			if (currentEvent >= 240)
			{
				base.stage++;
			}
			if (currentEvent >= 266)
			{
				base.stage++;
			}
			if (currentEvent >= 300)
			{
				base.stage++;
			}
			if (currentEvent >= 320)
			{
				base.stage++;
			}
			if (currentEvent >= 370)
			{
				base.stage++;
			}
			if (currentEvent >= 390)
			{
				base.stage++;
			}
			if (currentEvent >= 400)
			{
				base.stage++;
			}
			if (currentEvent >= 440)
			{
				base.stage++;
			}
			if (currentEvent >= 470)
			{
				base.stage++;
			}
			if (currentEvent >= 500)
			{
				base.stage++;
			}
			if (currentEvent >= 520)
			{
				base.stage++;
			}
			if (currentEvent >= 540)
			{
				base.stage++;
			}
			if (currentEvent >= 545)
			{
				base.stage++;
			}
			if (currentEvent >= 560)
			{
				base.stage++;
			}
			if (currentEvent >= 600)
			{
				base.stage++;
			}
			if (currentEvent >= 640)
			{
				base.stage++;
			}
			if (currentEvent >= 660)
			{
				base.stage++;
			}
			base.questGiver = CharacterType.Dust;
			List<string> list = new List<string>();
			if (currentEvent < 25)
			{
				list.Add("intro06");
			}
			else if (currentEvent < 35)
			{
				list.Add("intro10");
			}
			else if (currentEvent < 70)
			{
				list.Add("village03");
				base.questGiver = CharacterType.Bram;
			}
			else if (currentEvent < 95)
			{
				list.Add("forest10");
			}
			else if (currentEvent < 120)
			{
				list.Add("village03");
				base.questGiver = CharacterType.Bram;
			}
			else if (currentEvent < 130)
			{
				list.Add("village02");
				base.questGiver = CharacterType.Ginger;
			}
			else if (currentEvent < 140)
			{
				list.Add("village06");
			}
			else if (currentEvent < 180)
			{
				list.Add("cave12");
			}
			else if (currentEvent < 210)
			{
				list.Add("cave17");
				if (currentEvent < 200)
				{
					base.questGiver = CharacterType.Blop;
				}
			}
			else if (currentEvent < 215)
			{
				list.Add("cave12");
			}
			else if (currentEvent < 230)
			{
				list.Add("cave28");
			}
			else if (currentEvent < 266)
			{
				list.Add("cave12");
			}
			else if (currentEvent < 300)
			{
				list.Add("grave01");
			}
			else if (currentEvent < 320)
			{
				list.Add("grave06");
			}
			else if (currentEvent < 380)
			{
				base.questGiver = CharacterType.Cora;
				if (Game1.stats.Equipment[330] < 1)
				{
					list.Add("mansiona02");
				}
				if (Game1.stats.Equipment[331] < 1)
				{
					list.Add("mansionb03");
				}
				else
				{
					Game1.events.sideEventAvailable[58] = false;
				}
				if (Game1.stats.Equipment[332] < 1)
				{
					list.Add("mansionc03");
				}
				else
				{
					Game1.events.sideEventAvailable[59] = false;
				}
				if (Game1.stats.Equipment[333] < 1)
				{
					list.Add("mansiond02");
				}
				else
				{
					Game1.events.sideEventAvailable[60] = false;
				}
				if (list.Count == 0)
				{
					list.Add("grave06");
					Game1.events.currentEvent = 370;
				}
			}
			else if (currentEvent < 400)
			{
				list.Add("grave01");
			}
			else if (currentEvent < 425)
			{
				list.Add("snow06");
			}
			else if (currentEvent < 470)
			{
				list.Add("snow08");
			}
			else if (currentEvent < 500)
			{
				list.Add("snow27");
			}
			else if (currentEvent < 520)
			{
				base.questGiver = CharacterType.Elder;
				list.Add("lava01");
			}
			else if (currentEvent < 545)
			{
				if (currentEvent < 540)
				{
					list.Add("lava07");
				}
				if (!Game1.events.sideEventAvailable[62] && !Game1.events.sideEventAvailable[63])
				{
					base.questGiver = CharacterType.Elder;
					list.Add("lava01");
				}
				else if (!Game1.events.sideEventAvailable[70])
				{
					if (Game1.events.sideEventAvailable[62])
					{
						list.Add("lava04");
					}
					if (Game1.events.sideEventAvailable[63])
					{
						list.Add("lava05");
					}
				}
				else
				{
					list.Add("lava02");
				}
			}
			else if (currentEvent < 560)
			{
				list.Add("lava08");
			}
			else if (currentEvent < 570)
			{
				list.Add("lava09b");
			}
			else if (currentEvent < 580)
			{
				list.Add("lava08b");
			}
			else if (currentEvent < 620)
			{
				list.Add("lava15");
			}
			else
			{
				list.Add("lava21");
			}
			Game1.navManager.AddDestination(list);
			base.Update();
		}
	}
}
