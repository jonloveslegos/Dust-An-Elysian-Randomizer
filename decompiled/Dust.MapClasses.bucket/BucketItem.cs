using Microsoft.Xna.Framework;

namespace Dust.MapClasses.bucket
{
	internal class BucketItem
	{
		public Vector2 Location;

		public int Definition;

		public BucketItem(Vector2 _loc, int _charDef)
		{
			this.Location = _loc;
			this.Definition = _charDef;
		}
	}
}
