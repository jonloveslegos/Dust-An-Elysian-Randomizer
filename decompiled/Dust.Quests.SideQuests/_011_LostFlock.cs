using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _011_LostFlock : Quest
	{
		public _011_LostFlock()
		{
			base.SetName(this);
		}

		public override void ResetQuestGiver()
		{
			base.questGiver = CharacterType.OldGappy;
		}

		public override void Update()
		{
			base.questGiver = CharacterType.Dust;
			base.stage = 0;
			if (Game1.stats.Equipment[336] >= 6)
			{
				base.stage = 1;
				base.questGiver = CharacterType.OldGappy;
			}
			base.requirementMet = base.stage > 0;
			base.Update();
		}

		public override void Complete()
		{
			Game1.questManager.RewardGold(4000);
			Game1.stats.AcquireEquip(EquipType.GappySheep, -6, _bluePrint: false);
			Game1.stats.AcquireMateral(MaterialType.Cotton, 20);
			base.Complete();
		}
	}
}
