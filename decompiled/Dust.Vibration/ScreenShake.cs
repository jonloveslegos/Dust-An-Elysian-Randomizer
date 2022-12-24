using System;
using Microsoft.Xna.Framework;

namespace Dust.Vibration
{
	internal class ScreenShake
	{
		public float value;

		public Vector2 Vector
		{
			get
			{
				if (this.value <= 0f)
				{
					return Vector2.Zero;
				}
				return Rand.GetRandomVector2(0f - this.value, this.value, 0f - this.value, this.value) * 10f;
			}
		}

		public void Update()
		{
			if (this.value > 0f)
			{
				this.value = Math.Max(this.value - Game1.FrameTime, 0f);
			}
		}
	}
}
