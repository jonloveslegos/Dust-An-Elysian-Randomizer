using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dust.CharClasses;
using Dust.Quests.SideQuests;
using Dust.Strings;
using Lotus.Threading;
using Microsoft.Xna.Framework;

namespace Dust.Quests
{
	public class QuestManager
	{
		public Dictionary<int, Quest> availableQuests = new Dictionary<int, Quest>();

		public List<Quest> activeQuest = new List<Quest>();

		public List<Quest> completedQuest = new List<Quest>();

		public List<Notes> notes = new List<Notes>();

		private static int stringWidth;

		public bool updatingQuests;

		public QuestManager()
		{
			this.availableQuests.Clear();
			this.availableQuests.Add(0, new _000_Main());
			this.availableQuests.Add(1, new _001_ImpHides());
			this.availableQuests.Add(2, new _002_BeastSpears());
			this.availableQuests.Add(3, new _003_Slime());
			this.availableQuests.Add(4, new _004_BlackSmith());
			this.availableQuests.Add(5, new _005_RestockShop());
			this.availableQuests.Add(6, new _006_Laundry());
			this.availableQuests.Add(7, new _007_HighCombo());
			this.availableQuests.Add(8, new _008_Reunion());
			this.availableQuests.Add(9, new _009_ReedMemory());
			this.availableQuests.Add(10, new _010_Autograph());
			this.availableQuests.Add(11, new _011_LostFlock());
			this.availableQuests.Add(12, new _012_RedMoss());
			this.availableQuests.Add(13, new _013_BlopStick());
			this.availableQuests.Add(14, new _014_Trolks());
			this.availableQuests.Add(15, new _015_Doll());
			this.availableQuests.Add(16, new _016_Dentures());
			this.availableQuests.Add(17, new _017_GiantArmor());
			this.availableQuests.Add(18, new _018_AveeClaws());
			this.availableQuests.Add(19, new _019_FlornSparks());
			this.availableQuests.Add(20, new _020_PocketWatch());
			this.availableQuests.Add(21, new _021_BopoSnow());
		}

		public void ResetQuests()
		{
			int num = 0;
			while (this.updatingQuests)
			{
				num++;
				if (num > 1000)
				{
					break;
				}
			}
			for (int i = 0; i < this.availableQuests.Count; i++)
			{
				this.availableQuests[i].ResetQuest();
			}
			this.activeQuest.Clear();
			this.activeQuest.Add(new _000_Main());
			this.completedQuest.Clear();
			this.notes.Clear();
		}

		public void WriteQuests(BinaryWriter writer)
		{
			writer.Write(this.activeQuest.Count);
			for (int i = 0; i < this.activeQuest.Count; i++)
			{
				writer.Write(this.activeQuest[i].QuestID);
				writer.Write(this.activeQuest[i].stage);
				writer.Write(this.activeQuest[i].prevStage);
				for (int j = 0; j < this.activeQuest[i].sideStage.Length; j++)
				{
					writer.Write(this.activeQuest[i].sideStage[j]);
				}
			}
			writer.Write(this.completedQuest.Count);
			for (int k = 0; k < this.completedQuest.Count; k++)
			{
				writer.Write(this.completedQuest[k].QuestID);
			}
			writer.Write(this.notes.Count);
			for (int l = 0; l < this.notes.Count; l++)
			{
				writer.Write(this.notes[l].NoteID);
			}
		}

		public void ReadQuests(BinaryReader reader)
		{
			this.activeQuest.Clear();
			this.completedQuest.Clear();
			this.notes.Clear();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				this.AddFromReader(reader.ReadInt32(), reader.ReadInt32(), isActive: true);
				this.activeQuest[this.activeQuest.Count - 1].prevStage = reader.ReadInt32();
				for (int j = 0; j < this.activeQuest[this.activeQuest.Count - 1].sideStage.Length; j++)
				{
					this.activeQuest[this.activeQuest.Count - 1].sideStage[j] = reader.ReadBoolean();
				}
			}
			int num2 = reader.ReadInt32();
			for (int k = 0; k < num2; k++)
			{
				this.AddFromReader(reader.ReadInt32(), -1, isActive: false);
			}
			int num3 = reader.ReadInt32();
			for (int l = 0; l < num3; l++)
			{
				this.AddNote(reader.ReadInt32(), loading: true);
			}
		}

		private void AddFromReader(int quest, int stage, bool isActive)
		{
			Quest quest2 = this.availableQuests[quest];
			quest2.stage = stage;
			if (isActive)
			{
				Game1.questManager.activeQuest.Add(quest2);
				return;
			}
			quest2.MaxOutStages();
			Game1.questManager.completedQuest.Add(quest2);
		}

		public void AddNote(int noteID, bool loading)
		{
			for (int i = 0; i < this.notes.Count; i++)
			{
				if (this.notes[i].NoteID == noteID)
				{
					return;
				}
			}
			Notes notes = new Notes(noteID);
			if (notes.Name != null)
			{
				this.notes.Add(notes);
				if (!loading)
				{
					Game1.hud.InitQuestMiniPrompt("[ICON_362] " + Strings_Hud.Mini_NoteAdded + "\n[BACK] " + notes.Name, 2, noteID);
				}
			}
		}

		public void AddQuest(Quest newQuest)
		{
			for (int i = 0; i < this.availableQuests.Count; i++)
			{
				if (this.availableQuests[i].GetType() == newQuest.GetType())
				{
					this.AddQuest(i);
				}
			}
		}

		public void AddQuest(int questID)
		{
			if (questID < 0 || questID > this.availableQuests.Count())
			{
				return;
			}
			for (int i = 0; i < this.activeQuest.Count; i++)
			{
				if (this.activeQuest[i].GetType() == this.availableQuests[questID].GetType())
				{
					return;
				}
			}
			for (int j = 0; j < this.completedQuest.Count; j++)
			{
				if (this.completedQuest[j].GetType() == this.availableQuests[questID].GetType())
				{
					return;
				}
			}
			int num = 0;
			while (this.updatingQuests)
			{
				num++;
				if (num > 1000)
				{
					break;
				}
			}
			Quest item = this.activeQuest[this.activeQuest.Count - 1];
			this.activeQuest.Remove(item);
			this.availableQuests[questID].stage = 0;
			this.activeQuest.Add(this.availableQuests[questID]);
			this.activeQuest.Add(item);
			Game1.hud.InitQuestMiniPrompt("[ICON_360] " + Strings_Hud.Mini_QuestAdded + "\n[BACK] " + this.availableQuests[questID].Name, 0, questID);
			this.UpdateQuests(0);
		}

		public bool IfQuestCompleted(int questID)
		{
			for (int i = 0; i < this.completedQuest.Count; i++)
			{
				if (this.completedQuest[i].GetType() == this.availableQuests[questID].GetType())
				{
					return true;
				}
			}
			return false;
		}

		public bool IfQuestActive(int questID)
		{
			for (int i = 0; i < this.activeQuest.Count; i++)
			{
				if (this.activeQuest[i].GetType() == this.availableQuests[questID].GetType() && this.activeQuest[i].isActive)
				{
					this.activeQuest[i].Update();
					return true;
				}
			}
			return false;
		}

		public bool IfQuestStageMet(int questID, int stage)
		{
			for (int i = 0; i < this.activeQuest.Count; i++)
			{
				if (this.activeQuest[i].GetType() == this.availableQuests[questID].GetType() && this.activeQuest[i].isActive)
				{
					this.activeQuest[i].Update();
					if (this.activeQuest[i].stage >= stage)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IfQuestStageEquals(int questID, int stage)
		{
			for (int i = 0; i < this.activeQuest.Count; i++)
			{
				if (this.activeQuest[i].GetType() == this.availableQuests[questID].GetType() && this.activeQuest[i].isActive)
				{
					this.activeQuest[i].Update();
					if (this.activeQuest[i].stage == stage)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IfQuestRequirementMet(int questID)
		{
			for (int i = 0; i < this.activeQuest.Count; i++)
			{
				if (this.activeQuest[i].GetType() == this.availableQuests[questID].GetType())
				{
					this.activeQuest[i].Update();
					if (this.activeQuest[i].requirementMet)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IfNoteFound(int noteID)
		{
			for (int i = 0; i < this.notes.Count; i++)
			{
				if (this.notes[i].NoteID == noteID)
				{
					return true;
				}
			}
			return false;
		}

		public bool CheckQuestGivers(Character c)
		{
			if (c.NPC == NPCType.Friendly)
			{
				for (int i = 0; i < this.availableQuests.Count; i++)
				{
					bool flag = false;
					for (int j = 0; j < this.activeQuest.Count; j++)
					{
						if (this.activeQuest[j].GetType() == this.availableQuests[i].GetType())
						{
							flag = true;
						}
						if (this.activeQuest[j].GetCurrentQuestGiver() == c.Definition.charType)
						{
							return true;
						}
					}
					if (flag)
					{
						continue;
					}
					bool flag2 = false;
					for (int k = 0; k < this.completedQuest.Count; k++)
					{
						if (this.completedQuest[k].GetType() == this.availableQuests[i].GetType())
						{
							flag2 = true;
						}
					}
					if (!flag2 && this.availableQuests[i].GetCurrentQuestGiver() == c.Definition.charType)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void SetQuestStage(int questID, int stage)
		{
			for (int i = 0; i < this.activeQuest.Count; i++)
			{
				if (this.activeQuest[i].GetType() == this.availableQuests[questID].GetType())
				{
					if (this.activeQuest[i].stage < stage)
					{
						this.activeQuest[i].stage = stage;
					}
					this.activeQuest[i].Update();
				}
			}
			this.UpdateQuests(0);
		}

		public void SetQuestSideStage(int questID, int sideStage)
		{
			for (int i = 0; i < this.activeQuest.Count; i++)
			{
				if (this.activeQuest[i].GetType() == this.availableQuests[questID].GetType())
				{
					if (this.activeQuest[i].sideStage.Length > sideStage)
					{
						this.activeQuest[i].sideStage[sideStage] = true;
					}
					this.activeQuest[i].Update();
				}
			}
			this.UpdateQuests(0);
		}

		public void CompleteQuest(int questID)
		{
			for (int i = 0; i < this.activeQuest.Count; i++)
			{
				if (this.activeQuest[i].GetType() == this.availableQuests[questID].GetType())
				{
					this.activeQuest[i].Complete();
				}
			}
			this.UpdateQuests(0);
		}

		public void UpdateQuests(int _stringWidth)
		{
			for (int i = 0; i < this.activeQuest.Count - 1; i++)
			{
				this.activeQuest[i].Update();
				if (this.activeQuest[i].isActive)
				{
					continue;
				}
				int num = 0;
				while (this.updatingQuests)
				{
					num++;
					if (num > 1000)
					{
						break;
					}
				}
				this.completedQuest.Add(this.activeQuest[i]);
				this.activeQuest.Remove(this.activeQuest[i]);
			}
			if (this.completedQuest.Count >= 20)
			{
				Game1.awardsManager.EarnAchievement(Achievement.CompleteQuestsTwenty, forceCheck: false);
			}
			else if (this.completedQuest.Count >= 10)
			{
				Game1.awardsManager.EarnAchievement(Achievement.CompleteQuestsTen, forceCheck: false);
			}
			else if (this.completedQuest.Count >= 5)
			{
				Game1.awardsManager.EarnAchievement(Achievement.CompleteQuestsFive, forceCheck: false);
			}
			if (_stringWidth > 0)
			{
				QuestManager.stringWidth = _stringWidth;
				Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(UpdateQuestThread)));
			}
		}

		private void UpdateQuestThread()
		{
			this.updatingQuests = true;
			int num = 0;
			while (num < 10)
			{
				try
				{
					this.activeQuest[this.activeQuest.Count - 1].Update();
					for (int i = 0; i < this.activeQuest.Count; i++)
					{
						this.activeQuest[i].GetDesc(QuestManager.stringWidth);
					}
					if (Game1.hud.inventoryState != 0)
					{
						for (int j = 0; j < this.completedQuest.Count; j++)
						{
							this.completedQuest[j].GetDesc(QuestManager.stringWidth);
						}
					}
					Character[] character = Game1.character;
					for (int k = 0; k < character.Length; k++)
					{
						if (character[k].Exists == CharExists.Exists && character[k].Ai != null)
						{
							character[k].QuestGiver = Game1.questManager.CheckQuestGivers(character[k]);
						}
					}
				}
				catch (Exception)
				{
					num++;
					continue;
				}
				break;
			}
			this.updatingQuests = false;
		}

		public void RewardGold(int goldAmount)
		{
			Game1.stats.AcquireGold(goldAmount);
		}

		private void SpawnCoin(Vector2 loc, int coinAmount)
		{
			Game1.pManager.AddCoin(loc, new Vector2(Rand.GetRandomInt(-300, 300), Rand.GetRandomInt(-900, -500)), coinAmount, 5);
		}

		public void Draw()
		{
			float size = 1f;
			Game1.text.Color = Color.White;
			Game1.text.DrawText(new Vector2(320f, 180f), "Active Quests:", size, 1000f, TextAlign.Left);
			for (int i = 0; i < this.activeQuest.Count; i++)
			{
				Game1.text.DrawText(new Vector2(320f, 200 + 20 * i), i + ": " + this.activeQuest[i].Name.ToString(), size, 1000f, TextAlign.Left);
			}
			Game1.text.DrawText(new Vector2(640f, 180f), "Completed Quests:", size, 1000f, TextAlign.Left);
			for (int j = 0; j < this.completedQuest.Count; j++)
			{
				Game1.text.DrawText(new Vector2(640f, 220 + 20 * j), j + ": " + this.completedQuest[j].Name.ToString(), size, 1000f, TextAlign.Left);
			}
		}
	}
}
