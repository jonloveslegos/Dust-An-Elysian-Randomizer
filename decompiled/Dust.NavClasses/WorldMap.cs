using System;
using System.Collections.Generic;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Particles;
using Dust.PCClasses;
using Dust.Strings;
using Dust.Vibration;
using Lotus.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Dust.NavClasses
{
	public class WorldMap
	{
		private Texture2D videoTexture;

		private static object syncObject = new object();

		private static SpriteBatch sprite;

		private static Texture2D worldMapTex;

		private static Texture2D worldMapUITex;

		private static Texture2D nullTex;

		private static bool worldMapAssetsLoaded;

		private static bool populatingMap;

		private static int selection;

		private static int mapView;

		private static string regionName = string.Empty;

		private static string navName = string.Empty;

		private static string prevGameMap = string.Empty;

		private static Vector2 cursorDestPos;

		private static Vector2 cursorCurPos;

		private static Vector2[] cursorTrailPos = new Vector2[40];

		private static Vector2 mapPos;

		private static Vector2 freeLocTarg;

		private static Vector2 freeLocPos;

		private static bool freeLook;

		private static float freeLookAlpha;

		public bool canReturn;

		private static float screenFade;

		private static float mapViewFade = 0f;

		private static float mapLegendFade = 0f;

		private static byte flameAnimFrame;

		private static byte availAnimFrame;

		private static byte newAnimFrame;

		private static byte cursorAnimFrame;

		private static float animFrameTime;

		private static float mapGlowTarget = 1f;

		private static float mapGlowAlpha = 1f;

		private static List<string> availableList = new List<string>();

		private static List<Vector3> entranceList;

		private static TransitionDirection entranceDir;

		private static Character[] character;

		private static Map map;

		private static bool KeyLeft;

		private static bool KeyRight;

		private static bool KeyUp;

		private static bool KeyDown;

		public bool KeySelect;

		private static bool KeyCancel;

		private static bool KeyX;

		private static bool KeyY;

		private static GamePadState curState = default(GamePadState);

		private static GamePadState prevState = default(GamePadState);

		public WorldMap(SpriteBatch _sprite, Texture2D _nullTex, Character[] _character, Map _map)
		{
			WorldMap.sprite = _sprite;
			WorldMap.character = _character;
			WorldMap.map = _map;
			WorldMap.nullTex = _nullTex;
			WorldMap.mapPos = new Vector2(Game1.screenWidth / 2 - 512, Game1.screenHeight / 2 - 288);
			WorldMap.freeLocTarg = (WorldMap.freeLocPos = Vector2.Zero);
			WorldMap.freeLook = false;
			WorldMap.freeLookAlpha = 0f;
			WorldMap.worldMapAssetsLoaded = false;
			WorldMap.cursorDestPos = (WorldMap.cursorCurPos = Vector2.Zero);
			this.canReturn = false;
			WorldMap.screenFade = 0f;
			WorldMap.selection = 0;
			WorldMap.mapView = 0;
		}

		public void InitWorldMap(ParticleManager pMan, string _curPath, bool _canReturn)
		{
			if (_canReturn && !this.CheckWorldMapReady(pMan, _curPath))
			{
				return;
			}
			WeatherAudio.Stop();
			pMan.Reset(removeWeather: true, removeBombs: true);
			Sound.ResetAmbience(null);
			Sound.ResetCCues();
			Sound.ResetPCues();
			WorldMap.character[0].UnloadTextures();
			for (int i = 1; i < WorldMap.character.Length; i++)
			{
				WorldMap.character[i].Exists = CharExists.Dead;
			}
			WorldMap.character[0].HPLossFrame = 0f;
			Game1.wManager.ResetWeather();
			Game1.events.regionDisplayName = string.Empty;
			Game1.stats.curCharge = 100f;
			Game1.gameMode = Game1.GameModes.WorldMap;
			Game1.hud.regionIntroState = 0;
			Game1.hud.miniPromptList.Clear();
			Game1.cManager.ExitScoreBoard(canRestart: true);
			Game1.cManager.currentChallenge = -1;
			WorldMap.map.transInFrame = (WorldMap.map.transOutFrame = -1f);
			WorldMap.prevGameMap = _curPath;
			this.canReturn = _canReturn;
			WorldMap.screenFade = 1f;
			WorldMap.mapViewFade = (WorldMap.mapLegendFade = (WorldMap.mapView = 0));
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadTextures), new TaskFinishedDelegate(LoadingFinished)));
			Game1.questManager.updatingQuests = true;
			Game1.questManager.UpdateQuests((int)((float)Game1.screenWidth * 0.7f) - 60);
			int num = 0;
			while (Game1.questManager.updatingQuests)
			{
				num++;
				if (num > 1000)
				{
					break;
				}
			}
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(Game1.stats.GetWorldExplored)));
			this.PopulateAvailable(moveCursor: true);
			this.PopulateControls();
			if (this.canReturn && WorldMap.map.doorStage > 0)
			{
				WorldMap.entranceDir = TransitionDirection.Intro;
			}
			Music.Play("worldmap");
			Sound.PlayPersistCue("worldmap_ambience", new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f);
			for (int j = 0; j < WorldMap.cursorTrailPos.Length; j++)
			{
				ref Vector2 reference = ref WorldMap.cursorTrailPos[j];
				reference = WorldMap.cursorCurPos;
			}
			GC.Collect();
		}

		private void ExitWorldMap(ParticleManager pMan)
		{
			WorldMap.entranceList = null;
			Game1.videoPlayer.Stop();
			Game1.GetLargeContent().Unload();
			Game1.GetInventoryContent().Unload();
			Game1.gameMode = Game1.GameModes.Game;
			Game1.worldScale = 1f;
			WorldMap.worldMapAssetsLoaded = false;
			this.videoTexture = null;
			if (WorldMap.character[0].Location.X < WorldMap.map.leftEdge + 100f)
			{
				WorldMap.character[0].Location = new Vector2(WorldMap.character[0].Location.X, WorldMap.map.rightExitPoint);
				WorldMap.character[0].Face = CharDir.Right;
			}
			else
			{
				WorldMap.character[0].Location = new Vector2(WorldMap.character[0].Location.X, WorldMap.map.leftExitPoint);
				WorldMap.character[0].Face = CharDir.Right;
			}
			Sound.StopPersistCue("worldmap_ambience");
			Game1.hud.ClearHelp();
			Game1.camera.ResetCamera(WorldMap.character);
			pMan.ResetFidget(WorldMap.character);
			WorldMap.character[0].GroundCharacter();
			WorldMap.character[0].Trajectory = Vector2.Zero;
			WorldMap.character[0].KeyLeft = (WorldMap.character[0].KeyRight = false);
			WorldMap.character[0].SetAnim("run", 0, 2);
			if (WorldMap.map.path != WorldMap.prevGameMap)
			{
				Game1.navManager.RevealMap[WorldMap.navName].Updated = false;
			}
			this.KeySelect = (WorldMap.KeyCancel = (WorldMap.KeyLeft = (WorldMap.KeyRight = (WorldMap.KeyUp = (WorldMap.KeyDown = false)))));
			Game1.navManager.SetPlayerCell();
			Game1.navManager.ForceScroll(MathHelper.Clamp(Game1.hud.hudScale, 0.5f, 1.2f) * 0.5f, Game1.navManager.scrollX, Game1.navManager.scrollY);
			Game1.navManager.ExitWorldMap();
			this.InitChapter();
		}

		private void InitChapter()
		{
			int num = -1;
			switch (WorldMap.map.regionName)
			{
			case "aurora":
				num = 1;
				break;
			case "cave":
				num = 2;
				break;
			case "grave":
				num = 3;
				break;
			case "snow":
				num = 4;
				break;
			case "lava":
				num = 5;
				break;
			}
			if (num > -1 && Game1.events.sideEventAvailable[100 + num])
			{
				Game1.events.sideEventAvailable[100 + num] = false;
				Game1.menu.InitChapter(num);
			}
		}

		private void LoadTextures()
		{
			WorldMap.worldMapAssetsLoaded = false;
			Game1.loadingTime = 0f;
			WorldMap.worldMapTex = Game1.GetLargeContent().Load<Texture2D>("gfx/ui/worldMap");
			WorldMap.worldMapUITex = Game1.GetLargeContent().Load<Texture2D>("gfx/ui/worldMapUI");
			Game1.hud.InventoryTex = Game1.GetInventoryContent().Load<Texture2D>("gfx/ui/hud_elements_" + 6);
			Game1.video = Game1.GetLargeContent().Load<Video>("video/worldmap_video_" + $"{Rand.GetRandomInt(0, 3):D2}");
			Game1.videoPlayer.IsLooped = true;
			Game1.videoPlayer.Volume = Sound.masterMusicVolume;
			Game1.videoPlayer.Volume = 0f;
			Game1.videoPlayer.Play(Game1.video);
		}

		public static void LoadingFinished(int taskId)
		{
			WorldMap.worldMapAssetsLoaded = true;
			Game1.gameMode = Game1.GameModes.WorldMap;
		}

		private void DoInput(int index)
		{
			WorldMap.KeyUp = false;
			WorldMap.KeyDown = false;
			WorldMap.KeyLeft = false;
			WorldMap.KeyRight = false;
			this.KeySelect = false;
			WorldMap.KeyCancel = false;
			WorldMap.KeyX = false;
			WorldMap.KeyY = false;
			if (Game1.isPCBuild)
			{
				Game1.pcManager.UpdateWorldMapInput(ref WorldMap.KeyLeft, ref WorldMap.KeyRight, ref WorldMap.KeyUp, ref WorldMap.KeyDown, ref this.KeySelect, ref WorldMap.KeyCancel, ref WorldMap.KeyX, ref WorldMap.KeyY);
			}
			if (Game1.currentGamePad < 0)
			{
				return;
			}
			WorldMap.curState = GamePad.GetState((PlayerIndex)index);
			if (WorldMap.curState.Buttons.A == ButtonState.Pressed && WorldMap.prevState.Buttons.A == ButtonState.Released)
			{
				this.KeySelect = true;
			}
			if (WorldMap.curState.Buttons.B == ButtonState.Pressed && WorldMap.prevState.Buttons.B == ButtonState.Released)
			{
				WorldMap.KeyCancel = true;
			}
			if (WorldMap.curState.Buttons.X == ButtonState.Pressed && WorldMap.prevState.Buttons.X == ButtonState.Released)
			{
				WorldMap.KeyX = true;
			}
			if (WorldMap.curState.Buttons.Y == ButtonState.Pressed && WorldMap.prevState.Buttons.Y == ButtonState.Released)
			{
				WorldMap.KeyY = true;
			}
			if ((WorldMap.prevState.ThumbSticks.Left.Y < 0.2f && WorldMap.curState.ThumbSticks.Left.Y > 0.2f) || (WorldMap.curState.DPad.Up == ButtonState.Pressed && WorldMap.prevState.DPad.Up == ButtonState.Released))
			{
				WorldMap.KeyUp = true;
			}
			if ((WorldMap.prevState.ThumbSticks.Left.Y > -0.2f && WorldMap.curState.ThumbSticks.Left.Y < -0.2f) || (WorldMap.curState.DPad.Down == ButtonState.Pressed && WorldMap.prevState.DPad.Down == ButtonState.Released))
			{
				WorldMap.KeyDown = true;
			}
			if ((WorldMap.curState.ThumbSticks.Left.X < -0.2f && WorldMap.prevState.ThumbSticks.Left.X > -0.2f) || (WorldMap.curState.DPad.Left == ButtonState.Pressed && WorldMap.prevState.DPad.Left == ButtonState.Released))
			{
				WorldMap.KeyLeft = true;
			}
			if ((WorldMap.curState.ThumbSticks.Left.X > 0.2f && WorldMap.prevState.ThumbSticks.Left.X < 0.2f) || (WorldMap.curState.DPad.Right == ButtonState.Pressed && WorldMap.prevState.DPad.Right == ButtonState.Released))
			{
				WorldMap.KeyRight = true;
			}
			if (this.KeySelect || WorldMap.KeyCancel)
			{
				if (WorldMap.freeLook)
				{
					this.KeySelect = (WorldMap.KeyCancel = false);
				}
				WorldMap.freeLook = false;
			}
			if (WorldMap.KeyLeft || WorldMap.KeyRight || WorldMap.KeyUp || WorldMap.KeyDown)
			{
				WorldMap.freeLook = false;
			}
			if (!WorldMap.freeLook)
			{
				WorldMap.freeLocTarg += (Vector2.Zero - WorldMap.freeLocPos) * Game1.HudTime * 4f;
			}
			WorldMap.freeLocPos += (WorldMap.freeLocTarg - WorldMap.freeLocPos) * Game1.HudTime * 8f;
			if (WorldMap.mapView == 0)
			{
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
				{
					WorldMap.freeLook = false;
					WorldMap.freeLocPos += (WorldMap.freeLocTarg - WorldMap.freeLocPos) * Game1.HudTime * 8f;
				}
				else
				{
					Vector2 vector = WorldMap.curState.ThumbSticks.Right;
					if (vector.Length() < 0.1f)
					{
						vector = Vector2.Zero;
					}
					WorldMap.freeLocTarg.X = MathHelper.Clamp(WorldMap.freeLocTarg.X - vector.X * 30f, (float)(-WorldMap.worldMapTex.Width) - WorldMap.mapPos.X + (float)Game1.screenWidth, 0f - WorldMap.mapPos.X);
					WorldMap.freeLocTarg.Y = MathHelper.Clamp(WorldMap.freeLocTarg.Y + vector.Y * 30f, (float)(-WorldMap.worldMapTex.Height) - WorldMap.mapPos.Y + (float)Game1.screenHeight, 0f - WorldMap.mapPos.Y);
					if (WorldMap.curState.ThumbSticks.Right.Length() > 0.1f)
					{
						WorldMap.freeLook = true;
					}
				}
			}
			WorldMap.prevState = WorldMap.curState;
		}

		private Vector2 GetCursorPos(string name, bool getRegion)
		{
			if (getRegion)
			{
				WorldMap.navName = name;
			}
			Vector2 zero = Vector2.Zero;
			switch (name)
			{
			case "glade":
				if (getRegion)
				{
					WorldMap.regionName = Strings_Regions.glade;
				}
				return zero + new Vector2(565f, 1319f);
			case "aurora":
				if (getRegion)
				{
					WorldMap.regionName = Strings_Regions.aurora;
				}
				return zero + new Vector2(970f, 1229f);
			case "smith":
				if (getRegion)
				{
					WorldMap.regionName = Strings_Regions.smith;
				}
				return zero + new Vector2(1230f, 1200f);
			case "forest":
				if (getRegion)
				{
					WorldMap.regionName = Strings_Regions.forest;
				}
				return zero + new Vector2(1320f, 1360f);
			case "cave":
				if (getRegion)
				{
					WorldMap.regionName = Strings_Regions.cave;
				}
				return zero + new Vector2(950f, 1367f);
			case "grave":
				if (getRegion)
				{
					WorldMap.regionName = Strings_Regions.grave;
				}
				return zero + new Vector2(680f, 1140f);
			case "sanc":
				if (getRegion)
				{
					WorldMap.regionName = Strings_Regions.sanctuary;
				}
				return zero + new Vector2(980f, 1100f);
			case "snow":
				if (getRegion)
				{
					WorldMap.regionName = Strings_Regions.snow;
				}
				return zero + new Vector2(750f, 954f);
			case "lava":
				if (getRegion)
				{
					WorldMap.regionName = Strings_Regions.lava;
				}
				return zero + new Vector2(1174f, 947f);
			case "ivy":
				if (getRegion)
				{
					WorldMap.regionName = Strings_Regions.ivy;
				}
				return zero + new Vector2(1520f, 1330f);
			case "farm":
				if (getRegion)
				{
					WorldMap.regionName = Strings_Regions.farm;
				}
				return zero + new Vector2(790f, 1260f);
			case "cove":
				if (getRegion)
				{
					WorldMap.regionName = Strings_Regions.cove;
				}
				return zero + new Vector2(1500f, 1160f);
			default:
				return zero;
			}
		}

		private void FindNearest(int keyDirection)
		{
			int num = -1;
			float num2 = 0f;
			for (int i = 0; i < WorldMap.availableList.Count; i++)
			{
				if (this.GetCursorPos(WorldMap.availableList[i], getRegion: false) != WorldMap.cursorDestPos && this.CheckKeyDirection(WorldMap.cursorDestPos, this.GetCursorPos(WorldMap.availableList[i], getRegion: false), keyDirection))
				{
					float num3 = (WorldMap.cursorDestPos - this.GetCursorPos(WorldMap.availableList[i], getRegion: false)).Length();
					if (num == -1 || num3 < num2)
					{
						num = i;
						num2 = num3;
					}
				}
			}
			if (num > -1)
			{
				Sound.PlayCue("menu_click");
				WorldMap.selection = num;
				this.PopulateSubMap(thread: true);
			}
			WorldMap.cursorDestPos = this.GetCursorPos(WorldMap.availableList[WorldMap.selection], getRegion: true);
		}

		private void FindNearestCell(int keyDirection, bool self)
		{
			Vector2 vector = new Vector2(-1f, -1f);
			float num = 0f;
			Vector2 vector2 = new Vector2(Game1.navManager.scrollX, Game1.navManager.scrollY);
			for (int i = 0; i < WorldMap.entranceList.Count; i++)
			{
				Vector2 vector3 = new Vector2(WorldMap.entranceList[i].X, WorldMap.entranceList[i].Y);
				if (self || this.CheckKeyDirection(vector2, vector3, keyDirection))
				{
					float num2 = (vector2 - vector3).Length();
					if (vector == new Vector2(-1f, -1f) || num2 < num)
					{
						vector = vector3;
						num = num2;
						WorldMap.entranceDir = (TransitionDirection)WorldMap.entranceList[i].Z;
					}
				}
			}
			if (vector != new Vector2(-1f, -1f))
			{
				Game1.navManager.scrollX = (int)vector.X;
				Game1.navManager.scrollY = (int)vector.Y;
			}
			if (WorldMap.mapViewFade == 0f)
			{
				Game1.navManager.ForceScroll(0.75f, Game1.navManager.scrollX, Game1.navManager.scrollY);
			}
		}

		private bool CheckKeyDirection(Vector2 pos, Vector2 newPos, int keyDirection)
		{
			switch (keyDirection)
			{
			case 0:
				if (newPos.Y < pos.Y && Math.Abs(newPos.X - pos.X) < Math.Abs(newPos.Y - pos.Y))
				{
					return true;
				}
				return false;
			case 1:
				if (newPos.X > pos.X && Math.Abs(newPos.X - pos.X) / 2f > Math.Abs(newPos.Y - pos.Y))
				{
					return true;
				}
				return false;
			case 2:
				if (newPos.Y > pos.Y && Math.Abs(newPos.X - pos.X) < Math.Abs(newPos.Y - pos.Y))
				{
					return true;
				}
				return false;
			case 3:
				if (newPos.X < pos.X && Math.Abs(newPos.X - pos.X) / 2f > Math.Abs(newPos.Y - pos.Y))
				{
					return true;
				}
				return false;
			default:
				return false;
			}
		}

		private void PopulateSubMap(bool thread)
		{
			WorldMap.populatingMap = true;
			if (thread)
			{
				Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(PopulateThread)));
			}
			else
			{
				this.PopulateThread();
			}
		}

		private void PopulateThread()
		{
			lock (WorldMap.syncObject)
			{
				Game1.navManager.Read(WorldMap.navName);
				Game1.navManager.PopulateEntrances(WorldMap.entranceList);
				Game1.navManager.scrollX = (int)Game1.navManager.RevealMap[WorldMap.navName].LastVisitPos.X;
				Game1.navManager.scrollY = (int)Game1.navManager.RevealMap[WorldMap.navName].LastVisitPos.Y;
				Game1.navManager.ForceScroll(0.75f, Game1.navManager.scrollX, Game1.navManager.scrollY);
				this.FindNearestCell(0, self: true);
				WorldMap.populatingMap = false;
			}
		}

		public void PopulateControls()
		{
			int num = WorldMap.mapView;
			string text;
			if (num != 1)
			{
				text = Strings_HudInv.WorldMapControlsSelect;
				if (this.canReturn)
				{
					text = text + "     " + Strings_HudInv.WorldMapControlsReturn;
				}
			}
			else
			{
				text = Strings_HudInv.WorldMapControlsSubMap;
			}
			Game1.inventoryManager.itemControls = Game1.smallText.WordWrap(text, 0.7f, 930f, Game1.inventoryManager.itemControlsButtonList, TextAlign.Left);
		}

		public void PopulateAvailable(bool moveCursor)
		{
			WorldMap.availableList.Clear();
			int num = 0;
			int num2 = 0;
			foreach (KeyValuePair<string, RevealMap> item in Game1.navManager.RevealMap)
			{
				if (!(item.Key != "trial"))
				{
					continue;
				}
				int num3 = 0;
				for (int i = 0; i < item.Value.Width; i++)
				{
					for (int j = 0; j < item.Value.Height; j++)
					{
						if (item.Value.Cell[i, j] != null && item.Value.Cell[i, j].RevealState > 0)
						{
							num3 = 1;
							break;
						}
					}
				}
				if (num3 > 0)
				{
					WorldMap.availableList.Add(item.Key);
					if (item.Value.Visiting)
					{
						num2 = num;
					}
					num++;
				}
				item.Value.Visiting = false;
			}
			for (int k = 0; k < WorldMap.availableList.Count; k++)
			{
				if (k != num2)
				{
					WorldMap.selection = k;
					WorldMap.cursorDestPos = this.GetCursorPos(WorldMap.availableList[WorldMap.selection], getRegion: true);
					Game1.navManager.Read(WorldMap.navName);
				}
			}
			if (moveCursor)
			{
				WorldMap.selection = num2;
				WorldMap.cursorCurPos = (WorldMap.cursorDestPos = this.GetCursorPos(WorldMap.availableList[WorldMap.selection], getRegion: true));
				WorldMap.mapPos = new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2) - WorldMap.cursorCurPos;
				Game1.navManager.Read(WorldMap.navName);
				WorldMap.entranceList = new List<Vector3>();
				this.PopulateSubMap(thread: true);
			}
		}

		private bool CheckWorldMapReady(ParticleManager pMan, string prevPath)
		{
			int currentEvent = Game1.events.currentEvent;
			string text = string.Empty;
			switch (prevPath)
			{
			case "intro10":
				if (currentEvent < 60)
				{
					text = "village01";
				}
				break;
			case "village01":
				if (currentEvent < 60)
				{
					text = "intro10";
				}
				break;
			case "village06":
				if (currentEvent < 145)
				{
					text = "cave01";
				}
				break;
			case "cave01":
				if (currentEvent < 145)
				{
					text = "village06";
				}
				break;
			case "grave01b":
				if (currentEvent < 400)
				{
					text = "snow01";
				}
				break;
			case "snow01":
				if (currentEvent < 400)
				{
					text = "grave01b";
				}
				break;
			}
			if (text != string.Empty)
			{
				try
				{
					foreach (KeyValuePair<string, RevealMap> item in Game1.navManager.RevealMap)
					{
						item.Value.Visiting = false;
					}
					WorldMap.entranceList = new List<Vector3>();
					Game1.navManager.RevealCell(text, ref Game1.navManager.scrollX, ref Game1.navManager.scrollY, ref WorldMap.navName);
					this.PopulateSubMap(thread: false);
					WorldMap.mapView = 1;
					WorldMap.map.SwitchMap(pMan, WorldMap.character, text, loading: false);
					this.ExitWorldMap(pMan);
					return false;
				}
				catch (Exception)
				{
				}
			}
			return true;
		}

		public Vector2 WorldMapPos(bool background)
		{
			if (WorldMap.worldMapTex == null)
			{
				return Vector2.Zero;
			}
			Vector2 vector = new Vector2(MathHelper.Clamp(WorldMap.mapPos.X + WorldMap.freeLocPos.X, -WorldMap.worldMapTex.Width + Game1.screenWidth, 0f), MathHelper.Clamp(WorldMap.mapPos.Y + WorldMap.freeLocPos.Y, -WorldMap.worldMapTex.Height + Game1.screenHeight, 0f));
			if (background)
			{
				return vector * 0.88f;
			}
			return vector;
		}

		public void Update(ParticleManager pMan, float hudTime)
		{
			VibrationManager.Rumble(Game1.currentGamePad, 0f);
			if (!WorldMap.worldMapAssetsLoaded)
			{
				return;
			}
			if (WorldMap.mapView == 0)
			{
				if (WorldMap.KeyUp)
				{
					this.FindNearest(0);
				}
				if (WorldMap.KeyRight)
				{
					this.FindNearest(1);
				}
				if (WorldMap.KeyDown)
				{
					this.FindNearest(2);
				}
				if (WorldMap.KeyLeft)
				{
					this.FindNearest(3);
				}
				if (this.KeySelect && !WorldMap.populatingMap)
				{
					Sound.PlayCue("menu_confirm");
					WorldMap.mapView = 1;
					this.PopulateControls();
					if (WorldMap.entranceList.Count < 2 && Game1.navManager.RevealMap[Game1.navManager.NavPath].LastVisitPos == new Vector2(-1f, -1f))
					{
						try
						{
							WorldMap.map.transDir = WorldMap.entranceDir;
							WorldMap.map.SwitchMap(pMan, WorldMap.character, Game1.navManager.RevealMap[Game1.navManager.NavPath].Cell[Game1.navManager.scrollX, Game1.navManager.scrollY].CellMapName[0], loading: false);
							this.ExitWorldMap(pMan);
							return;
						}
						catch
						{
						}
					}
				}
				if (WorldMap.KeyCancel && this.canReturn)
				{
					Sound.PlayCue("menu_cancel");
					if (WorldMap.map.transDir == TransitionDirection.Left)
					{
						WorldMap.map.transDir = TransitionDirection.Right;
					}
					else if (WorldMap.map.transDir == TransitionDirection.Right)
					{
						WorldMap.map.transDir = TransitionDirection.Left;
					}
					WorldMap.map.SwitchMap(pMan, WorldMap.character, WorldMap.prevGameMap, loading: false);
					this.ExitWorldMap(pMan);
				}
			}
			else
			{
				if (WorldMap.KeyUp)
				{
					this.FindNearestCell(0, self: false);
				}
				if (WorldMap.KeyRight)
				{
					this.FindNearestCell(1, self: false);
				}
				if (WorldMap.KeyDown)
				{
					this.FindNearestCell(2, self: false);
				}
				if (WorldMap.KeyLeft)
				{
					this.FindNearestCell(3, self: false);
				}
				if (this.KeySelect)
				{
					bool flag = true;
					if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse && (Game1.navManager.scrollX >= Game1.navManager.RevealMap[Game1.navManager.NavPath].Width || Game1.navManager.scrollY >= Game1.navManager.RevealMap[Game1.navManager.NavPath].Height || Game1.navManager.RevealMap[Game1.navManager.NavPath].Cell[Game1.navManager.scrollX, Game1.navManager.scrollY] == null || Game1.navManager.RevealMap[Game1.navManager.NavPath].Cell[Game1.navManager.scrollX, Game1.navManager.scrollY].Entrance == -1))
					{
						Sound.PlayCue("fidget_fail");
						flag = false;
						this.FindNearestCell(1, self: false);
						this.FindNearestCell(2, self: false);
						this.FindNearestCell(3, self: false);
						this.FindNearestCell(0, self: false);
					}
					if (flag)
					{
						try
						{
							if (Game1.navManager.mouseX > -1)
							{
								Game1.navManager.scrollX = Game1.navManager.mouseX;
								Game1.navManager.scrollY = Game1.navManager.mouseY;
								this.FindNearestCell(0, self: true);
							}
							WorldMap.map.transDir = WorldMap.entranceDir;
							WorldMap.map.SwitchMap(pMan, WorldMap.character, Game1.navManager.RevealMap[Game1.navManager.NavPath].Cell[Game1.navManager.scrollX, Game1.navManager.scrollY].CellMapName[0], loading: false);
						}
						catch
						{
						}
						Sound.PlayCue("menu_confirm");
						this.ExitWorldMap(pMan);
					}
				}
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
				{
					Vector2 vector = Game1.hud.mousePos - new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2 + 30);
					int num = 48;
					vector = new Vector2((int)(vector.X / (float)num), (int)(vector.Y / (float)num));
					if (new Rectangle(Game1.screenWidth / 2 - 450, Game1.screenHeight / 2 - 300, 900, 600).Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y))
					{
						if (!Game1.pcManager.IsMouseLeftHeld() || WorldMap.mapViewFade < 1f)
						{
							Game1.hud.mapScrollPos = new Vector2(vector.X + (float)Game1.navManager.scrollX, vector.Y + (float)Game1.navManager.scrollY);
						}
						else
						{
							vector += new Vector2((int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Width, (int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Height);
							Game1.navManager.scrollX = (int)MathHelper.Clamp((float)(int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Width - vector.X + Game1.hud.mapScrollPos.X, 0f, (int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Width);
							Game1.navManager.scrollY = (int)MathHelper.Clamp((float)(int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Height - vector.Y + Game1.hud.mapScrollPos.Y, 0f, (int)Game1.navManager.RevealMap[Game1.navManager.NavPath].Height);
						}
					}
				}
				else
				{
					if (WorldMap.KeyCancel)
					{
						Sound.PlayCue("menu_cancel");
						WorldMap.mapView = 0;
						this.PopulateControls();
					}
					if (WorldMap.KeyY)
					{
						if (WorldMap.mapView != 2)
						{
							WorldMap.mapView = 2;
							Sound.PlayCue("menu_confirm");
						}
						else
						{
							WorldMap.mapView = 1;
							Sound.PlayCue("menu_cancel");
						}
					}
				}
			}
			if (WorldMap.worldMapAssetsLoaded)
			{
				if (Game1.videoPlayer != null && Game1.video != null)
				{
					this.videoTexture = Game1.videoPlayer.GetTexture();
				}
				WorldMap.cursorCurPos += (WorldMap.cursorDestPos - WorldMap.cursorCurPos) * hudTime * 14f;
				WorldMap.screenFade = MathHelper.Clamp(WorldMap.screenFade - hudTime, 0f, WorldMap.screenFade);
				if (WorldMap.mapView == 0)
				{
					WorldMap.mapViewFade = MathHelper.Clamp(WorldMap.mapViewFade - hudTime * 10f, 0f, 1f);
				}
				else
				{
					WorldMap.mapViewFade = MathHelper.Clamp(WorldMap.mapViewFade + hudTime * 10f, 0f, 1f);
				}
				if (WorldMap.mapView == 2)
				{
					WorldMap.mapLegendFade = MathHelper.Clamp(WorldMap.mapLegendFade + hudTime * 10f, 0f, 1f);
				}
				else
				{
					WorldMap.mapLegendFade = MathHelper.Clamp(WorldMap.mapLegendFade - hudTime * 10f, 0f, 1f);
				}
				Vector2 vector2 = WorldMap.mapPos;
				if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
				{
					vector2 = new Vector2(Game1.screenWidth / 2, (float)Game1.screenHeight * 0.4f) - WorldMap.cursorCurPos;
				}
				else if (!Game1.pcManager.mouseInBounds || WorldMap.mapView > 0)
				{
					vector2 = new Vector2(Game1.screenWidth / 2, (float)Game1.screenHeight * 0.6f) - WorldMap.cursorCurPos;
				}
				else
				{
					vector2.X = Game1.hud.mousePos.X / ((float)Game1.screenWidth / (float)(Game1.screenWidth - WorldMap.worldMapTex.Width));
					vector2.Y = Game1.hud.mousePos.Y / ((float)Game1.screenHeight / (float)(Game1.screenHeight - WorldMap.worldMapTex.Height));
					if (vector2.Y < -150f)
					{
						vector2.Y -= 100f * Game1.hiDefScaleOffset;
					}
				}
				Vector2 vector3 = new Vector2(MathHelper.Clamp(vector2.X, -WorldMap.worldMapTex.Width + Game1.screenWidth, 0f), MathHelper.Clamp(vector2.Y, -WorldMap.worldMapTex.Height + Game1.screenHeight, 0f));
				WorldMap.mapPos += (vector3 - WorldMap.mapPos) * hudTime * 2f;
				WorldMap.animFrameTime += hudTime * 24f;
				if (WorldMap.animFrameTime > 1f)
				{
					WorldMap.animFrameTime -= 1f;
					WorldMap.mapGlowTarget = Rand.GetRandomFloat(0f, 1f);
					WorldMap.flameAnimFrame++;
					if (WorldMap.flameAnimFrame > 40)
					{
						WorldMap.flameAnimFrame = 0;
					}
					WorldMap.availAnimFrame++;
					if (WorldMap.availAnimFrame > 9)
					{
						WorldMap.availAnimFrame = 0;
					}
					WorldMap.newAnimFrame++;
					if (WorldMap.newAnimFrame > 47)
					{
						WorldMap.newAnimFrame = 0;
					}
					WorldMap.cursorAnimFrame++;
					if (WorldMap.cursorAnimFrame > 31)
					{
						WorldMap.cursorAnimFrame = 0;
					}
				}
				WorldMap.mapGlowAlpha += (WorldMap.mapGlowTarget - WorldMap.mapGlowAlpha) * hudTime * 4f;
				if (Game1.skipFrame > 1 && WorldMap.mapView == 0)
				{
					Game1.navManager.flagAnimFrame++;
					if (Game1.navManager.flagAnimFrame > 23)
					{
						Game1.navManager.flagAnimFrame = 0;
					}
				}
				if (Rand.GetRandomInt(0, 16) == 0)
				{
					pMan.AddHudSmoke(new Vector2(850f, 800f), Rand.GetRandomVector2(-100f, 100f, -400f, -100f), 0.75f, 0.75f, 0.75f, 0.5f, Rand.GetRandomFloat(1f, 3f), Rand.GetRandomFloat(3.75f, 10f), 5);
				}
				if (Rand.GetRandomInt(0, 4) == 0)
				{
					pMan.AddHudFire(new Vector2(850f, 800f) + Rand.GetRandomVector2(-100f, 100f, 0f, 200f), Rand.GetRandomVector2(-300f, 300f, -1600f, -300f), 90f, Rand.GetRandomFloat(0.4f, 1f), 0, 5);
				}
				if (Rand.GetRandomInt(0, 40) == 0)
				{
					pMan.AddMenuLeaf(-this.WorldMapPos(background: false) + Rand.GetRandomVector2(-200f, Game1.screenWidth + 200, -400f, 0f), Rand.GetRandomVector2(-300f, 300f, 300f, 400f), 90f, Rand.GetRandomFloat(1f, 3f), 9);
				}
				pMan.UpdateParticles(hudTime, WorldMap.map, WorldMap.character, 5);
			}
			if ((!Game1.GamerServices || (Game1.GamerServices && !Guide.IsVisible)) && WorldMap.screenFade == 0f)
			{
				this.DoInput(Game1.currentGamePad);
			}
		}

		public void Draw(ParticleManager pMan, Texture2D[] particlesTex)
		{
			Game1.graphics.GraphicsDevice.Clear(Color.Black);
			WorldMap.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			if (!WorldMap.worldMapAssetsLoaded)
			{
				WorldMap.screenFade = 1f;
				Game1.DrawLoad(WorldMap.sprite, new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f);
				WorldMap.sprite.End();
				return;
			}
			Vector2 vector = this.WorldMapPos(background: true);
			if (this.videoTexture != null && !this.videoTexture.IsDisposed)
			{
				WorldMap.sprite.Draw(this.videoTexture, vector + new Vector2(410f, 0f), new Rectangle(0, 0, this.videoTexture.Width - 1, this.videoTexture.Height), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
			}
			WorldMap.sprite.Draw(WorldMap.worldMapTex, vector * 1.1f + new Vector2(-100f, 0f), new Rectangle(525, 0, 375, 425), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
			WorldMap.sprite.Draw(WorldMap.worldMapTex, vector * 1.1f + new Vector2(-100f, 0f), new Rectangle(900, 0, 375, 425), new Color(1f, 1f, 1f, 1f - WorldMap.mapGlowAlpha), 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
			WorldMap.sprite.End();
			WorldMap.sprite.Begin(SpriteSortMode.Deferred, BlendState.Additive);
			WorldMap.sprite.Draw(WorldMap.worldMapUITex, vector + new Vector2(850f, 900f), new Rectangle(WorldMap.flameAnimFrame * 100, 0, 100, 150), Color.White, 0f, new Vector2(50f, 150f), new Vector2(4f, 3f), SpriteEffects.None, 0f);
			WorldMap.sprite.End();
			WorldMap.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			pMan.DrawMapParticles(particlesTex, 1f, 5);
			Vector2 vector2 = this.WorldMapPos(background: false);
			WorldMap.sprite.Draw(WorldMap.worldMapTex, vector2 + new Vector2(0f, 525f), new Rectangle(0, 525, WorldMap.worldMapTex.Width, WorldMap.worldMapTex.Height - 525), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			WorldMap.sprite.End();
			WorldMap.sprite.Begin(SpriteSortMode.Deferred, BlendState.Additive);
			WorldMap.sprite.Draw(WorldMap.worldMapTex, vector2, new Rectangle(0, 0, 525, 425), new Color(1f, 1f, 1f, WorldMap.mapGlowAlpha), 0f, Vector2.Zero, 4f, SpriteEffects.None, 0f);
			WorldMap.sprite.End();
			WorldMap.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			for (int i = 0; i < WorldMap.availableList.Count; i++)
			{
				Vector2 vector3 = this.GetCursorPos(WorldMap.availableList[i], getRegion: false) + vector2;
				if (Game1.navManager.RevealMap[WorldMap.availableList[i]].Updated)
				{
					WorldMap.sprite.End();
					WorldMap.sprite.Begin(SpriteSortMode.Deferred, BlendState.Additive);
					WorldMap.sprite.Draw(WorldMap.worldMapUITex, vector3 + new Vector2(0f, 10f), new Rectangle(WorldMap.newAnimFrame * 60, 210, 60, 60), Color.White, 0f, new Vector2(30f, 30f), 3f, SpriteEffects.None, 0f);
					WorldMap.sprite.End();
					WorldMap.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
					Game1.smallText.Color = Color.Yellow;
					Game1.smallText.DrawOutlineText(vector3 + new Vector2(-250f, 30f), Strings_HudInv.WorldMapNew, 0.7f, 500, TextAlign.Center, fullOutline: true);
				}
				WorldMap.sprite.Draw(WorldMap.worldMapUITex, vector3, new Rectangle(WorldMap.availAnimFrame * 60, 150, 60, 60), Color.White, 0f, new Vector2(30f, 30f), 0.8f, SpriteEffects.None, 0f);
				if (Game1.navManager.RevealMap[WorldMap.availableList[i]].Destination)
				{
					WorldMap.sprite.Draw(Game1.navManager.NavTex, vector3, new Rectangle(64 * Game1.navManager.flagAnimFrame - 768 * (Game1.navManager.flagAnimFrame / 12), 96 + 48 * (Game1.navManager.flagAnimFrame / 12), 64, 48), Color.White, 0f, new Vector2(55f, 45f), 2f, SpriteEffects.FlipHorizontally, 0f);
				}
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && WorldMap.mapView == 0 && new Rectangle((int)vector3.X - 40, (int)vector3.Y - 30, 80, 60).Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y))
				{
					if (WorldMap.selection != i)
					{
						Sound.PlayCue("menu_click");
						WorldMap.selection = i;
						this.PopulateSubMap(thread: true);
						WorldMap.cursorDestPos = this.GetCursorPos(WorldMap.availableList[WorldMap.selection], getRegion: true);
					}
					if (Game1.pcManager.leftMouseClicked)
					{
						Game1.pcManager.leftMouseClicked = false;
						this.KeySelect = true;
					}
				}
			}
			pMan.DrawHudParticles(particlesTex, 1f, 9);
			for (int j = 0; j < WorldMap.cursorTrailPos.Length; j++)
			{
				float num = 1f - (float)j * 0.025f;
				if (j == 0)
				{
					WorldMap.cursorTrailPos[j] += (WorldMap.cursorCurPos - WorldMap.cursorTrailPos[j]) * Game1.HudTime * 40f;
					num *= Math.Min((WorldMap.cursorCurPos - WorldMap.cursorTrailPos[j]).Length(), 1f);
				}
				else
				{
					WorldMap.cursorTrailPos[j] += (WorldMap.cursorTrailPos[j - 1] - WorldMap.cursorTrailPos[j]) * Game1.HudTime * 40f;
					num *= Math.Min((WorldMap.cursorTrailPos[j - 1] - WorldMap.cursorTrailPos[j]).Length(), 1f);
				}
				WorldMap.sprite.Draw(WorldMap.worldMapUITex, WorldMap.cursorTrailPos[j] + vector2, new Rectangle(600 + WorldMap.cursorAnimFrame * 50, 150, 50, 50), new Color(1f, 1f, 1f, num), 0f, new Vector2(25f, 32f), new Vector2(1.2f, 1f) * (2f - (float)j * 0.05f), SpriteEffects.None, 0f);
			}
			WorldMap.sprite.End();
			WorldMap.sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			if (WorldMap.freeLook || WorldMap.mapView > 0 || (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && Game1.hud.mousePos.Y < (float)Game1.screenHeight * 0.3f))
			{
				WorldMap.freeLookAlpha = Math.Max(WorldMap.freeLookAlpha - Game1.HudTime * 4f, 0f);
			}
			else
			{
				WorldMap.freeLookAlpha = Math.Min(WorldMap.freeLookAlpha + Game1.HudTime * 4f, 1f);
			}
			float num2 = 0.8f;
			Game1.smallText.Color = Color.White * (1f - WorldMap.mapViewFade) * WorldMap.freeLookAlpha;
			try
			{
				int num3 = (int)(Game1.smallFont.MeasureString(Game1.questManager.activeQuest[Game1.questManager.activeQuest.Count - 1].stageString[0]).Y * num2) + 40;
				Vector2 vector4 = new Vector2(Game1.screenWidth / 2, (float)Game1.screenHeight - Math.Min((float)Game1.screenHeight * 0.14f, 100f) - (float)num3);
				if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
				{
					vector4.Y = (float)Game1.screenHeight * 0.05f;
				}
				Game1.hud.DrawBorder(vector4 + new Vector2(-(int)((float)Game1.screenWidth * 0.35f), 0f), (int)((float)Game1.screenWidth * 0.7f), num3, Game1.smallText.Color, 0.85f, 0);
				Game1.smallText.DrawText(new Vector2(0f, vector4.Y + 10f), Game1.questManager.activeQuest[Game1.questManager.activeQuest.Count - 1].stageString[0], num2, Game1.screenWidth, TextAlign.Center);
				if (WorldMap.mapViewFade == 0f)
				{
					if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
					{
						num2 = 0.7f;
						int num4 = (int)(Game1.smallFont.MeasureString(Game1.inventoryManager.itemControls).X * num2) + 20;
						Game1.hud.DrawMiniBorder(new Vector2((Game1.screenWidth - num4) / 2, vector4.Y + (float)num3 - 15f), num4, 30, Game1.smallText.Color, 1f);
						Game1.smallText.DrawButtonText(new Vector2(0f, vector4.Y + (float)num3 - 10f), Game1.inventoryManager.itemControls, num2, Game1.inventoryManager.itemControlsButtonList, bounce: false, Game1.screenWidth, TextAlign.Center);
					}
				}
				else
				{
					WorldMap.sprite.Draw(WorldMap.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, 0.5f * WorldMap.mapViewFade));
					if (Game1.hud.InventoryTex != null && !Game1.hud.InventoryTex.IsDisposed)
					{
						Color color = new Color(1f, 1f, 1f, WorldMap.mapViewFade);
						Vector2 vector5 = new Vector2(Game1.screenWidth / 2 - 465, (float)(Game1.screenHeight / 2 - 285) + (1f - WorldMap.mapViewFade) * 50f);
						WorldMap.sprite.Draw(Game1.hud.InventoryTex, vector5 + new Vector2(0f, -10f), new Rectangle(0, 0, 930, 570), new Color((int)color.R, (int)color.G, (int)color.B, (float)(int)color.A / 255f * 0.95f));
						this.DrawNav(vector5, color);
						Game1.hud.DrawMapTotals(vector5 + new Vector2(435f, 90f), 300, 1f, color, WorldMap.mapLegendFade);
						if (WorldMap.mapLegendFade > 0f)
						{
							Game1.hud.DrawMapLegend(vector5, color, WorldMap.mapLegendFade);
						}
						if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
						{
							if (Game1.pcManager.DrawMouseButton(vector5 + new Vector2(860f, -10f), 0.8f, color, 0, draw: true))
							{
								Sound.PlayCue("menu_cancel");
								WorldMap.mapView = 0;
								this.PopulateControls();
							}
							Vector2 loc = vector5 + new Vector2(815f, 460f);
							if (Game1.pcManager.DrawMouseButton(loc, 0.8f, color, 1, draw: true))
							{
								WorldMap.mapView++;
								if (WorldMap.mapView > 2)
								{
									WorldMap.mapView = 1;
									Sound.PlayCue("menu_cancel");
								}
								else
								{
									Sound.PlayCue("menu_confirm");
								}
							}
							Game1.pcManager.DrawMouseButton(loc, 0.8f, new Color(1f, 1f, 1f, 1f - WorldMap.mapLegendFade), 3, draw: true);
						}
					}
				}
			}
			catch (Exception)
			{
			}
			if (WorldMap.mapView == 0)
			{
				WorldMap.sprite.Draw(WorldMap.worldMapUITex, WorldMap.cursorCurPos + vector2, new Rectangle(600 + WorldMap.cursorAnimFrame * 50, 150, 50, 50), Color.White, 0f, new Vector2(25f, 32f), new Vector2(1.2f, 1f) * 2f, SpriteEffects.None, 0f);
				num2 = 0.75f;
				Game1.bigText.Color = Color.White * (1f - WorldMap.mapViewFade) * WorldMap.freeLookAlpha;
				int num5 = (int)Math.Max(Game1.bigFont.MeasureString(WorldMap.regionName).X * num2 + 80f, 280f);
				Vector2 vector6 = WorldMap.cursorCurPos + vector2 + new Vector2(-num5 / 2, -150f);
				Game1.hud.DrawMiniBorder(vector6 + new Vector2(0f, -10f), num5, 90, Game1.bigText.Color, 0.75f);
				Game1.bigText.DrawShadowText(new Vector2((int)vector6.X, (int)vector6.Y), WorldMap.regionName, num2, num5, TextAlign.Center, outline: true);
				WorldMap.sprite.Draw(Game1.hud.HudTex[2], new Vector2((int)(WorldMap.cursorCurPos.X + vector2.X), (int)vector6.Y + 30), new Rectangle(0, 502, 326, 18), Game1.bigText.Color, 0f, new Vector2(163f, 0f), 0.6f, SpriteEffects.None, 0f);
				Game1.hud.DrawMapTotals(new Vector2((int)(WorldMap.cursorCurPos.X + vector2.X - 30f), (int)vector6.Y + 50), 130, 0.8f, Game1.bigText.Color, 0f);
			}
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				if (WorldMap.mapView == 0 && this.canReturn && Game1.pcManager.DrawMouseButton(new Vector2((float)Game1.screenWidth * 0.9f - 30f, (float)Game1.screenHeight * 0.9f - 40f), 0.8f, Color.White, 1, draw: true))
				{
					Sound.PlayCue("menu_cancel");
					if (WorldMap.map.transDir == TransitionDirection.Left)
					{
						WorldMap.map.transDir = TransitionDirection.Right;
					}
					else if (WorldMap.map.transDir == TransitionDirection.Right)
					{
						WorldMap.map.transDir = TransitionDirection.Left;
					}
					WorldMap.map.SwitchMap(pMan, WorldMap.character, WorldMap.prevGameMap, loading: false);
					this.ExitWorldMap(pMan);
				}
				Game1.hud.mousePos = Game1.pcManager.DrawCursor(WorldMap.sprite, 0.8f, Color.White);
			}
			WorldMap.sprite.Draw(WorldMap.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, WorldMap.screenFade * 2f));
			WorldMap.sprite.End();
		}

		private void DrawNav(Vector2 screenOffset, Color color)
		{
			Vector2 vector = screenOffset + new Vector2(465f, 285f);
			Game1.navManager.Draw(vector, 0.75f, Game1.navManager.scrollX, Game1.navManager.scrollY, 17, 9, color, 0.75f, 1.2f, background: false, entrances: true);
			WorldMap.sprite.Draw(Game1.hud.InventoryTex, screenOffset + new Vector2(32f, 4f), new Rectangle(930, 0, 485, 285), color, 0f, new Vector2(3f, 14f), 2f, SpriteEffects.None, 0f);
			Game1.hud.DrawBorder(screenOffset + new Vector2(44f, 16f), 838, 514, color, 0f, 0);
			Color color2 = new Color(0f, 0f, 0f, 0.5f * (float)((int)color.A / 255));
			WorldMap.sprite.Draw(Game1.hud.HudTex[1], screenOffset + new Vector2(465f, 68f), new Rectangle(887, 20, 234, 180), color2, 0f, Vector2.Zero, new Vector2(1.2f, 1f), SpriteEffects.None, 0f);
			WorldMap.sprite.Draw(Game1.hud.HudTex[1], screenOffset + new Vector2(465f, 68f), new Rectangle(887, 20, 234, 180), color2, 0f, new Vector2(234f, 0f), new Vector2(1.2f, 1f), SpriteEffects.FlipHorizontally, 0f);
			WorldMap.sprite.Draw(Game1.hud.HudTex[2], screenOffset + new Vector2(465f, 60f), new Rectangle(0, 502, 326, 18), color, 0f, new Vector2(163f, 0f), new Vector2(1.5f, 1f), SpriteEffects.None, 0f);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			for (int i = 0; i < WorldMap.entranceList.Count; i++)
			{
				Vector2 vector2 = new Vector2(WorldMap.entranceList[i].X, WorldMap.entranceList[i].Y);
				if (vector2.X < (float)(Game1.navManager.scrollX - 1))
				{
					flag4 = true;
				}
				if (vector2.X > (float)(Game1.navManager.scrollX + 1))
				{
					flag2 = true;
				}
				if (vector2.Y < (float)(Game1.navManager.scrollY - 1))
				{
					flag = true;
				}
				if (vector2.Y > (float)(Game1.navManager.scrollY + 1))
				{
					flag3 = true;
				}
				if (vector2.X < (float)(Game1.navManager.scrollX - 6))
				{
					flag8 = true;
				}
				if (vector2.X > (float)(Game1.navManager.scrollX + 6))
				{
					flag6 = true;
				}
				if (vector2.Y < (float)(Game1.navManager.scrollY - 3))
				{
					flag5 = true;
				}
				if (vector2.Y > (float)(Game1.navManager.scrollY + 3))
				{
					flag7 = true;
				}
			}
			float num = Math.Abs((float)Math.Sin(Game1.hud.pulse * 2f)) * 6f;
			if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				if (flag)
				{
					WorldMap.sprite.Draw(Game1.navManager.NavTex, vector + new Vector2(0f, -32f - num), new Rectangle(752, 0, 60, 48), color, 0f, new Vector2(30f, 24f), 0.4f, SpriteEffects.None, 0f);
				}
				if (flag2)
				{
					WorldMap.sprite.Draw(Game1.navManager.NavTex, vector + new Vector2(32f + num, 0f), new Rectangle(752, 0, 60, 48), color, 1.57f, new Vector2(30f, 24f), 0.4f, SpriteEffects.None, 0f);
				}
				if (flag3)
				{
					WorldMap.sprite.Draw(Game1.navManager.NavTex, vector + new Vector2(0f, 32f + num), new Rectangle(752, 0, 60, 48), color, 3.14f, new Vector2(30f, 24f), 0.4f, SpriteEffects.None, 0f);
				}
				if (flag4)
				{
					WorldMap.sprite.Draw(Game1.navManager.NavTex, vector + new Vector2(-32f - num, 0f), new Rectangle(752, 0, 60, 48), color, 4.71f, new Vector2(30f, 24f), 0.4f, SpriteEffects.None, 0f);
				}
			}
			if (flag5)
			{
				WorldMap.sprite.Draw(Game1.navManager.NavTex, vector + new Vector2(0f, -160f - num), new Rectangle(752, 0, 60, 48), color, 0f, new Vector2(30f, 24f), 1f, SpriteEffects.None, 0f);
			}
			if (flag6)
			{
				WorldMap.sprite.Draw(Game1.navManager.NavTex, vector + new Vector2(304f + num, 0f), new Rectangle(752, 0, 60, 48), color, 1.57f, new Vector2(30f, 24f), 1f, SpriteEffects.None, 0f);
			}
			if (flag7)
			{
				WorldMap.sprite.Draw(Game1.navManager.NavTex, vector + new Vector2(0f, 160f + num), new Rectangle(752, 0, 60, 48), color, 3.14f, new Vector2(30f, 24f), 1f, SpriteEffects.None, 0f);
			}
			if (flag8)
			{
				WorldMap.sprite.Draw(Game1.navManager.NavTex, vector + new Vector2(-304f - num, 0f), new Rectangle(752, 0, 60, 48), color, 4.71f, new Vector2(30f, 24f), 1f, SpriteEffects.None, 0f);
			}
			Game1.bigText.Color = color;
			Game1.bigText.DrawShadowText(screenOffset + new Vector2(0f, 27f), WorldMap.regionName, 1f, 930, TextAlign.Center, outline: true);
			if (color.A >= 200 && Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
			{
				Game1.smallText.Color = color;
				int num2 = (int)(Game1.smallFont.MeasureString(Game1.inventoryManager.itemControls).X * 0.7f) + 20;
				Game1.hud.DrawMiniBorder(screenOffset + new Vector2(465 - num2 / 2, 505f), num2, 30, color, (int)color.A);
				Game1.smallText.DrawButtonText(new Vector2(0f, screenOffset.Y + 510f), Game1.inventoryManager.itemControls, 0.7f, Game1.inventoryManager.itemControlsButtonList, bounce: false, Game1.screenWidth, TextAlign.Center);
			}
		}
	}
}
