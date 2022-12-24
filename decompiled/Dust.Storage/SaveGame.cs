using System;
using System.Collections.Generic;
using System.IO;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.Storage
{
	public class SaveGame
	{
		public void Write(BinaryWriter writer)
		{
			Character character = Game1.character[0];
			int num = (int)Math.Min((float)character.MaxHP * Game1.stats.bonusHealth, 9999f);
			int val = (int)Math.Max((float)num * (1f - (float)(Game1.stats.gameDifficulty + 1) * 0.25f), (float)num * 0.125f);
			if (Game1.stats.gameDifficulty == 0)
			{
				val = num;
			}
			if (Game1.hud.savePos != Vector2.Zero)
			{
				Game1.hud.playerSaveLoc.X = Game1.hud.savePos.X;
			}
			writer.Write(Game1.Build);
			writer.Write(DateTime.Now.Ticks);
			writer.Write((int)Game1.stats.Completion);
			writer.Write(Game1.stats.gameComplete);
			writer.Write(Game1.events.regionDisplayName);
			writer.Write(Math.Max(character.HP, val));
			writer.Write(character.MaxHP);
			writer.Write(Game1.stats.bonusHealth);
			writer.Write(Game1.stats.LEVEL);
			writer.Write(Game1.stats.Gold);
			writer.Write(Game1.stats.gameDifficulty);
			writer.Write(Game1.stats.startDifficulty);
			writer.Write(Game1.stats.gameClock);
			writer.Write(Game1.stats.XP);
			writer.Write(Game1.stats.prevLevelXP);
			writer.Write(Game1.stats.nextLevelXP);
			writer.Write(Game1.stats.skillPoints);
			string value = "noname";
			if (Game1.GamerServices && Game1.awardsManager.MeSignedInGamer() != null)
			{
				value = Game1.awardsManager.MeSignedInGamer().Gamertag;
			}
			writer.Write(Game1.stats.canEarnAchievements);
			writer.Write(value);
			for (int i = 0; i < Game1.stats.upgrade.Length; i++)
			{
				writer.Write(Game1.stats.upgrade[i]);
			}
			for (int j = 0; j < Game1.stats.upgradedStat.Length; j++)
			{
				writer.Write(Game1.stats.upgradedStat[j]);
			}
			for (int k = 0; k < Game1.stats.achievementEarned.Length; k++)
			{
				writer.Write(Game1.stats.achievementEarned[k]);
			}
			writer.Write(Game1.stats.projectileType);
			writer.Write(Game1.stats.curCharge);
			for (int l = 0; l < Game1.stats.Equipment.Length; l++)
			{
				writer.Write(Game1.stats.Equipment[l]);
			}
			for (int m = 0; m < Game1.stats.EquipBluePrint.Length; m++)
			{
				writer.Write(Game1.stats.EquipBluePrint[m]);
			}
			for (int n = 0; n < Game1.stats.shopEquipment.Length; n++)
			{
				writer.Write(Game1.stats.shopEquipment[n]);
			}
			for (int num2 = 0; num2 < Game1.stats.Material.Length; num2++)
			{
				writer.Write(Game1.stats.Material[num2]);
			}
			for (int num3 = 0; num3 < Game1.stats.shopMaterial.Length; num3++)
			{
				writer.Write(Game1.stats.shopMaterial[num3]);
			}
			for (int num4 = 0; num4 < Game1.stats.StructureBuilt.Length; num4++)
			{
				writer.Write(Game1.stats.StructureBuilt[num4]);
			}
			writer.Write(Game1.hud.shopRestockTime);
			writer.Write(Game1.stats.attack);
			writer.Write(Game1.stats.defense);
			writer.Write(Game1.stats.fidget);
			writer.Write(Game1.stats.luck);
			writer.Write(Game1.stats.attackEquip);
			writer.Write(Game1.stats.defenseEquip);
			writer.Write(Game1.stats.newFidgetEquip);
			writer.Write(Game1.stats.luckEquip);
			writer.Write(Game1.stats.newHealtRegen);
			writer.Write(Game1.stats.currentItem);
			writer.Write(Game1.stats.currentPendant);
			writer.Write(Game1.stats.currentAugment);
			writer.Write(Game1.stats.currentArmor);
			writer.Write(Game1.stats.currentRingLeft);
			writer.Write(Game1.stats.currentRingRight);
			for (int num5 = 0; num5 < Game1.cManager.challengeArenas.Count; num5++)
			{
				writer.Write(Game1.cManager.challengeArenas[num5].HighScore);
			}
			writer.Write(Game1.stats.enemiesDefeated);
			writer.Write(Game1.stats.longestChain);
			writer.Write(Game1.events.currentEvent);
			for (int num6 = 0; num6 < Game1.events.sideEventAvailable.Length; num6++)
			{
				writer.Write(Game1.events.sideEventAvailable[num6]);
			}
			for (int num7 = 0; num7 < Game1.stats.villagerDialogue.Length; num7++)
			{
				writer.Write(Game1.stats.villagerDialogue[num7]);
			}
			for (int num8 = 0; num8 < Game1.events.regionIntroduced.Length; num8++)
			{
				writer.Write(Game1.events.regionIntroduced[num8]);
			}
			writer.Write(Game1.hud.playerSaveLoc.X);
			writer.Write(Game1.hud.playerSaveLoc.Y - (float)((character.State == CharState.Grounded) ? 200 : 0));
			writer.Write(Game1.map.path);
			writer.Write(Music.GetMusicName());
			writer.Write(Sound.fadeMusic);
			writer.Write((int)Game1.wManager.weatherType);
			writer.Write((int)Game1.wManager.randomType);
			writer.Write(Game1.wManager.weatherTimer);
			writer.Write((int)Game1.wManager.weatherAudioType);
			Game1.questManager.WriteQuests(writer);
			Game1.navManager.WriteSaveFile(writer);
		}

		public bool Read(BinaryReader reader)
		{
			Character character = Game1.character[0];
			reader.ReadSingle();
			reader.ReadInt64();
			Game1.stats.Completion = reader.ReadInt32();
			Game1.stats.gameComplete = reader.ReadBoolean();
			Game1.events.regionDisplayName = reader.ReadString();
			character.HP = (character.pHP = (character.HP = reader.ReadInt32()));
			character.MaxHP = reader.ReadInt32();
			reader.ReadSingle();
			Game1.stats.LEVEL = reader.ReadInt32();
			Game1.stats.Gold = reader.ReadInt32();
			Game1.stats.gameDifficulty = reader.ReadByte();
			Game1.stats.startDifficulty = reader.ReadByte();
			Game1.stats.gameClock = reader.ReadSingle();
			Game1.stats.XP = reader.ReadInt32();
			Game1.stats.prevLevelXP = reader.ReadInt32();
			Game1.stats.nextLevelXP = reader.ReadInt32();
			Game1.stats.skillPoints = reader.ReadInt32();
			Game1.stats.canEarnAchievements = reader.ReadBoolean();
			string text = reader.ReadString();
			if (Game1.GamerServices && Game1.awardsManager.MeSignedInGamer() != null && text != Game1.awardsManager.MeSignedInGamer().Gamertag)
			{
				Game1.stats.canEarnAchievements = false;
			}
			for (int i = 0; i < Game1.stats.upgrade.Length; i++)
			{
				Game1.stats.upgrade[i] = reader.ReadByte();
			}
			for (int j = 0; j < Game1.stats.upgradedStat.Length; j++)
			{
				Game1.stats.upgradedStat[j] = reader.ReadByte();
			}
			for (int k = 0; k < Game1.stats.achievementEarned.Length; k++)
			{
				Game1.stats.achievementEarned[k] = reader.ReadBoolean();
			}
			Game1.settings.SyncAchievements();
			Game1.stats.projectileType = reader.ReadInt32();
			Game1.stats.curCharge = reader.ReadSingle();
			for (int l = 0; l < Game1.stats.Equipment.Length; l++)
			{
				Game1.stats.Equipment[l] = reader.ReadByte();
			}
			for (int m = 0; m < Game1.stats.EquipBluePrint.Length; m++)
			{
				Game1.stats.EquipBluePrint[m] = reader.ReadByte();
			}
			for (int n = 0; n < Game1.stats.shopEquipment.Length; n++)
			{
				Game1.stats.shopEquipment[n] = reader.ReadByte();
			}
			for (int num = 0; num < Game1.stats.Material.Length; num++)
			{
				Game1.stats.Material[num] = reader.ReadInt32();
			}
			for (int num2 = 0; num2 < Game1.stats.shopMaterial.Length; num2++)
			{
				Game1.stats.shopMaterial[num2] = reader.ReadInt32();
			}
			for (int num3 = 0; num3 < Game1.stats.StructureBuilt.Length; num3++)
			{
				Game1.stats.StructureBuilt[num3] = reader.ReadBoolean();
			}
			Game1.hud.shopRestockTime = reader.ReadSingle();
			Game1.stats.attack = reader.ReadInt32();
			Game1.stats.defense = reader.ReadInt32();
			Game1.stats.fidget = reader.ReadInt32();
			Game1.stats.luck = reader.ReadInt32();
			Game1.stats.attackEquip = reader.ReadInt32();
			Game1.stats.defenseEquip = reader.ReadInt32();
			Game1.stats.newFidgetEquip = reader.ReadInt32();
			Game1.stats.luckEquip = reader.ReadInt32();
			Game1.stats.newHealtRegen = reader.ReadInt32();
			Game1.stats.currentItem = reader.ReadInt32();
			Game1.stats.currentPendant = reader.ReadInt32();
			Game1.stats.currentAugment = reader.ReadInt32();
			Game1.stats.currentArmor = reader.ReadInt32();
			Game1.stats.currentRingLeft = reader.ReadInt32();
			Game1.stats.currentRingRight = reader.ReadInt32();
			for (int num4 = 0; num4 < Game1.cManager.challengeArenas.Count; num4++)
			{
				Game1.cManager.challengeArenas[num4].HighScore = reader.ReadInt32();
			}
			Game1.stats.enemiesDefeated = reader.ReadInt32();
			Game1.stats.longestChain = reader.ReadInt32();
			Game1.events.currentEvent = reader.ReadInt32();
			for (int num5 = 0; num5 < Game1.events.sideEventAvailable.Length; num5++)
			{
				Game1.events.sideEventAvailable[num5] = reader.ReadBoolean();
			}
			for (int num6 = 0; num6 < Game1.stats.villagerDialogue.Length; num6++)
			{
				Game1.stats.villagerDialogue[num6] = reader.ReadInt32();
			}
			for (int num7 = 0; num7 < Game1.events.regionIntroduced.Length; num7++)
			{
				Game1.events.regionIntroduced[num7] = reader.ReadBoolean();
			}
			for (int num8 = 1; num8 < Game1.character.Length; num8++)
			{
				Game1.character[num8].Exists = CharExists.Dead;
			}
			character.UnloadTextures();
			character.Location = new Vector2(reader.ReadSingle(), reader.ReadSingle() - 100f);
			Game1.map.transDir = TransitionDirection.None;
			Game1.map.path = reader.ReadString();
			string musicString = reader.ReadString();
			float time = reader.ReadSingle();
			int weather = reader.ReadInt32();
			int randomType = reader.ReadInt32();
			float weatherTimer = reader.ReadSingle();
			int weather2 = reader.ReadInt32();
			try
			{
				Game1.questManager.ReadQuests(reader);
				Game1.navManager.ReadSaveFile(reader);
			}
			catch
			{
				return false;
			}
			Game1.pManager.Reset(removeWeather: true, removeBombs: true);
			Game1.map.SwitchMap(Game1.pManager, Game1.character, Game1.map.path, loading: true);
			Game1.wManager.SetWeather((WeatherType)weather, forceReset: true);
			Game1.wManager.randomType = (RandomType)randomType;
			Game1.wManager.weatherTimer = weatherTimer;
			WeatherAudio.Play((WeatherAudioType)weather2);
			Music.Play(null);
			Music.Play(musicString);
			Sound.FadeMusicOut(time);
			this.ReadyCharacter();
			return true;
		}

		public void CheckMemory(BinaryReader reader, int slot)
		{
			reader.ReadSingle();
			long num = reader.ReadInt64();
			if (Game1.menu.prompt != 0 && num >= Game1.menu.mostRecentTime)
			{
				Game1.menu.mostRecentTime = num;
				Game1.stats.saveSlot = (Game1.menu.tempSaveSlot = (byte)slot);
			}
			Game1.menu.saveCompletion[slot] = reader.ReadInt32();
			if (reader.ReadBoolean())
			{
				Game1.menu.canNewGamePlus = true;
			}
			Game1.menu.saveRegionName[slot] = reader.ReadString();
			Game1.menu.saveHP[slot] = reader.ReadInt32();
			Game1.menu.saveMaxHP[slot] = (int)Math.Min((float)reader.ReadInt32() * reader.ReadSingle(), 9999f);
			Game1.menu.saveLevel[slot] = reader.ReadInt32();
			Game1.menu.saveGold[slot] = reader.ReadInt32();
			Game1.menu.saveDifficulty[slot] = (int)reader.ReadByte();
			reader.ReadByte();
			Game1.menu.saveGameClock[slot] = reader.ReadSingle();
		}

		private void ReadyCharacter()
		{
			Character character = Game1.character[0];
			if (character.HP < 1)
			{
				character.HP = 1;
			}
			if (Game1.stats.playerLifeState != 0)
			{
				Game1.stats.playerLifeState = 2;
			}
			character.LifeBarPercent = 0f;
			character.HPLossFrame = 0f;
			character.Trajectory = Vector2.Zero;
			character.State = CharState.Air;
			character.SetAnim("idle00", 0, 2);
			character.Floating = false;
			character.StatusEffect = StatusEffects.Normal;
			character.StatusTime = 0f;
			character.GroundCharacter();
			character.SetAnim("idle01", 0, 2);
			Game1.stats.inFront = true;
			Game1.events.skippable = SkipType.Skippable;
			Game1.events.eventType = EventType.None;
			Game1.events.anyEvent = (Game1.hud.inBoss = false);
			Game1.hud.bossLife = (Game1.hud.bossMaxLife = 0f);
			Game1.SlowTime = 0f;
			Game1.pManager.ResetFidget(Game1.character);
			for (int i = 0; i < Game1.character.Length; i++)
			{
				if (Game1.character[i].Exists == CharExists.Exists)
				{
					Game1.character[i].Holding = false;
					Game1.character[i].GrabbedBy = -1;
				}
			}
			Game1.map.leftBlock = (Game1.map.rightBlock = 0f);
			Game1.worldScale = 1f;
			Game1.camera.ResetCamera(Game1.character);
			Game1.stats.projectileCost = Game1.stats.GetProjectileCost(Game1.stats.projectileType);
			Game1.cManager.ExitScoreBoard(canRestart: true);
			Game1.stats.GetWorldExplored();
		}

		public SaveItem GetItemID(List<SaveItem> itemList, string newID)
		{
			try
			{
				int index = -1;
				for (int i = 0; i < itemList.Count; i++)
				{
					if (itemList[i].UniqueID == newID)
					{
						index = i;
						break;
					}
				}
				return itemList[index];
			}
			catch
			{
				return null;
			}
		}

		public int GetItemArrayID(List<SaveItem> itemList, int newID, string newUniqueID)
		{
			int result = -1;
			for (int i = 0; i < itemList.Count; i++)
			{
				if (itemList[i].ID == newID && (newUniqueID == null || itemList[i].UniqueID == newUniqueID))
				{
					result = i;
				}
			}
			return result;
		}
	}
}
