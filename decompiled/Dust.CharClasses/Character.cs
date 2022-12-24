using System;
using Dust.ai;
using Dust.Audio;
using Dust.HUD;
using Dust.MapClasses;
using Dust.Particles;
using Dust.PCClasses;
using Dust.Vibration;
using Lotus.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dust.CharClasses
{
	public class Character
	{
		public byte RandomTextures = 1;

		public byte RandomWeapons = 1;

		public byte RandomSkin;

		private byte randomWeapon;

		private static Texture2D[] sprites_01_Tex = new Texture2D[99];

		private static Texture2D[] sprites_02_Tex = new Texture2D[23];

		private static Texture2D[] sprites_03_Tex = new Texture2D[14];

		private static Texture2D[] sprites_04_Tex = new Texture2D[2];

		private static Texture2D[] sprites_05_Tex = new Texture2D[1];

		private int textureID;

		private int textureIndex;

		private int prevTextureID;

		private int prevTextureIndex;

		private int[] textureRange = new int[4];

		public Team Team;

		public int ID;

		public NPCType NPC;

		public bool QuestGiver;

		public int CollectEquipID;

		public bool KeyLeft;

		public bool KeyRight;

		public bool KeyUp;

		public bool KeyDown;

		public bool KeyJump;

		public bool KeyAttack;

		public bool KeySecondary;

		public bool KeyThrow;

		public bool KeyEvade;

		public FlyingType FlyType;

		public PatrollingType PatrolType;

		private Vector2 flyingOffset = Vector2.Zero;

		private float flyingSpeed;

		private float animFrameTime;

		private int curAnimFrame;

		public int maxAnimFrames;

		public string AnimName;

		public int AnimFrame;

		private int Anim;

		private float prevDuration;

		private Frame prevFrame;

		private KeyFrame nextKeyFrame = new KeyFrame(0);

		private Frame returnFrame;

		private float et;

		private float curFrame;

		private float shadowAlpha;

		private float lungeTint = 255f;

		public float glowing;

		public bool refracting;

		private Rectangle updateRect;

		private Rectangle renderRect;

		public bool renderable = true;

		public Color defaultColor = Color.White;

		public int maxIdles;

		public string Name = string.Empty;

		public Vector2 Location;

		public Vector2 PLoc;

		public Vector2 Trajectory;

		private float CharRotationDest;

		public float CharRotation;

		public CharDir Face;

		public CharState State;

		public DyingTypes DieType;

		public SpawnTypes SpawnType;

		public DefenseStates Defense;

		public CanLiftType LiftType;

		public DamageTypes DamageType;

		public LungeStates LungeState;

		public int ledgeAttach = -1;

		private float runTimer;

		public bool IsFalling;

		private bool canJump = true;

		private float footOffset = 130f;

		private float Friction = 2000f;

		private float jumpTime;

		public float CharCol;

		public float Speed = 600f;

		public float JumpVelocity = 900f;

		public float LifeBarPercent;

		public float ShadowWidth = 0.75f;

		public float CanHurtFrame;

		public float CanHurtProjectileFrame;

		public float DyingFrame = -1f;

		public float HPLossFrame;

		public EtherealState Ethereal;

		public int MaxHP = 80;

		public int HP;

		public int pHP;

		public int Strength;

		public int DefaultWidth = 160;

		public int DefaultHeight = 220;

		public int Height;

		public int Aggression = 10;

		public int GrabbedBy = -1;

		public int ReturnExp;

		public float MaskGlow;

		public CharExists Exists;

		public bool CanCancel;

		public bool Floating;

		private bool Hanging;

		public int Boosting;

		public float BoostTraj;

		public bool WallInWay;

		public bool Holding;

		public bool CanFallThrough;

		public TerrainType terrainType;

		public bool alwaysUpdatable;

		public StatusEffects StatusEffect;

		public int StatusStrength;

		public float StatusTime;

		public float DownTime;

		public float MaxDownTime = 5f;

		private static GamePadState curState = default(GamePadState);

		private static GamePadState prevState = default(GamePadState);

		private Script script;

		public CharDef Definition;

		public AI Ai;

		public PartsSnapshot[] partsSnap;

		public PressedKeys PressedKey;

		public QueuedKey QueuedKey;

		public int[] GotoGoal = new int[3] { -1, -1, -1 };

		public int[] QueueGoal = new int[3] { -1, -1, -1 };

		public int QueueTrig = -1;

		public Texture2D GetSheet(int sheet, int id)
		{
			switch (sheet)
			{
				case 2: return Character.sprites_02_Tex[id];
				case 3: return Character.sprites_03_Tex[id];
				case 4: return Character.sprites_04_Tex[id];
				case 5: return Character.sprites_05_Tex[id];
			}
			return Character.sprites_01_Tex[id];
		}

		internal static void LoadDustTextures(ContentManager content)
		{
			Character.sprites_01_Tex[0] = content.Load<Texture2D>("gfx/chars/sprites_01_1");
			Character.sprites_02_Tex[0] = content.Load<Texture2D>("gfx/chars/sprites_02_1");
			Character.sprites_03_Tex[0] = content.Load<Texture2D>("gfx/chars/sprites_03_1");
			Character.sprites_04_Tex[0] = content.Load<Texture2D>("gfx/chars/sprites_04_1");
			Character.sprites_05_Tex[0] = content.Load<Texture2D>("gfx/chars/sprites_05_1");
		}

		public Character(Vector2 newLoc, CharDef newCharDef, int newId, string newName, Team newTeam)
		{
			this.script = new Script(this);
			this.NewCharacter(newLoc, newCharDef, newId, newName, newTeam, ground: false);
		}

		public int NewCharacter(Vector2 newLoc, CharDef newCharDef, int newId, string newName, Team newTeam, bool ground)
		{
			this.Exists = CharExists.Init;
			this.renderable = false;
			this.RandomTextures = 1;
			this.RandomSkin = 0;
			this.RandomWeapons = 1;
			this.randomWeapon = 0;
			this.textureID = 0;
			this.textureIndex = 0;
			this.prevTextureID = 0;
			this.prevTextureIndex = 0;
			for (int i = 0; i < this.textureRange.Length; i++)
			{
				this.textureRange[i] = 0;
			}
			this.FlyType = FlyingType.None;
			this.flyingOffset = Vector2.Zero;
			this.flyingSpeed = 0f;
			this.animFrameTime = 0f;
			this.maxAnimFrames = 0;
			this.AnimName = string.Empty;
			this.AnimFrame = 0;
			this.Anim = 0;
			this.et = 0f;
			this.curFrame = 0f;
			this.CharRotation = (this.CharRotationDest = 3.14f);
			this.shadowAlpha = 0f;
			this.lungeTint = 255f;
			this.glowing = 0f;
			this.refracting = false;
			this.defaultColor = new Color(1f, 1f, 1f, 1f);
			this.maxIdles = 1;
			this.Name = newName;
			this.Location = (this.PLoc = newLoc);
			this.Trajectory = Vector2.Zero;
			this.NPC = NPCType.None;
			this.ledgeAttach = -1;
			this.IsFalling = false;
			this.canJump = true;
			this.footOffset = 130f;
			this.Friction = 6000f;
			this.jumpTime = 0f;
			this.CharCol = 0f;
			this.runTimer = 0f;
			this.Speed = 600f;
			this.JumpVelocity = 900f;
			this.LifeBarPercent = 0f;
			this.ShadowWidth = 0.75f;
			this.CanHurtFrame = 0f;
			this.CanHurtProjectileFrame = 0f;
			this.DyingFrame = -1f;
			this.HPLossFrame = 0f;
			this.ReturnExp = -1;
			this.MaskGlow = 0f;
			this.MaxHP = (this.HP = (this.pHP = 80));
			this.Strength = 0;
			this.DefaultWidth = 160;
			this.DefaultHeight = 220;
			this.Height = 0;
			this.Aggression = 10;
			this.DamageType = DamageTypes.NotSpecies;
			this.LiftType = CanLiftType.Normal;
			this.GrabbedBy = -1;
			this.CanCancel = false;
			this.Floating = false;
			this.Boosting = -1;
			this.BoostTraj = 0f;
			this.WallInWay = false;
			this.Holding = false;
			this.CanFallThrough = false;
			this.Ai = null;
			this.Face = ((Rand.GetRandomInt(0, 2) != 0) ? CharDir.Right : CharDir.Left);
			this.Definition = newCharDef;
			this.ID = newId;
			this.Team = newTeam;
			this.partsSnap = new PartsSnapshot[this.Definition.Frames[0].Parts.Length];
			for (int j = 0; j < this.partsSnap.Length; j++)
			{
				this.partsSnap[j] = new PartsSnapshot();
			}
			this.State = CharState.Air;
			this.PatrolType = PatrollingType.Normal;
			this.StatusEffect = StatusEffects.Normal;
			this.LiftType = CanLiftType.Normal;
			this.Defense = DefenseStates.None;
			this.SpawnType = SpawnTypes.FlameSpawn;
			this.DieType = DyingTypes.BodyBurn;
			this.LungeState = LungeStates.None;
			if (this.Name.Contains("boss"))
			{
				this.alwaysUpdatable = true;
			}
			else
			{
				this.alwaysUpdatable = false;
			}
			this.SetEthereal(EtherealState.Normal);
			this.DownTime = (this.StatusTime = 0f);
			this.MaxDownTime = 5f;
			this.InitScript();
			this.QuestGiver = Game1.questManager.CheckQuestGivers(this);
			this.CollectEquipID = -1;
			while (this.AnimName == "init")
			{
				if (this.FlyType > FlyingType.None)
				{
					this.flyingSpeed = Rand.GetRandomFloat(0f, 6.28f);
					this.SetAnim("fly", Rand.GetRandomInt(0, 5), 0);
					continue;
				}
				try
				{
					if (ground)
					{
						this.GroundCharacter();
						this.SetAnim("idle00", Rand.GetRandomInt(0, 10), 0);
						continue;
					}
					this.Trajectory.Y = Rand.GetRandomFloat(-300f, -1f);
					this.SetAnim("jump", 0, 0);
					if (this.AnimName != "jump")
					{
						this.SetAnim("idle00", 0, 0);
					}
				}
				catch (Exception)
				{
				}
			}
			this.prevFrame = new Frame(this.Definition.maxParts);
			this.returnFrame = new Frame(this.Definition.maxParts);
			this.AnimFrame = Math.Min(this.AnimFrame, this.Definition.Animations[this.Anim].KeyFrames.Length - 2);
			this.prevFrame = this.Definition.Frames[this.Definition.Animations[this.Anim].KeyFrames[this.AnimFrame].FrameRef];
			this.nextKeyFrame = this.Definition.Animations[this.Anim].KeyFrames[this.AnimFrame + 1];
			this.curAnimFrame = Rand.GetRandomInt(0, this.maxAnimFrames);
			this.RandomSkin = (byte)Rand.GetRandomInt(0, this.RandomTextures);
			this.randomWeapon = (byte)Rand.GetRandomInt(0, this.RandomWeapons);
			this.InitTextures(null);
			SpawnTypes spawnType = this.SpawnType;
			if (spawnType == SpawnTypes.FlameSpawn && Game1.map.transInFrame <= 0f && this.State == CharState.Air && this.CheckRenderable() && this.NPC == NPCType.None && this.ID != 0)
			{
				Game1.pManager.AddShockRing(this.Location, 0.4f, 5);
				for (int k = 0; k < 20; k++)
				{
					Game1.pManager.AddDeathFlame(this.Location + Rand.GetRandomVector2((float)(-this.DefaultWidth) / 1.5f, (float)this.DefaultWidth / 1.5f, -this.DefaultHeight / 2, 60f), Rand.GetRandomVector2(-40f, 40f, -150f, -20f), 0.3f, Rand.GetRandomFloat(0.2f, 0.75f), Rand.GetRandomFloat(0.5f, 0.8f), Rand.GetRandomFloat(0.6f, 1f), 0, audio: true, 5);
				}
			}
			this.PrepParts(forceUpdate: true);
			this.Exists = CharExists.Exists;
			return this.ID;
		}

		public void InitTextures(Frame frame)
		{
			this.renderable = false;
			this.textureRange[0] = this.Definition.maxSprites[0] + this.Definition.maxSprites[1];
			this.textureRange[1] = this.Definition.maxSprites[0] + this.Definition.maxSprites[1] + this.Definition.maxSprites[2];
			this.textureRange[2] = this.Definition.maxSprites[0] + this.Definition.maxSprites[1] + this.Definition.maxSprites[2] + this.Definition.maxSprites[3];
			this.textureRange[3] = this.Definition.maxSprites[0] + this.Definition.maxSprites[1] + this.Definition.maxSprites[2] + this.Definition.maxSprites[3] + this.Definition.maxSprites[4];
			if (frame == null)
			{
				frame = this.Definition.Frames[0];
			}
			if (this.Definition.charType == CharacterType.Dust && Game1.gameMode == Game1.GameModes.Game)
			{
				Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadDustTextures)));
				return;
			}
			for (int i = 0; i < frame.Parts.Length; i++)
			{
				Part part = frame.Parts[i];
				int index = part.Index;
				if (index <= -1)
				{
					continue;
				}
				if (index < this.Definition.maxSprites[0])
				{
					this.textureID = 1;
					this.textureIndex = this.Definition.Sprites_01_Index + this.RandomSkin;
					if (Character.sprites_01_Tex[this.textureIndex] == null || Character.sprites_01_Tex[this.textureIndex].IsDisposed)
					{
						this.SetTextures();
					}
				}
				else if (index < this.textureRange[0])
				{
					this.textureID = 2;
					this.textureIndex = this.Definition.Sprites_02_Index + this.randomWeapon;
					if (Character.sprites_02_Tex[this.textureIndex] == null || Character.sprites_02_Tex[this.textureIndex].IsDisposed)
					{
						this.SetTextures();
					}
				}
				else if (index < this.textureRange[1])
				{
					this.textureID = 3;
					this.textureIndex = this.Definition.Sprites_03_Index;
					if (Character.sprites_03_Tex[this.textureIndex] == null || Character.sprites_03_Tex[this.textureIndex].IsDisposed)
					{
						this.SetTextures();
					}
				}
				else if (index < this.textureRange[2])
				{
					this.textureID = 4;
					this.textureIndex = this.Definition.Sprites_04_Index;
					if (Character.sprites_04_Tex[this.textureIndex] == null || Character.sprites_04_Tex[this.textureIndex].IsDisposed)
					{
						this.SetTextures();
					}
				}
				else
				{
					this.textureID = 5;
					this.textureIndex = this.Definition.Sprites_05_Index;
					if (Character.sprites_05_Tex[this.textureIndex] == null || Character.sprites_05_Tex[this.textureIndex].IsDisposed)
					{
						this.SetTextures();
					}
				}
			}
		}

		private void SetTextures()
		{
			if (Game1.map.transInFrame >= 0f)
			{
				Game1.map.transInFrame = Math.Min(Game1.map.transInFrame + 1f, 6f);
			}
			switch (this.textureID)
			{
			case 1:
				Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadTexture1)));
				break;
			case 2:
				Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadTexture2)));
				break;
			case 3:
				Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadTexture3)));
				break;
			case 4:
				Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadTexture4)));
				break;
			case 5:
				Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadTexture5)));
				break;
			}
			this.prevTextureID = this.textureID;
			this.prevTextureIndex = this.textureIndex;
		}

		public void LoadDustTextures()
		{
			Character.LoadDustTextures(Game1.DustContent);
		}

		private void LoadTexture1()
		{
			if (this.Definition.Sprites_01_Index > 0)
			{
				try
				{
					Character.sprites_01_Tex[this.Definition.Sprites_01_Index + this.RandomSkin] = Game1.GetEnemyContent().Load<Texture2D>("gfx/chars/sprites_01_" + (this.Definition.Sprites_01_Index + this.RandomSkin + 1));
				}
				catch (Exception)
				{
				}
			}
		}

		private void LoadTexture2()
		{
			if (this.Definition.Sprites_01_Index > 0)
			{
				try
				{
					Character.sprites_02_Tex[this.Definition.Sprites_02_Index + this.randomWeapon] = Game1.GetEnemyContent().Load<Texture2D>("gfx/chars/sprites_02_" + (this.Definition.Sprites_02_Index + this.randomWeapon + 1));
				}
				catch (Exception)
				{
				}
			}
		}

		private void LoadTexture3()
		{
			if (this.Definition.Sprites_01_Index > 0)
			{
				try
				{
					Character.sprites_03_Tex[this.Definition.Sprites_03_Index] = Game1.GetEnemyContent().Load<Texture2D>("gfx/chars/sprites_03_" + (this.Definition.Sprites_03_Index + 1));
				}
				catch (Exception)
				{
				}
			}
		}

		private void LoadTexture4()
		{
			if (this.Definition.Sprites_01_Index > 0)
			{
				try
				{
					Character.sprites_04_Tex[this.Definition.Sprites_04_Index] = Game1.GetEnemyContent().Load<Texture2D>("gfx/chars/sprites_04_" + (this.Definition.Sprites_04_Index + 1));
				}
				catch (Exception)
				{
				}
			}
		}

		private void LoadTexture5()
		{
			if (this.Definition.Sprites_01_Index > 0)
			{
				try
				{
					Character.sprites_05_Tex[this.Definition.Sprites_05_Index] = Game1.GetEnemyContent().Load<Texture2D>("gfx/chars/sprites_05_" + (this.Definition.Sprites_05_Index + 1));
				}
				catch (Exception)
				{
				}
			}
		}

		public void UnloadTextures()
		{
			this.prevTextureID = (this.textureID = (this.prevTextureIndex = (this.textureIndex = 0)));
			Game1.GetEnemyContent().Unload();
			for (int i = 1; i < Character.sprites_01_Tex.Length; i++)
			{
				Character.sprites_01_Tex[i] = null;
			}
			for (int j = 1; j < Character.sprites_02_Tex.Length; j++)
			{
				Character.sprites_02_Tex[j] = null;
			}
			for (int k = 1; k < Character.sprites_03_Tex.Length; k++)
			{
				Character.sprites_03_Tex[k] = null;
			}
			for (int l = 1; l < Character.sprites_04_Tex.Length; l++)
			{
				Character.sprites_04_Tex[l] = null;
			}
			for (int m = 1; m < Character.sprites_05_Tex.Length; m++)
			{
				Character.sprites_05_Tex[m] = null;
			}
		}

		private bool CheckRenderable()
		{
			if (this.AnimName == "null")
			{
				return false;
			}
			this.renderRect = new Rectangle(-100 - this.DefaultWidth, -320, Game1.screenWidth + 100 + this.DefaultWidth * 2, Game1.screenHeight + 400 + this.Height);
			if (this.renderRect.Contains((int)(this.Location.X * Game1.worldScale - Game1.Scroll.X), (int)(this.Location.Y * Game1.worldScale - Game1.Scroll.Y)))
			{
				if (!this.renderable)
				{
					this.PrepParts(forceUpdate: true);
				}
				return true;
			}
			this.CharRotation = 3.14f;
			return false;
		}

		public bool Updatable(int ceiling)
		{
			if (this.AnimName == "null")
			{
				return false;
			}
			if (this.ID == 0)
			{
				return true;
			}
			if (this.DyingFrame > -1f)
			{
				return true;
			}
			if (this.State == CharState.Air && this.FlyType == FlyingType.None && this.Ai != null && this.Ai.GetTarget() > -1)
			{
				return true;
			}
			if (this.alwaysUpdatable)
			{
				return true;
			}
			if (Game1.map.leftBlock == 0f)
			{
				if (this.FlyType != 0)
				{
					this.updateRect = new Rectangle(-2000, -ceiling, Game1.screenWidth + 4000, Game1.screenHeight + ceiling * 2);
				}
				else
				{
					this.updateRect = new Rectangle(-1400, -ceiling, Game1.screenWidth + 2800, Game1.screenHeight + ceiling * 2);
				}
				if (this.updateRect.Contains((int)(this.Location.X * Game1.worldScale - Game1.Scroll.X), (int)(this.Location.Y * Game1.worldScale - Game1.Scroll.Y)))
				{
					return true;
				}
			}
			else
			{
				if ((this.Ai != null && this.Ai.GetTarget() > -1) || (this.Location.X > Game1.map.leftBlock && this.Location.X < Game1.map.rightBlock))
				{
					return true;
				}
				if (Game1.hud.inBoss && this.Name.Contains("boss"))
				{
					return true;
				}
			}
			return false;
		}

		public bool InHitBounds(Vector2 hitLoc)
		{
			if (hitLoc.X > this.Location.X - (float)(this.DefaultWidth / 2) && hitLoc.X < this.Location.X + (float)(this.DefaultWidth / 2) && hitLoc.Y > this.Location.Y - (float)this.Height && hitLoc.Y < this.Location.Y)
			{
				return true;
			}
			return false;
		}

		public bool InRectBounds(Rectangle bounds)
		{
			if (new Rectangle((int)this.Location.X - this.DefaultWidth / 2, (int)this.Location.Y - this.Height, this.DefaultWidth, this.Height).Intersects(bounds))
			{
				return true;
			}
			return false;
		}

		public bool InCharBounds(Character[] c, int i)
		{
			int num = Math.Max(c[i].DefaultWidth / 2, 120);
			if (new Rectangle((int)(c[i].Location.X - (float)num), (int)(c[i].Location.Y - (float)c[i].Height), num * 2, c[i].Height).Contains((int)this.Location.X, (int)(this.Location.Y - (float)(this.DefaultHeight / 2))))
			{
				return true;
			}
			return false;
		}

		private void InitScript()
		{
			this.Ai = null;
			this.SetAnim("init", 0, 0);
			if (this.AnimName == "init")
			{
				for (int i = 0; i < this.Definition.Animations[this.Anim].KeyFrames.Length; i++)
				{
					if (this.Definition.Animations[this.Anim].KeyFrames[i] != null && this.Definition.Animations[this.Anim].KeyFrames[i].FrameRef > -1)
					{
						this.script.DoScript(this.Anim, i);
					}
				}
				this.maxIdles = 0;
				for (int j = 0; j < this.Definition.Animations.Length; j++)
				{
					if (this.Definition.Animations[j] != null && this.Definition.Animations[j].name.StartsWith("idle"))
					{
						this.maxIdles++;
					}
				}
			}
			else
			{
				this.maxIdles = 1;
			}
		}

		private void CheckTrig(ParticleManager pMan, KeyFrame keyframe)
		{
			float num = this.CharRotation - 3.14f;
			if (this.Face == CharDir.Left)
			{
				num = 0f - num;
			}
			float num2 = this.footOffset;
			if (this.FlyType > FlyingType.None && this.State == CharState.Air)
			{
				num2 = 65f;
			}
			float num3 = 0f;
			Vector2 zero = Vector2.Zero;
			float num4 = 0f;
			int frameRef = keyframe.FrameRef;
			Frame frame = this.Definition.Frames[frameRef];
			for (int i = 0; i < frame.maxRender; i++)
			{
				Part part = frame.Parts[i];
				if (part.Index > 10001)
				{
					if (this.CharRotation != 3.14f)
					{
						float num5 = part.Location.Y - num2;
						Vector2 vector = new Vector2((float)(Math.Cos(num) * (double)part.Location.X - Math.Sin(num) * (double)num5), (float)(Math.Cos(num) * (double)num5 + Math.Sin(num) * (double)part.Location.X));
						zero = vector;
						num3 = part.Rotation + num;
						num4 = part.Scaling.X;
					}
					else
					{
						zero = part.Location - new Vector2(0f, num2);
						num3 = part.Rotation;
						num4 = part.Scaling.X;
					}
					if (this.Face == CharDir.Left)
					{
						num3 = 0f - num3;
						zero.X = 0f - zero.X;
					}
					zero += this.Location;
					this.FireTrig((Trigger)(part.Index - 10000), zero, num3 - 1.57f, num4, pMan);
				}
			}
		}

		private void CheckAnimTrig(float newCharRotation, float origin, float rotation, Vector2 trigLocation, float scaling, Animation partAnim, int i, Part part)
		{
			float num = (float)partAnim.KeyFrames[this.AnimFrame].Duration * (this.curFrame / (float)partAnim.KeyFrames[this.AnimFrame].Duration) / (float)partAnim.KeyFrames[this.AnimFrame].Duration;
			origin = 130f;
			Part part2 = ((partAnim.KeyFrames[this.AnimFrame + 1].FrameRef == -1) ? this.Definition.Frames[partAnim.KeyFrames[0].FrameRef].Parts[i] : this.Definition.Frames[partAnim.KeyFrames[this.AnimFrame + 1].FrameRef].Parts[i]);
			if (this.CharRotation != 3.14f)
			{
				float num2 = part.Location.Y - origin;
				Vector2 vector = new Vector2((float)(Math.Cos(newCharRotation) * (double)part.Location.X - Math.Sin(newCharRotation) * (double)num2), (float)(Math.Cos(newCharRotation) * (double)num2 + Math.Sin(newCharRotation) * (double)part.Location.X));
				float num3 = part2.Location.Y - origin;
				Vector2 vector2 = new Vector2((float)(Math.Cos(newCharRotation) * (double)part2.Location.X - Math.Sin(newCharRotation) * (double)num3), (float)(Math.Cos(newCharRotation) * (double)num3 + Math.Sin(newCharRotation) * (double)part2.Location.X));
				trigLocation = (vector2 - vector) * num + vector;
				rotation = (part2.Rotation - part.Rotation) * num + part.Rotation + newCharRotation;
				scaling = (part2.Scaling.X - part.Scaling.X) * num + part.Scaling.X;
			}
			else
			{
				trigLocation = (part2.Location - part.Location) * num + part.Location - new Vector2(0f, origin);
				rotation = (part2.Rotation - part.Rotation) * num + part.Rotation;
				scaling = (part2.Scaling.X - part.Scaling.X) * num + part.Scaling.X;
			}
			if (this.Face == CharDir.Left)
			{
				rotation = 0f - rotation;
				trigLocation.X = 0f - trigLocation.X;
			}
			trigLocation += this.Location;
			this.FireTrig((Trigger)(part.Index - 10000), trigLocation, rotation - 1.57f, scaling, Game1.pManager);
		}

		public void CheckXCol(Map map, Vector2 pLoc, float offset)
		{
			int num = 0;
			float num2 = this.Trajectory.X + this.BoostTraj + this.CharCol + this.flyingOffset.X;
			bool sideCollision = true;
			if (num2 > 0f)
			{
				num = map.CheckCol(this.Location + new Vector2(32f, 0f - offset));
				if ((num > 0 || Game1.dManager.CheckCol(this, 0, ref sideCollision) > -1f) && sideCollision)
				{
					this.Location.X = (int)pLoc.X;
					this.Trajectory.X = 0f;
					this.CharCol = 0f;
					this.WallInWay = true;
					if (this.State == CharState.Grounded)
					{
						this.Location.Y = pLoc.Y;
					}
					if (this.FlyType > FlyingType.None)
					{
						this.flyingSpeed = 1.57f;
					}
					this.HangWall(CharDir.Right);
				}
			}
			if (num2 < 0f)
			{
				num = map.CheckCol(this.Location + new Vector2(-32f, 0f - offset));
				if ((num > 0 || Game1.dManager.CheckCol(this, 0, ref sideCollision) > -1f) && sideCollision)
				{
					this.Location.X = (int)pLoc.X;
					this.Trajectory.X = 0f;
					this.CharCol = 0f;
					this.WallInWay = true;
					if (this.State == CharState.Grounded)
					{
						this.Location.Y = pLoc.Y;
					}
					if (this.FlyType > FlyingType.None)
					{
						this.flyingSpeed = 3.14f;
					}
					this.HangWall(CharDir.Left);
				}
			}
			if (num > 2)
			{
				HitManager.CheckWallHazard(Game1.character, this.ID, Game1.pManager, 100, (ColType)num);
			}
		}

		private void HazardEffect(int checkCol)
		{
			if (checkCol == 4)
			{
				Game1.pManager.AddEmitLava(new Vector2(Rand.GetRandomFloat(this.Location.X - (float)this.DefaultWidth / 1.5f, this.Location.X + (float)this.DefaultWidth / 1.5f), this.Location.Y + 50f), Vector2.Zero, 2f, colliding: false, 3, 6);
				Game1.pManager.AddEmitLava(new Vector2(Rand.GetRandomFloat(this.Location.X - (float)this.DefaultWidth / 1.5f, this.Location.X + (float)this.DefaultWidth / 1.5f), this.Location.Y - 100f), Rand.GetRandomVector2(-300f, 300f, -600f, -200f), 0.5f, colliding: false, 1, 6);
			}
		}

		public bool CheckEvade()
		{
			if (Game1.stats.upgrade[10] > 0)
			{
				if (this.ID > 0 || Game1.stats.curCharge > (float)Game1.stats.projectileCost)
				{
					if (Game1.map.CheckCol(this.Location - new Vector2(0f, 96f)) == 0 && ((!this.AnimName.StartsWith("evade") && !this.AnimName.StartsWith("hurt") && !this.AnimName.StartsWith("crawl")) || this.CanCancel) && (this.AnimName != "attackairdown" || this.AnimFrame > 6))
					{
						this.KeyLeft = (this.KeyRight = false);
						this.KeyEvade = true;
						return true;
					}
				}
				else
				{
					Game1.hud.InitFidgetPrompt(FidgetPrompt.ChargeEmpty);
					Sound.PlayCue("fidget_fail");
				}
			}
			return false;
		}

		private void SetEthereal(EtherealState type)
		{
			if (Game1.events.anyEvent && Game1.events.safetyOn)
			{
				this.Ethereal = EtherealState.Ethereal;
			}
			else
			{
				this.Ethereal = type;
			}
		}

		public void GroundCharacter()
		{
			this.Trajectory.Y = 1f;
			this.Location.Y = (int)this.Location.Y;
			int num = 0;
			while (this.State == CharState.Air && num < 1000)
			{
				num++;
				this.PLoc = this.Location;
				this.Location.Y += 16f;
				this.MapCollision(Game1.map, Game1.character);
				if (this.Location.Y > Game1.map.bottomEdge + 128f)
				{
					return;
				}
			}
			this.CheckTerrain(Game1.map);
		}

		public void Impact(Character c, Vector2 loc)
		{
			float num = Math.Abs(((float)(Game1.screenWidth / 2) - (loc.X - Game1.Scroll.X)) / 4f) / 100f;
			if (num < 1f)
			{
				num = 1f;
			}
			Game1.map.MapSegFrameSpeed = 4f;
			VibrationManager.SetScreenShake(0.6f / num);
		}

		private void PlaySound(string sound)
		{
			if (Game1.events.screenFade.A > 0 || Game1.map.GetTransVal() > 0f)
			{
				return;
			}
			CharacterType charType = this.Definition.charType;
			float num = (Game1.character[0].Location - this.Location).Length();
			switch (sound)
			{
			case "jump":
			{
				CharacterType characterType = charType;
				if (characterType != CharacterType.Bunny && characterType != CharacterType.BunnySnow)
				{
					Sound.PlayCue("jump", this.Location, num);
				}
				break;
			}
			case "land":
				switch (charType)
				{
				case CharacterType.Giant:
					Sound.PlayCue("giantland", this.Location, num / 2f);
					break;
				case CharacterType.Bunny:
				case CharacterType.Deer:
				case CharacterType.BunnySnow:
					break;
				default:
					Sound.PlayCue("land", this.Location, num);
					break;
				}
				break;
			case "land_hurt":
				Sound.PlayCue("land_hurt", this.Location, num);
				break;
			case "die":
				switch (charType)
				{
				case CharacterType.Imp:
					Sound.PlayCue("impdie", this.Location, num / 4f);
					break;
				case CharacterType.LightBeast:
					Sound.PlayCue("lightdie", this.Location, num / 4f);
					break;
				case CharacterType.Avee:
					Sound.PlayCue("avee_die", this.Location, num / 4f);
					break;
				case CharacterType.Fuse:
					Sound.PlayCue("fuse_die", this.Location, num / 4f);
					break;
				case CharacterType.Blomb:
					Sound.PlayCue("blomb_startle", this.Location, num / 4f);
					break;
				case CharacterType.SquirtBug:
					Sound.PlayCue("squirtbug_die", this.Location, num / 4f);
					break;
				case CharacterType.RockHound:
					Sound.PlayCue("rockhound_die", this.Location, num / 4f);
					break;
				case CharacterType.Psylph:
					Sound.PlayCue("psylph_die", this.Location, num / 4f);
					break;
				case CharacterType.Assassin:
				case CharacterType.Soldier:
					Sound.PlayCue("soldier_die", this.Location, num / 4f);
					break;
				}
				break;
			case "slash":
				break;
			}
		}

		public void InterpolateOff(int t)
		{
			if (t == 0)
			{
				this.Definition.Animations[this.Anim].interpolate = false;
			}
			else
			{
				this.Definition.Animations[this.Anim].interpolate = true;
			}
		}

		private Color GetColor()
		{
			Color result = ((this.ID == 0 && (float)this.HP < (float)this.MaxHP * 0.2f && this.HP != 0 && Game1.stats.overHeating == 0f) ? new Color(1f, Game1.hud.lifeBarBeat + 0.7f, Game1.hud.lifeBarBeat + 0.7f, 1f) : ((!(this.lungeTint > 255f)) ? this.defaultColor : new Color(new Vector4((float)(int)this.defaultColor.R / this.lungeTint, (float)(int)this.defaultColor.G / this.lungeTint, (float)(int)this.defaultColor.B / this.lungeTint, (float)(int)this.defaultColor.A / 255f))));
			if (this.DyingFrame != -1f && this.DieType == DyingTypes.BodyBurn)
			{
				result = this.defaultColor * (1f - this.DyingFrame);
			}
			if (Game1.longSkipFrame < 3)
			{
				StatusEffects statusEffect = this.StatusEffect;
				if (statusEffect == StatusEffects.Poison)
				{
					result.R = 0;
					result.B = 128;
				}
			}
			return result;
		}

		private void SetRandomIdle(ref string newAnim)
		{
			while (this.Exists == CharExists.Exists)
			{
				int randomInt = Rand.GetRandomInt(-1, this.maxIdles);
				if (randomInt < 1)
				{
					break;
				}
				newAnim = "idle" + $"{randomInt:D2}";
				for (int i = 0; i < this.Definition.Animations.Length; i++)
				{
					if (this.Definition.Animations[i] != null && this.Definition.Animations[i].name == newAnim)
					{
						return;
					}
				}
			}
		}

		private Frame GetFrameSnapShot(Frame frame)
		{
			if (this.Definition.Frames[this.nextKeyFrame.FrameRef] == null)
			{
				return frame;
			}
			float num = this.prevDuration * (this.curFrame / this.prevDuration) / this.prevDuration;
			for (int i = 0; i < frame.Parts.Length; i++)
			{
				Part part = this.returnFrame.Parts[i];
				part.Index = frame.Parts[i].Index;
				part.Location = frame.Parts[i].Location;
				part.Rotation = frame.Parts[i].Rotation;
				part.Scaling = frame.Parts[i].Scaling;
				part.Flip = frame.Parts[i].Flip;
			}
			int num2 = Math.Min(this.returnFrame.Parts.Length, this.Definition.Frames[this.nextKeyFrame.FrameRef].Parts.Length);
			for (int j = 0; j < num2; j++)
			{
				Part part2 = this.returnFrame.Parts[j];
				if (part2.Index > -1 && part2.Index < 10000)
				{
					Part part3 = this.Definition.Frames[this.nextKeyFrame.FrameRef].Parts[j];
					if (num > 0f)
					{
						part2.Location += (part3.Location - part2.Location) * num;
						part2.Rotation += (part3.Rotation - part2.Rotation) * num;
						part2.Scaling += (part3.Scaling - part2.Scaling) * num;
					}
					else
					{
						part2.Location = part3.Location;
						part2.Rotation = part3.Rotation;
						part2.Scaling = part3.Scaling;
					}
				}
			}
			this.returnFrame.maxRender = frame.maxRender;
			return this.returnFrame;
		}

		private void UpdateAnimation(Character[] c)
		{
			if (this.maxAnimFrames > 0)
			{
				this.animFrameTime += Game1.FrameTime * 24f;
				if (this.animFrameTime > 1f)
				{
					this.curAnimFrame++;
					if (this.curAnimFrame > this.maxAnimFrames - 1)
					{
						this.curAnimFrame = 0;
					}
					this.animFrameTime = 0f;
					this.PrepParts(forceUpdate: true);
				}
			}
			if (this.ID == 0)
			{
				this.SpinBlade(c);
			}
			else if (this.LungeState > LungeStates.None && this.LungeState < LungeStates.LungeUpForAttack && this.Trajectory.Y < 800f)
			{
				this.Ethereal = EtherealState.Ethereal;
				if (this.lungeTint < 512f)
				{
					this.lungeTint += Game1.FrameTime * 2000f;
				}
			}
			else if (this.lungeTint > 255f)
			{
				this.SetEthereal(EtherealState.Normal);
				this.lungeTint -= Game1.FrameTime * 4000f;
				if (this.lungeTint < 255f)
				{
					this.lungeTint = 255f;
				}
			}
			Animation animation = this.Definition.Animations[this.Anim];
			if (this.AnimFrame >= animation.KeyFrames.Length)
			{
				return;
			}
			KeyFrame keyFrame = animation.KeyFrames[this.AnimFrame];
			this.curFrame += Game1.FrameTime * 24f;
			if (keyFrame == null || !(this.DyingFrame < 1f) || !(this.curFrame > this.prevDuration))
			{
				return;
			}
			if (this.renderable || this.alwaysUpdatable)
			{
				this.CheckTrig(Game1.pManager, keyFrame);
			}
			this.curFrame -= this.prevDuration;
			while (this.curFrame > 1f)
			{
				this.curFrame -= 1f;
			}
			this.AnimFrame++;
			if (this.AnimFrame >= animation.KeyFrames.Length || animation.KeyFrames[this.AnimFrame] == null)
			{
				this.AnimFrame = 0;
			}
			this.prevFrame = this.Definition.Frames[animation.KeyFrames[this.AnimFrame].FrameRef];
			this.prevDuration = animation.KeyFrames[this.AnimFrame].Duration;
			if (animation.interpolate)
			{
				int num = this.AnimFrame + 1;
				if (num >= animation.KeyFrames.Length || animation.KeyFrames[num] == null)
				{
					num = 0;
				}
				this.nextKeyFrame = animation.KeyFrames[num];
			}
			else
			{
				this.nextKeyFrame = animation.KeyFrames[this.AnimFrame];
			}
			this.script.DoScript(this.Anim, this.AnimFrame);
			this.PrepParts(forceUpdate: false);
		}

		private void PrepParts(bool forceUpdate)
		{
			while (Game1.drawState == Game1.DrawState.Character)
			{
			}
			for (int i = 0; i < this.prevFrame.maxRender; i++)
			{
				Part part = this.prevFrame.Parts[i];
				if (part.Index > -1 && part.Index < 10000)
				{
					if ((forceUpdate || part.Index != this.partsSnap[i].Index) && this.partsSnap[i].GetSource(part.Index, this.Definition, this.textureRange, this.RandomSkin, this.randomWeapon, this.maxAnimFrames, this.curAnimFrame, Character.sprites_01_Tex, Character.sprites_02_Tex, Character.sprites_03_Tex, Character.sprites_04_Tex, Character.sprites_05_Tex))
					{
						this.refracting = true;
					}
				}
				else
				{
					this.partsSnap[i].Index = -1;
				}
			}
		}

		public void SetFrame(int newFrame)
		{
			int anim = this.Anim;
			Animation animation = this.Definition.Animations[anim];
			this.AnimFrame = (int)MathHelper.Clamp(newFrame, 0f, animation.KeyFrames.Length - 1);
			if (this.AnimFrame >= animation.KeyFrames.Length)
			{
				this.AnimFrame = 0;
			}
			KeyFrame keyFrame = animation.KeyFrames[this.AnimFrame];
			if (keyFrame == null)
			{
				this.AnimFrame = 0;
			}
			int num = this.AnimFrame + 1;
			if (num >= animation.KeyFrames.Length || animation.KeyFrames[num] == null)
			{
				num = 0;
			}
			this.nextKeyFrame = animation.KeyFrames[num];
			if (this.Definition.Animations[anim].KeyFrames[this.AnimFrame].Scripts != null)
			{
				this.script.DoScript(anim, this.AnimFrame);
			}
			this.PrepParts(forceUpdate: false);
		}

		public void SetAnim(string newAnim, int startFrame, int newDuration)
		{
			if (newAnim == "null")
			{
				this.AnimName = "null";
				this.Ethereal = EtherealState.Ethereal;
				this.Trajectory = Vector2.Zero;
				return;
			}
			if (newAnim == "idle00" && this.maxIdles > 1)
			{
				this.SetRandomIdle(ref newAnim);
			}
			else if (this.FlyType > FlyingType.None)
			{
				if (newAnim.StartsWith("idle"))
				{
					this.SetAnim("fly", 0, 2);
					return;
				}
			}
			else if (this.DownTime > 0f && newAnim.StartsWith("idle"))
			{
				this.SetAnim("down", 0, 2);
				return;
			}
			if (this.GrabbedBy > -1)
			{
				int grabbedBy = this.GrabbedBy;
				if (!Game1.character[grabbedBy].Holding)
				{
					this.GrabbedBy = -1;
					Game1.character[grabbedBy].Holding = false;
				}
			}
			if (this.Holding)
			{
				Character[] character = Game1.character;
				for (int i = 0; i < character.Length; i++)
				{
					if (character[i].GrabbedBy == this.ID && character[i].Exists == CharExists.Dead)
					{
						this.Holding = false;
					}
				}
			}
			if (this.Ai != null)
			{
				newAnim = this.Ai.CheckAnimName(newAnim);
			}
			if (newAnim == null || !(this.AnimName != newAnim))
			{
				return;
			}
			for (int j = 0; j < this.Definition.Animations.Length; j++)
			{
				if (this.Definition.Animations[j] == null || !(this.Definition.Animations[j].name == newAnim))
				{
					continue;
				}
				startFrame = Math.Min(startFrame, this.Definition.Animations[j].KeyFrames.Length - 1);
				Animation animation = this.Definition.Animations[this.Anim];
				int anim = this.Anim;
				int num;
				if (newDuration == -1)
				{
					this.prevDuration = (newDuration = this.Definition.Animations[this.Anim].KeyFrames[(int)MathHelper.Clamp(this.AnimFrame - 1, 0f, this.AnimFrame)].Duration);
					num = startFrame;
					if (this.Definition.Animations[j].KeyFrames[num] == null)
					{
						num = 0;
					}
					this.nextKeyFrame = this.Definition.Animations[j].KeyFrames[num];
				}
				if (newDuration == 0 || this.Definition.Animations[j].KeyFrames[startFrame].Duration == 0)
				{
					if (this.AnimFrame < this.Definition.Animations[j].KeyFrames.Length && this.Definition.Animations[j].KeyFrames[this.AnimFrame] != null && this.Definition.Animations[j].KeyFrames[this.AnimFrame].FrameRef >= 0)
					{
						this.prevFrame = this.Definition.Frames[this.Definition.Animations[j].KeyFrames[0].FrameRef];
						if (1 < this.Definition.Animations[j].KeyFrames.Length)
						{
							this.nextKeyFrame = this.Definition.Animations[j].KeyFrames[1];
						}
					}
					this.prevDuration = this.Definition.Animations[j].KeyFrames[0].Duration;
				}
				else if (animation.interpolate)
				{
					if (this.Definition.Animations[this.Anim].KeyFrames[this.AnimFrame] != null && this.Definition.Animations[this.Anim].KeyFrames[this.AnimFrame].FrameRef >= 0)
					{
						this.prevFrame = this.GetFrameSnapShot(this.prevFrame);
					}
					this.prevDuration = newDuration;
				}
				this.Anim = j;
				if (this.Definition.Animations[this.Anim].KeyFrames[startFrame] == null)
				{
					startFrame = 0;
				}
				this.AnimFrame = startFrame;
				animation = this.Definition.Animations[this.Anim];
				KeyFrame keyFrame = animation.KeyFrames[this.AnimFrame];
				num = this.AnimFrame + 1;
				if (num >= animation.KeyFrames.Length || animation.KeyFrames[num] == null)
				{
					num = 0;
				}
				this.nextKeyFrame = animation.KeyFrames[num];
				if ((!animation.interpolate || !this.Definition.Animations[anim].interpolate) && this.Definition.Animations[this.Anim].KeyFrames[this.AnimFrame] != null)
				{
					this.prevFrame = this.Definition.Frames[animation.KeyFrames[this.AnimFrame].FrameRef];
					this.prevDuration = animation.KeyFrames[this.AnimFrame].Duration;
				}
				this.curFrame = 0f;
				this.AnimName = newAnim;
				bool forceUpdate = this.refracting;
				this.refracting = false;
				this.PrepParts(forceUpdate);
				if (this.AnimName.StartsWith("fly"))
				{
					this.SetEthereal(EtherealState.EtherealVulnerable);
				}
				else
				{
					this.SetEthereal(EtherealState.Normal);
				}
				this.Floating = false;
				this.Hanging = false;
				this.Defense = DefenseStates.None;
				this.CanCancel = false;
				if (this.ID == 0)
				{
					Game1.stats.inFront = true;
					Game1.stats.isSpinning = false;
				}
				else
				{
					this.InterpolateOff(1);
				}
				this.PressedKey = PressedKeys.None;
				for (int k = 0; k < this.GotoGoal.Length; k++)
				{
					this.GotoGoal[k] = -1;
				}
				this.QueuedKey = QueuedKey.None;
				for (int l = 0; l < this.GotoGoal.Length; l++)
				{
					this.QueueGoal[l] = -1;
				}
				if (keyFrame != null)
				{
					if (keyFrame.FrameRef < 0)
					{
						this.AnimFrame = 0;
					}
					this.script.DoScript(this.Anim, this.AnimFrame);
				}
				break;
			}
		}

		private void Land(ParticleManager pMan)
		{
			if (this.GrabbedBy > -1)
			{
				return;
			}
			this.State = CharState.Grounded;
			this.IsFalling = false;
			this.jumpTime = 0f;
			this.Floating = false;
			this.CheckTerrain(Game1.map);
			if (this.Trajectory.Y > 3600f)
			{
				VibrationManager.SetScreenShake(0.5f);
			}
			this.Trajectory.Y = 0f;
			this.CharRotation = (this.CharRotationDest = 3.14f);
			if (this.KeyLeft || this.KeyRight)
			{
				this.Friction = 0f;
			}
			else
			{
				this.Friction = 6000f;
			}
			if (this.AnimName == "die" || this.AnimName == "land" || this.AnimName == "attack02")
			{
				return;
			}
			float num = 0f;
			if (this.ID == 0)
			{
				Game1.stats.doubleJump = 0;
			}
			if (this.DyingFrame > -1f || (this.ID == 0 && Game1.stats.playerLifeState > 0))
			{
				num = 0.5f;
			}
			else
			{
				switch (this.AnimName)
				{
				case "fly":
					this.State = CharState.Air;
					this.Trajectory.Y = (0f - this.Trajectory.Y) / 2f;
					break;
				case "flyattack":
					this.State = CharState.Air;
					this.Trajectory.Y = (0f - this.Trajectory.Y) / 2f;
					this.Trajectory.Y = 0f;
					break;
				case "hurtup":
					num = 0.25f;
					this.Friction = 6000f;
					break;
				case "attackairdown":
					this.SetAnim("land", 0, 0);
					this.PlaySound("land");
					Game1.map.MapSegFrameSpeed = 4f;
					VibrationManager.SetScreenShake(VibrationManager.ScreenShake.value + 0.4f);
					pMan.MakeGroundDust(this.Location, Vector2.Zero, this.ID, 0.5f, 1f, 0, 5, this.terrainType);
					break;
				default:
					if (this.Ai != null && this.Ai.DodgeCounter(Game1.character))
					{
						this.Friction = 6000f;
						if (this.Exists == CharExists.Exists)
						{
							this.PlaySound("land");
						}
					}
					else
					{
						if (this.Holding)
						{
							this.SetJump(1400f, jumped: true);
							this.Slide(-800f);
							this.SetAnim("attackairslam", 0, 0);
							this.PlaySound("land_hurt");
							Game1.SlowTime = 0.05f;
							Game1.map.MapSegFrameSpeed = 6f;
							break;
						}
						if (this.AnimName == "evadeair")
						{
							this.SetAnim("evade", 8, 0);
							this.PlaySound("land");
							this.Ethereal = EtherealState.Ethereal;
							this.runTimer = 0f;
							this.Trajectory.X = 0f;
							this.Friction = 6000f;
						}
						else
						{
							if (this.KeyDown)
							{
								this.SetAnim("crouch", 0, 0);
							}
							else if (this.ID == 0 && (this.KeyLeft || this.KeyRight))
							{
								this.SetAnim("run", 0, 2);
							}
							else
							{
								this.SetAnim("land", 0, 0);
								this.Friction = 6000f;
							}
							if (this.Exists == CharExists.Exists)
							{
								this.PlaySound("land");
							}
						}
					}
					if (this.FlyType > FlyingType.None)
					{
						this.SetJump(1000f, jumped: false);
					}
					else if (this.DefaultWidth > 100 && this.renderable && Game1.events.screenFade.A < 200)
					{
						pMan.MakeGroundDust(this.Location, Vector2.Zero, this.ID, 0.5f, 0.5f, 0, 5, this.terrainType);
					}
					break;
				}
			}
			if (num > 0f)
			{
				if (this.Definition.charType == CharacterType.Dust)
				{
					this.SetAnim("land", 0, 0);
					this.SetAnim("hurtland", 0, 0);
				}
				else
				{
					this.SetAnim("land", 0, 0);
					this.SetAnim("hurtland", 0, 0);
					this.SetAnim("hurtland0" + Rand.GetRandomInt(0, 2), 0, 0);
				}
				this.PlaySound("land_hurt");
				pMan.MakeGroundDust(this.Location, Vector2.Zero, this.ID, 0.6f, 1f, 0, 5, this.terrainType);
				Game1.map.MapSegFrameSpeed = 4f;
				VibrationManager.SetScreenShake(num);
			}
			this.LungeState = LungeStates.None;
		}

		public void Slide(float distance)
		{
			if (this.LiftType < CanLiftType.Immovable)
			{
				float num = (float)this.Face * 2f * distance - distance;
				if (num > 0f && this.Trajectory.X > 0f)
				{
					this.Trajectory.X = Math.Max(num, this.Trajectory.X);
				}
				else if (num < 0f && this.Trajectory.X < 0f)
				{
					this.Trajectory.X = Math.Min(num, this.Trajectory.X);
				}
				else
				{
					this.Trajectory.X = num;
				}
			}
		}

		public void SetJump(float jump, bool jumped)
		{
			if (Game1.map.CheckCol(this.Location - new Vector2(0f, 96f)) > 0)
			{
				return;
			}
			if (this.ID == 0)
			{
				Sound.StopPersistCue("vine_slide");
				if (this.State == CharState.Grounded)
				{
					Game1.stats.lastSafeLoc = this.PLoc;
				}
			}
			if (jumped)
			{
				this.SetAnim("jump", 0, 2);
				if (this.State == CharState.Air)
				{
					this.SetAnim("jumpdouble", 0, 0);
				}
				this.PlaySound("jump");
				this.Trajectory.Y = 0f - jump;
				if (this.ID == 0)
				{
					Game1.camera.SetJumpPoint(this);
					this.jumpTime = 400f;
				}
				this.Trajectory.Y += Math.Min(Game1.dManager.PlatformTrajectory(this, this.Location).Y * 20f, 0f);
			}
			else
			{
				if (this.LiftType >= CanLiftType.NoLift)
				{
					return;
				}
				if (this.LiftType == CanLiftType.SmallLift)
				{
					this.Trajectory.Y = (0f - jump) / 2f;
				}
				else
				{
					this.Trajectory.Y = 0f - jump;
				}
			}
			this.IsFalling = false;
			this.ledgeAttach = -1;
			this.CanFallThrough = false;
			this.State = CharState.Air;
			this.terrainType = TerrainType.Dirt;
		}

		public bool SetLunge(LungeStates lungeType, float lungeSpeed, float jump)
		{
			if (this.LiftType == CanLiftType.NoLift || this.State == CharState.Air || this.AnimName.Contains("land"))
			{
				return false;
			}
			if ((!this.WallInWay || this.Ai.CanJumpWall(jump)) && !this.AnimName.StartsWith("hurt") && !this.AnimName.StartsWith("attack") && this.DownTime <= 0f)
			{
				this.SetAnim("jump", 0, 2);
				this.SetAnim("dodge", 0, 2);
				if (this.AnimName == "jump" || this.AnimName == "dodge")
				{
					this.Trajectory.Y = 0f - jump;
					this.Trajectory.X = (float)this.Face * 2f * lungeSpeed - lungeSpeed;
					this.State = CharState.Air;
					this.ledgeAttach = -1;
					this.CanFallThrough = false;
					this.Ethereal = EtherealState.Ethereal;
					this.Defense = DefenseStates.Uninterruptable;
					this.LungeState = lungeType;
					return true;
				}
			}
			return false;
		}

		public void FallOff(Character[] c, bool fallThrough)
		{
			if (this.PatrolType == PatrollingType.CannotFalloff)
			{
				this.KeyLeft = (this.KeyRight = false);
				this.Location.X = this.PLoc.X;
				if (this.Ai != null)
				{
					this.Ai.jobType = JobType.Idle;
				}
				return;
			}
			if (fallThrough)
			{
				this.Trajectory.Y = 0f;
				Vector2 vector = Game1.dManager.PlatformTrajectory(this, this.Location);
				if (vector != Vector2.Zero)
				{
					this.Trajectory.Y += Math.Max(vector.Y * 40f, 0f);
					this.Location.Y += Math.Max(vector.Y * 4f, 0f);
					this.Location.Y += 80f;
				}
				else
				{
					this.Location.Y += 35f;
				}
				if (this.KeyLeft || this.KeyRight)
				{
					this.Location.Y += 15f;
				}
				this.PLoc = this.Location;
				this.ledgeAttach = -1;
				this.State = CharState.Air;
				this.IsFalling = true;
				if (this.Definition.charType == CharacterType.Dust)
				{
					Game1.camera.ResetViewPoint(c, fallOff: true);
					Game1.stats.lastSafeLoc = this.PLoc;
					this.SetAnim("jump", 5, 0);
				}
				else
				{
					this.SetAnim("jump", 0, 2);
				}
				this.canJump = true;
				this.CanFallThrough = false;
				return;
			}
			if (this.Team == Team.Enemy && this.Location.Y > c[0].Location.Y)
			{
				this.Trajectory.Y = 0f - this.JumpVelocity;
				this.State = CharState.Air;
				this.jumpTime = 400f;
				this.ledgeAttach = -1;
			}
			else if (this.Team == Team.Enemy && Rand.GetRandomInt(0, 10) < 7)
			{
				this.Trajectory.Y = (0f - this.JumpVelocity) / 2f;
				this.State = CharState.Air;
				this.jumpTime = 400f;
				this.ledgeAttach = -1;
			}
			else
			{
				float x = Game1.dManager.PlatformTrajectory(this, this.PLoc).X;
				if ((this.Trajectory.X > 0f && x > 0f) || (this.Trajectory.X < 0f && x < 0f))
				{
					this.Location.X += x * 4f;
				}
				this.Trajectory.Y = 0f;
				this.State = CharState.Air;
				this.IsFalling = true;
				this.canJump = true;
				this.CanFallThrough = false;
				if (this.ID == 0)
				{
					Game1.stats.lastSafeLoc = this.PLoc;
				}
			}
			if (this.DyingFrame > -1f || (this.AnimName.StartsWith("hurt") && this.Trajectory.X != 0f))
			{
				this.SetAnim("hurtup", 1, 0);
			}
			else if (this.AnimName.StartsWith("evade"))
			{
				this.SetAnim("evadeair", this.AnimFrame, 0);
			}
			else if (this.IsFalling && this.Definition.charType == CharacterType.Dust)
			{
				this.SetAnim("jump", 5, 0);
			}
			else
			{
				this.SetAnim("jump", 0, 2);
			}
			this.terrainType = TerrainType.Dirt;
		}

		private void HangWall(CharDir dir)
		{
			if (this.State != CharState.Air || (!this.KeyRight && !this.KeyLeft && !this.KeyEvade && !this.AnimName.StartsWith("evade")) || this.AnimName.StartsWith("hurt") || Game1.map.CheckCol(this.Location + new Vector2(64 * ((dir != 0) ? 1 : (-1)), -240f)) != 2 || Game1.map.CheckCol(this.Location + new Vector2(64 * ((dir != 0) ? 1 : (-1)), 0f)) != 2 || Game1.map.CheckCol(this.Location + new Vector2(64 * ((dir != 0) ? 1 : (-1)), -120f)) != 2 || (this.ID == 0 && Game1.stats.upgrade[16] <= 0))
			{
				return;
			}
			this.Location.X = (this.PLoc.X = (float)((int)(this.PLoc.X / 64f) * 64) + 32f);
			this.SetAnim("hangwall", 0, 0);
			if (this.AnimName == "hangwall")
			{
				Sound.PlayCue("vine_grab", this.Location, (this.Location - Game1.character[0].Location).Length());
				this.Face = dir;
				this.Trajectory.Y = 0f;
				this.jumpTime = 400f;
				this.IsFalling = false;
				if (this.ID == 0)
				{
					Game1.stats.doubleJump = 0;
				}
			}
		}

		public bool InitDustSeek()
		{
			if (this.Definition.charType == CharacterType.Dust && this.State == CharState.Air)
			{
				if (this.KeyDown)
				{
					this.SetAnim("attackairdown", 0, 2);
				}
				else if (Game1.stats.upgrade[12] > 0)
				{
					this.Seek(force: true);
					return true;
				}
			}
			return false;
		}

		public void Seek(bool force)
		{
			Character[] character = Game1.character;
			float num = 0f;
			float num2 = 0f;
			int num3;
			if (this.Ai != null)
			{
				num3 = this.Ai.GetTarget();
				if (num3 > -1 && ((this.Face == CharDir.Right && character[num3].Location.X < this.Location.X) || (this.Face == CharDir.Left && character[num3].Location.X > this.Location.X)))
				{
					num3 = -1;
				}
			}
			else
			{
				int num4 = -1;
				num3 = -1;
				for (int i = 0; i < character.Length; i++)
				{
					if (i == this.ID || character[i].Exists != CharExists.Exists || character[i].NPC != 0 || character[i].Team == this.Team || !this.SeekBounds(character, i) || ((this.Face != CharDir.Right || !(character[i].Location.X > this.Location.X)) && (this.Face != 0 || !(character[i].Location.X < this.Location.X))))
					{
						continue;
					}
					float num5 = (this.Location - character[i].Location).Length();
					if (num3 == -1 || num5 < num)
					{
						num = num5;
						num3 = i;
						if (character[num3].State == CharState.Air)
						{
							num4 = i;
							num2 = num5;
						}
					}
				}
				if (num4 != -1)
				{
					num3 = num4;
					num = num2;
				}
			}
			if (num3 > -1)
			{
				if (!force && this.Location.X > character[num3].Location.X - (float)character[num3].DefaultWidth && this.Location.X < character[num3].Location.X + (float)character[num3].DefaultWidth && this.Location.Y < character[num3].Location.Y + (float)character[num3].DefaultHeight)
				{
					return;
				}
				this.Trajectory.X = MathHelper.Clamp((character[num3].Location.X - this.Location.X) * 4f, -1600f, 1600f);
				if (this.FlyType > FlyingType.None)
				{
					this.Trajectory.Y = MathHelper.Clamp((character[num3].Location.Y - (float)character[num3].Height - this.Location.Y) * 4f, -1000f, 1000f);
				}
				else
				{
					this.Trajectory.Y += MathHelper.Clamp((character[num3].Location.Y - (float)(character[num3].Height / 4) - this.Location.Y) * 4f, -1000f, 300f);
					if (this.Location.Y < character[num3].Location.Y)
					{
						this.Floating = true;
					}
				}
				if (force)
				{
					if (this.AnimName != "airspin")
					{
						this.Trajectory.Y = MathHelper.Clamp(this.Trajectory.Y, -3000f, -200f);
						this.SetAnim("airspin", 0, 0);
					}
					else
					{
						this.Trajectory.Y = MathHelper.Clamp(this.Trajectory.Y, -3000f, 0f);
					}
					this.Trajectory.Y *= 1.4f;
					this.Slide(1400f);
					this.Ethereal = EtherealState.EtherealVulnerable;
					this.Floating = true;
					this.Friction = 0f;
					if (!Game1.stats.isSpinning)
					{
						Game1.pManager.MakeGroundDust(this.Location + new Vector2(0f, -50f), Vector2.Zero, this.ID, 1f, 0.8f, 1, 5, this.terrainType);
						Game1.stats.isSpinning = true;
					}
					Game1.pManager.AddShockRing(this.Location, 0.3f, 5);
				}
			}
			else
			{
				if (!force || this.WallInWay)
				{
					return;
				}
				if (this.AnimName != "airspin")
				{
					if (this.KeyLeft)
					{
						this.Face = CharDir.Left;
					}
					else if (this.KeyRight)
					{
						this.Face = CharDir.Right;
					}
					this.SetAnim("airspin", 0, 0);
				}
				this.Slide(1000f);
				this.Ethereal = EtherealState.EtherealVulnerable;
				this.Floating = true;
				this.Friction = 0f;
				if (!Game1.stats.isSpinning)
				{
					Game1.pManager.MakeGroundDust(this.Location + new Vector2(0f, -50f), new Vector2(0f - ((float)this.Face * 2f - 1f), 0f), this.ID, 1f, 0.8f, 1, 5, this.terrainType);
					Game1.stats.isSpinning = true;
				}
				Game1.pManager.AddShockRing(this.Location, 0.3f, 5);
			}
		}

		private bool SeekBounds(Character[] c, int i)
		{
			if (this.FlyType > FlyingType.None && this.Location.X > c[i].Location.X - 1800f && this.Location.X < c[i].Location.X + 1800f && this.Location.Y > c[i].Location.Y - 200f && this.Location.Y < c[i].Location.Y + 3000f)
			{
				return true;
			}
			if (this.Location.X > c[i].Location.X - 1000f && this.Location.X < c[i].Location.X + 1000f && this.Location.Y > c[i].Location.Y - 600f && ((c[i].State == CharState.Air && this.Location.Y < c[i].Location.Y + 2000f) || (c[i].State == CharState.Grounded && this.Location.Y < c[i].Location.Y + 600f)))
			{
				return true;
			}
			return false;
		}

		public void Evade(ParticleManager pMan)
		{
			bool flag = false;
			if (this.State == CharState.Grounded)
			{
				if (this.AnimName.StartsWith("hurt") || this.AnimName.StartsWith("crawl"))
				{
					if (this.CanCancel)
					{
						Sound.PlayCue("dustevade");
						this.SetAnim("evade", 0, 0);
						pMan.MakeGroundDust(this.Location, new Vector2((float)this.Face * 2f * 200f - 200f, 0f), this.ID, 1f, 1f, 1, 5, this.terrainType);
						VibrationManager.Rumble(Game1.currentGamePad, 0.3f);
						flag = true;
					}
				}
				else if (this.AnimName != "evade")
				{
					this.runTimer = 0f;
					this.Trajectory.X = 0f;
					this.Friction = 6000f;
					Sound.PlayCue("dustevade");
					this.SetAnim("evade", 0, 0);
					pMan.MakeGroundDust(this.Location, new Vector2((float)this.Face * 2f * 200f - 200f, 0f), this.ID, 1f, 1f, 1, 5, this.terrainType);
					VibrationManager.Rumble(Game1.currentGamePad, 0.3f);
					flag = true;
				}
			}
			else if ((!this.AnimName.StartsWith("attack") && (this.AnimName != "evadeair" || this.CanCancel) && this.AnimName != "hurtup" && this.GrabbedBy == -1) || this.AnimName.StartsWith("attackair"))
			{
				this.SetAnim("jump", 0, 0);
				if (!this.AnimName.StartsWith("hang"))
				{
					this.Trajectory.Y = 0f;
					this.SetAnim("evadeair", 0, 0);
				}
				else
				{
					if (this.AnimName == "hang")
					{
						this.Trajectory.Y = -200f;
					}
					this.Slide(-2000f);
					this.Ethereal = EtherealState.Ethereal;
					this.SetAnim("hangjump", 0, 0);
				}
				pMan.MakeGroundDust(this.Location, new Vector2((float)this.Face * 2f * 200f - 200f, 0f), this.ID, 1f, 0.8f, 1, 5, this.terrainType);
				VibrationManager.Rumble(Game1.currentGamePad, 0.3f);
				Sound.PlayCue("dustevade");
				flag = true;
			}
			if ((this.Trajectory.X < 0f && this.Boosting == 3) || (this.Trajectory.X > 0f && this.Boosting == 9))
			{
				if (this.State == CharState.Grounded)
				{
					this.Trajectory.X *= 0.75f;
				}
				else
				{
					this.Trajectory.X *= 0.25f;
				}
			}
			if (flag && this.ID == 0)
			{
				Game1.stats.curCharge = Math.Max(Game1.stats.curCharge - 20f, 0f);
			}
		}

		public void InitParry(ParticleManager pMan, bool parrySuccess)
		{
			if (this.LiftType != 0)
			{
				return;
			}
			if (this.State == CharState.Grounded)
			{
				this.Friction = 6000f;
				if (Game1.map.CheckCol(this.Location - new Vector2(0f, 96f)) > 0)
				{
					this.SetAnim("crouch", 0, 0);
				}
				else if (parrySuccess)
				{
					this.SetAnim("parry", 0, 0);
					this.Slide(-1600f);
				}
				else
				{
					this.SetAnim("parry", 0, 0);
					this.Slide(-1800f);
				}
				if (!this.Definition.Animations[this.Anim].interpolate && Game1.SlowTime > 0f)
				{
					this.curFrame = 0.4f;
				}
			}
			else
			{
				this.Evade(pMan);
				if (this.Definition.charType == CharacterType.Dust)
				{
					Sound.PlayCue("parry");
				}
			}
		}

		public void KillMe(bool instantly)
		{
			if (!(this.DyingFrame < 0f) && !instantly)
			{
				return;
			}
			this.PlaySound("die");
			if (this.ReturnExp == -1 && this.Team == Team.Enemy)
			{
				this.ReturnExp = Game1.stats.CombatReward(Game1.character, Game1.pManager, this.ID);
				if (Game1.stats.playerLifeState == 0 && this.ReturnExp > 0)
				{
					Game1.stats.comboMeter++;
					Game1.stats.comboTimer = 2f;
					Game1.map.canLockEdge = true;
					Game1.hud.comboTextSize = 3.5f;
					if (Game1.stats.melodicHitTimer <= 0f && !Game1.events.anyEvent && Game1.events.currentEvent > -1)
					{
						Sound.PlayMelodicHit(kill: true, fidget: false);
					}
				}
			}
			this.DyingFrame = (instantly ? 2 : 0);
			this.Ethereal = EtherealState.Ethereal;
			this.Defense = DefenseStates.None;
			this.StatusEffect = StatusEffects.Normal;
			this.DownTime = (this.StatusTime = 0f);
			if (Game1.cManager.challengeMode == ChallengeManager.ChallengeMode.InChallenge)
			{
				this.SetFlag();
			}
			if (instantly)
			{
				if (this.Ai != null && this.ReturnExp > -1)
				{
					this.Ai.Die(Game1.pManager, Character.sprites_01_Tex[this.Definition.Sprites_01_Index + this.RandomSkin]);
					this.Exists = CharExists.Dead;
				}
				if (Game1.hud.hudDetails && this.ReturnExp > 0)
				{
					Game1.pManager.AddXP(this.Location + new Vector2(0f, -100f), this.ReturnExp, bonus: false, 9);
				}
			}
		}

		private void SetFlag()
		{
			if (!(this.Name != ""))
			{
				return;
			}
			bool flag = true;
			Character[] character = Game1.character;
			for (int i = 0; i < character.Length; i++)
			{
				if (character[i].DyingFrame == -1f && this.ID != character[i].ID && this.Name == character[i].Name)
				{
					flag = false;
				}
			}
			if (flag)
			{
				Game1.map.mapScript.Flags.SetFlag(this.Name);
			}
		}

		private void SpinBlade(Character[] c)
		{
			if (Game1.events.currentEvent < 0)
			{
				return;
			}
			if (!Game1.stats.isSpinning)
			{
				if (Game1.stats.overHeating > 0f)
				{
					Game1.stats.overHeating -= Game1.FrameTime;
					if (Game1.stats.overHeating < 0f)
					{
						Game1.stats.overHeating = 0f;
					}
				}
				if (this.defaultColor.G < byte.MaxValue)
				{
					ref Color reference = ref this.defaultColor;
					byte g = (this.defaultColor.B = (byte)(255f - Game1.stats.overHeating / 5f * 255f));
					reference.G = g;
					if (this.defaultColor.G >= 250)
					{
						ref Color reference2 = ref this.defaultColor;
						byte g2 = (this.defaultColor.B = byte.MaxValue);
						reference2.G = g2;
					}
				}
				return;
			}
			if (this.defaultColor.G > 0)
			{
				ref Color reference3 = ref this.defaultColor;
				byte g3 = (this.defaultColor.B = (byte)(255f - Game1.stats.overHeating / 5f * 255f));
				reference3.G = g3;
				if (this.defaultColor.G <= 2)
				{
					ref Color reference4 = ref this.defaultColor;
					byte g4 = (this.defaultColor.B = 0);
					reference4.G = g4;
				}
			}
			Game1.map.MapSegFrameSpeed = 2f;
			VibrationManager.SetScreenShake((float)(255 - this.defaultColor.G) / 255f);
			if (!(Game1.stats.overHeating < 5f))
			{
				return;
			}
			Game1.stats.overHeating += Game1.FrameTime;
			if (!(Game1.stats.overHeating > 5f))
			{
				return;
			}
			Game1.stats.overHeating = 5f;
			if (Game1.stats.gameDifficulty <= 0)
			{
				return;
			}
			Game1.stats.overHeating = 0f;
			Game1.stats.isSpinning = false;
			this.SetAnim("hurtup", 0, 2);
			if (Game1.stats.gameDifficulty > 1)
			{
				int hP = this.HP;
				this.HP = (int)MathHelper.Max(this.HP - Game1.stats.attack, 1f);
				if (Game1.hud.hudDetails)
				{
					Game1.pManager.AddHP(this.Location - new Vector2(0f, 200f * Game1.worldScale), this.HP - hP, critical: false, StatusEffects.Normal, 9);
				}
			}
			this.CanHurtFrame = (this.CanHurtProjectileFrame = 2f);
			CharDir dir = CharDir.Right;
			if (c[0].Face == CharDir.Right)
			{
				dir = CharDir.Left;
			}
			Game1.pManager.MakeSlash(CharacterType.Dust, CharacterType.Dust, new Vector2(c[0].Location.X, c[0].Location.Y - 200f), 1.2f, "KO", 0, Color.White, dir);
			Game1.events.InitEvent(3, isSideEvent: true);
			Game1.cManager.AddScore(-100, this.Location + new Vector2(0f, -260f));
			Game1.stats.comboTimer = (Game1.stats.comboEnemies = (Game1.stats.damageMeter = (Game1.stats.comboMeter = 0)));
			Game1.stats.comboBreak = 2;
			if (Game1.stats.melodicHitCount > -1)
			{
				Sound.PlayCue("melodic_hit_fail");
			}
			Game1.stats.melodicHitTimer = (Game1.stats.melodicMidHitTimer = (Game1.stats.melodicHitCount = -1));
		}

		private void FireTrig(Trigger trig, Vector2 loc, float rot, float scale, ParticleManager pMan)
		{
			switch (trig)
			{
			case Trigger.Emitter:
				if (Game1.FrameTime > 0f)
				{
					pMan.MakeProjectile(loc, new Vector2(1500f, 0f), rot, scale, this.Definition.charType, this.Face, this.ID, 5);
				}
				break;
			case Trigger.Throw:
				pMan.MakeProjectile(loc, new Vector2(1500f, 0f), rot, scale, this.Definition.charType, this.Face, this.ID, 5);
				break;
			case Trigger.Step:
				if (this.State == CharState.Grounded && Game1.events.screenFade.A < 200)
				{
					pMan.MakeFootStep(loc, this.ID, this.Face, 1f, rot + 1.57f, 1f, 5, this.terrainType);
				}
				break;
			case Trigger.GroundPound:
				pMan.MakeGroundDust(loc, Vector2.Zero, this.ID, 0.5f, 1f, 0, 5, this.terrainType);
				pMan.AddShockRing(loc, 0.4f, 5);
				this.Impact(this, loc);
				break;
			case Trigger.KickDirt:
				pMan.MakeGroundDust(loc, new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot)) * 200f * scale, this.ID, scale / 2f, 1f, 1, 5, this.terrainType);
				break;
			case Trigger.SpinEffect:
				pMan.AddSpinBlade(loc, this.Face, 5);
				break;
			case Trigger.Special0:
			case Trigger.Special1:
			case Trigger.Special2:
			case Trigger.Special3:
				if (this.ID == 0)
				{
					this.PlayerSpecialTrigger(trig, loc, rot, scale, pMan);
				}
				else if (this.Ai != null)
				{
					this.Ai.SpecialTrigger(trig, loc, rot, scale, pMan);
				}
				break;
			case Trigger.DebrisUp:
			{
				for (int j = 0; j < 3; j++)
				{
					pMan.AddBlood(new Vector2(loc.X + (float)Rand.GetRandomInt(-40, 40), loc.Y + (float)Rand.GetRandomInt(-40, 40)), new Vector2(Rand.GetRandomInt(-300, 300), Rand.GetRandomInt(-800, -200)), (int)this.defaultColor.R, (int)this.defaultColor.G, (int)this.defaultColor.B, 1f, 1f, this.Definition.charType, this.RandomSkin, 5);
				}
				break;
			}
			case Trigger.DebrisDown:
			{
				for (int i = 0; i < 2; i++)
				{
					pMan.AddBlood(new Vector2(loc.X + (float)Rand.GetRandomInt(-40, 40), loc.Y + (float)Rand.GetRandomInt(-40, 40)), new Vector2(Rand.GetRandomInt(-10, 10), Rand.GetRandomInt(0, 20)), (int)this.defaultColor.R, (int)this.defaultColor.G, (int)this.defaultColor.B, 1f, 0.5f, this.Definition.charType, this.RandomSkin, 5);
				}
				break;
			}
			case Trigger.BigThrow:
				pMan.AddHit(loc, new Vector2((float)this.Face, 0f), this.ID, (int)trig, 5);
				break;
			default:
				pMan.AddHit(loc, new Vector2((float)this.Face, 0f), this.ID, (int)trig, 5);
				break;
			case Trigger.Debris:
				break;
			}
		}

		private void UpdateLocation(Map map, Character[] c)
		{
			if (this.GrabbedBy > -1)
			{
				int grabbedBy = this.GrabbedBy;
				this.State = CharState.Air;
				this.Face = c[grabbedBy].Face;
				this.Location += (c[grabbedBy].Location - this.Location) * Game1.FrameTime * 20f;
				this.CharRotation = c[grabbedBy].CharRotation;
				if (!c[grabbedBy].AnimName.StartsWith("attack"))
				{
					this.SetAnim("hurtup", 0, 0);
					this.SetJump(400f, jumped: false);
					c[grabbedBy].Holding = false;
					this.GrabbedBy = -1;
				}
				return;
			}
			if (this.ID > 0)
			{
				if (this.Location.X < map.leftEdge)
				{
					this.Location.X = map.leftEdge;
				}
				if (this.Location.X > map.rightEdge)
				{
					this.Location.X = map.rightEdge;
				}
			}
			if (this.Location.Y < map.topEdge - 100f)
			{
				this.Location.Y = map.topEdge - 100f;
			}
			if (this.FlyType > FlyingType.None && this.State == CharState.Air)
			{
				this.Location.X += (this.Trajectory.X + this.BoostTraj + this.flyingOffset.X) * this.et;
			}
			else
			{
				this.Location.X += (this.Trajectory.X + this.BoostTraj) * this.et;
			}
			this.Location.X += this.CharCol * this.et;
			if (this.State == CharState.Grounded)
			{
				if (this.DyingFrame > -1f && this.Friction < 6000f)
				{
					this.Friction += 50f;
				}
				if (this.Trajectory.X > 0f)
				{
					this.Trajectory.X -= this.Friction * this.et;
					if (this.Trajectory.X < 0f)
					{
						this.Trajectory.X = 0f;
					}
				}
				else if (this.Trajectory.X < 0f)
				{
					this.Trajectory.X += this.Friction * this.et;
					if (this.Trajectory.X > 0f)
					{
						this.Trajectory.X = 0f;
					}
				}
				if (!this.AnimName.StartsWith("hurt") && !this.AnimName.StartsWith("die") && this.DyingFrame > -1f)
				{
					this.SetJump(600f, jumped: false);
				}
				return;
			}
			if (this.LungeState == LungeStates.None)
			{
				if (this.Trajectory.X > 0f)
				{
					this.Trajectory.X -= this.Friction * this.et;
					if (this.Trajectory.X < 0f)
					{
						this.Trajectory.X = 0f;
					}
				}
				else if (this.Trajectory.X < 0f)
				{
					this.Trajectory.X += this.Friction * this.et;
					if (this.Trajectory.X > 0f)
					{
						this.Trajectory.X = 0f;
					}
				}
			}
			if (this.Definition.charType == CharacterType.Dust && !this.IsFalling && !Game1.events.anyEvent && (Character.curState.Buttons.A == ButtonState.Pressed || Game1.pcManager.IsPlayerHeld("KeyJump")))
			{
				this.jumpTime /= 1.4f;
			}
			else
			{
				this.jumpTime /= 10f;
				if (this.jumpTime < 1f)
				{
					this.jumpTime = 0f;
				}
			}
			if (this.FlyType > FlyingType.None)
			{
				if (this.DyingFrame == -1f)
				{
					this.State = CharState.Air;
					if (!this.AnimName.StartsWith("hurt"))
					{
						if (this.Trajectory.Y > 0f)
						{
							this.Trajectory.Y -= this.Friction * this.et;
							if (this.Trajectory.Y < 0f)
							{
								this.Trajectory.Y = 0f;
							}
						}
						else if (this.Trajectory.Y < 0f)
						{
							this.Trajectory.Y += this.Friction * this.et;
							if (this.Trajectory.Y > 0f)
							{
								this.Trajectory.Y = 0f;
							}
						}
					}
					this.Location.Y += (this.Trajectory.Y + this.flyingOffset.Y) * this.et;
				}
				else
				{
					this.Trajectory.Y += 0f - this.jumpTime + this.et * Game1.Gravity;
					this.Location.Y += this.Trajectory.Y * this.et;
				}
			}
			else if (!this.Floating)
			{
				if (!this.Hanging)
				{
					this.Trajectory.Y += 0f - this.jumpTime + this.et * Game1.Gravity;
				}
				this.Location.Y += this.Trajectory.Y * this.et;
			}
			else
			{
				if (this.ID > 0 || (this.AnimName != "airspin" && !this.Hanging) || (this.AnimName == "airspin" && (this.Boosting == 3 || this.Boosting == 6 || this.Boosting == -1)))
				{
					this.Trajectory.Y += 0f - this.jumpTime + this.et * Game1.Gravity / 2f;
				}
				this.Location.Y += this.Trajectory.Y * this.et / 2f;
			}
		}

		private void UpdateRotation()
		{
			if (this.State == CharState.Air)
			{
				if (this.AnimName == "airspin")
				{
					if (this.Location.X > this.PLoc.X)
					{
						this.CharRotation = GlobalFunctions.GetAngle(this.PLoc, this.Location);
					}
					else if (this.Location.X < this.PLoc.X)
					{
						this.CharRotation = GlobalFunctions.GetAngle(this.Location, this.PLoc);
					}
				}
				else if (this.FlyType == FlyingType.None)
				{
					this.CharRotation = (this.CharRotationDest = 3.14f);
				}
				else
				{
					if (this.Location.X > this.PLoc.X)
					{
						this.CharRotationDest = GlobalFunctions.GetAngle(this.PLoc, this.Location);
					}
					else if (this.Location.X < this.PLoc.X)
					{
						this.CharRotationDest = GlobalFunctions.GetAngle(this.Location, this.PLoc);
					}
					else
					{
						this.CharRotationDest += (3.14f - this.CharRotationDest) * this.et;
					}
					this.CharRotationDest = MathHelper.Clamp(this.CharRotationDest, 2.6f, 3.7f);
					float num = MathHelper.Clamp(this.Speed / 600f, 0f, 1f) * this.et;
					if (this.Trajectory.Y < 0f)
					{
						num *= 0.25f;
					}
					if (this.CharRotation > this.CharRotationDest + 0.05f)
					{
						this.CharRotation -= num;
					}
					else if (this.CharRotation < this.CharRotationDest - 0.05f)
					{
						this.CharRotation += num;
					}
				}
			}
			else
			{
				bool flag = false;
				float num2 = Math.Min(Math.Abs(this.Trajectory.X) / 600f, 1f) * 200f * this.et;
				if (this.ID == 0 || Game1.events.anyEvent)
				{
					if (this.AnimName == "runend")
					{
						num2 = 300f * this.et;
						this.CharRotation = (this.CharRotationDest = 3.14f);
					}
					else if (this.AnimName == "run" || this.AnimName.StartsWith("crouch"))
					{
						flag = true;
					}
					else
					{
						num2 = 600f * this.et;
						this.CharRotationDest = 3.14f;
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					if (this.Location.X > this.PLoc.X)
					{
						this.CharRotationDest = GlobalFunctions.GetAngle(this.PLoc, this.Location);
					}
					else if (this.Location.X < this.PLoc.X)
					{
						this.CharRotationDest = GlobalFunctions.GetAngle(this.Location, this.PLoc);
					}
				}
				if (this.CharRotation != this.CharRotationDest && (this.ID == 0 || this.Location.X != this.PLoc.X))
				{
					if (this.ID == 0 && this.CharRotationDest > 3.04f && this.CharRotationDest < 3.24f)
					{
						num2 *= 3f;
					}
					this.CharRotation += (this.CharRotationDest - this.CharRotation) * num2 * this.et;
				}
			}
			this.CharRotation = MathHelper.Clamp(this.CharRotation, 2.6f, 3.7f);
		}

		public void ResetRotation()
		{
			this.CharRotation = (this.CharRotationDest = 3.14f);
		}

		private void UpdateCharCol(Character[] c, float frameTime)
		{
			float num = -1f;
			int num2 = -1;
			if (!this.renderable || this.NPC != 0)
			{
				return;
			}
			for (int i = 0; i < c.Length; i++)
			{
				if (i == this.ID || c[i].Exists != CharExists.Exists || !c[i].renderable || c[i].NPC != 0 || (this.Ethereal >= EtherealState.Ethereal && c[i].Defense < DefenseStates.Parrying) || c[i].Ethereal >= EtherealState.Ethereal || this.DyingFrame != -1f || !this.InCharBounds(c, i))
				{
					continue;
				}
				float num3 = Math.Max(c[i].DefaultWidth / 2, 120);
				float num4 = MathHelper.Clamp(Math.Abs(this.Location.X - c[i].Location.X) / num3, 0.1f, 0.4f);
				num = num3 / num4;
				num2 = i;
				if ((c[i].Ethereal == EtherealState.Immovable && this.LiftType < CanLiftType.NoLift) || c[i].LiftType >= CanLiftType.NoLift)
				{
					c[i].CharCol = 0f;
					continue;
				}
				if (this.Location.X < c[i].Location.X)
				{
					c[i].CharCol = num;
				}
				else
				{
					c[i].CharCol = 0f - num;
				}
				if (c[i].WallInWay)
				{
					c[i].CharCol = 0f;
				}
			}
			if (num > -1f)
			{
				if (this.Ethereal == EtherealState.Immovable && c[num2].LiftType < CanLiftType.NoLift)
				{
					this.CharCol = 0f;
				}
				else if (this.Location.X < c[num2].Location.X)
				{
					this.CharCol = 0f - num;
					if (this.Trajectory.X >= this.Speed)
					{
						this.Trajectory.X /= 4f;
					}
				}
				else
				{
					this.CharCol = num;
					if (this.Trajectory.X <= 0f - this.Speed)
					{
						this.Trajectory.X /= 4f;
					}
				}
			}
			if (this.CharCol != 0f)
			{
				if (num == -1f)
				{
					this.CharCol = MathHelper.Clamp(this.CharCol, -100f, 100f);
				}
				if (this.LiftType >= CanLiftType.NoLift || this.AnimName.StartsWith("crouch"))
				{
					this.CharCol = 0f;
				}
				else if (this.CharCol > 0f)
				{
					this.CharCol -= 2400f * frameTime;
					if (this.CharCol < 0f)
					{
						this.CharCol = 0f;
					}
				}
				else if (this.CharCol < 0f)
				{
					this.CharCol += 2400f * frameTime;
					if (this.CharCol > 0f)
					{
						this.CharCol = 0f;
					}
				}
			}
			if (this.WallInWay)
			{
				this.CharCol = 0f;
			}
		}

		private void UpdateHP()
		{
			int num = ((this.ID == 0) ? ((int)Math.Min((float)this.MaxHP * Game1.stats.bonusHealth, 9999f)) : this.MaxHP);
			this.HP = (int)MathHelper.Clamp(this.HP, 0f, num);
			if (this.pHP != this.HP)
			{
				this.HPLossFrame = 5f;
				if (this.HP > this.pHP && Game1.hud.hudDetails)
				{
					Game1.pManager.AddHP(this.Location - new Vector2(0f, 200f * Game1.worldScale), this.HP - this.pHP, critical: false, StatusEffects.Normal, 9);
				}
			}
			if (this.HPLossFrame > 0f)
			{
				this.HPLossFrame -= Game1.FrameTime;
			}
			this.pHP = this.HP;
			if (this.CanHurtFrame > 0f)
			{
				if (this.ID != 0 || !this.AnimName.StartsWith("hurt") || this.State == CharState.Grounded)
				{
					this.CanHurtFrame -= Game1.FrameTime;
				}
				if (this.CanHurtFrame < 0f)
				{
					this.CanHurtFrame = 0f;
				}
			}
			if (this.CanHurtProjectileFrame > 0f)
			{
				this.CanHurtProjectileFrame -= Game1.FrameTime;
				if (this.CanHurtProjectileFrame < 0f)
				{
					this.CanHurtProjectileFrame = 0f;
				}
			}
			if (this.HP <= 0 && this.ID > 0)
			{
				this.KillMe(instantly: false);
			}
		}

		private bool UpdateDeath()
		{
			if (this.Location.Y > Game1.map.bottomEdge && this.ID != 0 && this.Location.Y - (float)this.DefaultHeight > Game1.map.bottomEdge)
			{
				_ = this.NPC;
				_ = 1;
				this.DyingFrame = 2f;
				return true;
			}
			if (this.DyingFrame > -1f && this.DyingFrame < 1f)
			{
				ParticleManager pManager = Game1.pManager;
				this.FlyType = FlyingType.None;
				float dyingFrame = this.DyingFrame;
				if (this.GrabbedBy < 0)
				{
					this.DyingFrame += Game1.FrameTime / 2f;
					this.HPLossFrame -= Game1.FrameTime * 2f;
				}
				if (this.DyingFrame > 0.8f && dyingFrame < 0.8f && Game1.hud.hudDetails && this.ReturnExp > 0)
				{
					pManager.AddXP(this.Location + new Vector2(0f, -100f), this.ReturnExp, bonus: false, 9);
				}
				if (this.State == CharState.Grounded)
				{
					this.Ethereal = EtherealState.Ethereal;
				}
				switch (this.DieType)
				{
				case DyingTypes.BodyBurn:
					if (this.renderable && this.DyingFrame > 0.5f && this.DyingFrame < 0.7f)
					{
						int num = (int)MathHelper.Clamp((this.DefaultWidth + this.DefaultHeight) / 250, 1f, 100f);
						for (int j = 0; j < num; j++)
						{
							pManager.AddDeathFlame(this.Location + Rand.GetRandomVector2((float)(-this.DefaultWidth) / 1.5f, (float)this.DefaultWidth / 1.5f, -this.DefaultHeight / 2, 60f), Rand.GetRandomVector2(-40f, 40f, -150f, -20f), 0.3f, Rand.GetRandomFloat(0.2f, 0.75f), Rand.GetRandomFloat(0.5f, 0.8f), Rand.GetRandomFloat(0.6f, 1f), 0, audio: true, 5);
						}
					}
					break;
				case DyingTypes.BodyExplode:
					if (this.renderable && this.DyingFrame < 0.3f)
					{
						for (int i = 0; i < 2; i++)
						{
							Game1.pManager.AddExplosion(this.Location + Rand.GetRandomVector2((float)(-this.DefaultWidth) / 1.5f, (float)this.DefaultWidth / 1.5f, -this.DefaultHeight, 60f), Rand.GetRandomFloat(1.8f, 2.8f), (Rand.GetRandomInt(0, 3) != 0) ? true : false, 5);
						}
						Vector2 loc = this.Location + Rand.GetRandomVector2(-100f, 100f, -100f, 100f);
						Game1.pManager.AddBounceSpark(loc, Rand.GetRandomVector2(-800f, 800f, -500f, 10f), 0.5f, 5);
					}
					break;
				case DyingTypes.BodyVanish:
					if (this.Ai != null && this.ReturnExp > -1)
					{
						this.Ai.Die(pManager, Character.sprites_01_Tex[this.Definition.Sprites_01_Index + this.RandomSkin]);
					}
					if (Game1.hud.hudDetails && this.ReturnExp > 0)
					{
						pManager.AddXP(this.Location + new Vector2(0f, -100f), this.ReturnExp, bonus: false, 9);
					}
					this.DyingFrame = 2f;
					break;
				case DyingTypes.BodyStay:
					this.DyingFrame = 0f;
					this.Exists = CharExists.DeadBodyStay;
					break;
				}
				return true;
			}
			return false;
		}

		private void UpdateStatus(ParticleManager pMan)
		{
			if (this.HP <= 0 || Game1.events.regionIntroStage > 0)
			{
				return;
			}
			int num = (int)Math.Floor(this.StatusTime);
			if (!Game1.events.anyEvent)
			{
				this.StatusTime -= Game1.FrameTime;
			}
			bool flag = false;
			if (num > (int)Math.Floor(this.StatusTime))
			{
				flag = true;
			}
			Vector2 loc = this.Location - new Vector2(0f, 200f * Game1.worldScale);
			switch (this.StatusEffect)
			{
			case StatusEffects.Poison:
				if (Rand.GetRandomInt(0, 6) == 0)
				{
					pMan.AddBubble(this.Location + Rand.GetRandomVector2(-this.DefaultWidth / 2, this.DefaultWidth / 2, -this.DefaultHeight, 0f), new Vector2(0f, -200f), 0.3f, 0f, 1f, 0.5f, 5);
				}
				break;
			case StatusEffects.Burning:
				Game1.dManager.PlatformTrajectory(this, this.Location);
				pMan.AddFlamePuff(this.Location + Rand.GetRandomVector2(-this.DefaultWidth / 2, this.DefaultWidth / 2, -this.DefaultHeight, 0f), new Vector2(0f, -200f) + this.Trajectory * 0.2f + Game1.dManager.PlatformTrajectory(this, this.Location) * 40f, 1.5f, Rand.GetRandomInt(5, 7));
				break;
			case StatusEffects.Silent:
				if (this.ID == 0)
				{
					loc = pMan.GetFidgetLoc(accomodateScroll: false);
					pMan.AddVerticleBeam(loc, new Vector2(Rand.GetRandomFloat(-80f, 80f), 0f), 0f, 0f, 0f, 0.6f, 20, Rand.GetRandomInt(100, 300), Rand.GetRandomFloat(0.2f, 0.5f), 100, 6);
					loc += new Vector2(0f, 100f);
				}
				break;
			default:
				this.StatusStrength = 0;
				this.StatusTime = 0f;
				this.StatusEffect = StatusEffects.Normal;
				return;
			}
			if (flag)
			{
				this.HP -= this.StatusStrength;
				if (this.HP > 1 && Game1.hud.hudDetails)
				{
					pMan.AddHP(loc, -this.StatusStrength, critical: false, this.StatusEffect, 9);
				}
				if (this.ID == 0)
				{
					switch (this.StatusEffect)
					{
					case StatusEffects.Poison:
						Game1.events.InitEvent(8, isSideEvent: true);
						break;
					case StatusEffects.Silent:
						Game1.events.InitEvent(83, isSideEvent: true);
						break;
					}
				}
			}
			if (this.HP < 1)
			{
				this.HP = 1;
			}
			if (this.StatusTime <= 0f)
			{
				this.StatusEffect = StatusEffects.Normal;
			}
		}

		private void UpdateDowned()
		{
			float downTime = this.DownTime;
			this.DownTime = Math.Max(this.DownTime - Game1.FrameTime, 0f);
			if (!(downTime > 0f))
			{
				return;
			}
			if (this.Boosting == 0)
			{
				this.DownTime = 0f;
			}
			if (this.DownTime != 0f || !this.AnimName.Contains("down"))
			{
				return;
			}
			if (this.State == CharState.Grounded)
			{
				this.SetAnim("getup", 0, 2);
				if (this.AnimName != "getup")
				{
					this.SetAnim("idle00", 0, 0);
				}
			}
			else
			{
				this.SetAnim("hurtup", 0, 0);
			}
		}

		private void UpdateRegions(Map map)
		{
			this.Boosting = -1;
			if (this.LiftType < CanLiftType.NoLift && !this.Hanging)
			{
				for (int i = 0; i < map.boostRegions.Count; i++)
				{
					if (map.boostRegions[i] == null || !(this.Location.X > (float)map.boostRegions[i].Region.X) || !(this.Location.X < (float)(map.boostRegions[i].Region.X + map.boostRegions[i].Region.Width)) || !(this.Location.Y > (float)map.boostRegions[i].Region.Y) || !(this.Location.Y < (float)(map.boostRegions[i].Region.Y + map.boostRegions[i].Region.Height)))
					{
						continue;
					}
					switch (this.Boosting = map.boostRegions[i].Direction)
					{
					case 3:
						this.BoostTraj = Math.Max(((float)map.boostRegions[i].Region.Width - (this.Location.X - (float)map.boostRegions[i].Region.X)) / (float)map.boostRegions[i].Region.Width * 1.2f * this.Speed * Math.Max(this.Trajectory.X / this.Speed, 1f), 0f);
						if (new Rectangle(map.boostRegions[i].Region.X, map.boostRegions[i].Region.Y, 100, map.boostRegions[i].Region.Y).Contains((int)this.Location.X, (int)this.Location.Y))
						{
							this.BoostTraj *= 4f;
						}
						else if (this.AnimName == "airspin")
						{
							this.Slide(this.Speed * 1.5f);
							this.Trajectory.Y = 0f;
						}
						else if (this.State == CharState.Air)
						{
							this.BoostTraj *= 2f;
						}
						continue;
					case 6:
					{
						float num = ((float)map.boostRegions[i].Region.Height - (this.Location.Y - (float)map.boostRegions[i].Region.Y)) / (float)map.boostRegions[i].Region.Height * this.JumpVelocity;
						this.Trajectory.Y = MathHelper.Clamp(this.Trajectory.Y + num, 0f - this.JumpVelocity, 2400f);
						continue;
					}
					case 9:
						this.BoostTraj = 0f - Math.Max((this.Location.X - (float)map.boostRegions[i].Region.X) / (float)map.boostRegions[i].Region.Width * 1.2f * this.Speed * Math.Max((0f - this.Trajectory.X) / this.Speed, 1f), 0f);
						if (new Rectangle(map.boostRegions[i].Region.X + map.boostRegions[i].Region.Width - 100, map.boostRegions[i].Region.Y, 100, map.boostRegions[i].Region.Y).Contains((int)this.Location.X, (int)this.Location.Y))
						{
							this.BoostTraj *= 4f;
						}
						else if (this.AnimName == "airspin")
						{
							this.Slide(this.Speed * 1.5f);
							this.Trajectory.Y = 0f;
						}
						else if (this.State == CharState.Air)
						{
							this.BoostTraj *= 2f;
						}
						continue;
					}
					if (this.State == CharState.Grounded)
					{
						if (!this.AnimName.StartsWith("hurt"))
						{
							this.SetJump(200f, jumped: true);
						}
						continue;
					}
					bool flag = false;
					if (this.Definition.charType != 0)
					{
						if (!this.AnimName.StartsWith("hurt"))
						{
							flag = true;
						}
					}
					else if (Game1.stats.playerLifeState == 0 && (!this.AnimName.StartsWith("hurt") || this.AnimFrame > 10))
					{
						if (this.AnimName.StartsWith("hurt"))
						{
							this.SetAnim("evadeair", 5, 0);
						}
						if ((this.AnimName == "attackairdown" && this.AnimFrame > 9) || this.Holding)
						{
							this.SetAnim("evadeair", 1, 0);
						}
						flag = true;
					}
					if (flag && (this.Trajectory.Y > -4000f || this.AnimName != "jumpboost"))
					{
						float num2 = MathHelper.Clamp((float)map.boostRegions[i].Region.Height - Math.Abs(this.Location.Y - (float)(map.boostRegions[i].Region.Y + map.boostRegions[i].Region.Height)), 10f, 500f);
						this.LungeState = LungeStates.None;
						this.Trajectory.Y += (MathHelper.Clamp(this.Trajectory.Y - num2, -500f, 2400f) - this.Trajectory.Y) * Game1.FrameTime * 40f;
					}
				}
				if (this.Boosting == -1 && Game1.wManager.weatherType == WeatherType.SnowFierce && (this.ID > 0 || Game1.wManager.playerCanPrecipitate) && Game1.hud.dialogueState == DialogueState.Inactive && Game1.menu.prompt == promptDialogue.None && Game1.hud.shopType == ShopType.None && this.Location.X > map.leftEdge + 180f && this.Location.X < map.rightEdge - 180f && this.FlyType == FlyingType.None && !this.AnimName.StartsWith("crouch") && this.AnimName != "attackairdown")
				{
					this.BoostTraj = -300f;
				}
			}
			if (this.BoostTraj != 0f)
			{
				if (this.Hanging || this.GrabbedBy > -1 || Game1.hud.unlockState > 0 || Game1.events.anyEvent)
				{
					this.BoostTraj = 0f;
				}
				if (this.State == CharState.Grounded)
				{
					this.BoostTraj += (0f - this.BoostTraj) * this.et * 8f;
				}
				else if (this.KeyLeft || this.KeyRight)
				{
					this.BoostTraj += (0f - this.BoostTraj) * this.et * 20f;
				}
				else
				{
					this.BoostTraj += (0f - this.BoostTraj) * this.et * 4f;
				}
				if (Math.Abs(this.BoostTraj) < 4f)
				{
					this.BoostTraj = 0f;
				}
			}
			if (this.State == CharState.Grounded && Game1.halfSecFrame % 5 == 0)
			{
				this.CheckTerrain(map);
			}
		}

		private void CheckTerrain(Map map)
		{
			if (Game1.cManager.currentChallenge > -1)
			{
				this.terrainType = TerrainType.Rock;
			}
			else
			{
				switch (map.regionName)
				{
				case "cave":
					this.terrainType = TerrainType.Rock;
					break;
				case "snow":
				case "sanc":
					this.terrainType = TerrainType.Snow;
					break;
				case "grave":
					if (map.path.Contains("mansion"))
					{
						this.terrainType = TerrainType.WoodHeavy;
					}
					else
					{
						this.terrainType = TerrainType.Dirt;
					}
					break;
				default:
					this.terrainType = TerrainType.Dirt;
					break;
				}
			}
			for (int i = 0; i < map.terrainRegion.Count; i++)
			{
				if (map.terrainRegion[i] != null && map.terrainRegion[i].Region.Contains((int)this.Location.X, (int)this.Location.Y))
				{
					this.terrainType = (TerrainType)map.terrainRegion[i].Id;
				}
			}
			if (Game1.dManager.PlatformTrajectory(this, this.Location).X != 0f)
			{
				this.terrainType = TerrainType.Rock;
			}
		}

		public void MapCollision(Map map, Character[] c)
		{
			this.WallInWay = false;
			if (map.leftBlock != 0f)
			{
				if (this.ID == 0)
				{
					if (map.transInFrame < 0f)
					{
						if (this.Location.X < map.leftBlock + 300f)
						{
							map.CheckBlockedEdge(c, CharDir.Left, map.leftBlock + 300f, forceBlock: true);
						}
						if (this.Location.X > map.rightBlock - 300f)
						{
							map.CheckBlockedEdge(c, CharDir.Right, map.rightBlock - 300f, forceBlock: true);
						}
					}
				}
				else if (this.Team == Team.Enemy && this.DyingFrame == -1f && this.Ai != null && this.Ai.GetTarget() > -1)
				{
					if (this.Location.X < map.leftBlock + 200f)
					{
						if (this.Trajectory.X < 0f)
						{
							this.Trajectory.X = 0f;
						}
						this.Location.X = map.leftBlock + 200f;
						this.WallInWay = true;
					}
					if (this.Location.X > map.rightBlock - 200f)
					{
						if (this.Trajectory.X > 0f)
						{
							this.Trajectory.X = 0f;
						}
						this.Location.X = map.rightBlock - 200f;
						this.WallInWay = true;
					}
				}
			}
			if (this.GrabbedBy > -1)
			{
				return;
			}
			this.CheckXCol(map, this.PLoc, 32f);
			int num = map.CheckCol(this.Location);
			bool sideCollision = false;
			if (this.State == CharState.Air && this.GrabbedBy == -1)
			{
				this.CheckXCol(map, this.PLoc, 192f);
				this.CheckXCol(map, this.PLoc, 96f);
				if (this.Trajectory.Y < 0f)
				{
					bool flag = false;
					if (this.Location.Y < map.topEdge + (float)this.DefaultHeight - 32f && (map.TransitionDestination[0] == "" || this.Location.Y < map.topEdge - (float)this.DefaultHeight - 32f))
					{
						flag = true;
					}
					int num2 = map.CheckCol(this.Location - new Vector2(0f, this.DefaultHeight - 32));
					if (num2 > 0 || Game1.dManager.CheckColUpper(this.ID, this.DefaultHeight - 32))
					{
						flag = true;
					}
					if (flag)
					{
						if (this.FlyType != 0)
						{
							this.Trajectory.Y = this.Speed * 0.1f;
							if (this.Ai != null)
							{
								this.Ai.ResetJobFrame();
							}
						}
						else
						{
							this.Trajectory.Y = 0f;
						}
						this.Location.Y = this.PLoc.Y;
						this.IsFalling = true;
						if (num2 > 2)
						{
							HitManager.CheckWallHazard(Game1.character, this.ID, Game1.pManager, 100, (ColType)num2);
							this.Trajectory.Y = 0f - this.Trajectory.Y;
						}
						else if (this.AnimName.Contains("jump") && !this.AnimName.Contains("attack") && this.Definition.charType == CharacterType.Dust && this.Boosting == -1)
						{
							this.SetAnim("idle00", 0, 2);
							this.SetAnim("jump", 16, 0);
						}
						this.CheckXCol(map, this.PLoc, 185f);
					}
				}
				else if (Game1.dManager.CheckCol(this, 0, ref sideCollision) > -1f)
				{
					this.Location.Y = Game1.dManager.CheckCol(this, 0, ref sideCollision);
					this.ledgeAttach = -1;
					this.CanFallThrough = false;
					this.Land(Game1.pManager);
				}
				else if (num > 0)
				{
					this.Location.Y = (int)(this.Location.Y / 64f) * 64;
					while (map.CheckCol(this.Location - new Vector2(0f, 32f)) > 0 && this.Location.Y > map.topEdge)
					{
						this.Location.Y -= 64f;
					}
					this.ledgeAttach = -1;
					this.CanFallThrough = false;
					this.Land(Game1.pManager);
					if (num > 2)
					{
						HitManager.CheckWallHazard(Game1.character, this.ID, Game1.pManager, 100, (ColType)num);
						for (int i = 0; i < 10; i++)
						{
							this.HazardEffect(num);
						}
					}
				}
				if (((!(this.AnimName == "jump") && !(this.AnimName == "airhike")) || !(this.Trajectory.Y + MathHelper.Clamp(Math.Abs(this.Trajectory.X), 0f, 1000f) > 0f)) && (!(this.AnimName != "jump") || !(this.AnimName != "airhike")) && this.LungeState == LungeStates.None)
				{
					return;
				}
				for (int j = 0; j < map.maxPlayerLedges; j++)
				{
					if (!map.GetLedgeMinMax(j, this.Location.X))
					{
						continue;
					}
					int ledgeSec = map.GetLedgeSec(j, this.PLoc.X);
					int ledgeSec2 = map.GetLedgeSec(j, this.Location.X);
					if (ledgeSec <= -1 || ledgeSec2 <= -1)
					{
						continue;
					}
					float ledgeYLoc = map.GetLedgeYLoc(j, ledgeSec, this.PLoc.X);
					float ledgeYLoc2 = map.GetLedgeYLoc(j, ledgeSec2, this.Location.X);
					if (this.PLoc.Y <= ledgeYLoc + 30f && this.Location.Y >= ledgeYLoc2)
					{
						if (this.ID == 0)
						{
							this.AnimName.StartsWith("attack");
						}
						if (!this.AnimName.StartsWith("fly") || map.ledges[j].Flag == LedgeFlags.Solid)
						{
							this.Land(Game1.pManager);
							this.Location.Y = ledgeYLoc2;
							this.ledgeAttach = j;
							this.CanFallThrough = map.ledges[j].Flag == LedgeFlags.CanFallThrough;
						}
						break;
					}
				}
			}
			else
			{
				if (this.State != 0)
				{
					return;
				}
				if (!this.AnimName.StartsWith("crouch") && this.Height > 96)
				{
					this.CheckXCol(map, this.PLoc, 96f);
				}
				int num3 = this.ledgeAttach;
				if (num3 != -1)
				{
					if (num == 0 || this.Trajectory.Y <= 0f)
					{
						int ledgeSec3 = map.GetLedgeSec(num3, this.Location.X);
						if (ledgeSec3 == -1)
						{
							this.FallOff(c, fallThrough: false);
						}
						else
						{
							float y = this.Location.Y;
							this.Location.Y = map.GetLedgeYLoc(num3, ledgeSec3, this.Location.X);
							if (y >= this.Location.Y)
							{
								this.Trajectory.Y = -1f;
							}
							else
							{
								this.Trajectory.Y = 1f;
							}
						}
						if (this.Trajectory.Y < 0f && (map.CheckCol(this.Location - new Vector2(0f, this.DefaultHeight - 32)) > 0 || Game1.dManager.CheckColUpper(this.ID, this.DefaultHeight - 32)))
						{
							this.canJump = false;
							if (this.Trajectory.Y < 0f)
							{
								this.Location.Y = this.PLoc.Y;
							}
							if (this.Trajectory.X > 0f)
							{
								this.Location.X = this.PLoc.X;
							}
							if (this.Trajectory.X < 0f)
							{
								this.Location.X = this.PLoc.X;
							}
						}
						else
						{
							this.canJump = true;
						}
					}
					else
					{
						this.Location.Y = (int)(this.Location.Y / 64f) * 64;
						this.ledgeAttach = -1;
						this.CanFallThrough = false;
					}
					return;
				}
				this.Trajectory.Y = 0f;
				for (int k = 0; k < map.maxPlayerLedges; k++)
				{
					if (!map.GetLedgeMinMax(k, this.Location.X))
					{
						continue;
					}
					int ledgeSec4 = map.GetLedgeSec(k, this.PLoc.X);
					int ledgeSec5 = map.GetLedgeSec(k, this.Location.X);
					if (ledgeSec4 > -1 && ledgeSec5 > -1)
					{
						float ledgeYLoc3 = map.GetLedgeYLoc(k, ledgeSec4, this.PLoc.X);
						float ledgeYLoc4 = map.GetLedgeYLoc(k, ledgeSec5, this.Location.X);
						if (this.PLoc.Y <= ledgeYLoc3 + 20f && this.Location.Y >= ledgeYLoc4 - 2f)
						{
							this.Location.Y = ledgeYLoc4;
							this.ledgeAttach = k;
							this.CanFallThrough = map.ledges[k].Flag == LedgeFlags.CanFallThrough;
							break;
						}
					}
				}
				if (num == 0 && Game1.dManager.CheckCol(this, 32, ref sideCollision) == -1f && num3 == -1)
				{
					this.FallOff(c, fallThrough: false);
				}
				else if (num > 2)
				{
					HitManager.CheckWallHazard(Game1.character, this.ID, Game1.pManager, 100, (ColType)num);
					this.HazardEffect(num);
				}
			}
		}

		public void PlayerInput(int index)
		{
			if (Game1.stats.upgrade[15] == 0)
			{
				Game1.stats.doubleJump = 10;
			}
			Character.curState = GamePad.GetState((PlayerIndex)index);
			this.KeyJump = false;
			this.KeyUp = false;
			this.KeyDown = false;
			this.KeyAttack = false;
			this.KeySecondary = false;
			this.KeyThrow = false;
			this.KeyEvade = false;
			this.Defense = DefenseStates.None;
			if (Game1.map.GetTransVal() > 0f)
			{
				return;
			}
			this.KeyLeft = false;
			this.KeyRight = false;
			if (Game1.map.GetTransVal() > 2f || Game1.stats.playerLifeState > 0 || Game1.events.anyEvent)
			{
				return;
			}
			Game1.pcManager.UpdatePlayerInput(ref this.KeyLeft, ref this.KeyRight, ref this.KeyUp, ref this.KeyDown, ref this.KeyJump, ref this.KeyThrow, ref this.KeyAttack, ref this.KeySecondary, ref this.KeyEvade, this);
			if (!this.Holding)
			{
				if (Character.curState.ThumbSticks.Left.X < -0.1f && Character.curState.DPad.Right != ButtonState.Pressed)
				{
					this.KeyLeft = true;
					Game1.pcManager.inputDevice = InputDevice.GamePad;
				}
				else if (Character.curState.ThumbSticks.Left.X > 0.1f && Character.curState.DPad.Left != ButtonState.Pressed)
				{
					this.KeyRight = true;
					Game1.pcManager.inputDevice = InputDevice.GamePad;
				}
				if (Character.curState.DPad.Left == ButtonState.Pressed && Character.curState.ThumbSticks.Left.X < 0.1f)
				{
					this.KeyLeft = true;
					Game1.pcManager.inputDevice = InputDevice.GamePad;
				}
				else if (Character.curState.DPad.Right == ButtonState.Pressed && Character.curState.ThumbSticks.Left.X > -0.1f)
				{
					this.KeyRight = true;
					Game1.pcManager.inputDevice = InputDevice.GamePad;
				}
				if (Character.curState.ThumbSticks.Left.Y < -0.8f || Character.curState.DPad.Down == ButtonState.Pressed)
				{
					this.KeyDown = true;
					Game1.pcManager.inputDevice = InputDevice.GamePad;
				}
				else if (Character.curState.ThumbSticks.Left.Y > 0.8f || Character.curState.DPad.Up == ButtonState.Pressed)
				{
					this.KeyUp = true;
					Game1.pcManager.inputDevice = InputDevice.GamePad;
				}
				if (Character.curState.Buttons.A == ButtonState.Pressed && Character.prevState.Buttons.A == ButtonState.Released)
				{
					this.KeyJump = true;
					Game1.pcManager.inputDevice = InputDevice.GamePad;
				}
				if (Character.curState.ThumbSticks.Right.X < -0.1f && Character.prevState.ThumbSticks.Right.X > -0.1f)
				{
					if (this.CheckEvade())
					{
						this.Face = CharDir.Right;
					}
					Game1.pcManager.inputDevice = InputDevice.GamePad;
				}
				else if (Character.curState.ThumbSticks.Right.X > 0.1f && Character.prevState.ThumbSticks.Right.X < 0.1f)
				{
					if (this.CheckEvade())
					{
						this.Face = CharDir.Left;
					}
					Game1.pcManager.inputDevice = InputDevice.GamePad;
				}
				if (Character.curState.Triggers.Right > 0.1f && Character.prevState.Triggers.Right < 0.1f)
				{
					if (this.CheckEvade())
					{
						this.Face = CharDir.Left;
					}
					Game1.pcManager.inputDevice = InputDevice.GamePad;
				}
				else if (Character.curState.Triggers.Left > 0.1f && Character.prevState.Triggers.Left < 0.1f)
				{
					if (this.CheckEvade())
					{
						this.Face = CharDir.Right;
					}
					Game1.pcManager.inputDevice = InputDevice.GamePad;
				}
			}
			if (Game1.stats.gameDifficulty == 0 || Game1.settings.AutoCombo)
			{
				if (Character.curState.Buttons.X == ButtonState.Pressed)
				{
					this.KeyAttack = true;
					Game1.pcManager.inputDevice = InputDevice.GamePad;
					if (this.AnimName.StartsWith("attack"))
					{
						this.Defense = DefenseStates.Parrying;
						this.CanHurtFrame = 0f;
					}
				}
				if (Character.curState.Buttons.Y == ButtonState.Pressed)
				{
					if (this.State == CharState.Grounded || this.AnimName == "airspin")
					{
						this.KeySecondary = true;
					}
					else if (Character.prevState.Buttons.Y == ButtonState.Released)
					{
						this.KeySecondary = true;
					}
					Game1.pcManager.inputDevice = InputDevice.GamePad;
				}
				if (Character.curState.Buttons.B == ButtonState.Pressed && Game1.longSkipFrame == 1)
				{
					this.KeyThrow = true;
					Game1.pcManager.inputDevice = InputDevice.GamePad;
				}
			}
			else
			{
				if (Character.curState.Buttons.X == ButtonState.Pressed)
				{
					Game1.pcManager.inputDevice = InputDevice.GamePad;
					if (this.AnimName.StartsWith("attack"))
					{
						this.Defense = DefenseStates.Parrying;
						this.CanHurtFrame = 0f;
					}
					if (Character.prevState.Buttons.X == ButtonState.Released)
					{
						this.KeyAttack = true;
					}
				}
				if (Character.curState.Buttons.Y == ButtonState.Pressed)
				{
					Game1.pcManager.inputDevice = InputDevice.GamePad;
					if (this.AnimName.StartsWith("attack") || this.State == CharState.Air)
					{
						if (this.AnimName == "attack01" || this.AnimName == "airspin")
						{
							this.KeySecondary = true;
						}
						else if (Character.prevState.Buttons.Y == ButtonState.Released)
						{
							this.KeySecondary = true;
						}
					}
					else
					{
						this.KeySecondary = true;
					}
				}
				if (Character.curState.Buttons.B == ButtonState.Pressed && Character.prevState.Buttons.B == ButtonState.Released)
				{
					this.KeyThrow = true;
					if (Game1.isPCBuild)
					{
						Game1.pcManager.inputDevice = InputDevice.GamePad;
					}
				}
			}
			Character.prevState = Character.curState;
		}

		public void PlayerSpecialTrigger(Trigger trig, Vector2 loc, float rot, float scale, ParticleManager pMan)
		{
			if (trig == Trigger.Special0)
			{
				for (int i = 0; i < 2; i++)
				{
					float num = rot + Rand.GetRandomFloat(-1f, 1f);
					pMan.AddFeather(loc, new Vector2((float)Math.Cos(num), (float)Math.Sin(num)) * 200f, 1f, Rand.GetRandomFloat(0.5f, 2f), 5);
				}
			}
		}

		private void UpdateInput(Character[] c)
		{
			this.PressedKey = PressedKeys.None;
			this.Height = this.DefaultHeight;
			CharDir face = this.Face;
			if (this.GrabbedBy < 0)
			{
				if (this.State == CharState.Grounded && !this.KeyLeft && !this.KeyRight)
				{
					if (this.runTimer > 1f)
					{
						if (this.Friction < 6000f)
						{
							this.Friction += 100f;
						}
						else
						{
							this.runTimer = 0f;
						}
					}
					else
					{
						this.Friction = 6000f;
					}
				}
				if (this.AnimName == "hurtup")
				{
					this.Friction = 1000f;
					if (this.State == CharState.Grounded && this.FlyType == FlyingType.None)
					{
						this.SetJump(this.JumpVelocity, jumped: true);
					}
				}
				else if (this.AnimName.StartsWith("hurt"))
				{
					this.Friction = 3000f;
				}
				if (this.AnimName.StartsWith("idle") || (this.ID == 0 && this.AnimName == "land"))
				{
					if (this.KeyLeft && !this.KeyRight)
					{
						if (this.AnimName.StartsWith("idle"))
						{
							this.runTimer = 0f;
						}
						this.SetAnim("run", 0, 2);
						this.Trajectory.X = 0f - this.Speed;
						this.Face = CharDir.Left;
						this.Friction = 0f;
					}
					else if (this.KeyRight && !this.KeyLeft)
					{
						if (this.AnimName.StartsWith("idle"))
						{
							this.runTimer = 0f;
						}
						this.SetAnim("run", 0, 2);
						this.Trajectory.X = this.Speed;
						this.Face = CharDir.Right;
						this.Friction = 0f;
					}
					if (this.KeyAttack)
					{
						this.SetAnim("attack00", 0, 2);
					}
					if (this.KeySecondary && (this.ID != 0 || Game1.stats.upgrade[0] > 0))
					{
						this.SetAnim("attack01", 0, 2);
					}
					if (this.KeyThrow)
					{
						this.SetAnim("throw", 0, 2);
					}
					if (this.KeyDown)
					{
						this.SetAnim("crouch", 0, 2);
						this.Trajectory.X = 0f;
					}
					if (this.KeyJump)
					{
						if (!this.KeyDown && this.canJump)
						{
							this.SetJump(this.JumpVelocity, jumped: true);
						}
						else if (this.KeyDown && this.CanFallThrough)
						{
							this.FallOff(c, fallThrough: true);
						}
					}
				}
				else if (this.AnimName.StartsWith("run") || this.AnimName == "standup")
				{
					if (this.KeyLeft && !this.KeyRight)
					{
						if (this.AnimName == "runend" || this.AnimName == "standup")
						{
							this.runTimer = 0f;
							this.SetAnim("run", 0, 2);
						}
						this.Trajectory.X = 0f - this.Speed;
						this.Face = CharDir.Left;
						this.Friction = 0f;
					}
					else if (this.KeyRight && !this.KeyLeft)
					{
						if (this.AnimName == "runend" || this.AnimName == "standup")
						{
							this.runTimer = 0f;
							this.SetAnim("run", 0, 2);
						}
						this.Trajectory.X = this.Speed;
						this.Face = CharDir.Right;
						this.Friction = 0f;
					}
					else if (this.AnimName == "run")
					{
						this.SetAnim("runend", 0, 2);
						if (this.AnimName != "runend")
						{
							this.SetAnim("idle00", 0, 2);
						}
					}
					if (this.KeyAttack)
					{
						this.SetAnim("attack00", 0, 2);
					}
					if (this.KeySecondary && (this.ID != 0 || Game1.stats.upgrade[0] > 0))
					{
						this.SetAnim("attack01", 0, 2);
					}
					if (this.KeyThrow)
					{
						this.SetAnim("throw", 0, 2);
					}
					if (this.KeyDown)
					{
						this.SetAnim("crouch", 0, 2);
						this.Trajectory.X = 0f;
					}
					if (this.KeyJump)
					{
						if (!this.KeyDown && this.canJump)
						{
							this.SetJump(this.JumpVelocity, jumped: true);
						}
						else if (this.KeyDown && this.CanFallThrough)
						{
							this.FallOff(c, fallThrough: true);
						}
					}
				}
				else if (this.AnimName.StartsWith("crouch"))
				{
					this.Friction = 6000f;
					this.Height = (int)((float)this.DefaultHeight * 0.45f);
					if (this.AnimName != "crouchslide")
					{
						if (this.KeyRight)
						{
							if (this.Face == CharDir.Left)
							{
								Game1.camera.camOffset.X = -200f;
								this.Face = CharDir.Right;
								this.SetAnim("idle00", 0, 2);
								this.SetAnim("crouch", 2, 2);
							}
						}
						else if (this.KeyLeft && this.Face == CharDir.Right)
						{
							Game1.camera.camOffset.X = 200f;
							this.Face = CharDir.Left;
							this.SetAnim("idle00", 0, 2);
							this.SetAnim("crouch", 2, 2);
						}
						if (!this.KeyDown)
						{
							int num = Game1.map.CheckCol(this.Location - new Vector2(0f, 96f));
							if (num == 0)
							{
								this.SetAnim("standup", 0, 2);
							}
							else if (num > 2)
							{
								HitManager.CheckWallHazard(Game1.character, this.ID, Game1.pManager, 100, (ColType)num);
							}
						}
						if (this.KeyJump)
						{
							if (this.CanFallThrough)
							{
								this.FallOff(c, fallThrough: true);
							}
							else if (this.Trajectory.X == 0f && (this.ID != 0 || Game1.stats.upgrade[14] > 0))
							{
								this.SetAnim("crouchslide", 0, 0);
								this.Slide(2600f);
							}
						}
						if (this.KeyAttack && (this.AnimName == "crouch" || this.AnimFrame > 4))
						{
							this.SetAnim("idle00", 0, 2);
							this.SetAnim("crouchattack", 0, 2);
						}
						if (this.KeyEvade && Game1.map.CheckCol(this.Location - new Vector2(0f, 96f)) > 0)
						{
							this.KeyEvade = false;
						}
					}
				}
				else if (this.AnimName.StartsWith("crawl"))
				{
					if (this.KeyRight && !this.KeyLeft)
					{
						if (this.AnimName == "crawlidle")
						{
							this.runTimer = 0f;
							this.SetAnim("crawl", 0, 2);
						}
						this.Trajectory.X = 50f;
						this.Face = CharDir.Right;
						this.Friction = 0f;
					}
					else if (this.AnimName == "crawl")
					{
						this.SetAnim("crawlidle", 0, 2);
					}
				}
				else if (this.AnimName.StartsWith("jump"))
				{
					if (this.Trajectory.Y > 200f)
					{
						this.CanFallThrough = false;
					}
					if (this.KeyLeft)
					{
						this.Face = CharDir.Left;
						if (this.Trajectory.X >= 0f - this.Speed)
						{
							this.Trajectory.X = 0f - this.Speed;
							this.Friction = 0f;
						}
						else
						{
							this.Friction = 2000f;
						}
					}
					else if (this.KeyRight)
					{
						this.Face = CharDir.Right;
						if (this.Trajectory.X <= this.Speed)
						{
							this.Trajectory.X = this.Speed;
							this.Friction = 0f;
						}
						else
						{
							this.Friction = 4000f;
						}
					}
					else
					{
						this.Friction = 2000f;
					}
					if (this.KeyAttack)
					{
						this.SetAnim("attackair", 0, 2);
					}
					if (this.KeySecondary && this.InitDustSeek())
					{
						return;
					}
					if (this.KeyJump && this.canJump && this.ID == 0)
					{
						if (this.Boosting == 0 && Game1.stats.upgrade[17] > 0 && this.AnimName != "jumpboost")
						{
							c[0].SetAnim("jumpboost", 0, 0);
						}
						else if (Game1.stats.doubleJump < 1)
						{
							c[0].SetAnim("idle00", 0, 0);
							this.SetJump(this.JumpVelocity, jumped: true);
							Game1.stats.doubleJump++;
						}
					}
					if (this.KeyThrow)
					{
						this.SetAnim("throw", 0, 2);
					}
				}
				else if (this.AnimName == "hangjump")
				{
					if (this.KeyAttack || this.KeySecondary)
					{
						if (this.KeyLeft)
						{
							this.Face = CharDir.Left;
						}
						else if (this.KeyRight)
						{
							this.Face = CharDir.Right;
						}
					}
					if (this.KeyAttack)
					{
						this.SetAnim("attackair", 0, 2);
					}
					if (this.KeySecondary && this.InitDustSeek())
					{
						return;
					}
				}
				else if (this.AnimName == "airhike" || this.AnimName.StartsWith("attackair"))
				{
					if (this.Trajectory.Y > 200f)
					{
						this.CanFallThrough = false;
					}
					if (this.Holding)
					{
						this.Friction = 3000f;
					}
					if (this.AnimName == "airhike")
					{
						if (this.KeySecondary && this.AnimFrame > 3 && this.InitDustSeek())
						{
							return;
						}
					}
					else if (this.AnimName == "attackairdown")
					{
						this.Trajectory.X = 0f;
					}
					else if (this.KeyLeft)
					{
						this.Face = CharDir.Left;
						if (this.Trajectory.X >= 0f - this.Speed)
						{
							this.Trajectory.X = 0f - this.Speed;
							this.Friction = 0f;
						}
						else
						{
							this.Friction = 2000f;
						}
					}
					else if (this.KeyRight)
					{
						this.Face = CharDir.Right;
						if (this.Trajectory.X <= this.Speed)
						{
							this.Trajectory.X = this.Speed;
							this.Friction = 0f;
						}
						else
						{
							this.Friction = 2000f;
						}
					}
					else
					{
						this.Friction = 4000f;
					}
					if (this.KeyThrow)
					{
						this.SetAnim("throw", 0, 2);
					}
					if (this.KeyJump && this.canJump && this.ID == 0)
					{
						if (this.Boosting == 0 && Game1.stats.upgrade[17] > 0)
						{
							c[0].SetAnim("jumpboost", 0, 0);
						}
						else if (Game1.stats.doubleJump < 1)
						{
							c[0].SetAnim("idle00", 0, 0);
							this.SetJump(this.JumpVelocity, jumped: true);
							Game1.stats.doubleJump++;
						}
					}
					if (this.KeyDown && this.KeySecondary)
					{
						this.SetAnim("attackairdown", 0, 0);
					}
				}
				else if (this.AnimName == "airspin")
				{
					if (this.AnimFrame < 9 && this.KeySecondary)
					{
						bool flag = false;
						CharDir face2 = this.Face;
						if (this.KeyLeft)
						{
							this.Face = CharDir.Left;
						}
						else if (this.KeyRight)
						{
							this.Face = CharDir.Right;
						}
						if (this.Face != face2)
						{
							flag = true;
						}
						if (flag)
						{
							this.AnimFrame = 7;
						}
						if (this.AnimFrame > 6)
						{
							this.Seek(force: true);
							this.AnimFrame = (int)MathHelper.Clamp(this.AnimFrame - 6, 0f, this.AnimFrame);
						}
						if (this.Friction < 6000f)
						{
							this.Friction += 20f;
						}
					}
					if (this.WallInWay)
					{
						this.SetAnim("jump", 0, 2);
						if (!Game1.events.anyEvent)
						{
							this.Face = ((this.Face == CharDir.Left) ? CharDir.Right : CharDir.Left);
							this.Slide(1200f);
						}
					}
					if (this.KeyDown && this.KeySecondary)
					{
						this.SetAnim("attackairdown", 0, 0);
					}
				}
				else if (this.AnimName.StartsWith("attack"))
				{
					if (this.runTimer > 1f)
					{
						if (this.Friction < 6000f)
						{
							this.Friction += 100f;
						}
					}
					else
					{
						this.Friction += 150f;
					}
					if (this.ID == 0 && this.AnimName == "attack01")
					{
						if (Game1.stats.isSpinning)
						{
							if (this.AnimFrame > 7)
							{
								this.AnimFrame = (int)MathHelper.Clamp(this.AnimFrame - 3, 0f, this.AnimFrame);
								if (!this.KeySecondary)
								{
									Game1.stats.isSpinning = false;
								}
							}
						}
						else if (this.KeyAttack)
						{
							this.SetAnim("attack00", 0, 2);
						}
					}
					if (this.KeyDown)
					{
						this.SetAnim("crouch", 0, 2);
						this.Trajectory.X = 0f;
					}
					if (this.KeyJump)
					{
						if (!this.KeyDown && this.canJump)
						{
							this.SetJump(this.JumpVelocity, jumped: true);
						}
						else if (this.KeyDown && this.CanFallThrough)
						{
							this.FallOff(c, fallThrough: true);
						}
					}
					if (this.KeyThrow)
					{
						this.SetAnim("throw", 0, 2);
					}
				}
				else if (this.AnimName.StartsWith("throw"))
				{
					if (this.runTimer > 1f)
					{
						if (this.Friction < 6000f)
						{
							this.Friction += 100f;
						}
					}
					else
					{
						this.Friction += 150f;
					}
				}
				else if (this.AnimName.StartsWith("evade"))
				{
					if (this.State == CharState.Grounded)
					{
						this.Height = (int)((float)this.DefaultHeight * 0.45f);
						if (this.KeyDown && this.AnimFrame == 14)
						{
							this.SetAnim("crouch", 5, 2);
						}
					}
					else
					{
						this.Friction = 2000f;
						if (this.KeySecondary && this.AnimFrame > 6)
						{
							this.KeyDown = false;
							this.InitDustSeek();
						}
						if (this.Face == CharDir.Left)
						{
							this.Trajectory.X = Math.Max(200f, this.Trajectory.X);
						}
						else
						{
							this.Trajectory.X = Math.Min(this.Trajectory.X, -200f);
						}
					}
					if (this.KeyJump && this.canJump && this.ID == 0 && Game1.stats.doubleJump < 1)
					{
						this.SetJump(this.JumpVelocity, jumped: true);
						Game1.stats.doubleJump++;
					}
				}
				if (this.KeyEvade)
				{
					this.Evade(Game1.pManager);
				}
				else if (this.AnimName.StartsWith("hang"))
				{
					if (!this.AnimName.Contains("jump"))
					{
						if (this.KeyDown)
						{
							if (this.Trajectory.Y == 0f)
							{
								Sound.PlayPersistCue("vine_slide", new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f);
							}
							this.Trajectory.Y = MathHelper.Clamp(this.Trajectory.Y + this.et * 1000f, 0f, 2000f);
						}
						else
						{
							if (this.Trajectory.Y > 800f)
							{
								this.SetAnim("idle00", 0, 0);
								this.SetAnim("hangwall", 4, 0);
								this.InterpolateOff(0);
								Sound.PlayCue("vine_grab", this.Location, (this.Location - Game1.character[0].Location).Length());
							}
							if (this.Trajectory.Y > 0f)
							{
								Sound.StopPersistCue("vine_slide");
							}
							this.Trajectory.Y = 0f;
						}
						this.Friction = 0f;
						this.Hanging = true;
						if (Game1.map.CheckCol(this.Location + new Vector2(64 * ((this.Face != 0) ? 1 : (-1)), 0f)) != 2)
						{
							this.SetAnim("jump", 16, 0);
						}
						else if (this.KeyJump)
						{
							bool flag2 = false;
							if ((this.Face == CharDir.Left && this.KeyRight) || (this.Face == CharDir.Right && this.KeyLeft) || this.KeyDown)
							{
								if (!this.KeyRight && !this.KeyLeft)
								{
									this.SetAnim("jump", 16, 0);
									flag2 = true;
								}
								else
								{
									Vector2 vector = Vector2.Zero;
									if (Character.curState.ThumbSticks.Left.Length() > 0.1f)
									{
										vector = Character.curState.ThumbSticks.Left * new Vector2(this.Speed * 4f, this.JumpVelocity * 1f);
									}
									else
									{
										vector.X = ((this.KeyLeft ? (0f - this.Speed) : 0f) + (this.KeyRight ? this.Speed : 0f)) * 2f;
										vector.Y = (this.KeyUp ? this.JumpVelocity : 0f) + (this.KeyDown ? (0f - this.JumpVelocity) : 0f);
									}
									this.Slide(0f - Math.Min(Math.Abs(vector.X), this.Speed * 2.5f));
									if (vector.Y < 0f)
									{
										vector.Y *= 1.5f;
									}
									this.SetJump(vector.Y, jumped: true);
								}
							}
							else
							{
								this.SetJump(this.JumpVelocity, jumped: true);
								this.Slide(-300f);
							}
							if (!flag2)
							{
								this.SetAnim("hangjump", 0, 0);
							}
						}
						if (this.AnimName != "hangwall")
						{
							Sound.StopPersistCue("vine_slide");
						}
					}
					else
					{
						this.Friction = 2000f;
						if ((this.Face == CharDir.Left && this.KeyRight) || (this.Face == CharDir.Right && this.KeyLeft))
						{
							this.Friction = 0f;
						}
						if (this.KeyJump && this.canJump && this.ID == 0 && Game1.stats.doubleJump < 1)
						{
							this.SetJump(this.JumpVelocity, jumped: true);
							Game1.stats.doubleJump++;
						}
					}
				}
				if (this.FlyType != 0 && this.AnimName.StartsWith("fly"))
				{
					if (this.FlyType == FlyingType.CanFlySwaying)
					{
						this.flyingSpeed += Game1.FrameTime / 1f;
						if (this.flyingSpeed > 6.28f)
						{
							this.flyingSpeed -= 6.28f;
						}
						this.flyingOffset.X = (float)Math.Cos((double)this.flyingSpeed * 2.0) * (this.Speed / 200f);
						this.flyingOffset.Y = (0f - (float)Math.Sin((double)this.flyingSpeed * 4.0)) * (this.Speed / 1600f);
						this.flyingOffset *= 30f;
					}
					if (this.KeyLeft && !this.KeyRight)
					{
						if (this.Trajectory.X > 0f - this.Speed)
						{
							this.Trajectory.X -= Game1.FrameTime * 750f;
							if (this.Trajectory.X < 0f - this.Speed)
							{
								this.Trajectory.X = 0f - this.Speed;
							}
						}
						if (this.Ai == null || this.Ai.jobType != JobType.FlyKeepDistance)
						{
							this.Face = CharDir.Left;
						}
						this.Friction = 0f;
					}
					else if (this.KeyRight && !this.KeyLeft)
					{
						if (this.Trajectory.X < this.Speed)
						{
							this.Trajectory.X += Game1.FrameTime * 750f;
							if (this.Trajectory.X > this.Speed)
							{
								this.Trajectory.X = this.Speed;
							}
						}
						if (this.Ai == null || this.Ai.jobType != JobType.FlyKeepDistance)
						{
							this.Face = CharDir.Right;
						}
						this.Friction = 0f;
					}
					else
					{
						this.Friction = 2000f;
					}
					if (this.KeyUp)
					{
						this.Friction = 0f;
						if (this.Trajectory.Y > 0f - this.Speed)
						{
							this.Trajectory.Y -= Game1.FrameTime * 1500f;
							if (this.Trajectory.Y < 0f - this.Speed)
							{
								this.Trajectory.Y = 0f - this.Speed;
							}
						}
					}
					else if (this.KeyDown)
					{
						this.Friction = 0f;
						if (this.Trajectory.Y < this.Speed)
						{
							this.Trajectory.Y += Game1.FrameTime * 1500f;
							if (this.Trajectory.Y > this.Speed)
							{
								this.Trajectory.Y = this.Speed;
							}
						}
					}
					if (this.KeyAttack)
					{
						this.SetAnim("flyattack", 0, 2);
						if (Rand.GetRandomInt(0, 2) == 0)
						{
							this.SetAnim("flyattacksec", 0, 2);
						}
					}
					if (this.KeySecondary)
					{
						this.SetAnim("flyattacksec", 0, 2);
					}
					if (this.KeyThrow)
					{
						this.SetAnim("throw", 0, 2);
					}
				}
				if (this.CanCancel)
				{
					if (this.State == CharState.Air)
					{
						if (this.Trajectory.Y > 200f)
						{
							this.CanFallThrough = false;
						}
						if (this.KeyLeft)
						{
							if (this.Trajectory.X >= 0f - this.Speed)
							{
								this.Trajectory.X = 0f - this.Speed;
								this.Friction = 0f;
							}
							else
							{
								this.Friction = 2000f;
							}
						}
						else if (this.KeyRight)
						{
							if (this.Trajectory.X <= this.Speed)
							{
								this.Trajectory.X = this.Speed;
								this.Friction = 0f;
							}
							else
							{
								this.Friction = 2000f;
							}
						}
						else
						{
							this.Friction = 2000f;
						}
						if (this.KeyAttack)
						{
							if (this.KeyLeft)
							{
								this.Face = CharDir.Left;
							}
							else if (this.KeyRight)
							{
								this.Face = CharDir.Right;
							}
							this.SetAnim("attackair", 0, 2);
						}
						if (this.KeySecondary)
						{
							if (this.KeyLeft)
							{
								this.Face = CharDir.Left;
							}
							else if (this.KeyRight)
							{
								this.Face = CharDir.Right;
							}
							if (!this.InitDustSeek())
							{
								this.SetAnim("attackairdown", 0, 2);
							}
						}
						if (this.KeyThrow)
						{
							this.SetAnim("throw", 0, 2);
						}
					}
					else if (this.State == CharState.Grounded)
					{
						if (this.KeyLeft && !this.KeyRight)
						{
							this.runTimer = 0f;
							this.SetAnim("run", 0, 2);
							this.Trajectory.X = 0f - this.Speed;
							this.Face = CharDir.Left;
							this.Friction = 0f;
						}
						else if (this.KeyRight && !this.KeyLeft)
						{
							this.runTimer = 0f;
							this.SetAnim("run", 0, 2);
							this.Trajectory.X = this.Speed;
							this.Face = CharDir.Right;
							this.Friction = 0f;
						}
						if (this.KeyAttack)
						{
							this.SetAnim("attack00", 0, 2);
						}
						if (this.KeySecondary && (this.ID != 0 || Game1.stats.upgrade[0] > 0))
						{
							this.SetAnim("attack01", 0, 2);
						}
						if (this.KeyThrow)
						{
							this.SetAnim("throw", 0, 2);
						}
						if (this.KeyDown)
						{
							this.SetAnim("crouch", 0, 2);
							this.Trajectory.X = 0f;
						}
						if (this.KeyJump)
						{
							if (!this.KeyDown && this.canJump)
							{
								this.SetJump(this.JumpVelocity, jumped: true);
							}
							else if (this.KeyDown && this.CanFallThrough)
							{
								this.FallOff(c, fallThrough: true);
							}
						}
					}
				}
				if (this.KeyAttack)
				{
					this.PressedKey = PressedKeys.Attack;
					if (this.QueueGoal[1] > -1)
					{
						this.QueuedKey = QueuedKey.Attack;
					}
				}
				if (this.KeySecondary)
				{
					this.PressedKey = PressedKeys.Secondary;
					if (this.QueueGoal[2] > -1)
					{
						this.QueuedKey = QueuedKey.Secondary;
					}
				}
				if (this.ID == 0 && this.QueueTrig > -1 && Game1.stats.gameDifficulty > 0 && !Game1.settings.AutoCombo)
				{
					this.SetFrame(this.QueueTrig);
					this.QueuedKey = QueuedKey.None;
				}
				else if (this.PressedKey > PressedKeys.None)
				{
					if (this.GotoGoal[(int)this.PressedKey] == 1000)
					{
						Game1.stats.isSpinning = true;
						this.PressedKey = PressedKeys.None;
						this.QueuedKey = QueuedKey.None;
					}
					else if (this.GotoGoal[(int)this.PressedKey] > -1)
					{
						this.SetFrame(this.GotoGoal[(int)this.PressedKey]);
						if (this.KeyLeft)
						{
							this.Face = CharDir.Left;
						}
						else if (this.KeyRight)
						{
							this.Face = CharDir.Right;
						}
					}
				}
				this.QueueTrig = -1;
			}
			if (face != this.Face && this.FlyType > FlyingType.None)
			{
				this.CharRotation = (this.CharRotationDest = 3.14f);
			}
		}

		public void UpdateThread(Character[] c, float frameTime)
		{
			this.UpdateHP();
			this.UpdateDeath();
			if (this.StatusTime > 0f || this.StatusEffect != 0)
			{
				this.UpdateStatus(Game1.pManager);
			}
			if (this.Location.X < Game1.map.leftEdge + 200f || this.Location.X > Game1.map.rightEdge - 200f)
			{
				this.CharCol = 0f;
			}
			else
			{
				this.UpdateCharCol(c, frameTime);
			}
		}

		public void Update(Map map, Character[] c, float frameTime, DestructableManager dManager)
		{
			this.renderable = this.CheckRenderable();
			if (this.Updatable(2000))
			{
				dManager.UpdateMovingLedges(this);
				this.runTimer += frameTime;
				this.et = frameTime;
				this.PLoc = this.Location;
				if (this.Ai != null && Game1.skipFrame == 1)
				{
					this.Ai.Update(c, this.ID, map);
				}
				if (this.DownTime > 0f)
				{
					this.UpdateDowned();
				}
				this.UpdateAnimation(c);
				this.UpdateInput(c);
				this.UpdateRegions(map);
				this.UpdateLocation(map, c);
				if (this.Ai == null)
				{
					this.MapCollision(map, c);
				}
				else
				{
					this.Ai.MapCollision(map, c, this);
				}
				if (this.renderable)
				{
					this.UpdateRotation();
				}
			}
			if (this.DyingFrame > 1f)
			{
				this.SetFlag();
				this.Exists = CharExists.Dead;
				this.Name = string.Empty;
			}
			this.glowing = ((this.CanHurtFrame + this.CanHurtProjectileFrame > 0f) ? 0.8f : 0f) + ((this.DownTime > 0f && Game1.halfSecFrame % 10 == 0) ? 0.5f : 0f);
			if (this.glowing > 0f)
			{
				Game1.characterGlow = Math.Max(this.ID + 1, Game1.characterGlow);
			}
			if (this.refracting)
			{
				Game1.maxRefractingChars = Math.Max(this.ID + 1, Game1.maxRefractingChars);
			}
		}

		public virtual void Draw(SpriteBatch spriteBatch, Texture2D[] particleTex, bool specialMode)
		{
			if (!this.renderable)
			{
				return;
			}
			if (!specialMode)
			{
				if (this.State == CharState.Grounded)
				{
					if (this.shadowAlpha < 0.5f)
					{
						this.shadowAlpha += this.et;
						if (this.shadowAlpha > 0.5f)
						{
							this.shadowAlpha = 0.5f;
						}
					}
					spriteBatch.Draw(color: (this.DyingFrame == -1f) ? (Color.Black * this.shadowAlpha) : (Color.Black * (1f - this.DyingFrame) * this.shadowAlpha), texture: particleTex[1], position: this.Location * Game1.worldScale - Game1.Scroll, sourceRectangle: new Rectangle(1408, 128, 236, 32), rotation: this.CharRotation, origin: new Vector2(118f, 17f), scale: new Vector2(this.ShadowWidth, 1f) * Game1.worldScale, effects: SpriteEffects.None, layerDepth: 0f);
				}
				else
				{
					this.shadowAlpha = 0f;
				}
				this.et = 0f;
			}
			this.DrawCharacter(spriteBatch, this.prevFrame, specialMode);
		}

		private void DrawCharacter(SpriteBatch spriteBatch, Frame frame, bool specialMode)
		{
			float num = this.CharRotation - 3.14f;
			if (this.Face == CharDir.Left)
			{
				num = 0f - num;
			}
			float num2 = this.footOffset;
			if (this.FlyType > FlyingType.None && this.State == CharState.Air)
			{
				num2 = 65f;
			}
			float num3 = 0f;
			Vector2 vector = Vector2.Zero;
			Vector2 vector2 = Vector2.Zero;
			Animation animation = this.Definition.Animations[this.Anim];
			Frame frame2 = this.Definition.Frames[this.nextKeyFrame.FrameRef];
			if (frame == null || frame2 == null)
			{
				return;
			}
			int num4 = ((this.Definition.charType == CharacterType.Dust) ? (this.textureRange[2] + (this.textureRange[3] - this.textureRange[2]) / 2) : this.textureRange[1]);
			float num5 = this.prevDuration * (this.curFrame / this.prevDuration) / this.prevDuration;
			Color color = (specialMode ? (Color.White * this.glowing) : this.GetColor());
			for (int i = 0; i < frame.maxRender; i++)
			{
				Part part = frame.Parts[i];
				int index = part.Index;
				if (index <= -1)
				{
					continue;
				}
				if (index < 10000)
				{
					if ((Game1.refractive && (!Game1.refractive || index < num4)) || this.partsSnap[i] == null)
					{
						continue;
					}
					if (this.partsSnap[i].texture != null && !this.partsSnap[i].texture.IsDisposed)
					{
						if (animation.interpolate && this.prevDuration > 0f)
						{
							Part part2 = frame2.Parts[i];
							if (this.CharRotation != 3.14f)
							{
								float num6 = part.Location.Y - num2;
								Vector2 vector3 = new Vector2((float)(Math.Cos(num) * (double)part.Location.X - Math.Sin(num) * (double)num6), (float)(Math.Cos(num) * (double)num6 + Math.Sin(num) * (double)part.Location.X));
								float num7 = part2.Location.Y - num2;
								Vector2 vector4 = new Vector2((float)(Math.Cos(num) * (double)part2.Location.X - Math.Sin(num) * (double)num7), (float)(Math.Cos(num) * (double)num7 + Math.Sin(num) * (double)part2.Location.X));
								vector = (vector4 - vector3) * num5 + vector3;
								num3 = (part2.Rotation - part.Rotation) * num5 + part.Rotation + num;
								vector2 = (part2.Scaling - part.Scaling) * num5 + part.Scaling;
							}
							else
							{
								vector = (part2.Location - part.Location) * num5 + part.Location - new Vector2(0f, num2);
								num3 = (part2.Rotation - part.Rotation) * num5 + part.Rotation;
								vector2 = (part2.Scaling - part.Scaling) * num5 + part.Scaling;
							}
						}
						else
						{
							if (this.CharRotation != 3.14f)
							{
								float num8 = part.Location.Y - num2;
								Vector2 vector5 = new Vector2((float)(Math.Cos(num) * (double)part.Location.X - Math.Sin(num) * (double)num8), (float)(Math.Cos(num) * (double)num8 + Math.Sin(num) * (double)part.Location.X));
								vector = vector5;
								num3 = part.Rotation + num;
							}
							else
							{
								vector = part.Location - new Vector2(0f, num2);
								num3 = part.Rotation;
							}
							vector2 = part.Scaling;
						}
						if (this.Face == CharDir.Left)
						{
							num3 = 0f - num3;
							vector.X = 0f - vector.X;
						}
						vector += this.Location;
						bool flag = false;
						if ((this.Face == CharDir.Right && part.Flip == 0) || (this.Face == CharDir.Left && part.Flip == 1))
						{
							flag = true;
						}
						Rectangle sRect = this.partsSnap[i].sRect;
						float y = 0f;
						if (num2 != this.footOffset)
						{
							y = num2;
						}
						Vector2 position = (vector - new Vector2(0f, y)) * Game1.worldScale - Game1.Scroll;
						if (index < num4)
						{
							spriteBatch.Draw(this.partsSnap[i].texture, position, sRect, color, num3, new Vector2(sRect.Width, sRect.Height) / 2f, vector2 * Game1.worldScale, (!flag) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
						}
						else if (this.partsSnap[i].texture != null)
						{
							spriteBatch.Draw(this.partsSnap[i].texture, position, sRect, new Color((int)this.defaultColor.R, (int)this.defaultColor.G, (int)this.defaultColor.B, (float)(int)this.defaultColor.A / 255f * 0.25f), num3, new Vector2((float)sRect.Width / 2f, (float)sRect.Height / 2f), vector2 * Game1.worldScale, (!flag) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
						}
						else if (Game1.longSkipFrame > 3 || Game1.map.GetTransVal() > 0f)
						{
							this.InitTextures(frame);
						}
					}
					else if (Game1.longSkipFrame > 3 || Game1.map.GetTransVal() > 0f)
					{
						this.InitTextures(frame);
					}
				}
				else if (index < 10002)
				{
					this.CheckAnimTrig(num, num2, num3, vector, vector2.X, animation, i, part);
				}
			}
		}
	}
}
