using System;
using System.Collections.Generic;
using System.Globalization;
using Dust.Audio;
using Dust.CharClasses;
using Dust.Dialogue;
using Dust.PCClasses;
using Dust.Strings;
using Lotus.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Dust.HUD
{
	public class Dialogue
	{
		private static object syncObject = new object();

		public int dialogueState;

		public DialogueScript dialogueScript;

		private SpriteBatch sprite;

		private Texture2D nullTex;

		private Texture2D[] hudTex;

		private static int animFrame = 0;

		private static float animFrameTime = 0f;

		public string dialogueLine;

		public Dictionary<Vector3, string> dialogueLineButtonList = new Dictionary<Vector3, string>();

		private static string helpLine;

		private static Dictionary<Vector3, string> dialogueHelpButtonList = new Dictionary<Vector3, string>();

		private static int queueLoadCurrent;

		private static int queueVoiceCurrent;

		private static bool queueLoadInvert;

		public bool resetDialogue;

		public int resetToEvent = -1;

		public int resetToSideEvent = -1;

		private static int resetQueueEvent = -1;

		private static int resetQueueSideEvent = -1;

		public List<ResponseMaster> responseMasterList = new List<ResponseMaster>();

		public List<ResponseLocal> responseLocalList = new List<ResponseLocal>();

		private static int responseSelection;

		private static bool threadBusy;

		public float dialogueTextSize;

		private bool KeySelect;

		private bool KeyCancel;

		private bool firstLine;

		private bool gameScrolling;

		private int curLine;

		private int prevLine;

		private static float screenDimAlpha = 0f;

		private static float textAlpha = 0f;

		private static bool canHelp;

		private static float helpAlpha;

		private static List<byte> visualizationState = new List<byte>();

		private static float visualizationTimer;

		private static bool mouthIsOpen;

		private static int visualizationStep;

		public int[] portraitID = new int[2];

		public bool[] portraitFlip = new bool[2];

		private static Texture2D[] portraitTex = new Texture2D[2];

		private static LoadState[] portraitLoaded = new LoadState[2];

		private float[] portraitAlpha = new float[2];

		private static Song[] voice = new Song[2];

		public string[] voiceID = new string[2];

		private static bool[] voiceLoading = new bool[2];

		public int currentQueuePortrait;

		public int currentLivePortrait;

		private static byte speaking;

		private static int leftEdge;

		private static int newLeftEdge;

		public float topEdge;

		private static float windowHeight;

		private static int targetHeight;

		private static float continueTimer = 0f;

		private static float continueButtonAlpha = 0f;

		public Dialogue(SpriteBatch _sprite, Texture2D _nullTex, Texture2D[] _hudTex, int conversation, CharacterType charType)
		{
			this.sprite = _sprite;
			this.hudTex = _hudTex;
			this.nullTex = _nullTex;
			this.topEdge = Game1.screenHeight;
			this.dialogueLine = string.Empty;
			this.dialogueTextSize = 0.8f;
			this.topEdge = Game1.screenHeight - 1;
			Dialogue.animFrameTime = (Dialogue.animFrame = 0);
			Dialogue.textAlpha = -0.5f;
			Dialogue.continueButtonAlpha = 0f;
			Dialogue.canHelp = Game1.events.currentEvent < 22;
			Dialogue.helpAlpha = -1f;
			Dialogue.threadBusy = false;
			this.resetToEvent = (this.resetToSideEvent = (Dialogue.resetQueueEvent = (Dialogue.resetQueueSideEvent = -1)));
			for (int i = 0; i < this.portraitID.Length; i++)
			{
				this.portraitID[i] = -1;
				this.portraitFlip[i] = false;
				Dialogue.portraitLoaded[i] = LoadState.NotLoaded;
				this.portraitAlpha[i] = 0f;
				Dialogue.voice[i] = null;
				this.voiceID[i] = string.Empty;
				Dialogue.voiceLoading[i] = false;
			}
			this.currentQueuePortrait = 0;
			this.currentLivePortrait = 0;
			Dialogue.leftEdge = Game1.screenWidth / 2 - 350;
			this.firstLine = (this.gameScrolling = true);
			if (Game1.hud.helpState > 1)
			{
				Game1.hud.ClearHelp();
			}
			this.dialogueScript = new DialogueScript();
			this.curLine = this.dialogueScript.InitDialogueScript(conversation, charType, this);
			this.curLine = this.PrepDialogue(this.curLine, queuePortrait: true);
			Dialogue.visualizationTimer = 100f;
			Dialogue.visualizationStep = 0;
			Dialogue.mouthIsOpen = false;
			if (this.portraitID[this.currentLivePortrait] < 0)
			{
				Dialogue.newLeftEdge = Game1.screenWidth / 2 - 350;
			}
			else if (this.portraitFlip[this.currentLivePortrait])
			{
				Dialogue.newLeftEdge = (int)((float)(Game1.screenWidth / 2) - (float)Game1.screenWidth * 0.35f / Game1.hiDefScaleOffset);
			}
			else
			{
				Dialogue.newLeftEdge = (int)((float)(Game1.screenWidth / 2) + (float)Game1.screenWidth * 0.35f / Game1.hiDefScaleOffset) - 700;
			}
			Game1.stats.fidgetAwayTime = Math.Min(Game1.stats.fidgetAwayTime, 1f);
			Game1.events.blurOn = true;
		}

		public void InitExit()
		{
			this.resetDialogue = false;
			Dialogue.textAlpha = (Dialogue.continueButtonAlpha = (Dialogue.continueTimer = 0f));
			this.dialogueState = 2;
			this.dialogueScript = null;
			this.responseMasterList.Clear();
			this.responseLocalList.Clear();
			Dialogue.visualizationState.Clear();
			VoiceManager.UnloadVoice();
			Dialogue.screenDimAlpha = Math.Max(Dialogue.screenDimAlpha, 0.1f);
			Dialogue.threadBusy = false;
			if (Game1.events.eventType == EventType.None)
			{
				Game1.events.skippable = SkipType.DialogueOnly;
			}
			Dialogue.resetQueueEvent = this.resetToEvent;
			Dialogue.resetQueueSideEvent = this.resetToSideEvent;
			this.resetToEvent = (this.resetToSideEvent = -1);
			if (Game1.hud.converseWithID > -1)
			{
				Game1.events.SetEventCamera(Vector2.Zero, snapToLocation: false);
			}
			Game1.hud.converseWithID = -1;
			Game1.events.PrepareCharacters(0);
		}

		private int PrepDialogue(int _currentLine, bool queuePortrait)
		{
			this.KeySelect = false;
			lock (Dialogue.syncObject)
			{
				if (!queuePortrait || this.firstLine)
				{
					if (this.dialogueScript.UpdateTextBox(ref _currentLine, runCommands: true))
					{
						this.firstLine = false;
						TextBox selectedBox = this.dialogueScript.GetSelectedBox(_currentLine);
						string text = this.dialogueScript.manager.GetString(selectedBox.lineKey);
						if (text == null)
						{
							text = "Error: No String Defined!";
						}
						this.dialogueLine = string.Empty;
						if (text != string.Empty)
						{
							this.dialogueLine = Game1.bigText.WordWrap(text, this.dialogueTextSize, 640f, this.dialogueLineButtonList, TextAlign.Center);
						}
						this.responseLocalList.Clear();
						for (int i = 0; i < selectedBox.responseTarget.Length; i++)
						{
							if (selectedBox.responseTarget[i] > -1)
							{
								this.responseLocalList.Add(new ResponseLocal(selectedBox.responseKey[i], selectedBox.responseTarget[i], this.dialogueScript, this.dialogueTextSize, 600));
							}
						}
						Dialogue.responseSelection = 0;
						for (int j = 0; j < this.responseLocalList.Count; j++)
						{
							for (int k = 0; k < this.responseMasterList.Count; k++)
							{
								if (this.responseMasterList[k].responseKey == this.responseLocalList[Dialogue.responseSelection].responseKey && this.responseMasterList[k].responseState == 1)
								{
									Dialogue.responseSelection = Math.Min(Dialogue.responseSelection + 1, this.responseLocalList.Count - 1);
								}
							}
						}
						Dialogue.visualizationState.Clear();
						for (int l = 0; l < selectedBox.visualizationData.Count; l++)
						{
							Dialogue.visualizationState.Add(selectedBox.visualizationData[l]);
						}
						Dialogue.visualizationTimer = 100f;
						Dialogue.visualizationStep = 0;
						Dialogue.mouthIsOpen = false;
						if (Dialogue.canHelp)
						{
							Dialogue.helpLine = Game1.smallText.WordWrap(Strings_Help.dialogueprogress + ((this.responseLocalList.Count > 1) ? string.Empty : ("     " + Strings_Help.dialogueskip)), 0.8f, Game1.screenWidth, Dialogue.dialogueHelpButtonList, TextAlign.Left);
						}
					}
					else if (_currentLine == -1)
					{
						this.InitExit();
						return -1;
					}
				}
				if (queuePortrait)
				{
					this.dialogueScript.UpdateTextBox(ref _currentLine, runCommands: false);
					if (_currentLine == -1)
					{
						return -1;
					}
					int num = -1;
					int num2 = -1;
					TextBox selectedBox2 = this.dialogueScript.GetSelectedBox(_currentLine);
					int num3 = selectedBox2.portraitID;
					bool flag = selectedBox2.portraitFlip;
					this.voiceID[this.currentQueuePortrait] = selectedBox2.lineKey;
					if (!flag)
					{
						num = num3;
					}
					else
					{
						num2 = num3;
					}
					if (Game1.character[0].Face == CharDir.Left)
					{
						int num4 = num;
						num = num2;
						num2 = num4;
					}
					if (num2 > -1)
					{
						this.portraitID[this.currentQueuePortrait] = num2;
						this.portraitFlip[this.currentQueuePortrait] = true;
					}
					if (num > -1)
					{
						this.portraitID[this.currentQueuePortrait] = num;
						this.portraitFlip[this.currentQueuePortrait] = false;
					}
					this.currentQueuePortrait++;
					if (this.currentQueuePortrait > 1)
					{
						this.currentQueuePortrait = 0;
						return _currentLine;
					}
					return _currentLine;
				}
				return _currentLine;
			}
		}

		private void ProgressDialogue()
		{
			Dialogue.continueButtonAlpha = 0f;
			this.prevLine = this.curLine;
			if ((this.curLine = this.CheckTargets()) == -1)
			{
				this.resetDialogue = true;
				return;
			}
			Dialogue.textAlpha = (Dialogue.continueTimer = 0f);
			this.currentLivePortrait++;
			if (this.currentLivePortrait > 1)
			{
				this.currentLivePortrait = 0;
			}
			if (this.responseLocalList.Count > 0)
			{
				for (int i = 0; i < this.responseMasterList.Count; i++)
				{
					if (this.responseMasterList[i].responseKey == this.responseLocalList[Dialogue.responseSelection].responseKey && this.responseMasterList[i].responseState == 2)
					{
						this.responseMasterList[i].responseState = 1;
					}
				}
			}
			if (Dialogue.responseSelection > 0)
			{
				this.currentQueuePortrait++;
				if (this.currentQueuePortrait > 1)
				{
					this.currentQueuePortrait = 0;
				}
			}
			Dialogue.threadBusy = true;
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(BeginThreadDialogue)));
		}

		private void BeginThreadDialogue()
		{
			if (Dialogue.responseSelection > 0)
			{
				int num = this.portraitID[this.currentQueuePortrait];
				this.PrepDialogue(this.dialogueScript.GetSelectedBox(this.prevLine).responseTarget[Dialogue.responseSelection], queuePortrait: true);
				if (num != this.portraitID[this.currentLivePortrait])
				{
					Dialogue.portraitLoaded[this.currentQueuePortrait] = LoadState.NotLoaded;
				}
				this.InitLoadTexture(this.currentQueuePortrait, invert: true);
				this.LoadTexture(Dialogue.queueLoadCurrent, Dialogue.queueLoadInvert);
				this.SetVoice(this.CurrentID(Dialogue.queueVoiceCurrent, Dialogue.queueLoadInvert));
			}
			this.curLine = this.PrepDialogue(this.curLine, queuePortrait: false);
			if (this.portraitID[this.currentLivePortrait] < 0)
			{
				Dialogue.newLeftEdge = Game1.screenWidth / 2 - 350;
			}
			else if (this.portraitFlip[this.currentLivePortrait])
			{
				Dialogue.newLeftEdge = (int)((float)(Game1.screenWidth / 2) - (float)Game1.screenWidth * 0.35f / Game1.hiDefScaleOffset);
			}
			else
			{
				Dialogue.newLeftEdge = (int)((float)(Game1.screenWidth / 2) + (float)Game1.screenWidth * 0.35f / Game1.hiDefScaleOffset) - 700;
			}
			this.portraitID[(this.currentLivePortrait == 0) ? 1 : 0] = -1;
			Dialogue.threadBusy = false;
		}

		private bool CheckContinue()
		{
			if (Dialogue.queueLoadCurrent == -1 && Dialogue.queueVoiceCurrent == -1 && (this.portraitID[this.currentQueuePortrait] == -1 || (Dialogue.portraitLoaded[this.currentQueuePortrait] == LoadState.Loaded && this.portraitAlpha[this.currentLivePortrait] == 1f)))
			{
				Dialogue.continueTimer = Math.Min(Dialogue.continueTimer + Game1.HudTime, Math.Min(Dialogue.textAlpha, 1f));
			}
			if (Dialogue.continueTimer > 0.5f)
			{
				return true;
			}
			if (!Game1.VoiceNotReady)
			{
				Dialogue.continueButtonAlpha = 5.024f;
			}
			return false;
		}

		private int CheckTargets()
		{
			for (int i = 0; i < this.responseLocalList.Count; i++)
			{
				if (this.responseLocalList[i].responseTarget > -1)
				{
					return this.responseLocalList[Dialogue.responseSelection].responseTarget;
				}
			}
			return -1;
		}

		private void InitLoadTexture(int current, bool invert)
		{
			Dialogue.queueLoadCurrent = (Dialogue.queueVoiceCurrent = current);
			Dialogue.queueLoadInvert = invert;
		}

		private void LoadTexture(int current, bool invert)
		{
			if (!this.PortraitListed(this.portraitID[this.CurrentID(current, invert)]))
			{
				this.DebugPortraits(ref this.portraitID[this.CurrentID(current, invert)]);
			}
			for (int i = 0; i < this.portraitID.Length; i++)
			{
				if (Dialogue.portraitLoaded[i] == LoadState.Loading)
				{
					return;
				}
				if (this.portraitID[i] == -1)
				{
					Dialogue.portraitLoaded[i] = LoadState.NotLoaded;
				}
			}
			Dialogue.queueLoadCurrent = -1;
			Dialogue.continueTimer = 0f;
			this.SetPortrait(this.CurrentID(current, invert));
		}

		private int CurrentID(int current, bool invert)
		{
			if (!invert)
			{
				if (current == 0)
				{
					return 0;
				}
				return 1;
			}
			if (current == 0)
			{
				return 1;
			}
			return 0;
		}

		private void SetPortrait(int current)
		{
			this.portraitAlpha[current] = 0f;
			if (this.portraitID[current] >= 0 && Dialogue.portraitLoaded[current] != LoadState.Loading)
			{
				Dialogue.portraitLoaded[current] = LoadState.Loading;
				if (current == 0)
				{
					Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadPortrait0)));
				}
				else
				{
					Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadPortrait1)));
				}
			}
		}

		private void LoadPortrait0()
		{
			if (this.portraitID[0] <= -1)
			{
				return;
			}
			try
			{
				string text = "portrait_";
				if (Game1.settings.HiQualityPortraits)
				{
					text = "hq_" + text;
				}
				Game1.GetPortraitContent0().Unload();
				Dialogue.portraitTex[0] = Game1.GetPortraitContent0().Load<Texture2D>("gfx/portraits/" + text + $"{this.portraitID[0]:D2}");
				Dialogue.portraitLoaded[0] = LoadState.Loaded;
			}
			catch
			{
				try
				{
					Game1.GetPortraitContent0().Unload();
					Dialogue.portraitTex[0] = Game1.GetPortraitContent0().Load<Texture2D>("gfx/portraits/portrait_" + $"{this.portraitID[0]:D2}");
					Dialogue.portraitLoaded[0] = LoadState.Loaded;
				}
				catch (Exception)
				{
					this.portraitID[0] = -1;
					Dialogue.portraitLoaded[0] = LoadState.NotLoaded;
				}
			}
		}

		private void LoadPortrait1()
		{
			if (this.portraitID[1] <= -1)
			{
				return;
			}
			try
			{
				string text = "portrait_";
				if (Game1.settings.HiQualityPortraits)
				{
					text = "hq_" + text;
				}
				Game1.GetPortraitContent1().Unload();
				Dialogue.portraitTex[1] = Game1.GetPortraitContent1().Load<Texture2D>("gfx/portraits/" + text + $"{this.portraitID[1]:D2}");
				Dialogue.portraitLoaded[1] = LoadState.Loaded;
			}
			catch
			{
				try
				{
					Game1.GetPortraitContent1().Unload();
					Dialogue.portraitTex[1] = Game1.GetPortraitContent1().Load<Texture2D>("gfx/portraits/portrait_" + $"{this.portraitID[1]:D2}");
					Dialogue.portraitLoaded[1] = LoadState.Loaded;
				}
				catch (Exception)
				{
					this.portraitID[1] = -1;
					Dialogue.portraitLoaded[1] = LoadState.NotLoaded;
				}
			}
		}

		private void UnloadTextures()
		{
			bool flag = false;
			while (!flag)
			{
				flag = false;
				for (int i = 0; i < this.portraitID.Length; i++)
				{
					if (Dialogue.portraitLoaded[i] != LoadState.Loading)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				for (int j = 0; j < this.portraitID.Length; j++)
				{
					this.portraitID[j] = -1;
					this.portraitFlip[j] = false;
					Dialogue.portraitLoaded[j] = LoadState.NotLoaded;
					this.portraitAlpha[j] = 0f;
					Dialogue.voice[j] = null;
				}
			}
			Game1.GetPortraitContent0().Unload();
			Game1.GetPortraitContent1().Unload();
		}

		private void SetVoice(int current)
		{
			Dialogue.queueVoiceCurrent = -1;
			if (this.voiceID[current] != string.Empty)
			{
				Dialogue.speaking = 2;
				Dialogue.voiceLoading[current] = true;
			}
		}

		private void LoadVoice0()
		{
			try
			{
				Dialogue.voice[0] = Game1.GetPortraitContent0().Load<Song>("voice/" + this.dialogueScript.resourcePath + "/" + this.voiceID[0]);
			}
			catch (Exception)
			{
				Dialogue.voice[0] = null;
			}
			this.voiceID[0] = string.Empty;
		}

		private void LoadVoice1()
		{
			try
			{
				Dialogue.voice[1] = Game1.GetPortraitContent1().Load<Song>("voice/" + this.dialogueScript.resourcePath + "/" + this.voiceID[1]);
			}
			catch (Exception)
			{
				Dialogue.voice[1] = null;
			}
			this.voiceID[1] = string.Empty;
		}

		public void Update()
		{
			if (Game1.VoiceNotReady)
			{
				Game1.settings.AutoAdvance = false;
			}
			if (this.resetDialogue || this.resetToEvent > -1 || this.resetToSideEvent > -1)
			{
				this.InitExit();
				return;
			}
			if (this.KeyCancel)
			{
				Game1.hud.KeyCancel = true;
			}
			this.KeyCancel = false;
			if (Dialogue.animFrameTime > 1f)
			{
				while (Dialogue.animFrameTime > 1f)
				{
					Dialogue.animFrameTime -= 1f;
				}
				Dialogue.animFrame++;
				if (Dialogue.animFrame > 31)
				{
					Dialogue.animFrame = 0;
				}
			}
			float num = Dialogue.animFrameTime;
			Dialogue.animFrameTime += Game1.FrameTime * 20f;
			if (num < 1f && Dialogue.animFrameTime >= 1f && Dialogue.queueLoadCurrent > -1)
			{
				this.LoadTexture(Dialogue.queueLoadCurrent, Dialogue.queueLoadInvert);
			}
			if (num < 0.9f && Dialogue.animFrameTime >= 0.9f && Dialogue.queueVoiceCurrent > -1 && Dialogue.queueLoadCurrent == -1)
			{
				this.SetVoice(this.CurrentID(Dialogue.queueVoiceCurrent, Dialogue.queueLoadInvert));
			}
			if (this.dialogueState < 2)
			{
				if (this.dialogueState > 0)
				{
					Game1.BlurScene(1f);
				}
				Dialogue.speaking = VoiceManager.VoiceIsPlaying();
				if (this.topEdge > (float)(Game1.screenHeight - 250))
				{
					if (this.portraitID[this.currentQueuePortrait] < 0 || Dialogue.portraitLoaded[this.currentQueuePortrait] == LoadState.Loaded)
					{
						this.topEdge = Math.Max(this.topEdge - Game1.HudTime * 1000f, Game1.screenHeight - 250);
					}
					Dialogue.textAlpha = 0f;
				}
				if (Game1.events.eventType == EventType.None)
				{
					float num2 = (Game1.standardDef ? 0.8f : 1f);
					Game1.worldScale += (num2 * Game1.hiDefScaleOffset - Game1.worldScale) * Game1.HudTime;
				}
				if (Game1.events.blurOn)
				{
					Dialogue.screenDimAlpha = Math.Min(Dialogue.screenDimAlpha + Game1.HudTime * 2f, 0.5f);
				}
				else
				{
					Dialogue.screenDimAlpha = Math.Max(Dialogue.screenDimAlpha - Game1.HudTime * 2f, 0f);
				}
				Game1.hud.canInput = false;
				if (Game1.VoiceNotReady || Dialogue.speaking == 0)
				{
					Dialogue.continueButtonAlpha += Game1.HudTime * 4f;
					if (Dialogue.continueButtonAlpha > 6.28f)
					{
						Dialogue.continueButtonAlpha -= 6.28f;
					}
				}
				else
				{
					Dialogue.continueButtonAlpha = 5.024f;
				}
				if (this.responseLocalList.Count > 1 && !Dialogue.threadBusy)
				{
					this.UpdateResponse();
				}
				else if (this.CheckContinue() && (this.KeySelect || (Dialogue.speaking == 0 && Game1.menu.prompt == promptDialogue.None && Dialogue.animFrameTime > 1f && Sound.masterSFXVolume > 0f && Game1.settings.AutoAdvance)))
				{
					this.ProgressDialogue();
					if (this.KeySelect)
					{
						if (Game1.settings.Bloom && Game1.settings.DepthOfField)
						{
							Sound.PlayCue("dialogue_advance");
						}
						VoiceManager.StopVoiceCue();
					}
				}
				if (this.topEdge == (float)(Game1.screenHeight - 250))
				{
					float num3 = Dialogue.textAlpha;
					Dialogue.textAlpha = Math.Min(Dialogue.textAlpha + Game1.HudTime, 2f);
					if (Dialogue.textAlpha >= 0.4f && num3 < 0.4f && this.CheckTargets() > -1 && this.PrepDialogue(this.dialogueScript.GetSelectedBox(this.curLine).responseTarget[0], queuePortrait: true) != -1)
					{
						this.InitLoadTexture(this.currentQueuePortrait, invert: true);
					}
					if (Dialogue.textAlpha >= 0.5f && this.dialogueScript != null && this.voiceID[this.currentLivePortrait] != string.Empty)
					{
						VoiceManager.PlayVoice(this.dialogueScript.resourcePath + "_" + this.voiceID[this.currentLivePortrait], this.dialogueScript.resourcePath, loadingOnly: false);
						this.voiceID[this.currentLivePortrait] = string.Empty;
						Dialogue.visualizationTimer = 0.1f;
						Dialogue.visualizationStep = 0;
						Dialogue.mouthIsOpen = false;
					}
					if (Game1.screenWidth >= 1200)
					{
						Dialogue.leftEdge += (int)((float)(Dialogue.newLeftEdge - Dialogue.leftEdge) * Game1.HudTime * 10f);
					}
				}
				else if (this.topEdge > (float)(Game1.screenHeight - 20))
				{
					if (Game1.screenWidth < 1200)
					{
						Dialogue.leftEdge = Game1.screenWidth / 2 - 350;
					}
					else
					{
						Dialogue.leftEdge = Dialogue.newLeftEdge;
					}
				}
				this.UpdatePortraits();
			}
			else
			{
				if (this.topEdge < (float)Game1.screenHeight)
				{
					this.topEdge += Game1.HudTime * 1000f;
				}
				if (this.topEdge >= (float)Game1.screenHeight)
				{
					this.topEdge = Game1.screenHeight;
					this.UnloadTextures();
					Game1.hud.DialogueNull(Dialogue.resetQueueEvent, Dialogue.resetQueueSideEvent);
					Dialogue.resetQueueEvent = (Dialogue.resetQueueSideEvent = -1);
				}
				if (Dialogue.screenDimAlpha > 0f)
				{
					Dialogue.screenDimAlpha -= Game1.HudTime * 2f;
					if (Dialogue.screenDimAlpha < 0f)
					{
						Dialogue.screenDimAlpha = 0f;
					}
				}
				for (int i = 0; i < this.portraitID.Length; i++)
				{
					this.portraitAlpha[i] -= Game1.HudTime * 4f;
				}
			}
			if (this.dialogueLine != null)
			{
				Dialogue.targetHeight = (int)Math.Max(Game1.bigFont.MeasureString(this.dialogueLine).Y * this.dialogueTextSize + 40f, 76f);
				if (this.responseLocalList.Count > 1 && !Dialogue.threadBusy)
				{
					for (int j = 0; j < this.responseLocalList.Count; j++)
					{
						Dialogue.targetHeight += this.responseLocalList[j].responseHeight + 10;
					}
					Dialogue.targetHeight += 10;
				}
				Dialogue.windowHeight += ((float)Dialogue.targetHeight - Dialogue.windowHeight) * Game1.HudTime * 10f;
			}
			this.KeySelect = Game1.hud.KeySelect;
		}

		private void UpdateResponse()
		{
			if (this.CheckContinue() && this.KeySelect)
			{
				this.ProgressDialogue();
				Sound.PlayCue("dialogue_advance");
			}
			if (!(Dialogue.textAlpha < 0.5f))
			{
				int num = Dialogue.responseSelection;
				if (Game1.hud.KeyUp)
				{
					Game1.hud.KeyUp = false;
					Dialogue.responseSelection = Math.Max(Dialogue.responseSelection - 1, 0);
				}
				if (Game1.hud.KeyDown)
				{
					Game1.hud.KeyDown = false;
					Dialogue.responseSelection = Math.Min(Dialogue.responseSelection + 1, this.responseLocalList.Count - 1);
				}
				if (num != Dialogue.responseSelection)
				{
					Sound.PlayCue("menu_click");
				}
			}
		}

		private void UpdatePortraits()
		{
			int num = ((this.currentLivePortrait == 0) ? 1 : 0);
			if (this.portraitAlpha[num] > 0f)
			{
				this.portraitAlpha[num] -= Game1.HudTime * 6f;
				if (this.portraitAlpha[num] <= 0f)
				{
					this.portraitAlpha[num] = 0f;
					if (this.portraitID[num] == -1)
					{
						Dialogue.portraitLoaded[num] = LoadState.NotLoaded;
					}
				}
			}
			if (this.portraitAlpha[this.currentLivePortrait] < 1f && this.topEdge == (float)(Game1.screenHeight - 250))
			{
				this.portraitAlpha[this.currentLivePortrait] = Math.Min(this.portraitAlpha[this.currentLivePortrait] + Game1.HudTime * 2f, 1f);
			}
			if (this.topEdge > (float)(Game1.screenHeight - 250) && this.portraitID[0] > -1 && Dialogue.portraitLoaded[0] != LoadState.Loaded)
			{
				this.topEdge = Game1.screenHeight;
				Dialogue.screenDimAlpha = 0f;
				Dialogue.textAlpha = 0f;
				Dialogue.continueTimer = 0f;
				this.portraitAlpha[0] = 0f;
			}
			if (this.gameScrolling)
			{
				Game1.BlurScene(0f);
				if ((Game1.camera.tempScroll - Game1.camera.prevTempScroll).Length() * Game1.worldScale / Game1.hiDefScaleOffset < 0.8f)
				{
					if ((Dialogue.portraitLoaded[0] == LoadState.NotLoaded && this.portraitID[0] > -1) || this.portraitID[0] == -1)
					{
						this.InitLoadTexture(0, invert: false);
					}
					if (this.dialogueScript != null && this.voiceID[this.currentLivePortrait] != string.Empty)
					{
						VoiceManager.PlayVoice(this.dialogueScript.resourcePath + "_" + this.voiceID[this.currentLivePortrait], this.dialogueScript.resourcePath, loadingOnly: true);
						Dialogue.visualizationTimer = 0.1f;
						Dialogue.visualizationStep = 0;
						Dialogue.mouthIsOpen = false;
					}
					this.gameScrolling = false;
					this.dialogueState = 1;
				}
			}
			Dialogue.visualizationTimer -= Game1.FrameTime;
			if (Dialogue.visualizationTimer < 0f)
			{
				if (Dialogue.visualizationState.Count > 0)
				{
					Dialogue.visualizationStep = Math.Min(Dialogue.visualizationStep + 1, Dialogue.visualizationState.Count - 1);
					Dialogue.mouthIsOpen = Dialogue.visualizationState[Dialogue.visualizationStep] == 2;
				}
				else
				{
					Dialogue.mouthIsOpen = true;
				}
				Dialogue.visualizationTimer += 0.1f;
			}
		}

		private void DrawResponse()
		{
			Vector2 vector = new Vector2(80f, Game1.bigFont.MeasureString(this.dialogueLine).Y * this.dialogueTextSize + 40f);
			int num = 0;
			for (int i = 0; i < this.responseLocalList.Count + 1; i++)
			{
				this.sprite.End();
				this.sprite.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
				this.sprite.Draw(this.hudTex[2], new Vector2(180f, vector.Y + (float)num - 8f), new Rectangle(0, 502, 326, 18), new Color(1f, 1f, 1f, Dialogue.textAlpha), 0f, new Vector2(163f, 0f), new Vector2(0.5f, 0.25f), SpriteEffects.None, 0f);
				this.sprite.End();
				this.sprite.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
				if (i >= this.responseLocalList.Count)
				{
					continue;
				}
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && Dialogue.textAlpha >= 0.5f)
				{
					float num2 = 1f - (this.topEdge - (float)(Game1.screenHeight - 250)) / 250f;
					int num3 = (int)Math.Min(this.topEdge + 70f / Game1.hiDefScaleOffset - Dialogue.windowHeight / 2f * Game1.hiDefScaleOffset, (float)Game1.screenHeight * 0.875f - Dialogue.windowHeight + (1f - num2) * 250f);
					Vector2 vector2 = new Vector2(80f, vector.Y + (float)num - 6f) + new Vector2(Dialogue.leftEdge, num3);
					if (new Rectangle((int)vector2.X, (int)vector2.Y, 600, this.responseLocalList[i].responseHeight + 10).Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y))
					{
						if (i != Dialogue.responseSelection)
						{
							Dialogue.responseSelection = i;
							Sound.PlayCue("menu_click");
						}
						if (Game1.pcManager.leftMouseClicked)
						{
							Game1.pcManager.leftMouseClicked = false;
							this.KeySelect = (Game1.hud.KeySelect = true);
						}
					}
				}
				if (i == Dialogue.responseSelection)
				{
					this.responseLocalList[i].reponseOffset = Math.Min(this.responseLocalList[i].reponseOffset + Game1.HudTime * 400f, 30f);
				}
				else
				{
					this.responseLocalList[i].reponseOffset = Math.Max(this.responseLocalList[i].reponseOffset - Game1.HudTime * 400f, 0f);
				}
				Color color = Game1.bigText.Color;
				for (int j = 0; j < this.responseMasterList.Count; j++)
				{
					if (this.responseMasterList[j].responseKey == this.responseLocalList[i].responseKey && this.responseMasterList[j].responseState == 1)
					{
						Game1.bigText.Color *= 0.7f;
						break;
					}
				}
				Game1.bigText.DrawOutlineText(vector + new Vector2(this.responseLocalList[i].reponseOffset, num), this.responseLocalList[i].responseString, this.dialogueTextSize * 0.9f, 0, TextAlign.Left, fullOutline: true);
				Game1.bigText.Color = color;
				num = ((this.responseLocalList[i].responseString == null) ? (num + 45) : (num + (this.responseLocalList[i].responseHeight + 10)));
			}
			if (Game1.hud.shopType == ShopType.None)
			{
				float num4 = 0f;
				for (int k = 0; k < Dialogue.responseSelection; k++)
				{
					num4 += (float)(this.responseLocalList[k].responseHeight + 10);
				}
				Vector2 vector3 = vector + new Vector2(15f, 14f + num4);
				if (Dialogue.textAlpha < 0.5f)
				{
					Game1.hud.cursorPos = vector3;
				}
				else
				{
					Game1.hud.cursorPos += (vector3 - Game1.hud.cursorPos) * Game1.HudTime * 40f;
				}
				Game1.hud.DrawCursor(Game1.hud.cursorPos, 0.75f, Game1.bigText.Color, flip: false);
			}
		}

		public void DrawDialogueSource(GraphicsDevice device, Texture2D[] particlesTex)
		{
			Game1.bigText.Color = new Color(1f, 1f, 1f, Dialogue.textAlpha * 2f);
			device.SetRenderTarget(Game1.hud.textTarget);
			device.Clear(Color.Transparent);
			this.sprite.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
			Game1.bigText.DrawButtonOutlineText(new Vector2(30f, 20f), this.dialogueLine, this.dialogueTextSize, this.dialogueLineButtonList, bounce: true, 1000f, TextAlign.Left);
			if (this.responseLocalList.Count > 1 && !Dialogue.threadBusy)
			{
				this.DrawResponse();
			}
			this.sprite.End();
			device.SetRenderTarget(null);
			if (Dialogue.textAlpha > 0f)
			{
				device.SetRenderTarget(Game1.hud.textTargetMask);
				device.Clear(Color.Transparent);
				this.sprite.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
				float y = Math.Max(Dialogue.windowHeight / 86f, 2f);
				this.sprite.Draw(particlesTex[2], new Vector2(550f - Math.Min(Dialogue.textAlpha, 1f) * 350f, Dialogue.windowHeight / 2f), new Rectangle(1082, 2112, 183, 86), Color.White * (1f - Math.Min(Dialogue.textAlpha, 1f)), 0f, new Vector2(183f, 43f), new Vector2(3.5f, y), SpriteEffects.None, 1f);
				this.sprite.Draw(particlesTex[2], new Vector2(150f + Math.Min(Dialogue.textAlpha, 1f) * 350f, Dialogue.windowHeight / 2f), new Rectangle(1082, 2112, 183, 86), Color.White * (1f - Math.Min(Dialogue.textAlpha, 1f)), 0f, new Vector2(0f, 43f), new Vector2(3.5f, y), SpriteEffects.FlipHorizontally, 1f);
				this.sprite.End();
				device.SetRenderTarget(null);
			}
		}

		public void Draw(float promptAlpha)
		{
			if (this.portraitID[this.currentLivePortrait] > -1 && this.topEdge > (float)(Game1.screenHeight - 250) && Dialogue.portraitLoaded[this.currentLivePortrait] != LoadState.Loaded)
			{
				this.topEdge = Game1.screenHeight;
				return;
			}
			this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.Black * Dialogue.screenDimAlpha);
			if (Game1.hud.shop != null)
			{
				for (int i = 0; i < this.portraitAlpha.Length; i++)
				{
					this.portraitAlpha[i] = -1f;
				}
				VoiceManager.StopVoice();
				return;
			}
			for (int j = 0; j < this.portraitID.Length; j++)
			{
				if (Dialogue.portraitLoaded[j] == LoadState.Loaded && this.portraitAlpha[j] > 0f)
				{
					if (Dialogue.portraitTex[j] != null && !Dialogue.portraitTex[j].IsDisposed)
					{
						Vector2 position = ((!this.portraitFlip[j]) ? Vector2.Zero : new Vector2(Game1.screenWidth, 0f));
						Vector2 origin = ((!this.portraitFlip[j]) ? Vector2.Zero : new Vector2(500f, 0f));
						SpriteEffects effects = (this.portraitFlip[j] ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
						this.sprite.Draw(Dialogue.portraitTex[j], position, new Rectangle(500 * Dialogue.animFrame - 4000 * (Dialogue.animFrame / 8), 480 * (Dialogue.animFrame / 8) + 1, 499, 479), new Color(1f, 1f, 1f, this.portraitAlpha[j]), 0f, origin, new Vector2(1.125f, 1.505f) * ((float)Game1.screenHeight / 720f), effects, 0f);
						if (Dialogue.speaking == 2 && this.portraitAlpha[this.currentLivePortrait] >= 1f && Dialogue.portraitTex[j].Height > 1920 && Dialogue.mouthIsOpen)
						{
							int num = (Dialogue.portraitTex[j].Height - 1920) / 4;
							this.sprite.Draw(Dialogue.portraitTex[j], position, new Rectangle(500 * Dialogue.animFrame - 4000 * (Dialogue.animFrame / 8), 1920 + num * (Dialogue.animFrame / 8) + 1, 499, num - 2), new Color(1f, 1f, 1f, this.portraitAlpha[j]), 0f, origin, new Vector2(1.125f, 1.505f) * ((float)Game1.screenHeight / 720f), effects, 0f);
						}
					}
				}
				else if (this.portraitID[j] > -1)
				{
					this.portraitAlpha[j] = 0f;
				}
			}
			float num2 = 1f - (this.topEdge - (float)(Game1.screenHeight - 250)) / 250f;
			int num3 = (int)Math.Min(this.topEdge + 70f / Game1.hiDefScaleOffset - Dialogue.windowHeight / 2f * Game1.hiDefScaleOffset, (float)Game1.screenHeight * 0.875f - Dialogue.windowHeight + (1f - num2) * 250f);
			Game1.hud.DrawMiniBorder(new Vector2(Dialogue.leftEdge + 50, num3), 600, (int)Dialogue.windowHeight, num2, 1f, (int)Dialogue.windowHeight, 0.1f);
			float num4 = Math.Min(Dialogue.textAlpha * 4f, 0.5f);
			if (num4 > 0.1f && Dialogue.textAlpha < 0.5f && this.topEdge < (float)(Game1.screenHeight - 120) && Game1.menu.prompt == promptDialogue.None)
			{
				float size = Math.Max((float)Dialogue.targetHeight / 200f, 0.4f);
				Vector2 vector = new Vector2((float)(Dialogue.leftEdge + 375) - Math.Min(Dialogue.textAlpha, 1f) * 700f, (float)num3 + Dialogue.windowHeight / 2f);
				float randomFloat = Rand.GetRandomFloat(-2.35f, -0.785f);
				Vector2 vector2 = vector + Rand.GetRandomVector2(-5f, 5f, -40f, 0f);
				Vector2 traj = new Vector2((float)(Math.Cos(randomFloat) * (double)vector2.X - Math.Sin(randomFloat) * (double)vector2.Y), (float)(Math.Cos(randomFloat) * (double)vector2.Y + Math.Sin(randomFloat) * (double)vector2.X)) - new Vector2((float)(Math.Cos(randomFloat) * (double)vector.X - Math.Sin(randomFloat) * (double)vector.Y), (float)(Math.Cos(randomFloat) * (double)vector.Y + Math.Sin(randomFloat) * (double)vector.X));
				Game1.pManager.AddHudDust(vector, traj, size, randomFloat, new Color(0.4f, 1f, 1f, 1f), num4, 0, 9);
				vector = new Vector2((float)(Dialogue.leftEdge + 325) + Math.Min(Dialogue.textAlpha, 1f) * 700f, (float)num3 + Dialogue.windowHeight / 2f);
				randomFloat = Rand.GetRandomFloat(0.785f, 2.35f);
				vector2 = vector + Rand.GetRandomVector2(-5f, 5f, -40f, 0f);
				traj = new Vector2((float)(Math.Cos(randomFloat) * (double)vector2.X - Math.Sin(randomFloat) * (double)vector2.Y), (float)(Math.Cos(randomFloat) * (double)vector2.Y + Math.Sin(randomFloat) * (double)vector2.X)) - new Vector2((float)(Math.Cos(randomFloat) * (double)vector.X - Math.Sin(randomFloat) * (double)vector.Y), (float)(Math.Cos(randomFloat) * (double)vector.Y + Math.Sin(randomFloat) * (double)vector.X));
				Game1.pManager.AddHudDust(vector, traj, size, randomFloat, new Color(0.4f, 1f, 1f, 1f), num4, 0, 9);
			}
			if (Game1.hud.textTarget != null && Game1.hud.textTargetMask != null)
			{
				this.sprite.End();
				Game1.refractEffect.Parameters["strength"].SetValue(0.05f * (1f - Math.Min(Dialogue.textAlpha, 1f)));
				Game1.graphics.GraphicsDevice.Textures[1] = Game1.hud.textTargetMask;
				this.sprite.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
				Game1.refractEffect.CurrentTechnique.Passes[0].Apply();
				this.sprite.Draw(Game1.hud.textTarget, new Vector2(Dialogue.leftEdge + 8, num3), new Rectangle(4, 4, 700, (int)Dialogue.windowHeight - 8), Color.White);
				this.sprite.End();
				this.sprite.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
			}
			Game1.refractEffect.Parameters["strength"].SetValue(0.003f);
			if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				if (Dialogue.canHelp && Dialogue.helpLine != null && Game1.menu.prompt == promptDialogue.None)
				{
					if (num2 >= 0f && Dialogue.helpAlpha < 6.28f)
					{
						Dialogue.helpAlpha += Game1.HudTime * 0.1f;
					}
					float num5 = ((Dialogue.helpAlpha > 0f) ? ((float)Math.Sin(Dialogue.helpAlpha) * 2f) : 0f);
					Game1.smallText.Color = new Color(1f, 1f, 1f, num5 * num2);
					Game1.hud.DrawMiniBorder(new Vector2(Game1.screenWidth / 2 - 200, (float)Game1.screenHeight * 0.88f), 400, 30, num5 * num2, 1f, 30, 0.3f);
					Game1.smallText.DrawButtonOutlineText(new Vector2(0f, (float)Game1.screenHeight * 0.88f), Dialogue.helpLine, 0.8f, Dialogue.dialogueHelpButtonList, bounce: false, Game1.screenWidth, TextAlign.Center);
				}
				if (Game1.pcManager.inputDevice == InputDevice.GamePad && (Sound.masterSFXVolume == 0f || Dialogue.voice[this.currentLivePortrait] == null || !Game1.settings.AutoAdvance) && Dialogue.textAlpha >= 1f && num2 >= 1f && Game1.menu.prompt != promptDialogue.SkipEvent)
				{
					this.sprite.Draw(this.hudTex[0], new Vector2(Dialogue.leftEdge + 350, (float)num3 + Dialogue.windowHeight), new Rectangle(200, 140, 46, 46), new Color(1f, 1f, 1f, 0.75f + (float)Math.Sin(Dialogue.continueButtonAlpha)), 0f, new Vector2(24f, 22f), 0.8f, SpriteEffects.None, 0f);
				}
			}
			else
			{
				Color color = new Color(1f, 1f, 1f, (1f - promptAlpha) * num2);
				if (promptAlpha < 1f && this.responseLocalList.Count < 2)
				{
					if (Game1.pcManager.DrawMouseButton(new Vector2(Game1.screenWidth / 2 + 20 - 75, Game1.screenHeight - (int)(80f * Game1.hiDefScaleOffset) - 20), 0.8f, color, 7, draw: true))
					{
						this.KeyCancel = true;
					}
					if (Game1.pcManager.DrawMouseButton(new Vector2(Game1.screenWidth / 2 + 10, Game1.screenHeight - (int)(80f * Game1.hiDefScaleOffset) - 20), 0.8f, color, 8, draw: true))
					{
						this.KeySelect = true;
					}
				}
				Game1.hud.mousePos = Game1.pcManager.DrawCursor(this.sprite, 0.8f, color);
			}
			if (Game1.hud.debugDisplay)
			{
				Game1.bigText.DrawText(new Vector2(20f, 20f), "Current Live: " + this.currentLivePortrait, this.dialogueTextSize);
				Game1.bigText.DrawText(new Vector2(20f, 60f), "Current Queue: " + this.currentQueuePortrait, this.dialogueTextSize);
				string text = (this.portraitFlip[this.currentLivePortrait] ? "True" : "False");
				Game1.bigText.DrawText(new Vector2(20f, 100f), "Current Flip: " + text, this.dialogueTextSize);
				Game1.bigText.DrawText(new Vector2(20f, 140f), "Queued:\n0: " + this.portraitID[0] + " : " + Dialogue.portraitLoaded[0].ToString() + "\n1: " + this.portraitID[1] + " : " + Dialogue.portraitLoaded[1], this.dialogueTextSize);
			}
		}

		public void DebugPortraits(ref int ID)
		{
			if (ID > -1)
			{
				int num = this.portraitID[this.currentLivePortrait];
				do
				{
					ID = Rand.GetRandomInt(0, 100);
				}
				while (!this.PortraitListed(ID) || ID == num);
			}
		}

		private bool PortraitListed(int id)
		{
			if (Game1.VoiceNotReady)
			{
				this.voiceID[0] = (this.voiceID[1] = "dummy");
			}
			return true;
		}

		private string TestLoc(TextBox box)
		{
			CultureInfo culture = new CultureInfo("fr-FR");
			return this.dialogueScript.manager.GetString(box.lineKey, culture);
		}
	}
}
