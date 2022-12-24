using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class FusePillar : Particle
	{
		private int stage;

		private int animFrame;

		private float animFrameTime;

		private float frame;

		private float alpha;

		private float size;

		private Vector2 orig;

		private Rectangle dRect;

		private Rectangle sRect;

		private Rectangle renderRect;

		private Vector2 ploc = Vector2.Zero;

		public FusePillar(Vector2 loc, int owner)
		{
			this.Reset(loc, owner);
		}

		public void Reset(Vector2 loc, int owner)
		{
			base.exists = Exists.Init;
			this.stage = 0;
			this.orig = new Vector2(50f, 160f);
			this.animFrame = Rand.GetRandomInt(0, 20);
			this.animFrameTime = 0f;
			base.location = loc;
			base.owner = owner;
			base.trajectory.X = Rand.GetRandomFloat(-100f, 100f);
			this.size = Rand.GetRandomFloat(1.2f, 1.6f);
			base.strength = (int)((float)Game1.character[owner].Strength * 2f);
			this.frame = 0.5f;
			base.renderState = RenderState.Refract;
			this.renderRect = new Rectangle(-2000, -2000, Game1.screenWidth + 4000, Game1.screenHeight + 4000);
			this.sRect = new Rectangle(3960, 1448, 136, 112);
			Sound.PlayCue("fidgetproj_02_pillar_beam");
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.frame -= gameTime;
			if (this.stage == 0)
			{
				Vector2 vector = base.GameLocation(l);
				this.dRect = new Rectangle((int)vector.X, (int)vector.Y, (int)(400f * this.frame), 8000);
				if (this.frame < 0f)
				{
					this.frame = 2f;
					this.stage = 1;
					this.alpha = 1f;
					this.sRect = new Rectangle(540, 1861, 100, 148);
					this.dRect.Width = 0;
					this.dRect.Height = 8000;
					Sound.PlayCue("fidgetproj_02_pillar");
				}
			}
			else if (this.stage == 1)
			{
				if (this.dRect.Width < 400 && this.frame > 0.5f)
				{
					this.dRect.Width += (int)(Game1.FrameTime * 3000f);
				}
				else
				{
					this.dRect.Width = (int)(400f * MathHelper.Clamp(this.frame * 10f, 0f, 1f));
				}
				this.dRect.X = (int)(base.location.X - Game1.Scroll.X);
				this.dRect.Y = (int)(base.location.Y - Game1.Scroll.Y);
				if (this.frame > 0.25f && HitManager.CheckHitPossible(this, c, 0, new Rectangle((int)((float)this.dRect.X + Game1.Scroll.X - (float)(this.dRect.Width / 2)), (int)((float)this.dRect.Y + Game1.Scroll.Y) - 4000, this.dRect.Width, this.dRect.Height)) && HitManager.CheckIDHit(this, c, pMan, 0))
				{
					c[0].CanHurtFrame = (c[0].CanHurtProjectileFrame = 1.75f);
					Sound.PlayCue("enemy_die_flame", base.location, (c[0].Location - base.location).Length());
					for (int i = 0; i < 10; i++)
					{
						pMan.AddExplosion(c[0].Location + Rand.GetRandomVector2(-100f, 100f, -300f, 0f), Rand.GetRandomFloat(0.5f, 1.5f), makeSmoke: false, 6);
					}
				}
				if (this.frame < 0f)
				{
					base.Reset();
				}
			}
			this.animFrameTime += gameTime * 60f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 23)
				{
					this.animFrame = 0;
				}
				this.animFrameTime = 0f;
			}
			base.location += base.trajectory * gameTime;
			this.ploc = base.location;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (this.stage == 0)
			{
				if (this.frame < 1f)
				{
					this.alpha = this.frame;
				}
				sprite.Draw(particlesTex[2], this.dRect, this.sRect, new Color(1f, 1f, 1f, this.alpha * 3f), 0f, new Vector2(68f, 61f), SpriteEffects.None, 1f);
			}
			else
			{
				if (this.stage != 1)
				{
					return;
				}
				if (base.location.Y > Game1.Scroll.Y - 100f)
				{
					base.location.Y -= 592f;
				}
				else if (base.location.Y < Game1.Scroll.Y - 592f)
				{
					base.location.Y += 592f;
				}
				this.sRect.X = 540 + 100 * this.animFrame;
				if (this.frame < 1f)
				{
					this.alpha = this.frame;
				}
				float num = (int)base.GameLocation(l).X;
				for (int i = 0; i < 4; i++)
				{
					if (Game1.refractive)
					{
						sprite.Draw(particlesTex[2], new Vector2(num, this.dRect.Y + i * 592), this.sRect, new Color(1f, 1f, 1f, this.alpha * 4f), 0f, new Vector2(50f, 0f), new Vector2((float)this.dRect.Width / 60f * worldScale, 4f), SpriteEffects.FlipHorizontally, 1f);
					}
					if (!Game1.refractive)
					{
						sprite.Draw(particlesTex[2], new Vector2(num + Rand.GetRandomFloat(-2f, 2f), this.dRect.Y + i * 592), this.sRect, new Color(0.9f, 0.9f, 0.9f, this.alpha * 8f), 0f, new Vector2(50f, 0f), new Vector2((float)this.dRect.Width / 100f * worldScale, 4f), SpriteEffects.None, 1f);
					}
				}
			}
		}
	}
}
