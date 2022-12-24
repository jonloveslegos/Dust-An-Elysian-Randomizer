using Dust.Particles;
using Microsoft.Xna.Framework;

namespace Dust.MapClasses
{
	public class BombRegion
	{
		private int id;

		private int bombID;

		private Vector2 location;

		private int timer;

		public int ID => this.id;

		public int BombID => this.bombID;

		public Vector2 Location => this.location;

		public int Timer => this.timer;

		public BombRegion(Vector2 loc, int _ID, int _timer, bool _forceSpawn)
		{
			this.id = _ID;
			this.location = loc;
			if (_timer > 0)
			{
				this.timer = _timer;
			}
			else
			{
				this.timer = 60;
			}
			if (Game1.stats.gameDifficulty == 0)
			{
				this.timer += 4;
			}
			if (_forceSpawn)
			{
				this.bombID = Game1.pManager.AddBomb(Vector2.Zero, this.id, this.timer, 3, 5);
			}
		}

		public void AddBomb(ParticleManager pMan)
		{
			this.bombID = pMan.AddBomb(this.location, this.id, this.timer, 0, 5);
		}
	}
}
