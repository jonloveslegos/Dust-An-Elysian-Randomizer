using System;
using Microsoft.Xna.Framework;

namespace Dust
{
	internal class GlobalFunctions
	{
		public static float GetAngle(Vector2 v1, Vector2 v2)
		{
			Vector2 vector = new Vector2(v2.X - v1.X, v2.Y - v1.Y);
			if (vector.X == 0f)
			{
				if (vector.Y < 0f)
				{
					return (float)Math.PI / 2f;
				}
				if (vector.Y > 0f)
				{
					return 4.712389f;
				}
			}
			if (vector.Y == 0f)
			{
				if (vector.X < 0f)
				{
					return 0f;
				}
				if (vector.X > 0f)
				{
					return (float)Math.PI;
				}
			}
			float num = (float)Math.Atan(Math.Abs(vector.Y) / Math.Abs(vector.X));
			if (vector.X < 0f || vector.Y > 0f)
			{
				num = (float)Math.PI - num;
			}
			if (vector.X < 0f || vector.Y < 0f)
			{
				num = (float)Math.PI + num;
			}
			if (vector.X > 0f || vector.Y < 0f)
			{
				num = (float)Math.PI * 2f - num;
			}
			if (num < 0f)
			{
				num += (float)Math.PI * 2f;
			}
			return num;
		}

		public static Vector2 EaseIn(Vector2 location, Vector2 target, float maxSpeed)
		{
			Vector2 vector = target - location;
			float amount = Math.Min(1f / (vector * vector).Length() * maxSpeed, 1f);
			return Vector2.Lerp(location, target, amount);
		}

		public static Vector2 EaseInAndOut(Vector2 location, Vector2 target, float speed, float gameTime)
		{
			if (gameTime == 0f)
			{
				return location;
			}
			return (Vector2.Lerp(location, target, gameTime * speed) + GlobalFunctions.EaseIn(location, target, gameTime * speed * 0.01f)) / 2f;
		}

		public static float EaseIn(float source, float target, float maxSpeed)
		{
			float num = target - source;
			float num2 = num * num;
			float amount = Math.Min(1f / num2 * maxSpeed, 1f);
			return MathHelper.Lerp(source, target, amount);
		}

		public static float EaseInAndOut(float source, float target, float speed, float gameTime)
		{
			if (gameTime == 0f)
			{
				return source;
			}
			return (MathHelper.Lerp(source, target, gameTime * speed) + GlobalFunctions.EaseIn(source, target, gameTime * speed * 0.01f)) / 2f;
		}

		public static void EaseInEaseOut(ref Vector2 location, ref Vector2 origin, Vector2 target, float speed, float gameTime)
		{
		}

		public static float thing(float time, float start, float change, float duration)
		{
			return 0f;
		}
	}
}
