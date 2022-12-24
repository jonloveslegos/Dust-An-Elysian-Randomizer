using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Stone : Particle
	{
		private byte animFrame;

		private byte glowAnimFrame;

		private float frame;

		private float animFrameTime;

		private float glowAnimFrameTime;

		private bool stopped;

		private Vector2 ploc;

		private Color stoneColor;

		public Stone(Vector2 loc, Vector2 traj, int flag)
		{
			this.Reset(loc, traj, flag);
		}

		public void Reset(Vector2 loc, Vector2 traj, int flag)
		{
			base.exists = Exists.Init;
			this.animFrame = (byte)Rand.GetRandomInt(0, 40);
			this.stopped = false;
			this.animFrameTime = 0f;
			this.animFrame = (byte)Rand.GetRandomInt(0, 40);
			this.glowAnimFrame = (byte)Rand.GetRandomInt(0, 100);
			base.location = (this.ploc = loc);
			base.trajectory = traj;
			base.flag = flag;
			this.frame = 60f;
			switch (flag)
			{
			case 0:
				this.stoneColor = new Color(1f, 0.2f, 0.2f, 1f);
				break;
			case 1:
				this.stoneColor = new Color(0.25f, 1f, 1f, 1f);
				break;
			}
			base.renderState = RenderState.Additive;
			base.exists = Exists.Exists;
		}

		private void Pickup(ParticleManager pMan, int l)
		{
			Sound.PlayCue("coin_pickup");
			this.frame = 0f;
			for (int i = 0; i < 20; i++)
			{
				Vector2 loc = new Vector2(base.location.X + (float)Rand.GetRandomInt(-80, 80), base.location.Y + (float)Rand.GetRandomInt(-100, 100));
				int randomInt = Rand.GetRandomInt(12, 48);
				float randomFloat = Rand.GetRandomFloat(0.5f, 1.25f);
				pMan.AddSparkle(loc, (int)this.stoneColor.R, (int)this.stoneColor.G, (int)this.stoneColor.B, 1f, randomFloat, randomInt, l);
			}
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
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
			if (HitManager.CheckPickup(base.location + new Vector2(0f, -32f), c) && l == 5)
			{
				if (base.flag == 0)
				{
					Game1.stats.GetChestFromFile(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList[base.owner].UniqueID + " revive stone", pMan);
					this.Pickup(pMan, l);
				}
				else if (base.flag == 1)
				{
					Game1.stats.GetChestFromFile(Game1.navManager.RevealMap[Game1.navManager.NavPath].GameItemList[base.owner].UniqueID + " tp stone", pMan);
					this.Pickup(pMan, l);
				}
			}
			if (this.frame <= 0f)
			{
				base.Reset();
			}
			if (!this.stopped)
			{
				if (map.CheckCol(base.location) > 0 && l == 5)
				{
					base.location.X = this.ploc.X;
					base.trajectory.X = 0f - base.trajectory.X;
				}
				if (Math.Abs(base.trajectory.X) > 1f)
				{
					base.trajectory.X /= 1.04f;
				}
				else
				{
					base.trajectory.X = 0f;
				}
				if (base.trajectory.Y < 1500f && !this.stopped)
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
				else if (base.trajectory.Y > 0f)
				{
					float num = map.CheckPCol(base.location + new Vector2(0f, 60f), this.ploc, canFallThrough: false, init: false);
					if (num > 0f && l == 5)
					{
						base.location.Y = num - 60f;
						if (base.trajectory.Y > 200f)
						{
							if (flag)
							{
								Sound.PlayDropCue("coin_bounce", base.location, base.trajectory.Y);
							}
							base.trajectory.Y = (0f - base.trajectory.Y) / 1.5f;
						}
						else
						{
							base.trajectory.Y = 0f;
							base.isSpun = 0f;
							if (Math.Abs(base.trajectory.X) < 10f)
							{
								this.stopped = true;
							}
						}
					}
				}
			}
			else
			{
				base.trajectory.X = 0f;
			}
			this.animFrameTime += gameTime * 24f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 47)
				{
					this.animFrame = 0;
				}
				this.animFrameTime = 0f;
			}
			this.glowAnimFrameTime += Game1.FrameTime * 48f;
			if (this.glowAnimFrameTime > 1f)
			{
				this.glowAnimFrameTime = 0f;
				this.glowAnimFrame++;
				if (this.glowAnimFrame > 100)
				{
					this.glowAnimFrame = 0;
				}
			}
			this.ploc = base.location;
			base.location += base.trajectory * gameTime;
			if (!Game1.events.anyEvent && Game1.menu.prompt == promptDialogue.None)
			{
				this.frame -= gameTime;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 position = (base.location + new Vector2(0f, (float)Math.Sin(Game1.map.MapSegFrame * 16f) * 20f)) * Game1.worldScale - Game1.Scroll;
			if (!Game1.pManager.renderingAdditive || (this.animFrame < 10 && (int)this.animFrame % 3 == 0))
			{
				sprite.Draw(particlesTex[2], position, new Rectangle(1420 + this.animFrame * 55, 1561, 55, 99), new Color((float)(int)this.stoneColor.R / 255f, (float)(int)this.stoneColor.G / 255f, (float)(int)this.stoneColor.B / 255f, this.frame), 0f, new Vector2(27f, 50f), worldScale, SpriteEffects.None, 1f);
			}
			if (Game1.pManager.renderingAdditive && this.glowAnimFrame < 21)
			{
				sprite.Draw(particlesTex[2], position, new Rectangle(1600 + this.glowAnimFrame * 60, 2970, 60, 80), Color.White * this.frame, 0f, new Vector2(30f, 40f), 2.5f, SpriteEffects.None, 0f);
			}
		}
	}
}
