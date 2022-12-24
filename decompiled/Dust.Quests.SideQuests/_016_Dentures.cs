using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _016_Dentures : Quest
	{
		public _016_Dentures()
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
			if (Game1.stats.Material[24] >= 10)
			{
				base.stage = 1;
				base.questGiver = CharacterType.Blop;
			}
			base.requirementMet = base.stage > 0;
			base.Update();
		}

		public override void Complete()
		{
			Game1.questManager.RewardGold(2000);
			Game1.stats.AcquireMateral(MaterialType.HoundTeeth, -10);
			base.Complete();
		}
	}
}
