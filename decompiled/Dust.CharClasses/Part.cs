using Microsoft.Xna.Framework;

namespace Dust.CharClasses
{
	public class Part
	{
		public Vector2 Location;

		public float Rotation;

		public Vector2 Scaling;

		public int Index;

		public byte Flip;

		public Part()
		{
			this.Index = -1;
			this.Scaling = new Vector2(1f, 1f);
		}
	}
}
