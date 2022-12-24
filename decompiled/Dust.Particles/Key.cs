using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Key : Particle
	{
		private byte animFrame;

		private float animFrameTime;

		private float frame;

		private bool stopped;

		private Vector2 ploc = Vector2.Zero;

		public Key(Vector2 loc, Vector2 traj, int id)
		{
			this.Reset(loc, traj, id);
		}

		public void Reset(Vector2 loc, Vector2 traj, int id)
		{
			base.exists = Exists.Init;
			this.animFrame = 0;
			this.animFrameTime = Rand.GetRandomFloat(0f, 6.28f);
			this.stopped = false;
			base.maskGlow = 2f;
			base.location = (this.ploc = loc);
			base.trajectory = traj;
			if (base.trajectory.Y == 0f)
			{
				this.stopped = true;
			}
			base.owner = id;
			this.frame = -1f;
			base.renderState = RenderState.Additive;
			base.exists = Exists.Exists;
		}

		private void DrawCorona(SpriteBatch sprite, Texture2D[] particlesTex, Vector2 gameLoc, int l)
		{
			Vector2 vector = new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2);
			Vector2 vector2 = gameLoc - vector;
			float num = Math.Abs(vector2.X + vector2.Y) / 2000f;
			if (!(num < 1f))
			{
				return;
			}
			Color color = new Color(1f, 1f, 1f, 1f - num);
			for (int i = 0; i < 8; i++)
			{
				Vector2 vector3 = vector - vector2 * ((float)i / 2f - 1.75f);
				Rectangle value;
				Vector2 origin;
				Vector2 vector4;
				float rotation;
				switch (i)
				{
				case 1:
					value = new Rectangle(3879, 328, 99, 99);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(1f, 1f);
					rotation = 0f;
					break;
				case 2:
					value = new Rectangle(3780, 328, 99, 99);
					color = new Color(1f, 1f, 1f, 0.5f - num);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(0.5f, 0.5f);
					rotation = 0f;
					break;
				case 3:
					value = new Rectangle(3879, 328, 99, 99);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(0.25f, 0.25f);
					rotation = 0f;
					break;
				case 5:
					value = new Rectangle(3437, 1860, 560, 150);
					origin = new Vector2(280f, 100f);
					vector4 = new Vector2(1f, 2f) * (num + 0.75f);
					rotation = GlobalFunctions.GetAngle(vector3, vector) + 1.57f;
					break;
				case 4:
					value = new Rectangle(3780, 328, 99, 99);
					color = new Color(1f, 1f, 1f, 0.5f - num);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(0.5f, 0.5f);
					rotation = 0f;
					break;
				default:
					value = new Rectangle(3978, 328, 99, 99);
					origin = new Vector2(45f, 45f);
					vector4 = new Vector2(1f, 1f);
					rotation = 0f;
					break;
				}
				sprite.Draw(particlesTex[2], vector3, value, color, rotation, origin, vector4 * Game1.hiDefScaleOffset, SpriteEffects.None, 1f);
			}
			sprite.Draw(particlesTex[2], gameLoc, new Rectangle(2940, 1860, 497, 150), Color.Gray, 0f, new Vector2(248f, 75f), Game1.hiDefScaleOffset, SpriteEffects.None, 1f);
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (this.frame < 0f)
			{
				if (HitManager.CheckPickup(base.location, c) && Game1.stats.playerLifeState == 0)
				{
					Game1.stats.GetChestFromFile(Game1.navManager.RevealMap[Game1.navManager.NavPath].KeyList[base.owner].UniqueID+"key",pMan);
					Sound.PlayCue("key_pickup");
					if (base.owner > -1)
					{
						Game1.navManager.RevealMap[Game1.navManager.NavPath].KeyList[base.owner].Stage = 1;
					}
					if (Game1.worldDark == 0f)
					{
						base.Reset();
						Game1.navManager.CheckRegionTreasure(pMan);
					}
					else
					{
						this.frame = 2f;
					}
					for (int i = 0; i < 20; i++)
					{
						Vector2 loc = new Vector2(base.location.X + (float)Rand.GetRandomInt(-80, 80), base.location.Y + (float)Rand.GetRandomInt(-100, 100));
						int randomInt = Rand.GetRandomInt(12, 48);
						float randomFloat = Rand.GetRandomFloat(0.5f, 1.25f);
						pMan.AddSparkle(loc, Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 1f, randomFloat, randomInt, 5);

					}
				}
			}
			else
			{
				base.maskGlow = this.frame;
				this.frame -= gameTime;
				if (this.frame <= 0f)
				{
					this.frame = 0f;
					base.Reset();
					Game1.navManager.CheckRegionTreasure(pMan);
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
					if (map.CheckCol(base.location + new Vector2(0f, -32f)) > 0 || base.location.Y < map.topEdge)
					{
						base.trajectory.Y = 0f;
						base.location.Y = this.ploc.Y;
					}
				}
				else
				{
					float num = map.CheckPCol(base.location + new Vector2(0f, 100f), this.ploc, canFallThrough: false, init: false);
					if (num > 0f)
					{
						base.location.Y = num - 100f;
						if (base.trajectory.Y > 120f)
						{
							if (new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight).Contains((int)vector.X, (int)vector.Y))
							{
								Sound.PlayDropCue("coin_bounce", base.location, base.trajectory.Y);
							}
							base.trajectory.Y = (0f - base.trajectory.Y) / 2f;
						}
						else
						{
							base.trajectory.Y = 0f;
							this.stopped = true;
						}
					}
				}
			}
			else
			{
				base.trajectory.X = 0f;
			}
			this.animFrameTime += Game1.FrameTime * 24f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 63)
				{
					this.animFrame = 0;
				}
				this.animFrameTime = 0f;
			}
			this.ploc = base.location;
			base.location += base.trajectory * gameTime;
			if (new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight).Contains((int)vector.X, (int)vector.Y) && Rand.GetRandomInt(0, 5) == 0)
			{
				Vector2 loc2 = base.location + Rand.GetRandomVector2(-40f, 40f, -100f, 60f);
				int randomInt2 = Rand.GetRandomInt(12, 48);
				float randomFloat2 = Rand.GetRandomFloat(0.1f, 0.5f);
				pMan.AddSparkle(loc2, Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 0.75f, randomFloat2, randomInt2, 5);
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (this.frame < 0f)
			{
				Vector2 vector = (base.location + new Vector2(0f, (float)Math.Sin(Game1.map.MapSegFrameLocked * 16f + (float)base.owner) * 20f)) * Game1.worldScale - Game1.Scroll;
				if (Game1.pManager.renderingAdditive)
				{
					this.DrawCorona(sprite, particlesTex, vector, l);
				}
				else
				{
					sprite.Draw(particlesTex[2], vector, new Rectangle(this.animFrame * 64, 2200, 64, 128), Color.White, 0f, new Vector2(32f, 64f), 0.75f * worldScale, SpriteEffects.None, 1f);
					if (Game1.debugging)
						sprite.DrawString(Game1.font, Game1.navManager.RevealMap[Game1.navManager.NavPath].KeyList[base.owner].UniqueID + "key", vector - new Vector2(0, 200), Color.White);
				}
			}
		}
	}
}
