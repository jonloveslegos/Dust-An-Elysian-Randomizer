using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _003_Slime : Quest
	{
		public _003_Slime()
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
			if (Game1.stats.Material[18] >= 4 && Game1.stats.Material[19] >= 4)
			{
				base.stage++;
			}
			base.requirementMet = base.stage > 0;
			base.Update();
		}

		public override void Complete()
		{
			Game1.questManager.RewardGold(2000);
			Game1.stats.AcquireMateral(MaterialType.SlimyCoat, -4);
			Game1.stats.AcquireMateral(MaterialType.SlimySpike, -4);
			base.Complete();
		}
	}
}
