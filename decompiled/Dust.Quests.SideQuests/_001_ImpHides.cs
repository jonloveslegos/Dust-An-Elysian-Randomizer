using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _001_ImpHides : Quest
	{
		public _001_ImpHides()
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
			if (Game1.stats.Material[12] >= 5)
			{
				base.stage++;
			}
			base.requirementMet = base.stage > 0;
			base.Update();
		}

		public override void Complete()
		{
			Game1.questManager.RewardGold(600);
			Game1.stats.AcquireMateral(MaterialType.ImpHide, -5);
			base.Complete();
		}
	}
}
