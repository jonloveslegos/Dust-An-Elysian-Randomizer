using Dust.CharClasses;
using Dust.HUD;

namespace Dust.Quests.SideQuests
{
	internal class _020_PocketWatch : Quest
	{
		public _020_PocketWatch()
		{
			base.SetName(this);
		}

		public override void ResetQuestGiver()
		{
			base.questGiver = CharacterType.Oneida;
		}

		public override CharacterType GetInitialQuestGiver()
		{
			if (Game1.events.currentEvent >= 45)
			{
				return CharacterType.Oneida;
			}
			return CharacterType.Dust;
		}

		public override void Update()
		{
			base.questGiver = CharacterType.Geehan;
			if (base.stage == 1)
			{
				base.questGiver = CharacterType.Dust;
			}
			if (base.stage == 1 && Game1.stats.Equipment[341] > 0)
			{
				base.stage = 2;
				base.questGiver = CharacterType.Geehan;
			}
			base.requirementMet = base.stage == 2;
			base.Update();
		}

		public override void Complete()
		{
			Game1.stats.AcquireEquip(EquipType.GeehanWatch, -1, _bluePrint: false);
			Game1.stats.GetChestFromFile("Equip " + Game1.map.path + " " + EquipType.TreasureKey.ToString() + " " + 3.ToString(), Game1.pManager);
			base.Complete();
		}
	}
}
