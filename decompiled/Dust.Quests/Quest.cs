using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using Dust.CharClasses;
using Dust.Strings;

namespace Dust.Quests
{
	public class Quest
	{
		public string Name = string.Empty;

		public List<string> stageString = new List<string>();

		public bool isActive = true;

		public bool requirementMet;

		public int stage = -1;

		public int prevStage;

		public bool[] sideStage = new bool[0];

		public int maxStages = -1;

		public int QuestID;

		public CharacterType questGiver;

		public void ResetQuest()
		{
			this.isActive = true;
			this.requirementMet = false;
			this.stage = -1;
			this.prevStage = 0;
			this.questGiver = CharacterType.Dust;
			this.ResetQuestGiver();
		}

		public virtual void ResetQuestGiver()
		{
		}

		public void SetName(Quest newQuest)
		{
			string[] array = newQuest.ToString().Split('_');
			ResourceSet resourceSet = Strings_Quests.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, createIfNotExists: true, tryParents: true);
			foreach (DictionaryEntry item in resourceSet)
			{
				string text = (string)item.Key;
				if (text.StartsWith(array[1]) && text.Contains("Title"))
				{
					this.Name = Strings_Quests.ResourceManager.GetString(text);
					break;
				}
			}
			this.QuestID = Convert.ToInt32(array[1]);
			this.ResetQuestGiver();
		}

		public void MaxOutStages()
		{
			this.maxStages = -1;
			bool flag = true;
			while (flag)
			{
				this.maxStages++;
				string name = $"{this.QuestID:D3}" + "_" + $"{this.maxStages:D3}";
				if (Strings_Quests.ResourceManager.GetString(name) == null)
				{
					break;
				}
			}
		}

		public virtual void Update()
		{
			if (this.isActive && this.stage > this.prevStage)
			{
				if (this.requirementMet)
				{
					Game1.hud.InitQuestMiniPrompt("[ICON_360] " + Strings_Hud.Mini_QuestReady + "\n[BACK] " + this.Name, 0, this.QuestID);
				}
				else
				{
					Game1.hud.InitQuestMiniPrompt("[ICON_360] " + Strings_Hud.Mini_QuestUpdated + "\n[BACK] " + this.Name, 0, this.QuestID);
				}
			}
			this.prevStage = this.stage;
		}

		public virtual void Complete()
		{
			Game1.hud.InitQuestMiniPrompt("[ICON_361] " + Strings_Hud.Mini_QuestCompleted + "\n[BACK] " + this.Name, 1, this.QuestID);
			this.isActive = false;
			for (int i = 0; i < this.sideStage.Length; i++)
			{
				this.sideStage[i] = true;
			}
			this.MaxOutStages();
			for (int j = 0; j < Game1.character.Length; j++)
			{
				if (Game1.character[j].Definition.charType == this.questGiver)
				{
					Game1.character[j].QuestGiver = false;
				}
			}
		}

		public virtual void GetDesc(int width)
		{
			this.stageString.Clear();
			int num = this.stage;
			if (this.maxStages > 1)
			{
				num = this.maxStages;
			}
			for (int i = 0; i < num + 1; i++)
			{
				try
				{
					string name = $"{this.QuestID:D3}" + "_" + $"{i:D3}";
					if (Strings_Quests.ResourceManager.GetString(name) != null)
					{
						this.stageString.Add(Game1.smallText.WordWrap(Strings_Quests.ResourceManager.GetString(name), 0.8f, width, TextAlign.Left));
						continue;
					}
				}
				catch (Exception)
				{
				}
				break;
			}
			if (this.maxStages > -1)
			{
				this.stageString.Add(Game1.smallText.WordWrap(Strings_Quests.Quest_Complete, 0.8f, width, TextAlign.Left));
			}
			this.stageString.Reverse();
		}

		public CharacterType GetCurrentQuestGiver()
		{
			if (this.stage == -1)
			{
				return this.GetInitialQuestGiver();
			}
			return this.questGiver;
		}

		public virtual CharacterType GetInitialQuestGiver()
		{
			return this.questGiver;
		}

		public virtual bool CheckSideStage(int _sideStage)
		{
			if (this.sideStage.Length < 1)
			{
				return false;
			}
			return this.sideStage[_sideStage];
		}
	}
}
