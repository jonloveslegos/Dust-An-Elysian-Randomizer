using Dust.CharClasses;

namespace Dust.Quests.SideQuests
{
	internal class _004_BlackSmith : Quest
	{
		public _004_BlackSmith()
		{
			base.SetName(this);
		}

		public override void ResetQuestGiver()
		{
			base.questGiver = CharacterType.Avgustin;
		}

		public override void Update()
		{
			if (base.stage == 0)
			{
				base.questGiver = CharacterType.Haley;
			}
			else
			{
				base.questGiver = CharacterType.Dust;
			}
			if (base.stage == 1 && Game1.stats.Equipment[320] > 0 && Game1.hud.InitDialogue(1, CharacterType.Haley))
			{
				base.stage++;
				this.Complete();
			}
			base.Update();
		}
	}
}
