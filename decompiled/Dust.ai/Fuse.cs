using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Microsoft.Xna.Framework;

namespace Dust.ai
{
	internal class Fuse : AI
	{
		private int hurtCount;

		private float hitTime;

		public Fuse(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 3000;
			c.Speed = 900f;
			c.Aggression = 100;
			c.JumpVelocity = 2000f;
			c.Strength = 30;
			c.DefaultHeight = 250;
			c.DefaultWidth = 200;
			c.ShadowWidth = 1f;
			c.maxAnimFrames = 24;
			c.defaultColor = Color.White;
			base.PrepareStats(c, 1f);
		}

		public override string CheckAnimName(string animName)
		{
			if (animName.Contains("hurt"))
			{
				this.hurtCount++;
				this.hitTime = 1f;
			}
			if (base.me.AnimName == "biggrab" || this.hurtCount > 16)
			{
				base.me.AnimName = "idle00";
				base.me.Slide(-1500f);
				base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 2f);
				this.hurtCount = 0;
				base.me.SetJump(1200f, jumped: true);
				return "jump";
			}
			return animName;
		}

		public override bool PlaySound(string sound)
		{
			if (base.me.Name == "fuseboss")
			{
				Sound.PlayCue(sound, base.me.Location, 0f);
				return true;
			}
			return base.PlaySound(sound);
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			if (base.me.Name == "fuseboss" && base.me.DyingFrame > -1f)
			{
				Game1.events.currentEvent = 100;
				Game1.events.eventType = EventType.MainEvent;
				c[0].Ethereal = EtherealState.Ethereal;
			}
			if (base.me.Name == "fusescript")
			{
				base.me.Ethereal = EtherealState.Ethereal;
				base.me.Face = CharDir.Left;
				if (Game1.events.currentEvent == 100)
				{
					base.me.SetAnim("dying", 0, 2);
				}
			}
			else
			{
				if (!Game1.events.anyEvent && Game1.menu.prompt == promptDialogue.None && base.me.LungeState == LungeStates.None && base.me.DyingFrame == -1f)
				{
					if (this.hitTime > 0f)
					{
						this.hitTime -= Game1.FrameTime;
					}
					else
					{
						this.hurtCount = 0;
					}
					if (!base.me.renderable && Rand.GetRandomInt(0, 400) == 0)
					{
						Sound.PlayCue("fuse_over_here", base.me.Location, (c[0].Location - base.me.Location).Length() / 32f);
					}
					if (Rand.GetRandomInt(0, 40) == 0 && base.me.State == CharState.Grounded && (base.me.AnimName.StartsWith("idle") || base.me.AnimName.StartsWith("run")))
					{
						base.me.SetAnim("attack02", 0, 2);
					}
					if (base.me.Name == "fuseboss")
					{
						if (Rand.GetRandomInt(0, 20) == 0 && !base.me.AnimName.StartsWith("attack") && !base.me.AnimName.StartsWith("hurt") && base.me.State == CharState.Grounded)
						{
							base.me.KeyLeft = (base.me.KeyRight = false);
							if (base.me.Location.Y > 3600f)
							{
								if (base.me.Location.X < Game1.map.leftBlock + 1100f || base.me.Location.X > Game1.map.rightBlock - 1200f)
								{
									base.me.SetJump(2800f, jumped: true);
									base.me.SetAnim("jump", 0, 2);
								}
							}
							else if (base.jobType == JobType.Avoid)
							{
								base.me.FallOff(c, fallThrough: true);
							}
						}
						if (base.me.CanHurtFrame + base.me.CanHurtProjectileFrame > 0f && base.me.State == CharState.Grounded)
						{
							if (base.me.Location.X > map.rightBlock - 500f)
							{
								base.me.Face = CharDir.Left;
								base.me.SetAnim("idle00", 0, 0);
								base.me.SetLunge(LungeStates.Lunging, base.me.Speed * 2f, base.me.JumpVelocity);
							}
							else if (base.me.Location.X < map.leftBlock + 500f)
							{
								base.me.Face = CharDir.Right;
								base.me.SetAnim("idle00", 0, 0);
								base.me.SetLunge(LungeStates.Lunging, base.me.Speed * 2f, base.me.JumpVelocity);
							}
						}
					}
				}
				if (base.jobType == JobType.Avoid && base.me.State == CharState.Grounded)
				{
					base.me.CanHurtFrame = (base.me.CanHurtProjectileFrame = 0.1f);
				}
				if (base.me.AnimName.StartsWith("hurtland") || (base.me.AnimName.StartsWith("hurt") && Rand.GetRandomInt(0, 200) == 0))
				{
					base.jobType = JobType.Avoid;
					base.jobFrame = Rand.GetRandomFloat(1f, 3f);
					base.FaceToTarg(c);
				}
				else if (base.jobFrame < 0f)
				{
					float randomFloat = Rand.GetRandomFloat(0f, 1f);
					if (randomFloat < 0.2f)
					{
						base.jobType = JobType.ShootAnywhere;
						base.jobFrame = Rand.GetRandomFloat(2f, 4f);
					}
					if (randomFloat < 0.7f)
					{
						base.jobType = JobType.ChaseStop;
						base.jobFrame = Rand.GetRandomFloat(2f, 4f);
					}
					else if (randomFloat < 0.9f)
					{
						base.FaceToTarg(c);
						base.jobType = JobType.Idle;
						base.jobFrame = Rand.GetRandomFloat(0.5f, 2f);
					}
					else
					{
						base.jobType = JobType.Avoid;
						base.jobFrame = Rand.GetRandomFloat(0.2f, 1f);
						base.FaceToTarg(c);
					}
				}
			}
			base.Update(c, ID, map);
		}
	}
}
