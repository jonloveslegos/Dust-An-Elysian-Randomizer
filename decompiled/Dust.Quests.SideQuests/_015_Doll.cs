using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _015_Doll : Quest
	{
		public _015_Doll()
		{
			base.SetName(this);
		}

		public override void ResetQuestGiver()
		{
			base.questGiver = CharacterType.SmoBop;
		}

		public override void Update()
		{
			if (base.stage == 1 && Game1.stats.Material[2] >= 2 && Game1.stats.Material[6] >= 3 && Game1.stats.Material[7] >= 1)
			{
				base.stage = 2;
			}
			base.requirementMet = base.stage >= 3;
			base.questGiver = CharacterType.Dust;
			if (base.stage == 3)
			{
				base.questGiver = CharacterType.SmoBop;
			}
			base.Update();
		}

		public override void Complete()
		{
			Game1.stats.GetChestFromFile("Equip " + Game1.map.path + " " + EquipType.ReedBox.ToString() + " " + 1.ToString(), Game1.pManager);
			Game1.stats.AcquireEquip(EquipType.FidgetDoll, -1, _bluePrint: false);
			base.Complete();
		}
	}
}
