using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class VerticleBeam : Particle
	{
		private float lifeSpan;

		private float frame;

		private float r;

		private float g;

		private float b;

		private float a;

		private int width;

		private int height;

		private SpriteEffects spriteDir;

		public VerticleBeam(Vector2 loc, Vector2 traj, float r, float g, float b, float a, int width, int height, float _lifeSpan, int owner)
		{
			this.Reset(loc, traj, r, g, b, a, width, height, _lifeSpan, owner);
		}

		public void Reset(Vector2 loc, Vector2 traj, float _r, float _g, float _b, float _a, int _width, int _height, float _lifeSpan, int _owner)
		{
			base.exists = Exists.Init;
			base.location = loc;
			base.trajectory = traj;
			this.r = _r;
			this.g = _g;
			this.b = _b;
			this.a = _a;
			this.width = _width;
			this.height = _height;
			base.owner = _owner;
			if (base.owner == 100)
			{
				base.location -= Game1.pManager.GetFidgetLoc(accomodateScroll: false);
			}
			else if (base.owner > -1 && Game1.character[base.owner].Exists == CharExists.Exists)
			{
				base.location -= Game1.character[base.owner].Location;
				if (Rand.GetRandomInt(0, 2) == 0)
				{
					base.background = true;
				}
			}
			this.spriteDir = ((Rand.GetRandomInt(0, 2) != 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			this.frame = (this.lifeSpan = _lifeSpan);
			if (this.r + this.g + this.b < 0.5f)
			{
				base.renderState = RenderState.Normal;
			}
			else
			{
				base.renderState = RenderState.AdditiveOnly;
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.frame < 0f)
			{
				base.Reset();
			}
			this.width += 2;
			base.location += base.trajectory * gameTime;
			this.frame -= gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(position: (base.owner == 100) ? (Game1.pManager.GetFidgetLoc(accomodateScroll: true) + base.location * Game1.worldScale) : ((base.owner <= -1 || Game1.character[base.owner].Exists != CharExists.Exists) ? base.GameLocation(l) : ((Game1.character[base.owner].Location + base.location) * Game1.worldScale - Game1.Scroll)), texture: particlesTex[2], sourceRectangle: new Rectangle(4077, 336, 19, 264), color: new Color(this.r, this.g, this.b, this.a * (float)Math.Sin(6.28 * (double)this.frame / 2.0 / (double)this.lifeSpan)), rotation: 0f, origin: new Vector2(10f, 132f), scale: new Vector2((float)this.width / 19f, (float)this.height / 264f) * worldScale, effects: this.spriteDir, layerDepth: 1f);
		}
	}
}
