using Dust.CharClasses;

namespace Dust.Quests.SideQuests
{
	internal class _008_Reunion : Quest
	{
		public _008_Reunion()
		{
			base.SetName(this);
		}

		public override void ResetQuestGiver()
		{
			base.questGiver = CharacterType.Moska;
		}

		public override void Update()
		{
			if (base.stage == 1)
			{
				base.questGiver = CharacterType.Moska;
			}
			else
			{
				base.questGiver = CharacterType.Dust;
			}
			if (base.stage == 0 && Game1.questManager.IfNoteFound(14))
			{
				base.stage = 1;
			}
			base.Update();
		}
	}
}
