using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _019_FlornSparks : Quest
	{
		public _019_FlornSparks()
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
			if (Game1.stats.Material[23] >= 3 && Game1.stats.Material[22] >= 3)
			{
				base.stage++;
			}
			base.requirementMet = base.stage > 0;
			base.Update();
		}

		public override void Complete()
		{
			Game1.questManager.RewardGold(3000);
			Game1.stats.AcquireMateral(MaterialType.FlornSpark, -3);
			Game1.stats.AcquireMateral(MaterialType.FlornTentacle, -3);
			Game1.awardsManager.EarnAchievement(Achievement.CompleteQuestsFale, forceCheck: false);
			base.Complete();
		}
	}
}
