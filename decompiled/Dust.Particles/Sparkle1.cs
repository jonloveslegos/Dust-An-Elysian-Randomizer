using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Sparkle1 : Particle
	{
		private int animFrame;

		private int animSpeed;

		private float animFrameTime;

		private SpriteEffects flip;

		private float r;

		private float g;

		private float b;

		private float a;

		private float size;

		public Sparkle1(Vector2 loc, float r, float g, float b, float a, float size, int _animSpeed)
		{
			this.Reset(loc, r, g, b, a, size, _animSpeed);
		}

		public void Reset(Vector2 loc, float r, float g, float b, float a, float size, int _animSpeed)
		{
			base.exists = Exists.Init;
			this.animFrame = Rand.GetRandomInt(0, 11);
			this.animFrameTime = 0f;
			base.location = loc;
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
			this.size = size * Rand.GetRandomFloat(0.8f, 1.2f);
			base.renderState = RenderState.AdditiveOnly;
			this.animSpeed = _animSpeed;
			if (Rand.GetRandomInt(0, 10) < 5)
			{
				this.flip = SpriteEffects.None;
			}
			else
			{
				this.flip = SpriteEffects.FlipHorizontally;
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			base.GameLocation(l);
			if (l == 9)
			{
				if (this.animFrame > 0 && (Game1.menu.prompt != promptDialogue.None || Game1.events.anyEvent || Game1.hud.isPaused || Game1.hud.inventoryState != 0))
				{
					base.Reset();
				}
				this.animFrameTime += Game1.HudTime * (float)this.animSpeed;
			}
			else
			{
				this.animFrameTime += gameTime * (float)this.animSpeed;
			}
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 46)
				{
					this.animFrame = 46;
					base.Reset();
				}
				this.animFrameTime = 0f;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(this.animFrame * 64, 2328, 64, 128), new Color(this.r, this.g, this.b, this.a), 0f, new Vector2(32f, 64f), new Vector2(this.size * 2f, this.size) * worldScale, this.flip, 1f);
		}
	}
}
