using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _017_GiantArmor : Quest
	{
		public _017_GiantArmor()
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
			if (Game1.stats.Material[21] >= 2 && Game1.stats.Material[20] >= 2)
			{
				base.stage++;
			}
			base.requirementMet = base.stage > 0;
			base.Update();
		}

		public override void Complete()
		{
			Game1.questManager.RewardGold(3000);
			Game1.stats.AcquireMateral(MaterialType.GiantCore, -2);
			Game1.stats.AcquireMateral(MaterialType.GiantRock, -2);
			base.Complete();
		}
	}
}
