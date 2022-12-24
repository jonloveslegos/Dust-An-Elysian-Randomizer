using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.HUD
{
	public class ScoreDraw
	{
		public enum Justify
		{
			Left,
			Right,
			Center,
			Middle
		}

		private SpriteBatch spriteBatch;

		private Texture2D numbersTex;

		private static int place;

		private static long origScore;

		private long realScore;

		private static int width;

		private static int height;

		private static int yOffset;

		private static Vector2 origin;

		public ScoreDraw(SpriteBatch _spriteBatch, Texture2D _numbersTex)
		{
			this.spriteBatch = _spriteBatch;
			this.numbersTex = _numbersTex;
		}

		public void Draw(long score, Vector2 loc, Vector2 scale, Color color, Justify justify, int type)
		{
			ScoreDraw.place = 0;
			ScoreDraw.origScore = score;
			this.realScore = Math.Abs(score);
			ScoreDraw.origin = Vector2.Zero;
			switch (type)
			{
			case 0:
				ScoreDraw.width = 20;
				ScoreDraw.height = 27;
				ScoreDraw.yOffset = 0;
				break;
			case 1:
				ScoreDraw.width = 22;
				ScoreDraw.height = 32;
				ScoreDraw.yOffset = 28;
				break;
			default:
				ScoreDraw.width = 44;
				ScoreDraw.height = 64;
				ScoreDraw.yOffset = 64;
				if (type == 3)
				{
					ScoreDraw.yOffset = 128;
				}
				break;
			}
			switch (justify)
			{
			case Justify.Left:
			{
				loc.X -= (float)ScoreDraw.width * scale.X;
				long num2 = Math.Abs(score);
				if (num2 == 0)
				{
					loc.X += (float)ScoreDraw.width * scale.X;
					break;
				}
				while (num2 > 0 || num2 < 0)
				{
					num2 /= 10;
					loc.X += (float)ScoreDraw.width * scale.X;
				}
				break;
			}
			case Justify.Center:
			case Justify.Middle:
			{
				loc.X -= (float)ScoreDraw.width * scale.X;
				long num = Math.Abs(score);
				if (num < 10)
				{
					ScoreDraw.origin.X = ScoreDraw.width / 2;
				}
				else if (num < 100)
				{
					ScoreDraw.origin.X = ScoreDraw.width;
				}
				else if (num < 1000)
				{
					ScoreDraw.origin.X = ScoreDraw.width * 3 / 2;
				}
				else if (num < 10000)
				{
					ScoreDraw.origin.X = ScoreDraw.width * 2;
				}
				else if (num < 100000)
				{
					ScoreDraw.origin.X = ScoreDraw.width * 5 / 2;
				}
				else if (num < 1000000)
				{
					ScoreDraw.origin.X = ScoreDraw.width * 3;
				}
				else if (num < 10000000)
				{
					ScoreDraw.origin.X = ScoreDraw.width * 7 / 2;
				}
				if (justify == Justify.Middle)
				{
					ScoreDraw.origin.Y = ScoreDraw.height / 2;
				}
				if (num == 0)
				{
					loc.X += (float)ScoreDraw.width * scale.X;
					break;
				}
				while (num > 0 || num < 0)
				{
					num /= 10;
					loc.X += (float)ScoreDraw.width * scale.X;
				}
				break;
			}
			}
			do
			{
				long num3 = Math.Abs(score) % 10;
				score = Math.Abs(score) / 10;
				this.spriteBatch.Draw(this.numbersTex, loc + new Vector2((float)ScoreDraw.place * (float)(-ScoreDraw.width) * scale.X, 0f), new Rectangle((int)num3 * ScoreDraw.width, ScoreDraw.yOffset, ScoreDraw.width, ScoreDraw.height), color, 0f, ScoreDraw.origin, scale, SpriteEffects.None, 1f);
				ScoreDraw.place++;
			}
			while (score > 0);
		}

		public void Draw(long score, Vector2 loc, float scale, Color color, Justify justify, int type)
		{
			ScoreDraw.place = 0;
			ScoreDraw.origScore = score;
			this.realScore = Math.Abs(score);
			ScoreDraw.origin = Vector2.Zero;
			switch (type)
			{
			case 0:
				ScoreDraw.width = 20;
				ScoreDraw.height = 27;
				ScoreDraw.yOffset = 0;
				break;
			case 1:
				ScoreDraw.width = 22;
				ScoreDraw.height = 32;
				ScoreDraw.yOffset = 28;
				break;
			default:
				ScoreDraw.width = 44;
				ScoreDraw.height = 64;
				ScoreDraw.yOffset = 64;
				if (type == 3)
				{
					ScoreDraw.yOffset = 128;
				}
				break;
			}
			switch (justify)
			{
			case Justify.Left:
			{
				loc.X -= (float)ScoreDraw.width * scale;
				long num2 = Math.Abs(score);
				if (num2 == 0)
				{
					loc.X += (float)ScoreDraw.width * scale;
					break;
				}
				while (num2 > 0 || num2 < 0)
				{
					num2 /= 10;
					loc.X += (float)ScoreDraw.width * scale;
				}
				break;
			}
			case Justify.Center:
			case Justify.Middle:
			{
				loc.X -= (float)ScoreDraw.width * scale;
				long num = Math.Abs(score);
				if (num < 10)
				{
					ScoreDraw.origin.X = ScoreDraw.width / 2;
				}
				else if (num < 100)
				{
					ScoreDraw.origin.X = ScoreDraw.width;
				}
				else if (num < 1000)
				{
					ScoreDraw.origin.X = ScoreDraw.width * 3 / 2;
				}
				else if (num < 10000)
				{
					ScoreDraw.origin.X = ScoreDraw.width * 2;
				}
				else if (num < 100000)
				{
					ScoreDraw.origin.X = ScoreDraw.width * 5 / 2;
				}
				else if (num < 1000000)
				{
					ScoreDraw.origin.X = ScoreDraw.width * 3;
				}
				else if (num < 10000000)
				{
					ScoreDraw.origin.X = ScoreDraw.width * 7 / 2;
				}
				if (justify == Justify.Middle)
				{
					ScoreDraw.origin.Y = ScoreDraw.height / 2;
				}
				if (num == 0)
				{
					loc.X += (float)ScoreDraw.width * scale;
					break;
				}
				while (num > 0 || num < 0)
				{
					num /= 10;
					loc.X += (float)ScoreDraw.width * scale;
				}
				break;
			}
			}
			do
			{
				long num3 = Math.Abs(score) % 10;
				score = Math.Abs(score) / 10;
				this.spriteBatch.Draw(this.numbersTex, loc + new Vector2((float)ScoreDraw.place * (float)(-ScoreDraw.width) * scale, 0f), new Rectangle((int)num3 * ScoreDraw.width, ScoreDraw.yOffset, ScoreDraw.width, ScoreDraw.height), color, 0f, ScoreDraw.origin, scale, SpriteEffects.None, 1f);
				ScoreDraw.place++;
			}
			while (score > 0);
		}
	}
}
