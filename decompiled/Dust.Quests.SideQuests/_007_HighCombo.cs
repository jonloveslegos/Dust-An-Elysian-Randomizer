using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _007_HighCombo : Quest
	{
		public _007_HighCombo()
		{
			base.SetName(this);
		}

		public override void ResetQuestGiver()
		{
			base.questGiver = CharacterType.Corbin;
		}

		public override void Update()
		{
			if (Game1.stats.longestChain >= 1000)
			{
				base.stage = 1;
			}
			base.requirementMet = base.stage > 0;
			if (base.stage > 0)
			{
				base.questGiver = CharacterType.Corbin;
			}
			else
			{
				base.questGiver = CharacterType.Dust;
			}
			base.Update();
		}

		public override void Complete()
		{
			Game1.questManager.RewardGold(1000);

			Game1.stats.GetChestFromFile("Equip " + Game1.map.path + " " + EquipType.HotDog.ToString() + " " + 3.ToString(), Game1.pManager);
			base.Complete();
		}
	}
}
