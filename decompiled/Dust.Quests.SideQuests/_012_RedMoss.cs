using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _012_RedMoss : Quest
	{
		public _012_RedMoss()
		{
			base.SetName(this);
		}

		public override void ResetQuestGiver()
		{
			base.questGiver = CharacterType.FloHop;
		}

		public override void Update()
		{
			base.questGiver = CharacterType.Dust;
			base.stage = 0;
			if (Game1.stats.Equipment[337] > 0)
			{
				base.stage = 1;
				base.questGiver = CharacterType.FloHop;
			}
			base.requirementMet = base.stage > 0;
			base.Update();
		}

		public override void Complete()
		{
			Game1.questManager.RewardGold(1000);
			Game1.stats.AcquireEquip(EquipType.RedMoss, -1, _bluePrint: false);
			Game1.stats.GetChestFromFile("Equip " + Game1.map.path + " " + EquipType.Doenjang.ToString() + " " + 3.ToString(), Game1.pManager);
			Game1.stats.AcquireMateral(MaterialType.TrolkFinger, 2);
			Game1.stats.AcquireMateral(MaterialType.HoundHide, 2);
			base.Complete();
		}
	}
}
