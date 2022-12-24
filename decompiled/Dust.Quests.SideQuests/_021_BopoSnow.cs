using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _021_BopoSnow : Quest
	{
		public _021_BopoSnow()
		{
			base.SetName(this);
		}

		public override void ResetQuestGiver()
		{
			base.questGiver = CharacterType.Bopo;
		}

		public override CharacterType GetInitialQuestGiver()
		{
			if (Game1.events.currentEvent >= 250)
			{
				return CharacterType.Bopo;
			}
			return CharacterType.Dust;
		}

		public override void Update()
		{
			base.questGiver = CharacterType.Dust;
			if (base.stage == 1)
			{
				base.questGiver = CharacterType.Bopo;
			}
			if (base.stage == 0 && Game1.stats.Equipment[308] > 0)
			{
				base.stage = 1;
			}
			if (base.stage == 1 && Game1.stats.Equipment[342] > 0)
			{
				base.stage = 2;
			}
			base.requirementMet = base.stage > 1;
			base.Update();
		}

		public override void Complete()
		{
			Game1.stats.GetChestFromFile("Equip " + Game1.map.path + " " + EquipType.RainbowKey.ToString() + " " + 1.ToString(), Game1.pManager);
			Game1.stats.AcquireEquip(EquipType.CoolerSnow, -1, _bluePrint: false);
			Game1.awardsManager.EarnAchievement(Achievement.BopoSnow, forceCheck: false);
			base.Complete();
		}
	}
}
