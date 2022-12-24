using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;
using Dust.Audio;
using Dust.CharClasses;
using Dust.Dialogue;
using Dust.Vibration;
using Microsoft.Xna.Framework;

namespace Dust.HUD
{
	public class DialogueScript
	{
		public ResourceManager manager;

		public List<TextBox> textBox = new List<TextBox>();

		public int currentTextBox = -1;

		private int curConversation;

		private CharacterType charType;

		private Dialogue dialogue;

		public string resourcePath;

		public int InitDialogueScript(int _curConversation, CharacterType _charType, Dialogue _dialogue)
		{
			this.charType = _charType;
			this.curConversation = _curConversation;
			this.dialogue = _dialogue;
			this.resourcePath = this.charType.ToString().ToLower();
			this.manager = Game1.GetResource(this.resourcePath);
			this.Read(this.resourcePath, _curConversation);
			if (Game1.events.eventType == EventType.None)
			{
				for (int i = 0; i < this.textBox.Count; i++)
				{
					if (this.textBox[i].global)
					{
						int curLine = this.textBox[i].ID;
						this.UpdateTextBox(ref curLine, runCommands: true);
					}
				}
			}
			for (int j = 0; j < this.textBox.Count; j++)
			{
				if (this.textBox[j].startConversation == this.curConversation)
				{
					this.currentTextBox = this.textBox[j].ID;
					break;
				}
			}
			for (int k = 0; k < this.textBox.Count; k++)
			{
				if (this.textBox[k].textBoxType == TextBoxType.GlobalStart)
				{
					this.currentTextBox = this.textBox[k].ID;
					break;
				}
			}
			for (int l = 0; l < this.textBox.Count; l++)
			{
				int num = 0;
				for (int m = 0; m < this.textBox[l].responseKey.Length; m++)
				{
					if (this.textBox[l].responseKey[m] != null)
					{
						num++;
					}
				}
				if (num > 0)
				{
					for (int n = 0; n < num; n++)
					{
						this.dialogue.responseMasterList.Add(new ResponseMaster(this.textBox[l].responseKey[n]));
					}
				}
			}
			return this.currentTextBox;
		}

		public void Read(string _path, int curConversation)
		{
			if (_path.StartsWith("dust"))
			{
				this.resourcePath = "events";
				_path = ((!Game1.testDialogue) ? ("events" + $"{curConversation / 50 * 50:D3}") : "eventsall");
			}
			if (_path == "shopwild" && Game1.GamerServices && Game1.IsTrial)
			{
				_path = "shoptrial";
			}
			BinaryReader binaryReader;
			try
			{
				binaryReader = ((!Game1.Xbox360 && !Game1.isPCBuild) ? new BinaryReader(File.Open("../../../../../dialogue/" + _path + ".dsc", FileMode.Open, FileAccess.Read)) : new BinaryReader(File.Open("data/dialogue/" + _path + ".dsc", FileMode.Open, FileAccess.Read)));
			}
			catch (Exception)
			{
				return;
			}
			this.textBox.Clear();
			int num = binaryReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				int iD = binaryReader.ReadInt32();
				TextBoxType textBoxType = (TextBoxType)binaryReader.ReadInt32();
				this.textBox.Add(new TextBox(iD, textBoxType));
				TextBox textBox = this.textBox[i];
				textBox.lineKey = binaryReader.ReadString();
				textBox.portraitID = binaryReader.ReadInt32();
				textBox.portraitFlip = binaryReader.ReadBoolean();
				textBox.startConversation = binaryReader.ReadInt32();
				textBox.flag_0 = binaryReader.ReadInt32();
				textBox.flag_1 = binaryReader.ReadInt32();
				textBox.global = binaryReader.ReadBoolean();
				textBox.flag_string = binaryReader.ReadString();
				int num2 = binaryReader.ReadInt32();
				textBox.responseKey = new string[num2];
				textBox.responseTarget = new int[num2];
				for (int j = 0; j < num2; j++)
				{
					string text = binaryReader.ReadString();
					if (text != "null")
					{
						textBox.responseKey[j] = text;
					}
					textBox.responseTarget[j] = binaryReader.ReadInt32();
				}
				int num3 = binaryReader.ReadInt32();
				textBox.visualizationData.Clear();
				for (int k = 0; k < num3; k++)
				{
					textBox.visualizationData.Add(binaryReader.ReadByte());
				}
			}
			binaryReader.Close();
		}

		public TextBox GetSelectedBox(int id)
		{
			for (int i = 0; i < this.textBox.Count; i++)
			{
				if (this.textBox[i].ID == id)
				{
					return this.textBox[i];
				}
			}
			return null;
		}

		public bool UpdateTextBox(ref int curLine, bool runCommands)
		{
			if (curLine == -1)
			{
				return false;
			}
			TextBox selectedBox = this.GetSelectedBox(curLine);
			while (selectedBox.textBoxType != 0)
			{
				int num = 0;
				switch (selectedBox.textBoxType)
				{
				case TextBoxType.IfEvent:
					num = ((Game1.events.currentEvent < selectedBox.flag_0) ? 1 : 0);
					break;
				case TextBoxType.IfSideEvent:
					num = (Game1.events.sideEventAvailable[selectedBox.flag_0] ? 1 : 0);
					break;
				case TextBoxType.SetEvent:
					Game1.events.currentEvent = Math.Max(Game1.events.currentEvent, selectedBox.flag_0);
					break;
				case TextBoxType.SetSideEvent:
					Game1.events.sideEventAvailable[selectedBox.flag_0] = selectedBox.flag_1 > 0;
					break;
				case TextBoxType.IfConversation:
					num = ((this.curConversation != selectedBox.flag_0) ? 1 : 0);
					break;
				case TextBoxType.IfConversationGreater:
					num = ((this.curConversation < selectedBox.flag_0) ? 1 : 0);
					break;
				case TextBoxType.InitCommand:
					if (runCommands)
					{
						this.InitCommand(selectedBox.flag_0);
					}
					break;
				case TextBoxType.InitEvent:
					if (runCommands)
					{
						this.dialogue.resetToEvent = selectedBox.flag_0;
					}
					break;
				case TextBoxType.InitSideEvent:
					if (runCommands)
					{
						this.dialogue.resetToSideEvent = selectedBox.flag_0;
					}
					break;
				case TextBoxType.IfQuestActive:
					num = ((!Game1.questManager.IfQuestActive(selectedBox.flag_0)) ? 1 : 0);
					break;
				case TextBoxType.IfQuestCanComplete:
					num = ((!Game1.questManager.IfQuestRequirementMet(selectedBox.flag_0)) ? 1 : 0);
					break;
				case TextBoxType.IfQuestCompleted:
					num = ((selectedBox.flag_1 != -1) ? ((!Game1.questManager.IfQuestStageMet(selectedBox.flag_0, selectedBox.flag_1)) ? 1 : 0) : ((!Game1.questManager.IfQuestCompleted(selectedBox.flag_0)) ? 1 : 0));
					break;
				case TextBoxType.IfQuestStage:
					num = ((!Game1.questManager.IfQuestStageEquals(selectedBox.flag_0, selectedBox.flag_1)) ? 1 : 0);
					break;
				case TextBoxType.IfNoteFound:
					num = ((!Game1.questManager.IfNoteFound(selectedBox.flag_0)) ? 1 : 0);
					break;
				case TextBoxType.InitQuest:
					if (runCommands)
					{
						Game1.questManager.AddQuest(selectedBox.flag_0);
					}
					break;
				case TextBoxType.SetQuestStage:
					if (runCommands)
					{
						Game1.questManager.SetQuestStage(selectedBox.flag_0, selectedBox.flag_1);
					}
					break;
				case TextBoxType.SetQuestSideStage:
					if (runCommands)
					{
						Game1.questManager.SetQuestSideStage(selectedBox.flag_0, selectedBox.flag_1);
					}
					break;
				case TextBoxType.CompleteQuest:
					if (runCommands)
					{
						Game1.questManager.CompleteQuest(selectedBox.flag_0);
					}
					break;
				case TextBoxType.AddNote:
					if (runCommands)
					{
						Game1.questManager.AddNote(selectedBox.flag_0, loading: false);
					}
					break;
				case TextBoxType.Skippable:
					if (runCommands && Game1.events.skippable == SkipType.Unskippable)
					{
						Game1.events.skippable = SkipType.DialogueOnly;
					}
					break;
				case TextBoxType.UnSkippable:
					if (runCommands)
					{
						Game1.events.skippable = SkipType.Unskippable;
					}
					break;
				case TextBoxType.InitShop:
					if (runCommands)
					{
						Game1.hud.InitShop(selectedBox.flag_0, 0, -1, blueprint: false);
					}
					break;
				case TextBoxType.SetConv:
					if (runCommands)
					{
						if (this.charType != 0)
						{
							Game1.stats.villagerDialogue[(int)this.charType] = selectedBox.flag_0;
						}
						this.curConversation = selectedBox.flag_0;
					}
					break;
				case TextBoxType.SetMinConv:
					if (runCommands)
					{
						if (this.charType != 0)
						{
							Game1.stats.villagerDialogue[(int)this.charType] = Math.Max(selectedBox.flag_0, Game1.stats.villagerDialogue[(int)this.charType]);
						}
						this.curConversation = Math.Max(selectedBox.flag_0, this.curConversation);
					}
					break;
				case TextBoxType.Shake:
					if (runCommands)
					{
						VibrationManager.SetScreenShake((float)selectedBox.flag_0 / 100f);
					}
					break;
				case TextBoxType.PlaySound:
					if (runCommands)
					{
						Sound.PlayCue(selectedBox.flag_string);
					}
					break;
				case TextBoxType.RevealMap:
					if (runCommands)
					{
						Game1.navManager.RevealCell(selectedBox.flag_string);
					}
					break;
				case TextBoxType.BlurOn:
					if (runCommands)
					{
						Game1.events.blurOn = selectedBox.flag_0 > -1;
					}
					break;
				case TextBoxType.SetCamera:
					if (runCommands)
					{
						Vector2 loc = Vector2.Zero;
						if (selectedBox.flag_0 != -1)
						{
							loc = new Vector2(selectedBox.flag_0, selectedBox.flag_1);
						}
						Game1.events.SetEventCamera(loc, snapToLocation: false);
					}
					break;
				case TextBoxType.IfRandom:
					num = Math.Min((int)selectedBox.random / (100 / selectedBox.flag_0), selectedBox.responseTarget.Length - 1);
					break;
				case TextBoxType.FaceCharacter:
					if (runCommands)
					{
						string[] array2 = selectedBox.flag_string.Split(' ');
						if (array2.Length > 1)
						{
							Game1.events.FaceCharacter((CharacterType)Game1.GetCharacterFromString(array2[0]), (CharacterType)Game1.GetCharacterFromString(array2[1]), faceTowards: true);
						}
					}
					break;
				case TextBoxType.FaceLocation:
					if (runCommands)
					{
						Game1.events.FaceLoc((CharacterType)Game1.GetCharacterFromString(selectedBox.flag_string), new Vector2(selectedBox.flag_0, 0f), faceTowards: true);
					}
					break;
				case TextBoxType.SetAnim:
					if (runCommands)
					{
						string[] array = selectedBox.flag_string.Split(' ');
						if (array.Length > 1)
						{
							Game1.events.GetCharacter((CharacterType)Game1.GetCharacterFromString(array[0])).SetAnim(array[1], selectedBox.flag_0, 2);
						}
					}
					break;
				case TextBoxType.AddEquip:
					if (runCommands)
					{
							if ((EquipType)selectedBox.flag_0 == EquipType.BlueKey)
								Game1.stats.GetChestFromFile("Blue Resonance Gem", Game1.pManager);
							else if ((EquipType)selectedBox.flag_0 == EquipType.RedKey)
								Game1.stats.GetChestFromFile("Red Resonance Gem", Game1.pManager);
							else if ((EquipType)selectedBox.flag_0 == EquipType.GreenKey)
								Game1.stats.GetChestFromFile("Green Resonance Gem", Game1.pManager);
							else if ((EquipType)selectedBox.flag_0 == EquipType.PurpleKey)
								Game1.stats.GetChestFromFile("Purple Resonance Gem", Game1.pManager);
							else if ((EquipType)selectedBox.flag_0 == EquipType.RainbowKey)
								Game1.stats.GetChestFromFile("Rainbow Resonance Gem", Game1.pManager);
							else if ((EquipType)selectedBox.flag_0 == EquipType.WhiteKey)
								Game1.stats.GetChestFromFile("White Resonance Gem", Game1.pManager);
							else if ((EquipType)selectedBox.flag_0 == EquipType.YellowKey)
								Game1.stats.GetChestFromFile("Yellow Resonance Gem", Game1.pManager);
							else if ((EquipType)selectedBox.flag_0 == EquipType.ReedBox && Game1.stats.Equipment[340] > 0)
								Game1.stats.AcquireEquip((EquipType)selectedBox.flag_0, selectedBox.flag_1, _bluePrint: false);
							else if ((EquipType)selectedBox.flag_0 == EquipType.ReedBoxEvil)
								Game1.stats.AcquireEquip((EquipType)selectedBox.flag_0, selectedBox.flag_1, _bluePrint: false);
							else if (selectedBox.flag_1 < 0)
								Game1.stats.AcquireEquip((EquipType)selectedBox.flag_0, selectedBox.flag_1, _bluePrint: false);
							else
								Game1.stats.GetChestFromFile("Equip " + Game1.map.path + " " + selectedBox.flag_0.ToString() + " " + selectedBox.flag_1.ToString(), Game1.pManager);
					}
					break;
				case TextBoxType.AddBluePrint:
						if (runCommands)
						{
							Game1.stats.GetChestFromFile("Blueprint " + Game1.map.path + " " + selectedBox.flag_0.ToString() + " 1", Game1.pManager);
						}
						break;
				case TextBoxType.AddMaterial:
					if (runCommands)
					{
						Game1.stats.AcquireMateral((MaterialType)selectedBox.flag_0, selectedBox.flag_1);
					}
					break;
				case TextBoxType.AddGold:
					if (runCommands)
					{
						Game1.stats.AcquireGold(selectedBox.flag_0);
					}
					break;
				case TextBoxType.AddXP:
					if (runCommands)
					{
						Game1.stats.AcquireXP(selectedBox.flag_0);
					}
					break;
				}
				curLine = selectedBox.responseTarget[num];
				if (curLine == -1)
				{
					return false;
				}
				selectedBox = this.GetSelectedBox(curLine);
			}
			return true;
		}

		private void InitCommand(int command)
		{
			if (command != 0)
			{
				return;
			}
			for (int i = 0; i < Game1.character.Length; i++)
			{
				if (Game1.character[i].Exists == CharExists.Exists && Game1.character[i].Definition.charType == CharacterType.Soldier)
				{
					Game1.character[i].SetAnim("atease", 0, 4);
				}
			}
		}

		private void FormatToScript(ResourceManager manager)
		{
			string text = string.Empty;
			ResourceSet resourceSet = manager.GetResourceSet(CultureInfo.InvariantCulture, createIfNotExists: true, tryParents: true);
			if (resourceSet == null)
			{
				return;
			}
			List<string> list = new List<string>();
			foreach (DictionaryEntry item in resourceSet)
			{
				list.Add((string)item.Key);
			}
			list.Sort();
			for (int i = 0; i < list.Count; i++)
			{
				string[] array = manager.GetString(list[i]).Split(' ');
				if (array.Length > 1)
				{
					text = text + manager.GetString(list[i]).Remove(0, array[0].Length) + "\n";
				}
				string text2 = text;
				text = text2 + array[0] + " : (" + list[i] + ".wav) : " + manager.GetString(list[i]) + "\n";
			}
		}

		private void GetLine(string name, int dialogueID, int lineID, ref string text, ref int leftPortrait, ref int rightPortrait)
		{
			string text2 = null;
			string text3 = $"{dialogueID:D3}" + "_" + $"{lineID:D2}";
			ResourceSet resourceSet = this.manager.GetResourceSet(CultureInfo.InvariantCulture, createIfNotExists: true, tryParents: true);
			if (resourceSet != null)
			{
				foreach (DictionaryEntry item in resourceSet)
				{
					string text4 = (string)item.Key;
					if (!text4.StartsWith(text3))
					{
						continue;
					}
					text2 = this.manager.GetString(text4);
					string[] array = text4.Split('_');
					if (array.Length > 2)
					{
						if (array[2].StartsWith("l"))
						{
							leftPortrait = Convert.ToInt16(array[2].Split('l')[1]);
						}
						else if (array[2].StartsWith("r"))
						{
							rightPortrait = Convert.ToInt16(array[2].Split('r')[1]);
						}
					}
					break;
				}
			}
			if (text2 != null)
			{
				text = text2;
				this.dialogue.voiceID[0] = name + "_" + text3;
			}
		}
	}
}
