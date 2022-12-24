using Dust.CharClasses;

namespace Dust.Quests.SideQuests
{
	internal class _005_RestockShop : Quest
	{
		public _005_RestockShop()
		{
			base.SetName(this);
		}

		public override void ResetQuestGiver()
		{
			base.questGiver = CharacterType.ShopWild;
		}

		public override void Update()
		{
			base.requirementMet = base.stage > 0;
			base.Update();
		}

		public override void Complete()
		{
			Game1.questManager.RewardGold(800);
			base.Complete();
		}
	}
}
