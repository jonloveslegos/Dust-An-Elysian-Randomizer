using System.Collections.Generic;
using Dust.CharClasses;
using Dust.NavClasses;

namespace Dust.MapClasses.bucket
{
	public class Party
	{
		public int[] Name;

		public void GetParty(string type)
		{
			List<CharacterType> list = new List<CharacterType>();
			switch (type)
			{
			case "friends":
			{
				List<CharacterType> list2 = new List<CharacterType>();
				int num3 = 0;
				foreach (KeyValuePair<string, RevealMap> item in Game1.navManager.RevealMap)
				{
					for (int num4 = 0; num4 < item.Value.CageList.Count; num4++)
					{
						if (item.Value.CageList[num4].Stage > 0)
						{
							CharacterType friend = Game1.stats.GetFriend(item.Value.CageList[num4].UniqueID);
							list2.Add(friend);
							if (friend == CharacterType.HyperChris || friend == CharacterType.HyperDan)
							{
								num3++;
							}
						}
					}
				}
				if (num3 > 1)
				{
					for (int num5 = 0; num5 < list2.Count; num5++)
					{
						if (list2[num5] != CharacterType.HyperChris && list2[num5] != CharacterType.HyperDan)
						{
							list.Add(list2[num5]);
						}
					}
				}
				else
				{
					for (int num6 = 0; num6 < list2.Count; num6++)
					{
						list.Add(list2[num6]);
					}
				}
				break;
			}
			case "imp":
			{
				for (int num14 = 0; num14 < 4; num14++)
				{
					list.Add(CharacterType.Imp);
				}
				for (int num15 = 0; num15 < 2; num15++)
				{
					list.Add(CharacterType.LightBeast);
				}
				break;
			}
			case "light":
			{
				for (int k = 0; k < 2; k++)
				{
					list.Add(CharacterType.Imp);
				}
				for (int l = 0; l < 4; l++)
				{
					list.Add(CharacterType.LightBeast);
				}
				break;
			}
			case "avee":
			{
				for (int num8 = 0; num8 < 5; num8++)
				{
					list.Add(CharacterType.Avee);
				}
				break;
			}
			case "florn":
			{
				for (int num = 0; num < 3; num++)
				{
					list.Add(CharacterType.Florn);
				}
				break;
			}
			case "squirtbug":
			{
				for (int num16 = 0; num16 < 3; num16++)
				{
					list.Add(CharacterType.SquirtBug);
				}
				break;
			}
			case "hound":
			{
				for (int num12 = 0; num12 < 5; num12++)
				{
					list.Add(CharacterType.RockHound);
				}
				break;
			}
			case "cutter":
				list.Add(CharacterType.StoneCutter);
				list.Add(CharacterType.RockHound);
				list.Add(CharacterType.RockHound);
				list.Add(CharacterType.RockHound);
				break;
			case "singlegiant":
				list.Add(CharacterType.Imp);
				list.Add(CharacterType.Imp);
				list.Add(CharacterType.LightBeast);
				list.Add(CharacterType.LightBeast);
				list.Add(CharacterType.Giant);
				break;
			case "wolves":
			{
				for (int m = 0; m < 2; m++)
				{
					list.Add(CharacterType.LightBeastSnow);
				}
				for (int n = 0; n < 3; n++)
				{
					list.Add(CharacterType.Wolf);
				}
				break;
			}
			case "implava":
			{
				for (int num17 = 0; num17 < 4; num17++)
				{
					list.Add(CharacterType.ImpLava);
				}
				break;
			}
			case "soldiers":
			{
				for (int num13 = 0; num13 < 4; num13++)
				{
					list.Add(CharacterType.Soldier);
				}
				list.Add(CharacterType.WolfSoldier);
				break;
			}
			case "soldierwolves":
			{
				list.Add(CharacterType.Soldier);
				for (int num11 = 0; num11 < 4; num11++)
				{
					list.Add(CharacterType.WolfSoldier);
				}
				break;
			}
			case "bunnyparty":
			{
				for (int num10 = 0; num10 < 3; num10++)
				{
					list.Add(CharacterType.Bunny);
				}
				break;
			}
			case "bunnysnowparty":
			{
				for (int num9 = 0; num9 < 3; num9++)
				{
					list.Add(CharacterType.BunnySnow);
				}
				break;
			}
			case "deerparty":
			{
				for (int num7 = 0; num7 < 3; num7++)
				{
					list.Add(CharacterType.Deer);
				}
				break;
			}
			case "deersnowparty":
			{
				for (int num2 = 0; num2 < 3; num2++)
				{
					list.Add(CharacterType.DeerSnow);
				}
				break;
			}
			case "remains":
			{
				for (int i = 0; i < 3; i++)
				{
					list.Add(CharacterType.Remains);
				}
				for (int j = 0; j < 2; j++)
				{
					list.Add(CharacterType.RemainsHalf);
				}
				break;
			}
			default:
				this.Name = null;
				return;
			}
			this.Name = new int[list.Count];
			for (int num18 = 0; num18 < this.Name.Length; num18++)
			{
				this.Name[num18] = (int)list[num18];
			}
		}
	}
}
