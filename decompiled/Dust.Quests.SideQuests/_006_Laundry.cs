using Dust.CharClasses;

namespace Dust.Quests.SideQuests
{
	internal class _006_Laundry : Quest
	{
		public _006_Laundry()
		{
			base.SetName(this);
		}

		public override void ResetQuestGiver()
		{
			base.questGiver = CharacterType.Colleen;
		}

		public override void Update()
		{
			if (Game1.stats.Equipment[338] > 0 && base.stage == 2)
			{
				base.stage = 3;
				Game1.hud.InitDialogue(100, CharacterType.Gianni);
			}
			if (base.stage == 1)
			{
				base.questGiver = CharacterType.Corbin;
			}
			else
			{
				base.questGiver = CharacterType.Gianni;
			}
			base.requirementMet = base.stage > 4;
			base.Update();
		}

		public override void Complete()
		{
			if (base.stage >= 100)
			{
				Game1.awardsManager.EarnAchievement(Achievement.Renegade, forceCheck: false);
			}
			else
			{
				Game1.awardsManager.EarnAchievement(Achievement.Paragon, forceCheck: false);
			}
			base.Complete();
		}
	}
}
