using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Particles;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.ai
{
	internal class Gaius : AI
	{
		private int bossStage;

		private bool evadingLava;

		private bool overrideAnim;

		private float sphereTimer;

		private int hurtCount;

		private float hitTime;

		public Gaius(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 60000;
			if (Game1.map.path == "lava22")
			{
				this.bossStage = 10;
			}
			if (Game1.map.path == "lava22b")
			{
				this.bossStage = 20;
				c.MaxHP *= 2;
			}
			c.Speed = 700f;
			c.Aggression = 100;
			c.JumpVelocity = 2200f;
			c.Strength = 500;
			c.DefaultHeight = 250;
			c.DefaultWidth = 200;
			c.ShadowWidth = 1.5f;
			c.maxAnimFrames = 40;
			c.DamageType = DamageTypes.NotTeam;
			c.SpawnType = SpawnTypes.NoEffect;
			c.defaultColor = Color.White;
			c.MaskGlow = 1f;
			c.alwaysUpdatable = true;
			base.jobType = JobType.Idle;
			base.PrepareStats(c, 1f);
		}

		public override void SpecialTrigger(Trigger trig, Vector2 loc, float rot, float scale, ParticleManager pMan)
		{
			if (trig == Trigger.Special0)
			{
				Vector2 vector = new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot)) * 400f;
				pMan.AddGaiusSphere(loc, Game1.character[0].Location + new Vector2(0f, -300f + vector.Y), 1f, base.me.Strength / 2, base.me.ID, 5);
				for (int i = 0; i < 4; i++)
				{
					pMan.AddElectricBolt(loc, -1, 0.2f, 1000, 100, 0.5f, 6);
				}
			}
			base.SpecialTrigger(trig, loc, rot, scale, pMan);
		}

		public override string CheckAnimName(string animName)
		{
			if (this.overrideAnim)
			{
				this.overrideAnim = false;
				return animName;
			}
			if (animName.Contains("hurt"))
			{
				if (Rand.GetRandomInt(0, 4) == 0)
				{
					Sound.PlayCue("gaius_hurt", base.me.Location, (Game1.character[0].Location - base.me.Location).Length());
				}
				this.hurtCount++;
				this.hitTime = 1f;
			}
			if (base.me.AnimName == "biggrab" || this.hurtCount > 12)
			{
				base.me.AnimName = "idle00";
				base.me.Slide(-1500f);
				base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 2f);
				this.hurtCount = 0;
				base.me.SetJump(1200f, jumped: true);
				return "jump";
			}
			if (animName == "land" && base.me.AnimName == "attack01")
			{
				return "hurtland";
			}
			if (animName.StartsWith("attack0"))
			{
				return "attack0" + Rand.GetRandomInt(0, 3);
			}
			return animName;
		}

		public override void Die(ParticleManager pMan, Texture2D texture)
		{
			base.Die(pMan, texture);
		}

		public override bool CheckHit(Particle p)
		{
			switch (this.bossStage)
			{
			case 2:
				if (p != null && p is Hit)
				{
					this.bossStage = 3;
					this.InitHurtEffect(p, CharDir.Left, 1000f, -2000f);
					Game1.events.InitEvent(710, isSideEvent: false);
				}
				base.me.CanHurtProjectileFrame = 2f;
				return false;
			case 4:
				if (p != null && p is Hit)
				{
					this.bossStage = 5;
					this.InitHurtEffect(p, base.me.Face, 1500f, -200f);
					Game1.events.InitEvent(720, isSideEvent: false);
				}
				base.me.CanHurtProjectileFrame = 2f;
				return false;
			case 12:
				if (p != null && p is Hit)
				{
					Sound.PlayCue("gaius_die", base.me.Location, (Game1.character[0].Location - base.me.Location).Length());
					this.bossStage = 20;
					this.InitHurtEffect(p, (Game1.character[0].Face == CharDir.Left) ? CharDir.Right : CharDir.Left, 1200f, -1800f);
					Game1.events.InitEvent(760, isSideEvent: false);
					base.me.CanHurtProjectileFrame = 2f;
					base.me.Ethereal = EtherealState.Ethereal;
				}
				return false;
			default:
			{
				int num = Game1.stats.gameDifficulty + 1;
				if (((base.me.State == CharState.Grounded && Rand.GetRandomInt(0, 24) < num) || (base.me.State == CharState.Air && Rand.GetRandomInt(0, 48) < num)) && p != null && p.flag != 20)
				{
					base.me.Defense = DefenseStates.Parrying;
					if (!(p is Hit) && base.me.State == CharState.Grounded)
					{
						base.me.SetAnim("parry", 0, 0);
					}
				}
				return true;
			}
			}
		}

		private void InitHurtEffect(Particle p, CharDir dir, float jump, float slide)
		{
			if (p != null)
			{
				Character[] character = Game1.character;
				Game1.pManager.MakeSlash(character[p.owner].Definition.charType, base.me.Definition.charType, p.location, 1.2f, "KO", base.me.RandomSkin, base.me.defaultColor, character[p.owner].Face);
				Game1.pManager.AddShockRing(base.me.Location, 0.5f, 5);
				VibrationManager.SetScreenShake(0.4f);
				VibrationManager.SetBlast(0.5f, p.location);
				base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 2f);
				base.me.Face = dir;
				base.me.SetJump(jump, jumped: false);
				base.me.Slide(slide);
				base.me.SetAnim("hurtup", 0, 0);
			}
		}

		public override void MapCollision(Map map, Character[] c, Character me)
		{
			if (!this.evadingLava || me.Location.Y < 4440f || me.Location.X < 2500f || me.Location.X > 6400f)
			{
				base.MapCollision(map, c, me);
			}
		}

		public Vector2 PlayerLayerLoc(Vector2 locSrc, int l)
		{
			float num = this.LayerScale(l);
			Vector2 vector = locSrc * Game1.worldScale - Game1.Scroll * num + new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2) * (1f - num) * (1f - Game1.worldScale);
			return (vector + Game1.Scroll) / Game1.worldScale;
		}

		public float LayerScale(int l)
		{
			switch (l)
			{
				case 0:
					return 0.2f;
				case 1:
					return 0.3f;
				case 2:
					return 0.5f;
				case 3:
					return 0.85f;
				case 4:
					return 0.9f;
				case 7:
					return 1.1f;
				case 8:
					return 1.25f;
				case 10:
					return 1.5f;
				case 11:
					return 1.75f;
			}
			return 1f;
		}

		private void AvoidLava(Character[] c)
		{
			if (base.me.Location.Y > 5500f)
			{
				base.me.SetAnim("idle00", 0, 0);
				base.me.SetJump(2400f, jumped: true);
				base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 0.5f);
				base.me.Face = ((base.me.Location.X < c[0].Location.X) ? CharDir.Right : CharDir.Left);
				base.me.Slide(1200f);
				if (base.me.GrabbedBy > -1)
				{
					c[base.me.GrabbedBy].Holding = false;
					base.me.GrabbedBy = -1;
				}
				if (base.me.Location.X > 2500f && base.me.Location.X < 6400f)
				{
					this.evadingLava = true;
					base.jobType = JobType.MeleeChase;
				}
			}
			else if (base.me.Trajectory.Y > 0f)
			{
				this.evadingLava = false;
			}
		}

		private void ThrowSpheres(Character[] c)
		{
			if (this.sphereTimer < 0f && base.me.State == CharState.Grounded && ((base.me.Location - c[0].Location).Length() > 600f || c[0].AnimName.StartsWith("hang")) && !base.me.AnimName.StartsWith("attack"))
			{
				base.FindTarg(c);
				if (base.targ > -1)
				{
					if (base.me.Location.X > c[base.targ].Location.X && base.me.Face == CharDir.Right)
					{
						base.me.Face = CharDir.Left;
					}
					else if (base.me.Location.X < c[base.targ].Location.X && base.me.Face == CharDir.Left)
					{
						base.me.Face = CharDir.Right;
					}
				}
				base.me.SetAnim("attackthrow", 0, 1);
				this.sphereTimer = 3f;
			}
			if (base.me.AnimName == "attackthrow")
			{
				if (base.me.Location.X > c[0].Location.X && base.me.Face == CharDir.Right)
				{
					base.me.Face = CharDir.Left;
				}
				else if (base.me.Location.X < c[0].Location.X && base.me.Face == CharDir.Left)
				{
					base.me.Face = CharDir.Right;
				}
			}
		}

		private void AddExplosion(Character[] c, int random)
		{
			if (Rand.GetRandomInt(0, random) == 0)
			{
				VibrationManager.SetScreenShake(1f / Game1.worldScale);
				VibrationManager.Rumble(Game1.currentGamePad, 0.2f);
				Vector2 vector = c[0].Location * this.LayerScale(4) + Rand.GetRandomVector2(-500f, 500f, -400f, 400f);
				VibrationManager.SetBlast(0.6f, vector);
				for (int i = 0; i < 20; i++)
				{
					Game1.pManager.AddExplosion(vector + Rand.GetRandomVector2(-250f, 250f, -250f, 250f), 3f, Rand.GetRandomInt(0, 3) == 0, 4);
				}
			}
		}

		private void SpawnAllie(Character[] c, CharacterType charType, Vector2 loc, int max)
		{
			if (Game1.events.anyEvent)
			{
				return;
			}
			int num = 0;
			for (int i = 1; i < c.Length; i++)
			{
				if (c[i].Exists == CharExists.Exists && c[i].Definition.charType == charType)
				{
					num++;
					if (num == max)
					{
						break;
					}
				}
			}
			if (num < max)
			{
				int num2 = Game1.events.SpawnCharacter(loc, "enemy", charType, Team.Enemy, ground: false);
				c[num2].alwaysUpdatable = true;
			}
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (this.sphereTimer >= 0f)
			{
				this.sphereTimer -= Game1.FrameTime * 2f;
			}
			if (this.hitTime > 0f)
			{
				this.hitTime -= Game1.FrameTime;
			}
			else
			{
				this.hurtCount = 0;
			}
			if (base.me.Name == "atease")
			{
				if (base.me.AnimName != "atease")
				{
					base.me.SetAnim("atease", 0, 0);
				}
				base.me.Ethereal = EtherealState.Ethereal;
				base.Update(c, ID, map);
				return;
			}
			bool flag = false;
			switch (this.bossStage)
			{
			case 1:
			{
				map.rightBlock = 5520f;
				flag = true;
				base.me.Ethereal = EtherealState.Ethereal;
				float num = 4750f;
				base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 1f);
				if (base.me.State == CharState.Grounded)
				{
					base.me.SetAnim("run", 0, 0);
					if (base.me.Location.X < num)
					{
						base.jobType = JobType.RunRight;
					}
					else if (base.me.Location.X > num + 40f)
					{
						base.jobType = JobType.RunLeft;
					}
					else
					{
						this.bossStage = 2;
						base.jobType = JobType.Idle;
						base.me.Trajectory.X *= 0.2f;
					}
					if (c[0].Location.X > base.me.Location.X)
					{
						c[0].Face = CharDir.Right;
						c[0].Evade(Game1.pManager);
					}
				}
				break;
			}
			case 2:
				base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 0f);
				map.rightBlock = 5460f;
				flag = true;
				base.jobType = JobType.Idle;
				base.me.Ethereal = EtherealState.Immovable;
				base.me.Face = CharDir.Left;
				if (base.me.State == CharState.Grounded && base.me.AnimName != "injured")
				{
					base.me.SetAnim("injured", 0, 0);
				}
				if (c[0].Location.X > base.me.Location.X)
				{
					c[0].Face = CharDir.Right;
					c[0].Evade(Game1.pManager);
				}
				break;
			case 3:
				if (Rand.GetRandomInt(0, 100) == 0)
				{
					this.SpawnAllie(c, CharacterType.AirShip, new Vector2(Rand.GetRandomInt(2800, 4600) * 2, 100f), 1);
				}
				if (Rand.GetRandomInt(0, 100) == 0)
				{
					this.SpawnAllie(c, CharacterType.Moonblood, new Vector2(Rand.GetRandomInt(2800, 4600) * 2, 100f), 3);
				}
				break;
			case 4:
				base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 0f);
				flag = true;
				base.jobType = JobType.Idle;
				if (base.me.State == CharState.Grounded && base.me.AnimName != "injured")
				{
					base.me.SetAnim("injured", 0, 0);
				}
				break;
			case 5:
				if (Rand.GetRandomInt(0, 150) == 0)
				{
					this.SpawnAllie(c, CharacterType.Soldier, new Vector2(Rand.GetRandomInt(2800, 4600) * 2, 1300f), 2);
				}
				break;
			case 6:
				Game1.events.InitEvent(740, isSideEvent: false);
				break;
			case 10:
				this.AvoidLava(c);
				this.AddExplosion(c, 100);
				break;
			case 11:
			{
				Game1.camera.GetZoomRectList.Clear();
				this.AvoidLava(c);
				this.AddExplosion(c, 100);
				flag = true;
				base.me.Ethereal = EtherealState.Ethereal;
				float num2 = 4360f;
				base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 1f);
				if (base.me.State == CharState.Grounded)
				{
					base.me.SetAnim("run", 0, 0);
					if (base.me.Location.X < num2)
					{
						base.jobType = JobType.RunRight;
						break;
					}
					if (base.me.Location.X > num2 + 40f)
					{
						base.jobType = JobType.RunLeft;
						break;
					}
					Game1.camera.GetZoomRectList.Add(new Rectangle(0, 0, 10000, 10000), new Vector2(70f, 20f));
					Game1.camera.GetZoomRectList.Add(new Rectangle(3600, 4000, 1520, 1120), new Vector2(100f, 40f));
					Game1.camera.GetPanRectList.Clear();
					this.bossStage = 12;
					base.jobType = JobType.Idle;
					base.me.Trajectory.X *= 0.2f;
				}
				break;
			}
			case 12:
				c[0].StatusEffect = StatusEffects.Normal;
				base.me.StatusEffect = StatusEffects.Normal;
				this.AddExplosion(c, 40);
				base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 0f);
				flag = true;
				base.jobType = JobType.Idle;
				base.me.Ethereal = EtherealState.Immovable;
				base.FaceToTarg(c);
				if (base.me.State == CharState.Grounded && base.me.AnimName != "injured")
				{
					base.me.SetAnim("injured", 0, 0);
				}
				break;
			case 20:
				c[0].StatusEffect = StatusEffects.Normal;
				base.me.StatusEffect = StatusEffects.Normal;
				this.AddExplosion(c, 40);
				flag = true;
				base.jobType = JobType.Idle;
				if (map.path == "lava22b")
				{
					base.me.Face = CharDir.Right;
					base.me.Ethereal = EtherealState.Ethereal;
					if (base.me.AnimName != "suffer")
					{
						base.me.SetAnim("suffer", 0, 0);
					}
					base.me.ShadowWidth = 0f;
					base.me.Location = (base.me.PLoc = new Vector2(3316f, 1970f));
				}
				break;
			}
			if (this.evadingLava)
			{
				return;
			}
			if (base.me.DyingFrame > -1f)
			{
				base.me.DyingFrame = -1f;
				base.me.HP = base.me.MaxHP;
				base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 1f);
				this.hurtCount = 0;
				this.bossStage++;
				switch (this.bossStage)
				{
				case 1:
					Game1.camera.GetZoomRectList[new Rectangle(1800, 10, 3600, 1790)] = new Vector2(80f, 50f);
					break;
				case 4:
				{
					for (int i = 0; i < c.Length; i++)
					{
						if (c[i].Exists == CharExists.Exists && c[i].Definition.charType == CharacterType.Moonblood)
						{
							c[i].KillMe(instantly: false);
						}
					}
					break;
				}
				}
			}
			if (flag)
			{
				this.hurtCount = 0;
				if (base.jobType == JobType.Avoid)
				{
					base.jobType = JobType.Idle;
				}
			}
			else
			{
				if (!base.overrideAI && !Game1.events.anyEvent)
				{
					if (this.bossStage > 2)
					{
						this.ThrowSpheres(c);
						if (base.targ > -1 && c[base.targ].Location.Y < base.me.Location.Y - 200f && base.me.State == CharState.Grounded && base.CanAttack() && Rand.GetRandomInt(0, 20) == 0)
						{
							base.FaceTarg(c);
							this.overrideAnim = true;
							base.me.SetAnim("attack01", 1, 1);
						}
					}
					if (base.DodgeAttack(c, base.targ, 400f, 1000f, base.me.JumpVelocity / 2f, faceTarg: true, base.me.Aggression / 2))
					{
						Sound.PlayCue("gaius_evade", base.me.Location, (c[0].Location - base.me.Location).Length());
					}
				}
				if (base.jobType == JobType.Avoid)
				{
					base.jobFrame = -1f;
				}
				if (base.jobFrame < 0f)
				{
					float randomFloat = Rand.GetRandomFloat(0f, 1f);
					if (randomFloat < 0.8f)
					{
						base.jobType = JobType.MeleeChase;
						base.jobFrame = Rand.GetRandomFloat(2f, 6f);
					}
					else
					{
						base.FaceToTarg(c);
						base.jobType = JobType.Idle;
						base.jobFrame = Rand.GetRandomFloat(0.2f, 2f);
					}
					base.targ = base.FindTarg(c);
				}
			}
			base.Update(c, ID, map);
		}
	}
}
