using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _018_AveeClaws : Quest
	{
		public _018_AveeClaws()
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
			if (Game1.stats.Material[16] >= 6 && Game1.stats.Material[17] >= 6)
			{
				base.stage++;
			}
			base.requirementMet = base.stage > 0;
			base.Update();
		}

		public override void Complete()
		{
			Game1.questManager.RewardGold(3000);
			Game1.stats.AcquireMateral(MaterialType.AveeClaw, -6);
			Game1.stats.AcquireMateral(MaterialType.AveeWing, -6);
			base.Complete();
		}
	}
}
