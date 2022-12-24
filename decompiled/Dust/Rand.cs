using System;
using Microsoft.Xna.Framework;

namespace Dust
{
	internal class Rand
	{
		public static Random random;

		private static object syncObject = new object();

		public static float GetRandomFloat(float fMin, float fMax)
		{
			lock (Rand.syncObject)
			{
				return (float)Rand.random.NextDouble() * (fMax - fMin) + fMin;
			}
		}

		public static double GetRandomDouble(double dMin, double dMax)
		{
			lock (Rand.syncObject)
			{
				return Rand.random.NextDouble() * (dMax - dMin) + dMin;
			}
		}

		public static Vector2 GetRandomVector2(float xMin, float xMax, float yMin, float yMax)
		{
			lock (Rand.syncObject)
			{
				return new Vector2((float)Rand.random.NextDouble() * (xMax - xMin) + xMin, (float)Rand.random.NextDouble() * (yMax - yMin) + yMin);
			}
		}

		public static int GetRandomInt(int iMin, int iMax)
		{
			lock (Rand.syncObject)
			{
				return Rand.random.Next(iMax - iMin) + iMin;
			}
		}
	}
}
