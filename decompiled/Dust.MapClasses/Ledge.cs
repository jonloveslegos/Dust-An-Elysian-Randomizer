using Microsoft.Xna.Framework;

namespace Dust.MapClasses
{
	public class Ledge
	{
		public Vector2[] Nodes;

		public LedgeFlags Flag;

		public Ledge(int totalNodes, LedgeFlags flag)
		{
			this.Nodes = new Vector2[totalNodes];
			this.Flag = flag;
		}
	}
}
