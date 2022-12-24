using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _002_BeastSpears : Quest
	{
		public _002_BeastSpears()
		{
			base.SetName(this);
		}

		public override void ResetQuestGiver()
		{
			base.questGiver = CharacterType.Fale;
		}

		public override void Update()
		{
			base.stage = 0;
			if (Game1.stats.Material[14] >= 8)
			{
				base.stage++;
			}
			base.requirementMet = base.stage > 0;
			base.Update();
		}

		public override void Complete()
		{
			Game1.questManager.RewardGold(1000);
			Game1.stats.AcquireMateral(MaterialType.BeastSpear, -8);
			base.Complete();
		}
	}
}
