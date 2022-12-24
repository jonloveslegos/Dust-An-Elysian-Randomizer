using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Hit : Particle
	{
		public Hit(Vector2 loc, Vector2 traj, int owner, int flag)
		{
			this.Reset(loc, traj, owner, flag);
		}

		public void Reset(Vector2 loc, Vector2 traj, int owner, int flag)
		{
			base.exists = Exists.Init;
			base.location = loc;
			base.trajectory = traj;
			base.owner = owner;
			base.flag = flag;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (base.owner == 0)
			{
				HitManager.CheckDestructionHit(this, c, pMan, Game1.dManager);
			}
			HitManager.CheckHit(this, c, pMan);
			base.Reset();
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
		}
	}
}
