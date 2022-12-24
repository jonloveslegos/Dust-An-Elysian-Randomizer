using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class HudSpark : Particle
	{
		private float lifeSpan;

		private float frame;

		private float size;

		public HudSpark(Vector2 loc, Vector2 traj, float size)
		{
			this.Reset(loc, traj, size);
		}

		public void Reset(Vector2 loc, Vector2 traj, float size)
		{
			base.exists = Exists.Init;
			base.location = loc;
			base.renderState = RenderState.AdditiveOnly;
			this.size = size * Rand.GetRandomFloat(0.8f, 1.1f);
			this.lifeSpan = Rand.GetRandomFloat(1f, 3f) * size;
			this.frame = this.lifeSpan;
			base.trajectory = (traj *= Rand.GetRandomFloat(1f, 2f));
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			base.trajectory.Y += Game1.HudTime * 1000f;
			base.location += base.trajectory * Game1.HudTime;
			this.frame -= Game1.HudTime;
			if (!new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight).Contains((int)base.location.X, (int)base.location.Y) || this.frame < 0f || (Game1.gameMode == Game1.GameModes.Game && Game1.hud.inventoryState == InventoryState.None))
			{
				base.Reset();
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			float rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) + 1.57f;
			sprite.Draw(particlesTex[1], base.location, new Rectangle(1760, 128, 32, 32), new Color(1f, 1f, 1f, this.frame / (this.lifeSpan * 0.2f)), rotation, new Vector2(1f, 16f), new Vector2(this.size, this.size * 2f), SpriteEffects.None, 1f);
		}
	}
}
