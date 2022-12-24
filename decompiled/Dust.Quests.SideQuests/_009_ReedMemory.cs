using Dust.CharClasses;

namespace Dust.Quests.SideQuests
{
	internal class _009_ReedMemory : Quest
	{
		public _009_ReedMemory()
		{
			base.SetName(this);
		}

		public override void ResetQuestGiver()
		{
			base.questGiver = CharacterType.Reed;
		}

		public override void Update()
		{
			base.questGiver = CharacterType.Dust;
			if (base.stage > 0)
			{
				base.questGiver = CharacterType.Reed;
			}
			if (base.stage == 0 && Game1.stats.Equipment[339] > 0)
			{
				base.stage = 1;
			}
			base.requirementMet = base.stage >= 3;
			base.Update();
		}
	}
}
