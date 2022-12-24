using System;
using System.Collections.Generic;
using System.IO;
using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses.bucket;
using Dust.MapClasses.script;
using Dust.Particles;
using Dust.Strings;
using Dust.Vibration;
using Lotus.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Dust.MapClasses
{
	public class Map
	{
		public string path = "map";

		public string regionName = "glade";

		public Ledge[] ledges;

		private static object syncObject = new object();

		private static Texture2D nullTex;

		private static Layer[] mapLayer;

		private static byte[,] col;

		private static byte colX = 200;

		private static byte colY = 200;

		public byte maxPlayerLedges;

		public bool mapAssetsLoaded;

		public byte blurAmount = 1;

		public byte refractType;

		public byte fgRefract;

		private static byte backDrop = 0;

		private static byte sourceIndex;

		private static byte defIndex;

		private static byte[] usedTexture;

		private static int[] prevUsedTexture;

		private static byte prevBackDrop;

		private static Video backdropVideo;

		private static Texture2D backdropVideoTexture;

		private static VideoPlayer backdropVideoPlayer;

		private static Rectangle sourceRect;

		private static Texture2D sourceIndexTexture;

		private static Vector2 segmentScale;

		private static Color drawColor = default(Color);

		private static bool flagEnabled;

		private static Texture2D[] mapsTex = new Texture2D[4];

		private static SegmentDefinition[,] segDef;

		private static Texture2D backDropTex;

		public float rightEdge;

		public float leftEdge;

		public float topEdge;

		public float bottomEdge;

		public float rightExitPoint;

		public float leftExitPoint;

		public float topExitPoint;

		public float bottomExitPoint;

		public float rightBlock;

		public float leftBlock;

		private static float scale;

		private static float layerScale;

		public byte backScroll;

		public bool canLockEdge;

		private static int flag;

		private static int totalFrameCount;

		private static int curFrameCount;

		private static double speed;

		private static Vector2 pos;

		private static Vector2 center;

		private static Vector2 dimen;

		public float MapSegFrameSpeed = 10f;

		public float MapSegFrame;

		public float MapSegFrameLocked;

		private static float rotateRandom;

		private static float moveRandomX;

		private static float moveRandomY;

		private static float scaleRandom;

		private static float alphaRandom;

		private static float particleTimer;

		private static float counter = 0f;

		private static WeatherManager wManager;

		private static HazardSpeed hazardSpeed;

		private static float hazardTimer;

		private static float prevHazardTimer;

		public double hazardRot;

		private static float hazardCountdown;

		private static float hazardScale;

		private static int hazardID = -1;

		private static int hazardType = -1;

		private static int hazardLayer;

		private static Vector2 hazardLoc;

		public int bugCount;

		public int fishCount;

		public int slashCount;

		public int bloodCount;

		public int projectileCount;

		public int shockRingCount;

		private static float fogRegionAlpha = 0f;

		public Vector4 fogColor;

		public Rectangle fogRegion;

		public bool fogRegionClear = true;

		public Color playerLayerColor;

		public MapScript mapScript;

		public Bucket bucket;

		public Party party;

		public float transInFrame;

		public float transOutFrame;

		public string[] TransitionDestination = new string[6] { "", "", "", "", "", "" };

		public TransitionDirection transDir;

		public int warpStage;

		public int doorStage;

		private float warpTimer;

		public string warpDestPath;

		public Vector2 warpDestPos;

		private static float darkness = 0f;

		private static float darkSpeed = 0.5f;

		public float reverbPercent;

		public float reverbMin;

		public bool killBombs;

		private static bool initingSegments;

		private static Dictionary<Vector2, RopeElement> ropes = new Dictionary<Vector2, RopeElement>();

		public List<BoostRegion> boostRegions = new List<BoostRegion>();

		public List<BombRegion> bombRegions = new List<BombRegion>();

		public List<DarkRegion> darkRegions = new List<DarkRegion>();

		public List<BrightRegion> brightRegions = new List<BrightRegion>();

		public List<WarpRegion> warpRegions = new List<WarpRegion>();

		public List<ReverbRegion> reverbRegions = new List<ReverbRegion>();

		public List<EventRegion> eventRegion = new List<EventRegion>();

		public List<DoorRegion> doorRegions = new List<DoorRegion>();

		public List<IdRectRegion> terrainRegion = new List<IdRectRegion>();

		public List<Rectangle> noPrecipRegion = new List<Rectangle>();

		public List<IdRectRegion> weatherColorRegion = new List<IdRectRegion>();

		public List<IdRectRegion> weatherRegion = new List<IdRectRegion>();

		public float Counter
		{
			set
			{
				Map.counter = value;
			}
		}

		public Layer[] GetMapLayer()
		{
			return Map.mapLayer;
		}

		public Texture2D GetTexture(int id)
		{
			for (int i = 0; i < Map.usedTexture.Length; i++)
			{
				if (Map.usedTexture[i] == id)
				{
					return Map.mapsTex[i];
				}
			}
			return Map.mapsTex[0];
		}

		public Map(Texture2D _nullTex, WeatherManager _wManager)
		{
			Map.nullTex = _nullTex;
			Map.wManager = _wManager;
			this.transDir = TransitionDirection.Intro;
			this.transInFrame = 1.1f;
			Map.mapLayer = new Layer[9];
			for (int i = 0; i < Map.mapLayer.Length; i++)
			{
				Map.mapLayer[i] = new Layer();
			}
			Map.col = new byte[Map.colX, Map.colY];
			Map.segDef = new SegmentDefinition[Map.mapsTex.Length, 110];
			Map.usedTexture = new byte[Map.mapsTex.Length];
			Map.prevUsedTexture = new int[Map.mapsTex.Length];
			for (int j = 0; j < Map.usedTexture.Length; j++)
			{
				Map.usedTexture[j] = (byte)j;
				Map.prevUsedTexture[j] = -1;
			}
		}

		private bool CheckRenderable(int l, int i, float scale, float worldScale, Rectangle sourceRect)
		{
			if (scale == 1f)
			{
				Map.pos = Map.mapLayer[l].mapSeg[i].Location * worldScale - Game1.Scroll * scale;
			}
			else
			{
				float num = (Game1.hiDefScaleOffset - Game1.worldScale) / Game1.hiDefScaleOffset;
				Map.pos = Map.mapLayer[l].mapSeg[i].Location * worldScale - Game1.Scroll * scale + new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f * (1f - scale) * num;
			}
			int num2 = (int)((float)sourceRect.Width * Map.mapLayer[l].mapSeg[i].Scale.X * scale * Game1.worldScale);
			int num3 = (int)((float)sourceRect.Height * Map.mapLayer[l].mapSeg[i].Scale.Y * scale * Game1.worldScale);
			switch (l)
			{
			case 0:
				num2 *= 10;
				num3 *= 10;
				break;
			case 1:
				num2 *= 6;
				num3 *= 6;
				break;
			}
			switch (Map.flag)
			{
			case 5:
			case 15:
				num2 *= 4;
				break;
			case 17:
			case 18:
				if (new Rectangle(-num2, -num3, Game1.screenWidth + num2 * 2, Game1.screenHeight + num3 * 2 + (int)(740f * scale * worldScale)).Contains((int)Map.pos.X, (int)Map.pos.Y))
				{
					return true;
				}
				break;
			case 19:
				num2 += (int)(1200f * worldScale);
				num3 += (int)(1200f * worldScale);
				break;
			case 32:
			case 33:
			case 34:
			case 35:
			case 36:
				if (num2 > num3)
				{
					num3 = num2;
				}
				else
				{
					num2 = num3;
				}
				num3 *= 2;
				break;
			case 46:
				return true;
			case 55:
			case 56:
			case 57:
			case 58:
			case 59:
				num2 += (int)(500f * worldScale);
				num3 += (int)(1000f * worldScale);
				break;
			case 100:
				num3 *= 2;
				break;
			case 101:
			case 102:
			case 103:
				num2 *= 5;
				num3 *= 2;
				break;
			}
			if (Map.mapLayer[l].mapSeg[i].Rotation != 0f)
			{
				if (num2 > num3)
				{
					num3 = num2;
				}
				else
				{
					num2 = num3;
				}
			}
			if (new Rectangle(-num2, -num3, Game1.screenWidth + num2 * 2, Game1.screenHeight + num3 * 2).Contains((int)Map.pos.X, (int)Map.pos.Y))
			{
				return true;
			}
			return false;
		}

		private bool CheckRefract(int refractive)
		{
			switch (refractive)
			{
				case 1:
					return Game1.refractive;
				case 2:
					return true;
				default:
					return !Game1.refractive;
			}
		}

		public bool GetLedgeMinMax(int l, float x)
		{
			lock (Map.syncObject)
			{
				if (this.ledges[l].Nodes.Length > 0 && x >= this.ledges[l].Nodes[0].X && x <= this.ledges[l].Nodes[this.ledges[l].Nodes.Length - 1].X)
				{
					return true;
				}
				return false;
			}
		}

		public int GetLedgeSec(int l, float x)
		{
			lock (Map.syncObject)
			{
				if (l < this.ledges.Length && this.ledges[l] != null)
				{
					for (int i = 0; i < this.ledges[l].Nodes.Length - 1; i++)
					{
						if (x >= this.ledges[l].Nodes[i].X && x <= this.ledges[l].Nodes[i + 1].X)
						{
							return i;
						}
					}
				}
				return -1;
			}
		}

		public float GetLedgeYLoc(int l, int i, float x)
		{
			if (l < 0)
			{
				return 0f;
			}
			lock (Map.syncObject)
			{
				return ((this.ledges[l].Nodes[i + 1].Y - this.ledges[l].Nodes[i].Y) * ((x - this.ledges[l].Nodes[i].X) / (this.ledges[l].Nodes[i + 1].X - this.ledges[l].Nodes[i].X)) + this.ledges[l].Nodes[i].Y) * 1f;
			}
		}

		public int CheckCol(Vector2 loc)
		{
			lock (Map.syncObject)
			{
				int num = (int)(loc.X / 64f);
				int num2 = (int)(loc.Y / 64f);
				if (num >= 0 && num2 >= 0 && num < Map.colX && num2 < Map.colY)
				{
					return Map.col[num, num2];
				}
				return 0;
			}
		}

		public float CheckPCol(Vector2 loc, Vector2 pLoc, bool canFallThrough, bool init)
		{
			if (init || this.GetTransVal() <= 0f)
			{
				int num = (int)(loc.X / 64f);
				int num2 = (int)(loc.Y / 64f);
				if (num >= 0 && num2 >= 0 && num < Map.colX && num2 < Map.colY && Map.col[num, num2] > 0)
				{
					return (int)(loc.Y / 64f) * 64;
				}
				for (int i = 0; i < this.maxPlayerLedges; i++)
				{
					if (!this.GetLedgeMinMax(i, loc.X))
					{
						continue;
					}
					int ledgeSec = this.GetLedgeSec(i, pLoc.X);
					int ledgeSec2 = this.GetLedgeSec(i, loc.X);
					if (ledgeSec2 > -1 && ledgeSec > -1)
					{
						float ledgeYLoc = this.GetLedgeYLoc(i, ledgeSec, pLoc.X);
						float ledgeYLoc2 = this.GetLedgeYLoc(i, ledgeSec2, loc.X);
						if (pLoc.Y <= ledgeYLoc + 30f && loc.Y >= ledgeYLoc2 && (!canFallThrough || this.ledges[i].Flag == LedgeFlags.Solid))
						{
							return ledgeYLoc2;
						}
					}
				}
			}
			return 0f;
		}

		public float CheckSpecialLedge(Vector2 loc, Vector2 pLoc, LedgeFlags flagType)
		{
			for (int i = this.maxPlayerLedges; i < this.ledges.Length; i++)
			{
				if (this.ledges[i] == null || this.ledges[i].Flag != flagType || !this.GetLedgeMinMax(i, loc.X))
				{
					continue;
				}
				int ledgeSec = this.GetLedgeSec(i, pLoc.X);
				int ledgeSec2 = this.GetLedgeSec(i, loc.X);
				if (ledgeSec2 > -1 && ledgeSec > -1)
				{
					float ledgeYLoc = this.GetLedgeYLoc(i, ledgeSec, pLoc.X);
					float ledgeYLoc2 = this.GetLedgeYLoc(i, ledgeSec2, loc.X);
					if (pLoc.Y <= ledgeYLoc + 30f && loc.Y >= ledgeYLoc2)
					{
						return ledgeYLoc2;
					}
				}
			}
			return 0f;
		}

		public bool CheckParticleColOld(Vector2 loc, Vector2 pLoc, bool canFallThrough)
		{
			if (this.CheckCol(loc) > 0)
			{
				return true;
			}
			for (int i = 0; i < this.maxPlayerLedges; i++)
			{
				if (!this.GetLedgeMinMax(i, loc.X))
				{
					continue;
				}
				int ledgeSec = this.GetLedgeSec(i, pLoc.X);
				int ledgeSec2 = this.GetLedgeSec(i, loc.X);
				if (ledgeSec2 > -1 && ledgeSec > -1)
				{
					float ledgeYLoc = this.GetLedgeYLoc(i, ledgeSec, pLoc.X);
					float ledgeYLoc2 = this.GetLedgeYLoc(i, ledgeSec2, loc.X);
					if (pLoc.Y <= ledgeYLoc + 30f && loc.Y >= ledgeYLoc2 && (!canFallThrough || this.ledges[i].Flag == LedgeFlags.Solid))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void AddColRow(int y)
		{
			if (y > -1 && y < 200)
			{
				for (int i = 0; i < 200; i++)
				{
					Map.col[i, y] = 1;
					Map.col[i, y + 1] = 1;
				}
			}
		}

		public float GetXLim()
		{
			if (this.rightBlock == 0f && this.leftBlock == 0f)
			{
				return this.rightEdge * Game1.worldScale - (float)Game1.screenWidth;
			}
			if (this.rightBlock < this.leftBlock + (float)Game1.screenWidth)
			{
				this.rightBlock = this.leftBlock + (float)Game1.screenWidth;
			}
			return this.rightBlock - (float)Game1.screenWidth;
		}

		public float GetYLim()
		{
			return this.bottomEdge * Game1.worldScale - (float)Game1.screenHeight;
		}

		public float GetTransVal()
		{
			if (this.transInFrame > 0f)
			{
				return this.transInFrame;
			}
			if (this.transOutFrame > 0f)
			{
				return 1f - this.transOutFrame;
			}
			return 0f;
		}

		private void GetSrect(int l, int i)
		{
			Map.curFrameCount = Map.mapLayer[l].mapSeg[i].FrameCount;
			int num = Map.sourceRect.X;
			while (num + (Map.curFrameCount + 1) * Map.sourceRect.Width > Map.sourceIndexTexture.Width)
			{
				num -= Map.sourceIndexTexture.Width - Map.sourceRect.X - (Map.sourceRect.Width - Map.segDef[Map.sourceIndex, Map.defIndex].RowOffset);
				Map.sourceRect.Y += Map.sourceRect.Height;
			}
			Map.sourceRect.X = num + Map.curFrameCount * Map.sourceRect.Width;
		}

		public void UpdateVideo(float frameTime)
		{
			if (Map.backdropVideoPlayer == null)
			{
				return;
			}
			if (frameTime == 0f)
			{
				Map.backdropVideoPlayer.Pause();
			}
			else if (Map.backdropVideoPlayer != null)
			{
				if (Map.backdropVideoPlayer.State == MediaState.Paused)
				{
					Map.backdropVideoPlayer.Resume();
				}
				if (this.GetTransVal() < 0.5f)
				{
					Map.backdropVideoTexture = Map.backdropVideoPlayer.GetTexture();
				}
			}
		}

		public void CheckTransitions(Character[] c, ParticleManager pMan)
		{
			if (!this.canLockEdge && c[0].Location.X > this.leftEdge + 600f && c[0].Location.X < this.rightEdge - 600f)
			{
				this.canLockEdge = true;
			}
			if (this.transOutFrame <= 0f && this.transInFrame <= 0f)
			{
				if (this.leftBlock == 0f)
				{
					_ = c[0].Trajectory.X;
					_ = c[0].CharCol;
					_ = Game1.dManager.PlatformTrajectory(c[0], c[0].Location).X;
					if (c[0].Location.X > this.rightEdge - 800f)
					{
						if (this.TransitionDestination[1] != "" && !this.CheckBlockedEdge(c, CharDir.Right, this.rightEdge - 400f, forceBlock: false) && c[0].Location.X > this.rightEdge - 64f)
						{
							if (Game1.stats.playerLifeState == 0 && !c[0].AnimName.StartsWith("hurt"))
							{
								this.transOutFrame = 1f;
								this.transDir = TransitionDirection.Right;
							}
							else
							{
								c[0].Location.X = (int)c[0].PLoc.X;
								c[0].Trajectory.X = 0f;
							}
						}
					}
					else if (c[0].Location.X < this.leftEdge + 800f && this.TransitionDestination[3] != "" && !this.CheckBlockedEdge(c, CharDir.Left, this.leftEdge + 400f, forceBlock: false) && c[0].Location.X < this.leftEdge + 64f)
					{
						if (Game1.stats.playerLifeState == 0 && !c[0].AnimName.StartsWith("hurt"))
						{
							this.transOutFrame = 1f;
							this.transDir = TransitionDirection.Left;
						}
						else
						{
							c[0].Location.X = (int)c[0].PLoc.X;
							c[0].Trajectory.X = 0f;
						}
					}
				}
				if (c[0].Location.Y < this.topEdge)
				{
					if (c[0].Trajectory.Y + Game1.dManager.PlatformTrajectory(c[0], c[0].Location).Y < 0f && this.TransitionDestination[0] != "")
					{
						this.transOutFrame = 1f;
						this.transDir = TransitionDirection.Up;
						c[0].Trajectory.Y = -2200f;
					}
				}
				else if (c[0].Location.Y > this.bottomEdge && c[0].Trajectory.Y + Game1.dManager.PlatformTrajectory(c[0], c[0].Location).Y > 0f)
				{
					if (this.TransitionDestination[2] != "")
					{
						this.transOutFrame = 1f;
						this.transDir = TransitionDirection.Down;
						return;
					}
					c[0].SetAnim("jump", 0, 0);
					c[0].Trajectory = Vector2.Zero;
					c[0].PLoc = (c[0].Location = Game1.stats.lastSafeLoc + new Vector2(0f, -128f));
					c[0].Holding = false;
					Game1.camera.ResetCamera(c);
				}
				return;
			}
			if (this.transOutFrame > 0f)
			{
				Game1.BlurScene(1f);
				this.transOutFrame -= Game1.HudTime * 6f;
				if (this.transOutFrame <= 0f && this.warpStage == 0 && this.doorStage == 0)
				{
					this.SwitchMap(pMan, c, null, loading: false);
				}
			}
			if (this.transInFrame > 0f)
			{
				if (this.transInFrame > 1f)
				{
					Game1.BlurScene(1f);
				}
				this.transInFrame -= Game1.HudTime * 6f;
			}
		}

		public bool CheckBlockedEdge(Character[] c, CharDir dir, float pushLoc, bool forceBlock)
		{
			int num = (forceBlock ? 1 : 0);
			if (num == 1 || this.canLockEdge)
			{
				float num2 = ((dir == CharDir.Right) ? this.leftExitPoint : this.rightExitPoint);
				if (c[0].Location.Y < num2 + 1200f || num == 1)
				{
					for (int i = 1; i < c.Length; i++)
					{
						if (c[i].Exists == CharExists.Exists && c[i].Team == Team.Enemy && c[i].Definition.charType != CharacterType.KaneGhostChase && c[i].DyingFrame == -1f && (c[i].Ethereal != EtherealState.Ethereal || c[i].LungeState != 0) && Math.Abs(c[i].Location.X - c[0].Location.X) < 1200f && ((c[0].State == CharState.Grounded && c[i].Location.Y < c[0].Location.Y + 400f && c[i].Location.Y > c[0].Location.Y - 600f) || ((c[0].State == CharState.Air || c[i].FlyType != 0) && Math.Abs(c[i].Location.Y - c[0].Location.Y) < 1200f)))
						{
							num = 2;
							break;
						}
					}
				}
			}
			if (num > 0)
			{
				if ((dir == CharDir.Left && c[0].Location.X < pushLoc) || (dir == CharDir.Right && c[0].Location.X > pushLoc))
				{
					if (dir == CharDir.Right)
					{
						c[0].BoostTraj = 0f - Math.Max((c[0].Location.X - pushLoc) / 128f * 1.2f * c[0].Speed * Math.Max(c[0].Trajectory.X / c[0].Speed, 1f), 0f);
					}
					else
					{
						c[0].BoostTraj = 0f - Math.Min((c[0].Location.X - pushLoc) / 128f * 1.2f * c[0].Speed * Math.Max((0f - c[0].Trajectory.X) / c[0].Speed, 1f), 0f);
					}
					if (c[0].AnimName == "airspin")
					{
						c[0].Slide(c[0].Speed * 1.5f);
						c[0].Trajectory.Y = 0f;
						c[0].SetAnim("jump", 0, 0);
					}
					if (num > 1 && ((dir == CharDir.Left && c[0].Trajectory.X < 0f) || (dir == CharDir.Right && c[0].Trajectory.X > 0f)) && !Game1.hud.inBoss)
					{
						Game1.hud.InitFidgetPrompt(FidgetPrompt.NoEscape);
					}
				}
				if ((dir == CharDir.Left && (c[0].Trajectory.X < 0f || c[0].Face == CharDir.Left)) || (dir == CharDir.Right && (c[0].Trajectory.X > 0f || c[0].Face == CharDir.Right)))
				{
					float num3 = ((dir == CharDir.Left) ? (-150) : 150);
					Vector2 vector = new Vector2(pushLoc + num3, c[0].Location.Y);
					Game1.pManager.AddVerticleBeam(vector + new Vector2(0f, Rand.GetRandomInt(-500, 200)), Rand.GetRandomVector2(-20f, 20f, -400f, 400f), 1f, 0.2f, 0.2f, 1f, Rand.GetRandomInt(40, 100), 400, Rand.GetRandomFloat(0.2f, 1f), -1, Math.Max(Rand.GetRandomInt(3, 7), 5));
					if (Game1.longSkipFrame == 2)
					{
						Game1.pManager.AddUpgradeBurn(vector + Rand.GetRandomVector2(-40f, 40f, -400f, 400f), 1f, 6);
					}
				}
				return true;
			}
			return false;
		}

		public string CheckMapName(string _path)
		{
			switch (_path)
			{
			case "worldmap":
			{
				bool flag = false;
				if (Game1.events.currentEvent == 110)
				{
					Game1.events.InitEvent(115, isSideEvent: false);
					Game1.events.skippable = SkipType.Unskippable;
					flag = true;
				}
				if (Game1.events.currentEvent == 266)
				{
					Game1.events.InitEvent(270, isSideEvent: false);
					Game1.events.skippable = SkipType.Unskippable;
					flag = true;
				}
				if (Game1.events.currentEvent == 500)
				{
					Game1.events.InitEvent(505, isSideEvent: false);
					Game1.events.skippable = SkipType.Unskippable;
					flag = true;
				}
				if (flag)
				{
					Game1.GetLargeContent().Unload();
					Game1.GetInventoryContent().Unload();
					Game1.GetPortraitContent0().Unload();
					Game1.GetPortraitContent1().Unload();
					Game1.GetMapContent0().Unload();
					Game1.GetMapContent1().Unload();
					Game1.GetMapContent2().Unload();
					Game1.GetMapContent3().Unload();
					Game1.GetMapBackContent().Unload();
					Game1.GetDestructContent().Unload();
					Game1.character[0].UnloadTextures();
					return "gaiusscene";
				}
				return _path;
			}
			case "village02":
				if (Game1.events.currentEvent < 120)
				{
					return "village02";
				}
				return "village02alt0";
			case "forest08c":
				if (Game1.events.currentEvent < 86)
				{
					return "forest08c";
				}
				return "forest08calt0";
			case "forest10":
				if (Game1.events.currentEvent < 100)
				{
					return "forest10";
				}
				return "forest10alt1";
			case "snow08":
				if (Game1.events.sideEventAvailable[53])
				{
					return "snow08";
				}
				return "snow08alt0";
			case "mansiond03":
				if (!Game1.events.sideEventAvailable[60])
				{
					return "mansiond05";
				}
				break;
			case "lava01":
				if (Game1.events.currentEvent >= 620)
				{
					return "lava01b";
				}
				break;
			}
			return _path;
		}

		private string CheckRegionName(string _path)
		{
			if (_path.StartsWith("challenge"))
			{
				string text = "challenge";
				string text2 = _path.Remove(0, text.Length);
				text2 = text2.Remove(2, text2.Length - 2);
				Game1.cManager.currentChallenge = int.Parse(text2);
				Game1.cManager.challengeArenas[Game1.cManager.currentChallenge].ResetLamps(resetChallenge: false);
				Game1.events.InitEvent(6, isSideEvent: true);
				return Strings_Regions.trial + " #" + (Game1.cManager.currentChallenge + 1);
			}
			Game1.cManager.ExitScoreBoard(canRestart: true);
			Game1.cManager.currentChallenge = -1;
			Game1.cManager.challengeState = 0;
			Game1.dManager.destructLamps.Clear();
			if (_path.StartsWith("village"))
			{
				this.regionName = "aurora";
				return Strings_Regions.aurora;
			}
			if (_path.StartsWith("smith"))
			{
				this.regionName = "smith";
				return Strings_Regions.smith;
			}
			if (_path.StartsWith("forest"))
			{
				this.regionName = "forest";
				if (_path.StartsWith("forest1"))
				{
					return Strings_Regions.denham;
				}
				return Strings_Regions.forest;
			}
			if (_path.StartsWith("cave"))
			{
				this.regionName = "cave";
				if (_path.StartsWith("cave12") || _path == "cave13")
				{
					return Strings_Regions.cavevillage;
				}
				return Strings_Regions.cave;
			}
			if (_path.StartsWith("grave") || _path.StartsWith("mansion"))
			{
				this.regionName = "grave";
				if (!_path.StartsWith("grave"))
				{
					string text3 = _path.Remove(0, "mansion".Length);
					if (text3.StartsWith("a"))
					{
						return Strings_Regions.mansion1;
					}
					if (text3.StartsWith("b"))
					{
						return Strings_Regions.mansion2;
					}
					if (text3.StartsWith("c"))
					{
						return Strings_Regions.mansion3;
					}
					return Strings_Regions.mansion4;
				}
				if (!_path.EndsWith("01") || Game1.events.currentEvent >= 300)
				{
					return Strings_Regions.grave;
				}
				Game1.events.regionDisplayName = string.Empty;
			}
			else
			{
				if (_path.StartsWith("trial"))
				{
					this.regionName = "trial";
					if (_path != "trial01")
					{
						return Strings_Regions.grave;
					}
					return string.Empty;
				}
				if (_path.StartsWith("snow"))
				{
					this.regionName = "snow";
					if (_path == "snow27" || _path == "snowhome1")
					{
						return Strings_Regions.zeplich;
					}
					if (!_path.EndsWith("01") || Game1.events.currentEvent >= 400)
					{
						return Strings_Regions.snow;
					}
					Game1.events.regionDisplayName = string.Empty;
				}
				else
				{
					if (_path.StartsWith("lava"))
					{
						this.regionName = "lava";
						if (_path.StartsWith("lava0"))
						{
							return Strings_Regions.lavacamp;
						}
						return Strings_Regions.lava;
					}
					if (_path.StartsWith("ivy"))
					{
						this.regionName = "ivy";
						return Strings_Regions.ivy;
					}
					if (_path.StartsWith("cove"))
					{
						this.regionName = "cove";
						return Strings_Regions.cove;
					}
					if (_path.StartsWith("farm"))
					{
						this.regionName = "farm";
						return Strings_Regions.farm;
					}
					if (_path.StartsWith("sanc"))
					{
						this.regionName = "sanc";
						return Strings_Regions.sanctuary;
					}
					this.regionName = "glade";
					if (_path != "intro01")
					{
						return Strings_Regions.glade;
					}
				}
			}
			return string.Empty;
		}

		public void WorldMapFromEvent(ParticleManager pMan, Character[] c, string prevPath, TransitionDirection direction, string ResetPrevRegion)
		{
			this.TransitionDestination[3] = (this.TransitionDestination[1] = "worldmap");
			this.transDir = direction;
			this.path = prevPath;
			this.SwitchMap(pMan, c, null, loading: false);
			if (Game1.navManager.worldMap != null)
			{
				Game1.navManager.worldMap.canReturn = false;
				Game1.navManager.worldMap.PopulateControls();
			}
		}

		public void SwitchMap(ParticleManager pMan, Character[] c, string _path, bool loading)
		{
			this.transInFrame = 2.4f;
			Game1.InitFrameRate(0);
			Game1.hud.ExitShop();
			this.bugCount = 0;
			this.fishCount = 0;
			this.slashCount = 0;
			this.bloodCount = 0;
			this.projectileCount = 0;
			this.shockRingCount = 0;
			Map.wManager.lightningBloom = 0f;
			this.ResetHazards();
			this.boostRegions.Clear();
			this.bombRegions.Clear();
			this.warpRegions.Clear();
			this.darkRegions.Clear();
			this.brightRegions.Clear();
			this.reverbRegions.Clear();
			this.eventRegion.Clear();
			this.doorRegions.Clear();
			this.terrainRegion.Clear();
			this.noPrecipRegion.Clear();
			this.weatherColorRegion.Clear();
			this.weatherRegion.Clear();
			Map.ropes.Clear();
			this.reverbMin = 0f;
			Map.darkness = 0f;
			Map.darkSpeed = 0.5f;
			this.killBombs = false;
			this.canLockEdge = false;
			Game1.maxRefractingChars = 0;
			Game1.stats.comboTimer *= 0.7f;
			Game1.camera.GetZoomRectList.Clear();
			Game1.camera.GetPanRectList.Clear();
			Game1.hud.ClearHelp();
			Game1.hud.saveable = false;
			Game1.hud.shopID = -1;
			Game1.hud.npcAvailable = false;
			Game1.hud.converseWithID = -1;
			Game1.hud.collectID = -1;
			Game1.hud.npcAvailable = false;
			Game1.hud.InitFidgetPrompt(FidgetPrompt.None);
			Game1.menu.ClearPrompt();
			if (Game1.hud.regionIntroTime <= 0f)
			{
				Game1.hud.regionIntroState = 0;
			}
			Game1.cManager.ResetPositions();
			this.fogRegion = Rectangle.Empty;
			this.fogRegionClear = true;
			VibrationManager.Reset();
			TransitionDirection direction = this.transDir;
			string text = this.path;
			if (_path == null)
			{
				if (this.transDir > TransitionDirection.None)
				{
					this.path = this.TransitionDestination[(int)this.transDir];
				}
				this.path = this.CheckMapName(this.path);
				if (this.path == "worldmap")
				{
					Game1.navManager.InitWorldMap(text, _canReturn: true);
					return;
				}
				this.path = this.CheckMapName(this.path);
			}
			else
			{
				if (!loading && this.transDir == TransitionDirection.None)
				{
					this.transDir = TransitionDirection.Intro;
				}
				this.path = this.CheckMapName(_path);
			}
			for (int i = 1; i < c.Length; i++)
			{
				c[i].Exists = CharExists.Dead;
				c[i].Name = string.Empty;
			}
			this.transInFrame = 2.4f;
			Game1.hud.savePos = (Game1.hud.shopPos = Vector2.Zero);
			string newRegion = this.CheckRegionName(this.path);
			WeatherType newWeather = WeatherType.Realtime;
			this.Read(this.path, ref newWeather);
			this.SetTextures();
			Game1.navManager.BeginReadThread(this.regionName, forcePopulate: false);
			Game1.navManager.LimitScroll(Game1.navManager.navScale, Game1.navManager.playerX, Game1.navManager.playerY, 1, 1);
			this.SetMusic(this.path);
			Sound.ResetCCues();
			Sound.ResetPCues();
			List<AmbientCue> list = new List<AmbientCue>();
			this.mapScript.DoScript(c, pMan, list);
			this.InitUniqueSegs();
			Map.wManager.SetWeather(newWeather, forceReset: false);
			Map.wManager.SetWeatherColor(Map.wManager.weatherType);
			this.ReadyPlayer(c, direction, c[0].Location);
			Game1.hud.InitRegionIntro(newRegion, this.path, text == "worldmap");
			if (!this.killBombs)
			{
				pMan.RepopBombs();
			}
			pMan.ResetWeather();
			if (this.fogRegionClear && this.fogRegion.Contains((int)c[0].Location.X, (int)c[0].Location.Y))
			{
				Map.fogRegionAlpha = 0f;
			}
			if (Game1.stats.fidgetAwayTime <= 0f && Game1.stats.fidgetState == FidgetState.Normal)
			{
				pMan.AddFidget(c[0].Location + new Vector2(0f, -200f));
			}
			Game1.hud.InitFidgetPrompt(FidgetPrompt.TreasureNear);
			this.transInFrame = Math.Max(this.transInFrame, 2.4f);
			Sound.ResetAmbience(list);
			GC.Collect();
		}

		private void ReadyPlayer(Character[] c, TransitionDirection direction, Vector2 newLoc)
		{
			c[0].Floating = false;
			c[0].Holding = false;
			c[0].GrabbedBy = -1;
			c[0].BoostTraj = 0f;
			c[0].Ethereal = EtherealState.Normal;
			Game1.stats.inFront = true;
			c[0].Location.X = MathHelper.Clamp(newLoc.X, this.leftEdge + 64f, this.rightEdge - 64f);
			c[0].Location.Y = MathHelper.Clamp(newLoc.Y, this.topEdge, this.bottomEdge);
			if (direction == TransitionDirection.Down || direction == TransitionDirection.Up)
			{
				if (c[0].KeyLeft)
				{
					while (this.CheckCol(c[0].Location) > 0)
					{
						c[0].Location.X += 32f;
					}
				}
				else if (c[0].KeyRight)
				{
					while (this.CheckCol(c[0].Location) > 0)
					{
						c[0].Location.X -= 32f;
					}
				}
			}
			if (c[0].State == CharState.Air)
			{
				if (c[0].AnimName.StartsWith("attackair"))
				{
					c[0].SetAnim("jump", 12, 0);
				}
				c[0].Location.Y -= 50f;
				c[0].Trajectory.Y = MathHelper.Clamp(c[0].Trajectory.Y, -3000f, 3000f);
			}
			else
			{
				c[0].State = CharState.Air;
				c[0].Location.Y -= 64f;
				c[0].GroundCharacter();
				if (c[0].Trajectory.X == 0f && !c[0].AnimName.StartsWith("idle"))
				{
					c[0].SetAnim("idle00", 0, 0);
				}
			}
			if (this.warpStage > 0)
			{
				c[0].SetAnim("null", 0, 0);
			}
			Game1.stats.lastSafeLoc = c[0].Location;
			Game1.camera.ResetCamera(c);
			Game1.camera.Update(c, this, Game1.FrameTime, updateViewPoint: false);
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(Game1.stats.GetWorldExplored)));
		}

		public void SetTextures()
		{
			lock (Map.syncObject)
			{
				this.mapAssetsLoaded = false;
				Game1.GetDestructContent().Unload();
				Game1.loadingTime = 0f;
				Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadTextures), new TaskFinishedDelegate(LoadingFinished)));
			}
		}

		private void LoadTextures()
		{
			while (Game1.drawState != 0)
			{
			}
			if (Game1.gameMode == Game1.GameModes.Game)
			{
				Game1.SetResources();
			}
			this.mapAssetsLoaded = false;
			Map.backdropVideoTexture = null;
			int num = 0;
			while (num < 10)
			{
				try
				{
					for (int i = 0; i < Map.usedTexture.Length; i++)
					{
						if (Map.usedTexture[i] != Map.prevUsedTexture[i])
						{
							while (Game1.drawState != 0)
							{
							}
							this.GetManager(i).Unload();
							Map.mapsTex[i] = this.GetManager(i).Load<Texture2D>("gfx/backgrounds/mapTexture_" + (int)(Map.usedTexture[i] + 1));
							this.transInFrame = 6f;
						}
						Map.prevUsedTexture[i] = Map.usedTexture[i];
					}
					num = 20;
				}
				catch (Exception)
				{
					num++;
				}
			}
			if (Map.backDrop > 1)
			{
				if (Map.backDrop != Map.prevBackDrop)
				{
					if (Map.backdropVideoPlayer != null)
					{
						Map.backdropVideoPlayer.Stop();
						Map.backdropVideoPlayer = null;
						Map.backdropVideo = null;
					}
					Game1.GetMapBackContent().Unload();
				}
				try
				{
					Map.backDropTex = Game1.GetMapBackContent().Load<Texture2D>("gfx/backgrounds/backdrop_" + Map.backDrop);
					if (Map.backdropVideoPlayer == null)
					{
						Map.backdropVideoPlayer = new VideoPlayer();
						Map.backdropVideoPlayer.IsLooped = true;
						Map.backdropVideoPlayer.Volume = 0f;
						Map.backdropVideo = Game1.GetLargeContent().Load<Video>("video/bg_" + Map.backDrop);
						this.transInFrame = 6f;
						Map.backdropVideoPlayer.Play(Map.backdropVideo);
					}
				}
				catch
				{
					Map.backdropVideoPlayer = null;
				}
			}
			if (Map.backdropVideoPlayer != null)
			{
				Map.backdropVideoPlayer.Stop();
				Map.backdropVideoPlayer = null;
				Map.backdropVideo = null;
				Game1.GetMapBackContent().Unload();
			}
			Map.prevBackDrop = Map.backDrop;
		}

		private ContentManager GetManager(int manager)
		{
			switch (manager)
			{
				case 1: return Game1.GetMapContent1();
				case 2: return Game1.GetMapContent2();
				case 3: return Game1.GetMapContent3();
			}
			return Game1.GetMapContent0();
		}

		private static void LoadingFinished(int taskId)
		{
			Game1.map.mapAssetsLoaded = true;
			Game1.gameMode = Game1.GameModes.Game;
			Game1.InitFrameRate(60);
		}

		private static void UnloadTextures()
		{
			Game1.GetMapContent0().Unload();
			for (int i = 0; i < Map.usedTexture.Length; i++)
			{
				Map.prevUsedTexture[i] = -2;
			}
		}

		private void InitUniqueSegs()
		{
			Map.initingSegments = true;
			for (int i = 0; i < Map.mapLayer.Length; i++)
			{
				for (int j = 0; j < Map.mapLayer[i].mapSeg.Length; j++)
				{
					if (Map.mapLayer[i].mapSeg[j] != null)
					{
						switch (Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].Flag)
						{
						case 32:
							Map.ropes.Add(new Vector2(i, j), new RopeElement(j, i, 0, Map.mapLayer[i].mapSeg[j], Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].FrameCount, Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].SrcRect, (int)Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].Speed, this));
							break;
						case 33:
							Map.ropes.Add(new Vector2(i, j), new RopeElement(j, i, 1, Map.mapLayer[i].mapSeg[j], Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].FrameCount, Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].SrcRect, (int)Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].Speed, this));
							break;
						case 34:
							Map.ropes.Add(new Vector2(i, j), new RopeElement(j, i, 2, Map.mapLayer[i].mapSeg[j], Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].FrameCount, Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].SrcRect, (int)Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].Speed, this));
							break;
						case 35:
							Map.ropes.Add(new Vector2(i, j), new RopeElement(j, i, 2, Map.mapLayer[i].mapSeg[j], Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].FrameCount, Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].SrcRect, (int)Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].Speed, this));
							break;
						case 36:
							Map.ropes.Add(new Vector2(i, j), new RopeElement(j, i, 3, Map.mapLayer[i].mapSeg[j], Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].FrameCount, Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].SrcRect, (int)Map.segDef[Map.mapLayer[i].mapSeg[j].SourceIndexSlot, Map.mapLayer[i].mapSeg[j].Index].Speed, this));
							break;
						}
					}
				}
			}
			Map.initingSegments = false;
		}

		public void InitSegDefs()
		{
			for (int i = 0; i < Map.mapsTex.Length; i++)
			{
				for (int j = 0; j < 100; j++)
				{
					Map.segDef[i, j] = null;
				}
			}
			int num = 0;
			for (int k = 0; k < Map.usedTexture.Length; k++)
			{
				this.ReadSegmentDefinitions(Map.usedTexture[k], num);
				num++;
			}
		}

		private void ReadSegmentDefinitions(int mapTexture, int segDefIndex)
		{
			StreamReader streamReader;
			if (!Game1.Xbox360)
			{
				streamReader = ((!Game1.isPCBuild) ? new StreamReader("../../../../../backgrounds/mapTexture_" + (mapTexture + 1) + ".txt") : new StreamReader("data/mapdefs/mapTexture_" + (mapTexture + 1) + ".dst"));
			}
			else
			{
				string text = ".txt";
				if (Game1.XBLABuild)
				{
					text = ".dst";
				}
				streamReader = new StreamReader("data/mapdefs/mapTexture_" + (mapTexture + 1) + text);
			}
			byte b = 0;
			int rowOffset = 0;
			byte b2 = 0;
			byte b3 = 10;
			int num = -1;
			Rectangle srcRect = default(Rectangle);
			string empty = string.Empty;
			do
			{
				empty = streamReader.ReadLine();
			}
			while (!empty.StartsWith("#"));
			while (!streamReader.EndOfStream)
			{
				empty = streamReader.ReadLine();
				if (empty.StartsWith("#"))
				{
					continue;
				}
				num++;
				empty = streamReader.ReadLine();
				string[] array = empty.Split(' ');
				srcRect.X = Convert.ToInt32(array[0]);
				srcRect.Y = Convert.ToInt32(array[1]);
				srcRect.Width = Convert.ToInt32(array[2]) - srcRect.X;
				srcRect.Height = Convert.ToInt32(array[3]) - srcRect.Y;
				empty = streamReader.ReadLine();
				array = empty.Split(' ');
				b2 = Convert.ToByte(array[0]);
				b3 = Convert.ToByte(array[1]);
				empty = streamReader.ReadLine();
				array = empty.Split(' ');
				b = Convert.ToByte(array[0]);
				for (int i = 0; i < b; i++)
				{
					if (i * srcRect.Width > 4096)
					{
						rowOffset = i * srcRect.Width - 4096;
						break;
					}
				}
				Map.segDef[segDefIndex, num] = new SegmentDefinition(srcRect, b2, b3, b, rowOffset);
			}
		}

		public void Read(string mapPath, ref WeatherType newWeather)
		{
			int num = 1000;
			while (num > 0)
			{
				try
				{
					Color[] array = new Color[Map.mapLayer.Length];
					BinaryReader binaryReader = ((!Game1.Xbox360 && !Game1.isPCBuild) ? new BinaryReader(File.Open("../../../../../maps/" + mapPath + ".map", FileMode.Open, FileAccess.Read)) : new BinaryReader(File.Open("data/maps/" + mapPath + ".map", FileMode.Open, FileAccess.Read)));
					this.ledges = new Ledge[binaryReader.ReadInt32()];
					this.maxPlayerLedges = (byte)binaryReader.ReadInt32();
					for (int i = 0; i < this.ledges.Length; i++)
					{
						byte b = (byte)binaryReader.ReadInt32();
						Vector2[] array2 = new Vector2[b];
						for (int j = 0; j < array2.Length; j++)
						{
							ref Vector2 reference = ref array2[j];
							reference = new Vector2(binaryReader.ReadSingle(), binaryReader.ReadSingle()) * 2f;
						}
						LedgeFlags ledgeFlags = (LedgeFlags)binaryReader.ReadInt32();
						if (i < this.maxPlayerLedges || b > 0)
						{
							this.ledges[i] = new Ledge(b, ledgeFlags);
							for (int k = 0; k < this.ledges[i].Nodes.Length; k++)
							{
								ref Vector2 reference2 = ref this.ledges[i].Nodes[k];
								reference2 = array2[k];
							}
						}
					}
					this.mapScript = new MapScript(this);
					for (int l = 0; l < this.mapScript.Lines.Length; l++)
					{
						string text = binaryReader.ReadString();
						if (text.Length > 0)
						{
							this.mapScript.Lines[l] = new MapScriptLine(text);
						}
						else
						{
							this.mapScript.Lines[l] = null;
						}
					}
					this.leftEdge = binaryReader.ReadSingle();
					this.topEdge = binaryReader.ReadSingle();
					this.rightEdge = binaryReader.ReadSingle();
					this.bottomEdge = binaryReader.ReadSingle();
					Map.backDrop = (byte)binaryReader.ReadInt32();
					newWeather = (WeatherType)binaryReader.ReadInt32();
					this.blurAmount = (byte)binaryReader.ReadInt32();
					this.refractType = (byte)binaryReader.ReadInt32();
					this.fgRefract = (byte)binaryReader.ReadInt32();
					this.fogColor.W = binaryReader.ReadSingle();
					this.fogColor.X = binaryReader.ReadSingle();
					this.fogColor.Y = binaryReader.ReadSingle();
					this.fogColor.Z = binaryReader.ReadSingle();
					this.backScroll = (byte)binaryReader.ReadInt32();
					for (int m = 0; m < Map.usedTexture.Length; m++)
					{
						Map.usedTexture[m] = (byte)binaryReader.ReadInt32();
					}
					for (int n = 0; n < Map.mapLayer.Length; n++)
					{
						Map.mapLayer[n].mapSeg = new MapSegment[binaryReader.ReadInt32()];
					}
					for (int num2 = 0; num2 < Map.mapLayer.Length; num2++)
					{
						ref Color reference3 = ref array[num2];
						reference3 = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
						for (int num3 = 0; num3 < Map.mapLayer[num2].mapSeg.Length; num3++)
						{
							int num4 = binaryReader.ReadInt32();
							if (num4 == -1)
							{
								Map.mapLayer[num2].mapSeg[num3] = null;
								continue;
							}
							Map.mapLayer[num2].mapSeg[num3] = new MapSegment();
							Map.mapLayer[num2].mapSeg[num3].Index = (byte)num4;
							binaryReader.ReadInt32();
							binaryReader.ReadInt32();
							binaryReader.ReadInt32();
							binaryReader.ReadInt32();
							Map.mapLayer[num2].mapSeg[num3].Location = new Vector2(binaryReader.ReadSingle(), binaryReader.ReadSingle()) * 2f;
							Map.mapLayer[num2].mapSeg[num3].Rotation = binaryReader.ReadSingle();
							Map.mapLayer[num2].mapSeg[num3].Scale = new Vector2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
							Map.mapLayer[num2].mapSeg[num3].Flip = binaryReader.ReadBoolean();
							Map.mapLayer[num2].mapSeg[num3].color = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
							Map.mapLayer[num2].mapSeg[num3].FlagEnabled = binaryReader.ReadBoolean();
							Map.mapLayer[num2].mapSeg[num3].SourceIndex = (byte)binaryReader.ReadInt32();
							Map.mapLayer[num2].mapSeg[num3].SourceIndexSlot = (byte)binaryReader.ReadInt32();
							Map.mapLayer[num2].mapSeg[num3].Location.X += Map.mapLayer[num2].mapSeg[num3].SourceIndexSlot * 400;
						}
					}
					this.playerLayerColor = array[5];
					for (int num5 = 0; num5 < Map.colX; num5++)
					{
						for (int num6 = 0; num6 < Map.colY; num6++)
						{
							Map.col[num5, num6] = (byte)binaryReader.ReadInt32();
						}
					}
					binaryReader.Close();
					this.bucket = null;
					this.rightBlock = 0f;
					this.leftBlock = 0f;
					for (int num7 = 0; num7 < this.TransitionDestination.Length; num7++)
					{
						this.TransitionDestination[num7] = "";
					}
					if (this.mapScript.GotoTag("init"))
					{
						this.mapScript.IsReading = true;
					}
					int num8 = 0;
					for (int num9 = 0; num9 < Map.usedTexture.Length; num9++)
					{
						if (Map.usedTexture[num9] != Map.prevUsedTexture[num9])
						{
							num8++;
						}
					}
					if (num8 > 0)
					{
						this.InitSegDefs();
					}
					try
					{
						for (int num10 = 0; num10 < Map.mapLayer.Length; num10++)
						{
							for (int num11 = 0; num11 < Map.mapLayer[num10].mapSeg.Length; num11++)
							{
								if (Map.mapLayer[num10].mapSeg[num11] != null)
								{
									Map.mapLayer[num10].mapSeg[num11].PrepareLayerColor(array[num10], Map.segDef);
								}
							}
						}
						break;
					}
					catch (Exception)
					{
						break;
					}
				}
				catch (Exception)
				{
					num--;
					if (num < 0)
					{
						break;
					}
				}
			}
		}

		public void SetMusic(string mapName)
		{
			if (mapName == null)
			{
				return;
			}
			if (mapName.StartsWith("challenge"))
			{
				this.reverbMin = 0.3f;
				Music.Play("challenge");
			}
			else
			{
				if (Game1.events.anyEvent)
				{
					return;
				}
				if (mapName.StartsWith("village"))
				{
					if (mapName.EndsWith("5") || mapName.EndsWith("6"))
					{
						Sound.FadeMusicOut(4f);
					}
					else
					{
						Music.Play("villageforest");
					}
				}
				else if (mapName.StartsWith("intro"))
				{
					if (mapName.EndsWith("09"))
					{
						Sound.FadeMusicOut(4f);
					}
					else if (!mapName.EndsWith("01") && !mapName.EndsWith("01b") && !mapName.EndsWith("02"))
					{
						Music.Play("glade");
					}
				}
				else if (mapName.StartsWith("forest") || mapName.StartsWith("smith"))
				{
					if (mapName.StartsWith("forest10"))
					{
						Sound.FadeMusicOut(4f);
					}
					else
					{
						Music.Play("forest");
					}
				}
				else if (mapName.StartsWith("cave"))
				{
					if (mapName.StartsWith("cave12") || mapName == "cave13")
					{
						Music.Play("villagecave");
					}
					else
					{
						Music.Play("cave");
					}
					if (mapName != "cave13")
					{
						this.reverbMin = 0.3f;
					}
				}
				else if (mapName.StartsWith("grave"))
				{
					switch (mapName)
					{
					default:
						if ((!mapName.StartsWith("grave13") || !(mapName != "grave13")) && (!mapName.StartsWith("grave16") || !(mapName != "grave16")))
						{
							break;
						}
						goto case "grave01";
					case "grave01":
					case "grave01b":
					case "grave10":
						Sound.FadeMusicOut(2f);
						return;
					}
					Music.Play("grave");
				}
				else if (mapName.StartsWith("trial"))
				{
					if (mapName == "trial09" || mapName == "trial10")
					{
						Sound.FadeMusicOut(2f);
					}
					else if (mapName != "trial00")
					{
						Music.Play("grave");
					}
				}
				else if (mapName.StartsWith("mansion"))
				{
					Sound.FadeMusicOut(2f);
				}
				else if (mapName.StartsWith("snow"))
				{
					if (mapName.StartsWith("snowhome") || mapName.EndsWith("26") || mapName.EndsWith("27"))
					{
						Music.Play("souls");
					}
					else if (mapName.EndsWith("01") || mapName.EndsWith("24") || mapName.EndsWith("25"))
					{
						Sound.FadeMusicOut(2f);
					}
					else if (mapName.EndsWith("13") && Game1.events.currentEvent < 460)
					{
						Sound.FadeMusicOut(4f);
					}
					else
					{
						Music.Play("snow");
					}
				}
				else if (mapName.StartsWith("lava"))
				{
					if (mapName.StartsWith("lava0"))
					{
						if (mapName != "lava07" || Game1.events.currentEvent >= 540)
						{
							Music.Play("villagelava");
						}
						else
						{
							Sound.FadeMusicOut(4f);
						}
					}
					else if (mapName == "lava20b" || mapName == "lava21")
					{
						Sound.FadeMusicOut(4f);
					}
					else if (!mapName.StartsWith("lava22"))
					{
						Music.Play("lava");
					}
				}
				else
				{
					Sound.FadeMusicOut(4f);
				}
			}
		}

		private void DrawFog(SpriteBatch sprite, int l)
		{
			if (!Game1.settings.DepthOfField)
			{
				return;
			}
			if (this.fogColor.W == 0f)
			{
				if (l == 4)
				{
					sprite.Draw(Map.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(this.fogColor.X, this.fogColor.Y, this.fogColor.Z, Map.wManager.fogWeatherAlpha));
				}
				return;
			}
			float num = (this.fogColor.W + Map.wManager.fogWeatherAlpha) * Map.fogRegionAlpha;
			switch (l)
			{
			case 2:
				num *= 1.5f;
				break;
			case 3:
				num *= 1.25f;
				break;
			}
			sprite.Draw(Map.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(this.fogColor.X, this.fogColor.Y, this.fogColor.Z, num));
		}

		public void ResetHazards()
		{
			this.hazardRot = (Map.hazardTimer = 0f);
			Map.hazardCountdown = (Map.hazardType = (Map.hazardID = -1));
			Map.hazardLoc = Vector2.Zero;
		}

		private bool CheckHazard(Vector2 loc, int distance, float rarity)
		{
			if (Game1.events.anyEvent && Game1.events.safetyOn)
			{
				return false;
			}
			if (Game1.longSkipFrame < 4 || Rand.GetRandomFloat(0f, 100f) < rarity)
			{
				return false;
			}
			loc += Game1.Scroll;
			Rectangle rectangle = new Rectangle((int)loc.X - distance, (int)loc.Y - distance, distance * 2, distance * 2);
			float x = Game1.character[0].Location.X;
			x += Game1.character[0].Trajectory.X;
			if (rectangle.Contains((int)x, (int)Game1.character[0].Location.Y - 200))
			{
				return true;
			}
			return false;
		}

		private void InitHazard(Vector2 loc, float scale, float timer, int type, int segID, int layer)
		{
			Map.hazardLoc = loc + Game1.Scroll;
			Map.hazardScale = scale;
			Map.hazardCountdown = timer;
			Map.hazardType = type;
			Map.hazardID = segID;
			Map.hazardLayer = layer;
			Sound.PlayCue("bubbleplant_shake", Map.hazardLoc, (Map.hazardLoc - Game1.character[0].Location).Length());
		}

		private void CompleteHazard(ParticleManager pMan)
		{
			switch (Map.hazardType)
			{
			case 0:
			{
				for (int k = 0; k < 30; k++)
				{
					pMan.AddBubbleSquirt(Map.hazardLoc, Rand.GetRandomVector2(-200f, 200f, -1000f, -400f), Map.hazardScale, -1, Map.hazardType, Map.hazardLayer);
				}
				float num = Math.Abs(((float)(Game1.screenWidth / 2) - (Map.hazardLoc.X - Game1.Scroll.X)) / 4f) / 100f;
				if (num < 1f)
				{
					num = 1f;
				}
				this.MapSegFrameSpeed = 4f;
				VibrationManager.SetScreenShake(0.6f / num);
				Sound.PlayCue("bubbleplant_explode", Map.hazardLoc, (Map.hazardLoc - Game1.character[0].Location).Length());
				break;
			}
			case 1:
			{
				for (int j = 0; j < 30; j++)
				{
					pMan.AddBubbleSquirt(Map.hazardLoc, Rand.GetRandomVector2(-200f, 200f, -1000f, -400f), Map.hazardScale, -1, Map.hazardType, Map.hazardLayer);
				}
				float num = Math.Abs(((float)(Game1.screenWidth / 2) - (Map.hazardLoc.X - Game1.Scroll.X)) / 4f) / 100f;
				if (num < 1f)
				{
					num = 1f;
				}
				this.MapSegFrameSpeed = 4f;
				VibrationManager.SetScreenShake(0.6f / num);
				Sound.PlayCue("bubbleplant_explode", Map.hazardLoc, (Map.hazardLoc - Game1.character[0].Location).Length());
				break;
			}
			case 2:
			{
				pMan.AddIcicle(Map.hazardLoc, new Vector2(0f, 200f), Rand.GetRandomFloat(0.5f, 0.75f) * Math.Max(1f, Map.hazardScale), 1, Map.hazardLayer);
				for (int i = 0; i < 5; i++)
				{
					pMan.AddSpray(Map.hazardLoc + new Vector2(Rand.GetRandomInt(-40, 40), -50f), new Vector2(0f, Rand.GetRandomInt(0, 200)), 0.5f, 2, 1, 5);
				}
				Sound.PlayCue("icicle_release", Map.hazardLoc, (Map.hazardLoc - Game1.character[0].Location).Length());
				break;
			}
			}
			Map.hazardType = -1;
			Map.hazardLoc = Vector2.Zero;
			Map.hazardID = -1;
		}

		private void UpdateEmitter(Vector2 loc, int l, int i, ParticleManager pMan)
		{
			if (Game1.hud.isPaused || Game1.menu.prompt != promptDialogue.None)
			{
				return;
			}
			int num = 0;
			int num2 = (int)(Map.mapLayer[l].mapSeg[i].Scale.X * 3f) - 1;
			float num3 = Map.mapLayer[l].mapSeg[i].Rotation + Rand.GetRandomFloat(-0.1f, 0.1f);
			float num5;
			float num4;
			switch (Map.flag)
			{
			default:
				Map.hazardSpeed = HazardSpeed.EmitFlame;
				num4 = 0.15f;
				num5 = 0f;
				break;
			case 56:
				Map.hazardSpeed = HazardSpeed.EmitRocks;
				num4 = 0.1f;
				num5 = 0.3f;
				break;
			case 57:
				Map.hazardSpeed = HazardSpeed.EmitLava;
				num4 = 0.2f;
				num5 = 0.5f;
				break;
			}
			Vector2 vector = this.PlayerLayerLoc(loc);
			if (Game1.hud.dialogueState == DialogueState.Active)
			{
				num4 = 0f;
			}
			if (l == 5)
			{
				float num6 = Math.Min(1000f / (Game1.character[0].Location - vector).Length(), 2f);
				num5 *= num6;
				num4 *= num6;
			}
			else
			{
				num5 = (num4 = 0f);
			}
			switch (num2)
			{
			case 0:
				if (Map.hazardTimer < 3f)
				{
					num = 2;
				}
				else if (Map.hazardTimer > 5f)
				{
					num = 1;
				}
				break;
			case 1:
				if (Map.hazardTimer > 1f)
				{
					if (Map.hazardTimer < 2f)
					{
						num = 1;
					}
					else if (Map.hazardTimer < 5f)
					{
						num = 2;
					}
				}
				break;
			case 2:
				if (Map.hazardTimer > 3f && Map.hazardTimer < 4f)
				{
					num = 1;
				}
				else if (Map.hazardTimer < 1f || Map.hazardTimer > 4f)
				{
					num = 2;
				}
				break;
			case 3:
				num = 2;
				break;
			case 4:
				num = 2;
				num3 += (float)this.hazardRot;
				break;
			case 5:
				num = 2;
				num3 -= (float)this.hazardRot;
				break;
			}
			Sound.SetHazardStage(l, i, vector, Map.hazardSpeed, num);
			switch (num)
			{
			case 1:
				if (Rand.GetRandomInt(0, 3) == 0)
				{
					Vector2 vector2 = new Vector2((float)Math.Cos(num3), (float)Math.Sin(num3));
					pMan.MakeHazard(Map.flag, loc, vector2 * 1000f, num3, 1.5f, 1, l);
					pMan.MakeHazard(Map.flag, loc, vector2 * 150f, num3, 0.3f, 0, l);
					VibrationManager.SetScreenShake(num5);
				}
				return;
			case 0:
				return;
			}
			Vector2 vector3 = new Vector2((float)Math.Cos(num3), (float)Math.Sin(num3));
			pMan.MakeHazard(Map.flag, loc, vector3 * 1000f, num3, 1.1f, 2, l);
			int randomInt = Rand.GetRandomInt(0, 8);
			if (randomInt == 0)
			{
				pMan.MakeHazard(Map.flag, loc, vector3 * 800f, num3, 0.5f, 0, (l == 5) ? 6 : l);
			}
			if (num2 > 3)
			{
				pMan.MakeHazard(Map.flag, loc, -vector3 * 1000f, num3, 1.1f, 2, l);
				if (randomInt == 0)
				{
					pMan.MakeHazard(Map.flag, loc, -vector3 * 900f, num3, 0.5f, 0, (l == 5) ? 6 : l);
				}
			}
			VibrationManager.SetScreenShake(num4);
		}

		private Vector2 PlayerLayerLoc(Vector2 locSrc)
		{
			Vector2 vector = locSrc * Game1.worldScale - Game1.Scroll * Map.scale + new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2) * (1f - Map.scale) * (1f - Game1.worldScale);
			return (vector + Game1.Scroll) / Game1.worldScale;
		}

		public void InitWarp(Vector2 newPos, string newPath, Character[] c)
		{
			this.warpDestPos = newPos;
			this.warpDestPath = newPath;
			this.warpStage = 1;
			this.warpTimer = 1f;
			c[0].SetAnim("null", 0, 0);
			c[0].State = CharState.Grounded;
			Game1.camera.playerJumpPoint = 0f;
			Vector2 vector = c[0].Location - new Vector2(0f, 120f);
			for (int i = 0; i < 20; i++)
			{
				Vector2 loc = vector + Rand.GetRandomVector2(-100f, 100f, -100f, 100f);
				Game1.pManager.AddElectricBolt(loc, -1, Rand.GetRandomFloat(0.6f, 1.2f), 1000, 100, 1f, 5);
				Game1.pManager.AddBounceSpark(loc, Rand.GetRandomVector2(-800f, 800f, -500f, 10f), 0.5f, 6);
			}
			if (newPath == string.Empty)
			{
				Sound.PlayCue("warp_small");
			}
			else
			{
				Sound.PlayCue("warp_large");
				Vector2 fidgetLoc = Game1.pManager.GetFidgetLoc(accomodateScroll: false);
				for (int j = 0; j < 4; j++)
				{
					Game1.pManager.AddElectricBolt(fidgetLoc, -1, Rand.GetRandomFloat(0.6f, 1.2f), 1000, 100, 1f, 5);
				}
				Game1.pManager.RemoveParticle(new Fidget(Vector2.Zero));
			}
			Game1.worldDark = 0f;
			VibrationManager.SetScreenShake(1f);
			this.MapSegFrameSpeed = 0.4f;
			Game1.pManager.AddShockRing(vector, 1f, 5);
			VibrationManager.SetBlast(1f, vector);
		}

		private void UpdateWarp(ParticleManager pMan, Character[] c, float frameTime)
		{
			Game1.hud.LimitInput();
			Game1.hud.canInput = false;
			if (this.transInFrame <= 0f)
			{
				this.warpTimer -= frameTime;
			}
			if (this.warpStage == 1)
			{
				if (!(this.warpTimer < 0f))
				{
					return;
				}
				if (this.warpDestPath == "")
				{
					this.warpStage = 3;
					return;
				}
				if (this.warpDestPath != "self" && !this.warpDestPath.StartsWith("challenge"))
				{
					this.warpTimer = 1f;
				}
				Game1.cManager.ExitScoreBoard(canRestart: true);
				this.warpStage++;
			}
			else if (this.warpStage == 2)
			{
				if (this.warpTimer < 0f)
				{
					this.transOutFrame = 1f;
					this.warpTimer = 0.14f;
					this.warpStage++;
				}
			}
			else if (this.warpStage == 3)
			{
				if (!(this.warpTimer < 0f))
				{
					return;
				}
				this.warpStage++;
				if (this.warpDestPath == "")
				{
					c[0].Location = this.warpDestPos;
				}
				else
				{
					if (this.warpDestPath == "self")
					{
						this.warpDestPath = this.path;
					}
					else if (!this.warpDestPath.StartsWith("challenge"))
					{
						this.warpTimer = 1f;
					}
					pMan.Reset(removeWeather: false, removeBombs: true);
					this.SwitchMap(pMan, c, this.warpDestPath, loading: false);
					Game1.worldScale = 1.1f * Game1.hiDefScaleOffset;
					if (this.warpDestPos != Vector2.Zero)
					{
						c[0].State = CharState.Air;
						this.ReadyPlayer(c, TransitionDirection.Intro, this.warpDestPos);
						c[0].Location = this.warpDestPos;
						c[0].State = CharState.Grounded;
						pMan.ReposBombs();
					}
				}
				pMan.AddFidget(c[0].Location);
				pMan.ResetWeather();
			}
			else if (this.warpStage == 4)
			{
				Vector2 loc = c[0].Location - new Vector2(0f, 100f);
				if (Game1.halfSecFrame > 30)
				{
					pMan.AddElectricBolt(loc, -1, Rand.GetRandomFloat(0.2f, 0.6f), 1000, 40, 0.4f, 5);
				}
				if (this.warpTimer < 0f && new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight).Contains((int)(c[0].Location.X * Game1.worldScale - Game1.Scroll.X), (int)(c[0].Location.Y * Game1.worldScale - Game1.Scroll.Y)))
				{
					this.warpStage++;
					this.warpTimer = 0.5f;
					Sound.PlayCue("warp_inverted");
				}
			}
			else
			{
				if (this.warpStage != 5)
				{
					return;
				}
				Vector2 loc2 = c[0].Location - new Vector2(0f, 100f);
				if (Game1.longSkipFrame > 3)
				{
					pMan.AddElectricBolt(loc2, -1, Rand.GetRandomFloat(0.2f, 0.6f), 1000, 40, 0.4f, 5);
				}
				if (this.warpTimer < 0f)
				{
					c[0].SetJump(1200f, jumped: true);
					c[0].SetAnim("evadeair", 1, 0);
					c[0].Ethereal = EtherealState.Normal;
					c[0].CanCancel = true;
					this.warpStage = 0;
					Game1.worldDark = 0f;
					this.MapSegFrameSpeed = 0.4f;
					VibrationManager.SetBlast(0.5f, loc2);
					VibrationManager.SetScreenShake(0.6f);
					pMan.AddShockRing(loc2, 1f, 5);
					for (int i = 0; i < 20; i++)
					{
						pMan.AddElectricBolt(loc2, -1, Rand.GetRandomFloat(0.1f, 0.5f), 1000, 100, 1f, 5);
						pMan.AddBounceSpark(loc2, Rand.GetRandomVector2(-400f, 400f, -800f, 10f), 0.5f, 6);
					}
					Sound.PlayCue("warp_small");
				}
			}
		}

		public void InitDoor(Vector2 newPos, string newPath, Character[] c)
		{
			this.warpDestPos = newPos;
			this.warpDestPath = newPath;
			this.doorStage = 1;
			this.warpTimer = 0f;
			Game1.camera.playerJumpPoint = 0f;
		}

		private void UpdateDoor(ParticleManager pMan, Character[] c, float frameTime)
		{
			Game1.hud.LimitInput();
			Game1.hud.canInput = false;
			c[0].Ethereal = EtherealState.Ethereal;
			if (this.transInFrame <= 0f)
			{
				this.warpTimer -= frameTime;
			}
			if (this.doorStage == 1)
			{
				if (this.warpTimer < 0f)
				{
					this.transOutFrame = 1f;
					this.warpTimer = 0.14f;
					this.doorStage++;
				}
			}
			else if (this.doorStage == 2)
			{
				if (!(this.warpTimer < 0f))
				{
					return;
				}
				if (this.warpDestPath == "worldmap")
				{
					Game1.navManager.InitWorldMap(this.path, _canReturn: true);
					this.doorStage = 0;
					return;
				}
				if (this.warpDestPath == "self")
				{
					this.warpDestPath = this.path;
				}
				else if (!this.warpDestPath.StartsWith("challenge"))
				{
					this.warpTimer = 1f;
				}
				this.SwitchMap(pMan, c, this.warpDestPath, loading: false);
				Game1.worldScale = Game1.hiDefScaleOffset;
				this.ReadyPlayer(c, TransitionDirection.Intro, this.warpDestPos);
				Game1.pManager.ReposBombs();
				c[0].State = CharState.Air;
				c[0].GroundCharacter();
				c[0].Ethereal = EtherealState.Normal;
				c[0].CanCancel = true;
				c[0].Face = ((c[0].Face == CharDir.Left) ? CharDir.Right : CharDir.Left);
				c[0].SetAnim("runend", 0, 0);
				pMan.AddFidget(c[0].Location - new Vector2(0f, 220f));
				Game1.camera.ResetCamera(c);
				Game1.hud.runInTime = 0f;
				this.doorStage++;
			}
			else if (this.doorStage == 3 && this.GetTransVal() <= 0f)
			{
				this.doorStage = 0;
			}
		}

		private bool UpdateMotion(int l, int i, float worldScale, SpriteBatch sprite)
		{
			Map.center = new Vector2(Map.sourceRect.Width, Map.sourceRect.Height) / 2f;
			Map.dimen = Map.segmentScale * Map.scale;
			Map.rotateRandom = 0f;
			Map.moveRandomX = 0f;
			Map.moveRandomY = 0f;
			Map.scaleRandom = 0f;
			Map.alphaRandom = 1f;
			if (Map.flag > 0 && Map.flag < 100 && Map.flagEnabled)
			{
				switch (Map.flag)
				{
				case 1:
				case 6:
					Map.rotateRandom = (float)Math.Cos(Map.speed) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale / 1000f);
					Map.moveRandomX = (float)Math.Cos(Map.speed) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale * Map.segmentScale.X / 4f);
					Map.scaleRandom = (float)Math.Sin(Map.speed) * 0.05f * Map.scale * ((Map.segmentScale.X + Map.segmentScale.Y / 2f) * 0.25f * Map.scale);
					break;
				case 2:
					Map.rotateRandom = (float)Math.Cos(Map.speed) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale / 100f);
					Map.moveRandomX = (float)Math.Cos(Map.speed) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale * Map.segmentScale.X / 4f);
					Map.scaleRandom = (float)Math.Sin(Map.speed) * 0.05f * Map.scale * ((Map.segmentScale.X + Map.segmentScale.Y / 2f) * 0.25f * Map.scale);
					break;
				case 3:
				case 11:
					Map.rotateRandom = (float)Math.Cos(Map.speed) * 0.05f * Map.scale * ((float)Map.sourceRect.Height * Map.scale / 1000f);
					Map.moveRandomX = (0f - (float)Math.Cos(Map.speed) * 0.05f * Map.scale) * ((float)Map.sourceRect.Height * Map.scale * Map.segmentScale.Y / 4f);
					Map.moveRandomY = (float)Math.Sin(Map.speed) * 0.03f * Map.scale * (float)Map.sourceRect.Height * Map.scale * Map.segmentScale.X / 4f;
					Map.scaleRandom = (float)Math.Sin(Map.speed) * 0.05f * Map.scale * ((Map.segmentScale.X + Map.segmentScale.Y / 2f) * 0.25f * Map.scale);
					break;
				case 4:
					Map.alphaRandom = 1f - Math.Abs((float)Math.Cos((double)this.MapSegFrame * (double)(int)Map.segDef[Map.sourceIndex, Map.defIndex].Speed + (double)(i * 4)) * 0.75f);
					Map.moveRandomX = (float)Math.Cos(Map.speed) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale * Map.segmentScale.X / 4f);
					Map.moveRandomY = (float)Math.Sin(Map.speed) * 0.01f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale * Map.segmentScale.Y / 4f);
					Map.scaleRandom = (float)Math.Sin(Map.speed) * 0.05f * Map.scale * ((Map.segmentScale.X + Map.segmentScale.Y / 2f) * 0.25f * Map.scale);
					break;
				case 7:
					Map.rotateRandom = (float)Math.Cos(Map.speed) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale / 1000f);
					Map.moveRandomX = (float)Math.Cos(Map.speed) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale * Map.segmentScale.X / 1.5f);
					Map.moveRandomY = (float)Math.Sin(Map.speed) * 0.03f * Map.scale * (float)Map.sourceRect.Height * Map.scale * Map.segmentScale.X;
					Map.scaleRandom = (float)Math.Sin(Map.speed) * 0.05f * Map.scale * ((Map.segmentScale.X + Map.segmentScale.Y / 2f) * 0.25f * Map.scale);
					break;
				case 8:
				case 40:
					Map.rotateRandom = (float)Math.Cos(Map.speed) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale / 1000f);
					Map.moveRandomX = (float)Math.Cos(Map.speed) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale * Map.segmentScale.X / 4f);
					break;
				case 9:
				{
					Map.drawColor = Map.mapLayer[l].mapSeg[i].color;
					Vector2 vector5 = Map.mapLayer[l].mapSeg[i].Location - Game1.Scroll * Map.scale;
					if (l == 5 && Map.hazardCountdown <= 0f && this.CheckHazard(vector5, 500, 98 - Game1.stats.gameDifficulty))
					{
						int type = 0;
						if (Rand.GetRandomInt(0, Game1.stats.gameDifficulty + 4) == 0)
						{
							type = 1;
						}
						float num16 = MathHelper.Clamp(1.5f - (float)(int)Game1.stats.gameDifficulty * 0.45f, 0.2f, 5f);
						if (Game1.hud.inBoss)
						{
							num16 = MathHelper.Clamp(num16, 1f, 10f);
						}
						this.InitHazard(vector5, Map.dimen.X * Map.layerScale * Map.scale, num16, type, i, l);
						Game1.pManager.AddLenseFlare(vector5 + Game1.Scroll, 0.6f * Map.dimen.X, 1, 5);
					}
					if (Map.hazardCountdown > 0f && Map.hazardID == i)
					{
						float num17 = 1.5f - Map.hazardCountdown;
						if (Map.hazardType == 0)
						{
							Map.drawColor = Map.mapLayer[l].mapSeg[i].MultiplyColor(new Color(1f, 1f - num17, 1f - num17, 1f));
						}
						else if (Map.hazardType == 1)
						{
							Map.drawColor = Map.mapLayer[l].mapSeg[i].MultiplyColor(new Color(1f - num17, 1f, 1f, 1f));
						}
						Map.moveRandomX = (float)Rand.GetRandomInt(-5, 5) * num17;
					}
					float rotation = (float)Math.Sin(Map.speed) * 0.1f;
					sprite.Draw(Map.sourceIndexTexture, Map.pos + new Vector2(Map.moveRandomX, 0f), new Rectangle(3042, 823, 130, 87), Map.drawColor, rotation, new Vector2(65f, 33.5f), Map.dimen.X * Map.layerScale * Map.scale * worldScale, SpriteEffects.None, 1f);
					return true;
				}
				case 12:
				{
					Map.rotateRandom = (float)Math.Cos(Map.speed) * 0.05f * Map.scale * ((float)Map.sourceRect.Height * Map.scale / 1000f);
					Map.moveRandomX = (0f - (float)Math.Cos(Map.speed) * 0.05f * Map.scale) * ((float)Map.sourceRect.Height * Map.scale * Map.segmentScale.Y / 8f);
					Map.scaleRandom = (float)Math.Sin(Map.speed) * 0.05f * Map.scale * ((Map.segmentScale.X + Map.segmentScale.Y / 2f) * 0.25f * Map.scale);
					float num18 = MathHelper.Clamp((float)Math.Sin(Game1.hud.pulse) * 0.25f, 0f, 1f);
					Map.drawColor = Map.mapLayer[l].mapSeg[i].AddColor(new Vector4(num18, num18, num18, 0f));
					return true;
				}
				case 13:
				{
					int num = (int)((float)Map.sourceRect.Height * Map.mapLayer[l].mapSeg[i].Scale.Y * Map.scale * worldScale);
					float num2 = (float)(int)Map.segDef[Map.sourceIndex, Map.defIndex].Speed / 100f * ((float)Game1.screenHeight - (Map.pos.Y + (float)num)) / (float)(Game1.screenHeight / 4);
					float num3 = this.MapSegFrame * 10f + (float)i;
					Map.rotateRandom = (float)Math.Sin(num3) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale / 1000f) * num2;
					Map.moveRandomX = (float)Math.Sin(num3) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale * Map.segmentScale.X / 4f) * num2;
					Map.scaleRandom = (float)Math.Sin(num3) * 0.05f * Map.scale * ((Map.segmentScale.X + Map.segmentScale.Y / 2f) * 0.25f * Map.scale) * num2;
					break;
				}
				case 14:
					if (Game1.halfSecFrame > 30 && Game1.cManager.challengeArenas[Game1.cManager.currentChallenge].HighScore > 0)
					{
						Game1.events.InitEvent(7, isSideEvent: true);
					}
					sprite.Draw(Map.sourceIndexTexture, Map.pos, Map.sourceRect, Color.White, Map.mapLayer[l].mapSeg[i].Rotation, Map.center, new Vector2(Map.dimen.X * Map.layerScale - Map.scaleRandom, Map.dimen.Y * Map.layerScale + Map.scaleRandom) * worldScale, Map.mapLayer[l].mapSeg[i].Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
					if (Game1.cManager.currentChallenge > -1)
					{
						if (Game1.cManager.challengeArenas[Game1.cManager.currentChallenge].HighScore > 0)
						{
							float num4 = Map.scale * worldScale;
							Vector2 loc = Map.pos + new Vector2(20f, -50f) * num4;
							Game1.hud.scoreDraw.Draw(Game1.cManager.challengeArenas[Game1.cManager.currentChallenge].HighScore, loc, 0.4f * worldScale, Color.White, ScoreDraw.Justify.Center, 2);
							float num5 = Math.Min((float)Game1.cManager.challengeArenas[Game1.cManager.currentChallenge].HighScore / (float)(Game1.cManager.challengeArenas[Game1.cManager.currentChallenge].RankScore - 1) * 3f, 4f);
							int num6 = 0;
							for (int k = 0; (float)k < num5; k++)
							{
								num6++;
							}
							for (int m = 0; m < num6; m++)
							{
								int num7 = 0;
								if (m == 0 || m == num6 - 1)
								{
									num7 = 10;
								}
								sprite.Draw(Map.sourceIndexTexture, new Vector2(loc.X + (float)((m - 1) * 30 + 16 - (num6 - 2) * 15) * num4, loc.Y + (float)(56 - num7) * num4), new Rectangle(4047, 593, 49, 43), Color.White, 0f, new Vector2(25f, 22f), num4, SpriteEffects.None, 0f);
							}
						}
						if (Game1.cManager.challengeMode == ChallengeManager.ChallengeMode.None && Game1.character[0].State == CharState.Grounded && new Rectangle((int)(Map.mapLayer[l].mapSeg[i].Location.X - 130f), (int)Map.mapLayer[l].mapSeg[i].Location.Y - 150, 300, 450).Contains((int)Game1.character[0].Location.X, (int)Game1.character[0].Location.Y))
						{
							Game1.hud.InitFidgetPrompt(FidgetPrompt.Score);
							if (Game1.hud.KeyUp)
							{
								Game1.cManager.InitScoreBoard();
							}
						}
					}
					return false;
				case 16:
					Map.rotateRandom = (float)Math.Cos(this.MapSegFrameLocked * (float)(int)Map.segDef[Map.sourceIndex, Map.defIndex].Speed) * 0.1f;
					break;
				case 17:
				case 18:
				{
					Map.drawColor = Map.mapLayer[l].mapSeg[i].color;
					Vector2 zero = Vector2.Zero;
					float num19 = MathHelper.Clamp(Map.wManager.windStrength / 20f, -60f, 60f);
					num19 = ((Map.wManager.weatherType != WeatherType.SnowFierce) ? (num19 + (float)Math.Cos(Game1.map.MapSegFrameLocked * 10f + (float)i) * 5f) : (num19 + (float)Math.Cos(Game1.map.MapSegFrameLocked * 50f + (float)i) * 30f));
					Map.mapLayer[l].mapSeg[i].Rotation += (num19 - Map.mapLayer[l].mapSeg[i].Rotation) * Game1.FrameTime / 2f;
					float num20 = 0f;
					for (int n = 0; n < 6; n++)
					{
						switch (n)
						{
						case 1:
							Map.sourceRect = new Rectangle(825, 675, 687, 294);
							break;
						case 2:
							Map.sourceRect = new Rectangle(1512, 675, 662, 271);
							break;
						case 3:
							Map.sourceRect = new Rectangle(2174, 675, 511, 266);
							break;
						case 4:
							Map.sourceRect = new Rectangle(2282, 392, 382, 258);
							break;
						case 5:
							if (Map.flag == 17)
							{
								Map.sourceRect = new Rectangle(2664, 392, 188, 267);
							}
							else
							{
								Map.sourceRect = new Rectangle(3045, 910, 239, 274);
							}
							Map.center = new Vector2(Map.sourceRect.Width / 2, (float)Map.sourceRect.Height / 1.6f);
							break;
						}
						if (n < 5)
						{
							Map.center = new Vector2(Map.sourceRect.Width, Map.sourceRect.Height) / 2f;
						}
						Map.rotateRandom = (float)Math.Cos(Map.speed + (double)n) * 0.05f;
						Map.moveRandomX = (0f - (float)Math.Cos(Map.speed + (double)n)) * 2f;
						num20 += Map.mapLayer[l].mapSeg[i].Rotation / 200f;
						if (n > 0)
						{
							float num21 = (float)(-Map.sourceRect.Height) / 1.6f;
							zero += new Vector2((float)(0.0 - Math.Sin(num20)) * num21, (float)Math.Cos(num20) * num21);
						}
						sprite.Draw(Map.sourceIndexTexture, Map.pos + (zero + new Vector2(Map.moveRandomX, 0f)) * Map.dimen * worldScale, Map.sourceRect, Map.drawColor, num20 * Map.dimen.X / Map.dimen.Y + Map.rotateRandom, Map.center, Map.dimen * Map.layerScale * worldScale, Map.mapLayer[l].mapSeg[i].Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
					}
					return false;
				}
				case 19:
				case 44:
				case 45:
				case 49:
					return false;
				case 29:
				{
					Map.alphaRandom = 1f + (float)Math.Cos((double)this.MapSegFrameLocked * 20.0 + (double)i) / 4f;
					float num9 = Game1.character[0].Location.X * Game1.worldScale - Game1.Scroll.X;
					float num10 = Math.Abs((num9 - Map.pos.X) / (float)Game1.screenWidth) * 1.5f;
					float a2 = ((l < 6) ? (Game1.refractive ? 1f : ((1f - num10) * Map.alphaRandom)) : (Game1.refractive ? (1f - num10) : (Math.Max(num10, 0.5f) * Map.alphaRandom)));
					float num11 = (Game1.refractive ? 0.9f : 1f);
					Map.drawColor = Map.mapLayer[l].mapSeg[i].MultiplyColor(new Color(1f, 1f, 1f, a2));
					sprite.Draw(Map.sourceIndexTexture, Map.pos, Map.sourceRect, Map.drawColor, Map.mapLayer[l].mapSeg[i].Rotation, Map.center, Map.dimen * Map.layerScale * worldScale * num11, Map.mapLayer[l].mapSeg[i].Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
					return false;
				}
				case 32:
				case 33:
				case 34:
				case 35:
				case 36:
					Map.ropes[new Vector2(l, i)].Draw(sprite, Map.sourceIndexTexture, Map.mapLayer[l].mapSeg[i], Map.pos, Map.segDef[Map.sourceIndex, Map.defIndex].SrcRect, Map.segmentScale, Map.scale, worldScale, Game1.FrameTime);
					return false;
				case 28:
					if (Game1.refractive)
					{
						Map.drawColor = Map.mapLayer[l].mapSeg[i].color;
					}
					else
					{
						Map.alphaRandom = 1f + (float)Math.Cos((double)this.MapSegFrameLocked * 20.0 + (double)i) / 4f;
						float num12 = Game1.character[0].Location.X * Game1.worldScale - Game1.Scroll.X;
						float num13 = Math.Abs((num12 - Map.pos.X) / (float)Game1.screenWidth) * 1.5f;
						Map.drawColor = Map.mapLayer[l].mapSeg[i].MultiplyColor(new Color(1f, 1f, 1f, (1f - num13) * Map.alphaRandom));
						float num14 = (float)(Map.sourceRect.Width / 4) * (Map.dimen.X * Map.layerScale);
						Vector2 vector4 = Map.mapLayer[l].mapSeg[i].Location - Game1.Scroll * Map.scale + new Vector2(Rand.GetRandomFloat(0f - num14, num14), 0f);
						if (l == 6 && Map.hazardCountdown <= 0f && this.CheckHazard(vector4, 600, 98 - Game1.stats.gameDifficulty))
						{
							float timer = MathHelper.Clamp(1f - (float)(int)Game1.stats.gameDifficulty * 0.45f, 0.2f, 5f);
							this.InitHazard(vector4, Map.dimen.X * Map.layerScale * Map.scale, timer, 2, i, l);
							Game1.pManager.AddLenseFlare(vector4 + Game1.Scroll, 0.2f * Map.dimen.X, 0, 5);
						}
						if (Map.hazardCountdown > 0f && Map.hazardID == i && Map.hazardLayer == l)
						{
							float num15 = 1.5f - Map.hazardCountdown;
							Map.drawColor = Map.mapLayer[l].mapSeg[i].MultiplyColor(new Color(1f, 1f, 1f, Rand.GetRandomFloat(0.25f, 1f)));
							Map.moveRandomX = (float)Rand.GetRandomInt(-2, 2) * num15;
							Game1.pManager.AddSpray(Map.mapLayer[l].mapSeg[i].Location + Rand.GetRandomVector2(0f - num14, num14, -50f, 0f), new Vector2(0f, Rand.GetRandomFloat(-100f, 400f)), 0.5f, 0, 1, 5);
						}
					}
					sprite.Draw(Map.sourceIndexTexture, Map.pos + new Vector2(Map.moveRandomX, 0f), Map.sourceRect, Map.drawColor, Map.mapLayer[l].mapSeg[i].Rotation, Map.center, Map.dimen * Map.layerScale * worldScale * new Vector2(1f, Game1.refractive ? 0.9f : 1f), Map.mapLayer[l].mapSeg[i].Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
					return false;
				case 46:
				{
					Map.rotateRandom = (float)Math.Cos(Map.speed) * 0.05f;
					float num8 = (int)Map.mapLayer[l].mapSeg[i].color.A;
					Vector2 vector2 = Game1.character[0].Location * worldScale - Game1.Scroll;
					if ((Math.Abs(Map.pos.X - vector2.X) < 260f * worldScale && Math.Abs(Map.pos.Y - vector2.Y) < 400f * worldScale) || num8 < 250f)
					{
						num8 += (-20f - num8) * Game1.FrameTime * 10f;
					}
					if (Game1.character[1].Exists == CharExists.Exists)
					{
						Vector2 vector3 = Game1.character[1].Location * worldScale - Game1.Scroll;
						if ((Math.Abs(Map.pos.X - vector3.X) < 260f * worldScale && Math.Abs(Map.pos.Y - vector3.Y) < 400f * worldScale) || num8 < 250f)
						{
							num8 += (-20f - num8) * Game1.FrameTime * 10f;
						}
					}
					byte a = Map.mapLayer[l].mapSeg[i].color.A;
					Map.mapLayer[l].mapSeg[i].color.A = (byte)MathHelper.Clamp(num8, 0f, 255f);
					if (l > 2 && Map.mapLayer[l].mapSeg[i].color.A == 0 && a > 0)
					{
						Sound.PlayCue("terrain_grow");
					}
					Map.drawColor = new Color(Map.mapLayer[l].mapSeg[i].color.R, Map.mapLayer[l].mapSeg[i].color.G, Map.mapLayer[l].mapSeg[i].color.B, 255 - Map.mapLayer[l].mapSeg[i].color.A);
					sprite.Draw(Map.sourceIndexTexture, Map.pos + new Vector2(Map.rotateRandom * 60f, (float)(Map.mapLayer[l].mapSeg[i].color.A * 2) * worldScale), Map.sourceRect, Map.drawColor, Map.mapLayer[l].mapSeg[i].Rotation + Map.rotateRandom, Map.center, Map.dimen * Map.layerScale * worldScale * new Vector2((float)(255 - Map.mapLayer[l].mapSeg[i].color.A) / 255f, 1f), Map.mapLayer[l].mapSeg[i].Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
					return false;
				}
				case 47:
					Map.speed = (double)this.MapSegFrameLocked * (double)(int)Map.segDef[Map.sourceIndex, Map.defIndex].Speed + (double)i;
					Map.moveRandomY = 0f - Math.Abs((float)Math.Sin(Map.speed) * 145f * Map.scale);
					if (Map.moveRandomY > -4f && Map.moveRandomY < 4f)
					{
						Vector2 vector = Map.mapLayer[l].mapSeg[i].Location / Map.scale;
						Sound.PlayCue("piston", vector, (Game1.character[0].Location - vector).Length());
						VibrationManager.SetScreenShake(0.5f);
						for (int j = 0; j < 5; j++)
						{
							Game1.pManager.AddSpray(Map.mapLayer[l].mapSeg[i].Location + new Vector2(0f, Rand.GetRandomInt(90, 110)), new Vector2(0f, Rand.GetRandomFloat(-300f, -100f)), 0.4f, 0, 4, (l == 5) ? 5 : l);
						}
					}
					break;
				case 99:
					Map.rotateRandom = (0f - (float)Math.Sin(Map.speed) * 0.05f * Map.scale) * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale / 200f);
					Map.moveRandomX = (float)Math.Cos(Map.speed) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale * Map.segmentScale.X / 2f);
					Map.moveRandomY = (float)Math.Sin(Map.speed) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale * Map.segmentScale.X / 2f);
					break;
				}
				Map.moveRandomX *= worldScale;
				Map.moveRandomY *= worldScale;
			}
			if (Map.alphaRandom >= 1f)
			{
				Map.drawColor = Map.mapLayer[l].mapSeg[i].color;
			}
			else
			{
				Map.drawColor = Map.mapLayer[l].mapSeg[i].MultiplyColor(new Color(1f, 1f, 1f, Map.alphaRandom));
			}
			if (Map.flag >= 100)
			{
				if (Map.flag == 100)
				{
					for (int num22 = 0; num22 < Map.totalFrameCount; num22++)
					{
						Map.moveRandomX = (float)Math.Cos(Map.speed) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale * Map.segmentScale.X / 4f);
						Map.rotateRandom = (float)Math.Cos(Map.speed - (double)num22) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale / 100f);
						Map.scaleRandom = (float)Math.Sin(Map.speed + (double)num22) * 0.05f * Map.scale * ((Map.segmentScale.X + Map.segmentScale.Y / 2f) * 0.25f * Map.scale);
						if (num22 > 0)
						{
							Map.sourceRect.X += Map.sourceRect.Width;
						}
						float num23 = (float)Map.sourceRect.Height * Map.dimen.Y * Map.layerScale * (float)num22 * 0.05f * worldScale;
						Map.rotateRandom *= num22 + 1;
						Map.rotateRandom /= 4f;
						Map.moveRandomX *= num22;
						Map.moveRandomX /= 4f;
						Map.moveRandomY *= num22;
						Map.moveRandomX /= 4f;
						Map.scaleRandom *= num22;
						Map.scaleRandom /= 4f;
						if (!Map.mapLayer[l].mapSeg[i].Flip)
						{
							sprite.Draw(Map.sourceIndexTexture, Map.pos + new Vector2(Map.moveRandomX, 0f - num23 + Map.moveRandomY), Map.sourceRect, Map.drawColor, Map.mapLayer[l].mapSeg[i].Rotation + Map.rotateRandom, Map.center, (Map.dimen * Map.layerScale + new Vector2(0f - Map.scaleRandom, Map.scaleRandom)) * worldScale, SpriteEffects.None, 1f);
						}
						else
						{
							sprite.Draw(Map.sourceIndexTexture, Map.pos + new Vector2(Map.moveRandomX, 0f - num23 + Map.moveRandomY), Map.sourceRect, Map.drawColor, Map.mapLayer[l].mapSeg[i].Rotation + Map.rotateRandom, Map.center, (Map.dimen * Map.layerScale + new Vector2(0f - Map.scaleRandom, Map.scaleRandom)) * worldScale, SpriteEffects.FlipHorizontally, 1f);
						}
					}
				}
				else if (Map.flag > 100 && Map.flag < 104)
				{
					Map.sourceIndex = Map.mapLayer[l].mapSeg[i].SourceIndexSlot;
					Map.sourceIndexTexture = Map.mapsTex[Map.sourceIndex];
					Rectangle value = Map.sourceRect;
					for (int num24 = 0; num24 < Map.totalFrameCount; num24++)
					{
						Map.rotateRandom = (float)Math.Cos(Map.speed + (double)num24) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale / 20f);
						Map.moveRandomX = (float)Math.Cos(Map.speed + (double)num24) * 0.05f * Map.scale * ((float)(Map.sourceRect.Width + Map.sourceRect.Height / 2) * Map.scale * Map.segmentScale.X * 10f) * worldScale;
						Map.scaleRandom = (float)Math.Sin(Map.speed + (double)num24) * 0.05f * Map.scale * ((Map.segmentScale.X + Map.segmentScale.Y / 2f) * 0.25f * Map.scale);
						switch (Map.flag)
						{
						case 102:
							Map.rotateRandom /= 12f;
							Map.moveRandomX /= 8f;
							Map.scaleRandom = 0f;
							break;
						case 103:
							Map.rotateRandom /= 2f;
							Map.moveRandomX /= 8f;
							Map.scaleRandom /= 4f;
							break;
						default:
							Map.rotateRandom /= 2f;
							Map.moveRandomX /= 12f;
							Map.scaleRandom /= 4f;
							break;
						}
						float num25 = 0f;
						float y = 0f;
						if (num24 > 0)
						{
							value.X += Map.sourceRect.Width;
							if (num24 > 4)
							{
								num25 = (float)Map.sourceRect.Width * Map.dimen.X * Map.layerScale * 2.5f * worldScale;
								y = (float)Map.sourceRect.Height * Map.dimen.Y * Map.layerScale * 0.25f * worldScale;
							}
						}
						if (!Map.mapLayer[l].mapSeg[i].Flip)
						{
							sprite.Draw(Map.sourceIndexTexture, Map.pos + new Vector2(0f - num25 + (float)(num24 * Map.sourceRect.Width / 2) * Map.dimen.X * Map.layerScale * worldScale + Map.moveRandomX, y), value, Map.drawColor, Map.mapLayer[l].mapSeg[i].Rotation + Map.rotateRandom, Map.center, (Map.dimen * Map.layerScale + new Vector2(0f - Map.scaleRandom, Map.scaleRandom)) * worldScale, SpriteEffects.None, 1f);
						}
						else
						{
							sprite.Draw(Map.sourceIndexTexture, Map.pos + new Vector2(num25 + (float)(-num24 * Map.sourceRect.Width / 2) * Map.dimen.X * Map.layerScale * worldScale + Map.moveRandomX, y), value, Map.drawColor, Map.mapLayer[l].mapSeg[i].Rotation + Map.rotateRandom, Map.center, (Map.dimen * Map.layerScale + new Vector2(0f - Map.scaleRandom, Map.scaleRandom)) * worldScale, SpriteEffects.FlipHorizontally, 1f);
						}
					}
				}
				return false;
			}
			return true;
		}

		private void UpdateParticles(Vector2 partLoc, int l, int i, float finalScale, ParticleManager pMan)
		{
			float num = (Map.segmentScale.X + Map.segmentScale.Y) / 2f * finalScale;
			float num2 = (float)(Map.sourceRect.Width / 2) * (Map.segmentScale.X * finalScale);
			float num3 = (float)(Map.sourceRect.Height / 2) * (Map.segmentScale.Y * finalScale);
			if ((double)Map.particleTimer > 0.4)
			{
				switch (Map.flag)
				{
				case 5:
				{
					pMan.AddSmoke(partLoc, Rand.GetRandomVector2(-40f, 40f, -100f, -75f) * num + new Vector2(0f, -20f / num), 0.75f, 0.75f, 0.75f, 0.5f, Rand.GetRandomFloat(0.5f, 1.5f) * num - 0.1f, Rand.GetRandomFloat(3.75f, 6.25f), l);
					float num4 = (int)Map.mapLayer[l].mapSeg[i].color.A / 255;
					for (int j = 0; j < 2; j++)
					{
						pMan.AddFlameSpark(partLoc + Rand.GetRandomVector2(0f - num2, num2, 0f - num3, 0f), Rand.GetRandomVector2(-60f, 60f, -400f, -40f) * num, 90f, Rand.GetRandomFloat(0.4f, 0.6f) * Map.scale * num4, 0, l);
					}
					if (num4 == 1f && Rand.GetRandomInt(0, 10) == 0)
					{
						int randomInt = Rand.GetRandomInt(0, 4);
						Vector2 loc = partLoc + Rand.GetRandomVector2(0f - num2, num2, 0f - num3, num3);
						for (int k = 0; k < randomInt; k++)
						{
							pMan.AddBounceSpark(loc, Rand.GetRandomVector2(-120f, 120f, -200f, -100f), 0.2f, l);
						}
					}
					if (Map.segmentScale.X <= Map.segmentScale.Y)
					{
						pMan.AddFlamePuff(partLoc + new Vector2(Rand.GetRandomFloat((0f - num2) / 2f, num2 / 2f), 0f), Rand.GetRandomVector2(-5f, 5f, -100f, -40f) * num, Rand.GetRandomFloat(0.8f, 1.2f) * num, l);
					}
					return;
				}
				case 6:
					if (Rand.GetRandomInt(0, 10) == 0)
					{
						pMan.AddLeaf(partLoc + Rand.GetRandomVector2(0f - num2, num2, 0f, num3), Rand.GetRandomVector2(-40f, 40f, 80f, 120f) * Map.scale, 1f, 1f, 1f, 1f, Rand.GetRandomFloat(0.9f, 1.2f) * Map.scale, l);
					}
					return;
				case 11:
					if (Rand.GetRandomInt(0, 20 / (int)(num2 / 50f)) == 0)
					{
						pMan.AddCaveLeaf(partLoc + new Vector2(Rand.GetRandomFloat(0f - num2, num2), 0f), Rand.GetRandomVector2(-40f, 40f, 80f, 120f) * Map.scale, Rand.GetRandomFloat(0.2f, 1f) * Map.scale, l);
					}
					return;
				case 10:
					if (Rand.GetRandomInt(0, 20 / (int)(num2 / 50f)) == 0)
					{
						pMan.AddSmoke(partLoc + Rand.GetRandomVector2(0f - num2, num2, 0f - num3, num3), Rand.GetRandomVector2(-40f, 40f, -100f, -75f) * num + new Vector2(0f, -20f / num), 1f, 1f, 1f, 0.75f, Rand.GetRandomFloat(0.75f, 2f) * num, Rand.GetRandomFloat(3.75f, 6.25f), l);
					}
					return;
				}
			}
			switch (Map.flag)
			{
			case 8:
				if (Rand.GetRandomInt(0, 100) < 10)
				{
					pMan.AddGlowSpark(partLoc + Rand.GetRandomVector2(-Map.sourceRect.Width / 3, Map.sourceRect.Width / 3, 0f - num3, 0f), new Vector2(0f, 0f - Rand.GetRandomFloat(1f, 20f)) * num, Rand.GetRandomFloat(0.5f, 2f) * num, 1f, num, l);
				}
				break;
			case 103:
				if (Rand.GetRandomInt(0, 100) < 4)
				{
					float num12 = (float)Map.sourceRect.Width * Map.segmentScale.X * finalScale * ((float)Map.totalFrameCount * 0.25f);
					if (Map.mapLayer[l].mapSeg[i].Flip)
					{
						num12 *= -1f;
					}
					num = Math.Max(num, 1f);
					pMan.AddGlowSpark(partLoc + Rand.GetRandomVector2(0f, num12, 0f - num3, num3), new Vector2(0f, 0f - Rand.GetRandomFloat(1f, 20f)) * num, Rand.GetRandomFloat(0.5f, 2f) * num, 1f, 0.5f * num, l);
				}
				break;
			case 48:
				if (Rand.GetRandomInt(0, 100) < 10)
				{
					for (int m = 0; m < Map.segDef[Map.sourceIndex, Map.defIndex].Speed; m++)
					{
						pMan.AddGlowSpark(partLoc + Rand.GetRandomVector2(0f - num2, num2, 0f - num3, num3), Vector2.Zero, Rand.GetRandomFloat(0.2f, 1f), 1f, num * 0.5f, l);
					}
				}
				break;
			case 9:
				if (Map.particleTimer > 0.4f)
				{
					float num6 = 45f * num;
					pMan.AddBubbleDrip(partLoc + Rand.GetRandomVector2(0f - num6, num6, 0f, num6 / 1.5f), Rand.GetRandomFloat(0.5f, 1.2f) * num, l);
				}
				break;
			case 15:
				if (Map.particleTimer > 0.3f)
				{
					float randomFloat = Rand.GetRandomFloat(0f, 350f);
					if (Map.mapLayer[l].mapSeg[i].Flip)
					{
						pMan.AddCheckPointGlow(partLoc + new Vector2(0f - randomFloat, 150f + randomFloat * -0.5f), Rand.GetRandomFloat(0.5f, 1.2f) * num, 5);
					}
					else
					{
						pMan.AddCheckPointGlow(partLoc + new Vector2(randomFloat, 150f + randomFloat * -0.5f), Rand.GetRandomFloat(0.5f, 1.2f) * num, 5);
					}
				}
				break;
			case 19:
				if (Rand.GetRandomFloat(0f, 20f) < Map.segmentScale.Y * finalScale)
				{
					pMan.AddWaterDrip(partLoc + new Vector2(Rand.GetRandomFloat(0f - num2, num2), 0f - num3), Rand.GetRandomFloat(0.2f, 0.5f), l);
				}
				break;
			case 20:
				if (l == 4 && (Map.wManager.weatherType == WeatherType.RainLight || Map.wManager.weatherType == WeatherType.RainHeavy) && Game1.longSkipFrame > 3 && Map.wManager.playerCanPrecipitate)
				{
					pMan.AddPondRipple(partLoc + new Vector2(Rand.GetRandomFloat(0f - num2, num2), Rand.GetRandomFloat(-Map.sourceRect.Height, Map.sourceRect.Height) / 4f * Map.segmentScale.Y * finalScale), Rand.GetRandomFloat(0.01f, 0.5f) * Map.scale, l);
				}
				break;
			case 22:
				if (Game1.longSkipFrame > 3 && Rand.GetRandomInt(0, 4) == 0)
				{
					float num9 = Rand.GetRandomFloat(0.4f, 2f) * num2 / 200f;
					pMan.AddWaterFall(partLoc + Rand.GetRandomVector2(0f - num2, num2, 0f - num3, 0f), 1500f / num9 * Map.layerScale, num9 * finalScale, (int)(partLoc.Y + num3), l);
				}
				break;
			case 23:
				if (Rand.GetRandomInt(0, 8) < (int)(num2 / 20f))
				{
					if (Game1.longSkipFrame > 3)
					{
						pMan.AddWaterSplash(partLoc + new Vector2(Rand.GetRandomFloat(-Map.sourceRect.Width, Map.sourceRect.Width) / 3f * Map.segmentScale.X * finalScale, (0f - num3) / 1.1f), Rand.GetRandomFloat(0.002f, 0.008f) * num, 1, l);
					}
					if (Rand.GetRandomInt(0, 15) == 0)
					{
						pMan.AddSmoke(partLoc + new Vector2(Rand.GetRandomFloat(-Map.sourceRect.Width, Map.sourceRect.Width) / 2f * Map.segmentScale.X * finalScale, 0f - Rand.GetRandomFloat(num3 / 1.5f, num3 / 3f)), Rand.GetRandomVector2(-80f, 80f, 20f, 100f) * Map.scale, 1f, 1f, 1f, 0.5f, 0.4f * num, 2f, l);
					}
				}
				break;
			case 30:
				if (this.fishCount < 6 && l > 2 && Map.wManager.weatherType != WeatherType.RainHeavy && Rand.GetRandomInt(1, 100) < 2 * l)
				{
					pMan.AddFish(partLoc + Rand.GetRandomVector2(-300f, 300f, -140f, 0f), Vector2.Zero, Rand.GetRandomFloat(0.6f, 1.5f) * Map.scale, (int)((float)i + this.MapSegFrame * 100f / Game1.FrameTime), l);
				}
				break;
			case 31:
				if (Rand.GetRandomInt(0, 8) < (int)(num2 / 20f))
				{
					if (Game1.skipFrame > 1)
					{
						pMan.AddWaterSplash(partLoc + new Vector2(Rand.GetRandomFloat(-Map.sourceRect.Width, Map.sourceRect.Width) / 8f * Map.segmentScale.X * finalScale, 0f), Rand.GetRandomFloat(0.2f, 0.3f) * num, 0, l);
					}
					if (Rand.GetRandomInt(0, 12) == 0)
					{
						pMan.AddSmoke(partLoc + new Vector2(Rand.GetRandomFloat(-Map.sourceRect.Width, Map.sourceRect.Width) / 2f * Map.segmentScale.X * finalScale, 0f), Rand.GetRandomVector2(-100f, 100f, -60f, 0f) * Map.scale, 1f, 1f, 1f, 0.5f, 0.5f * num - 0.1f, 2f, l);
					}
				}
				break;
			case 43:
				if (Rand.GetRandomInt(0, 100) == 0)
				{
					float rotation4 = Map.mapLayer[l].mapSeg[i].Rotation;
					Vector2 vector5 = new Vector2(Rand.GetRandomFloat(0f - num2, num2), Rand.GetRandomFloat(-Map.sourceRect.Height, Map.sourceRect.Height) / 6f * Map.segmentScale.Y * finalScale);
					partLoc += new Vector2((float)(Math.Cos(rotation4) * (double)vector5.X - Math.Sin(rotation4) * (double)vector5.Y), (float)(Math.Cos(rotation4) * (double)vector5.Y + Math.Sin(rotation4) * (double)vector5.X));
					float randomFloat2 = Rand.GetRandomFloat(0.1f, 0.5f);
					Game1.pManager.AddEmitLava(partLoc, Vector2.Zero, randomFloat2, colliding: false, 3, l);
					for (int num13 = 0; num13 < 3; num13++)
					{
						Game1.pManager.AddEmitLava(partLoc, Rand.GetRandomVector2(-200f, 200f, -400f, -200f), randomFloat2 / 2f, colliding: true, 0, l);
					}
				}
				break;
			case 44:
			{
				float rotation3 = Map.mapLayer[l].mapSeg[i].Rotation;
				Vector2 vector3 = new Vector2(Rand.GetRandomFloat(0f - num2, num2), 0f - num3);
				partLoc += new Vector2((float)(Math.Cos(rotation3) * (double)vector3.X - Math.Sin(rotation3) * (double)vector3.Y), (float)(Math.Cos(rotation3) * (double)vector3.Y + Math.Sin(rotation3) * (double)vector3.X));
				Vector2 vector4 = partLoc + new Vector2(0f, Map.segmentScale.Y * finalScale * 100f);
				Vector2 traj = new Vector2((float)(Math.Cos(rotation3) * (double)vector4.X - Math.Sin(rotation3) * (double)vector4.Y), (float)(Math.Cos(rotation3) * (double)vector4.Y + Math.Sin(rotation3) * (double)vector4.X)) - new Vector2((float)(Math.Cos(rotation3) * (double)partLoc.X - Math.Sin(rotation3) * (double)partLoc.Y), (float)(Math.Cos(rotation3) * (double)partLoc.Y + Math.Sin(rotation3) * (double)partLoc.X));
				pMan.AddLavaSpray(partLoc, traj, 0.75f, 0, l);
				break;
			}
			case 45:
				if (Rand.GetRandomInt(0, 5) == 0)
				{
					float rotation2 = Map.mapLayer[l].mapSeg[i].Rotation;
					int num10 = (int)Math.Max(num2 / 80f, 1f);
					for (int num11 = 0; num11 < num10; num11++)
					{
						Vector2 vector2 = new Vector2(Rand.GetRandomFloat(0f - num2, num2), 0f - num3);
						vector2 = new Vector2((float)(Math.Cos(rotation2) * (double)vector2.X - Math.Sin(rotation2) * (double)vector2.Y), (float)(Math.Cos(rotation2) * (double)vector2.Y + Math.Sin(rotation2) * (double)vector2.X));
						pMan.AddEmitLava(partLoc + vector2, Vector2.Zero, Rand.GetRandomFloat(0.5f, 0.9f), colliding: true, 3, l);
					}
				}
				break;
			case 49:
				if ((double)Map.particleTimer > 0.4)
				{
					int num7 = (int)(Map.segmentScale.X * finalScale * 4f);
					for (int n = 0; n < num7; n++)
					{
						float rotation = Map.mapLayer[l].mapSeg[i].Rotation;
						Vector2 vector = new Vector2(Rand.GetRandomFloat(0f - num2, num2), 0f);
						partLoc += new Vector2((float)(Math.Cos(rotation) * (double)vector.X - Math.Sin(rotation) * (double)vector.Y), (float)(Math.Cos(rotation) * (double)vector.Y + Math.Sin(rotation) * (double)vector.X));
						float num8 = Map.segmentScale.Y * finalScale;
						pMan.AddSmoke(partLoc, Rand.GetRandomVector2(-60f, 60f, -200f, -50f) * num8, 0f, 0f, 0f, 0.5f, Rand.GetRandomFloat(0.5f, 1f) * num8, 3f, l);
					}
				}
				break;
			case 101:
				if (l > 2 && this.bugCount < 6 && Map.wManager.weatherType != WeatherType.RainHeavy && Rand.GetRandomInt(1, 100) < 2 * l)
				{
					float num5 = (float)Map.sourceRect.Width * ((float)Map.totalFrameCount * 0.25f);
					if (Map.mapLayer[l].mapSeg[i].Flip)
					{
						num5 *= -1f;
					}
					pMan.AddButterfly(partLoc + Rand.GetRandomVector2(0f, num5, -Map.sourceRect.Height / 2, Map.sourceRect.Height / 2), Vector2.Zero, Rand.GetRandomFloat(0.4f, 1f) * Map.scale, Rand.GetRandomInt(0, 3), (int)((float)i + this.MapSegFrame * 100f / Game1.FrameTime), l);
				}
				break;
			}
		}

		public void Update(ParticleManager pMan, Character[] c, float frameTime, float worldScale)
		{
			this.CheckTransitions(c, pMan);
			if (this.mapScript.IsReading && Game1.stats.playerLifeState == 0 && (this.GetTransVal() == 0f || !Game1.events.anyEvent))
			{
				this.mapScript.DoScript(c, pMan, null);
			}
			if (Game1.longSkipFrame == 1)
			{
				for (int i = 0; i < this.eventRegion.Count; i++)
				{
					if (this.eventRegion[i] != null)
					{
						this.eventRegion[i].Update(c, pMan, frameTime);
					}
				}
			}
			else if (Game1.longSkipFrame == 2)
			{
				for (int j = 0; j < this.bombRegions.Count; j++)
				{
					if (this.bombRegions[j] != null && !pMan.CheckBomb(this.bombRegions[j].ID))
					{
						this.bombRegions[j].AddBomb(pMan);
					}
				}
				Map.darkness = 0f;
				Map.darkSpeed = 0.5f;
				for (int k = 0; k < this.darkRegions.Count; k++)
				{
					if (this.darkRegions[k] != null && this.darkRegions[k].Region.Contains((int)c[0].Location.X, (int)c[0].Location.Y))
					{
						Map.darkness = this.darkRegions[k].Darkness;
						Map.darkSpeed = this.darkRegions[k].Speed;
					}
				}
				if (!Map.initingSegments)
				{
					foreach (KeyValuePair<Vector2, RopeElement> rope in Map.ropes)
					{
						rope.Value.Update(frameTime * 4f);
					}
				}
			}
			else if (Game1.longSkipFrame == 3)
			{
				float num = Math.Max(this.reverbMin, 0f);
				for (int l = 0; l < this.reverbRegions.Count; l++)
				{
					if (this.reverbRegions[l] != null && this.reverbRegions[l].Region.Contains((int)c[0].Location.X, (int)c[0].Location.Y))
					{
						num = this.reverbRegions[l].ReverbPercent;
					}
				}
				if (this.transInFrame > 0f)
				{
					this.reverbPercent = num;
				}
				else if (this.reverbPercent > num)
				{
					this.reverbPercent = Math.Max(this.reverbPercent - frameTime * 2f, 0f);
				}
				else if (this.reverbPercent < num)
				{
					this.reverbPercent = Math.Min(this.reverbPercent + frameTime * 2f, num);
				}
			}
			if (this.transInFrame > 0f)
			{
				Game1.worldDark = Map.darkness + 0.01f;
			}
			else if (Game1.worldDark > Map.darkness)
			{
				Game1.worldDark = Math.Max(Game1.worldDark - frameTime * Map.darkSpeed, 0f);
			}
			else if (Game1.worldDark < Map.darkness)
			{
				Game1.worldDark = Math.Min(Game1.worldDark + frameTime * Map.darkSpeed, Map.darkness);
			}
			if (this.mapAssetsLoaded)
			{
				for (int m = 0; m < this.warpRegions.Count && (this.warpRegions[m] == null || !this.warpRegions[m].Warp(c, this, pMan, this.warpStage > 0)); m++)
				{
				}
				if (this.warpStage > 0)
				{
					this.UpdateWarp(pMan, c, frameTime);
				}
				for (int n = 0; n < this.doorRegions.Count && (this.doorRegions[n] == null || !this.doorRegions[n].EnterDoor(c, this, pMan, this.doorStage > 0)); n++)
				{
				}
				if (this.doorStage > 0)
				{
					this.UpdateDoor(pMan, c, frameTime);
				}
			}
			if (Game1.longSkipFrame > 3)
			{
				if (this.fogRegion.Width != 0)
				{
					bool flag = this.fogRegion.Contains((int)Game1.character[0].Location.X, (int)Game1.character[0].Location.Y);
					float num2 = frameTime * 1.6f;
					if (this.fogRegionClear)
					{
						if (flag)
						{
							Map.fogRegionAlpha -= num2;
						}
						else
						{
							Map.fogRegionAlpha += num2;
						}
					}
					else if (flag)
					{
						Map.fogRegionAlpha += num2;
					}
					else
					{
						Map.fogRegionAlpha -= num2;
					}
				}
				else
				{
					Map.fogRegionAlpha += frameTime * 1.6f;
				}
				Map.fogRegionAlpha = MathHelper.Clamp(Map.fogRegionAlpha, 0f, 1f);
			}
			if (this.MapSegFrameSpeed < 10f)
			{
				this.MapSegFrameSpeed += frameTime * 2f;
				if (this.MapSegFrameSpeed > 10f)
				{
					this.MapSegFrameSpeed = 10f;
				}
			}
			this.MapSegFrame += frameTime / this.MapSegFrameSpeed;
			if (this.MapSegFrame > 6.28f)
			{
				this.MapSegFrame -= 6.28f;
			}
			this.MapSegFrameLocked += frameTime / 10f;
			if (this.MapSegFrameLocked > 6.28f)
			{
				this.MapSegFrameLocked -= 6.28f;
			}
			if (Map.particleTimer > 0.4f)
			{
				Map.particleTimer -= 0.4f;
			}
			Map.particleTimer += frameTime;
			if (Map.counter > 1f)
			{
				Map.counter -= 1f;
			}
			Map.counter += frameTime * 24f;
			Map.prevHazardTimer = Map.hazardTimer;
			float num3 = 1f;
			switch (Map.hazardSpeed)
			{
				case HazardSpeed.EmitRocks: num3 = 0.8f; break;
				case HazardSpeed.EmitLava: num3 = 0.6f; break;
			}
			if (Game1.cManager.currentChallenge > -1)
			{
				if (Game1.cManager.challengeState != 0)
				{
					Map.hazardTimer += frameTime * num3 * 1.5f;
					this.hazardRot += frameTime;
				}
				else
				{
					this.hazardRot = (Map.hazardTimer = (Map.hazardCountdown = 0f));
				}
			}
			else
			{
				Map.hazardTimer += frameTime * num3;
				if (this.hazardRot > Math.PI * 2.0)
				{
					this.hazardRot -= Math.PI * 2.0;
				}
				this.hazardRot += frameTime;
			}
			if (Map.hazardTimer > 6f)
			{
				Map.hazardTimer -= 6f;
			}
			if (Map.hazardCountdown > 0f)
			{
				float num4 = Map.hazardCountdown;
				Map.hazardCountdown -= frameTime;
				if (num4 > 0f && Map.hazardCountdown < 0f)
				{
					this.CompleteHazard(pMan);
				}
			}
			Game1.wManager.UpdateRegions(pMan, c, this);
		}

		public void Draw(SpriteBatch sprite, int startLayer, int endLayer, float worldScale, ParticleManager pMan, Texture2D[] particlesTex)
		{
			if (!this.mapAssetsLoaded)
			{
				Game1.BlurScene(1f);
				this.transInFrame = 6f;
				return;
			}
			if (startLayer == 0)
			{
				Game1.wManager.DrawBackdrop(sprite, pMan, Map.backDropTex, Map.backdropVideoTexture, Map.mapsTex, particlesTex, Map.nullTex, Map.backDrop, this.leftEdge, this.topEdge);
			}
			bool flag = !Game1.refractive && Game1.hud.inventoryState == InventoryState.None && !Game1.hud.isPaused && Game1.menu.prompt != promptDialogue.SkipEvent;
			if (this.GetTransVal() > 0f)
			{
				for (int i = 0; i < Map.mapsTex.Length; i++)
				{
					if (Map.mapsTex[i] == null || Map.mapsTex[i].IsDisposed)
					{
						return;
					}
				}
			}
			for (int j = startLayer; j < endLayer; j++)
			{
				int num;
				if ((num = Map.mapLayer[j].mapSeg.Length) > 0)
				{
					Map.layerScale = 1f;
					switch (j)
					{
					case 0:
						Map.scale = 0.2f;
						Map.layerScale = 10f;
						break;
					case 1:
						Map.scale = 0.3f;
						Map.layerScale = 6f;
						break;
					case 2:
						Map.scale = 0.5f;
						break;
					case 3:
						Map.scale = 0.85f;
						break;
					case 4:
						if (this.backScroll == 1)
						{
							Map.scale = 0.85f;
						}
						else
						{
							Map.scale = 0.9f;
						}
						break;
					case 5:
					case 6:
						Map.scale = 1f;
						break;
					case 7:
						Map.scale = 1.1f;
						break;
					case 8:
						Map.scale = 1.25f;
						break;
					}
					for (int k = 0; k < num; k++)
					{
						if (Map.mapLayer[j].mapSeg[k] == null)
						{
							continue;
						}
						Map.sourceIndex = Map.mapLayer[j].mapSeg[k].SourceIndexSlot;
						Map.defIndex = Map.mapLayer[j].mapSeg[k].Index;
						Map.sourceRect = Map.segDef[Map.sourceIndex, Map.defIndex].SrcRect;
						Map.flag = Map.segDef[Map.sourceIndex, Map.defIndex].Flag;
						if (!this.CheckRenderable(j, k, Map.scale, worldScale, Map.sourceRect))
						{
							continue;
						}
						if (Map.flag > 54 && Map.flag < 60)
						{
							this.UpdateEmitter(Map.mapLayer[j].mapSeg[k].Location, j, k, pMan);
							continue;
						}
						Map.totalFrameCount = Map.segDef[Map.sourceIndex, Map.defIndex].FrameCount;
						Map.sourceIndexTexture = Map.mapsTex[Map.sourceIndex];
						Map.speed = (double)this.MapSegFrame * (double)(int)Map.segDef[Map.sourceIndex, Map.defIndex].Speed + (double)k;
						Map.segmentScale = Map.mapLayer[j].mapSeg[k].Scale;
						Map.flagEnabled = Map.mapLayer[j].mapSeg[k].FlagEnabled;
						if (Map.counter > 1f && Map.totalFrameCount > 1 && Map.flag < 100 && flag)
						{
							Map.mapLayer[j].mapSeg[k].FrameCount++;
							if (Map.mapLayer[j].mapSeg[k].FrameCount > Map.totalFrameCount - 1)
							{
								Map.mapLayer[j].mapSeg[k].FrameCount = 0;
							}
						}
						if (Map.flag > 4 && j > 2 && (double)Map.particleTimer > 0.1 && Map.flagEnabled && flag)
						{
							this.UpdateParticles(Map.mapLayer[j].mapSeg[k].Location, j, k, Map.scale * Map.layerScale, pMan);
						}
						if ((j != 4 && j < 6) || this.CheckRefract(Map.mapLayer[j].mapSeg[k].Refractive))
						{
							if (Map.totalFrameCount > 1)
							{
								this.GetSrect(j, k);
							}
							if (this.UpdateMotion(j, k, worldScale, sprite))
							{
								sprite.Draw(Map.sourceIndexTexture, Map.pos + new Vector2(Map.moveRandomX, Map.moveRandomY), Map.sourceRect, Map.drawColor, Map.mapLayer[j].mapSeg[k].Rotation + Map.rotateRandom, Map.center, (Map.dimen + new Vector2(0f - Map.scaleRandom, Map.scaleRandom)) * Map.layerScale * worldScale, Map.mapLayer[j].mapSeg[k].Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
							}
						}
					}
				}
				if (!Game1.refractive)
				{
					if (j > pMan.startLayer)
					{
						pMan.DrawMapParticles(particlesTex, worldScale, j);
					}
					if (j > 1 && j < 5 && (this.fogColor.W + Map.wManager.fogWeatherAlpha) * Map.fogRegionAlpha > 0f)
					{
						this.DrawFog(sprite, j);
					}
				}
			}
		}
	}
}
