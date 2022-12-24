using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _013_BlopStick : Quest
	{
		public _013_BlopStick()
		{
			base.SetName(this);
		}

		public override void ResetQuestGiver()
		{
			base.questGiver = CharacterType.Blop;
		}

		public override void Update()
		{
			base.questGiver = CharacterType.Dust;
			base.stage = 0;
			if (Game1.stats.Equipment[335] > 0)
			{
				base.stage = 1;
				base.questGiver = CharacterType.Blop;
			}
			base.requirementMet = base.stage > 0;
			base.Update();
		}

		public override void Complete()
		{
			Game1.questManager.RewardGold(1000);
			Game1.stats.AcquireEquip(EquipType.BlopStick, -1, _bluePrint: false);
			base.Complete();
		}
	}
}
