using System;
using Dust.ai;
using Dust.Audio;
using Dust.CharClasses;
using Dust.Dialogue;
using Dust.HUD;
using Dust.MapClasses;
using Dust.Particles;
using Dust.Vibration;
using Microsoft.Xna.Framework;

namespace Dust
{
	public class EventManager
	{
		public EventType eventType;

		public int currentEvent = -1000;

		public bool[] sideEventAvailable;

		public int curSideEvent = -1;

		public int curRepeatEvent = -1;

		public int subEvent;

		public bool anyEvent;

		public Vector2 eventCamera = Vector2.Zero;

		public int regionIntroStage;

		public float regionIntroTimer;

		public float regionIntroFade;

		public Vector2[] regionIntroTargets = new Vector2[5];

		public float[] regionIntroZooms = new float[6];

		public SkipType skippable;

		public bool safetyOn;

		public bool blurOn = true;

		public float eventTimer;

		public float fadeTimer;

		public float fadeLength;

		public Color screenFade;

		private static Vector2 hotSpot;

		private static int hotID = 1;

		private static bool readyPlayer;

		public string regionDisplayName = string.Empty;

		public bool[] regionIntroduced;

		private Character[] character;

		private ParticleManager pMan;

		private Map map;

		public EventManager(Character[] _character, Map _map)
		{
			this.character = _character;
			this.map = _map;
			this.sideEventAvailable = new bool[150];
			for (int i = 0; i < this.sideEventAvailable.Length; i++)
			{
				this.sideEventAvailable[i] = true;
			}
			this.regionIntroduced = new bool[20];
			for (int j = 0; j < this.regionIntroduced.Length; j++)
			{
				this.regionIntroduced[j] = false;
			}
		}

		public void SetPMan(ParticleManager _pManager)
		{
			this.pMan = _pManager;
		}

		public void Reset()
		{
			this.currentEvent = -1000;
			this.curSideEvent = -1;
			for (int i = 0; i < this.sideEventAvailable.Length; i++)
			{
				this.sideEventAvailable[i] = true;
			}
			for (int j = 0; j < this.regionIntroduced.Length; j++)
			{
				this.regionIntroduced[j] = false;
			}
			for (int k = 0; k < this.map.eventRegion.Count; k++)
			{
				this.map.eventRegion[k].Reset();
			}
			this.ClearEvent();
			this.skippable = SkipType.Skippable;
			this.eventType = EventType.None;
			this.anyEvent = false;
			this.fadeTimer = (this.fadeLength = 0f);
			this.screenFade = new Color(0, 0, 0, 0);
		}

		private void StartDialogue(int conversation, CharacterType charType)
		{
			Game1.hud.InitDialogue(conversation, charType);
		}

		private bool DialogueOver()
		{
			return Game1.hud.dialogueState == DialogueState.Inactive;
		}

		public void ClearEvent()
		{
			Character[] array = this.character;
			if ((this.currentEvent == 0 || this.curSideEvent == 87) && this.subEvent < 16)
			{
				if (array[0].AnimName == "intrograb")
				{
					array[0].SetAnim("standup", 0, 2);
				}
				this.pMan.AddFidget(new Vector2(array[0].Location.X + 800f, array[0].Location.Y - 200f));
			}
			if (this.currentEvent == 0)
			{
				Game1.awardsManager.EarnAvatarAward(new string[1] { "AvatarAwards1" });
			}
			this.eventType = EventType.None;
			this.anyEvent = false;
			this.curSideEvent = -1;
			this.curRepeatEvent = -1;
			this.subEvent = 0;
			this.skippable = SkipType.Skippable;
			this.SetEventCamera(Vector2.Zero, snapToLocation: false);
			if (Game1.hud.dialogueState == DialogueState.Active)
			{
				Game1.hud.dialogue.InitExit();
			}
			Game1.hud.canInput = false;
			Game1.hud.fidgetPrompt = FidgetPrompt.None;
			Sound.DimSFXVolume(1f);
			this.fadeLength = (this.fadeTimer = (this.eventTimer = 0f));
			this.screenFade = new Color(0f, 0f, 0f, 0f);
			if (this.currentEvent < 20)
			{
				this.pMan.RemoveParticle(new DustSword(Vector2.Zero, Vector2.Zero));
			}
			this.safetyOn = false;
			EventManager.readyPlayer = false;
			this.PrepareCharacters(320);
			Game1.stats.GetWorldExplored();
		}

		public void PrepareCharacters(int questDescWidth)
		{
			if (Game1.events.eventType != 0)
			{
				return;
			}
			this.blurOn = true;
			Game1.questManager.UpdateQuests(questDescWidth);
			for (int i = 0; i < this.character.Length; i++)
			{
				if (this.character[i].Exists == CharExists.Exists)
				{
					this.character[i].Ethereal = EtherealState.Normal;
					if (this.character[i].Ai != null)
					{
						this.character[i].Ai.overrideAI = false;
					}
				}
			}
		}

		private void ReadyPlayer()
		{
			Character[] array = this.character;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Ethereal = EtherealState.Ethereal;
			}
			if (array[0].Trajectory.X > 0f)
			{
				array[0].Face = CharDir.Right;
			}
			else if (array[0].Trajectory.X < 0f)
			{
				array[0].Face = CharDir.Left;
			}
			array[0].Trajectory.X /= 2f;
			if (array[0].AnimName.StartsWith("attack") && array[0].State == CharState.Grounded)
			{
				array[0].SetAnim("runend", 0, 2);
			}
			Game1.stats.fidgetAwayTime = Math.Min(Game1.stats.fidgetAwayTime, 1f);
			EventManager.readyPlayer = false;
		}

		public void ReturnToTitle()
		{
			int currentGamePad = Game1.currentGamePad;
			Game1.menu.QuitGame(this.pMan);
			Game1.currentGamePad = currentGamePad;
		}

		private void SpawnIDCharacter(Vector2 loc, int ID, string name, CharacterType type, Team team, bool ground)
		{
			this.character[ID].Exists = CharExists.Dead;
			this.character[ID].NewCharacter(loc, Game1.charDef[(int)type], ID, name, team, ground);
			_ = this.character[ID].Exists;
			_ = 2;
			this.character[ID].Ethereal = EtherealState.Ethereal;
			if ((this.character[ID].NPC == NPCType.Friendly || this.character[ID].Team == Team.Friendly) && this.character[ID].Ai != null)
			{
				this.character[ID].Ai.overrideAI = true;
			}
			if (this.character[ID].Location.X > this.character[0].Location.X)
			{
				this.character[ID].Face = CharDir.Left;
			}
			else
			{
				this.character[ID].Face = CharDir.Right;
			}
		}

		public int SpawnCharacter(Vector2 loc, string name, CharacterType type, Team team, bool ground)
		{
			for (int i = 0; i < this.character.Length; i++)
			{
				if (this.character[i].Exists == CharExists.Dead)
				{
					this.character[i].NewCharacter(loc, Game1.charDef[(int)type], i, name, team, ground);
					_ = this.character[i].Exists;
					_ = 2;
					if (this.character[i].NPC == NPCType.Friendly && this.character[i].Ai != null)
					{
						this.character[i].Ai.overrideAI = true;
					}
					if (this.character[i].Location.X > this.character[0].Location.X)
					{
						this.character[i].Face = CharDir.Left;
					}
					else
					{
						this.character[i].Face = CharDir.Right;
					}
					return i;
				}
			}
			return -1;
		}

		public int SpawnCharacterAppend(Vector2 loc, string name, CharacterType type, Team team, bool ground)
		{
			for (int num = this.character.Length - 1; num > 0; num--)
			{
				if (this.character[num].Exists == CharExists.Dead)
				{
					this.character[num].NewCharacter(loc, Game1.charDef[(int)type], num, name, team, ground);
					if (this.character[num].NPC == NPCType.Friendly && this.character[num].Ai != null)
					{
						this.character[num].Ai.overrideAI = true;
					}
					if (this.character[num].Location.X > this.character[0].Location.X)
					{
						this.character[num].Face = CharDir.Left;
					}
					else
					{
						this.character[num].Face = CharDir.Right;
					}
					return num;
				}
			}
			return -1;
		}

		private void RelocateDust(Vector2 newLoc)
		{
			this.character[0].SetJump(0f, jumped: false);
			this.character[0].Location = (this.character[0].PLoc = newLoc);
			this.character[0].Trajectory = Vector2.Zero;
			this.character[0].GroundCharacter();
			this.character[0].PLoc = (Game1.stats.lastSafeLoc = this.character[0].Location);
			this.character[0].SetAnim("idle01", 0, 2);
			this.pMan.ResetFidget(this.character);
			Game1.camera.ResetCamera(this.character);
			Game1.navManager.ForceScroll(Game1.navManager.navScale, Game1.navManager.playerX, Game1.navManager.playerY);
		}

		private void RelocateCharacter(CharacterType type, Vector2 newLoc)
		{
			Character character = this.GetCharacter(type);
			if (character != null)
			{
				character.SetJump(0f, jumped: false);
				character.Location = (character.PLoc = newLoc);
				character.Trajectory = Vector2.Zero;
				character.GroundCharacter();
				character.SetAnim("idle00", 0, 2);
				if (character.Ai != null)
				{
					character.Ai.overrideAI = true;
					character.Ai.initPos = newLoc.X;
				}
			}
		}

		private void RemoveCharacter(CharacterType type)
		{
			Character character = this.GetCharacter(type);
			if (character != null)
			{
				character.Exists = CharExists.Dead;
			}
		}

		public Character GetCharacter(CharacterType type)
		{
			for (int i = 0; i < this.character.Length; i++)
			{
				if (this.character[i].Exists == CharExists.Exists && this.character[i].Definition.charType == type)
				{
					return this.character[i];
				}
			}
			return null;
		}

		public Character GetCharacter(string characterName)
		{
			for (int i = 0; i < this.character.Length; i++)
			{
				if (this.character[i].Exists == CharExists.Exists && this.character[i].Name == characterName)
				{
					return this.character[i];
				}
			}
			return null;
		}

		public void FaceCharacter(CharacterType ownerCharType, CharacterType destCharType, bool faceTowards)
		{
			Character character = this.GetCharacter(ownerCharType);
			Character character2 = this.GetCharacter(destCharType);
			if (character == null || character2 == null)
			{
				return;
			}
			if (character.AnimName.StartsWith("attack") && character.State == CharState.Grounded)
			{
				character.SetAnim("runend", 0, 2);
			}
			character.Trajectory.X = 0f;
			character.KeyLeft = (character.KeyRight = false);
			CharDir face = character.Face;
			if (character.Location.X < character2.Location.X)
			{
				character.Face = CharDir.Right;
			}
			else
			{
				character.Face = CharDir.Left;
			}
			if (!faceTowards)
			{
				if (character.Face == CharDir.Left)
				{
					character.Face = CharDir.Right;
				}
				else
				{
					character.Face = CharDir.Left;
				}
			}
			if (character.Face != face)
			{
				if (ownerCharType == CharacterType.Dust)
				{
					Game1.camera.camOffset.X = 100 * ((character.Face != CharDir.Right) ? 1 : (-1));
				}
				character.SetAnim("idle00", 0, 0);
				character.SetAnim("runend", 0, 2);
			}
			if (character.Ai != null)
			{
				character.Ai.overrideAI = true;
			}
		}

		public void FaceCharacter(string characterName, CharacterType destCharType, bool faceTowards)
		{
			Character character = this.GetCharacter(characterName);
			Character character2 = this.GetCharacter(destCharType);
			if (character == null || character2 == null)
			{
				return;
			}
			if (character.AnimName.StartsWith("attack") && character.State == CharState.Grounded)
			{
				character.SetAnim("runend", 0, 2);
			}
			if (character.State != 0)
			{
				return;
			}
			character.Trajectory.X = 0f;
			character.KeyLeft = (character.KeyRight = false);
			CharDir face = character.Face;
			if (character.Location.X < character2.Location.X)
			{
				character.Face = CharDir.Right;
			}
			else
			{
				character.Face = CharDir.Left;
			}
			if (!faceTowards)
			{
				if (character.Face == CharDir.Left)
				{
					character.Face = CharDir.Right;
				}
				else
				{
					character.Face = CharDir.Left;
				}
			}
			if (character.Face != face)
			{
				character.SetAnim("idle00", 0, 0);
				character.SetAnim("runend", 0, 2);
			}
			if (character.Ai != null)
			{
				character.Ai.overrideAI = true;
			}
		}

		public void FaceLoc(CharacterType ownerCharType, Vector2 loc, bool faceTowards)
		{
			Character character = this.GetCharacter(ownerCharType);
			if (character == null)
			{
				return;
			}
			character.Trajectory.X = 0f;
			character.KeyLeft = (character.KeyRight = false);
			CharDir face = character.Face;
			if (character.Location.X > loc.X)
			{
				character.Face = CharDir.Left;
			}
			else
			{
				character.Face = CharDir.Right;
			}
			if (!faceTowards)
			{
				if (character.Face == CharDir.Left)
				{
					character.Face = CharDir.Right;
				}
				else
				{
					character.Face = CharDir.Left;
				}
			}
			if (character.Face != face)
			{
				if (ownerCharType == CharacterType.Dust)
				{
					Game1.camera.camOffset.X = 100 * ((character.Face != CharDir.Right) ? 1 : (-1));
				}
				character.SetAnim("idle00", 0, 0);
				character.SetAnim("runend", 0, 2);
			}
		}

		private bool ApproachLoc(CharacterType type, Vector2 dest, int bufferDistance, string stopAnim)
		{
			bufferDistance = Math.Max(bufferDistance, 10);
			Character character = this.GetCharacter(type);
			if (character != null)
			{
				if (character.Ai != null)
				{
					int num = 0;
					if (character.Location.X < dest.X + (float)bufferDistance - 20f && character.Location.X > dest.X - (float)bufferDistance + 20f && character.Location.X > this.map.leftBlock + (float)bufferDistance && character.Location.X < this.map.rightBlock - (float)bufferDistance)
					{
						num = ((!(character.Location.X < dest.X)) ? 1 : (-1));
					}
					if (character.FlyType != 0)
					{
						if (character.Location.Y < dest.Y - (float)bufferDistance)
						{
							character.KeyDown = true;
						}
						else if (character.Location.Y > dest.Y + (float)bufferDistance)
						{
							character.KeyUp = true;
						}
					}
					if (character.Location.X > dest.X + (float)bufferDistance || num < 0)
					{
						character.Ai.jobType = JobType.RunLeft;
						if (character.NPC == NPCType.None)
						{
							character.Ai.overrideAI = true;
						}
						else
						{
							character.Ai.overrideAI = false;
						}
					}
					else
					{
						if (!(character.Location.X < dest.X - (float)bufferDistance) && num <= 0)
						{
							character.Ai.overrideAI = true;
							if (character.NPC == NPCType.None)
							{
								character.Ai.jobType = JobType.Still;
							}
							if (character.Location.X > dest.X)
							{
								character.Face = CharDir.Left;
							}
							else
							{
								character.Face = CharDir.Right;
							}
							if (stopAnim != null && character.AnimName != stopAnim)
							{
								character.SetAnim(stopAnim, 0, 2);
							}
							character.Trajectory.X = 0f;
							return true;
						}
						character.Ai.jobType = JobType.RunRight;
						if (character.NPC == NPCType.None)
						{
							character.Ai.overrideAI = true;
						}
						else
						{
							character.Ai.overrideAI = false;
						}
					}
				}
				else if (character.Location.X > dest.X + (float)bufferDistance)
				{
					character.KeyLeft = true;
				}
				else
				{
					if (!(character.Location.X < dest.X - (float)bufferDistance))
					{
						if (stopAnim != null && character.AnimName != stopAnim)
						{
							character.SetAnim(stopAnim, 0, 2);
						}
						character.Trajectory.X = 0f;
						return true;
					}
					character.KeyRight = true;
				}
			}
			return false;
		}

		private bool ApproachLoc(string characterName, Vector2 dest, int bufferDistance, string stopAnim)
		{
			bufferDistance = Math.Max(bufferDistance, 10);
			Character character = this.GetCharacter(characterName);
			if (character != null)
			{
				if (character.Ai != null)
				{
					int num = 0;
					if (character.Location.X < dest.X + (float)bufferDistance - 20f && character.Location.X > dest.X - (float)bufferDistance + 20f)
					{
						num = ((!(character.Location.X < dest.X)) ? 1 : (-1));
					}
					if (character.Location.X > dest.X + (float)bufferDistance || num < 0)
					{
						character.Ai.jobType = JobType.RunLeft;
						if (character.NPC == NPCType.None)
						{
							character.Ai.overrideAI = true;
						}
						else
						{
							character.Ai.overrideAI = false;
						}
					}
					else
					{
						if (!(character.Location.X < dest.X - (float)bufferDistance) && num <= 0)
						{
							character.Ai.overrideAI = true;
							if (character.NPC == NPCType.None)
							{
								character.Ai.jobType = JobType.Still;
							}
							if (character.Location.X > dest.X)
							{
								character.Face = CharDir.Left;
							}
							else
							{
								character.Face = CharDir.Right;
							}
							if (stopAnim != null && character.AnimName != stopAnim)
							{
								character.SetAnim(stopAnim, 0, 2);
							}
							character.Trajectory.X = 0f;
							return true;
						}
						character.Ai.jobType = JobType.RunRight;
						if (character.NPC == NPCType.None)
						{
							character.Ai.overrideAI = true;
						}
						else
						{
							character.Ai.overrideAI = false;
						}
					}
				}
				else if (character.Location.X > dest.X + (float)bufferDistance)
				{
					character.KeyLeft = true;
				}
				else
				{
					if (!(character.Location.X < dest.X - (float)bufferDistance))
					{
						if (stopAnim != null && character.AnimName != stopAnim)
						{
							character.SetAnim(stopAnim, 0, 2);
						}
						character.Trajectory.X = 0f;
						return true;
					}
					character.KeyRight = true;
				}
			}
			return false;
		}

		public void SetEventCamera(Vector2 loc, bool snapToLocation)
		{
			if (loc == Vector2.Zero)
			{
				this.eventCamera = loc;
				return;
			}
			this.eventCamera = loc;
			if (snapToLocation)
			{
				Game1.camera.ResetCamera(loc + new Vector2(0f, (float)Game1.screenHeight * 0.35f / Game1.hiDefScaleOffset));
			}
			Game1.camera.Update(this.character, this.map, Game1.FrameTime, updateViewPoint: true);
		}

		private void FadeOut(float time, Color color)
		{
			this.fadeLength = (this.fadeTimer = 0f - time);
			this.screenFade = new Color(color.R, color.G, color.B, 0);
		}

		private void FadeIn(float time, Color color)
		{
			this.fadeLength = (this.fadeTimer = time);
			this.screenFade = color;
		}

		public void Update()
		{
			if (Game1.stats.playerLifeState == 0)
			{
				if (this.eventType != EventType.None)
				{
					this.anyEvent = true;
					Game1.BlurScene(0f);
					Game1.hud.canInput = false;
					if (this.map.mapAssetsLoaded)
					{
						if (this.eventTimer >= 0f && Game1.menu.prompt != promptDialogue.SkipEvent)
						{
							this.eventTimer -= Game1.FrameTime;
						}
						switch (this.eventType)
						{
							case EventType.MainEvent:
								this.DoEvent(this.currentEvent);
								break;
							case EventType.SideEvent:
								this.DoSideEvent(this.curSideEvent);
								break;
						}
					}
				}
				else if (Game1.hud.dialogueState != DialogueState.Inactive || Game1.hud.shopType != ShopType.None)
				{
					this.anyEvent = true;
				}
				else
				{
					this.anyEvent = false;
				}
			}
			if (this.fadeTimer != 0f && this.map.mapAssetsLoaded)
			{
				if (this.fadeTimer > 0f)
				{
					this.fadeTimer -= Game1.FrameTime;
					if (this.fadeTimer < 0f)
					{
						this.fadeTimer = 0f;
					}
					this.screenFade.A = (byte)(this.fadeTimer / this.fadeLength * 255f);
					return;
				}
				if (this.fadeTimer < 0f)
				{
					this.fadeTimer += Game1.FrameTime;
					if (this.fadeTimer > 0f)
					{
						this.fadeTimer = 0f;
					}
					this.screenFade.A = (byte)((1f - this.fadeTimer / this.fadeLength) * 255f);
				}
			}
		}

		public void InitEvent(int newEvent, bool isSideEvent)
		{
			this.InitEvent(newEvent, isSideEvent, forceSideEvent: false);
		}

		public void InitEvent(int newEvent, bool isSideEvent, bool forceSideEvent)
		{
			EventManager.readyPlayer = false;
			if (Game1.stats.playerLifeState > 0 || this.map.warpStage > 0 || this.map.doorStage > 0)
			{
				return;
			}
			bool flag = false;
			if (!isSideEvent)
			{
				if (newEvent <= Game1.events.currentEvent)
				{
					return;
				}
				this.eventType = EventType.MainEvent;
				this.currentEvent = newEvent;
				flag = true;
			}
			else if (!this.anyEvent || forceSideEvent)
			{
				if (!this.sideEventAvailable[newEvent])
				{
					return;
				}
				this.sideEventAvailable[newEvent] = false;
				this.eventType = EventType.SideEvent;
				this.curSideEvent = newEvent;
				flag = true;
			}
			if (flag)
			{
				this.anyEvent = true;
				EventManager.readyPlayer = true;
				this.safetyOn = true;
				this.blurOn = true;
				this.subEvent = 0;
				Game1.questManager.UpdateQuests(0);
			}
		}

		public bool InitRegionIntro(string path)
		{
			if (this.anyEvent)
			{
				return false;
			}
			for (int i = 0; i < this.regionIntroTargets.Length; i++)
			{
				ref Vector2 reference = ref this.regionIntroTargets[i];
				reference = Vector2.Zero;
			}
			for (int j = 0; j < this.regionIntroZooms.Length; j++)
			{
				this.regionIntroZooms[j] = 0.8f;
			}
			this.regionIntroStage = 0;
			this.regionIntroTimer = 0f;
			int num = -1;
			bool result = false;
			switch (path)
			{
			case "intro01b":
			{
				num = 0;
				ref Vector2 reference47 = ref this.regionIntroTargets[0];
				reference47 = new Vector2(3480f, 2900f) * 2f;
				ref Vector2 reference48 = ref this.regionIntroTargets[1];
				reference48 = new Vector2(2970f, 2800f) * 2f;
				this.regionIntroZooms[1] = 0.7f;
				ref Vector2 reference49 = ref this.regionIntroTargets[2];
				reference49 = new Vector2(4010f, 1700f) * 2f;
				ref Vector2 reference50 = ref this.regionIntroTargets[3];
				reference50 = new Vector2(4410f, 2150f) * 2f;
				this.regionIntroZooms[3] = 0.9f;
				ref Vector2 reference51 = ref this.regionIntroTargets[4];
				reference51 = new Vector2(3390f, 1400f) * 2f;
				this.regionIntroZooms[4] = 0.8f;
				this.regionIntroZooms[5] = 0.55f;
				break;
			}
			case "village01":
			{
				num = 1;
				ref Vector2 reference42 = ref this.regionIntroTargets[0];
				reference42 = new Vector2(1980f, 380f) * 2f;
				ref Vector2 reference43 = ref this.regionIntroTargets[1];
				reference43 = new Vector2(1980f, 730f) * 2f;
				this.regionIntroZooms[0] = 1f;
				this.regionIntroZooms[1] = 0.9f;
				ref Vector2 reference44 = ref this.regionIntroTargets[2];
				reference44 = new Vector2(4410f, 1010f) * 2f;
				ref Vector2 reference45 = ref this.regionIntroTargets[3];
				reference45 = new Vector2(5040f, 750f) * 2f;
				this.regionIntroZooms[3] = 0.7f;
				ref Vector2 reference46 = ref this.regionIntroTargets[4];
				reference46 = new Vector2(3290f, 770f) * 2f;
				this.regionIntroZooms[4] = 0.9f;
				this.regionIntroZooms[5] = 0.6f;
				break;
			}
			case "smith00":
			{
				num = 2;
				ref Vector2 reference37 = ref this.regionIntroTargets[0];
				reference37 = new Vector2(5840f, 1600f) * 2f;
				ref Vector2 reference38 = ref this.regionIntroTargets[1];
				reference38 = new Vector2(5730f, 1160f) * 2f;
				this.regionIntroZooms[0] = 0.8f;
				this.regionIntroZooms[1] = 0.5f;
				ref Vector2 reference39 = ref this.regionIntroTargets[2];
				reference39 = new Vector2(2020f, 640f) * 2f;
				ref Vector2 reference40 = ref this.regionIntroTargets[3];
				reference40 = new Vector2(2340f, 1200f) * 2f;
				this.regionIntroZooms[3] = 0.7f;
				ref Vector2 reference41 = ref this.regionIntroTargets[4];
				reference41 = new Vector2(2920f, 860f) * 2f;
				this.regionIntroZooms[4] = 0.6f;
				this.regionIntroZooms[5] = 0.4f;
				break;
			}
			case "forest01":
			{
				num = 3;
				ref Vector2 reference32 = ref this.regionIntroTargets[0];
				reference32 = new Vector2(5320f, 230f) * 2f;
				ref Vector2 reference33 = ref this.regionIntroTargets[1];
				reference33 = new Vector2(5000f, 625f) * 2f;
				this.regionIntroZooms[0] = 1f;
				this.regionIntroZooms[1] = 0.9f;
				ref Vector2 reference34 = ref this.regionIntroTargets[2];
				reference34 = new Vector2(3550f, 1020f) * 2f;
				ref Vector2 reference35 = ref this.regionIntroTargets[3];
				reference35 = new Vector2(2850f, 800f) * 2f;
				this.regionIntroZooms[3] = 0.7f;
				ref Vector2 reference36 = ref this.regionIntroTargets[4];
				reference36 = new Vector2(5760f, 960f) * 2f;
				this.regionIntroZooms[4] = 0.9f;
				this.regionIntroZooms[5] = 0.6f;
				break;
			}
			case "cave01":
			{
				num = 4;
				ref Vector2 reference27 = ref this.regionIntroTargets[0];
				reference27 = new Vector2(5800f, 2440f) * 2f;
				ref Vector2 reference28 = ref this.regionIntroTargets[1];
				reference28 = new Vector2(5340f, 2440f) * 2f;
				this.regionIntroZooms[0] = 1f;
				this.regionIntroZooms[1] = 0.9f;
				ref Vector2 reference29 = ref this.regionIntroTargets[2];
				reference29 = new Vector2(2990f, 530f) * 2f;
				ref Vector2 reference30 = ref this.regionIntroTargets[3];
				reference30 = new Vector2(2990f, 1100f) * 2f;
				this.regionIntroZooms[3] = 0.7f;
				ref Vector2 reference31 = ref this.regionIntroTargets[4];
				reference31 = new Vector2(1840f, 830f) * 2f;
				this.regionIntroZooms[4] = 0.8f;
				this.regionIntroZooms[5] = 0.5f;
				break;
			}
			case "cave12":
			{
				Game1.awardsManager.EarnGamerPicture("gamerpic2");
				num = 5;
				ref Vector2 reference22 = ref this.regionIntroTargets[0];
				reference22 = new Vector2(2650f, 1690f) * 2f;
				ref Vector2 reference23 = ref this.regionIntroTargets[1];
				reference23 = new Vector2(3040f, 1600f) * 2f;
				this.regionIntroZooms[1] = 0.9f;
				ref Vector2 reference24 = ref this.regionIntroTargets[2];
				reference24 = new Vector2(5170f, 1630f) * 2f;
				ref Vector2 reference25 = ref this.regionIntroTargets[3];
				reference25 = new Vector2(4750f, 1510f) * 2f;
				this.regionIntroZooms[3] = 0.7f;
				ref Vector2 reference26 = ref this.regionIntroTargets[4];
				reference26 = new Vector2(1800f, 760f) * 2f;
				this.regionIntroZooms[4] = 0.8f;
				this.regionIntroZooms[5] = 0.5f;
				break;
			}
			case "grave02":
			{
				num = 6;
				ref Vector2 reference17 = ref this.regionIntroTargets[0];
				reference17 = new Vector2(2720f, 1700f) * 2f;
				ref Vector2 reference18 = ref this.regionIntroTargets[1];
				reference18 = new Vector2(3030f, 1420f) * 2f;
				this.regionIntroZooms[1] = 0.9f;
				ref Vector2 reference19 = ref this.regionIntroTargets[2];
				reference19 = new Vector2(4600f, 1885f) * 2f;
				ref Vector2 reference20 = ref this.regionIntroTargets[3];
				reference20 = new Vector2(4100f, 1800f) * 2f;
				this.regionIntroZooms[3] = 0.9f;
				ref Vector2 reference21 = ref this.regionIntroTargets[4];
				reference21 = new Vector2(3220f, 1680f) * 2f;
				this.regionIntroZooms[4] = 0.55f;
				this.regionIntroZooms[5] = 0.45f;
				break;
			}
			case "snow02":
			{
				num = 7;
				ref Vector2 reference12 = ref this.regionIntroTargets[0];
				reference12 = new Vector2(640f, 2650f) * 2f;
				ref Vector2 reference13 = ref this.regionIntroTargets[1];
				reference13 = new Vector2(1200f, 2650f) * 2f;
				this.regionIntroZooms[1] = 0.7f;
				this.regionIntroZooms[2] = 1.2f;
				ref Vector2 reference14 = ref this.regionIntroTargets[2];
				reference14 = new Vector2(4780f, 1780f) * 2f;
				ref Vector2 reference15 = ref this.regionIntroTargets[3];
				reference15 = new Vector2(4200f, 1380f) * 2f;
				this.regionIntroZooms[3] = 0.9f;
				ref Vector2 reference16 = ref this.regionIntroTargets[4];
				reference16 = new Vector2(3280f, 1450f) * 2f;
				this.regionIntroZooms[4] = 0.55f;
				this.regionIntroZooms[5] = 0.45f;
				break;
			}
			case "lava01":
			{
				num = 8;
				ref Vector2 reference7 = ref this.regionIntroTargets[0];
				reference7 = new Vector2(2760f, 1360f) * 2f;
				ref Vector2 reference8 = ref this.regionIntroTargets[1];
				reference8 = new Vector2(2760f, 1760f) * 2f;
				this.regionIntroZooms[1] = 0.7f;
				this.regionIntroZooms[2] = 1.2f;
				ref Vector2 reference9 = ref this.regionIntroTargets[2];
				reference9 = new Vector2(3230f, 2140f) * 2f;
				ref Vector2 reference10 = ref this.regionIntroTargets[3];
				reference10 = new Vector2(3550f, 1800f) * 2f;
				this.regionIntroZooms[3] = 0.8f;
				ref Vector2 reference11 = ref this.regionIntroTargets[4];
				reference11 = new Vector2(5000f, 2050f) * 2f;
				this.regionIntroZooms[4] = 0.65f;
				this.regionIntroZooms[5] = 0.55f;
				break;
			}
			case "lava10":
			{
				num = 9;
				ref Vector2 reference2 = ref this.regionIntroTargets[0];
				reference2 = new Vector2(1660f, 1680f) * 2f;
				ref Vector2 reference3 = ref this.regionIntroTargets[1];
				reference3 = new Vector2(1090f, 1400f) * 2f;
				this.regionIntroZooms[1] = 0.7f;
				this.regionIntroZooms[2] = 1.2f;
				ref Vector2 reference4 = ref this.regionIntroTargets[2];
				reference4 = new Vector2(5730f, 1430f) * 2f;
				ref Vector2 reference5 = ref this.regionIntroTargets[3];
				reference5 = new Vector2(5100f, 1620f) * 2f;
				this.regionIntroZooms[3] = 0.7f;
				ref Vector2 reference6 = ref this.regionIntroTargets[4];
				reference6 = new Vector2(4190f, 1660f) * 2f;
				this.regionIntroZooms[4] = 0.55f;
				this.regionIntroZooms[5] = 0.45f;
				break;
			}
			}
			if (num > -1)
			{
				if (!this.regionIntroduced[num])
				{
					this.regionIntroStage = 1;
					this.regionIntroTimer = 4f;
					result = true;
				}
				this.regionIntroduced[num] = true;
			}
			return result;
		}

		public void DoEvent(int currentE)
		{
			switch (currentE)
			{
			case -100:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.Unskippable;
					Sound.DimSFXVolume(0f);
					Music.Play("cave");
					Game1.cutscene.InitCutscene(0);
					this.subEvent = 10;
				}
				else if (this.subEvent == 10)
				{
					Game1.navManager.Reset();
					this.character[0].GroundCharacter();
					this.character[0].Face = CharDir.Left;
					this.character[0].HP = (this.character[0].MaxHP = 9999);
					this.character[0].defaultColor = new Color(0.2f, 0f, 0f, 1f);
					Game1.camera.tempScale = (Game1.worldScale = 0.5f);
					Game1.worldScale *= Game1.hiDefScaleOffset;
					this.eventCamera = new Vector2(4900f, 270f) * 2f;
					Game1.camera.eventCamInfluence = 1f;
					Game1.Scroll = (Game1.camera.tempScroll = this.eventCamera);
					Game1.events.regionDisplayName = string.Empty;
					Game1.hud.InitHelp("defend", restart: true, -1);
					this.FadeIn(4f, Color.Black);
					this.eventTimer = 8f;
					Game1.stats.GetWorldExplored();
					Game1.hud.miniPromptList.Clear();
					for (int num6 = 1; num6 < this.character.Length; num6++)
					{
						this.character[num6].Exists = CharExists.Dead;
					}
					Sound.DimSFXVolume(0.3f);
					this.subEvent++;
				}
				else
				{
					if (this.subEvent != 11)
					{
						break;
					}
					float num7 = this.eventTimer / 8f;
					Game1.worldScale = 0.9f - 0.3f * num7;
					Game1.worldScale *= Game1.hiDefScaleOffset;
					float num8 = 2380f;
					this.eventCamera.Y = num8 - 1500f * num7;
					this.SetEventCamera(new Vector2(10400f, num8 - 1500f * num7), snapToLocation: true);
					if ((num7 < 0.6f && num7 > 0.5f) || (num7 < 0.2f && num7 > 0.1f))
					{
						for (int num9 = 10; num9 < 14; num9++)
						{
							if (this.character[num9].Exists == CharExists.Dead)
							{
								Sound.PlayCue("slash_body");
								this.SpawnIDCharacter((Game1.Scroll + new Vector2(Game1.screenWidth + 200, (float)Game1.screenHeight / 1.5f)) / Game1.worldScale, num9, "enemy", CharacterType.DarkVillager, Team.Enemy, ground: false);
								this.character[num9].HP = 0;
								this.character[num9].Face = CharDir.Right;
								this.character[num9].SetAnim("hurtup", 0, 0);
								this.character[num9].SetJump(Rand.GetRandomInt(400, 1200), jumped: false);
								this.character[num9].Slide(Rand.GetRandomInt(-2200, -1400));
							}
						}
					}
					if (num7 < 0.2f && this.ApproachLoc(CharacterType.Dust, this.eventCamera, 100, null))
					{
						this.ClearEvent();
						Sound.DimSFXVolume(0.3f);
					}
				}
				break;
			case -90:
				if (this.subEvent == 0)
				{
					Sound.DimSFXVolume(0f);
					this.skippable = SkipType.Unskippable;
					this.FadeIn(100000f, Color.Black);
					this.pMan.ResetWeather();
					Game1.navManager.Reset();
					this.map.SwitchMap(this.pMan, this.character, "intro01", loading: false);
					Game1.wManager.SetWeather(WeatherType.Pollen, forceReset: true);
					Game1.wManager.ForceColor();
					Game1.navManager.RevealCell(this.map.path);
					Game1.navManager.RevealMap[Game1.navManager.NavPath].LastVisitPos = Vector2.Zero;
					Game1.navManager.RevealMap[Game1.navManager.NavPath].Updated = false;
					this.eventTimer = 2f;
					Sound.FadeMusicOut(6f);
					this.subEvent++;
				}
				else if (this.eventTimer <= 0f && Game1.cutscene.SceneType == CutsceneType.None)
				{
					Sound.DimSFXVolume(0f);
					this.currentEvent = 0;
					this.subEvent = 0;
				}
				break;
			case 0:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.Skippable;
					this.RelocateDust(new Vector2(6032f, 1414f));
					this.character[0].Face = CharDir.Right;
					this.character[0].SetAnim("intrograb", 0, 2);
					this.character[0].HP = 65;
					this.character[0].MaxHP = 80;
					this.character[0].defaultColor = Color.White;
					this.pMan.RemoveParticle(new Fidget(Vector2.Zero));
					Game1.camera.ResetCamera(this.character);
					Game1.events.regionDisplayName = string.Empty;
					Game1.hud.InitHelp("move", restart: true, -1);
					Game1.worldScale = 0.75f * Game1.hiDefScaleOffset;
					this.eventTimer = 100000f;
					this.FadeIn(this.eventTimer, Color.Black);
					Game1.stats.GetWorldExplored();
					Game1.hud.miniPromptList.Clear();
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.character[0].KeyDown = true;
					if (Game1.menu.menuMode == MenuMode.None)
					{
						this.subEvent = 10;
					}
				}
				else if (this.subEvent == 10)
				{
					this.character[0].KeyDown = true;
					Game1.wManager.SetWeather(WeatherType.Pollen, forceReset: true);
					Game1.wManager.randomType = RandomType.None;
					Game1.wManager.weatherTimer = 1200f;
					Game1.camera.ResetCamera(this.character);
					Music.Play("souls");
					this.eventTimer = 6f;
					this.FadeIn(this.eventTimer, Color.Black);
					this.subEvent++;
				}
				else if (this.subEvent == 11)
				{
					this.character[0].SetAnim("intrograb", 0, 2);
					this.character[0].KeyDown = true;
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(0, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 12)
				{
					Sound.DimSFXVolume(Math.Min(Sound.overrideCinematicVolume + Game1.HudTime, 1f));
					this.character[0].KeyDown = true;
					if (this.DialogueOver())
					{
						this.pMan.AddDustSword(new Vector2(this.character[0].Location.X + 750f, this.character[0].Location.Y - 200f), Vector2.Zero, 6);
						this.eventTimer = 5f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 13)
				{
					this.character[0].KeyDown = true;
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(1, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 14)
				{
					this.character[0].KeyDown = true;
					if (this.DialogueOver())
					{
						this.eventTimer = 4.5f;
						this.character[0].SetAnim("idle00", 0, 2);
						this.character[0].SetAnim("intrograb", 6, 2);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 15)
				{
					if (this.eventTimer > 3f)
					{
						this.character[0].KeyDown = true;
						if (Rand.GetRandomInt(0, 5) == 0)
						{
							this.pMan.AddSparkle(new Vector2(this.character[0].Location.X + 150f + (float)Rand.GetRandomInt(-20, 20), this.character[0].Location.Y + (float)Rand.GetRandomInt(-220, -40)), 0f, 1f, 1f, 1f, Rand.GetRandomFloat(0.2f, 0.8f), Rand.GetRandomInt(14, 28), 5);
						}
					}
					else if ((double)this.eventTimer > 2.9)
					{
						this.character[0].SetAnim("standup", 0, 2);
						Sound.PlayCue("dustswordgrab");
						this.pMan.RemoveParticle(new DustSword(Vector2.Zero, Vector2.Zero));
						for (int num16 = 0; num16 < 4; num16++)
						{
							this.pMan.AddSparkle(new Vector2(this.character[0].Location.X + (float)Rand.GetRandomInt(-100, 100), this.character[0].Location.Y + (float)Rand.GetRandomInt(-200, -20)), 0.5f, 1f, 0.5f, 1f, Rand.GetRandomFloat(0.8f, 1.2f), Rand.GetRandomInt(14, 28), 5);
							this.pMan.AddSparkle(new Vector2(this.character[0].Location.X + 150f + (float)Rand.GetRandomInt(-20, 20), this.character[0].Location.Y + (float)Rand.GetRandomInt(-220, -40)), 1f, 0.5f, 0.5f, 1f, Rand.GetRandomFloat(0.8f, 1.2f), Rand.GetRandomInt(14, 28), 5);
						}
					}
					if (this.eventTimer <= 1f)
					{
						this.pMan.AddFidget(new Vector2(this.character[0].Location.X + 800f, this.character[0].Location.Y - 200f));
						Game1.hud.InitFidgetPrompt(FidgetPrompt.None);
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 16)
				{
					if (this.eventTimer < 0f)
					{
						this.StartDialogue(2, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					Game1.awardsManager.EarnAvatarAward(new string[1] { "AvatarAwards1" });
					this.ClearEvent();
				}
				break;
			case 5:
				if (this.subEvent == 0)
				{
					Game1.hud.InitHelp("treasure", restart: true, -1);
					this.StartDialogue(15, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
					Game1.hud.InitFidgetPrompt(FidgetPrompt.TreasureNear);
				}
				break;
			case 6:
				this.ClearEvent();
				break;
			case 16:
				EventManager.hotSpot = new Vector2(2922f, 1520f) * 2f;
				if (this.subEvent == 0)
				{
					this.SpawnCharacterAppend(EventManager.hotSpot, "deeridle", CharacterType.Deer, Team.Friendly, ground: true);
					this.GetCharacter("deeridle").RandomSkin = 0;
					this.GetCharacter("deeridle").Speed = 800f;
					this.GetCharacter("deeridle").Ai.overrideAI = true;
					this.GetCharacter("deeridle").SetAnim("eat", 0, 0);
					this.skippable = SkipType.DialogueOnly;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.GetCharacter("deeridle").SetAnim("eat", 0, 0);
					this.GetCharacter("deeridle").Location.X = EventManager.hotSpot.X;
					this.GetCharacter("deeridle").Face = CharDir.Left;
					if (this.ApproachLoc(CharacterType.Dust, new Vector2(5400f, 0f), 10, null))
					{
						this.eventTimer = 10f;
						Sound.FadeMusicOut(8f);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer < 7f)
					{
						this.GetCharacter("deeridle").SetAnim("scared", 0, 2);
						this.subEvent++;
					}
					else if (this.eventTimer < 7.2f)
					{
						VibrationManager.SetScreenShake(0.2f);
						for (int num10 = 0; num10 < 2; num10++)
						{
							this.pMan.AddLeaf(EventManager.hotSpot + new Vector2(Rand.GetRandomInt(-400, 100), Rand.GetRandomInt(-550, -460)), Rand.GetRandomVector2(-60f, 60f, 80f, 300f), 1f, 1f, 1f, 1f, Rand.GetRandomFloat(0.9f, 1.2f), 5);
						}
					}
					else
					{
						this.GetCharacter("deeridle").SetAnim("eat", 0, 0);
					}
				}
				else if (this.subEvent == 3)
				{
					if (!(this.eventTimer < 4f))
					{
						break;
					}
					this.GetCharacter("deeridle").Ai.overrideAI = false;
					this.GetCharacter("deeridle").Ai.jobType = JobType.RunLeft;
					this.GetCharacter("deeridle").SetAnim("run", 0, 2);
					if (!(this.eventTimer < 2f))
					{
						break;
					}
					for (int num11 = 1; num11 < this.character.Length; num11++)
					{
						if (this.character[num11].Exists == CharExists.Exists && this.character[num11].Definition.charType == CharacterType.Deer)
						{
							this.character[num11].Exists = CharExists.Dead;
						}
					}
					this.subEvent++;
				}
				else if (this.subEvent == 4)
				{
					Music.Play("glade");
					for (int num12 = 10; num12 < 13; num12++)
					{
						this.SpawnIDCharacter(new Vector2(this.character[0].Location.X + 200f + (float)((num12 - 10) * 200), this.character[0].Location.Y + (float)Rand.GetRandomInt(-700, -500)), num12, "deerfight", CharacterType.Imp, Team.Enemy, ground: false);
					}
					for (int num13 = 13; num13 < 14; num13++)
					{
						this.SpawnIDCharacter(new Vector2(this.character[0].Location.X + 450f, this.character[0].Location.Y + (float)Rand.GetRandomInt(-700, -500)), num13, "deerfight", CharacterType.LightBeast, Team.Enemy, ground: false);
					}
					for (int num14 = 10; num14 < 14; num14++)
					{
						this.character[num14].Face = CharDir.Left;
					}
					VibrationManager.SetScreenShake(0.4f);
					for (int num15 = 0; num15 < 20; num15++)
					{
						this.pMan.AddLeaf(EventManager.hotSpot + new Vector2(Rand.GetRandomInt(-400, 100), Rand.GetRandomInt(-550, -460)), Rand.GetRandomVector2(-60f, 60f, 80f, 300f), 1f, 1f, 1f, 1f, Rand.GetRandomFloat(0.9f, 1.2f), 5);
					}
					this.map.leftBlock = this.character[0].Location.X - 1400f;
					this.map.rightBlock = this.character[0].Location.X + 1400f;
					this.eventTimer = 2f;
					this.subEvent++;
				}
				else if (this.subEvent == 5)
				{
					if (this.eventTimer < 0f)
					{
						this.skippable = SkipType.Skippable;
						this.StartDialogue(5, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 17:
				if (this.subEvent == 0)
				{
					this.map.rightBlock = (this.map.leftBlock = 0f);
					if (this.sideEventAvailable[20])
					{
						Game1.hud.InitHelp("healthItem", restart: true, -1);
						this.sideEventAvailable[20] = false;
					}
					this.StartDialogue(6, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 18:
				Game1.hud.InitHelp("save", restart: true, -1);
				this.ClearEvent();
				break;
			case 22:
				Game1.wManager.weatherTimer = Rand.GetRandomInt(10, 40);
				if (this.character[0].Location.X < 1280f)
				{
					this.character[0].KeyRight = true;
				}
				else
				{
					this.character[0].Trajectory = new Vector2(0f, this.character[0].Trajectory.Y);
				}
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.Unskippable;
					this.safetyOn = false;
					for (int num3 = 0; num3 < 4; num3++)
					{
						this.SpawnCharacter(new Vector2(850 + num3 * 100, 840f) * 2f, "enemy", CharacterType.Imp, Team.Enemy, ground: false);
					}
					Game1.wManager.randomType = RandomType.Forest;
					Game1.stats.projectileType = 0;
					this.character[0].Face = CharDir.Right;
					Game1.camera.camOffset.X = -250f;
					this.StartDialogue(10, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 1.5f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer <= 1f)
					{
						this.character[0].KeySecondary = true;
					}
					if (this.eventTimer <= 0f)
					{
						this.character[0].KeySecondary = false;
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(11, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 1.5f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					if (this.eventTimer <= 0f)
					{
						Game1.stats.curCharge = Game1.stats.maxCharge;
						Game1.stats.projectileType = 0;
						Game1.stats.projectileCost = Game1.stats.GetProjectileCost(Game1.stats.projectileType);
						Game1.stats.canThrow = true;
						this.character[0].KeyThrow = true;
						this.eventTimer = 2.5f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 6)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(12, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 7)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 8)
				{
					if (this.eventTimer <= 0f)
					{
						Game1.stats.curCharge = Game1.stats.maxCharge;
						Game1.stats.projectileType = 0;
						Game1.stats.projectileCost = Game1.stats.GetProjectileCost(Game1.stats.projectileType);
						Game1.stats.canThrow = true;
						this.character[0].KeyThrow = true;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 9)
				{
					if (this.eventTimer <= 0f)
					{
						Game1.stats.canThrow = true;
						this.character[0].KeyThrow = true;
						this.eventTimer = 1.65f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 10)
				{
					if (this.eventTimer <= 1.1f)
					{
						this.character[0].KeySecondary = true;
					}
					if (this.eventTimer <= 0f)
					{
						Game1.stats.curCharge = Game1.stats.maxCharge;
						this.character[0].KeySecondary = false;
						this.eventTimer = 3f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 11)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(13, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 12 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 25:
				if (this.subEvent == 0)
				{
					Game1.hud.InitHelp("quest", restart: true, -1);
					Game1.wManager.SetWeather(WeatherType.Pollen, forceReset: true);
					Game1.wManager.weatherTimer = Rand.GetRandomInt(60, 340);
					this.eventTimer = 1f;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(20, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 27:
				if (this.subEvent == 0)
				{
					Game1.hud.InitHelp("shop", restart: true, -1);
					this.StartDialogue(31, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 28:
				if (this.subEvent == 0)
				{
					EventManager.hotSpot = new Vector2(3200f, 0f);
					if (this.character[0].Location.X < EventManager.hotSpot.X)
					{
						this.character[0].KeyRight = true;
						break;
					}
					this.character[0].Trajectory = new Vector2(this.character[0].Trajectory.X / 2f, this.character[0].Trajectory.Y);
					this.SetEventCamera(new Vector2(3600f, 4800f), snapToLocation: false);
					this.StartDialogue(51, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.DialogueOver())
					{
						this.skippable = SkipType.DialogueOnly;
						Game1.stats.upgrade[16] = 1;
						this.SetEventCamera(Vector2.Zero, snapToLocation: false);
						this.character[0].SetJump(0f, jumped: true);
						this.character[0].Trajectory = new Vector2(this.character[0].Trajectory.X, -2000f);
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (!this.character[0].AnimName.StartsWith("hang"))
					{
						this.character[0].KeyRight = true;
						break;
					}
					Game1.stats.upgrade[16] = 0;
					this.eventTimer = 1f;
					this.subEvent++;
				}
				else if (this.subEvent == 3)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(52, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					if (this.DialogueOver())
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					if (this.character[0].State == CharState.Air)
					{
						this.character[0].KeyDown = true;
						break;
					}
					this.skippable = SkipType.Skippable;
					Game1.camera.camOffset.X = -100f;
					this.StartDialogue(53, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 6 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 30:
				EventManager.hotID = this.character.Length - 1;
				if (this.character[EventManager.hotID].Exists == CharExists.Exists)
				{
					this.SetEventCamera((this.character[0].Location + this.character[EventManager.hotID].Location) / 2f - new Vector2(0f, (float)Game1.screenHeight * 0.3f / Game1.hiDefScaleOffset), snapToLocation: false);
					for (int l = 0; l < this.character.Length; l++)
					{
						if (this.character[l].Definition.charType == CharacterType.Deer)
						{
							this.character[l].Ai.jobType = JobType.Avoid;
						}
					}
				}
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.StartDialogue(33, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 0.5f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer <= 0f)
					{
						EventManager.hotSpot = this.character[0].Location;
						this.SpawnIDCharacter(EventManager.hotSpot + new Vector2(300f, -600f), EventManager.hotID, "slime", CharacterType.Slime, Team.Enemy, ground: false);
						this.character[EventManager.hotID].Face = CharDir.Left;
						this.character[EventManager.hotID].Ai.overrideAI = true;
						this.character[0].SetAnim("idle01", 0, 2);
						this.character[0].SetAnim("runend", 0, 2);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.character[EventManager.hotID].State == CharState.Grounded)
					{
						this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
						this.character[EventManager.hotID].KeyAttack = true;
						if (this.character[EventManager.hotID].AnimName.StartsWith("attack"))
						{
							this.character[0].Evade(this.pMan);
							this.character[0].Trajectory = new Vector2(-2300f, this.character[0].Trajectory.Y);
							this.eventTimer = 1f;
							this.subEvent++;
						}
					}
					else
					{
						this.character[EventManager.hotID].Trajectory = new Vector2(-400f, this.character[EventManager.hotID].Trajectory.Y);
					}
				}
				else if (this.subEvent == 4)
				{
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(34, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.DialogueOver())
					{
						this.character[0].SetAnim("idle01", 0, 2);
						this.character[0].SetAnim("runend", 0, 2);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 6)
				{
					if (this.character[EventManager.hotID].Location.X > this.character[0].Location.X + 250f)
					{
						this.character[EventManager.hotID].Ai.jobType = JobType.RunLeft;
						break;
					}
					EventManager.hotSpot = this.character[EventManager.hotID].Location - new Vector2(0f, 200f);
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					this.eventTimer = 2f;
					this.subEvent++;
				}
				else if (this.subEvent == 7)
				{
					if (this.eventTimer < 1f)
					{
						this.character[EventManager.hotID].KeyAttack = true;
					}
					if (!this.character[EventManager.hotID].AnimName.StartsWith("attack") || !(this.eventTimer < 0.4f))
					{
						break;
					}
					this.character[0].SetAnim("attack00", 0, 0);
					if (this.character[EventManager.hotID].AnimFrame >= 15)
					{
						this.character[0].InitParry(this.pMan, parrySuccess: true);
						this.character[EventManager.hotID].Slide(-1400f);
						this.character[EventManager.hotID].SetAnim("godown", 0, 0);
						this.character[EventManager.hotID].DownTime = 10000f;
						this.character[EventManager.hotID].Ethereal = EtherealState.Ethereal;
						Game1.SlowTime = 0.4f;
						VibrationManager.SetBlast(1.5f, EventManager.hotSpot);
						VibrationManager.SetScreenShake(0.25f);
						Game1.stats.curCharge = MathHelper.Clamp(Game1.stats.curCharge + 50f, 0f, 100f);
						this.pMan.AddLenseFlare(EventManager.hotSpot, 0.4f, 0, 5);
						for (int m = 0; m < 40; m++)
						{
							this.pMan.AddBounceSpark(EventManager.hotSpot + Rand.GetRandomVector2(-40f, 40f, -40f, 40f), Rand.GetRandomVector2(-500f, 500f, -1000f, 10f), 0.3f, 5);
						}
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 8)
				{
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(35, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 9)
				{
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.character[0].Location.X < this.character[EventManager.hotID].Location.X - 300f)
					{
						this.character[0].KeyRight = true;
					}
					if (this.DialogueOver())
					{
						Game1.hud.InitHelp("parry", restart: true, -1);
						this.character[0].SetAnim("idle01", 0, 2);
						this.character[0].SetAnim("runend", 0, 2);
						this.ClearEvent();
					}
				}
				break;
			case 31:
				if (this.subEvent == 0)
				{
					Game1.hud.InitHelp("villager", restart: true, -1);
					this.StartDialogue(39, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 35:
				if (this.subEvent == 0)
				{
					Sound.FadeMusicOut(2f);
					this.skippable = SkipType.DialogueOnly;
					Game1.wManager.SetWeather(WeatherType.RainLight, forceReset: true);
					Game1.wManager.randomType = RandomType.None;
					this.SpawnIDCharacter(new Vector2(3300f, 990f) * 2f, 1, "oneida", CharacterType.Oneida, Team.Friendly, ground: true);
					this.character[1].SetAnim("sitting", 0, 0);
					this.character[1].Face = CharDir.Left;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.character[0].Location.X < 6160f)
					{
						this.character[0].KeyRight = true;
						break;
					}
					EventManager.hotSpot = this.character[0].Location;
					this.eventTimer = 1f;
					this.subEvent++;
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(40, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.DialogueOver())
					{
						this.SpawnIDCharacter(new Vector2(this.character[0].Location.X - 500f, this.character[0].Location.Y + 5000f), 20, "ready", CharacterType.Giant, Team.Enemy, ground: false);
						this.character[20].RandomSkin = 0;
						this.character[20].InitTextures(null);
						this.SpawnIDCharacter(new Vector2(this.character[0].Location.X - 500f, this.character[0].Location.Y + 5000f), 21, "ready", CharacterType.Giant, Team.Enemy, ground: false);
						this.character[21].RandomSkin = 1;
						this.character[21].InitTextures(null);
						this.SpawnIDCharacter(new Vector2(this.character[0].Location.X + 500f, this.character[0].Location.Y + 5000f), 22, "ready", CharacterType.Giant, Team.Enemy, ground: false);
						this.character[22].RandomSkin = 2;
						this.character[22].InitTextures(null);
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					if (this.eventTimer <= 0f)
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					for (int num28 = 10; num28 < 13; num28++)
					{
						this.SpawnIDCharacter(new Vector2(this.character[0].Location.X + 500f + (float)Rand.GetRandomInt(-1000, 1000), this.character[0].Location.Y - 350f + (float)Rand.GetRandomInt(-500, 0)), num28, "miniboss", CharacterType.Imp, Team.Enemy, ground: false);
					}
					for (int num29 = 13; num29 < 15; num29++)
					{
						this.SpawnIDCharacter(new Vector2(this.character[0].Location.X + 500f + (float)Rand.GetRandomInt(-1000, 1000), this.character[0].Location.Y - 350f + (float)Rand.GetRandomInt(-500, 0)), num29, "miniboss", CharacterType.LightBeast, Team.Enemy, ground: false);
					}
					this.SpawnIDCharacter(new Vector2(this.character[0].Location.X + 500f, this.character[0].Location.Y - 550f), 15, "miniboss", CharacterType.Giant, Team.Enemy, ground: false);
					this.character[15].RandomSkin = 0;
					Music.Play("boss1");
					this.map.leftBlock = EventManager.hotSpot.X - 1300f;
					this.map.rightBlock = EventManager.hotSpot.X + 1400f;
					Game1.hud.InitBoss(this.character, "miniboss");
					this.eventTimer = 2f;
					this.subEvent++;
				}
				else if (this.subEvent == 6)
				{
					if (this.eventTimer <= 0f)
					{
						this.skippable = SkipType.DialogueOnly;
						this.StartDialogue(41, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 36:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.eventTimer = 1f;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer <= 0f)
					{
						Game1.wManager.SetWeather(WeatherType.RainHeavy, forceReset: true);
						Game1.wManager.randomType = RandomType.None;
						this.StartDialogue(42, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 0.5f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.eventTimer <= 0f)
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					this.SpawnIDCharacter(new Vector2(this.character[0].Location.X + 500f, this.character[0].Location.Y - 500f), 20, "miniboss2", CharacterType.Giant, Team.Enemy, ground: false);
					this.character[20].RandomSkin = 1;
					this.SpawnIDCharacter(new Vector2(this.character[0].Location.X - 500f, this.character[0].Location.Y - 500f), 21, "miniboss2", CharacterType.Giant, Team.Enemy, ground: false);
					this.character[21].RandomSkin = 2;
					Game1.hud.InitBoss(this.character, "miniboss2");
					this.map.leftBlock = EventManager.hotSpot.X - 1300f;
					this.map.rightBlock = EventManager.hotSpot.X + 2400f;
					this.ClearEvent();
				}
				break;
			case 40:
				if (this.subEvent == 0)
				{
					Game1.wManager.SetWeather(WeatherType.RainLight, forceReset: true);
					Game1.wManager.randomType = RandomType.Forest;
					Sound.FadeMusicOut(1.5f);
					this.skippable = SkipType.DialogueOnly;
					Game1.hud.bossLife = (Game1.hud.bossMaxLife = 0f);
					if (this.character[0].Location.X > EventManager.hotSpot.X - 10f)
					{
						this.character[0].KeyLeft = true;
					}
					else if (this.character[0].Location.X < EventManager.hotSpot.X - 20f)
					{
						this.character[0].KeyRight = true;
					}
					else
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 1)
				{
					this.character[0].Trajectory = new Vector2(0f, this.character[0].Trajectory.Y);
					this.character[0].Face = CharDir.Right;
					Game1.camera.camOffset.X = -250f;
					EventManager.hotSpot = Vector2.Zero;
					this.skippable = SkipType.Skippable;
					this.subEvent++;
				}
				else if (this.subEvent == 2)
				{
					Music.Play("glade");
					this.StartDialogue(43, CharacterType.Oneida);
					this.subEvent++;
				}
				else if (this.subEvent == 3 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 45:
				if (this.subEvent == 0)
				{
					this.StartDialogue(45, CharacterType.Calum);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.ApproachLoc(CharacterType.Bean, this.character[0].Location, 220, "idle01");
					this.ApproachLoc(CharacterType.Calum, this.character[0].Location, 350, "idle01");
					if (this.DialogueOver())
					{
						this.ClearEvent();
					}
				}
				break;
			case 50:
				if (this.subEvent == 0)
				{
					this.StartDialogue(50, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 60:
				if (this.subEvent == 0)
				{
					this.StartDialogue(60, CharacterType.Ginger);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.ApproachLoc(CharacterType.Ginger, this.character[0].Location, 200, null);
					if (this.DialogueOver())
					{
						this.ClearEvent();
					}
				}
				break;
			case 65:
				if (this.subEvent == 0)
				{
					this.StartDialogue(65, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 67:
				if (this.subEvent == 0)
				{
					this.StartDialogue(67, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 70:
			case 75:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(75, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.DialogueOver())
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					for (int num18 = 1; num18 < 8; num18++)
					{
						this.SpawnIDCharacter(new Vector2(this.character[0].Location.X + (float)Rand.GetRandomInt(-800, 800), this.character[0].Location.Y + (float)Rand.GetRandomInt(-800, -500)), num18, "avee", CharacterType.Avee, Team.Enemy, ground: false);
					}
					this.ClearEvent();
				}
				break;
			case 80:
				if (this.subEvent == 0)
				{
					this.StartDialogue(80, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 82:
				if (this.subEvent == 0)
				{
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 85:
				if (this.subEvent == 0)
				{
					EventManager.hotSpot = new Vector2(2370f, 0f) * 2f;
					this.skippable = SkipType.DialogueOnly;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.character[0].Location.X > EventManager.hotSpot.X)
					{
						this.character[0].KeyLeft = true;
						break;
					}
					this.safetyOn = false;
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 2 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 86:
				if (this.subEvent == 0)
				{
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 90:
				if (this.subEvent == 0)
				{
					this.StartDialogue(90, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 95:
				if (this.subEvent == 0)
				{
					EventManager.hotID = this.character.Length - 1;
					this.SpawnIDCharacter(new Vector2(5700f, 1980f) * 2f, EventManager.hotID, "fusescript", CharacterType.Fuse, Team.Enemy, ground: true);
					Sound.FadeMusicOut(4f);
					this.skippable = SkipType.DialogueOnly;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.character[EventManager.hotID].Ethereal = EtherealState.Ethereal;
					this.character[EventManager.hotID].Face = CharDir.Left;
					this.character[EventManager.hotID].SetAnim("intro", 0, 2);
					if (this.character[0].Location.X < 10750f)
					{
						this.character[0].KeyRight = true;
					}
					else
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					this.eventTimer = 2f;
					this.subEvent++;
				}
				else if (this.subEvent == 3)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(95, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 5.5f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					this.character[EventManager.hotID].SetAnim("intro2", 0, 2);
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(96, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 6)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 3f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 7)
				{
					this.character[EventManager.hotID].SetAnim("intro3", 0, 2);
					if (this.eventTimer <= 0f)
					{
						this.eventTimer = 3f;
						this.FadeOut(this.eventTimer, Color.White);
						this.subEvent++;
					}
					else
					{
						if (!(this.eventTimer < 2f))
						{
							break;
						}
						if ((double)this.eventTimer > 1.98)
						{
							for (int i = 0; i < 4; i++)
							{
								this.pMan.AddFusePillar(Game1.Scroll / Game1.worldScale + new Vector2((float)i * ((float)(Game1.screenWidth / 4) / Game1.worldScale) + 100f, 0f), 0, 6);
							}
						}
						if (Rand.GetRandomInt(0, 40) == 0)
						{
							this.pMan.AddFusePillar((Game1.Scroll + Rand.GetRandomVector2(0f, Game1.screenWidth, 0f, 0f)) / Game1.worldScale, 0, 6);
						}
					}
				}
				else if (this.subEvent == 8)
				{
					Vector2 loc = (Game1.Scroll + Rand.GetRandomVector2(0f, Game1.screenWidth, 0f, Game1.screenHeight)) / Game1.worldScale;
					this.pMan.AddExplosion(loc, Rand.GetRandomFloat(1f, 3f), makeSmoke: false, 6);
					if (Rand.GetRandomInt(0, 3) == 0)
					{
						Sound.PlayCue("explosion_01", loc, Math.Abs(loc.X - this.character[0].Location.X) + Math.Abs(loc.Y - this.character[0].Location.Y));
					}
					VibrationManager.SetScreenShake(0.5f);
					if (Game1.wManager.weatherColor.X < 4f)
					{
						Game1.wManager.weatherColor.X = (Game1.wManager.weatherColor.Y = (Game1.wManager.weatherColor.Z += 0.04f));
					}
					if (!(this.eventTimer <= 0f))
					{
						break;
					}
					this.fadeLength = (this.fadeTimer = 1f);
					Vector2 location = this.character[0].Location;
					Vector2 location2 = this.character[EventManager.hotID].Location;
					_ = Game1.Scroll;
					Vector2 camOffset = Game1.camera.camOffset;
					this.map.SwitchMap(this.pMan, this.character, "forest10alt0", loading: false);
					Game1.wManager.SetWeather(WeatherType.Fire, forceReset: true);
					this.SpawnIDCharacter(location2, EventManager.hotID, "fuseboss", CharacterType.Fuse, Team.Enemy, ground: true);
					this.character[EventManager.hotID].SetAnim("idle00", 0, 0);
					this.character[0].Location = location;
					Game1.camera.camOffset = camOffset;
					for (int j = 0; j < 30; j++)
					{
						loc = (Game1.Scroll + Rand.GetRandomVector2(0f, Game1.screenWidth, 0f, Game1.screenHeight)) / Game1.worldScale;
						this.pMan.AddExplosion(loc, Rand.GetRandomFloat(1f, 2f), makeSmoke: true, 6);
						if (Rand.GetRandomInt(0, 3) == 0)
						{
							Sound.PlayCue("explosion_01", loc, Math.Abs(loc.X - this.character[0].Location.X) + Math.Abs(loc.Y - this.character[0].Location.Y));
						}
					}
					Music.Play("boss3");
					this.map.leftBlock = 8100f;
					this.map.rightBlock = this.map.rightEdge;
					Game1.hud.InitBoss(this.character, "fuseboss");
					this.eventTimer = 2f;
					this.subEvent++;
				}
				else if (this.subEvent == 9)
				{
					if (Game1.wManager.weatherColor.X > 1f)
					{
						Game1.wManager.weatherColor.X = (Game1.wManager.weatherColor.Y = (Game1.wManager.weatherColor.Z -= 0.04f));
					}
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(97, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 10 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 100:
				if (this.subEvent == 0)
				{
					Game1.awardsManager.EarnAchievement(Achievement.DefeatFuse, forceCheck: false);
					this.skippable = SkipType.DialogueOnly;
					Sound.FadeMusicOut(1f);
					Game1.wManager.SetWeather(WeatherType.None, forceReset: true);
					Game1.SlowTime = 4f;
					this.eventTimer = 0.4f;
					this.FadeOut(1f, Color.White);
					this.character[EventManager.hotID].DyingFrame = -0.99f;
					this.character[EventManager.hotID].Trajectory = new Vector2(0f, this.character[EventManager.hotID].Trajectory.Y);
					if (this.character[EventManager.hotID].Trajectory.Y > 0f)
					{
						this.character[EventManager.hotID].Trajectory = new Vector2(this.character[EventManager.hotID].Trajectory.X, 0f);
					}
					this.character[EventManager.hotID].Name = "fuse_die";
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.character[0].Ethereal = EtherealState.Ethereal;
					if (this.character[EventManager.hotID].Trajectory.Y > 0f)
					{
						this.character[EventManager.hotID].Trajectory = new Vector2(this.character[EventManager.hotID].Trajectory.X, 0f);
					}
					if (this.eventTimer <= 0f)
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.screenFade.A >= byte.MaxValue)
					{
						this.character[EventManager.hotID].Exists = CharExists.Dead;
						this.map.leftBlock = (this.map.rightBlock = 0f);
						_ = this.character[0].Location;
						this.RelocateDust(new Vector2(960f, 6080f));
						this.character[0].Face = CharDir.Right;
						this.SpawnIDCharacter(new Vector2(1240f, 6080f), EventManager.hotID, "fusescript", CharacterType.Fuse, Team.Enemy, ground: true);
						Game1.camera.ResetCamera(this.character);
						Game1.camera.camOffset.X = -200f;
						this.screenFade = Color.White;
						this.eventTimer = 3f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(100, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					if (this.DialogueOver())
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					this.FadeIn(6f, Color.White);
					if (this.eventTimer <= 0f)
					{
						this.eventTimer = 6f;
						Music.Play("beauty");
						this.subEvent++;
					}
				}
				else if (this.subEvent == 6)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(101, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 7)
				{
					if (this.DialogueOver())
					{
						this.FadeOut(4f, Color.Black);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 8)
				{
					if (this.screenFade.A >= byte.MaxValue)
					{
						this.StartDialogue(105, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 9)
				{
					if (this.DialogueOver() && Game1.hud.dialogue == null)
					{
						this.FadeIn(3f, Color.Black);
						this.map.SwitchMap(this.pMan, this.character, "forest10", loading: false);
						Sound.FadeMusicOut(4f);
						Game1.wManager.SetWeather(WeatherType.RainLight, forceReset: true);
						this.RelocateDust(new Vector2(10543f, 4075f));
						Game1.camera.ResetCamera(this.character);
						Game1.camera.camOffset.X = -250f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 10 && this.screenFade.A <= 0)
				{
					this.ClearEvent();
				}
				break;
			case 102:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.StartDialogue(106, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					Game1.awardsManager.EarnAvatarAward(new string[1] { "AvatarAwards2" });
					this.ClearEvent();
				}
				break;
			case 110:
				if (this.subEvent == 0)
				{
					this.StartDialogue(110, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 115:
				Game1.worldScale = (Game1.camera.tempScale = 1.2f);
				Game1.worldScale *= Game1.hiDefScaleOffset;
				this.character[0].Face = CharDir.Right;
				if (this.subEvent == 0)
				{
					this.RelocateDust(new Vector2(255f, 830f) * 2f);
					this.FaceCharacter(CharacterType.Soldier, CharacterType.Gaius, faceTowards: true);
					this.FaceCharacter(CharacterType.Gaius, CharacterType.Soldier, faceTowards: true);
					this.SetEventCamera((this.GetCharacter(CharacterType.Soldier).Location + this.GetCharacter(CharacterType.Gaius).Location) / 2f - new Vector2(0f, (float)Game1.screenHeight * 0.3f / Game1.hiDefScaleOffset), snapToLocation: true);
					Music.Play("silent");
					this.skippable = SkipType.DialogueOnly;
					this.eventTimer = 5f;
					this.screenFade = Color.Black;
					Game1.cutscene.InitCutscene(30);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer < 0f)
					{
						this.eventTimer = 4f;
						this.FadeIn(this.eventTimer, Color.Black);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer < 0f)
					{
						this.StartDialogue(600, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.DialogueOver())
					{
						this.GetCharacter(CharacterType.Soldier).Name = "soldier";
						this.GetCharacter(CharacterType.Soldier).SetAnim("idle00", 0, 2);
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					if (this.eventTimer < 0f)
					{
						this.SetEventCamera(this.GetCharacter(CharacterType.Gaius).Location - new Vector2(0f, (float)Game1.screenHeight * 0.3f / Game1.hiDefScaleOffset), snapToLocation: false);
						this.StartDialogue(601, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					this.ApproachLoc(CharacterType.Soldier, Vector2.Zero, 0, null);
					if (this.DialogueOver())
					{
						this.eventTimer = 1f;
						this.FadeOut(this.eventTimer, Color.Black);
						this.subEvent++;
					}
				}
				else if (this.eventTimer < 0f)
				{
					this.ClearEvent();
					this.map.WorldMapFromEvent(this.pMan, this.character, "forest11", TransitionDirection.Right, "lava");
				}
				break;
			case 125:
				if (this.subEvent == 0)
				{
					if (this.GetCharacter(CharacterType.Dust).Location.X < 7000f)
					{
						this.skippable = SkipType.Unskippable;
						if (this.GetCharacter(CharacterType.Dust).Location.X > 3000f && this.GetCharacter(CharacterType.Dust).Location.X < 3400f && this.GetCharacter(CharacterType.Dust).State == CharState.Grounded)
						{
							this.GetCharacter(CharacterType.Dust).SetJump(2000f, jumped: true);
						}
						if (this.ApproachLoc(CharacterType.Dust, new Vector2(5400f, 2200f), 20, null))
						{
							this.skippable = SkipType.Skippable;
							this.map.leftBlock = 3900f;
							this.map.rightBlock = 12400f;
							this.StartDialogue(121, CharacterType.Dust);
							this.subEvent++;
						}
					}
					else
					{
						this.map.leftBlock = 3900f;
						this.map.rightBlock = 12400f;
						this.StartDialogue(121, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 130:
				this.ApproachLoc(CharacterType.Ginger, this.character[0].Location, 200, null);
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.StartDialogue(130, CharacterType.Ginger);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.DialogueOver())
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					Sound.PlayCue("loud_distant_explosion", this.character[0].Location - new Vector2(100f, 0f), 0f);
					VibrationManager.SetBlast(2f, this.character[0].Location + new Vector2(-800f, 0f));
					this.FadeIn(2f, Color.White);
					this.map.MapSegFrameSpeed = 2f;
					VibrationManager.SetScreenShake(2f);
					Music.Play("silent");
					this.eventTimer = 2f;
					this.subEvent++;
				}
				else if (this.subEvent == 3)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(131, CharacterType.Ginger);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					if (this.DialogueOver())
					{
						for (int k = 20; k < 23; k++)
						{
							this.SpawnIDCharacter(new Vector2(2400 + (k - 20) * 120, 900 + Rand.GetRandomInt(-50, 100)) * 2f, k, "villagefight", CharacterType.SquirtBug, Team.Enemy, ground: false);
							this.character[k].alwaysUpdatable = true;
							this.character[k].Ai.SetTarget(0);
						}
						this.RelocateDust(new Vector2(3300f, 1000f) * 2f);
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					this.eventCamera = this.character[21].Location;
					if (this.eventTimer <= 0f)
					{
						this.FaceCharacter(CharacterType.Dust, CharacterType.SquirtBug, faceTowards: true);
						this.StartDialogue(132, CharacterType.Ginger);
						this.map.rightBlock = 7600f;
						this.GetCharacter(CharacterType.Ginger).Exists = CharExists.Dead;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 6 && this.DialogueOver())
				{
					this.FaceCharacter(CharacterType.Dust, CharacterType.SquirtBug, faceTowards: false);
					this.FaceCharacter(CharacterType.Dust, CharacterType.SquirtBug, faceTowards: true);
					this.ClearEvent();
					this.map.leftBlock = 3900f;
					this.map.rightBlock = 7200f;
					this.currentEvent = 131;
				}
				break;
			case 132:
				this.ApproachLoc(CharacterType.Ginger, this.character[0].Location, 200, null);
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					Music.Play("villageforest");
					this.SpawnCharacter(new Vector2(3800f, 950f) * 2f, "ginger", CharacterType.Ginger, Team.Friendly, ground: true);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.ApproachLoc(CharacterType.Dust, this.GetCharacter(CharacterType.Ginger).Location, 200, null))
					{
						this.StartDialogue(133, CharacterType.Ginger);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 135:
				if (this.subEvent == 0)
				{
					this.StartDialogue(135, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 140:
				if (this.subEvent == 0)
				{
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 142:
				if (this.subEvent == 0)
				{
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 145:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.eventTimer = 1f;
					this.subEvent++;
				}
				if (this.subEvent == 1)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(145, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.DialogueOver())
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					this.character[0].Face = CharDir.Left;
					this.character[0].SetAnim("idle01", 0, 2);
					this.character[0].SetAnim("idle00", 0, 2);
					Game1.camera.camOffset.X = 250f;
					EventManager.hotID = this.character.Length - 1;
					this.SpawnIDCharacter(this.character[0].Location - new Vector2(900f, 200f), EventManager.hotID, "dumb", CharacterType.RockHound, Team.Enemy, ground: false);
					this.character[EventManager.hotID].Ai.overrideAI = true;
					this.subEvent++;
				}
				else if (this.subEvent == 4)
				{
					this.character[EventManager.hotID].Ai.jobType = JobType.RunRight;
					if (this.character[EventManager.hotID].Location.X > this.character[0].Location.X - 500f)
					{
						this.character[EventManager.hotID].SetAnim("attack00", 0, 2);
						this.character[EventManager.hotID].Ai.overrideAI = false;
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					if (this.eventTimer <= 0f)
					{
						this.skippable = SkipType.Skippable;
						this.StartDialogue(146, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 6 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 148:
				if (this.subEvent == 0)
				{
					EventManager.hotSpot = new Vector2(4400f, 1080f) * 2f;
					EventManager.hotID = this.character.Length - 1;
					this.SpawnIDCharacter(EventManager.hotSpot + new Vector2(-1000f, 0f), EventManager.hotID, "trolk", CharacterType.Trolk, Team.Enemy, ground: true);
					this.character[EventManager.hotID].Face = CharDir.Right;
					this.character[EventManager.hotID].Ai.overrideAI = true;
					this.skippable = SkipType.DialogueOnly;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.character[EventManager.hotID].SetAnim("idle00", 0, 2);
					if (this.character[0].Location.X > EventManager.hotSpot.X)
					{
						this.character[0].KeyLeft = true;
						break;
					}
					this.safetyOn = false;
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 2)
				{
					this.SetEventCamera((this.character[0].Location + this.character[EventManager.hotID].Location) / 2f - new Vector2(0f, (float)Game1.screenHeight * 0.4f / Game1.hiDefScaleOffset), snapToLocation: false);
					if (this.character[EventManager.hotID].Location.X < EventManager.hotSpot.X - 800f)
					{
						this.character[EventManager.hotID].Ai.jobType = JobType.RunRight;
						break;
					}
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					this.character[EventManager.hotID].SetAnim("intro01", 6, 2);
					this.subEvent++;
				}
				else if (this.subEvent == 3)
				{
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.DialogueOver())
					{
						this.character[0].SetAnim("idle01", 0, 2);
						this.character[0].SetAnim("runend", 0, 2);
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					this.SetEventCamera(Vector2.Zero, snapToLocation: false);
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.eventTimer < 1f && this.eventTimer > 0.8f && this.character[0].AnimName != "parry")
					{
						this.character[0].KeyAttack = true;
					}
					if (this.eventTimer <= 0f)
					{
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(149, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 6)
				{
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.character[EventManager.hotID].Location.X < EventManager.hotSpot.X - 800f)
					{
						this.character[EventManager.hotID].Ai.jobType = JobType.RunRight;
					}
					if (this.character[0].Location.X > EventManager.hotSpot.X - 200f)
					{
						this.character[0].KeyLeft = true;
					}
					if (this.DialogueOver())
					{
						this.eventTimer = 2f;
						this.character[0].SetAnim("idle01", 0, 2);
						this.character[0].SetAnim("runend", 0, 2);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 7)
				{
					this.SetEventCamera((this.character[0].Location + this.character[EventManager.hotID].Location) / 2f - new Vector2(0f, (float)Game1.screenHeight * 0.4f / Game1.hiDefScaleOffset), snapToLocation: false);
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.eventTimer < 1f)
					{
						this.character[EventManager.hotID].KeyAttack = true;
					}
					if (!(this.character[EventManager.hotID].AnimName == "attack00") || !(this.eventTimer < 0.2f))
					{
						break;
					}
					this.character[0].SetAnim("attack00", 0, 0);
					if (this.character[EventManager.hotID].AnimFrame >= 11)
					{
						this.character[EventManager.hotID].SetAnim("godown", 0, 0);
						this.character[EventManager.hotID].DownTime = 10000f;
						this.character[EventManager.hotID].Ethereal = EtherealState.Ethereal;
						Game1.SlowTime = 0.4f;
						VibrationManager.SetBlast(1f, EventManager.hotSpot);
						VibrationManager.SetScreenShake(0.25f);
						Game1.stats.curCharge = MathHelper.Clamp(Game1.stats.curCharge + 50f, 0f, 100f);
						Vector2 vector6 = (this.character[0].Location + this.character[EventManager.hotID].Location) / 2f + new Vector2(0f, -250f);
						this.pMan.AddLenseFlare(vector6, 0.4f, 0, 5);
						for (int num26 = 0; num26 < 30; num26++)
						{
							this.pMan.AddBounceSpark(vector6 + Rand.GetRandomVector2(-40f, 40f, -40f, 40f), Rand.GetRandomVector2(-500f, 500f, -1000f, 10f), 0.3f, 5);
						}
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 8)
				{
					if (this.character[EventManager.hotID].AnimName == "godown" && this.character[EventManager.hotID].AnimFrame == 1)
					{
						this.character[0].InitParry(this.pMan, parrySuccess: true);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 9)
				{
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(150, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 10)
				{
					this.SetEventCamera((this.character[0].Location + this.character[EventManager.hotID].Location) / 2f - new Vector2(0f, (float)Game1.screenHeight * 0.4f / Game1.hiDefScaleOffset), snapToLocation: false);
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.character[0].Location.X > EventManager.hotSpot.X - 300f)
					{
						this.character[0].KeyLeft = true;
					}
					if (this.DialogueOver())
					{
						this.character[EventManager.hotID].DownTime = 1f;
						this.character[0].SetAnim("idle01", 0, 2);
						this.character[0].SetAnim("runend", 0, 2);
						this.ClearEvent();
					}
				}
				break;
			case 152:
				if (this.subEvent == 0)
				{
					this.SpawnCharacter(new Vector2(550f, 2100f) * 2f, "bopo", CharacterType.Bopo, Team.Friendly, ground: true);
					this.GetCharacter(CharacterType.Bopo).Face = CharDir.Left;
					this.skippable = SkipType.DialogueOnly;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.GetCharacter(CharacterType.Bopo).SetAnim("intro01", 0, 0);
					if (this.ApproachLoc(CharacterType.Dust, new Vector2(1600f, 0f), 10, null))
					{
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer < 0f)
					{
						this.GetCharacter(CharacterType.Bopo).SetAnim("run", 0, 0);
						this.GetCharacter(CharacterType.Bopo).SetAnim("intro01", 6, 0);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.GetCharacter(CharacterType.Bopo).AnimFrame > 11)
					{
						this.GetCharacter(CharacterType.Bopo).Face = CharDir.Right;
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					if (this.eventTimer < 0f)
					{
						this.GetCharacter(CharacterType.Bopo).Face = CharDir.Left;
						this.GetCharacter(CharacterType.Bopo).SetAnim("fastrun", 0, 2);
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					this.GetCharacter(CharacterType.Bopo).Trajectory = new Vector2(0f - this.GetCharacter(CharacterType.Bopo).Speed, 0f);
					if (this.eventTimer < 0f)
					{
						this.GetCharacter(CharacterType.Bopo).Exists = CharExists.Dead;
						this.skippable = SkipType.Skippable;
						this.StartDialogue(currentE, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 155:
				if (this.subEvent == 0)
				{
					this.SpawnCharacter(new Vector2(2750f, 1750f) * 2f, "bopo", CharacterType.Bopo, Team.Friendly, ground: true);
					this.GetCharacter(CharacterType.Bopo).Face = CharDir.Left;
					this.skippable = SkipType.DialogueOnly;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.GetCharacter(CharacterType.Bopo).SetAnim("intro01", 0, 0);
					this.GetCharacter(CharacterType.Bopo).Face = CharDir.Left;
					if (this.ApproachLoc(CharacterType.Dust, new Vector2(6000f, 0f), 10, null))
					{
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer < 0f)
					{
						this.GetCharacter(CharacterType.Bopo).SetAnim("run", 0, 0);
						this.GetCharacter(CharacterType.Bopo).SetAnim("intro01", 6, 0);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.GetCharacter(CharacterType.Bopo).AnimFrame > 11)
					{
						this.GetCharacter(CharacterType.Bopo).Face = CharDir.Right;
						this.StartDialogue(currentE, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					if (this.eventTimer < 0f)
					{
						this.GetCharacter(CharacterType.Bopo).Face = CharDir.Left;
						this.GetCharacter(CharacterType.Bopo).SetAnim("fastrun", 0, 2);
						this.pMan.AddEquipment(this.GetCharacter(CharacterType.Bopo).Location, new Vector2(-200f, -800f), 3, bluePrint: false, -1, 5);
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 6)
				{
					this.GetCharacter(CharacterType.Bopo).Trajectory = new Vector2(0f - this.GetCharacter(CharacterType.Bopo).Speed, 0f);
					if (this.eventTimer < 0f)
					{
						this.GetCharacter(CharacterType.Bopo).Exists = CharExists.Dead;
						this.skippable = SkipType.Skippable;
						this.StartDialogue(156, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 160:
				if (this.subEvent == 0)
				{
					this.SpawnCharacter(new Vector2(2010f, 2420f) * 2f, "bopo", CharacterType.Bopo, Team.Friendly, ground: true);
					this.GetCharacter(CharacterType.Bopo).Face = CharDir.Left;
					this.skippable = SkipType.DialogueOnly;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.GetCharacter(CharacterType.Bopo).SetAnim("intro01", 0, 0);
					this.GetCharacter(CharacterType.Bopo).Face = CharDir.Left;
					if (this.ApproachLoc(CharacterType.Dust, new Vector2(4400f, 0f), 10, null))
					{
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer < 0f)
					{
						this.GetCharacter(CharacterType.Bopo).SetAnim("run", 0, 0);
						this.GetCharacter(CharacterType.Bopo).SetAnim("intro01", 6, 0);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.GetCharacter(CharacterType.Bopo).AnimFrame > 11)
					{
						this.GetCharacter(CharacterType.Bopo).Face = CharDir.Right;
						this.StartDialogue(currentE, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					if (this.eventTimer < 0f)
					{
						this.GetCharacter(CharacterType.Bopo).Face = CharDir.Left;
						this.GetCharacter(CharacterType.Bopo).SetAnim("fastrun", 0, 2);
						for (int num4 = 0; num4 < 2; num4++)
						{
							this.pMan.AddEquipment(this.GetCharacter(CharacterType.Bopo).Location, new Vector2(-200 + 400 * num4, -800f), 1, bluePrint: false, -1, 5);
						}
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 6)
				{
					this.GetCharacter(CharacterType.Bopo).Trajectory = new Vector2(0f - this.GetCharacter(CharacterType.Bopo).Speed, 0f);
					if (this.eventTimer < 0f)
					{
						this.GetCharacter(CharacterType.Bopo).Exists = CharExists.Dead;
						this.skippable = SkipType.Skippable;
						this.StartDialogue(161, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 170:
				if (this.subEvent == 0)
				{
					this.SpawnCharacter(new Vector2(670f, 2500f) * 2f, "bopo", CharacterType.Bopo, Team.Friendly, ground: true);
					this.GetCharacter(CharacterType.Bopo).Face = CharDir.Right;
					this.GetCharacter(CharacterType.Bopo).SetAnim("scared", 0, 0);
					EventManager.hotSpot = this.GetCharacter(CharacterType.Bopo).Location + new Vector2(300f, 0f);
					Sound.FadeMusicOut(4f);
					this.skippable = SkipType.DialogueOnly;
					Game1.wManager.SetWeather(WeatherType.CaveCalm, forceReset: true);
					Game1.wManager.SetManualColor(new Vector4(1f, 0.6f, 0.6f, 0f), 1.5f, 0.25f, immediately: false);
					this.subEvent++;
					this.SpawnCharacter(new Vector2(this.GetCharacter(CharacterType.Bopo).Location.X - 300f, this.GetCharacter(CharacterType.Bopo).Location.Y - 50f), "miniboss", CharacterType.Trolk, Team.Enemy, ground: true);
				}
				else if (this.subEvent == 1)
				{
					this.GetCharacter(CharacterType.Bopo).SetAnim("scared", 0, 0);
					if (this.ApproachLoc(CharacterType.Dust, EventManager.hotSpot, 10, null))
					{
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(currentE, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.DialogueOver())
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					this.SpawnCharacter(this.character[0].Location + new Vector2(1200f, -150f), "miniboss", CharacterType.Trolk, Team.Enemy, ground: true);
					this.map.leftBlock = 300f;
					this.map.rightBlock = 3200f;
					Game1.hud.InitBoss(this.character, "miniboss");
					Music.Play("boss2");
					this.ClearEvent();
				}
				break;
			case 175:
				if (this.subEvent == 0)
				{
					Game1.wManager.SetWeather(WeatherType.Fog, forceReset: true);
					Sound.FadeMusicOut(1.5f);
					this.skippable = SkipType.DialogueOnly;
					Game1.hud.bossLife = (Game1.hud.bossMaxLife = 0f);
					if (this.ApproachLoc(CharacterType.Dust, EventManager.hotSpot, 0, null))
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 1)
				{
					this.character[0].Face = CharDir.Left;
					Game1.camera.camOffset.X = 250f;
					EventManager.hotSpot = Vector2.Zero;
					this.subEvent++;
				}
				else if (this.subEvent == 2)
				{
					this.GetCharacter(CharacterType.Bopo).SetAnim("intro01", 11, 0);
					this.GetCharacter(CharacterType.Bopo).Face = CharDir.Right;
					this.eventTimer = 2f;
					this.subEvent++;
				}
				else if (this.subEvent == 3)
				{
					if (this.eventTimer < 0f)
					{
						this.GetCharacter(CharacterType.Bopo).Face = CharDir.Left;
						this.GetCharacter(CharacterType.Bopo).SetAnim("fastrun", 0, 2);
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					this.GetCharacter(CharacterType.Bopo).Trajectory = new Vector2(0f - this.GetCharacter(CharacterType.Bopo).Speed, 0f);
					if (this.eventTimer <= 0f)
					{
						this.GetCharacter(CharacterType.Bopo).Exists = CharExists.Dead;
						this.skippable = SkipType.Skippable;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 180:
				if (this.subEvent == 0)
				{
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 185:
				if (this.subEvent == 0)
				{
					this.SpawnCharacter(new Vector2(3380f, 1100f) * 2f, "mamop", CharacterType.MaMop, Team.Friendly, ground: true);
					this.SpawnCharacter(new Vector2(3280f, 1100f) * 2f, "bopo", CharacterType.Bopo, Team.Friendly, ground: true);
					this.GetCharacter(CharacterType.Bopo).Ai.initPos = 7800f;
					this.GetCharacter(CharacterType.MaMop).Ai.initPos = 6400f;
					this.GetCharacter(CharacterType.Bopo).Face = CharDir.Right;
					this.GetCharacter(CharacterType.MaMop).Face = CharDir.Left;
					this.GetCharacter(CharacterType.Bopo).Ai.overrideAI = true;
					this.GetCharacter(CharacterType.MaMop).Ai.overrideAI = true;
					this.StartDialogue(185, CharacterType.Bopo);
					this.skippable = SkipType.DialogueOnly;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.DialogueOver())
					{
						this.SetEventCamera(new Vector2(3320f, 2000f) * 2f - new Vector2(0f, (float)Game1.screenHeight * 0.4f / Game1.hiDefScaleOffset), snapToLocation: false);
						this.eventTimer = 1.5f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					this.GetCharacter(CharacterType.Bopo).Face = CharDir.Right;
					this.GetCharacter(CharacterType.MaMop).Face = CharDir.Left;
					if (this.eventTimer < 0f)
					{
						this.StartDialogue(186, CharacterType.Bopo);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.DialogueOver())
					{
						this.SetEventCamera(Vector2.Zero, snapToLocation: false);
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					if (this.eventTimer <= 0f)
					{
						this.GetCharacter(CharacterType.Bopo).SetAnim("idle00", 0, 2);
						this.StartDialogue(187, CharacterType.Bopo);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					if (this.DialogueOver())
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 6)
				{
					if (this.character[0].Location.X > 7200f)
					{
						this.character[0].KeyLeft = true;
						break;
					}
					this.StartDialogue(188, CharacterType.Bopo);
					this.skippable = SkipType.Skippable;
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 200:
			case 210:
			case 215:
				this.ClearEvent();
				break;
			case 230:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.SpawnCharacter(new Vector2(4020f, 1060f) * 2f, "boss", CharacterType.LadyBoss, Team.Enemy, ground: false);
					this.StartDialogue(currentE, CharacterType.Lady);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.DialogueOver())
					{
						Sound.FadeMusicOut(4f);
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					this.SetEventCamera((this.GetCharacter(CharacterType.LadyBoss).Location + this.character[0].Location) / 2f - new Vector2(0f, (float)Game1.screenHeight * 0.2f / Game1.hiDefScaleOffset), snapToLocation: false);
					if (this.ApproachLoc(CharacterType.LadyBoss, new Vector2(4450f, 1300f) * 2f, 10, null) && this.eventTimer < 0f)
					{
						this.StartDialogue(231, CharacterType.Lady);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					this.SetEventCamera((this.GetCharacter(CharacterType.LadyBoss).Location + this.character[0].Location) / 2f - new Vector2(0f, (float)Game1.screenHeight * 0.2f / Game1.hiDefScaleOffset), snapToLocation: false);
					if (this.ApproachLoc(CharacterType.LadyBoss, new Vector2(4450f, 1300f) * 2f, 10, null) && this.DialogueOver())
					{
						this.GetCharacter(CharacterType.LadyBoss).Ai.overrideAI = true;
						this.GetCharacter(CharacterType.LadyBoss).SetAnim("attack00", 0, 0);
						Music.Play("boss2");
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					this.ApproachLoc(CharacterType.LadyBoss, new Vector2(4450f, 1300f) * 2f, 10, null);
					if (this.eventTimer < 0f)
					{
						this.StartDialogue(232, CharacterType.Lady);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					Game1.hud.InitBoss(this.character, "boss");
					this.map.leftBlock = 7000f;
					this.map.rightBlock = 10400f;
					this.ClearEvent();
				}
				break;
			case 240:
				if (this.subEvent == 0)
				{
					Game1.awardsManager.EarnAchievement(Achievement.DefeatLady, forceCheck: false);
					this.skippable = SkipType.DialogueOnly;
					Sound.FadeMusicOut(1f);
					Game1.SlowTime = 4f;
					this.eventTimer = 0.4f;
					this.FadeOut(this.eventTimer, Color.White);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer < 0f)
					{
						this.GetCharacter(CharacterType.LadyBoss).Exists = CharExists.Dead;
						this.map.leftBlock = (this.map.rightBlock = 0f);
						this.RelocateDust(new Vector2(4480f, 1300f) * 2f);
						this.character[0].Face = CharDir.Left;
						this.SpawnCharacter(new Vector2(4280f, 1300f) * 2f, "lady", CharacterType.Lady, Team.Friendly, ground: true);
						this.FaceCharacter(CharacterType.Lady, CharacterType.Dust, faceTowards: true);
						Game1.camera.ResetCamera(this.character);
						Game1.camera.camOffset.X = 200f;
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer < 0f)
					{
						this.eventTimer = 3f;
						this.FadeIn(this.eventTimer, Color.White);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.eventTimer < 0f)
					{
						this.StartDialogue(240, CharacterType.Lady);
						Music.Play("beauty");
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 245:
				if (this.subEvent == 0)
				{
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 250:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.RemoveCharacter(CharacterType.MaMop);
					this.RelocateCharacter(CharacterType.Bopo, new Vector2(3200f, 1840f) * 2f);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.ApproachLoc(CharacterType.Bopo, this.character[0].Location, 200, null);
					if (this.ApproachLoc(CharacterType.Dust, this.GetCharacter(CharacterType.Bopo).Location, 200, null))
					{
						this.StartDialogue(250, CharacterType.Bopo);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.DialogueOver())
					{
						Sound.PlayCue("chest_open", new Vector2(3430f, 1850f) * 2f, 300f);
						this.SpawnCharacter(new Vector2(3620f, 1870f) * 2f, "mamop", CharacterType.MaMop, Team.Friendly, ground: true);
						this.FaceCharacter(CharacterType.Dust, CharacterType.MaMop, faceTowards: true);
						this.FaceCharacter(CharacterType.Bopo, CharacterType.MaMop, faceTowards: true);
						this.FaceCharacter(CharacterType.MaMop, CharacterType.Bopo, faceTowards: true);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.ApproachLoc(CharacterType.MaMop, this.GetCharacter(CharacterType.Bopo).Location, 200, null) || this.ApproachLoc(CharacterType.MaMop, this.GetCharacter(CharacterType.Dust).Location, 200, null))
					{
						Sound.FadeMusicOut(5f);
						this.StartDialogue(251, CharacterType.Bopo);
						this.FadeOut(4f, Color.Black);
						this.eventTimer = 4f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					if (this.DialogueOver() && this.screenFade.A >= byte.MaxValue)
					{
						this.map.SwitchMap(this.pMan, this.character, "cave12b", loading: false);
						this.RelocateDust(new Vector2(1640f, 1260f));
						this.SpawnCharacter(new Vector2(1790f, 1260f), "ma", CharacterType.MaMop, Team.Friendly, ground: true);
						this.SpawnCharacter(new Vector2(2060f, 1260f), "bopo", CharacterType.Bopo, Team.Friendly, ground: true);
						this.FaceLoc(CharacterType.Dust, new Vector2(5000f, 0f), faceTowards: true);
						this.FaceLoc(CharacterType.MaMop, new Vector2(5000f, 0f), faceTowards: true);
						this.FaceLoc(CharacterType.Bopo, new Vector2(5000f, 0f), faceTowards: true);
						this.GetCharacter(CharacterType.Bopo).SetAnim("cry", 0, 0);
						Music.Play("beauty");
						this.eventTimer = 6f;
						this.FadeIn(6f, Color.Black);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(255, CharacterType.Bopo);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 6)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 6f;
						this.FadeOut(6f, Color.Black);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 7)
				{
					if (this.eventTimer <= 0f)
					{
						this.map.SwitchMap(this.pMan, this.character, "cave12", loading: false);
						this.RelocateDust(new Vector2(3510f, 1880f) * 2f);
						this.RemoveCharacter(CharacterType.MaMop);
						this.RemoveCharacter(CharacterType.Bopo);
						this.eventTimer = 3f;
						this.FadeIn(3f, Color.Black);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 8)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(260, CharacterType.Bopo);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 9)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 5f;
						this.FadeOut(4f, Color.Black);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 10)
				{
					if (this.eventTimer <= 0f)
					{
						this.map.SwitchMap(this.pMan, this.character, "cave12", loading: false);
						this.RelocateDust(new Vector2(4690f, 620f) * 2f);
						this.RelocateCharacter(CharacterType.OldGappy, new Vector2(5040f, 640f) * 2f);
						this.FaceCharacter(CharacterType.OldGappy, CharacterType.Dust, faceTowards: true);
						this.FaceCharacter(CharacterType.Dust, CharacterType.OldGappy, faceTowards: true);
						this.RelocateCharacter(CharacterType.Bopo, new Vector2(4800f, 620f) * 2f);
						this.FaceCharacter(CharacterType.Bopo, CharacterType.OldGappy, faceTowards: true);
						this.GetCharacter(CharacterType.Bopo).SetAnim("sad", 0, 0);
						this.RelocateCharacter(CharacterType.MaMop, new Vector2(4840f, 620f) * 2f);
						this.FaceCharacter(CharacterType.MaMop, CharacterType.OldGappy, faceTowards: true);
						this.SetEventCamera(new Vector2(5000f, 570f) * 2f, snapToLocation: true);
						this.eventTimer = 6f;
						this.FadeIn(this.eventTimer, Color.Black);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 11)
				{
					if (this.eventCamera.X > 8880f)
					{
						this.eventCamera.X -= Game1.FrameTime * 30f;
					}
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(265, CharacterType.Bopo);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 12)
				{
					if (this.eventCamera.X > this.character[0].Location.X)
					{
						this.eventCamera.X -= Game1.FrameTime * 30f;
					}
					if (this.DialogueOver())
					{
						this.GetCharacter(CharacterType.Bopo).SetAnim("idle00", 1, 1);
						this.ClearEvent();
						Sound.FadeMusicOut(4f);
					}
				}
				break;
			case 266:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					Music.Play("villagecave");
					this.ClearEvent();
				}
				break;
			case 270:
				Game1.worldScale = (Game1.camera.tempScale = 1.2f);
				Game1.worldScale *= Game1.hiDefScaleOffset;
				this.character[0].Face = CharDir.Right;
				if (this.subEvent == 0)
				{
					this.RelocateDust(new Vector2(255f, 830f) * 2f);
					this.FaceCharacter(CharacterType.Soldier, CharacterType.Gaius, faceTowards: true);
					this.FaceCharacter(CharacterType.Gaius, CharacterType.Soldier, faceTowards: true);
					this.SetEventCamera((this.GetCharacter(CharacterType.Soldier).Location + this.GetCharacter(CharacterType.Gaius).Location) / 2f - new Vector2(0f, (float)Game1.screenHeight * 0.3f / Game1.hiDefScaleOffset), snapToLocation: true);
					Music.Play("silent");
					this.skippable = SkipType.DialogueOnly;
					this.eventTimer = 4f;
					this.FadeIn(this.eventTimer, Color.Black);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer < 0f)
					{
						this.StartDialogue(605, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.DialogueOver())
					{
						this.GetCharacter(CharacterType.Soldier).Name = "soldier";
						this.GetCharacter(CharacterType.Soldier).SetAnim("idle00", 0, 2);
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.eventTimer < 0f)
					{
						this.SetEventCamera(this.GetCharacter(CharacterType.Gaius).Location - new Vector2(0f, (float)Game1.screenHeight * 0.3f / Game1.hiDefScaleOffset), snapToLocation: false);
						this.StartDialogue(606, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					this.ApproachLoc(CharacterType.Soldier, Vector2.Zero, 0, null);
					if (this.DialogueOver())
					{
						this.eventTimer = 1f;
						this.FadeOut(this.eventTimer, Color.Black);
						this.subEvent++;
					}
				}
				else if (this.eventTimer < 0f)
				{
					this.ClearEvent();
					this.map.WorldMapFromEvent(this.pMan, this.character, "cave13", TransitionDirection.Left, "lava");
				}
				break;
			case 300:
				this.ApproachLoc(CharacterType.Avgustin, this.character[0].Location, 200, null);
				if (this.subEvent == 0)
				{
					if (this.ApproachLoc(CharacterType.Dust, this.GetCharacter(CharacterType.Avgustin).Location, 200, null))
					{
						this.StartDialogue(240, CharacterType.Avgustin);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 301:
				Sound.PlayCue("spook_grave_00");
				this.ClearEvent();
				break;
			case 302:
				Sound.PlayCue("spook_grave_02");
				this.ClearEvent();
				break;
			case 310:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.Unskippable;
					Sound.FadeMusicOut(1.5f);
					this.SpawnCharacter(new Vector2(2700f, 2290f) * 2f, "miniboss", CharacterType.Psylph, Team.Enemy, ground: true);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.ApproachLoc(CharacterType.Dust, new Vector2(2100f, 0f) * 2f, 10, null))
					{
						this.map.leftBlock = 2000f;
						this.map.rightBlock = 7140f;
						this.eventTimer = 1.5f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer < 1f)
					{
						this.SetEventCamera(this.GetCharacter(CharacterType.Psylph).Location + new Vector2(-200f, -200f), snapToLocation: false);
						Music.Play("boss3");
					}
					if (this.eventTimer <= 0f)
					{
						this.GetCharacter(CharacterType.Psylph).SetLunge(LungeStates.Lunging, 1200f, 1200f);
						this.eventTimer = 0.8f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					this.SetEventCamera(this.GetCharacter(CharacterType.Psylph).Location + new Vector2(-200f, -200f), snapToLocation: false);
					if (this.eventTimer < 0f)
					{
						this.character[0].Evade(this.pMan);
						this.character[0].Trajectory.X *= 0.9f;
						Game1.hud.InitBoss(this.character, "miniboss");
						this.eventTimer = 0.5f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					this.SetEventCamera(this.GetCharacter(CharacterType.Psylph).Location + new Vector2(-200f, -200f), snapToLocation: false);
					if (this.eventTimer < 0f)
					{
						this.skippable = SkipType.Skippable;
						this.StartDialogue(310, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 311:
				Music.Play("grave");
				this.ClearEvent();
				break;
			case 320:
				if (this.subEvent == 0)
				{
					if (this.GetCharacter(CharacterType.Cora) == null)
					{
						this.SpawnCharacter(new Vector2(2660f, 2270f) * 2f, "cora", CharacterType.Cora, Team.Friendly, ground: true);
					}
					Game1.hud.InitHelp("multipledest", restart: true, -1);
					this.StartDialogue(320, CharacterType.Cora);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.ApproachLoc(CharacterType.Dust, this.GetCharacter(CharacterType.Cora).Location, 200, null);
					this.ApproachLoc(CharacterType.Cora, this.character[0].Location, 200, null);
					if (this.DialogueOver())
					{
						this.ClearEvent();
					}
				}
				break;
			case 328:
				Sound.PlayCue("spook_grave_02");
				VibrationManager.Rumble(Game1.currentGamePad, 0.5f);
				this.ClearEvent();
				break;
			case 329:
				Sound.PlayCue("spook_grave_04");
				this.ClearEvent();
				break;
			case 330:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.eventTimer = 1f;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer <= 0f)
					{
						Music.Play("mansionchase");
						this.SpawnCharacter(new Vector2(3300f, 2900f) * 2f, "ghost", CharacterType.KaneGhostChase, Team.Enemy, ground: false);
						this.GetCharacter(CharacterType.KaneGhostChase).Ai.overrideAI = true;
						this.subEvent++;
						this.eventTimer = 3f;
					}
				}
				else if (this.subEvent == 2)
				{
					this.GetCharacter(CharacterType.KaneGhostChase).Ai.jobType = JobType.RunLeft;
					this.SetEventCamera(this.GetCharacter(CharacterType.KaneGhostChase).Location + new Vector2(0f, -200f), snapToLocation: false);
					if (this.eventTimer <= 0f)
					{
						this.SetEventCamera(Vector2.Zero, snapToLocation: false);
						this.StartDialogue(currentE, CharacterType.Dust);
						this.map.doorRegions.Clear();
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 340:
				if (this.subEvent == 0)
				{
					if (this.character[0].Location.Y < 1600f)
					{
						this.character[0].KeyLeft = true;
					}
					else if (this.character[0].State == CharState.Grounded)
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 1)
				{
					this.StartDialogue(340, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 342:
				if (this.subEvent == 0)
				{
					this.StartDialogue(342, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.pMan.AddUpgrade(new Vector2(5360f, 1070f) * 2f, 2016, 5);
					this.FaceLoc(CharacterType.Dust, this.character[0].Location + new Vector2(200f, 0f), faceTowards: true);
					this.ClearEvent();
				}
				break;
			case 345:
				this.ClearEvent();
				break;
			case 370:
				if (this.subEvent == 0)
				{
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 380:
				if (this.subEvent == 0)
				{
					Game1.wManager.SetWeather(WeatherType.RainLight, forceReset: true);
					Game1.wManager.randomType = RandomType.None;
					this.StartDialogue(currentE, CharacterType.Cora);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.ApproachLoc(CharacterType.Cora, this.character[0].Location, 200, null);
					if (this.DialogueOver())
					{
						this.eventTimer = 3f;
						Sound.FadeMusicOut(this.eventTimer);
						this.FadeOut(this.eventTimer, Color.White);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					this.ApproachLoc(CharacterType.Cora, this.character[0].Location, 200, null);
					VibrationManager.SetScreenShake(1f);
					if (this.eventTimer < 0f)
					{
						this.RemoveCharacter(CharacterType.Cora);
						this.RelocateDust(new Vector2(3300f, 2280f) * 2f);
						this.SpawnCharacter(new Vector2(3300f, 2100f) * 2f, "boss", CharacterType.KaneGhost, Team.Enemy, ground: true);
						this.GetCharacter(CharacterType.KaneGhost).SetAnim("spawn", 0, 0);
						this.map.leftBlock = 4400f;
						this.map.rightBlock = 8400f;
						this.eventTimer = 3f;
						this.FadeIn(this.eventTimer, Color.White);
						Music.Play("boss1");
						this.character[0].SetAnim("idle00", 0, 0);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.eventTimer < 0f)
					{
						this.eventTimer = 5f;
						this.safetyOn = false;
						this.subEvent++;
					}
				}
				else
				{
					if (this.subEvent != 4)
					{
						break;
					}
					if (this.eventTimer < 4f && this.eventTimer > 1f)
					{
						this.character[0].KeyDown = true;
						if (Rand.GetRandomInt(0, 6) == 0)
						{
							this.pMan.AddCoraProjectile(this.character[0].Location + new Vector2(0f, -100f), new Vector2(Rand.GetRandomInt(1000, 2200) * ((Rand.GetRandomInt(0, 2) != 0) ? 1 : (-1)), 200f), 5);
						}
					}
					if (this.eventTimer < 0f)
					{
						Game1.hud.InitBoss(this.character, "boss");
						this.ClearEvent();
					}
				}
				break;
			case 390:
				if (this.subEvent == 0)
				{
					this.character[0].StatusTime = 0f;
					Game1.awardsManager.EarnAchievement(Achievement.DefeatKane, forceCheck: false);
					this.skippable = SkipType.DialogueOnly;
					Sound.FadeMusicOut(1f);
					Game1.SlowTime = 4f;
					this.eventTimer = 0.4f;
					this.FadeOut(this.eventTimer, Color.White);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer < 0f)
					{
						for (int num27 = 1; num27 < this.character.Length; num27++)
						{
							this.character[num27].Exists = CharExists.Dead;
						}
						this.pMan.Reset(removeWeather: false, removeBombs: false);
						this.pMan.AddFidget(this.character[0].Location);
						this.map.leftBlock = (this.map.rightBlock = 0f);
						this.RelocateDust(new Vector2(3200f, 2300f) * 2f);
						this.character[0].Face = CharDir.Right;
						this.SpawnCharacter(new Vector2(3360f, 2300f) * 2f, "kane", CharacterType.Kane, Team.Friendly, ground: true);
						this.FaceCharacter(CharacterType.Kane, CharacterType.Dust, faceTowards: true);
						this.SpawnCharacter(new Vector2(3100f, 2300f) * 2f, "cora", CharacterType.Cora, Team.Friendly, ground: true);
						Game1.camera.ResetCamera(this.character);
						Game1.camera.camOffset.X = -100f;
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer < 0f)
					{
						this.eventTimer = 3f;
						this.FadeIn(this.eventTimer, Color.White);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.eventTimer < 0f)
					{
						this.StartDialogue(currentE, CharacterType.Kane);
						Music.Play("beauty");
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 400:
				this.ClearEvent();
				break;
			case 410:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					Sound.PlayCue("avalanche_start", new Vector2(-180f, 1500f) * 2f, 400f);
					VibrationManager.SetScreenShake(2f);
					this.eventTimer = 1f;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.pMan.AddSpray(this.character[0].Location + Rand.GetRandomVector2(-1000f, 1000f, -900f, -600f), new Vector2(0f, 200f), 1f, 3, 5, 7);
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(410, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.DialogueOver())
					{
						VibrationManager.SetScreenShake(2f);
						Sound.PlayCue("avalanche_start", new Vector2(-180f, 1500f) * 2f, 400f);
						this.pMan.AddAvalanche(new Vector2(-180f, 1500f) * 2f, CharDir.Right, 400, 24, 6);
						this.eventTimer = 4f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(412, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 420:
				if (this.subEvent == 0)
				{
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 425:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.Unskippable;
					Sound.FadeMusicOut(3f);
					this.SpawnCharacter(new Vector2(6000f, 2800f) * 2f, "miniboss", CharacterType.Frite, Team.Enemy, ground: true);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.ApproachLoc(CharacterType.Dust, new Vector2(5400f, 0f) * 2f, 10, null))
					{
						this.map.leftBlock = 9400f;
						this.map.rightBlock = 11960f;
						this.eventTimer = 3f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer < 1f)
					{
						this.SetEventCamera(this.GetCharacter("miniboss").Location + new Vector2(-200f, -200f), snapToLocation: false);
						Music.Play("boss2");
					}
					if (this.eventTimer <= 0f)
					{
						this.GetCharacter("miniboss").SetAnim("flyattack", 0, 1);
						this.eventTimer = 2.3f;
						this.subEvent++;
					}
				}
				else
				{
					this.SetEventCamera(this.GetCharacter("miniboss").Location + new Vector2(-200f, -200f), snapToLocation: false);
					if (this.eventTimer <= 0f)
					{
						this.character[0].Evade(this.pMan);
						this.character[0].Trajectory.X *= 0.9f;
						Game1.hud.InitBoss(this.character, "miniboss");
						this.ClearEvent();
					}
				}
				break;
			case 426:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.eventTimer = 1f;
					Sound.FadeMusicOut(this.eventTimer);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer < 0f)
					{
						this.StartDialogue(currentE, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					Music.Play("snow");
					this.ClearEvent();
				}
				break;
			case 440:
				if (this.subEvent == 0)
				{
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 445:
				if (this.subEvent == 0)
				{
					Vector2 vector = new Vector2(this.character[0].Location.X - 600f, 2880f);
					for (int n = 0; n < 3; n++)
					{
						Vector2 vector2 = vector + Rand.GetRandomVector2(-200f, 100f, -400f, 0f);
						this.pMan.AddIcicle(vector2, new Vector2(0f, 200f), Rand.GetRandomFloat(0.25f, 0.75f), 1, 6);
						for (int num = 0; num < 5; num++)
						{
							this.pMan.AddSpray(vector2 + new Vector2(Rand.GetRandomInt(-40, 40), -50f), new Vector2(0f, Rand.GetRandomInt(0, 200)), 0.5f, 2, 1, 5);
						}
					}
					Sound.PlayCue("icicle_release", vector, (vector - Game1.character[0].Location).Length());
					this.eventTimer = 2f;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(445, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 460:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.Unskippable;
					this.SpawnCharacter(new Vector2(1440f, 1600f) * 2f, "miniboss", CharacterType.Frite, Team.Enemy, ground: false);
					this.SpawnCharacter(new Vector2(1980f, 1440f) * 2f, "miniboss", CharacterType.Frite, Team.Enemy, ground: false);
					this.SpawnCharacter(new Vector2(2180f, 1630f) * 2f, "miniboss", CharacterType.Frite, Team.Enemy, ground: false);
					this.map.leftBlock = 2300f;
					this.map.rightBlock = 5200f;
					Game1.hud.InitBoss(this.character, "miniboss");
					Music.Play("boss2");
					this.eventTimer = 1f;
					this.subEvent++;
				}
				else if (this.eventTimer < 0f)
				{
					this.ClearEvent();
				}
				break;
			case 465:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					Sound.FadeMusicOut(2f);
					Sound.PlayCue("frite_echo_scream");
					this.map.leftBlock = (this.map.rightBlock = 0f);
					this.eventTimer = 3f;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.SetEventCamera(new Vector2(2400f, 1530f) * 2f, snapToLocation: false);
					this.pMan.AddShockRing(Rand.GetRandomVector2(2300f, 2700f, 1300f, 1700f) * 2f, 0.3f, 5);
					if (this.eventTimer < 0f)
					{
						for (int num35 = 0; num35 < Game1.dManager.destructWalls.Count; num35++)
						{
							Game1.dManager.destructWalls[num35].KillMe(this.pMan, CharDir.Right);
						}
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer < 0f)
					{
						this.SetEventCamera(Vector2.Zero, snapToLocation: false);
						Music.Play("snow");
						this.StartDialogue(currentE, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 470:
				if (this.subEvent == 0)
				{
					this.StartDialogue(471, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 475:
				if (this.subEvent == 0)
				{
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 480:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.map.leftBlock = 200f;
					this.map.rightBlock = 3600f;
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.DialogueOver())
					{
						this.SpawnCharacter(new Vector2(100f, 1180f) * 2f, "miniboss", CharacterType.Soldier, Team.Enemy, ground: true);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					this.SetEventCamera((this.GetCharacter(CharacterType.Soldier).Location + this.character[0].Location) / 2f + new Vector2(0f, (float)(-Game1.screenHeight) * 0.35f / Game1.hiDefScaleOffset), snapToLocation: false);
					this.FaceCharacter(CharacterType.Dust, CharacterType.Soldier, faceTowards: true);
					if (this.ApproachLoc(CharacterType.Soldier, this.character[0].Location + new Vector2((this.character[0].Face == CharDir.Left) ? (-400) : 400, 0f), 0, null))
					{
						this.FaceCharacter(CharacterType.Soldier, CharacterType.Dust, faceTowards: true);
						this.StartDialogue(481, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					for (int num17 = 0; num17 < 2; num17++)
					{
						this.SpawnCharacter(new Vector2(1470f, 1180f) * 2f, "miniboss", CharacterType.Soldier, Team.Enemy, ground: true);
					}
					Game1.hud.InitBoss(this.character, "miniboss");
					this.ClearEvent();
				}
				break;
			case 485:
				if (this.subEvent == 0)
				{
					this.map.leftBlock = (this.map.rightBlock = 0f);
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 490:
			case 495:
			case 496:
				if (this.subEvent == 0)
				{
					if (currentE == 490)
					{
						this.map.leftBlock = 1800f;
						this.map.rightBlock = 10800f;
					}
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 500:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.eventTimer = 2f;
					this.FadeOut(this.eventTimer, Color.Black);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer <= 0f)
					{
						this.eventTimer = 1f;
						this.FadeIn(this.eventTimer, Color.Black);
						this.SpawnCharacter(new Vector2(900f, 740f) * 2f, "ginger", CharacterType.Ginger, Team.Friendly, ground: true);
						this.SpawnCharacter(new Vector2(980f, 740f) * 2f, "elder", CharacterType.Elder, Team.Friendly, ground: true);
						this.FaceCharacter(CharacterType.Dust, CharacterType.Ginger, faceTowards: true);
						this.FaceCharacter(CharacterType.Ginger, CharacterType.Dust, faceTowards: true);
						this.FaceCharacter(CharacterType.Elder, CharacterType.Dust, faceTowards: true);
						Game1.camera.ResetCamera(this.character);
						Game1.cutscene.InitCutscene(50);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(500, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 2f;
						this.FadeOut(this.eventTimer, Color.Black);
						Sound.FadeMusicOut(this.eventTimer);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					if (this.eventTimer <= 0f)
					{
						this.eventTimer = 1f;
						this.FadeIn(this.eventTimer, Color.Black);
						Game1.cutscene.InitCutscene(60);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					if (this.eventTimer <= 0f)
					{
						Music.Play("souls");
						this.StartDialogue(501, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 6)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 4f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 7)
				{
					this.ApproachLoc(CharacterType.Ginger, new Vector2(4000f, 2000f), 0, null);
					this.ApproachLoc(CharacterType.Elder, new Vector2(4000f, 2000f), 0, null);
					if (this.eventTimer <= 0f)
					{
						this.RemoveCharacter(CharacterType.Ginger);
						this.RemoveCharacter(CharacterType.Elder);
						Game1.awardsManager.EarnAvatarAward(new string[1] { "AvatarAwards3" });
						this.ClearEvent();
					}
				}
				break;
			case 505:
				Game1.worldScale = (Game1.camera.tempScale = 1.2f);
				Game1.worldScale *= Game1.hiDefScaleOffset;
				this.character[0].Face = CharDir.Right;
				if (this.subEvent == 0)
				{
					this.RelocateDust(new Vector2(255f, 830f) * 2f);
					this.FaceCharacter(CharacterType.Soldier, CharacterType.Gaius, faceTowards: true);
					this.FaceCharacter(CharacterType.Gaius, CharacterType.Soldier, faceTowards: true);
					this.SetEventCamera((this.GetCharacter(CharacterType.Soldier).Location + this.GetCharacter(CharacterType.Gaius).Location) / 2f - new Vector2(0f, (float)Game1.screenHeight * 0.3f / Game1.hiDefScaleOffset), snapToLocation: true);
					Music.Play("silent");
					this.skippable = SkipType.DialogueOnly;
					this.eventTimer = 4f;
					this.FadeIn(this.eventTimer, Color.Black);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer < 0f)
					{
						this.StartDialogue(610, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.DialogueOver())
					{
						this.GetCharacter(CharacterType.Soldier).Name = "soldier";
						this.GetCharacter(CharacterType.Soldier).SetAnim("idle00", 0, 2);
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.eventTimer < 0f)
					{
						this.SetEventCamera(this.GetCharacter(CharacterType.Gaius).Location - new Vector2(0f, (float)Game1.screenHeight * 0.3f / Game1.hiDefScaleOffset), snapToLocation: false);
						this.StartDialogue(611, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					this.ApproachLoc(CharacterType.Soldier, Vector2.Zero, 0, null);
					if (this.DialogueOver())
					{
						this.eventTimer = 1f;
						this.FadeOut(this.eventTimer, Color.Black);
						this.subEvent++;
					}
				}
				else if (this.eventTimer < 0f)
				{
					this.ClearEvent();
					this.map.WorldMapFromEvent(this.pMan, this.character, "snow27", TransitionDirection.Left, "lava");
					Game1.navManager.RevealCell("lava01");
				}
				break;
			case 510:
				if (this.subEvent == 0)
				{
					EventManager.hotID = this.character.Length - 1;
					this.SpawnIDCharacter(new Vector2(1920f, 2190f) * 2f, EventManager.hotID, "guard", CharacterType.Moonblood, Team.Friendly, ground: true);
					this.character[EventManager.hotID].RandomSkin = 2;
					this.character[EventManager.hotID].NPC = NPCType.Friendly;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.ApproachLoc("guard", this.character[0].Location, 300, null))
					{
						this.character[EventManager.hotID].NPC = NPCType.None;
						this.StartDialogue(currentE, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.character[EventManager.hotID].Name = "runner";
					this.ClearEvent();
				}
				break;
			case 520:
				if (this.subEvent == 0)
				{
					this.StartDialogue(0, CharacterType.Elder);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.ApproachLoc(CharacterType.Elder, this.character[0].Location, 200, null);
					if (this.DialogueOver())
					{
						this.ClearEvent();
					}
				}
				break;
			case 530:
				if (this.subEvent == 0)
				{
					Sound.PlayCue("spook_assassin");
					VibrationManager.Rumble(Game1.currentGamePad, 0.5f);
					this.eventTimer = 1f;
					this.subEvent++;
				}
				if (this.subEvent == 1)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(currentE, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 532:
				Sound.PlayCue("spook_assassin");
				VibrationManager.Rumble(Game1.currentGamePad, 0.5f);
				this.ClearEvent();
				break;
			case 534:
				Sound.PlayCue("spook_assassin");
				VibrationManager.Rumble(Game1.currentGamePad, 0.5f);
				this.ClearEvent();
				break;
			case 540:
				if (this.subEvent == 0)
				{
					Game1.questManager.AddNote(10, loading: false);
					this.skippable = SkipType.DialogueOnly;
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.FaceLoc(CharacterType.Dust, Vector2.Zero, faceTowards: true);
					this.ClearEvent();
					Music.Play("villagelava");
				}
				break;
			case 560:
				EventManager.hotID = this.character.Length - 1;
				if (this.subEvent == 0)
				{
					this.SpawnIDCharacter(new Vector2(2850f, 1820f) * 2f, EventManager.hotID, "idle", CharacterType.Moonblood, Team.Friendly, ground: true);
					this.character[EventManager.hotID].RandomSkin = 2;
					this.character[EventManager.hotID].Face = CharDir.Left;
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else
				{
					this.character[EventManager.hotID].Face = CharDir.Left;
					if (this.DialogueOver())
					{
						this.ClearEvent();
					}
				}
				break;
			case 570:
				this.ClearEvent();
				break;
			case 580:
				if (this.subEvent == 0)
				{
					for (int num24 = 1; num24 < this.character.Length; num24++)
					{
						if (this.character[num24].Exists == CharExists.Exists && this.character[num24].Definition.charType == CharacterType.Moonblood)
						{
							this.character[num24].Face = CharDir.Right;
						}
					}
					this.StartDialogue(currentE, CharacterType.Dust);
					Sound.FadeMusicOut(8f);
					this.subEvent++;
				}
				else
				{
					if (!this.DialogueOver())
					{
						break;
					}
					for (int num25 = 1; num25 < this.character.Length; num25++)
					{
						if (this.character[num25].Exists == CharExists.Exists && this.character[num25].Definition.charType == CharacterType.Moonblood)
						{
							this.character[num25].Name = "runner";
						}
					}
					this.ClearEvent();
				}
				break;
			case 600:
				this.ClearEvent();
				break;
			case 620:
				EventManager.hotID = this.character.Length - 1;
				if (this.subEvent == 0)
				{
					this.SpawnIDCharacter(new Vector2(6270f, 1670f) * 2f, EventManager.hotID, "guard", CharacterType.Moonblood, Team.Friendly, ground: true);
					this.character[EventManager.hotID].RandomSkin = 3;
					this.character[EventManager.hotID].NPC = NPCType.Friendly;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.ApproachLoc("guard", this.character[0].Location, 300, null))
					{
						this.GetCharacter("guard").Trajectory.X *= 0.1f;
						this.character[EventManager.hotID].NPC = NPCType.None;
						this.StartDialogue(650, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.character[EventManager.hotID].Name = "runner";
					this.ClearEvent();
				}
				break;
			case 640:
				if (this.subEvent == 0)
				{
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 660:
				if (this.subEvent == 0)
				{
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 680:
				Game1.navManager.RevealCell("lava21");
				Game1.navManager.RevealCell("lava22");
				this.ClearEvent();
				break;
			case 700:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.SpawnCharacter(new Vector2(2200f, 670f) * 2f, "gaiusboss", CharacterType.Gaius, Team.Enemy, ground: true);
					this.GetCharacter(CharacterType.Gaius).Face = CharDir.Left;
					this.GetCharacter(CharacterType.Gaius).SetAnim("atease", 0, 0);
					for (int num5 = 0; num5 < 2; num5++)
					{
						this.SpawnCharacter(new Vector2(5000 + num5 * 100, 1280f), "soldier" + num5, CharacterType.Soldier, Team.Enemy, ground: true);
					}
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					EventManager.hotSpot = new Vector2(3780f, 0f);
					this.ApproachLoc("soldier0", EventManager.hotSpot, 100, null);
					this.ApproachLoc("soldier1", EventManager.hotSpot, 240, null);
					if (this.ApproachLoc(CharacterType.Dust, EventManager.hotSpot, 150, null))
					{
						this.StartDialogue(currentE, CharacterType.Dust);
						this.map.leftBlock = 2360f;
						this.map.rightBlock = 5120f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					this.SetEventCamera((this.character[0].Location + this.GetCharacter(CharacterType.Gaius).Location) / 2f - new Vector2(0f, (float)Game1.screenHeight * 0.3f / Game1.hiDefScaleOffset), snapToLocation: false);
					if (this.ApproachLoc("soldier0", EventManager.hotSpot, 100, null) && this.ApproachLoc("soldier1", EventManager.hotSpot, 240, null) && this.DialogueOver())
					{
						this.GetCharacter(CharacterType.Gaius).SetAnim("idle00", 0, 2);
						Game1.cutscene.InitCutscene(100);
						this.SetEventCamera(Vector2.Zero, snapToLocation: false);
						this.eventTimer = 0.5f;
						this.FadeIn(this.eventTimer, Color.Black);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.eventTimer < 0.2f && this.GetCharacter("soldier0").Location.X > this.character[0].Location.X)
					{
						this.GetCharacter("soldier0").SetLunge(LungeStates.Lunging, 1200f, 800f);
					}
					if (this.eventTimer < 0f)
					{
						if (this.GetCharacter("soldier1").Location.X > this.character[0].Location.X)
						{
							this.GetCharacter("soldier1").SetLunge(LungeStates.Lunging, 1200f, 800f);
						}
						Game1.hud.InitBoss(this.character, "gaiusboss");
						Music.Play("gaius");
						this.eventTimer = 1f;
						this.subEvent++;
					}
					this.FaceCharacter("soldier0", CharacterType.Dust, faceTowards: true);
					this.FaceCharacter("soldier1", CharacterType.Dust, faceTowards: true);
				}
				else if (this.subEvent == 4)
				{
					this.FaceCharacter("soldier0", CharacterType.Dust, faceTowards: true);
					this.FaceCharacter("soldier1", CharacterType.Dust, faceTowards: true);
					if (this.eventTimer < 0f)
					{
						this.ClearEvent();
					}
				}
				break;
			case 710:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					Game1.SlowTime = 2f;
					if (!(this.GetCharacter(CharacterType.Gaius).Location.X > 5000f))
					{
						break;
					}
					int num36 = Game1.map.GetMapLayer()[7].mapSeg.Length;
					for (int num37 = 0; num37 < num36; num37++)
					{
						if (Game1.map.GetMapLayer()[7].mapSeg[num37] != null && Game1.map.GetMapLayer()[7].mapSeg[num37].SourceIndex == 19 && Game1.map.GetMapLayer()[7].mapSeg[num37].Index == 1)
						{
							Game1.map.GetMapLayer()[7].mapSeg[num37].color.A = 0;
						}
					}
					Vector2 vector9 = new Vector2(5200f, this.GetCharacter(CharacterType.Gaius).Location.Y);
					this.pMan.AddShockRing(vector9, 2f, 5);
					VibrationManager.SetScreenShake(1f);
					VibrationManager.SetBlast(1.5f, vector9);
					Game1.map.MapSegFrameSpeed = 0.4f;
					Sound.PlayCue("gaius_wall_destroy");
					for (int num38 = 0; num38 < 30; num38++)
					{
						this.pMan.AddBrickWall(new Vector2(4960f, 840f) + Rand.GetRandomVector2(0f, 700f, 0f, 700f), Rand.GetRandomVector2(-200f, 1000f, -800f, 100f), 1f, Rand.GetRandomInt(5, 7));
					}
					for (int num39 = 0; num39 < 300; num39++)
					{
						this.pMan.AddBlood(new Vector2(4960f, 840f) + Rand.GetRandomVector2(0f, 700f, 0f, 700f), Rand.GetRandomVector2(-200f, 1000f, -800f, 100f), 1f, 1f, 0.5f, 1f, 2f, (CharacterType)1000, 0, Rand.GetRandomInt(5, 7));
					}
					this.map.rightBlock = 9600f;
					this.eventTimer = 1f;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer > 0f)
					{
						this.SetEventCamera(this.GetCharacter(CharacterType.Gaius).Location - new Vector2(0f, (float)Game1.screenHeight * 0.3f / Game1.hiDefScaleOffset), snapToLocation: false);
						break;
					}
					this.SetEventCamera((this.character[0].Location + this.GetCharacter(CharacterType.Gaius).Location) / 2f - new Vector2(0f, (float)Game1.screenHeight * 0.3f / Game1.hiDefScaleOffset), snapToLocation: false);
					bool flag = false;
					if (this.ApproachLoc(CharacterType.Gaius, new Vector2(6900f, 0f), 10, null))
					{
						this.FaceCharacter(CharacterType.Gaius, CharacterType.Dust, faceTowards: true);
						flag = true;
					}
					if (this.ApproachLoc(CharacterType.Dust, new Vector2(6400f, 0f), 10, null) && flag)
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					this.map.leftBlock = 5480f;
					this.map.rightBlock = 9600f;
					this.SetEventCamera(Vector2.Zero, snapToLocation: false);
					this.StartDialogue(currentE, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					Game1.hud.InitBoss(this.character, "gaiusboss");
					this.ClearEvent();
				}
				break;
			case 720:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.map.rightBlock = 9200f;
					if (this.GetCharacter(CharacterType.Gaius).State != 0)
					{
						break;
					}
					Game1.SlowTime = 1f;
					int num30 = Game1.map.GetMapLayer()[5].mapSeg.Length;
					for (int num31 = 0; num31 < num30; num31++)
					{
						if (Game1.map.GetMapLayer()[5].mapSeg[num31] != null && Game1.map.GetMapLayer()[5].mapSeg[num31].SourceIndex == 16 && Game1.map.GetMapLayer()[5].mapSeg[num31].Index == 1)
						{
							Game1.map.GetMapLayer()[5].mapSeg[num31].color.A = 0;
						}
					}
					this.map.maxPlayerLedges -= 3;
					Vector2 location4 = this.GetCharacter(CharacterType.Gaius).Location;
					Game1.SlowTime = 0.5f;
					this.pMan.AddShockRing(location4, 2f, 5);
					VibrationManager.SetScreenShake(1f);
					VibrationManager.SetBlast(1.5f, location4);
					Game1.map.MapSegFrameSpeed = 0.4f;
					Sound.PlayCue("gaius_wall_destroy");
					Rectangle rectangle2 = new Rectangle(-200, 0, Game1.screenWidth + 400, Game1.screenHeight);
					for (int num32 = 0; num32 < 200; num32++)
					{
						Vector2 vector7 = new Vector2(6400f, 1720f) + Rand.GetRandomVector2(0f, 2000f, 0f, 280f);
						Vector2 vector8 = vector7 * Game1.worldScale - Game1.Scroll;
						if (rectangle2.Contains((int)vector8.X, (int)vector8.Y))
						{
							this.pMan.AddBlood(vector7, Rand.GetRandomVector2(-400f, 400f, -500f, 20f), 1f, 1f, 1f, 1f, Rand.GetRandomFloat(3f, 5f), (CharacterType)1016, 0, Rand.GetRandomInt(5, 7));
						}
					}
					for (int num33 = 0; num33 < 100; num33++)
					{
						Vector2 vector7 = new Vector2(5500f, 1800f) + Rand.GetRandomVector2(0f, 900f, 0f, 240f);
						Vector2 vector8 = vector7 * Game1.worldScale - Game1.Scroll;
						if (rectangle2.Contains((int)vector8.X, (int)vector8.Y))
						{
							this.pMan.AddBlood(vector7, Rand.GetRandomVector2(-400f, 400f, -500f, 20f), 1f, 1f, 1f, 1f, Rand.GetRandomFloat(3f, 5f), (CharacterType)1016, 0, Rand.GetRandomInt(5, 7));
						}
					}
					for (int num34 = 0; num34 < this.character.Length; num34++)
					{
						if (this.character[num34].Exists == CharExists.Exists)
						{
							if (this.character[num34].State == CharState.Grounded)
							{
								this.character[num34].SetJump(1200f, jumped: true);
								this.character[num34].SetAnim("hurtup", 0, 0);
							}
							if (num34 > 0 && this.character[num34].Definition.charType != CharacterType.Gaius)
							{
								this.character[num34].KillMe(instantly: false);
							}
						}
					}
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.character[0].Trajectory.X *= 0.5f;
					if (this.character[0].State == CharState.Grounded)
					{
						this.eventTimer = 0.5f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer < 0f && this.ApproachLoc(CharacterType.Dust, this.GetCharacter(CharacterType.Gaius).Location, 500, null) && this.ApproachLoc(CharacterType.Gaius, this.GetCharacter(CharacterType.Dust).Location, 500, null))
					{
						this.FaceCharacter(CharacterType.Dust, CharacterType.Gaius, faceTowards: true);
						this.FaceCharacter(CharacterType.Gaius, CharacterType.Dust, faceTowards: true);
						this.StartDialogue(currentE, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					Game1.hud.InitBoss(this.character, "gaiusboss");
					this.ClearEvent();
				}
				break;
			case 740:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.Unskippable;
					if (this.GetCharacter(CharacterType.Gaius).State != 0)
					{
						break;
					}
					Game1.SlowTime = 1f;
					int num19 = Game1.map.GetMapLayer()[5].mapSeg.Length;
					for (int num20 = 0; num20 < num19; num20++)
					{
						if (Game1.map.GetMapLayer()[5].mapSeg[num20] != null && Game1.map.GetMapLayer()[5].mapSeg[num20].Location.Y > 5100f)
						{
							Game1.map.GetMapLayer()[5].mapSeg[num20].color.A = 0;
						}
					}
					this.map.maxPlayerLedges--;
					Vector2 location3 = this.GetCharacter(CharacterType.Gaius).Location;
					Game1.SlowTime = 0.5f;
					this.pMan.AddShockRing(location3, 2f, 5);
					VibrationManager.SetScreenShake(1f);
					VibrationManager.SetBlast(1.5f, location3);
					Game1.map.MapSegFrameSpeed = 0.4f;
					Sound.PlayCue("gaius_wall_destroy");
					Rectangle rectangle = new Rectangle(-200, 0, Game1.screenWidth + 400, Game1.screenHeight);
					for (int num21 = 0; num21 < 200; num21++)
					{
						Vector2 vector4 = new Vector2(5480f, 5600f) + Rand.GetRandomVector2(0f, 1960f, 0f, 280f);
						Vector2 vector5 = vector4 * Game1.worldScale - Game1.Scroll;
						if (rectangle.Contains((int)vector5.X, (int)vector5.Y))
						{
							this.pMan.AddBlood(vector4, Rand.GetRandomVector2(-400f, 400f, -500f, 20f), 1f, 1f, 1f, 1f, Rand.GetRandomFloat(3f, 5f), (CharacterType)1016, 0, Rand.GetRandomInt(5, 7));
						}
					}
					for (int num22 = 0; num22 < 200; num22++)
					{
						Vector2 vector4 = new Vector2(7440f, 5500f) + Rand.GetRandomVector2(0f, 1960f, 0f, 280f);
						Vector2 vector5 = vector4 * Game1.worldScale - Game1.Scroll;
						if (rectangle.Contains((int)vector5.X, (int)vector5.Y))
						{
							this.pMan.AddBlood(vector4, Rand.GetRandomVector2(-400f, 400f, -500f, 20f), 1f, 1f, 1f, 1f, Rand.GetRandomFloat(3f, 5f), (CharacterType)1016, 0, Rand.GetRandomInt(5, 7));
						}
					}
					for (int num23 = 0; num23 < this.character.Length; num23++)
					{
						if (this.character[num23].Exists == CharExists.Exists && this.character[num23].State == CharState.Grounded)
						{
							this.character[num23].SetJump(1200f, jumped: true);
							this.character[num23].SetAnim("hurtup", 0, 0);
							this.character[num23].Trajectory.X *= 0.2f;
						}
					}
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					Character character2 = this.GetCharacter(CharacterType.Gaius);
					if (character2 != null && character2.State == CharState.Grounded && this.map.GetTransVal() <= 0f)
					{
						this.map.AddColRow(33);
						this.eventTimer = 0.5f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer < 0f && this.ApproachLoc(CharacterType.Dust, this.GetCharacter(CharacterType.Gaius).Location, 500, null) && this.ApproachLoc(CharacterType.Gaius, this.GetCharacter(CharacterType.Dust).Location, 500, null))
					{
						this.FaceCharacter(CharacterType.Dust, CharacterType.Gaius, faceTowards: true);
						this.FaceCharacter(CharacterType.Gaius, CharacterType.Dust, faceTowards: true);
						this.skippable = SkipType.DialogueOnly;
						this.StartDialogue(currentE, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					Game1.hud.InitBoss(this.character, "gaiusboss");
					this.ClearEvent();
				}
				break;
			case 760:
				if (this.subEvent == 0)
				{
					Sound.FadeMusicOut(0.4f);
					this.skippable = SkipType.Unskippable;
					VibrationManager.ScreenShake.value = 0f;
					Game1.SlowTime = 4f;
					if (this.character[0].State == CharState.Air)
					{
						this.character[0].Trajectory.Y = Math.Max(this.character[0].Trajectory.Y, -100f);
					}
					Character character = this.GetCharacter(CharacterType.Gaius);
					if ((character.Location.X < 4080f || character.Location.X > 4700f) && this.character[0].State == CharState.Grounded)
					{
						Sound.PlayPersistCue("volcano_explode_1", Vector2.Zero);
						this.FaceCharacter(CharacterType.Dust, CharacterType.Gaius, faceTowards: true);
						this.eventTimer = 0.35f;
						this.subEvent++;
					}
				}
				if (this.subEvent == 1)
				{
					this.FaceCharacter(CharacterType.Dust, CharacterType.Gaius, faceTowards: true);
					if (this.eventTimer < 0f)
					{
						Sound.StopPersistCue("volcano_explode_1");
						Sound.PlayCue("volcano_explode_2");
						Music.Play("deathcrawl");
						Vector2 vector3 = this.character[0].Location + new Vector2((this.character[0].Face == CharDir.Left) ? 400 : (-400), 0f);
						this.pMan.AddShockRing(vector3, 2f, 5);
						VibrationManager.SetScreenShake(0.5f);
						VibrationManager.SetBlast(0.4f, this.character[0].Location + new Vector2(0f, -100f));
						Game1.map.MapSegFrameSpeed = 0.4f;
						for (int num2 = 0; num2 < 20; num2++)
						{
							Game1.pManager.AddExplosion(vector3 + Rand.GetRandomVector2(-250f, 250f, -250f, 250f), 3f, Rand.GetRandomInt(0, 3) == 0, 5);
						}
						this.character[0].SetJump(1400f, jumped: true);
						this.character[0].Slide(1400f);
						this.character[0].SetAnim("blowup", 0, 0);
						this.character[0].Floating = true;
						this.pMan.RemoveParticle(new Fidget(Vector2.Zero));
						this.pMan.AddDustSwordFall(this.character[0].Location + new Vector2(0f, -100f), new Vector2(((this.character[0].Face != 0) ? 1 : (-1)) * 700, -700f), 5);
						this.pMan.AddDustHat(this.character[0].Location + new Vector2(0f, -300f), new Vector2(((this.character[0].Face != 0) ? 1 : (-1)) * 350, -800f), 5);
						this.eventTimer = 0.4f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (Math.Abs(this.character[0].Trajectory.X) < 300f)
					{
						this.character[0].Trajectory.X = ((this.character[0].Face != 0) ? 1 : (-1)) * 300;
					}
					this.SetEventCamera(this.character[0].Location - new Vector2(0f, (float)Game1.screenHeight * 0.3f / Game1.hiDefScaleOffset), snapToLocation: true);
					Game1.SlowTime = 1f;
					if (this.eventTimer < 0f)
					{
						Game1.awardsManager.EarnAchievement(Achievement.DefeatGaius, forceCheck: false);
							if (Game1.settings.RandoMode)
							{
								Game1.connected_server.SetGoalAchieved();
							}
						this.eventTimer = 0.2f;
						this.FadeOut(this.eventTimer, Color.White);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (Math.Abs(this.character[0].Trajectory.X) < 300f)
					{
						this.character[0].Trajectory.X = ((this.character[0].Face != 0) ? 1 : (-1)) * 300;
					}
					this.SetEventCamera(this.character[0].Location - new Vector2(0f, (float)Game1.screenHeight * 0.3f / Game1.hiDefScaleOffset), snapToLocation: true);
					Game1.SlowTime = 1f;
					if (this.eventTimer < 0f)
					{
						this.RemoveCharacter(CharacterType.Gaius);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					this.eventTimer = 5f;
					Music.Play("deathcrawl");
					if (Game1.stats.startDifficulty > 1)
					{
						Game1.awardsManager.EarnAchievement(Achievement.DefeatGameHard, forceCheck: false);
					}
					this.map.SwitchMap(this.pMan, this.character, "lava22b", loading: false);
					this.RelocateDust(new Vector2(860f, 1000f) * 2f);
					this.character[0].SetAnim("crawlidle", 0, 0);
					this.character[0].Face = CharDir.Right;
					this.pMan.RemoveParticle(new Fidget(Vector2.Zero));
					this.SetEventCamera(Vector2.Zero, snapToLocation: true);
					this.subEvent++;
				}
				else if (this.subEvent == 5)
				{
					if (this.eventTimer < 0f)
					{
						this.eventTimer = 3f;
						this.FadeIn(this.eventTimer, Color.White);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 6)
				{
					this.SetEventCamera(this.character[0].Location, snapToLocation: true);
					if (Rand.GetRandomInt(0, 10) == 0)
					{
						VibrationManager.SetScreenShake(0.4f);
					}
					if ((double)this.eventTimer > 2.5)
					{
						this.pMan.AddBlood(this.character[0].Location + Rand.GetRandomVector2(-200f, 200f, -400f, -350f), new Vector2(0f, 200f), 1f, 1f, 1f, 1f, Rand.GetRandomFloat(0.5f, 4f), (CharacterType)1006, 0, 5);
						this.pMan.AddBlood(this.character[0].Location + Rand.GetRandomVector2(-250f, 250f, -400f, -350f), new Vector2(0f, 200f), 1f, 1f, 1f, 1f, Rand.GetRandomFloat(0.5f, 2f), (CharacterType)1016, 0, 5);
					}
					if (this.eventTimer < 0f)
					{
						VibrationManager.SetScreenShake(1f);
						this.ClearEvent();
						Game1.camera.ResetCameraFromEvent(this.character);
					}
				}
				break;
			case 800:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.Unskippable;
					this.eventTimer = 0.5f;
					Sound.FadeMusicOut(this.eventTimer);
					this.FadeOut(this.eventTimer, Color.White);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer < 0f)
					{
						this.RemoveCharacter(CharacterType.Gaius);
						this.screenFade = Color.Black;
						Game1.cutscene.InitCutscene(120);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					Game1.cutscene.ExitCutscene();
					Game1.InitCreditsScroll();
					this.subEvent++;
				}
				else if (Game1.gameMode != Game1.GameModes.CreditsScroll)
				{
					this.ClearEvent();
					this.ReturnToTitle();
				}
				break;
			default:
				this.ClearEvent();
				break;
			case 120:
			case 131:
			case 220:
			case 545:
				break;
			}
			if (EventManager.readyPlayer)
			{
				this.ReadyPlayer();
			}
		}

		public void DoSideEvent(int currentS)
		{
			switch (currentS)
			{
			case 0:
				if (this.subEvent == 0)
				{
					this.StartDialogue(25, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 2:
				if (this.subEvent == 0)
				{
					this.StartDialogue(57, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 3:
				if (this.subEvent == 0)
				{
					this.StartDialogue(27, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 4:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.StartDialogue(28, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
					Game1.hud.ForceInventory(InventoryState.Character);
				}
				break;
			case 5:
				if (this.subEvent == 0)
				{
					this.StartDialogue(112, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 6:
				Game1.hud.InitHelp("challenge", restart: true, -1);
				this.ClearEvent();
				break;
			case 7:
				Game1.hud.InitHelp("challengescoreboard", restart: true, -1);
				this.ClearEvent();
				break;
			case 8:
				if (this.subEvent == 0)
				{
					if (this.sideEventAvailable[20])
					{
						Game1.hud.InitHelp("healthItem", restart: true, -1);
						this.sideEventAvailable[20] = false;
					}
					Game1.stats.GetChestFromFile("SideEvent " + currentS.ToString() + " " + ((int)EquipType.Mushroom).ToString() + " " + 2.ToString(), Game1.pManager);
					this.StartDialogue(138, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 9:
				Game1.hud.InitHelp("recharge", restart: true, -1);
				this.ClearEvent();
				break;
			case 20:
				Game1.hud.InitHelp("healthItem", restart: true, -1);
				this.ClearEvent();
				break;
			case 21:
				Game1.hud.InitHelp("saveheal", restart: true, -1);
				this.ClearEvent();
				break;
			case 22:
				Game1.hud.InitHelp("needbomb", restart: true, -1);
				this.ClearEvent();
				break;
			case 23:
				Game1.hud.InitHelp("carrybomb", restart: true, -1);
				this.ClearEvent();
				break;
			case 25:
				Game1.hud.InitHelp("damage", restart: true, -1);
				this.ClearEvent();
				break;
			case 26:
				Game1.hud.InitHelp("exp", restart: true, -1);
				this.ClearEvent();
				break;
			case 27:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(75, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.DialogueOver())
					{
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					for (int num3 = 1; num3 < ((this.map.regionName == "smith") ? 4 : 8); num3++)
					{
						this.SpawnIDCharacter(this.character[0].Location + Rand.GetRandomVector2(-800f, 800f, -800f, -700f), num3, "avee", CharacterType.Avee, Team.Enemy, ground: false);
					}
					this.ClearEvent();
				}
				break;
			case 28:
				if (this.subEvent == 0)
				{
					Game1.hud.InitHelp("blomb", restart: true, -1);
					this.skippable = SkipType.DialogueOnly;
					if (this.map.regionName == "smith")
					{
						EventManager.hotSpot = new Vector2(950f, 2160f) * 2f;
					}
					else
					{
						EventManager.hotSpot = new Vector2(3000f, 3220f);
					}
					EventManager.hotID = this.character.Length - 1;
					this.SpawnIDCharacter(EventManager.hotSpot, EventManager.hotID - 1, "blomb", CharacterType.Blomb, Team.Enemy, ground: false);
					this.SpawnIDCharacter(EventManager.hotSpot + new Vector2(900f, 0f), EventManager.hotID, "imp", CharacterType.Imp, Team.Enemy, ground: true);
					this.eventTimer = 1f;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer <= 0f)
					{
						this.eventCamera = EventManager.hotSpot;
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer <= 0f)
					{
						this.safetyOn = false;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.character[EventManager.hotID].Location.X > this.character[EventManager.hotID - 1].Location.X)
					{
						this.eventCamera += (new Vector2((this.character[EventManager.hotID].Location.X + EventManager.hotSpot.X) / 2f, EventManager.hotSpot.Y) - this.eventCamera) * Game1.FrameTime * 4f;
						this.character[EventManager.hotID].Ai.overrideAI = true;
						this.character[EventManager.hotID].Ai.jobType = JobType.RunLeft;
					}
					else
					{
						this.character[EventManager.hotID - 1].SetAnim("die", 0, 0);
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					if (this.eventTimer <= 0f)
					{
						this.character[EventManager.hotID].Exists = CharExists.Dead;
						this.eventCamera = Vector2.Zero;
						this.StartDialogue(76, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 29:
				if (this.subEvent == 0)
				{
					Game1.awardsManager.EarnAchievement(Achievement.MeetHaley, forceCheck: false);
					this.skippable = SkipType.DialogueOnly;
					this.character[0].Face = CharDir.Right;
					Game1.camera.camOffset.X = -50f;
					if (this.character[0].State == CharState.Grounded)
					{
						this.StartDialogue(Game1.stats.villagerDialogue[15], CharacterType.Haley);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 1)
				{
					this.ApproachLoc(CharacterType.Haley, this.character[0].Location, 220, null);
					this.ApproachLoc(CharacterType.Matti, this.character[0].Location, 260, null);
					if (this.DialogueOver())
					{
						this.ClearEvent();
					}
				}
				break;
			case 30:
			case 31:
			case 32:
			case 33:
			case 34:
			case 35:
			case 36:
			case 37:
			case 38:
			case 39:
				Game1.hud.InitHelp("combat" + (currentS - 30), restart: true, -1);
				this.ClearEvent();
				break;
			case 40:
				if (this.subEvent == 0)
				{
					this.StartDialogue(73, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1 && this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 41:
				Game1.hud.InitHelp("rescued", restart: true, -1);
				this.ClearEvent();
				break;
			case 42:
				Game1.hud.InitHelp("questgiver", restart: true, -1);
				this.ClearEvent();
				break;
			case 43:
				if (this.subEvent == 0)
				{
					this.StartDialogue(474, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 44:
				Game1.hud.InitHelp("gate", restart: true, -1);
				this.ClearEvent();
				break;
			case 50:
			case 51:
			case 52:
			case 53:
				if (this.subEvent == 0)
				{
					for (int k = 0; k < 100; k++)
					{
						this.pMan.AddSpray(this.character[0].Location + Rand.GetRandomVector2(-500f, 500f, -1500f, -500f), Rand.GetRandomVector2(-300f, 300f, -100f, 800f), Rand.GetRandomFloat(1f, 3f), 1, 6, Rand.GetRandomInt(5, 8));
					}
					VibrationManager.SetScreenShake(3f);
					int conversation;
					switch (currentS)
					{
					default:
						conversation = 442;
						break;
					case 51:
						conversation = 450;
						break;
					case 52:
						conversation = 465;
						break;
					case 53:
						conversation = 470;
						Sound.PlayCue("avalanche_start", this.character[0].Location + new Vector2(200f, -500f), 400f);
						break;
					}
					this.StartDialogue(conversation, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 55:
			case 56:
			case 57:
					{
						bool flag;
						switch (currentS)
						{
							case 56:
								flag = (Game1.stats.Equipment[332] > 0);
								break;
							case 57:
								flag = (Game1.stats.Equipment[333] > 0);
								break;
							default:
								flag = (Game1.stats.Equipment[331] > 0);
								break;
						}
						if (flag)
						{
							this.ClearEvent();
							break;
						}
						this.sideEventAvailable[currentS] = true;
						Vector2 value;
						switch (currentS)
						{
							case 56:
								value = new Vector2(4560f, 2700f);
								break;
							case 57:
								value = new Vector2(4560f, 2700f);
								break;
							default:
								value = new Vector2(4560f, 2700f);
								break;
						}
						for (int j = 1; j < this.character.Length; j++)
						{
							if (this.character[j].Definition.charType != CharacterType.KaneGhostChase)
							{
								this.character[j].Exists = CharExists.Dead;
							}
						}
						if (this.GetCharacter(CharacterType.KaneGhostChase) == null)
						{
							this.SpawnCharacter(value * 2f, "ghost", CharacterType.KaneGhostChase, Team.Enemy, ground: false);
							Music.Play("mansionchase");
							this.map.doorRegions.Clear();
						}
						this.ClearEvent();
						Game1.hud.canInput = true;
						break;
					}
			case 61:
				if (this.subEvent == 0)
				{
					this.StartDialogue(190, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 62:
			case 63:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.eventTimer = 2f;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue((currentS == 62) ? 551 : 553, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 71:
				if (this.subEvent == 0)
				{
					this.StartDialogue(192, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 72:
				if (this.subEvent == 0)
				{
					this.StartDialogue(10, CharacterType.Geehan);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 73:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.SpawnCharacter(new Vector2(3650f, 2740f) * 2f, "sarahi", CharacterType.Sarahi, Team.Friendly, ground: true);
					Music.Play("beauty");
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.ApproachLoc(CharacterType.Sarahi, this.character[0].Location, 200, null);
					if (this.ApproachLoc(CharacterType.Dust, this.GetCharacter(CharacterType.Sarahi).Location, 200, null))
					{
						this.StartDialogue(0, CharacterType.Sarahi);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					this.ApproachLoc(CharacterType.Sarahi, this.character[0].Location, 200, null);
					if (this.DialogueOver())
					{
						this.SpawnCharacter(new Vector2(2700f, 2500f) * 2f, "moska", CharacterType.Moska, Team.Friendly, ground: true);
						this.FaceCharacter(CharacterType.Dust, CharacterType.Moska, faceTowards: true);
						this.StartDialogue(1, CharacterType.Sarahi);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					this.eventCamera = (this.GetCharacter(CharacterType.Moska).Location + this.GetCharacter(CharacterType.Sarahi).Location) / 2f - new Vector2(0f, (float)Game1.screenHeight * 0.4f / Game1.hiDefScaleOffset);
					this.ApproachLoc(CharacterType.Moska, new Vector2(5800f, 0f), 10, null);
					this.ApproachLoc(CharacterType.Sarahi, this.GetCharacter(CharacterType.Moska).Location, 200, null);
					if (this.DialogueOver())
					{
						this.eventCamera = Vector2.Zero;
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					this.ApproachLoc(CharacterType.Moska, new Vector2(5400f, 0f), 10, null);
					this.ApproachLoc(CharacterType.Sarahi, new Vector2(5400f, 0f), 10, null);
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(2, CharacterType.Sarahi);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					this.ApproachLoc(CharacterType.Moska, new Vector2(5400f, 0f), 10, null);
					if (this.ApproachLoc(CharacterType.Sarahi, new Vector2(5400f, 0f), 10, null) && this.DialogueOver())
					{
						this.RemoveCharacter(CharacterType.Sarahi);
						this.RemoveCharacter(CharacterType.Moska);
						this.ClearEvent();
					}
				}
				break;
			case 74:
				if (this.subEvent == 0)
				{
					this.StartDialogue(30, CharacterType.Reed);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 75:
				this.map.leftBlock = 1180f;
				this.map.rightBlock = 4290f;
				this.SpawnCharacter(new Vector2(730f, 1050f) * 2f, "miniboss", CharacterType.Giant, Team.Enemy, ground: false);
				this.SpawnCharacter(new Vector2(1850f, 1050f) * 2f, "miniboss", CharacterType.Giant, Team.Enemy, ground: false);
				Game1.hud.InitBoss(this.character, "miniboss");
				this.ClearEvent();
				break;
			case 76:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.SpawnCharacter(new Vector2(2630f, 1400f) * 2f, "corbin", CharacterType.Corbin, Team.Friendly, ground: true);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.ApproachLoc(CharacterType.Corbin, this.character[0].Location, 200, null);
					if (this.ApproachLoc(CharacterType.Dust, this.GetCharacter(CharacterType.Corbin).Location, 200, null))
					{
						this.StartDialogue(0, CharacterType.Corbin);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					this.ApproachLoc(CharacterType.Corbin, this.character[0].Location, 200, null);
					if (this.DialogueOver())
					{
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					this.ApproachLoc(CharacterType.Corbin, this.character[0].Location + new Vector2(-1000f, 0f), 10, null);
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(1, CharacterType.Corbin);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4 && this.ApproachLoc(CharacterType.Corbin, this.character[0].Location + new Vector2(-1000f, 0f), 10, null) && this.DialogueOver())
				{
					this.RemoveCharacter(CharacterType.Corbin);
					this.ClearEvent();
				}
				break;
			case 77:
				if (this.subEvent == 0)
				{
					this.StartDialogue(300, CharacterType.Bopo);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 78:
			{
				Vector2 vector2 = new Vector2(2400f, 1930f) * 2f;
				if (this.subEvent == 0)
				{
					this.sideEventAvailable[78] = true;
					this.skippable = SkipType.Unskippable;
					this.pMan.AddWhirlwind(new Vector2(2500f, 2200f), Vector2.Zero, 5);
					this.eventTimer = 10f;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.character[0].KeyDown = true;
					if (this.pMan.DirectParticle(new Whirlwind(Vector2.Zero, Vector2.Zero, 1), 0))
					{
						this.character[0].SetAnim("null", 0, 0);
						Vector2 vector3 = this.character[0].Location - new Vector2(0f, 120f);
						for (int l = 0; l < 20; l++)
						{
							Vector2 loc = vector3 + Rand.GetRandomVector2(-100f, 100f, -100f, 100f);
							Game1.pManager.AddElectricBolt(loc, -1, Rand.GetRandomFloat(0.6f, 1.2f), 1000, 100, 1f, 5);
							Game1.pManager.AddBounceSpark(loc, Rand.GetRandomVector2(-800f, 800f, -500f, 10f), 0.5f, 6);
						}
						Sound.PlayCue("warp_large");
						Vector2 fidgetLoc = Game1.pManager.GetFidgetLoc(accomodateScroll: false);
						for (int m = 0; m < 4; m++)
						{
							Game1.pManager.AddElectricBolt(fidgetLoc, -1, Rand.GetRandomFloat(0.6f, 1.2f), 1000, 100, 1f, 5);
						}
						Game1.pManager.RemoveParticle(new Fidget(Vector2.Zero));
						this.eventTimer = 3f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer < 2f)
					{
						this.pMan.DirectParticle(new Whirlwind(Vector2.Zero, Vector2.Zero, 1), 1);
					}
					if (this.eventTimer <= 0f)
					{
						this.pMan.Reset(removeWeather: true, removeBombs: true);
						this.map.SwitchMap(this.pMan, this.character, "snow18", loading: false);
						for (int n = 0; n < 40; n++)
						{
							this.pMan.AddSnowTime(Rand.GetRandomVector2(-200f, 0f, 100f, 400f), Rand.GetRandomFloat(1f, 2f), 3);
						}
						for (int num = 0; num < 80; num++)
						{
							this.pMan.AddSnowTime(Rand.GetRandomVector2(-200f, 0f, 100f, 400f), Rand.GetRandomFloat(1f, 2f), 6);
						}
						Sound.FadeMusicOut(2f);
						this.character[0].SetAnim("null", 0, 0);
						this.SetEventCamera(vector2, snapToLocation: true);
						Game1.pManager.RemoveParticle(new Fidget(Vector2.Zero));
						this.eventTimer = 3f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					this.SetEventCamera(vector2, snapToLocation: true);
					if (this.eventTimer <= 0f)
					{
						this.pMan.AddWhirlwind(vector2 + new Vector2(600f, 100f) * 2f, Vector2.Zero, 5);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					this.SetEventCamera(vector2, snapToLocation: true);
					if (this.pMan.DirectParticle(new Whirlwind(Vector2.Zero, Vector2.Zero, 1), 0))
					{
						Vector2 vector4 = this.character[0].Location - new Vector2(0f, 100f);
						this.character[0].Face = CharDir.Left;
						this.character[0].SetJump(1200f, jumped: true);
						this.character[0].SetAnim("evadeair", 1, 0);
						this.character[0].Ethereal = EtherealState.Normal;
						VibrationManager.SetBlast(0.5f, vector4);
						VibrationManager.SetScreenShake(0.6f);
						this.pMan.AddShockRing(vector4, 1f, 5);
						for (int num2 = 0; num2 < 20; num2++)
						{
							this.pMan.AddElectricBolt(vector4, -1, Rand.GetRandomFloat(0.1f, 0.5f), 1000, 100, 1f, 5);
							this.pMan.AddBounceSpark(vector4, Rand.GetRandomVector2(-400f, 400f, -800f, 10f), 0.5f, 6);
						}
						Sound.PlayCue("warp_small");
						this.pMan.AddFidget(this.character[0].Location);
						this.pMan.RemoveParticle(new Whirlwind(Vector2.Zero, Vector2.Zero, 1));
						this.SetEventCamera(Vector2.Zero, snapToLocation: false);
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5 && this.eventTimer <= 0f)
				{
					this.ClearEvent();
					this.InitEvent(79, isSideEvent: true);
				}
				break;
			}
			case 79:
				if (this.subEvent == 0)
				{
					this.StartDialogue(430, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 80:
			case 81:
				if (this.subEvent == 0)
				{
					this.StartDialogue((currentS == 80) ? 550 : 552, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 82:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.StartDialogue(290, CharacterType.Bopo);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.ApproachLoc(CharacterType.Bopo, this.character[0].Location, 200, null);
					if (this.DialogueOver())
					{
						this.eventTimer = 2f;
						this.FadeOut(this.eventTimer, Color.Black);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer <= 0f)
					{
						this.RelocateDust(new Vector2(4500f, 620f) * 2f);
						this.RelocateCharacter(CharacterType.Bopo, new Vector2(4960f, 620f) * 2f);
						this.FaceCharacter(CharacterType.Dust, CharacterType.Bopo, faceTowards: true);
						this.FaceCharacter(CharacterType.Bopo, CharacterType.Dust, faceTowards: true);
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					this.ApproachLoc(CharacterType.Bopo, this.character[0].Location, 200, null);
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(291, CharacterType.Bopo);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					this.ApproachLoc(CharacterType.Bopo, this.character[0].Location, 200, null);
					if (this.DialogueOver())
					{
						this.ClearEvent();
						this.FadeIn(2f, Color.Black);
					}
				}
				break;
			case 83:
				if (this.subEvent == 0)
				{
					this.StartDialogue(315, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 84:
				Game1.awardsManager.EarnAchievement(Achievement.BugMatti, forceCheck: false);
				this.ClearEvent();
				break;
			case 87:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.Skippable;
					this.character[0].Face = CharDir.Right;
					this.character[0].SetAnim("intrograb", 0, 2);
					this.character[0].defaultColor = Color.White;
					this.pMan.RemoveParticle(new Fidget(Vector2.Zero));
					Game1.camera.ResetCamera(this.character);
					Game1.hud.InitHelp("trialmove", restart: true, -1);
					Game1.worldScale = 0.75f;
					this.eventTimer = 100000f;
					this.FadeIn(this.eventTimer, Color.Black);
					Game1.stats.GetWorldExplored();
					Game1.hud.miniPromptList.Clear();
					this.character[0].KeyDown = true;
					Game1.wManager.SetWeather(WeatherType.Pollen, forceReset: true);
					Game1.wManager.randomType = RandomType.None;
					Music.Play("souls");
					this.eventTimer = 6f;
					this.FadeIn(this.eventTimer, Color.Black);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.character[0].SetAnim("intrograb", 0, 2);
					this.character[0].KeyDown = true;
					if (this.eventTimer < 0f)
					{
						this.StartDialogue(0, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					Sound.DimSFXVolume(Math.Min(Sound.overrideCinematicVolume + Game1.HudTime, 1f));
					this.character[0].KeyDown = true;
					if (this.DialogueOver())
					{
						this.pMan.AddDustSword(new Vector2(this.character[0].Location.X + 750f, this.character[0].Location.Y - 200f), Vector2.Zero, 6);
						this.eventTimer = 5f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					this.character[0].KeyDown = true;
					if (this.eventTimer < 0f)
					{
						this.StartDialogue(1, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					this.character[0].KeyDown = true;
					if (this.DialogueOver())
					{
						this.eventTimer = 4.5f;
						this.character[0].SetAnim("idle00", 0, 2);
						this.character[0].SetAnim("intrograb", 6, 2);
						this.subEvent = 15;
					}
				}
				else if (this.subEvent == 15)
				{
					if (this.eventTimer > 3f)
					{
						this.character[0].KeyDown = true;
						if (Rand.GetRandomInt(0, 5) == 0)
						{
							this.pMan.AddSparkle(new Vector2(this.character[0].Location.X + 150f + (float)Rand.GetRandomInt(-20, 20), this.character[0].Location.Y + (float)Rand.GetRandomInt(-220, -40)), 0f, 1f, 1f, 1f, Rand.GetRandomFloat(0.2f, 0.8f), Rand.GetRandomInt(14, 28), 5);
						}
					}
					else if ((double)this.eventTimer > 2.9)
					{
						this.character[0].SetAnim("standup", 0, 2);
						Sound.PlayCue("dustswordgrab");
						this.pMan.RemoveParticle(new DustSword(Vector2.Zero, Vector2.Zero));
						for (int i = 0; i < 4; i++)
						{
							this.pMan.AddSparkle(new Vector2(this.character[0].Location.X + (float)Rand.GetRandomInt(-100, 100), this.character[0].Location.Y + (float)Rand.GetRandomInt(-200, -20)), 0.5f, 1f, 0.5f, 1f, Rand.GetRandomFloat(0.8f, 1.2f), Rand.GetRandomInt(14, 28), 5);
							this.pMan.AddSparkle(new Vector2(this.character[0].Location.X + 150f + (float)Rand.GetRandomInt(-20, 20), this.character[0].Location.Y + (float)Rand.GetRandomInt(-220, -40)), 1f, 0.5f, 0.5f, 1f, Rand.GetRandomFloat(0.8f, 1.2f), Rand.GetRandomInt(14, 28), 5);
						}
					}
					if (this.eventTimer < 1f)
					{
						this.pMan.AddFidget(new Vector2(this.character[0].Location.X + 800f, this.character[0].Location.Y - 200f));
						Game1.hud.InitFidgetPrompt(FidgetPrompt.None);
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 16)
				{
					if (this.eventTimer < 0f)
					{
						this.StartDialogue(910, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					Game1.awardsManager.EarnAvatarAward(new string[1] { "AvatarAwards1" });
					Sound.FadeMusicOut(4f);
					this.ClearEvent();
				}
				break;
			case 88:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.Unskippable;
					this.eventTimer = 1f;
					Sound.FadeMusicOut(this.eventTimer);
					this.FadeOut(this.eventTimer, Color.Black);
					this.subEvent++;
				}
				else if (this.eventTimer <= 0f)
				{
					this.ClearEvent();
					Game1.stats.ResetDebug(this.pMan, this.character);
				}
				break;
			case 89:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.Unskippable;
					this.eventTimer = 1f;
					this.FadeOut(this.eventTimer, Color.Black);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.eventTimer <= 0f)
					{
						this.eventTimer = 1f;
						this.FadeIn(this.eventTimer, Color.Black);
						this.pMan.Reset(removeWeather: false, removeBombs: true);
						Game1.cutscene.InitCutscene(200);
						this.subEvent++;
					}
				}
				else
				{
					this.ClearEvent();
					Game1.stats.ResetDebug(this.pMan, this.character);
				}
				break;
			case 90:
				this.map.leftBlock = 1200f;
				this.map.rightBlock = 4880f;
				Game1.hud.InitHelp("attack", restart: true, -1);
				this.SpawnCharacter(new Vector2(2000f, 1660f) * 2f, "enemy1", CharacterType.Imp, Team.Enemy, ground: true);
				this.SpawnCharacter(new Vector2(2000f, 1660f) * 2f, "enemy1", CharacterType.Imp, Team.Enemy, ground: true);
				this.SpawnCharacter(new Vector2(2270f, 1660f) * 2f, "enemy1", CharacterType.RockHound, Team.Enemy, ground: true);
				this.ClearEvent();
				break;
			case 91:
				Game1.hud.InitHelp("trialupgrade1", restart: true, -1);
				this.map.leftBlock = 1200f;
				this.map.rightBlock = 5960f;
				this.ClearEvent();
				break;
			case 92:
				if (this.subEvent == 0)
				{
					this.map.leftBlock = (this.map.rightBlock = 0f);
					Game1.hud.InitHelp("trialupgrade2", restart: true, -1);
					this.safetyOn = false;
					for (int num7 = 0; num7 < 4; num7++)
					{
						this.SpawnCharacter(new Vector2(2750 + num7 * 100 + Rand.GetRandomInt(-40, 40), 1820 + Rand.GetRandomInt(-100, 40)) * 2f, "enemy", CharacterType.Imp, Team.Enemy, ground: false);
					}
					this.StartDialogue(10, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.ApproachLoc(CharacterType.Dust, new Vector2(5200f, 0f), 10, null))
					{
						Game1.camera.camOffset.X = -250f;
						this.character[0].Face = CharDir.Right;
						if (this.DialogueOver())
						{
							this.eventTimer = 1.5f;
							this.subEvent++;
						}
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer <= 1f)
					{
						this.character[0].KeySecondary = true;
					}
					if (this.eventTimer <= 0f)
					{
						this.character[0].KeySecondary = false;
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(11, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					if (this.eventTimer <= 0f)
					{
						Game1.stats.curCharge = Game1.stats.maxCharge;
						Game1.stats.projectileType = 0;
						Game1.stats.projectileCost = Game1.stats.GetProjectileCost(Game1.stats.projectileType);
						Game1.stats.canThrow = true;
						this.character[0].KeyThrow = true;
						this.skippable = SkipType.Skippable;
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 6)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(12, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 7)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 8)
				{
					if (this.eventTimer <= 0f)
					{
						Game1.stats.curCharge = Game1.stats.maxCharge;
						Game1.stats.projectileType = 0;
						Game1.stats.projectileCost = Game1.stats.GetProjectileCost(Game1.stats.projectileType);
						Game1.stats.canThrow = true;
						this.character[0].KeyThrow = true;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 9)
				{
					if (this.eventTimer <= 0f)
					{
						Game1.stats.canThrow = true;
						this.character[0].KeyThrow = true;
						this.eventTimer = 1.65f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 10)
				{
					if (this.eventTimer <= 1.1f)
					{
						this.character[0].KeySecondary = true;
					}
					if (this.eventTimer <= 0f)
					{
						Game1.stats.curCharge = Game1.stats.maxCharge;
						this.character[0].KeySecondary = false;
						this.eventTimer = 3f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 11)
				{
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(901, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 93:
				Game1.hud.InitHelp("trialdestroywall", restart: true, -1);
				this.ClearEvent();
				break;
			case 94:
				if (Game1.stats.upgrade[14] == 0)
				{
					this.sideEventAvailable[94] = true;
				}
				this.ClearEvent();
				break;
			case 95:
				this.map.leftBlock = 200f;
				this.map.rightBlock = 6000f;
				Game1.hud.InitHelp("triallocked", restart: true, -1);
				this.ClearEvent();
				break;
			case 96:
				if (this.map.path == "trial08")
				{
					Sound.FadeMusicOut(3f);
				}
				else
				{
					Music.Play("grave");
					Game1.stats.AcquireXP(2400);
				}
				this.sideEventAvailable[96] = true;
				this.ClearEvent();
				break;
			case 97:
				if (this.subEvent == 0)
				{
					for (int num4 = 1; num4 < this.character.Length; num4++)
					{
						if (this.character[num4].Exists == CharExists.Exists && this.character[num4].NPC == NPCType.None)
						{
							this.character[num4].KillMe(instantly: false);
						}
					}
					Game1.hud.InitHelp("parry", restart: true, -1);
					this.map.leftBlock = 4920f;
					this.map.rightBlock = 9000f;
					EventManager.hotSpot = new Vector2(3600f, 1500f) * 2f;
					EventManager.hotID = this.character.Length - 1;
					this.SpawnIDCharacter(EventManager.hotSpot + new Vector2(1000f, 0f), EventManager.hotID, "boss", CharacterType.Trolk, Team.Enemy, ground: true);
					this.character[EventManager.hotID].Face = CharDir.Left;
					this.character[EventManager.hotID].Ai.overrideAI = true;
					this.skippable = SkipType.DialogueOnly;
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					this.character[EventManager.hotID].SetAnim("idle00", 0, 2);
					if (this.character[0].Location.X < EventManager.hotSpot.X)
					{
						this.character[0].KeyRight = true;
						break;
					}
					this.safetyOn = false;
					this.StartDialogue(148, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 2)
				{
					this.SetEventCamera((this.character[0].Location + this.character[EventManager.hotID].Location) / 2f - new Vector2(0f, (float)Game1.screenHeight * 0.4f / Game1.hiDefScaleOffset), snapToLocation: false);
					if (this.character[EventManager.hotID].Location.X > EventManager.hotSpot.X + 800f)
					{
						this.character[EventManager.hotID].Ai.jobType = JobType.RunLeft;
						break;
					}
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					this.character[EventManager.hotID].SetAnim("intro01", 6, 2);
					this.subEvent++;
				}
				else if (this.subEvent == 3)
				{
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.DialogueOver())
					{
						this.character[0].SetAnim("idle01", 0, 2);
						this.character[0].SetAnim("runend", 0, 2);
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					this.SetEventCamera(Vector2.Zero, snapToLocation: false);
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.eventTimer < 1f && this.eventTimer > 0.8f && this.character[0].AnimName != "parry")
					{
						this.character[0].KeyAttack = true;
					}
					if (this.eventTimer <= 0f)
					{
						this.eventTimer = 1f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 5)
				{
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(149, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 6)
				{
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.character[EventManager.hotID].Location.X > EventManager.hotSpot.X + 800f)
					{
						this.character[EventManager.hotID].Ai.jobType = JobType.RunLeft;
					}
					if (this.character[0].Location.X < EventManager.hotSpot.X + 200f)
					{
						this.character[0].KeyRight = true;
					}
					if (this.DialogueOver())
					{
						this.eventTimer = 2f;
						this.character[0].SetAnim("idle01", 0, 2);
						this.character[0].SetAnim("runend", 0, 2);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 7)
				{
					this.SetEventCamera((this.character[0].Location + this.character[EventManager.hotID].Location) / 2f - new Vector2(0f, (float)Game1.screenHeight * 0.4f / Game1.hiDefScaleOffset), snapToLocation: false);
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.eventTimer < 1f)
					{
						this.character[EventManager.hotID].KeyAttack = true;
					}
					if (!(this.character[EventManager.hotID].AnimName == "attack00") || !(this.eventTimer < 0.2f))
					{
						break;
					}
					this.character[0].SetAnim("attack00", 0, 0);
					if (this.character[EventManager.hotID].AnimFrame >= 11)
					{
						this.character[EventManager.hotID].SetAnim("godown", 0, 0);
						this.character[EventManager.hotID].DownTime = 10000f;
						this.character[EventManager.hotID].Ethereal = EtherealState.Ethereal;
						Game1.SlowTime = 0.4f;
						VibrationManager.SetBlast(1f, EventManager.hotSpot);
						VibrationManager.SetScreenShake(0.25f);
						Game1.stats.curCharge = MathHelper.Clamp(Game1.stats.curCharge + 50f, 0f, 100f);
						Vector2 vector5 = (this.character[0].Location + this.character[EventManager.hotID].Location) / 2f + new Vector2(0f, -250f);
						this.pMan.AddLenseFlare(vector5, 0.4f, 0, 5);
						for (int num5 = 0; num5 < 30; num5++)
						{
							this.pMan.AddBounceSpark(vector5 + Rand.GetRandomVector2(-40f, 40f, -40f, 40f), Rand.GetRandomVector2(-500f, 500f, -1000f, 10f), 0.3f, 5);
						}
						this.eventTimer = 2f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 8)
				{
					if (this.character[EventManager.hotID].AnimName == "godown" && this.character[EventManager.hotID].AnimFrame == 1)
					{
						this.character[0].InitParry(this.pMan, parrySuccess: true);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 9)
				{
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.eventTimer <= 0f)
					{
						this.StartDialogue(150, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else
				{
					if (this.subEvent != 10)
					{
						break;
					}
					this.SetEventCamera((this.character[0].Location + this.character[EventManager.hotID].Location) / 2f - new Vector2(0f, (float)Game1.screenHeight * 0.4f / Game1.hiDefScaleOffset), snapToLocation: false);
					this.character[EventManager.hotID].Ai.jobType = JobType.Idle;
					if (this.character[0].Location.X < EventManager.hotSpot.X + 300f)
					{
						this.character[0].KeyRight = true;
					}
					if (this.DialogueOver())
					{
						this.character[EventManager.hotID].DownTime = 0.1f;
						this.character[0].SetAnim("idle01", 0, 2);
						this.character[0].SetAnim("runend", 0, 2);
						for (int num6 = 0; num6 < 5; num6++)
						{
							this.SpawnCharacter(new Vector2(3700f, 800f) * 2f + Rand.GetRandomVector2(-300f, 300f, -200f, 200f), "boss", CharacterType.Avee, Team.Enemy, ground: true);
						}
						Game1.hud.InitBoss(this.character, "boss");
						Music.Play("boss1");
						this.ClearEvent();
					}
				}
				break;
			case 98:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.DialogueOnly;
					this.StartDialogue(900, CharacterType.Dust);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.DialogueOver())
					{
						this.eventTimer = 1f;
						Sound.FadeMusicOut(1f);
						this.FadeOut(this.eventTimer, Color.Black);
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer <= 0f)
					{
						this.eventTimer = 1f;
						this.FadeIn(this.eventTimer, Color.Black);
						this.pMan.Reset(removeWeather: false, removeBombs: true);
						this.skippable = SkipType.Unskippable;
						Game1.cutscene.InitCutscene(200);
						this.subEvent++;
					}
				}
				else
				{
					this.ClearEvent();
					if (!Game1.Convention)
					{
						Game1.InitUpsell(quitting: false);
					}
					else
					{
						Game1.stats.ResetDebug(this.pMan, this.character);
					}
				}
				break;
			case 99:
				if (this.subEvent == 0)
				{
					this.skippable = SkipType.Unskippable;
					Sound.FadeMusicOut(1.5f);
					this.SpawnCharacter(new Vector2(2700f, 2290f) * 2f, "miniboss", CharacterType.Psylph, Team.Enemy, ground: true);
					this.subEvent++;
				}
				else if (this.subEvent == 1)
				{
					if (this.ApproachLoc(CharacterType.Dust, new Vector2(2100f, 0f) * 2f, 10, null))
					{
						this.map.leftBlock = 2000f;
						this.map.rightBlock = 7140f;
						this.eventTimer = 1.5f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 2)
				{
					if (this.eventTimer < 1f)
					{
						this.SetEventCamera(this.GetCharacter(CharacterType.Psylph).Location + new Vector2(-200f, -200f), snapToLocation: false);
						Music.Play("boss3");
					}
					if (this.eventTimer <= 0f)
					{
						this.GetCharacter(CharacterType.Psylph).SetLunge(LungeStates.Lunging, 1200f, 1200f);
						this.eventTimer = 0.8f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 3)
				{
					this.SetEventCamera(this.GetCharacter(CharacterType.Psylph).Location + new Vector2(-200f, -200f), snapToLocation: false);
					if (this.eventTimer < 0f)
					{
						this.character[0].Evade(this.pMan);
						this.character[0].Trajectory.X *= 0.9f;
						Game1.hud.InitBoss(this.character, "miniboss");
						this.eventTimer = 0.5f;
						this.subEvent++;
					}
				}
				else if (this.subEvent == 4)
				{
					this.SetEventCamera(this.GetCharacter(CharacterType.Psylph).Location + new Vector2(-200f, -200f), snapToLocation: false);
					if (this.eventTimer < 0f)
					{
						this.skippable = SkipType.Skippable;
						this.StartDialogue(310, CharacterType.Dust);
						this.subEvent++;
					}
				}
				else if (this.DialogueOver())
				{
					this.ClearEvent();
				}
				break;
			case 110:
				if (this.subEvent == 0)
				{
					this.subEvent++;
				}
				else
				{
					this.ClearEvent();
				}
				break;
			default:
				this.ClearEvent();
				break;
			case 58:
			case 59:
			case 60:
			case 64:
			case 65:
			case 66:
			case 67:
			case 70:
				break;
			}
			if (EventManager.readyPlayer)
			{
				this.ReadyPlayer();
			}
		}
	}
}
