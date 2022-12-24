using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class SpawnBeam : Particle
	{
		private float frame;

		private Rectangle sRect;

		private Rectangle dRect;

		private float yContact;

		private float width;

		public SpawnBeam(Vector2 loc, float width)
		{
			this.Reset(loc, width);
		}

		public void Reset(Vector2 loc, float _width)
		{
			base.exists = Exists.Init;
			base.location = loc;
			base.renderState = RenderState.Refract;
			this.width = _width;
			this.frame = 0.5f;
			this.sRect = new Rectangle(540, 1660, 100, 199);
			Sound.PlayCue("summoner_beam", base.location, (Game1.character[0].Location - base.location).Length() / 2f);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.frame -= gameTime;
			if (this.frame < 0f)
			{
				base.Reset();
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 position = base.GameLocation(l);
			sprite.Draw(particlesTex[2], new Rectangle((int)position.X, 0, (int)(this.width * this.frame), (int)position.Y), new Rectangle(3960, 1448, 136, 112), new Color(1f, 1f, 1f, this.frame * 2f), 0f, new Vector2(68f, 0f), SpriteEffects.None, 1f);
			sprite.Draw(particlesTex[2], position, new Rectangle(4000, 2796, 96, 96), new Color(1f, 1f, 1f, this.frame * 3f), 0f, new Vector2(48f, 48f), this.width / 50f * this.frame, SpriteEffects.None, 1f);
		}
	}
}
