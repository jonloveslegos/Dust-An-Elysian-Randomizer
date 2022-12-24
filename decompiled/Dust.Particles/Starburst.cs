using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Starburst : Particle
	{
		private float lifeSpan;

		private float frame;

		private float size;

		private float rot;

		private Vector2 origOffset;

		private float r;

		private float g;

		private float b;

		private float a;

		private SpriteEffects spriteDir;

		public Starburst(Vector2 loc, float _size, float r, float g, float b, float a, float _lifeSpan, int owner, bool _background)
		{
			this.Reset(loc, _size, r, g, b, a, _lifeSpan, owner, _background);
		}

		public void Reset(Vector2 loc, float _size, float _r, float _g, float _b, float _a, float _lifeSpan, int _owner, bool _background)
		{
			base.exists = Exists.Init;
			base.location = loc;
			this.r = _r;
			this.g = _g;
			this.b = _b;
			this.a = _a;
			this.size = _size;
			base.owner = _owner;
			if (base.owner > -1 && Game1.character[base.owner].Exists == CharExists.Exists)
			{
				base.location -= Game1.character[base.owner].Location;
			}
			base.background = _background;
			this.spriteDir = ((Rand.GetRandomInt(0, 2) != 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			this.frame = (this.lifeSpan = _lifeSpan);
			base.renderState = RenderState.AdditiveOnly;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.frame < 0f)
			{
				base.Reset();
			}
			if (this.spriteDir == SpriteEffects.None)
			{
				this.rot += gameTime;
			}
			else
			{
				this.rot -= gameTime;
			}
			this.size += gameTime / 2f;
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Character[] character = Game1.character;
			Vector2 position;
			if (base.owner > -1 && character[base.owner].Exists == CharExists.Exists)
			{
				float num = (Game1.character[base.owner].CharRotation - 3.14f) / 8f;
				float y = 0f;
				if (character[base.owner].FlyType > FlyingType.None && character[base.owner].State == CharState.Air)
				{
					y = 65f;
				}
				Vector2 vector = base.location - new Vector2(0f, y);
				Vector2 vector2 = new Vector2((float)(Math.Cos(num) * (double)vector.X - Math.Sin(num) * (double)vector.Y), (float)(Math.Cos(num) * (double)vector.Y + Math.Sin(num) * (double)vector.X));
				position = (character[base.owner].Location + vector2) * Game1.worldScale - Game1.Scroll;
			}
			else
			{
				position = base.GameLocation(l);
			}
			sprite.Draw(particlesTex[2], position, new Rectangle(3750, 2970, 346, 346), new Color(this.r, this.g, this.b, this.a * (float)Math.Sin(6.28 * (double)this.frame / 2.0 / (double)this.lifeSpan)), this.rot, new Vector2(173f), this.size * worldScale, this.spriteDir, 1f);
		}
	}
}
