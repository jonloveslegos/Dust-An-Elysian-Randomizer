using Microsoft.Xna.Framework;

namespace Dust.MapClasses
{
	public class ReverbRegion
	{
		private Rectangle region;

		private float reverbPercent;

		public Rectangle Region => this.region;

		public float ReverbPercent => this.reverbPercent;

		public ReverbRegion(Rectangle _region, float _reverbPercent)
		{
			this.region = _region;
			this.reverbPercent = _reverbPercent;
		}
	}
}
