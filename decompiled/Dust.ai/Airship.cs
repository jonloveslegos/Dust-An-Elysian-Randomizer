using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Dust.Particles;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.ai
{
	public class Airship : AI
	{
		private byte attackStage;

		public Airship(Character c)
		{
			Sound.PlayFollowCharCue("airship_hover", c.ID);
			this.attackStage = 0;
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 5000;
			c.Strength = 200;
			c.Speed = 1400f;
			c.JumpVelocity = 1400f;
			c.DefaultHeight = 300;
			c.DefaultWidth = 400;
			c.FlyType = FlyingType.CanFly;
			c.LiftType = CanLiftType.Immovable;
			c.DamageType = DamageTypes.NotSpecies;
			c.alwaysUpdatable = true;
			base.PrepareStats(c, 1f);
		}

		public override void GetAmbientAudio(ref Vector2 loc, ref Vector2 traj, ref bool _exists)
		{
			loc = (base.me.Location + Game1.character[0].Location) / 2f;
			_exists = base.me.Exists == CharExists.Exists;
		}

		public override void MapCollision(Map map, Character[] c, Character me)
		{
			if (me.DyingFrame > -1f || me.AnimName.StartsWith("hurt"))
			{
				base.MapCollision(map, c, me);
			}
		}

		public override void SpecialTrigger(Trigger trig, Vector2 loc, float rot, float scale, ParticleManager pMan)
		{
			if (!base.me.renderable)
			{
				return;
			}
			switch (trig)
			{
			case Trigger.Special0:
				pMan.AddCannonFireBall(loc, new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot)) * 2000f, 0.7f, base.me.Strength, base.me.ID, 5);
				break;
			case Trigger.Special1:
			{
				for (int i = 0; i < 4; i++)
				{
					Game1.pManager.AddExplosion(loc + Rand.GetRandomVector2(-200f, 200f, -200f, 200f), 2f, makeSmoke: false, Rand.GetRandomInt(5, 7));
				}
				break;
			}
			}
		}

		public override string CheckAnimName(string animName)
		{
			if ((float)base.me.HP > (float)base.me.MaxHP * 0.25f)
			{
				if (animName == "hurtup" && Rand.GetRandomInt(0, 20) > 0)
				{
					return base.me.AnimName;
				}
				if (animName == "biggrab")
				{
					return base.me.AnimName;
				}
			}
			if (base.me.AnimName == "biggrab")
			{
				base.me.DieType = DyingTypes.BodyVanish;
				base.me.KillMe(instantly: true);
				Character[] character = Game1.character;
				for (int i = 1; i < character.Length; i++)
				{
					if (character[i].Exists != CharExists.Exists || character[i].LiftType >= CanLiftType.Immovable || !((character[i].Location - character[0].Location).Length() < 3000f))
					{
						continue;
					}
					if (character[i].Defense != 0)
					{
						character[i].SetAnim("godown", 0, 0);
						if (character[i].AnimName == "godown")
						{
							character[i].DownTime = character[i].MaxDownTime;
						}
					}
					else
					{
						character[i].SetJump(Rand.GetRandomInt(1400, 3000), jumped: false);
						character[i].Slide(Rand.GetRandomInt(-2000, -800));
						HitManager.SetHurt(character[0].Location.X, character, i, up: true);
					}
				}
			}
			return base.CheckAnimName(animName);
		}

		public override void Die(ParticleManager pMan, Texture2D texture)
		{
			Vector2 vector = base.me.Location + new Vector2(0f, -200f);
			pMan.AddShockRing(vector, 1f, 5);
			VibrationManager.SetBlast(1f, vector);
			VibrationManager.SetScreenShake(2f);
			Game1.map.MapSegFrameSpeed = 0.4f;
			Sound.PlayCue("cannon_explode", base.me.Location, (base.me.Location - Game1.character[0].Location).Length() / 4f);
			int num = 400;
			for (int i = 0; i < 24; i++)
			{
				Game1.pManager.AddExplosion(vector + Rand.GetRandomVector2(-num, num, -num, num + 50), Rand.GetRandomFloat(1.8f, 2.8f), (Rand.GetRandomInt(0, 3) != 0) ? true : false, 6);
			}
			for (int j = 0; j < 8; j++)
			{
				Vector2 loc = vector + Rand.GetRandomVector2(-100f, 100f, -100f, 100f);
				Game1.pManager.AddBounceSpark(loc, Rand.GetRandomVector2(-800f, 800f, -500f, 10f), 0.5f, 6);
			}
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			base.me.KeyLeft = (base.me.KeyRight = (base.me.KeyUp = (base.me.KeyDown = false)));
			if (base.me.GrabbedBy > -1)
			{
				Sound.StopFollowCharCue(base.me.ID);
			}
			else
			{
				if ((Game1.events.anyEvent || Game1.menu.prompt != promptDialogue.None || Game1.hud.unlockState != 0) && !base.overrideAI)
				{
					return;
				}
				if (base.me.renderable && (float)base.me.HP < (float)base.me.MaxHP * 0.25f)
				{
					base.me.LiftType = CanLiftType.Normal;
					if (base.me.State == CharState.Air)
					{
						if (base.me.DyingFrame > -1f)
						{
							base.me.Floating = true;
							base.me.DyingFrame = 0f;
							Game1.pManager.AddExplosion(base.me.Location + Rand.GetRandomVector2(-base.me.DefaultWidth, base.me.DefaultWidth, -base.me.DefaultHeight, 0f), 2f, Rand.GetRandomInt(0, 3) == 0, Rand.GetRandomInt(5, 7));
							if (Rand.GetRandomInt(0, 20) == 0)
							{
								Sound.PlayCue("explosion_01", base.me.Location, (base.me.Location - c[0].Location).Length() / 2f);
							}
							return;
						}
						Game1.pManager.AddFlamePuff(base.me.Location + Rand.GetRandomVector2(-base.me.DefaultWidth / 2, base.me.DefaultWidth / 2, -base.me.DefaultHeight, 0f), new Vector2(0f, -200f) + base.me.Trajectory * 0.2f, 1.5f, Rand.GetRandomInt(5, 7));
					}
					else if (base.me.DyingFrame > -1f)
					{
						base.me.KillMe(instantly: true);
					}
				}
				if (base.me.State == CharState.Grounded)
				{
					base.me.SetJump(1000f, jumped: true);
				}
				base.targ = 0;
				switch (this.attackStage)
				{
				case 0:
					if (base.me.renderable)
					{
						this.attackStage = 1;
						base.me.Face = ((!(base.me.Location.X > c[base.targ].Location.X)) ? CharDir.Right : CharDir.Left);
					}
					break;
				case 1:
					base.me.Speed = 1400f;
					if (base.me.Face != CharDir.Right)
					{
						_ = (Game1.Scroll.X + (float)Game1.screenWidth + 400f) / Game1.worldScale;
					}
					else
					{
						_ = (Game1.Scroll.X - 400f) / Game1.worldScale;
					}
					base.me.SetAnim("flyby", 0, 0);
					this.attackStage = 2;
					break;
				case 2:
					if ((base.me.Face == CharDir.Left && base.me.Trajectory.X > 0f) || (base.me.Face == CharDir.Right && base.me.Trajectory.X < 0f))
					{
						base.me.ResetRotation();
					}
					if (base.me.Face == CharDir.Left)
					{
						base.me.KeyLeft = true;
					}
					else
					{
						base.me.KeyRight = true;
					}
					if (base.me.Location.Y < c[base.targ].Location.Y - 400f)
					{
						base.me.Trajectory.Y = 0f - (base.me.Location.Y - (c[base.targ].Location.Y - 400f));
					}
					else if (base.me.Location.Y > c[base.targ].Location.Y - 300f)
					{
						base.me.Trajectory.Y = 0f - (base.me.Location.Y - (c[base.targ].Location.Y - 300f));
					}
					else
					{
						base.me.Trajectory.Y = 0f;
					}
					if (!Game1.events.anyEvent && base.me.AnimName.StartsWith("fly") && Math.Abs(base.me.Location.X - c[base.targ].Location.X) < 1000f && base.me.Location.Y < c[base.targ].Location.Y - 300f)
					{
						base.me.KeyLeft = (base.me.KeyRight = false);
						base.me.SetAnim("turnin", 0, 0);
						base.me.Trajectory.Y = 0f;
						base.me.Speed = 400f;
						this.attackStage = 3;
					}
					break;
				case 3:
					base.me.Trajectory.X += ((float)((base.me.Face != 0) ? 1 : (-1)) * base.me.Speed - base.me.Trajectory.X) * Game1.FrameTime * 10f;
					if (base.me.Face == CharDir.Left)
					{
						base.me.KeyLeft = true;
					}
					else
					{
						base.me.KeyRight = true;
					}
					if (base.me.AnimName == "turnout")
					{
						this.attackStage = 4;
						base.me.Speed = 1400f;
					}
					break;
				case 4:
				{
					if (base.me.Trajectory.Y > -1200f)
					{
						base.me.Trajectory.Y -= Game1.FrameTime * 150f;
					}
					bool flag;
					if (base.me.Face == CharDir.Left)
					{
						base.me.KeyLeft = true;
						flag = base.me.Location.X > (Game1.Scroll.X + 500f) / Game1.worldScale;
					}
					else
					{
						base.me.KeyRight = true;
						flag = base.me.Location.X < (Game1.Scroll.X + (float)Game1.screenWidth - 500f) / Game1.worldScale;
					}
					if (!flag)
					{
						base.me.Trajectory.X *= 0.5f;
						this.attackStage = 5;
					}
					break;
				}
				case 5:
					if (base.me.AnimName == "flyby")
					{
						base.me.SetAnim("turnaround", 0, 0);
					}
					if (base.me.AnimName == "turnout")
					{
						base.me.Face = ((base.me.Face != CharDir.Right) ? CharDir.Right : CharDir.Left);
						this.attackStage = 2;
					}
					break;
				}
				if (this.attackStage > 1 && this.attackStage < 5 && ((base.me.Location.X < map.leftEdge + 1000f && base.me.Face == CharDir.Left) || (base.me.Location.X > map.rightEdge - 1000f && base.me.Face == CharDir.Right) || base.me.WallInWay))
				{
					if (base.me.AnimName == "flyby")
					{
						base.me.SetAnim("turnaround", 0, 0);
					}
					if (base.me.AnimName == "turnout")
					{
						base.me.Face = ((base.me.Face != CharDir.Right) ? CharDir.Right : CharDir.Left);
						this.attackStage = 2;
					}
				}
			}
		}
	}
}
