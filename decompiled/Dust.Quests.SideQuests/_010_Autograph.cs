using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _010_Autograph : Quest
	{
		public _010_Autograph()
		{
			base.SetName(this);
		}

		public override void ResetQuestGiver()
		{
			base.questGiver = CharacterType.Moska;
		}

		public override CharacterType GetInitialQuestGiver()
		{
			if (!Game1.events.sideEventAvailable[73])
			{
				return CharacterType.Moska;
			}
			return CharacterType.Dust;
		}

		public override void Update()
		{
			base.stage = 0;
			if (Game1.stats.Material[5] > 0)
			{
				base.stage = 1;
			}
			base.requirementMet = base.stage > 0;
			base.questGiver = CharacterType.Dust;
			if (base.stage > 0)
			{
				base.questGiver = CharacterType.Moska;
			}
			base.Update();
		}

		public override void Complete()
		{
			Game1.questManager.RewardGold(4000);
			Game1.stats.GetChestFromFile("Equip " + Game1.map.path + " " + EquipType.BirthdayCake.ToString() + " " + 2.ToString(), Game1.pManager);
			base.Complete();
		}
	}
}
