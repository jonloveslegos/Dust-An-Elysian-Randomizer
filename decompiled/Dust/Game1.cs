using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using Dust.ai;
using Dust.ai.NPC;
using Dust.Audio;
using Dust.CharClasses;
using Dust.Dialogue;
using Dust.HUD;
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Dust
{
	public class Game1 : Game
	{
		public enum GameModes
		{
			MainMenu,
			Game,
			Menu,
			WorldMap,
			Quit,
			UpSell,
			Startup,
			CreditsScroll
		}

		public enum RenderMode
		{
			Normal,
			Panels
		}

		public enum DrawState
		{
			None,
			Normal,
			Character,
			Map
		}

		private static bool SkipMenu = false;

		public static bool canDebug = false;

		public static bool awardAchievements = true;

		public static string startMap = "village01";

		public static bool GamerServices = false;

		public static bool standardDef = false;

		public static bool invulnerable;

		public static bool VoiceNotReady = false;

		public static bool debugging = true;

		public static bool testDialogue = false;

		public static bool Convention = false;

		private static float ConventionTimer = 200f;

		public static float ConventionResetTimer;

		public static float Build = 1.04f;

		public static bool IsTrial = false;

		public static bool isPCBuild = true;

		public static bool hasLeaderboards = false;

		public static bool Xbox360 = false;

		public static bool XBLABuild = false;

		public static PCManager pcManager;

		public static bool CheckIsActive;

		public static GraphicsDeviceManager graphics;

		public static ManagedThread loadingThread;

		private static ManagedThread particleThreadMain;

		private static ManagedThread particleThreadSec;

		private static ManagedThread hudThread;

		private static SpriteBatch spriteBatch;

		public static int screenWidth;

		public static int screenHeight;

		public static int currentGamePad = -1;

		public static Map map;

		public static NavManager navManager;

		public static ParticleManager pManager;

		public static DestructableManager dManager;

		public static WeatherManager wManager;

		public static ChallengeManager cManager;

		private static UpsellManager upsellManager;

		public static Cutscene cutscene;

		public static ContentManager DustContent;

		private static ContentManager InventoryContent;

		private static ContentManager TitleContent;

		private static ContentManager MapContent0;

		private static ContentManager MapContent1;

		private static ContentManager MapContent2;

		private static ContentManager MapContent3;

		private static ContentManager MapBackdropContent;

		private static ContentManager DestructionContent;

		private static ContentManager LargeContent;

		private static ContentManager EnemyContent;

		private static ContentManager PortraitContent0;

		private static ContentManager PortraitContent1;

		public static EventManager events;

		public static InventoryManager inventoryManager;

		public static QuestManager questManager;

		public static Menu menu;

		public static Dust.HUD.HUD hud;

		public static GetText getText;

		public static Stats stats;

		public static Camera camera;

		public static AwardsManager awardsManager;

		public static CreditScroll creditsScroll;

		public static Text text;

		public static Text bigText;

		public static Text smallText;

		public static SpriteFont font;

		public static SpriteFont bigFont;

		public static SpriteFont smallFont;

		public static SpriteFont glyphFont;

		private static Texture2D[] particlesTex = new Texture2D[6];

		private static Texture2D[] hudTex = new Texture2D[3];

		private static Texture2D numbersTex;

		private static Texture2D nullTex;

		private static Texture2D loadTex;

		private static Texture2D splashTex;

		public static Video video;

		public static VideoPlayer videoPlayer;

		private static int layer = 9;

		public static int skipFrame = 10;

		public static int longSkipFrame = 100;

		public static int halfSecFrame = 0;

		public static int fixedTime;

		public static int frameRate = 0;

		private static TimeSpan targetTime;

		public static Character[] character = new Character[36];

		public static CharDef[] charDef = new CharDef[200];

		public static float SlowTime = 0f;

		public static float FrameTime = 0f;

		public static float HudTime = 0f;

		public static float loadingTime;

		private static float loadFrame;

		private static int loadAnimFrame;

		private static bool initLoaded = false;

		private static float lightMaskRot;

		public static Vector2 Scroll = Vector2.Zero;

		public static Vector2 pScroll = Vector2.Zero;

		public static float Gravity = 3500f;

		private static float setBlurScene;

		public static float blurScene = 0f;

		public static float worldScale = 1f;

		public static float worldDark = 0f;

		public static int characterGlow = 0;

		public static int maxRefractingChars = 0;

		public static float hiDefScaleOffset;

		private static float startUpTimer;

		private static int startUpStage = 0;

		public static Store storage;

		public static SaveGame savegame;

		public static Settings settings;

		public static GameModes gameMode = GameModes.Startup;

		public static RenderMode renderMode = RenderMode.Normal;

		public static DrawState drawState = DrawState.Normal;

		private static RenderTarget2D finalTarget;

		private static RenderTarget2D mainTarget;

		private static RenderTarget2D frontLayersTarget;

		private static RenderTarget2D blurTarget;

		public static RenderTarget2D navMapTarget;

		private static RenderTarget2D spotLightTarget;

		private static RenderTarget2D spotLightFinal;

		private static RenderTarget2D bloomTarget;

		private static RenderTarget2D[] refractTarget;

		public static bool refractive = false;

		private static Effect filterEffect;

		public static Effect refractEffect;

		private static Effect bloomEffect;

		public static Effect maskEffect;

		public static ContentManager GetInventoryContent()
		{
			return Game1.InventoryContent;
		}

		public static ContentManager GetMapContent0()
		{
			return Game1.MapContent0;
		}

		public static ContentManager GetMapContent1()
		{
			return Game1.MapContent1;
		}

		public static ContentManager GetMapContent2()
		{
			return Game1.MapContent2;
		}

		public static ContentManager GetMapContent3()
		{
			return Game1.MapContent3;
		}

		public static ContentManager GetMapBackContent()
		{
			return Game1.MapBackdropContent;
		}

		public static ContentManager GetDestructContent()
		{
			return Game1.DestructionContent;
		}

		public static ContentManager GetEnemyContent()
		{
			return Game1.EnemyContent;
		}

		public static ContentManager GetLargeContent()
		{
			return Game1.LargeContent;
		}

		public static ContentManager GetTitleContent()
		{
			return Game1.TitleContent;
		}

		public static ContentManager GetPortraitContent0()
		{
			return Game1.PortraitContent0;
		}

		public static ContentManager GetPortraitContent1()
		{
			return Game1.PortraitContent1;
		}

		public static ManagedThread LoadingThread()
		{
			return Game1.loadingThread;
		}

		public Game1()
		{
			Game1.initLoaded = false;
			Game1.InitFrameRate(24);
			Game1.graphics = new GraphicsDeviceManager(this);
			Game1.loadingThread = new ManagedThread(4);
			Game1.particleThreadSec = new ManagedThread(3);
			Game1.particleThreadMain = new ManagedThread(5);
			Game1.hudThread = new ManagedThread(3);
			base.Content.RootDirectory = "Content";
			Game1.DustContent = new ContentManager(base.Services);
			Game1.DustContent.RootDirectory = "Content";
			Game1.InventoryContent = new ContentManager(base.Services);
			Game1.InventoryContent.RootDirectory = "Content";
			Game1.TitleContent = new ContentManager(base.Services);
			Game1.TitleContent.RootDirectory = "Content";
			Game1.MapContent0 = new ContentManager(base.Services);
			Game1.MapContent0.RootDirectory = "Content";
			Game1.MapContent1 = new ContentManager(base.Services);
			Game1.MapContent1.RootDirectory = "Content";
			Game1.MapContent2 = new ContentManager(base.Services);
			Game1.MapContent2.RootDirectory = "Content";
			Game1.MapContent3 = new ContentManager(base.Services);
			Game1.MapContent3.RootDirectory = "Content";
			Game1.MapBackdropContent = new ContentManager(base.Services);
			Game1.MapBackdropContent.RootDirectory = "Content";
			Game1.DestructionContent = new ContentManager(base.Services);
			Game1.DestructionContent.RootDirectory = "Content";
			Game1.LargeContent = new ContentManager(base.Services);
			Game1.LargeContent.RootDirectory = "Content";
			Game1.EnemyContent = new ContentManager(base.Services);
			Game1.EnemyContent.RootDirectory = "Content";
			Game1.PortraitContent0 = new ContentManager(base.Services);
			Game1.PortraitContent0.RootDirectory = "Content";
			Game1.PortraitContent1 = new ContentManager(base.Services);
			Game1.PortraitContent1.RootDirectory = "Content";
			Game1.SkipMenu = false;
			if (Game1.GamerServices)
			{
				base.Components.Add(new GamerServicesComponent(this));
			}
			base.IsFixedTimeStep = true;
			Game1.targetTime = base.TargetElapsedTime;
			Game1.SetResources();
			if (Game1.IsTrial)
			{
				Guide.SimulateTrialMode = true;
			}
			if (!Game1.isPCBuild)
			{
				Game1.IsTrial = Guide.IsTrialMode;
			}
		}

		public static void SetResources()
		{
			Strings_Achievements.Culture = (Strings_Chapters.Culture = (Strings_Credits.Culture = (Strings_EquipDetails.Culture = (Strings_Equipment.Culture = (Strings_FriendlyNames.Culture = (Strings_Help.Culture = (Strings_Hud.Culture = (Strings_HudInv.Culture = (Strings_Leaderboards.Culture = (Strings_MainMenu.Culture = (Strings_Manual.Culture = (Strings_ManualGraphics.Culture = (Strings_Materials.Culture = (Strings_Notes.Culture = (Strings_Options.Culture = (Strings_PauseMenu.Culture = (Strings_PC.Culture = (Strings_Prompts.Culture = (Strings_Quests.Culture = (Strings_Regions.Culture = (Strings_Shop.Culture = CultureInfo.CurrentCulture)))))))))))))))))))));
			Dialogue_Events.Culture = (Dialogue_Avgustin.Culture = (Dialogue_Bean.Culture = (Dialogue_Blop.Culture = (Dialogue_Bopo.Culture = (Dialogue_Bram.Culture = (Dialogue_Calum.Culture = (Dialogue_Colleen.Culture = (Dialogue_Cora.Culture = (Dialogue_Corbin.Culture = (Dialogue_Cutscenes.Culture = (Dialogue_Elder.Culture = (Dialogue_Fale.Culture = (Dialogue_FloHop.Culture = (Dialogue_Geehan.Culture = (Dialogue_Gianni.Culture = (Dialogue_Ginger.Culture = (Dialogue_Haley.Culture = (Dialogue_Kane.Culture = (Dialogue_Lady.Culture = (Dialogue_MaMop.Culture = (Dialogue_Matti.Culture = (Dialogue_Moska.Culture = (Dialogue_OldGappy.Culture = (Dialogue_Oneida.Culture = (Dialogue_Reed.Culture = (Dialogue_Sanjin.Culture = (Dialogue_Sarahi.Culture = (Dialogue_ShopAurora.Culture = (Dialogue_ShopWild.Culture = (Dialogue_SmoBop.Culture = CultureInfo.CurrentCulture))))))))))))))))))))))))))))));
			List<ResourceManager> list = new List<ResourceManager>();
			list.Add(Strings_Achievements.ResourceManager);
			list.Add(Strings_Chapters.ResourceManager);
			list.Add(Strings_Credits.ResourceManager);
			list.Add(Strings_EquipDetails.ResourceManager);
			list.Add(Strings_Equipment.ResourceManager);
			list.Add(Strings_FriendlyNames.ResourceManager);
			list.Add(Strings_Help.ResourceManager);
			list.Add(Strings_Hud.ResourceManager);
			list.Add(Strings_HudInv.ResourceManager);
			list.Add(Strings_Leaderboards.ResourceManager);
			list.Add(Strings_MainMenu.ResourceManager);
			list.Add(Strings_Manual.ResourceManager);
			list.Add(Strings_ManualGraphics.ResourceManager);
			list.Add(Strings_Materials.ResourceManager);
			list.Add(Strings_Notes.ResourceManager);
			list.Add(Strings_Options.ResourceManager);
			list.Add(Strings_PauseMenu.ResourceManager);
			list.Add(Strings_PC.ResourceManager);
			list.Add(Strings_Prompts.ResourceManager);
			list.Add(Strings_Quests.ResourceManager);
			list.Add(Strings_Regions.ResourceManager);
			list.Add(Strings_Shop.ResourceManager);
			list.Add(Dialogue_Events.ResourceManager);
			list.Add(Dialogue_Avgustin.ResourceManager);
			list.Add(Dialogue_Bean.ResourceManager);
			list.Add(Dialogue_Blop.ResourceManager);
			list.Add(Dialogue_Bopo.ResourceManager);
			list.Add(Dialogue_Bram.ResourceManager);
			list.Add(Dialogue_Calum.ResourceManager);
			list.Add(Dialogue_Colleen.ResourceManager);
			list.Add(Dialogue_Cora.ResourceManager);
			list.Add(Dialogue_Corbin.ResourceManager);
			list.Add(Dialogue_Cutscenes.ResourceManager);
			list.Add(Dialogue_Elder.ResourceManager);
			list.Add(Dialogue_Fale.ResourceManager);
			list.Add(Dialogue_FloHop.ResourceManager);
			list.Add(Dialogue_Geehan.ResourceManager);
			list.Add(Dialogue_Gianni.ResourceManager);
			list.Add(Dialogue_Ginger.ResourceManager);
			list.Add(Dialogue_Haley.ResourceManager);
			list.Add(Dialogue_Kane.ResourceManager);
			list.Add(Dialogue_Lady.ResourceManager);
			list.Add(Dialogue_MaMop.ResourceManager);
			list.Add(Dialogue_Matti.ResourceManager);
			list.Add(Dialogue_Moska.ResourceManager);
			list.Add(Dialogue_OldGappy.ResourceManager);
			list.Add(Dialogue_Oneida.ResourceManager);
			list.Add(Dialogue_Reed.ResourceManager);
			list.Add(Dialogue_Sanjin.ResourceManager);
			list.Add(Dialogue_Sarahi.ResourceManager);
			list.Add(Dialogue_ShopAurora.ResourceManager);
			list.Add(Dialogue_ShopWild.ResourceManager);
			list.Add(Dialogue_SmoBop.ResourceManager);
			for (int i = 0; i < list.Count; i++)
			{
				try
				{
					list[i].GetString("000");
				}
				catch (Exception)
				{
				}
			}
		}

		protected void SetRenderTargets()
		{
			Game1.finalTarget = new RenderTarget2D(base.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
			Game1.mainTarget = new RenderTarget2D(base.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
			Game1.frontLayersTarget = new RenderTarget2D(base.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
			Game1.blurTarget = new RenderTarget2D(base.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth / 2, Game1.graphics.PreferredBackBufferHeight / 2);
			Game1.navMapTarget = new RenderTarget2D(base.GraphicsDevice, 176, 112);
			Game1.spotLightTarget = new RenderTarget2D(base.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth / 4, Game1.graphics.PreferredBackBufferHeight / 4);
			Game1.spotLightFinal = new RenderTarget2D(base.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
			Game1.refractTarget = new RenderTarget2D[2];
			for (int i = 0; i < 2; i++)
			{
				Game1.refractTarget[i] = new RenderTarget2D(base.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
			}
			Game1.bloomTarget = new RenderTarget2D(base.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth / 2, Game1.graphics.PreferredBackBufferHeight / 2);
			Game1.graphics.GraphicsDevice.SetRenderTarget(Game1.refractTarget[0]);
			Game1.graphics.GraphicsDevice.Clear(Color.Black);
			Game1.graphics.GraphicsDevice.SetRenderTarget(null);
			Game1.graphics.GraphicsDevice.SetRenderTarget(Game1.mainTarget);
			Game1.graphics.GraphicsDevice.Clear(Color.Black);
			Game1.graphics.GraphicsDevice.SetRenderTarget(null);
			Game1.graphics.GraphicsDevice.SetRenderTarget(Game1.navMapTarget);
			Game1.graphics.GraphicsDevice.Clear(Color.White);
			Game1.graphics.GraphicsDevice.SetRenderTarget(null);
			Game1.graphics.GraphicsDevice.SetRenderTarget(Game1.spotLightTarget);
			Game1.graphics.GraphicsDevice.Clear(Color.White);
			Game1.graphics.GraphicsDevice.SetRenderTarget(null);
			Game1.graphics.GraphicsDevice.SetRenderTarget(Game1.spotLightFinal);
			Game1.graphics.GraphicsDevice.Clear(Color.Black);
			Game1.graphics.GraphicsDevice.SetRenderTarget(null);
			Game1.graphics.GraphicsDevice.SetRenderTarget(Game1.bloomTarget);
			Game1.graphics.GraphicsDevice.Clear(Color.Black);
			Game1.graphics.GraphicsDevice.SetRenderTarget(null);
		}

		private static void InitCharacters()
		{
			Game1.charDef[0] = new CharDef("dust", CharacterType.Dust);
			Game1.charDef[1] = new CharDef("ginger", CharacterType.Ginger);
			Game1.charDef[11] = new CharDef("cora", CharacterType.Cora);
			Game1.charDef[2] = new CharDef("bean", CharacterType.Bean);
			Game1.charDef[3] = new CharDef("calum", CharacterType.Calum);
			Game1.charDef[4] = new CharDef("moska", CharacterType.Moska);
			Game1.charDef[5] = new CharDef("gianni", CharacterType.Gianni);
			Game1.charDef[6] = new CharDef("fale", CharacterType.Fale);
			Game1.charDef[7] = new CharDef("reed", CharacterType.Reed);
			Game1.charDef[8] = new CharDef("avgustin", CharacterType.Avgustin);
			Game1.charDef[9] = new CharDef("bram", CharacterType.Bram);
			Game1.charDef[10] = new CharDef("oneida", CharacterType.Oneida);
			Game1.charDef[21] = new CharDef("bopo", CharacterType.Bopo);
			Game1.charDef[22] = new CharDef("mamop", CharacterType.MaMop);
			Game1.charDef[14] = new CharDef("matti", CharacterType.Matti);
			Game1.charDef[15] = new CharDef("haley", CharacterType.Haley);
			Game1.charDef[16] = new CharDef("shopwild", CharacterType.ShopWild);
			Game1.charDef[17] = new CharDef("shopaurora", CharacterType.ShopAurora);
			Game1.charDef[19] = new CharDef("geehan", CharacterType.Geehan);
			Game1.charDef[95] = new CharDef("kaneghostchase", CharacterType.KaneGhostChase);
			Game1.charDef[96] = new CharDef("kaneghost", CharacterType.KaneGhost);
			Game1.charDef[28] = new CharDef("kane", CharacterType.Kane);
			Game1.charDef[50] = new CharDef("meatboy", CharacterType.MeatBoy);
			Game1.charDef[51] = new CharDef("bandagegirl", CharacterType.BandageGirl);
			Game1.charDef[52] = new CharDef("spelunky", CharacterType.Spelunky);
			Game1.charDef[53] = new CharDef("damsel", CharacterType.Damsel);
			Game1.charDef[54] = new CharDef("dishwasher", CharacterType.Dishwasher);
			Game1.charDef[55] = new CharDef("yuki", CharacterType.Yuki);
			Game1.charDef[56] = new CharDef("kid", CharacterType.Kid);
			Game1.charDef[57] = new CharDef("tim", CharacterType.Tim);
			Game1.charDef[58] = new CharDef("hyperchris", CharacterType.HyperChris);
			Game1.charDef[59] = new CharDef("hyperdan", CharacterType.HyperDan);
			Game1.charDef[60] = new CharDef("gomez", CharacterType.Gomez);
			Game1.charDef[61] = new CharDef("maw", CharacterType.Maw);
			Game1.charDef[26] = new CharDef("blop", CharacterType.Blop);
			Game1.charDef[23] = new CharDef("oldgappy", CharacterType.OldGappy);
			Game1.charDef[24] = new CharDef("flohop", CharacterType.FloHop);
			Game1.charDef[25] = new CharDef("smobop", CharacterType.SmoBop);
			Game1.charDef[20] = new CharDef("sarahi", CharacterType.Sarahi);
			Game1.charDef[12] = new CharDef("corbin", CharacterType.Corbin);
			Game1.charDef[13] = new CharDef("colleen", CharacterType.Colleen);
			Game1.charDef[29] = new CharDef("elder", CharacterType.Elder);
			Game1.charDef[30] = new CharDef("sanjin", CharacterType.Sanjin);
			Game1.charDef[31] = new CharDef("moonblood", CharacterType.Moonblood);
			Game1.charDef[62] = new CharDef("bunny", CharacterType.Bunny);
			Game1.charDef[63] = new CharDef("deer", CharacterType.Deer);
			Game1.charDef[66] = new CharDef("sheep", CharacterType.SheepCave);
			Game1.charDef[67] = new CharDef("gappysheep", CharacterType.GappySheep);
			Game1.charDef[70] = new CharDef("bunnysnow", CharacterType.BunnySnow);
			Game1.charDef[74] = new CharDef("darkvillager", CharacterType.DarkVillager);
			Game1.charDef[75] = new CharDef("imp", CharacterType.Imp);
			Game1.charDef[76] = new CharDef("light", CharacterType.LightBeast);
			Game1.charDef[77] = new CharDef("slime", CharacterType.Slime);
			Game1.charDef[79] = new CharDef("giant", CharacterType.Giant);
			Game1.charDef[78] = new CharDef("avee", CharacterType.Avee);
			Game1.charDef[80] = new CharDef("florn", CharacterType.Florn);
			Game1.charDef[81] = new CharDef("fuse", CharacterType.Fuse);
			Game1.charDef[86] = new CharDef("blomb", CharacterType.Blomb);
			Game1.charDef[82] = new CharDef("hound", CharacterType.RockHound);
			Game1.charDef[83] = new CharDef("cutter", CharacterType.StoneCutter);
			Game1.charDef[84] = new CharDef("trolk", CharacterType.Trolk);
			Game1.charDef[85] = new CharDef("squirtbug", CharacterType.SquirtBug);
			Game1.charDef[27] = new CharDef("lady", CharacterType.Lady);
			Game1.charDef[87] = new CharDef("ladyboss", CharacterType.LadyBoss);
			Game1.charDef[92] = new CharDef("psylph", CharacterType.Psylph);
			Game1.charDef[88] = new CharDef("remains", CharacterType.Remains);
			Game1.charDef[89] = new CharDef("remainshalf", CharacterType.RemainsHalf);
			Game1.charDef[90] = new CharDef("remainsbomb", CharacterType.RemainsBomb);
			Game1.charDef[91] = new CharDef("summoner", CharacterType.Summoner);
			Game1.charDef[93] = new CharDef("fleshfly", CharacterType.FleshFly);
			Game1.charDef[94] = new CharDef("fleshflyhive", CharacterType.FleshFlyHive);
			Game1.charDef[98] = new CharDef("frite", CharacterType.Frite);
			Game1.charDef[97] = new CharDef("kush", CharacterType.Kush);
			Game1.charDef[100] = new CharDef("lightsnow", CharacterType.LightBeastSnow);
			Game1.charDef[99] = new CharDef("blombsnow", CharacterType.BlombSnow);
			Game1.charDef[102] = new CharDef("wolf", CharacterType.Wolf);
			Game1.charDef[113] = new CharDef("wolfsoldier", CharacterType.WolfSoldier);
			Game1.charDef[104] = new CharDef("implava", CharacterType.ImpLava);
			Game1.charDef[109] = new CharDef("assassin", CharacterType.Assassin);
			Game1.charDef[112] = new CharDef("kushsoldier", CharacterType.KushSoldier);
			Game1.charDef[110] = new CharDef("soldier", CharacterType.Soldier);
			Game1.charDef[114] = new CharDef("cannon", CharacterType.Cannon);
			Game1.charDef[116] = new CharDef("airship", CharacterType.AirShip);
			Game1.charDef[117] = new CharDef("gaius", CharacterType.Gaius);
			for (int i = 1; i < Game1.character.Length; i++)
			{
				Game1.character[i] = new Character(Vector2.Zero, Game1.charDef[62], 0, "bunny", Team.Friendly);
				Game1.character[i].Exists = CharExists.Dead;
			}
			Game1.character[0] = new Character(new Vector2(300f, 300f), Game1.charDef[0], 0, "dust", Team.Friendly);
			Game1.character[0].LifeBarPercent = 0f;
		}

		public static int GetCharacterFromString(string m)
		{
			switch (m)
			{
				case "dust":
					return 0;
				case "imp":
					return 75;
				case "light":
					return 76;
				case "giant":
					return 79;
				case "slime":
					return 77;
				case "avee":
					return 78;
				case "ginger":
					return 1;
				case "cora":
					return 11;
				case "calum":
					return 3;
				case "bean":
					return 2;
				case "florn":
					return 80;
				case "hound":
					return 82;
				case "cutter":
					return 83;
				case "trolk":
					return 84;
				case "squirtbug":
					return 85;
				case "blomb":
					return 86;
				case "darkvillager":
					return 74;
				case "fuse":
					return 81;
				case "moska":
					return 4;
				case "gianni":
					return 5;
				case "fale":
					return 6;
				case "reed":
					return 7;
				case "avgustin":
					return 8;
				case "bram":
					return 9;
				case "oneida":
					return 10;
				case "bopo":
					return 21;
				case "mamop":
					return 22;
				case "matti":
					return 14;
				case "haley":
					return 15;
				case "shopwild":
					return 16;
				case "shopaurora":
					return 17;
				case "architect":
					return 18;
				case "geehan":
					return 19;
				case "meatboy":
					return 50;
				case "bandagegirl":
					return 51;
				case "spelunky":
					return 52;
				case "damsel":
					return 53;
				case "dishwasher":
					return 54;
				case "yuki":
					return 55;
				case "kid":
					return 56;
				case "tim":
					return 57;
				case "hyperchris":
					return 58;
				case "hyperdan":
					return 59;
				case "maw":
					return 61;
				case "gomez":
					return 60;
				case "bunny":
					return 62;
				case "deer":
					return 63;
				case "tarantula":
					return 64;
				case "sheep":
					return 66;
				case "gappysheep":
					return 67;
				case "gravecreeper":
					return 68;
				case "crow":
					return 69;
				case "bunnysnow":
					return 70;
				case "deersnow":
					return 71;
				case "snowfox":
					return 72;
				case "lavacreeper":
					return 73;
				case "blop":
					return 26;
				case "oldgappy":
					return 23;
				case "flohop":
					return 24;
				case "smobop":
					return 25;
				case "sarahi":
					return 20;
				case "corbin":
					return 12;
				case "colleen":
					return 13;
				case "elder":
					return 29;
				case "sanjin":
					return 30;
				case "gaius":
					return 117;
				case "lady":
					return 27;
				case "ladyboss":
					return 87;
				case "psylph":
					return 92;
				case "remains":
					return 88;
				case "remainshalf":
					return 89;
				case "remainsbomb":
					return 90;
				case "summoner":
					return 91;
				case "fleshfly":
					return 93;
				case "fleshflyhive":
					return 94;
				case "kaneghostchase":
					return 95;
				case "kaneghost":
					return 96;
				case "kane":
					return 28;
				case "frite":
					return 98;
				case "kush":
					return 97;
				case "lightsnow":
					return 100;
				case "blombsnow":
					return 99;
				case "wolf":
					return 102;
				case "implava":
					return 104;
				case "fleshflylava":
					return 105;
				case "fleshflyhivelava":
					return 106;
				case "moonblood":
					return 31;
				case "assassin":
					return 109;
				case "soldier":
					return 110;
				case "soldiergun":
					return 111;
				case "kushsoldier":
					return 112;
				case "wolfsoldier":
					return 113;
				case "cannon":
					return 114;
				case "airtransport":
					return 115;
				case "airship":
					return 116;
			}
			return 75;
		}

		public static AI GetCharacterAI(string charName, Character character)
		{
			switch (charName)
			{
			case "bunny":
			case "bunnysnow":
				return new Bunny(character);
			case "deer":
			case "deersnow":
				return new Deer(character);
			case "sheep":
			case "gappysheep":
				return new Sheep(character);
			case "npc":
				return new NPC(character);
			case "npcstill":
				return new NPCStill(character);
			case "oneida":
				if (Game1.events.currentEvent >= 45)
				{
					return new NPC(character);
				}
				return new OneidaBoss(character);
			case "friend":
				return new Friend(character);
			case "darkvillager":
				return new DarkVillager(character);
			case "imp":
			case "implava":
				return new Imp(character);
			case "lightbeast":
			case "lightbeastsnow":
				return new LightBeast(character);
			case "giant":
				return new Giant(character);
			case "slime":
				return new Slime(character);
			case "avee":
				return new Avee(character);
			case "florn":
				return new Florn(character);
			case "fuse":
				return new Fuse(character);
			case "rockhound":
				return new RockHound(character);
			case "cutter":
				return new StoneCutter(character);
			case "trolk":
				return new Trolk(character);
			case "squirtbug":
				return new SquirtBug(character);
			case "blomb":
			case "blombsnow":
				return new Blomb(character);
			case "ladyboss":
				return new LadyBoss(character);
			case "psylph":
				return new Psylph(character);
			case "remains":
			case "remainshalf":
			case "remainsexplode":
				return new Remains(character);
			case "summoner":
				return new Summoner(character);
			case "fleshfly":
			case "fleshflysnow":
				return new FleshFly(character);
			case "fleshflyhive":
			case "fleshflyhivesnow":
				return new FleshFlyHive(character);
			case "kaneghostchase":
				return new KaneGhostChase(character);
			case "kaneghost":
				return new KaneGhost(character);
			case "frite":
				return new Frite(character);
			case "kush":
			case "kushsoldier":
				return new Kush(character);
			case "wolf":
			case "wolfsoldier":
				return new Wolf(character);
			case "moonblood":
				return new Moonblood(character);
			case "assassin":
				return new Assassin(character);
			case "soldier":
				return new Soldier(character);
			case "cannon":
				return new Cannon(character);
			case "airship":
				return new Airship(character);
			case "gaius":
				return new Gaius(character);
			default:
				return null;
			}
		}

		public static ResourceManager GetResource(string name)
		{
			switch (name)
			{
			case "avgustin":
				return Dialogue_Avgustin.ResourceManager;
			case "bean":
				return Dialogue_Bean.ResourceManager;
			case "blop":
				return Dialogue_Blop.ResourceManager;
			case "bopo":
				return Dialogue_Bopo.ResourceManager;
			case "bram":
				return Dialogue_Bram.ResourceManager;
			case "calum":
				return Dialogue_Calum.ResourceManager;
			case "colleen":
				return Dialogue_Colleen.ResourceManager;
			case "cora":
				return Dialogue_Cora.ResourceManager;
			case "corbin":
				return Dialogue_Corbin.ResourceManager;
			case "elder":
				return Dialogue_Elder.ResourceManager;
			case "fale":
				return Dialogue_Fale.ResourceManager;
			case "flohop":
				return Dialogue_FloHop.ResourceManager;
			case "geehan":
				return Dialogue_Geehan.ResourceManager;
			case "gianni":
				return Dialogue_Gianni.ResourceManager;
			case "ginger":
				return Dialogue_Ginger.ResourceManager;
			case "haley":
				return Dialogue_Haley.ResourceManager;
			case "kane":
				return Dialogue_Kane.ResourceManager;
			case "lady":
				return Dialogue_Lady.ResourceManager;
			case "mamop":
				return Dialogue_MaMop.ResourceManager;
			case "matti":
				return Dialogue_Matti.ResourceManager;
			case "moska":
				return Dialogue_Moska.ResourceManager;
			case "oldgappy":
				return Dialogue_OldGappy.ResourceManager;
			case "oneida":
				return Dialogue_Oneida.ResourceManager;
			case "reed":
				return Dialogue_Reed.ResourceManager;
			case "sanjin":
				return Dialogue_Sanjin.ResourceManager;
			case "sarahi":
				return Dialogue_Sarahi.ResourceManager;
			case "shopaurora":
				return Dialogue_ShopAurora.ResourceManager;
			case "shoptrial":
			case "shopwild":
				return Dialogue_ShopWild.ResourceManager;
			case "smobop":
				return Dialogue_SmoBop.ResourceManager;
			default:
				return Dialogue_Events.ResourceManager;
			}
		}

		public static string GetFriendlyName(CharacterType charType)
		{
			switch (charType)
			{
				case CharacterType.Ginger:
					return Strings_FriendlyNames.Ginger;
				case CharacterType.Bean:
					return Strings_FriendlyNames.Bean;
				case CharacterType.Calum:
					return Strings_FriendlyNames.Calum;
				case CharacterType.Moska:
					return Strings_FriendlyNames.Moska;
				case CharacterType.Gianni:
					return Strings_FriendlyNames.Gianni;
				case CharacterType.Fale:
					return Strings_FriendlyNames.Fale;
				case CharacterType.Reed:
					return Strings_FriendlyNames.Reed;
				case CharacterType.Avgustin:
					return Strings_FriendlyNames.Avgustin;
				case CharacterType.Bram:
					return Strings_FriendlyNames.Bram;
				case CharacterType.Oneida:
					return Strings_FriendlyNames.Oneida;
				case CharacterType.Cora:
					return Strings_FriendlyNames.Cora;
				case CharacterType.Corbin:
					return Strings_FriendlyNames.Corbin;
				case CharacterType.Colleen:
					return Strings_FriendlyNames.Colleen;
				case CharacterType.Matti:
					return Strings_FriendlyNames.Matti;
				case CharacterType.Haley:
					return Strings_FriendlyNames.Haley;
				case CharacterType.Geehan:
					return Strings_FriendlyNames.Geehan;
				case CharacterType.Sarahi:
					return Strings_FriendlyNames.Sarahi;
				case CharacterType.Bopo:
					return Strings_FriendlyNames.Bopo;
				case CharacterType.MaMop:
					return Strings_FriendlyNames.MaMop;
				case CharacterType.OldGappy:
					return Strings_FriendlyNames.OldGappy;
				case CharacterType.FloHop:
					return Strings_FriendlyNames.FloHop;
				case CharacterType.SmoBop:
					return Strings_FriendlyNames.SmoBop;
				case CharacterType.Blop:
					return Strings_FriendlyNames.Blop;
				case CharacterType.Lady:
					return Strings_FriendlyNames.Lady;
				case CharacterType.Kane:
					return Strings_FriendlyNames.Kane;
				case CharacterType.Elder:
					return Strings_FriendlyNames.Elder;
				case CharacterType.Sanjin:
					return Strings_FriendlyNames.Sanjin;
			}
			return Strings_FriendlyNames.Dust;
		}

		protected override void Initialize()
		{
			if (Game1.graphics.GraphicsDevice.DisplayMode.Width < 1280)
			{
				Game1.standardDef = true;
			}
			int num = (int)MathHelper.Clamp(Game1.graphics.GraphicsDevice.DisplayMode.Width, 1024f, 1280f);
			int preferredBackBufferHeight = (int)((float)num * ((float)Game1.graphics.GraphicsDevice.DisplayMode.Height / (float)Game1.graphics.GraphicsDevice.DisplayMode.Width));
			Game1.graphics.PreferredBackBufferWidth = num;
			Game1.graphics.PreferredBackBufferHeight = preferredBackBufferHeight;
			Game1.graphics.ApplyChanges();
			Game1.screenWidth = base.GraphicsDevice.Viewport.Width;
			Game1.screenHeight = base.GraphicsDevice.Viewport.Height;
			Game1.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
			Game1.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			if ((float)Game1.graphics.PreferredBackBufferWidth < (float)Game1.graphics.PreferredBackBufferHeight * 2.5f)
			{
				Game1.graphics.ToggleFullScreen();
			}
			Game1.graphics.ApplyChanges();
			Game1.screenWidth = base.GraphicsDevice.Viewport.Width;
			Game1.screenHeight = base.GraphicsDevice.Viewport.Height;
			Game1.hiDefScaleOffset = (float)Game1.screenWidth / 1280f;
			this.SetRenderTargets();
			base.Initialize();
		}

		protected override void LoadContent()
		{
			Game1.initLoaded = false;
			Game1.spriteBatch = new SpriteBatch(base.GraphicsDevice);
			Game1.loadTex = base.Content.Load<Texture2D>("gfx/ui/loadTex");
			Game1.nullTex = base.Content.Load<Texture2D>("gfx/ui/1x1");
			Game1.startUpTimer = 0.5f;
			Game1.splashTex = Game1.TitleContent.Load<Texture2D>("gfx/splash/rating");
			Game1.videoPlayer = new VideoPlayer();
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadInitContent), new TaskFinishedDelegate(LoadingFinished)));
		}

		private void LoadInitContent()
		{
			Game1.glyphFont = base.Content.Load<SpriteFont>("Fonts/dust_glyphs");
			Game1.font = base.Content.Load<SpriteFont>("Fonts/Arial");
			if (CultureInfo.CurrentCulture.Name.StartsWith("en") || CultureInfo.CurrentCulture.Name.StartsWith("ja"))
			{
				Game1.smallFont = base.Content.Load<SpriteFont>("Fonts/SmallFont");
			}
			else
			{
				Game1.smallFont = base.Content.Load<SpriteFont>("Fonts/SmallFontNoKern");
			}
			Game1.bigFont = base.Content.Load<SpriteFont>("Fonts/BigFont");
			Game1.text = new Text(base.Content, Game1.spriteBatch, Game1.font, Game1.glyphFont, Game1.particlesTex);
			Game1.bigText = new Text(base.Content, Game1.spriteBatch, Game1.bigFont, Game1.glyphFont, Game1.particlesTex);
			Game1.smallText = new Text(base.Content, Game1.spriteBatch, Game1.smallFont, Game1.glyphFont, Game1.particlesTex);
			Sound.Initialize();
			VibrationManager.Init();
			Game1.getText = new GetText();
			Rand.random = new Random();
			Game1.video = Game1.GetLargeContent().Load<Video>("video/splash");
			Game1.wManager = new WeatherManager();
			Game1.map = new Map(Game1.nullTex, Game1.wManager);
			Game1.cutscene = new Cutscene(Game1.map);
			Game1.stats = new Stats();
			if (Game1.SkipMenu)
			{
				Game1.map.path = Game1.startMap;
			}
			else
			{
				Game1.map.path = "blank";
			}
			WeatherType newWeather = WeatherType.Realtime;
			Game1.map.Read(Game1.map.path, ref newWeather);
			Game1.events = new EventManager(Game1.character, Game1.map);
			Game1.cManager = new ChallengeManager();
			Game1.questManager = new QuestManager();
			Game1.InitCharacters();
			Game1.pManager = new ParticleManager(Game1.spriteBatch, Game1.character);
			Game1.dManager = new DestructableManager(Game1.spriteBatch, Game1.character, Game1.map);
			Game1.events.SetPMan(Game1.pManager);
			Game1.numbersTex = base.Content.Load<Texture2D>("Fonts/numbersTex");
			for (int i = 0; i < Game1.hudTex.Length; i++)
			{
				Game1.hudTex[i] = base.Content.Load<Texture2D>("gfx/ui/hud_elements_" + (i + 1));
			}
			for (int j = 0; j < Game1.particlesTex.Length; j++)
			{
				Game1.particlesTex[j] = base.Content.Load<Texture2D>("gfx/particles/particles_" + (j + 1));
				System.IO.Stream stream = System.IO.File.Create(System.IO.Directory.GetCurrentDirectory() + "particlesTex"+j.ToString()+".png");
				Game1.particlesTex[j].SaveAsPng(stream, Game1.particlesTex[j].Width, Game1.particlesTex[j].Height);
				stream.Dispose();
			}
			Game1.filterEffect = base.Content.Load<Effect>("shaders/filter");
			Game1.refractEffect = base.Content.Load<Effect>("shaders/refract");
			Game1.bloomEffect = base.Content.Load<Effect>("shaders/bloom");
			Game1.maskEffect = base.Content.Load<Effect>("shaders/mask");
			Game1.refractEffect.Parameters["strength"].SetValue(0.003f);
			Game1.bloomEffect.Parameters["a"].SetValue(1f);
			Game1.bloomEffect.Parameters["v"].SetValue(0.035f);
			Game1.inventoryManager = new InventoryManager();
			Game1.stats.ResetItems();
			Game1.hud = new Dust.HUD.HUD(Game1.spriteBatch, Game1.particlesTex, Game1.nullTex, Game1.hudTex, Game1.numbersTex, Game1.character, Game1.map);
			Game1.navManager = new NavManager(Game1.spriteBatch, Game1.nullTex);
			Game1.navManager.LoadContent(base.Content);
			Game1.menu = new Menu(Game1.spriteBatch, Game1.particlesTex, Game1.nullTex, Game1.hudTex, Game1.numbersTex, Game1.character, Game1.map);
			Game1.menu.LoadMenuTextures();
			Game1.menu.PopulateMenu();
			Game1.camera = new Camera();
			Game1.camera.ResetCamera(Game1.character);
			Game1.pManager.PopulateParticles();
			Game1.pManager.AddFidget(new Vector2(Game1.character[0].Location.X, Game1.character[0].Location.Y - 100f));
			Game1.awardsManager = new AwardsManager();
			Game1.storage = new Store();
			Game1.savegame = new SaveGame();
			Game1.settings = new Settings();
			Game1.questManager.ResetQuests();
			Game1.pManager.Reset(removeWeather: true, removeBombs: true);
			if (Game1.isPCBuild)
			{
				Game1.pcManager = new PCManager(Game1.spriteBatch, Game1.particlesTex, Game1.hudTex);
				LogicalGamer.SetPlayerIndex(LogicalGamerIndex.One, PlayerIndex.One);
				Game1.storage.GetDevice(LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One));
				Game1.storage.Read(0);
			}
		}

		private static void LoadingFinished(int taskId)
		{
			Game1.initLoaded = true;
			Game1.InitFrameRate(60);
		}

		public static void InitFrameRate(int _frameRate)
		{
			Game1.frameRate = _frameRate;
		}

		private void SetFrameRate()
		{
			Game1.frameRate = 0;
			if (!Game1.Xbox360)
			{
				base.IsFixedTimeStep = true;
			}
		}

		public static void BlurScene(float blur)
		{
			Game1.setBlurScene = Math.Max(Game1.setBlurScene, blur);
		}

		public static bool SetResolution(int width, int height)
		{
			width = Math.Min(width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width);
			height = Math.Min(height, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
			if (Game1.graphics.PreferredBackBufferWidth == width && Game1.graphics.PreferredBackBufferHeight == height)
			{
				return true;
			}
			if (!Game1.CheckIsActive && Game1.startUpStage > 5)
			{
				return false;
			}
			try
			{
				Game1.graphics.PreferredBackBufferWidth = width;
				Game1.graphics.PreferredBackBufferHeight = height;
				Game1.graphics.ApplyChanges();
				Game1.screenWidth = Game1.graphics.GraphicsDevice.Viewport.Width;
				Game1.screenHeight = Game1.graphics.GraphicsDevice.Viewport.Height;
				Game1.finalTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
				Game1.mainTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
				Game1.frontLayersTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
				Game1.blurTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth / 2, Game1.graphics.PreferredBackBufferHeight / 2);
				Game1.navMapTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, 176, 112);
				Game1.spotLightTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth / 4, Game1.graphics.PreferredBackBufferHeight / 4);
				Game1.spotLightFinal = new RenderTarget2D(Game1.graphics.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
				Game1.refractTarget = new RenderTarget2D[2];
				for (int i = 0; i < 2; i++)
				{
					Game1.refractTarget[i] = new RenderTarget2D(Game1.graphics.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
				}
				Game1.bloomTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, Game1.graphics.PreferredBackBufferWidth / 2, Game1.graphics.PreferredBackBufferHeight / 2);
				Game1.graphics.GraphicsDevice.SetRenderTarget(Game1.refractTarget[0]);
				Game1.graphics.GraphicsDevice.Clear(Color.Black);
				Game1.graphics.GraphicsDevice.SetRenderTarget(null);
				Game1.graphics.GraphicsDevice.SetRenderTarget(Game1.mainTarget);
				Game1.graphics.GraphicsDevice.Clear(Color.Black);
				Game1.graphics.GraphicsDevice.SetRenderTarget(null);
				Game1.graphics.GraphicsDevice.SetRenderTarget(Game1.navMapTarget);
				Game1.graphics.GraphicsDevice.Clear(Color.White);
				Game1.graphics.GraphicsDevice.SetRenderTarget(null);
				Game1.graphics.GraphicsDevice.SetRenderTarget(Game1.spotLightTarget);
				Game1.graphics.GraphicsDevice.Clear(Color.White);
				Game1.graphics.GraphicsDevice.SetRenderTarget(null);
				Game1.graphics.GraphicsDevice.SetRenderTarget(Game1.spotLightFinal);
				Game1.graphics.GraphicsDevice.Clear(Color.Black);
				Game1.graphics.GraphicsDevice.SetRenderTarget(null);
				Game1.graphics.GraphicsDevice.SetRenderTarget(Game1.bloomTarget);
				Game1.graphics.GraphicsDevice.Clear(Color.Black);
				Game1.graphics.GraphicsDevice.SetRenderTarget(null);
			}
			catch (Exception)
			{
				return false;
			}
			Game1.settings.Resolution = new Vector2(width, height);
			Game1.hiDefScaleOffset = (float)Game1.screenWidth / 1280f;
			Game1.hiDefScaleOffset = ((float)Game1.screenWidth / 1280f + (float)Game1.screenHeight / 720f) / 2f;
			Game1.worldScale = (Game1.camera.tempScale = Game1.hiDefScaleOffset);
			Game1.standardDef = Game1.graphics.GraphicsDevice.DisplayMode.Width < 1280;
			Game1.menu.ResetMenuDimensions();
			return true;
		}

		public static List<Vector2> GetResolutions()
		{
			List<Vector2> list = new List<Vector2>();
			foreach (DisplayMode supportedDisplayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
			{
				bool flag = true;
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].X == (float)supportedDisplayMode.Width && list[i].Y == (float)supportedDisplayMode.Height)
					{
						flag = false;
						break;
					}
				}
				if (flag && supportedDisplayMode.Width >= 960 && supportedDisplayMode.Height >= 664 && supportedDisplayMode.Width <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width && supportedDisplayMode.Height <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
				{
					list.Add(new Vector2(supportedDisplayMode.Width, supportedDisplayMode.Height));
				}
			}
			return list;
		}

		public static void SelectProfile()
		{
			if (!Game1.initLoaded)
			{
				return;
			}
			Game1.awardsManager.prevGamer = null;
			Game1.IsTrial = false;
			for (int i = 0; i < 4; i++)
			{
				if (Game1.GamerServices && (!Game1.GamerServices || Guide.IsVisible))
				{
					continue;
				}
				GamePadState state = GamePad.GetState((PlayerIndex)i);
				if (state.Buttons.A != ButtonState.Pressed && state.Buttons.Start != ButtonState.Pressed)
				{
					continue;
				}
				LogicalGamer.SetPlayerIndex(LogicalGamerIndex.One, (PlayerIndex)i);
				Game1.currentGamePad = (int)LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One);
				if (Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)] == null)
				{
					if (Game1.GamerServices && !Guide.IsVisible)
					{
						Game1.menu.SetGuideTime = 0.2f;
						Game1.settings.ResetSettings();
						if (!Game1.isPCBuild)
						{
							Game1.IsTrial = Guide.IsTrialMode;
						}
						Guide.ShowSignIn(1, onlineOnly: false);
					}
				}
				else
				{
					Game1.awardsManager.prevGamer = Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)];
					AwardsManager.SetRichPresence("ViewingMenus");
					if (!Game1.isPCBuild)
					{
						Game1.IsTrial = Guide.IsTrialMode;
					}
					if (Game1.GamerServices && !Game1.IsTrial)
					{
						Game1.SelectStorage();
					}
				}
				break;
			}
			if (Game1.isPCBuild && Game1.pcManager.CheckInitInput())
			{
				LogicalGamer.SetPlayerIndex(LogicalGamerIndex.One, PlayerIndex.One);
				Game1.currentGamePad = (int)LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One);
				Game1.menu.SetGuideTime = 0.2f;
				Game1.storage.GetDevice(LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One));
			}
		}

		public static void SelectStorage()
		{
			if (Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)] != null && (!Game1.GamerServices || (Game1.GamerServices && !Guide.IsVisible && !Guide.IsTrialMode)))
			{
				Game1.menu.SetGuideTime = 0.2f;
				Game1.storage.GetDevice(LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One));
			}
		}

		protected override void Update(GameTime gameTime)
		{
			if (Game1.skipFrame > 1)
			{
				Game1.skipFrame = 0;
			}
			if (Game1.longSkipFrame > 3)
			{
				Game1.longSkipFrame = 0;
			}
			if (Game1.halfSecFrame > 30)
			{
				Game1.halfSecFrame = 0;
			}
			Game1.HudTime = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 0.06f);
			if (Game1.loadFrame < 2f)
			{
				Game1.loadFrame += Game1.HudTime * 36f;
			}
			if (Game1.loadingTime < 6f)
			{
				Game1.loadingTime += Game1.HudTime * 20f;
			}
			if (!Game1.initLoaded)
			{
				return;
			}
			if (Game1.setBlurScene > -2f && Game1.events.blurOn && Game1.settings.DepthOfField)
			{
				Game1.blurScene = MathHelper.Clamp(Game1.blurScene + Game1.HudTime * 2f, 0f, Game1.setBlurScene);
				base.IsFixedTimeStep = true;
			}
			else if (Game1.blurScene > -2f)
			{
				Game1.blurScene -= Game1.HudTime * 4f;
				if (Game1.blurScene < -2f)
				{
					Game1.blurScene = -2f;
					if (Game1.settings.UnlockedTime)
					{
						base.IsFixedTimeStep = false;
					}
				}
			}
			if (!base.IsActive)
			{
				base.IsFixedTimeStep = true;
			}
			Game1.setBlurScene = -2f;
			Sound.Update(Game1.character[0]);
			Game1.particleThreadSec.AddTask(new ThreadTask(new ThreadTaskDelegate(UpdateCharacters)));
			Game1.skipFrame++;
			Game1.longSkipFrame++;
			Game1.halfSecFrame++;
			Game1.hudThread.AddTask(new ThreadTask(new ThreadTaskDelegate(UpdateHud)));
			Game1.hud.Update(Game1.pManager);
			if (Game1.longSkipFrame == 1)
			{
				Game1.storage.Update();
			}
			Game1.CheckIsActive = base.IsActive;
			if (Game1.isPCBuild)
			{
				Game1.pcManager.Update(Game1.graphics);
			}
			Game1.FrameTime = 0f;
			switch (Game1.gameMode)
			{
			case GameModes.Game:
				this.UpdateGame(gameTime);
				break;
			case GameModes.MainMenu:
				Game1.menu.Update(Game1.pManager);
				Game1.blurScene = 1f;
				break;
			case GameModes.WorldMap:
				Game1.navManager.worldMap.Update(Game1.pManager, Game1.HudTime);
				break;
			case GameModes.Quit:
				base.GraphicsDevice.Clear(Color.Black);
				Game1.loadingThread.Kill();
				Game1.particleThreadMain.Kill();
				Game1.particleThreadSec.Kill();
				VibrationManager.Rumble(Game1.currentGamePad, 0f);
				base.Exit();
				break;
			case GameModes.UpSell:
				Game1.upsellManager.Update(Game1.HudTime);
				break;
			}
			Game1.stats.Update(gameTime, Game1.FrameTime);
			base.Update(gameTime);
		}

		protected void UpdateGame(GameTime gameTime)
		{
			if (Game1.hud.inventoryState == InventoryState.None && !Game1.hud.isPaused && Game1.menu.prompt != promptDialogue.SkipEvent && Game1.menu.menuMode == MenuMode.None && Game1.cutscene.SceneType != CutsceneType.Video)
			{
				if (Game1.map.GetTransVal() < 0.9f)
				{
					Game1.FrameTime = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 0.05f);
				}
				else
				{
					Game1.FrameTime = 0f;
				}
				if (Game1.SlowTime > 0f)
				{
					Game1.SlowTime -= Game1.FrameTime;
					Game1.FrameTime /= 10f;
				}
				Game1.map.Update(Game1.pManager, Game1.character, Game1.FrameTime, Game1.worldScale);
				Game1.camera.Update(Game1.character, Game1.map, Game1.FrameTime, updateViewPoint: true);
				bool flag = Game1.map.GetTransVal() < 0.8f && Game1.FrameTime > 0f;
				if (flag)
				{
					Game1.dManager.UpdateDestructables(Game1.FrameTime, Game1.map, Game1.cManager, Game1.character);
					Game1.events.Update();
					if (Game1.hud.canInput && Game1.hud.shopType == ShopType.None)
					{
						Game1.character[0].PlayerInput(Game1.currentGamePad);
					}
					Game1.character[0].Update(Game1.map, Game1.character, Game1.FrameTime, Game1.dManager);
				}
				Game1.particleThreadSec.AddTask(new ThreadTask(new ThreadTaskDelegate(UpdateParticles)));
				if (flag)
				{
					for (int i = 1; i < Game1.character.Length; i++)
					{
						if (Game1.character[i].Exists >= CharExists.Exists)
						{
							Game1.character[i].Update(Game1.map, Game1.character, Game1.FrameTime, Game1.dManager);
						}
					}
					Game1.pManager.UpdateCritical(Game1.FrameTime, Game1.map, Game1.character);
					if (Game1.hud.unlockState > 0)
					{
						Game1.UpdateParticlesMain();
					}
					else
					{
						Game1.particleThreadMain.AddTask(new ThreadTask(new ThreadTaskDelegate(UpdateParticlesMain)));
					}
				}
			}
			else
			{
				Game1.FrameTime = 0f;
			}
			if (Game1.cutscene.SceneType != 0)
			{
				Game1.cutscene.Update(Game1.HudTime);
			}
		}

		private static void UpdateHud()
		{
			Game1.map.UpdateVideo(Game1.FrameTime);
			Game1.hud.UpdateThread(Game1.pManager, Game1.character);
			VibrationManager.Update();
		}

		private static void UpdateParticlesMain()
		{
			Game1.hud.UpdateFidget(Game1.pManager);
			Game1.pManager.UpdateParticles(Game1.FrameTime, Game1.map, Game1.character, 5);
			Game1.hud.KeyUpOpen = false;
		}

		private static void UpdateParticles()
		{
			Game1.wManager.UpdateWeather(Game1.pManager, Game1.map, Game1.map.path);
			for (int i = 2; i < Game1.layer; i++)
			{
				if (i != 5)
				{
					Game1.pManager.UpdateParticles(Game1.FrameTime, Game1.map, Game1.character, i);
				}
			}
		}

		private static void UpdateCharacters()
		{
			if (Game1.gameMode == GameModes.Game && Game1.hud.inventoryState == InventoryState.None && Game1.map.GetTransVal() <= 0f)
			{
				for (int num = Game1.character.Length - 1; num > -1; num--)
				{
					if (Game1.character[num].Exists >= CharExists.Exists)
					{
						Game1.character[num].UpdateThread(Game1.character, Game1.FrameTime);
					}
				}
			}
			Game1.pManager.UpdateParticles(Game1.FrameTime, Game1.map, Game1.character, 9);
		}

		public static void InitUpsell(bool quitting)
		{
			if (Game1.GamerServices && Game1.IsTrial)
			{
				Game1.upsellManager = new UpsellManager(quitting);
			}
		}

		public static void InitCreditsScroll()
		{
			Game1.creditsScroll = new CreditScroll();
			Game1.gameMode = GameModes.CreditsScroll;
		}

		protected override void Draw(GameTime gameTime)
		{
			if (Game1.gameMode == GameModes.Startup)
			{
				this.DrawStartup();
				return;
			}
			Game1.drawState = DrawState.Normal;
			if (Game1.menu.menuMode == MenuMode.Help)
			{
				Game1.menu.DrawScrollSource(Game1.graphics.GraphicsDevice, credits: false);
			}
			else if (Game1.menu.menuMode == MenuMode.Credits)
			{
				Game1.menu.DrawScrollSource(Game1.graphics.GraphicsDevice, credits: true);
			}
			else if (Game1.hud.inventoryState == InventoryState.Quests)
			{
				Game1.hud.DrawQuestScrollSource(base.GraphicsDevice);
			}
			switch (Game1.gameMode)
			{
			case GameModes.MainMenu:
				Game1.menu.Draw();
				break;
			case GameModes.WorldMap:
				Game1.navManager.worldMap.Draw(Game1.pManager, Game1.particlesTex);
				break;
			case GameModes.UpSell:
				Game1.upsellManager.Draw(Game1.spriteBatch, Game1.graphics.GraphicsDevice, Game1.nullTex);
				break;
			case GameModes.Quit:
				base.GraphicsDevice.Clear(Color.Black);
				break;
			default:
			{
				if (Game1.hud.dialogueState == DialogueState.Active && Game1.hud.dialogue.topEdge < (float)Game1.screenHeight)
				{
					Game1.hud.dialogue.DrawDialogueSource(Game1.graphics.GraphicsDevice, Game1.particlesTex);
				}
				RenderMode renderMode = Game1.renderMode;
				if (renderMode != RenderMode.Panels)
				{
					this.DrawGame(gameTime, VibrationManager.Blast.value);
				}
				else
				{
					this.DrawPanels();
				}
				if (Game1.cutscene.SceneType != 0)
				{
					Game1.cutscene.Draw(Game1.spriteBatch, Game1.nullTex);
				}
				break;
			}
			case GameModes.Startup:
				this.DrawStartup();
				break;
			case GameModes.CreditsScroll:
				Game1.creditsScroll.Draw(Game1.spriteBatch, Game1.graphics.GraphicsDevice, Game1.HudTime, Game1.nullTex);
				break;
			}
			Game1.pcManager.ClearMouseInput();
			Game1.drawState = DrawState.None;
		}

		private void DrawStartup()
		{
			base.GraphicsDevice.Clear(Color.Black);
			if (Game1.SkipMenu)
			{
				return;
			}
			Game1.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied);
			Game1.startUpTimer -= Game1.HudTime;
			if (Game1.startUpStage == 0)
			{
				if (Game1.startUpTimer < 0f)
				{
					Game1.startUpStage++;
					Game1.startUpTimer = 0.5f;
				}
			}
			else if (Game1.startUpStage == 1)
			{
				Game1.spriteBatch.Draw(Game1.splashTex, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, Game1.splashTex.Width, Game1.splashTex.Height), new Color(1f, 1f, 1f, 1f - Game1.startUpTimer * 2f), 0f, new Vector2(Game1.splashTex.Width, Game1.splashTex.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				if (Game1.startUpTimer < 0f)
				{
					Game1.startUpStage++;
					Game1.startUpTimer = 5f;
				}
			}
			else if (Game1.startUpStage == 2)
			{
				Game1.spriteBatch.Draw(Game1.splashTex, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, Game1.splashTex.Width, Game1.splashTex.Height), Color.White, 0f, new Vector2(Game1.splashTex.Width, Game1.splashTex.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				if (Game1.startUpTimer < 0f)
				{
					Game1.startUpStage++;
					if (Game1.isPCBuild)
					{
						Game1.startUpStage = 6;
					}
					Game1.startUpTimer = 0.5f;
				}
			}
			else if (Game1.startUpStage == 3)
			{
				Game1.spriteBatch.Draw(Game1.splashTex, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, Game1.splashTex.Width, Game1.splashTex.Height), new Color(1f, 1f, 1f, Game1.startUpTimer * 2f), 0f, new Vector2(Game1.splashTex.Width, Game1.splashTex.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				if (Game1.startUpTimer < 0f)
				{
					Game1.splashTex = Game1.TitleContent.Load<Texture2D>("gfx/splash/xbla_logo");
					Game1.startUpStage++;
					Game1.startUpTimer = 0.5f;
				}
			}
			else if (Game1.startUpStage == 4)
			{
				Game1.spriteBatch.Draw(Game1.splashTex, new Vector2(Game1.screenWidth / 2 - 640, Game1.screenHeight / 2 - 360), new Color(1f, 1f, 1f, 1f - Game1.startUpTimer * 2f));
				if (Game1.startUpTimer < 0f)
				{
					Game1.startUpStage++;
					Game1.startUpTimer = 3f;
				}
			}
			else if (Game1.startUpStage == 5)
			{
				Game1.spriteBatch.Draw(Game1.splashTex, new Vector2(Game1.screenWidth / 2 - 640, Game1.screenHeight / 2 - 360), Color.White);
				if (Game1.startUpTimer < 0f)
				{
					Game1.startUpStage++;
					Game1.startUpTimer = 0.5f;
				}
			}
			else if (Game1.startUpStage == 6)
			{
				if (Game1.isPCBuild)
				{
					Game1.spriteBatch.Draw(Game1.splashTex, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, Game1.splashTex.Width, Game1.splashTex.Height), new Color(1f, 1f, 1f, Game1.startUpTimer * 2f), 0f, new Vector2(Game1.splashTex.Width, Game1.splashTex.Height) / 2f, Math.Max(1f, Game1.hiDefScaleOffset), SpriteEffects.None, 0f);
				}
				else
				{
					Game1.spriteBatch.Draw(Game1.splashTex, new Vector2(Game1.screenWidth / 2 - 640, Game1.screenHeight / 2 - 360), new Color(1f, 1f, 1f, Game1.startUpTimer * 2f));
				}
				if (Game1.startUpTimer < 0f && Game1.video != null)
				{
					Game1.videoPlayer.IsLooped = false;
					Game1.videoPlayer.Play(Game1.video);
					Game1.videoPlayer.Volume = Sound.masterMusicVolume * 0.5f;
					Game1.startUpStage++;
					Game1.startUpTimer = 1f;
					Game1.loadingTime = 0f;
				}
			}
			else
			{
				if (Game1.videoPlayer != null && Game1.videoPlayer.State == MediaState.Playing)
				{
					Game1.videoPlayer.Volume = Sound.masterMusicVolume * 0.5f;
					Game1.spriteBatch.Draw(Game1.videoPlayer.GetTexture(), new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f, new Rectangle(0, 0, 1280, 720), Color.White, 0f, new Vector2(640f, 360f), Game1.hiDefScaleOffset, SpriteEffects.None, 0f);
				}
				else
				{
					Game1.spriteBatch.Draw(Game1.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.Black);
				}
				if (Game1.initLoaded)
				{
					for (int i = 0; i < 4; i++)
					{
						GamePadState state = GamePad.GetState((PlayerIndex)i);
						if (state.Buttons.A == ButtonState.Pressed || state.Buttons.Start == ButtonState.Pressed)
						{
							Game1.videoPlayer.Stop();
							Sound.PlayCue("menu_confirm");
							Game1.menu.SkipToStartPage();
						}
					}
					if (Game1.isPCBuild && Game1.pcManager.CheckInitInput())
					{
						Game1.videoPlayer.Stop();
						Sound.PlayCue("menu_confirm");
						Game1.menu.SkipToStartPage();
					}
					if (Game1.videoPlayer.State == MediaState.Stopped)
					{
						Game1.gameMode = GameModes.MainMenu;
					}
				}
				else if (Game1.videoPlayer.State == MediaState.Stopped)
				{
					Game1.spriteBatch.End();
					Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
					Game1.DrawLoad(Game1.spriteBatch, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f);
				}
			}
			Game1.spriteBatch.End();
		}

		public static void DrawLoad(SpriteBatch spriteBatch, Vector2 loc)
		{
			if (Game1.loadFrame > 1f)
			{
				Game1.loadFrame = 0f;
				Game1.loadAnimFrame++;
				if (Game1.loadAnimFrame > 31)
				{
					Game1.loadAnimFrame = 0;
				}
			}
			Color color = new Color(1f, 1f, 1f, Game1.loadingTime / 6f);
			spriteBatch.Draw(Game1.loadTex, loc, new Rectangle(Game1.loadAnimFrame * 50, 0, 50, 50), color, 0f, new Vector2(25f, 25f), new Vector2(2f, 1f) * 1.5f, SpriteEffects.None, 0f);
			if (Game1.smallText != null)
			{
				Game1.smallText.Color = color;
				if (Game1.gameMode != GameModes.Game || Game1.events == null || Game1.events.currentEvent < 70)
				{
					Game1.smallText.DrawOutlineText(loc + new Vector2(-100f, 40f), Strings_MainMenu.Loading, 0.8f, 200, TextAlign.Center, fullOutline: true);
				}
			}
		}

		private void DrawDark()
		{
			if (!(Game1.worldDark > 0f) || Game1.skipFrame <= 1)
			{
				return;
			}
			Game1.lightMaskRot += Game1.FrameTime * 4f;
			base.GraphicsDevice.SetRenderTarget(Game1.spotLightTarget);
			base.GraphicsDevice.Clear(new Color(1f - Game1.worldDark, 1f - Game1.worldDark, 1f - Game1.worldDark, 1f));
			Game1.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
			float num = 1.2f;
			if (Game1.stats.currentRingLeft == 249 || Game1.stats.currentRingRight == 249)
			{
				num = Rand.GetRandomFloat(2.6f, 3f);
			}
			Game1.spriteBatch.Draw(Game1.particlesTex[2], ((Game1.character[0].Location - new Vector2(0f, 120f)) * Game1.worldScale - Game1.Scroll) / 4f, new Rectangle(4000, 2796, 95, 95), Color.White * 0.8f, Game1.lightMaskRot, new Vector2(48f, 48f), Game1.worldScale * num, SpriteEffects.None, 1f);
			Game1.pManager.DrawLightSources(Game1.spriteBatch, Game1.particlesTex, Game1.worldScale, Game1.lightMaskRot);
			Game1.dManager.DrawLightSources(Game1.particlesTex, Game1.worldScale, Game1.lightMaskRot);
			for (int i = 1; i < Game1.character.Length; i++)
			{
				if (Game1.character[i].Exists >= CharExists.Exists && Game1.character[i].MaskGlow > 0f)
				{
					float num2 = Game1.character[i].MaskGlow + 0.75f;
					if (Game1.character[i].DyingFrame != -1f)
					{
						num2 *= 1f - Game1.character[i].DyingFrame / 1f;
					}
					Game1.spriteBatch.Draw(Game1.particlesTex[2], ((Game1.character[i].Location - new Vector2(0f, Game1.character[i].Height / 2)) * Game1.worldScale - Game1.Scroll) / 4f, new Rectangle(4000, 2796, 95, 95), Color.White * (num2 - 0.5f) * 2f, Game1.lightMaskRot + (float)i, new Vector2(48f, 48f), Game1.worldScale * num2, SpriteEffects.None, 1f);
				}
			}
			Game1.spriteBatch.End();
			base.GraphicsDevice.SetRenderTarget(null);
		}

		private RenderTarget2D MainTarget()
		{
			if (Game1.worldDark == 0f)
			{
				return Game1.mainTarget;
			}
			return Game1.spotLightFinal;
		}

		protected void DrawGame(GameTime gameTime, float blastValue)
		{
			Game1.hud.DrawMapTarget(Game1.spriteBatch, base.GraphicsDevice, Game1.navMapTarget);
			if (Game1.FrameTime > 0f && Game1.map.GetTransVal() < 1f)
			{
				this.DrawDark();
				if (Game1.settings.Bloom)
				{
					if ((Game1.map.refractType == 1 && Game1.skipFrame > 1) || Game1.map.refractType == 2)
					{
						Game1.refractive = true;
						base.GraphicsDevice.SetRenderTarget(Game1.refractTarget[0]);
						base.GraphicsDevice.Clear(Color.Black);
						Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
						Game1.map.Draw(Game1.spriteBatch, 4, 5, Game1.worldScale, Game1.pManager, Game1.particlesTex);
						Game1.pManager.DrawMapRefractParticles(Game1.particlesTex, Game1.worldScale, 4);
						Game1.spriteBatch.Draw(Game1.nullTex, new Rectangle(-10, -10, 1, 1), Color.Black);
						Game1.spriteBatch.End();
						base.GraphicsDevice.SetRenderTarget(null);
						Game1.refractive = false;
					}
					Game1.refractive = true;
					base.GraphicsDevice.SetRenderTarget(Game1.refractTarget[1]);
					base.GraphicsDevice.Clear(Color.Black);
					Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
					Game1.drawState = DrawState.Character;
					if (Game1.character[0].refracting)
					{
						Game1.character[0].Draw(Game1.spriteBatch, Game1.particlesTex, specialMode: true);
					}
					for (int i = 1; i < Game1.maxRefractingChars; i++)
					{
						if (Game1.character[i].Exists > CharExists.Init && Game1.character[i].refracting)
						{
							Game1.character[i].Ai.Draw(Game1.spriteBatch, Game1.particlesTex, specialMode: true);
						}
					}
					Game1.drawState = DrawState.Normal;
					Game1.maxRefractingChars = -1;
					Game1.pManager.DrawRefractParticles(Game1.particlesTex, Game1.worldScale, 5);
					Game1.wManager.DrawRegions(Game1.spriteBatch, Game1.map, Game1.particlesTex, Game1.worldScale, refractive: true, additive: false);
					if (Game1.map.fgRefract > 0)
					{
						Game1.map.Draw(Game1.spriteBatch, 6, Game1.map.fgRefract + 6, Game1.worldScale, Game1.pManager, Game1.particlesTex);
					}
					Game1.pManager.DrawMapRefractParticles(Game1.particlesTex, Game1.worldScale, 6);
					Game1.spriteBatch.Draw(Game1.nullTex, new Rectangle(-10, -10, 1, 1), Color.Black);
					Game1.spriteBatch.End();
					base.GraphicsDevice.SetRenderTarget(null);
				}
				Game1.refractive = false;
				Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				Game1.refractive = false;
				base.GraphicsDevice.SetRenderTarget(Game1.mainTarget);
				Game1.map.Draw(Game1.spriteBatch, 0, 4, Game1.worldScale, Game1.pManager, Game1.particlesTex);
				base.GraphicsDevice.SetRenderTarget(null);
				float num = (Game1.settings.DepthOfField ? Game1.map.blurAmount : 0);
				float num2 = ((!(num > 0f)) ? 0f : (0.2f * (1f - blastValue / 0.3f)));
				num2 -= Math.Max(Game1.blurScene, 0f);
				Game1.spriteBatch.End();
				if (num2 > 0f)
				{
					base.GraphicsDevice.SetRenderTarget(Game1.blurTarget);
					Game1.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Opaque);
					Game1.spriteBatch.Draw(Game1.mainTarget, new Rectangle(0, 0, Game1.screenWidth / 2, Game1.screenHeight / 2), Color.White);
					Game1.spriteBatch.End();
					Game1.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied);
					if (num == 1f)
					{
						for (int j = 0; j < 2; j++)
						{
							for (int k = 0; k < 2; k++)
							{
								Game1.spriteBatch.Draw(Game1.mainTarget, new Rectangle(j * 2 - 1, k * 2 - 1, Game1.screenWidth / 2, Game1.screenHeight / 2), new Color(1f, 1f, 1f, num2));
							}
						}
					}
					else if (num == 2f)
					{
						for (int l = 0; l < 2; l++)
						{
							for (int m = 0; m < 2; m++)
							{
								Game1.spriteBatch.Draw(Game1.mainTarget, new Vector2((float)(l * 3) - 1.5f, (float)(m * 3) - 1.5f), new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(1f, 1f, 1f, num2), 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
							}
						}
					}
					Game1.spriteBatch.End();
					base.GraphicsDevice.SetRenderTarget(null);
				}
				base.GraphicsDevice.SetRenderTarget(Game1.frontLayersTarget);
				Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
				if (Game1.map.refractType > 0 && Game1.settings.Bloom)
				{
					base.GraphicsDevice.Textures[1] = Game1.refractTarget[0];
					Game1.refractEffect.CurrentTechnique.Passes[0].Apply();
				}
				if (num2 > 0f)
				{
					Game1.spriteBatch.Draw(Game1.blurTarget, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.White);
				}
				else
				{
					Game1.spriteBatch.Draw(Game1.mainTarget, Vector2.Zero, Color.White);
				}
				Game1.spriteBatch.End();
				Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				Game1.map.Draw(Game1.spriteBatch, 4, 6, Game1.worldScale, Game1.pManager, Game1.particlesTex);
				Game1.dManager.DrawDestructables(Game1.particlesTex);
				Game1.drawState = DrawState.Character;
				if (!Game1.stats.inFront)
				{
					Game1.character[0].Draw(Game1.spriteBatch, Game1.particlesTex, specialMode: false);
				}
				for (int n = 1; n < Game1.character.Length; n++)
				{
					if (Game1.character[n].Exists > CharExists.Init)
					{
						Game1.character[n].Ai.Draw(Game1.spriteBatch, Game1.particlesTex, specialMode: false);
					}
				}
				if (Game1.stats.inFront)
				{
					Game1.character[0].Draw(Game1.spriteBatch, Game1.particlesTex, specialMode: false);
				}
				if (Game1.longSkipFrame > 1 && Game1.characterGlow > -1)
				{
					Game1.spriteBatch.End();
					Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
					if (Game1.character[0].glowing > 0f)
					{
						Game1.character[0].Draw(Game1.spriteBatch, Game1.particlesTex, specialMode: true);
					}
					for (int num3 = 1; num3 < Game1.characterGlow; num3++)
					{
						if (Game1.character[num3].Exists > CharExists.Init && Game1.character[num3].glowing > 0f)
						{
							Game1.character[num3].Ai.Draw(Game1.spriteBatch, Game1.particlesTex, specialMode: true);
						}
					}
					Game1.spriteBatch.End();
					Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
					Game1.characterGlow = -1;
				}
				Game1.drawState = DrawState.Normal;
				Game1.pManager.DrawParticles(Game1.particlesTex, Game1.worldScale, 5);
				Game1.spriteBatch.End();
				base.GraphicsDevice.SetRenderTarget(Game1.mainTarget);
				base.GraphicsDevice.Textures[1] = Game1.refractTarget[1];
				Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
				if (Game1.settings.Bloom)
				{
					Game1.refractEffect.CurrentTechnique.Passes[0].Apply();
				}
				Game1.spriteBatch.Draw(Game1.frontLayersTarget, Vector2.Zero, Color.White);
				Game1.spriteBatch.End();
				base.GraphicsDevice.Textures[1] = null;
				Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				Game1.map.Draw(Game1.spriteBatch, 6, 9, Game1.worldScale, Game1.pManager, Game1.particlesTex);
				Game1.spriteBatch.End();
				base.GraphicsDevice.SetRenderTarget(null);
				if (Game1.worldDark > 0f)
				{
					base.GraphicsDevice.SetRenderTarget(Game1.spotLightFinal);
					base.GraphicsDevice.Clear(new Color(0f, 0f, 0f, 0f));
					Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
					Game1.maskEffect.CurrentTechnique.Passes[0].Apply();
					Game1.maskEffect.Parameters["MaskTexture"].SetValue(Game1.spotLightTarget);
					Game1.spriteBatch.Draw(Game1.mainTarget, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.White);
					Game1.spriteBatch.End();
					base.GraphicsDevice.SetRenderTarget(null);
				}
				if (!Game1.settings.Bloom)
				{
					blastValue = 0f;
				}
				else if (blastValue < 1f || Game1.longSkipFrame == 1)
				{
					base.GraphicsDevice.SetRenderTarget(Game1.bloomTarget);
					Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
					Game1.bloomEffect.CurrentTechnique.Passes[0].Apply();
					Game1.spriteBatch.Draw(this.MainTarget(), new Rectangle(0, 0, Game1.screenWidth / 2, Game1.screenHeight / 2), Color.White);
					Game1.spriteBatch.End();
					base.GraphicsDevice.SetRenderTarget(null);
				}
			}
			if (Game1.settings.Bloom)
			{
				Game1.filterEffect.Parameters["saturation"].SetValue(Game1.wManager.weatherSaturation);
				Game1.filterEffect.Parameters["r"].SetValue(Game1.wManager.weatherColor.X);
				Game1.filterEffect.Parameters["g"].SetValue(Game1.wManager.weatherColor.Y);
				Game1.filterEffect.Parameters["b"].SetValue(Game1.wManager.weatherColor.Z);
				Game1.filterEffect.Parameters["brite"].SetValue(Game1.wManager.weatherColor.W);
			}
			else
			{
				Game1.filterEffect.Parameters["saturation"].SetValue(1);
				Game1.filterEffect.Parameters["r"].SetValue(1);
				Game1.filterEffect.Parameters["g"].SetValue(1);
				Game1.filterEffect.Parameters["b"].SetValue(1);
				Game1.filterEffect.Parameters["brite"].SetValue(0.05f);
			}
			Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
			Game1.filterEffect.CurrentTechnique.Passes[0].Apply();
			Game1.spriteBatch.Draw(this.MainTarget(), new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.White);
			Game1.spriteBatch.End();
			if (Game1.blurScene > 0f)
			{
				Game1.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied);
				for (int num4 = 0; num4 < 2; num4++)
				{
					for (int num5 = 0; num5 < 2; num5++)
					{
						Game1.spriteBatch.Draw(this.MainTarget(), new Vector2((float)(num4 * 3) - 1.5f, (float)(num5 * 3) - 1.5f), new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(1f, 1f, 1f, 0.4f * Game1.blurScene), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
					}
				}
				Game1.spriteBatch.End();
			}
			Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
			if (blastValue < 1f && Game1.settings.Bloom)
			{
				Game1.spriteBatch.Draw(Game1.bloomTarget, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(1f, 1f, 1f, Game1.wManager.weatherBloom * (1f - Game1.wManager.lightningBloom) * (1f - blastValue)));
			}
			if (Game1.wManager.lightningBloom > 0f)
			{
				Game1.spriteBatch.Draw(Game1.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(1f, 1f, 1f, Game1.wManager.lightningBloom));
			}
			if (blastValue > 0f && Game1.settings.Bloom)
			{
				Vector2 vector = VibrationManager.Blast.center * Game1.worldScale - Game1.Scroll;
				for (int num6 = 0; num6 < (int)(blastValue * 20f); num6++)
				{
					Game1.spriteBatch.Draw(this.MainTarget(), vector, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0.5f, 0.5f, 0.5f, blastValue / (float)(num6 + 1)), 0f, vector, 1f + (float)(num6 + 1) / 100f, SpriteEffects.None, 1f);
				}
			}
			Game1.spriteBatch.End();
			Game1.hud.Draw(Game1.pManager);
		}

		protected void DrawPanels()
		{
			if (!Game1.map.path.StartsWith("snow30"))
			{
				Game1.renderMode = RenderMode.Normal;
			}
			Game1.hud.DrawMapTarget(Game1.spriteBatch, base.GraphicsDevice, Game1.navMapTarget);
			Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			base.GraphicsDevice.SetRenderTarget(Game1.frontLayersTarget);
			Game1.map.Draw(Game1.spriteBatch, 0, 6, Game1.worldScale, Game1.pManager, Game1.particlesTex);
			Game1.dManager.DrawDestructables(Game1.particlesTex);
			Game1.drawState = DrawState.Character;
			for (int i = 1; i < Game1.character.Length; i++)
			{
				if (Game1.character[i].Exists >= CharExists.Exists)
				{
					Game1.character[i].Draw(Game1.spriteBatch, Game1.particlesTex, specialMode: false);
				}
			}
			Game1.character[0].Draw(Game1.spriteBatch, Game1.particlesTex, specialMode: false);
			Game1.drawState = DrawState.Normal;
			Game1.pManager.DrawParticles(Game1.particlesTex, Game1.worldScale, 5);
			Game1.spriteBatch.End();
			Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			Game1.map.Draw(Game1.spriteBatch, 6, 9, Game1.worldScale, Game1.pManager, Game1.particlesTex);
			base.GraphicsDevice.SetRenderTarget(null);
			Game1.spriteBatch.End();
			Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
			base.GraphicsDevice.SetRenderTarget(Game1.mainTarget);
			float num = 0.25f;
			Game1.spriteBatch.Draw(Game1.frontLayersTarget, Vector2.Zero, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.White, 0f, Vector2.Zero, num, SpriteEffects.None, 0f);
			base.GraphicsDevice.SetRenderTarget(null);
			Game1.spriteBatch.End();
			Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
			Game1.spriteBatch.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			base.GraphicsDevice.Clear(Color.Black);
			int num2 = 6;
			Vector2 vector = -new Vector2(Game1.screenWidth - num2, Game1.screenHeight - num2) / 2f * num * 0.5f;
			vector = new Vector2(Game1.screenWidth, Game1.screenHeight) * num / 3f * 0.5f;
			vector = new Vector2(Game1.screenWidth, Game1.screenHeight) * num * 0.5f;
			int num3 = (int)((float)Game1.screenWidth * num) + num2;
			int num4 = (int)((float)Game1.screenHeight * num) + num2;
			int num5 = 2;
			int num6 = (int)((float)Game1.mainTarget.Width * num * 0.333f) - num5;
			int num7 = (int)((float)Game1.mainTarget.Height * num * 0.333f) - num5;
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					Game1.spriteBatch.Draw(Game1.nullTex, vector + new Vector2(j * num3 - num5, k * num4 - num5), new Rectangle(0, 0, num3 - num5 * 2 - num2, num4 - num5 * 2 - num2), Color.Gray);
				}
			}
			SpriteEffects effects = ((Game1.stats.gameDifficulty >= 2) ? ((Game1.stats.gameDifficulty < 3) ? SpriteEffects.FlipHorizontally : ((SpriteEffects)255)) : SpriteEffects.None);
			Game1.spriteBatch.Draw(Game1.mainTarget, vector + new Vector2(num3 * 2, num4 * 2), new Rectangle(0, 0, num6, num7), Color.White, 0f, Vector2.Zero, 3f, effects, 0f);
			Game1.spriteBatch.Draw(Game1.mainTarget, vector + new Vector2(num3 * 2, num4), new Rectangle(0, num7, num6, num7), Color.White, 0f, Vector2.Zero, 3f, effects, 0f);
			Game1.spriteBatch.Draw(Game1.mainTarget, vector + new Vector2(0f, 0f), new Rectangle(0, num7 * 2, num6, num7), Color.White, 0f, Vector2.Zero, 3f, effects, 0f);
			Game1.spriteBatch.Draw(Game1.mainTarget, vector + new Vector2(0f, num4 * 2), new Rectangle(num6, 0, num6, num7), Color.White, 0f, Vector2.Zero, 3f, effects, 0f);
			Game1.spriteBatch.Draw(Game1.mainTarget, vector + new Vector2(num3, num4), new Rectangle(num6, num7, num6, num7), Color.White, 0f, Vector2.Zero, 3f, effects, 0f);
			Game1.spriteBatch.Draw(Game1.mainTarget, vector + new Vector2(num3 * 2, 0f), new Rectangle(num6, num7 * 2, num6, num7), Color.White, 0f, Vector2.Zero, 3f, effects, 0f);
			Game1.spriteBatch.Draw(Game1.mainTarget, vector + new Vector2(num3, 0f), new Rectangle(num6 * 2, 0, num6, num7), Color.White, 0f, Vector2.Zero, 3f, effects, 0f);
			Game1.spriteBatch.Draw(Game1.mainTarget, vector + new Vector2(0f, num4), new Rectangle(num6 * 2, num7, num6, num7), Color.White, 0f, Vector2.Zero, 3f, effects, 0f);
			Game1.spriteBatch.Draw(Game1.mainTarget, vector + new Vector2(num3, num4 * 2), new Rectangle(num6 * 2, num7 * 2, num6, num7), Color.White, 0f, Vector2.Zero, 3f, effects, 0f);
			Game1.spriteBatch.End();
			if (Game1.map.GetTransVal() > 0f)
			{
				Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				Game1.spriteBatch.Draw(Game1.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, Game1.map.GetTransVal() + 0.1f));
				Game1.spriteBatch.End();
			}
			if (Game1.hud.isPaused || Game1.events.anyEvent || Game1.hud.inventoryState != 0 || Game1.menu.prompt != promptDialogue.None || Game1.hud.unlockState > 0)
			{
				Game1.hud.Draw(Game1.pManager);
			}
		}

		public static void TrialInit()
		{
			Game1.stats.ResetGame(Game1.pManager, Game1.character);
			Game1.settings.ResetSettings();
			if (Game1.Convention)
			{
				Game1.stats.gameDifficulty = 1;
				Sound.SetMusicVolume(1f);
				Sound.SetSFXVolume(1f);
			}
			Game1.events.currentEvent = 10;
			Game1.events.screenFade = Color.Black;
			Game1.events.fadeLength = (Game1.events.fadeTimer = 2f);
			Game1.settings.AutoLevelUp = true;
			for (int i = 0; i < 7; i++)
			{
				Game1.stats.XP = Game1.stats.nextLevelXP;
				Game1.stats.CheckLevelUp();
			}
			Game1.stats.XP = Game1.stats.nextLevelXP - 2800;
			Game1.settings.AutoLevelUp = false;
			for (int j = 10; j < 20; j++)
			{
				if (j != 12 && j != 14 && j != 16 && j != 17)
				{
					Game1.stats.EarnUpgrade(j, 1);
				}
			}
			Game1.stats.AcquireEquip(EquipType.RevivalStone, 4, _bluePrint: false);
			Game1.hud.miniPromptList.Clear();
			for (int k = 10; k < 20; k++)
			{
				Game1.events.sideEventAvailable[k] = false;
			}
			Game1.events.sideEventAvailable[0] = false;
			Game1.map.path = "trial00";
			Game1.map.SwitchMap(Game1.pManager, Game1.character, Game1.map.path, loading: false);
			Game1.wManager.weatherTimer = 10f;
			Game1.character[0].State = CharState.Air;
			Game1.character[0].Location = new Vector2(3020f, 1100f) * 2f;
			Game1.camera.ResetCamera(Game1.character);
			Game1.character[0].GroundCharacter();
			Game1.character[0].SetAnim("run", 0, 2);
			Game1.character[0].Trajectory = Vector2.Zero;
			Game1.character[0].HP -= 20;
			Game1.character[0].pHP = Game1.character[0].HP;
			Game1.hud.runInTime = 1f;
			Game1.navManager.RevealMap[Game1.navManager.NavPath].Updated = false;
			Game1.navManager.SetPlayerCell();
			Game1.navManager.ForceScroll(MathHelper.Clamp(Game1.hud.hudScale, 0.5f, 1.2f) * 0.5f, Game1.navManager.scrollX, Game1.navManager.scrollY);
			Game1.events.regionDisplayName = Strings_Regions.grave;
			Game1.hud.InitFidgetPrompt(FidgetPrompt.None);
			Game1.worldScale = 0.75f;
		}

		public static void ConventionUpdate(GameTime gameTime)
		{
			if (Game1.gameMode == GameModes.MainMenu)
			{
				return;
			}
			int num = 200;
			if (Game1.currentGamePad <= -1)
			{
				return;
			}
			GamePadState state = GamePad.GetState((PlayerIndex)Game1.currentGamePad);
			if (state.Buttons.A == ButtonState.Pressed || state.Buttons.B == ButtonState.Pressed || state.Buttons.X == ButtonState.Pressed || state.Buttons.Y == ButtonState.Pressed || state.Buttons.Back == ButtonState.Pressed || state.Buttons.Start == ButtonState.Pressed || state.Buttons.LeftShoulder == ButtonState.Pressed || state.Buttons.RightShoulder == ButtonState.Pressed || state.DPad.Up == ButtonState.Pressed || state.DPad.Down == ButtonState.Pressed || state.DPad.Left == ButtonState.Pressed || state.DPad.Right == ButtonState.Pressed || state.Triggers.Left > 0.5f || state.Triggers.Right > 0.5f || state.ThumbSticks.Left.Length() > 0.5f || state.ThumbSticks.Right.Length() > 0.5f)
			{
				Game1.ConventionTimer = num;
			}
			bool flag = false;
			if (state.Buttons.Back == ButtonState.Pressed && Game1.cutscene.SceneType == CutsceneType.None)
			{
				float conventionResetTimer = Game1.ConventionResetTimer;
				Game1.ConventionResetTimer -= Game1.HudTime;
				if (Game1.ConventionResetTimer < 0f && conventionResetTimer >= 0f)
				{
					flag = true;
				}
			}
			else
			{
				Game1.ConventionResetTimer = 4f;
			}
			if (Game1.stats.playerLifeState > 0)
			{
				Game1.ConventionTimer = Math.Min(Game1.ConventionTimer, 30f);
			}
			else if (Game1.ConventionTimer > 0f && Game1.cutscene.SceneType == CutsceneType.None)
			{
				Game1.ConventionTimer -= Game1.HudTime;
				if (Game1.ConventionTimer <= 0f)
				{
					flag = true;
				}
			}
			if (flag && (!Game1.GamerServices || !Guide.IsVisible))
			{
				flag = false;
				Game1.ConventionReset();
			}
		}

		public static void ConventionReset()
		{
			if (Game1.gameMode == GameModes.WorldMap)
			{
				Game1.character[0].State = CharState.Air;
				Game1.character[0].Location = new Vector2(Game1.map.leftEdge + 128f, Game1.map.leftExitPoint - 200f);
				Game1.camera.ResetCamera(Game1.character);
				Game1.character[0].GroundCharacter();
			}
			Game1.gameMode = GameModes.Game;
			Game1.cutscene.ExitCutscene();
			Sound.FadeMusicOut(1f);
			Game1.settings.ResetSettings();
			Game1.stats.gameDifficulty = 1;
			Game1.menu.ResetMenu();
			Game1.hud.ResetHud(all: true);
			Game1.storage.Write(0, -1);
			Game1.events.ClearEvent();
			Game1.events.Reset();
			Game1.events.currentEvent = 10;
			if (Game1.ConventionResetTimer <= 0f)
			{
				Game1.events.InitEvent(88, isSideEvent: true);
			}
			else
			{
				Game1.events.InitEvent(89, isSideEvent: true);
			}
			Game1.ConventionTimer = 200f;
			Game1.ConventionResetTimer = 4f;
		}

		protected void ConventionDraw()
		{
			if (Game1.gameMode != 0 && Game1.cutscene.SceneType != CutsceneType.Video)
			{
				if (Game1.ConventionResetTimer > 0f && Game1.ConventionResetTimer < 3f)
				{
					Game1.spriteBatch.Begin();
					Game1.bigText.Color = Color.White;
					Game1.bigText.DrawShadowText(new Vector2((float)Game1.screenWidth * 0.9f, (float)Game1.screenHeight * 0.9f), "Reset in " + ((int)Game1.ConventionResetTimer + 1), 1f, 1000, TextAlign.Right, outline: true);
					Game1.spriteBatch.End();
				}
				else if (Game1.ConventionTimer > 0f && Game1.ConventionTimer < 3f)
				{
					Game1.spriteBatch.Begin();
					Game1.bigText.Color = Color.White;
					Game1.bigText.DrawShadowText(new Vector2((float)Game1.screenWidth * 0.9f, (float)Game1.screenHeight * 0.9f), "Trailer in " + ((int)Game1.ConventionTimer + 1), 1f, 1000, TextAlign.Right, outline: true);
					Game1.spriteBatch.End();
				}
			}
		}
	}
}
