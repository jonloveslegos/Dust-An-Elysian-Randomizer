using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Material : Particle
	{
		private bool stopped;

		private float pulseTime;

		private float animFrameTime;

		private int animFrame;

		private float frame;

		private float rotation;

		private Rectangle sRect;

		private Rectangle renderRect;

		private Vector2 ploc = Vector2.Zero;

		public Material(Vector2 loc, Vector2 traj, int _flag)
		{
			this.Reset(loc, traj, _flag);
		}

		public void Reset(Vector2 loc, Vector2 traj, int _flag)
		{
			base.exists = Exists.Init;
			this.frame = 20f;
			base.location = (this.ploc = loc);
			base.trajectory = traj;
			base.flag = _flag;
			this.pulseTime = Rand.GetRandomFloat(0f, 6.28f);
			this.animFrame = Rand.GetRandomInt(0, 100);
			this.animFrameTime = 0f;
			this.stopped = false;
			base.renderState = RenderState.Additive;
			int num = base.flag / 12;
			this.sRect = new Rectangle(num * 60, (base.flag - num * 12) * 60, 60, 60);
			this.renderRect = new Rectangle(-2000, -2000, Game1.screenWidth + 4000, Game1.screenHeight + 3000);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			bool flag = true;
			if (pMan.Seek(c, this, 0, 60f, 1000f))
			{
				flag = false;
				this.stopped = false;
				if (base.isSpun < 0.02f)
				{
					base.trajectory.Y = -1000f;
				}
				base.trajectory.X = MathHelper.Clamp(base.trajectory.X, -3000f, 3000f);
				base.trajectory.Y = MathHelper.Clamp(base.trajectory.Y, -3000f, 3000f);
			}
			if (HitManager.CheckPickup(base.location + new Vector2(0f, -32f), c) && Game1.stats.playerLifeState == 0)
			{
				if (Game1.stats.Material[base.flag] < 9999)
				{
					this.frame = 0f;
					Game1.hud.InitMiniPrompt(MiniPromptType.MaterialAcquired, base.flag, blueprint: false);
					Sound.PlayCue("item_pickup");
					Game1.stats.Material[base.flag] = Math.Max(Game1.stats.Material[base.flag], 0);
					Game1.stats.Material[base.flag]++;
					Game1.questManager.UpdateQuests(0);
				}
				else if (Game1.hud.miniPromptState == 0)
				{
					Game1.hud.InitMiniPrompt(MiniPromptType.MaterialFull, base.flag, blueprint: false);
				}
			}
			if (!this.stopped)
			{
				if (map.CheckCol(base.location) > 0)
				{
					base.location.X = this.ploc.X;
					base.trajectory.X = 0f - base.trajectory.X;
				}
				if (Math.Abs(base.trajectory.X) > 2f)
				{
					base.trajectory.X /= 1.04f;
				}
				else
				{
					base.trajectory.X = 0f;
				}
				if (base.trajectory.Y < 1500f)
				{
					base.trajectory.Y += Game1.FrameTime * 2000f;
				}
				if (base.trajectory.Y < 0f)
				{
					if (map.CheckCol(new Vector2(base.location.X, base.location.Y - 32f)) > 0 || base.location.Y < map.topEdge)
					{
						base.trajectory.Y = 0f;
						base.location.Y = this.ploc.Y;
					}
				}
				else
				{
					float num = map.CheckPCol(base.location + new Vector2(0f, 30f), this.ploc, canFallThrough: false, init: false);
					if (num > 0f)
					{
						base.location.Y = num - 30f;
						if (base.trajectory.Y > 250f)
						{
							if (flag)
							{
								Sound.PlayDropCue("item_bounce", base.location, base.trajectory.Y);
							}
							if (base.isSpun == 0f)
							{
								base.trajectory.Y = (0f - base.trajectory.Y) / 2f;
							}
							else
							{
								base.trajectory.Y = -1000f;
								base.isSpun = 0f;
							}
						}
						else
						{
							base.trajectory = Vector2.Zero;
							this.stopped = true;
						}
					}
				}
			}
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y) || this.frame <= 0f)
			{
				base.Reset();
				if (this.frame == 0f)
				{
					pMan.AddLenseFlare(base.location, 0.4f, 1, 5);
				}
			}
			if (!Game1.events.anyEvent && Game1.menu.prompt == promptDialogue.None)
			{
				this.frame -= gameTime;
			}
			this.pulseTime += Game1.FrameTime;
			if (this.pulseTime > 6.28f)
			{
				this.pulseTime -= 6.28f;
			}
			this.animFrameTime += Game1.FrameTime * 48f;
			if (this.animFrameTime > 1f)
			{
				this.animFrameTime = 0f;
				this.animFrame++;
				if (this.animFrame > 100)
				{
					this.animFrame = 0;
				}
			}
			this.ploc = base.location;
			this.rotation = (float)Math.Sin(this.pulseTime) / 8f;
			base.location += base.trajectory * gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			base.maskGlow = MathHelper.Clamp(this.frame, 0f, 0.25f);
			Vector2 position = (base.location + new Vector2(0f, (float)Math.Sin(this.pulseTime * 2f) * 20f)) * Game1.worldScale - Game1.Scroll;
			float num = MathHelper.Clamp(this.frame, 0f, 1f);
			if (!Game1.pManager.renderingAdditive || (this.pulseTime < 0.5f && (int)(this.pulseTime * 20f) % 2 == 0))
			{
				sprite.Draw(particlesTex[5], position, this.sRect, new Color(1f, 1f, 1f, num), this.rotation, new Vector2(30f, 30f), 1.2f * worldScale, SpriteEffects.None, 1f);
			}
			if (Game1.pManager.renderingAdditive && this.animFrame < 21)
			{
				sprite.Draw(particlesTex[2], position, new Rectangle(1600 + this.animFrame * 60, 2970, 60, 80), Color.White * num, 0f, new Vector2(30f, 40f), 2f, SpriteEffects.None, 0f);
			}
		}
	}
}
