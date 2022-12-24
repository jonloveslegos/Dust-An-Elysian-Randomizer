using Microsoft.Xna.Framework;

namespace Dust.MapClasses
{
	public class IdRectRegion
	{
		private Rectangle region;

		private int id;

		public Rectangle Region => this.region;

		public int Id => this.id;

		public IdRectRegion(Rectangle _region, int _id)
		{
			this.region = _region;
			this.id = _id;
		}
	}
}
