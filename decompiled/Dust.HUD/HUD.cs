using System;
using System.Collections.Generic;
using Dust.Audio;
using Dust.CharClasses;
using Dust.Dialogue;
using Dust.MapClasses;
using Dust.NavClasses;
using Dust.Particles;
using Dust.PCClasses;
using Dust.Quests;
using Dust.Storage;
using Dust.Strings;
using Dust.Vibration;
using Lotus.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dust.HUD
{
	public class HUD
	{
		public RenderTarget2D textTarget;

		public RenderTarget2D textTargetMask;

		public DialogueState dialogueState;

		private int newConversation;

		private CharacterType newCharType;

		public Vector2 mousePos;

		public Vector3 scrollBarPos;

		private float questScrollBar;

		private float mouseHighLighted;

		public Vector2 mapScrollPos;

		private static object syncObject = new object();

		public Dialogue dialogue;

		public InventoryState inventoryState;

		private static QuestPage questPage;

		private static SpriteBatch sprite;

		private static Texture2D[] particlesTex;

		private static Texture2D nullTex;

		private static Texture2D[] hudTex;

		private static Texture2D numbersTex;

		private static Texture2D inventoryTex;

		public int screenTopOffset;

		public int screenLeftOffset;

		public Vector2 inventoryScreenOffset;

		public float eventLeftOffset;

		private static float cineOffset;

		private static bool eventOffset;

		public bool noEvent;

		private static float eventIcon;

		public float pauseImageOffset;

		public bool debugDisplay;

		public bool hudDetails = true;

		public float hideHudDetails;

		public bool noFilters;

		public bool isPaused;

		public int invSubStage;

		private static float equipFade = 0f;

		private static float equipFade2 = 0f;

		private static int equipCategory = 0;

		public int equipSelection;

		private static int equipSelection2 = 0;

		private static int equipSelection3 = 0;

		private static float inventoryTitlePos;

		private static Vector2 inventorySelectionPos;

		private static Vector2 itemSelector;

		private static int invResponse;

		private static bool inventoryTransRight;

		private static bool inventoryTransLeft;

		private static float questScroll;

		private static float questDescScroll;

		private static bool loadingHud;

		public List<Vector3> equipList = new List<Vector3>();

		public List<Vector3> materialList = new List<Vector3>();

		private static int newAttack;

		private static int newDefense;

		private static int newFidget;

		private static int newLuck;

		public Shop shop;

		public ShopType shopType;

		private static float miniPromptTime;

		public int miniPromptState;

		public List<MiniPrompt> miniPromptList = new List<MiniPrompt>();

		private static float miniBorderGlowPos;

		private static float helpTime;

		private static int helpContinue;

		public int helpState;

		private static string helpPrevText;

		private static string helpText;

		private static int helpTextWidth;

		private static bool loadingHelp;

		private static Dictionary<Vector3, string> helpTextButtonList = new Dictionary<Vector3, string>();

		public FidgetPrompt fidgetPrompt;

		private static float fidgetBubble;

		private static float fidgetBubbleTimer;

		private static int fidgetPromptX;

		private static string fidgetText;

		private static Dictionary<Vector3, string> fidgetButtonList = new Dictionary<Vector3, string>();

		private static int[] upgradeCategory = new int[4];

		private static int curUpgrade;

		public int levelUpQueue;

		public bool levelUpEffect;

		public float regionIntroTime;

		public byte regionIntroState;

		public byte unlockState;

		private UnlockGame unlockGame;

		public bool inBoss;

		public float bossLife;

		public float bossMaxLife;

		private static float bossLifeBarPos;

		private static float bossLifeBarPercent;

		public string bossName;

		public bool npcAvailable;

		public bool saveable;

		public int shopID;

		private static bool shoppable;

		public float shopRestockTime;

		public int converseWithID;

		public int collectID;

		private static int prevConverseWithID;

		public bool canInput;

		public Vector2 savePos;

		public Vector2 shopPos;

		private float autoSaveTime;

		public float XPBarPos;

		private static float barSpeed = 7f;

		private static float lifeBarPos;

		private static float lifeBarAnimTime;

		private static int lifeBarAnim;

		public float lifeBarBeat;

		public float comboTextSize;

		public float expDialogueOffset;

		public float hudScale;

		public float screenDimAlpha;

		public Vector2 cursorPos;

		public int tempGold;

		public int cursorFrame;

		private static int chargeBarFrame;

		private static int coinAnimFrame;

		private static int dizzyAnimFrame;

		private static float chargeBarPos;

		private static float changeProjectileTime;

		private static float hudAnimFrameTime;

		public float hudDim;

		private static float navDim;

		private static float navAlpha;

		private static float promptAlpha;

		private static float itemAlpha;

		public float pulse;

		private static float attackIconAlpha;

		private static float defenseIconAlpha;

		public float runInTime;

		public Vector2 playerSaveLoc;

		private Character[] character;

		private Map map;

		public ScoreDraw scoreDraw;

		private float holdMoveTime;

		public bool KeyLeft;

		public bool KeyRight;

		public bool KeyUp;

		public bool KeyDown;

		private bool KeyThumb_Left;

		private bool KeyThumb_Right;

		public bool KeySelect;

		public bool KeyCancel;

		public bool KeyX;

		public bool KeyY;

		public bool KeyPause;

		public bool OpenMenuButton;

		public bool KeyLeftTrigger;

		public bool KeyRightTrigger;

		public bool KeyLeftBumper;

		public bool KeyRightBumper;

		public bool KeyUpOpen;

		private GamePadState curState = default(GamePadState);

		private GamePadState prevState = default(GamePadState);

		private static RenderTarget2D scrollMask;

		private static RenderTarget2D scrollSource;

		private Texture2D scrollMaskTexture;

		private Texture2D scrollSourceTexture;

		public Texture2D[] HudTex => HUD.hudTex;

		public Texture2D NumbersTex => HUD.numbersTex;

		public Texture2D InventoryTex
		{
			get
			{
				return HUD.inventoryTex;
			}
			set
			{
				HUD.inventoryTex = value;
			}
		}

		public GamePadState GetCurstate()
		{
			return this.curState;
		}

		public GamePadState GetPrevCurstate()
		{
			return this.prevState;
		}

		public HUD(SpriteBatch _sprite, Texture2D[] _particlesTex, Texture2D _nullTex, Texture2D[] _hudTex, Texture2D _numbersTex, Character[] _character, Map _map)
		{
			HUD.sprite = _sprite;
			HUD.particlesTex = _particlesTex;
			HUD.hudTex = _hudTex;
			this.character = _character;
			this.map = _map;
			HUD.nullTex = _nullTex;
			HUD.numbersTex = _numbersTex;
			this.scoreDraw = new ScoreDraw(HUD.sprite, HUD.numbersTex);
			this.ResetHud(all: true);
			this.textTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, 700, 700);
			this.textTargetMask = new RenderTarget2D(Game1.graphics.GraphicsDevice, 700, 700);
		}

		public void ResetHud(bool all)
		{
			this.eventLeftOffset = 800f;
			HUD.eventOffset = false;
			HUD.cineOffset = 0f;
			this.noEvent = false;
			Game1.loadingTime = 0f;
			HUD.eventIcon = 0f;
			this.pauseImageOffset = 700f;
			this.hideHudDetails = 1f;
			this.isPaused = false;
			HUD.inventorySelectionPos = Vector2.Zero;
			this.cursorPos = new Vector2(270f, 255f);
			HUD.itemSelector = Vector2.Zero;
			HUD.inventoryTransRight = false;
			HUD.inventoryTransLeft = false;
			HUD.questScroll = (HUD.questDescScroll = (this.questScrollBar = 0f));
			this.invSubStage = 0;
			HUD.equipFade = (HUD.equipFade2 = 0f);
			this.equipSelection = (HUD.equipSelection2 = (HUD.equipSelection3 = 0));
			this.saveable = false;
			this.shopID = -1;
			this.canInput = true;
			this.converseWithID = -1;
			this.tempGold = 0;
			this.regionIntroTime = (int)(this.regionIntroState = 0);
			HUD.miniPromptTime = (HUD.helpTime = (this.miniPromptState = (this.helpState = 0)));
			HUD.helpContinue = -1;
			HUD.helpText = string.Empty;
			this.miniPromptList.Clear();
			this.fidgetPrompt = FidgetPrompt.None;
			HUD.fidgetText = string.Empty;
			HUD.changeProjectileTime = 1f;
			HUD.hudAnimFrameTime = 0f;
			this.hudDim = (HUD.navDim = 1f);
			HUD.promptAlpha = 0f;
			HUD.itemAlpha = 0f;
			this.bossLife = (this.bossMaxLife = 0f);
			this.eventLeftOffset = 800f;
			this.XPBarPos = 0f;
			HUD.curUpgrade = (this.levelUpQueue = 0);
			this.inBoss = false;
			this.runInTime = 0f;
			this.expDialogueOffset = -300f;
			this.comboTextSize = 1f;
			this.screenDimAlpha = 0f;
			this.inventoryState = InventoryState.None;
			this.inventoryScreenOffset = new Vector2(0f, 1f);
			for (int i = 0; i < HUD.upgradeCategory.Length; i++)
			{
				HUD.upgradeCategory[i] = 0;
			}
			HUD.curUpgrade = 0;
			HUD.questPage = QuestPage.Active;
			this.unlockGame = null;
			this.unlockState = 0;
			if (all)
			{
				this.hudDetails = true;
				if (!Game1.standardDef)
				{
					this.screenLeftOffset = Game1.screenWidth / 9;
					this.screenTopOffset = Game1.screenHeight / 9;
					this.hudScale = 1f;
				}
				else
				{
					this.screenLeftOffset = Game1.screenWidth / 6;
					this.screenTopOffset = Game1.screenHeight / 8;
					this.hudScale = 1.4f;
				}
			}
		}

		private bool CheckSave(Character c)
		{
			if (this.savePos.X != 0f)
			{
				if (Game1.longSkipFrame == 2 && (this.savePos.X + 150f) * Game1.worldScale > Game1.Scroll.X && (this.savePos.X - 150f) * Game1.worldScale < Game1.Scroll.X + (float)Game1.screenWidth && (this.savePos.Y + 200f) * Game1.worldScale > Game1.Scroll.Y && (this.savePos.Y - 400f) * Game1.worldScale < Game1.Scroll.Y + (float)Game1.screenHeight)
				{
					Game1.pManager.AddVerticleBeam(this.savePos - new Vector2(Rand.GetRandomInt(-50, 50), 160f), Rand.GetRandomVector2(-150f, 150f, -10f, 10f), Rand.GetRandomFloat(0f, 1f), 1f, 1f, 0.1f, Rand.GetRandomInt(50, 100), 800, Rand.GetRandomFloat(0.2f, 1f), -1, 5);
				}
				if (c.Location.X > this.savePos.X - 150f && c.Location.X < this.savePos.X + 150f && c.Location.Y > this.savePos.Y - 400f && c.Location.Y < this.savePos.Y + 200f && Game1.menu.prompt != promptDialogue.StorageDisconnected)
				{
					if ((!Game1.GamerServices || !Game1.IsTrial) && HUD.fidgetBubbleTimer <= 0f && c.State == CharState.Grounded)
					{
						this.InitFidgetPrompt(FidgetPrompt.Save);
					}
					return true;
				}
			}
			return false;
		}

		private bool CheckShop(Character c)
		{
			if (this.shopID > -1 && c.State == CharState.Grounded && c.Location.X > this.shopPos.X - 100f && c.Location.X < this.shopPos.X + 100f && c.Location.Y > this.shopPos.Y - 400f && c.Location.Y < this.shopPos.Y + 200f)
			{
				this.InitFidgetPrompt(FidgetPrompt.Shop);
				return true;
			}
			return false;
		}

		public void LimitInput()
		{
			Character character = this.character[0];
			character.PressedKey = PressedKeys.None;
			character.KeyLeft = false;
			character.KeyRight = false;
			character.KeyJump = false;
			character.KeyUp = false;
			character.KeyDown = false;
			character.KeyAttack = false;
			character.KeySecondary = false;
			character.KeyThrow = false;
			character.KeyEvade = false;
			character.Ethereal = EtherealState.Ethereal;
		}

		public void SaveGame(ParticleManager pMan)
		{
			if (this.character[0].HP <= 0 || Game1.stats.playerLifeState != 0 || Game1.events.anyEvent)
			{
				return;
			}
			HUD.promptAlpha = 0f;
			this.KeySelect = false;
			this.playerSaveLoc = this.character[0].Location;
			Game1.stats.manualSaveSlot = Game1.stats.saveSlot;
			Game1.storage.Write(1, Game1.stats.saveSlot);
			Game1.storage.Write(0, 0);
			if (Game1.storage.storeResult == StoreResult.Saved)
			{
				Game1.menu.prompt = promptDialogue.None;
				Game1.menu.ClearPrompt();
				this.InitFidgetPrompt(FidgetPrompt.Saving);
				this.autoSaveTime = 2f;
				Game1.storage.storeResult = StoreResult.None;
				Sound.PlayCue("fidget_autosave");
				for (int i = 0; i < 8; i++)
				{
					Game1.pManager.AddVerticleBeam(this.character[0].Location - new Vector2(Rand.GetRandomInt(-50, 50), 200f), Rand.GetRandomVector2(-100f, 100f, -10f, 10f), Rand.GetRandomFloat(0f, 1f), 1f, 1f, 1f, Rand.GetRandomInt(50, 100), 1000, Rand.GetRandomFloat(0.2f, 1f), 0, 6);
				}
			}
			else if (Game1.gameMode != 0)
			{
				if (Game1.storage.storeResult == StoreResult.StorageFull)
				{
					Game1.menu.prompt = promptDialogue.StorageFull;
				}
				else
				{
					Game1.menu.prompt = promptDialogue.StorageDisconnected;
				}
				Game1.menu.ClearPrompt();
				this.isPaused = false;
			}
			Game1.storage.DisposeContainer();
			GC.Collect();
		}

		public void AutoSaveGame()
		{
			if (this.character[0].HP <= 0 || Game1.stats.playerLifeState != 0 || Game1.events.anyEvent || (Game1.GamerServices && Game1.IsTrial))
			{
				return;
			}
			Game1.storage.Write(1, 0);
			Game1.storage.Write(0, 0);
			if (Game1.storage.storeResult == StoreResult.Saved)
			{
				if (!Game1.GamerServices || !Game1.IsTrial)
				{
					this.InitFidgetPrompt(FidgetPrompt.Saving);
				}
				Game1.storage.storeResult = StoreResult.None;
				Sound.PlayCue("fidget_autosave");
				for (int i = 0; i < 8; i++)
				{
					Game1.pManager.AddVerticleBeam(this.character[0].Location - new Vector2(Rand.GetRandomInt(-50, 50), 200f), Rand.GetRandomVector2(-100f, 100f, -10f, 10f), Rand.GetRandomFloat(0f, 1f), 1f, 1f, 1f, Rand.GetRandomInt(50, 100), 1000, Rand.GetRandomFloat(0.2f, 1f), 0, 6);
				}
			}
			else if (Game1.gameMode != 0)
			{
				if (Game1.storage.storeResult == StoreResult.StorageFull)
				{
					Game1.menu.prompt = promptDialogue.StorageFull;
				}
				else
				{
					Game1.menu.prompt = promptDialogue.StorageDisconnected;
				}
				Game1.menu.ClearPrompt();
				this.isPaused = false;
			}
			Game1.storage.DisposeContainer();
		}

		public void LoadGame()
		{
			HUD.promptAlpha = 0f;
			this.KeySelect = false;
			this.autoSaveTime = 5f;
			Game1.menu.prompt = promptDialogue.None;
			this.character[0].UnloadTextures();
			Game1.storage.Read(1);
			if (Game1.storage.storeResult == StoreResult.Loaded)
			{
				this.isPaused = false;
				this.runInTime = 0f;
				this.tempGold = Game1.stats.Gold;
				Game1.storage.storeResult = StoreResult.None;
				Game1.map.transInFrame = 3f;
			}
			else if (Game1.gameMode != 0)
			{
				Game1.menu.prompt = promptDialogue.Failed;
				Game1.menu.ClearPrompt();
				this.isPaused = false;
			}
			GC.Collect();
		}

		public void Pause()
		{
			this.KeyPause = false;
			if (!this.isPaused)
			{
				Game1.gameMode = Game1.GameModes.Menu;
				Game1.menu.menuMode = MenuMode.None;
				Game1.menu.curMenuPage = 0;
				Game1.menu.curMenuOption = 0;
				Game1.menu.PopulateMenu();
				this.isPaused = true;
				this.OpenMenuButton = false;
				Sound.PlayCue("menu_init");
				if (this.inventoryState != 0)
				{
					Game1.gameMode = Game1.GameModes.Game;
				}
			}
		}

		private string Capitalize(string s)
		{
			if (s.Length > 0)
			{
				return s.Substring(0, 1).ToUpper() + s.Substring(1, s.Length - 1);
			}
			return s;
		}

		private void MoveLifeBar(int i)
		{
			if ((float)this.character[i].HP < this.character[i].LifeBarPercent - 0.001f)
			{
				this.character[i].LifeBarPercent -= Game1.HudTime * (HUD.barSpeed * Math.Abs((float)this.character[i].HP - this.character[i].LifeBarPercent));
			}
			else if ((float)this.character[i].HP > this.character[i].LifeBarPercent + 0.001f)
			{
				this.character[i].LifeBarPercent += Game1.HudTime * (HUD.barSpeed * Math.Abs((float)this.character[i].HP - this.character[i].LifeBarPercent));
			}
		}

		private void MoveXPBar()
		{
			float num = (float)(Game1.stats.XP - Game1.stats.prevLevelXP) / (float)(Game1.stats.nextLevelXP - Game1.stats.prevLevelXP);
			if (num > this.XPBarPos + 0.001f && this.levelUpQueue == 0)
			{
				this.XPBarPos = MathHelper.Clamp(this.XPBarPos + Game1.HudTime * HUD.barSpeed * Math.Abs(num - this.XPBarPos), 0f, num);
			}
			else if (this.levelUpQueue > 0)
			{
				this.XPBarPos += Game1.HudTime * (HUD.barSpeed * (1f - this.XPBarPos));
				if ((double)this.XPBarPos > 0.985)
				{
					this.XPBarPos = 0f;
					this.levelUpQueue--;
				}
			}
			this.XPBarPos = MathHelper.Clamp(this.XPBarPos, 0.01f, 1f);
		}

		private void MoveChargeBar()
		{
			if (Game1.stats.curCharge / (float)Game1.stats.maxCharge > HUD.chargeBarPos + 0.0015f)
			{
				HUD.chargeBarPos = MathHelper.Clamp(HUD.chargeBarPos + Game1.HudTime * (HUD.barSpeed * Math.Abs(Game1.stats.curCharge / (float)Game1.stats.maxCharge - HUD.chargeBarPos)), 0f, 1f);
			}
			else if (Game1.stats.curCharge / (float)Game1.stats.maxCharge < HUD.chargeBarPos - 0.001f)
			{
				HUD.chargeBarPos = MathHelper.Clamp(HUD.chargeBarPos - Game1.HudTime * (HUD.barSpeed * Math.Abs(Game1.stats.curCharge / (float)Game1.stats.maxCharge - HUD.chargeBarPos)), 0f, 1f);
			}
			else
			{
				HUD.chargeBarPos = MathHelper.Clamp(Game1.stats.curCharge / (float)Game1.stats.maxCharge, 0f, 1f);
			}
		}

		public void DrawMenuHud(ParticleManager pMan)
		{
			this.eventLeftOffset = 0f;
			this.hudDim = 1f;
			HUD.fidgetBubble = 0f;
			this.DrawHud(pMan, menu: true, 0f);
		}

		private void DrawHud(ParticleManager pMan, bool menu, float eventOffset)
		{
			if (!menu)
			{
				if (Game1.gameMode != Game1.GameModes.Game)
				{
					return;
				}
				if (eventOffset > 600f)
				{
					if (this.regionIntroState > 0)
					{
						this.DrawFidgetBubble(pMan);
					}
					return;
				}
			}
			Vector2 vector = new Vector2(this.screenLeftOffset - (int)eventOffset, this.screenTopOffset - (int)eventOffset / 2) - new Vector2(72f, 45f) * this.hudScale;
			if (this.bossLife > 0f)
			{
				this.DrawBossLifeBar(vector.Y);
			}
			if (HUD.helpTime > 0f && Game1.menu.menuMode != MenuMode.HudAdjust)
			{
				float num = 1f;
				if (this.helpState == 1)
				{
					num = 1f - HUD.helpTime * 5f;
				}
				else if (this.helpState == 3)
				{
					num = HUD.helpTime * 5f;
				}
				vector.Y += num * (Game1.smallFont.MeasureString(HUD.helpText).Y * 0.8f + 40f);
			}
			float num2 = (float)Math.Abs(Math.Sin(this.pulse));
			Color color = new Color(1f, 1f, 1f, this.hudDim);
			HUD.sprite.Draw(HUD.hudTex[0], vector + new Vector2(88f, 38f) * this.hudScale, new Rectangle(335, 0, 129, 68), color, 0f, Vector2.Zero, new Vector2(2f, 1f) * this.hudScale, SpriteEffects.None, 0f);
			int num3 = (int)Math.Min((float)this.character[0].MaxHP * Game1.stats.bonusHealth, 9999f);
			if (this.character[0].HP > 0)
			{
				Color color2 = new Color(0.5f, 1f, 0.2f, this.hudDim);
				if ((float)this.character[0].HP < (float)num3 * 0.2f)
				{
					color2 = new Color(1f, this.lifeBarBeat, this.lifeBarBeat, 1f);
				}
				this.DrawBar(vector + new Vector2(106f, 51f) * this.hudScale, HUD.lifeBarPos, color2, new Vector2(0.71f, 1f) * this.hudScale, backGround: false);
			}
			if (Game1.stats.XP > 0)
			{
				this.DrawBar(vector + new Vector2(155f, 65f) * this.hudScale, this.XPBarPos, new Color(0f, 1f, 1f, this.hudDim), new Vector2(0.51f, 1f) * this.hudScale, backGround: false);
			}
			HUD.sprite.Draw(HUD.hudTex[0], vector, new Rectangle(0, 0, 335, 120), color, 0f, Vector2.Zero, this.hudScale, SpriteEffects.None, 0f);
			Color color3 = color;
			if (HUD.changeProjectileTime > 1f)
			{
				color3 = new Color(1f, 1f, 1f, (float)((int)color.A / 255) - HUD.changeProjectileTime / 4f);
			}
			Color color4;
			if (!Game1.stats.canThrow)
			{
				float num4 = (float)Math.Cos((float)HUD.chargeBarFrame / 4f * 6.28f);
				color3 = new Color(1f, num4, num4, (int)color.A);
				color4 = color3;
			}
			else
			{
				color4 = new Color(1f, HUD.chargeBarPos * 2f, HUD.chargeBarPos * 2f, (this.hudDim - 0.25f) * 4f);
			}
			HUD.sprite.Draw(HUD.hudTex[0], vector + new Vector2(131f, 30f) * this.hudScale, new Rectangle(Game1.stats.projectileType * 40, 376, 40, 40), color3, HUD.changeProjectileTime - 1f, new Vector2(20f, 20f), this.hudScale * HUD.changeProjectileTime, SpriteEffects.None, 0f);
			Vector2 vector2 = vector + new Vector2(356f, 24f) * this.hudScale;
			float scale = MathHelper.Clamp(this.hudScale + this.hudScale * (0.1f / HUD.chargeBarPos), this.hudScale, 2f * this.hudScale);
			HUD.sprite.Draw(HUD.particlesTex[2], vector2 - new Vector2(180f * this.hudScale - 180f * this.hudScale * HUD.chargeBarPos, 0f), new Rectangle(34 * HUD.chargeBarFrame, 1380, 34, (int)(180f * HUD.chargeBarPos)), color4, 1.57f, Vector2.Zero, this.hudScale, SpriteEffects.None, 0f);
			HUD.sprite.Draw(HUD.particlesTex[2], new Vector2(vector2.X - 196f * this.hudScale, vector2.Y + 26f * this.hudScale), new Rectangle(2040 + 34 * HUD.chargeBarFrame, 1380, 34, 67), color4, 1.57f, new Vector2(22f, 60f), scale, SpriteEffects.None, 0f);
			Game1.smallText.Color = color;
			this.scoreDraw.Draw(Game1.stats.LEVEL / 4, vector + new Vector2(132f, 68f) * this.hudScale, 0.7f * this.hudScale, color, ScoreDraw.Justify.Center, 0);
			if ((float)this.character[0].HP < (float)num3 * 0.2f)
			{
				Game1.smallText.Color = new Color(1f, this.lifeBarBeat, this.lifeBarBeat, 1f);
			}
			this.scoreDraw.Draw(this.character[0].HP, vector + new Vector2(57f, 17f) * this.hudScale, 0.9f * this.hudScale, Game1.smallText.Color, ScoreDraw.Justify.Center, 0);
			this.scoreDraw.Draw(num3, vector + new Vector2(57f, 45f) * this.hudScale, 0.5f * this.hudScale, color, ScoreDraw.Justify.Center, 0);
			if (Game1.stats.currentItem > -1)
			{
				HUD.sprite.Draw(HUD.particlesTex[4], vector + new Vector2(57f, 103f) * this.hudScale, new Rectangle(0, Game1.stats.currentItem * 60, 60, 60), Game1.smallText.Color, 0f, new Vector2(30f, 30f), this.hudScale, SpriteEffects.None, 0f);
				this.scoreDraw.Draw(Game1.stats.Equipment[Game1.stats.currentItem], vector + new Vector2(55f, 107f) * this.hudScale, this.hudScale, Game1.smallText.Color, ScoreDraw.Justify.Center, 0);
				if ((float)this.character[0].HP < (float)num3 * 0.2f && Game1.pcManager.inputDevice == InputDevice.GamePad)
				{
					HUD.sprite.Draw(HUD.hudTex[0], vector + new Vector2(18f + num2 * 5f, 100f) * this.hudScale, new Rectangle(150, 140, 50, 45), Game1.smallText.Color, 0f, new Vector2(25f, 20f), this.hudScale * 0.8f, SpriteEffects.None, 0f);
				}
			}
			this.scoreDraw.Draw(this.tempGold, vector + new Vector2(222f, 90f) * this.hudScale, 0.7f * this.hudScale, new Color(1f, 1f, 0.5f, this.hudDim), ScoreDraw.Justify.Left, 0);
			HUD.sprite.Draw(HUD.particlesTex[2], vector + new Vector2(209f, 99f) * this.hudScale, new Rectangle(540 + HUD.coinAnimFrame * 20, 2110, 20, 20), new Color(1f, 1f, 0f, this.hudDim), 0f, new Vector2(10f, 10f), 1f * this.hudScale, SpriteEffects.None, 1f);
			Vector2 vector3 = new Vector2((float)Game1.screenWidth - vector.X, vector.Y);
			if (Game1.stats.comboTimer > 0f)
			{
				float a = MathHelper.Clamp(Game1.stats.comboTimer * 3f, 0f, HUD.navDim);
				Game1.smallText.Color = new Color(1f, 1f, 1f, a);
				HUD.sprite.Draw(HUD.hudTex[0], vector3 + new Vector2(-80f, 50f) * this.hudScale, new Rectangle(0, 205, 200, 171), Game1.smallText.Color, 0f, new Vector2(120f, 50f), (0.8f + this.comboTextSize * 0.2f) * this.hudScale, SpriteEffects.None, 0f);
				this.scoreDraw.Draw(Game1.stats.damageMeter, vector3 + new Vector2(-60f, 125f) * this.hudScale, this.comboTextSize * 0.75f * this.hudScale, new Color(1f, 1f, 0f, a), ScoreDraw.Justify.Middle, 1);
				this.scoreDraw.Draw(Game1.stats.comboMeter, vector3 + new Vector2(-80f, 50f) * this.hudScale, this.comboTextSize * this.hudScale, Game1.smallText.Color, ScoreDraw.Justify.Middle, 2);
			}
			else if (Game1.stats.comboBreak > 0 && Game1.events.currentEvent > 0)
			{
				if (Game1.stats.comboBreak == 2)
				{
					pMan.AddCaption(new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 3 - 30), Strings_Hud.Status_ChainBreak, 1.2f, new Color(1f, 1f, 0f, 1f), 2f, 9);
				}
				Vector2 loc = vector3 + new Vector2(-80f, 50f) * this.hudScale;
				for (int i = 0; i < 2; i++)
				{
					pMan.AddSlash(loc, 0.75f * this.hudScale, Rand.GetRandomFloat(0f, 3.14f), CharDir.Right, 9);
				}
				for (int j = 0; j < 12; j++)
				{
					pMan.AddComboBreak(loc, (0.8f + this.comboTextSize * 0.2f) * this.hudScale, j, 9);
				}
				Game1.stats.comboBreak = 0;
			}
			this.DrawMap(new Vector2(eventOffset, vector.Y), Color.White, 0f, 0);
			this.DrawFidgetBubble(pMan);
			if (menu)
			{
				return;
			}
			if (this.miniPromptList.Count > 0)
			{
				if (!Game1.events.anyEvent)
				{
					this.DrawMiniPrompt(0);
				}
			}
			else if (HUD.miniPromptTime > 0f)
			{
				HUD.miniPromptTime -= Game1.HudTime;
				if (HUD.miniPromptTime < 0f)
				{
					this.miniPromptState = 0;
				}
			}
		}

		public void InitFidgetPrompt(FidgetPrompt newPrompt)
		{
			bool flag = false;
			switch (newPrompt)
			{
			case FidgetPrompt.TreasureNear:
				if (Game1.pManager.CheckMapTreasureList() && Game1.events.currentEvent >= 5)
				{
					flag = true;
					HUD.fidgetBubbleTimer = 3f;
				}
				break;
			case FidgetPrompt.ChargeEmpty:
				if (Game1.stats.gameDifficulty > 0)
				{
					flag = true;
					HUD.fidgetBubbleTimer = 0.5f;
					if (this.fidgetPrompt != newPrompt)
					{
						Sound.PlayCue("fidget_fail");
					}
					Game1.events.InitEvent(9, isSideEvent: true);
				}
				break;
			case FidgetPrompt.NoEscape:
				flag = true;
				HUD.fidgetBubbleTimer = 1.5f;
				Sound.PlayCue("fidget_fail");
				break;
			case FidgetPrompt.Saving:
				flag = true;
				HUD.fidgetBubbleTimer = 1f;
				break;
			default:
				HUD.fidgetBubbleTimer = 0f;
				flag = true;
				break;
			}
			if (flag && (this.fidgetPrompt != newPrompt || this.converseWithID != HUD.prevConverseWithID))
			{
				if (this.fidgetPrompt != 0)
				{
					HUD.fidgetBubble = 0f;
				}
				this.fidgetPrompt = newPrompt;
				HUD.fidgetText = this.GetFidgetText();
				if (this.fidgetPrompt > FidgetPrompt.NoLookUp)
				{
					HUD.fidgetPromptX = (int)((this.fidgetPrompt < FidgetPrompt.Save) ? (this.fidgetPrompt - 2) : (this.fidgetPrompt - 18));
				}
			}
		}

		public void UpdateFidget(ParticleManager pMan)
		{
			if (Game1.longSkipFrame > 3)
			{
				if (Game1.stats.curCharge < (float)Game1.stats.projectileCost)
				{
					this.InitFidgetPrompt(FidgetPrompt.ChargeEmpty);
				}
				else if (this.character[0].CanFallThrough && this.character[0].ledgeAttach > -1 && this.character[0].Location == this.character[0].PLoc && !this.character[0].AnimName.StartsWith("attack") && (this.fidgetPrompt < FidgetPrompt.Save || this.fidgetPrompt == FidgetPrompt.DropDown || this.fidgetPrompt == FidgetPrompt.ChargeEmpty))
				{
					this.InitFidgetPrompt(FidgetPrompt.DropDown);
				}
				else if (this.converseWithID > -1)
				{
					this.InitFidgetPrompt(FidgetPrompt.Speak);
				}
				else if (this.collectID > -1)
				{
					this.InitFidgetPrompt(FidgetPrompt.Collect);
				}
				else if (this.fidgetPrompt == FidgetPrompt.NoLookUp)
				{
					if (!this.character[0].KeyUp)
					{
						this.fidgetPrompt = FidgetPrompt.None;
					}
				}
				else if (this.fidgetPrompt > (FidgetPrompt)19)
				{
					this.fidgetPrompt = FidgetPrompt.None;
				}
				HUD.prevConverseWithID = this.converseWithID;
			}
			if (Game1.skipFrame <= 1 || !this.noEvent)
			{
				return;
			}
			this.saveable = this.CheckSave(this.character[0]);
			HUD.shoppable = this.CheckShop(this.character[0]);
			if (this.saveable && !this.isPaused && this.inventoryState == InventoryState.None)
			{
				this.SaveEffect(pMan);
				if (this.autoSaveTime <= 0f)
				{
					this.playerSaveLoc = this.character[0].Location;
					this.AutoSaveGame();
				}
				this.autoSaveTime = 2f;
			}
		}

		private string GetFidgetText()
		{
			HUD.fidgetButtonList.Clear();
			float size = 1f;
			switch (this.fidgetPrompt)
			{
			case FidgetPrompt.ChargeEmpty:
				return Game1.smallText.WordWrap("[ICON_363]", size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.OpenTreasure:
			case FidgetPrompt.OpenCage:
			{
				int num = 1;
				if (this.fidgetPrompt == FidgetPrompt.OpenCage)
				{
					num = 4;
				}
				if (Game1.stats.Equipment[305] < num)
				{
					return Game1.smallText.WordWrap("[ICON_365]" + Game1.stats.Equipment[305], size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
				}
				return Game1.smallText.WordWrap("[UP] " + Strings_Hud.Fidget_OpenChest + "\n  [ICON_365]" + Game1.stats.Equipment[305], size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			}
			case FidgetPrompt.DropDown:
				return Game1.smallText.WordWrap(" [DOWN]+[A]", size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.TreasureNear:
				return Game1.smallText.WordWrap("[ICON_364] !?", size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.Saving:
				return Game1.smallText.WordWrap(Strings_Hud.Fidget_AutoSaving, size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.Save:
				return Game1.smallText.WordWrap("[UP] " + Strings_Hud.Fidget_Save, size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.Shop:
				return Game1.smallText.WordWrap("[UP] " + Strings_Hud.Fidget_Shop, size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.EnterDoor:
				return Game1.smallText.WordWrap("[UP] " + Strings_Hud.Fidget_Enter, size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.Score:
				return Game1.smallText.WordWrap("[UP] " + Strings_Hud.Fidget_Score, size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.GateBlue:
				return Game1.smallText.WordWrap(Strings_Hud.Gate_Blue, size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.GateRed:
				return Game1.smallText.WordWrap(Strings_Hud.Gate_Red, size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.GateGreen:
				return Game1.smallText.WordWrap(Strings_Hud.Gate_Green, size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.GateYellow:
				return Game1.smallText.WordWrap(Strings_Hud.Gate_Yellow, size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.GateWhite:
				return Game1.smallText.WordWrap(Strings_Hud.Gate_White, size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.GatePurple:
				return Game1.smallText.WordWrap(Strings_Hud.Gate_Purple, size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.GateRainbow:
				return Game1.smallText.WordWrap(Strings_Hud.Gate_Rainbow, size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.Collect:
				if (this.collectID > -1)
				{
					return Game1.smallText.WordWrap("[UP] " + Strings_Hud.Fidget_Collect + "\n" + Strings_Equipment.ResourceManager.GetString($"{this.character[this.collectID].CollectEquipID:D3}"), size, 1000f, HUD.fidgetButtonList, TextAlign.LeftAndCenter);
				}
				break;
			case FidgetPrompt.NoEscape:
				return Game1.smallText.WordWrap(Strings_Hud.Fidget_NoEscape, size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
			case FidgetPrompt.Speak:
				if (this.converseWithID > -1)
				{
					CharacterType charType = CharacterType.Dust;
					for (int i = 1; i < this.character.Length; i++)
					{
						if (i == this.converseWithID)
						{
							charType = this.character[i].Definition.charType;
							break;
						}
					}
					return Game1.smallText.WordWrap("[UP] " + Game1.GetFriendlyName(charType), size, 1000f, HUD.fidgetButtonList, TextAlign.Left);
				}
				return HUD.fidgetText;
			}
			return HUD.fidgetText;
		}

		private void DrawFidgetBubble(ParticleManager pMan)
		{
			if ((HUD.fidgetBubbleTimer > 0f || this.fidgetPrompt > (FidgetPrompt)19) && this.map.warpStage == 0 && this.map.doorStage == 0 && (this.regionIntroState == 0 || this.fidgetPrompt != FidgetPrompt.TreasureNear) && this.unlockState == 0 && this.dialogueState == DialogueState.Inactive)
			{
				if (this.map.GetTransVal() <= 0f && Game1.menu.prompt == promptDialogue.None && !Game1.events.anyEvent)
				{
					HUD.fidgetBubbleTimer -= Game1.FrameTime;
					if (HUD.fidgetBubble == 0f && this.fidgetPrompt != FidgetPrompt.Saving && this.fidgetPrompt != FidgetPrompt.ChargeEmpty && Game1.gameMode == Game1.GameModes.Game)
					{
						Sound.PlayCue("fidget_prompt");
					}
					HUD.fidgetBubble = Math.Min(HUD.fidgetBubble + Game1.FrameTime * 6f, 1.5f);
				}
			}
			else
			{
				HUD.fidgetBubble = Math.Max(HUD.fidgetBubble - Game1.FrameTime * 6f, 0f);
			}
			if (HUD.fidgetBubble > 0f)
			{
				float num = Math.Min(HUD.fidgetBubble, 1f);
				Vector2 bubblePos = ((Game1.stats.fidgetState != 0) ? ((this.character[0].Location - new Vector2(0f, 340f)) * Game1.worldScale - Game1.Scroll) : (pMan.GetFidgetLoc(accomodateScroll: true) - new Vector2(20f, 120f) * Game1.hiDefScaleOffset));
				Game1.smallText.Color = Color.White * num;
				float num2 = Math.Max(Game1.smallFont.MeasureString(HUD.fidgetText).X + 50f, 100f);
				float num3 = Math.Max(num2 - 80f * (1f - num), 100f);
				float bubbleHeight = Game1.smallFont.MeasureString(HUD.fidgetText).Y - 7f;
				bubblePos.X = MathHelper.Clamp(bubblePos.X, num3 / 2f - 30f, (float)Game1.screenWidth - num3 / 2f - 80f);
				bubblePos.Y = MathHelper.Clamp(bubblePos.Y, (float)Game1.screenHeight * 0.05f, (float)Game1.screenHeight * 0.95f);
				bubblePos = this.DrawTextBubble(bubblePos, num3, bubbleHeight, num * 0.75f);
				Game1.smallText.DrawButtonText(bubblePos + new Vector2((0f - num2) / 2f + 52f, 10f), HUD.fidgetText, 1f, HUD.fidgetButtonList, bounce: false, num2 + 10f, TextAlign.Center);
			}
		}

		public Vector2 DrawTextBubble(Vector2 bubblePos, float bubbleWidth, float bubbleHeight, float alpha)
		{
			float num = bubbleWidth - 60f;
			bubbleHeight = Math.Max(bubbleHeight, 30f);
			bubblePos.Y -= bubbleHeight - 30f;
			HUD.sprite.Draw(HUD.hudTex[0], bubblePos + new Vector2((0f - num) / 2f + 50f, 0f), new Rectangle(325, 185, 1, 30), Color.White * alpha, 0f, Vector2.Zero, new Vector2(num, 1f), SpriteEffects.None, 0f);
			HUD.sprite.Draw(HUD.hudTex[0], bubblePos + new Vector2((0f - num) / 2f + 22f, 0f), new Rectangle(295, 185, 28, 30), Color.White * alpha);
			HUD.sprite.Draw(HUD.hudTex[0], bubblePos + new Vector2(num / 2f + 50f, 0f), new Rectangle(364, 185, 31, 30), Color.White * alpha);
			if (bubbleHeight > 30f)
			{
				HUD.sprite.Draw(HUD.hudTex[0], bubblePos + new Vector2((0f - num) / 2f + 22f, 30f), new Rectangle(295, 215, 28, 1), Color.White * alpha, 0f, Vector2.Zero, new Vector2(1f, bubbleHeight - 30f), SpriteEffects.None, 0f);
				HUD.sprite.Draw(HUD.hudTex[0], bubblePos + new Vector2((0f - num) / 2f + 50f, 30f), new Rectangle(330, 215, 1, 1), Color.White * alpha, 0f, Vector2.Zero, new Vector2(num, bubbleHeight - 30f), SpriteEffects.None, 0f);
				HUD.sprite.Draw(HUD.hudTex[0], bubblePos + new Vector2(num / 2f + 50f, 30f), new Rectangle(364, 215, 31, 1), Color.White * alpha, 0f, Vector2.Zero, new Vector2(1f, bubbleHeight - 30f), SpriteEffects.None, 0f);
			}
			bubbleHeight -= 30f;
			HUD.sprite.Draw(HUD.hudTex[0], bubblePos + new Vector2((0f - (bubbleWidth - 60f)) / 2f + 50f, 30f + bubbleHeight), new Rectangle(325, 215, 1, 30), Color.White * alpha, 0f, Vector2.Zero, new Vector2(bubbleWidth / 2f - 50f, 1f), SpriteEffects.None, 0f);
			HUD.sprite.Draw(HUD.hudTex[0], bubblePos + new Vector2(68f, 30f + bubbleHeight), new Rectangle(325, 215, 1, 30), Color.White * alpha, 0f, Vector2.Zero, new Vector2(bubbleWidth / 2f - 48f, 1f), SpriteEffects.None, 0f);
			HUD.sprite.Draw(HUD.hudTex[0], bubblePos + new Vector2((0f - num) / 2f + 22f, 30f + bubbleHeight), new Rectangle(295, 215, 28, 30), Color.White * alpha);
			HUD.sprite.Draw(HUD.hudTex[0], bubblePos + new Vector2(num / 2f + 50f, 30f + bubbleHeight), new Rectangle(364, 215, 31, 30), Color.White * alpha);
			HUD.sprite.Draw(HUD.hudTex[0], bubblePos + new Vector2(30f, 30f + bubbleHeight), new Rectangle(325, 215, 38, 45), Color.White * alpha);
			return bubblePos;
		}

		public void InitHelp(string text, bool restart, int _helpContinue)
		{
			if (HUD.helpPrevText != text)
			{
				restart = true;
			}
			if (this.helpState == 0 || restart)
			{
				HUD.helpPrevText = text;
				if (_helpContinue > -1)
				{
					if (!Game1.events.sideEventAvailable[_helpContinue])
					{
						return;
					}
					HUD.helpContinue = _helpContinue;
					Game1.events.sideEventAvailable[_helpContinue] = false;
					HUD.loadingHelp = true;
					HUD.helpTextWidth = 640;
					HUD.helpText = text;
					Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadHelpText)));
				}
				else
				{
					HUD.loadingHelp = true;
					HUD.helpTextWidth = Game1.screenWidth - this.screenLeftOffset * 2;
					HUD.helpText = text;
					Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadHelpText)));
				}
			}
			if (this.helpState == 2)
			{
				HUD.helpTime = 1f;
			}
		}

		private void LoadHelpText()
		{
			try
			{
				string name = ((Game1.isPCBuild && Game1.pcManager.inputDevice != 0) ? ("pc" + HUD.helpText) : HUD.helpText);
				if (Strings_Help.ResourceManager.GetString(name) == null)
				{
					name = HUD.helpText;
				}
				HUD.helpText = Game1.smallText.WordWrap(Strings_Help.ResourceManager.GetString(name), 0.8f, HUD.helpTextWidth, HUD.helpTextButtonList, TextAlign.Center);
				this.helpState = 1;
				HUD.helpTime = 0.2f;
			}
			catch (Exception)
			{
			}
			HUD.loadingHelp = false;
		}

		public void ClearHelp()
		{
			HUD.helpContinue = -1;
			this.helpState = 0;
			HUD.helpTime = 0f;
		}

		private void DrawHelp()
		{
			Color color = Color.White;
			float num = 0f;
			if ((HUD.helpContinue == -1 && (Game1.events.anyEvent || Game1.FrameTime == 0f)) || HUD.loadingHelp)
			{
				color.A = 0;
			}
			else if (this.helpState == 1)
			{
				color = new Color(1f, 1f, 1f, 1f - HUD.helpTime * 5f);
				num = HUD.helpTime * 5f * 20f;
			}
			else if (this.helpState == 3)
			{
				color = new Color(1f, 1f, 1f, HUD.helpTime * 5f);
				num = (1f - HUD.helpTime * 5f) * -20f;
			}
			float num2 = 0.8f;
			Game1.smallText.Color = color;
			if (HUD.helpContinue == -1)
			{
				int num3 = (int)(Game1.smallFont.MeasureString(HUD.helpText).X * num2);
				int num4 = (int)(Game1.smallFont.MeasureString(HUD.helpText).Y * num2) + 20;
				Vector2 orig = new Vector2((Game1.screenWidth - num3) / 2, (float)Game1.screenHeight * 0.1f);
				this.DrawMiniBorder(orig, num3, num4, (float)(int)color.A / 300f, 1f, num4, 1f);
				Game1.smallText.DrawButtonText(new Vector2(this.screenLeftOffset, orig.Y + 10f), HUD.helpText, num2, HUD.helpTextButtonList, bounce: true, Game1.screenWidth, TextAlign.Left);
			}
			else
			{
				int num5 = 700;
				int num4 = 75 + (int)(Game1.smallFont.MeasureString(HUD.helpText).Y * num2);
				Vector2 orig = new Vector2((Game1.screenWidth - num5) / 2, (float)((Game1.screenHeight - num4) / 3) - num);
				HUD.sprite.Draw(HUD.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, 0.6f));
				this.DrawMiniBorder(orig, num5, num4, color, 0.95f);
				Game1.smallText.DrawButtonText(orig + new Vector2(30f, 10f), HUD.helpText, num2, HUD.helpTextButtonList, bounce: true, num5, TextAlign.Left);
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
				{
					if (Game1.pcManager.DrawMouseButton(orig + new Vector2(num5 - 60, num4 - 60), 0.8f, color, 2, draw: true))
					{
						this.helpState = 3;
						HUD.helpTime = 0.2f;
					}
					Game1.pcManager.DrawCursor(HUD.sprite, 0.8f, color);
				}
				else if (Game1.pcManager.inputDevice == InputDevice.GamePad)
				{
					HUD.sprite.Draw(HUD.hudTex[0], orig + new Vector2(num5 - 30, num4 - 30), new Rectangle(200, 140, 46, 46), new Color(1f, 1f, 1f, 0.75f + (float)Math.Sin(this.pulse)), 0f, new Vector2(24f, 22f), 0.8f, SpriteEffects.None, 0f);
				}
			}
			if (Game1.events.anyEvent)
			{
				return;
			}
			if (HUD.helpTime > 0f)
			{
				if ((this.eventLeftOffset < 100f && HUD.helpContinue == -1) || (HUD.helpContinue > -1 && this.helpState != 2))
				{
					HUD.helpTime -= Game1.HudTime;
				}
				if (HUD.helpTime < 0f)
				{
					this.helpState++;
					if (this.helpState == 2)
					{
						HUD.helpTime = 10f;
						if (Game1.events.currentEvent > -1 && this.inventoryState == InventoryState.None)
						{
							Sound.PlayCue("help_init");
						}
					}
					else
					{
						HUD.helpTime = 0.2f;
					}
					if (this.helpState > 3)
					{
						this.ClearHelp();
					}
				}
			}
			if (this.inventoryState != 0 && HUD.helpContinue == -1)
			{
				this.ClearHelp();
			}
			if (Game1.menu.prompt != promptDialogue.None && Game1.menu.prompt != promptDialogue.SkipEvent)
			{
				this.ClearHelp();
			}
		}

		public void InitMiniPrompt(MiniPromptType promptCategory, int id, bool blueprint)
		{
			lock (HUD.syncObject)
			{
				if (this.miniPromptState == 0)
				{
					this.miniPromptState = 1;
					HUD.miniPromptTime = 0.2f;
				}
				string text = null;
				int num = -10;
				int displayTime = 4;
				bool flag = true;
				switch (promptCategory)
				{
					case MiniPromptType.ShopRestocked:
					{
						text = Strings_Hud.Mini_ShopRestock;
						for (int k = 0; k < this.miniPromptList.Count; k++)
						{
							if (this.miniPromptList[k].rawText == text)
							{
								flag = false;
							}
						}
						if (flag)
						{
							this.miniPromptList.Add(new MiniPrompt(text, (int)promptCategory, 5));
						}
						return;
					}
					case MiniPromptType.FriendRescued:
						text = Strings_Hud.Mini_Rescued;
						this.miniPromptList.Add(new MiniPrompt(text, (int)promptCategory, 5));
						return;
					case MiniPromptType.RegionAdded:
					{
						text = Strings_Hud.Mini_RegionAdded;
						for (int j = 0; j < this.miniPromptList.Count; j++)
						{
							if (this.miniPromptList[j].rawText == text)
							{
								flag = false;
							}
						}
						if (flag)
						{
							this.miniPromptList.Add(new MiniPrompt(text, (int)promptCategory, 5));
						}
						return;
					}
					case MiniPromptType.ItemsRemoved:
					{
						text = Strings_Hud.Mini_ItemsRemoved;
						for (int i = 0; i < this.miniPromptList.Count; i++)
						{
							if (this.miniPromptList[i].rawText == text)
							{
								flag = false;
							}
						}
						if (flag)
						{
							this.miniPromptList.Add(new MiniPrompt(text, (int)promptCategory, displayTime));
						}
						return;
					}
					case MiniPromptType.GoldAcquired:
						text = Strings_Hud.Mini_GoldAcquired;
						this.miniPromptList.Add(new MiniPrompt(id + " " + text, (int)promptCategory, 5));
						return;
					case MiniPromptType.GoldRemoved:
						text = Strings_Hud.Mini_GoldRemoved;
						this.miniPromptList.Add(new MiniPrompt(id + " " + text, (int)promptCategory, 5));
						return;
					case MiniPromptType.XPAcquired:
						text = Strings_Hud.Mini_XPAcquired;
						this.miniPromptList.Add(new MiniPrompt(id + " " + text, (int)promptCategory, 5));
						return;
					case MiniPromptType.MaterialAcquired:
						return;
					case MiniPromptType.MaterialFull:
						text = Strings_Hud.Mini_InvFull;
						num = id + 1000;
						displayTime = 1;
						break;
					case MiniPromptType.LevelUp:
						text = "[BACK] " + Strings_Hud.Mini_SkillGem;
						num = (int)(id + promptCategory);
						displayTime = 5;
						break;
					case MiniPromptType.UpgradePrompt:
						switch (id)
						{
							case 0:
								text = "DUST STORM";
								break;
							case 1:
								text = "FIDGET PROJECTILE";
								break;
							case 2:
								text = "FIDGET PROJECTILE";
								break;
							case 3:
								text = "FIDGET PROJECTILE";
								break;
							case 4:
								text = "SLASH";
								break;
							case 5:
								text = "JUMP";
								break;
							case 10:
								text = "DASH";
								break;
							case 12:
								text = "AERIAL DUST STORM";
								break;
							case 14:
								text = "CROUCH SLIDE";
								break;
							case 15:
								text = "DOUBLE JUMP";
								break;
							case 16:
								text = "IRON GRIP";
								break;
							case 17:
								text = "BOOST JUMP";
								break;
							default:
								text = "UNKNOWN ITEM";
								break;
						}
						text = text + " Acquired.";
						num = 20000;
						displayTime = 2;
						break;
					default:
						if (Game1.inventoryManager.equipItem[id] != null)
						{
							if (promptCategory > (MiniPromptType)(-1))
							{
								text = Game1.inventoryManager.equipItem[id].Name;
								displayTime = 2;
							}
							else
							{
								text = Strings_Hud.Mini_InvFull;
								displayTime = 1;
							}
						}
						else
						{
							text = "Unnamed equipment!";
						}
						if (blueprint)
						{
							text = text + " (" + Strings_HudInv.Blueprint + ")";
						}
						num = id;
						break;
				}
				if (text == null)
				{
					return;
				}
				bool flag2 = true;
				if (promptCategory > (MiniPromptType)(-1) && promptCategory < MiniPromptType.Quest)
				{
					for (int l = 0; l < this.miniPromptList.Count; l++)
					{
						if (this.miniPromptList[l].rawText == text)
						{
							this.miniPromptList[l].AddMultiplier();
							if (l == 0 && this.miniPromptState == 2)
							{
								HUD.miniPromptTime = this.miniPromptList[l].displayTime;
							}
							flag2 = false;
						}
					}
				}
				if (flag2)
				{
					this.miniPromptList.Add(new MiniPrompt(text, num, displayTime));
				}
			}
		}

		public void RemoveMiniPrompt(MiniPromptType promptCategory, int id)
		{
			for (int i = 0; i < this.miniPromptList.Count; i++)
			{
				if (this.miniPromptList[i].ID == id)
				{
					this.miniPromptList.Remove(this.miniPromptList[i]);
				}
			}
		}

		private void DrawMiniPrompt(int promptID)
		{
			if (Game1.events.anyEvent)
			{
				return;
			}
			Color color = Color.White;
			float num = 0.8f;
			float num2 = 0f;
			if (this.miniPromptState == 1)
			{
				color = new Color(1f, 1f, 1f, 1f - HUD.miniPromptTime * 5f);
				num2 = HUD.miniPromptTime * 5f * 20f;
			}
			else if (this.miniPromptState == 3)
			{
				color = new Color(1f, 1f, 1f, HUD.miniPromptTime * 5f);
				num2 = (1f - HUD.miniPromptTime * 5f) * -20f;
			}
			int num3 = (int)(Game1.smallFont.MeasureString(this.miniPromptList[promptID].text).Y * num) + 15;
			int num4 = (int)(Game1.smallFont.MeasureString(this.miniPromptList[promptID].text).X * num) + 40;
			Vector2 vector = new Vector2(Math.Max(0, this.screenLeftOffset - 60), Math.Min(Game1.screenHeight - this.screenTopOffset - num3 + 20, Game1.screenHeight - num3));
			Vector2 vector2 = vector + new Vector2(0f - this.eventLeftOffset, 0f - num2 + this.eventLeftOffset / 2f);
			this.DrawMiniBorder(vector2, num4, num3, (float)(int)color.A / 255f, 0.9f, num3, 1f);
			Game1.smallText.Color = color;
			Game1.smallText.DrawButtonText(vector2 + new Vector2(25f, 8f), this.miniPromptList[promptID].text, num, this.miniPromptList[promptID].textButtonList, bounce: false, num4, TextAlign.Left);
			if (HUD.miniPromptTime > 0f)
			{
				if (this.eventLeftOffset < 100f && this.map.GetTransVal() == 0f)
				{
					HUD.miniPromptTime -= Game1.HudTime;
				}
				if (HUD.miniPromptTime < 0f)
				{
					this.miniPromptState++;
					if (this.miniPromptState == 2)
					{
						HUD.miniPromptTime = this.miniPromptList[0].displayTime;
					}
					else
					{
						HUD.miniPromptTime = 0.2f;
					}
					if (this.miniPromptState > 3)
					{
						this.miniPromptList.RemoveRange(0, 1);
						if (this.miniPromptList.Count > 0)
						{
							this.miniPromptState = 1;
						}
						else
						{
							this.miniPromptState = 0;
						}
					}
				}
			}
			if (this.miniPromptState == 2 && this.miniPromptList[0].audio > -1 && this.eventLeftOffset < 20f)
			{
				switch (this.miniPromptList[0].audio)
				{
				case 2:
					Sound.PlayCue("quest_added");
					break;
				case 4:
					Sound.PlayCue("quest_complete");
					break;
				}
				this.miniPromptList[0].audio = -1;
				for (int i = 0; i < 30; i++)
				{
					Game1.pManager.AddSparkle(vector + Rand.GetRandomVector2(-20f, num4 + 40, -10f, num3 + 20), 1f, 1f, 1f, 1f, 0.5f, Rand.GetRandomInt(36, 48), 9);
				}
			}
		}

		public void InitRegionIntro(string newRegion, string mapPath, bool fromWorldMap)
		{
			bool flag = false;
			this.regionIntroState = 0;
			if (newRegion != string.Empty && newRegion != Game1.events.regionDisplayName)
			{
				if (!Game1.events.InitRegionIntro(mapPath))
				{
					this.regionIntroState = 1;
					this.regionIntroTime = 0.2f;
				}
				Game1.wManager.ForceColor();
				Game1.events.regionDisplayName = newRegion;
				flag = true;
			}
			if (this.character[0].Location.X < this.map.leftEdge + 200f || this.character[0].Location.X > this.map.rightEdge - 200f)
			{
				if (fromWorldMap)
				{
					this.runInTime = 0.7f;
				}
				else if (flag)
				{
					this.runInTime = 0.3f;
				}
			}
		}

		private void DrawRegionIntro(ParticleManager pMan)
		{
			if (Game1.GamerServices && Game1.IsTrial)
			{
				this.regionIntroState = 0;
				return;
			}
			if (Game1.menu.prompt != promptDialogue.None || Game1.events.anyEvent || this.isPaused || this.inventoryState != 0 || Game1.cManager.challengeState > 0)
			{
				this.regionIntroState = 0;
				return;
			}
			Vector2 position = new Vector2(Game1.screenWidth / 2, (float)Game1.screenHeight * 0.2f);
			float num = 0f;
			float a;
			if (this.regionIntroState == 1)
			{
				this.eventLeftOffset = 800f;
				a = 0f;
			}
			else if (this.regionIntroState == 2)
			{
				this.eventLeftOffset = 800f;
				num = MathHelper.Clamp(180f - this.regionIntroTime * 180f, 0f, 180f);
				Color white = Color.White;
				a = 1f - this.regionIntroTime;
				position += new Vector2(0f - num, 0f);
				HUD.sprite.Draw(HUD.hudTex[0], new Vector2((int)position.X + 1, position.Y), new Rectangle(195 - (int)num, 456, (int)num * 2, 180), white);
			}
			else if (this.regionIntroState == 3)
			{
				this.eventLeftOffset = 800f;
				Color white = Color.White;
				a = 1f;
				position += new Vector2(-179f, 0f);
				HUD.sprite.Draw(HUD.hudTex[0], position, new Rectangle(16, 456, 358, 180), white);
			}
			else
			{
				a = this.regionIntroTime;
				Color white = new Color(1f, 1f, 1f, a);
				position += new Vector2(-179f, 0f);
				HUD.sprite.Draw(HUD.hudTex[0], position, new Rectangle(16, 456, 358, 180), white);
			}
			Game1.bigText.Color = new Color(1f, 1f, 1f, a);
			Game1.bigText.DrawOutlineText(new Vector2(0f, position.Y + 58f), Game1.events.regionDisplayName, 1f, Game1.screenWidth, TextAlign.Center, fullOutline: true);
			if (this.regionIntroState == 2 && num > 0f)
			{
				if (num > 40f)
				{
					float x = ((Rand.GetRandomInt(0, 2) != 0) ? (position.X + num * 2f + 50f) : (position.X - 50f));
					pMan.AddSparkle(new Vector2(x, position.Y + Rand.GetRandomFloat(10f, 160f)), 1f, 1f, 1f, 1f, 0.4f, 48, 9);
				}
				HUD.sprite.End();
				HUD.sprite.Begin(SpriteSortMode.Texture, BlendState.Additive);
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						float num2 = 40f * (num / 180f);
						Vector2 vector = new Vector2(position.X + (float)j * (num * 2f) + (float)i * num2 - num2);
						float num3;
						float num4;
						if (i == 1)
						{
							num3 = 1f;
							num4 = num / 180f;
						}
						else
						{
							num3 = 0.5f;
							num4 = num / 180f * 1.5f;
						}
						HUD.sprite.Draw(HUD.hudTex[0], new Vector2(vector.X, position.Y + 90f), new Rectangle(0, 456, 16, 180), new Color(1f, 1f, 1f, 1f - num4), 0f, new Vector2(8f, 90f), new Vector2(1f - num4, num3 + num4 / 2f), SpriteEffects.None, 0f);
					}
				}
				HUD.sprite.End();
				HUD.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			}
			if (!(this.regionIntroTime > 0f))
			{
				return;
			}
			if (this.map.GetTransVal() <= 0f)
			{
				this.regionIntroTime -= Game1.HudTime;
			}
			if (!(this.regionIntroTime < 0f))
			{
				return;
			}
			this.regionIntroState++;
			if (this.regionIntroState == 3)
			{
				this.regionIntroTime = 2f;
				if (Game1.events.regionIntroStage > 0)
				{
					this.regionIntroTime = 5f;
				}
			}
			else
			{
				this.regionIntroTime = 1f;
			}
			if (this.regionIntroState > 4)
			{
				this.regionIntroState = 0;
			}
		}

		private void DrawRegionIntroOld(ParticleManager pMan)
		{
			if (Game1.menu.prompt != promptDialogue.None || Game1.events.anyEvent || this.isPaused || this.inventoryState != 0 || Game1.cManager.challengeState > 0)
			{
				this.regionIntroState = 0;
				return;
			}
			Vector2 position = new Vector2(Game1.screenWidth / 2, 80f);
			float num = 0f;
			float a;
			if (this.regionIntroState == 1)
			{
				this.eventLeftOffset = 800f;
				a = 0f;
			}
			else if (this.regionIntroState == 2)
			{
				this.eventLeftOffset = 800f;
				num = MathHelper.Clamp(180f - this.regionIntroTime * 180f, 0f, 180f);
				Color white = Color.White;
				a = 1f - this.regionIntroTime;
				position += new Vector2(0f - num, 0f);
				HUD.sprite.Draw(HUD.hudTex[0], new Vector2((int)position.X + 1, position.Y), new Rectangle(195 - (int)num, 456, (int)num * 2, 180), white);
			}
			else if (this.regionIntroState == 3)
			{
				this.eventLeftOffset = 800f;
				Color white = Color.White;
				a = 1f;
				position += new Vector2(-179f, 0f);
				HUD.sprite.Draw(HUD.hudTex[0], position, new Rectangle(16, 456, 358, 180), white);
			}
			else
			{
				a = this.regionIntroTime;
				Color white = new Color(1f, 1f, 1f, a);
				position += new Vector2(-179f, 0f);
				HUD.sprite.Draw(HUD.hudTex[0], position, new Rectangle(16, 456, 358, 180), white);
			}
			Game1.bigText.Color = new Color(1f, 1f, 1f, a);
			Game1.bigText.DrawOutlineText(new Vector2(0f, 138f), Game1.events.regionDisplayName, 1f, Game1.screenWidth, TextAlign.Center, fullOutline: true);
			if (this.regionIntroState == 2 && num > 0f)
			{
				if (num > 40f)
				{
					float x = ((Rand.GetRandomInt(0, 2) != 0) ? (position.X + num * 2f + 50f) : (position.X - 50f));
					pMan.AddSparkle(new Vector2(x, position.Y + Rand.GetRandomFloat(10f, 160f)), 1f, 1f, 1f, 1f, 0.4f, 48, 9);
				}
				HUD.sprite.End();
				HUD.sprite.Begin(SpriteSortMode.Deferred, BlendState.Additive);
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						float num2 = 40f * (num / 180f);
						Vector2 vector = new Vector2(position.X + (float)j * (num * 2f) + (float)i * num2 - num2);
						float num3;
						float num4;
						if (i == 1)
						{
							num3 = 1f;
							num4 = num / 180f;
						}
						else
						{
							num3 = 0.5f;
							num4 = num / 180f * 1.5f;
						}
						HUD.sprite.Draw(HUD.hudTex[0], new Vector2(vector.X, position.Y + 90f), new Rectangle(0, 456, 16, 180), new Color(1f, 1f, 1f, 1f - num4), 0f, new Vector2(8f, 90f), new Vector2(1f - num4, num3 + num4 / 2f), SpriteEffects.None, 0f);
					}
				}
				HUD.sprite.End();
				HUD.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			}
			if (!(this.regionIntroTime > 0f))
			{
				return;
			}
			if (this.map.GetTransVal() <= 0f)
			{
				this.regionIntroTime -= Game1.HudTime;
			}
			if (this.regionIntroTime < 0f)
			{
				this.regionIntroState++;
				if (this.regionIntroState == 3)
				{
					this.regionIntroTime = 2f;
				}
				else
				{
					this.regionIntroTime = 1f;
				}
				if (this.regionIntroState > 4)
				{
					this.regionIntroState = 0;
				}
			}
		}

		public void InitUnlocking(int lockType, float lockCenter)
		{
			this.unlockState = 1;
			this.unlockGame = new UnlockGame(lockType, this.character, lockCenter);
		}

		public bool UpdateUnlocking(ParticleManager pMan, bool updating)
		{
			this.LimitInput();
			Game1.character[0].Ethereal = EtherealState.Ethereal;
			this.canInput = false;
			if (this.unlockGame != null)
			{
				if (updating)
				{
					int num = this.unlockGame.Update(pMan, this.character, Game1.HudTime, this);
					if (num > 0)
					{
						this.unlockState = 0;
						this.unlockGame = null;
						if (num == 1)
						{
							return false;
						}
						return true;
					}
				}
				else
				{
					this.unlockGame.UpdateInput(this);
					if (Game1.character[0].AnimName.StartsWith("hurt"))
					{
						this.unlockState = 0;
						this.unlockGame.ExitUnlocking();
					}
				}
			}
			return false;
		}

		public bool InitDialogue(int conversation, CharacterType charType)
		{
			if (this.dialogueState != 0)
			{
				return false;
			}
			Game1.BlurScene(0f);
			Game1.events.anyEvent = true;
			if (conversation == -1)
			{
				Game1.stats.villagerDialogue[(int)charType] = 0;
				Game1.stats.AcquireXP((Game1.stats.nextLevelXP - Game1.stats.prevLevelXP) / 5);
			}
			this.newConversation = Math.Max(conversation, 0);
			this.newCharType = charType;
			this.dialogueState = DialogueState.Loading;
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(InitThreadDialogue)));
			return true;
		}

		private void InitThreadDialogue()
		{
			this.dialogue = new Dialogue(HUD.sprite, HUD.nullTex, HUD.hudTex, this.newConversation, this.newCharType);
			this.dialogueState = DialogueState.Active;
		}

		public void DialogueNull(int resetToEvent, int resetToSideEvent)
		{
			this.dialogueState = DialogueState.Inactive;
			this.dialogue = null;
			if (resetToEvent > -1)
			{
				Game1.events.InitEvent(resetToEvent, isSideEvent: false);
			}
			else if (resetToSideEvent > -1)
			{
				Game1.events.InitEvent(resetToSideEvent, isSideEvent: true, forceSideEvent: true);
			}
			else
			{
				Game1.questManager.UpdateQuests(1000);
			}
		}

		public void InitShopKeeper(int shopID)
		{
			CharacterType characterType;
			switch (shopID)
			{
				case 100:
					characterType = CharacterType.Haley;
					break;
				case 101:
					characterType = CharacterType.Architect;
					break;
				default:
					if (shopID != 110)
					{
						characterType = CharacterType.ShopWild;
					}
					else
					{
						characterType = CharacterType.ShopAurora;
					}
					break;
			}
			Game1.questManager.UpdateQuests(0);
			this.InitDialogue(Game1.stats.villagerDialogue[(int)characterType], characterType);
			Game1.events.FaceLoc(CharacterType.Dust, this.shopPos, faceTowards: true);
			Game1.camera.camOffset.X = 100 * ((this.character[0].Face != CharDir.Right) ? 1 : (-1));
		}

		public void InitShop(int shopID, int selectedCategory, int selectedItem, bool blueprint)
		{
			Game1.GetInventoryContent().Unload();
			Game1.GetLargeContent().Unload();
			this.shop = new Shop(HUD.sprite, HUD.particlesTex, HUD.nullTex, HUD.hudTex, HUD.numbersTex, this.character, this.map);
			this.shop.InitShop(shopID, selectedCategory, selectedItem, blueprint);
		}

		public void RestockItems()
		{
			if (Game1.stats.villagerDialogue[16] <= 0 && Game1.events.currentEvent < 70)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < Game1.stats.shopMaterial.Length; i++)
			{
				if (Game1.stats.shopMaterial[i] > -1)
				{
					int num = ((Rand.GetRandomInt(0, Math.Max(100 - Game1.stats.luck, 1)) < 50) ? 1 : 0);
					if (num > 0)
					{
						flag = true;
						Game1.stats.shopMaterial[i] = Math.Min(Game1.stats.shopMaterial[i] + Rand.GetRandomInt(1, 5), 9999);
					}
				}
			}
			if (flag)
			{
				this.InitMiniPrompt(MiniPromptType.ShopRestocked, 0, blueprint: false);
			}
		}

		public void ExitShop()
		{
			if (this.shop != null)
			{
				this.shop.UnloadTextures();
			}
			this.shop = null;
			this.shopType = ShopType.None;
			if (this.inventoryState != InventoryState.Equipment)
			{
				return;
			}
			this.PopulateEquippable();
			if (this.equipList.Count > 0)
			{
				for (int i = 0; i < this.equipList.Count; i++)
				{
					if (this.equipList[i].Z == 0f && this.equipList[i].X == (float)Game1.inventoryManager.invSelection)
					{
						HUD.equipSelection2 = i;
					}
				}
				Game1.inventoryManager.PopulateDescription(isShopping: false, (int)this.equipList[HUD.equipSelection2].X);
			}
			this.inventoryScreenOffset.Y = 1f;
		}

		public bool UseItem(int item, ParticleManager pMan, bool autoHeal)
		{
			if (item < 0 || item >= Game1.inventoryManager.invSelMax)
			{
				return false;
			}
			if (Game1.inventoryManager.equipItem[item] == null)
			{
				return false;
			}
			bool flag = Game1.stats.Equipment[item] <= 0;
			if (autoHeal)
			{
				Game1.stats.Equipment[item] = Math.Max(Game1.stats.Equipment[item], (byte)1);
			}
			if (Game1.stats.Equipment[item] > 0)
			{
				Character character = this.character[0];
				bool flag2 = false;
				switch (Game1.inventoryManager.equipItem[item].Flag)
				{
				case 1:
					if (character.StatusEffect == StatusEffects.Poison)
					{
						character.StatusEffect = StatusEffects.Normal;
						flag2 = true;
					}
					break;
				case 2:
					if (character.StatusEffect == StatusEffects.Burning)
					{
						character.StatusEffect = StatusEffects.Normal;
						flag2 = true;
					}
					break;
				case 3:
					if (character.StatusEffect == StatusEffects.Silent)
					{
						character.StatusEffect = StatusEffects.Normal;
						flag2 = true;
					}
					break;
				case 10:
					flag2 = true;
					this.DefenseBonusEffect(pMan);
					Game1.stats.defenseBonusTime += 30f;
					break;
				case 11:
					flag2 = true;
					this.AttackBonusEffect(pMan);
					Game1.stats.attackBonusTime += 15f;
					break;
				}
				if (autoHeal && flag)
				{
					Game1.stats.Equipment[item] = 0;
				}
				int num = (int)Math.Min((float)character.MaxHP * Game1.stats.bonusHealth, 9999f);
				if (character.HP < num || flag2)
				{
					character.HP = (int)MathHelper.Clamp(character.HP + Game1.inventoryManager.equipItem[item].Heal, 0f, num);
					this.HealEffect(pMan);
					Game1.stats.curCharge = Game1.stats.maxCharge;
					if (!autoHeal && !flag)
					{
						Game1.stats.Equipment[item] = (byte)MathHelper.Max(Game1.stats.Equipment[item] - 1, 0f);
					}
					if (Game1.stats.Equipment[item] <= 0)
					{
						Game1.stats.Equipment[item] = 0;
						if (item == Game1.stats.currentItem)
						{
							Game1.stats.currentItem = -1;
						}
					}
					return true;
				}
			}
			return false;
		}

		private bool EquipItem()
		{
			if (Game1.stats.Equipment[Game1.inventoryManager.invSelection] < 1)
			{
				return false;
			}
			bool result = true;
			switch (HUD.equipCategory)
			{
			case 0:
				Game1.stats.currentItem = Game1.inventoryManager.invSelection;
				break;
			case 1:
				Game1.stats.currentPendant = Game1.inventoryManager.invSelection;
				break;
			case 2:
				Game1.stats.currentAugment = Game1.inventoryManager.invSelection;
				break;
			case 3:
				Game1.stats.currentArmor = Game1.inventoryManager.invSelection;
				break;
			case 4:
				if (this.equipSelection == 4)
				{
					if (Game1.stats.currentRingRight != Game1.inventoryManager.invSelection || Game1.stats.Equipment[Game1.inventoryManager.invSelection] > 1)
					{
						Game1.stats.currentRingLeft = Game1.inventoryManager.invSelection;
					}
					else
					{
						result = false;
					}
				}
				else if (this.equipSelection == 6)
				{
					if (Game1.stats.currentRingLeft != Game1.inventoryManager.invSelection || Game1.stats.Equipment[Game1.inventoryManager.invSelection] > 1)
					{
						Game1.stats.currentRingRight = Game1.inventoryManager.invSelection;
					}
					else
					{
						result = false;
					}
				}
				break;
			}
			Game1.inventoryManager.UpdateEquipStats(HUD.equipCategory, updateEquip: true, string.Empty);
			return result;
		}

		public void CheckItemDiff()
		{
			HUD.newAttack = Game1.inventoryManager.UpdateEquipStats(HUD.equipCategory, updateEquip: false, "attack");
			HUD.newDefense = Game1.inventoryManager.UpdateEquipStats(HUD.equipCategory, updateEquip: false, "defense");
			HUD.newFidget = Game1.inventoryManager.UpdateEquipStats(HUD.equipCategory, updateEquip: false, "fidget");
			HUD.newLuck = Game1.inventoryManager.UpdateEquipStats(HUD.equipCategory, updateEquip: false, "luck");
		}

		public bool CheckBluePrint(int selection)
		{
			if (selection >= Game1.inventoryManager.invSelMax && selection < Game1.inventoryManager.invSelMax * 5 && this.equipList.Count > 0 && HUD.equipSelection2 < this.equipList.Count && this.equipList[HUD.equipSelection2].Z == 1f && Game1.stats.EquipBluePrint[selection - Game1.inventoryManager.invSelMax] > 0)
			{
				return true;
			}
			return false;
		}

		public void HealEffect(ParticleManager pMan)
		{
			Sound.PlayCue("item_heal");
			for (int i = 0; i < 20; i++)
			{
				Vector2 loc = new Vector2(this.character[0].Location.X + (float)Rand.GetRandomInt(-60, 60), this.character[0].Location.Y + (float)Rand.GetRandomInt(-250, 0));
				int randomInt = Rand.GetRandomInt(20, 48);
				pMan.AddSparkle(loc, 0.5f, 1f, 0.5f, 1f, 1f, randomInt, 5);
			}
		}

		private void SaveEffect(ParticleManager pMan)
		{
			if (Game1.gameMode != Game1.GameModes.Game)
			{
				return;
			}
			if (Game1.menu.prompt == promptDialogue.None && Game1.stats.playerLifeState == 0)
			{
				Game1.stats.curCharge += 4f;
				int num = (int)Math.Min((float)this.character[0].MaxHP * Game1.stats.bonusHealth, 9999f);
				float num2 = Math.Max((float)num * (1f - (float)(Game1.stats.gameDifficulty + 1) * 0.25f), (float)num * 0.125f);
				if (Game1.GamerServices && Game1.IsTrial)
				{
					num2 = this.character[0].MaxHP;
				}
				bool flag = false;
				if (Game1.stats.gameDifficulty == 0)
				{
					num2 = num;
				}
				if ((float)this.character[0].HP < num2)
				{
					flag = true;
					this.character[0].HP += (int)MathHelper.Clamp((num2 - (float)this.character[0].HP) * 0.1f, 1f, 100f);
				}
				if (flag)
				{
					Sound.PlayPersistCue("save_heal", new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f);
				}
				else
				{
					Sound.StopPersistCue("save_heal");
				}
			}
			if (Rand.GetRandomInt(0, 10) < 8)
			{
				Vector2 loc = new Vector2(this.character[0].Location.X + (float)Rand.GetRandomInt(-160, 160), this.character[0].Location.Y + (float)Rand.GetRandomInt(-450, 0));
				int randomInt = Rand.GetRandomInt(12, 48);
				float randomFloat = Rand.GetRandomFloat(0.5f, 1.25f);
				pMan.AddSparkle(loc, Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.25f, 0.75f), Rand.GetRandomFloat(0.75f, 1f), 0.5f, randomFloat, randomInt, 5);
			}
		}

		public void LevelUpEffect(ParticleManager pMan)
		{
			if (this.map.GetTransVal() > 0f || Game1.events.fadeTimer > 0f || Game1.FrameTime == 0f || this.map.doorStage > 0 || this.map.warpStage > 0 || this.unlockState > 0)
			{
				return;
			}
			if (!this.character[0].AnimName.StartsWith("crouch") || Game1.map.CheckCol(this.character[0].Location - new Vector2(0f, 96f)) == 0)
			{
				this.character[0].SetAnim("evadeair", 1, 0);
				this.character[0].SetJump(1000f, jumped: false);
				this.character[0].Trajectory.X = 0f;
			}
			Vector2 vector = this.character[0].Location + new Vector2(0f, -100f);
			VibrationManager.SetScreenShake(1f);
			VibrationManager.SetBlast(1.5f, vector);
			Sound.PlayCue("levelup_burst");
			for (int i = 0; i < 8; i++)
			{
				pMan.AddVerticleBeam(this.character[0].Location - new Vector2(Rand.GetRandomInt(-50, 50), 200f), Rand.GetRandomVector2(-100f, 100f, -10f, 10f), Rand.GetRandomFloat(0f, 1f), 1f, 1f, 1f, Rand.GetRandomInt(50, 150), 2000, Rand.GetRandomFloat(0.2f, 1f), 0, 5);
			}
			for (int j = 0; j < 6; j++)
			{
				pMan.AddVerticleBeam(this.character[0].Location - new Vector2(0f, 200f), Rand.GetRandomVector2(-500f, 500f, -10f, 10f), Rand.GetRandomFloat(0f, 1f), 1f, 1f, 0.1f, Rand.GetRandomInt(50, 150), 2000, Rand.GetRandomFloat(0.2f, 1f), 0, 5);
			}
			pMan.AddShockRing(vector, 0.6f, 5);
			for (int k = 0; k < 40; k++)
			{
				pMan.AddBounceSpark(vector + Rand.GetRandomVector2(-40f, 40f, -40f, 40f), Rand.GetRandomVector2(-500f, 500f, -1000f, 10f), 0.4f, 5);
			}
			for (int l = 0; l < 20; l++)
			{
				pMan.AddSparkle(vector + Rand.GetRandomVector2(-80f, 80f, -400f, 300f), 0.2f, 0.6f, 1f, 1f, 1f, Rand.GetRandomInt(20, 48), 5);
			}
			for (int m = 1; m < this.character.Length; m++)
			{
				if (this.character[m].Exists != CharExists.Exists || this.character[m].Team != Team.Enemy || this.character[m].LiftType >= CanLiftType.Immovable || !((this.character[m].Location - this.character[0].Location).Length() < 2000f))
				{
					continue;
				}
				if (this.character[m].Defense != 0)
				{
					this.character[m].SetAnim("godown", 0, 0);
					if (this.character[m].AnimName == "godown")
					{
						this.character[m].DownTime = this.character[m].MaxDownTime;
					}
				}
				else
				{
					this.character[m].SetJump(Rand.GetRandomInt(800, 2000), jumped: false);
					this.character[m].Slide(Rand.GetRandomInt(-2000, -800));
					HitManager.SetHurt(this.character[0].Location.X, this.character, m, up: true);
				}
			}
		}

		private void AttackBonusEffect(ParticleManager pMan)
		{
			for (int i = 0; i < 8; i++)
			{
				Vector2 loc = new Vector2(this.character[0].Location.X + (float)Rand.GetRandomInt(-60, 60), this.character[0].Location.Y + (float)Rand.GetRandomInt(-250, 0));
				int randomInt = Rand.GetRandomInt(20, 48);
				pMan.AddSparkle(loc, 1f, 0.5f, 0.5f, 1f, 1f, randomInt, 5);
			}
		}

		private void DefenseBonusEffect(ParticleManager pMan)
		{
			for (int i = 0; i < 8; i++)
			{
				Vector2 loc = new Vector2(this.character[0].Location.X + (float)Rand.GetRandomInt(-60, 60), this.character[0].Location.Y + (float)Rand.GetRandomInt(-250, 0));
				int randomInt = Rand.GetRandomInt(20, 48);
				pMan.AddSparkle(loc, 0.2f, 0.6f, 1f, 1f, 1f, randomInt, 5);
			}
		}

		public void ExitInventory()
		{
			HUD.loadingHud = false;
			this.inventoryState = InventoryState.None;
			this.inventoryScreenOffset = new Vector2(0f, 1f);
			HUD.inventoryTransRight = (HUD.inventoryTransLeft = false);
			HUD.questPage = QuestPage.Active;
			this.isPaused = false;
			HUD.questScroll = (HUD.questDescScroll = 0f);
			this.equipList.Clear();
			this.materialList.Clear();
			this.invSubStage = 0;
			HUD.equipFade = (HUD.equipFade2 = (HUD.itemAlpha = 0f));
			HUD.curUpgrade = 0;
			this.KeyCancel = (this.OpenMenuButton = false);
			this.canInput = false;
			Sound.PlayCue("menu_cancel");
			Game1.inventoryManager.UpdateEquipStats(HUD.equipCategory, updateEquip: true, string.Empty);
			Game1.GetInventoryContent().Unload();
			Game1.GetLargeContent().Unload();
			GC.Collect();
		}

		public bool LeavePage(InventoryState state)
		{
			switch (this.inventoryState)
			{
			case InventoryState.Character:
			{
				int num = 0;
				for (int i = 0; i < HUD.upgradeCategory.Length; i++)
				{
					num += HUD.upgradeCategory[i];
				}
				if (num + Game1.stats.skillPoints <= 0)
				{
					break;
				}
				if (Game1.stats.skillPoints == 0)
				{
					Game1.menu.prompt = promptDialogue.UpgradeComplete;
				}
				else
				{
					if (this.KeyCancel || this.OpenMenuButton)
					{
						HUD.invResponse = 0;
					}
					else if (this.KeyLeftBumper || Game1.pcManager.KeyInvLeft)
					{
						HUD.invResponse = 1;
					}
					else if (this.KeyRightBumper || Game1.pcManager.KeyInvRight || Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
					{
						HUD.invResponse = 2;
					}
					Game1.menu.prompt = promptDialogue.UpgradeIncomplete;
				}
				HUD.itemAlpha = 0f;
				Game1.menu.ClearPrompt();
				this.invSubStage = 1;
				return false;
			}
			case InventoryState.Map:
				Game1.navManager.SetPlayerCell();
				break;
			}
			return true;
		}

		public void SetInventoryTextures()
		{
			HUD.loadingHud = true;
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadInventoryTextures), new TaskFinishedDelegate(LoadingInventoryFinished)));
		}

		public void LoadInventoryTextures()
		{
			if (this.inventoryState != 0)
			{
				HUD.inventoryTex = Game1.GetInventoryContent().Load<Texture2D>("gfx/ui/hud_elements_" + (int)this.inventoryState);
			}
		}

		public void LoadingInventoryFinished(int taskId)
		{
			HUD.loadingHud = false;
		}

		private void DrawTriggers(float posInvY)
		{
			HUD.sprite.Draw(HUD.hudTex[0], new Vector2(Game1.screenWidth / 2 - 160, (float)(Game1.screenHeight / 2 - 260) - posInvY / 8f), new Rectangle(150, 140, 50, 45), Color.White, 0f, new Vector2(25f, 20f), 0.8f, SpriteEffects.None, 0f);
			HUD.sprite.Draw(HUD.hudTex[0], new Vector2(Game1.screenWidth / 2 + 160, (float)(Game1.screenHeight / 2 - 260) - posInvY / 8f), new Rectangle(0, 140, 50, 45), Color.White, 0f, new Vector2(25f, 20f), 0.8f, SpriteEffects.None, 0f);
		}

		private bool CanDrawItemBorder(int i)
		{
			switch (HUD.equipCategory)
			{
			case 0:
				if (i == Game1.stats.currentItem)
				{
					return true;
				}
				break;
			case 1:
				if (i == Game1.stats.currentPendant)
				{
					return true;
				}
				break;
			case 2:
				if (i == Game1.stats.currentAugment)
				{
					return true;
				}
				break;
			case 3:
				if (i == Game1.stats.currentArmor)
				{
					return true;
				}
				break;
			case 4:
				if (this.equipSelection == 4 && i == Game1.stats.currentRingLeft)
				{
					return true;
				}
				if (this.equipSelection == 6 && i == Game1.stats.currentRingRight)
				{
					return true;
				}
				break;
			}
			return false;
		}

		private void PopulateCharacter()
		{
			float size = 0.7f;
			int num = 0;
			for (int i = 0; i < HUD.upgradeCategory.Length; i++)
			{
				num += HUD.upgradeCategory[i];
			}
			string text;
			if (Game1.stats.skillPoints == 0 && num > 0)
			{
				text = ((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse) ? Strings_PC.UpgradeComplete : Strings_HudInv.UpgradeComplete);
			}
			else
			{
				text = Strings_HudInv.ResourceManager.GetString("CharStat" + HUD.curUpgrade + "Desc");
				if (HUD.curUpgrade == 0 && Game1.stats.bonusHealth - 1f > 0f)
				{
					object obj = text;
					text = string.Concat(obj, "\n+", (int)Math.Round((Game1.stats.bonusHealth - 1f) * 100f), "% ", Strings_HudInv.CharStat0DescStat);
				}
			}
			Game1.menu.optionDesc = Game1.smallText.WordWrap(text, size, 360f, Game1.menu.optionDescButtonList, TextAlign.Left);
			bool flag = false;
			for (int j = 0; j < HUD.upgradeCategory.Length; j++)
			{
				if (HUD.upgradeCategory[j] > 0)
				{
					flag = true;
				}
			}
			if (Game1.stats.skillPoints > 0 || flag)
			{
				Game1.inventoryManager.itemControls = Game1.smallText.WordWrap(Strings_HudInv.CharControls, size, 410f, Game1.inventoryManager.itemControlsButtonList, TextAlign.Left);
			}
			else
			{
				Game1.inventoryManager.itemControls = Game1.smallText.WordWrap(Strings_HudInv.Exit, size, 410f, Game1.inventoryManager.itemControlsButtonList, TextAlign.Left);
			}
		}

		private void PopulateEquipped()
		{
			bool flag = false;
			int selection;
			switch (this.equipSelection)
			{
			default:
				selection = Game1.stats.currentItem;
				break;
			case 1:
				selection = Game1.stats.currentArmor;
				break;
			case 2:
				selection = Game1.stats.currentAugment;
				break;
			case 3:
				selection = 300;
				flag = true;
				break;
			case 4:
				selection = Game1.stats.currentRingLeft;
				break;
			case 5:
				selection = Game1.stats.currentPendant;
				break;
			case 6:
				selection = Game1.stats.currentRingRight;
				break;
			}
			Game1.inventoryManager.PopulateDescription(isShopping: false, selection);
			if (flag)
			{
				Game1.inventoryManager.itemCost = 0;
				Game1.inventoryManager.itemName = Strings_HudInv.EquipCategory5;
				Game1.inventoryManager.itemInfo = Game1.smallText.WordWrap(Strings_HudInv.EquipCategory5Desc, 0.7f, 455f, TextAlign.Left);
			}
		}

		private void PopulateEquippable()
		{
			this.equipList.Clear();
			for (int i = HUD.equipCategory * Game1.inventoryManager.invSelMax; i < (HUD.equipCategory + 1) * Game1.inventoryManager.invSelMax; i++)
			{
				if (Game1.stats.Equipment[i] > 0)
				{
					this.equipList.Add(new Vector3(i, (int)Game1.stats.Equipment[i], 0f));
				}
			}
			for (int j = HUD.equipCategory * Game1.inventoryManager.invSelMax; j < (HUD.equipCategory + 1) * Game1.inventoryManager.invSelMax; j++)
			{
				if (HUD.equipCategory > 0 && HUD.equipCategory < 5 && Game1.stats.EquipBluePrint[j - Game1.inventoryManager.invSelMax] > 0)
				{
					this.equipList.Add(new Vector3(j, (int)Game1.stats.EquipBluePrint[j - Game1.inventoryManager.invSelMax], 1f));
				}
			}
		}

		private void PopulateMaterial()
		{
			if (this.equipSelection < this.materialList.Count && Game1.inventoryManager.material[(int)this.materialList[this.equipSelection].X] != null)
			{
				Game1.inventoryManager.itemName = Game1.inventoryManager.material[(int)this.materialList[this.equipSelection].X].name;
				Game1.inventoryManager.itemInfo = Game1.smallText.WordWrap(Game1.inventoryManager.material[(int)this.materialList[this.equipSelection].X].getDescription, 0.7f, 300f, TextAlign.Left);
				Game1.inventoryManager.itemCost = Game1.inventoryManager.material[(int)this.materialList[this.equipSelection].X].value;
				if (Game1.stats.shopMaterial[(int)this.materialList[this.equipSelection].X] > -1)
				{
					InventoryManager inventoryManager = Game1.inventoryManager;
					inventoryManager.itemStats = inventoryManager.itemStats + "\n" + Strings_HudInv.MaterialCatalogued;
				}
			}
			else
			{
				Game1.inventoryManager.itemName = (Game1.inventoryManager.itemInfo = (Game1.inventoryManager.itemStats = "null"));
			}
		}

		private void ReadyPage(InventoryState setState)
		{
			VibrationManager.StopRumble();
			float size = 0.7f;
			this.inventoryState = setState;
			switch (this.inventoryState)
			{
			case InventoryState.Character:
				this.PopulateCharacter();
				this.cursorPos = new Vector2(380f, 120f);
				break;
			case InventoryState.Equipment:
				HUD.equipCategory = 0;
				this.PopulateEquipped();
				this.cursorPos = new Vector2(270f, 255f);
				break;
			case InventoryState.Quests:
				HUD.questScroll = (HUD.questDescScroll = (this.questScrollBar = 0f));
				HUD.equipCategory = 0;
				HUD.questPage = QuestPage.Active;
				Game1.inventoryManager.itemControls = Game1.smallText.WordWrap(Strings_HudInv.QuestControls, size, 930f, Game1.inventoryManager.itemControlsButtonList, TextAlign.Left);
				break;
			case InventoryState.Map:
				Game1.navManager.scrollX = Game1.navManager.playerX;
				Game1.navManager.scrollY = Game1.navManager.playerY;
				Game1.navManager.ForceScroll(0.75f, Game1.navManager.playerX, Game1.navManager.playerY);
				Game1.inventoryManager.itemControls = Game1.smallText.WordWrap((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse) ? Strings_PC.Legend : Strings_HudInv.MapControls, size, 930f, Game1.inventoryManager.itemControlsButtonList, TextAlign.Left);
				break;
			case InventoryState.Materials:
			{
				this.materialList.Clear();
				for (int i = 0; i < Game1.stats.Material.Length; i++)
				{
					if (Game1.stats.Material[i] > -1)
					{
						this.materialList.Add(new Vector3(i, Game1.stats.Material[i], 0f));
					}
				}
				this.PopulateMaterial();
				Game1.inventoryManager.itemControls = Game1.smallText.WordWrap(Strings_HudInv.Exit, size, 930f, Game1.inventoryManager.itemControlsButtonList, TextAlign.Left);
				break;
			}
			}
			HUD.curUpgrade = 0;
			this.equipSelection = (HUD.equipSelection2 = (HUD.equipSelection3 = 0));
			HUD.equipFade = (HUD.equipFade2 = 0f);
			this.invSubStage = 0;
			HUD.invResponse = -1;
			this.SetInventoryTextures();
			int num = (int)(this.inventoryState - 4);
			this.InitHelp("menuHelp" + num, restart: true, num + 10);
		}

		public void ForceInventory(InventoryState setState)
		{
			if (this.inventoryState != 0)
			{
				this.ExitInventory();
			}
			else
			{
				this.ReadyPage(InventoryState.Character);
			}
			if (this.miniPromptState > 0)
			{
				this.miniPromptState = 3;
				HUD.miniPromptTime = 0.1f;
			}
			HUD.inventoryTitlePos = Game1.screenWidth / 2 - 150 + 1200 - 300 * (int)this.inventoryState;
			Sound.PlayCue("menu_init");
		}

		private void AddGem(ParticleManager pMan, int maxRange, int minStat)
		{
			if (Game1.stats.skillPoints > 0)
			{
				if (Game1.stats.upgradedStat[HUD.curUpgrade] + HUD.upgradeCategory[HUD.curUpgrade] < Math.Min(15, minStat + maxRange))
				{
					Sound.PlayCue("skill_gem_add");
					VibrationManager.Rumble(Game1.currentGamePad, 0.4f);
					HUD.upgradeCategory[HUD.curUpgrade]++;
					Game1.stats.skillPoints--;
					this.PopulateCharacter();
					if (Game1.stats.skillPoints == 0)
					{
						GC.Collect();
					}
					for (int i = 0; i < 10; i++)
					{
						Vector2 loc = this.cursorPos + new Vector2(0f, 11f);
						float randomFloat = Rand.GetRandomFloat(0f, 6.28f);
						Vector2 randomVector = Rand.GetRandomVector2(-30f, 79f, -60f, 60f);
						pMan.AddHudSpark(this.cursorPos + randomVector, randomVector, 0.2f, 9);
						pMan.AddHudDust(loc, new Vector2(0f, Rand.GetRandomFloat(10f, 30f)), 0.6f, randomFloat, new Color(0f, 1f, 1f, 1f), 0.4f, 0, 9);
					}
				}
				else
				{
					Sound.PlayCue("shop_fail");
				}
			}
			else
			{
				int num = 0;
				for (int j = 0; j < HUD.upgradeCategory.Length; j++)
				{
					num += HUD.upgradeCategory[j];
				}
				if (num + Game1.stats.skillPoints > 0)
				{
					Sound.PlayCue("menu_confirm");
					HUD.itemAlpha = 0f;
					Game1.menu.prompt = promptDialogue.UpgradeComplete;
					Game1.menu.ClearPrompt();
					this.invSubStage = 1;
				}
			}
		}

		private void RemoveGem(int maxRange, int maxStat)
		{
			if (HUD.upgradeCategory[HUD.curUpgrade] > Math.Max(maxStat - maxRange - Game1.stats.upgradedStat[HUD.curUpgrade], 0))
			{
				Sound.PlayCue("skill_gem_remove");
				HUD.upgradeCategory[HUD.curUpgrade]--;
				Game1.stats.skillPoints++;
			}
			else if (Game1.stats.skillPoints > 0)
			{
				Sound.PlayCue("shop_fail");
			}
			this.PopulateCharacter();
		}

		private void ConfirmGems()
		{
			Game1.stats.UpgradeStats(HUD.upgradeCategory);
			for (int i = 0; i < HUD.upgradeCategory.Length; i++)
			{
				HUD.upgradeCategory[i] = 0;
			}
			HUD.itemAlpha = 0f;
			this.invSubStage = 0;
			Sound.PlayCue("skill_gem_complete");
			VibrationManager.Rumble(Game1.currentGamePad, 0.6f);
			this.PopulateCharacter();
			Game1.awardsManager.EarnAchievement(Achievement.LevelUp, forceCheck: false);
		}

		private void UpdateInventory(ParticleManager pMan)
		{
			if ((this.isPaused && this.inventoryState == InventoryState.None) || Game1.stats.playerLifeState != 0 || Game1.gameMode != Game1.GameModes.Game || !this.noEvent || this.unlockState > 0 || this.shop != null)
			{
				return;
			}
			if (this.inventoryState == InventoryState.None && Game1.menu.prompt == promptDialogue.None)
			{
				if (this.KeyLeftBumper && Game1.events.currentEvent > -1 && Game1.events.currentEvent < 760)
				{
					this.KeyLeftBumper = false;
					this.UseItem(Game1.stats.currentItem, pMan, autoHeal: false);
				}
				if (this.KeyRightBumper && Game1.events.currentEvent > -1 && Game1.events.currentEvent < 760)
				{
					this.KeyRightBumper = false;
					int projectileType = Game1.stats.projectileType;
					for (int i = Game1.stats.projectileType + 1; i < Game1.stats.projectileType + 10; i++)
					{
						int num = i;
						if (num > 9)
						{
							num -= 10;
						}
						if (num != 3 && num != 4 && num != 5)
						{
							if (Game1.stats.upgrade[3] > 0 && num == 0)
							{
								Game1.stats.projectileType = num;
								break;
							}
							if (Game1.stats.upgrade[num] > 0 && num != 0)
							{
								Game1.stats.projectileType = num;
								break;
							}
						}
					}
					if (projectileType != Game1.stats.projectileType)
					{
						HUD.changeProjectileTime = 4f;
						Sound.PlayCue("fidget_proj_switch");
						this.map.projectileCount = 0;
						Game1.stats.projectileCost = Game1.stats.GetProjectileCost(Game1.stats.projectileType);
					}
				}
			}
			else if (!HUD.loadingHud)
			{
				Game1.BlurScene(1f);
				if (this.inventoryScreenOffset.Y == 0f && HUD.helpContinue == -1)
				{
					switch (this.inventoryState)
					{
					case InventoryState.Character:
						this.UpdateCharacter(pMan);
						break;
					case InventoryState.Equipment:
						this.UpdateEquipment(pMan);
						break;
					case InventoryState.Map:
						this.UpdateMap();
						break;
					case InventoryState.Quests:
						this.UpdateQuest();
						break;
					case InventoryState.Materials:
						this.UpdateMaterials();
						break;
					case InventoryState.Stats:
						this.UpdateStatsPage();
						break;
					}
					if ((this.invSubStage == 0 || this.inventoryState != InventoryState.Character) && !HUD.inventoryTransLeft && !HUD.inventoryTransRight)
					{
						if ((Game1.pcManager.inputDevice == InputDevice.GamePad && this.KeyLeftBumper) || Game1.pcManager.KeyInvLeft)
						{
							this.KeyLeftBumper = (Game1.pcManager.KeyInvLeft = false);
							Sound.PlayCue("menu_page_turn");
							HUD.inventoryTransLeft = true;
							HUD.itemAlpha = (this.invSubStage = 0);
						}
						else if ((Game1.pcManager.inputDevice == InputDevice.GamePad && this.KeyRightBumper) || Game1.pcManager.KeyInvRight)
						{
							this.KeyRightBumper = (Game1.pcManager.KeyInvRight = false);
							Sound.PlayCue("menu_page_turn");
							HUD.inventoryTransRight = true;
							HUD.itemAlpha = (this.invSubStage = 0);
						}
					}
				}
				else
				{
					this.inventoryScreenOffset.Y = MathHelper.Clamp(this.inventoryScreenOffset.Y - Game1.HudTime * 4f, 0f, 1f);
				}
				if (HUD.itemAlpha < 1f)
				{
					HUD.itemAlpha += Game1.HudTime * 4f;
					if (HUD.itemAlpha > 1f)
					{
						HUD.itemAlpha = 1f;
					}
				}
				if (this.inventoryState != 0 || Game1.menu.prompt != promptDialogue.None || this.isPaused || (this.shopType != 0 && this.dialogueState == DialogueState.Inactive))
				{
					this.screenDimAlpha += Game1.HudTime * 4f;
					if (this.screenDimAlpha > 0.75f)
					{
						this.screenDimAlpha = 0.75f;
					}
				}
				else
				{
					this.screenDimAlpha = 0f;
				}
				if (this.inventoryState != 0)
				{
					if (HUD.inventoryTransLeft)
					{
						if (this.inventoryScreenOffset.X < 0f)
						{
							this.inventoryScreenOffset.X += Game1.HudTime * 10f;
							if (this.inventoryScreenOffset.X > 0f)
							{
								this.inventoryScreenOffset.X = 0f;
								HUD.inventoryTransLeft = false;
							}
						}
						else
						{
							this.inventoryScreenOffset.X += Game1.HudTime * 10f;
							if (this.inventoryScreenOffset.X > 1f)
							{
								this.inventoryScreenOffset.X = -1f;
								this.inventoryState--;
								if (this.inventoryState < InventoryState.Character)
								{
									this.inventoryState = InventoryState.Stats;
								}
								this.ReadyPage(this.inventoryState);
							}
						}
					}
					else if (HUD.inventoryTransRight)
					{
						if (this.inventoryScreenOffset.X > 0f)
						{
							this.inventoryScreenOffset.X -= Game1.HudTime * 10f;
							if (this.inventoryScreenOffset.X < 0f)
							{
								this.inventoryScreenOffset.X = 0f;
								HUD.inventoryTransRight = false;
							}
						}
						else
						{
							this.inventoryScreenOffset.X -= Game1.HudTime * 10f;
							if (this.inventoryScreenOffset.X < -1f)
							{
								this.inventoryScreenOffset.X = 1f;
								this.inventoryState++;
								if (this.inventoryState > InventoryState.Stats)
								{
									this.inventoryState = InventoryState.Character;
								}
								this.ReadyPage(this.inventoryState);
							}
						}
					}
				}
				if (HUD.helpContinue > -1)
				{
					if (this.KeySelect)
					{
						this.helpState = 3;
						HUD.helpTime = 0.2f;
					}
					return;
				}
			}
			if (this.KeyThumb_Right && this.inventoryState == InventoryState.None && Game1.menu.prompt == promptDialogue.None && !this.debugDisplay && Game1.events.currentEvent > -1 && Game1.events.currentEvent < 760)
			{
				this.KeyThumb_Left = (this.KeyThumb_Right = false);
				this.InitQuests();
				Game1.stats.GetWorldExplored();
				this.ReadyPage(InventoryState.Map);
				HUD.inventoryTitlePos = Game1.screenWidth / 2 - 150 + 1200 - 300 * (int)this.inventoryState;
				Sound.PlayCue("menu_init");
			}
			if (!this.OpenMenuButton || Game1.menu.prompt != promptDialogue.None || Game1.events.currentEvent <= -1 || Game1.events.currentEvent >= 760)
			{
				return;
			}
			this.OpenMenuButton = false;
			if (this.inventoryState != 0)
			{
				this.ExitInventory();
				return;
			}
			this.InitQuests();
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(Game1.stats.GetWorldExplored)));
			int num2 = -1;
			if (this.miniPromptList.Count > 0)
			{
				num2 = this.miniPromptList[0].ID;
			}
			if (num2 < 1000 || num2 >= 20000)
			{
				this.equipSelection = 0;
				this.ReadyPage(InventoryState.Equipment);
				if (num2 > -1 && num2 < 20000)
				{
					this.invSubStage = 1;
					HUD.equipCategory = num2 / (int)Game1.inventoryManager.invSelMax;
					switch (HUD.equipCategory)
					{
					case 0:
						this.equipSelection = 0;
						break;
					case 3:
						this.equipSelection = 1;
						break;
					case 2:
						this.equipSelection = 2;
						break;
					case 5:
						this.equipSelection = 3;
						break;
					case 4:
						this.equipSelection = 4;
						break;
					case 1:
						this.equipSelection = 5;
						break;
					}
					this.PopulateEquippable();
					for (int j = 0; j < this.equipList.Count; j++)
					{
						if (((this.miniPromptList[0].rawText.Contains(Strings_HudInv.Blueprint) && this.equipList[j].Z == 1f) || this.equipList[j].Z == 0f) && this.equipList[j].X == (float)num2)
						{
							HUD.equipSelection2 = j;
						}
					}
					Game1.inventoryManager.invSelection = num2;
					Game1.inventoryManager.PopulateDescription(isShopping: false, Game1.inventoryManager.invSelection);
					if (this.equipList.Count > 0 && this.equipList[HUD.equipSelection2].Z == 1f)
					{
						InventoryManager inventoryManager = Game1.inventoryManager;
						inventoryManager.itemStats = inventoryManager.itemStats + "\n(" + Strings_HudInv.Blueprint + ")";
					}
				}
			}
			else if (num2 < 2000)
			{
				this.equipSelection = 0;
				this.ReadyPage(InventoryState.Materials);
				for (int k = 0; k < this.materialList.Count; k++)
				{
					if (this.materialList[k].X == (float)(num2 - 1000))
					{
						this.equipSelection = k;
						this.PopulateMaterial();
					}
				}
			}
			else if (num2 < 10000)
			{
				this.ReadyPage(InventoryState.Quests);
				num2 -= 2000;
				int num3 = num2 / 2000;
				num2 -= 2000 * num3;
				switch (num3)
				{
				case 0:
				{
					HUD.questPage = QuestPage.Active;
					for (int m = 0; m < Game1.questManager.activeQuest.Count; m++)
					{
						if (Game1.questManager.activeQuest[m].QuestID == num2)
						{
							this.equipSelection = Game1.questManager.activeQuest.Count - m - 1;
						}
					}
					break;
				}
				case 1:
				{
					HUD.questPage = QuestPage.Completed;
					for (int n = 0; n < Game1.questManager.completedQuest.Count; n++)
					{
						if (Game1.questManager.completedQuest[n].QuestID == num2)
						{
							this.equipSelection = Game1.questManager.completedQuest.Count - n - 1;
						}
					}
					break;
				}
				case 2:
				{
					HUD.questPage = QuestPage.Notes;
					for (int l = 0; l < Game1.questManager.notes.Count; l++)
					{
						if (Game1.questManager.notes[l].NoteID == num2)
						{
							this.equipSelection = Game1.questManager.notes.Count - l - 1;
						}
					}
					break;
				}
				}
			}
			else
			{
				this.ReadyPage(InventoryState.Character);
			}
			if (this.miniPromptState > 0)
			{
				this.miniPromptState = 3;
				HUD.miniPromptTime = 0.1f;
			}
			HUD.inventoryTitlePos = Game1.screenWidth / 2 - 150 + 1200 - 300 * (int)this.inventoryState;
			Sound.PlayCue("menu_init");
		}

		public void UpdateCharacter(ParticleManager pMan)
		{
			if (this.invSubStage == 0)
			{
				int maxRange = 4;
				int num = 16;
				int num2 = 0;
				for (int i = 0; i < Game1.stats.upgradedStat.Length; i++)
				{
					num2 = Math.Max(Game1.stats.upgradedStat[i] + HUD.upgradeCategory[i], num2);
					num = Math.Min(Game1.stats.upgradedStat[i] + HUD.upgradeCategory[i], num);
				}
				if (this.KeyUp)
				{
					Sound.PlayCue("menu_click");
					HUD.itemAlpha = 0f;
					HUD.curUpgrade--;
					if (HUD.curUpgrade < 0)
					{
						HUD.curUpgrade = 3;
					}
					this.PopulateCharacter();
				}
				if (this.KeyDown)
				{
					Sound.PlayCue("menu_click");
					HUD.itemAlpha = 0f;
					HUD.curUpgrade++;
					if (HUD.curUpgrade > 3)
					{
						HUD.curUpgrade = 0;
					}
					this.PopulateCharacter();
				}
				if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse && this.KeySelect)
				{
					this.KeySelect = false;
					this.AddGem(pMan, maxRange, num);
				}
				if (this.KeyCancel || this.KeyLeftBumper || this.KeyRightBumper || this.OpenMenuButton || (Game1.pcManager.inputDevice != 0 && (Game1.pcManager.KeyInvLeft || Game1.pcManager.KeyInvRight)))
				{
					if (!this.LeavePage(this.inventoryState))
					{
						this.KeyCancel = (this.KeyLeftBumper = (this.KeyRightBumper = (this.OpenMenuButton = (Game1.pcManager.KeyInvLeft = (Game1.pcManager.KeyInvRight = false)))));
					}
					else if (this.KeyCancel)
					{
						this.ExitInventory();
					}
				}
				if (this.KeyX)
				{
					this.RemoveGem(maxRange, num2);
				}
			}
			int num3 = Game1.menu.UpdatePrompt();
			if (this.invSubStage != 1)
			{
				return;
			}
			switch (num3)
			{
			case 0:
				this.invSubStage = 0;
				this.PopulateCharacter();
				break;
			case 1:
				this.ConfirmGems();
				break;
			case 2:
			{
				this.invSubStage = 0;
				if (HUD.invResponse == 0)
				{
					this.ExitInventory();
				}
				else if (HUD.invResponse == 1)
				{
					HUD.inventoryTransLeft = true;
				}
				else if (HUD.invResponse == 2)
				{
					HUD.inventoryTransRight = true;
				}
				for (int j = 0; j < HUD.upgradeCategory.Length; j++)
				{
					Game1.stats.skillPoints += HUD.upgradeCategory[j];
					HUD.upgradeCategory[j] = 0;
				}
				break;
			}
			}
		}

		private void UpdateEquipment(ParticleManager pMan)
		{
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				Vector2 vector = new Vector2((float)(Game1.screenWidth / 2 - 465) + this.inventoryScreenOffset.X * 50f, Game1.screenHeight / 2 - 250);
				vector.Y += 30f;
				if (this.invSubStage == 0)
				{
					Vector2 vector2 = vector + new Vector2(482f, 100f);
					for (int i = 0; i < Game1.inventoryManager.invCatMax + 1; i++)
					{
						Vector2 vector3 = vector2 + new Vector2(-29 + i * 102 - i / 4 * 408, 12 + i / 4 * 115);
						if (i > 3)
						{
							vector3.X += 51f;
						}
						if (!new Rectangle((int)vector3.X, (int)vector3.Y, 60, 60).Contains((int)this.mousePos.X, (int)this.mousePos.Y))
						{
							continue;
						}
						if (this.equipSelection != i)
						{
							this.equipSelection = i;
							HUD.itemAlpha = 0f;
							Sound.PlayCue("menu_click");
							switch (this.equipSelection)
							{
							default:
								HUD.equipCategory = 0;
								break;
							case 1:
								HUD.equipCategory = 3;
								break;
							case 2:
								HUD.equipCategory = 2;
								break;
							case 3:
								HUD.equipCategory = 5;
								break;
							case 4:
								HUD.equipCategory = 4;
								break;
							case 5:
								HUD.equipCategory = 1;
								break;
							case 6:
								HUD.equipCategory = 4;
								break;
							}
							this.PopulateEquipped();
						}
						if (Game1.pcManager.leftMouseClicked)
						{
							Game1.pcManager.leftMouseClicked = false;
							this.KeySelect = true;
						}
					}
				}
				else if (this.invSubStage == 1)
				{
					Vector2 vector4 = vector + new Vector2(354f, -10f - HUD.equipFade * 20f);
					int num = 5;
					if (HUD.equipFade < 1f)
					{
						Vector2 vector5 = vector4 + new Vector2(110f, 125f);
						vector5.Y -= Math.Max(HUD.equipSelection2 / num - 1, 0) * 78;
						HUD.inventorySelectionPos.Y = vector5.Y;
					}
					else
					{
						if (Game1.pcManager.leftMouseClicked)
						{
							if (new Rectangle((int)vector4.X + 510, (int)vector4.Y + 32, 45, 45).Contains((int)this.mousePos.X, (int)this.mousePos.Y))
							{
								Game1.pcManager.leftMouseClicked = false;
								this.KeyCancel = true;
							}
							else if (new Rectangle((int)vector4.X + 510, (int)vector4.Y + 92, 45, 45).Contains((int)this.mousePos.X, (int)this.mousePos.Y))
							{
								Game1.pcManager.leftMouseClicked = false;
								this.KeyX = true;
							}
						}
						bool flag = false;
						for (int j = 0; j < this.equipList.Count; j++)
						{
							float num2 = HUD.inventorySelectionPos.Y + (float)(j / num * 78);
							if (!(num2 > vector4.Y + 110f) || !(num2 < vector4.Y + 300f))
							{
								continue;
							}
							Vector2 vector6 = HUD.inventorySelectionPos + new Vector2(j * 78 - j / num * num * 78 - 30, j / num * 78 - 30);
							if (!new Rectangle((int)vector6.X, (int)vector6.Y, 60, 60).Contains((int)this.mousePos.X, (int)this.mousePos.Y))
							{
								continue;
							}
							flag = true;
							if (j != HUD.equipSelection2)
							{
								HUD.equipSelection2 = j;
								Sound.PlayCue("menu_click");
								HUD.itemAlpha = 0f;
								if (this.equipList.Count > 0)
								{
									Game1.inventoryManager.invSelection = (int)this.equipList[HUD.equipSelection2].X;
									Game1.inventoryManager.PopulateDescription(isShopping: false, (int)this.equipList[HUD.equipSelection2].X);
									if (this.equipList[HUD.equipSelection2].Z == 1f)
									{
										InventoryManager inventoryManager = Game1.inventoryManager;
										inventoryManager.itemStats = inventoryManager.itemStats + "\n(" + Strings_HudInv.Blueprint + ")";
									}
								}
							}
							if (Game1.pcManager.leftMouseClicked)
							{
								Game1.pcManager.leftMouseClicked = false;
								this.KeySelect = true;
								if (Game1.pcManager.doubleClicked)
								{
									this.KeyY = true;
									Game1.pcManager.ResetDoubleClick();
								}
							}
						}
						if (flag)
						{
							this.mouseHighLighted = Math.Min(this.mouseHighLighted + Game1.HudTime * 8f, 1f);
						}
						else
						{
							this.mouseHighLighted = Math.Max(this.mouseHighLighted - Game1.HudTime * 8f, 0f);
						}
					}
				}
			}
			if (this.invSubStage < 2 && this.KeySelect)
			{
				this.KeySelect = false;
				int num3 = -1;
				if (this.invSubStage == 0)
				{
					this.invSubStage = 1;
					HUD.itemAlpha = 0f;
					HUD.equipSelection2 = 0;
					Sound.PlayCue("menu_confirm");
					this.PopulateEquippable();
					bool flag2 = false;
					switch (this.equipSelection)
					{
					default:
						HUD.equipCategory = 0;
						if (Game1.stats.currentItem > -1)
						{
							num3 = Game1.stats.currentItem;
						}
						else
						{
							flag2 = true;
						}
						break;
					case 1:
						HUD.equipCategory = 3;
						if (Game1.stats.currentArmor > -1)
						{
							num3 = Game1.stats.currentArmor;
						}
						else
						{
							flag2 = true;
						}
						break;
					case 2:
						HUD.equipCategory = 2;
						if (Game1.stats.currentAugment > -1)
						{
							num3 = Game1.stats.currentAugment;
						}
						else
						{
							flag2 = true;
						}
						break;
					case 3:
						HUD.equipCategory = 5;
						flag2 = true;
						break;
					case 4:
						HUD.equipCategory = 4;
						if (Game1.stats.currentRingLeft > -1)
						{
							num3 = Game1.stats.currentRingLeft;
						}
						else
						{
							flag2 = true;
						}
						break;
					case 5:
						HUD.equipCategory = 1;
						if (Game1.stats.currentPendant > -1)
						{
							num3 = Game1.stats.currentPendant;
						}
						else
						{
							flag2 = true;
						}
						break;
					case 6:
						HUD.equipCategory = 4;
						if (Game1.stats.currentRingRight > -1)
						{
							num3 = Game1.stats.currentRingRight;
						}
						else
						{
							flag2 = true;
						}
						break;
					}
					if (flag2)
					{
						num3 = HUD.equipCategory * Game1.inventoryManager.invSelMax;
						if (this.equipList.Count > 0)
						{
							num3 = (int)Math.Max(num3, this.equipList[0].X);
						}
					}
					Game1.inventoryManager.invSelection = num3;
					for (int k = 0; k < this.equipList.Count; k++)
					{
						if (this.equipList[k].Z == 0f && this.equipList[k].X == (float)num3)
						{
							HUD.equipSelection2 = k;
						}
					}
				}
				else if (this.equipList.Count > 0)
				{
					num3 = Game1.inventoryManager.invSelection;
					bool flag3 = false;
					int num4 = this.invSubStage;
					switch (HUD.equipCategory)
					{
					case 0:
						if (Game1.stats.Equipment[(int)this.equipList[HUD.equipSelection2].X] > 0)
						{
							Game1.stats.currentItem = (int)this.equipList[HUD.equipSelection2].X;
							flag3 = true;
						}
						break;
					default:
						if (this.equipList.Count <= 0)
						{
							break;
						}
						if (this.equipList[HUD.equipSelection2].Z == 1f)
						{
							if (this.invSubStage < 2 && Game1.stats.Equipment[320] > 0)
							{
								this.mouseHighLighted = 0f;
								Game1.inventoryManager.invSelection = (int)this.equipList[HUD.equipSelection2].X;
								this.InitShop(100, Game1.inventoryManager.invSelection / (int)Game1.inventoryManager.invSelMax, (int)this.equipList[HUD.equipSelection2].X, this.equipList[HUD.equipSelection2].Z == 1f);
								return;
							}
						}
						else
						{
							flag3 = this.EquipItem();
						}
						break;
					case 5:
					case 6:
						break;
					}
					Game1.inventoryManager.invSelection = num3;
					if (num4 == this.invSubStage)
					{
						if (flag3)
						{
							Sound.PlayCue("equip_assign");
							VibrationManager.Rumble(Game1.currentGamePad, 0.4f);
							for (int l = 0; l < 10; l++)
							{
								Vector2 loc = this.cursorPos + new Vector2(22f, 22f);
								float randomFloat = Rand.GetRandomFloat(0f, 6.28f);
								Vector2 randomVector = Rand.GetRandomVector2(-30f, 100f, -60f, 60f);
								pMan.AddHudSpark(this.cursorPos + randomVector, randomVector, 0.2f, 9);
								pMan.AddHudDust(loc, new Vector2(0f, Rand.GetRandomFloat(10f, 30f)), 0.6f, randomFloat, new Color(0f, 1f, 1f, 1f), 0.4f, 0, 9);
							}
						}
						else
						{
							Sound.PlayCue("shop_fail");
						}
					}
				}
				Game1.inventoryManager.PopulateDescription(isShopping: false, Game1.inventoryManager.invSelection);
				if (this.equipList.Count > 0 && this.equipList[HUD.equipSelection2].Z == 1f)
				{
					InventoryManager inventoryManager2 = Game1.inventoryManager;
					inventoryManager2.itemStats = inventoryManager2.itemStats + "\n(" + Strings_HudInv.Blueprint + ")";
				}
			}
			if (this.invSubStage == 0)
			{
				HUD.equipFade = Math.Max(HUD.equipFade - Game1.HudTime * 10f, 0f);
				bool flag4 = false;
				if (this.KeyLeft)
				{
					this.equipSelection--;
					flag4 = true;
				}
				if (this.KeyRight)
				{
					this.equipSelection++;
					flag4 = true;
				}
				if (this.KeyUp)
				{
					this.equipSelection -= 4;
					flag4 = true;
				}
				if (this.KeyDown)
				{
					this.equipSelection += 4;
					flag4 = true;
				}
				if (flag4)
				{
					if (this.equipSelection < 0)
					{
						this.equipSelection += 7;
					}
					if (this.equipSelection > 6)
					{
						this.equipSelection -= 7;
					}
					Sound.PlayCue("menu_click");
					HUD.itemAlpha = 0f;
					switch (this.equipSelection)
					{
					default:
						HUD.equipCategory = 0;
						break;
					case 1:
						HUD.equipCategory = 3;
						break;
					case 2:
						HUD.equipCategory = 2;
						break;
					case 3:
						HUD.equipCategory = 5;
						break;
					case 4:
						HUD.equipCategory = 4;
						break;
					case 5:
						HUD.equipCategory = 1;
						break;
					case 6:
						HUD.equipCategory = 4;
						break;
					}
					this.PopulateEquipped();
				}
				HUD.inventorySelectionPos.X = Game1.inventoryManager.invSelection;
				if (Game1.inventoryManager.invSelection > 2)
				{
					HUD.inventorySelectionPos.X -= 3f;
				}
				if (this.KeyCancel)
				{
					this.ExitInventory();
				}
			}
			else
			{
				HUD.equipFade = Math.Min(HUD.equipFade + Game1.HudTime * 10f, 1f);
			}
			if (this.invSubStage == 1)
			{
				_ = HUD.equipCategory;
				_ = Game1.inventoryManager.invSelMax;
				int num5 = (int)MathHelper.Clamp(HUD.equipSelection2, 0f, this.equipList.Count - 1);
				int num6 = 5;
				if (this.KeyLeft)
				{
					HUD.equipSelection2--;
				}
				if (this.KeyRight)
				{
					HUD.equipSelection2++;
				}
				if (this.KeyUp)
				{
					HUD.equipSelection2 -= num6;
				}
				if (this.KeyDown)
				{
					HUD.equipSelection2 += num6;
				}
				HUD.equipSelection2 = (int)MathHelper.Clamp(HUD.equipSelection2, 0f, this.equipList.Count - 1);
				if (num5 != HUD.equipSelection2)
				{
					Sound.PlayCue("menu_click");
					HUD.itemAlpha = 0f;
					if (this.equipList.Count > 0)
					{
						Game1.inventoryManager.invSelection = (int)this.equipList[HUD.equipSelection2].X;
						Game1.inventoryManager.PopulateDescription(isShopping: false, (int)this.equipList[HUD.equipSelection2].X);
						if (this.equipList[HUD.equipSelection2].Z == 1f)
						{
							InventoryManager inventoryManager3 = Game1.inventoryManager;
							inventoryManager3.itemStats = inventoryManager3.itemStats + "\n(" + Strings_HudInv.Blueprint + ")";
						}
					}
					else
					{
						Game1.inventoryManager.PopulateDescription(isShopping: false, -1);
					}
				}
				if (this.KeyCancel)
				{
					this.KeyCancel = false;
					this.invSubStage = 0;
					Sound.PlayCue("menu_cancel");
					this.PopulateEquipped();
				}
				if (this.KeyX)
				{
					this.KeyX = false;
					bool flag5 = true;
					switch (HUD.equipCategory)
					{
					case 0:
						if (Game1.stats.currentItem == -1)
						{
							flag5 = false;
						}
						Game1.stats.currentItem = -1;
						break;
					case 1:
						if (Game1.stats.currentPendant == -1)
						{
							flag5 = false;
						}
						Game1.stats.currentPendant = -1;
						break;
					case 2:
						if (Game1.stats.currentAugment == -1)
						{
							flag5 = false;
						}
						Game1.stats.currentAugment = -1;
						break;
					case 3:
						if (Game1.stats.currentArmor == -1)
						{
							flag5 = false;
						}
						Game1.stats.currentArmor = -1;
						break;
					case 4:
						if (this.equipSelection == 4)
						{
							if (Game1.stats.currentRingLeft == -1)
							{
								flag5 = false;
							}
							Game1.stats.currentRingLeft = -1;
						}
						if (this.equipSelection == 6)
						{
							if (Game1.stats.currentRingRight == -1)
							{
								flag5 = false;
							}
							Game1.stats.currentRingRight = -1;
						}
						break;
					default:
						flag5 = false;
						break;
					}
					Game1.inventoryManager.UpdateEquipStats(HUD.equipCategory, updateEquip: true, string.Empty);
					Game1.inventoryManager.PopulateDescription(isShopping: false, Game1.inventoryManager.invSelection);
					if (this.equipList.Count > 0 && HUD.equipSelection2 < this.equipList.Count && this.equipList[HUD.equipSelection2].Z == 1f)
					{
						InventoryManager inventoryManager4 = Game1.inventoryManager;
						inventoryManager4.itemStats = inventoryManager4.itemStats + "\n(" + Strings_HudInv.Blueprint + ")";
					}
					if (flag5)
					{
						Sound.PlayCue("equip_remove");
					}
				}
				if (this.KeyY && this.equipList.Count > 0)
				{
					this.KeyY = false;
					if (HUD.equipCategory == 0)
					{
						this.UseItem(Game1.inventoryManager.invSelection, pMan, autoHeal: false);
						this.PopulateEquippable();
						if (this.equipList.Count > 0)
						{
							while (HUD.equipSelection2 >= this.equipList.Count && HUD.equipSelection2 > 0)
							{
								HUD.equipSelection2--;
							}
							Game1.inventoryManager.invSelection = (int)this.equipList[HUD.equipSelection2].X;
							Game1.inventoryManager.PopulateDescription(isShopping: false, Game1.inventoryManager.invSelection);
						}
					}
					else if (this.equipList[HUD.equipSelection2].Z == 1f && Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
					{
						Sound.PlayCue("menu_confirm");
						this.invSubStage = 2;
						Game1.inventoryManager.PopulateDescription(isShopping: false, Game1.inventoryManager.invSelection);
						if (this.equipList[HUD.equipSelection2].Z == 1f)
						{
							InventoryManager inventoryManager5 = Game1.inventoryManager;
							inventoryManager5.itemStats = inventoryManager5.itemStats + "\n(" + Strings_HudInv.Blueprint + ")";
						}
					}
				}
			}
			if (this.invSubStage == 2)
			{
				HUD.equipFade2 = Math.Min(HUD.equipFade2 + Game1.HudTime * 10f, 1f);
				if (this.KeyCancel || this.KeyY || Game1.inventoryManager.equipItem[Game1.inventoryManager.invSelection] == null)
				{
					this.KeyCancel = (this.KeyY = false);
					this.invSubStage = 1;
					HUD.equipSelection3 = 0;
					Sound.PlayCue("menu_cancel");
					Game1.inventoryManager.PopulateDescription(isShopping: false, Game1.inventoryManager.invSelection);
					if (this.equipList.Count > 0 && this.equipList[HUD.equipSelection2].Z == 1f)
					{
						InventoryManager inventoryManager6 = Game1.inventoryManager;
						inventoryManager6.itemStats = inventoryManager6.itemStats + "\n(" + Strings_HudInv.Blueprint + ")";
					}
					return;
				}
				int num7 = 0;
				for (int m = 0; m < Game1.inventoryManager.equipItem[Game1.inventoryManager.invSelection].MaterialReq.Length; m++)
				{
					if (Game1.inventoryManager.equipItem[Game1.inventoryManager.invSelection].MaterialReq[m] > -1)
					{
						num7++;
					}
				}
				if (this.KeyLeft)
				{
					HUD.equipSelection3--;
					if (HUD.equipSelection3 < 0)
					{
						HUD.equipSelection3 = 0;
					}
					else
					{
						Sound.PlayCue("menu_click");
					}
				}
				if (this.KeyRight)
				{
					HUD.equipSelection3++;
					if (HUD.equipSelection3 >= num7)
					{
						HUD.equipSelection3 = num7 - 1;
					}
					else
					{
						Sound.PlayCue("menu_click");
					}
				}
				if (this.KeySelect && Game1.stats.Equipment[320] > 0)
				{
					this.KeySelect = false;
					this.invSubStage = 1;
					HUD.equipFade2 = 0f;
					Game1.inventoryManager.invSelection = (int)this.equipList[HUD.equipSelection2].X;
					this.InitShop(100, Game1.inventoryManager.invSelection / (int)Game1.inventoryManager.invSelMax, (int)this.equipList[HUD.equipSelection2].X, this.equipList[HUD.equipSelection2].Z == 1f);
				}
			}
			else
			{
				HUD.equipFade2 = Math.Max(HUD.equipFade2 - Game1.HudTime * 10f, 0f);
			}
		}

		private void UpdateMap()
		{
			if (this.KeyY)
			{
				this.equipSelection++;
				if (this.equipSelection > 1)
				{
					this.equipSelection = 0;
					Sound.PlayCue("menu_cancel");
				}
				else
				{
					Sound.PlayCue("menu_confirm");
				}
			}
			if (this.equipSelection == 0)
			{
				HUD.equipFade = MathHelper.Clamp(HUD.equipFade - Game1.HudTime * 6f, 0f, 1f);
			}
			else
			{
				HUD.equipFade = MathHelper.Clamp(HUD.equipFade + Game1.HudTime * 6f, 0f, 1f);
			}
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				Vector2 vector = new Vector2((float)(Game1.screenWidth / 2 - 465) + this.inventoryScreenOffset.X * 50f, Game1.screenHeight / 2 - 250);
				vector.Y += 30f;
				Vector2 vector2 = this.mousePos - new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2 + 30);
				int num = 48;
				vector2 = new Vector2((int)(vector2.X / (float)num), (int)(vector2.Y / (float)num));
				if (new Rectangle((int)vector.X - 50, (int)vector.Y + 60, 1020, 500).Contains((int)this.mousePos.X, (int)this.mousePos.Y))
				{
					if (!Game1.pcManager.IsMouseLeftHeld())
					{
						this.mapScrollPos = new Vector2(vector2.X + (float)Game1.navManager.scrollX, vector2.Y + (float)Game1.navManager.scrollY);
					}
					else
					{
						vector2 += new Vector2((int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Width, (int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Height);
						Game1.navManager.scrollX = (int)MathHelper.Clamp((float)(int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Width - vector2.X + this.mapScrollPos.X, 0f, (int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Width);
						Game1.navManager.scrollY = (int)MathHelper.Clamp((float)(int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Height - vector2.Y + this.mapScrollPos.Y, 0f, (int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Height);
					}
				}
			}
			else if (this.KeyX)
			{
				Sound.PlayCue("menu_cancel");
				Game1.navManager.scrollX = Game1.navManager.playerX;
				Game1.navManager.scrollY = Game1.navManager.playerY;
			}
			if (!(Game1.navManager.NavPath == string.Empty))
			{
				if (this.KeyCancel || this.KeyThumb_Right)
				{
					this.KeyCancel = (this.KeyThumb_Left = (this.KeyThumb_Right = false));
					Game1.navManager.SetPlayerCell();
					this.ExitInventory();
				}
				if (this.KeyLeft)
				{
					Game1.navManager.scrollX = (int)MathHelper.Clamp(Game1.navManager.scrollX - 1, 0f, (int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Width);
				}
				if (this.KeyRight)
				{
					Game1.navManager.scrollX = (int)MathHelper.Clamp(Game1.navManager.scrollX + 1, 0f, (int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Width);
				}
				if (this.KeyUp)
				{
					Game1.navManager.scrollY = (int)MathHelper.Clamp(Game1.navManager.scrollY - 1, 0f, (int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Height);
				}
				if (this.KeyDown)
				{
					Game1.navManager.scrollY = (int)MathHelper.Clamp(Game1.navManager.scrollY + 1, 0f, (int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Height);
				}
			}
		}

		private void UpdateMaterials()
		{
			int num = 5;
			int num2 = this.equipSelection;
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				Vector2 vector = new Vector2((float)(Game1.screenWidth / 2 - 465) + this.inventoryScreenOffset.X * 50f, Game1.screenHeight / 2 - 250);
				vector.Y += 30f;
				int num3 = 78;
				int num4 = 5;
				int num5 = (int)vector.Y + 50;
				int num6 = num5 + num3 * 5 + 65;
				for (int i = 0; i < this.materialList.Count; i++)
				{
					float num7 = HUD.inventorySelectionPos.Y + (float)(i / num4 * num3);
					if (num7 > (float)num5 && num7 < (float)num6)
					{
						Vector2 vector2 = HUD.inventorySelectionPos + new Vector2(i * num3 - i / num4 * num4 * num3 - 30, i / num4 * num3 - 30);
						if (new Rectangle((int)vector2.X, (int)vector2.Y, 64, 64).Contains((int)this.mousePos.X, (int)this.mousePos.Y) && this.equipSelection != i)
						{
							this.equipSelection = (int)MathHelper.Clamp(i, 0f, this.materialList.Count - 1);
							HUD.itemAlpha = 0f;
							Sound.PlayCue("menu_click");
							this.PopulateMaterial();
						}
					}
				}
			}
			if (this.KeyLeft)
			{
				this.equipSelection--;
			}
			if (this.KeyRight)
			{
				this.equipSelection++;
			}
			if (this.KeyUp)
			{
				this.equipSelection -= num;
			}
			if (this.KeyDown)
			{
				this.equipSelection += num;
			}
			this.equipSelection = (int)MathHelper.Clamp(this.equipSelection, 0f, this.materialList.Count - 1);
			if (num2 != this.equipSelection)
			{
				Sound.PlayCue("menu_click");
				HUD.itemAlpha = 0f;
				this.PopulateMaterial();
			}
			if (this.KeyCancel)
			{
				this.ExitInventory();
			}
		}

		private void UpdateStatsPage()
		{
			if (this.KeyCancel)
			{
				this.ExitInventory();
			}
		}

		private void DrawInventory()
		{
			if (this.shop != null)
			{
				return;
			}
			Color color = new Color(1f, 1f, 1f, 1f - Math.Abs(this.inventoryScreenOffset.X + this.inventoryScreenOffset.Y));
			Vector2 vector = new Vector2((float)(Game1.screenWidth / 2 - 465) + this.inventoryScreenOffset.X * 50f, Game1.screenHeight / 2 - 250);
			if (Game1.pcManager.inputDevice != 0)
			{
				vector.Y += 30f;
			}
			float posInvY = (float)Math.Abs(Math.Cos(this.pulse) * 40.0);
			if (Game1.pcManager.inputDevice == InputDevice.GamePad)
			{
				HUD.sprite.Draw(HUD.hudTex[1], new Vector2(Game1.screenWidth / 2, vector.Y - 60f), new Rectangle(887, 0, 234, 200), color, 0f, Vector2.Zero, new Vector2(3f, 1f), SpriteEffects.None, 0f);
				HUD.sprite.Draw(HUD.hudTex[1], new Vector2(Game1.screenWidth / 2, vector.Y - 60f), new Rectangle(887, 0, 234, 200), color, 0f, new Vector2(234f, 0f), new Vector2(3f, 1f), SpriteEffects.FlipHorizontally, 0f);
				this.DrawTriggers(posInvY);
				int num = Game1.screenWidth / 2 - 150 + 1200 - 300 * (int)this.inventoryState;
				HUD.inventoryTitlePos += ((float)num - HUD.inventoryTitlePos) * Game1.HudTime * 20f;
				for (int i = 0; i < 6; i++)
				{
					Game1.bigText.Color = new Color(1f, 1f, 1f, 1f - (float)Math.Abs((int)(this.inventoryState - i - 4)) / 2f);
					Game1.bigText.DrawText(new Vector2(HUD.inventoryTitlePos + (float)(300 * i), vector.Y - 45f), Strings_HudInv.ResourceManager.GetString("InvCat" + i), 1f, 300f, TextAlign.Center);
				}
			}
			else
			{
				Color color2 = new Color(1f, 1f, 1f, 1f - this.inventoryScreenOffset.Y);
				for (int j = 0; j < 6; j++)
				{
					Vector2 vector2 = new Vector2(Game1.screenWidth / 2 - 420 + 260 * j - j / 3 * 260 * 3, vector.Y - 88f + (float)(j / 3 * 40));
					HUD.sprite.Draw(HUD.hudTex[1], vector2, new Rectangle(580, 300, 305, 118), color2);
					if (this.helpState == 0 && new Rectangle((int)vector2.X + 20, (int)vector2.Y, 240, 40).Contains((int)this.mousePos.X, (int)this.mousePos.Y))
					{
						float num2 = (float)Math.Abs(Math.Sin(this.pulse));
						HUD.sprite.Draw(HUD.hudTex[1], vector2 + new Vector2(0f, 4f), new Rectangle(580, 418, 305, 112), new Color(1f, 1f, 1f, (float)(int)color.A / 255f * num2));
						if (Game1.pcManager.leftMouseClicked && this.inventoryState != (InventoryState)(j + 4))
						{
							Game1.pcManager.leftMouseClicked = false;
							this.KeyRightBumper = true;
							if (this.LeavePage(this.inventoryState))
							{
								this.inventoryState = (InventoryState)(j + 3);
								Sound.PlayCue("menu_page_turn");
								HUD.inventoryTransRight = true;
								this.inventoryScreenOffset.X = -1f;
								HUD.itemAlpha = (this.invSubStage = 0);
							}
						}
					}
					bool flag = this.inventoryState - 4 == (InventoryState)j;
					Game1.bigText.Color = (flag ? color : new Color(0.5f, 0.5f, 0.5f, (int)color2.A));
					vector2 = new Vector2(Game1.screenWidth / 2 - 410 + 260 * j - j / 3 * 260 * 3, vector.Y - 80f + (float)(j / 3 * 40));
					if (flag)
					{
						HUD.sprite.Draw(HUD.hudTex[2], vector2 + new Vector2(140f, 14f), new Rectangle(0, 502, 326, 18), Game1.bigText.Color * 0.8f, 0f, new Vector2(163f, 0f), new Vector2(0.8f, 0.5f), SpriteEffects.None, 0f);
					}
					Game1.bigText.DrawOutlineText(vector2, Strings_HudInv.ResourceManager.GetString("InvCat" + j), 0.8f, 280, TextAlign.Center, fullOutline: true);
				}
				if (this.helpState == 0 && Game1.pcManager.DrawMouseButton(new Vector2(Game1.screenWidth / 2 + 394, vector.Y - 90f), 0.8f, new Color(1f, 1f, 1f, 1f - this.inventoryScreenOffset.Y), 0, draw: true) && this.LeavePage(this.inventoryState))
				{
					this.ExitInventory();
				}
			}
			if (HUD.inventoryTex != null && !HUD.inventoryTex.IsDisposed)
			{
				Rectangle value = new Rectangle(0, 0, 930, 570);
				if (this.inventoryState == InventoryState.Quests && this.inventoryScreenOffset.X > -1f)
				{
					value = new Rectangle(0, 0, 1000, 634);
				}
				HUD.sprite.Draw(HUD.inventoryTex, vector + new Vector2(0f, -10f), value, color);
				if (this.inventoryScreenOffset.X > -1f)
				{
					switch (this.inventoryState)
					{
					case InventoryState.Character:
						this.DrawCharacter(vector, color, posInvY);
						break;
					case InventoryState.Equipment:
						this.DrawEquipment(vector, color, posInvY);
						break;
					case InventoryState.Quests:
						this.DrawQuests(vector, color, posInvY);
						break;
					case InventoryState.Map:
						this.DrawMap(vector, color, posInvY, 1);
						break;
					case InventoryState.Materials:
						this.DrawMaterials(vector, color, posInvY);
						break;
					case InventoryState.Stats:
						this.DrawStats(vector, color);
						break;
					}
				}
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
				{
					Game1.pcManager.DrawCursor(HUD.sprite, 0.8f, Color.White);
				}
			}
			else
			{
				this.SetInventoryTextures();
			}
		}

		private void DrawCharacter(Vector2 screenOffset, Color color, float posInvY)
		{
			bool flag = this.invSubStage == 0 && this.helpState == 0 && Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && new Rectangle((int)screenOffset.X - 50, (int)screenOffset.Y, 1020, 500).Contains((int)this.mousePos.X, (int)this.mousePos.Y);
			int num = 4;
			int num2 = 16;
			for (int i = 0; i < Game1.stats.upgradedStat.Length; i++)
			{
				num2 = Math.Min(Game1.stats.upgradedStat[i] + HUD.upgradeCategory[i], num2);
			}
			int[] array = new int[HUD.upgradeCategory.Length];
			for (int j = 0; j < array.Length; j++)
			{
				if (j == HUD.curUpgrade && Game1.stats.skillPoints > 0 && HUD.upgradeCategory[j] + Game1.stats.upgradedStat[j] < Math.Min(15, num2 + num))
				{
					array[j] = 1;
				}
				else
				{
					array[j] = 0;
				}
			}
			float num3 = (Math.Min(num2 + num - 1, 14) - 1) * 26;
			if (color.A < byte.MaxValue)
			{
				HUD.equipFade = num3;
			}
			else
			{
				HUD.equipFade += (num3 - HUD.equipFade) * Game1.HudTime * 20f;
			}
			Vector2 vector = screenOffset + new Vector2(74f, 35f);
			Vector2 vector2 = screenOffset + new Vector2(140f, 60f);
			Vector2 vector3 = screenOffset + new Vector2(447f, 38f);
			Color color2 = color;
			for (int k = 0; k < HUD.upgradeCategory.Length; k++)
			{
				for (int l = 2; l < Math.Min(num2 + num - 1, 14); l++)
				{
					if (l < Math.Min(num2 + num - 1, 14) - 1)
					{
						HUD.sprite.Draw(HUD.inventoryTex, vector3 + new Vector2(26 * l - 1, 70 * k - 1), new Rectangle(300, 570, 87, 60), color);
					}
					else
					{
						HUD.sprite.Draw(HUD.inventoryTex, vector3 + new Vector2(HUD.equipFade - 1f, 70 * k - 1), new Rectangle(300, 570, 87, 60), color);
					}
				}
				for (int m = 0; m < Game1.stats.upgradedStat[k] + HUD.upgradeCategory[k]; m++)
				{
					HUD.sprite.Draw(HUD.inventoryTex, vector3 + new Vector2(26 * m, 70 * k), new Rectangle(930, 60 * k, 60, 60), color);
					color2.A = (byte)MathHelper.Clamp((float)(Math.Sin(this.pulse - (float)m - (float)(k * 2)) * 255.0) * ((float)(int)color.A / 255f), 0f, 255f);
					HUD.sprite.Draw(HUD.inventoryTex, vector3 + new Vector2(26 * m, 70 * k - 60), new Rectangle(930, 360, 60, 160), color2);
				}
				int num4 = -1;
				if (flag)
				{
					if (new Rectangle(0, (int)(screenOffset.Y + 26f + (float)(k * 70)), Game1.screenWidth, 68).Contains((int)this.mousePos.X, (int)this.mousePos.Y) && HUD.curUpgrade != k)
					{
						HUD.curUpgrade = k;
						Sound.PlayCue("menu_click");
						HUD.itemAlpha = 0f;
						this.PopulateCharacter();
					}
					for (int n = 0; n < HUD.upgradeCategory[k] + array[k]; n++)
					{
						Vector2 vector4 = vector3 + new Vector2(Game1.stats.upgradedStat[k] * 26 + 26 * n, 70 * k - 10);
						if (!new Rectangle((int)vector4.X + 10, (int)vector4.Y, 32, 68).Contains((int)this.mousePos.X, (int)this.mousePos.Y) || HUD.upgradeCategory[k] <= 0 || n != HUD.upgradeCategory[k] - 1)
						{
							continue;
						}
						num4 = k;
						if (Game1.pcManager.leftMouseClicked)
						{
							Game1.pcManager.leftMouseClicked = false;
							int num5 = 0;
							for (int num6 = 0; num6 < Game1.stats.upgradedStat.Length; num6++)
							{
								num5 = Math.Max(Game1.stats.upgradedStat[num6] + HUD.upgradeCategory[num6], num5);
								num2 = Math.Min(Game1.stats.upgradedStat[num6] + HUD.upgradeCategory[num6], num2);
							}
							this.KeySelect = false;
							this.RemoveGem(num, num5);
						}
					}
				}
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && k == HUD.curUpgrade)
				{
					int num7 = 0;
					for (int num8 = 0; num8 < HUD.upgradeCategory.Length; num8++)
					{
						num7 += HUD.upgradeCategory[num8];
					}
					if (Game1.stats.skillPoints == 0 && num7 > 0)
					{
						Vector2 vector5 = vector3 + new Vector2(HUD.equipFade + 90f, 70 * k + 1);
						vector5.X = Math.Min(vector5.X, Game1.screenWidth / 2 + 409);
						vector5 = screenOffset + new Vector2(874f, 143f);
						vector5 = screenOffset + new Vector2(815f, 460f);
						if (Game1.pcManager.DrawMouseButton(vector5, 0.8f, color, 2, draw: true))
						{
							this.ConfirmGems();
						}
						Vector2 loc = screenOffset + new Vector2(745f, 460f);
						if (Game1.pcManager.DrawMouseButton(loc, 0.8f, color, 1, draw: true))
						{
							Sound.PlayCue("skill_gem_remove");
							for (int num9 = 0; num9 < HUD.upgradeCategory.Length; num9++)
							{
								Game1.stats.skillPoints += HUD.upgradeCategory[num9];
								HUD.upgradeCategory[num9] = 0;
							}
						}
					}
				}
				Color color3 = ((num4 == k) ? new Color(1f, 0f, 0f, Math.Abs((float)Math.Sin(this.pulse * 5f)) * (float)(int)color.A / 255f) : new Color(1f, 1f, 1f, Math.Abs((float)Math.Sin(this.pulse * 5f)) * (float)(int)color.A / 255f));
				for (int num10 = 0; num10 < HUD.upgradeCategory[k] + array[k]; num10++)
				{
					Vector2 position = vector3 + new Vector2(Game1.stats.upgradedStat[k] * 26 + 26 * num10, 70 * k - 10);
					HUD.sprite.Draw(HUD.inventoryTex, position, new Rectangle(930, 290, 60, 60), color3);
					if (flag && Game1.stats.skillPoints > 0 && Game1.pcManager.leftMouseClicked && new Rectangle((int)position.X + 10, (int)position.Y, Game1.screenWidth, 68).Contains((int)this.mousePos.X, (int)this.mousePos.Y))
					{
						Game1.pcManager.leftMouseClicked = false;
						this.AddGem(Game1.pManager, num, num2);
					}
				}
				if (k == HUD.curUpgrade)
				{
					HUD.sprite.Draw(HUD.inventoryTex, screenOffset + new Vector2(179f, 40 + k * 70), new Rectangle(0, 570, 300, 56), new Color(1f, 1f, 1f, HUD.itemAlpha * ((float)(int)color.A / 255f)));
				}
			}
			this.scoreDraw.Draw(Game1.stats.XP, vector2 + new Vector2(100f, 414f), 0.8f, color, ScoreDraw.Justify.Right, 1);
			Game1.bigText.Color = color;
			Game1.bigText.DrawText(new Vector2(vector2.X + 120f, vector2.Y + 406f), "/", 0.8f, 0f, TextAlign.Left);
			this.scoreDraw.Draw(Game1.stats.nextLevelXP, vector2 + new Vector2(140f, 414f), 0.8f, color, ScoreDraw.Justify.Left, 1);
			this.DrawBar(vector + new Vector2(0f, 465f), this.XPBarPos, new Color(0.25f, 1f, 1f, (float)(int)color.A / 255f), new Vector2(1f, 0.5f), backGround: true);
			Game1.smallText.Color = color;
			for (int num11 = 0; num11 < 7; num11++)
			{
				Game1.smallText.DrawText(vector + new Vector2(20f, -5 + 70 * num11), Strings_HudInv.ResourceManager.GetString("CharStatName" + num11), 0.6f);
			}
			this.scoreDraw.Draw((int)Math.Min((float)this.character[0].MaxHP * Game1.stats.bonusHealth, 9999f), vector2 + new Vector2(100f, -6f), 0.8f, color, ScoreDraw.Justify.Right, 1);
			this.scoreDraw.Draw(Game1.stats.attack, vector2 + new Vector2(100f, 64f), 0.8f, color, ScoreDraw.Justify.Right, 1);
			this.scoreDraw.Draw(Game1.stats.defense, vector2 + new Vector2(100f, 134f), 0.8f, color, ScoreDraw.Justify.Right, 1);
			this.scoreDraw.Draw((int)MathHelper.Clamp(Game1.stats.fidget, 1f, 10000000f), vector2 + new Vector2(100f, 204f), 0.8f, color, ScoreDraw.Justify.Right, 1);
			this.scoreDraw.Draw(Game1.stats.luck, vector2 + new Vector2(100f, 274f), 0.8f, color, ScoreDraw.Justify.Right, 1);
			this.scoreDraw.Draw(Game1.stats.LEVEL / 4, vector2 + new Vector2(100f, 344f), 0.8f, color, ScoreDraw.Justify.Right, 1);
			int num12 = 0;
			if (HUD.upgradeCategory[0] + array[0] > 0)
			{
				float num13 = 1f;
				for (int num14 = 0; num14 < Game1.stats.upgradedStat[0] + HUD.upgradeCategory[0] + array[0]; num14++)
				{
					num12 += (int)(100f * num13);
					num13 += 0.2f;
				}
			}
			if (num12 > 0)
			{
				this.scoreDraw.Draw((int)MathHelper.Clamp((float)(num12 + 80) * Game1.stats.bonusHealth, 0f, 9999f), vector2 + new Vector2(180f, -7f), 0.8f, new Color(0f, 1f, 0.2f, (float)(int)color.A / 255f), ScoreDraw.Justify.Left, 1);
			}
			for (int num15 = 0; num15 < HUD.upgradeCategory.Length; num15++)
			{
				if (HUD.upgradeCategory[num15] + array[num15] <= 0)
				{
					continue;
				}
				HUD.sprite.Draw(HUD.hudTex[0], vector2 + new Vector2(150f, posInvY / 10f + (float)(70 * num15)), new Rectangle(374, 88, 23, 32), Color.White, 0f, new Vector2(12f, 16f), 1f, SpriteEffects.None, 0f);
				if (num15 <= 0)
				{
					continue;
				}
				float num16 = 1f;
				int num17 = 0;
				switch (num15)
				{
				case 1:
				{
					num16 = 0.5f;
					for (int num20 = 0; num20 < Game1.stats.upgradedStat[1] + HUD.upgradeCategory[num15] + array[num15]; num20++)
					{
						num17 += (int)(12f * num16);
						num16 += 1f;
					}
					num17 += 4;
					break;
				}
				case 2:
				{
					for (int num19 = 0; num19 < Game1.stats.upgradedStat[2] + HUD.upgradeCategory[num15] + array[num15]; num19++)
					{
						num17 += (int)(10f * num16);
						num16 += 0.5f;
					}
					break;
				}
				case 3:
				{
					for (int num18 = 0; num18 < Game1.stats.upgradedStat[3] + HUD.upgradeCategory[num15] + array[num15]; num18++)
					{
						num17 += (int)(14f * num16);
						num16 += 0.25f;
					}
					num17++;
					break;
				}
				}
				this.scoreDraw.Draw(num17, vector2 + new Vector2(180f, 70 * num15 - 7), 0.8f, new Color(0f, 1f, 0.2f, (float)(int)color.A / 255f), ScoreDraw.Justify.Left, 1);
			}
			if (HUD.promptAlpha <= 0f)
			{
				if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
				{
					int num21 = (int)Math.Max(Game1.smallFont.MeasureString(Game1.inventoryManager.itemControls).X * 0.7f + 20f, 130f);
					this.DrawMiniBorder(screenOffset + new Vector2(678 - num21 / 2, 510f), num21, 30, color, (int)color.A);
					Game1.smallText.DrawButtonText(screenOffset + new Vector2(470f, 515f), Game1.inventoryManager.itemControls, 0.7f, Game1.inventoryManager.itemControlsButtonList, bounce: false, 410f, TextAlign.Center);
				}
				Game1.smallText.Color = new Color(1f, 1f, 1f, HUD.itemAlpha * ((float)(int)color.A / 255f));
				Game1.smallText.DrawButtonText(screenOffset + new Vector2(492f, 350f), Game1.menu.optionDesc, 0.7f, Game1.menu.optionDescButtonList, bounce: false, 375f, TextAlign.Left);
				if (Game1.stats.skillPoints > 0)
				{
					Game1.smallText.Color = new Color(0f, 1f, 0f, HUD.itemAlpha * (float)(int)color.A / 255f);
					Game1.smallText.DrawText(screenOffset + new Vector2(492f, 350f + Game1.smallFont.MeasureString(Game1.menu.optionDesc).Y * 0.7f), Strings_HudInv.ResourceManager.GetString("UpgradeRemaining") + " " + Game1.stats.skillPoints, 0.7f, 0f, TextAlign.Left);
				}
			}
			this.cursorPos += (vector3 + new Vector2(20f, 5f) + new Vector2(MathHelper.Clamp(Math.Min(Game1.stats.upgradedStat[HUD.curUpgrade] + HUD.upgradeCategory[HUD.curUpgrade] - ((Game1.stats.skillPoints <= 0) ? 1 : 0), num2 + num - 1), 0f, 15f) * 26f, HUD.curUpgrade * 70) - this.cursorPos) * Game1.HudTime * 20f;
			if ((Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse || flag) && Game1.stats.skillPoints > 0)
			{
				Game1.smallText.Color = color;
				int num22 = (int)(Game1.smallFont.MeasureString(Strings_HudInv.UpgradeRemaining + " " + Game1.stats.skillPoints).X * 0.7f) + 30;
				Vector2 vector6 = ((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse) ? new Vector2(Math.Min(this.mousePos.X + 50f, Game1.screenWidth - num22), this.mousePos.Y + 50f) : (this.cursorPos + new Vector2(-num22 / 2 - 20, -60f)));
				this.DrawMiniBorder(vector6, num22, 10, color, 0.85f);
				Game1.smallText.DrawText(vector6 + new Vector2(0f, 5f), Strings_HudInv.UpgradeRemaining + " " + Game1.stats.skillPoints, 0.7f, num22, TextAlign.Center);
			}
			if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				this.DrawCursor(this.cursorPos, 0.75f, color, flip: false);
			}
			if (HUD.promptAlpha > 0f)
			{
				HUD.sprite.Draw(HUD.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, 0.5f * HUD.promptAlpha));
				Game1.menu.DrawPauseMenu(HUD.itemAlpha);
			}
		}

		private void DrawEquipment(Vector2 screenOffset, Color color, float posInvY)
		{
			Vector2 vector = screenOffset + new Vector2(74f, 35f);
			Vector2 vector2 = screenOffset + new Vector2(100f, 60f);
			float num = 0.8f;
			Color color4 = (Game1.smallText.Color = (Game1.bigText.Color = color));
			for (int i = 0; i < 7; i++)
			{
				Game1.smallText.DrawText(vector + new Vector2(20f, -5 + 70 * i), Strings_HudInv.ResourceManager.GetString("EquipStatName" + i), 0.6f);
			}
			Game1.bigText.DrawText(vector2 + new Vector2(120f, -15f), "/", num, 0f, TextAlign.Left);
			Game1.bigText.DrawText(vector2 + new Vector2(120f, 406f), "/", num, 0f, TextAlign.Left);
			int num2 = (int)Math.Min((float)this.character[0].MaxHP * Game1.stats.bonusHealth, 9999f);
			if ((float)this.character[0].HP < (float)num2 * 0.2f)
			{
				Game1.smallText.Color = new Color(1f, this.lifeBarBeat, this.lifeBarBeat, (float)(int)color.A / 255f);
			}
			if (this.character[0].HP > -1)
			{
				this.scoreDraw.Draw(this.character[0].HP, vector2 + new Vector2(100f, -7f), num, Game1.smallText.Color, ScoreDraw.Justify.Right, 1);
			}
			else
			{
				this.scoreDraw.Draw(0L, vector2 + new Vector2(495f, 230f), num, Game1.smallText.Color, ScoreDraw.Justify.Right, 1);
			}
			this.scoreDraw.Draw(num2, vector2 + new Vector2(140f, -7f), num, color, ScoreDraw.Justify.Left, 1);
			Color color5 = new Color(0.5f, 1f, 0.2f, (float)(int)color.A / 255f);
			if ((float)this.character[0].HP < (float)num2 * 0.2f)
			{
				color5 = new Color(1f, this.lifeBarBeat, this.lifeBarBeat, (float)(int)color.A / 255f);
			}
			this.DrawBar(vector + new Vector2(0f, 45f), HUD.lifeBarPos, color5, new Vector2(0.9f, 0.5f), backGround: true);
			this.scoreDraw.Draw(Game1.stats.XP, vector2 + new Vector2(100f, 414f), 0.8f, color, ScoreDraw.Justify.Right, 1);
			this.scoreDraw.Draw(Game1.stats.nextLevelXP, vector2 + new Vector2(140f, 414f), 0.8f, color, ScoreDraw.Justify.Left, 1);
			this.DrawBar(vector + new Vector2(0f, 465f), this.XPBarPos, new Color(0.25f, 1f, 1f, (float)(int)color.A / 255f), new Vector2(0.9f, 0.5f), backGround: true);
			int attackEquip = Game1.stats.attackEquip;
			int defenseEquip = Game1.stats.defenseEquip;
			int newFidgetEquip = Game1.stats.newFidgetEquip;
			int luckEquip = Game1.stats.luckEquip;
			if (this.invSubStage > 0 && (Game1.stats.Equipment[Game1.inventoryManager.invSelection] > 0 || (Game1.inventoryManager.invSelection >= Game1.inventoryManager.invSelMax && Game1.inventoryManager.invSelection < Game1.inventoryManager.invSelMax * 5 && Game1.stats.EquipBluePrint[Game1.inventoryManager.invSelection - Game1.inventoryManager.invSelMax] > 0)) && HUD.equipCategory > 0)
			{
				new Color(1f, 1f, 1f, posInvY / 40f);
				if (attackEquip != HUD.newAttack)
				{
					Color color6;
					if (attackEquip > HUD.newAttack)
					{
						color6 = Color.Red;
						HUD.sprite.Draw(HUD.hudTex[0], vector2 + new Vector2(150f, 70f + posInvY / 10f), new Rectangle(397, 88, 23, 32), Color.White, 0f, new Vector2(12f, 16f), 1f, SpriteEffects.None, 0f);
					}
					else
					{
						color6 = new Color(0f, 1f, 0f, 1f);
						HUD.sprite.Draw(HUD.hudTex[0], vector2 + new Vector2(150f, 70f + posInvY / 10f), new Rectangle(374, 88, 23, 32), Color.White, 0f, new Vector2(12f, 16f), 1f, SpriteEffects.None, 0f);
					}
					this.scoreDraw.Draw(HUD.newAttack, vector2 + new Vector2(180f, 64f), num, color6, ScoreDraw.Justify.Left, 1);
				}
				if (defenseEquip != HUD.newDefense)
				{
					Color color7;
					if (defenseEquip > HUD.newDefense)
					{
						color7 = Color.Red;
						HUD.sprite.Draw(HUD.hudTex[0], vector2 + new Vector2(150f, 140f + posInvY / 10f), new Rectangle(397, 88, 23, 32), Color.White, 0f, new Vector2(12f, 16f), 1f, SpriteEffects.None, 0f);
					}
					else
					{
						color7 = new Color(0f, 1f, 0f, 1f);
						HUD.sprite.Draw(HUD.hudTex[0], vector2 + new Vector2(150f, 140f + posInvY / 10f), new Rectangle(374, 88, 23, 32), Color.White, 0f, new Vector2(12f, 16f), 1f, SpriteEffects.None, 0f);
					}
					this.scoreDraw.Draw(HUD.newDefense, vector2 + new Vector2(180f, 134f), num, color7, ScoreDraw.Justify.Left, 1);
				}
				if (newFidgetEquip != HUD.newFidget)
				{
					Color color8;
					if (newFidgetEquip > HUD.newFidget)
					{
						color8 = Color.Red;
						HUD.sprite.Draw(HUD.hudTex[0], vector2 + new Vector2(150f, 210f + posInvY / 10f), new Rectangle(397, 88, 23, 32), Color.White, 0f, new Vector2(12f, 16f), 1f, SpriteEffects.None, 0f);
					}
					else
					{
						color8 = new Color(0f, 1f, 0f, 1f);
						HUD.sprite.Draw(HUD.hudTex[0], vector2 + new Vector2(150f, 210f + posInvY / 10f), new Rectangle(374, 88, 23, 32), Color.White, 0f, new Vector2(12f, 16f), 1f, SpriteEffects.None, 0f);
					}
					this.scoreDraw.Draw(HUD.newFidget, vector2 + new Vector2(180f, 204f), num, color8, ScoreDraw.Justify.Left, 1);
				}
				if (luckEquip != HUD.newLuck)
				{
					Color color9;
					if (luckEquip > HUD.newLuck)
					{
						color9 = Color.Red;
						HUD.sprite.Draw(HUD.hudTex[0], vector2 + new Vector2(150f, 280f + posInvY / 10f), new Rectangle(397, 88, 23, 32), Color.White, 0f, new Vector2(12f, 16f), 1f, SpriteEffects.None, 0f);
					}
					else
					{
						color9 = new Color(0f, 1f, 0f, 1f);
						HUD.sprite.Draw(HUD.hudTex[0], vector2 + new Vector2(150f, 280f + posInvY / 10f), new Rectangle(374, 88, 23, 32), Color.White, 0f, new Vector2(12f, 16f), 1f, SpriteEffects.None, 0f);
					}
					this.scoreDraw.Draw(HUD.newLuck, vector2 + new Vector2(180f, 274f), num, color9, ScoreDraw.Justify.Left, 1);
				}
			}
			float num3 = 1f;
			int num4 = 1;
			if (Game1.stats.attackBonusTime > 0f)
			{
				num3 = 1.2f;
			}
			if (Game1.stats.defenseBonusTime > 0f)
			{
				num4 = 4;
			}
			this.scoreDraw.Draw((int)((float)attackEquip * num3), vector2 + new Vector2(100f, 64f), num, color, ScoreDraw.Justify.Right, 1);
			this.scoreDraw.Draw(defenseEquip * num4, vector2 + new Vector2(100f, 134f), num, color, ScoreDraw.Justify.Right, 1);
			this.scoreDraw.Draw((int)((float)newFidgetEquip * num3), vector2 + new Vector2(100f, 204f), num, color, ScoreDraw.Justify.Right, 1);
			this.scoreDraw.Draw(luckEquip, vector2 + new Vector2(100f, 274f), num, color, ScoreDraw.Justify.Right, 1);
			this.scoreDraw.Draw(Game1.stats.Gold, vector2 + new Vector2(100f, 344f), num, color, ScoreDraw.Justify.Right, 1);
			Game1.bigText.DrawOutlineText(screenOffset + new Vector2(390f, 20f), Strings_HudInv.ResourceManager.GetString("EquipSelection" + this.equipSelection), 0.8f, 480, TextAlign.Center, fullOutline: true);
			Vector2 vector3 = screenOffset + new Vector2(482f, 100f);
			for (int j = 0; j < Game1.inventoryManager.invCatMax + 1; j++)
			{
				Rectangle value = new Rectangle(-100, -100, 1, 1);
				int invSelMax = Game1.inventoryManager.invSelMax;
				switch (j)
				{
				case 0:
					if (Game1.stats.currentItem > -1 && Game1.stats.Equipment[Game1.stats.currentItem] > 0)
					{
						value = new Rectangle(0, Game1.stats.currentItem * 60, 60, 60);
					}
					break;
				case 1:
					if (Game1.stats.currentArmor > -1 && Game1.stats.Equipment[Game1.stats.currentArmor] > 0)
					{
						value = new Rectangle(180, (Game1.stats.currentArmor - invSelMax * 3) * 60, 60, 60);
					}
					break;
				case 2:
					if (Game1.stats.currentAugment > -1 && Game1.stats.Equipment[Game1.stats.currentAugment] > 0)
					{
						value = new Rectangle(120, (Game1.stats.currentAugment - invSelMax * 2) * 60, 60, 60);
					}
					break;
				case 3:
					value = new Rectangle(360, 420, 60, 60);
					break;
				case 4:
					if (Game1.stats.currentRingLeft > -1 && Game1.stats.Equipment[Game1.stats.currentRingLeft] > 0)
					{
						value = new Rectangle(240, (Game1.stats.currentRingLeft - invSelMax * 4) * 60, 60, 60);
					}
					break;
				case 5:
					if (Game1.stats.currentPendant > -1 && Game1.stats.Equipment[Game1.stats.currentPendant] > 0)
					{
						value = new Rectangle(60, (Game1.stats.currentPendant - invSelMax) * 60, 60, 60);
					}
					break;
				case 6:
					if (Game1.stats.currentRingRight > -1 && Game1.stats.Equipment[Game1.stats.currentRingRight] > 0)
					{
						value = new Rectangle(240, (Game1.stats.currentRingRight - invSelMax * 4) * 60, 60, 60);
					}
					break;
				}
				float num5 = 0f;
				float num6 = 1f;
				float rotation = 0f;
				float a = 0.5f;
				if (j == this.equipSelection && HUD.equipFade < 1f)
				{
					num5 = posInvY;
					num6 = MathHelper.Clamp(0.5f + HUD.itemAlpha * 2f, 1f, 1.5f);
					rotation = (float)Math.Sin(this.pulse) / 10f;
					a = MathHelper.Clamp(HUD.itemAlpha * (6.28f / posInvY), 0f, 0.5f);
				}
				Vector2 vector4 = vector3 + new Vector2(1 + j * 102 - j / 4 * 408, 42 + j / 4 * 115);
				if (j > 3)
				{
					vector4.X += 51f;
				}
				HUD.sprite.Draw(HUD.particlesTex[4], vector4 + new Vector2(10f + num5 / 2f, 17f), value, new Color(0f, 0f, 0f, a), rotation, new Vector2(30f, 30f), num6 * new Vector2(1f, 0.5f), SpriteEffects.None, 0f);
				HUD.sprite.Draw(HUD.particlesTex[4], vector4 + new Vector2(0f, (0f - num5) / 2f), value, color, rotation, new Vector2(30f, 30f), num6, SpriteEffects.None, 0f);
				if (j == this.equipSelection && this.invSubStage == 0)
				{
					new Color(1f, 1f, 1f, HUD.itemAlpha * HUD.equipFade);
					Vector2 vector5 = vector4 + new Vector2(-23f, -13f);
					if (color.A < byte.MaxValue)
					{
						this.cursorPos = vector5;
					}
					else
					{
						this.cursorPos += (vector5 - this.cursorPos) * Game1.HudTime * 20f;
					}
					if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
					{
						this.DrawCursor(this.cursorPos, 0.75f, color, flip: false);
					}
				}
			}
			if (Game1.stats.currentItem > -1 && Game1.stats.Equipment[Game1.stats.currentItem] > 0)
			{
				this.scoreDraw.Draw(Game1.stats.Equipment[Game1.stats.currentItem], vector3 + new Vector2(15f, 52f), 0.8f, color, ScoreDraw.Justify.Center, 0);
			}
			if (HUD.equipFade > 0f)
			{
				Color color10 = new Color(1f, 1f, 1f, HUD.equipFade);
				Vector2 vector6 = screenOffset + new Vector2(354f, -10f - HUD.equipFade * 20f);
				HUD.sprite.Draw(HUD.inventoryTex, vector6 + new Vector2(20f, 20f), new Rectangle(930, 0, 525, 350), color10);
				Game1.bigText.Color = color10;
				Game1.bigText.DrawOutlineText(vector6 + new Vector2(20f, 40f), Game1.inventoryManager.categoryName, 0.8f, 525, TextAlign.Center, fullOutline: true);
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
				{
					Game1.pcManager.DrawMouseButton(vector6 + new Vector2(510f, 30f), 0.8f, color10, 1, draw: true);
					Game1.pcManager.DrawMouseButton(vector6 + new Vector2(510f, 90f), 0.8f, color10, 4, draw: true);
				}
				int num7 = 5;
				Vector2 vector7 = vector6 + new Vector2(110f, 125f);
				vector7.Y -= Math.Max(HUD.equipSelection2 / num7 - 1, 0) * 78;
				HUD.inventorySelectionPos.X = vector7.X;
				if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
				{
					if (HUD.equipFade < 1f)
					{
						HUD.inventorySelectionPos.Y = vector7.Y;
					}
					else
					{
						HUD.inventorySelectionPos.Y += (vector7.Y - HUD.inventorySelectionPos.Y) * Game1.HudTime * 20f;
					}
				}
				for (int k = 0; k < this.equipList.Count; k++)
				{
					float num8 = HUD.inventorySelectionPos.Y + (float)(k / num7 * 78);
					if (!(num8 > vector6.Y + 110f) || !(num8 < vector6.Y + 300f))
					{
						continue;
					}
					Color color11 = new Color(1f, 1f - posInvY / 40f, posInvY / 40f, HUD.equipFade);
					int num9 = (int)this.equipList[k].X - Game1.inventoryManager.invSelMax * HUD.equipCategory;
					if (this.CanDrawItemBorder((int)this.equipList[k].X) && this.equipList[k].Z == 0f)
					{
						HUD.sprite.Draw(HUD.inventoryTex, HUD.inventorySelectionPos + new Vector2(-1 + k * 78 - k / num7 * num7 * 78, 1 + k / num7 * 78), new Rectangle(0, 570, 76, 76), color11, 0f, new Vector2(38f, 38f), 1f, SpriteEffects.None, 0f);
					}
					float num10 = 0f;
					float num11 = 1f;
					float rotation2 = 0f;
					float a2 = 0.5f;
					if (k == HUD.equipSelection2 || (Game1.inventoryManager.ItemEquipped((int)this.equipList[k].X, HUD.equipSelection2) && this.equipList[k].Z == 0f))
					{
						if (k == HUD.equipSelection2)
						{
							num11 = MathHelper.Clamp(0.5f + HUD.itemAlpha * 2f, 1f, 1.5f);
						}
						num10 = posInvY;
						rotation2 = (float)Math.Sin(this.pulse) / 10f;
						a2 = MathHelper.Clamp(6.28f / posInvY, 0f, 0.5f) * HUD.equipFade;
					}
					if (this.equipList[k].Z == 1f)
					{
						num11 *= 0.6f;
						HUD.sprite.Draw(HUD.particlesTex[1], HUD.inventorySelectionPos + new Vector2(2 + k * 78 - k / num7 * num7 * 78, (float)(k / num7 * 78) - num10 / 2f), new Rectangle(0, 0, 102, 128), color10, rotation2, new Vector2(52f, 60f), num11, SpriteEffects.None, 1f);
					}
					else
					{
						HUD.sprite.Draw(HUD.particlesTex[4], HUD.inventorySelectionPos + new Vector2((float)(10 + k * 78 - k / num7 * num7 * 78) + num10 / 2f, 17 + k / num7 * 78), new Rectangle(HUD.equipCategory * 60, num9 * 60, 60, 60), new Color(0f, 0f, 0f, a2), rotation2, new Vector2(30f, 30f), num11 * new Vector2(1f, 0.5f), SpriteEffects.None, 0f);
					}
					HUD.sprite.Draw(HUD.particlesTex[4], HUD.inventorySelectionPos + new Vector2(k * 78 - k / num7 * num7 * 78, (float)(k / num7 * 78) - num10 / 2f), new Rectangle(HUD.equipCategory * 60, num9 * 60, 60, 60), color10, rotation2, new Vector2(30f, 30f), num11, SpriteEffects.None, 0f);
					if (this.equipList[k].Y > 0f)
					{
						this.scoreDraw.Draw((long)this.equipList[k].Y, HUD.inventorySelectionPos + new Vector2(19 + k * 78 - k / num7 * num7 * 78, 15 + Math.Abs(k / num7) * 78), 0.8f, color10, ScoreDraw.Justify.Center, 0);
					}
					if (HUD.equipSelection2 == k && this.invSubStage == 1)
					{
						Vector2 vector8 = HUD.inventorySelectionPos + new Vector2(-20 + k * 78 - k / num7 * num7 * 78, -23 + k / num7 * 78);
						if (color.A < byte.MaxValue)
						{
							this.cursorPos = vector8;
						}
						else
						{
							this.cursorPos += (vector8 - this.cursorPos) * Game1.HudTime * 20f;
						}
						if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
						{
							this.DrawCursor(this.cursorPos, 0.75f, color * HUD.equipFade, flip: false);
						}
					}
				}
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
				{
					int num12 = 78;
					int num13 = this.equipList.Count / num7 * num12;
					int num14 = (int)(vector6.Y + 120f);
					float persistFloat = 0f - HUD.inventorySelectionPos.Y + (float)num14;
					this.DrawScrollBarMouse(vector6 + new Vector2(490f, 120f), ref persistFloat, 140, num13, num12, (float)(int)color.A / 255f);
					Game1.pcManager.MouseWheelAdjust(ref persistFloat, num12, 0f, num13);
					HUD.inventorySelectionPos.Y = 0f - persistFloat + (float)num14;
				}
				else
				{
					this.scrollBarPos = this.DrawScrollBar(vector6 + new Vector2(490f, 120f), (float)(HUD.equipSelection2 / num7) / (float)(this.equipList.Count / num7), 140f, HUD.equipFade);
				}
			}
			if (HUD.equipFade2 > 0f)
			{
				Color color12 = new Color(1f, 1f, 1f, HUD.equipFade2);
				Vector2 vector9 = screenOffset + new Vector2(364f, 0f - HUD.equipFade2 * 20f);
				HUD.sprite.Draw(HUD.inventoryTex, vector9 + new Vector2(20f, 20f), new Rectangle(930, 350, 525, 350), color12);
				Game1.bigText.Color = color12;
				Game1.bigText.DrawOutlineText(vector9 + new Vector2(20f, 35f), Strings_HudInv.BlueprintReq, 0.8f, 525, TextAlign.Center, fullOutline: true);
				Game1.smallText.Color = color12;
				string s = Game1.smallText.WordWrap(Strings_HudInv.BlueprintDesc, 0.7f, 410f, TextAlign.Left);
				Game1.smallText.DrawText(vector9 + new Vector2(70f, 240f), s, 0.7f);
				if (this.equipList.Count > 0 && this.equipList[HUD.equipSelection2].Z == 1f)
				{
					this.DrawMaterialsRequired(HUD.equipFade2, vector9, Game1.inventoryManager.invSelection, HUD.equipSelection3, alignName: false);
				}
			}
			Vector2 vector10 = screenOffset + new Vector2(412f, 340f);
			if ((this.invSubStage == 0 || Game1.stats.Equipment[Game1.inventoryManager.invSelection] > 0 || (Game1.inventoryManager.invSelection >= Game1.inventoryManager.invSelMax && Game1.inventoryManager.invSelection < Game1.inventoryManager.invSelMax * 5 && Game1.stats.EquipBluePrint[Game1.inventoryManager.invSelection - Game1.inventoryManager.invSelMax] > 0)) && Game1.inventoryManager.itemName != string.Empty && Game1.inventoryManager.itemName != null)
			{
				float num15 = 0.8f;
				Game1.smallText.Color = new Color(1f, 1f, 1f, HUD.itemAlpha * ((float)(int)color.A / 255f));
				Game1.smallText.DrawText(vector10, Game1.inventoryManager.itemName, num15, 455f, TextAlign.Left);
				int num16 = (int)(Game1.smallFont.MeasureString(Game1.inventoryManager.itemName).X * num15);
				if (Game1.inventoryManager.itemCost > 0 && num16 < 360)
				{
					this.scoreDraw.Draw(Game1.inventoryManager.itemCost, screenOffset + new Vector2(830f, 344f), num15, Game1.smallText.Color, ScoreDraw.Justify.Right, 0);
					HUD.sprite.Draw(HUD.particlesTex[2], screenOffset + new Vector2(850f, 344f), new Rectangle(540 + HUD.coinAnimFrame * 20, 2110, 20, 20), Game1.smallText.Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
				}
				vector10.Y += Game1.smallFont.MeasureString(Game1.inventoryManager.itemName).Y * num15;
				HUD.sprite.Draw(HUD.hudTex[2], vector10 + new Vector2(150f, -4f), new Rectangle(0, 502, 326, 18), Game1.smallText.Color, 0f, new Vector2(163f, 0f), new Vector2(1f, 0.5f), SpriteEffects.None, 0f);
				vector10.Y += 5f;
				num15 = 0.7f;
				Game1.smallText.DrawText(vector10, Game1.inventoryManager.itemInfo, num15);
				vector10.Y += Game1.smallFont.MeasureString(Game1.inventoryManager.itemInfo).Y * num15;
				num15 = 0.65f;
				Game1.smallText.Color = new Color(0f, 1f, 0f, HUD.itemAlpha * ((float)(int)color.A / 255f));
				string text = Game1.smallText.WordWrap(Game1.inventoryManager.itemStats, num15, 455f, TextAlign.Left);
				Game1.smallText.DrawText(vector10, text, num15);
				vector10.Y += Game1.smallFont.MeasureString(text).Y * num15;
			}
			if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				Game1.smallText.Color = color;
				int num17 = (int)(Game1.smallFont.MeasureString(Game1.inventoryManager.itemControls).X * 0.7f) + 20;
				int num18 = (int)(Game1.smallFont.MeasureString(Game1.inventoryManager.itemControls).Y * 0.7f) + 10;
				this.DrawMiniBorder(screenOffset + new Vector2(638 - num17 / 2, 510 - Math.Max(0, (num18 - 30) / 2)), num17, num18, color, 1f);
				Game1.smallText.DrawButtonText(screenOffset + new Vector2(648 - num17 / 2, 515 - Math.Max(0, (num18 - 30) / 2)), Game1.inventoryManager.itemControls, 0.7f, Game1.inventoryManager.itemControlsButtonList, bounce: false, 1000f, TextAlign.Left);
			}
			else if (this.mouseHighLighted > 0f && this.invSubStage == 1 && this.equipList.Count > 0 && this.equipList[HUD.equipSelection2].Z == 1f)
			{
				this.DrawMaterialsRequiredMouse(this.mouseHighLighted, (int)this.equipList[HUD.equipSelection2].X);
			}
		}

		public int DrawMaterialsRequired(float windowAlpha, Vector2 screenOffset, int selection, int materialSelection, bool alignName)
		{
			int result = -1;
			Color color = new Color(1f, 1f, 1f, windowAlpha);
			int num = 0;
			if (selection < 0 || Game1.inventoryManager.equipItem[selection] == null)
			{
				return -1;
			}
			for (int i = 0; i < Game1.inventoryManager.equipItem[selection].MaterialReq.Length; i++)
			{
				if (Game1.inventoryManager.equipItem[selection].MaterialReq[i] > -1)
				{
					num++;
				}
			}
			Game1.smallText.Color = color;
			float num2 = 0.8f;
			for (int j = 0; j < num; j++)
			{
				int num3 = Game1.inventoryManager.equipItem[selection].MaterialReq[j];
				if (num3 <= -1)
				{
					continue;
				}
				int num4 = Game1.inventoryManager.equipItem[selection].MaterialReqAmt[j];
				int num5 = num3 / 12;
				Rectangle value = new Rectangle(num5 * 60, (num3 - num5 * 12) * 60, 60, 60);
				Color color2;
				Color color3;
				if (Game1.stats.Material[num3] < num4)
				{
					float num6 = windowAlpha * 0.2f + MathHelper.Clamp((float)Math.Sin(this.pulse * 4f), 0f, 1f);
					color2 = new Color(num6 * 2f, num6, num6, windowAlpha);
					color3 = Color.Red;
				}
				else
				{
					color2 = color;
					color3 = Color.White;
				}
				HUD.sprite.Draw(HUD.particlesTex[5], screenOffset + new Vector2(100 + 72 * j + (6 - num) * 36, 120f), value, color2, 0f, new Vector2(30f, 30f), 1f, SpriteEffects.None, 1f);
				int num7 = Game1.stats.Material[Game1.inventoryManager.equipItem[selection].MaterialReq[j]];
				if (num7 < 100)
				{
					this.scoreDraw.Draw(Math.Max(num7, 0), screenOffset + new Vector2(104 + 72 * j + (6 - num) * 36, 130f) + new Vector2(-20f, 20f), 0.7f, color3 * windowAlpha, ScoreDraw.Justify.Right, 0);
				}
				else
				{
					this.scoreDraw.Draw(Math.Max(num7, 0), screenOffset + new Vector2(108 + 72 * j + (6 - num) * 36, 130f) + new Vector2(-20f, 20f), new Vector2(0.4f, 0.7f), color3 * windowAlpha, ScoreDraw.Justify.Right, 0);
				}
				Game1.smallText.DrawOutlineText(screenOffset + new Vector2(96 + 72 * j + (6 - num) * 36, 130f) + new Vector2(2f, 20f), "/", 0.6f, 0, TextAlign.Left, fullOutline: true);
				if (num4 < 100)
				{
					this.scoreDraw.Draw(num4, screenOffset + new Vector2(100 + 72 * j + (6 - num) * 36, 130f) + new Vector2(5f, 20f), 0.7f, Color.White * windowAlpha, ScoreDraw.Justify.Left, 0);
				}
				else
				{
					this.scoreDraw.Draw(num4, screenOffset + new Vector2(102 + 72 * j + (6 - num) * 36, 130f) + new Vector2(5f, 20f), new Vector2(0.4f, 0.7f), Color.White * windowAlpha, ScoreDraw.Justify.Left, 0);
				}
				if (Game1.inventoryManager.material[num3] == null)
				{
					continue;
				}
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
				{
					Vector2 vector = screenOffset + new Vector2(70 + 72 * j + (6 - num) * 36, 90f);
					if (new Rectangle((int)vector.X, (int)vector.Y, 60, 60).Contains((int)this.mousePos.X, (int)this.mousePos.Y))
					{
						result = j;
					}
				}
				else if (materialSelection == j)
				{
					Vector2 vector2 = screenOffset + new Vector2(80 + 72 * j + (6 - num) * 36, 110f);
					this.cursorPos += (vector2 - this.cursorPos) * Game1.HudTime * 20f;
					if (!alignName)
					{
						Game1.smallText.DrawText(screenOffset + new Vector2(70f, 210f), Game1.inventoryManager.material[num3].name, num2);
					}
					this.DrawCursor(this.cursorPos, 0.75f, color, flip: false);
				}
			}
			if (!alignName)
			{
				this.scoreDraw.Draw(Game1.inventoryManager.equipItem[selection].Value / 4, screenOffset + new Vector2(465f, 215f), num2, color, ScoreDraw.Justify.Right, 0);
				HUD.sprite.Draw(HUD.particlesTex[2], screenOffset + new Vector2(485f, 215f), new Rectangle(540 + HUD.coinAnimFrame * 20, 2110, 20, 20), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			}
			return result;
		}

		private void DrawMaterialsRequiredMouse(float windowAlpha, int selection)
		{
			Color color = new Color(1f, 1f, 1f, windowAlpha);
			int num = 0;
			if (selection < 0 || Game1.inventoryManager.equipItem[selection] == null)
			{
				return;
			}
			for (int i = 0; i < Game1.inventoryManager.equipItem[selection].MaterialReq.Length; i++)
			{
				if (Game1.inventoryManager.equipItem[selection].MaterialReq[i] > -1)
				{
					num++;
				}
			}
			if (num == 0)
			{
				return;
			}
			int num2 = Math.Max(num * 72 + 50, 200);
			Vector2 vector = new Vector2(Math.Min(this.mousePos.X - (float)(num2 / 2) + 60f, Game1.screenWidth - num2 + 80), this.mousePos.Y + 70f - 180f);
			this.DrawMiniBorder(vector - new Vector2(60f, 65f), num2, 125, color, 0.95f);
			Game1.smallText.Color = color;
			Game1.smallText.DrawText(vector - new Vector2(60f, 64f), Strings_HudInv.BlueprintReq, 0.8f, num2, TextAlign.Center);
			HUD.sprite.Draw(HUD.hudTex[2], vector + new Vector2(num2 / 2 - 60, -36f), new Rectangle(0, 502, 326, 18), color, 0f, new Vector2(163f, 0f), new Vector2(0.6f, 0.3f), SpriteEffects.None, 0f);
			if (num == 1)
			{
				vector.X += 36f;
			}
			for (int j = 0; j < num; j++)
			{
				int num3 = Game1.inventoryManager.equipItem[selection].MaterialReq[j];
				if (num3 > -1)
				{
					int num4 = Game1.inventoryManager.equipItem[selection].MaterialReqAmt[j];
					int num5 = num3 / 12;
					Rectangle value = new Rectangle(num5 * 60, (num3 - num5 * 12) * 60, 60, 60);
					Color color2;
					Color color3;
					if (Game1.stats.Material[num3] < num4)
					{
						float num6 = windowAlpha * 0.2f + MathHelper.Clamp((float)Math.Sin(this.pulse * 4f), 0f, 1f);
						color2 = new Color(num6 * 2f, num6, num6, windowAlpha);
						color3 = Color.Red;
					}
					else
					{
						color2 = color;
						color3 = Color.White;
					}
					HUD.sprite.Draw(HUD.particlesTex[5], vector + new Vector2(72 * j, 0f), value, color2, 0f, new Vector2(30f, 30f), 0.9f, SpriteEffects.None, 1f);
					int num7 = Game1.stats.Material[Game1.inventoryManager.equipItem[selection].MaterialReq[j]];
					if (num7 < 100)
					{
						this.scoreDraw.Draw(Math.Max(num7, 0), vector + new Vector2(4 + 72 * j, 10f) + new Vector2(-20f, 20f), 0.7f, color3 * windowAlpha, ScoreDraw.Justify.Right, 0);
					}
					else
					{
						this.scoreDraw.Draw(Math.Max(num7, 0), vector + new Vector2(8 + 72 * j, 10f) + new Vector2(-20f, 20f), new Vector2(0.4f, 0.7f), color3 * windowAlpha, ScoreDraw.Justify.Right, 0);
					}
					Game1.smallText.DrawOutlineText(vector + new Vector2(-4 + 72 * j, 10f) + new Vector2(2f, 20f), "/", 0.6f, 0, TextAlign.Left, fullOutline: true);
					if (num4 < 100)
					{
						this.scoreDraw.Draw(num4, vector + new Vector2(72 * j, 10f) + new Vector2(5f, 20f), 0.7f, Color.White * windowAlpha, ScoreDraw.Justify.Left, 0);
					}
					else
					{
						this.scoreDraw.Draw(num4, vector + new Vector2(2 + 72 * j, 10f) + new Vector2(5f, 20f), new Vector2(0.4f, 0.7f), Color.White * windowAlpha, ScoreDraw.Justify.Left, 0);
					}
				}
			}
		}

		private void DrawMaterials(Vector2 screenOffset, Color color, float posInvY)
		{
			HUD.equipFade = (HUD.equipFade2 = 1f);
			Color color2 = new Color(1f, 1f, 1f, HUD.equipFade);
			int num = 78;
			int num2 = 5;
			int num3 = 2;
			int num4 = (int)screenOffset.Y + 50;
			int num5 = num4 + num * 5 + 65;
			Vector2 vector = screenOffset + new Vector2(460f, 90f);
			if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				vector.Y -= Math.Max(this.equipSelection / num2 - num3, 0) * num;
			}
			HUD.inventorySelectionPos.X = vector.X;
			if (color.A < byte.MaxValue)
			{
				HUD.inventorySelectionPos.Y = vector.Y;
			}
			else if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				HUD.inventorySelectionPos.Y += (vector.Y - HUD.inventorySelectionPos.Y) * Game1.HudTime * 20f;
			}
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				int num6 = this.materialList.Count / num2 * num;
				int num7 = (int)vector.Y;
				float persistFloat = 0f - HUD.inventorySelectionPos.Y + (float)num7;
				this.DrawScrollBarMouse(screenOffset + new Vector2(850f, 70f), ref persistFloat, 390, num6, num, (float)(int)color.A / 255f);
				Game1.pcManager.MouseWheelAdjust(ref persistFloat, num, 0f, num6);
				HUD.inventorySelectionPos.Y = Math.Min(0f - persistFloat + (float)num7, num7);
			}
			else
			{
				this.DrawScrollBar(screenOffset + new Vector2(850f, 70f), (float)(this.equipSelection / num2) / (float)Math.Max(this.materialList.Count / num2, 4), 390f, (float)(int)color.A / 255f);
			}
			for (int i = 0; i < this.materialList.Count; i++)
			{
				float num8 = HUD.inventorySelectionPos.Y + (float)(i / num2 * num);
				if (!(num8 > (float)num4) || !(num8 < (float)num5))
				{
					continue;
				}
				float num9 = 0f;
				float num10 = 1f;
				float rotation = 0f;
				float a = 0.5f;
				if (i == this.equipSelection)
				{
					if (i == Game1.inventoryManager.invSelection)
					{
						num10 = MathHelper.Clamp(2.5f, 1f, 1.5f);
					}
					num9 = posInvY;
					rotation = (float)Math.Sin(this.pulse) / 10f;
					a = MathHelper.Clamp(6.28f / posInvY, 0f, 0.5f) * HUD.equipFade;
				}
				int num11 = (int)this.materialList[i].X / 12;
				Rectangle value = new Rectangle(num11 * 60, ((int)this.materialList[i].X - num11 * 12) * 60, 60, 60);
				HUD.sprite.Draw(HUD.particlesTex[5], HUD.inventorySelectionPos + new Vector2((float)(10 + i * num) + num9 / 2f - (float)(i / num2 * num2 * num), 17 + i / num2 * num), value, new Color(0f, 0f, 0f, a), rotation, new Vector2(30f, 30f), num10 * new Vector2(1f, 0.5f), SpriteEffects.None, 0f);
				HUD.sprite.Draw(HUD.particlesTex[5], HUD.inventorySelectionPos + new Vector2(i * num - i / num2 * num2 * num, (float)(i / num2 * num) - num9 / 2f), value, color2, rotation, new Vector2(30f, 30f), num10, SpriteEffects.None, 0f);
				Color color3 = color2;
				if (this.materialList[i].Y < 1f)
				{
					color3 = Color.Red;
				}
				this.scoreDraw.Draw((int)this.materialList[i].Y, HUD.inventorySelectionPos + new Vector2(19 + i * num - i / num2 * num2 * num, 15 + Math.Abs(i / num2) * num), 0.8f, color3, ScoreDraw.Justify.Center, 0);
				if (Game1.stats.shopMaterial[(int)this.materialList[i].X] > -1)
				{
					HUD.sprite.Draw(HUD.inventoryTex, HUD.inventorySelectionPos + new Vector2(30 + i * num - i / num2 * num2 * num, -30 + Math.Abs(i / num2) * num), new Rectangle(930, 0, 40, 30), color2, 0f, new Vector2(20f, 10f), 1f, SpriteEffects.None, 0f);
				}
				if (i == this.equipSelection)
				{
					Vector2 vector2 = vector + new Vector2(-20 + i * num - i / num2 * num2 * num, -23 + i / num2 * num);
					if (color.A < 1)
					{
						this.cursorPos = vector2;
					}
					else
					{
						this.cursorPos += (vector2 - this.cursorPos) * Game1.HudTime * 20f;
					}
					if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
					{
						this.DrawCursor(this.cursorPos, 0.75f, color, flip: false);
					}
				}
			}
			Vector2 vector3 = screenOffset + new Vector2(60f, 240f);
			float num13;
			if (this.equipSelection < this.materialList.Count)
			{
				Vector2 vector4 = screenOffset + new Vector2(210f, 140f);
				int num12 = (int)this.materialList[this.equipSelection].X / 12;
				Rectangle value2 = new Rectangle(num12 * 60, ((int)this.materialList[this.equipSelection].X - num12 * 12) * 60, 60, 60);
				Game1.hud.DrawMiniBorder(vector4 - new Vector2(75f, 70f), 150, 140, color2 * 0.6f, 1f);
				HUD.sprite.Draw(HUD.particlesTex[5], vector4 + new Vector2(20f, 35f), value2, new Color(0f, 0f, 0f, 0.5f), 0f, new Vector2(30f, 30f), 1.5f * new Vector2(1f, 0.5f), SpriteEffects.None, 0f);
				HUD.sprite.Draw(HUD.particlesTex[5], vector4, value2, color2, 0f, new Vector2(30f, 30f), 1.5f, SpriteEffects.None, 0f);
				if (Game1.stats.shopMaterial[(int)this.materialList[this.equipSelection].X] > -1)
				{
					HUD.sprite.Draw(HUD.inventoryTex, vector4 + new Vector2(45f, -40f), new Rectangle(930, 0, 40, 30), color2, 0f, new Vector2(20f, 15f), 1.2f, SpriteEffects.None, 0f);
				}
				Color color4 = color2;
				if (this.materialList[this.equipSelection].Y < 1f)
				{
					color4 = Color.Red;
				}
				this.scoreDraw.Draw((int)this.materialList[this.equipSelection].Y, vector4 + new Vector2(40f, 40f), 0.8f, color4, ScoreDraw.Justify.Center, 0);
				num13 = 0.8f;
				Game1.smallText.Color = new Color(1f, 1f, 1f, HUD.itemAlpha * ((float)(int)color.A / 255f));
				Game1.smallText.DrawText(vector3, Game1.inventoryManager.itemName, num13, 300f, TextAlign.Left);
				vector3.Y += Game1.smallFont.MeasureString(Game1.inventoryManager.itemName).Y * num13;
				HUD.sprite.Draw(HUD.hudTex[2], vector3 + new Vector2(150f, -4f), new Rectangle(0, 502, 326, 18), Game1.smallText.Color, 0f, new Vector2(163f, 0f), new Vector2(1f, 0.5f), SpriteEffects.None, 0f);
				vector3.Y += 5f;
				num13 = 0.7f;
				Game1.smallText.DrawText(vector3, Game1.inventoryManager.itemInfo, num13);
				vector3.Y += Game1.smallFont.MeasureString(Game1.inventoryManager.itemInfo).Y * num13;
				vector3.Y += 10f;
				this.scoreDraw.Draw(Game1.inventoryManager.itemCost, vector3 + new Vector2(260f, 0f), num13, Game1.smallText.Color, ScoreDraw.Justify.Right, 0);
				HUD.sprite.Draw(HUD.particlesTex[2], vector3 + new Vector2(280f, 0f), new Rectangle(540 + HUD.coinAnimFrame * 20, 2110, 20, 20), Game1.smallText.Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			}
			num13 = 0.7f;
			Game1.smallText.Color = new Color(1f, 1f, 1f, (float)(int)color.A / 255f);
			Vector2 vector5 = screenOffset + new Vector2(60f, 420f);
			string text = Game1.smallText.WordWrap(Strings_HudInv.MaterialShopCatalogue, num13, 300f, TextAlign.Left);
			Game1.smallText.DrawText(vector5, text, num13);
			vector5.Y += Game1.smallFont.MeasureString(text).Y * num13;
			Game1.smallText.Color = new Color(0f, 1f, 0f, (float)(int)color.A / 255f);
			num13 = 0.65f;
			int num14 = 0;
			for (int j = 0; j < Game1.stats.shopMaterial.Length; j++)
			{
				if (Game1.stats.shopMaterial[j] > -1)
				{
					num14++;
				}
			}
			Game1.smallText.DrawText(vector5 + new Vector2(20f, 0f), Strings_HudInv.InvCat4 + " (" + num14 + "/" + Game1.stats.maxMaterials + ")", num13);
			if (this.materialList.Count > 0 && Game1.stats.shopMaterial[(int)this.materialList[this.equipSelection].X] > -1)
			{
				Game1.smallText.DrawText(vector5 + new Vector2(20f, 20f), Strings_Shop.MaterialCatalogued, num13);
			}
			if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				Game1.smallText.Color = color;
				int num15 = (int)(Game1.smallFont.MeasureString(Game1.inventoryManager.itemControls).X * 0.7f) + 20 + 40;
				this.DrawMiniBorder(screenOffset + new Vector2(636 - num15 / 2, 505f), num15, 30, color, (int)color.A);
				Game1.smallText.DrawButtonText(screenOffset + new Vector2(396f, 510f), Game1.inventoryManager.itemControls, 0.7f, Game1.inventoryManager.itemControlsButtonList, bounce: false, 480f, TextAlign.Center);
			}
		}

		public void DrawMapTarget(SpriteBatch sprite, GraphicsDevice device, RenderTarget2D navMapTarget)
		{
			float num = HUD.navDim;
			if (Game1.stats.comboTimer > 0f || Game1.cManager.challengeMode != 0 || this.inventoryState != 0)
			{
				num = 0f;
			}
			if (HUD.navAlpha > num)
			{
				HUD.navAlpha = MathHelper.Clamp(HUD.navAlpha - Game1.HudTime * 0.8f, 0.1f, num);
			}
			else if (HUD.navAlpha < num)
			{
				HUD.navAlpha = MathHelper.Clamp(HUD.navAlpha + Game1.HudTime, 0f, num);
			}
			if (!(HUD.navAlpha < 0.2f))
			{
				device.SetRenderTarget(navMapTarget);
				device.Clear(Color.Transparent);
				sprite.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
				Game1.navManager.DrawNavTarget();
				sprite.End();
				device.SetRenderTarget(null);
			}
		}

		public void DrawMap(Vector2 screenOffset, Color color, float posInvY, int type)
		{
			if (type == 0)
			{
				if (!(HUD.navAlpha < 0.2f) && this.inventoryState == InventoryState.None)
				{
					float num = MathHelper.Clamp(this.hudScale, 0.5f, 1.2f);
					Vector2 vector = new Vector2((float)Game1.screenWidth + Math.Min((float)(-this.screenLeftOffset + 75) - 176f * num, -180f * num), Math.Max(screenOffset.Y + 20f, 0f));
					vector += new Vector2(screenOffset.X, (0f - screenOffset.X) / 2f);
					float num2 = Math.Min(HUD.navAlpha * 2f, 1f);
					int num3 = (int)(112f * num);
					this.DrawMiniBorder(vector, (int)(176f * num), num3, num2, 0.5f, num3, 0.1f);
					HUD.sprite.End();
					HUD.sprite.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
					HUD.sprite.Draw(Game1.navMapTarget, vector, new Rectangle(0, 0, 176, 112), Color.White * num2, 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
					HUD.sprite.End();
					HUD.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				}
				return;
			}
			Game1.navManager.Draw(new Vector2(screenOffset.X + 465f, Game1.screenHeight / 2 + ((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse) ? 30 : 0)), 0.75f, Game1.navManager.scrollX, Game1.navManager.scrollY, 17, 9, color, 0.75f, 1.2f, background: false, entrances: false);
			HUD.sprite.Draw(HUD.inventoryTex, screenOffset + new Vector2(32f, 4f), new Rectangle(930, 0, 485, 285), color, 0f, new Vector2(3f, 14f), 2f, SpriteEffects.None, 0f);
			this.DrawBorder(screenOffset + new Vector2(44f, 16f), 838, 514, color, 0f, 0);
			Color color2 = new Color(0f, 0f, 0f, 0.5f * ((float)(int)color.A / 255f));
			HUD.sprite.Draw(HUD.hudTex[1], screenOffset + new Vector2(465f, 68f), new Rectangle(887, 20, 234, 180), color2, 0f, Vector2.Zero, new Vector2(1.2f, 1f), SpriteEffects.None, 0f);
			HUD.sprite.Draw(HUD.hudTex[1], screenOffset + new Vector2(465f, 68f), new Rectangle(887, 20, 234, 180), color2, 0f, new Vector2(234f, 0f), new Vector2(1.2f, 1f), SpriteEffects.FlipHorizontally, 0f);
			HUD.sprite.Draw(HUD.hudTex[2], screenOffset + new Vector2(465f, 60f), new Rectangle(0, 502, 326, 18), color, 0f, new Vector2(163f, 0f), new Vector2(1.5f, 1f), SpriteEffects.None, 0f);
			this.DrawMapLegend(screenOffset, color, HUD.equipFade);
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				Vector2 loc = screenOffset + new Vector2(815f, 460f);
				if (this.helpState == 0)
				{
					if (Game1.pcManager.DrawMouseButton(loc, 0.8f, color, 1, draw: true))
					{
						this.equipSelection++;
						if (this.equipSelection > 1)
						{
							this.equipSelection = 0;
							Sound.PlayCue("menu_cancel");
						}
						else
						{
							Sound.PlayCue("menu_confirm");
						}
					}
					Game1.pcManager.DrawMouseButton(loc, 0.8f, new Color(1f, 1f, 1f, 1f - HUD.equipFade), 3, draw: true);
				}
			}
			else
			{
				Game1.inventoryManager.itemControls = Game1.smallText.WordWrap((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse) ? Strings_PC.Legend : Strings_HudInv.MapControls, 0.7f, 930f, Game1.inventoryManager.itemControlsButtonList, TextAlign.Left);
				Game1.smallText.Color = color;
				int num4 = (int)(Game1.smallFont.MeasureString(Game1.inventoryManager.itemControls).X * 0.7f) + 20;
				this.DrawMiniBorder(screenOffset + new Vector2(465 - num4 / 2, 505f), num4, 30, color, (int)color.A);
				Game1.smallText.DrawButtonText(screenOffset + new Vector2(0f, 510f), Game1.inventoryManager.itemControls, 0.7f, Game1.inventoryManager.itemControlsButtonList, bounce: false, 930f, TextAlign.Center);
			}
			Game1.bigText.Color = color;
			Game1.bigText.DrawOutlineText(screenOffset + new Vector2(0f, 20f), Game1.events.regionDisplayName, 1f, 930, TextAlign.Center, fullOutline: true);
			this.DrawMapTotals(screenOffset + new Vector2(435f, 90f), 300, 1f, color, HUD.equipFade);
		}

		public void DrawMapLegend(Vector2 screenOffset, Color color, float alpha)
		{
			screenOffset.Y += ((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse) ? 20 : 0);
			Color color2 = new Color((int)color.R, (int)color.G, (int)color.B, (float)(int)color.A / 255f * alpha);
			if (color2.A <= 0)
			{
				return;
			}
			this.DrawMiniBorder(screenOffset + new Vector2(90f, 380f - alpha * 10f), 750, 100, color2, 0.75f);
			Game1.smallText.Color = color2;
			for (int i = 0; i < 6; i++)
			{
				Vector2 vector = screenOffset + new Vector2(100 + i * 250 - i / 3 * 750, 390f - alpha * 10f + (float)(i / 3 * 40));
				Game1.smallText.DrawText(vector + new Vector2(52f, 8f), Strings_HudInv.ResourceManager.GetString("MapIcon" + i), 0.75f, 0f, TextAlign.Left);
				if (i < 5)
				{
					HUD.sprite.Draw(HUD.inventoryTex, vector, new Rectangle(930 + 39 * i, 286, 39, 39), color2);
				}
				else if (i < 6)
				{
					HUD.sprite.Draw(Game1.navManager.NavTex, vector + new Vector2(36f, 36f), new Rectangle(64 * Game1.navManager.flagAnimFrame - 768 * (Game1.navManager.flagAnimFrame / 12), 96 + 48 * (Game1.navManager.flagAnimFrame / 12), 64, 48), color, 0f, new Vector2(55f, 45f), 0.75f, SpriteEffects.FlipHorizontally, 0f);
				}
			}
		}

		public void DrawMapTotals(Vector2 screenOffset, int seperation, float scale, Color color, float legend)
		{
			screenOffset.X -= seperation / 2;
			float size = 0.7f * scale;
			if (legend > 0f)
			{
				Game1.smallText.Color = color * legend;
				Game1.smallText.DrawOutlineText(screenOffset + new Vector2(-160f, 35f * scale), Strings_HudInv.MapExploredTotal, size, 400, TextAlign.Center, fullOutline: true);
				Game1.smallText.DrawOutlineText(screenOffset + new Vector2(-160 + seperation, 35f * scale), Strings_HudInv.MapItemsTotal, size, 400, TextAlign.Center, fullOutline: true);
			}
			Game1.smallText.Color = color;
			HUD.sprite.Draw(Game1.navManager.NavTex, screenOffset, new Rectangle(816, 0, 64, 48), color, 0f, new Vector2(32f, 10f), scale * 0.8f, SpriteEffects.None, 0f);
			this.scoreDraw.Draw((int)Game1.navManager.RevealMap[Game1.navManager.NavPath].GetTotalExplored(), screenOffset + new Vector2(60f * scale, 0f), scale, color, ScoreDraw.Justify.Right, 0);
			Game1.smallText.DrawText(screenOffset + new Vector2(80f * scale, 0f), "%", size);
			HUD.sprite.Draw(Game1.navManager.NavTex, screenOffset + new Vector2(seperation, 0f), new Rectangle(816, 48, 64, 48), color, 0f, new Vector2(32f, 10f), scale * 0.8f, SpriteEffects.None, 0f);
			this.scoreDraw.Draw((int)Game1.navManager.RevealMap[Game1.navManager.NavPath].GetTreasureFound(), screenOffset + new Vector2(60f * scale + (float)seperation, 0f), scale, color, ScoreDraw.Justify.Right, 0);
			Game1.smallText.DrawText(screenOffset + new Vector2(80f * scale + (float)seperation, 0f), "%", size);
		}

		public void DrawStats(Vector2 screenOffset, Color color)
		{
			Vector2 vector = screenOffset + new Vector2(120f, 35f);
			Vector2 vector2 = screenOffset + new Vector2(300f, 60f);
			Game1.smallText.Color = color;
			for (int i = 0; i < 7; i++)
			{
				Game1.smallText.DrawText(vector + new Vector2(20f, -5 + 70 * i), Strings_HudInv.ResourceManager.GetString("StatName" + i), 0.6f);
			}
			Game1.bigText.Color = color * 0.6f;
			for (int j = 0; j < 3; j++)
			{
				Game1.bigText.DrawText(vector2 + new Vector2(125f, 62 + 70 * j), "%", 0.6f);
			}
			this.DrawTimer(vector2 + new Vector2(-10f, -6f), Game1.stats.gameClock, 0.8f, color, pad: true, milli: false, 1);
			this.scoreDraw.Draw((int)Game1.stats.Completion, vector2 + new Vector2(100f, 64f), 0.8f, color, ScoreDraw.Justify.Right, 1);
			this.scoreDraw.Draw(Game1.stats.Explored, vector2 + new Vector2(100f, 134f), 0.8f, color, ScoreDraw.Justify.Right, 1);
			this.scoreDraw.Draw(Game1.stats.TreasureFound, vector2 + new Vector2(100f, 204f), 0.8f, color, ScoreDraw.Justify.Right, 1);
			this.scoreDraw.Draw(Game1.stats.longestChain, vector2 + new Vector2(100f, 274f), 0.8f, color, ScoreDraw.Justify.Right, 1);
			this.scoreDraw.Draw(Game1.stats.enemiesDefeated, vector2 + new Vector2(100f, 344f), 0.8f, color, ScoreDraw.Justify.Right, 1);
			if (HUD.equipSelection2 == 0)
			{
				this.equipSelection = 0;
				HUD.equipSelection2 = 0;
				foreach (KeyValuePair<string, RevealMap> item in Game1.navManager.RevealMap)
				{
					if (!(item.Key != "trial"))
					{
						continue;
					}
					for (int k = 0; k < item.Value.CageList.Count; k++)
					{
						HUD.equipSelection2++;
						if (item.Value.CageList[k].Stage > 0)
						{
							this.equipSelection++;
						}
					}
				}
			}
			this.scoreDraw.Draw(this.equipSelection, vector2 + new Vector2(40f, 414f), 0.8f, color, ScoreDraw.Justify.Right, 1);
			Game1.bigText.Color = color;
			Game1.bigText.DrawText(vector2 + new Vector2(65f, 406f), "/", 0.8f);
			this.scoreDraw.Draw(HUD.equipSelection2, vector2 + new Vector2(100f, 414f), 0.8f, color, ScoreDraw.Justify.Right, 1);
		}

		private void InitQuests()
		{
			int num = 400;
			int height = ((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse) ? 400 : 360);
			Game1.questManager.UpdateQuests(num - 80);
			HUD.scrollMask = new RenderTarget2D(Game1.graphics.GraphicsDevice, num, height);
			HUD.scrollSource = new RenderTarget2D(Game1.graphics.GraphicsDevice, num, height);
			this.DrawQuestScrollMask(HUD.scrollMask.Width, HUD.scrollMask.Height, 20, Game1.graphics.GraphicsDevice);
		}

		public void InitQuestMiniPrompt(string questName, int questCategory, int id)
		{
			int num = 2000 + questCategory * 2000 + id;
			this.miniPromptState = 1;
			HUD.miniPromptTime = 0.2f;
			for (int i = 0; i < this.miniPromptList.Count; i++)
			{
				if (this.miniPromptList[i].questID % 1000 == num % 1000)
				{
					this.miniPromptList.Remove(this.miniPromptList[i]);
				}
			}
			this.miniPromptList.Reverse();
			this.miniPromptList.Add(new MiniPrompt(questName, num, 8));
			this.miniPromptList.Reverse();
		}

		private void UpdateQuest()
		{
			int num = 0;
			int num2 = 0;
			float num3 = 0.9f;
			if (HUD.questPage == QuestPage.Active)
			{
				num = Game1.questManager.activeQuest.Count - 1;
				int index = Math.Max(0, Game1.questManager.activeQuest.Count - this.equipSelection - 1);
				for (int i = 0; i < Game1.questManager.activeQuest[index].stageString.Count; i++)
				{
					num2 += (int)(Game1.smallFont.MeasureString(Game1.questManager.activeQuest[index].stageString[i]).Y * num3) + 20;
				}
			}
			else if (HUD.questPage == QuestPage.Completed)
			{
				if (Game1.questManager.completedQuest.Count > 0)
				{
					num = Game1.questManager.completedQuest.Count - 1;
					int index2 = Math.Max(0, Game1.questManager.completedQuest.Count - this.equipSelection - 1);
					for (int j = 0; j < Game1.questManager.completedQuest[index2].stageString.Count; j++)
					{
						num2 += (int)(Game1.smallFont.MeasureString(Game1.questManager.completedQuest[index2].stageString[j]).Y * num3) + 20;
					}
				}
			}
			else if (HUD.questPage == QuestPage.Notes && Game1.questManager.notes.Count > 0)
			{
				num = Game1.questManager.notes.Count - 1;
				num2 = 120;
			}
			float num4 = this.equipSelection;
			if (HUD.equipCategory == 0)
			{
				if (this.KeyUp)
				{
					this.equipSelection = (int)MathHelper.Clamp(this.equipSelection - 1, 0f, num);
				}
				else if (this.KeyDown)
				{
					this.equipSelection = (int)MathHelper.Clamp(this.equipSelection + 1, 0f, num);
				}
			}
			float num5 = MathHelper.Clamp((this.equipSelection - 3) * 60, 0f, this.equipSelection * 60);
			if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				HUD.questScroll += (num5 - HUD.questScroll) * Game1.HudTime * 20f;
			}
			if (num2 == 0)
			{
				num2 = 1000;
			}
			HUD.equipFade = (float)(-num2) * num3 + 40f;
			HUD.questDescScroll = MathHelper.Clamp(HUD.questDescScroll + GamePad.GetState((PlayerIndex)Game1.currentGamePad).ThumbSticks.Right.Y * 20f, (float)(-num2) * num3 + 40f, 0f);
			if ((float)this.equipSelection != num4)
			{
				HUD.questDescScroll = 0f;
				num2 = 0;
				HUD.itemAlpha = 0f;
				Sound.PlayCue("menu_click");
			}
			if (Game1.pcManager.inputDevice != InputDevice.KeyboardOnly)
			{
				HUD.equipCategory = 0;
			}
			else
			{
				int num6 = HUD.equipCategory;
				if (this.KeyLeft)
				{
					HUD.equipCategory = 0;
				}
				else if (this.KeyRight)
				{
					HUD.equipCategory = 1;
				}
				if (num6 != HUD.equipCategory)
				{
					Sound.PlayCue("menu_click");
				}
				if (HUD.equipCategory > 0)
				{
					int num7 = 0;
					if (this.KeyUp)
					{
						num7 = 1;
					}
					else if (this.KeyDown)
					{
						num7 = -1;
					}
					if (num7 != 0)
					{
						HUD.questDescScroll = MathHelper.Clamp(HUD.questDescScroll + (float)(120 * num7), (float)(-num2) * num3 + 40f, 0f);
						Sound.PlayCue("menu_click");
					}
				}
			}
			QuestPage questPage = HUD.questPage;
			if ((Game1.pcManager.inputDevice == InputDevice.GamePad && this.KeyRightTrigger) || Game1.pcManager.KeyInvSubRight)
			{
				this.KeyRightTrigger = (Game1.pcManager.KeyInvSubRight = false);
				HUD.questPage++;
				if (HUD.questPage > QuestPage.Notes)
				{
					HUD.questPage -= 3;
				}
			}
			if ((Game1.pcManager.inputDevice == InputDevice.GamePad && this.KeyLeftTrigger) || Game1.pcManager.KeyInvSubLeft)
			{
				this.KeyLeftTrigger = (Game1.pcManager.KeyInvSubLeft = false);
				HUD.questPage--;
				if (HUD.questPage < QuestPage.Active)
				{
					HUD.questPage += 3;
				}
			}
			if (questPage != HUD.questPage)
			{
				Sound.PlayCue("menu_page_turn");
				HUD.itemAlpha = 0f;
				HUD.questScroll = (HUD.questDescScroll = (this.questScrollBar = 0f));
				this.equipSelection = (HUD.equipSelection3 = 0);
				HUD.equipCategory = 0;
				HUD.itemAlpha = 0f;
			}
			if (this.KeyCancel)
			{
				this.ExitInventory();
			}
		}

		private void DrawQuests(Vector2 screenOffset, Color color, float posInvY)
		{
			Game1.bigText.Color = color;
			Game1.bigText.DrawOutlineText(screenOffset + new Vector2(46f, 25f), Strings_HudInv.ResourceManager.GetString("QuestCat" + (int)HUD.questPage), 0.8f, 410, TextAlign.Center, fullOutline: true);
			HUD.sprite.Draw(HUD.inventoryTex, screenOffset + new Vector2(160 + 62 * (int)HUD.questPage, 70f), new Rectangle(600 + 62 * (int)HUD.questPage, 634, 62, 62), color);
			if (Game1.pcManager.inputDevice != 0)
			{
				if (this.helpState == 0)
				{
					for (int i = 0; i < 3; i++)
					{
						Vector2 vector = screenOffset + new Vector2(160 + 62 * i, 65f);
						if (new Rectangle((int)vector.X, (int)vector.Y, 62, 64).Contains((int)this.mousePos.X, (int)this.mousePos.Y))
						{
							HUD.sprite.Draw(HUD.inventoryTex, screenOffset + new Vector2(160 + 62 * i, 70f), new Rectangle(600 + 62 * i, 634, 62, 62), new Color(1f, 1f, 1f, 0.5f));
							if (Game1.pcManager.leftMouseClicked && HUD.questPage != (QuestPage)i)
							{
								Game1.pcManager.leftMouseClicked = false;
								HUD.questPage = (QuestPage)i;
								Sound.PlayCue("menu_page_turn");
								HUD.itemAlpha = 0f;
								HUD.questScroll = (HUD.questDescScroll = (this.questScrollBar = 0f));
								this.equipSelection = (HUD.equipSelection3 = 0);
								HUD.itemAlpha = 0f;
							}
						}
					}
				}
			}
			else
			{
				HUD.sprite.Draw(HUD.hudTex[0], screenOffset + new Vector2(120f - posInvY / 8f, 105f), new Rectangle(100, 140, 50, 45), color, 0f, new Vector2(25f, 30f), 0.8f, SpriteEffects.None, 0f);
				HUD.sprite.Draw(HUD.hudTex[0], screenOffset + new Vector2(380f + posInvY / 8f, 105f), new Rectangle(50, 140, 50, 45), color, 0f, new Vector2(25f, 30f), 0.8f, SpriteEffects.None, 0f);
			}
			Vector2 vector2 = screenOffset + new Vector2(100f, 180f);
			Vector2 vector3 = screenOffset + new Vector2(490f, 70f);
			if (Game1.questManager.updatingQuests)
			{
				return;
			}
			if (color.A == byte.MaxValue)
			{
				this.DrawQuestDesc(Game1.graphics.GraphicsDevice, vector3 + new Vector2(0f, 60f));
			}
			if (HUD.questPage == QuestPage.Active)
			{
				if (Game1.questManager.activeQuest.Count < 2)
				{
					HUD.sprite.Draw(HUD.inventoryTex, vector2 + new Vector2(-40f, 25f), new Rectangle(0, 634, 359, 24), Game1.bigText.Color);
				}
				for (int j = 0; j < Game1.questManager.activeQuest.Count; j++)
				{
					if (this.equipSelection == Game1.questManager.activeQuest.Count - 1 - j)
					{
						Game1.bigText.Color = Color.White;
						Game1.bigText.DrawOutlineText(vector3, Game1.questManager.activeQuest[j].Name, 0.8f, 400, TextAlign.Center, fullOutline: true);
					}
					this.GetQuestColor(j, (int)color.A);
					if (Game1.bigText.Color.A <= 0)
					{
						continue;
					}
					int num = (int)((float)((Game1.questManager.activeQuest.Count - 1) * 60 - 60 * j) - HUD.questScroll);
					Color color2 = Game1.bigText.Color;
					if (j == Game1.questManager.activeQuest.Count - 1)
					{
						color2 = new Color(1f, 0f, 1f, (int)Game1.bigText.Color.A);
					}
					HUD.sprite.Draw(HUD.inventoryTex, vector2 + new Vector2(-30f, -15 + num), new Rectangle(600, 634, 62, 62), color2, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
					if (j < Game1.questManager.activeQuest.Count - 1)
					{
						HUD.sprite.Draw(HUD.inventoryTex, vector2 + new Vector2(-40f, -35 + num), new Rectangle(0, 634, 359, 24), Game1.bigText.Color);
					}
					Game1.bigText.DrawOutlineText(vector2 + new Vector2(20f, num), Game1.questManager.activeQuest[j].Name, 0.6f, 0, TextAlign.Left, fullOutline: true);
					if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && this.helpState == 0)
					{
						Vector2 vector4 = vector2 + new Vector2(-40f, -20 + num);
						if (new Rectangle((int)vector4.X, (int)vector4.Y, 350, 54).Contains((int)this.mousePos.X, (int)this.mousePos.Y) && Game1.pcManager.leftMouseClicked && this.equipSelection != Game1.questManager.activeQuest.Count - 1 - j)
						{
							this.equipSelection = Game1.questManager.activeQuest.Count - 1 - j;
							HUD.questDescScroll = 0f;
							HUD.itemAlpha = 0f;
							Sound.PlayCue("menu_click");
							Game1.pcManager.leftMouseClicked = false;
						}
					}
				}
				if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse && Game1.questManager.activeQuest.Count > 1)
				{
					this.questScrollBar = (float)this.equipSelection / (float)Game1.questManager.activeQuest.Count * 100f;
				}
			}
			else if (HUD.questPage == QuestPage.Completed)
			{
				if (Game1.questManager.completedQuest.Count < 2)
				{
					HUD.sprite.Draw(HUD.inventoryTex, vector2 + new Vector2(-40f, 25f), new Rectangle(0, 634, 359, 24), Game1.bigText.Color);
				}
				for (int k = 0; k < Game1.questManager.completedQuest.Count; k++)
				{
					if (this.equipSelection == Game1.questManager.completedQuest.Count - 1 - k)
					{
						Game1.bigText.Color = Color.White;
						Game1.bigText.DrawOutlineText(vector3, Game1.questManager.completedQuest[k].Name, 0.8f, 400, TextAlign.Center, fullOutline: true);
					}
					this.GetQuestColor(k, (int)color.A);
					if (Game1.bigText.Color.A <= 0)
					{
						continue;
					}
					int num2 = (int)((float)((Game1.questManager.completedQuest.Count - 1) * 60 - 60 * k) - HUD.questScroll);
					HUD.sprite.Draw(HUD.inventoryTex, vector2 + new Vector2(-30f, -15 + num2), new Rectangle(662, 634, 62, 62), Game1.bigText.Color, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
					if (k < Game1.questManager.completedQuest.Count - 1)
					{
						HUD.sprite.Draw(HUD.inventoryTex, vector2 + new Vector2(-40f, -35 + num2), new Rectangle(0, 634, 359, 24), Game1.bigText.Color);
					}
					Game1.bigText.DrawOutlineText(vector2 + new Vector2(20f, num2), Game1.questManager.completedQuest[k].Name, 0.6f, 0, TextAlign.Left, fullOutline: true);
					if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && this.helpState == 0)
					{
						Vector2 vector5 = vector2 + new Vector2(-40f, -20 + num2);
						if (new Rectangle((int)vector5.X, (int)vector5.Y, 350, 54).Contains((int)this.mousePos.X, (int)this.mousePos.Y) && Game1.pcManager.leftMouseClicked && this.equipSelection != Game1.questManager.completedQuest.Count - 1 - k)
						{
							this.equipSelection = Game1.questManager.completedQuest.Count - 1 - k;
							HUD.questDescScroll = 0f;
							HUD.itemAlpha = 0f;
							Sound.PlayCue("menu_click");
							Game1.pcManager.leftMouseClicked = false;
						}
					}
				}
				if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse && Game1.questManager.completedQuest.Count > 1)
				{
					this.questScrollBar = (float)this.equipSelection / (float)Game1.questManager.completedQuest.Count * 100f;
				}
			}
			else if (HUD.questPage == QuestPage.Notes)
			{
				if (Game1.questManager.notes.Count < 2)
				{
					HUD.sprite.Draw(HUD.inventoryTex, vector2 + new Vector2(-40f, 25f), new Rectangle(0, 634, 359, 24), Game1.bigText.Color);
				}
				for (int l = 0; l < Game1.questManager.notes.Count; l++)
				{
					if (this.equipSelection == Game1.questManager.notes.Count - 1 - l)
					{
						Game1.bigText.Color = Color.White;
						Game1.bigText.DrawOutlineText(vector3, Game1.questManager.notes[l].Name, 0.8f, 400, TextAlign.Center, fullOutline: true);
					}
					this.GetQuestColor(l, (int)color.A);
					if (Game1.bigText.Color.A <= 0)
					{
						continue;
					}
					int num3 = (int)((float)((Game1.questManager.notes.Count - 1) * 60 - 60 * l) - HUD.questScroll);
					HUD.sprite.Draw(HUD.inventoryTex, vector2 + new Vector2(-30f, -15 + num3), new Rectangle(724, 634, 62, 62), Game1.bigText.Color, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
					if (l < Game1.questManager.notes.Count - 1)
					{
						HUD.sprite.Draw(HUD.inventoryTex, vector2 + new Vector2(-40f, -35 + num3), new Rectangle(0, 634, 359, 24), Game1.bigText.Color);
					}
					Game1.bigText.DrawOutlineText(vector2 + new Vector2(20f, num3), Game1.questManager.notes[l].Name, 0.6f, 0, TextAlign.Left, fullOutline: true);
					if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && this.helpState == 0)
					{
						Vector2 vector6 = vector2 + new Vector2(-40f, -20 + num3);
						if (new Rectangle((int)vector6.X, (int)vector6.Y, 350, 54).Contains((int)this.mousePos.X, (int)this.mousePos.Y) && Game1.pcManager.leftMouseClicked && this.equipSelection != Game1.questManager.notes.Count - 1 - l)
						{
							this.equipSelection = Game1.questManager.notes.Count - 1 - l;
							HUD.questDescScroll = 0f;
							HUD.itemAlpha = 0f;
							Sound.PlayCue("menu_click");
							Game1.pcManager.leftMouseClicked = false;
						}
					}
				}
				if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse && Game1.questManager.notes.Count > 1)
				{
					this.questScrollBar = (float)this.equipSelection / (float)Game1.questManager.notes.Count * 100f;
				}
			}
			if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				Vector2 vector7 = ((HUD.equipCategory == 0) ? (vector2 + new Vector2(-20f, MathHelper.Clamp(this.equipSelection * 60, 0f, 180f) + 5f)) : (vector3 + new Vector2(40f, 20f)));
				this.cursorPos += (vector7 - this.cursorPos) * Game1.HudTime * 20f;
				this.DrawCursor(this.cursorPos, 0.75f, color, flip: false);
				this.DrawScrollBar(vector2 + new Vector2(335f, 10f), this.questScrollBar / 100f, 270f, (float)(int)color.A / 255f);
				this.DrawScrollBar(vector3 + new Vector2(400f, 20f), HUD.questDescScroll / HUD.equipFade, 380f, (float)(int)color.A / 255f);
			}
			else
			{
				this.cursorPos = vector2 + new Vector2(-20f, 0f - HUD.questScroll + (float)(this.equipSelection * 60));
				if (this.cursorPos.Y > vector2.Y - 5f && this.cursorPos.Y < vector2.Y + 310f)
				{
					this.DrawCursor(this.cursorPos, 0.75f, color, flip: false);
				}
				if (this.helpState == 0)
				{
					this.DrawScrollBarMouse(vector3 + new Vector2(400f, 20f), ref HUD.questDescScroll, 380, (int)HUD.equipFade, 1, (float)(int)color.A / 255f);
					if (new Rectangle(Game1.screenWidth / 2, 0, Game1.screenWidth / 2, Game1.screenHeight).Contains((int)this.mousePos.X, (int)this.mousePos.Y))
					{
						Game1.pcManager.MouseWheelAdjust(ref HUD.questDescScroll, -120f, HUD.equipFade, 0f);
					}
					else
					{
						Game1.pcManager.MouseWheelAdjust(ref this.questScrollBar, 20f, 0f, 100f);
					}
					this.scrollBarPos = this.DrawScrollBar(vector2 + new Vector2(335f, 10f), this.questScrollBar / 100f, 270f, (float)(int)color.A / 255f);
					if (new Rectangle((int)this.scrollBarPos.X, (int)this.scrollBarPos.Y, 54, (int)this.scrollBarPos.Z).Contains((int)this.mousePos.X, (int)this.mousePos.Y) && Game1.pcManager.IsMouseLeftHeld())
					{
						float num4 = MathHelper.Clamp(this.mousePos.Y - (vector2.Y + 10f), 0f, 270f) / 270f;
						this.questScrollBar = 100f * num4;
					}
					switch (HUD.questPage)
					{
					case QuestPage.Active:
						if (Game1.questManager.activeQuest.Count > 1)
						{
							int num9 = (int)Math.Min((float)Game1.questManager.activeQuest.Count * this.questScrollBar / 100f, Game1.questManager.activeQuest.Count - 1);
							float num10 = MathHelper.Clamp(num9 * 60, 0f, num9 * 60);
							HUD.questScroll += (num10 - HUD.questScroll) * Game1.HudTime * 20f;
						}
						else
						{
							this.questScrollBar = 0f;
						}
						break;
					case QuestPage.Completed:
						if (Game1.questManager.completedQuest.Count > 1)
						{
							int num7 = (int)Math.Min((float)Game1.questManager.completedQuest.Count * this.questScrollBar / 100f, Game1.questManager.completedQuest.Count - 1);
							float num8 = MathHelper.Clamp(num7 * 60, 0f, num7 * 60);
							HUD.questScroll += (num8 - HUD.questScroll) * Game1.HudTime * 20f;
						}
						else
						{
							this.questScrollBar = 0f;
						}
						break;
					case QuestPage.Notes:
						if (Game1.questManager.notes.Count > 1)
						{
							int num5 = (int)Math.Min((float)Game1.questManager.notes.Count * this.questScrollBar / 100f, Game1.questManager.notes.Count - 1);
							float num6 = MathHelper.Clamp(num5 * 60, 0f, num5 * 60);
							HUD.questScroll += (num6 - HUD.questScroll) * Game1.HudTime * 20f;
						}
						else
						{
							this.questScrollBar = 0f;
						}
						break;
					}
				}
			}
			if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				Game1.inventoryManager.itemControls = Game1.smallText.WordWrap((Game1.pcManager.inputDevice == InputDevice.GamePad) ? Strings_HudInv.QuestControls : Strings_PC.QuestControls, 0.7f, 930f, Game1.inventoryManager.itemControlsButtonList, TextAlign.Left);
				Game1.smallText.Color = color;
				int num11 = (int)(Game1.smallFont.MeasureString(Game1.inventoryManager.itemControls).X * 0.7f) + 20;
				this.DrawMiniBorder(screenOffset + new Vector2(465 - num11 / 2, 520f), num11, 30, color, (int)color.A);
				Game1.smallText.DrawButtonText(screenOffset + new Vector2(465 - num11 / 2, 525f), Game1.inventoryManager.itemControls, 0.7f, Game1.inventoryManager.itemControlsButtonList, bounce: false, num11, TextAlign.Center);
			}
		}

		public void DrawQuestScrollSource(GraphicsDevice graphicsDevice)
		{
			if (HUD.inventoryTex != null && !HUD.inventoryTex.IsDisposed)
			{
				graphicsDevice.SetRenderTarget(HUD.scrollSource);
				graphicsDevice.Clear(Color.Black);
				HUD.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				Color color = new Color(1f, 1f, 1f, HUD.itemAlpha);
				Game1.smallText.Color = color;
				float textSize = 0.8f;
				HUD.sprite.Draw(HUD.inventoryTex, Vector2.Zero, new Rectangle(490, 140, 1000, 1000), Color.White);
				if (HUD.questPage == QuestPage.Active)
				{
					this.DrawQuestListDescSource(Game1.questManager.activeQuest, Game1.questManager.activeQuest.Count - this.equipSelection, textSize);
				}
				else if (HUD.questPage == QuestPage.Completed)
				{
					this.DrawQuestListDescSource(Game1.questManager.completedQuest, Game1.questManager.completedQuest.Count - this.equipSelection, textSize);
				}
				else
				{
					this.DrawNoteDescSource(Game1.questManager.notes, Game1.questManager.notes.Count - 1 - this.equipSelection, textSize);
				}
				HUD.sprite.End();
				graphicsDevice.SetRenderTarget(null);
				this.scrollSourceTexture = HUD.scrollSource;
			}
		}

		private void DrawQuestScrollMask(int width, int height, int fadeHeight, GraphicsDevice graphicsDevice)
		{
			graphicsDevice.SetRenderTarget(HUD.scrollMask);
			graphicsDevice.Clear(Color.Black);
			HUD.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			HUD.sprite.Draw(HUD.particlesTex[1], new Rectangle(-50, -2, width + 100, fadeHeight + 2), new Rectangle(1794, 130, 29, 29), Color.White);
			HUD.sprite.Draw(HUD.particlesTex[1], new Rectangle(-50, height - fadeHeight - 2, width + 100, fadeHeight + 2), new Rectangle(1794, 130, 29, 29), Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
			HUD.sprite.Draw(HUD.nullTex, new Rectangle(0, fadeHeight - 2, width, height - fadeHeight * 2 + 4), Color.White);
			HUD.sprite.End();
			graphicsDevice.SetRenderTarget(null);
			this.scrollMaskTexture = HUD.scrollMask;
		}

		private void DrawQuestDesc(GraphicsDevice graphicsDevice, Vector2 pos)
		{
			if (this.scrollMaskTexture != null && this.scrollSourceTexture != null)
			{
				HUD.sprite.End();
				Game1.maskEffect.Parameters["MaskTexture"].SetValue(this.scrollMaskTexture);
				HUD.sprite.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
				Game1.maskEffect.CurrentTechnique.Passes[0].Apply();
				HUD.sprite.Draw(this.scrollSourceTexture, new Rectangle((int)pos.X, (int)pos.Y, HUD.scrollSource.Width, HUD.scrollSource.Height), Color.White);
				HUD.sprite.End();
				HUD.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			}
		}

		private void GetQuestColor(int a, float alpha)
		{
			int num = 0;
			int num2 = 0;
			if (HUD.questPage == QuestPage.Active)
			{
				num = Game1.questManager.activeQuest.Count - 1;
				num2 = 60 * a;
			}
			else if (HUD.questPage == QuestPage.Completed)
			{
				num = Game1.questManager.completedQuest.Count - 1;
				num2 = 60 * a;
			}
			else if (HUD.questPage == QuestPage.Notes)
			{
				num = Game1.questManager.notes.Count - 1;
				num2 = 60 * a;
			}
			Color color = ((HUD.questScroll > (float)(num * 60 - 60 * a)) ? new Color(1f, 1f, 1f, 1f - (HUD.questScroll - (float)(num * 60 - 60 * a)) / 32f) : ((!(HUD.questScroll + 290f < (float)(num * 60 - num2))) ? new Color(1f, 1f, 1f, alpha / 255f) : new Color(1f, 1f, 1f, alpha / 255f - ((float)(num * 60 - num2) - (HUD.questScroll + 290f)) / 32f)));
			Color color4 = (Game1.bigText.Color = (Game1.smallText.Color = color));
		}

		private void DrawNoteDescSource(List<Notes> notes, int a, float textSize)
		{
			if (notes.Count >= 1 && a >= 0 && !Game1.questManager.updatingQuests)
			{
				Game1.smallText.DrawOutlineText(new Vector2(20f, HUD.questDescScroll + 20f), notes[a].Description, textSize, 2000, TextAlign.Left, fullOutline: true);
				Game1.smallText.DrawText(new Vector2(20f, HUD.questDescScroll + 20f), notes[a].Description, textSize, 2000f, TextAlign.Left);
			}
		}

		private void DrawQuestListDescSource(List<Quest> questList, int a, float textSize)
		{
			a--;
			if (questList.Count < 1 || a < 0 || Game1.questManager.updatingQuests)
			{
				return;
			}
			int count = questList[a].stageString.Count;
			if (count < 1)
			{
				Game1.questManager.UpdateQuests(320);
			}
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				if (questList[a].stageString[i] == null)
				{
					continue;
				}
				int num2 = (int)(HUD.questDescScroll + (float)num) + 20;
				int num3 = (int)(Game1.smallFont.MeasureString(questList[a].stageString[i]).Y * textSize) + 20;
				int num4 = ((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse) ? 400 : 360);
				if (num2 > -num3 && num2 < num4)
				{
					Game1.smallText.DrawOutlineText(new Vector2(60f, num2), questList[a].stageString[i], textSize, 2000, TextAlign.Left, fullOutline: true);
					Game1.smallText.DrawText(new Vector2(60f, num2), questList[a].stageString[i], textSize, 2000f, TextAlign.Left);
					if (i == 0 && HUD.questPage == QuestPage.Active)
					{
						HUD.sprite.Draw(HUD.inventoryTex, new Vector2(4f, num2 - 13), new Rectangle(502, 634, 48, 60), Game1.smallText.Color);
					}
					else
					{
						HUD.sprite.Draw(HUD.inventoryTex, new Vector2(4f, num2 - 13), new Rectangle(550, 634, 48, 60), Game1.smallText.Color);
					}
				}
				num += num3;
			}
		}

		public void InitBoss(Character[] c, string name)
		{
			HUD.bossLifeBarPercent = 0f;
			HUD.bossLifeBarPos = 0f;
			this.bossMaxLife = 0f;
			for (int i = 0; i < c.Length; i++)
			{
				if (c[i].Exists == CharExists.Exists && c[i].Name == name)
				{
					this.bossMaxLife += c[i].MaxHP;
				}
			}
			this.bossLife = this.bossMaxLife;
			this.bossName = name;
			this.inBoss = true;
		}

		private void MoveBossLifeBar()
		{
			if (this.bossLife < HUD.bossLifeBarPercent - 0.001f)
			{
				HUD.bossLifeBarPercent -= Game1.HudTime * (HUD.barSpeed * Math.Abs(this.bossLife - HUD.bossLifeBarPercent));
			}
			else if (this.bossLife > HUD.bossLifeBarPercent + 0.001f)
			{
				HUD.bossLifeBarPercent += Game1.HudTime * (HUD.barSpeed * this.bossLife - HUD.bossLifeBarPercent) * 0.075f;
			}
			HUD.bossLifeBarPos = HUD.bossLifeBarPercent / this.bossMaxLife;
			if (HUD.bossLifeBarPos < 0.04f)
			{
				HUD.bossLifeBarPos = 0.04f;
			}
		}

		private void UpdateBoss(Character[] c, ParticleManager pMan)
		{
			if (Game1.longSkipFrame > 3)
			{
				this.bossLife = 0f;
				for (int i = 0; i < c.Length; i++)
				{
					if (c[i].Exists == CharExists.Exists && c[i].Name == this.bossName)
					{
						this.bossLife += c[i].HP;
					}
				}
			}
			if (this.dialogueState == DialogueState.Inactive && this.eventLeftOffset == 0f)
			{
				this.MoveBossLifeBar();
			}
			if (!(this.bossLife <= 0f))
			{
				return;
			}
			this.inBoss = false;
			this.bossMaxLife = 0f;
			this.bossName = string.Empty;
			if (!Game1.events.anyEvent)
			{
				Sound.PlayCue("combo_break");
			}
			for (int j = 0; j < 2; j++)
			{
				for (int k = 0; k < 18; k++)
				{
					pMan.AddComboBreak(new Vector2(Rand.GetRandomFloat(Game1.screenWidth / 2 - 305, Game1.screenWidth / 2 + 305), Game1.screenHeight - this.screenTopOffset / 2), 0.5f, k, 9);
				}
			}
		}

		private void DrawBossLifeBar(float topOffset)
		{
			Color color = new Color(1f, 3f * (HUD.bossLifeBarPercent / this.bossMaxLife), 0f, 1f);
			Vector2 vector = new Vector2(Game1.screenWidth / 2 - 305, MathHelper.Clamp((float)Game1.screenHeight - topOffset * 2f, (float)Game1.screenHeight * 0.8f, Game1.screenHeight - 30));
			HUD.sprite.Draw(HUD.hudTex[0], new Rectangle((int)vector.X + 62, (int)vector.Y - 21, 486, 45), new Rectangle(294, 140, 18, 45), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			HUD.sprite.Draw(HUD.hudTex[0], vector + new Vector2(0f, -21.5f), new Rectangle(310, 140, 62, 45), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
			HUD.sprite.Draw(HUD.hudTex[0], vector + new Vector2(548f, -21.5f), new Rectangle(310, 140, 62, 45), Color.White);
			this.DrawBar(vector + new Vector2(20f, -10f), HUD.bossLifeBarPos, color, new Vector2(1.8f, 1f), backGround: false);
		}

		private void DrawCineFade()
		{
			if (HUD.cineOffset > 0f)
			{
				int num = 112 - (int)HUD.cineOffset;
				float num2 = 1f - (float)num / 100f;
				Color color = new Color(0f, 0f, 0f, num2);
				Color color2 = new Color(1f, 1f, 1f, (1f - HUD.cineOffset / 100f) * 8f * num2);
				num = 12;
				HUD.sprite.Draw(HUD.particlesTex[1], new Rectangle(0, (int)(-70f * Game1.hiDefScaleOffset) - num, Game1.screenWidth, (int)(150f * Game1.hiDefScaleOffset)), new Rectangle(1835, 140, 19, 20), color, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
				HUD.sprite.Draw(HUD.hudTex[2], new Vector2((float)Game1.screenWidth * (1f - num2), (int)(80f * Game1.hiDefScaleOffset) - num), new Rectangle(0, 502, 326, 18), color2, 0f, new Vector2(163f, 9f), new Vector2((float)Game1.screenWidth / 280f, 0.75f), SpriteEffects.None, 0f);
				HUD.sprite.Draw(HUD.particlesTex[1], new Rectangle(0, Game1.screenHeight - (int)(80f * Game1.hiDefScaleOffset) + num, Game1.screenWidth, (int)(150f * Game1.hiDefScaleOffset)), new Rectangle(1835, 140, 19, 20), color);
				HUD.sprite.Draw(HUD.hudTex[2], new Vector2((float)Game1.screenWidth * num2, Game1.screenHeight - (int)(80f * Game1.hiDefScaleOffset) + num), new Rectangle(0, 502, 326, 18), color2, 0f, new Vector2(163f, 9f), new Vector2((float)Game1.screenWidth / 280f, 0.75f), SpriteEffects.None, 0f);
				if (HUD.eventIcon > 0f)
				{
					HUD.sprite.Draw(HUD.hudTex[0], new Vector2((float)this.screenLeftOffset - 71f * this.hudScale, (float)this.screenTopOffset - 38f * this.hudScale), new Rectangle(212, 185, 83, 81), new Color(1f, 1f, 1f, HUD.eventIcon * 6f));
				}
			}
			if (Game1.events.screenFade.A > 0)
			{
				HUD.sprite.Draw(HUD.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Game1.events.screenFade);
			}
			if (Game1.events.regionIntroStage > 0)
			{
				HUD.sprite.Draw(HUD.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, Game1.events.regionIntroFade));
			}
		}

		private void DrawPause(ParticleManager pMan)
		{
			if (this.pauseImageOffset != 700f)
			{
				if (this.inventoryState != 0)
				{
					HUD.sprite.Draw(HUD.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, 0.5f * this.screenDimAlpha));
				}
				if (Game1.cutscene.SceneType != 0)
				{
					HUD.sprite.Draw(HUD.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.Black);
				}
				Color color = ((!this.isPaused || this.inventoryState != 0) ? new Color(0.5f, 0.5f, 0.5f, 1f - this.pauseImageOffset / 700f) : new Color(1f, 1f, 1f, 1f - this.pauseImageOffset / 700f));
				HUD.sprite.Draw(HUD.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, 0.25f * ((float)(int)color.A / 255f)));
				HUD.sprite.Draw(HUD.hudTex[1], new Vector2((0f - this.pauseImageOffset) / 4f, Game1.screenHeight / 2 - 360), new Rectangle(0, 530, 640, 720), color);
				HUD.sprite.Draw(HUD.hudTex[1], new Vector2((float)(Game1.screenWidth - 550) + this.pauseImageOffset / 4f, Game1.screenHeight / 2 - 360), new Rectangle(640, 530, 550, 720), color);
				if (Game1.screenHeight / 2 - 360 > 0)
				{
					HUD.sprite.Draw(HUD.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight / 2 - 360), new Color(0f, 0f, 0f, 1f - this.pauseImageOffset / 700f));
					HUD.sprite.Draw(HUD.nullTex, new Rectangle(0, Game1.screenHeight / 2 + 360, Game1.screenWidth, Game1.screenHeight / 2 - 360), new Color(0f, 0f, 0f, 1f - this.pauseImageOffset / 700f));
				}
				if (Rand.GetRandomInt(0, 80) == 0)
				{
					pMan.AddMenuLeaf(new Vector2(Rand.GetRandomFloat(-50f, Game1.screenWidth + 50), Rand.GetRandomFloat(-200f, Game1.screenHeight / 2)), Rand.GetRandomVector2(100f, 300f, 100f, 300f), 90f, Rand.GetRandomFloat(0.2f, 3f), 9);
				}
			}
			if (Game1.gameMode == Game1.GameModes.Menu)
			{
				if (this.inventoryState == InventoryState.None && Game1.menu.prompt == promptDialogue.None && Rand.GetRandomInt(0, 15) == 0)
				{
					pMan.AddMenuLeaf(new Vector2(Rand.GetRandomFloat(-50f, Game1.screenWidth + 50), Rand.GetRandomFloat(-200f, Game1.screenHeight / 2)), Rand.GetRandomVector2(100f, 300f, 100f, 300f), 90f, Rand.GetRandomFloat(0.5f, 4f), 9);
				}
				Game1.menu.DrawPauseMenu(1f - this.pauseImageOffset / 700f);
			}
		}

		private void DrawStatDetails()
		{
			if (this.character[0].LifeBarPercent != (float)this.character[0].HP)
			{
				this.MoveLifeBar(0);
			}
			if (!this.hudDetails || Game1.gameMode != Game1.GameModes.Game)
			{
				return;
			}
			MathHelper.Clamp(this.hudScale, 1f, 20f);
			for (int i = 1; i < this.character.Length; i++)
			{
				if (this.character[i].Exists == CharExists.Exists && this.character[i].renderable)
				{
					Vector2 vector = this.character[i].Location * Game1.worldScale - Game1.Scroll;
					if (this.character[i].NPC == NPCType.None)
					{
						if (this.character[i].DownTime > 0f)
						{
							HUD.sprite.Draw(HUD.particlesTex[2], vector - new Vector2(0f, (float)this.character[i].DefaultHeight * Game1.worldScale), new Rectangle(3277 + 50 * HUD.dizzyAnimFrame, 2011, 50, 65), new Color(1f, 1f, 1f, (this.character[i].DownTime - 0.25f) * 2f), 1.57f, new Vector2(25f, 32f), new Vector2(0.75f, 2f), SpriteEffects.None, 0f);
						}
						if (this.character[i].LifeBarPercent != (float)this.character[i].HP)
						{
							this.MoveLifeBar(i);
						}
						if (this.character[i].HPLossFrame > 0f && this.character[i].Team == Team.Enemy)
						{
							HUD.sprite.Draw(HUD.hudTex[0], vector - new Vector2(53f, 0f), new Rectangle(0, 185, 106, 20), new Color(1f, 1f, 1f, this.character[i].HPLossFrame * 3f));
							HUD.sprite.Draw(HUD.hudTex[0], vector - new Vector2(50.5f, 0f), new Rectangle(108, 185, (int)(101f * this.character[i].LifeBarPercent / (float)this.character[i].MaxHP), 20), new Color(1f, 3f * (this.character[i].LifeBarPercent / (float)this.character[i].MaxHP), 0f, this.character[i].HPLossFrame * 3f));
						}
					}
					else if (this.character[i].QuestGiver && Game1.blurScene < 0f && !Game1.events.anyEvent)
					{
						float num = (float)Math.Cos(this.pulse);
						float num2 = (1f - (this.character[i].Location - this.character[0].Location).Length() / 500f) * 4f;
						Vector2 bubblePos = vector + new Vector2(-20f, (float)(-this.character[i].Height) * Game1.worldScale - 120f * Game1.hiDefScaleOffset - num * 10f);
						bubblePos = this.DrawTextBubble(bubblePos, 100f, 0f, Math.Min(num2, 0.75f));
						HUD.sprite.Draw(HUD.particlesTex[4], bubblePos + new Vector2(52f, 30f), new Rectangle(360, 0, 60, 60), Color.White * num2, 0f, new Vector2(30f, 30f), 0.8f, SpriteEffects.None, 0f);
					}
				}
				if (!(Game1.stats.attackBonusTime > 0f) && !(Game1.stats.defenseBonusTime > 0f))
				{
					continue;
				}
				float num3 = (float)Math.Abs(Math.Cos(this.pulse) * 40.0);
				int num4 = 0;
				if (Game1.stats.attackBonusTime > 0f && Game1.stats.defenseBonusTime > 0f)
				{
					num4 = 16;
				}
				if (Game1.stats.attackBonusTime > 0f)
				{
					if (Game1.stats.attackBonusTime < 1f)
					{
						HUD.attackIconAlpha /= 1.09f;
					}
					else
					{
						HUD.attackIconAlpha = 1f;
					}
					HUD.sprite.Draw(HUD.particlesTex[1], this.character[0].Location * Game1.worldScale - Game1.Scroll + new Vector2(-num4, -250f * Game1.worldScale - num3), new Rectangle(1344, 128, 32, 32), new Color(1f, 1f, 1f, HUD.attackIconAlpha * (num3 / 40f)), 0f, new Vector2(16f, 16f), 1f, SpriteEffects.None, 0f);
				}
				if (Game1.stats.defenseBonusTime > 0f)
				{
					if (Game1.stats.defenseBonusTime < 1f)
					{
						HUD.defenseIconAlpha /= 1.09f;
					}
					else
					{
						HUD.defenseIconAlpha = 1f;
					}
					HUD.sprite.Draw(HUD.particlesTex[1], this.character[0].Location * Game1.worldScale - Game1.Scroll + new Vector2(num4, -250f * Game1.worldScale - num3), new Rectangle(1376, 128, 32, 32), new Color(1f, 1f, 1f, HUD.defenseIconAlpha * (num3 / 40f)), 0f, new Vector2(16f, 16f), 1f, SpriteEffects.None, 0f);
				}
			}
			if (!this.map.mapAssetsLoaded)
			{
				return;
			}
			for (int j = 0; j < this.map.doorRegions.Count; j++)
			{
				if (this.map.doorRegions[j] != null)
				{
					Vector2 vector2 = new Vector2(this.map.doorRegions[j].Region.X + this.map.doorRegions[j].Region.Width / 2, this.map.doorRegions[j].Region.Y);
					float num5 = Math.Min((1f - (vector2 - this.character[0].Location).Length() / 800f) * 4f, 1f);
					if (this.regionIntroState == 0 && !Game1.events.anyEvent)
					{
						HUD.sprite.Draw(HUD.particlesTex[4], vector2 * Game1.worldScale - Game1.Scroll, new Rectangle(360, 360, 60, 60), Color.White * num5 * (float)Math.Abs(Math.Cos(this.pulse)), 0f, new Vector2(30f, 30f), 1f, SpriteEffects.None, 0f);
					}
				}
			}
		}

		private void DrawMapFade()
		{
			if (this.map.GetTransVal() > 0f)
			{
				HUD.sprite.Draw(HUD.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(new Vector4(0f, 0f, 0f, this.map.GetTransVal() + 0.1f)));
			}
			if (!Game1.map.mapAssetsLoaded)
			{
				HUD.sprite.Draw(HUD.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.Black);
				Game1.DrawLoad(HUD.sprite, new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2));
			}
		}

		public void DrawMiniBorder(Vector2 orig, int width, int height, float alpha, float bgAlpha, int bottom, float glow)
		{
			Vector2 vector = orig + new Vector2(width / 2, -5f);
			Color color = new Color(0f, 0f, 0f, alpha * bgAlpha);
			Vector2 scale = new Vector2((float)width / 260f, (float)height / 50f);
			Rectangle value = ((bottom != 0) ? new Rectangle(887, 20, 234, (int)((float)bottom / scale.Y)) : new Rectangle(887, 20, 234, 180));
			HUD.sprite.Draw(HUD.hudTex[1], vector + new Vector2(0f, 6f), value, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			HUD.sprite.Draw(HUD.hudTex[1], vector + new Vector2(0f, 6f), value, color, 0f, new Vector2(234f, 0f), scale, SpriteEffects.FlipHorizontally, 0f);
			HUD.sprite.Draw(HUD.hudTex[2], vector, new Rectangle(0, 502, 326, 18), new Color(1f, 1f, 1f, alpha), 0f, new Vector2(163f, 0f), new Vector2((float)width / 280f, 0.5f), SpriteEffects.None, 0f);
			if (bottom > 0)
			{
				if (glow > 0f && HUD.miniBorderGlowPos < 1f)
				{
					float num = (float)Math.Sin(6.28 * (double)(HUD.miniBorderGlowPos / 2f / 1f)) * alpha * glow;
					Vector2 scale2 = new Vector2(Math.Max(HUD.miniBorderGlowPos * 2f, 0.2f), 1f) * bottom / 100f;
					HUD.sprite.Draw(HUD.hudTex[1], vector + new Vector2((float)(-width / 2) + (float)width * HUD.miniBorderGlowPos, 5f), new Rectangle(580, 200, 180, 100), new Color(1f, 1f, 1f, num * 2f), 0f, new Vector2(90f, 0f), scale2, SpriteEffects.None, 0f);
					HUD.sprite.Draw(HUD.hudTex[1], vector + new Vector2((float)(width / 4) - (float)(width / 2) * HUD.miniBorderGlowPos, 5f), new Rectangle(760, 200, 120, 100), new Color(1f, 1f, 1f, num), 0f, new Vector2(80f, 0f), scale2, SpriteEffects.None, 0f);
				}
				HUD.sprite.Draw(HUD.hudTex[2], vector + new Vector2(0f, bottom), new Rectangle(0, 502, 326, 18), new Color(1f, 1f, 1f, alpha), 0f, new Vector2(163f, 0f), new Vector2((float)width / 280f, 0.5f), SpriteEffects.None, 0f);
			}
		}

		public void DrawMiniBorder(Vector2 orig, int width, int height, Color color, float bgAlpha)
		{
			int num = (int)orig.X - 14;
			int num2 = (int)orig.Y - 14;
			int width2 = Math.Min(width, 570);
			int height2 = Math.Min(height, 520);
			HUD.sprite.Draw(HUD.hudTex[1], new Rectangle((int)orig.X, (int)orig.Y, Math.Max(width, 126), Math.Max(height, 40)), new Rectangle(0, 0, width2, height2), new Color((float)(int)color.R / 255f, (float)(int)color.B / 255f, (float)(int)color.G / 255f, (float)(int)color.A / 255f * bgAlpha));
			HUD.sprite.Draw(HUD.hudTex[2], new Vector2(num, num2), new Rectangle(0, 422, 74, 30), color);
			HUD.sprite.Draw(HUD.hudTex[2], new Vector2(num + Math.Max(width - 50, 74), num2), new Rectangle(84, 422, 86, 30), color);
			HUD.sprite.Draw(HUD.hudTex[2], new Vector2(num, num2 + Math.Max(height - 4, 30)), new Rectangle(0, 462, 74, 40), color);
			HUD.sprite.Draw(HUD.hudTex[2], new Vector2(num + Math.Max(width - 50, 74), num2 + Math.Max(height - 4, 30)), new Rectangle(84, 462, 86, 40), color);
			HUD.sprite.Draw(HUD.hudTex[2], new Rectangle(num, num2 + 30, 25, height - 34), new Rectangle(0, 452, 25, 10), color);
			HUD.sprite.Draw(HUD.hudTex[2], new Rectangle(Math.Max(num + width + 11, num + 135), num2 + 30, 25, height - 34), new Rectangle(145, 452, 25, 10), color);
			HUD.sprite.Draw(HUD.hudTex[2], new Rectangle(num + 74, num2, width - 124, 30), new Rectangle(74, 422, 10, 30), color);
			HUD.sprite.Draw(HUD.hudTex[2], new Rectangle(num + 74, num2 + Math.Max(height - 4, 30), width - 124, 40), new Rectangle(74, 462, 10, 40), color);
		}

		public void DrawBorder(Vector2 orig, int width, int height, Color color, float bgAlpha, int dividerLoc)
		{
			int num = (int)orig.X - 26;
			int num2 = (int)orig.Y - 24;
			int width2;
			int height2;
			if (height < 230)
			{
				width2 = (int)MathHelper.Clamp(width, 0f, 570f);
				height2 = (int)MathHelper.Clamp(height, 0f, 520f);
				HUD.sprite.Draw(HUD.hudTex[1], new Rectangle((int)orig.X, (int)orig.Y, (int)MathHelper.Clamp(width, 265f, width), (int)MathHelper.Clamp(height, 80f, height)), new Rectangle(0, 0, width2, height2), new Color((float)(int)color.R / 255f, (float)(int)color.B / 255f, (float)(int)color.G / 255f, (float)(int)color.A / 255f * bgAlpha));
				HUD.sprite.Draw(HUD.hudTex[2], new Vector2(num, num2), new Rectangle(0, 291, 159, 63), color);
				HUD.sprite.Draw(HUD.hudTex[2], new Vector2((float)num + MathHelper.Clamp(width - 105, 159f, width), num2), new Rectangle(167, 291, 159, 63), color);
				HUD.sprite.Draw(HUD.hudTex[2], new Vector2(num, (float)num2 + MathHelper.Clamp(height - 14, 63f, height)), new Rectangle(0, 354, 159, 68), color);
				HUD.sprite.Draw(HUD.hudTex[2], new Vector2((float)num + MathHelper.Clamp(width - 105, 159f, width), (float)num2 + MathHelper.Clamp(height - 14, 63f, height)), new Rectangle(167, 354, 159, 68), color);
				HUD.sprite.Draw(HUD.hudTex[2], new Rectangle(num, num2 + 63, 40, height - 77), new Rectangle(0, 139, 40, 10), color);
				HUD.sprite.Draw(HUD.hudTex[2], new Rectangle((int)MathHelper.Clamp(num + width + 14, num + 278, 10000f), num2 + 63, 40, height - 77), new Rectangle(286, 139, 40, 10), color);
				HUD.sprite.Draw(HUD.hudTex[2], new Rectangle(num + 159, num2, width - 264, 40), new Rectangle(157, 0, 10, 40), color);
				HUD.sprite.Draw(HUD.hudTex[2], new Rectangle(num + 159, num2 + (int)MathHelper.Clamp(height + 14, 91f, 10000f), width - 264, 40), new Rectangle(157, 251, 10, 40), color);
				return;
			}
			width2 = (int)MathHelper.Clamp(width, 0f, 570f);
			height2 = (int)MathHelper.Clamp(height, 0f, 520f);
			HUD.sprite.Draw(HUD.hudTex[1], new Rectangle((int)orig.X, (int)orig.Y, (int)MathHelper.Clamp(width, 265f, width), (int)MathHelper.Clamp(height, 227f, height)), new Rectangle(0, 0, width2, height2), new Color((float)(int)color.R / 255f, (float)(int)color.B / 255f, (float)(int)color.G / 255f, (float)(int)color.A / 255f * bgAlpha));
			HUD.sprite.Draw(HUD.hudTex[2], new Vector2(num, num2), new Rectangle(0, 0, 159, 139), color);
			HUD.sprite.Draw(HUD.hudTex[2], new Vector2((float)num + MathHelper.Clamp(width - 105, 159f, width), num2), new Rectangle(167, 0, 159, 139), color);
			HUD.sprite.Draw(HUD.hudTex[2], new Vector2(num, (float)num2 + MathHelper.Clamp(height - 88, 139f, height)), new Rectangle(0, 149, 159, 139), color);
			HUD.sprite.Draw(HUD.hudTex[2], new Vector2((float)num + MathHelper.Clamp(width - 105, 159f, width), (float)num2 + MathHelper.Clamp(height - 88, 139f, height)), new Rectangle(167, 149, 159, 139), color);
			HUD.sprite.Draw(HUD.hudTex[2], new Rectangle(num, num2 + 139, 40, height - 227), new Rectangle(0, 139, 40, 10), color);
			HUD.sprite.Draw(HUD.hudTex[2], new Rectangle((int)MathHelper.Clamp(num + width + 14, num + 278, 10000f), num2 + 139, 40, height - 227), new Rectangle(286, 139, 40, 10), color);
			HUD.sprite.Draw(HUD.hudTex[2], new Rectangle(num + 159, num2, width - 264, 40), new Rectangle(157, 0, 10, 40), color);
			HUD.sprite.Draw(HUD.hudTex[2], new Rectangle(num + 159, num2 + (int)MathHelper.Clamp(height + 14, 241f, 10000f), width - 264, 40), new Rectangle(157, 251, 10, 40), color);
			if (dividerLoc > 0)
			{
				HUD.sprite.Draw(HUD.hudTex[2], new Vector2(num + 11, num2 + dividerLoc), new Rectangle(170, 424, 20, 30), color);
				HUD.sprite.Draw(HUD.hudTex[2], new Vector2(MathHelper.Clamp(num + width + 22, num + 286, 10000f), num2 + dividerLoc), new Rectangle(200, 424, 21, 30), color);
				HUD.sprite.Draw(HUD.hudTex[2], new Rectangle(num + 31, num2 + dividerLoc, (int)MathHelper.Clamp(width - 9, 259f, 10000f), 30), new Rectangle(190, 424, 10, 30), color);
			}
		}

		public void DrawCursor(Vector2 orig, float scale, Color color, bool flip)
		{
			HUD.sprite.Draw(HUD.particlesTex[2], orig + new Vector2(5f, 10f) * scale, new Rectangle(80 * this.cursorFrame, 2920, 80, 50), new Color(0f, 0f, 0f, (float)(int)color.A / 255f * 0.5f), 0f, new Vector2(72f, 25f), scale, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
			HUD.sprite.Draw(HUD.particlesTex[2], orig, new Rectangle(80 * this.cursorFrame, 2920, 80, 50), color, 0f, new Vector2(72f, 25f), scale, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
		}

		public Vector3 DrawScrollBar(Vector2 pos, float percent, float height, float alpha)
		{
			float num = height * Math.Min(percent, 1f);
			Color color = new Color(1f, 1f, 1f, alpha);
			HUD.sprite.Draw(HUD.hudTex[2], new Vector2(pos.X, pos.Y + 5f), new Rectangle(175, 471, 5, 10), color, 0f, Vector2.Zero, new Vector2(1f, height / 10f), SpriteEffects.None, 0f);
			HUD.sprite.Draw(HUD.hudTex[2], new Vector2(pos.X - 5f, pos.Y), new Rectangle(170, 456, 14, 15), color);
			HUD.sprite.Draw(HUD.hudTex[2], new Vector2(pos.X - 5f, pos.Y + height), new Rectangle(170, 456, 14, 15), color, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0f);
			Vector2 position = new Vector2(pos.X + 3f, pos.Y + num + 13f);
			HUD.sprite.Draw(HUD.hudTex[2], position, new Rectangle(220, 422, 22, 75), color, 0f, new Vector2(11f, 37.5f), 1.2f, SpriteEffects.None, 0f);
			if (this.debugDisplay)
			{
				this.scrollBarPos = new Vector3(pos.X - 24f, pos.Y - 20f, height + 80f);
				HUD.sprite.Draw(HUD.nullTex, new Rectangle((int)this.scrollBarPos.X, (int)this.scrollBarPos.Y, 54, (int)this.scrollBarPos.Z), Color.White * 0.5f);
			}
			return new Vector3(pos.X - 24f, pos.Y - 20f, height + 80f);
		}

		public void DrawScrollBarMouse(Vector2 loc, ref float persistFloat, int scrollBarLength, int maxScrollHeight, int ratchetHeight, float alpha)
		{
			this.scrollBarPos = new Vector3(loc.X - 24f, loc.Y - 20f, scrollBarLength + 80);
			float num = (float)Math.Round(persistFloat / (float)ratchetHeight, 0) * (float)ratchetHeight;
			if (new Rectangle((int)this.scrollBarPos.X, (int)this.scrollBarPos.Y, 80, (int)this.scrollBarPos.Z).Contains((int)this.mousePos.X, (int)this.mousePos.Y) && Game1.pcManager.IsMouseLeftHeld())
			{
				float num2 = MathHelper.Clamp(this.mousePos.Y - loc.Y, 0f, scrollBarLength) / (float)scrollBarLength;
				num = (int)(num2 * (float)maxScrollHeight / (float)ratchetHeight) * ratchetHeight;
			}
			persistFloat += (num - persistFloat) * Game1.HudTime * 20f;
			this.scrollBarPos = this.DrawScrollBar(loc, persistFloat / (float)maxScrollHeight, scrollBarLength, alpha);
		}

		public void DrawBar(Vector2 loc, float percentPos, Color color, Vector2 scale, bool backGround)
		{
			percentPos = MathHelper.Clamp(percentPos, 0.04f, 1f);
			if (backGround)
			{
				HUD.sprite.Draw(HUD.hudTex[0], loc + new Vector2(4f, 4f) * scale, new Rectangle(0, 120, 310, 20), new Color(0, 0, 0, (int)color.A / 255), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
				HUD.sprite.Draw(HUD.hudTex[0], loc, new Rectangle(0, 120, 310, 20), new Color((float)(int)color.R / 500f, (float)(int)color.G / 500f, (float)(int)color.B / 500f, (int)color.A), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			}
			HUD.sprite.Draw(HUD.hudTex[0], loc, new Rectangle(0, 120, (int)(305f * percentPos), 20), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			HUD.sprite.Draw(HUD.hudTex[0], loc + new Vector2(304f * scale.X - (305f * scale.X - 305f * scale.X * percentPos), 0f), new Rectangle(304, 120, 10, 20), color, 0f, Vector2.Zero, scale.Y, SpriteEffects.None, 0f);
			HUD.sprite.Draw(HUD.hudTex[1], loc + new Vector2(0f, 4f * scale.Y), new Rectangle(580, HUD.lifeBarAnim, 305, 10), new Color((float)(int)color.R / 255f, (float)(int)color.G / 255f, (float)(int)color.B / 255f, (float)(int)color.A / 300f * percentPos), 0f, Vector2.Zero, new Vector2(305f * percentPos / 305f, 1f) * scale, SpriteEffects.None, 0f);
		}

		public void DrawTimer(Vector2 timerOffset, float timer, float scale, Color color, bool pad, bool milli, int type)
		{
			int num = (int)timer;
			int num2 = num / 60;
			int num3 = Math.Min(num2 / 60, 99);
			num -= num2 * 60;
			num2 -= num3 * 60;
			if (num3 >= 99)
			{
				num2 = Math.Min(num2, 59);
				num = Math.Min(num, 59);
			}
			Vector2 vector = new Vector2(31f, 0f);
			float num4 = 1f;
			if (type == 1)
			{
				num4 = 1.2f;
				vector = new Vector2(33f, 3f);
			}
			Rectangle value;
			if (type == 2)
			{
				num4 = 2.2f;
				vector = new Vector2(37f, 3f);
				value = new Rectangle(344, 0, 24, 47);
			}
			else
			{
				value = new Rectangle(200, 0, 24, 25);
			}
			if (num3 > 0 || pad)
			{
				if (num3 < 10)
				{
					this.scoreDraw.Draw(0L, timerOffset, scale, color, ScoreDraw.Justify.Right, type);
				}
				HUD.sprite.Draw(HUD.numbersTex, timerOffset + vector * scale * num4, value, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
				if (num2 < 10)
				{
					this.scoreDraw.Draw(0L, timerOffset + new Vector2(48f * scale * num4, 0f), scale, color, ScoreDraw.Justify.Right, type);
				}
			}
			HUD.sprite.Draw(HUD.numbersTex, timerOffset + (vector + new Vector2(48f, 0f)) * scale * num4, value, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			if (num < 10)
			{
				this.scoreDraw.Draw(0L, timerOffset + new Vector2(96f * scale * num4, 0f), scale, color, ScoreDraw.Justify.Right, type);
			}
			if (num3 > 0 || pad)
			{
				this.scoreDraw.Draw(num3, timerOffset + new Vector2(18f * scale * num4, 0f), scale, color, ScoreDraw.Justify.Right, type);
			}
			if (num3 > 0 || num2 >= 0 || pad)
			{
				this.scoreDraw.Draw(num2, timerOffset + new Vector2(66f * scale * num4, 0f), scale, color, ScoreDraw.Justify.Right, type);
			}
			this.scoreDraw.Draw(num, timerOffset + new Vector2(114f * scale * num4, 0f), scale, color, ScoreDraw.Justify.Right, type);
			if (milli)
			{
				HUD.sprite.Draw(HUD.numbersTex, timerOffset + (vector + new Vector2(96f, 0f)) * scale * num4, value, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
				int num5 = (int)Math.Abs(timer * 100f) % 100;
				if (num5 < 10)
				{
					this.scoreDraw.Draw(0L, timerOffset + new Vector2(144f * scale * num4, 0f), scale, color, ScoreDraw.Justify.Right, type);
				}
				this.scoreDraw.Draw(num5, timerOffset + new Vector2(162f * scale * num4, 0f), scale, color, ScoreDraw.Justify.Right, type);
			}
		}

		public void UpdateThread(ParticleManager pMan, Character[] c)
		{
			int num = (int)Math.Min((float)this.character[0].MaxHP * Game1.stats.bonusHealth, 9999f);
			HUD.lifeBarPos = MathHelper.Clamp(this.character[0].LifeBarPercent / (float)num, 0.04f, 1f);
			this.MoveXPBar();
			this.MoveChargeBar();
			HUD.lifeBarAnimTime += Game1.HudTime * 48f;
			if (HUD.lifeBarAnimTime > 1f)
			{
				HUD.lifeBarAnim++;
				if (HUD.lifeBarAnim > 189)
				{
					HUD.lifeBarAnim = 0;
				}
				HUD.lifeBarAnimTime = 0f;
			}
			if ((float)this.character[0].HP < (float)num * 0.2f)
			{
				this.lifeBarBeat += MathHelper.Clamp(Game1.HudTime * 4f * ((float)num * 0.2f / (float)this.character[0].HP), 0f, 0.1f);
				if (this.lifeBarBeat > 1f || this.lifeBarBeat < 0f)
				{
					this.lifeBarBeat = 0f;
				}
			}
			this.UpdateThreadTimers(pMan, c);
		}

		private void UpdateThreadTimers(ParticleManager pMan, Character[] c)
		{
			this.pulse += Game1.HudTime * 3f;
			if (this.pulse > 6.28f)
			{
				this.pulse -= 6.28f;
			}
			if (this.tempGold < Game1.stats.Gold)
			{
				if (this.tempGold < Game1.stats.Gold - 200)
				{
					this.tempGold += 200;
				}
				else if (this.tempGold < Game1.stats.Gold - 40)
				{
					this.tempGold += 40;
				}
				else if (Game1.longSkipFrame > 3)
				{
					this.tempGold++;
				}
			}
			else if (this.tempGold > Game1.stats.Gold)
			{
				if (this.tempGold > Game1.stats.Gold + 200)
				{
					this.tempGold -= 200;
				}
				else if (this.tempGold > Game1.stats.Gold + 40)
				{
					this.tempGold -= 40;
				}
				else if (Game1.longSkipFrame > 3)
				{
					this.tempGold--;
				}
			}
			HUD.hudAnimFrameTime += Game1.HudTime * 24f;
			if (HUD.hudAnimFrameTime > 1f)
			{
				this.cursorFrame++;
				if (this.cursorFrame > 48)
				{
					this.cursorFrame = 0;
				}
				HUD.chargeBarFrame++;
				if (HUD.chargeBarFrame > 59)
				{
					HUD.chargeBarFrame = 0;
				}
				HUD.coinAnimFrame++;
				if (HUD.coinAnimFrame > 23)
				{
					HUD.coinAnimFrame = 0;
				}
				HUD.dizzyAnimFrame++;
				if (HUD.dizzyAnimFrame > 15)
				{
					HUD.dizzyAnimFrame = 0;
				}
				HUD.hudAnimFrameTime -= 1f;
			}
			if (HUD.eventOffset)
			{
				if (this.eventLeftOffset == 0f)
				{
					HUD.eventIcon = 0f;
				}
				this.eventLeftOffset = Math.Min(this.eventLeftOffset + Game1.HudTime * 3000f, 800f * this.hudScale);
				if (HUD.eventIcon > 0f)
				{
					HUD.eventIcon = Math.Max(HUD.eventIcon - Game1.HudTime, 0f);
				}
				if (Game1.events.anyEvent || Game1.events.regionIntroStage > 0)
				{
					HUD.cineOffset = Math.Min(HUD.cineOffset + Game1.HudTime * 300f, 100f);
				}
			}
			else if (this.eventLeftOffset != 0f)
			{
				this.eventLeftOffset -= Game1.HudTime * 5000f * (this.eventLeftOffset / (800f * this.hudScale));
				if (this.eventLeftOffset < 1f)
				{
					this.eventLeftOffset = 0f;
					HUD.eventIcon = 0f;
				}
				if (HUD.cineOffset > 0f)
				{
					HUD.cineOffset = Math.Max(HUD.cineOffset - Game1.HudTime * 300f, 0f);
				}
			}
			if (HUD.changeProjectileTime > 1f)
			{
				HUD.changeProjectileTime -= Game1.HudTime * 20f * (2f / HUD.changeProjectileTime);
				if (HUD.changeProjectileTime < 1f)
				{
					HUD.changeProjectileTime = 1f;
				}
			}
			if (Game1.stats.comboDisplayTimer > 0.2f)
			{
				this.expDialogueOffset += Game1.HudTime * 3000f * (this.expDialogueOffset / -300f);
				if (this.expDialogueOffset > -1f)
				{
					this.expDialogueOffset = 0f;
				}
			}
			else if (this.expDialogueOffset != -300f)
			{
				this.expDialogueOffset -= Game1.HudTime * 1500f;
				if (this.expDialogueOffset < -299f)
				{
					this.expDialogueOffset = -300f;
				}
			}
			if (Game1.camera.HudInWay)
			{
				this.hudDim = Math.Max(this.hudDim - Game1.HudTime * 8f, 0.1f);
			}
			else if (this.helpState > 0)
			{
				this.hudDim = Math.Max(this.hudDim - Game1.HudTime * 8f, 0.5f);
			}
			else if (this.hudDim < 1f)
			{
				this.hudDim = Math.Min(this.hudDim + Game1.HudTime * 4f, 1f);
			}
			if (Game1.camera.NavInWay || this.helpState > 0 || Game1.cManager.challengeState > 0)
			{
				HUD.navDim = Math.Max(HUD.navDim - Game1.HudTime * 8f, 0.2f);
			}
			else if (HUD.navDim < 1f)
			{
				HUD.navDim = Math.Min(HUD.navDim + Game1.HudTime * 4f, 1f);
			}
			if (this.comboTextSize > 1f)
			{
				this.comboTextSize -= Game1.HudTime * 20f * (this.comboTextSize / 2f);
				if (this.comboTextSize < 1f)
				{
					this.comboTextSize = 1f;
				}
			}
			if (Game1.menu.prompt != promptDialogue.None)
			{
				if (this.map.transInFrame < 0.4f || Game1.events.anyEvent)
				{
					HUD.promptAlpha += Game1.HudTime * 4f;
				}
				if (HUD.promptAlpha > 1f)
				{
					HUD.promptAlpha = 1f;
				}
			}
			else if (HUD.promptAlpha > 0f)
			{
				HUD.promptAlpha = 0f;
				Game1.menu.curMenuOption = 0;
			}
			if (this.autoSaveTime > 0f && HUD.promptAlpha <= 0f)
			{
				this.autoSaveTime -= Game1.FrameTime;
			}
			if (!this.isPaused && this.shopType == ShopType.None)
			{
				this.shopRestockTime -= Game1.HudTime;
			}
			if (this.shopRestockTime < 0f)
			{
				this.shopRestockTime = 240f;
				this.RestockItems();
			}
			HUD.miniBorderGlowPos += Game1.HudTime * 1.5f;
			if (HUD.miniBorderGlowPos > 4f)
			{
				HUD.miniBorderGlowPos = 0f;
			}
			if (Game1.stats.skillPoints > 0 && this.miniPromptState == 0 && !this.isPaused && !Game1.events.anyEvent && this.inventoryState == InventoryState.None)
			{
				this.InitMiniPrompt(MiniPromptType.LevelUp, 0, blueprint: false);
			}
		}

		private void UpdateTimers(ParticleManager pMan, Character[] c)
		{
			if (this.isPaused || this.inventoryState != 0 || Game1.menu.prompt != promptDialogue.None || this.shop != null)
			{
				if (this.map.transInFrame < 0.4f)
				{
					this.pauseImageOffset -= Game1.HudTime * 5000f * (this.pauseImageOffset / 700f);
					if (this.pauseImageOffset < 1f)
					{
						this.pauseImageOffset = 0f;
					}
				}
			}
			else if (this.pauseImageOffset != 700f)
			{
				this.pauseImageOffset += Game1.HudTime * 3000f;
				if (this.pauseImageOffset > 700f)
				{
					this.pauseImageOffset = 700f;
				}
			}
			if (this.inBoss)
			{
				this.UpdateBoss(c, pMan);
			}
			if (Game1.halfSecFrame > 30)
			{
				Game1.awardsManager.UpdateLeaderboardStats();
			}
		}

		public void DoInput(int index)
		{
			this.KeyLeft = false;
			this.KeyRight = false;
			this.KeySelect = false;
			this.KeyUp = false;
			this.KeyDown = false;
			this.KeyX = false;
			this.KeyY = false;
			this.KeyCancel = false;
			this.KeyPause = false;
			this.KeyRightTrigger = false;
			this.KeyLeftTrigger = false;
			this.OpenMenuButton = false;
			this.KeyRightBumper = false;
			this.KeyLeftBumper = false;
			this.KeyThumb_Left = false;
			this.KeyThumb_Right = false;
			Game1.hud.mousePos = Game1.pcManager.GetMouseLoc();
			if (this.map.warpStage > 0 || this.map.doorStage > 0 || Game1.events.regionIntroStage > 0 || (Game1.GamerServices && Guide.IsVisible))
			{
				return;
			}
			bool flag = this.inventoryState != 0 || HUD.promptAlpha > 0f || Game1.gameMode != Game1.GameModes.Game || this.isPaused || (this.dialogueState == DialogueState.Active && this.dialogue.responseLocalList.Count > 1);
			Game1.pcManager.UpdateHudInput(ref this.KeyLeft, ref this.KeyRight, ref this.KeyUp, ref this.KeyDown, ref this.KeyUpOpen, ref this.KeyThumb_Left, ref this.KeyThumb_Right, ref this.KeySelect, ref this.KeyCancel, ref this.KeyX, ref this.KeyY, ref this.KeyPause, ref this.OpenMenuButton, ref this.KeyLeftTrigger, ref this.KeyRightTrigger, ref this.KeyLeftBumper, ref this.KeyRightBumper);
			if (Game1.currentGamePad < 0)
			{
				return;
			}
			this.curState = GamePad.GetState((PlayerIndex)index);
			if (this.curState.Buttons.A == ButtonState.Pressed && this.prevState.Buttons.A == ButtonState.Released)
			{
				Game1.pcManager.inputDevice = InputDevice.GamePad;
				this.KeySelect = true;
			}
			if (this.curState.Buttons.Start == ButtonState.Pressed && this.prevState.Buttons.Start == ButtonState.Released)
			{
				Game1.pcManager.inputDevice = InputDevice.GamePad;
				this.KeyPause = true;
			}
			if (!this.curState.IsConnected && this.prevState.IsConnected && !this.isPaused)
			{
				this.KeyPause = true;
			}
			if (this.curState.Buttons.B == ButtonState.Pressed && this.prevState.Buttons.B == ButtonState.Released)
			{
				Game1.pcManager.inputDevice = InputDevice.GamePad;
				this.KeyCancel = true;
			}
			if (this.curState.DPad.Up == ButtonState.Pressed && this.prevState.DPad.Up == ButtonState.Released)
			{
				Game1.pcManager.inputDevice = InputDevice.GamePad;
				this.KeyUp = true;
				this.KeyUpOpen = true;
			}
			if (this.curState.ThumbSticks.Left.Y > 0.2f)
			{
				if (Game1.menu.menuMode != 0 || Game1.menu.prompt != promptDialogue.None || this.isPaused || this.inventoryState != 0)
				{
					if (this.prevState.ThumbSticks.Left.Y < 0.2f && this.curState.ThumbSticks.Left.Y > 0.2f)
					{
						Game1.pcManager.inputDevice = InputDevice.GamePad;
						this.KeyUp = true;
						this.KeyUpOpen = true;
					}
				}
				else if (this.prevState.ThumbSticks.Left.Y < 0.8f && this.curState.ThumbSticks.Left.Y > 0.8f)
				{
					Game1.pcManager.inputDevice = InputDevice.GamePad;
					this.KeyUp = true;
					this.KeyUpOpen = true;
				}
			}
			if ((this.prevState.ThumbSticks.Left.Y > -0.2f && this.curState.ThumbSticks.Left.Y < -0.2f) || (this.curState.DPad.Down == ButtonState.Pressed && this.prevState.DPad.Down == ButtonState.Released))
			{
				Game1.pcManager.inputDevice = InputDevice.GamePad;
				this.KeyDown = true;
			}
			if (this.curState.Buttons.Back == ButtonState.Pressed && this.prevState.Buttons.Back == ButtonState.Released)
			{
				Game1.pcManager.inputDevice = InputDevice.GamePad;
				this.OpenMenuButton = true;
			}
			if (Game1.stats.playerLifeState == 0 || Game1.menu.menuMode != 0)
			{
				if (this.curState.Buttons.LeftShoulder == ButtonState.Pressed && this.prevState.Buttons.LeftShoulder == ButtonState.Released)
				{
					Game1.pcManager.inputDevice = InputDevice.GamePad;
					this.KeyLeftBumper = true;
				}
				if (this.curState.Buttons.RightShoulder == ButtonState.Pressed && this.prevState.Buttons.RightShoulder == ButtonState.Released)
				{
					Game1.pcManager.inputDevice = InputDevice.GamePad;
					this.KeyRightBumper = true;
				}
				if (this.curState.Buttons.LeftStick == ButtonState.Pressed && this.prevState.Buttons.LeftStick == ButtonState.Released)
				{
					this.KeyThumb_Left = true;
				}
				if (this.curState.Buttons.RightStick == ButtonState.Pressed && this.prevState.Buttons.RightStick == ButtonState.Released)
				{
					this.KeyThumb_Right = true;
				}
				if (this.curState.Buttons.Y == ButtonState.Pressed && this.prevState.Buttons.Y == ButtonState.Released)
				{
					Game1.pcManager.inputDevice = InputDevice.GamePad;
					this.KeyY = true;
				}
				if (this.curState.Buttons.X == ButtonState.Pressed && this.prevState.Buttons.X == ButtonState.Released)
				{
					Game1.pcManager.inputDevice = InputDevice.GamePad;
					this.KeyX = true;
				}
			}
			if (flag)
			{
				if (this.curState.ThumbSticks.Left.X != 0f || this.curState.ThumbSticks.Left.Y != 0f || this.curState.DPad.Up == ButtonState.Pressed || this.curState.DPad.Down == ButtonState.Pressed || this.curState.DPad.Right == ButtonState.Pressed || this.curState.DPad.Left == ButtonState.Pressed || this.curState.Buttons.LeftShoulder == ButtonState.Pressed || this.curState.Buttons.RightShoulder == ButtonState.Pressed || this.curState.Triggers.Left != 0f || this.curState.Triggers.Right != 0f)
				{
					if (this.holdMoveTime > 0f)
					{
						this.holdMoveTime -= Game1.HudTime;
						if (this.holdMoveTime < 0f)
						{
							this.holdMoveTime = 0f;
						}
					}
					else if (Game1.longSkipFrame > 3)
					{
						if (this.curState.ThumbSticks.Left.Y > 0.5f || this.curState.DPad.Up == ButtonState.Pressed)
						{
							this.KeyUp = true;
						}
						else if (this.curState.ThumbSticks.Left.Y < -0.5f || this.curState.DPad.Down == ButtonState.Pressed)
						{
							this.KeyDown = true;
						}
						if (this.curState.ThumbSticks.Left.X < -0.5f || this.curState.DPad.Left == ButtonState.Pressed)
						{
							this.KeyLeft = true;
						}
						else if (this.curState.ThumbSticks.Left.X > 0.5f || this.curState.DPad.Right == ButtonState.Pressed)
						{
							this.KeyRight = true;
						}
						if (this.curState.Buttons.LeftShoulder == ButtonState.Pressed)
						{
							this.KeyLeftBumper = true;
						}
						else if (this.curState.Buttons.RightShoulder == ButtonState.Pressed && (this.isPaused || this.inventoryState != 0))
						{
							this.KeyRightBumper = true;
						}
						if (this.curState.Triggers.Left > 0.1f)
						{
							this.KeyLeftTrigger = true;
						}
						else if (this.curState.Triggers.Right > 0.1f)
						{
							this.KeyRightTrigger = true;
						}
					}
				}
				else
				{
					this.holdMoveTime = 0.5f;
				}
				if ((this.curState.ThumbSticks.Left.X < -0.2f && this.prevState.ThumbSticks.Left.X > -0.2f) || (this.curState.DPad.Left == ButtonState.Pressed && this.prevState.DPad.Left == ButtonState.Released))
				{
					Game1.pcManager.inputDevice = InputDevice.GamePad;
					this.KeyLeft = true;
				}
				else if ((this.curState.ThumbSticks.Left.X > 0.2f && this.prevState.ThumbSticks.Left.X < 0.2f) || (this.curState.DPad.Right == ButtonState.Pressed && this.prevState.DPad.Right == ButtonState.Released))
				{
					Game1.pcManager.inputDevice = InputDevice.GamePad;
					this.KeyRight = true;
				}
				if (this.curState.Triggers.Left > 0.1f && this.prevState.Triggers.Left < 0.1f)
				{
					Game1.pcManager.inputDevice = InputDevice.GamePad;
					this.KeyLeftTrigger = true;
				}
				if (this.curState.Triggers.Right > 0.1f && this.prevState.Triggers.Right < 0.1f)
				{
					Game1.pcManager.inputDevice = InputDevice.GamePad;
					this.KeyRightTrigger = true;
				}
			}
			this.prevState = this.curState;
		}

		public bool CheckSignOut()
		{
			if (Game1.GamerServices && Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)] == null && (Game1.gameMode == Game1.GameModes.Game || Game1.gameMode == Game1.GameModes.Menu || Game1.gameMode == Game1.GameModes.WorldMap) && Game1.menu.prompt != promptDialogue.SignedOut)
			{
				return true;
			}
			return false;
		}

		public void SignOut(ParticleManager pMan)
		{
			Game1.awardsManager.prevGamer = null;
			if (Game1.gameMode != 0)
			{
				this.isPaused = false;
				if (Game1.gameMode == Game1.GameModes.WorldMap || Game1.cutscene.SceneType != 0)
				{
					Game1.gameMode = Game1.GameModes.Game;
					Game1.videoPlayer.Stop();
					Game1.GetLargeContent().Unload();
					Game1.navManager.ExitWorldMap();
					Game1.cutscene.ExitCutscene();
					Game1.map.SwitchMap(pMan, this.character, "blank", loading: false);
					this.character[0].Location = new Vector2(800f, 800f);
					this.character[0].State = CharState.Air;
				}
				if (this.dialogueState != 0)
				{
					this.dialogue.InitExit();
					this.DialogueNull(-1, -1);
				}
				if (this.inventoryState != 0)
				{
					this.ExitInventory();
					this.ClearHelp();
				}
				if (Game1.menu.menuMode != 0)
				{
					Game1.menu.menuMode = MenuMode.None;
				}
				if (Game1.cManager.challengeMode != 0 && Game1.cManager.challengeMode != ChallengeManager.ChallengeMode.InChallenge)
				{
					Game1.cManager.ExitScoreBoard(canRestart: false);
				}
				Game1.events.ClearEvent();
				Game1.events.currentEvent = 0;
				Game1.events.regionIntroStage = 0;
				Sound.PlayCue("shop_fail");
				Music.Play("silent");
				Game1.stats.ResetLeaderboardRanks();
				Game1.stats.ResetItems();
				for (int i = 0; i < Game1.stats.shopEquipment.Length; i++)
				{
					Game1.stats.shopEquipment[i] = 0;
				}
				for (int j = 0; j < Game1.stats.shopMaterial.Length; j++)
				{
					Game1.stats.shopMaterial[j] = -1;
				}
				Game1.questManager.ResetQuests();
				Game1.navManager.Reset();
				Game1.cManager.Reset();
				for (int k = 0; k < Game1.cManager.challengeArenas.Count; k++)
				{
					Game1.cManager.challengeArenas[k].HighScore = 0;
				}
				Game1.stats.GetWorldExplored();
				Game1.stats.Completion = 0f;
				Game1.storage.RemoveDevice();
				this.map.warpStage = (this.map.doorStage = (Game1.events.regionIntroStage = 0));
				Game1.menu.prompt = promptDialogue.SignedOut;
				Game1.menu.ClearPrompt();
			}
		}

		public void Update(ParticleManager pMan)
		{
			this.noEvent = !Game1.events.anyEvent && Game1.menu.prompt > (promptDialogue)49 && Game1.cutscene.SceneType != CutsceneType.Video;
			if (Game1.longSkipFrame > 3)
			{
				if (this.noEvent && this.eventLeftOffset != 800f && this.inventoryState == InventoryState.None)
				{
					this.hideHudDetails = MathHelper.Clamp(this.hideHudDetails + Game1.HudTime * 10f, 0f, 1f);
				}
				else
				{
					this.hideHudDetails = MathHelper.Clamp(this.hideHudDetails - Game1.HudTime * 20f, 0f, 1f);
				}
			}
			if (Game1.halfSecFrame > 30)
			{
				HUD.eventOffset = this.inventoryState != 0 || this.isPaused || Game1.events.anyEvent || Game1.menu.prompt != promptDialogue.None || Game1.events.regionIntroStage > 0;
				if (this.CheckSignOut())
				{
					this.SignOut(pMan);
				}
			}
			this.DoInput(Game1.currentGamePad);
			if (Game1.gameMode != 0)
			{
				if (!this.canInput)
				{
					if (this.curState.Buttons.A == ButtonState.Pressed || this.curState.Buttons.B == ButtonState.Pressed || this.curState.Buttons.X == ButtonState.Pressed || this.curState.Buttons.Y == ButtonState.Pressed || (this.shop != null && !this.shop.canInput) || Game1.pcManager.IsMouseLeftHeld() || Game1.pcManager.IsHudHeld("KeySelect") || Game1.pcManager.IsHudHeld("KeyCancel"))
					{
						this.canInput = false;
					}
					else if (Game1.menu.prompt == promptDialogue.None && !Game1.events.anyEvent)
					{
						this.canInput = true;
					}
					if (this.shop != null)
					{
						this.shop.canInput = true;
					}
				}
				if (HUD.promptAlpha > 0f && this.inventoryState == InventoryState.None)
				{
					this.canInput = false;
					this.LimitInput();
					Game1.menu.UpdatePrompt();
				}
				if (Game1.menu.menuMode == MenuMode.Chapter)
				{
					this.canInput = false;
					this.LimitInput();
					Game1.menu.chapter.Update(Game1.HudTime);
					return;
				}
				if (this.shopType != 0 && this.shop != null)
				{
					this.shop.UpdateShop(pMan, this.character);
				}
				else if (Game1.events.anyEvent || this.dialogueState == DialogueState.Active)
				{
					if (Game1.menu.prompt == promptDialogue.None && this.dialogueState == DialogueState.Active)
					{
						this.dialogue.Update();
					}
					this.LimitInput();
					this.canInput = false;
					if (HUD.cineOffset >= 100f)
					{
						if ((this.KeyX || this.KeyY || this.KeyLeftBumper || this.KeyRightBumper || this.KeySelect) && this.dialogueState == DialogueState.Inactive && Game1.menu.prompt == promptDialogue.None && Game1.events.currentEvent > -1 && Game1.SlowTime == 0f && this.shopType == ShopType.None && Game1.cutscene.SceneType != CutsceneType.Video)
						{
							Sound.PlayCue("shop_fail");
							HUD.eventIcon = 1f;
						}
						if (this.KeyPause || this.KeyCancel || (Game1.GamerServices && Guide.IsVisible && Game1.menu.prompt == promptDialogue.None))
						{
							this.KeyPause = false;
							this.KeyCancel = false;
							if (Game1.menu.prompt != promptDialogue.SkipEvent && (this.dialogueState != DialogueState.Active || this.dialogue.responseLocalList.Count < 2 || Game1.cutscene.SceneType == CutsceneType.Video))
							{
								Sound.PlayCue("menu_confirm");
								Game1.menu.prompt = promptDialogue.SkipEvent;
								Game1.menu.ClearPrompt();
								if (Game1.events.skippable != SkipType.Unskippable && (this.dialogueState == DialogueState.Active || Game1.cutscene.SceneType == CutsceneType.Video))
								{
									Game1.menu.curMenuOption = 1;
									Game1.menu.PopulatePrompt();
								}
								VoiceManager.PauseVoice();
							}
							else if (!Game1.GamerServices || !Guide.IsVisible)
							{
								Sound.PlayCue("shop_fail");
								HUD.eventIcon = 1f;
							}
						}
					}
				}
				if (!this.npcAvailable)
				{
					if (Game1.halfSecFrame > 30)
					{
						for (int i = 1; i < this.character.Length; i++)
						{
							if (this.character[i].Exists == CharExists.Exists && this.character[i].NPC != 0)
							{
								this.npcAvailable = true;
							}
						}
					}
				}
				else if (Game1.longSkipFrame > 3 && this.dialogueState == DialogueState.Inactive && this.noEvent && !this.inBoss)
				{
					this.converseWithID = -1;
					this.collectID = -1;
					if (this.character[0].State == CharState.Grounded && this.fidgetPrompt != FidgetPrompt.EnterDoor)
					{
						for (int j = 1; j < this.character.Length; j++)
						{
							if (this.character[j].Exists == CharExists.Exists && this.character[j].NPC != 0 && this.character[0].InCharBounds(this.character, j))
							{
								if (this.character[j].NPC == NPCType.Friendly)
								{
									this.converseWithID = j;
								}
								else if (this.character[j].CollectEquipID > -1)
								{
									this.collectID = j;
								}
							}
						}
					}
				}
				if (this.isPaused)
				{
					Game1.BlurScene(1f);
					this.LimitInput();
					VibrationManager.StopRumble();
					this.canInput = false;
					if (Game1.gameMode == Game1.GameModes.Menu)
					{
						Game1.menu.Update(pMan);
					}
					if (this.KeyPause && Game1.menu.menuMode == MenuMode.None && (Game1.cManager.challengeMode == ChallengeManager.ChallengeMode.None || Game1.cManager.challengeMode == ChallengeManager.ChallengeMode.InChallenge) && Game1.stats.playerLifeState == 0)
					{
						this.KeyPause = false;
						Game1.menu.ClearMenu();
						Sound.PlayCue("menu_cancel");
						GC.Collect();
					}
				}
				else if ((this.KeyPause || (Game1.GamerServices && Guide.IsVisible)) && this.map.GetTransVal() <= 0f && Game1.stats.playerLifeState == 0 && this.inventoryState == InventoryState.None && Game1.gameMode == Game1.GameModes.Game && Game1.menu.prompt == promptDialogue.None && !Game1.events.anyEvent && this.shopType == ShopType.None && Game1.events.regionIntroStage == 0 && this.unlockState == 0 && this.map.warpStage == 0 && this.map.doorStage == 0 && (Game1.cManager.challengeMode == ChallengeManager.ChallengeMode.None || Game1.cManager.challengeMode == ChallengeManager.ChallengeMode.InChallenge))
				{
					this.Pause();
				}
				if (this.noEvent)
				{
					if (this.KeyUp && this.inventoryState == InventoryState.None && !this.isPaused && !this.inBoss && Game1.gameMode != Game1.GameModes.WorldMap && Game1.stats.playerLifeState == 0)
					{
						if (this.saveable && this.character[0].State == CharState.Grounded)
						{
							this.KeyUp = false;
							if (this.character[0].AnimName.StartsWith("attack") || this.character[0].AnimName.StartsWith("evade"))
							{
								this.character[0].SetAnim("runend", 0, 2);
								this.character[0].Trajectory = new Vector2(0f, this.character[0].Trajectory.Y);
							}
							if (!Game1.GamerServices || !Game1.IsTrial)
							{
								Game1.menu.prompt = promptDialogue.Save;
							}
							Game1.menu.ClearPrompt();
						}
						else if (this.converseWithID > -1 && this.character[0].State == CharState.Grounded)
						{
							this.KeyUp = false;
							Game1.events.FaceCharacter(CharacterType.Dust, this.character[this.converseWithID].Definition.charType, faceTowards: true);
							Game1.questManager.UpdateQuests(0);
							this.InitDialogue(Game1.stats.villagerDialogue[(int)this.character[this.converseWithID].Definition.charType], this.character[this.converseWithID].Definition.charType);
							Game1.camera.camOffset.X = ((this.character[0].Face == CharDir.Left) ? 100 : (-100));
						}
						else if (this.collectID > -1 && this.character[0].State == CharState.Grounded)
						{
							this.KeyUp = false;
							Game1.events.InitEvent(71, isSideEvent: true);
							foreach (RevealMap value in Game1.navManager.RevealMap.Values)
							{
								int itemArrayID = Game1.savegame.GetItemArrayID(value.GameItemList, this.character[this.collectID].CollectEquipID + 1000, this.character[this.collectID].Name);
								if (itemArrayID > -1 && value.GameItemList[itemArrayID].UniqueID == this.character[this.collectID].Name)
								{
									value.GameItemList[itemArrayID].Stage = 1;
									Game1.stats.GetChestFromFile("Equip " + this.character[this.collectID].Name + " " + (EquipType)this.character[this.collectID].CollectEquipID + " " + 1.ToString(), Game1.pManager);
									Game1.questManager.UpdateQuests(320);
									for (int k = 0; k < 20; k++)
									{
										Vector2 loc = new Vector2(this.character[this.collectID].Location.X + (float)Rand.GetRandomInt(-80, 80), this.character[this.collectID].Location.Y + (float)Rand.GetRandomInt(-200, 20));
										int randomInt = Rand.GetRandomInt(24, 48);
										float randomFloat = Rand.GetRandomFloat(0.5f, 1.25f);
										pMan.AddSparkle(loc, Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 1f, randomFloat, randomInt, 5);
									}
									this.character[this.collectID].Exists = CharExists.Dead;
									this.collectID = -1;
									Game1.navManager.CheckRegionTreasure(pMan);
									this.character[0].KeyUp = true;
									this.character[0].SetAnim("crouch", 0, 0);
									Sound.PlayCue("note_pickup");
									this.InitFidgetPrompt(FidgetPrompt.NoLookUp);
									break;
								}
							}
						}
						else if (HUD.shoppable)
						{
							this.KeyUp = false;
							this.canInput = false;
							this.InitShopKeeper(this.shopID);
						}
					}
					this.UpdateInventory(pMan);
					if (this.unlockState > 0)
					{
						this.UpdateUnlocking(pMan, updating: false);
					}
					Game1.cManager.Update(this.character, this.map, pMan);
				}
			}
			if (this.runInTime > 0f)
			{
				this.LimitInput();
				this.canInput = false;
				if (Game1.map.GetTransVal() < 1f && Game1.events.regionIntroStage == 0)
				{
					if (this.character[0].Location.X < this.map.leftEdge + 1000f)
					{
						this.character[0].KeyRight = true;
					}
					else if (this.character[0].Location.X > this.map.rightEdge - 1000f)
					{
						this.character[0].KeyLeft = true;
					}
					this.runInTime -= Game1.FrameTime;
				}
				if (this.runInTime < 0f)
				{
					this.character[0].Ethereal = EtherealState.Normal;
					this.canInput = true;
				}
			}
			this.UpdateTimers(pMan, this.character);
		}

		public void Draw(ParticleManager pMan)
		{
			HUD.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			if ((Game1.events.currentEvent < 0 || Game1.events.currentEvent > 740) && HUD.promptAlpha == 0f)
			{
				this.DrawCineFade();
				this.DrawPause(pMan);
				if (this.helpState > 0)
				{
					this.DrawHelp();
				}
				HUD.sprite.End();
				return;
			}
			this.DrawStatDetails();
			this.DrawHud(pMan, menu: false, this.eventLeftOffset);
			this.DrawMapFade();
			this.DrawCineFade();
			if (this.regionIntroState > 0)
			{
				this.DrawRegionIntro(pMan);
			}
			this.DrawPause(pMan);
			Game1.cManager.Draw(HUD.sprite, HUD.hudTex, this.hudScale);
			if (this.inventoryState == InventoryState.None)
			{
				this.inventoryScreenOffset.Y = 1f;
			}
			else
			{
				this.DrawInventory();
			}
			if (this.unlockState > 0 && this.unlockGame != null)
			{
				this.unlockGame.Draw(HUD.sprite, pMan, HUD.nullTex);
			}
			if (this.helpState > 0)
			{
				this.DrawHelp();
			}
			if (this.dialogue != null)
			{
				this.dialogue.Draw(HUD.promptAlpha);
			}
			if (this.shopType != 0 && this.shop != null)
			{
				this.shop.DrawShop();
			}
			if (HUD.promptAlpha > 0f)
			{
				Game1.menu.DrawPauseMenu(HUD.promptAlpha);
			}
			if (Game1.menu.menuMode == MenuMode.Chapter)
			{
				Game1.menu.chapter.Draw(HUD.sprite);
			}
			if (Game1.gameMode != Game1.GameModes.Menu)
			{
				pMan.DrawHudParticles(HUD.particlesTex, 1f, 9);
			}
			HUD.sprite.End();
		}
	}
}
