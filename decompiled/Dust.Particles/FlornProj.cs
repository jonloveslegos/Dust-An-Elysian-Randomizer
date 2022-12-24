using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class FlornProj : Particle
	{
		private SpriteEffects[] spriteDir;

		private float[] rot;

		private float[] scale;

		private Vector2[] orig;

		private bool dying;

		private float trailTime;

		private float frame;

		private Rectangle renderRect;

		private Vector2 ploc = Vector2.Zero;

		public FlornProj(Vector2 loc, int owner)
		{
			this.Reset(loc, owner);
		}

		public void Reset(Vector2 loc, int owner)
		{
			base.exists = Exists.Init;
			this.spriteDir = new SpriteEffects[2];
			this.rot = new float[2];
			this.scale = new float[2];
			this.orig = new Vector2[2];
			this.dying = false;
			this.trailTime = Rand.GetRandomFloat(0f, 0.15f);
			base.maskGlow = 1f;
			base.statusType = StatusEffects.Silent;
			base.location = loc;
			base.trajectory = Game1.character[0].Location - Game1.character[owner].Location + new Vector2(0f, -120f);
			float angle = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory);
			base.trajectory = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * -600f;
			base.owner = owner;
			base.strength = Game1.character[owner].Strength;
			base.renderState = RenderState.Additive;
			this.frame = 3f;
			this.renderRect = new Rectangle(-1000, -1000, Game1.screenWidth + 2000, Game1.screenHeight + 1600);
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
			Vector2 origin = new Vector2(45f, 45f);
			Rectangle value = new Rectangle(3879, 328, 99, 99);
			Color color = new Color(1f, 1f, 1f, (1f - num) / 8f);
			for (int i = 0; i < 4; i++)
			{
				Vector2 position = vector - vector2 * ((float)i / 2f - 1.75f);
				float num2;
				float rotation;
				switch (i)
				{
				default:
					num2 = 1f;
					rotation = 0f;
					break;
				case 1:
					num2 = 1f;
					rotation = 0f;
					break;
				case 2:
					num2 = 0.25f;
					rotation = 0f;
					break;
				}
				sprite.Draw(particlesTex[2], position, value, color, rotation, origin, num2 * Game1.hiDefScaleOffset, SpriteEffects.None, 1f);
			}
			sprite.Draw(particlesTex[2], gameLoc, new Rectangle(2940, 1860, 497, 150), new Color(1f, 1f, 1f, 0.75f), 0f, new Vector2(248f, 75f), Game1.hiDefScaleOffset, SpriteEffects.None, 1f);
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.frame -= gameTime;
			if (!this.dying)
			{
				if (HitManager.CheckHit(this, c, pMan) || this.frame < 0f || map.CheckPCol(base.location, this.ploc, canFallThrough: false, init: false) > 0f)
				{
					Sound.PlayCue("florn_proj_contact", base.location, (c[0].Location - base.location).Length());
					this.dying = true;
					for (int i = 0; i < 2; i++)
					{
						base.trajectory = Rand.GetRandomVector2(-100f, 100f, -100f, 100f);
						this.scale[i] = 2f;
					}
					pMan.AddElectricBolt(base.location - new Vector2(0f, 100f), -1, Rand.GetRandomFloat(0.2f, 0.6f), 1000, 40, 0.4f, 5);
					this.frame = 0.5f;
				}
				if (base.trajectory.Y < 1500f)
				{
					base.trajectory.Y += Game1.FrameTime * 500f;
				}
				this.trailTime += Game1.FrameTime;
				if (this.trailTime > 0.15f)
				{
					this.trailTime = 0f;
					pMan.AddSparkle(base.location, Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 0.75f, Rand.GetRandomFloat(0.1f, 0.5f), Rand.GetRandomInt(40, 80), 5);
				}
				if (Game1.longSkipFrame > 2)
				{
					for (int j = 0; j < 2; j++)
					{
						if (Rand.GetRandomInt(0, 2) == 0)
						{
							this.spriteDir[j] = SpriteEffects.None;
						}
						else
						{
							this.spriteDir[j] = SpriteEffects.FlipHorizontally;
						}
						this.rot[j] = Rand.GetRandomFloat(0f, 6.28f);
						this.scale[j] = Rand.GetRandomFloat(0.2f, 0.8f);
						ref Vector2 reference = ref this.orig[j];
						reference = new Vector2(Rand.GetRandomInt(60, 100), Rand.GetRandomInt(30, 80));
					}
				}
				Vector2 vector = base.GameLocation(l);
				if (!this.renderRect.Contains((int)vector.X, (int)vector.Y))
				{
					base.Reset();
				}
			}
			else
			{
				base.maskGlow = this.frame * 4f;
				for (int k = 0; k < 2; k++)
				{
					if (Rand.GetRandomInt(0, 2) == 0)
					{
						this.spriteDir[k] = SpriteEffects.None;
					}
					else
					{
						this.spriteDir[k] = SpriteEffects.FlipHorizontally;
					}
					this.rot[k] = Rand.GetRandomFloat(0f, 6.28f);
					this.scale[k] = Rand.GetRandomFloat(1.6f, 2f);
					ref Vector2 reference2 = ref this.orig[k];
					reference2 = new Vector2(Rand.GetRandomInt(60, 100), Rand.GetRandomInt(30, 80));
				}
				if (this.frame < 0f)
				{
					base.Reset();
				}
			}
			base.location += base.trajectory * gameTime;
			this.ploc = base.location;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (!this.dying)
			{
				for (int i = 0; i < 2; i++)
				{
					sprite.Draw(particlesTex[2], vector, new Rectangle(3920, 1240, 176, 140), Color.White, this.rot[i], this.orig[i], new Vector2(this.scale[i] * 1.2f, this.scale[i] * 1f) * worldScale, this.spriteDir[i], 1f);
				}
				if (Game1.pManager.renderingAdditive)
				{
					this.DrawCorona(sprite, particlesTex, vector, l);
				}
			}
			else
			{
				for (int j = 0; j < 2; j++)
				{
					sprite.Draw(particlesTex[2], vector, new Rectangle(3920, 1240, 176, 140), new Color(1f, 1f, 1f, this.frame), this.rot[j], this.orig[j], new Vector2(this.scale[j] * 1.2f, this.scale[j] * 1f) * worldScale, this.spriteDir[j], 1f);
				}
			}
		}
	}
}
