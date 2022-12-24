using System;
using Microsoft.Xna.Framework;

namespace Dust.Vibration
{
	internal class Blast
	{
		public float value;

		public Vector2 center;

		public void Update()
		{
			if (this.value > 0f)
			{
				this.value = Math.Max(this.value - Game1.FrameTime, 0f);
			}
		}
	}
}
