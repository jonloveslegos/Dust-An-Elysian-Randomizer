using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class CoraProjectile : Particle
	{
		private SpriteEffects spriteDir;

		private float frame;

		private Vector2 ploc = Vector2.Zero;

		public CoraProjectile(Vector2 loc, Vector2 traj, int l)
		{
			this.Reset(loc, traj, l);
		}

		public void Reset(Vector2 loc, Vector2 traj, int l)
		{
			base.exists = Exists.Init;
			base.strength = 0;
			base.flag = Rand.GetRandomInt(0, 4);
			this.frame = 4f;
			base.trajectory = traj;
			base.location = loc;
			base.owner = 0;
			base.renderState = RenderState.Additive;
			this.spriteDir = ((Rand.GetRandomInt(0, 2) != 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			base.isSpun = 10f;
			Sound.PlayCue("cora_throw", base.location, (Game1.character[0].Location - base.location).Length());
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			pMan.Seek(c, this, -1, 70f, 1000f);
			if (this.frame < 3.8f && HitManager.CheckHit(this, c, pMan))
			{
				base.Reset();
				pMan.AddShockRing(base.location, 0.2f, 6);
				pMan.AddLenseFlare(base.location, 0.4f, 1, 5);
				Sound.PlayCue("cora_impact", base.location, (c[0].Location - base.location).Length() * Game1.worldScale);
			}
			this.frame -= gameTime;
			if (this.frame < 0f)
			{
				base.Reset();
				pMan.AddFidgetPuff(base.location, 6);
			}
			base.location += base.trajectory * gameTime;
			this.ploc = base.location;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[4], base.location * worldScale - Game1.Scroll, new Rectangle(300, 1800 + base.flag * 60, 60, 60), Color.White, GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) + 1.57f, new Vector2(30f, 30f), 2f * worldScale, this.spriteDir, 1f);
			if (Game1.pManager.renderingAdditive && Rand.GetRandomInt(0, 10) == 0)
			{
				sprite.Draw(particlesTex[2], base.location * worldScale - Game1.Scroll, new Rectangle(2940, 1860, 497, 150), Color.White * this.frame * 4f, 0f, new Vector2(248f, 75f), 0.75f * worldScale, SpriteEffects.None, 1f);
			}
		}
	}
}
