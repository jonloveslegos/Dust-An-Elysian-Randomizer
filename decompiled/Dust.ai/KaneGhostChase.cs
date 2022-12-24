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
	public class KaneGhostChase : AI
	{
		private int pathID;

		private Vector2 pathLoc;

		private double pathTimeTarget;

		private double pathTime;

		private Vector2[] pathNodes;

		private Vector2 playerTargetLoc;

		private float pulseTime = -4f;

		private byte pathAttempt;

		public KaneGhostChase(Character c)
		{
			base.me = c;
			c.Team = Team.Enemy;
			c.MaxHP = 30000000;
			c.Strength = (int)((float)Game1.character[0].MaxHP * 0.2f);
			c.FlyType = FlyingType.CanFly;
			c.LiftType = CanLiftType.NoLift;
			c.Speed = 300f;
			c.JumpVelocity = 1400f;
			c.Aggression = 5;
			c.DefaultHeight = 600;
			c.DefaultWidth = 400;
			c.maxAnimFrames = 24;
			c.MaxDownTime = 10f;
			c.MaskGlow = 2f;
			c.alwaysUpdatable = true;
			c.defaultColor = new Color(0.2f, 1f, 0.2f, 1f);
			Sound.PlayFollowCharCue("kaneghost_hover", c.ID);
			this.BuildPath(c.Location, Game1.map);
		}

		private void BuildPath(Vector2 loc, Map map)
		{
			float num = 1E+09f;
			byte b = 0;
			for (int i = 0; i < map.ledges.Length; i++)
			{
				if (map.ledges[i] != null && map.ledges[i].Flag == LedgeFlags.PlatformPath)
				{
					float num2 = (loc - map.ledges[i].Nodes[0]).Length();
					if (num2 < num)
					{
						num = num2;
						b = (byte)i;
					}
				}
			}
			this.pathID = b;
			this.pathNodes = new Vector2[Game1.map.ledges[this.pathID].Nodes.Length];
			for (int j = 0; j < this.pathNodes.Length; j++)
			{
				ref Vector2 reference = ref this.pathNodes[j];
				reference = Game1.map.ledges[this.pathID].Nodes[j];
			}
			this.playerTargetLoc = loc;
			double num3 = 0.0;
			for (int k = 1; k < this.pathNodes.Length; k++)
			{
				num3 += (double)(this.pathNodes[k] - this.pathNodes[k - 1]).Length();
			}
			this.pathTimeTarget = (this.pathTime = this.GetDestination(Game1.character[0].Location, Game1.map, num3));
			if (this.pathTime > 0.5)
			{
				this.pathTime = 1.0;
				base.me.Location = this.pathNodes[this.pathNodes.Length - 1];
			}
			else
			{
				this.pathTime = 0.0;
				base.me.Location = this.pathNodes[0];
			}
		}

		private float GetDestination(Vector2 destLoc, Map map, double maxPathLength)
		{
			float num = 1E+09f;
			float num2 = 0f;
			float result = 0f;
			Vector2 vector = Vector2.Zero;
			int num3 = (int)(maxPathLength / 20.0);
			for (int i = 0; i < num3; i++)
			{
				double num4 = maxPathLength * (double)num2;
				int num5 = 0;
				for (int j = 0; j < this.pathNodes.Length - 1; j++)
				{
					float num6 = (this.pathNodes[num5] - this.pathNodes[num5 + 1]).Length();
					if (num4 > (double)num6)
					{
						num4 -= (double)num6;
						num5++;
					}
				}
				if (num5 < this.pathNodes.Length - 1)
				{
					float num7 = (float)(num4 / (double)(this.pathNodes[num5] - this.pathNodes[num5 + 1]).Length());
					vector = this.pathNodes[num5] * (1f - num7) + this.pathNodes[num5 + 1] * num7;
				}
				float num8 = (destLoc - vector).Length();
				if (num8 < num)
				{
					num = num8;
					result = num2;
				}
				num2 += 10f / (float)num3;
			}
			return result;
		}

		public override void Update(Character[] c, int ID, Map map)
		{
			base.me.CanHurtFrame = Rand.GetRandomInt(-10, 2);
			base.me.Defense = DefenseStates.Parrying;
			base.me.Ethereal = EtherealState.Ethereal;
			base.Update(c, ID, map);
			if (Game1.events.anyEvent || Game1.menu.prompt != promptDialogue.None || Game1.hud.unlockState != 0 || base.me.LungeState != 0 || base.me.DyingFrame != -1f || !(base.me.DownTime <= 0f))
			{
				return;
			}
			base.jobType = JobType.Idle;
			if (!base.me.AnimName.Contains("attack"))
			{
				this.playerTargetLoc += (c[0].Location - new Vector2(0f, 200f) - this.playerTargetLoc) * Game1.FrameTime * 10f;
				double num = 0.0;
				for (int i = 1; i < this.pathNodes.Length; i++)
				{
					num += (double)(this.pathNodes[i] - this.pathNodes[i - 1]).Length();
				}
				this.pathAttempt++;
				if (this.pathAttempt > 30)
				{
					this.pathAttempt = 0;
					this.pathTimeTarget = this.GetDestination(c[0].Location, map, num);
				}
				this.pathTime += (this.pathTimeTarget - this.pathTime) * (double)Game1.FrameTime;
				num *= this.pathTime;
				int num2 = 0;
				for (int j = 0; j < this.pathNodes.Length - 1; j++)
				{
					float num3 = (this.pathNodes[num2] - this.pathNodes[num2 + 1]).Length();
					if (num > (double)num3)
					{
						num -= (double)num3;
						num2++;
					}
				}
				if (num2 < this.pathNodes.Length - 1)
				{
					float num4 = (float)(num / (double)(this.pathNodes[num2] - this.pathNodes[num2 + 1]).Length());
					this.pathLoc = (this.pathNodes[num2] * (1f - num4) + this.pathNodes[num2 + 1] * num4 + this.playerTargetLoc) / 2f;
				}
				base.me.Trajectory = this.pathLoc - base.me.Location;
				base.me.Trajectory.X *= 8f;
				if (map.CheckCol(base.me.Location) > 0)
				{
					base.me.Trajectory.Y += 400 * ((c[0].Location.Y > base.me.Location.Y) ? 1 : (-1));
				}
				base.FaceToTarg(c);
				if ((base.me.Location - c[0].Location).Length() < 400f)
				{
					base.me.KeyAttack = true;
				}
			}
			this.pulseTime -= Game1.FrameTime;
			if (this.pulseTime < 0f)
			{
				this.pulseTime = 0.25f;
				VibrationManager.SetScreenShake(this.pulseTime);
			}
			float num5 = Math.Min(2f - (base.me.Location - c[0].Location).Length() / 1000f, 2f);
			VibrationManager.SetBlast(this.pulseTime * 2f * num5, base.me.Location + Rand.GetRandomVector2(-200f, 200f, -200f, 200f));
			Game1.wManager.SetManualColor(new Vector4(0.8f, 0.8f, 0.8f, Math.Min(Game1.wManager.weatherColor.W + 0.001f, 0f)), Math.Min(1f - num5, 0.2f), Math.Max(Game1.wManager.weatherSaturation - 0.001f, 0.1f), immediately: true);
		}

		public override void Draw(SpriteBatch spriteBatch, Texture2D[] particleTex, bool specialMode)
		{
			base.Draw(spriteBatch, particleTex, specialMode);
		}

		public override void MapCollision(Map map, Character[] c, Character me)
		{
		}

		public override bool CheckHit(Particle p)
		{
			return false;
		}

		public override string CheckAnimName(string animName)
		{
			if (animName == "flyattack")
			{
				return "attack0" + Rand.GetRandomInt(0, 2);
			}
			return animName;
		}

		public void DrawDebug(SpriteBatch sprite, Map map, Texture2D[] particlesTex)
		{
			double num = 0.0;
			for (int i = 1; i < this.pathNodes.Length; i++)
			{
				num += (double)(this.pathNodes[i] - this.pathNodes[i - 1]).Length();
			}
			num *= this.pathTime;
			int num2 = 0;
			for (int j = 0; j < this.pathNodes.Length; j++)
			{
				sprite.Draw(particlesTex[1], this.pathNodes[j] * Game1.worldScale - Game1.Scroll, new Rectangle(576 + Rand.GetRandomInt(0, 10) * 32, 128, 32, 32), Color.White, 0f, new Vector2(16f, 16f), 0.5f, SpriteEffects.None, 0f);
				if (j < this.pathNodes.Length - 1)
				{
					int num3 = (int)((this.pathNodes[j] - this.pathNodes[j + 1]).Length() / 40f);
					for (int k = 0; k < num3; k++)
					{
						float num4 = (float)k / (float)num3;
						Vector2 vector = this.pathNodes[j] * num4 + this.pathNodes[j + 1] * (1f - num4);
						sprite.Draw(particlesTex[1], vector * Game1.worldScale - Game1.Scroll, new Rectangle(576 + Rand.GetRandomInt(0, 10) * 32, 128, 32, 32), Color.White, 0f, new Vector2(16f, 16f), 0.2f, SpriteEffects.None, 0f);
					}
				}
				if (num2 < this.pathNodes.Length - 1)
				{
					float num5 = (this.pathNodes[num2] - this.pathNodes[num2 + 1]).Length();
					if (num > (double)num5)
					{
						num -= (double)num5;
						num2++;
					}
				}
			}
			if (num2 < this.pathNodes.Length - 1)
			{
				float num6 = (float)(num / (double)(this.pathNodes[num2] - this.pathNodes[num2 + 1]).Length());
				Vector2 vector2 = this.pathNodes[num2] * (1f - num6) + this.pathNodes[num2 + 1] * num6;
				vector2 = (this.pathNodes[num2] * (1f - num6) + this.pathNodes[num2 + 1] * num6 + Game1.character[0].Location - new Vector2(0f, 200f)) / 2f;
				sprite.Draw(particlesTex[1], vector2 * Game1.worldScale - Game1.Scroll, new Rectangle(832, 128, 32, 32), Color.Red, 0f, new Vector2(16f, 16f), 1f, SpriteEffects.None, 0f);
			}
		}
	}
}
