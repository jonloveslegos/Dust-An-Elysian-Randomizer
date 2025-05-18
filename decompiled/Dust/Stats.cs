using System;
using System.Collections.Generic;
using System.Diagnostics;
using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Dust.NavClasses;
using Dust.Particles;
using Dust.Strings;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Dust
{
	public class Stats
	{
		public byte gameDifficulty = 1;

		public byte startDifficulty;

		public byte saveSlot;

		public byte manualSaveSlot;

		public bool gameComplete;

		public int Score;

		public int playerLifeState;

		public int XP;

		public int nextLevelXP;

		public int prevLevelXP;

		public float comboXP;

		public int comboXPTally;

		public int chainXPTally;

		public float comboEnemies;

		public int gainedXP;

		public float Completion;

		public int Explored;

		public int TreasureFound;

		public int FriendsFound;

		public bool[] StructureBuilt = new bool[10];

		public int comboMeter;

		public int damageMeter;

		public bool comboComplete;

		public float comboTimer;

		public float comboDisplayTimer;

		public int comboBreak;

		public int longestChain;

		public int melodicHitCount;

		public float melodicHitTimer;

		public float melodicMidHitTimer;

		public int LEVEL;

		public int LEVELCap = 240;

		public float curCharge;

		public int maxCharge;

		public int skillPoints;

		public byte[] upgrade = new byte[24];

		public byte[] upgradedStat = new byte[4];

		public bool[] achievementEarned = new bool[30];

		public bool canEarnAchievements = true;

		public int attack = 4;

		public int attackEquip;

		public int defense;

		public int defenseEquip;

		public int fidget = 1;

		public int newFidgetEquip;

		public int luck;

		public int luckEquip;

		public float bonusHealth = 1f;

		public int newHealtRegen;

		private float healthRegenTime;

		public int enemiesDefeated;

		public float attackBonusTime;

		public float defenseBonusTime;

		public int projectileType;

		public int projectileCost = 10;

		public bool canThrow = true;

		public float overHeating;

		public bool isSpinning;

		public bool inFront;

		public byte doubleJump;

		public Vector2 lastSafeLoc;

		public FidgetState fidgetState;

		public float fidgetAwayTime;

		public int[] villagerDialogue = new int[50];

		public float gameClock;

		public int currentItem = -1;

		public int currentPendant = -1;

		public int currentAugment = -1;

		public int currentArmor = -1;

		public int currentRingLeft = -1;

		public int currentRingRight = -1;

		public byte[] Equipment = new byte[360];

		public byte[] EquipBluePrint;

		public byte[] shopEquipment;

		public byte[] shopEquipGotten;

		public int[] Material = new int[100];

		public int[] shopMaterial;

		public int Gold;

		public int maxMaterials;

		private Vector4 prevWeatherColor;

		private float prevWeatherSaturation;

		private float prevWeatherBloom;

		private float reviveTimer;

		public Stats()
		{
			this.ResetItems();
			for (int i = 0; i < this.villagerDialogue.Length; i++)
			{
				this.villagerDialogue[i] = -1;
			}
		}

		public void ResetTimers()
		{
			this.comboXP = 0f;
			this.gainedXP = 0;
			this.attackBonusTime = 0f;
			this.defenseBonusTime = 0f;
			this.comboMeter = 0;
			this.comboComplete = false;
			this.comboTimer = 0f;
			this.comboDisplayTimer = 0f;
		}

		public void ResetItems()
		{
			this.Gold = 0;
			for (int i = 0; i < this.upgrade.Length; i++)
			{
				this.upgrade[i] = 0;
			}
			for (int j = 0; j < this.upgradedStat.Length; j++)
			{
				this.upgradedStat[j] = 0;
			}
			for (int k = 0; k < this.achievementEarned.Length; k++)
			{
				this.achievementEarned[k] = false;
			}
			for (int l = 0; l < this.Equipment.Length; l++)
			{
				this.Equipment[l] = 0;
			}
			this.EquipBluePrint = new byte[this.Equipment.Length - 120];
			for (int m = 0; m < this.EquipBluePrint.Length; m++)
			{
				this.EquipBluePrint[m] = 0;
			}
			for (int n = 0; n < this.Material.Length; n++)
			{
				this.Material[n] = -1;
			}
			for (int num = 0; num < this.StructureBuilt.Length; num++)
			{
				this.StructureBuilt[num] = false;
			}
			this.ResetShopItems();
			this.currentItem = -1;
			this.currentPendant = -1;
			this.currentAugment = -1;
			this.currentArmor = -1;
			this.currentRingLeft = -1;
			this.currentRingRight = -1;
			this.nextLevelXP = 600;
			this.curCharge = (this.maxCharge = 100);
		}

		private void ResetShopItems()
		{
			this.shopEquipment = new byte[this.Equipment.Length];
			this.shopEquipGotten = new byte[this.Equipment.Length];
			for (int i = 0; i < this.shopEquipment.Length; i++)
			{
				this.shopEquipment[i] = 0;
				this.shopEquipGotten[i] = 0;
			}
			this.shopMaterial = new int[this.Material.Length];
			for (int j = 0; j < this.shopMaterial.Length; j++)
			{
				this.shopMaterial[j] = -1;
			}
			if (Game1.inventoryManager == null)
			{
				return;
			}
			for (int k = 0; k < this.shopEquipment.Length; k++)
			{
				if (k < 3 || (k >= 300 && k < 305))
				{
					this.shopEquipment[k] = byte.MaxValue;
				}
			}
			this.shopEquipment[305] = 6;
			this.shopEquipment[60] = 4;
			this.shopEquipment[120] = 4;
			this.shopEquipment[121] = 4;
			this.shopEquipment[122] = 4;
			this.shopEquipment[180] = 4;
			this.shopEquipment[181] = 4;
			this.shopEquipment[182] = 4;
			this.shopEquipment[241] = 4;
			for (int l = 0; l < 6; l++)
			{
				this.shopMaterial[l] = 4;
			}
		}

		public void ResetGame(ParticleManager pMan, Character[] c)
		{
			Game1.events.Reset();
			Game1.camera.ResetCamera(c);
			Game1.hud.DialogueNull(-1, -1);
			Game1.gameMode = Game1.GameModes.Game;
			Game1.cutscene.ExitCutscene();
			for (int i = 0; i < Game1.map.eventRegion.Count; i++)
			{
				Game1.map.eventRegion[i].Reset();
			}
			for (int j = 0; j < this.villagerDialogue.Length; j++)
			{
				this.villagerDialogue[j] = -1;
			}
			this.fidgetState = FidgetState.Normal;
			this.fidgetAwayTime = 0f;
			Game1.SlowTime = 0f;
			this.Score = 0;
			this.inFront = true;
			if (this.playerLifeState != 0)
			{
				this.playerLifeState = 2;
			}
			this.startDifficulty = (this.gameDifficulty = 1);
			this.gameComplete = false;
			this.curCharge = (this.maxCharge = 100);
			this.XP = 0;
			this.comboXP = 0f;
			this.gainedXP = 0;
			this.LEVEL = 0;
			this.prevLevelXP = 0;
			this.nextLevelXP = 600;
			this.skillPoints = 0;
			this.attack = 4;
			this.defense = 0;
			this.fidget = 1;
			this.luck = 0;
			this.attackBonusTime = (this.defenseBonusTime = 0f);
			this.healthRegenTime = 0f;
			this.projectileType = 0;
			this.projectileCost = 10;
			this.comboComplete = false;
			this.enemiesDefeated = (this.longestChain = (this.comboMeter = 0));
			this.comboTimer = (this.comboDisplayTimer = 0f);
			this.gameClock = 0f;
			for (int k = 0; k < this.upgrade.Length; k++)
			{
				this.upgrade[k] = 0;
				if (Game1.debugging)
					this.upgrade[k] = 1;
			}
			for (int l = 0; l < this.upgradedStat.Length; l++)
			{
				this.upgradedStat[l] = 0;
			}
			for (int m = 0; m < this.achievementEarned.Length; m++)
			{
				this.achievementEarned[m] = false;
			}
			this.canEarnAchievements = true;
			for (int n = 0; n < Game1.cManager.challengeArenas.Count; n++)
			{
				Game1.cManager.challengeArenas[n].HighScore = 0;
			}
			this.ResetItems();
			Game1.inventoryManager.ResetInventoryManager();
			Game1.hud.ExitShop();
			Game1.wManager.ResetWeather();
			Game1.navManager.Reset();
			Game1.cManager.Reset();
			Game1.map.warpStage = (Game1.map.doorStage = 0);
			Sound.FadeMusicOut(0f);
			Sound.curMusicVolume = 1f;
			Sound.ResetAmbience(null);
			Music.Play("silent");
			WeatherAudio.BeginWeatherSound("weather");
			this.ResetCharacters(c);
			Game1.menu.ResetMenu();
			Game1.hud.ResetHud(all: false);
			Game1.questManager.ResetQuests();
			VibrationManager.Reset();
			GC.Collect();
		}

		public void ResetCharacters(Character[] c)
		{
			c[0].HP = (c[0].pHP = (c[0].MaxHP = 80));
			c[0].LifeBarPercent = 0f;
			c[0].HPLossFrame = 0f;
			c[0].CanHurtFrame = 0f;
			c[0].CanHurtProjectileFrame = 0f;
			c[0].Holding = false;
			c[0].Floating = false;
			c[0].KeySecondary = false;
			c[0].StatusTime = 0f;
			c[0].StatusEffect = StatusEffects.Normal;
			c[0].UnloadTextures();
		}

		public void ResetDebug(ParticleManager pMan, Character[] c)
		{
			Game1.map.path = Game1.startMap;
			this.ResetGame(pMan, c);
			if (Game1.Convention)
			{
				Game1.TrialInit();
			}
			if (!Game1.Convention)
			{
				Game1.map.SwitchMap(pMan, c, Game1.map.path, loading: false);
			}
		}

		public void ResetLeaderboardRanks()
		{
			this.Completion = (this.enemiesDefeated = (this.longestChain = 0));
			this.gameClock = 0f;
			for (int i = 0; i < this.achievementEarned.Length; i++)
			{
				this.achievementEarned[i] = false;
			}
			for (int j = 0; j < Game1.cManager.challengeArenas.Count; j++)
			{
				Game1.cManager.challengeArenas[j].HighScore = 0;
			}
			this.canEarnAchievements = true;
		}

		public void UnlockEquipment(int amount, bool unlockKeys)
		{
			for (int i = 0; i < this.Equipment.Length; i++)
			{
				if (Game1.inventoryManager.equipItem[i] != null && ((!unlockKeys && (i < 321 || i >= 330)) || (unlockKeys && i >= 321 && i < 330)) && i != 340)
				{
					this.Equipment[i] = (byte)Math.Min(this.Equipment[i] + amount, 50);
				}
			}
			Game1.questManager.UpdateQuests(0);
		}

		public void UnlockBluePrints(int count)
		{
			for (int i = 0; i < this.EquipBluePrint.Length; i++)
			{
				if (Game1.inventoryManager.equipItem[i + Game1.inventoryManager.invSelMax] != null)
				{
					this.EquipBluePrint[i] = (byte)count;
				}
			}
			Game1.questManager.UpdateQuests(0);
		}

		public void UnlockMaterials(int amount, bool shop)
		{
			for (int i = 0; i < this.Material.Length; i++)
			{
				if (Game1.inventoryManager.material[i] != null)
				{
					if (!shop)
					{
						this.Material[i] = (int)MathHelper.Clamp(this.Material[i] + amount, 0f, 9999f);
					}
					else
					{
						this.shopMaterial[i] = (int)MathHelper.Clamp(this.shopMaterial[i] + amount, 0f, 9999f);
					}
				}
			}
			Game1.questManager.UpdateQuests(0);
		}

		public void ResetLevel()
		{
			this.ResetCharacters(Game1.character);
			this.XP = 0;
			this.comboXP = 0f;
			this.gainedXP = 0;
			this.LEVEL = 0;
			this.prevLevelXP = 0;
			this.nextLevelXP = 600;
			this.skillPoints = 0;
			this.attack = 4;
			this.defense = 0;
			this.fidget = 1;
			this.luck = 0;
			this.attackBonusTime = (this.defenseBonusTime = 0f);
			this.healthRegenTime = 0f;
			for (int i = 0; i < this.upgradedStat.Length; i++)
			{
				this.upgradedStat[i] = 0;
			}
			this.currentItem = -1;
			this.currentPendant = -1;
			this.currentAugment = -1;
			this.currentArmor = -1;
			this.currentRingLeft = -1;
			this.currentRingRight = -1;
			Game1.inventoryManager.UpdateEquipStats(0, updateEquip: true, string.Empty);
		}

		public void ResetEquip()
		{
			for (int i = 0; i < this.Equipment.Length; i++)
			{
				this.Equipment[i] = 0;
				this.shopEquipment[i] = 0;
				this.shopEquipGotten[i] = 0;
			}
			for (int j = 0; j < this.EquipBluePrint.Length; j++)
			{
				this.EquipBluePrint[j] = 0;
			}
			for (int k = 0; k < this.Material.Length; k++)
			{
				this.Material[k] = -1;
				this.shopMaterial[k] = -1;
			}
			this.currentItem = -1;
			this.currentPendant = -1;
			this.currentAugment = -1;
			this.currentArmor = -1;
			this.currentRingLeft = -1;
			this.currentRingRight = -1;
			Game1.questManager.UpdateQuests(0);
		}

		public void UnlockUpgrades()
		{
			for (int i = 0; i < this.upgrade.Length; i++)
			{
				if (i < 3 || i >= 10)
				{
					this.upgrade[i] = 1;
					foreach (RevealMap value in Game1.navManager.RevealMap.Values)
					{
						int itemArrayID = Game1.savegame.GetItemArrayID(value.GameItemList, i + 2000, null);
						if (itemArrayID > -1)
						{
							value.GameItemList[itemArrayID].Stage = 1;
							break;
						}
					}
				}
				else
				{
					this.upgrade[i] = 0;
				}
			}
		}

		public void RelockUpgrades()
		{
			for (int i = 0; i < this.upgrade.Length; i++)
			{
				this.upgrade[i] = 0;
				foreach (RevealMap value in Game1.navManager.RevealMap.Values)
				{
					int itemArrayID = Game1.savegame.GetItemArrayID(value.GameItemList, i + 2000, null);
					if (itemArrayID > -1)
					{
						value.GameItemList[itemArrayID].Stage = 0;
						break;
					}
				}
			}
		}

		public void UnlockMaps()
		{
			foreach (RevealMap value in Game1.navManager.RevealMap.Values)
			{
				value.Unlock();
			}
			for (int i = 0; i < Game1.events.regionIntroduced.Length; i++)
			{
				Game1.events.regionIntroduced[i] = true;
			}
			for (int j = 100; j < 110; j++)
			{
				Game1.events.sideEventAvailable[j] = false;
			}
		}

		public void ReLockMaps()
		{
			foreach (RevealMap value in Game1.navManager.RevealMap.Values)
			{
				value.Reset();
			}
			for (int i = 0; i < Game1.events.regionIntroduced.Length; i++)
			{
				Game1.events.regionIntroduced[i] = false;
			}
		}

		public void DisableHelp()
		{
			for (int i = 0; i < 29; i++)
			{
				Game1.events.sideEventAvailable[i] = false;
			}
			for (int j = 0; j < Game1.events.regionIntroduced.Length; j++)
			{
				Game1.events.regionIntroduced[j] = true;
			}
		}

		public void UnlockQuestsNotes(bool complete)
		{
			if (!complete)
			{
				for (int i = 0; i < Game1.questManager.availableQuests.Count; i++)
				{
					Game1.questManager.AddQuest(i);
				}
				for (int j = 0; j < 100; j++)
				{
					Game1.questManager.AddNote(j, loading: false);
				}
			}
			else
			{
				for (int k = 1; k < Game1.questManager.availableQuests.Count; k++)
				{
					Game1.questManager.CompleteQuest(k);
				}
			}
			Game1.hud.miniPromptList.Clear();
		}

		public void UnlockFriends(bool relock)
		{
			foreach (KeyValuePair<string, RevealMap> item in Game1.navManager.RevealMap)
			{
				for (int i = 0; i < item.Value.CageList.Count; i++)
				{
					item.Value.CageList[i].Stage = (byte)((!relock) ? 1u : 0u);
				}
			}
			Game1.navManager.RevealCell("sanc01");
		}

		public void CheckTreasureCount()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			foreach (KeyValuePair<string, RevealMap> item in Game1.navManager.RevealMap)
			{
				if (item.Key != "trial")
				{
					num += item.Value.KeyList.Count;
					num2 += item.Value.CageList.Count;
					num3 += item.Value.ChestList.Count;
					num4 += item.Value.GameItemList.Count;
					num5 += item.Value.DestructableList.Count;
				}
			}
		}

		public void AcquireEquip(EquipType equipType, int amount, bool _bluePrint)
		{
			if (amount > 0)
			{
				int promptCategory = (byte)((int)equipType / (int)Game1.inventoryManager.invSelMax);
				for (int i = 0; i < amount; i++)
				{
					Game1.hud.InitMiniPrompt((MiniPromptType)promptCategory, (int)equipType, _bluePrint);
				}
			}
			else if (!_bluePrint && this.Equipment[(int)equipType] > 0)
			{
				Game1.hud.InitMiniPrompt(MiniPromptType.ItemsRemoved, (int)equipType, blueprint: false);
			}
			if (_bluePrint)
			{
				this.EquipBluePrint[(int)(equipType - Game1.inventoryManager.invSelMax)] = (byte)MathHelper.Clamp(this.EquipBluePrint[(int)(equipType - Game1.inventoryManager.invSelMax)] + amount, 0f, 250f);
			}
			else
			{
				this.Equipment[(int)equipType] = (byte)MathHelper.Clamp(this.Equipment[(int)equipType] + amount, 0f, 250f);
			}
			if ((int)equipType < (int)Game1.inventoryManager.invSelMax && Game1.stats.currentItem == -1)
			{
				Game1.stats.currentItem = (int)equipType;
			}
			Game1.questManager.UpdateQuests(0);
		}

		public void AcquireMateral(MaterialType type, int amount)
		{
			if (amount > 0)
			{
				for (int i = 0; i < amount; i++)
				{
					Game1.hud.InitMiniPrompt(MiniPromptType.MaterialAcquired, (int)type, blueprint: false);
				}
			}
			else if (this.Material[(int)type] > 0)
			{
				Game1.hud.InitMiniPrompt(MiniPromptType.ItemsRemoved, (int)type, blueprint: false);
			}
			this.Material[(int)type] = Math.Max(this.Material[(int)type], 0);
			this.Material[(int)type] = (int)MathHelper.Clamp(this.Material[(int)type] + amount, 0f, 250f);
			Game1.questManager.UpdateQuests(0);
		}

		public void AcquireGold(int amount)
		{
			this.Gold += amount;
			if (amount > 0)
			{
				Game1.hud.InitMiniPrompt(MiniPromptType.GoldAcquired, amount, blueprint: false);
			}
			else if (amount < 0)
			{
				Game1.hud.InitMiniPrompt(MiniPromptType.GoldRemoved, Math.Abs(amount), blueprint: false);
			}
		}

		public void AcquireXP(int amount)
		{
			amount = Math.Max(amount, 0);
			this.XP += (amount * Game1.settings.ExpMult);
			Game1.hud.InitMiniPrompt(MiniPromptType.XPAcquired, amount * Game1.settings.ExpMult, blueprint: false);
			if (Game1.hud.hudDetails)
			{
				Game1.pManager.AddXP(Game1.character[0].Location + new Vector2(0f, -100f), amount * Game1.settings.ExpMult, bonus: false, 9);
			}
		}

		public void EarnUpgrade(int upgradable, byte amount)
		{
			this.upgrade[upgradable] = amount;
			Game1.hud.InitMiniPrompt(MiniPromptType.UpgradePrompt, upgradable, false);
		}

		public int GetProjectileCost(int projectile)
		{
			switch (projectile)
			{
				case 0: return 20;
				case 1: return 22;
				case 2: return 24;
				case 3: return 50;
			}
			return 10;
		}

		private EquipType ValidMaxEquip(int category)
		{
			string regionName;
			if ((regionName = Game1.map.regionName) != null)
			{
				if (!(regionName == "cave"))
				{
					if (!(regionName == "trial") && !(regionName == "grave"))
					{
						if (!(regionName == "snow"))
						{
							if (regionName == "lava")
							{
								switch (category)
								{
									case 1:
										return EquipType.BattleMastersPendant;
									case 2:
										return EquipType.MithrarinAug;
									case 3:
										return EquipType.MithrarinRobe;
									case 4:
										return EquipType.WeddingRing;
									default:
										return EquipType.Galbi;
								}
							}
						}
						else
						{
							switch (category)
							{
								case 1:
									return EquipType.WarriorsPendant;
								case 2:
									return EquipType.MetalAug;
								case 3:
									return EquipType.MountainGear;
								case 4:
									return EquipType.FocusRing;
								default:
									return EquipType.BuffaloBurger;
							}
						}
					}
					else
					{
						switch (category)
						{
							case 1:
								return EquipType.CowardsPendant;
							case 2:
								return EquipType.WickedAug;
							case 3:
								return EquipType.SpectralVest;
							case 4:
								return EquipType.PatienceRing;
							default:
								return EquipType.Champong;
						}
					}
				}
				else
				{
					switch (category)
					{
						case 1:
							return EquipType.PoorMansPendant;
						case 2:
							return EquipType.TrolkFistAug;
						case 3:
							return EquipType.TrolkArmor;
						case 4:
							return EquipType.ApprenticeRing;
						default:
							return EquipType.KimBap;
					}
				}
			}
			switch (category)
			{
				case 1:
					return EquipType.ChildPendant;
				case 2:
					return EquipType.GiantAug;
				case 3:
					return EquipType.LightStoneArmor;
				case 4:
					return EquipType.SmoothRing;
				default:
					return EquipType.CupCake;
			}
		}

		public void GetChestFromFile(string chestID, ParticleManager pMan)
		{
			var itemid = ReturnChestItems(chestID);
			var upgradegot = false;
			if (Game1.debugging)
            {
				Debug.WriteLine(chestID);
                pMan.AddCaption(new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 3 - 60), chestID, 1.2f, new Color(0.5f, 1f, 1f, 1f), 3f, 9);
			}
			foreach (var gotten in itemid)
			{
				if (gotten.Contains("~"))
				{
					if (!upgradegot)
						pMan.AddCaption(new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 3 - 30), Strings_Hud.Status_Upgraded, 1.2f, new Color(0.5f, 1f, 1f, 1f), 3f, 9);
					upgradegot = true;
					this.EarnUpgrade(int.Parse(gotten.Replace("~", "")), (byte)(Game1.stats.upgrade[int.Parse(gotten.Replace("~", ""))] + 1));
				}
				else if (gotten == "!")
                {
					if (Game1.settings.AutoLevelUp || this.gameDifficulty == 0)
					{
						this.UpgradeStats(this.BalanceUpgrades(1));
						if (!Game1.IsTrial)
						{
							Game1.awardsManager.EarnAchievement(Achievement.LevelUp, forceCheck: false);
						}
					}
					else
					{
						this.skillPoints++;
					}
					Game1.hud.levelUpEffect = true;
				}
				else if (int.Parse(gotten) < 0)
					this.AcquireEquip((EquipType)(-int.Parse(gotten)), 1, _bluePrint: true);
				else
					this.AcquireEquip((EquipType)int.Parse(gotten), 1, _bluePrint: false);
			}
		}

		public void GetChestFromFileNoText(string chestID)
		{
			var itemid = ReturnChestItems(chestID);
			if (Game1.debugging)
			{
				Debug.WriteLine(chestID);
			}
			foreach (var gotten in itemid)
			{
				if (gotten.Contains("~"))
					this.EarnUpgrade(int.Parse(gotten.Replace("~", "")), (byte)(Game1.stats.upgrade[int.Parse(gotten.Replace("~", ""))] + 1));
				else if (gotten == "!")
				{
					if (Game1.settings.AutoLevelUp || this.gameDifficulty == 0)
					{
						this.UpgradeStats(this.BalanceUpgrades(1));
						if (!Game1.IsTrial)
						{
							Game1.awardsManager.EarnAchievement(Achievement.LevelUp, forceCheck: false);
						}
					}
					else
					{
						this.skillPoints++;
					}
				}
				else if (int.Parse(gotten) < 0)
					this.AcquireEquip((EquipType)(-int.Parse(gotten)), 1, _bluePrint: true);
				else
					this.AcquireEquip((EquipType)int.Parse(gotten), 1, _bluePrint: false);
			}
		}

		public List<string> ReturnChestItems(string chestID)
		{
			foreach (string line in System.IO.File.ReadLines(System.IO.Directory.GetCurrentDirectory() + "\\data\\seed.data"))
			{
				var itemid = new List<string>();
				var found = "";
				var failed = false;
				if (!failed)
				{
					foreach (char chara in line)
					{
						if (chara.ToString() == ":")
						{
							if (found != chestID)
							{
								failed = true;
								break;
							}
							found = "";
						}
						else if (chara.ToString() == ",")
						{
							itemid.Add(found);
							found = "";
						}
						else
							found += chara;
					}
					if (!failed)
					{
						return itemid;
					}
				}
			}
			return new List<string>();
		}
		public void OpenChest(Vector2 loc, ParticleManager pMan, string chestID)
		{
			int num = 0;
			switch (chestID)
			{
				case "intro01b 0":
					num = 55;
					GetChestFromFile(chestID, pMan);
					break;
				case "intro02b 0":
					{
						num = 499;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "intro05 0":
					{
						num = 299;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "intro07b 0":
					{
						num = 299;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "village02 0":
					{
						num = 399;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "village03 0":
					num = 299;
					GetChestFromFile(chestID, pMan);
					break;
				case "forest02 0":
					{
						num = 29;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "forest04c 0":
					num = 109;
					GetChestFromFile(chestID, pMan);
					break;
				case "forest08 0":
					{
						num = 509;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "forest09b 0":
					num = 1119;
					GetChestFromFile(chestID, pMan);
					break;
				case "smith03 0":
					num = 499;
					GetChestFromFile(chestID, pMan);
					break;
				case "smith03b 0":
					GetChestFromFile(chestID, pMan);
					num = 1009;
					break;
				case "smith03b 1":
					GetChestFromFile(chestID, pMan);
					num = 1009;
					break;
				case "smith03b 2":
					{
						GetChestFromFile(chestID, pMan);
						for (int num20 = 0; num20 < 5; num20++)
						{
							Game1.events.SpawnCharacter(loc, "enemy", CharacterType.Avee, Team.Enemy, ground: false);
						}
						num = 1009;
						break;
					}
				case "challenge05 0":
					num = 2999;
					GetChestFromFile(chestID, pMan);
					break;
				case "farm01 0":
					{
						num = 999;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "cove01 0":
					{
						num = 1999;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "cave02 0":
					{
						num = 1119;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "cave04 0":
					num = 2119;
					GetChestFromFile(chestID, pMan);
					break;
				case "cave04 1":
					{
						num = 1119;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "cave05b 0":
					num = 1119;
					GetChestFromFile(chestID, pMan);
					break;
				case "cave17 0":
					{
						num = 2999;
						Game1.questManager.AddNote(11, loading: false);
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "cave19b 0":
					num = 9999;
					GetChestFromFile(chestID, pMan);
					break;
				case "cave22 0":
					num = 999;
					GetChestFromFile(chestID, pMan);
					break;
				case "cave24 0":
					{
						num = 999;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "cave27 0":
					num = 999;
					GetChestFromFile(chestID, pMan);
					break;
				case "grave01b 0":
					{
						num = 2999;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "grave03 0":
					{
						num = 1999;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "grave04 0":
					{
						num = 299;
						GetChestFromFile(chestID, pMan);
						for (int num21 = 0; num21 < 10; num21++)
						{
							Game1.events.SpawnCharacter(loc, "enemy", CharacterType.FleshFly, Team.Enemy, ground: false);
						}
						break;
					}
				case "grave05 0":
					num = 999;
					GetChestFromFile(chestID, pMan);
					break;
				case "grave07 0":
					num = 1999;
					GetChestFromFile(chestID, pMan);
					break;
				case "grave08 0":
					num = 999;
					GetChestFromFile(chestID, pMan);
					break;
				case "grave09 0":
					num = 999;
					GetChestFromFile(chestID, pMan);
					break;
				case "grave13 0":
					{
						num = 1999;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "grave14 0":
					num = 999;
					GetChestFromFile(chestID, pMan);
					break;
				case "grave22 0":
					{
						num = 1999;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "grave24 0":
					{
						num = 1999;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "mansionb01 0":
					num = 199;
					GetChestFromFile(chestID, pMan);
					break;
				case "mansionc02 0":
					num = 199;
					GetChestFromFile(chestID, pMan);
					break;
				case "mansiond02 0":
					num = 199;
					GetChestFromFile(chestID, pMan);
					break;
				case "mansiond02 1":
					num = 199;
					GetChestFromFile(chestID, pMan);
					break;
				case "snow02 0":
					{
						num = 999;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "snow04 0":
					num = 2999;
					break;
				case "snow04 1":
					num = 499;
					GetChestFromFile(chestID, pMan);
					break;
				case "snow05 0":
					num = 2999;
					GetChestFromFile(chestID, pMan);
					break;
				case "snow07 0":
					num = 2999;
					GetChestFromFile(chestID, pMan);
					break;
				case "snow11 0":
					{
						num = 1999;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "snow14 0":
					num = 999;
					GetChestFromFile(chestID, pMan);
					break;
				case "snow20 0":
					num = 999;
					GetChestFromFile(chestID, pMan);
					break;
				case "snow21 0":
					num = 999;
					GetChestFromFile(chestID, pMan);
					break;
				case "snow22 0":
					num = 999;
					GetChestFromFile(chestID, pMan);
					break;
				case "snow23 0":
					{
						num = 2999;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "snow23 1":
					num = 1999;
					GetChestFromFile(chestID, pMan);
					break;
				case "snow25 0":
					num = 999;
					GetChestFromFile(chestID, pMan);
					break;
				case "lava02 0":
					num = 2999;
					GetChestFromFile(chestID, pMan);
					break;
				case "lava07 0":
					{
						num = 1999;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "lava08b 0":
					num = 499;
					GetChestFromFile(chestID, pMan);
					break;
				case "lava08b 1":
					num = 299;
					GetChestFromFile(chestID, pMan);
					break;
				case "lava09 0":
					{
						num = 499;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "lava09 1":
					{
						GetChestFromFile(chestID, pMan);
						for (int num2 = 0; num2 < 10; num2++)
						{
							Game1.events.SpawnCharacter(loc, "enemy", CharacterType.ImpLava, Team.Enemy, ground: false);
						}
						break;
					}
				case "lava11b 0":
					num = 1499;
					GetChestFromFile(chestID, pMan);
					break;
				case "lava12 0":
					num = 2499;
					GetChestFromFile(chestID, pMan);
					break;
				case "lava13 0":
					{
						num = 499;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "lava15 0":
					{
						num = 2999;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "lava16 0":
					num = 1999;
					GetChestFromFile(chestID, pMan);
					break;
				case "lava18b 0":
					num = 1999;
					GetChestFromFile(chestID, pMan);
					break;
				case "trial03 0":
					{
						num = 309;
						GetChestFromFile(chestID, pMan);
						break;
					}
				case "trial04 0":
					num = 309;
					GetChestFromFile(chestID, pMan);
					break;
				case "trial05 0":
					num = 209;
					GetChestFromFile(chestID, pMan);
					break;
			}
			int num26 = num / 400;
			int num27 = (num - num26 * 400) / 100;
			int num28 = (num - num26 * 400 - num27 * 100) / 30;
			int num29 = (num - num26 * 400 - num27 * 100 - num28 * 30) / 5;
			num = num - num26 * 400 - num27 * 100 - num28 * 30 - num29 * 5;
			for (int num30 = 0; num30 < num26; num30++)
			{
				this.SpawnGold(loc, pMan, 400);
			}
			for (int num31 = 0; num31 < num27; num31++)
			{
				this.SpawnGold(loc, pMan, 100);
			}
			for (int num32 = 0; num32 < num28; num32++)
			{
				this.SpawnGold(loc, pMan, 30);
			}
			for (int num33 = 0; num33 < num29; num33++)
			{
				this.SpawnGold(loc, pMan, 5);
			}
			for (int num34 = 0; num34 < num; num34++)
			{
				this.SpawnGold(loc, pMan, 1);
			}
		}

		public CharacterType GetFriend(string cageMapName)
		{
			switch (cageMapName)
			{
				case "intro08 0":
					return CharacterType.MeatBoy;
				case "forest03b 0":
					return CharacterType.HyperDan;
				case "forest08b 0":
					return CharacterType.BandageGirl;
				case "cave13 0":
					return CharacterType.Damsel;
				case "cave16 0":
					return CharacterType.Spelunky;
				case "grave13g 0":
					return CharacterType.Dishwasher;
				case "grave16d 0":
					return CharacterType.Yuki;
				case "village05 0":
					return CharacterType.HyperChris;
				case "smith04b 0":
					return CharacterType.Kid;
				case "snow30e 0":
					return CharacterType.Gomez;
				case "snow18 0":
					return CharacterType.Tim;
				case "lava09b 0":
					return CharacterType.Maw;
			}
			return CharacterType.MeatBoy;
		}

		public bool CanHyperDub()
		{
			int num = 0;
			foreach (KeyValuePair<string, RevealMap> item in Game1.navManager.RevealMap)
			{
				for (int i = 0; i < item.Value.CageList.Count; i++)
				{
					if (item.Value.CageList[i].Stage > 0)
					{
						CharacterType friend = this.GetFriend(item.Value.CageList[i].UniqueID);
						if (friend == CharacterType.HyperChris || friend == CharacterType.HyperDan)
						{
							num++;
						}
					}
				}
			}
			if (num > 1)
			{
				return true;
			}
			return false;
		}

		private void SpawnGold(Vector2 loc, ParticleManager pMan, int coinAmount)
		{
			pMan.AddCoin(loc, new Vector2(Rand.GetRandomInt(-600, 600), Rand.GetRandomInt(-800, -500)), coinAmount, 5);
		}

		private void SpawnEquipment(Vector2 loc, ParticleManager pMan, EquipType type, bool bluePrint, bool chest)
		{
			Vector2 traj;
			if (!chest)
			{
				if (Game1.events.currentEvent < 18)
				{
					return;
				}
				traj = new Vector2(Rand.GetRandomInt(-600, 600), Rand.GetRandomInt(-800, -500));
			}
			else
			{
				int num = ((Rand.GetRandomInt(0, 2) != 0) ? Rand.GetRandomInt(400, 1000) : Rand.GetRandomInt(-1000, -400));
				traj = new Vector2(num, Rand.GetRandomInt(-1400, -1000));
			}
			if (type == EquipType.TreasureKey)
			{
				pMan.AddKey(loc - new Vector2(0f, 50f), traj, -1, 5);
			}
			else
			{
				pMan.AddEquipment(loc, traj, (int)type, bluePrint, -1, 5);
			}
			if (bluePrint)
			{
				return;
			}
			int num2 = (int)type / (int)Game1.inventoryManager.invSelMax * Game1.inventoryManager.invSelMax;
			for (int i = num2; i < num2 + Game1.inventoryManager.invSelMax; i++)
			{
				if (i <= (int)(type + 1) && i < 300 && Game1.inventoryManager.equipItem[i] != null && this.shopEquipment[i] < 1)
				{
					if (type < EquipType.FilthyPendant)
					{
						this.shopEquipment[i] = byte.MaxValue;
					}
					else
					{
						this.shopEquipment[i] = 2;
					}
				}
			}
		}

		private void SpawnMaterial(Vector2 loc, ParticleManager pMan, MaterialType type, bool chest)
		{
			Vector2 traj;
			if (!chest)
			{
				traj = new Vector2(Rand.GetRandomInt(-600, 600), Rand.GetRandomInt(-800, -500));
			}
			else
			{
				int num = ((Rand.GetRandomInt(0, 2) != 0) ? Rand.GetRandomInt(700, 1000) : Rand.GetRandomInt(-1000, -700));
				traj = new Vector2(num, Rand.GetRandomInt(-1600, -500));
			}
			pMan.AddMaterial(loc, traj, (int)type, 5);
		}

		private void UpdateEquipmentStats()
		{
			Character character = Game1.character[0];
			if (Game1.halfSecFrame == 10)
			{
				if (this.Equipment[308] > 0)
				{
					Game1.events.InitEvent(77, isSideEvent: true);
				}
				if (this.Equipment[340] > 0 && Game1.events.regionIntroStage == 0 && Game1.hud.unlockState == 0 && Game1.map.warpStage == 0 && Game1.map.doorStage == 0)
				{
					if (character.HP > 1)
					{
						int num = Math.Max((int)((float)character.MaxHP * 0.005f), 1);
						character.HP = Math.Max(character.HP - num, 1);
						if (Game1.hud.hudDetails)
						{
							Game1.pManager.AddHP(character.Location - new Vector2(0f, 200f * Game1.worldScale), -num, critical: false, StatusEffects.Normal, 9);
						}
					}
					VibrationManager.SetBlast(0.5f, character.Location + Rand.GetRandomVector2(-200f, 200f, -240f, 0f));
					int randomInt = Rand.GetRandomInt(0, 20);
					if (randomInt == 0)
					{
						character.SetAnim("hurtup", 0, 0);
					}
					else if (randomInt < 5)
					{
						if (character.State == CharState.Grounded)
						{
							character.SetAnim("hurt0" + Rand.GetRandomInt(0, 2), 0, 0);
						}
						else
						{
							character.SetAnim("hurtup", 0, 0);
						}
					}
				}
			}
			if (this.newHealtRegen > 0)
			{
				this.healthRegenTime += Game1.FrameTime;
				if (this.healthRegenTime > 4f)
				{
					this.healthRegenTime -= 4f;
					_ = character.HP;
					character.HP = (int)MathHelper.Clamp(character.HP + this.newHealtRegen, character.HP, (int)Math.Min((float)character.MaxHP * this.bonusHealth, 9999f));
				}
			}
		}

		private void Combo(float frameTime)
		{
			if (this.comboTimer > 0f)
			{
				this.chainXPTally = (int)(((float)this.comboMeter + this.comboEnemies * (float)this.LEVEL) / 4f) * Game1.settings.ExpMult;
				if (!Game1.character[0].Holding && !Game1.hud.isPaused && Game1.hud.inventoryState == InventoryState.None)
				{
					this.comboTimer -= frameTime;
				}
				if (this.melodicMidHitTimer > 0f)
				{
					this.melodicMidHitTimer -= frameTime;
					if (this.melodicMidHitTimer < 0f)
					{
						Sound.PlayCue("melodic_hit_rhythm");
						this.UpdateMelodicTimer();
						Game1.hud.comboTextSize = Math.Max(Game1.hud.comboTextSize, 1.4f);
					}
				}
				if (this.comboTimer < 0f)
				{
					this.melodicHitTimer = (this.melodicMidHitTimer = (this.melodicHitCount = -1));
					if (Game1.hud.eventLeftOffset == 0f && Game1.events.currentEvent > 0 && !Game1.events.anyEvent)
					{
						Sound.PlayCue("combo_break");
						if (this.comboBreak != 2 && this.damageMeter > 0 && this.comboMeter > 10)
						{
							Sound.PlayCue("melodic_hit_end");
						}
					}
					this.comboBreak = 1;
					this.comboTimer = 0f;
					if (this.comboXP > 0f)
					{
						int num = (int)this.comboXP + (int)(((float)this.comboMeter + this.comboEnemies * (float)this.LEVEL) / 4f);
						this.XP += num * Game1.settings.ExpMult;
						this.comboXPTally = (int)this.comboXP;
						this.Score += num;
						this.comboXP = 0f;
						Game1.hud.expDialogueOffset = -300f;
						this.comboDisplayTimer = 8f;
						if (this.chainXPTally > 0 && Game1.hud.hudDetails)
						{
							Game1.pManager.AddXP(Game1.character[0].Location - new Vector2(30f, 130f), this.chainXPTally * Game1.settings.ExpMult, bonus: true, 9);
						}
					}
					if (this.comboMeter > this.longestChain)
					{
						this.longestChain = this.comboMeter;
					}
					if (this.comboMeter >= 1000)
					{
						Game1.awardsManager.EarnAchievement(Achievement.ComboLarge, forceCheck: false);
						Game1.questManager.SetQuestStage(7, 1);
					}
					if (this.comboMeter >= 200)
					{
						Game1.awardsManager.EarnAchievement(Achievement.ComboSmall, forceCheck: false);
					}
					this.damageMeter = 0;
					this.comboMeter = 0;
					this.comboEnemies = 0f;
					this.comboComplete = true;
				}
			}
			if (this.comboComplete)
			{
				if (Game1.hud.eventLeftOffset == 0f)
				{
					this.comboDisplayTimer -= frameTime;
				}
				if (this.comboDisplayTimer < 0f)
				{
					this.comboDisplayTimer = 0f;
					this.comboComplete = false;
					this.comboXPTally = 0;
				}
			}
		}

		public void UpdateMelodicTimer()
		{
			if (this.melodicHitCount < 36)
			{
				this.melodicMidHitTimer = 0.75f;
			}
			else if (this.melodicHitCount < 48)
			{
				this.melodicMidHitTimer = 0.5f;
			}
			else if (this.melodicHitCount < 60)
			{
				this.melodicMidHitTimer = 0.33f;
			}
			else
			{
				this.melodicMidHitTimer = 0.25f;
			}
		}

		public int[] BalanceUpgrades(int upgrades)
		{
			int[] array = new int[this.upgradedStat.Length];
			int[] array2 = new int[this.upgradedStat.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = this.upgradedStat[i];
			}
			for (int j = 0; j < upgrades; j++)
			{
				int num = 0;
				for (int k = 0; k < array2.Length; k++)
				{
					if (array2[k] < array2[num])
					{
						num = k;
					}
				}
				array[num]++;
				array2[num]++;
			}
			return array;
		}

		public void UpgradeStats(int[] upgrade)
		{
			for (int i = 0; i < this.upgradedStat.Length; i++)
			{
				this.upgradedStat[i] += (byte)upgrade[i];
			}
			this.attack = 4;
			this.defense = 0;
			this.fidget = 1;
			float num = 1f;
			int num2 = 0;
			for (int j = 0; j < this.upgradedStat[0]; j++)
			{
				num2 += (int)(100f * num);
				num += 0.2f;
			}
			int num3 = (int)MathHelper.Clamp((float)Game1.character[0].MaxHP * this.bonusHealth, 0f, 9999f) - Game1.character[0].HP;
			Game1.character[0].MaxHP = (int)MathHelper.Clamp(num2 + 80, 0f, 9999f);
			this.GetWorldExplored();
			Game1.character[0].HP = (int)MathHelper.Clamp((float)Game1.character[0].MaxHP * this.bonusHealth, 0f, 9999f) - num3;
			num = 0.5f;
			for (int k = 0; k < this.upgradedStat[1]; k++)
			{
				this.attack += (int)(12f * num);
				num += 1f;
			}
			num = 1f;
			for (int l = 0; l < this.upgradedStat[2]; l++)
			{
				this.defense += (int)(10f * num);
				num += 0.5f;
			}
			num = 1f;
			for (int m = 0; m < this.upgradedStat[3]; m++)
			{
				this.fidget += (int)(14f * num);
				num += 0.25f;
			}
			Game1.inventoryManager.UpdateEquipStats(0, updateEquip: true, string.Empty);
		}

		public void CheckLevelUp()
		{
			if (this.XP >= this.nextLevelXP && this.LEVEL < this.LEVELCap && this.playerLifeState == 0)
			{
				if (!Game1.settings.AutoLevelUp && !Game1.events.anyEvent && this.gameDifficulty > 0)
				{
					Game1.events.InitEvent(4, isSideEvent: true);
				}
				this.LevelUp();
			}
		}

		private void LevelUp()
		{
			this.prevLevelXP = this.nextLevelXP;
			this.LEVEL += 4;
			this.nextLevelXP = 0;
			int num = 1;
			for (int i = 0; i < this.LEVEL; i++)
			{
				this.nextLevelXP = Math.Max(this.nextLevelXP + num * 80, 400);
				num++;
			}
			this.luck = Math.Max(this.LEVEL / 8, 1);
			Game1.hud.levelUpQueue++;
			GetChestFromFile("Level"+(this.LEVEL/4).ToString(), Game1.pManager);
		}

		private void CheckStatDrops(ref float dropBonus, ref float goldBonus, ref float xpBonus)
		{
			if (this.currentPendant > 0)
			{
				this.CheckStat(this.currentPendant, ref dropBonus, ref goldBonus, ref xpBonus);
			}
			if (this.currentRingLeft > 0)
			{
				this.CheckStat(this.currentRingLeft, ref dropBonus, ref goldBonus, ref xpBonus);
			}
			if (this.currentRingRight > 0)
			{
				this.CheckStat(this.currentRingRight, ref dropBonus, ref goldBonus, ref xpBonus);
			}
		}

		private void CheckStat(int equipID, ref float dropBonus, ref float goldBonus, ref float xpBonus)
		{
			int flag = Game1.inventoryManager.equipItem[equipID].Flag;
			if (flag == 20)
			{
				goldBonus *= 1.2f;
			}
			if (flag == 21)
			{
				goldBonus *= 1.5f;
			}
			if (flag == 30)
			{
				dropBonus *= 1.5f;
			}
			if (flag == 31)
			{
				dropBonus *= 3f;
			}
			if (flag == 40)
			{
				xpBonus *= 1.1f;
			}
			if (flag == 41)
			{
				xpBonus *= 1.2f;
			}
			if (flag == 50)
			{
				goldBonus *= 2f;
				dropBonus *= 3f;
				xpBonus *= 1.5f;
			}
		}

		public int CombatReward(Character[] c, ParticleManager pMan, int ID)
		{
			bool flag = true;
			Vector2 loc = new Vector2(c[ID].Location.X, c[ID].Location.Y - (float)c[ID].Height);
			if (c[0].AnimName == "attackairslam" || (c[0].AnimName == "attack02" && c[0].AnimFrame > 33))
			{
				loc += new Vector2(100 * ((c[0].Face == CharDir.Right) ? 1 : (-1)), -100f);
			}
			for (int num = Game1.map.CheckCol(loc); num > 0; num = Game1.map.CheckCol(loc))
			{
				loc.Y += 64f;
			}
			int randomInt = Rand.GetRandomInt(0, 400 - (int)MathHelper.Clamp(this.luckEquip, 0f, 99f));
			int num2 = 0;
			float dropBonus = 1f;
			float goldBonus = 1f;
			float xpBonus = 1f;
			this.CheckStatDrops(ref dropBonus, ref goldBonus, ref xpBonus);
			switch (c[ID].Definition.charType)
			{
			case CharacterType.DarkVillager:
			case CharacterType.Gaius:
				return 0;
			case CharacterType.Imp:
				this.gainedXP = 5;
				num2 = Rand.GetRandomInt(1, 3);
				if ((float)randomInt < 10f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.ImpHide : MaterialType.ImpClaw, chest: false);
				}
				break;
			case CharacterType.LightBeast:
				this.gainedXP = 10;
				num2 = Rand.GetRandomInt(1, 5);
				if ((float)randomInt < 10f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.BeastLeather : MaterialType.BeastSpear, chest: false);
				}
				break;
			case CharacterType.Giant:
				this.gainedXP = 200;
				num2 = Rand.GetRandomInt(10, 40);
				this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.GiantCore : MaterialType.GiantRock, chest: false);
				break;
			case CharacterType.Slime:
				this.gainedXP = 60;
				num2 = Rand.GetRandomInt(10, 30);
				if ((float)randomInt < 50f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.SlimyCoat : MaterialType.SlimySpike, chest: false);
				}
				break;
			case CharacterType.Avee:
				this.gainedXP = 50;
				num2 = Rand.GetRandomInt(4, 20);
				if ((float)randomInt < 10f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.AveeClaw : MaterialType.AveeWing, chest: false);
				}
				break;
			case CharacterType.Florn:
				this.gainedXP = 80;
				num2 = Rand.GetRandomInt(8, 15);
				if ((float)randomInt < 40f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.FlornTentacle : MaterialType.FlornSpark, chest: false);
				}
				break;
			case CharacterType.Blomb:
			case CharacterType.BlombSnow:
				this.gainedXP = 20;
				num2 = Rand.GetRandomInt(1, 5);
				break;
			case CharacterType.Fuse:
				this.gainedXP = 3000;
				flag = false;
				break;
			case CharacterType.SquirtBug:
				this.gainedXP = 160;
				num2 = Rand.GetRandomInt(10, 15);
				if ((float)randomInt < 10f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.SquirtEye : MaterialType.SquirtArm, chest: false);
				}
				else if ((float)randomInt < 30f * dropBonus)
				{
					this.SpawnEquipment(loc, pMan, EquipType.Mushroom, bluePrint: false, chest: false);
				}
				break;
			case CharacterType.RockHound:
				this.gainedXP = 100;
				num2 = Rand.GetRandomInt(10, 15);
				if ((float)randomInt < 10f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.HoundTeeth : MaterialType.HoundHide, chest: false);
				}
				break;
			case CharacterType.StoneCutter:
				this.gainedXP = 400;
				num2 = Rand.GetRandomInt(10, 30);
				if ((float)randomInt < 20f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.StoneCarapace : MaterialType.StoneFeeler, chest: false);
				}
				if ((float)randomInt < 100f * dropBonus)
				{
					this.SpawnEquipment(loc, pMan, EquipType.Mushroom, bluePrint: false, chest: false);
				}
				break;
			case CharacterType.Trolk:
				this.gainedXP = 500;
				num2 = Rand.GetRandomInt(10, 50);
				if ((float)randomInt < 100f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.TrolkShell : MaterialType.TrolkFinger, chest: false);
				}
				break;
			case CharacterType.Lady:
				this.gainedXP = 4000;
				flag = false;
				break;
			case CharacterType.Remains:
			case CharacterType.RemainsHalf:
			case CharacterType.RemainsBomb:
				this.gainedXP = 60;
				num2 = Rand.GetRandomInt(4, 20);
				if ((float)randomInt < 2f * dropBonus)
				{
					this.SpawnEquipment(loc, pMan, EquipType.DeadlyDelight, bluePrint: false, chest: false);
				}
				else if ((float)randomInt < 10f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.FleshChunk : MaterialType.Bone, chest: false);
				}
				break;
			case CharacterType.FleshFly:
				this.gainedXP = 80;
				num2 = Rand.GetRandomInt(1, 4);
				if ((float)randomInt < 20f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.Maggot : MaterialType.Ectoplasm, chest: false);
				}
				break;
			case CharacterType.FleshFlyHive:
			{
				this.gainedXP = 800;
				num2 = Rand.GetRandomInt(20, 80);
				for (int i = 0; i < 3; i++)
				{
					this.SpawnMaterial(loc, pMan, MaterialType.Maggot, chest: false);
				}
				break;
			}
			case CharacterType.Summoner:
			{
				this.gainedXP = 1000;
				num2 = Rand.GetRandomInt(20, 200);
				this.SpawnMaterial(loc, pMan, MaterialType.LostSoul, chest: false);
				for (int j = 0; j < 2; j++)
				{
					this.SpawnMaterial(loc, pMan, MaterialType.Ectoplasm, chest: false);
				}
				break;
			}
			case CharacterType.Psylph:
				this.gainedXP = 1000;
				num2 = Rand.GetRandomInt(20, 80);
				if ((float)randomInt < 10f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.FleshChunk : MaterialType.Ectoplasm, chest: false);
				}
				break;
			case CharacterType.KaneGhost:
				this.gainedXP = 30000;
				break;
			case CharacterType.Wolf:
				this.gainedXP = 600;
				num2 = Rand.GetRandomInt(10, 30);
				if ((float)randomInt < 10f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.WolfPelt : MaterialType.Bone, chest: false);
				}
				break;
			case CharacterType.LightBeastSnow:
				this.gainedXP = 600;
				num2 = Rand.GetRandomInt(10, 30);
				if ((float)randomInt < 10f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.BeastSpear : MaterialType.BeastLeather, chest: false);
				}
				break;
			case CharacterType.Frite:
				this.gainedXP = 1800;
				num2 = Rand.GetRandomInt(10, 30);
				this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.HollowShard : MaterialType.LostSoul, chest: false);
				break;
			case CharacterType.Kush:
				this.gainedXP = 2200;
				num2 = Rand.GetRandomInt(10, 30);
				this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.KushPelt : MaterialType.ToughMetal, chest: false);
				break;
			case CharacterType.ImpLava:
				this.gainedXP = 200;
				num2 = Rand.GetRandomInt(10, 20);
				if ((float)randomInt < 10f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.ImpHide : MaterialType.ImpClaw, chest: false);
				}
				break;
			case CharacterType.Assassin:
			case CharacterType.Soldier:
			case CharacterType.WolfSoldier:
				this.gainedXP = 600;
				num2 = Rand.GetRandomInt(20, 40);
				if ((float)randomInt < 10f * dropBonus)
				{
					this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.DogTag : MaterialType.ToughMetal, chest: false);
				}
				break;
			case CharacterType.KushSoldier:
				this.gainedXP = 2400;
				num2 = Rand.GetRandomInt(20, 50);
				this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 2) == 0) ? MaterialType.KushPelt : MaterialType.ToughMetal, chest: false);
				break;
			case CharacterType.AirShip:
				this.gainedXP = 1200;
				num2 = Rand.GetRandomInt(20, 50);
				this.SpawnMaterial(loc, pMan, (Rand.GetRandomInt(0, 3) > 0) ? MaterialType.ScrapMetal : MaterialType.ToughMetal, chest: false);
				break;
			default:
				this.gainedXP = 0;
				break;
			}
			if (flag)
			{
				if ((float)randomInt < 10f * dropBonus)
				{
					int num3 = Math.Max(Rand.GetRandomInt(-7, 5), 0);
					int num4 = (int)this.ValidMaxEquip(num3);
					EquipType randomInt2;
					do
					{
						randomInt2 = (EquipType)Rand.GetRandomInt(Math.Max(num3 * Game1.inventoryManager.invSelMax, num4 - 10), num4 + 1);
					}
					while (Game1.inventoryManager.equipItem[(int)randomInt2] == null);
					this.SpawnEquipment(loc, pMan, randomInt2, num3 != 0 && Rand.GetRandomInt(0, 50) > 0, chest: false);
				}
				if (randomInt % 5 == 0)
				{
					this.SpawnMaterial(loc, pMan, (MaterialType)Rand.GetRandomInt(0, 12), chest: false);
				}
				if (c[ID].Ai != null)
				{
					num2 = (int)((float)num2 * c[ID].Ai.regionMultiplier);
					this.gainedXP = (int)((float)this.gainedXP * c[ID].Ai.regionMultiplier);
				}
				num2 = (int)((float)num2 * goldBonus);
				if (num2 > 0)
				{
					int num5 = num2 / 400;
					int num6 = (num2 - num5 * 400) / 100;
					int num7 = (num2 - num5 * 400 - num6 * 100) / 30;
					int num8 = (num2 - num5 * 400 - num6 * 100 - num7 * 30) / 5;
					num2 = num2 - num5 * 400 - num6 * 100 - num7 * 30 - num8 * 5;
					for (int k = 0; k < num5; k++)
					{
						this.SpawnGold(loc, pMan, 400);
					}
					for (int l = 0; l < num6; l++)
					{
						this.SpawnGold(loc, pMan, 100);
					}
					for (int m = 0; m < num7; m++)
					{
						this.SpawnGold(loc, pMan, 30);
					}
					for (int n = 0; n < num8; n++)
					{
						this.SpawnGold(loc, pMan, 5);
					}
					for (int num9 = 0; num9 < num2; num9++)
					{
						this.SpawnGold(loc, pMan, 1);
					}
				}
			}
			this.gainedXP = (int)((float)this.gainedXP * xpBonus);
			this.comboXP += this.gainedXP;
			this.enemiesDefeated++;
			if (this.enemiesDefeated >= 500)
			{
				Game1.awardsManager.EarnAchievement(Achievement.KillEnemies, forceCheck: false);
			}
			this.comboEnemies += 1f;
			return this.gainedXP;
		}

		public void GetWorldExplored()
		{
			if (Game1.navManager == null || Game1.navManager.RevealMap == null)
			{
				return;
			}
			float num = 0f;
			float num2 = 0f;
			int num3 = 0;
			int num4 = 0;
			foreach (KeyValuePair<string, RevealMap> item in Game1.navManager.RevealMap)
			{
				if (!(item.Key != "trial"))
				{
					continue;
				}
				num += item.Value.GetTotalExplored();
				num2 += item.Value.GetTreasureFound();
				for (int i = 0; i < item.Value.CageList.Count; i++)
				{
					num4++;
					if (item.Value.CageList[i].Stage > 0)
					{
						num3++;
					}
				}
			}
			this.bonusHealth = 1f;
			for (int j = 0; j < num3; j++)
			{
				this.bonusHealth += 0.05f;
			}
			int num5 = Game1.navManager.RevealMap.Count - 1;
			this.FriendsFound = (int)((float)num3 / (float)num4 * 100f);
			this.Explored = (int)(num / (float)num5);
			this.TreasureFound = (int)(num2 / (float)num5);
			float num6 = MathHelper.Clamp((float)Game1.events.currentEvent / 680f * 100f, 0f, 100f);
			float num7 = 0f;
			for (int k = 0; k < Game1.questManager.availableQuests.Count; k++)
			{
				for (int l = 1; l < Game1.questManager.activeQuest.Count; l++)
				{
					if (Game1.questManager.activeQuest[l].GetType() == Game1.questManager.availableQuests[k].GetType())
					{
						num7 += 25f;
					}
				}
				for (int m = 0; m < Game1.questManager.completedQuest.Count; m++)
				{
					if (Game1.questManager.completedQuest[m].GetType() == Game1.questManager.availableQuests[k].GetType())
					{
						num7 += 100f;
					}
				}
			}
			num7 /= (float)(Game1.questManager.availableQuests.Count - 1);
			int num8 = 0;
			for (int n = 0; n < this.Material.Length; n++)
			{
				if (Game1.inventoryManager.material[n] != null)
				{
					num8++;
				}
			}
			int num9 = 0;
			for (int num10 = 0; num10 < this.shopMaterial.Length; num10++)
			{
				if (this.shopMaterial[num10] > 0)
				{
					num9++;
				}
			}
			float num11 = (float)num9 / (float)num8 * 100f;
			int num12 = 0;
			for (int num13 = 0; num13 < Game1.cManager.challengeArenas.Count; num13++)
			{
				if (Game1.cManager.challengeArenas[num13].RankScore > 0)
				{
					num12 = Math.Max(num12, num13 + 1);
				}
			}
			float num14 = 0f;
			for (int num15 = 0; num15 < num12; num15++)
			{
				num14 += (float)Game1.cManager.challengeArenas[num15].CheckStarCount() * 25f;
			}
			num14 /= (float)num12;
			this.Completion = MathHelper.Clamp((num6 + num7 + (float)this.FriendsFound + (float)this.Explored + (float)this.TreasureFound + num11 + num14) / 7f * 1.175f, 1f, 117f);
		}

		public void SendFidget(FidgetState state, float timer)
		{
			this.fidgetState = state;
			if (this.fidgetAwayTime == 0f)
			{
				this.fidgetAwayTime = timer;
			}
		}

		public void Update(GameTime gameTime, float frameTime)
		{
			if (frameTime > 0f && this.playerLifeState == 0 && !Game1.events.anyEvent && Game1.menu.prompt == promptDialogue.None)
			{
				this.UpdateEquipmentStats();
			}
			Character character = Game1.character[0];
			switch (this.playerLifeState)
			{
			case 0:
				if (character.HP > 0 || Game1.events.anyEvent)
				{
					break;
				}
				this.prevWeatherColor = Game1.wManager.weatherColor;
				this.prevWeatherSaturation = Game1.wManager.weatherSaturation;
				this.prevWeatherBloom = Game1.wManager.weatherBloom;
				Game1.wManager.weatherColor = new Vector4(1f, 0f, 0f, 0f);
				Game1.wManager.weatherSaturation = 1.6f;
				Game1.wManager.weatherBloom = 0.3f;
				if (character.AnimName != "hurtup" && character.AnimName != "hurtland")
				{
					character.SetAnim("hurtup", 0, 0);
				}
				Sound.PlayCue("dustdie");
				if (this.Equipment[340] > 0)
				{
					this.playerLifeState = 2;
					this.reviveTimer = 0.2f;
					Game1.SlowTime = 3f;
					Game1.pManager.InitFidgetThrow();
				}
				else if (this.Equipment[301] > 0 || this.gameDifficulty == 0 || (Game1.GamerServices && Game1.IsTrial))
				{
					if (this.gameDifficulty > 0)
					{
						this.Equipment[301] = (byte)Math.Max(this.Equipment[301] - 1, 0);
					}
					this.playerLifeState = 2;
					this.reviveTimer = 0.2f;
					Game1.SlowTime = 3f;
					Game1.pManager.InitFidgetThrow();
				}
				else
				{
					this.playerLifeState = 1;
					this.reviveTimer = 1f;
					Game1.SlowTime = 3f;
				}
				break;
			case 1:
				character.Ethereal = EtherealState.Ethereal;
				if (character.State == CharState.Grounded)
				{
					character.SetAnim("hurtland", 0, 0);
				}
				character.StatusTime = 0f;
				character.StatusEffect = StatusEffects.Normal;
				if (this.reviveTimer > 0f)
				{
					this.reviveTimer -= frameTime;
					if (this.reviveTimer < 0f)
					{
						this.reviveTimer = 0f;
						Game1.menu.prompt = promptDialogue.Dead;
						Game1.menu.ClearPrompt();
						GC.Collect();
					}
				}
				else if (Game1.menu.prompt == promptDialogue.None && Game1.gameMode == Game1.GameModes.Game)
				{
					this.reviveTimer = 0.1f;
				}
				break;
			case 2:
				this.reviveTimer -= frameTime;
				if (this.reviveTimer <= 0f)
				{
					this.playerLifeState = 3;
				}
				break;
			case 3:
				this.playerLifeState = 0;
				Game1.SlowTime = 0f;
				Game1.hud.LevelUpEffect(Game1.pManager);
				if (Game1.GamerServices && Game1.IsTrial)
				{
					Game1.pManager.AddCaption(new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 3 - 30), Strings_Hud.Status_Revive_Trial, 1.2f, new Color(0.5f, 1f, 1f, 1f), 3f, 9);
				}
				else if (this.gameDifficulty > 0 && this.Equipment[340] == 0)
				{
					Game1.pManager.AddCaption(new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 3 - 30), Strings_Hud.Status_Revive, 1.2f, new Color(0.5f, 1f, 1f, 1f), 3f, 9);
				}
				character.CanHurtFrame = (character.CanHurtProjectileFrame = 4f);
				if (character.HP <= 0)
				{
					character.HP = (int)Math.Min((float)character.MaxHP * this.bonusHealth, 9999f);
				}
				if (Game1.map.CheckCol(character.Location - new Vector2(0f, 96f)) > 0)
				{
					character.SetAnim("crouch", 0, 0);
				}
				Game1.wManager.weatherColor = this.prevWeatherColor;
				Game1.wManager.weatherSaturation = this.prevWeatherSaturation;
				Game1.wManager.weatherBloom = this.prevWeatherBloom;
				if (this.Equipment[340] > 0)
				{
					Game1.events.InitEvent(74, isSideEvent: true);
				}
				break;
			}
			if (Game1.skipFrame > 1 && character.HP > 0)
			{
				if (this.curCharge < (float)this.maxCharge)
				{
					if (this.gameDifficulty == 0)
					{
						this.curCharge += frameTime * 200f;
					}
					else
					{
						this.curCharge += frameTime * 16f / (float)(int)this.gameDifficulty;
					}
				}
				else if (this.curCharge > (float)this.maxCharge)
				{
					this.curCharge = this.maxCharge;
				}
				if (this.curCharge < (float)this.projectileCost)
				{
					this.canThrow = false;
				}
				else
				{
					this.canThrow = true;
				}
			}
			if (Game1.gameMode != Game1.GameModes.Menu)
			{
				this.gameClock += (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			if (Game1.hud.dialogueState == DialogueState.Inactive && Game1.menu.prompt == promptDialogue.None)
			{
				if (this.attackBonusTime > 0f)
				{
					this.attackBonusTime -= frameTime;
					if (this.attackBonusTime < 0f)
					{
						this.attackBonusTime = 0f;
					}
				}
				if (this.defenseBonusTime > 0f)
				{
					this.defenseBonusTime -= frameTime;
					if (this.defenseBonusTime < 0f)
					{
						this.defenseBonusTime = 0f;
					}
				}
			}
			if (this.melodicHitTimer > 0f)
			{
				this.melodicHitTimer -= frameTime;
			}
			if (this.fidgetAwayTime > 0f)
			{
				this.fidgetAwayTime -= Game1.HudTime;
				if (this.fidgetAwayTime < 0f)
				{
					this.fidgetAwayTime = 0f;
					if (this.fidgetState == FidgetState.Shopping)
					{
						this.fidgetState = FidgetState.Normal;
						if (Game1.pManager.GetFidgetLoc(accomodateScroll: true) == Vector2.Zero)
						{
							Game1.pManager.AddFidget(Game1.character[0].Location + new Vector2(1000f, -200f));
						}
					}
				}
			}
			this.Combo(frameTime);
			this.CheckLevelUp();
			if (Game1.hud.levelUpEffect && Game1.hud.dialogueState == DialogueState.Inactive && !Game1.events.anyEvent)
			{
				Game1.pManager.AddCaption(new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 3 - 30), Strings_Hud.Status_LevelUp, 1.2f, new Color(0.5f, 1f, 1f, 1f), 3f, 9);
				Game1.hud.LevelUpEffect(Game1.pManager);
				Game1.hud.levelUpEffect = false;
			}
		}
	}
}
