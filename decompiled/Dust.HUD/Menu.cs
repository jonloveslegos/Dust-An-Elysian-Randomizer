using System;
using System.Collections.Generic;
using Dust.Audio;
using Dust.CharClasses;
using Dust.Dialogue;
using Dust.MapClasses;
using Dust.NavClasses;
using Dust.Particles;
using Dust.PCClasses;
using Dust.Storage;
using Dust.Strings;
using Lotus.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Dust.HUD
{
	public class Menu
	{
		public MenuMode menuMode;

		public promptDialogue prompt;

		private static int maxFiles = 11;

		public FileManage[] fileManage = new FileManage[Menu.maxFiles];

		private SpriteBatch sprite;

		private Texture2D[] particlesTex;

		private Texture2D[] hudTex;

		private Texture2D nullTex;

		private Texture2D numbersTex;

		private static Texture2D miscTex;

		private static Texture2D menuTex;

		private static Texture2D gamerPic;

		private static bool helpLoaded;

		private static int trialOffset = 0;

		private Character[] character;

		private Map map;

		private ScoreDraw scoreDraw;

		public Chapter chapter;

		private static int menuOptions;

		private static string menuTitle;

		private static string[] menuName;

		public int curMenuPage = -10;

		public int curMenuOption;

		private static int prevMenuOption;

		private static float textAlpha;

		private static float pauseAlpha;

		private static float checkGuideTime;

		private static float noInputTime;

		private static Vector2 cursorPos;

		private static float continueButtonAlpha;

		private static float fadeInAlpha = 1f;

		private static float savingAlpha = 0f;

		private static float menuSelectMove;

		private static float titleFloat;

		private static float titleScale = 1f;

		private static float titleFade = 1f;

		private static float titleSubFade = 1f;

		private static float screenFade;

		private static int titleEvent = 0;

		private static float titleLightning = 0f;

		private static int leftEdge;

		private static int topEdge;

		private static int rightEdge;

		private static int bottomEdge;

		private static float movingBottomEdge = 0f;

		private static float windowWidth;

		private static float listScroll;

		private static float maxScroll;

		private static float manualContentsPos;

		private static bool manualContents;

		private static bool savingHelpDisplayed;

		public byte configuringControls;

		private static float confirmingResolution;

		private static bool KeyCancel;

		private static float prevSFXVolume;

		private static float prevMusicVolume;

		private static bool prevRumble;

		private static bool prevAutoCombo;

		private static bool prevAutoHeal;

		private static bool prevAutoLevelUp;

		private static bool prevAutoAdvance;

		private static bool prevColorBlind;

		public byte prevDifficulty;

		private static Vector2 prevResolution;

		private static bool prevBloom;

		private static bool prevDepth;

		private static bool prevFullScreen;

		private static int prevInputMethod;

		private static int prevScreenTopOffset;

		private static int prevScreenLeftOffset;

		private static float prevHudScale;

		private static bool prevHudDetails;

		public byte tempSaveSlot;

		public bool confirming;

		private static float confirmAlpha;

		private static float fileAlpha;

		private static float fileScroll;

		private static int settingSelection;

		public long mostRecentTime;

		public bool canNewGamePlus;

		public int[] saveCompletion = new int[Menu.maxFiles];

		public int[] saveHP = new int[Menu.maxFiles];

		public int[] saveBonusHP = new int[Menu.maxFiles];

		public int[] saveMaxHP = new int[Menu.maxFiles];

		public int[] saveLevel = new int[Menu.maxFiles];

		public int[] saveGold = new int[Menu.maxFiles];

		public float[] saveDifficulty = new float[Menu.maxFiles];

		public float[] saveGameClock = new float[Menu.maxFiles];

		public string[] saveRegionName = new string[Menu.maxFiles];

		private static string[] saveTitle = new string[Menu.maxFiles];

		private static string[] saveDescription = new string[Menu.maxFiles];

		public string optionDesc = string.Empty;

		public Dictionary<Vector3, string> optionDescButtonList = new Dictionary<Vector3, string>();

		private string buttonText = string.Empty;

		private Dictionary<Vector3, string> buttonTextButtonList = new Dictionary<Vector3, string>();

		private string simpleString = string.Empty;

		private Dictionary<Vector3, string> simpleStringButtonList = new Dictionary<Vector3, string>();

		private Dictionary<float, string> scrollString = new Dictionary<float, string>();

		private Dictionary<Rectangle, Vector2> scrollImages = new Dictionary<Rectangle, Vector2>();

		private Dictionary<Vector2, string> scrollBookMarks = new Dictionary<Vector2, string>();

		private static RenderTarget2D scrollMask;

		private static RenderTarget2D scrollSource;

		private Texture2D scrollMaskTexture;

		private Texture2D scrollSourceTexture;

		public float SetGuideTime
		{
			set
			{
				Menu.checkGuideTime = value;
			}
		}

		public int TitleEvent
		{
			set
			{
				Menu.titleEvent = value;
			}
		}

		public Menu(SpriteBatch _sprite, Texture2D[] _particlesTex, Texture2D _nullTex, Texture2D[] _hudTex, Texture2D _numbersTex, Character[] _character, Map _map)
		{
			this.sprite = _sprite;
			this.particlesTex = _particlesTex;
			this.character = _character;
			this.map = _map;
			this.nullTex = _nullTex;
			this.hudTex = _hudTex;
			this.numbersTex = _numbersTex;
			this.scoreDraw = new ScoreDraw(this.sprite, this.numbersTex);
			Menu.menuTitle = string.Empty;
			Menu.menuOptions = 15;
			if (Game1.testDialogue)
			{
				Menu.menuOptions = 50;
			}
			Menu.menuName = new string[Menu.menuOptions];
			this.optionDesc = string.Empty;
			for (int i = 0; i < Menu.menuOptions; i++)
			{
				Menu.menuName[i] = string.Empty;
			}
			Menu.savingHelpDisplayed = false;
			this.ResetMenu();
		}

		public void ResetMenu()
		{
			this.menuMode = MenuMode.None;
			this.prompt = promptDialogue.None;
			Menu.titleFloat = 0f;
			Menu.titleScale = 1f;
			Menu.titleFade = 1f;
			Menu.titleSubFade = 1f;
			Menu.screenFade = 0f;
			Menu.titleEvent = 0;
			this.curMenuPage = -10;
			this.curMenuOption = 0;
			Menu.fadeInAlpha = 1f;
			Menu.listScroll = (Menu.maxScroll = 0f);
			Menu.manualContents = false;
			this.confirming = false;
			Menu.confirmAlpha = (Menu.fileAlpha = (Menu.textAlpha = (Menu.pauseAlpha = (Menu.checkGuideTime = 0f))));
			Menu.movingBottomEdge = 0f;
			Menu.cursorPos = new Vector2(571f, 778f);
			this.ResetMenuDimensions();
		}

		public void ResetMenuDimensions()
		{
			Menu.leftEdge = (int)((float)Game1.screenWidth * 0.075f);
			Menu.topEdge = (int)((float)Game1.screenHeight * 0.075f);
			Menu.rightEdge = (int)((float)Game1.screenWidth * 0.925f);
			Menu.bottomEdge = (int)((float)Game1.screenHeight * 0.925f);
			float num = 0.7f;
			if (Game1.standardDef)
			{
				num = 0.65f;
			}
			Menu.windowWidth = Menu.rightEdge - Menu.leftEdge - (int)(100f * num);
		}

		public void LoadMenuTextures()
		{
			Menu.menuTex = Game1.GetTitleContent().Load<Texture2D>("gfx/ui/main_menu");
		}

		public void UnloadMenuTextures()
		{
			Game1.GetTitleContent().Unload();
			Game1.GetLargeContent().Unload();
			Menu.helpLoaded = false;
			Game1.hud.ExitShop();
		}

		public void SetHelpTextures()
		{
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadHelpTextures), new TaskFinishedDelegate(LoadingHelpFinished)));
		}

		public void LoadHelpTextures()
		{
			if (this.menuMode == MenuMode.Help)
			{
				this.PopulateScroll((int)Menu.windowWidth);
				this.PopulateHelp((int)Menu.windowWidth);
			}
			Menu.miscTex = Game1.GetLargeContent().Load<Texture2D>("gfx/ui/help_elements");
			GC.Collect();
		}

		public void LoadingHelpFinished(int taskId)
		{
			Menu.helpLoaded = true;
		}

		public void LoadPCHighScoresTextures()
		{
			Menu.miscTex = Game1.GetLargeContent().Load<Texture2D>("gfx/ui/achievement_sheet");
		}

		public void PopulateMenu()
		{
			Menu.trialOffset = ((!Game1.GamerServices || !Game1.IsTrial) ? 1 : 0);
			float size = 1f;
			if (this.prompt != promptDialogue.None)
			{
				this.PopulatePrompt();
			}
			else
			{
				if (this.menuMode == MenuMode.FileManage)
				{
					return;
				}
				for (int i = 0; i < Menu.menuOptions; i++)
				{
					Menu.menuName[i] = string.Empty;
				}
				this.optionDesc = string.Empty;
				string s = string.Empty;
				if (Game1.gameMode == Game1.GameModes.MainMenu)
				{
					switch (this.curMenuPage)
					{
						case -3:
							this.simpleString = Game1.bigText.WordWrap(Strings_MainMenu.BeginSignedOut + "\n" + (Game1.isPCBuild ? Strings_PC.Begin : Strings_MainMenu.Begin), size, 480f, this.simpleStringButtonList, TextAlign.LeftAndCenter);
							break;
						case -2:
							this.simpleString = Game1.bigText.WordWrap(Strings_MainMenu.BeginFail, size, 480f, this.simpleStringButtonList, TextAlign.LeftAndCenter);
							break;
						default:
							this.simpleString = Game1.bigText.WordWrap(Game1.isPCBuild ? Strings_PC.Begin : Strings_MainMenu.Begin, size, 480f, this.simpleStringButtonList, TextAlign.LeftAndCenter);
							break;
						case 0:
							{
								for (int k = 0; k < 6 - Menu.trialOffset; k++)
								{
									int num2 = k;
									if (k > 0)
									{
										num2 += Menu.trialOffset;
									}
									Menu.menuName[k] = Strings_MainMenu.ResourceManager.GetString("Main" + num2);
									if (this.curMenuOption == k)
									{
										switch (num2)
										{
											case 0:
												s = ((!Game1.IsTrial) ? Strings_MainMenu.ResourceManager.GetString("Main0Desc") : Strings_MainMenu.ResourceManager.GetString("Main0Desc2"));
												break;
											case 1:
												{
													SignedInGamer signedInGamer = Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)];
													s = ((signedInGamer != null && signedInGamer.IsSignedInToLive) ? (signedInGamer.Privileges.AllowPurchaseContent ? Strings_MainMenu.ResourceManager.GetString("Main1Desc") : Strings_MainMenu.ResourceManager.GetString("Main3Desc3")) : Strings_MainMenu.ResourceManager.GetString("Main1Desc2"));
													break;
												}
											case 3:
												{
													SignedInGamer signedInGamer2 = Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)];
													s = ((Game1.isPCBuild || (signedInGamer2 != null && signedInGamer2.IsSignedInToLive)) ? Strings_MainMenu.ResourceManager.GetString("Main3Desc") : Strings_MainMenu.ResourceManager.GetString("Main1Desc2"));
													break;
												}
											case 5:
												s = ((!Game1.isPCBuild) ? Strings_MainMenu.ResourceManager.GetString("Main" + num2 + "Desc") : Strings_PC.ResourceManager.GetString("Main5Desc"));
												break;
											default:
												s = Strings_MainMenu.ResourceManager.GetString("Main" + num2 + "Desc");
												break;
										}
									}
								}
								if (!Game1.hasLeaderboards)
								{
									Menu.menuName[2] = string.Empty;
								}
								break;
							}
						case 1:
							{
								int num3 = 4;
								Menu.menuName[num3] = Strings_MainMenu.MainBack;
								for (int l = 0; l < num3; l++)
								{
									Menu.menuName[l] = Strings_MainMenu.ResourceManager.GetString("ChooseDiff" + l);
								}
								s = ((this.curMenuOption >= num3) ? Strings_MainMenu.MainBackDesc : Strings_MainMenu.ResourceManager.GetString("ChooseDiff" + this.curMenuOption + "Desc"));
								break;
							}
						case 3:
							{
								int num = 5;
								Menu.menuName[num] = Strings_MainMenu.MainBack;
								for (int j = 0; j < num; j++)
								{
									Menu.menuName[j] = Strings_MainMenu.ResourceManager.GetString("Options" + j);
								}
								s = ((this.curMenuOption >= num) ? Strings_MainMenu.MainBackDesc : Strings_MainMenu.ResourceManager.GetString("Options" + this.curMenuOption + "Desc"));
								break;
							}
						case 4:
							Menu.menuName[0] = Strings_MainMenu.QuitYes;
							Menu.menuName[1] = Strings_MainMenu.QuitNo;
							s = Strings_MainMenu.QuitDesc;
							break;
						case 5:
							Menu.menuName[0] = Strings_MainMenu.FindStorage0;
							Menu.menuName[1] = Strings_MainMenu.FindStorage1;
							s = Strings_MainMenu.QuitDesc;
							break;
					}
					this.optionDesc = Game1.smallText.WordWrap(s, size, 620f, TextAlign.Center);
				}
				else
				{
					string text = Strings_PauseMenu.PauseTitle;
					switch (this.curMenuPage)
					{
						case 0:
							{
								for (int num5 = 0; num5 < 7 - Menu.trialOffset; num5++)
								{
									int num6 = num5;
									if (num5 > 0)
									{
										num6 += Menu.trialOffset;
									}
									Menu.menuName[num5] = Strings_PauseMenu.ResourceManager.GetString("Main" + num6);
									if (this.curMenuOption == num5)
									{
										if (num6 == 1)
										{
											SignedInGamer signedInGamer3 = Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)];
											s = ((signedInGamer3 != null && signedInGamer3.IsSignedInToLive) ? (signedInGamer3.Privileges.AllowPurchaseContent ? Strings_MainMenu.ResourceManager.GetString("Main1Desc") : Strings_MainMenu.ResourceManager.GetString("Main3Desc3")) : Strings_MainMenu.ResourceManager.GetString("Main1Desc2"));
										}
										else if (num6 == 3 && Game1.GamerServices && Game1.IsTrial)
										{
											s = Strings_PauseMenu.ResourceManager.GetString("Main3DescTrial");
										}
										else if (num6 == 4)
										{
											SignedInGamer signedInGamer4 = Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)];
											s = ((Game1.isPCBuild || (signedInGamer4 != null && signedInGamer4.IsSignedInToLive)) ? Strings_PauseMenu.ResourceManager.GetString("Main" + num6 + "Desc") : Strings_MainMenu.ResourceManager.GetString("Main1Desc2"));
										}
										else
										{
											s = Strings_PauseMenu.ResourceManager.GetString("Main" + num6 + "Desc");
										}
									}
									if (!Game1.hasLeaderboards)
									{
										Menu.menuName[3] = string.Empty;
									}
								}
								break;
							}
						case 1:
							{
								text = Strings_PauseMenu.OptionsTitle;
								int num4 = 5;
								Menu.menuName[num4] = Strings_PauseMenu.PauseBack;
								for (int n = 0; n < num4; n++)
								{
									Menu.menuName[n] = Strings_PauseMenu.ResourceManager.GetString("Option" + n);
								}
								s = ((this.curMenuOption >= num4) ? Strings_PauseMenu.PauseBackDesc : Strings_PauseMenu.ResourceManager.GetString("Option" + this.curMenuOption + "Desc"));
								break;
							}
						case 2:
							{
								text = Strings_PauseMenu.QuitTitle;
								for (int m = 0; m < 2; m++)
								{
									Menu.menuName[m] = Strings_PauseMenu.ResourceManager.GetString("Quit" + m);
								}
								s = Strings_PauseMenu.ResourceManager.GetString("Quit" + this.curMenuOption + "Desc");
								break;
							}
						case 10:
							text = "Debug Menu";
							Menu.menuName[0] = "Unlock World";
							Menu.menuName[1] = "Level Up";
							Menu.menuName[2] = "Add Gold";
							Menu.menuName[3] = "Unlock Abilities";
							Menu.menuName[4] = "Invulnerability";
							Menu.menuName[5] = "Unlock BluePrints/Materials";
							Menu.menuName[6] = "Unlock Equipment";
							Menu.menuName[7] = "Unlock Friends";
							Menu.menuName[8] = "Destructable Walls";
							Menu.menuName[9] = "Unblock Sides";
							Menu.menuName[10] = "Set Event";
							Menu.menuName[11] = "Unlock Quests";
							Menu.menuName[12] = "View Cutscenes";
							switch (this.curMenuOption)
							{
								case 0:
									s = "Press [A] to unlock all regions.\nPress [X] to jump to the World Map with full access.\nPress [Y] to reset all regions.";
									s += "\n\nNote that accessing new areas may break event continuity, and require leveling-up.";
									break;
								case 1:
									{
										s = "Press [A] to initiate a Level-Up. Press [X] to reset level/XP/Equipped items.\n\nYou are currently at level (" + Game1.stats.LEVEL / 4 + ").";
										object obj2 = s;
										s = string.Concat(obj2, "\n Total XP:", Game1.stats.XP, ", Next XP:", Game1.stats.nextLevelXP);
										break;
									}
								case 2:
									s = "Press [A] to add 5000 gold.\nPress [Y] to reset gold.\n\nYou are currently have (" + Game1.stats.Gold + ") gold.";
									break;
								case 3:
									s = "Press [A] to unlock all abilities and projectile types.\nPress [Y] to remove all abilities.";
									s += "\n\nNote that this may break event continuity where an upgrade is expected, and require 'Unblocking sides' below.";
									break;
								case 4:
									s = "Press [A] to toggle in-game invulnerability.  Currently set to ";
									s += (Game1.invulnerable ? "(On)" : "(Off)");
									s += ".\n\nNote that you may still be knocked down by hazards.";
									break;
								case 5:
									s = "Press [A] to acquire blueprints for all equipment.\n Press [X] to add 9999 pieces of every material.\n Press [Y] to populate shops with 9999 pieces of every material.";
									break;
								case 6:
									s = "Press [A] to add 250 pieces of every item, treasure key, equipment, and material.\n Press [X] for every Resonance Gem and 250 Treasure Keys.\n Press [Y] to clear keys, inventory, and shops.";
									break;
								case 7:
									s = "Press [A] to unlock all caged friends and open the Sanctuary.  Press [X] to relock all friends.";
									break;
								case 8:
									s = "Press [A] to destroy local map destructable walls.\n Press [X] to destroy world-wide walls.\n Press [Y] to reset all walls (may require re-entering local map).";
									break;
								case 9:
									s = "Press [A] to unblock the edges of the map.  This may happen during cinematics, or while picking up upgrades.  Note that unlocking abilities in Debug can break event continuity, and require unblocking edges in certain cases.";
									break;
								case 10:
									{
										s = "Press [X] to rewind events by 10.\nPress [A] to forward events by 10.\n Press [Y] to reset all event, side-event, and dialogue continuity.\nPress [LB] to complete all side events.";
										object obj = s;
										s = string.Concat(obj, "\n\nYou are currently at event (", Game1.events.currentEvent, ").");
										break;
									}
								case 11:
									s = "Press [A] to initiate all side quests and unlock all notes.\nPress [X] to complete all side quests.\nPress [Y] to reset all quests.";
									break;
								case 12:
									s = "[A] Cutscene 50 (Bedroom)\n[X] Cutscene 60 (Jin/Cassius Death)\n[Y] Cutscene 100 (I am Dust)\n[LB] Cutscene 120 (Ending)\n[LT]Credits - Note you cannot cancel out of this!";
									break;
							}
							break;
						case 11:
							text = "Test Dialogue";
							Menu.menuName[0] = "events 0 - 99";
							Menu.menuName[1] = "events 100 - 199";
							Menu.menuName[2] = "events 200 - 299";
							Menu.menuName[3] = "events 300 - 399";
							Menu.menuName[4] = "events 400 - 499";
							Menu.menuName[5] = "events 500 - 599";
							Menu.menuName[6] = "events 600 - 699";
							Menu.menuName[7] = "events 700+";
							Menu.menuName[8] = "avgustin";
							Menu.menuName[9] = "bean";
							Menu.menuName[10] = "blop";
							Menu.menuName[11] = "bram";
							Menu.menuName[12] = "calum";
							Menu.menuName[13] = "colleen";
							Menu.menuName[14] = "corbin";
							Menu.menuName[15] = "elder";
							Menu.menuName[16] = "fale";
							Menu.menuName[17] = "flohop";
							Menu.menuName[18] = "geehan";
							Menu.menuName[19] = "gianni";
							Menu.menuName[20] = "ginger";
							Menu.menuName[21] = "haley";
							Menu.menuName[22] = "kane";
							Menu.menuName[23] = "lady";
							Menu.menuName[24] = "mamop";
							Menu.menuName[25] = "matti";
							Menu.menuName[26] = "moska";
							Menu.menuName[27] = "oldgappy";
							Menu.menuName[28] = "oneida";
							Menu.menuName[29] = "reed";
							Menu.menuName[30] = "sanjin";
							Menu.menuName[31] = "sarahi";
							Menu.menuName[32] = "shopaurora";
							Menu.menuName[33] = "shopwild";
							Menu.menuName[34] = "smobop";
							Menu.menuName[35] = "cora";
							s = "Press [A] to test dialogue";
							break;
					}
					Menu.menuTitle = text;
					this.optionDesc = Game1.smallText.WordWrap(s, 1f, 620f, this.optionDescButtonList, TextAlign.Center);
				}
				this.buttonText = Game1.smallText.WordWrap(Strings_PauseMenu.PauseControls, 0.75f, 620f, this.buttonTextButtonList, TextAlign.Left);
			}
		}

		private void UpdateMainMenu(ParticleManager pMan)
		{
			if (Menu.fileAlpha > 0f)
			{
				return;
			}
			int num = this.curMenuPage;
			int num2 = 0;
			for (int i = 0; i < Menu.menuOptions; i++)
			{
				if (Menu.menuName[i] != string.Empty)
				{
					num2 = i;
				}
			}
			if (Game1.hud.KeyUp && this.curMenuOption > 0)
			{
				Game1.hud.KeyUp = false;
				bool flag = false;
				do
				{
					flag = true;
					this.curMenuOption--;
				}
				while (this.curMenuOption > 0 && Menu.menuName[this.curMenuOption] == string.Empty);
				if (flag)
				{
					this.PopulateMenu();
					Menu.fadeInAlpha = 1f;
					Sound.PlayCue("menu_click");
				}
			}
			else if (Game1.hud.KeyDown && this.curMenuOption < num2)
			{
				Game1.hud.KeyDown = false;
				bool flag2 = false;
				do
				{
					flag2 = true;
					this.curMenuOption++;
				}
				while (this.curMenuOption < num2 && Menu.menuName[this.curMenuOption] == string.Empty);
				if (flag2)
				{
					this.PopulateMenu();
					Menu.fadeInAlpha = 1f;
					Sound.PlayCue("menu_click");
				}
			}
			if (Game1.hud.KeyCancel)
			{
				Sound.PlayCue("menu_cancel");
				Game1.hud.KeyCancel = (Menu.KeyCancel = false);
				if (this.curMenuPage == 0)
				{
					this.curMenuPage = -1;
					Game1.currentGamePad = -1;
					Menu.continueButtonAlpha = 0f;
				}
				else
				{
					this.curMenuPage = 0;
				}
				this.curMenuOption = Menu.prevMenuOption;
				this.PopulateMenu();
				this.SetMenuSelectMove();
			}
			else if (Game1.hud.KeySelect)
			{
				Sound.PlayCue("menu_confirm");
				Menu.prevMenuOption = this.curMenuOption;
				Game1.hud.KeySelect = false;
				Game1.hud.canInput = false;
				if (this.curMenuPage == 0)
				{
					if (this.curMenuOption == 0)
					{
						Game1.awardsManager.EarnGamerPicture("gamerpic1");
						Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(Game1.character[0].LoadDustTextures)));
						if (Game1.GamerServices && Game1.IsTrial)
						{
							this.curMenuPage = 1;
							this.curMenuOption = 1;
						}
						else
						{
							this.InitFileManager();
						}
					}
					else if (this.curMenuOption == 1 && Menu.trialOffset == 0)
					{
						Game1.awardsManager.InitPurchase();
					}
					else if (this.curMenuOption + Menu.trialOffset == 2)
					{
						this.curMenuPage = 3;
						this.curMenuOption = 0;
					}
					else if (this.curMenuOption + Menu.trialOffset == 3)
					{
						Game1.awardsManager.InitLeaderBoard(LeaderBoardFilter.Completion, -1);
					}
					else if (this.curMenuOption + Menu.trialOffset == 4)
					{
						if (Game1.isPCBuild)
						{
							this.InitPCHighScores();
						}
						else
						{
							Game1.awardsManager.ViewAchievements();
						}
					}
					else if (this.curMenuOption + Menu.trialOffset == 5)
					{
						this.curMenuPage = 4;
						this.curMenuOption = 1;
					}
				}
				else if (this.curMenuPage == 1)
				{
					if (Menu.fileAlpha <= 0f)
					{
						Menu.menuSelectMove = 0f;
						if (this.curMenuOption < 4)
						{
							this.menuMode = MenuMode.Loading;
							Menu.titleFade = 1f;
						}
						else
						{
							this.curMenuPage = 0;
							this.curMenuOption = 0;
						}
					}
				}
				else if (this.curMenuPage == 3)
				{
					if (this.curMenuOption == 0)
					{
						this.InitHelp();
					}
					else if (this.curMenuOption == 1)
					{
						this.InitHelp();
					}
					else if (this.curMenuOption == 2)
					{
						this.InitSettings();
					}
					else if (this.curMenuOption == 3)
					{
						this.InitHUDAdjust();
					}
					else if (this.curMenuOption == 4)
					{
						this.InitCredits();
					}
					else if (this.curMenuOption == 5)
					{
						this.curMenuPage = 0;
						this.curMenuOption = 1 + (1 - Menu.trialOffset);
					}
				}
				else if (this.curMenuPage == 4)
				{
					if (this.curMenuOption == 0)
					{
						if (Game1.GamerServices && Game1.IsTrial)
						{
							Game1.InitUpsell(quitting: true);
						}
						else
						{
							this.menuMode = MenuMode.Quitting;
						}
						Menu.titleFade = 1f;
					}
					else if (this.curMenuOption == 1)
					{
						this.curMenuPage = 0;
						this.curMenuOption = 4 + (1 - Menu.trialOffset);
					}
				}
				else if (this.curMenuPage == 5)
				{
					if (this.curMenuOption == 0)
					{
						Game1.storage.GetDevice(LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One));
						this.menuMode = MenuMode.None;
						this.curMenuPage = 2;
						this.curMenuOption = 0;
					}
					else if (this.curMenuOption == 1)
					{
						Game1.storage.device = null;
						this.curMenuPage = 0;
						this.curMenuOption = 0;
					}
				}
				if (this.menuMode == MenuMode.None)
				{
					this.PopulateMenu();
				}
				this.SetMenuSelectMove();
			}
			if (num != this.curMenuPage)
			{
				Menu.textAlpha = 100f;
				Menu.fadeInAlpha = 1f;
			}
		}

		private void SetMenuSelectMove()
		{
			int num = 0;
			for (int i = 0; i < this.curMenuOption; i++)
			{
				if (Menu.menuName[i] != string.Empty)
				{
					num++;
				}
			}
			Menu.menuSelectMove = num * 30;
		}

		private void DrawMainMenu()
		{
			int num = Game1.screenWidth / 2 - 350;
			int num2 = Game1.screenHeight - 460;
			Color color = new Color(1f, 1f, 1f, 1f - Menu.fileAlpha);
			if (this.curMenuPage > -10 && this.curMenuPage < 0 && Menu.checkGuideTime == 0f)
			{
				Game1.bigText.Color = Color.White * Math.Abs((float)Math.Sin(Menu.continueButtonAlpha));
				if (Menu.noInputTime <= 0f)
				{
					Game1.bigText.DrawButtonOutlineText(new Vector2(0f, Game1.screenHeight / 2 + 50), this.simpleString, 1f, this.simpleStringButtonList, bounce: false, Game1.screenWidth, TextAlign.Center);
				}
			}
			else if (this.curMenuPage >= 0 && (double)Menu.titleScale <= 1.1)
			{
				int num3 = 90;
				if (this.optionDesc != string.Empty)
				{
					num3 += (int)Game1.smallFont.MeasureString(this.optionDesc).Y;
				}
				int num4 = Math.Max(170 + num3, 0);
				Menu.movingBottomEdge = Math.Max(Menu.movingBottomEdge + ((float)num4 - Menu.movingBottomEdge) * Game1.HudTime * 12f, 280f);
				if ((float)num2 + Menu.movingBottomEdge > (float)Game1.screenHeight * 0.9f)
				{
					num2 = (int)((float)num2 - ((float)num2 + Menu.movingBottomEdge - (float)Game1.screenHeight * 0.9f));
				}
				Vector2 vector = new Vector2(Game1.screenWidth / 2 - 300, num2 + 55);
				Game1.hud.DrawBorder(new Vector2(num, num2), 700, (int)Menu.movingBottomEdge, color, 0.9f, 190);
				float num5 = 1f;
				int num6 = (int)(Game1.smallFont.MeasureString(this.buttonText).X * 0.75f) + 20;
				Game1.hud.DrawMiniBorder(new Vector2(num + 350 - num6 / 2, (float)num2 + Menu.movingBottomEdge - 15f), num6, 30, color, 1f);
				Game1.smallText.Color = color;
				Game1.smallText.DrawButtonText(new Vector2(num, num2 + (int)Menu.movingBottomEdge - 10), this.buttonText, 0.75f, this.buttonTextButtonList, bounce: false, 700f, TextAlign.Center);
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				int num7 = 0;
				for (int i = 0; i < Menu.menuOptions; i++)
				{
					if (Menu.menuName[i] != string.Empty)
					{
						dictionary.Add(num7, i);
						num7++;
					}
				}
				int num8 = 0;
				for (int j = 0; j < this.curMenuOption; j++)
				{
					if (Menu.menuName[j] != string.Empty)
					{
						num8++;
					}
				}
				foreach (KeyValuePair<int, int> item in dictionary)
				{
					float num9 = MathHelper.Clamp(1f / ((float)Math.Abs(num8 - item.Key) * 1.5f), 0f, 1f);
					if (num8 == item.Key)
					{
						Game1.bigText.Color = new Color(1f, 1f, 1f, (1f - Menu.fileAlpha) * (1f - Menu.textAlpha / 100f));
						num5 = 1.2f;
						this.DrawCursors(new Vector2((float)(Game1.screenWidth / 2 + 100) - Game1.bigFont.MeasureString(Menu.menuName[item.Value]).X / 2f * num5, (int)vector.Y + 25), 0.5f, Game1.bigFont.MeasureString(Menu.menuName[item.Value]).X * num5, 1f - Menu.textAlpha / 100f);
					}
					else
					{
						Game1.bigText.Color = new Color(0.5f, 0.5f, 0.5f, (1f - Menu.fileAlpha) * num9 * (1f - Menu.textAlpha / 100f));
						num5 = 1f;
					}
					if (Math.Abs(num8 - item.Key) < 3)
					{
						Game1.bigText.DrawText(new Vector2(0f, vector.Y + 30f + (float)(item.Key * 30) - Menu.menuSelectMove), Menu.menuName[item.Value], num5, new Vector2(num5, num5 * num9), Game1.screenWidth, TextAlign.Center);
					}
				}
			}
			if ((double)Menu.titleScale <= 1.1 && this.curMenuPage > -1 && this.optionDesc != string.Empty)
			{
				Game1.smallText.Color = new Color(1f, 1f, 1f, (1f - Menu.fadeInAlpha) * (float)((int)color.A / 255));
				Game1.smallText.DrawText(new Vector2(num + 40, num2 + 200), this.optionDesc, 1f, 560f, TextAlign.Left);
			}
			if (Menu.savingAlpha > 0f)
			{
				this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.Black * Math.Min(Menu.savingAlpha * 6f, 0.75f));
				Game1.smallText.Color = color * Menu.savingAlpha * 6f;
				int num10 = (int)Math.Max(Game1.smallFont.MeasureString(Strings_Hud.Fidget_AutoSaving).X * 0.75f + 20f, 200f);
				Game1.hud.DrawMiniBorder(new Vector2(num + 350 - num10 / 2, num2 + 155), num10, 50, color * Menu.savingAlpha * 6f, 1f);
				Game1.smallText.DrawText(new Vector2(num, num2 + 160), Strings_Hud.Fidget_AutoSaving, 1f, 700f, TextAlign.Center);
			}
			if (Menu.fileAlpha > 0f)
			{
				this.DrawFileScreen();
			}
			if (this.menuMode == MenuMode.Loading || this.menuMode == MenuMode.Quitting)
			{
				this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, Menu.screenFade));
			}
		}

		private void DrawMainMenuPC()
		{
			if (this.curMenuPage > -10 && this.curMenuPage < 0 && Menu.checkGuideTime == 0f)
			{
				Game1.bigText.Color = Color.White * Math.Abs((float)Math.Sin(Menu.continueButtonAlpha));
				if (Menu.noInputTime <= 0f)
				{
					Game1.bigText.DrawButtonOutlineText(new Vector2(0f, Game1.screenHeight / 2 + 50), this.simpleString, 1f, this.simpleStringButtonList, bounce: false, Game1.screenWidth, TextAlign.Center);
				}
			}
			else
			{
				if (this.curMenuPage < 0)
				{
					return;
				}
				if ((double)Menu.titleScale <= 1.1)
				{
					int num = 0;
					for (int i = 0; i < Menu.menuOptions; i++)
					{
						if (Menu.menuName[i] != string.Empty)
						{
							num++;
						}
					}
					int num2 = num * 40 + 80;
					int num3 = Game1.screenWidth / 2 - 350;
					int num4 = Game1.screenHeight - 400;
					float num5 = 1f - Menu.fileAlpha;
					Color color = new Color(1f, 1f, 1f, num5);
					int num6 = 0;
					if (this.optionDesc != string.Empty)
					{
						num6 += (int)Game1.smallFont.MeasureString(this.optionDesc).Y;
					}
					int num7 = Math.Max(num2 + Math.Max(num6, 140), 0);
					Menu.movingBottomEdge = Math.Max(Menu.movingBottomEdge + ((float)num7 - Menu.movingBottomEdge) * Game1.HudTime * 12f, 300f);
					if ((float)num4 + Menu.movingBottomEdge > (float)Game1.screenHeight * 0.9f)
					{
						num4 -= (int)((float)num4 + Menu.movingBottomEdge - (float)Game1.screenHeight * 0.9f);
					}
					Vector2 vector = new Vector2(num3, num4 + 50);
					Game1.hud.DrawBorder(new Vector2(num3, num4), 700, (int)Menu.movingBottomEdge, color, 0.9f, num2);
					float num8 = 1f;
					int num9 = 0;
					for (int j = 0; j < Menu.menuOptions; j++)
					{
						if (this.curMenuOption == j)
						{
							Game1.bigText.Color = new Color(1f, 1f, 1f, num5 * (1f - Menu.textAlpha / 100f));
							num8 = 1f;
							this.DrawCursors(new Vector2((float)(Game1.screenWidth / 2 + 100) - Game1.bigFont.MeasureString(Menu.menuName[j]).X / 2f * num8, (int)vector.Y + 25), 0.5f, Game1.bigFont.MeasureString(Menu.menuName[j]).X * num8, 0f);
						}
						else
						{
							Game1.bigText.Color = new Color(0.5f, 0.5f, 0.5f, num5 * (1f - Menu.textAlpha / 100f));
							num8 = 0.8f;
						}
						Game1.bigText.DrawText(new Vector2(0f, vector.Y + (float)num9), Menu.menuName[j], num8, new Vector2(num8, num8), Game1.screenWidth, TextAlign.Center);
						if (j < Menu.menuName.Length - 1 && Menu.menuName[j + 1] != string.Empty)
						{
							num9 += 40;
						}
					}
					if (this.optionDesc != string.Empty)
					{
						Game1.smallText.Color = new Color(1f, 1f, 1f, (1f - Menu.fadeInAlpha) * num5);
						Game1.smallText.DrawButtonText(new Vector2(num3 + 40, num4 + num2 + 10), this.optionDesc, 1f, this.optionDescButtonList, bounce: true, 0f, TextAlign.Left);
					}
					if (Menu.savingAlpha > 0f)
					{
						this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.Black * Math.Min(Menu.savingAlpha * 6f, 0.75f));
						Game1.smallText.Color = color * Menu.savingAlpha * 6f;
						int num10 = (int)Math.Max(Game1.smallFont.MeasureString(Strings_Hud.Fidget_AutoSaving).X * 0.75f + 20f, 200f);
						Game1.hud.DrawMiniBorder(new Vector2(num3 + 350 - num10 / 2, num4 + 220), num10, 50, color * Menu.savingAlpha * 6f, 1f);
						Game1.smallText.DrawText(new Vector2(num3, num4 + 225), Strings_Hud.Fidget_AutoSaving, 1f, 700f, TextAlign.Center);
					}
					if (Menu.savingAlpha <= 0f && Menu.fileAlpha <= 0f)
					{
						num9 = 0;
						for (int k = 0; k < Menu.menuOptions; k++)
						{
							int num11 = (int)Game1.bigFont.MeasureString(Menu.menuName[k]).X;
							Vector2 vector2 = new Vector2(Game1.screenWidth / 2 - num11 / 2, vector.Y + (float)num9);
							if (k < Menu.menuOptions - 1 && Menu.menuName[k + 1] != string.Empty)
							{
								num9 += 40;
							}
							if (new Rectangle((int)vector2.X, (int)vector2.Y - 20, num11, 38).Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y))
							{
								if (this.curMenuOption != k)
								{
									this.curMenuOption = k;
									this.PopulateMenu();
									Menu.fadeInAlpha = 1f;
									Sound.PlayCue("menu_click");
								}
								this.curMenuOption = k;
								if (Game1.pcManager.leftMouseClicked)
								{
									Game1.pcManager.leftMouseClicked = false;
									Game1.hud.KeySelect = true;
									this.sprite.End();
									this.UpdateMainMenu(Game1.pManager);
									this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
								}
							}
						}
					}
				}
				if (Menu.fileAlpha > 0f)
				{
					this.DrawFileScreen();
				}
				if (this.menuMode == MenuMode.Loading || this.menuMode == MenuMode.Quitting)
				{
					this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, Menu.screenFade));
				}
			}
		}

		private void UpdatePauseMenu(ParticleManager pMan)
		{
			if (Menu.fileAlpha > 0f)
			{
				return;
			}
			int num = this.curMenuPage;
			if (this.CheckUnlockedGame(Game1.GamerServices && Guide.IsVisible && Game1.halfSecFrame == 10))
			{
				return;
			}
			if (Game1.hud.OpenMenuButton && this.curMenuPage != 10 && Game1.canDebug)
			{
				Sound.PlayCue("menu_confirm");
				Menu.prevMenuOption = this.curMenuOption;
				this.curMenuOption = 0;
				this.curMenuPage = 10;
				this.PopulateMenu();
			}
			if (Game1.hud.KeyLeft && this.curMenuPage != 11 && Game1.testDialogue)
			{
				Sound.PlayCue("menu_confirm");
				Menu.prevMenuOption = this.curMenuOption;
				this.curMenuOption = 0;
				this.curMenuPage = 11;
				this.PopulateMenu();
			}
			if (this.curMenuPage == 10)
			{
				if (Game1.hud.KeySelect)
				{
					if (this.curMenuOption == 0)
					{
						Game1.stats.UnlockMaps();
					}
					else if (this.curMenuOption == 1)
					{
						Game1.stats.XP = Game1.stats.nextLevelXP;
						Game1.stats.CheckLevelUp();
					}
					else if (this.curMenuOption == 2)
					{
						Game1.stats.Gold += 5000;
						Game1.hud.tempGold = Game1.stats.Gold;
					}
					else if (this.curMenuOption == 3)
					{
						Game1.stats.UnlockUpgrades();
					}
					else if (this.curMenuOption == 4)
					{
						Game1.invulnerable = !Game1.invulnerable;
					}
					else if (this.curMenuOption == 5)
					{
						Game1.stats.UnlockBluePrints(250);
					}
					else if (this.curMenuOption == 6)
					{
						Game1.stats.UnlockEquipment(250, unlockKeys: false);
						Game1.stats.UnlockBluePrints(250);
						Game1.stats.UnlockMaterials(250, shop: false);
						Game1.stats.UnlockMaterials(250, shop: true);
					}
					else if (this.curMenuOption == 7)
					{
						Game1.stats.UnlockFriends(relock: false);
					}
					else if (this.curMenuOption == 8)
					{
						for (int i = 0; i < Game1.dManager.destructWalls.Count; i++)
						{
							Game1.dManager.destructWalls[i].KillMe(pMan, this.character[0].Face);
						}
					}
					else if (this.curMenuOption == 9)
					{
						Game1.map.leftBlock = (Game1.map.rightBlock = 0f);
					}
					else if (this.curMenuOption == 10)
					{
						Game1.events.currentEvent += 10;
					}
					else if (this.curMenuOption == 11)
					{
						Game1.stats.UnlockQuestsNotes(complete: false);
					}
					else if (this.curMenuOption == 12)
					{
						this.TestCutscene(50);
					}
				}
				if (Game1.hud.KeyX)
				{
					if (this.curMenuOption == 0)
					{
						this.ClearMenu();
						Game1.stats.UnlockMaps();
						Game1.stats.lastSafeLoc = (this.character[0].Location = Vector2.Zero);
						Game1.navManager.InitWorldMap(this.map.path, _canReturn: false);
						this.character[0].Location = (this.character[0].PLoc = (Game1.stats.lastSafeLoc = new Vector2(200f, 200f)));
					}
					else if (this.curMenuOption == 1)
					{
						Game1.stats.ResetLevel();
					}
					else if (this.curMenuOption == 5)
					{
						Game1.stats.UnlockMaterials(10000, shop: false);
					}
					else if (this.curMenuOption == 6)
					{
						Game1.stats.UnlockEquipment(1, unlockKeys: true);
						Game1.stats.Equipment[305] = 250;
					}
					else if (this.curMenuOption == 7)
					{
						Game1.stats.UnlockFriends(relock: true);
					}
					else if (this.curMenuOption == 8)
					{
						for (int j = 0; j < Game1.dManager.destructWalls.Count; j++)
						{
							Game1.dManager.destructWalls[j].KillMe(pMan, this.character[0].Face);
						}
						foreach (KeyValuePair<string, RevealMap> item in Game1.navManager.RevealMap)
						{
							for (int k = 0; k < item.Value.DestructableList.Count; k++)
							{
								item.Value.DestructableList[k].Stage = 1;
							}
						}
					}
					else if (this.curMenuOption == 10)
					{
						Game1.events.currentEvent = Math.Max(Game1.events.currentEvent - 10, -1);
					}
					else if (this.curMenuOption == 11)
					{
						Game1.stats.UnlockQuestsNotes(complete: true);
					}
					else if (this.curMenuOption == 12)
					{
						this.TestCutscene(60);
					}
				}
				if (Game1.hud.KeyY)
				{
					if (this.curMenuOption == 0)
					{
						Game1.stats.ReLockMaps();
					}
					else if (this.curMenuOption == 2)
					{
						Game1.stats.Gold = (Game1.hud.tempGold = 0);
					}
					else if (this.curMenuOption == 3)
					{
						Game1.stats.RelockUpgrades();
					}
					else if (this.curMenuOption == 5)
					{
						Game1.stats.UnlockMaterials(10000, shop: true);
					}
					else if (this.curMenuOption == 6)
					{
						Game1.stats.ResetEquip();
					}
					else if (this.curMenuOption == 8)
					{
						foreach (KeyValuePair<string, RevealMap> item2 in Game1.navManager.RevealMap)
						{
							for (int l = 0; l < item2.Value.DestructableList.Count; l++)
							{
								item2.Value.DestructableList[l].Stage = 0;
							}
						}
					}
					if (this.curMenuOption == 10)
					{
						Game1.events.Reset();
						Game1.questManager.ResetQuests();
						for (int m = 0; m < Game1.stats.villagerDialogue.Length; m++)
						{
							Game1.stats.villagerDialogue[m] = -1;
						}
						Game1.events.currentEvent = 0;
					}
					else if (this.curMenuOption == 11)
					{
						Game1.questManager.ResetQuests();
					}
					else if (this.curMenuOption == 12)
					{
						this.TestCutscene(100);
					}
				}
				if (Game1.hud.KeySelect || Game1.hud.KeyX || Game1.hud.KeyY)
				{
					Sound.PlayCue("menu_confirm");
					Game1.hud.canInput = false;
					this.PopulateMenu();
				}
				if (Game1.hud.KeyLeftBumper)
				{
					if (this.curMenuOption == 10)
					{
						for (int n = 0; n < Game1.events.sideEventAvailable.Length; n++)
						{
							Game1.events.sideEventAvailable[n] = false;
						}
					}
					else if (this.curMenuOption == 12)
					{
						this.TestCutscene(120);
					}
					Game1.hud.KeyLeftBumper = false;
				}
				if (Game1.hud.KeyLeftTrigger && this.curMenuOption == 12)
				{
					Game1.cutscene.ExitCutscene();
					Music.Play("silent");
					this.ClearMenu();
					this.prompt = promptDialogue.None;
					this.ClearPrompt();
					Game1.InitCreditsScroll();
				}
				if (Game1.events.anyEvent)
				{
					this.ClearMenu();
					this.prompt = promptDialogue.None;
					this.ClearPrompt();
				}
			}
			else if (this.curMenuPage == 11 && Game1.hud.KeySelect)
			{
				int conversation = 0;
				CharacterType charType = CharacterType.Dust;
				switch (this.curMenuOption)
				{
					default:
						conversation = this.curMenuOption * 100;
						break;
					case 8:
						charType = CharacterType.Avgustin;
						break;
					case 9:
						charType = CharacterType.Bean;
						break;
					case 10:
						charType = CharacterType.Blop;
						break;
					case 11:
						charType = CharacterType.Bram;
						break;
					case 12:
						charType = CharacterType.Calum;
						break;
					case 13:
						charType = CharacterType.Colleen;
						break;
					case 14:
						charType = CharacterType.Corbin;
						break;
					case 15:
						charType = CharacterType.Elder;
						break;
					case 16:
						charType = CharacterType.Fale;
						break;
					case 17:
						charType = CharacterType.FloHop;
						break;
					case 18:
						charType = CharacterType.Geehan;
						break;
					case 19:
						charType = CharacterType.Gianni;
						break;
					case 20:
						charType = CharacterType.Ginger;
						break;
					case 21:
						charType = CharacterType.Haley;
						break;
					case 22:
						charType = CharacterType.Kane;
						break;
					case 23:
						charType = CharacterType.Lady;
						break;
					case 24:
						charType = CharacterType.MaMop;
						break;
					case 25:
						charType = CharacterType.Matti;
						break;
					case 26:
						charType = CharacterType.Moska;
						break;
					case 27:
						charType = CharacterType.OldGappy;
						break;
					case 28:
						charType = CharacterType.Oneida;
						break;
					case 29:
						charType = CharacterType.Reed;
						break;
					case 30:
						charType = CharacterType.Sanjin;
						break;
					case 31:
						charType = CharacterType.Sarahi;
						break;
					case 32:
						charType = CharacterType.ShopAurora;
						break;
					case 33:
						charType = CharacterType.ShopWild;
						break;
					case 34:
						charType = CharacterType.SmoBop;
						break;
					case 35:
						charType = CharacterType.Cora;
						break;
				}
				this.ClearMenu();
				this.prompt = promptDialogue.None;
				this.ClearPrompt();
				Game1.hud.InitDialogue(conversation, charType);
			}
			int num2 = 0;
			for (int num3 = 0; num3 < Menu.menuOptions; num3++)
			{
				if (Menu.menuName[num3] != string.Empty)
				{
					num2 = num3;
				}
			}
			if (Game1.hud.KeyUp && this.curMenuOption > 0)
			{
				Game1.hud.KeyUp = false;
				bool flag = false;
				do
				{
					flag = true;
					this.curMenuOption--;
				}
				while (this.curMenuOption > 0 && Menu.menuName[this.curMenuOption] == string.Empty);
				if (flag)
				{
					this.PopulateMenu();
					Menu.fadeInAlpha = 1f;
					Sound.PlayCue("menu_click");
				}
			}
			else if (Game1.hud.KeyDown && this.curMenuOption < num2)
			{
				Game1.hud.KeyDown = false;
				bool flag2 = false;
				do
				{
					flag2 = true;
					this.curMenuOption++;
				}
				while (this.curMenuOption < num2 && Menu.menuName[this.curMenuOption] == string.Empty);
				if (flag2)
				{
					this.PopulateMenu();
					Menu.fadeInAlpha = 1f;
					Sound.PlayCue("menu_click");
				}
			}
			if (Menu.KeyCancel)
			{
				Game1.hud.KeyCancel = true;
			}
			if (Game1.hud.KeyCancel)
			{
				Sound.PlayCue("menu_cancel");
				if (Game1.stats.playerLifeState != 0)
				{
					this.ClearMenu();
					this.prompt = promptDialogue.Dead;
					this.ClearPrompt();
				}
				else if (this.curMenuPage == 0)
				{
					this.ClearMenu();
					GC.Collect();
				}
				else
				{
					this.curMenuPage = 0;
					this.curMenuOption = Menu.prevMenuOption;
					this.PopulateMenu();
					this.SetMenuSelectMove();
				}
				Game1.hud.KeyCancel = (Menu.KeyCancel = false);
			}
			else if (Game1.hud.KeySelect)
			{
				Sound.PlayCue("menu_confirm");
				if (this.curMenuPage == 0)
				{
					Menu.prevMenuOption = this.curMenuOption;
				}
				Game1.hud.KeySelect = false;
				Game1.hud.canInput = false;
				if (this.curMenuPage == 0)
				{
					if (this.curMenuOption == 0)
					{
						if (Game1.stats.playerLifeState != 0)
						{
							this.ClearMenu();
							this.prompt = promptDialogue.Dead;
							this.ClearPrompt();
						}
						else
						{
							this.ClearMenu();
							GC.Collect();
						}
					}
					else if (this.curMenuOption == 1 && Menu.trialOffset == 0)
					{
						Game1.awardsManager.InitPurchase();
					}
					else if (this.curMenuOption + Menu.trialOffset == 2)
					{
						this.curMenuPage = 1;
						this.curMenuOption = 0;
					}
					else if (this.curMenuOption + Menu.trialOffset == 3)
					{
						if (!Game1.GamerServices || !Game1.IsTrial)
						{
							this.InitFileManager();
						}
					}
					else if (this.curMenuOption + Menu.trialOffset == 4)
					{
						Game1.awardsManager.InitLeaderBoard(LeaderBoardFilter.Completion, -1);
					}
					else if (this.curMenuOption + Menu.trialOffset == 5)
					{
						if (Game1.isPCBuild)
						{
							this.InitPCHighScores();
						}
						else
						{
							Game1.awardsManager.ViewAchievements();
						}
					}
					else if (this.curMenuOption + Menu.trialOffset == 6)
					{
						this.curMenuPage = 2;
						this.curMenuOption = 0;
					}
				}
				else if (this.curMenuPage == 1)
				{
					if (this.curMenuOption == 0)
					{
						this.InitHelp();
					}
					if (this.curMenuOption == 1)
					{
						this.InitHelp();
					}
					else if (this.curMenuOption == 2)
					{
						this.InitSettings();
					}
					else if (this.curMenuOption == 3)
					{
						this.InitHUDAdjust();
					}
					else if (this.curMenuOption == 4)
					{
						this.InitCredits();
					}
					else if (this.curMenuOption == 5)
					{
						this.curMenuPage = 0;
						this.curMenuOption = 1 + (1 - Menu.trialOffset);
					}
				}
				else if (this.curMenuPage == 2)
				{
					if (this.curMenuOption == 0)
					{
						this.menuMode = MenuMode.Quitting;
						if (Game1.isPCBuild)
						{
							Game1.storage.Write(0, -1);
							Game1.storage.DisposeContainer();
						}
						Menu.titleFade = 1f;
					}
					else if (this.curMenuOption == 1)
					{
						this.curMenuPage = 0;
						this.curMenuOption = 5 + (1 - Menu.trialOffset);
					}
				}
				if (this.menuMode == MenuMode.None)
				{
					this.PopulateMenu();
				}
				this.SetMenuSelectMove();
			}
			if (num != this.curMenuPage)
			{
				Menu.textAlpha = 100f;
				Menu.fadeInAlpha = 1f;
			}
		}

		public void DrawPauseMenu(float alpha)
		{
			if (this.prompt == promptDialogue.SkipEvent)
			{
				this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, 0.5f * alpha));
			}
			Game1.pManager.DrawHudParticles(this.particlesTex, 1f, 9);
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			if (this.menuMode == MenuMode.None)
			{
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
				{
					this.DrawPauseMenuPC(alpha);
					return;
				}
				int num = Game1.screenWidth / 2 - 350;
				int num2 = Game1.screenHeight / 2 - 220 - (int)(0.035 - (double)Game1.hud.pauseImageOffset * 0.035);
				int num3 = 60;
				if (this.optionDesc != string.Empty)
				{
					num3 += (int)Game1.smallFont.MeasureString(this.optionDesc).Y;
				}
				int num4 = Math.Max(270 + num3, 0);
				Menu.movingBottomEdge = Math.Max(Menu.movingBottomEdge + ((float)num4 - Menu.movingBottomEdge) * Game1.HudTime * 12f, 360f);
				int num5 = num2;
				if (Menu.movingBottomEdge + (float)num3 > (float)Game1.screenHeight * 0.75f)
				{
					num5 -= (int)(Menu.movingBottomEdge + (float)num3 - (float)Game1.screenHeight * 0.75f);
				}
				num2 = (int)((float)num2 + (float)(num5 - num2) * Game1.HudTime * 12f);
				Vector2 vector = new Vector2(num, num2 + 120);
				Color color = new Color(1f, 1f, 1f, alpha);
				Game1.hud.DrawBorder(new Vector2(num, num2), 700, (int)Menu.movingBottomEdge, color, 0.9f, 255);
				Game1.smallText.Color = color;
				float size = 1f;
				int num6 = (int)(Game1.smallFont.MeasureString(this.buttonText).X * 0.75f) + 20;
				Game1.hud.DrawMiniBorder(new Vector2(num + 350 - num6 / 2, (float)num2 + Menu.movingBottomEdge - 15f), num6, 30, color, 1f);
				Game1.smallText.DrawButtonText(new Vector2(num, num2 + (int)Menu.movingBottomEdge - 10), this.buttonText, 0.75f, this.buttonTextButtonList, bounce: false, 700f, TextAlign.Center);
				Color color2 = new Color(0f, 0f, 0f, 0.5f * alpha);
				this.sprite.Draw(this.hudTex[1], new Vector2(Game1.screenWidth / 2, num2 + 68), new Rectangle(887, 20, 234, 180), color2, 0f, Vector2.Zero, new Vector2(1.2f, 1f), SpriteEffects.None, 0f);
				this.sprite.Draw(this.hudTex[1], new Vector2(Game1.screenWidth / 2, num2 + 68), new Rectangle(887, 20, 234, 180), color2, 0f, new Vector2(234f, 0f), new Vector2(1.2f, 1f), SpriteEffects.FlipHorizontally, 0f);
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				int num7 = 0;
				for (int i = 0; i < Menu.menuOptions; i++)
				{
					if (Menu.menuName[i] != string.Empty)
					{
						dictionary.Add(num7, i);
						num7++;
					}
				}
				Game1.bigText.Color = new Color(1f, 1f, 1f, alpha * (1f - Menu.textAlpha / 100f));
				Game1.bigText.DrawOutlineText(new Vector2(num, num2 + 15), Menu.menuTitle, size, 700, TextAlign.Center, fullOutline: true);
				int num8 = 0;
				for (int j = 0; j < this.curMenuOption; j++)
				{
					if (Menu.menuName[j] != string.Empty)
					{
						num8++;
					}
				}
				foreach (KeyValuePair<int, int> item in dictionary)
				{
					float num9 = MathHelper.Clamp(1f / ((float)Math.Abs(num8 - item.Key) * 1.5f), 0f, 1f);
					if (num8 == item.Key)
					{
						Game1.bigText.Color = new Color(1f, 1f, 1f, alpha * (1f - Menu.textAlpha / 100f));
						size = 1.2f;
						this.DrawCursors(new Vector2((float)(Game1.screenWidth / 2 + 100) - Game1.bigFont.MeasureString(Menu.menuName[item.Value]).X / 2f * size, (int)vector.Y + 25), 0.5f, Game1.bigFont.MeasureString(Menu.menuName[item.Value]).X * size, 1f - Menu.textAlpha / 100f);
					}
					else
					{
						Game1.bigText.Color = new Color(0.5f, 0.5f, 0.5f, alpha * num9 * (1f - Menu.textAlpha / 100f));
						size = 1f;
					}
					if (Math.Abs(num8 - item.Key) < 3)
					{
						Game1.bigText.DrawText(new Vector2(0f, vector.Y + 30f + (float)(item.Key * 30) - Menu.menuSelectMove), Menu.menuName[item.Value], size, new Vector2(size, size * num9), Game1.screenWidth, TextAlign.Center);
					}
				}
				if (this.optionDesc != string.Empty)
				{
					Game1.smallText.Color = new Color(1f, 1f, 1f, (1f - Menu.fadeInAlpha) * alpha);
					Game1.smallText.DrawButtonText(new Vector2(num + 40, num2 + 270), this.optionDesc, 1f, this.optionDescButtonList, bounce: true, 0f, TextAlign.Left);
				}
				this.sprite.Draw(this.hudTex[2], new Vector2(Game1.screenWidth / 2, num2 + 60), new Rectangle(0, 502, 326, 18), color, 0f, new Vector2(163f, 0f), new Vector2(1.5f, 1f), SpriteEffects.None, 0f);
				if (Menu.savingAlpha > 0f)
				{
					this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.Black * Math.Min(Menu.savingAlpha * 6f, 0.75f));
					Game1.smallText.Color = color * Menu.savingAlpha * 6f;
					int num10 = (int)Math.Max(Game1.smallFont.MeasureString(Strings_Hud.Fidget_AutoSaving).X * 0.75f + 20f, 200f);
					Game1.hud.DrawMiniBorder(new Vector2(num + 350 - num10 / 2, num2 + 220), num10, 50, color * Menu.savingAlpha * 6f, 1f);
					Game1.smallText.DrawText(new Vector2(num, num2 + 225), Strings_Hud.Fidget_AutoSaving, 1f, 700f, TextAlign.Center);
				}
			}
			else if (this.menuMode == MenuMode.Help)
			{
				this.DrawHelp();
			}
			else if (this.menuMode == MenuMode.Credits)
			{
				this.DrawCredits(1f - Menu.fadeInAlpha);
			}
			else if (this.menuMode == MenuMode.Settings)
			{
				this.DrawSettings();
			}
			else if (this.menuMode == MenuMode.HudAdjust)
			{
				this.DrawHudAdjust();
			}
			else if (this.menuMode == MenuMode.LeaderBoards)
			{
				Game1.awardsManager.DrawLeaderBoard(this.sprite, this.hudTex, Vector2.Zero);
			}
			else if (this.menuMode == MenuMode.PCHighScores)
			{
				this.DrawPCHighScores();
			}
			if (Menu.fileAlpha > 0f)
			{
				this.DrawFileScreen();
			}
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				Game1.hud.mousePos = Game1.pcManager.DrawCursor(this.sprite, 0.8f, Color.White);
			}
		}

		public void DrawPauseMenuPC(float alpha)
		{
			int num = 0;
			for (int i = 0; i < Menu.menuOptions; i++)
			{
				if (Menu.menuName[i] != string.Empty)
				{
					num++;
				}
			}
			int num2 = num * 40 + 130;
			int num3 = Game1.screenWidth / 2 - 350;
			int num4 = Game1.screenHeight / 2 - 220 - (int)(0.035 - (double)Game1.hud.pauseImageOffset * 0.035);
			int num5 = 60;
			if (this.optionDesc != string.Empty)
			{
				num5 += (int)Game1.smallFont.MeasureString(this.optionDesc).Y;
			}
			int num6 = Math.Max(num2 + num5, 0);
			Menu.movingBottomEdge = Math.Max(Menu.movingBottomEdge + ((float)num6 - Menu.movingBottomEdge) * Game1.HudTime * 12f, 300f);
			Vector2 vector = new Vector2(num3, num4 + 110);
			Color color = new Color(1f, 1f, 1f, alpha);
			Game1.hud.DrawBorder(new Vector2(num3, num4), 700, (int)Menu.movingBottomEdge, color, 0.9f, num2);
			Menu.KeyCancel = false;
			if (Game1.menu.menuMode == MenuMode.None && (Game1.cManager.challengeMode == ChallengeManager.ChallengeMode.None || Game1.cManager.challengeMode == ChallengeManager.ChallengeMode.InChallenge) && Game1.stats.playerLifeState == 0 && Game1.pcManager.DrawMouseButton(new Vector2(num3 + 677, num4 - 30), 0.8f, color, 0, draw: true) && Game1.stats.playerLifeState == 0)
			{
				this.ClearMenu();
				this.prompt = promptDialogue.None;
				Sound.PlayCue("menu_cancel");
				GC.Collect();
				return;
			}
			Game1.smallText.Color = color;
			float size = 1f;
			Color color2 = new Color(0f, 0f, 0f, 0.5f * alpha);
			this.sprite.Draw(this.hudTex[1], new Vector2(Game1.screenWidth / 2, num4 + 68), new Rectangle(887, 20, 234, 180), color2, 0f, Vector2.Zero, new Vector2(1.2f, 1f), SpriteEffects.None, 0f);
			this.sprite.Draw(this.hudTex[1], new Vector2(Game1.screenWidth / 2, num4 + 68), new Rectangle(887, 20, 234, 180), color2, 0f, new Vector2(234f, 0f), new Vector2(1.2f, 1f), SpriteEffects.FlipHorizontally, 0f);
			Game1.bigText.Color = new Color(1f, 1f, 1f, alpha * (1f - Menu.textAlpha / 100f));
			Game1.bigText.DrawOutlineText(new Vector2(num3, num4 + 15), Menu.menuTitle, size, 700, TextAlign.Center, fullOutline: true);
			int num7 = 0;
			for (int j = 0; j < Menu.menuOptions; j++)
			{
				if (this.curMenuOption == j)
				{
					Game1.bigText.Color = new Color(1f, 1f, 1f, alpha * (1f - Menu.textAlpha / 100f));
					size = 1f;
					this.DrawCursors(new Vector2((float)(Game1.screenWidth / 2 + 100) - Game1.bigFont.MeasureString(Menu.menuName[j]).X / 2f * size, (int)vector.Y + 25), 0.5f, Game1.bigFont.MeasureString(Menu.menuName[j]).X * size, 0f);
				}
				else
				{
					Game1.bigText.Color = new Color(0.5f, 0.5f, 0.5f, alpha * (1f - Menu.textAlpha / 100f));
					size = 0.8f;
				}
				Game1.bigText.DrawText(new Vector2(0f, vector.Y + (float)num7), Menu.menuName[j], size, new Vector2(size, size), Game1.screenWidth, TextAlign.Center);
				if (j < Menu.menuName.Length - 1 && Menu.menuName[j + 1] != string.Empty)
				{
					num7 += 40;
				}
			}
			if (this.optionDesc != string.Empty)
			{
				Game1.smallText.Color = new Color(1f, 1f, 1f, (1f - Menu.fadeInAlpha) * alpha);
				Game1.smallText.DrawButtonText(new Vector2(num3 + 40, num4 + num2 + 20), this.optionDesc, 1f, this.optionDescButtonList, bounce: true, 0f, TextAlign.Left);
			}
			this.sprite.Draw(this.hudTex[2], new Vector2(Game1.screenWidth / 2, num4 + 60), new Rectangle(0, 502, 326, 18), color, 0f, new Vector2(163f, 0f), new Vector2(1.5f, 1f), SpriteEffects.None, 0f);
			if (Menu.savingAlpha > 0f)
			{
				this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.Black * Math.Min(Menu.savingAlpha * 6f, 0.75f));
				Game1.smallText.Color = color * Menu.savingAlpha * 6f;
				int num8 = (int)Math.Max(Game1.smallFont.MeasureString(Strings_Hud.Fidget_AutoSaving).X * 0.75f + 20f, 200f);
				Game1.hud.DrawMiniBorder(new Vector2(num3 + 350 - num8 / 2, num4 + 220), num8, 50, color * Menu.savingAlpha * 6f, 1f);
				Game1.smallText.DrawText(new Vector2(num3, num4 + 225), Strings_Hud.Fidget_AutoSaving, 1f, 700f, TextAlign.Center);
			}
			if (Menu.fileAlpha > 0f)
			{
				this.DrawFileScreen();
			}
			if (Menu.savingAlpha <= 0f)
			{
				num7 = 0;
				for (int k = 0; k < Menu.menuOptions; k++)
				{
					int num9 = (int)Game1.bigFont.MeasureString(Menu.menuName[k]).X;
					Vector2 vector2 = new Vector2(Game1.screenWidth / 2 - num9 / 2, vector.Y + (float)num7);
					if (k < Menu.menuOptions - 1 && Menu.menuName[k + 1] != string.Empty)
					{
						num7 += 40;
					}
					if (!new Rectangle((int)vector2.X, (int)vector2.Y - 20, num9, 38).Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y))
					{
						continue;
					}
					if (this.curMenuOption != k)
					{
						this.curMenuOption = k;
						this.PopulateMenu();
						Menu.fadeInAlpha = 1f;
						Sound.PlayCue("menu_click");
					}
					this.curMenuOption = k;
					if (Game1.pcManager.leftMouseClicked)
					{
						Game1.pcManager.leftMouseClicked = false;
						Game1.hud.KeySelect = true;
						this.sprite.End();
						if (this.prompt == promptDialogue.None)
						{
							this.UpdatePauseMenu(Game1.pManager);
						}
						else if (Game1.hud.inventoryState == InventoryState.None)
						{
							this.UpdatePrompt();
						}
						else
						{
							Game1.hud.UpdateCharacter(Game1.pManager);
						}
						this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
					}
				}
			}
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				Game1.hud.mousePos = Game1.pcManager.DrawCursor(this.sprite, 0.8f, Color.White);
			}
		}

		public void PopulatePrompt()
		{
			for (int i = 0; i < Menu.menuOptions; i++)
			{
				Menu.menuName[i] = string.Empty;
			}
			Menu.menuTitle = string.Empty;
			this.optionDesc = string.Empty;
			string text = string.Empty;
			string s = string.Empty;
			Menu.KeyCancel = false;
			switch (this.prompt)
			{
				case promptDialogue.Save:
					{
						text = Strings_Prompts.SaveTitle;
						for (int num3 = 0; num3 < 3; num3++)
						{
							Menu.menuName[num3] = Strings_Prompts.ResourceManager.GetString("Save" + num3);
						}
						switch (this.curMenuOption)
						{
							case 0:
								s = Strings_Prompts.Save0Desc;
								break;
							case 1:
								s = Strings_Prompts.Save1Desc;
								break;
							case 2:
								s = ((Game1.events.currentEvent >= 70) ? ((Game1.stats.Equipment[300] >= 1) ? Strings_Prompts.Save2Desc3 : Strings_Prompts.Save2Desc2) : Strings_Prompts.Save2Desc1);
								break;
						}
						break;
					}
				case promptDialogue.SaveSuccess:
					text = Strings_Prompts.SaveSuccessTitle;
					Menu.menuName[0] = Strings_Prompts.SaveSuccess;
					s = Strings_Prompts.SaveSuccessDesc;
					break;
				case promptDialogue.Failed:
					{
						if (this.curMenuOption > 1)
						{
							this.curMenuOption = 0;
							this.SetMenuSelectMove();
						}
						text = Strings_Prompts.FailTitle;
						for (int num6 = 0; num6 < 2; num6++)
						{
							Menu.menuName[num6] = Strings_Prompts.ResourceManager.GetString("Fail" + num6);
						}
						s = Strings_Prompts.ResourceManager.GetString("Fail" + this.curMenuOption + "Desc");
						break;
					}
				case promptDialogue.StorageDisconnected:
					{
						text = Strings_Prompts.DisconnectTitle;
						for (int num2 = 0; num2 < 2; num2++)
						{
							Menu.menuName[num2] = Strings_Prompts.ResourceManager.GetString("Fail" + num2);
						}
						s = Strings_Prompts.ResourceManager.GetString("Disconnect" + this.curMenuOption + "Desc");
						break;
					}
				case promptDialogue.StorageFull:
					{
						text = Strings_Prompts.StorageFullTitle;
						for (int n = 0; n < 2; n++)
						{
							Menu.menuName[n] = Strings_Prompts.ResourceManager.GetString("Fail" + n);
						}
						switch (this.curMenuOption)
						{
							case 0:
								s = Strings_Prompts.StorageFullDesc;
								break;
							case 1:
								s = Strings_Prompts.Fail1Desc;
								break;
						}
						break;
					}
				case promptDialogue.Teleport:
					{
						text = Strings_Prompts.TeleportTitle;
						for (int l = 0; l < 2; l++)
						{
							Menu.menuName[l] = Strings_Prompts.ResourceManager.GetString("Save" + l);
						}
						switch (this.curMenuOption)
						{
							case 0:
								s = Strings_Prompts.Teleport0Desc;
								break;
							case 1:
								s = Strings_Prompts.Teleport1Desc;
								break;
						}
						break;
					}
				case promptDialogue.Dead:
					{
						text = Strings_Prompts.DeadTitle;
						for (int k = 0; k < 2; k++)
						{
							Menu.menuName[k] = Strings_Prompts.ResourceManager.GetString("Dead" + k);
						}
						switch (this.curMenuOption)
						{
							case 0:
								s = Strings_Prompts.Dead0Desc;
								break;
							case 1:
								s = Strings_Prompts.Dead1Desc;
								break;
						}
						break;
					}
				case promptDialogue.SkipEvent:
					{
						text = Strings_Prompts.SkipTitle;
						int num4 = 0;
						if (Game1.events.skippable == SkipType.Unskippable || (Game1.events.skippable == SkipType.DialogueOnly && Game1.hud.dialogueState == DialogueState.Inactive && Game1.cutscene.SceneType != CutsceneType.Video))
						{
							num4 = 1;
						}
						for (int num5 = num4; num5 < 3; num5++)
						{
							Menu.menuName[num5 - num4] = Strings_Prompts.ResourceManager.GetString("Skip" + num5);
						}
						s = ((this.curMenuOption != -num4) ? ((this.curMenuOption >= 2 - num4) ? ((!Game1.settings.AutoAdvance) ? Strings_Prompts.ResourceManager.GetString("Skip2Desc1") : Strings_Prompts.ResourceManager.GetString("Skip2Desc0")) : Strings_Prompts.ResourceManager.GetString("Skip1Desc")) : ((Game1.hud.dialogueState != 0) ? Strings_Prompts.ResourceManager.GetString("Skip0Desc1") : Strings_Prompts.ResourceManager.GetString("Skip0Desc0")));
						break;
					}
				case promptDialogue.SignedOut:
					text = Strings_Prompts.SignOut;
					Menu.menuName[0] = Strings_Prompts.SaveSuccess;
					s = Strings_Prompts.SignOutDesc;
					break;
				case promptDialogue.FileCorrupt:
					text = Strings_Prompts.FileCorruptTitle;
					Menu.menuName[0] = Strings_Prompts.SaveSuccess;
					s = Strings_Prompts.FileCorruptDesc;
					break;
				case promptDialogue.LiveDisconnected:
					text = Strings_Prompts.DisconnectedLiveTitle;
					Menu.menuName[0] = Strings_Prompts.SaveSuccess;
					s = Strings_Prompts.DisconnectedLiveDesc;
					break;
				case promptDialogue.UpgradeComplete:
					{
						text = Strings_Prompts.UpgradeCompTitle;
						for (int num = 0; num < 2; num++)
						{
							Menu.menuName[num] = Strings_Prompts.ResourceManager.GetString("Save" + num);
						}
						s = Strings_Prompts.UpgradeCompDesc;
						break;
					}
				case promptDialogue.UpgradeIncomplete:
					{
						text = Strings_Prompts.UpgradeIncompTitle;
						for (int m = 0; m < 2; m++)
						{
							Menu.menuName[m] = Strings_Prompts.ResourceManager.GetString("Save" + m);
						}
						s = Strings_Prompts.UpgradeIncompDesc;
						break;
					}
				case promptDialogue.PurchaseAchievement:
					{
						Game1.hud.ExitInventory();
						text = Strings_Prompts.PurchaseAchievementTitle;
						for (int j = 0; j < 2; j++)
						{
							Menu.menuName[j] = Strings_Prompts.ResourceManager.GetString("Purchase" + j);
						}
						s = Strings_Prompts.PurchaseAchievementDesc;
						break;
					}
				case promptDialogue.QueuedAchievement:
					Game1.hud.ExitInventory();
					text = "Achievement";
					Menu.menuName[0] = Strings_Prompts.SaveSuccess;
					s = "You would have earned an Achievement here";
					break;
			}
			Menu.menuTitle = text;
			this.optionDesc = Game1.smallText.WordWrap(s, 1f, 620f, this.optionDescButtonList, TextAlign.Center);
			this.buttonText = Game1.smallText.WordWrap(Strings_PauseMenu.PauseControls, 0.75f, 620f, this.buttonTextButtonList, TextAlign.Left);
		}

		public int UpdatePrompt()
		{
			Game1.BlurScene(1f);
			int result = -1;
			int num = this.curMenuPage;
			if (this.prompt == promptDialogue.None)
			{
				return result;
			}
			this.UpdateTimers();
			if (this.menuMode == MenuMode.FileManage)
			{
				this.UpdateFileScreen(Game1.pManager);
				return 0;
			}
			if (Menu.checkGuideTime == 0f)
			{
				if (this.prompt == promptDialogue.SignedOut)
				{
					for (int i = 0; i < 4; i++)
					{
						if (GamePad.GetState((PlayerIndex)i).Buttons.A == ButtonState.Pressed)
						{
							Game1.hud.KeySelect = true;
						}
					}
				}
				if (Game1.hud.KeyUp && this.curMenuOption > 0)
				{
					Game1.hud.KeyUp = false;
					Sound.PlayCue("menu_click");
					this.curMenuOption--;
					Menu.fadeInAlpha = 1f;
					this.PopulatePrompt();
				}
				else if (Game1.hud.KeyDown && this.curMenuOption < Menu.menuOptions - 1)
				{
					Game1.hud.KeyDown = false;
					if (Menu.menuName[this.curMenuOption + 1] != string.Empty)
					{
						Sound.PlayCue("menu_click");
						this.curMenuOption++;
						Menu.fadeInAlpha = 1f;
						this.PopulatePrompt();
					}
				}
				if (Menu.KeyCancel)
				{
					Game1.hud.KeyCancel = true;
				}
				if (Game1.hud.KeyCancel)
				{
					Game1.hud.KeyCancel = (Menu.KeyCancel = false);
					if (this.prompt != promptDialogue.SignedOut)
					{
						Game1.hud.canInput = false;
						Sound.PlayCue("menu_cancel");
						this.curMenuOption = 0;
						if (this.prompt == promptDialogue.StorageDisconnected)
						{
							Game1.storage.device = null;
						}
						if (this.prompt == promptDialogue.SkipEvent)
						{
							VoiceManager.ResumeVoice();
						}
						this.prompt = promptDialogue.None;
						if (Game1.stats.playerLifeState != 0)
						{
							Game1.hud.Pause();
						}
						result = 0;
						GC.Collect();
					}
					else
					{
						Menu.titleFade = 1f;
						this.QuitGame(Game1.pManager);
						Game1.hud.KeySelect = false;
						this.prompt = promptDialogue.None;
					}
				}
				else if (Game1.hud.KeySelect)
				{
					Game1.hud.KeySelect = false;
					Sound.PlayCue("menu_confirm");
					Menu.prevMenuOption = this.curMenuOption;
					Game1.hud.canInput = false;
					if (this.prompt == promptDialogue.Save)
					{
						if (this.curMenuOption == 0)
						{
							this.InitFileManager();
							this.tempSaveSlot = Math.Max(Game1.stats.manualSaveSlot, (byte)1);
						}
						else if (this.curMenuOption == 1)
						{
							this.prompt = promptDialogue.None;
							this.curMenuOption = 0;
						}
						else if (this.curMenuOption == 2)
						{
							if (Game1.events.currentEvent >= 70 && Game1.stats.Equipment[300] > 0)
							{
								this.prompt = promptDialogue.Teleport;
								this.curMenuOption = 0;
							}
							else
							{
								Sound.PlayCue("shop_fail");
								Menu.textAlpha = 0f;
								Menu.fadeInAlpha = 0f;
							}
						}
					}
					else if (this.prompt == promptDialogue.SaveSuccess)
					{
						this.prompt = promptDialogue.None;
						this.curMenuOption = 0;
					}
					else if (this.prompt == promptDialogue.Failed || this.prompt == promptDialogue.FileCorrupt || this.prompt == promptDialogue.StorageDisconnected || this.prompt == promptDialogue.StorageFull)
					{
						if (this.curMenuOption == 0)
						{
							Game1.storage.GetDevice(LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One));
							this.prompt = promptDialogue.None;
							this.menuMode = MenuMode.None;
							Game1.gameMode = Game1.GameModes.Game;
							Game1.hud.isPaused = false;
						}
						else if (this.curMenuOption == 1)
						{
							Game1.storage.device = null;
							this.prompt = promptDialogue.None;
						}
						this.curMenuOption = 0;
					}
					else if (this.prompt == promptDialogue.LiveDisconnected)
					{
						this.prompt = promptDialogue.None;
						this.curMenuOption = 0;
					}
					else if (this.prompt == promptDialogue.Teleport)
					{
						if (this.curMenuOption == 0)
						{
							Game1.stats.Equipment[300]--;
							this.prompt = promptDialogue.None;
							this.curMenuOption = 0;
							string path = "worldmap";
							path = this.map.CheckMapName(path);
							if (path == "worldmap")
							{
								Game1.stats.lastSafeLoc = (this.character[0].Location = Vector2.Zero);
								Game1.navManager.InitWorldMap(this.map.path, _canReturn: false);
							}
							else
							{
								this.map.SwitchMap(Game1.pManager, Game1.character, path, loading: false);
							}
						}
						else if (this.curMenuOption == 1)
						{
							this.prompt = promptDialogue.Save;
							this.curMenuOption = 2;
						}
					}
					else if (this.prompt == promptDialogue.Dead)
					{
						if (this.curMenuOption == 0)
						{
							this.prompt = promptDialogue.None;
							Game1.hud.Pause();
							Game1.stats.saveSlot = 0;
							this.InitFileManager();
						}
						else if (this.curMenuOption == 1)
						{
							this.prompt = promptDialogue.None;
							Game1.hud.Pause();
							this.curMenuOption = 0;
						}
					}
					else if (this.prompt == promptDialogue.SkipEvent)
					{
						int num2 = 0;
						if (Game1.events.skippable == SkipType.Unskippable || (Game1.events.skippable == SkipType.DialogueOnly && Game1.hud.dialogueState == DialogueState.Inactive && Game1.cutscene.SceneType != CutsceneType.Video))
						{
							num2 = 1;
						}
						if (this.curMenuOption == -num2)
						{
							this.prompt = promptDialogue.None;
							if (Game1.events.skippable == SkipType.Skippable)
							{
								Game1.events.ClearEvent();
							}
							else if (Game1.events.skippable == SkipType.DialogueOnly)
							{
								if (Game1.hud.dialogueState == DialogueState.Active)
								{
									Game1.hud.dialogue.InitExit();
								}
								else
								{
									Game1.hud.DialogueNull(-1, -1);
								}
							}
							VoiceManager.StopVoiceCue();
							Game1.cutscene.ExitCutscene();
							Game1.hud.canInput = false;
							Game1.hud.LimitInput();
						}
						else if (this.curMenuOption == 1 - num2)
						{
							this.prompt = promptDialogue.None;
						}
						else if (this.curMenuOption == 2 - num2)
						{
							Game1.settings.AutoAdvance = !Game1.settings.AutoAdvance;
						}
					}
					else if (this.prompt == promptDialogue.SignedOut)
					{
						Menu.titleFade = 1f;
						this.QuitGame(Game1.pManager);
						Game1.hud.KeySelect = false;
						this.prompt = promptDialogue.None;
					}
					else if (this.prompt == promptDialogue.UpgradeComplete)
					{
						this.prompt = promptDialogue.None;
						result = ((this.curMenuOption == 0) ? 1 : 0);
					}
					else if (this.prompt == promptDialogue.UpgradeIncomplete)
					{
						this.prompt = promptDialogue.None;
						result = ((this.curMenuOption == 0) ? 2 : 0);
					}
					else if (!Game1.GamerServices || Game1.IsTrial)
					{
						if (this.prompt == promptDialogue.PurchaseAchievement)
						{
							this.prompt = promptDialogue.None;
							if (this.curMenuOption == 0)
							{
								Game1.awardsManager.InitPurchase();
							}
						}
						this.curMenuOption = 0;
					}
					if (this.prompt == promptDialogue.QueuedAchievement)
					{
						this.prompt = promptDialogue.None;
						this.curMenuOption = 0;
					}
					if (this.prompt != promptDialogue.None)
					{
						this.PopulatePrompt();
					}
					this.SetMenuSelectMove();
				}
			}
			if (num != this.curMenuPage)
			{
				Menu.textAlpha = 100f;
				Menu.fadeInAlpha = 1f;
			}
			return result;
		}

		private void InitHelp()
		{
			this.menuMode = MenuMode.Help;
			Menu.helpLoaded = false;
			Menu.scrollMask = new RenderTarget2D(Game1.graphics.GraphicsDevice, (int)Menu.windowWidth, Menu.bottomEdge - Menu.topEdge - 80);
			Menu.scrollSource = new RenderTarget2D(Game1.graphics.GraphicsDevice, (int)Menu.windowWidth, Menu.bottomEdge - Menu.topEdge - 80);
			this.SetHelpTextures();
			this.DrawScrollMask(Menu.scrollMask.Width, Menu.scrollMask.Height, 60, Game1.graphics.GraphicsDevice);
			if (this.curMenuOption > 0)
			{
				Menu.manualContents = false;
			}
			else
			{
				Menu.manualContents = true;
			}
			this.curMenuOption = 0;
			Menu.manualContentsPos = 0f;
		}

		private void UnloadHelp()
		{
			Game1.GetLargeContent().Unload();
			Menu.helpLoaded = false;
			Game1.hud.ExitShop();
			Menu.scrollSource = null;
			this.scrollSourceTexture = null;
			Menu.scrollMask = null;
			this.scrollMaskTexture = null;
			this.scrollBookMarks.Clear();
			this.scrollImages.Clear();
			this.scrollString.Clear();
			Menu.listScroll = 0f;
			if (Game1.gameMode == Game1.GameModes.MainMenu)
			{
				this.ExitMode(3, 0);
			}
			else
			{
				this.ExitMode(1, 0);
			}
		}

		private void PopulateHelp(int width)
		{
			this.menuMode = MenuMode.None;
			if (Menu.manualContents)
			{
				this.buttonText = Game1.smallText.WordWrap(Strings_Manual.ContentControls, 0.8f, width, this.buttonTextButtonList, TextAlign.Left);
			}
			else
			{
				this.buttonText = Game1.smallText.WordWrap(Strings_Manual.Controls, 0.8f, width, this.buttonTextButtonList, TextAlign.Left);
			}
			this.menuMode = MenuMode.Help;
		}

		private void PopulateScroll(int width)
		{
			float num = 0.8f;
			float imageSize = 1f;
			if (Game1.standardDef)
			{
				imageSize = 0.7f;
			}
			this.scrollString = Game1.smallText.scrollWrap(Strings_Manual.Manual, num, width - 30, this.optionDescButtonList, this.scrollImages, this.scrollBookMarks, imageSize);
			Menu.maxScroll = (float)(-Game1.screenHeight) * 0.5f;
			foreach (KeyValuePair<float, string> item in this.scrollString)
			{
				Menu.maxScroll += Game1.smallFont.MeasureString(item.Value).Y * num;
			}
			Menu.listScroll = 0f;
		}

		private void UpdateHelp(float textSize)
		{
			if (!Menu.helpLoaded)
			{
				return;
			}
			if (Menu.manualContents)
			{
				if (Game1.hud.KeyCancel)
				{
					Sound.PlayCue("menu_cancel");
					Menu.manualContents = !Menu.manualContents;
					this.PopulateHelp((int)Menu.windowWidth);
				}
				else if (Game1.hud.KeySelect)
				{
					foreach (KeyValuePair<Vector2, string> scrollBookMark in this.scrollBookMarks)
					{
						if ((float)this.curMenuOption == scrollBookMark.Key.X)
						{
							Menu.listScroll = 0f - scrollBookMark.Key.Y;
						}
					}
					Sound.PlayCue("menu_confirm");
					Game1.hud.KeySelect = false;
					Game1.hud.canInput = false;
					Menu.manualContents = !Menu.manualContents;
					this.PopulateHelp((int)Menu.windowWidth);
				}
				if (Game1.hud.KeyUp && this.curMenuOption > 0)
				{
					Game1.hud.KeyUp = false;
					this.curMenuOption--;
					Sound.PlayCue("menu_click");
				}
				if (Game1.hud.KeyDown && this.curMenuOption < this.scrollBookMarks.Count - 1)
				{
					Game1.hud.KeyDown = false;
					this.curMenuOption++;
					Sound.PlayCue("menu_click");
				}
				Menu.manualContentsPos += Game1.HudTime * 6000f * (1f - Menu.manualContentsPos / 400f);
				if (Menu.manualContentsPos > 400f)
				{
					Menu.manualContentsPos = 400f;
				}
				return;
			}
			this.UpdateHelpScroll(textSize);
			if (Game1.hud.KeyCancel)
			{
				this.UnloadHelp();
			}
			else if (Game1.hud.KeySelect && Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				Sound.PlayCue("menu_confirm");
				Game1.hud.KeySelect = false;
				Game1.hud.canInput = false;
				Menu.manualContents = !Menu.manualContents;
				this.PopulateHelp((int)Menu.windowWidth);
			}
			if (Menu.manualContentsPos > 0f)
			{
				Menu.manualContentsPos -= Game1.HudTime * 6000f * (Menu.manualContentsPos / 400f);
				if (Menu.manualContentsPos < 1f)
				{
					Menu.manualContentsPos = 0f;
				}
			}
		}

		private void UpdateHelpScroll(float textSize)
		{
			float num = Menu.listScroll;
			GamePadState state = GamePad.GetState((PlayerIndex)Game1.currentGamePad);
			if (Game1.longSkipFrame > 3)
			{
				if (state.DPad.Up == ButtonState.Pressed || Game1.pcManager.IsHudHeld("KeyUp"))
				{
					Menu.listScroll += 120f;
				}
				else if (state.DPad.Down == ButtonState.Pressed || Game1.pcManager.IsHudHeld("KeyDown"))
				{
					Menu.listScroll -= 120f;
				}
			}
			if (state.ThumbSticks.Left.Y < -0.05f)
			{
				Menu.listScroll += state.ThumbSticks.Left.Y * 20f;
			}
			else if (state.ThumbSticks.Left.Y > 0.05f)
			{
				Menu.listScroll -= state.ThumbSticks.Left.Y * -20f;
			}
			if (Menu.listScroll > 50f * textSize)
			{
				Menu.listScroll = 50f * textSize;
			}
			if (Menu.listScroll < 0f - Menu.maxScroll)
			{
				Menu.listScroll = 0f - Menu.maxScroll;
			}
			if (Math.Abs(Menu.listScroll - num) > 1f && Game1.longSkipFrame > 3)
			{
				Sound.PlayCue("menu_click");
			}
		}

		private void DrawHelp()
		{
			int num = Menu.leftEdge + (int)Menu.manualContentsPos;
			int num2 = Menu.rightEdge + (int)Menu.manualContentsPos;
			Game1.hud.DrawBorder(new Vector2(num, Menu.topEdge), Menu.rightEdge - Menu.leftEdge, Menu.bottomEdge - Menu.topEdge, Color.White, 1f, 0);
			if (!Menu.helpLoaded)
			{
				Menu.textAlpha = 200f;
				Game1.DrawLoad(this.sprite, new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2));
				return;
			}
			this.DrawScroll(Game1.graphics.GraphicsDevice, new Vector2(num + 30, Menu.topEdge + 30));
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				Game1.hud.DrawScrollBarMouse(new Vector2(num2 - 35, Menu.topEdge + 55), ref Menu.listScroll, Menu.bottomEdge - Menu.topEdge - 180, -(int)Menu.maxScroll, 1, 1f);
				if (!Menu.manualContents)
				{
					Game1.pcManager.MouseWheelAdjust(ref Menu.listScroll, -120f, 0f - Menu.maxScroll, 0f);
				}
				if (Game1.pcManager.DrawMouseButton(new Vector2(num + Menu.rightEdge - Menu.leftEdge - 20, Menu.topEdge - 30), 0.8f, Color.White, 0, draw: true))
				{
					this.UnloadHelp();
				}
				if (Game1.pcManager.DrawMouseButton(new Vector2(num + Menu.rightEdge - Menu.leftEdge - 65, Menu.bottomEdge - 70), 0.8f, Color.White, 3, draw: true))
				{
					Sound.PlayCue("menu_confirm");
					Game1.hud.canInput = false;
					Menu.manualContents = !Menu.manualContents;
					this.PopulateHelp((int)Menu.windowWidth);
				}
			}
			else
			{
				Game1.hud.DrawScrollBar(new Vector2(num2 - 35, Menu.topEdge + 55), 0f - Menu.listScroll / Menu.maxScroll, Menu.bottomEdge - Menu.topEdge - 130, 1f);
				if (!Menu.manualContents)
				{
					Game1.smallText.Color = Color.White;
					Game1.smallText.DrawButtonText(new Vector2(num, Menu.bottomEdge - 40), this.buttonText, 0.8f, this.buttonTextButtonList, bounce: false, num2 - num, TextAlign.Center);
				}
			}
			if (Menu.manualContentsPos != 0f)
			{
				this.DrawHelpContents();
			}
		}

		private void DrawHelpContents()
		{
			int num = -650 + (int)(Menu.manualContentsPos * 2f);
			int num2 = num + 600;
			int num3 = Menu.topEdge - 20;
			int num4 = Menu.bottomEdge + 20;
			Game1.hud.DrawBorder(new Vector2(num, num3), 600, num4 - num3, Color.White, 0.95f, 0);
			Game1.bigText.Color = Color.White;
			Game1.bigText.DrawShadowText(new Vector2(num, num3 + 20), Strings_Manual.ContentTitle, 0.7f, num2 - num, TextAlign.Center, outline: true);
			float num5 = 0.8f;
			Game1.smallText.Color = Color.White;
			Vector2 vector = new Vector2(num, num3 + 60);
			Game1.hud.DrawCursor(vector + new Vector2(40f, 10f + (float)this.curMenuOption * (40f * num5)), 0.8f, Color.White, flip: false);
			foreach (KeyValuePair<Vector2, string> scrollBookMark in this.scrollBookMarks)
			{
				Vector2 vector2 = vector + new Vector2(40f, scrollBookMark.Key.X * (40f * num5));
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && new Rectangle((int)vector2.X, (int)vector2.Y, 300, (int)(40f * num5)).Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y))
				{
					if (scrollBookMark.Key.X != (float)this.curMenuOption)
					{
						this.curMenuOption = (int)scrollBookMark.Key.X;
						Sound.PlayCue("menu_click");
					}
					if (Game1.pcManager.leftMouseClicked)
					{
						Game1.pcManager.leftMouseClicked = false;
						foreach (KeyValuePair<Vector2, string> scrollBookMark2 in this.scrollBookMarks)
						{
							if ((float)this.curMenuOption == scrollBookMark2.Key.X)
							{
								Menu.listScroll = 0f - scrollBookMark2.Key.Y;
							}
						}
						Sound.PlayCue("menu_confirm");
						Game1.hud.KeySelect = false;
						Game1.hud.canInput = false;
						Menu.manualContents = !Menu.manualContents;
						this.PopulateHelp((int)Menu.windowWidth);
					}
				}
				Game1.smallText.DrawText(vector + new Vector2(40 + ((scrollBookMark.Key.X == (float)this.curMenuOption) ? 10 : 0), scrollBookMark.Key.X * (40f * num5)), scrollBookMark.Value, num5);
			}
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				if (Game1.pcManager.DrawMouseButton(new Vector2(num + 600 - 65, num4 - 70), 0.8f, Color.White, 1, draw: true))
				{
					Menu.manualContents = !Menu.manualContents;
					Sound.PlayCue("menu_cancel");
				}
			}
			else
			{
				Game1.smallText.DrawButtonText(new Vector2(num, num4 - 60), this.buttonText, 0.8f, this.buttonTextButtonList, bounce: false, num2 - num, TextAlign.Center);
			}
		}

		public void DrawScrollText(Dictionary<float, string> text, bool credits)
		{
			this.sprite.Draw(this.hudTex[1], new Rectangle(-Menu.leftEdge + 70, -Menu.topEdge + 24, Menu.rightEdge - Menu.leftEdge + 10, Menu.bottomEdge - Menu.topEdge), new Rectangle(0, 0, 570, 520), Color.White);
			if (Menu.helpLoaded)
			{
				Game1.smallText.Color = new Color(1f, 1f, 1f, 1f - Menu.textAlpha / 100f - Menu.manualContentsPos / 600f);
				float imageSize = 1f;
				if (Game1.standardDef)
				{
					imageSize = 0.8f;
				}
				if (credits)
				{
					this.DrawCreditsText(new Vector2(0f, Menu.listScroll), text, 0.8f, Menu.listScroll, Menu.bottomEdge - Menu.topEdge - 80, Menu.windowWidth - 40f);
				}
				else
				{
					Game1.smallText.DrawScrollText(new Vector2(0f, Menu.listScroll + 50f), text, this.optionDescButtonList, this.scrollImages, this.scrollBookMarks, 0.8f, Menu.listScroll, Menu.bottomEdge - Menu.topEdge - 80, Menu.windowWidth - 40f, imageSize);
				}
			}
		}

		public void DrawScrollImage(Vector2 pos, Rectangle sourceRect, Rectangle cropRect, float scale, Color color, float lineWidth, int caption)
		{
			float num;
			if (caption > 0)
			{
				scale = ((!Game1.standardDef) ? 0.9f : (scale * 0.8f));
				scale *= Math.Min(Game1.hiDefScaleOffset, 1f);
				num = (float)sourceRect.Width * scale / 2f;
				pos.Y += (float)sourceRect.Height * scale / 8f;
				this.DrawScrollImageText(new Vector2(pos.X + lineWidth / 2f - num, pos.Y), scale, caption);
			}
			else
			{
				num = (float)sourceRect.Width * scale / 2f;
			}
			this.sprite.Draw(Menu.miscTex, new Vector2(pos.X + lineWidth / 2f - num, pos.Y), sourceRect, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		}

		private void DrawScrollImageText(Vector2 pos, float scale, int caption)
		{
			switch (caption)
			{
				case 1:
					this.DrawScrollImageTextSingle(new Vector2(220f, 320f), Strings_ManualGraphics.ButtonRThumb, TextAlign.Left, 0, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(460f, 260f), Strings_ManualGraphics.ButtonX, TextAlign.Left, 0, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(460f, 200f), Strings_ManualGraphics.ButtonA, TextAlign.Left, 0, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(460f, 130f), Strings_ManualGraphics.ButtonB, TextAlign.Left, 0, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(460f, 60f), Strings_ManualGraphics.ButtonY, TextAlign.Left, 0, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(460f, 5f), Strings_ManualGraphics.ButtonRB, TextAlign.Left, 0, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(310f, -30f), Strings_ManualGraphics.ButtonRT, TextAlign.Left, 0, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(240f, 13f), Strings_ManualGraphics.ButtonStart, TextAlign.Right, 0, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(150f, -30f), Strings_ManualGraphics.ButtonLT, TextAlign.Right, 1000, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(0f, 0f), Strings_ManualGraphics.ButtonLB, TextAlign.Right, 1000, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(0f, 90f), Strings_ManualGraphics.ButtonLThumb, TextAlign.Right, 1000, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(0f, 270f), Strings_ManualGraphics.ButtonBack, TextAlign.Right, 1000, pos, scale);
					break;
				case 2:
					this.DrawScrollImageTextSingle(new Vector2(450f, 245f), Strings_ManualGraphics.HudInfo8, TextAlign.Left, 0, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(450f, 165f), Strings_ManualGraphics.HudInfo7, TextAlign.Left, 0, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(320f, -30f), Strings_ManualGraphics.HudInfo6, TextAlign.Left, 0, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(450f, 15f), Strings_ManualGraphics.HudInfo5, TextAlign.Left, 0, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(100f, -30f), Strings_ManualGraphics.HudInfo1, TextAlign.Right, 1000, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(30f, 45f), Strings_ManualGraphics.HudInfo2, TextAlign.Right, 1000, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(100f, 205f), Strings_ManualGraphics.HudInfo3, TextAlign.Right, 1000, pos, scale);
					this.DrawScrollImageTextSingle(new Vector2(100f, 285f), Strings_ManualGraphics.HudInfo4, TextAlign.Right, 1000, pos, scale);
					break;
			}
		}

		private void DrawScrollImageTextSingle(Vector2 textPos, string text, TextAlign align, int width, Vector2 pos, float scale)
		{
			float size = 0.8f * scale;
			Vector2 pos2 = new Vector2(pos.X + textPos.X * scale, pos.Y + textPos.Y * scale);
			Game1.smallText.DrawText(pos2, text, size, width, align);
		}

		private void DrawScrollMask(int width, int height, int fadeHeight, GraphicsDevice graphicsDevice)
		{
			graphicsDevice.SetRenderTarget(Menu.scrollMask);
			graphicsDevice.Clear(Color.Black);
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			this.sprite.Draw(this.particlesTex[1], new Rectangle(-50, -2, width + 100, fadeHeight + 2), new Rectangle(1794, 130, 29, 29), Color.White);
			this.sprite.Draw(this.particlesTex[1], new Rectangle(-50, height - fadeHeight - 2, width + 100, fadeHeight + 2), new Rectangle(1794, 130, 29, 29), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
			this.sprite.Draw(this.nullTex, new Rectangle(0, fadeHeight - 2, width, height - fadeHeight * 2 + 4), Color.White);
			this.sprite.End();
			graphicsDevice.SetRenderTarget(null);
			this.scrollMaskTexture = Menu.scrollMask;
		}

		public void DrawScrollSource(GraphicsDevice graphicsDevice, bool credits)
		{
			graphicsDevice.SetRenderTarget(Menu.scrollSource);
			graphicsDevice.Clear(Color.Black);
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			this.DrawScrollText(this.scrollString, credits);
			this.sprite.End();
			graphicsDevice.SetRenderTarget(null);
			this.scrollSourceTexture = Menu.scrollSource;
		}

		public void DrawScroll(GraphicsDevice graphicsDevice, Vector2 pos)
		{
			if (this.scrollMaskTexture != null && this.scrollSourceTexture != null)
			{
				this.sprite.End();
				Game1.maskEffect.Parameters["MaskTexture"].SetValue(this.scrollMaskTexture);
				this.sprite.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
				Game1.maskEffect.CurrentTechnique.Passes[0].Apply();
				this.sprite.Draw(this.scrollSourceTexture, new Rectangle((int)pos.X, (int)pos.Y, Menu.scrollSource.Width, Menu.scrollSource.Height), Color.White);
				this.sprite.End();
				this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			}
		}

		private void InitSettings()
		{
			Menu.prevSFXVolume = Sound.masterSFXVolume;
			Menu.prevMusicVolume = Sound.masterMusicVolume;
			Menu.prevRumble = Game1.settings.Rumble;
			Menu.prevAutoCombo = Game1.settings.AutoCombo;
			Menu.prevAutoHeal = Game1.settings.AutoHeal;
			Menu.prevAutoLevelUp = Game1.settings.AutoLevelUp;
			Menu.prevAutoAdvance = Game1.settings.AutoAdvance;
			Menu.prevColorBlind = Game1.settings.ColorBlind;
			this.prevDifficulty = Game1.stats.gameDifficulty;
			Menu.prevResolution = new Vector2(Game1.screenWidth, Game1.screenHeight);
			bool flag = (Game1.settings.FullScreen = Game1.graphics.IsFullScreen);
			Menu.prevFullScreen = flag;
			Menu.prevBloom = Game1.settings.Bloom;
			Menu.prevDepth = Game1.settings.DepthOfField;
			int num = (Menu.prevInputMethod = (Game1.settings.InputMethod = (int)Game1.pcManager.inputDevice));
			this.menuMode = MenuMode.Settings;
			this.curMenuPage = 0;
			this.curMenuOption = 0;
			Menu.listScroll = 0f;
			Menu.settingSelection = 0;
			this.configuringControls = 0;
			Menu.confirmingResolution = 0f;
			this.PopulateSettings((int)Menu.windowWidth);
			Game1.hud.equipSelection = 0;
			List<Vector2> resolutions = this.GetResolutions();
			for (int i = 0; i < resolutions.Count; i++)
			{
				if (resolutions[i].X == (float)Game1.screenWidth && resolutions[i].Y == (float)Game1.screenHeight)
				{
					Game1.hud.equipSelection = i;
				}
			}
		}

		private void UnloadSettings(bool success)
		{
			Game1.hud.KeySelect = false;
			Game1.hud.KeyCancel = false;
			this.menuMode = MenuMode.None;
			this.configuringControls = 0;
			Menu.confirmingResolution = 0f;
			if (success)
			{
				if (Game1.gameMode == Game1.GameModes.MainMenu)
				{
					this.ExitMode(3, 2);
				}
				else
				{
					this.ExitMode(1, 2);
				}
			}
		}

		private void PopulateSettings(int width)
		{
			this.optionDesc = string.Empty;
			string text = string.Empty;
			switch (this.curMenuOption)
			{
				case 0:
					text = Strings_Options.Sound;
					break;
				case 1:
					text = Strings_Options.Music;
					break;
				case 2:
					text = Strings_Options.AutoAdvance;
					break;
				case 3:
					text = Strings_Options.Rumble;
					break;
				case 4:
					text = ((Game1.stats.gameDifficulty != 0) ? Strings_Options.AutoFire2 : Strings_Options.AutoFire1);
					break;
				case 5:
					text = ((Game1.stats.gameDifficulty != 0) ? Strings_Options.AutoHeal2 : Strings_Options.AutoHeal1);
					break;
				case 6:
					text = ((Game1.stats.gameDifficulty != 0) ? Strings_Options.AutoLevel2 : Strings_Options.AutoLevel1);
					break;
				case 7:
					text = Strings_Options.ColorBlind;
					break;
				case 8:
					Strings_Options.ResourceManager.GetString("Difficulty" + this.prevDifficulty);
					text = Strings_Options.DifficultyChange;
					if (Game1.stats.playerLifeState != 0)
					{
						text = text + "[N]" + Strings_Options.Cheating;
					}
					break;
			}
			this.optionDesc = Game1.smallText.WordWrap(text, 0.9f, width, TextAlign.Center);
			this.buttonText = Game1.smallText.WordWrap(Strings_Options.SettingsControls, 0.8f, width, this.buttonTextButtonList, TextAlign.Left);
		}

		private void UpdateSettings()
		{
			if (Game1.isPCBuild)
			{
				int num = 19;
				int count = Game1.pcManager.inputKeyList.Count;
				int num2 = 3;
				if (Menu.confirmingResolution > 0f && Game1.CheckIsActive)
				{
					Menu.confirmingResolution -= Game1.HudTime;
					if (!(Menu.confirmingResolution < 0f))
					{
						return;
					}
					if (Menu.prevFullScreen != Game1.graphics.IsFullScreen)
					{
						Game1.graphics.ToggleFullScreen();
					}
					if (Menu.prevResolution != new Vector2(Game1.screenWidth, Game1.screenHeight))
					{
						Game1.SetResolution((int)Menu.prevResolution.X, (int)Menu.prevResolution.Y);
					}
					List<Vector2> resolutions = this.GetResolutions();
					for (int i = 0; i < resolutions.Count; i++)
					{
						if (resolutions[i].X == (float)Game1.screenWidth && resolutions[i].Y == (float)Game1.screenHeight)
						{
							Game1.hud.equipSelection = i;
						}
					}
					return;
				}
				if (this.configuringControls == 0)
				{
					if (Game1.hud.KeyUp)
					{
						Game1.hud.KeyUp = false;
						do
						{
							this.curMenuOption--;
						}
						while (this.CheckValidItem(this.curMenuOption));
					}
					if (Game1.hud.KeyDown)
					{
						Game1.hud.KeyDown = false;
						do
						{
							this.curMenuOption++;
						}
						while (this.CheckValidItem(this.curMenuOption));
					}
					if (this.curMenuOption < 0)
					{
						this.curMenuOption = num + num2;
					}
					else if (this.curMenuOption > num + num2)
					{
						this.curMenuOption = 0;
					}
					if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
					{
						this.curMenuOption = 0;
					}
					else
					{
						Game1.hud.mousePos = Vector2.Zero;
					}
					if (Game1.hud.KeyCancel)
					{
						this.RevertSettings();
					}
				}
				else if (this.configuringControls == 1)
				{
					if (Game1.hud.KeyUp)
					{
						Game1.hud.KeyUp = false;
						this.curMenuOption--;
					}
					if (Game1.hud.KeyDown)
					{
						Game1.hud.KeyDown = false;
						this.curMenuOption++;
					}
					if (this.curMenuOption < 0)
					{
						this.curMenuOption = count + 1;
					}
					else if (this.curMenuOption > count + 1)
					{
						this.curMenuOption = 0;
					}
					if (Game1.hud.KeySelect && this.curMenuOption < count)
					{
						Sound.PlayCue("menu_confirm");
						this.configuringControls = 2;
						Menu.screenFade = 0f;
						this.optionDesc = Game1.smallText.WordWrap(Strings_Options.PCCustomizeHelp1, 0.7f, 460f, TextAlign.Center);
					}
				}
				if (this.configuringControls > 0 && Game1.hud.KeyCancel && Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
				{
					Sound.PlayCue("menu_confirm");
					this.configuringControls--;
					if (this.configuringControls == 0)
					{
						Menu.fileScroll = 0f;
						this.curMenuOption = num + 1;
					}
				}
				return;
			}
			if (!Game1.hud.KeyRight && !Game1.hud.KeyLeft)
			{
				int num3 = 9;
				if (Game1.gameMode == Game1.GameModes.MainMenu)
				{
					num3 = 8;
				}
				if (Game1.hud.KeyUp && this.curMenuOption > 0)
				{
					this.curMenuOption--;
					Menu.fadeInAlpha = 1f;
					this.PopulateSettings((int)Menu.windowWidth);
					Sound.PlayCue("menu_click");
				}
				else if (Game1.hud.KeyDown && this.curMenuOption < num3 - 1)
				{
					this.curMenuOption++;
					Menu.fadeInAlpha = 1f;
					this.PopulateSettings((int)Menu.windowWidth);
					Sound.PlayCue("menu_click");
				}
				float num4 = Math.Max(this.curMenuOption - 1, 0) * 60;
				Menu.listScroll += (num4 - Menu.listScroll) * Game1.HudTime * 16f;
			}
			if (!Game1.hud.KeyUp && !Game1.hud.KeyDown && (Game1.hud.GetCurstate().ThumbSticks.Left.X != 0f || Game1.hud.KeyRight || Game1.hud.KeyLeft))
			{
				if (this.curMenuOption == 0)
				{
					if (Game1.longSkipFrame > 3 || Game1.hud.KeyRight || Game1.hud.KeyLeft)
					{
						Sound.PlayCue("menu_click");
					}
					Sound.masterSFXVolume += Game1.hud.GetCurstate().ThumbSticks.Left.X / 100f;
					if (Game1.hud.KeyRight && Game1.hud.GetCurstate().ThumbSticks.Left.X == 0f)
					{
						Sound.masterSFXVolume += 0.1f;
					}
					else if (Game1.hud.KeyLeft && Game1.hud.GetCurstate().ThumbSticks.Left.X == 0f)
					{
						Sound.masterSFXVolume -= 0.1f;
					}
					if (Sound.masterSFXVolume > 1f)
					{
						Sound.masterSFXVolume = 1f;
					}
					else if (Sound.masterSFXVolume < 0f)
					{
						Sound.masterSFXVolume = 0f;
					}
					Sound.SetSFXVolume(Sound.masterSFXVolume);
				}
				else if (this.curMenuOption == 1)
				{
					Sound.masterMusicVolume += Game1.hud.GetCurstate().ThumbSticks.Left.X / 100f;
					if (Game1.hud.KeyRight && Game1.hud.GetCurstate().ThumbSticks.Left.X == 0f)
					{
						Sound.masterMusicVolume += 0.1f;
					}
					else if (Game1.hud.KeyLeft && Game1.hud.GetCurstate().ThumbSticks.Left.X == 0f)
					{
						Sound.masterMusicVolume -= 0.1f;
					}
					if (Sound.masterMusicVolume > 1f)
					{
						Sound.masterMusicVolume = 1f;
					}
					else if (Sound.masterMusicVolume < 0f)
					{
						Sound.masterMusicVolume = 0f;
					}
				}
				else if (this.curMenuOption == 2)
				{
					if (Game1.hud.KeyRight)
					{
						if (Game1.settings.AutoAdvance)
						{
							Sound.PlayCue("menu_click");
							Game1.settings.AutoAdvance = false;
						}
					}
					else if (Game1.hud.KeyLeft && !Game1.settings.AutoAdvance)
					{
						Sound.PlayCue("menu_click");
						Game1.settings.AutoAdvance = true;
					}
				}
				else if (this.curMenuOption == 3)
				{
					if (Game1.hud.KeyRight)
					{
						if (Game1.settings.Rumble)
						{
							Sound.PlayCue("menu_click");
							Game1.settings.Rumble = false;
						}
					}
					else if (Game1.hud.KeyLeft && !Game1.settings.Rumble)
					{
						Sound.PlayCue("menu_click");
						Game1.settings.Rumble = true;
					}
				}
				else if (this.curMenuOption == 7)
				{
					if (Game1.hud.KeyRight)
					{
						if (Game1.settings.ColorBlind)
						{
							Sound.PlayCue("menu_click");
							Game1.settings.ColorBlind = false;
						}
					}
					else if (Game1.hud.KeyLeft && !Game1.settings.ColorBlind)
					{
						Sound.PlayCue("menu_click");
						Game1.settings.ColorBlind = true;
					}
				}
				else if (this.curMenuOption == 8)
				{
					if (Game1.stats.playerLifeState == 0)
					{
						if (Game1.hud.KeyRight)
						{
							Sound.PlayCue("menu_click");
							Game1.stats.gameDifficulty++;
							if (Game1.stats.gameDifficulty > 3)
							{
								Game1.stats.gameDifficulty = 0;
							}
						}
						else if (Game1.hud.KeyLeft)
						{
							Sound.PlayCue("menu_click");
							int gameDifficulty = Game1.stats.gameDifficulty;
							gameDifficulty--;
							if (gameDifficulty < 0)
							{
								gameDifficulty = 3;
							}
							Game1.stats.gameDifficulty = (byte)gameDifficulty;
						}
					}
				}
				else if (Game1.stats.gameDifficulty > 0)
				{
					if (this.curMenuOption == 4)
					{
						if (Game1.hud.KeyRight)
						{
							if (Game1.settings.AutoCombo)
							{
								Sound.PlayCue("menu_click");
								Game1.settings.AutoCombo = false;
							}
						}
						else if (Game1.hud.KeyLeft && !Game1.settings.AutoCombo)
						{
							Sound.PlayCue("menu_click");
							Game1.settings.AutoCombo = true;
						}
					}
					else if (this.curMenuOption == 5)
					{
						if (Game1.hud.KeyRight)
						{
							if (Game1.settings.AutoHeal)
							{
								Sound.PlayCue("menu_click");
								Game1.settings.AutoHeal = false;
							}
						}
						else if (Game1.hud.KeyLeft && !Game1.settings.AutoHeal)
						{
							Sound.PlayCue("menu_click");
							Game1.settings.AutoHeal = true;
						}
					}
					else if (this.curMenuOption == 6)
					{
						if (Game1.hud.KeyRight)
						{
							if (Game1.settings.AutoLevelUp)
							{
								Sound.PlayCue("menu_click");
								Game1.settings.AutoLevelUp = false;
							}
						}
						else if (Game1.hud.KeyLeft && !Game1.settings.AutoLevelUp)
						{
							Sound.PlayCue("menu_click");
							Game1.settings.AutoLevelUp = true;
						}
					}
				}
			}
			if (Game1.hud.KeySelect && this.ApplySettings())
			{
				return;
			}
			if (Game1.hud.KeyCancel)
			{
				this.RevertSettings();
			}
			if (Game1.hud.KeyX)
			{
				Sound.PlayCue("menu_confirm");
				Sound.masterMusicVolume = (Sound.masterSFXVolume = 1f);
				Sound.SetSFXVolume(Sound.masterSFXVolume);
				Game1.settings.ResetSettings();
				if (Game1.gameMode != 0)
				{
					Game1.stats.gameDifficulty = this.prevDifficulty;
				}
			}
		}

		private bool ApplySettings()
		{
			bool flag = false;
			if (Game1.settings.FullScreen != Game1.graphics.IsFullScreen)
			{
				flag = true;
				Game1.graphics.ToggleFullScreen();
			}
			Vector2 vector = this.GetResolutions()[Game1.hud.equipSelection];
			if (vector != new Vector2(Game1.screenWidth, Game1.screenHeight))
			{
				flag = true;
				if (!Game1.SetResolution((int)vector.X, (int)vector.Y))
				{
					this.RevertSettings();
				}
			}
			if (flag)
			{
				this.curMenuOption = 0;
				Menu.confirmingResolution = 15f;
				Sound.PlayCue("menu_confirm");
				return false;
			}
			if (this.prevDifficulty > Game1.stats.gameDifficulty && Game1.stats.gameDifficulty < 2)
			{
				Game1.stats.startDifficulty = 0;
			}
			Game1.pcManager.inputDevice = (InputDevice)Game1.settings.InputMethod;
			if (!Game1.GamerServices || !Game1.IsTrial)
			{
				Game1.storage.Write(0, -1);
				Game1.storage.DisposeContainer();
				if (Game1.storage.storeResult != StoreResult.Saved)
				{
					Sound.PlayCue("fidget_fail");
					Game1.menu.SelectStorage();
					return true;
				}
				Menu.savingAlpha = 1f;
			}
			this.UnloadSettings(this.prompt != promptDialogue.SignedOut);
			return false;
		}

		private void RevertSettings()
		{
			Sound.masterSFXVolume = Menu.prevSFXVolume;
			Sound.SetSFXVolume(Sound.masterSFXVolume);
			Sound.masterMusicVolume = Menu.prevMusicVolume;
			Game1.settings.Rumble = Menu.prevRumble;
			Game1.settings.AutoCombo = Menu.prevAutoCombo;
			Game1.settings.AutoHeal = Menu.prevAutoHeal;
			Game1.settings.AutoLevelUp = Menu.prevAutoLevelUp;
			Game1.settings.AutoAdvance = Menu.prevAutoAdvance;
			Game1.settings.ColorBlind = Menu.prevColorBlind;
			Game1.stats.gameDifficulty = this.prevDifficulty;
			Game1.SetResolution((int)Menu.prevResolution.X, (int)Menu.prevResolution.Y);
			if (Menu.prevFullScreen != Game1.graphics.IsFullScreen)
			{
				Game1.settings.FullScreen = Menu.prevFullScreen;
				Game1.graphics.ToggleFullScreen();
			}
			Game1.settings.Bloom = Menu.prevBloom;
			Game1.settings.DepthOfField = Menu.prevDepth;
			Game1.pcManager.inputDevice = (InputDevice)Menu.prevInputMethod;
			this.UnloadSettings(success: true);
		}

		private void DrawSettings()
		{
			if (Game1.isPCBuild)
			{
				this.DrawSettingsPC();
				return;
			}
			int num = 9;
			if (Game1.gameMode == Game1.GameModes.MainMenu)
			{
				num = 8;
			}
			Game1.hud.DrawBorder(new Vector2(Menu.leftEdge, Menu.topEdge), Menu.rightEdge - Menu.leftEdge, Menu.bottomEdge - Menu.topEdge, Color.White, 0.9f, 365);
			Color color = new Color(0f, 0f, 0f, 0.5f);
			this.sprite.Draw(this.hudTex[1], new Vector2(Game1.screenWidth / 2, Menu.topEdge + 68), new Rectangle(887, 20, 234, 180), color, 0f, Vector2.Zero, new Vector2(1.2f, 1f), SpriteEffects.None, 0f);
			this.sprite.Draw(this.hudTex[1], new Vector2(Game1.screenWidth / 2, Menu.topEdge + 68), new Rectangle(887, 20, 234, 180), color, 0f, new Vector2(234f, 0f), new Vector2(1.2f, 1f), SpriteEffects.FlipHorizontally, 0f);
			this.sprite.Draw(this.hudTex[2], new Vector2(Game1.screenWidth / 2, Menu.topEdge + 60), new Rectangle(0, 502, 326, 18), Color.White, 0f, new Vector2(163f, 0f), new Vector2(1.5f, 1f), SpriteEffects.None, 0f);
			Game1.bigText.Color = Color.White;
			Game1.bigText.DrawOutlineText(new Vector2(Menu.leftEdge, Menu.topEdge + 15), Strings_Options.SettingsTitle, 1f, Menu.rightEdge - Menu.leftEdge, TextAlign.Center, fullOutline: true);
			Game1.smallText.Color = Color.White;
			Game1.smallText.DrawButtonText(new Vector2(Menu.leftEdge, Menu.bottomEdge - 40), this.buttonText, 0.8f, this.buttonTextButtonList, bounce: false, Menu.rightEdge - Menu.leftEdge, TextAlign.Center);
			Game1.smallText.Color = new Color(1f, 1f, 1f, 1f - Menu.fadeInAlpha);
			Game1.smallText.DrawText(new Vector2(Menu.leftEdge + 40, Menu.topEdge + 380), this.optionDesc, 0.9f, 0f, TextAlign.Left);
			Vector2 vector = new Vector2(Game1.screenWidth / 2, (float)(Menu.topEdge + 100) - Menu.listScroll);
			for (int i = 0; i < num; i++)
			{
				this.DrawItem(i, new Vector2(vector.X, vector.Y + (float)(60 * i)), (int)this.GetListColor(i, 180f).A);
			}
			Game1.hud.DrawCursor(new Vector2(vector.X + 60f, Menu.topEdge + 108 + Math.Min(this.curMenuOption, 1) * 60), 0.75f, Color.White, flip: false);
			Game1.hud.DrawScrollBar(new Vector2(Menu.rightEdge - 35, Menu.topEdge + 55), Menu.listScroll / (float)((num - 1) * 60), 220f, 1f);
			if (Game1.Build > 1f)
			{
				Game1.smallText.Color = Color.White;
				Game1.hud.DrawMiniBorder(new Vector2(Menu.leftEdge, Menu.topEdge), 100, 35, Color.White, 1f);
				Game1.smallText.DrawText(new Vector2(Menu.leftEdge, Menu.topEdge + 5), "  Ver. " + Game1.Build, 0.7f, 1000f, TextAlign.Left);
			}
		}

		private void DrawItem(int item, Vector2 pos, float alpha)
		{
			alpha /= 255f;
			float size = 0.7f;
			Game1.bigText.Color = new Color(1f, 1f, 1f, alpha);
			switch (item)
			{
				case 0:
					Game1.bigText.DrawText(pos, Strings_Options.SoundTitle, size, 600f, TextAlign.Right);
					Game1.hud.DrawBar(pos + new Vector2(50f, 0f), Sound.masterSFXVolume / 1f, new Color(0.25f, 1f, 1f, alpha), new Vector2(0.8f, 1f), backGround: true);
					break;
				case 1:
					Game1.bigText.DrawText(pos, Strings_Options.MusicTitle, size, 600f, TextAlign.Right);
					Game1.hud.DrawBar(pos + new Vector2(50f, 0f), Sound.masterMusicVolume / 1f, new Color(0.25f, 1f, 1f, alpha), new Vector2(0.8f, 1f), backGround: true);
					break;
				case 2:
					{
						Game1.bigText.DrawText(pos, Strings_Options.AutoAdvanceTitle, size, 600f, TextAlign.Right);
						float num11 = 1f;
						float num12 = 0.25f;
						if (!Game1.settings.AutoAdvance)
						{
							num11 = 0.25f;
							num12 = 1f;
						}
						Game1.bigText.Color = new Color(1f, 1f, 1f, num11 * alpha);
						Game1.bigText.DrawText(pos + new Vector2(100f, 0f), Strings_Options.On, size, 600f, TextAlign.Right);
						Game1.bigText.Color = new Color(1f, 1f, 1f, num12 * alpha);
						Game1.bigText.DrawText(pos + new Vector2(140f, 0f), Strings_Options.Off, size, 600f, TextAlign.Left);
						break;
					}
				case 3:
					{
						Game1.bigText.DrawText(pos, Strings_Options.RumbleTitle, size, 600f, TextAlign.Right);
						float num3 = 1f;
						float num4 = 0.25f;
						if (!Game1.settings.Rumble)
						{
							num3 = 0.25f;
							num4 = 1f;
						}
						Game1.bigText.Color = new Color(1f, 1f, 1f, num3 * alpha);
						Game1.bigText.DrawText(pos + new Vector2(100f, 0f), Strings_Options.On, size, 600f, TextAlign.Right);
						Game1.bigText.Color = new Color(1f, 1f, 1f, num4 * alpha);
						Game1.bigText.DrawText(pos + new Vector2(140f, 0f), Strings_Options.Off, size, 600f, TextAlign.Left);
						break;
					}
				case 4:
					Game1.bigText.DrawText(pos, Strings_Options.AutoFireTitle, size, 600f, TextAlign.Right);
					if (Game1.stats.gameDifficulty > 0)
					{
						float num9 = 1f;
						float num10 = 0.25f;
						if (!Game1.settings.AutoCombo)
						{
							num9 = 0.25f;
							num10 = 1f;
						}
						Game1.bigText.Color = new Color(1f, 1f, 1f, num9 * alpha);
						Game1.bigText.DrawText(pos + new Vector2(100f, 0f), Strings_Options.On, size, 600f, TextAlign.Right);
						Game1.bigText.Color = new Color(1f, 1f, 1f, num10 * alpha);
						Game1.bigText.DrawText(pos + new Vector2(140f, 0f), Strings_Options.Off, size, 600f, TextAlign.Left);
					}
					else
					{
						Game1.bigText.Color = new Color(1f, 1f, 1f, alpha);
						Game1.bigText.DrawText(pos + new Vector2(100f, 0f), Strings_Options.On, size, 600f, TextAlign.Right);
					}
					break;
				case 5:
					Game1.bigText.DrawText(pos, Strings_Options.AutoHealTitle, size, 600f, TextAlign.Right);
					if (Game1.stats.gameDifficulty > 0)
					{
						float num5 = 1f;
						float num6 = 0.25f;
						if (!Game1.settings.AutoHeal)
						{
							num5 = 0.25f;
							num6 = 1f;
						}
						Game1.bigText.Color = new Color(1f, 1f, 1f, num5 * alpha);
						Game1.bigText.DrawText(pos + new Vector2(100f, 0f), Strings_Options.On, size, 600f, TextAlign.Right);
						Game1.bigText.Color = new Color(1f, 1f, 1f, num6 * alpha);
						Game1.bigText.DrawText(pos + new Vector2(140f, 0f), Strings_Options.Off, size, 600f, TextAlign.Left);
					}
					else
					{
						Game1.bigText.Color = new Color(1f, 1f, 1f, alpha);
						Game1.bigText.DrawText(pos + new Vector2(100f, 0f), Strings_Options.On, size, 600f, TextAlign.Right);
					}
					break;
				case 6:
					Game1.bigText.DrawText(pos, Strings_Options.AutoLevelTitle, size, 600f, TextAlign.Right);
					if (Game1.stats.gameDifficulty > 0)
					{
						float num7 = 1f;
						float num8 = 0.25f;
						if (!Game1.settings.AutoLevelUp)
						{
							num7 = 0.25f;
							num8 = 1f;
						}
						Game1.bigText.Color = new Color(1f, 1f, 1f, num7 * alpha);
						Game1.bigText.DrawText(pos + new Vector2(100f, 0f), Strings_Options.On, size, 600f, TextAlign.Right);
						Game1.bigText.Color = new Color(1f, 1f, 1f, num8 * alpha);
						Game1.bigText.DrawText(pos + new Vector2(140f, 0f), Strings_Options.Off, size, 600f, TextAlign.Left);
					}
					else
					{
						Game1.bigText.Color = new Color(1f, 1f, 1f, alpha);
						Game1.bigText.DrawText(pos + new Vector2(100f, 0f), Strings_Options.On, size, 600f, TextAlign.Right);
					}
					break;
				case 7:
					{
						Game1.bigText.DrawText(pos, Strings_Options.ColorBlindTitle, size, 600f, TextAlign.Right);
						float num = 1f;
						float num2 = 0.25f;
						if (!Game1.settings.ColorBlind)
						{
							num = 0.25f;
							num2 = 1f;
						}
						Game1.bigText.Color = new Color(1f, 1f, 1f, num * alpha);
						Game1.bigText.DrawText(pos + new Vector2(100f, 0f), Strings_Options.On, size, 600f, TextAlign.Right);
						Game1.bigText.Color = new Color(1f, 1f, 1f, num2 * alpha);
						Game1.bigText.DrawText(pos + new Vector2(140f, 0f), Strings_Options.Off, size, 600f, TextAlign.Left);
						break;
					}
				case 8:
					Game1.bigText.DrawText(pos, Strings_Options.DifficultyTitle, size, 600f, TextAlign.Right);
					Game1.bigText.Color = new Color(1f, 1f, 1f, alpha);
					Game1.bigText.DrawText(pos, "< " + Strings_Options.ResourceManager.GetString("Difficulty" + Game1.stats.gameDifficulty) + " >", size, 300f, TextAlign.Center);
					break;
				case 9:
					break;
			}
		}

		private Color GetListColor(int a, float maxHeight)
		{
			int num = 60 * a;
			Color result;
			if (Menu.listScroll > (float)(60 * a))
			{
				result = new Color(1f, 1f, 1f, 1f - (Menu.listScroll - (float)(60 * a)) / 32f);
			}
			else
			{
				if (!(Menu.listScroll + maxHeight < (float)num))
				{
					return Color.White;
				}
				result = new Color(1f, 1f, 1f, 1f - ((float)num - (Menu.listScroll + maxHeight)) / 32f);
			}
			return result;
		}

		private void DrawSettingsPC()
		{
			int num = 19;
			int count = Game1.pcManager.inputKeyList.Count;
			int num2 = Math.Max(Menu.rightEdge - Menu.leftEdge, 1000);
			if (this.configuringControls > 0)
			{
				num2 = Math.Max(Menu.rightEdge - Menu.leftEdge, 900);
			}
			int num3 = (Game1.screenWidth + num2) / 2;
			Game1.hud.DrawBorder(new Vector2(Game1.screenWidth / 2 - num2 / 2, Menu.topEdge), num2, Menu.bottomEdge - Menu.topEdge, Color.White, 0.97f, 0);
			if (Game1.Build > 1f && this.configuringControls == 0)
			{
				Game1.smallText.Color = Color.White;
				Game1.hud.DrawMiniBorder(new Vector2(Menu.leftEdge + 40, Menu.bottomEdge - 62), 100, 35, Color.White, 1f);
				Game1.smallText.DrawText(new Vector2(Menu.leftEdge + 40, Menu.bottomEdge - 62 + 5), "  Ver. " + Game1.Build, 0.7f, 1000f, TextAlign.Left);
			}
			Game1.bigText.Color = Color.White;
			Game1.bigText.DrawOutlineText(new Vector2(Menu.leftEdge, Menu.topEdge + 15), (this.configuringControls == 0) ? Strings_Options.SettingsTitle : Strings_Options.PCCustomizeTitle, 1f, Menu.rightEdge - Menu.leftEdge, TextAlign.Center, fullOutline: true);
			float num4 = 0.7f;
			Vector2 vector = Vector2.Zero;
			Vector2 vector2 = ((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse || this.configuringControls == 1) ? new Vector2(Menu.rightEdge - 135, Menu.bottomEdge - 70) : new Vector2(Menu.rightEdge - 210, Menu.bottomEdge - 70));
			Vector2 vector3 = ((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse) ? new Vector2(Menu.rightEdge - 20, Menu.topEdge - 30) : new Vector2(Menu.rightEdge - 65, Menu.bottomEdge - 70));
			Vector2 vector4 = ((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse || this.configuringControls == 1) ? new Vector2(Menu.rightEdge - 65, Menu.bottomEdge - 70) : new Vector2(Menu.rightEdge - 135, Menu.bottomEdge - 70));
			if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				Game1.hud.mousePos = Vector2.Zero;
			}
			int num5 = (int)(Game1.smallFont.MeasureString(Strings_Options.PCCustomizeTitle).X * num4) + 80;
			if (Menu.confirmingResolution <= 0f && this.configuringControls < 2)
			{
				if (this.configuringControls == 0)
				{
					if (Game1.pcManager.DrawMouseButton(vector2, 0.8f, Color.White, 2, draw: true, Strings_Options.PCCustomizeTitle, TextAlign.Right) || (this.curMenuOption == num + 1 && Game1.hud.KeySelect))
					{
						Sound.PlayCue("menu_confirm");
						Game1.hud.KeySelect = false;
						this.configuringControls = 1;
						this.curMenuOption = 0;
						Menu.fileScroll = 0f;
						return;
					}
					if (Game1.pcManager.DrawMouseButton(vector3, 0.8f, Color.White, 0, draw: true) || (this.curMenuOption == num + 3 && Game1.hud.KeySelect))
					{
						Sound.PlayCue("menu_cancel");
						Game1.hud.KeySelect = false;
						this.RevertSettings();
						return;
					}
				}
				if (Game1.pcManager.DrawMouseButton(vector4, 0.8f, Color.White, 2, draw: true) || (((this.configuringControls == 0 && this.curMenuOption == num + 2) || (this.configuringControls == 1 && (this.curMenuOption == count || this.curMenuOption == count + 1))) && Game1.hud.KeySelect))
				{
					Game1.hud.KeySelect = false;
					if (this.configuringControls == 0)
					{
						this.ApplySettings();
						return;
					}
					Sound.PlayCue("menu_confirm");
					if (this.curMenuOption == count)
					{
						Game1.pcManager.ResetMapping();
						return;
					}
					this.configuringControls = 0;
					Menu.fileScroll = 0f;
					this.curMenuOption = num + 1;
					return;
				}
				if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
				{
					if (this.configuringControls == 0)
					{
						if (this.curMenuOption == num + 1)
						{
							vector = vector2 + new Vector2(50 - num5, 20f);
						}
						if (this.curMenuOption == num + 2)
						{
							vector = vector4 + new Vector2(15f, 20f);
						}
						if (this.curMenuOption == num + 3)
						{
							vector = vector3 + new Vector2(15f, 20f);
						}
					}
					else
					{
						if (this.curMenuOption == count)
						{
							int num6 = (int)(Game1.smallFont.MeasureString(Strings_Options.PCCustomizeRestore).X * num4) + 80;
							vector = vector2 + new Vector2(50 - num6, 20f);
						}
						if (this.curMenuOption == count + 1)
						{
							vector = vector4 + new Vector2(15f, 20f);
						}
					}
				}
			}
			Game1.smallText.Color = Color.White;
			float num7 = 40f;
			Vector2 vector5 = new Vector2(Game1.screenWidth / 2 - 440, Menu.topEdge + 80);
			if (this.configuringControls > 0)
			{
				Menu.fadeInAlpha = 0f;
				float num8 = 270f;
				for (int i = 0; i < 3; i++)
				{
					Game1.bigText.DrawText(vector5 + new Vector2((float)i * num8, 0f), Strings_Options.ResourceManager.GetString("PCCustomizeCol" + i), num4);
					if (i > 0)
					{
						this.sprite.Draw(this.hudTex[2], new Vector2(vector5.X + (float)i * num8, Game1.screenHeight / 2), new Rectangle(0, 502, 326, 18), Color.White * 0.7f, 1.57f, new Vector2(163f, 0f), new Vector2((float)Game1.screenHeight / 500f, 0.3f), SpriteEffects.None, 0f);
					}
				}
				if (this.configuringControls == 1)
				{
					if (Game1.pcManager.DrawMouseButton(vector2, 0.8f, Color.White, 2, draw: true, Strings_Options.PCCustomizeRestore, TextAlign.Right))
					{
						Sound.PlayCue("menu_confirm");
						Game1.pcManager.ResetMapping();
						return;
					}
					if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
					{
						int num9 = (int)(10f * num7);
						Game1.hud.DrawScrollBarMouse(new Vector2(num3 - 40, Menu.topEdge + 50), ref Menu.fileScroll, Menu.bottomEdge - Menu.topEdge - 200, num9, (int)num7, 1f);
						Game1.pcManager.MouseWheelAdjust(ref Menu.fileScroll, num7, 0f, num9);
					}
					else
					{
						Game1.hud.DrawScrollBar(new Vector2(num3 - 40, Menu.topEdge + 50), Menu.fileScroll / 200f, Menu.bottomEdge - Menu.topEdge - 200, 1f);
					}
					int num10 = Math.Min(num2, Game1.screenWidth - 60) - num5 - 80;
					string text = Game1.smallText.WordWrap(Strings_Options.PCCustomizeHelp0, num4, num10 - 40, TextAlign.Center);
					Vector2 pos = new Vector2(Math.Max((Game1.screenWidth - num2) / 2 + 20, 30), (float)Menu.bottomEdge - Game1.smallFont.MeasureString(text).Y * num4 - 20f);
					Game1.smallText.DrawText(pos, text, num4);
				}
				float num11 = num7 + (0f - Menu.fileScroll);
				Vector2 vector6 = Menu.cursorPos;
				for (int j = 0; j < count; j++)
				{
					float num12 = this.DrawKeyboardMapping(vector5 + new Vector2(10f, num11), j, num8, num4, isMapping: false);
					if (j == this.curMenuOption)
					{
						vector6 = vector5 + new Vector2(0f, num11 + num12 / 2f - 4f);
					}
					num11 += num12;
				}
				if (this.curMenuOption < count)
				{
					Menu.cursorPos += (vector6 - Menu.cursorPos) * Game1.HudTime * 40f;
					if (Menu.cursorPos.Y > (float)(Menu.topEdge + 118) && Menu.cursorPos.Y < (float)(Menu.bottomEdge - 130))
					{
						Game1.hud.DrawCursor(Menu.cursorPos, 0.6f, Color.White, flip: false);
					}
					if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
					{
						if (Menu.cursorPos.Y < (float)(Menu.topEdge + 118) + num7 * 2f)
						{
							Menu.fileScroll = Math.Max(Menu.fileScroll - num7, 0f);
						}
						if (Menu.cursorPos.Y > (float)(Menu.bottomEdge - 130) - num7)
						{
							Menu.fileScroll += num7;
						}
					}
				}
				else
				{
					Menu.cursorPos += (vector - Menu.cursorPos) * Game1.HudTime * 20f;
					Game1.hud.DrawCursor(Menu.cursorPos, 0.6f, Color.White, flip: false);
				}
				if (this.configuringControls > 1)
				{
					Menu.screenFade = Math.Min(Menu.screenFade + Game1.HudTime * 4f, (Game1.pcManager.IsMouseLeftHeld() || Keyboard.GetState().IsKeyDown(Keys.Enter)) ? 0.9f : 1f);
					this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, Menu.screenFade * 0.6f));
					Vector2 vector7 = new Vector2(500f, Game1.smallFont.MeasureString(this.optionDesc).Y * num4 + 40f);
					Game1.hud.DrawMiniBorder(new Vector2((float)Game1.screenWidth - vector7.X, (float)Game1.screenHeight - vector7.Y) / 2f, (int)vector7.X, (int)vector7.Y, new Color(1f, 1f, 1f, Menu.screenFade), 0.95f);
					Game1.smallText.DrawText(new Vector2((float)Game1.screenWidth - vector7.X, (float)Game1.screenHeight - vector7.Y) / 2f + new Vector2(20f, 20f), this.optionDesc, num4);
				}
				return;
			}
			float toggleOffset = 220f;
			int num13 = 11;
			for (int k = 0; k < num13; k++)
			{
				this.sprite.Draw(this.hudTex[2], new Vector2(Game1.screenWidth / 2, vector5.Y + (float)k * num7 - 8f), new Rectangle(0, 502, 326, 18), Color.White * 0.7f, 0f, new Vector2(163f, 0f), new Vector2(Math.Max((Menu.rightEdge - Menu.leftEdge) / 300, 3), 0.3f), SpriteEffects.None, 0f);
			}
			this.sprite.Draw(this.hudTex[2], new Vector2(Game1.screenWidth / 2, vector5.Y + 4f * num7), new Rectangle(0, 502, 326, 18), Color.White * 0.7f, 1.57f, new Vector2(163f, 0f), new Vector2(1.2f, 0.3f), SpriteEffects.None, 0f);
			Menu.fadeInAlpha = 0f;
			for (int l = 0; l < num + 1; l++)
			{
				this.DrawItemsPC(vector5 + new Vector2(l / num13 * 460, (float)l * num7 - (float)(l / num13 * num13) * num7), toggleOffset, l, num4);
			}
			if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse && Menu.confirmingResolution <= 0f)
			{
				for (int m = 0; m < num + 1; m++)
				{
					if (m == this.curMenuOption)
					{
						vector = vector5 + new Vector2(-8f, 10f) + new Vector2(m / num13 * 460, (float)m * num7 - (float)(m / num13 * num13) * num7);
					}
				}
				Menu.cursorPos += (vector - Menu.cursorPos) * Game1.HudTime * 20f;
				Game1.hud.DrawCursor(Menu.cursorPos, 0.6f, Color.White, flip: false);
				vector = Menu.cursorPos;
			}
			if (this.curMenuOption < num + 1)
			{
				int num14 = Game1.screenWidth / 2 + 40;
				int height = (int)(Game1.smallFont.MeasureString(this.optionDesc).Y * 0.7f) + 40;
				Vector2 vector8 = ((vector == Vector2.Zero) ? (Game1.hud.mousePos + new Vector2(50f, 70f)) : (vector + new Vector2(10f, 40f)));
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && Game1.hud.mousePos.X > (float)(Game1.screenWidth / 2))
				{
					vector8.X -= num14 + 60;
				}
				vector8.X = MathHelper.Clamp(vector8.X, 40f, Game1.screenWidth - num14 - 40);
				Game1.smallText.Color = ((vector == Vector2.Zero) ? new Color(1f, 1f, 1f, Menu.fadeInAlpha) : Color.White);
				Game1.hud.DrawMiniBorder(vector8, num14, height, Game1.smallText.Color, 0.95f);
				Game1.smallText.DrawText(vector8 + new Vector2(20f, 20f), this.optionDesc, num4, 0f, TextAlign.Left);
			}
			if (Menu.confirmingResolution > 1f)
			{
				Menu.screenFade = Math.Min(Menu.screenFade + Game1.HudTime * 4f, 1f);
				int num15 = 500;
				this.optionDesc = Game1.smallText.WordWrap(Strings_Options.PCResolutionConfirm, num4, num15 - 40, TextAlign.Center);
				this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, Menu.screenFade * 0.6f));
				Vector2 vector9 = new Vector2(500f, Game1.smallFont.MeasureString(this.optionDesc).Y * num4 + 100f);
				Game1.hud.DrawMiniBorder(new Vector2((float)Game1.screenWidth - vector9.X, (float)Game1.screenHeight - vector9.Y) / 2f, (int)vector9.X, (int)vector9.Y, new Color(1f, 1f, 1f, Menu.screenFade), 0.95f);
				Game1.smallText.Color = new Color(1f, 1f, 1f, Menu.screenFade);
				Game1.smallText.DrawText(new Vector2((float)Game1.screenWidth - vector9.X, (float)Game1.screenHeight - vector9.Y) / 2f + new Vector2(20f, 20f), this.optionDesc, num4);
				Game1.smallText.DrawText(new Vector2((float)Game1.screenWidth - vector9.X, (float)Game1.screenHeight + vector9.Y) / 2f + new Vector2(0f, -50f), (int)Menu.confirmingResolution + "...", num4, num15, TextAlign.Center);
				if (Game1.pcManager.DrawMouseButton(new Vector2((float)Game1.screenWidth + vector9.X, (float)Game1.screenHeight + vector9.Y) / 2f - new Vector2(60f, 60f), 0.8f, new Color(1f, 1f, 1f, Menu.screenFade), 2, draw: true) || Game1.hud.KeySelect)
				{
					Sound.PlayCue("menu_confirm");
					Game1.hud.KeySelect = false;
					Menu.confirmingResolution = 0f;
					this.curMenuOption = num + 2;
					Menu.prevResolution = Game1.settings.Resolution;
					Menu.prevFullScreen = Game1.settings.FullScreen;
				}
				if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
				{
					Menu.cursorPos += (new Vector2((float)Game1.screenWidth + vector9.X, (float)Game1.screenHeight + vector9.Y) / 2f - new Vector2(50f, 38f) - Menu.cursorPos) * Game1.HudTime * 20f;
					Game1.hud.DrawCursor(Menu.cursorPos, 0.6f, Color.White, flip: false);
				}
				if (!Game1.CheckIsActive)
				{
					this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, Menu.screenFade * 0.6f));
				}
			}
		}

		private void DrawItemsPC(Vector2 pos, float toggleOffset, int id, float textSize)
		{
			switch (id)
			{
				case 0:
					{
						Game1.smallText.DrawText(pos, Strings_Options.PCResolutionTitle, textSize);
						List<Vector2> resolutions = this.GetResolutions();
						int num2 = this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.PCResolutionTitle, resolutions[Game1.hud.equipSelection].X + " x " + resolutions[Game1.hud.equipSelection].Y, Strings_Options.PCResolution, textSize);
						Game1.hud.equipSelection = (int)MathHelper.Clamp(Game1.hud.equipSelection + num2, 0f, resolutions.Count - 1);
						break;
					}
				case 1:
					Game1.smallText.DrawText(pos, Strings_Options.PCFullscreenTitle, textSize);
					if (this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.PCFullscreenTitle, Game1.settings.FullScreen ? Strings_Options.PCEnabled : Strings_Options.PCDisabled, Strings_Options.PCFullScreen, textSize) != 0)
					{
						Game1.settings.FullScreen = !Game1.settings.FullScreen;
					}
					break;
				case 2:
					Game1.smallText.DrawText(pos, Strings_Options.PCBlurTitle, textSize);
					if (this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.PCBlurTitle, Game1.settings.DepthOfField ? Strings_Options.PCEnabled : Strings_Options.PCDisabled, Strings_Options.PCBlur, textSize) != 0)
					{
						Game1.settings.DepthOfField = !Game1.settings.DepthOfField;
					}
					break;
				case 3:
					Game1.smallText.DrawText(pos, Strings_Options.PCPostProcessTitle, textSize);
					if (this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.PCPostProcessTitle, Game1.settings.Bloom ? Strings_Options.PCEnabled : Strings_Options.PCDisabled, Strings_Options.PCPostProcess, textSize) != 0)
					{
						Game1.settings.Bloom = !Game1.settings.Bloom;
					}
					break;
				case 4:
					Game1.smallText.DrawText(pos, Strings_Options.PCWeatherTitle, textSize);
					if (this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.PCWeatherTitle, Game1.settings.WeatherOn ? Strings_Options.PCEnabled : Strings_Options.PCDisabled, Strings_Options.PCWeather, textSize) != 0)
					{
						Game1.settings.WeatherOn = !Game1.settings.WeatherOn;
					}
					break;
				case 5:
					Game1.smallText.DrawText(pos, Strings_Options.PCStutterTitle, textSize);
					if (this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.PCStutterTitle, Game1.settings.UnlockedTime ? Strings_Options.PCEnabled : Strings_Options.PCDisabled, Strings_Options.PCStutter, textSize) != 0)
					{
						Game1.settings.UnlockedTime = !Game1.settings.UnlockedTime;
					}
					break;
				case 6:
					Game1.smallText.DrawText(pos, Strings_Options.PCPortraitTitle, textSize);
					if (this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.PCPortraitTitle, Game1.settings.HiQualityPortraits ? Strings_Options.PCPortraitOption1 : Strings_Options.PCPortraitOption0, Strings_Options.PCPortrait, textSize) != 0)
					{
						Game1.settings.HiQualityPortraits = !Game1.settings.HiQualityPortraits;
					}
					break;
				case 8:
					{
						Game1.smallText.DrawText(pos, Strings_Options.SoundTitle, textSize);
						int num = this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.SoundTitle, (int)Math.Round(Sound.masterSFXVolume * 100f) / 10 * 10 + " %", Strings_Options.Sound, textSize);
						Sound.masterSFXVolume = MathHelper.Clamp(Sound.masterSFXVolume + (float)num * 0.1f, 0f, 1f);
						if (num != 0)
						{
							Sound.SetSFXVolume(Sound.masterSFXVolume);
						}
						break;
					}
				case 9:
					{
						Game1.smallText.DrawText(pos, Strings_Options.MusicTitle, textSize);
						int num = this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.MusicTitle, (int)Math.Round(Sound.masterMusicVolume * 100f) / 10 * 10 + " %", Strings_Options.Music, textSize);
						Sound.masterMusicVolume = MathHelper.Clamp(Sound.masterMusicVolume + (float)num * 0.1f, 0f, 1f);
						break;
					}
				case 11:
					Game1.smallText.DrawText(pos, Strings_Options.ColorBlindTitle, textSize);
					if (this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.ColorBlindTitle, Game1.settings.ColorBlind ? Strings_Options.PCEnabled : Strings_Options.PCDisabled, Strings_Options.ColorBlind, textSize) != 0)
					{
						Game1.settings.ColorBlind = !Game1.settings.ColorBlind;
					}
					break;
				case 12:
					Game1.smallText.DrawText(pos, Strings_Options.AutoAdvanceTitle, textSize);
					if (this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.AutoAdvanceTitle, Game1.settings.AutoAdvance ? Strings_Options.PCEnabled : Strings_Options.PCDisabled, Strings_Options.AutoAdvance, textSize) != 0)
					{
						Game1.settings.AutoAdvance = !Game1.settings.AutoAdvance;
					}
					break;
				case 13:
					Game1.smallText.DrawText(pos, Strings_Options.AutoHealTitle, textSize);
					if (Game1.stats.gameDifficulty > 0)
					{
						if (this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.AutoHealTitle, Game1.settings.AutoHeal ? Strings_Options.PCEnabled : Strings_Options.PCDisabled, Strings_Options.AutoHeal2, textSize) != 0)
						{
							Game1.settings.AutoHeal = !Game1.settings.AutoHeal;
						}
					}
					else
					{
						this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.AutoHealTitle, Strings_Options.PCEnabledLocked, Strings_Options.AutoHeal1, textSize);
					}
					break;
				case 14:
					Game1.smallText.DrawText(pos, Strings_Options.AutoLevelTitle, textSize);
					if (Game1.stats.gameDifficulty > 0)
					{
						if (this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.AutoLevelTitle, Game1.settings.AutoLevelUp ? Strings_Options.PCEnabled : Strings_Options.PCDisabled, Strings_Options.AutoLevel2, textSize) != 0)
						{
							Game1.settings.AutoLevelUp = !Game1.settings.AutoLevelUp;
						}
					}
					else
					{
						this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.AutoLevelTitle, Strings_Options.PCEnabledLocked, Strings_Options.AutoLevel1, textSize);
					}
					break;
				case 15:
					if (Game1.gameMode != 0)
					{
						Game1.smallText.DrawText(pos, Strings_Options.DifficultyTitle, textSize);
						string selectionOption = string.Empty;
						switch (Game1.stats.gameDifficulty)
						{
							case 0:
								selectionOption = Strings_Options.Difficulty0;
								break;
							case 1:
								selectionOption = Strings_Options.Difficulty1;
								break;
							case 2:
								selectionOption = Strings_Options.Difficulty2;
								break;
							case 3:
								selectionOption = Strings_Options.Difficulty3;
								break;
						}
						int num4 = this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.DifficultyTitle, selectionOption, Strings_Options.DifficultyChange, textSize);
						if (num4 < 0)
						{
							Game1.stats.gameDifficulty = (byte)Math.Max(Game1.stats.gameDifficulty - 1, 0);
						}
						else if (num4 > 0)
						{
							Game1.stats.gameDifficulty = (byte)Math.Min(Game1.stats.gameDifficulty + 1, 3);
						}
					}
					break;
				case 17:
					{
						Game1.smallText.DrawText(pos, Strings_Options.PCControlsTitle, textSize);
						int num3 = this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.PCControlsTitle, Strings_Options.ResourceManager.GetString("PCControlsOption" + Game1.settings.InputMethod), Strings_Options.PCControls, textSize);
						Game1.settings.InputMethod += num3;
						if (Game1.settings.InputMethod < 0)
						{
							Game1.settings.InputMethod += 3;
						}
						else if (Game1.settings.InputMethod > 2)
						{
							Game1.settings.InputMethod -= 3;
						}
						break;
					}
				case 18:
					Game1.smallText.DrawText(pos, Strings_Options.RumbleTitle, textSize);
					if (this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.RumbleTitle, Game1.settings.Rumble ? Strings_Options.PCEnabled : Strings_Options.PCDisabled, Strings_Options.Rumble, textSize) != 0)
					{
						Game1.settings.Rumble = !Game1.settings.Rumble;
					}
					break;
				case 19:
					Game1.smallText.DrawText(pos, Strings_Options.AutoFireTitle, textSize);
					if (Game1.stats.gameDifficulty > 0)
					{
						if (this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.AutoFireTitle, Game1.settings.AutoCombo ? Strings_Options.PCEnabled : Strings_Options.PCDisabled, Strings_Options.AutoFire2, textSize) != 0)
						{
							Game1.settings.AutoCombo = !Game1.settings.AutoCombo;
						}
					}
					else
					{
						this.DrawToggleCursors(pos, toggleOffset, id, Strings_Options.AutoFireTitle, Strings_Options.PCEnabledLocked, Strings_Options.AutoFire1, textSize);
					}
					break;
				case 7:
				case 10:
				case 16:
					break;
			}
		}

		private int DrawToggleCursors(Vector2 pos, float toggleOffset, int selectionID, string selectionTitle, string selectionOption, string selectionDesc, float textSize)
		{
			float val = Game1.smallFont.MeasureString(selectionTitle).X * textSize + 30f;
			float x = pos.X;
			pos.X += Math.Max(val, toggleOffset);
			float num = Game1.smallFont.MeasureString(selectionOption).X * textSize;
			float num2 = 1f;
			float num3 = 1f;
			int result = 0;
			if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				if (selectionID == this.curMenuOption)
				{
					this.sprite.Draw(this.nullTex, new Rectangle((int)x - 20, (int)pos.Y - 3, 455, 35), new Color(1f, 1f, 1f, 0.2f));
					this.optionDesc = Game1.smallText.WordWrap(selectionDesc, 0.7f, Game1.screenWidth / 2, TextAlign.Center);
					if (Menu.confirmingResolution <= 0f)
					{
						if (Game1.hud.KeyLeft)
						{
							Sound.PlayCue("menu_click");
							result = -1;
							Game1.hud.KeyLeft = false;
						}
						if (Game1.hud.KeyRight)
						{
							Sound.PlayCue("menu_click");
							result = 1;
							Game1.hud.KeyRight = false;
						}
					}
				}
			}
			else if (Menu.confirmingResolution <= 0f && new Rectangle((int)x - 15, (int)pos.Y - 3, 445, 35).Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y))
			{
				Menu.fadeInAlpha = 1f;
				this.sprite.Draw(this.nullTex, new Rectangle((int)x - 20, (int)pos.Y - 3, 455, 35), new Color(1f, 1f, 1f, 0.2f));
				if (selectionID != Menu.settingSelection)
				{
					Sound.PlayCue("menu_click");
					Menu.settingSelection = selectionID;
					this.optionDesc = Game1.smallText.WordWrap(selectionDesc, 0.7f, Game1.screenWidth / 2, TextAlign.Center);
				}
				if (new Rectangle((int)pos.X - 20, (int)pos.Y - 3, 40, 35).Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y))
				{
					if (!Game1.pcManager.IsMouseLeftHeld())
					{
						num2 = 1.5f;
					}
					if (Game1.pcManager.leftMouseClicked)
					{
						Game1.pcManager.leftMouseClicked = false;
						Sound.PlayCue("menu_click");
						result = -1;
					}
				}
				if (new Rectangle((int)pos.X + 20, (int)pos.Y - 3, (int)num + 40, 35).Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y))
				{
					if (!Game1.pcManager.IsMouseLeftHeld())
					{
						num3 = 1.5f;
					}
					if (Game1.pcManager.leftMouseClicked)
					{
						Game1.pcManager.leftMouseClicked = false;
						Sound.PlayCue("menu_click");
						result = 1;
					}
				}
			}
			float num4 = Math.Abs((float)Math.Sin(Game1.hud.pulse * 2f)) * 3f;
			Color white = Color.White;
			this.sprite.Draw(Game1.navManager.NavTex, pos + new Vector2(0f - num4, 14f), new Rectangle(752, 0, 60, 48), white * (num2 - 0.5f), 4.71f, new Vector2(30f, 24f), 0.5f * num2, SpriteEffects.None, 0f);
			Game1.smallText.DrawText(pos + new Vector2(20f, 0f), selectionOption, textSize);
			this.sprite.Draw(Game1.navManager.NavTex, pos + new Vector2(num4 + num + 40f, 14f), new Rectangle(752, 0, 60, 48), white * (num3 - 0.5f), 1.57f, new Vector2(30f, 24f), 0.5f * num3, SpriteEffects.None, 0f);
			return result;
		}

		private bool CheckValidItem(int id)
		{
			switch (id)
			{
				case 7:
				case 10:
				case 16:
					return true;
				case 15:
					return Game1.gameMode == Game1.GameModes.MainMenu;
				default:
					return false;
			}
		}

		private float DrawKeyboardMapping(Vector2 pos, int id, float columnWidth, float textSize, bool isMapping)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			switch (id)
			{
				default:
					empty = Strings_ManualGraphics.KeyboardUp;
					empty2 = "up";
					break;
				case 1:
					empty = Strings_ManualGraphics.KeyboardLeft;
					empty2 = "left";
					break;
				case 2:
					empty = Strings_ManualGraphics.KeyboardDown;
					empty2 = "down";
					break;
				case 3:
					empty = Strings_ManualGraphics.KeyboardRight;
					empty2 = "right";
					break;
				case 4:
					empty = Strings_ManualGraphics.ButtonA;
					empty2 = "a";
					break;
				case 5:
					empty = Strings_ManualGraphics.ButtonX;
					empty2 = "x";
					break;
				case 6:
					empty = Strings_ManualGraphics.ButtonY;
					empty2 = "y";
					break;
				case 7:
					empty = Strings_ManualGraphics.ButtonB;
					empty2 = "b";
					break;
				case 8:
					empty = Strings_ManualGraphics.ButtonLB;
					empty2 = "lb";
					break;
				case 9:
					empty = Strings_ManualGraphics.ButtonRB;
					empty2 = "rb";
					break;
				case 10:
					empty = Strings_ManualGraphics.ButtonLT;
					empty2 = "lt";
					break;
				case 11:
					empty = Strings_ManualGraphics.ButtonRT;
					empty2 = "rt";
					break;
				case 12:
					empty = Strings_ManualGraphics.ButtonStart;
					empty2 = "start";
					break;
				case 13:
					empty = Strings_ManualGraphics.ButtonBack;
					empty2 = "back";
					break;
				case 14:
					empty = Strings_ManualGraphics.KeyboardInvLeft;
					empty2 = "invleft";
					break;
				case 15:
					empty = Strings_ManualGraphics.KeyboardInvRight;
					empty2 = "invright";
					break;
				case 16:
					empty = Strings_ManualGraphics.KeyboardInvSubLeft;
					empty2 = "invsubleft";
					break;
				case 17:
					empty = Strings_ManualGraphics.KeyboardInvSubRight;
					empty2 = "invsubright";
					break;
				case 18:
					empty = Strings_ManualGraphics.KeyboardMap;
					empty2 = "rclick";
					break;
			}
			float num = Game1.smallFont.MeasureString(empty).Y * textSize;
			if (pos.Y > (float)(Menu.topEdge + 118) && pos.Y < (float)(Menu.bottomEdge - 130))
			{
				Rectangle destinationRectangle = new Rectangle((int)pos.X, (int)pos.Y, (int)(columnWidth * 3f), (int)num + 4);
				if (this.configuringControls == 1 && destinationRectangle.Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y))
				{
					if (this.curMenuOption != id)
					{
						Sound.PlayCue("menu_click");
					}
					this.curMenuOption = id;
				}
				if (this.curMenuOption == id)
				{
					this.sprite.Draw(this.nullTex, destinationRectangle, Color.White * 0.5f);
					if (this.configuringControls == 2)
					{
						if (Menu.screenFade >= 1f && Game1.pcManager.inputKeyList[empty2].AssignFunction())
						{
							Game1.hud.KeySelect = false;
							Sound.PlayCue("menu_confirm");
							this.configuringControls = 1;
							Menu.screenFade = 0f;
							this.optionDesc = Game1.smallText.WordWrap(Strings_Options.PCCustomizeHelp1, 0.7f, 460f, TextAlign.Center);
						}
					}
					else if (Game1.pcManager.leftMouseClicked)
					{
						Game1.pcManager.leftMouseClicked = false;
						if (destinationRectangle.Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y))
						{
							Sound.PlayCue("menu_confirm");
							this.configuringControls = 2;
							Menu.screenFade = 0f;
							this.optionDesc = Game1.smallText.WordWrap(Strings_Options.PCCustomizeHelp1, 0.7f, 460f, TextAlign.Center);
						}
					}
					else
					{
						Game1.pcManager.leftMouseClicked = false;
						if (Keyboard.GetState().IsKeyDown(Keys.Delete) && Game1.pcManager.inputKeyList[empty2].ClearMapping())
						{
							Sound.PlayCue("menu_cancel");
						}
					}
				}
				Game1.smallText.DrawText(pos, empty, textSize);
				InputKey inputKey = Game1.pcManager.inputKeyList[empty2];
				Game1.smallText.DrawText(pos + new Vector2(columnWidth, 0f), inputKey.MappedKeyString(), textSize);
				Dictionary<Vector3, string> buttonList = new Dictionary<Vector3, string>();
				Game1.smallText.DrawButtonText(pos + new Vector2(columnWidth * 2f, 0f), Game1.smallText.WordWrap(inputKey.MappedMouseString(), textSize, 1000f, buttonList, TextAlign.Left), textSize, buttonList, bounce: true, 1000f, TextAlign.Left);
				this.sprite.Draw(this.hudTex[2], new Vector2(Game1.screenWidth / 2, pos.Y + num), new Rectangle(0, 502, 326, 18), Color.White * 0.7f, 0f, new Vector2(163f, 0f), new Vector2(3f, 0.3f), SpriteEffects.None, 0f);
			}
			return num + 4f;
		}

		private List<Vector2> GetResolutions()
		{
			return Game1.GetResolutions();
		}

		private void InitHUDAdjust()
		{
			Menu.prevScreenTopOffset = Game1.hud.screenTopOffset;
			Menu.prevScreenLeftOffset = Game1.hud.screenLeftOffset;
			Menu.prevHudScale = Game1.hud.hudScale;
			Menu.prevHudDetails = Game1.hud.hudDetails;
			this.menuMode = MenuMode.HudAdjust;
			this.curMenuPage = 0;
			this.curMenuOption = 0;
			this.SetHelpTextures();
			this.buttonText = Game1.smallText.WordWrap(Strings_Options.HUDControls, 0.7f, Game1.screenWidth, this.buttonTextButtonList, TextAlign.LeftAndCenter);
		}

		private void UnloadHUDAdjust()
		{
			Menu.helpLoaded = false;
			Game1.GetLargeContent().Unload();
			if (Game1.gameMode == Game1.GameModes.MainMenu)
			{
				this.ExitMode(3, 3);
			}
			else
			{
				this.ExitMode(1, 3);
			}
		}

		private void UpdateHudAdjust()
		{
			if (!Menu.helpLoaded)
			{
				return;
			}
			if (Game1.pcManager.inputDevice == InputDevice.GamePad)
			{
				Game1.hud.screenLeftOffset += (int)(Game1.hud.GetCurstate().ThumbSticks.Left.X * 8f);
				Game1.hud.screenTopOffset -= (int)(Game1.hud.GetCurstate().ThumbSticks.Left.Y * 8f);
				Game1.hud.hudScale += Game1.hud.GetCurstate().ThumbSticks.Right.X / 20f;
				Game1.hud.hudScale += Game1.hud.GetCurstate().ThumbSticks.Right.Y / 20f;
			}
			else if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				if (Game1.hud.mousePos.Y < (float)Game1.screenHeight * 0.5f + 100f)
				{
					if (Game1.pcManager.IsMouseLeftHeld())
					{
						Game1.hud.screenLeftOffset += (int)Game1.pcManager.mouseDiff.X;
						Game1.hud.screenTopOffset += (int)Game1.pcManager.mouseDiff.Y;
					}
					if (Game1.pcManager.IsMouseRightHeld())
					{
						Game1.hud.hudScale = MathHelper.Clamp(Game1.hud.hudScale + (Game1.pcManager.mouseDiff.X + Game1.pcManager.mouseDiff.Y) * 0.01f, 0.5f, 2f);
					}
				}
			}
			else if (Game1.pcManager.inputDevice == InputDevice.KeyboardOnly)
			{
				if (Game1.pcManager.IsHudHeld("KeyLeft"))
				{
					Game1.hud.screenLeftOffset--;
				}
				else if (Game1.pcManager.IsHudHeld("KeyRight"))
				{
					Game1.hud.screenLeftOffset++;
				}
				if (Game1.pcManager.IsHudHeld("KeyUp"))
				{
					Game1.hud.screenTopOffset--;
				}
				else if (Game1.pcManager.IsHudHeld("KeyDown"))
				{
					Game1.hud.screenTopOffset++;
				}
				if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
				{
					Game1.hud.hudScale += 0.02f;
				}
				else if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
				{
					Game1.hud.hudScale -= 0.02f;
				}
			}
			if (Game1.hud.screenLeftOffset < -60)
			{
				Game1.hud.screenLeftOffset = -60;
			}
			if (Game1.hud.screenTopOffset < 0)
			{
				Game1.hud.screenTopOffset = 0;
			}
			if (Game1.hud.screenLeftOffset > Game1.screenWidth / 4)
			{
				Game1.hud.screenLeftOffset = Game1.screenWidth / 4;
			}
			if (Game1.hud.screenTopOffset > Game1.screenHeight / 4)
			{
				Game1.hud.screenTopOffset = Game1.screenHeight / 4;
			}
			Game1.hud.hudScale = MathHelper.Clamp(Game1.hud.hudScale, 0.5f, 2f);
			if (Game1.hud.KeyCancel)
			{
				Game1.hud.screenTopOffset = Menu.prevScreenTopOffset;
				Game1.hud.screenLeftOffset = Menu.prevScreenLeftOffset;
				Game1.hud.hudScale = Menu.prevHudScale;
				Game1.hud.hudDetails = Menu.prevHudDetails;
				this.UnloadHUDAdjust();
			}
			else if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				if (Game1.hud.KeySelect)
				{
					if (!Game1.GamerServices || !Game1.IsTrial)
					{
						Game1.storage.Write(0, -1);
						Game1.storage.DisposeContainer();
						if (Game1.storage.storeResult != StoreResult.Saved)
						{
							Sound.PlayCue("fidget_fail");
							Game1.menu.SelectStorage();
							return;
						}
						Menu.savingAlpha = 1f;
					}
					this.UnloadHUDAdjust();
				}
				if (Game1.hud.KeyX)
				{
					Sound.PlayCue("menu_confirm");
					Game1.hud.hudDetails = true;
					if (!Game1.standardDef)
					{
						Game1.hud.screenLeftOffset = Game1.screenWidth / 9;
						Game1.hud.screenTopOffset = Game1.screenHeight / 9;
						Game1.hud.hudScale = 1f;
					}
					else
					{
						Game1.hud.screenLeftOffset = Game1.screenWidth / 6;
						Game1.hud.screenTopOffset = Game1.screenHeight / 8;
						Game1.hud.hudScale = 1.4f;
					}
				}
				if (Game1.hud.KeyY)
				{
					if (Game1.hud.hudDetails)
					{
						Sound.PlayCue("menu_cancel");
					}
					else
					{
						Sound.PlayCue("menu_confirm");
					}
					Game1.hud.hudDetails = !Game1.hud.hudDetails;
				}
			}
			Game1.navManager.navScale = 0.5f;
		}

		private void DrawHudAdjust()
		{
			this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, 0.75f));
			this.sprite.Draw(this.nullTex, new Rectangle(Menu.leftEdge, Menu.topEdge, Menu.rightEdge - Menu.leftEdge, 2), new Color(1f, 1f, 1f, 0.25f));
			this.sprite.Draw(this.nullTex, new Rectangle(Menu.leftEdge, Menu.topEdge, 2, Menu.bottomEdge - Menu.topEdge), new Color(1f, 1f, 1f, 0.25f));
			this.sprite.Draw(this.nullTex, new Rectangle(Menu.rightEdge, Menu.topEdge, 2, Menu.bottomEdge - Menu.topEdge), new Color(1f, 1f, 1f, 0.25f));
			this.sprite.Draw(this.nullTex, new Rectangle(Menu.leftEdge, Menu.bottomEdge, Menu.rightEdge - Menu.leftEdge, 2), new Color(1f, 1f, 1f, 0.25f));
			this.sprite.Draw(this.nullTex, new Rectangle(Menu.leftEdge, Game1.screenHeight / 2, Menu.rightEdge - Menu.leftEdge, 2), new Color(1f, 1f, 1f, 0.1f));
			this.sprite.Draw(this.nullTex, new Rectangle(Game1.screenWidth / 2, Menu.topEdge, 2, Menu.bottomEdge - Menu.topEdge), new Color(1f, 1f, 1f, 0.1f));
			Game1.hud.DrawMenuHud(Game1.pManager);
			if (Menu.helpLoaded)
			{
				Vector2 vector = new Vector2(Game1.screenWidth - 159, Game1.screenHeight - 110) / 2f;
				Game1.hud.DrawMiniBorder(new Vector2(Game1.screenWidth - 300, Game1.screenHeight - 200) / 2f, 300, 250, Color.White, 0.8f);
				this.sprite.Draw(Menu.miscTex, vector, new Rectangle(451, 0, 159, 145), Color.White);
				if (Game1.hud.hudDetails)
				{
					this.sprite.Draw(Menu.miscTex, vector + new Vector2(25f, 140f), new Rectangle(450, 145, 110, 55), Color.White);
					this.sprite.Draw(Menu.miscTex, vector + new Vector2(55f, -35f), new Rectangle(560, 145, 50, 55), Color.White);
				}
				this.buttonText = Game1.smallText.WordWrap((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse) ? Strings_Options.PCHUDControls1 : ((Game1.pcManager.inputDevice == InputDevice.GamePad) ? Strings_Options.HUDControls : Strings_Options.PCHUDControlsKeyboard), 0.7f, Game1.screenWidth, this.buttonTextButtonList, TextAlign.LeftAndCenter);
				int num = (int)(Game1.smallFont.MeasureString(this.buttonText).X * 0.7f) + 20;
				int num2 = (int)(Game1.smallFont.MeasureString(this.buttonText).Y * 0.7f) + 10;
				Vector2 vector2 = new Vector2((Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse) ? ((float)(Game1.screenWidth / 2)) : ((float)Game1.screenWidth * 0.25f), (float)Game1.screenHeight * 0.9f - (float)num2);
				Game1.hud.DrawMiniBorder(vector2 - new Vector2(num / 2, 0f), num, num2, Color.White, 0.8f);
				Game1.smallText.Color = Color.White;
				Game1.smallText.DrawButtonText(vector2 + new Vector2(-num / 2 + 10, 5f), this.buttonText, 0.7f, this.buttonTextButtonList, bounce: false, 410f, TextAlign.Left);
				if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
				{
					return;
				}
				if (Game1.pcManager.DrawMouseButton(new Vector2((float)Game1.screenWidth * 0.9f - 30f, (float)Game1.screenHeight * 0.9f - 40f), 0.8f, Color.White, 2, draw: true))
				{
					if (!Game1.GamerServices || !Game1.IsTrial)
					{
						Game1.storage.Write(0, -1);
						Game1.storage.DisposeContainer();
						if (Game1.storage.storeResult != StoreResult.Saved)
						{
							Sound.PlayCue("fidget_fail");
							Game1.menu.SelectStorage();
							return;
						}
						Menu.savingAlpha = 1f;
					}
					this.UnloadHUDAdjust();
				}
				if (Game1.pcManager.DrawMouseButton(new Vector2((float)Game1.screenWidth * 0.9f - 100f, (float)Game1.screenHeight * 0.9f - 40f), 0.8f, Color.White, 2, draw: true, Strings_Options.PCCustomizeRestore, TextAlign.Right))
				{
					Sound.PlayCue("menu_confirm");
					Game1.hud.hudDetails = true;
					if (!Game1.standardDef)
					{
						Game1.hud.screenLeftOffset = Game1.screenWidth / 9;
						Game1.hud.screenTopOffset = Game1.screenHeight / 9;
						Game1.hud.hudScale = 1f;
					}
					else
					{
						Game1.hud.screenLeftOffset = Game1.screenWidth / 6;
						Game1.hud.screenTopOffset = Game1.screenHeight / 8;
						Game1.hud.hudScale = 1.4f;
					}
				}
				if (Game1.pcManager.DrawMouseButton(new Vector2(Game1.screenWidth / 2, vector.Y + 200f), 0.8f, Color.White, 1, draw: true, Strings_Options.PCHUDControls2, TextAlign.Center))
				{
					if (Game1.hud.hudDetails)
					{
						Sound.PlayCue("menu_cancel");
					}
					else
					{
						Sound.PlayCue("menu_confirm");
					}
					Game1.hud.hudDetails = !Game1.hud.hudDetails;
				}
			}
			else
			{
				Menu.textAlpha = 200f;
				Game1.DrawLoad(this.sprite, new Vector2(Game1.screenWidth / 2, Menu.bottomEdge - Game1.screenHeight / 4));
			}
		}

		private void InitCredits()
		{
			this.menuMode = MenuMode.Credits;
			Menu.helpLoaded = false;
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadCredits), new TaskFinishedDelegate(LoadingHelpFinished)));
			Menu.scrollMask = new RenderTarget2D(Game1.graphics.GraphicsDevice, (int)Menu.windowWidth, Menu.bottomEdge - Menu.topEdge - 80);
			Menu.scrollSource = new RenderTarget2D(Game1.graphics.GraphicsDevice, (int)Menu.windowWidth, Menu.bottomEdge - Menu.topEdge - 80);
			this.DrawScrollMask(Menu.scrollMask.Width, Menu.scrollMask.Height, 60, Game1.graphics.GraphicsDevice);
			this.curMenuPage = 0;
			this.curMenuOption = 0;
		}

		private void LoadCredits()
		{
			float num = 0.8f;
			string text = "\n\n\n" + Strings_MainMenu.Options4 + "\n\n";
			for (int i = 0; i < 9; i++)
			{
				text = text + "\n\n\n\n" + Strings_Credits.ResourceManager.GetString("credits" + $"{i:D3}");
			}
			text = text.Replace("Dust/Jin/Cassius", "Dust");
			float num2 = (float)Game1.smallFont.LineSpacing * num;
			string[] array = text.Split('\n');
			this.scrollString.Clear();
			for (int j = 0; j < array.Length; j++)
			{
				this.scrollString.Add(j, Game1.smallText.WordWrap(array[j], num, Menu.windowWidth - 100f, TextAlign.Center));
			}
			Menu.maxScroll = (float)(-Game1.screenHeight) * 0.4f + 20f;
			Menu.maxScroll += (float)array.Length * num2;
			Menu.listScroll = 50f * num;
			this.buttonText = Game1.smallText.WordWrap(Strings_HudInv.Return, 0.7f, Menu.rightEdge - Menu.leftEdge - 30, this.buttonTextButtonList, TextAlign.Left);
		}

		public void ExitMode(int setPage, int setOption)
		{
			Sound.PlayCue("menu_cancel");
			Game1.hud.KeySelect = false;
			Game1.hud.KeyCancel = false;
			Menu.fadeInAlpha = 1f;
			Menu.textAlpha = 100f;
			this.menuMode = MenuMode.None;
			this.curMenuPage = setPage;
			this.curMenuOption = setOption;
			this.PopulateMenu();
			this.SetMenuSelectMove();
		}

		private void UpdateCredits()
		{
			Menu.listScroll -= Game1.HudTime * 40f;
			this.UpdateHelpScroll(1f);
			if (Game1.hud.KeyCancel)
			{
				Menu.helpLoaded = false;
				Game1.GetLargeContent().Unload();
				if (Game1.gameMode == Game1.GameModes.MainMenu)
				{
					this.ExitMode(3, 4);
				}
				else
				{
					this.ExitMode(1, 4);
				}
			}
		}

		private void DrawCredits(float alpha)
		{
			Game1.hud.DrawBorder(new Vector2(Menu.leftEdge, Menu.topEdge), Menu.rightEdge - Menu.leftEdge, Menu.bottomEdge - Menu.topEdge, Color.White, 1f, 0);
			if (!Menu.helpLoaded)
			{
				Menu.textAlpha = 200f;
				Game1.DrawLoad(this.sprite, new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2));
				return;
			}
			this.DrawScroll(Game1.graphics.GraphicsDevice, new Vector2(Menu.leftEdge + 60, Menu.topEdge + 30));
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				Game1.hud.DrawScrollBarMouse(new Vector2(Menu.rightEdge - 35, Menu.topEdge + 55), ref Menu.listScroll, Menu.bottomEdge - Menu.topEdge - 130, -(int)Menu.maxScroll, 1, 1f);
				Game1.pcManager.MouseWheelAdjust(ref Menu.listScroll, -120f, 0f - Menu.maxScroll, 0f);
				if (Game1.pcManager.DrawMouseButton(new Vector2(Menu.leftEdge + Menu.rightEdge - Menu.leftEdge - 20, Menu.topEdge - 30), 0.8f, Color.White, 0, draw: true))
				{
					Game1.hud.KeyCancel = true;
					this.UpdateCredits();
				}
			}
			else
			{
				Game1.hud.DrawScrollBar(new Vector2(Menu.rightEdge - 35, Menu.topEdge + 55), 0f - Menu.listScroll / Menu.maxScroll, Menu.bottomEdge - Menu.topEdge - 130, 1f);
				Game1.smallText.Color = Color.White;
				Game1.smallText.DrawButtonText(new Vector2(Menu.leftEdge, Menu.bottomEdge - 30), this.buttonText, 0.7f, this.buttonTextButtonList, bounce: false, Menu.rightEdge - Menu.leftEdge, TextAlign.Center);
			}
		}

		private void DrawCreditsText(Vector2 pos, Dictionary<float, string> s, float size, float topEdge, float scrollHeight, float lineWidth)
		{
			float num = (float)Game1.smallFont.LineSpacing * size;
			for (int i = 0; i < s.Count; i++)
			{
				Vector2 pos2 = pos + new Vector2(0f, (float)i * num);
				if (pos2.Y > (0f - num) * 6f && pos2.Y < (float)Game1.screenHeight)
				{
					Game1.smallText.DrawText(pos2, s[i], size);
				}
			}
		}

		private void UpdateFileScreen(ParticleManager pMan)
		{
			Game1.hud.mousePos = Game1.pcManager.GetMouseLoc();
			if (Game1.GamerServices && Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)] == null && Game1.gameMode != 0)
			{
				Game1.hud.isPaused = false;
				Game1.gameMode = Game1.GameModes.Game;
				this.menuMode = MenuMode.None;
				this.prompt = promptDialogue.SignedOut;
				this.ClearPrompt();
				return;
			}
			if (!Menu.savingHelpDisplayed)
			{
				if (Game1.hud.KeySelect)
				{
					Game1.hud.KeySelect = false;
					Sound.PlayCue("menu_confirm");
					Menu.savingHelpDisplayed = true;
					this.buttonText = Game1.smallText.WordWrap(Game1.isPCBuild ? Strings_PC.LoadControls : Strings_MainMenu.LoadControls, 0.8f, Game1.screenWidth, this.buttonTextButtonList, TextAlign.LeftAndCenter);
				}
				return;
			}
			int num = -1;
			for (int i = 0; i < this.fileManage.Length; i++)
			{
				if (this.fileManage[i] == FileManage.CopyFrom)
				{
					num = i;
				}
			}
			if (!this.confirming)
			{
				if (Game1.hud.KeyUp || Game1.hud.KeyDown)
				{
					int num2 = this.tempSaveSlot;
					if (Game1.hud.KeyUp)
					{
						if (this.tempSaveSlot > 0)
						{
							this.tempSaveSlot--;
						}
					}
					else if (Game1.hud.KeyDown && this.tempSaveSlot < Menu.maxFiles - 1)
					{
						this.tempSaveSlot++;
					}
					Game1.hud.KeyUp = (Game1.hud.KeyDown = false);
					if (num2 != this.tempSaveSlot)
					{
						Menu.fadeInAlpha = 1f;
						Sound.PlayCue("menu_click");
					}
					for (int j = 0; j < this.fileManage.Length; j++)
					{
						if (this.fileManage[j] == FileManage.Empty)
						{
							Menu.saveDescription[j] = Game1.smallText.WordWrap(Strings_MainMenu.SaveSlotEmpty, 0.9f, 540f, this.simpleStringButtonList, TextAlign.Left);
						}
					}
				}
				if (Game1.hud.KeyCancel && this.InitCancel(num))
				{
					return;
				}
				if (Game1.hud.KeySelect && Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
				{
					this.InitLoadCopy(num);
				}
				if (num < 0)
				{
					if (Game1.hud.KeyY && Menu.saveDescription[0] != null && this.fileManage[this.tempSaveSlot] != 0 && this.fileManage[this.tempSaveSlot] != FileManage.StorageDeviceNull)
					{
						Sound.PlayCue("menu_click");
						this.confirming = true;
						this.fileManage[this.tempSaveSlot] = FileManage.Delete;
						this.PopulateFileConfirm(Strings_MainMenu.DeleteConfirm);
					}
					if (Game1.hud.KeyX && Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse && Menu.saveDescription[0] != null && this.fileManage[this.tempSaveSlot] == FileManage.Load)
					{
						Sound.PlayCue("menu_click");
						this.fileManage[this.tempSaveSlot] = FileManage.CopyFrom;
					}
					if (Menu.checkGuideTime == 0f)
					{
						if (Game1.isPCBuild)
						{
							if (Game1.storage.device == null)
							{
								this.SelectStorage();
							}
						}
						else if (Game1.hud.OpenMenuButton)
						{
							this.SelectStorage();
						}
					}
				}
			}
			else if ((Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse || this.fileManage[this.tempSaveSlot] == FileManage.Empty) && this.UpdateConfirm() && this.FinalizeConfirm(num))
			{
				return;
			}
			if (this.confirming || num > -1)
			{
				if (Menu.confirmAlpha < 1f)
				{
					Menu.confirmAlpha += Game1.HudTime * 10f;
					if (Menu.confirmAlpha > 1f)
					{
						Menu.confirmAlpha = 1f;
					}
				}
			}
			else
			{
				Menu.confirmAlpha = 0f;
			}
		}

		private void InitLoadCopy(int copying)
		{
			Game1.hud.KeySelect = false;
			if (Menu.saveDescription[0] == null || this.fileManage[this.tempSaveSlot] == FileManage.StorageDeviceNull || this.fileManage[this.tempSaveSlot] == FileManage.FileCorrupt)
			{
				return;
			}
			if (this.fileManage[this.tempSaveSlot] == FileManage.Load || (copying > -1 && copying != this.tempSaveSlot) || (copying == -1 && this.prompt == promptDialogue.Save))
			{
				if ((this.prompt == promptDialogue.Save || copying > -1) && this.tempSaveSlot == 0)
				{
					Sound.PlayCue("shop_fail");
					return;
				}
				Sound.PlayCue("menu_click");
				this.confirming = true;
				Menu.confirmAlpha = 0f;
				if (copying > -1 || this.prompt == promptDialogue.Save)
				{
					this.PopulateFileConfirm((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse) ? Strings_PC.CopyConfirm : Strings_MainMenu.CopyConfirm);
				}
				else if (Game1.gameMode == Game1.GameModes.MainMenu)
				{
					this.PopulateFileConfirm(Strings_MainMenu.ResumeConfirm);
				}
				else
				{
					this.PopulateFileConfirm((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse) ? Strings_PC.LoadConfirm : Strings_MainMenu.LoadConfirm);
				}
			}
			else if (this.fileManage[this.tempSaveSlot] == FileManage.Empty && Game1.gameMode == Game1.GameModes.MainMenu)
			{
				this.confirming = true;
				Menu.confirmAlpha = 0f;
				this.PopulateFileConfirm(Strings_MainMenu.NewGameConfirm);
			}
		}

		private bool InitCancel(int copying)
		{
			Game1.hud.KeySelect = false;
			Sound.PlayCue("menu_cancel");
			if (copying > -1)
			{
				this.fileManage[copying] = FileManage.Load;
				return true;
			}
			for (int i = 0; i < Menu.saveDescription.Length; i++)
			{
				this.saveRegionName[i] = null;
				Menu.saveTitle[i] = null;
				Menu.saveDescription[i] = null;
				this.fileManage[i] = FileManage.Empty;
			}
			Menu.fadeInAlpha = 1f;
			this.menuMode = MenuMode.None;
			this.PopulateMenu();
			this.SetMenuSelectMove();
			return false;
		}

		private void DrawFileScreen()
		{
			this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, 0.5f * Menu.fileAlpha));
			int num = -1;
			for (int i = 0; i < this.fileManage.Length; i++)
			{
				if (this.fileManage[i] == FileManage.CopyFrom)
				{
					num = i;
				}
			}
			float num2 = (float)Math.Abs(Math.Cos(Game1.hud.pulse));
			int height = 40;
			if (Menu.saveTitle[0] != null)
			{
				height = (int)Game1.smallFont.MeasureString(Menu.saveTitle[0]).Y + 10;
			}
			for (int j = 0; j < Menu.maxFiles; j++)
			{
				Color color;
				int num3;
				if (this.fileManage[j] == FileManage.Delete)
				{
					color = new Color(1f, num2, num2, Menu.fileAlpha);
					num3 = (int)(20f * num2);
				}
				else if (this.fileManage[j] == FileManage.CopyFrom)
				{
					color = new Color(MathHelper.Clamp(num2, 0f, 0.75f), 0.75f, 1f, Menu.fileAlpha);
					num3 = (int)(20f * num2);
				}
				else if (j == this.tempSaveSlot)
				{
					color = new Color(1f, 1f, 1f, Menu.fileAlpha);
					num3 = ((!this.confirming) ? ((int)(20f * (1f - Menu.fadeInAlpha))) : ((int)(20f * (1f - Menu.fadeInAlpha) * num2)));
				}
				else
				{
					float num4 = 0.4f * Menu.fileAlpha;
					color = new Color(num4, num4, num4, Menu.fileAlpha);
					num3 = 0;
				}
				int num5 = Game1.screenWidth / 2 - 350 + num3;
				int num6 = (int)((float)Game1.screenHeight * 0.12f + (float)(j * 220) * Menu.fileAlpha - Menu.fileScroll);
				Game1.hud.DrawBorder(new Vector2(num5, num6), 700, 180, color, 0.9f, 0);
				Game1.smallText.Color = new Color((int)color.R, (int)color.G, (int)color.B, (1f - Menu.textAlpha / 100f) * Menu.fileAlpha);
				if ((Menu.fileAlpha < 1f && this.menuMode == MenuMode.FileManage) || Menu.checkGuideTime > 0f)
				{
					Game1.DrawLoad(this.sprite, new Vector2(Game1.screenWidth / 2, num6 + 80));
				}
				else
				{
					if (Menu.saveDescription[j] == null)
					{
						continue;
					}
					Game1.smallText.Color = color * (1f - Menu.textAlpha / 100f) * Menu.fileAlpha;
					float size = 0.9f;
					if (this.fileManage[j] == FileManage.StorageDeviceNull)
					{
						Game1.smallText.DrawButtonText(new Vector2(num5 + 20, num6 + 40), Menu.saveDescription[j], size, this.simpleStringButtonList, bounce: false, 0f, TextAlign.Left);
					}
					else
					{
						Game1.smallText.DrawText(new Vector2(num5 + 20, num6 + 40), Menu.saveDescription[j], size);
					}
					size = 1f;
					int num7 = (int)Game1.smallFont.MeasureString(Menu.saveTitle[j]).X + 60;
					Game1.hud.DrawMiniBorder(new Vector2(num5 - 20, num6 - 25), num7, height, Game1.smallText.Color, 0.95f);
					Game1.smallText.DrawText(new Vector2(num5 - 15, num6 - 20), Menu.saveTitle[j], size, num7, TextAlign.Center);
					if (j == Game1.stats.saveSlot)
					{
						int num8 = (int)Game1.smallFont.MeasureString(Strings_MainMenu.SaveRecent).X + 60;
						Game1.hud.DrawMiniBorder(new Vector2(num5 + 720 - num8, num6 - 25), num8, height, Game1.smallText.Color, 0.95f);
						Game1.smallText.DrawText(new Vector2(num5 + 725 - num8, num6 - 20), Strings_MainMenu.SaveRecent, size, num8, TextAlign.Center);
					}
					if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && !this.confirming && Menu.savingHelpDisplayed && Menu.fileAlpha == 1f && this.fileManage[j] != FileManage.CopyFrom)
					{
						if (Game1.pcManager.mouseInBounds && new Rectangle(num5, num6, 700, 180).Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y) && this.tempSaveSlot != j)
						{
							this.tempSaveSlot = (byte)j;
							Menu.fadeInAlpha = 1f;
							Sound.PlayCue("menu_click");
						}
						if (j == this.tempSaveSlot)
						{
							if (num > -1)
							{
								if (Game1.pcManager.DrawMouseButton(new Vector2(num5 + 640, num6 + 115), 0.8f, color, 2, draw: true))
								{
									Game1.hud.KeySelect = true;
									if (this.UpdateConfirm())
									{
										if (this.fileManage[j] == FileManage.Empty)
										{
											this.FinalizeConfirm(num);
										}
										else
										{
											this.InitLoadCopy(num);
										}
									}
								}
							}
							else
							{
								if (this.fileManage[j] == FileManage.Load)
								{
									if (Game1.pcManager.DrawMouseButton(new Vector2(num5 + 520, num6 + 115), 0.8f, color, 6, draw: true) && Menu.saveDescription[0] != null && this.fileManage[this.tempSaveSlot] != 0 && this.fileManage[this.tempSaveSlot] != FileManage.StorageDeviceNull)
									{
										Sound.PlayCue("menu_click");
										this.confirming = true;
										this.fileManage[this.tempSaveSlot] = FileManage.Delete;
										this.PopulateFileConfirm((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse) ? Strings_PC.DeleteConfirm : Strings_MainMenu.DeleteConfirm);
									}
									if (Game1.pcManager.DrawMouseButton(new Vector2(num5 + 580, num6 + 115), 0.8f, color, 5, draw: true) && Menu.saveDescription[0] != null && this.fileManage[this.tempSaveSlot] == FileManage.Load)
									{
										Game1.hud.KeySelect = false;
										Sound.PlayCue("menu_click");
										this.fileManage[this.tempSaveSlot] = FileManage.CopyFrom;
									}
								}
								if (((this.fileManage[j] == FileManage.Load && (j > 0 || this.prompt != 0)) || (this.fileManage[j] == FileManage.Empty && this.prompt == promptDialogue.Save) || Game1.gameMode == Game1.GameModes.MainMenu) && Game1.pcManager.DrawMouseButton(new Vector2(num5 + 640, num6 + 115), 0.8f, color, 2, draw: true))
								{
									this.InitLoadCopy(num);
								}
							}
						}
					}
					if (j == this.tempSaveSlot)
					{
						if (!Game1.isPCBuild)
						{
							this.DrawGamerPic(new Vector2(num5 + 620, num6 + 90), 1f - Menu.fadeInAlpha);
						}
						if (this.confirming)
						{
							this.DrawConfirm(j, num6, num);
						}
					}
					if (this.fileManage[j] == FileManage.CopyFrom)
					{
						this.DrawConfirm(j, num6, num);
					}
				}
			}
			if (!(Menu.fileAlpha >= 1f) || this.menuMode != MenuMode.FileManage)
			{
				return;
			}
			float num9 = (this.confirming ? 0.5f : 1f);
			Color color2 = new Color(num9, num9, num9, 1f - Menu.textAlpha / 100f);
			Vector2 vector = new Vector2(Game1.screenWidth / 2 + 430, (float)Game1.screenHeight * 0.2f);
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && Menu.savingHelpDisplayed && Game1.pcManager.DrawMouseButton(vector + new Vector2(-23f, -80f), 0.8f, color2, 0, draw: true) && !this.confirming && this.InitCancel(num))
			{
				return;
			}
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && Menu.savingHelpDisplayed && !this.confirming)
			{
				int num10 = 220;
				int num11 = (Menu.maxFiles - 2) * num10;
				Game1.hud.DrawScrollBarMouse(vector, ref Menu.fileScroll, (int)((float)Game1.screenHeight * 0.6f), num11, num10, num9);
				Game1.pcManager.MouseWheelAdjust(ref Menu.fileScroll, num10, 0f, num11);
			}
			else
			{
				int num12 = 220 * Math.Max(this.tempSaveSlot - 1, 0);
				Menu.fileScroll += ((float)num12 - Menu.fileScroll) * Game1.HudTime * 20f;
				float percent = Menu.fileScroll / 220f * 0.11f;
				Game1.hud.DrawScrollBar(vector, percent, (float)Game1.screenHeight * 0.6f, num9);
			}
			if (!Menu.savingHelpDisplayed)
			{
				this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, 0.6f * Menu.fileAlpha));
				float num13 = 0.8f;
				Game1.smallText.Color = new Color(1f, 1f, 1f, (1f - Menu.textAlpha / 100f) * Menu.fileAlpha);
				int num14 = 700;
				int num15 = 75 + (int)(Game1.smallFont.MeasureString(this.buttonText).Y * num13);
				Vector2 vector2 = new Vector2((Game1.screenWidth - num14) / 2, (Game1.screenHeight - num15) / 3);
				Game1.hud.DrawMiniBorder(vector2, num14, num15, Game1.smallText.Color, 0.95f);
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
				{
					if (Game1.pcManager.DrawMouseButton(vector2 + new Vector2(num14 - 60, num15 - 60), 0.8f, Game1.smallText.Color, 2, draw: true))
					{
						Sound.PlayCue("menu_confirm");
						Menu.savingHelpDisplayed = true;
					}
				}
				else if (Game1.pcManager.inputDevice == InputDevice.GamePad)
				{
					this.sprite.Draw(this.hudTex[0], vector2 + new Vector2(num14 - 30, num15 - 30), new Rectangle(200, 140, 46, 46), new Color(1f, 1f, 1f, 0.75f + (float)Math.Sin(num2)), 0f, new Vector2(24f, 22f), 0.8f, SpriteEffects.None, 0f);
				}
				Game1.smallText.DrawButtonText(vector2 + new Vector2(30f, 10f), this.buttonText, num13, this.buttonTextButtonList, bounce: true, num14, TextAlign.Left);
			}
			else if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				this.buttonText = Game1.smallText.WordWrap(Game1.isPCBuild ? Strings_PC.LoadControls : Strings_MainMenu.LoadControls, 0.8f, Game1.screenWidth, this.buttonTextButtonList, TextAlign.LeftAndCenter);
				float num16 = (float)Game1.screenHeight * 0.82f + Menu.textAlpha / 4f;
				float num17 = 0.8f;
				Game1.smallText.Color = new Color(1f, 1f, 1f, (1f - Menu.textAlpha / 100f) * Menu.fileAlpha);
				int num18 = (int)(Game1.smallFont.MeasureString(this.buttonText).X * num17) + 140;
				int num19 = (int)(Game1.smallFont.MeasureString(this.buttonText).Y * num17);
				Game1.hud.DrawMiniBorder(new Vector2((Game1.screenWidth - num18) / 2, num16 - 12f), num18, num19 + 24, Game1.smallText.Color, 0.95f);
				Game1.smallText.DrawButtonText(new Vector2(0f, num16), this.buttonText, num17, this.buttonTextButtonList, bounce: false, Game1.screenWidth, TextAlign.Center);
			}
		}

		private void PopulateFileConfirm(string text)
		{
			this.simpleString = Game1.smallText.WordWrap(text, 0.8f, 1000f, this.simpleStringButtonList, TextAlign.Left);
		}

		private bool UpdateConfirm()
		{
			if (this.fileManage[this.tempSaveSlot] == FileManage.Empty || Game1.hud.KeySelect)
			{
				Sound.PlayCue("menu_confirm");
				Menu.confirmAlpha = 0f;
				this.confirming = false;
				return true;
			}
			if (Game1.hud.KeyCancel)
			{
				Sound.PlayCue("menu_click");
				this.confirming = false;
				Menu.confirmAlpha = 0f;
				for (int i = 0; i < this.fileManage.Length; i++)
				{
					if (this.fileManage[i] != 0)
					{
						this.fileManage[i] = FileManage.Load;
					}
				}
				return false;
			}
			return false;
		}

		private bool FinalizeConfirm(int copying)
		{
			Menu.menuSelectMove = 0f;
			Menu.pauseAlpha = 0f;
			if (copying > -1)
			{
				this.CopyFile(copying, this.tempSaveSlot);
				return true;
			}
			switch (this.fileManage[this.tempSaveSlot])
			{
				case FileManage.Delete:
					this.DeleteFile();
					break;
				case FileManage.Empty:
					if (this.prompt == promptDialogue.Save)
					{
						this.SaveFile();
					}
					else
					{
						this.ChooseDiff();
					}
					break;
				default:
					if (this.prompt == promptDialogue.Save)
					{
						this.SaveFile();
						break;
					}
					this.menuMode = MenuMode.Loading;
					if (Game1.gameMode == Game1.GameModes.MainMenu)
					{
						this.PopulateMenu();
					}
					Menu.titleFade = 1f;
					break;
			}
			return false;
		}

		private void DrawConfirm(int i, int y, int copying)
		{
			float num = 0.8f;
			Color color = new Color(1f, 1f, 1f, Menu.confirmAlpha);
			Game1.smallText.Color = color;
			int num2 = (int)(Game1.smallFont.MeasureString(this.simpleString).X * num) + 60;
			int num3 = (int)(Game1.smallFont.MeasureString(this.simpleString).Y * num) + 10;
			if (copying == i)
			{
				num2 = (int)(Game1.smallFont.MeasureString(Strings_MainMenu.CopyFrom).X * num) + 60;
				num3 = (int)(Game1.smallFont.MeasureString(Strings_MainMenu.CopyFrom).Y * num) + 10;
			}
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				num3 += 60;
				Vector2 vector = new Vector2((float)(Game1.screenWidth / 2 + 400) - 40f * Menu.confirmAlpha - (float)num2, y + 180 - num3);
				Game1.hud.DrawMiniBorder(vector, num2, num3, color, 0.85f);
				if (copying == i)
				{
					Game1.smallText.DrawText(vector + new Vector2(5f, 5f), Strings_MainMenu.CopyFrom, num, num2, TextAlign.Center);
				}
				else
				{
					Game1.smallText.DrawButtonText(vector + new Vector2(5f, 5f), this.simpleString, num, this.simpleStringButtonList, bounce: true, num2, TextAlign.Center);
					if (Game1.pcManager.DrawMouseButton(vector + new Vector2(num2 - 115, num3 - 55), 0.8f, color, 1, draw: true))
					{
						Game1.hud.KeyCancel = true;
						if (this.UpdateConfirm())
						{
							if (this.prompt == promptDialogue.Save)
							{
								this.InitCancel(copying);
							}
							else
							{
								this.FinalizeConfirm(copying);
							}
						}
					}
				}
				if (!Game1.pcManager.DrawMouseButton(vector + new Vector2(num2 - 55, num3 - 55), 0.8f, color, (copying == i) ? 1 : 2, draw: true))
				{
					return;
				}
				Game1.hud.KeySelect = true;
				if (this.UpdateConfirm())
				{
					if (copying == i)
					{
						this.InitCancel(copying);
					}
					else
					{
						this.FinalizeConfirm(copying);
					}
				}
			}
			else
			{
				Vector2 vector2 = new Vector2((float)(Game1.screenWidth / 2 + 400) - 40f * Menu.confirmAlpha - (float)num2, y + 180 - num3);
				Game1.hud.DrawMiniBorder(vector2, num2, num3, color, 0.85f);
				if (copying == i)
				{
					Game1.smallText.DrawText(vector2 + new Vector2(5f, 5f), Strings_MainMenu.CopyFrom, num, num2, TextAlign.Center);
				}
				else
				{
					Game1.smallText.DrawButtonText(vector2 + new Vector2(5f, 5f), this.simpleString, num, this.simpleStringButtonList, bounce: true, num2, TextAlign.Center);
				}
			}
		}

		private void InitFileManager()
		{
			Game1.loadingTime = 0f;
			this.tempSaveSlot = Game1.stats.saveSlot;
			this.menuMode = MenuMode.FileManage;
			this.confirming = false;
			Menu.confirmAlpha = 0f;
			this.mostRecentTime = 0L;
		}

		private void ChooseDiff()
		{
			Game1.stats.saveSlot = this.tempSaveSlot;
			this.menuMode = MenuMode.None;
			this.curMenuPage = 1;
			this.curMenuOption = Game1.stats.gameDifficulty;
			this.PopulateMenu();
			this.SetMenuSelectMove();
		}
		public List<string> GetItemsFromFile()
		{
			var itemid = new List<string>();
			foreach (string line in System.IO.File.ReadLines(System.IO.Directory.GetCurrentDirectory() + "\\data\\items.data"))
			{
				var found = "";
				if (line[0] != '[')
				{
					foreach (char chara in line)
					{
						if (chara.ToString() == ":")
						{
							found = "";
						}
						else if (chara.ToString() == "[")
						{
							found = "";
						}
						else if (chara.ToString() == "]")
						{
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
				}
			}
			return itemid;
		}
		public List<string> GetLocationsFromFile()
		{
			var itemid = new List<string>();
			foreach (string line in System.IO.File.ReadLines(System.IO.Directory.GetCurrentDirectory() + "\\data\\items.data"))
			{
				var found = "";
				if (line[0] != '[')
				{
					foreach (char chara in line)
					{
						if (chara.ToString() == ":")
						{
							itemid.Add(found);
							found = "";
						}
						else if (chara.ToString() == "[")
						{
							found = "";
						}
						else if (chara.ToString() == "]")
						{
							found = "";
						}
						else if (chara.ToString() == ",")
						{
							found = "";
						}
						else
							found += chara;
					}
				}
			}
			return itemid;
		}
		public List<int> GetLocationItemsFromFile()
		{
			var itemid = new List<int>();
			foreach (string line in System.IO.File.ReadLines(System.IO.Directory.GetCurrentDirectory() + "\\data\\items.data"))
			{
				var found = 0;
				if (line[0] != '[')
				{
					foreach (char chara in line)
					{
						if (chara.ToString() == ",")
						{
							found++;
						}
					}
					itemid.Add(found);
				}
			}
			return itemid;
		}
		public List<string> GetProgFromFile()
		{
			var itemid = new List<string>();
			string line = System.IO.File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + "\\data\\items.data")[0];
			var found = "";
			if (line[0] == '[')
			{
				foreach (char chara in line)
				{
					if (chara.ToString() == "[")
					{
						found = "";
					}
					else if (chara.ToString() == "]")
					{
						itemid.Add(found);
						found = "";
					}
					else if (chara.ToString() == ",")
					{
						itemid.Add(found);
						found = "";
					}
                    else
                    {
						found += chara;

					}
				}
			}
			return itemid;
		}
		public List<string> GetLocationLogicsFromFile()
		{
			var itemid = new List<string>();
			foreach (string line in System.IO.File.ReadLines(System.IO.Directory.GetCurrentDirectory() + "\\data\\items.data"))
			{
				var found = "";
				if (line[0] != '[')
				{
					foreach (char chara in line)
					{
						if (chara.ToString() == ":")
						{
							found = "";
						}
						else if (chara.ToString() == ",")
						{
							found = "";
						}
						else if (chara.ToString() == "[")
						{
							found = "";
							found += "[";
						}
						else if (chara.ToString() == "]")
						{
							found += "_";
							break;
						}
						else
						{
							found += chara;
						}
					}
					itemid.Add(found);
				}
			}
			return itemid;
		}

		private bool ParseParenthesis(string toParse, List<string> tempplaced, List<string> temptempremoved, out List<string> placed, out List<string> tempremoved, bool checksKeysIn, out bool checksKeys)
        {
			var found = "";
            var parenthesis = false;
			var ors = false;
			checksKeys = checksKeysIn;
			placed = tempplaced;
			tempremoved = temptempremoved;
			if (toParse.Length <= 0)
			{
				return true;
			}
			else if (toParse[0].ToString() != "[")
            {
				return true;
            }
			foreach (var chara in toParse)
            {
                if (parenthesis)
                {
                    if (chara.ToString() == ")")
                    {
                        found += "_";
						if (ors)
						{
							found = "";
							parenthesis = false;
						}
						else if (ParseParenthesis(found, placed, tempremoved, out placed, out tempremoved, checksKeys, out checksKeys))
                        {
                            found = "";
                            parenthesis = false;
							ors = true;

						}
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        found += chara.ToString();
                    }
				}
				else if (chara.ToString() == "[")
				{
					found = "";
				}
				else if (chara.ToString() == "|")
				{
					if (ors)
					{
						found = "";
					}
					else if (placed.Contains(found.Replace("+", "")))
					{
						if (!found.Contains("~") && !found.Contains("+"))
						{
							tempremoved.Add(found);
							placed.Remove(found);
						}
						found = "";
						ors = true;
					}
					else
					{
						ors = false;
					}
				}
				else if (chara.ToString() == "&" || chara.ToString() == "_")
				{
					if (ors)
					{
						found = "";
						ors = false;
					}
					else if (placed.Contains(found.Replace("+","")))
					{
						if (!found.Contains("~") && !found.Contains("+"))
						{
							tempremoved.Add(found);
							placed.Remove(found);
						}
						found = "";
					}
					else
					{
						return false;
					}
				}
				else if (chara.ToString() == "(")
				{
					found = "";
					found += "[";
					parenthesis = true;
				}
				else
					found += chara.ToString();
			}
			return true;
		}

		private bool MakeSeed()
		{
			List<string> progs = new List<string>();
			progs.AddRange(GetProgFromFile());
			List<string> pool = new List<string>();
			pool.AddRange(GetItemsFromFile());
			List<string> placed = new List<string>();
			List<string> spots = new List<string>();
			spots.AddRange(GetLocationsFromFile());
			List<int> spotcounts = new List<int>();
			spotcounts.AddRange(GetLocationItemsFromFile());
			List<string> spotslogic = new List<string>();
			spotslogic.AddRange(GetLocationLogicsFromFile());
			var indexcur = Rand.GetRandomInt(0, spots.Count);
			var startcur = indexcur;
			var filetowrite = System.IO.File.CreateText(System.IO.Directory.GetCurrentDirectory() + "\\data\\seed.data");
            while (spots.Count > 0)
			{
				var pastind = indexcur;
				List<int> placeablespots = new List<int>();
				for (indexcur = 0; indexcur < spots.Count; indexcur++)
				{

					var tempremoved = new List<string>();
					var needskey = false;
					var canplace = ParseParenthesis(spotslogic[indexcur], placed, tempremoved, out placed, out tempremoved, false, out needskey);
					if (canplace && needskey)
					{
						foreach (var parenLogic in spotslogic)
						{
							var fake = placed;
							var fakefake = tempremoved;
							fake.Add("305");
							if (ParseParenthesis(spotslogic[indexcur], fake, fakefake, out fake, out fakefake, false, out needskey))
							{
								fake = placed;
								fakefake = tempremoved;
								if (!ParseParenthesis(spotslogic[indexcur], fake, fakefake, out fake, out fakefake, false, out needskey))
								{
									canplace = false;
								}
							}
						}
					}
					if (canplace)
					{
						placeablespots.Add(indexcur);
					}
					placed.AddRange(tempremoved);

				}
				if (placeablespots.Count > 0)
				{
					indexcur = placeablespots[Rand.GetRandomInt(0, placeablespots.Count)];
					var item = spots[indexcur];
					var tempremoved = new List<string>();
					var needskey = false;
					var canplace = ParseParenthesis(spotslogic[indexcur], placed, tempremoved, out placed, out tempremoved, false, out needskey);
					if (canplace && needskey)
					{
						foreach (var parenLogic in spotslogic)
						{
							var fake = placed;
							var fakefake = tempremoved;
							fake.Add("305");
							if (ParseParenthesis(spotslogic[indexcur], fake, fakefake, out fake, out fakefake, false, out needskey))
							{
								fake = placed;
								fakefake = tempremoved;
								if (!ParseParenthesis(spotslogic[indexcur], fake, fakefake, out fake, out fakefake, false, out needskey))
								{
									canplace = false;
								}
							}
						}
					}
					System.Diagnostics.Debug.Print("placed " + spotslogic[indexcur]);
					var itemstring = "";
					if (spotcounts[spots.IndexOf(item)] >= 1)
					{
						for (int i = 0; i < spotcounts[spots.IndexOf(item)]; i++)
						//for (int i = pool.Count; i > 0; i--)
						{
							var chosen = Rand.GetRandomInt(0, pool.Count);
							if (i == 0 && placeablespots.Count == 1)
							{
								List<int> canchoose = new List<int>();
								for (int e = 0; e < pool.Count; e++)
								{
									if (progs.Contains(pool[e]))
									{
										canchoose.Add(e);
									}
								}
								if (canchoose.Count > 0)
								{
									chosen = canchoose[Rand.GetRandomInt(0, canchoose.Count)];
								}
							}
							itemstring += pool[chosen] + ",";
							placed.Add(pool[chosen]);
							pool.RemoveAt(chosen);
						}
					}
					System.Diagnostics.Debug.Print("ITEM " + itemstring);
					filetowrite.WriteLine(item + ":" + itemstring);
					spots.RemoveAt(indexcur);
					spotcounts.RemoveAt(indexcur);
					spotslogic.RemoveAt(indexcur);
					indexcur = Rand.GetRandomInt(0, spots.Count);
					startcur = indexcur;
				}
				else
				{
						var toprintstring = "";
						foreach (var itmitm in placed)
						{
							toprintstring += itmitm + " ";
						}
						System.Diagnostics.Debug.Print(toprintstring + "\n");
						toprintstring = "";
						foreach (var itmitm in pool)
						{
							toprintstring += itmitm + " ";
						}
						System.Diagnostics.Debug.Print(toprintstring + "\n");
						toprintstring = "";
						foreach (var itmitm in spots)
						{
							toprintstring += itmitm + " ";
						}
						System.Diagnostics.Debug.Print(toprintstring + "\n");
						filetowrite.Close();
						System.Diagnostics.Debug.Print("RESET");
						return false;
				}
			}
			filetowrite.Close();
			return true;
        }

        private void NewGame(ParticleManager pMan, string newPath, int difficulty)
		{
			bool didit = this.MakeSeed();
			while (!didit)
			{
				didit = this.MakeSeed();
			}
			WeatherAudio.Play(WeatherAudioType.Silent);
			Game1.stats.ResetGame(pMan, this.character);
			Game1.stats.gameDifficulty = (Game1.stats.startDifficulty = (byte)difficulty);
			this.UnloadMenuTextures();
			Game1.map.SwitchMap(pMan, this.character, newPath, loading: false);
			pMan.RemoveParticle(new Fidget(Vector2.Zero));
			Menu.pauseAlpha = 0f;
			Game1.events.screenFade = Color.Black;
			Game1.worldScale = 0.75f;
			Game1.events.fadeLength = (Game1.events.fadeTimer = 10000f);
			Sound.DimSFXVolume(0f);
		}

		public void QuitGame(ParticleManager pMan)
		{
			if (Game1.gameMode == Game1.GameModes.MainMenu)
			{
				Game1.gameMode = Game1.GameModes.Quit;
				return;
			}
			WeatherAudio.Stop();
			Game1.stats.ResetGame(pMan, this.character);
			Game1.hud.isPaused = false;
			if (Game1.videoPlayer.State == MediaState.Playing)
			{
				Game1.videoPlayer.Stop();
			}
			Music.Play("beauty");
			this.SkipToStartPage();
			pMan.Reset(removeWeather: true, removeBombs: true);
			this.LoadMenuTextures();
		}

		private void LoadFile()
		{
			this.menuMode = MenuMode.None;
			Game1.stats.ResetGame(Game1.pManager, Game1.character);
			Game1.gameMode = Game1.GameModes.Game;
			this.UnloadMenuTextures();
			Game1.stats.saveSlot = this.tempSaveSlot;
			Game1.hud.LoadGame();
			Game1.hud.KeySelect = false;
		}

		private void DeleteFile()
		{
			this.mostRecentTime = 0L;
			byte b = this.tempSaveSlot;
			Game1.storage.Delete(this.tempSaveSlot);
			Game1.storage.DisposeContainer();
			this.PopulateFileScreen();
			this.tempSaveSlot = b;
			Game1.hud.KeySelect = false;
		}

		private void CopyFile(int src, int dest)
		{
			this.mostRecentTime = 0L;
			byte b = this.tempSaveSlot;
			Game1.storage.Copy(src, dest);
			Game1.storage.DisposeContainer();
			this.PopulateFileScreen();
			if (Game1.storage.storeResult != StoreResult.Copied)
			{
				this.fileManage[b] = FileManage.Empty;
				Menu.saveDescription[b] = Game1.smallText.WordWrap(Strings_Prompts.StorageFullDesc, 0.9f, 540f, this.simpleStringButtonList, TextAlign.Left);
			}
			Game1.stats.saveSlot = (this.tempSaveSlot = b);
			Game1.hud.KeySelect = false;
		}

		private void SaveFile()
		{
			Game1.stats.saveSlot = this.tempSaveSlot;
			Game1.hud.SaveGame(Game1.pManager);
			this.menuMode = MenuMode.None;
			Game1.hud.KeySelect = false;
		}

		public void SelectStorage()
		{
			Game1.loadingTime = 0f;
			Menu.checkGuideTime = 0.2f;
			Game1.storage.GetDevice(LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One));
			this.SetGamerPic();
		}

		private void PopulateFileScreen()
		{
			Menu.textAlpha = 200f;
			if (!Menu.savingHelpDisplayed)
			{
				this.buttonText = Game1.smallText.WordWrap(Strings_MainMenu.SaveHelp, 0.8f, 640f, this.buttonTextButtonList, TextAlign.Center);
			}
			else
			{
				this.buttonText = Game1.smallText.WordWrap(Game1.isPCBuild ? Strings_PC.LoadControls : Strings_MainMenu.LoadControls, 0.8f, Game1.screenWidth, this.buttonTextButtonList, TextAlign.LeftAndCenter);
			}
			Game1.menu.canNewGamePlus = false;
			for (int i = 0; i < this.fileManage.Length; i++)
			{
				this.SetGamerPic();
				string text = ((i != 0) ? (Strings_MainMenu.SaveSlot + " " + i) : Strings_MainMenu.SaveSlotAuto);
				string s = string.Empty;
				switch (Game1.storage.Check(i))
				{
				case -2:
					this.fileManage[i] = FileManage.FileCorrupt;
					s = Strings_Prompts.FileCorruptDesc;
					break;
				case -1:
					this.fileManage[i] = FileManage.StorageDeviceNull;
					s = Strings_MainMenu.LoadNoDevice;
					break;
				case 0:
					this.fileManage[i] = FileManage.Empty;
					s = Strings_MainMenu.SaveSlotEmpty;
					break;
				case 1:
				{
					this.fileManage[i] = FileManage.Load;
					string @string = Strings_MainMenu.ResourceManager.GetString("ChooseDiff" + this.saveDifficulty[i]);
					int num = (int)this.saveGameClock[i];
					int num2 = num / 60;
					int num3 = Math.Min(num2 / 60, 99);
					num -= num2 * 60;
					num2 -= num3 * 60;
					string text2 = $"{num3:00}:{num2:00}:{num:00}";
					object obj = text;
					text = string.Concat(obj, ": ", this.saveCompletion[i], "% ", Strings_MainMenu.SaveCompletion);
					s = ((this.saveRegionName[i] != string.Empty) ? (this.saveRegionName[i] + "\n") : "\n") + Strings_MainMenu.SaveHP + " " + this.saveHP[i] + "/" + this.saveMaxHP[i] + "   " + Strings_MainMenu.SaveLevel + " " + this.saveLevel[i] / 4 + "\n" + Strings_MainMenu.SaveGold + " " + this.saveGold[i] + "   " + Strings_MainMenu.SaveDifficulty + " " + @string + "\n" + Strings_MainMenu.SaveTime + " " + text2;
					break;
				}
				}
				Menu.saveTitle[i] = Game1.smallText.WordWrap(text, 1f, 2000f, this.simpleStringButtonList, TextAlign.Left);
				Menu.saveDescription[i] = Game1.smallText.WordWrap(s, 0.9f, 540f, this.simpleStringButtonList, TextAlign.Left);
			}
		}

		public void SetGamerPic()
		{
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(ThreadGamerPic)));
		}

		public void ThreadGamerPic()
		{
			this.LoadGamerPic();
		}

		private void LoadGamerPic()
		{
			if (!Game1.GamerServices)
			{
				Menu.gamerPic = null;
				return;
			}
			Menu.gamerPic = null;
			try
			{
				SignedInGamer signedInGamer = Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)];
				if (signedInGamer != null && (Game1.Xbox360 || signedInGamer.IsSignedInToLive))
				{
					GamerProfile profile = signedInGamer.GetProfile();
					Menu.gamerPic = Texture2D.FromStream(Game1.graphics.GraphicsDevice, profile.GetGamerPicture());
				}
			}
			catch (Exception)
			{
			}
		}

		public void InitChapter(int _chapter)
		{
			this.menuMode = MenuMode.Chapter;
			this.chapter = new Chapter(_chapter, this.map);
		}

		public void ExitChapter()
		{
			this.chapter = null;
			this.menuMode = MenuMode.None;
		}

		private void InitPCHighScores()
		{
			this.menuMode = MenuMode.PCHighScores;
			Game1.hud.equipSelection = -1;
			Menu.helpLoaded = false;
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadPCHighScoresTextures), new TaskFinishedDelegate(LoadingHelpFinished)));
		}

		private void PopulateHighScores(bool hidden)
		{
			if (Game1.hud.equipSelection >= 0)
			{
				Sound.PlayCue("menu_click");
				float size = 0.7f;
				int num = 400;
				int num2 = Game1.hud.equipSelection + 1;
				string text = string.Empty;
				if (Game1.gameMode != 0)
				{
					text = "\n\n" + Strings_PC.AchievementLocal;
				}
				if (!hidden)
				{
					this.optionDesc = Game1.smallText.WordWrap("'" + Strings_Achievements.ResourceManager.GetString("Achievement_" + $"{num2:D2}" + "_Title") + "'\n\n" + Strings_Achievements.ResourceManager.GetString("Achievement_" + $"{num2:D2}" + "_Desc") + text, size, num - 40, TextAlign.Center);
				}
				else
				{
					this.optionDesc = Game1.smallText.WordWrap(Strings_PC.AchievementSecret + text, size, num - 40, TextAlign.Center);
				}
			}
		}

		private static bool CheckHidden(int i)
		{
			if ((Game1.gameMode == Game1.GameModes.MainMenu && !Game1.settings.GlobalAchievement[i]) || (Game1.gameMode != 0 && !Game1.stats.achievementEarned[i]))
			{
				switch (i + 1)
				{
				case 1:
				case 2:
				case 3:
				case 4:
				case 20:
				case 21:
				case 24:
				case 30:
					return true;
				}
			}
			return false;
		}

		private void UpdatePCHighScores()
		{
			float num = Math.Min(Game1.hiDefScaleOffset - 0.1f, 1.1f);
			num = (float)(Game1.screenWidth + Game1.screenHeight) * 0.8f * 0.5f / 1000f;
			int num2 = (int)(15f * num);
			int num3 = (int)((float)((Menu.rightEdge - Menu.leftEdge) / 2 - 60) / (80f * num + (float)num2));
			int equipSelection = Game1.hud.equipSelection;
			if (Game1.hud.KeyLeft)
			{
				Game1.hud.equipSelection--;
			}
			if (Game1.hud.KeyRight)
			{
				Game1.hud.equipSelection++;
			}
			if (Game1.hud.KeyUp)
			{
				Game1.hud.equipSelection -= num3;
			}
			if (Game1.hud.KeyDown)
			{
				Game1.hud.equipSelection += num3;
			}
			if (equipSelection != Game1.hud.equipSelection)
			{
				Game1.hud.KeyUp = (Game1.hud.KeyDown = (Game1.hud.KeyLeft = (Game1.hud.KeyRight = false)));
				if (equipSelection == -1)
				{
					Game1.hud.equipSelection = 0;
				}
				Game1.hud.equipSelection = (int)MathHelper.Clamp(Game1.hud.equipSelection, 0f, 29f);
				this.PopulateHighScores(Menu.CheckHidden(Game1.hud.equipSelection));
			}
			if (Game1.hud.KeyCancel)
			{
				Game1.GetLargeContent().Unload();
				Menu.helpLoaded = false;
				if (Game1.gameMode == Game1.GameModes.MainMenu)
				{
					this.ExitMode(0, 3);
				}
				else
				{
					this.ExitMode(0, 4);
				}
			}
		}

		private void DrawPCHighScores()
		{
			Game1.hud.DrawBorder(new Vector2(Menu.leftEdge, Menu.topEdge), Menu.rightEdge - Menu.leftEdge, Menu.bottomEdge - Menu.topEdge, Color.White, 1f, 0);
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				if (Game1.pcManager.DrawMouseButton(new Vector2(Menu.leftEdge + Menu.rightEdge - Menu.leftEdge - 20, Menu.topEdge - 30), 0.8f, Color.White, 0, draw: true))
				{
					Game1.hud.KeyCancel = true;
					this.UpdatePCHighScores();
					return;
				}
			}
			else
			{
				Game1.smallText.Color = Color.White;
				Game1.smallText.DrawButtonText(new Vector2(Menu.leftEdge, Menu.bottomEdge - 30), Game1.smallText.WordWrap(Strings_HudInv.Return, 0.7f, Menu.rightEdge - Menu.leftEdge - 30, this.buttonTextButtonList, TextAlign.Left), 0.7f, this.buttonTextButtonList, bounce: false, Menu.rightEdge - Menu.leftEdge, TextAlign.Center);
			}
			Color color2 = (Game1.bigText.Color = (Game1.smallText.Color = Color.White));
			Game1.bigText.DrawOutlineText(new Vector2(Menu.leftEdge, Menu.topEdge + 15), Strings_MainMenu.Main4, 1f, (Menu.rightEdge - Menu.leftEdge) / 2, TextAlign.Center, fullOutline: true);
			Game1.bigText.DrawOutlineText(new Vector2(Game1.screenWidth / 2, Menu.topEdge + 15), Strings_PC.HighScores, 1f, (Menu.rightEdge - Menu.leftEdge) / 2, TextAlign.Center, fullOutline: true);
			if (!Menu.helpLoaded)
			{
				Menu.textAlpha = 200f;
				Game1.DrawLoad(this.sprite, new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2));
				return;
			}
			this.sprite.Draw(this.hudTex[2], new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 502, 326, 18), Color.White * 0.7f, 1.57f, new Vector2(163f, 0f), new Vector2((float)Game1.screenHeight / 500f, 0.3f), SpriteEffects.None, 0f);
			Vector2 vector = Vector2.Zero;
			float num = Math.Min(Game1.hiDefScaleOffset - 0.1f, 1.1f);
			num = (float)(Game1.screenWidth + Game1.screenHeight) * 0.8f * 0.5f / 1000f;
			int num2 = (int)(15f * num);
			int num3 = (int)((float)((Menu.rightEdge - Menu.leftEdge) / 2 - 60) / (80f * num + (float)num2));
			Vector2 vector2 = new Vector2((float)(Game1.screenWidth / 2 - (Menu.rightEdge - Menu.leftEdge) / 4) - (float)num3 * (80f * num + (float)num2) / 2f, Menu.topEdge + 80);
			bool flag = false;
			float size = 0.7f;
			int num4 = 400;
			for (int i = 0; i < 30; i++)
			{
				Vector2 vector3 = vector2 + new Vector2((float)i * (80f * num + (float)num2) - (float)(i / num3 * num3) * (80f * num + (float)num2), (float)(i / num3) * (80f * num + (float)num2));
				bool flag2 = false;
				flag2 = Menu.CheckHidden(i);
				if (!flag2)
				{
					Color color3 = (((Game1.gameMode == Game1.GameModes.MainMenu && Game1.settings.GlobalAchievement[i]) || (Game1.gameMode != 0 && Game1.stats.achievementEarned[i])) ? Color.White : new Color(0f, 0f, 0f, 0.4f));
					Rectangle value = new Rectangle(i * 80 - i / 5 * 5 * 80, i / 5 * 80, 80, 80);
					this.sprite.Draw(Menu.miscTex, vector3, value, color3, 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
				}
				else
				{
					this.sprite.Draw(Menu.miscTex, vector3, new Rectangle(160, 480, 80, 80), Color.Black, 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
				}
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
				{
					if (new Rectangle((int)vector3.X, (int)vector3.Y, (int)(80f * num), (int)(80f * num)).Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y))
					{
						flag = true;
						if (i != Game1.hud.equipSelection)
						{
							Game1.hud.equipSelection = i;
							this.PopulateHighScores(flag2);
						}
					}
				}
				else
				{
					flag = true;
					if (i == Game1.hud.equipSelection)
					{
						vector = vector3 + new Vector2(100f, 100f) * num;
					}
				}
				if (i == Game1.hud.equipSelection)
				{
					this.sprite.Draw(Menu.miscTex, vector3, new Rectangle(0, 480, 80, 80), Color.White, 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
				}
			}
			int num5 = 0;
			for (int j = 0; j < Game1.cManager.challengeArenas.Count; j++)
			{
				if (Game1.cManager.challengeArenas[j].RankScore > 0)
				{
					num5 = Math.Max(num5, j + 1);
				}
			}
			for (int k = 0; k < num5 + 1; k++)
			{
				this.sprite.Draw(this.hudTex[2], new Vector2(Game1.screenWidth / 2 + (Menu.rightEdge - Menu.leftEdge) / 4, vector2.Y + (float)(k * 80)), new Rectangle(0, 502, 326, 18), Color.White * 0.7f, 0f, new Vector2(163f, 0f), new Vector2((float)Game1.screenHeight / 600f, 0.3f), SpriteEffects.None, 0f);
				if (k < num5)
				{
					Game1.bigText.DrawText(new Vector2(Game1.screenWidth / 2 + (Menu.rightEdge - Menu.leftEdge) / 4 - 180, vector2.Y + (float)(k * 80) + 10f), Strings_Regions.trial + " #" + (k + 1), 0.6f);
					this.scoreDraw.Draw(Game1.cManager.challengeArenas[k].HighScore, new Vector2(Game1.screenWidth / 2 + (Menu.rightEdge - Menu.leftEdge) / 4 - 60, vector2.Y + (float)(k * 80) + 45f), 0.9f, Color.White, ScoreDraw.Justify.Right, 1);
					int num6 = Game1.cManager.challengeArenas[k].CheckStarCount();
					for (int l = 0; l < 4; l++)
					{
						this.sprite.Draw(Menu.miscTex, new Vector2(Game1.screenWidth / 2 + (Menu.rightEdge - Menu.leftEdge) / 4 + 20 + l * 40, vector2.Y + (float)(k * 80) + 57f), new Rectangle(80, 480, 80, 80), (l < num6) ? Color.White : Color.Black, 0f, new Vector2(40f, 40f), 0.5f, SpriteEffects.None, 0f);
					}
				}
			}
			if (flag && Game1.hud.equipSelection > -1)
			{
				int num7 = (int)(Game1.smallFont.MeasureString(this.optionDesc).Y * 0.7f) + 40;
				Vector2 vector4 = ((vector == Vector2.Zero) ? (Game1.hud.mousePos + new Vector2(50f, 70f)) : vector);
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && Game1.hud.mousePos.X > (float)(Game1.screenWidth / 2))
				{
					vector4.X -= num4 + 60;
				}
				vector4.X = MathHelper.Clamp(vector4.X, 40f, Game1.screenWidth - num4 - 40);
				vector4.Y = MathHelper.Clamp(vector4.Y, 40f, Game1.screenHeight - num7 - 40);
				Game1.hud.DrawMiniBorder(vector4, num4, num7, Game1.smallText.Color, 0.95f);
				Game1.smallText.DrawText(vector4 + new Vector2(20f, 20f), this.optionDesc, size, 0f, TextAlign.Left);
			}
		}

		private void UpdateLoading(ParticleManager pMan)
		{
			Menu.screenFade += Game1.HudTime;
			Menu.titleFade += Game1.HudTime;
			Menu.titleSubFade += Game1.HudTime;
			Menu.titleScale += Game1.HudTime / 4f;
			Game1.map.transInFrame += Game1.HudTime;
			Game1.hud.pauseImageOffset += Game1.HudTime * 6000f;
			if (!(Menu.screenFade > 1.1f))
			{
				return;
			}
			if (this.menuMode == MenuMode.Quitting)
			{
				this.QuitGame(pMan);
			}
			else
			{
				switch (this.fileManage[this.tempSaveSlot])
				{
				case FileManage.Empty:
					this.NewGame(pMan, "intro01", this.curMenuOption);
					break;
				case FileManage.Load:
					this.LoadFile();
					break;
				}
			}
			this.menuMode = MenuMode.None;
		}

		public void SkipToStartPage()
		{
			Game1.hud.KeySelect = false;
			Game1.gameMode = Game1.GameModes.MainMenu;
			this.prompt = promptDialogue.None;
			this.curMenuPage = -1;
			Menu.screenFade = 0f;
			Menu.titleScale = 1f;
			Menu.titleFade = 1f;
			Menu.titleSubFade = 1f;
			Menu.titleEvent = 20;
			Menu.noInputTime = 0.5f;
			Music.Play("beauty");
			this.PopulateMenu();
		}

		private void UpdateStartPage()
		{
			if (Menu.noInputTime > 0f)
			{
				Menu.noInputTime -= Game1.HudTime;
				Menu.continueButtonAlpha = 0f;
				return;
			}
			if (this.curMenuPage < -1)
			{
				for (int i = 0; i < 4; i++)
				{
					GamePadState state = GamePad.GetState((PlayerIndex)i);
					if (state.Buttons.A == ButtonState.Pressed || state.Buttons.Start == ButtonState.Pressed)
					{
						Sound.PlayCue("menu_confirm");
						this.SkipToStartPage();
					}
				}
				if (Game1.isPCBuild && Game1.pcManager.CheckInitInput())
				{
					Sound.PlayCue("menu_confirm");
					this.SkipToStartPage();
				}
			}
			else if (Game1.currentGamePad > -1)
			{
				if (!Game1.GamerServices || Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)] != null)
				{
					if (Menu.checkGuideTime <= 0f)
					{
						Game1.storage.Read(0);
						Sound.PlayCue("menu_confirm");
						if (Game1.GamerServices && !Game1.isPCBuild)
						{
							Game1.IsTrial = Guide.IsTrialMode;
						}
						this.curMenuPage = 0;
						this.curMenuOption = 0;
						Menu.menuSelectMove = 0f;
						Menu.textAlpha = 100f;
						this.PopulateMenu();
					}
					else if (Menu.checkGuideTime == 0.2f)
					{
						if (!Game1.GamerServices || (Game1.GamerServices && !Guide.IsVisible))
						{
							Game1.SelectStorage();
							Menu.checkGuideTime = 0.1f;
						}
					}
					else if (GamePad.GetState((PlayerIndex)Game1.currentGamePad).Buttons.B == ButtonState.Pressed)
					{
						Game1.currentGamePad = -1;
					}
				}
				else if (!Game1.GamerServices || (Game1.GamerServices && !Guide.IsVisible))
				{
					Game1.currentGamePad = -1;
				}
			}
			else if (Game1.currentGamePad < 0)
			{
				Game1.SelectProfile();
			}
			Game1.hud.KeySelect = false;
		}

		private void UpdateTitleEvent()
		{
			if (Menu.titleEvent >= 10 && Menu.titleScale > 1f)
			{
				Menu.titleScale -= Game1.HudTime * (Menu.titleScale - 1f) / 4f;
				if (Menu.titleScale < 1f)
				{
					Menu.titleScale = 1f;
				}
			}
			if (Menu.titleEvent == 0)
			{
				if (Game1.pManager != null)
				{
					Game1.pManager.Reset(removeWeather: true, removeBombs: true);
				}
				this.curMenuPage = -10;
				this.curMenuOption = 0;
				this.PopulateMenu();
				Menu.titleLightning = 0f;
				Menu.titleEvent++;
			}
			else if (Menu.titleEvent == 1)
			{
				Menu.screenFade = 2f;
				Menu.titleScale = 2f;
				Menu.titleFade = 0f;
				Menu.titleSubFade = 0f;
				Menu.titleEvent = 10;
			}
			else if (Menu.titleEvent == 10)
			{
				this.curMenuPage = -10;
				this.curMenuOption = 0;
				Menu.screenFade = 2f;
				Menu.titleScale = 2f;
				Menu.titleFade = 0f;
				Menu.titleSubFade = 0f;
				Menu.titleEvent++;
			}
			else if (Menu.titleEvent == 11)
			{
				Menu.screenFade -= Game1.HudTime / 2f;
				if (Menu.screenFade < 1f)
				{
					Music.Play("beauty");
				}
				if (Menu.screenFade <= 0f)
				{
					Menu.screenFade = 0f;
					Menu.titleEvent++;
				}
			}
			else if (Menu.titleEvent == 12)
			{
				Menu.titleSubFade += Game1.HudTime * 0.5f;
				if (Menu.titleSubFade > 1f)
				{
					Menu.titleSubFade = 1f;
					Menu.titleEvent++;
				}
			}
			else if (Menu.titleEvent == 13)
			{
				Menu.titleFade += Game1.HudTime * 0.5f;
				if (Menu.titleFade > 1f)
				{
					Menu.titleFade = 1f;
					Menu.titleEvent++;
				}
			}
			else if (Menu.titleEvent == 14)
			{
				if (this.curMenuPage < -1)
				{
					Game1.currentGamePad = -1;
					this.curMenuPage = -1;
				}
				Menu.continueButtonAlpha = 0f;
				Menu.titleEvent++;
			}
			else if (Menu.titleEvent == 15 && Menu.titleScale < 1f)
			{
				Menu.titleScale = 1f;
				Menu.titleEvent = 20;
			}
		}

		private void UpdateTimers()
		{
			Game1.hud.mousePos = Game1.pcManager.GetMouseLoc();
			if (Menu.fadeInAlpha > 0f)
			{
				Menu.fadeInAlpha -= Game1.HudTime * 4f;
				if (Menu.fadeInAlpha < 0f)
				{
					Menu.fadeInAlpha = 0f;
				}
			}
			if (Menu.textAlpha > 0f)
			{
				Menu.textAlpha -= Game1.HudTime * 750f * (Menu.textAlpha / 100f);
				if (Menu.textAlpha < 1f)
				{
					Menu.textAlpha = 0f;
				}
			}
			if (Game1.gameMode == Game1.GameModes.Game && this.prompt == promptDialogue.None)
			{
				Menu.pauseAlpha = 0f;
				return;
			}
			Menu.pauseAlpha = MathHelper.Clamp(Menu.pauseAlpha + Game1.HudTime * 4f, 0f, 1f);
			if (Menu.savingAlpha > 0f)
			{
				Menu.savingAlpha -= Game1.HudTime;
			}
			Menu.titleFloat += Game1.HudTime / 10f;
			if (Menu.titleFloat > 6.28f)
			{
				Menu.titleFloat -= 6.28f;
			}
			if (this.menuMode != MenuMode.Settings || !Game1.isPCBuild)
			{
				int num = 0;
				for (int i = 0; i < this.curMenuOption; i++)
				{
					if (Menu.menuName[i] != string.Empty)
					{
						num++;
					}
				}
				float num2 = num * 30;
				if (Menu.pauseAlpha < 1f)
				{
					Menu.menuSelectMove = num2;
				}
				if (Menu.menuSelectMove < num2)
				{
					float num3 = 300 * (int)MathHelper.Clamp(Math.Abs((Menu.menuSelectMove - num2) / 14f), 1f, 10f);
					Menu.menuSelectMove += Game1.HudTime * num3;
					if (Menu.menuSelectMove > num2)
					{
						Menu.menuSelectMove = num2;
					}
				}
				else if (Menu.menuSelectMove > num2)
				{
					float num4 = 300 * (int)MathHelper.Clamp(Math.Abs((Menu.menuSelectMove - num2) / 14f), 1f, 10f);
					Menu.menuSelectMove -= Game1.HudTime * num4;
					if (Menu.menuSelectMove < num2)
					{
						Menu.menuSelectMove = num2;
					}
				}
			}
			if (this.menuMode == MenuMode.FileManage)
			{
				if (Menu.fileAlpha < 1f)
				{
					Menu.fileAlpha += (1f - Menu.fileAlpha) * Game1.HudTime * 14f;
					if ((double)Menu.fileAlpha > 0.999)
					{
						Menu.fileAlpha = 1f;
						this.PopulateFileScreen();
					}
				}
			}
			else if (Menu.fileAlpha > 0f)
			{
				Menu.fileAlpha = MathHelper.Clamp(Menu.fileAlpha - Game1.HudTime * 4f, 0f, 1f);
			}
			if (Game1.gameMode == Game1.GameModes.MainMenu)
			{
				if (this.curMenuPage > -1 && ((Game1.GamerServices && Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)] == null) || Game1.currentGamePad < 0))
				{
					Sound.PlayCue("menu_cancel");
					this.curMenuPage = -3;
					Game1.currentGamePad = -1;
					Menu.continueButtonAlpha = 0f;
					this.curMenuOption = Menu.prevMenuOption;
					this.menuMode = MenuMode.None;
					this.PopulateMenu();
					this.SetMenuSelectMove();
				}
				if (Menu.titleEvent < 20)
				{
					this.UpdateTitleEvent();
				}
				Menu.continueButtonAlpha += Game1.HudTime;
				if (Menu.continueButtonAlpha > 6.28f)
				{
					Menu.continueButtonAlpha -= 6.28f;
				}
				if (Menu.titleEvent >= 10)
				{
					WeatherAudio.Play(WeatherAudioType.RainLight);
					Game1.pManager.AddHudRain(Rand.GetRandomVector2(0f, Game1.screenWidth + 200, -100f, 0f), Rand.GetRandomVector2(-800f, -200f, 1200f, 2000f), Rand.GetRandomFloat(0.5f, 2f), 9);
					if (Rand.GetRandomInt(0, 100) == 0)
					{
						Game1.pManager.AddHudLightning(Rand.GetRandomVector2(-200f, Game1.screenWidth + 400, 0f, 400f * Game1.hiDefScaleOffset), master: true, Rand.GetRandomFloat(0.5f, 1.5f), Math.Max(Rand.GetRandomInt(4, 10), 4), 60, 0.4f, 5);
						Menu.titleLightning = Math.Min(Menu.titleLightning + Rand.GetRandomFloat(0.1f, 1f), 1.2f);
						Sound.PlayCue("thunder");
					}
					Game1.pManager.UpdateParticles(Game1.FrameTime, this.map, this.character, 5);
					if (Menu.titleLightning > 0f)
					{
						Menu.titleLightning -= Game1.HudTime;
					}
				}
			}
			this.UpdateGuideTime();
		}

		private void UpdateGuideTime()
		{
			if (!(Menu.checkGuideTime > 0f) || (Game1.GamerServices && Guide.IsVisible))
			{
				return;
			}
			Menu.checkGuideTime -= Game1.HudTime;
			if (!(Menu.checkGuideTime < 0f))
			{
				return;
			}
			Menu.checkGuideTime = 0f;
			if (Game1.GamerServices && Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)] == null)
			{
				return;
			}
			if (Game1.GamerServices && !Game1.isPCBuild)
			{
				bool isTrial = Game1.IsTrial;
				Game1.IsTrial = Guide.IsTrialMode;
				if (isTrial != Game1.IsTrial)
				{
					this.PopulateMenu();
				}
			}
			if (this.menuMode == MenuMode.FileManage)
			{
				this.PopulateFileScreen();
			}
			if (this.curMenuPage == -1)
			{
				Game1.storage.Read(0);
				Sound.PlayCue("menu_confirm");
				this.curMenuPage = 0;
				this.curMenuOption = 0;
				Menu.menuSelectMove = 0f;
				Menu.textAlpha = 100f;
				this.PopulateMenu();
			}
		}

		public void ClearMenu()
		{
			Menu.fadeInAlpha = 1f;
			Menu.textAlpha = 100f;
			Menu.pauseAlpha = 0f;
			this.curMenuOption = 0;
			Menu.prevMenuOption = 0;
			this.SetMenuSelectMove();
			Menu.savingAlpha = -1f;
			Game1.hud.isPaused = false;
			Game1.gameMode = Game1.GameModes.Game;
		}

		public void ClearPrompt()
		{
			Menu.fadeInAlpha = 1f;
			Menu.textAlpha = 100f;
			Menu.fileAlpha = 0f;
			this.curMenuOption = 0;
			this.PopulatePrompt();
			this.SetMenuSelectMove();
		}

		public void TestCutscene(int id)
		{
			if (Game1.creditsScroll != null)
			{
				Game1.creditsScroll.ExitCredits();
			}
			Music.Play("silent");
			Game1.events.sideEventAvailable[110] = true;
			Game1.events.InitEvent(110, isSideEvent: true);
			Game1.cutscene.InitCutscene(id);
		}

		private bool CheckUnlockedGame(bool forceTrialCheck)
		{
			if (Game1.navManager.NavPath == "trial" && Game1.GamerServices)
			{
				if (forceTrialCheck)
				{
					Game1.IsTrial = Guide.IsTrialMode;
				}
				if (!Game1.IsTrial)
				{
					if (Game1.stats.achievementEarned[15])
					{
						Game1.awardsManager.EarnAchievement(Achievement.LevelUp, forceCheck: true);
					}
					this.menuMode = MenuMode.Quitting;
					Menu.titleFade = 1f;
					return true;
				}
			}
			return false;
		}

		private void DrawGamerPic(Vector2 loc, float scale)
		{
			if (Menu.gamerPic != null && !Menu.gamerPic.IsDisposed)
			{
				this.sprite.Draw(Menu.gamerPic, loc, new Rectangle(0, 0, 64, 64), Color.White, 0f, new Vector2(32f, 32f), scale, SpriteEffects.None, 0f);
			}
			else
			{
				this.sprite.Draw(this.nullTex, loc, new Rectangle(0, 0, 64, 64), Color.White, 0f, new Vector2(32f, 32f), scale, SpriteEffects.None, 0f);
			}
		}

		private void DrawCursors(Vector2 loc, float scale, float width, float alpha)
		{
			if (!(Menu.fileAlpha > 0f))
			{
				if ((double)alpha < 0.5)
				{
					Menu.cursorPos.X = loc.X - 140f;
					Menu.cursorPos.Y = loc.X + 10f + width;
				}
				else
				{
					Menu.cursorPos.X += (loc.X - 140f - Menu.cursorPos.X) * Game1.HudTime * 10f;
					Menu.cursorPos.Y += (loc.X + 10f + width - Menu.cursorPos.Y) * Game1.HudTime * 10f;
				}
				Game1.hud.DrawCursor(new Vector2(Menu.cursorPos.X, loc.Y), scale * 2f, new Color(1f, 1f, 1f, alpha), flip: false);
				Game1.hud.DrawCursor(new Vector2(Menu.cursorPos.Y, loc.Y), scale * 2f, new Color(1f, 1f, 1f, alpha), flip: true);
			}
		}

		private void DrawBackground()
		{
			float x = (float)(Math.Cos(Menu.titleFloat * 5f) * 10.0);
			float y = (float)(Math.Cos(Menu.titleFloat * 1f) * 10.0);
			float num = (Menu.titleScale + (Menu.titleScale - 1f)) * Math.Max(Game1.hiDefScaleOffset, 1f);
			this.sprite.Draw(Menu.menuTex, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f - new Vector2(x, y) * 1f, new Rectangle(0, 700, 640, 360), Color.White, 0f, new Vector2(320f, 180f), 2.1f * num, SpriteEffects.None, 0f);
			this.sprite.Draw(Menu.menuTex, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f - new Vector2(x, y) * 1.75f, new Rectangle(0, 1062, 1160, 180), Color.White, 0f, new Vector2(580f, -135f), 1.2f * num, SpriteEffects.None, 0f);
			Game1.pManager.DrawHudParticles(this.particlesTex, 1f, 5);
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			this.sprite.Draw(Menu.menuTex, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f - new Vector2(x, y) * 2.5f, new Rectangle(0, 1062, 1160, 180), Color.White, 0f, new Vector2(580f, -135f), 1.4f * num, SpriteEffects.FlipHorizontally, 0f);
			if (this.menuMode == MenuMode.None)
			{
				this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(1f, 1f, 1f, Menu.screenFade / 2f));
			}
			if (Menu.titleLightning > 0f)
			{
				this.sprite.Draw(this.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(1f, 1f, 1f, Menu.titleLightning));
			}
			Vector2 vector = new Vector2(Game1.screenWidth / 2, (float)Game1.screenHeight / 3.5f);
			float num2 = 1f - Menu.titleLightning / 2f;
			float num3 = 2.5f - num;
			float a = Menu.titleFade * (1f - Menu.fileAlpha);
			this.sprite.Draw(Menu.menuTex, vector + new Vector2(x, y) * 2f, new Rectangle(0, 0, 1000, 400), new Color(num2, num2, num2, a), 0f, new Vector2(500f, 250f), num, SpriteEffects.None, 0f);
			float num4 = (1f - Menu.titleFade) * (1f - Menu.fileAlpha) * num3;
			this.sprite.Draw(Menu.menuTex, vector + new Vector2(x, y) * 2f, new Rectangle(620, 400, 540, 214), new Color(num2, num2, num2, num4 / 2f), 0f, new Vector2(270f, 137f), 2f * num, SpriteEffects.None, 0f);
			Game1.pManager.DrawHudParticles(this.particlesTex, 1f, 9);
			this.sprite.End();
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			num3 = 3.5f - num;
			float a2 = Menu.titleSubFade * (1f - Menu.fileAlpha);
			this.sprite.Draw(Menu.menuTex, vector + new Vector2(x, y) * 3f, new Rectangle(0, 400, 620, 300), new Color(1f, 1f, 1f, a2), 0f, new Vector2(317f, 130f), num, SpriteEffects.None, 0f);
			float num5 = (1f - Menu.titleSubFade) * (1f - Menu.fileAlpha) * num3;
			this.sprite.Draw(Menu.menuTex, vector + new Vector2(x, y) * 3f, new Rectangle(1000, 0, 160, 400), new Color(1f, 1f, 1f, num5 / 2f), 1.57f, new Vector2(70f, 200f), num * 2f, SpriteEffects.None, 0f);
		}

		public void DrawSelection(Vector2 pos, int width, int height, float alpha)
		{
			Color color = new Color(1f, 1f, 1f, alpha);
			this.sprite.Draw(this.nullTex, new Rectangle((int)pos.X + 8, (int)pos.Y + 5, width - 45, (int)((float)height * 0.8f)), new Rectangle(490, 299, 5, 100), new Color(1f, 1f, 1f, 0.2f * alpha), 0f, Vector2.Zero, SpriteEffects.None, 0f);
			this.sprite.Draw(this.hudTex[0], new Rectangle((int)pos.X, (int)pos.Y, 50, height), new Rectangle(440, 299, 50, 100), color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			this.sprite.Draw(this.hudTex[0], new Rectangle((int)pos.X + 50, (int)pos.Y, width - 130, height), new Rectangle(490, 299, 5, 100), color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			this.sprite.Draw(this.hudTex[0], new Rectangle((int)pos.X + width - 80, (int)pos.Y, 50, height), new Rectangle(440, 299, 50, 100), color, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
		}

		public void Update(ParticleManager pMan)
		{
			this.prompt = promptDialogue.None;
			if (this.curMenuPage < 0)
			{
				this.UpdateStartPage();
			}
			else
			{
				switch (this.menuMode)
				{
				case MenuMode.None:
					if (Menu.checkGuideTime != 0f)
					{
						break;
					}
					if (Game1.gameMode == Game1.GameModes.MainMenu)
					{
						this.UpdateMainMenu(pMan);
					}
					else
					{
						this.UpdatePauseMenu(pMan);
					}
					if (Game1.awardsManager.prevGamer != null && (Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)] == null || Game1.awardsManager.prevGamer.Gamertag != Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)].Gamertag) && this.prompt != promptDialogue.SignedOut)
					{
						Game1.hud.SignOut(pMan);
						if (Game1.gameMode == Game1.GameModes.MainMenu)
						{
							Menu.savingHelpDisplayed = false;
							this.curMenuPage = -3;
							Game1.currentGamePad = -1;
							Menu.continueButtonAlpha = 0f;
							this.curMenuOption = Menu.prevMenuOption;
							this.menuMode = MenuMode.None;
							this.PopulateMenu();
							this.SetMenuSelectMove();
						}
					}
					break;
				case MenuMode.Help:
					this.UpdateHelp(1f);
					break;
				case MenuMode.Settings:
					this.UpdateSettings();
					break;
				case MenuMode.HudAdjust:
					this.UpdateHudAdjust();
					break;
				case MenuMode.Credits:
					this.UpdateCredits();
					break;
				case MenuMode.FileManage:
					this.UpdateFileScreen(pMan);
					break;
				case MenuMode.LeaderBoards:
					Game1.awardsManager.UpdateLeaderBoard();
					break;
				case MenuMode.PCHighScores:
					this.UpdatePCHighScores();
					break;
				default:
					this.UpdateLoading(pMan);
					break;
				}
			}
			this.UpdateTimers();
		}

		public void Draw()
		{
			Game1.graphics.GraphicsDevice.Clear(Color.Black);
			if (Menu.titleEvent <= 1)
			{
				return;
			}
			this.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			this.DrawBackground();
			switch (this.menuMode)
			{
			case MenuMode.Help:
				this.DrawHelp();
				break;
			case MenuMode.Credits:
				this.DrawCredits(1f - Menu.fadeInAlpha);
				break;
			case MenuMode.Settings:
				this.DrawSettings();
				break;
			case MenuMode.HudAdjust:
				this.DrawHudAdjust();
				break;
			case MenuMode.LeaderBoards:
				Game1.awardsManager.DrawLeaderBoard(this.sprite, this.hudTex, Vector2.Zero);
				break;
			case MenuMode.PCHighScores:
				this.DrawPCHighScores();
				break;
			default:
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
				{
					this.DrawMainMenuPC();
				}
				else
				{
					this.DrawMainMenu();
				}
				break;
			}
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				Game1.pcManager.DrawCursor(this.sprite, 0.8f, Color.White);
			}
			this.sprite.End();
		}
	}
}
