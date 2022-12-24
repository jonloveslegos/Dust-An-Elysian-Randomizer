using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _014_Trolks : Quest
	{
		public _014_Trolks()
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
			if (Game1.stats.Material[31] >= 4)
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
			Game1.stats.AcquireMateral(MaterialType.TrolkFinger, -4);
			base.Complete();
		}
	}
}
