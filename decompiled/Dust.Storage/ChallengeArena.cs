using System;
using System.Collections.Generic;
using Dust.HUD;
using Dust.MapClasses;
using Dust.Particles;
using Microsoft.Xna.Framework;

namespace Dust.Storage
{
	public class ChallengeArena
	{
		public int Id;

		public int HighScore;

		public int RankScore;

		private EquipType treasureType;

		public int Timer;

		private bool rewardIsBlueprint;

		public List<DestructLamp> lampList = new List<DestructLamp>();

		public ChallengeArena(int _id)
		{
			this.Id = _id;
			this.HighScore = 0;
			this.rewardIsBlueprint = false;
			switch (this.Id)
			{
			case 5:
				this.treasureType = EquipType.ScrapAug;
				this.Timer = 35;
				this.RankScore = 4800;
				break;
			case 4:
				this.treasureType = EquipType.VigilanceRing;
				this.Timer = 55;
				this.RankScore = 9000;
				break;
			case 3:
				this.treasureType = EquipType.WarriorsPendant;
				this.Timer = 45;
				this.RankScore = 8400;
				break;
			case 2:
				this.treasureType = EquipType.SpectralVest;
				this.Timer = 40;
				this.RankScore = 6000;
				break;
			case 1:
				this.treasureType = EquipType.WetSuit;
				this.Timer = 100;
				this.RankScore = 10500;
				break;
			case 0:
				this.treasureType = EquipType.GiantAug;
				this.Timer = 135;
				this.RankScore = 9000;
				break;
			default:
				this.RankScore = 0;
				this.treasureType = EquipType.HalmeoniPendant;
				this.Timer = 135;
				break;
			}
		}

		public void ResetLamps(bool resetChallenge)
		{
			if (resetChallenge)
			{
				for (int i = 0; i < this.lampList.Count; i++)
				{
					this.lampList[i].Exists = true;
				}
			}
			this.AddLamps();
		}

		public int CheckStarCount()
		{
			float num = Math.Min((float)this.HighScore / (float)(this.RankScore - 1) * 3f, 4f);
			int num2 = 0;
			for (int i = 0; (float)i < num; i++)
			{
				num2++;
			}
			return num2;
		}

		public void AddLamps()
		{
			Game1.dManager.AddDestructLamps(this.lampList);
		}

		public void SpawnReward(Vector2 loc, int treasureID, string uniqueID, ParticleManager pMan)
		{
			pMan.AddEquipment(loc, Vector2.Zero, (int)this.treasureType, this.rewardIsBlueprint, treasureID, uniqueID, 5);
		}

		public void CompleteChallenge(int treasureID, string uniqueID)
		{
			int itemArrayID = Game1.savegame.GetItemArrayID(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList, treasureID, uniqueID);
			if (itemArrayID > -1)
			{
				Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList[itemArrayID].Stage = 1;
			}
		}
	}
}
