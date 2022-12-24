using Dust.CharClasses;
using Dust.Particles;
using Microsoft.Xna.Framework;

namespace Dust.MapClasses.script
{
	public class WarpRegion
	{
		private int id;

		private Rectangle region;

		private Vector2 destLocation;

		private string destPath;

		private Vector2 origin;

		public int ID => this.id;

		public Rectangle Region => this.region;

		public Vector2 DestLocation => this.destLocation;

		public string DestPath => this.destPath;

		public WarpRegion(int _ID, Vector2 _origin, Vector2 _destLoc, string _destPath)
		{
			this.id = _ID;
			this.origin = _origin;
			this.region = new Rectangle((int)this.origin.X - 75, (int)this.origin.Y, 150, 175);
			this.destLocation = _destLoc;
			this.destPath = _destPath;
		}

		public bool Warp(Character[] c, Map map, ParticleManager pMan, bool warping)
		{
			if (this.region.Contains((int)c[0].Location.X, (int)c[0].Location.Y) && !warping && !Game1.events.anyEvent && Game1.stats.playerLifeState == 0)
			{
				c[0].Location = this.origin + new Vector2(0f, 120f);
				map.InitWarp(this.destLocation, this.destPath, c);
				return true;
			}
			if (new Rectangle(-50, -50, Game1.screenWidth + 100, Game1.screenHeight + 100).Contains((int)(this.origin.X * Game1.worldScale - Game1.Scroll.X), (int)(this.origin.Y * Game1.worldScale - Game1.Scroll.Y)))
			{
				pMan.AddDeathFlame(this.origin + Rand.GetRandomVector2(-100f, 100f, -100f, 100f), new Vector2(0f, -100f), 1f, 1f, 1f, 0.4f, 0, audio: false, 5);
				if (Rand.GetRandomInt(0, 40) == 0)
				{
					pMan.AddElectricBolt(this.origin + Rand.GetRandomVector2(-40f, 40f, -40f, 40f), -1, Rand.GetRandomFloat(0.2f, 0.6f), 600, 40, 0.4f, 5);
				}
			}
			return false;
		}
	}
}
