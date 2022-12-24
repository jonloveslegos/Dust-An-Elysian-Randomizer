using Microsoft.Xna.Framework;

namespace Dust.MapClasses
{
	public class DarkRegion
	{
		private Rectangle region;

		private float darkness;

		private float speed;

		public Rectangle Region => this.region;

		public float Speed => this.speed;

		public float Darkness => this.darkness;

		public DarkRegion(Rectangle _region, int _darkness, int _speed)
		{
			this.region = _region;
			this.darkness = (float)_darkness / 100f;
			if (_speed != 0)
			{
				this.speed = (float)_speed / 100f;
			}
			else
			{
				this.speed = 0.25f;
			}
		}
	}
}
