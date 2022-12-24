using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class MenuDebris : Particle
	{
		private int debrisType;

		private Rectangle sRect;

		private float sizeY;

		private float r;

		private float g;

		private float b;

		private float a;

		private float size;

		private float rotation;

		private Vector2 ploc = Vector2.Zero;

		public MenuDebris(Vector2 loc, Vector2 traj, float r, float g, float b, float a, float size, int debrisType)
		{
			this.Reset(loc, traj, r, g, b, a, size, debrisType);
		}

		public void Reset(Vector2 loc, Vector2 traj, float _r, float _g, float _b, float _a, float _size, int _debrisType)
		{
			base.exists = Exists.Init;
			this.sizeY = 0f;
			base.location = (this.ploc = loc);
			base.trajectory = traj * Game1.hiDefScaleOffset;
			this.debrisType = _debrisType;
			this.a = _a;
			this.r = (this.g = (this.b = 1f));
			if (this.debrisType == 0)
			{
				base.trajectory.Y *= 2f;
				this.rotation = Rand.GetRandomFloat(0f, 6.28f);
				this.size = Rand.GetRandomFloat(0.1f, 1f) * _size * Game1.hiDefScaleOffset;
				this.sizeY = Rand.GetRandomFloat(0.2f, 1f) * this.size;
				base.renderState = RenderState.Normal;
				this.r = _r;
				this.g = _g;
				this.b = _b;
				this.sRect = new Rectangle(_debrisType * 196 + Rand.GetRandomInt(0, 3) * 64 + 1984, 2660, 64, 64);
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (!new Rectangle(-50, -400, Game1.screenWidth + 100, Game1.screenHeight + 500).Contains((int)base.location.X, (int)base.location.Y) || Game1.cManager.challengeMode == ChallengeManager.ChallengeMode.None)
			{
				base.Reset();
			}
			base.trajectory.Y += Game1.HudTime * 2000f;
			if (base.trajectory.X > 0f)
			{
				this.rotation += Game1.HudTime * 8f;
			}
			else
			{
				this.rotation -= Game1.HudTime * 8f;
			}
			this.ploc = base.location;
			base.location += base.trajectory * Game1.HudTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (this.debrisType == 0)
			{
				sprite.Draw(particlesTex[2], base.location, this.sRect, new Color(new Vector4(this.r, this.g, this.b, this.a)), this.rotation, new Vector2(32f, 32f), new Vector2(this.size, this.sizeY), SpriteEffects.None, 1f);
			}
		}
	}
}
