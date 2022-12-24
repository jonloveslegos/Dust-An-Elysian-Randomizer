using System;
using Dust.Audio;
using Dust.CharClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.MapClasses
{
	public class MovingPlatform
	{
		private byte pathID;

		private byte loopType;

		public Vector2 prevLoc;

		public Vector2 pathLoc;

		private double pathTime;

		private double speed;

		private float alpha;

		private float clothRot;

		private bool reverse;

		private ColType collisionType;

		public byte segments;

		private Vector2[] pathNodes;

		private float pathOrigin;

		private SpriteEffects spriteDir;

		public MovingPlatform(Vector2 loc, int width, int _pathPercent, int _speed, int _loopType)
		{
			this.pathTime = (this.pathOrigin = (float)_pathPercent / 100f);
			this.segments = (byte)(width / 240);
			this.speed = (float)_speed / 100f;
			this.loopType = (byte)_loopType;
			this.spriteDir = ((Rand.GetRandomInt(0, 2) != 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			this.BuildPath(loc + new Vector2(width / 2, 30f), Game1.map);
			this.clothRot = 0f;
			float transInFrame = Game1.map.transInFrame;
			Game1.map.transInFrame = 1f;
			this.Update(0f, Game1.map);
			Game1.map.transInFrame = transInFrame;
			if (Game1.map.transInFrame >= 1f)
			{
				this.alpha = 1f;
			}
			Vector2 vector = this.pathLoc + new Vector2(120f, 30f);
			Sound.PlayCue("platform_moving_grow", vector, (vector - Game1.character[0].Location).Length());
		}

		public void ResetPath()
		{
			this.pathTime = this.pathOrigin;
			this.alpha = 0f;
		}

		public Vector2 GetLoc()
		{
			return -new Vector2(this.segments * 240 / 2, 30f) + this.pathLoc;
		}

		private double GetPathTime()
		{
			double num = this.pathTime;
			if (num > 1.0)
			{
				num -= 1.0;
			}
			if (this.reverse)
			{
				return 1.0 - num;
			}
			return num;
		}

		private void BuildPath(Vector2 loc, Map map)
		{
			float num = 1E+09f;
			byte b = 0;
			for (int i = 0; i < map.ledges.Length; i++)
			{
				if (map.ledges[i] != null && map.ledges[i].Flag == LedgeFlags.PlatformPath)
				{
					float num2 = (loc - map.ledges[i].Nodes[0]).Length();
					if (num2 < num)
					{
						num = num2;
						b = (byte)i;
					}
				}
			}
			this.pathID = b;
			bool flag = this.loopType == 0;
			this.pathNodes = new Vector2[Game1.map.ledges[this.pathID].Nodes.Length + (flag ? 1 : 0)];
			for (int j = 0; j < this.pathNodes.Length - (flag ? 1 : 0); j++)
			{
				ref Vector2 reference = ref this.pathNodes[j];
				reference = Game1.map.ledges[this.pathID].Nodes[j];
			}
			if (flag)
			{
				ref Vector2 reference2 = ref this.pathNodes[this.pathNodes.Length - 1];
				reference2 = this.pathNodes[0];
			}
		}

		public void Update(float gameTime, Map map)
		{
			bool flag = this.speed == 1.0;
			if (map.GetTransVal() < 1f)
			{
				switch (this.loopType)
				{
				case 0:
					if (flag)
					{
						this.pathTime = map.hazardRot / Math.PI / 2.0 + (double)this.pathOrigin;
					}
					else
					{
						this.pathTime += (double)gameTime * this.speed;
					}
					if (this.pathTime > 1.0)
					{
						this.pathTime -= 1.0;
					}
					break;
				case 1:
					if (flag)
					{
						double num3 = this.pathTime;
						this.pathTime = map.hazardRot / Math.PI / 2.0 + (double)this.pathOrigin;
						if (this.pathTime >= 1.0 && num3 < 1.0)
						{
							this.reverse = !this.reverse;
						}
					}
					else
					{
						this.pathTime += (double)gameTime * this.speed * 2.0;
						if (this.pathTime > 1.0)
						{
							this.pathTime -= 1.0;
							this.reverse = !this.reverse;
						}
					}
					break;
				case 2:
				{
					bool flag2 = false;
					if (flag)
					{
						double num = this.pathTime;
						this.pathTime = map.hazardRot / Math.PI / 2.0 + (double)this.pathOrigin;
						if (this.pathTime >= 1.0 && num < 1.0)
						{
							flag2 = true;
						}
					}
					else
					{
						this.pathTime += (double)gameTime * this.speed;
						if (this.pathTime > 1.0)
						{
							this.pathTime -= 1.0;
							flag2 = true;
						}
					}
					if (!flag2)
					{
						break;
					}
					Vector2 vector = this.pathLoc + new Vector2(120f, 30f);
					Sound.PlayCue("platform_moving_die", vector, (vector - Game1.character[0].Location).Length());
					int num2 = this.segments * 240;
					Vector2 vector2 = (this.pathLoc + new Vector2(120f, 30f)) * Game1.worldScale - Game1.Scroll;
					if (new Rectangle(-num2, -200, Game1.screenWidth + num2 * 2, Game1.screenHeight + 200).Contains((int)vector2.X, (int)vector2.Y))
					{
						for (int i = 0; i < this.segments; i++)
						{
							Game1.pManager.AddShockRing(this.pathLoc + new Vector2(120 - num2 / 2 + i * 240, -20f), 0.2f, 5);
						}
					}
					this.alpha = 0f;
					this.clothRot = 0f;
					this.prevLoc = (this.pathLoc = this.pathNodes[0]);
					vector2 = (this.pathLoc + new Vector2(120f, 30f)) * Game1.worldScale - Game1.Scroll;
					if (new Rectangle(-num2, -200, Game1.screenWidth + num2 * 2, Game1.screenHeight + 200).Contains((int)vector2.X, (int)vector2.Y))
					{
						for (int j = 0; j < this.segments; j++)
						{
							Game1.pManager.AddShockRing(this.pathLoc + new Vector2(120 - num2 / 2 + j * 240, -20f), 0.2f, 5);
						}
					}
					vector = this.pathLoc + new Vector2(120f, 30f);
					Sound.PlayCue("platform_moving_grow", vector, (vector - Game1.character[0].Location).Length());
					break;
				}
				}
			}
			this.prevLoc = this.GetLoc();
			double num4 = 0.0;
			for (int k = 1; k < this.pathNodes.Length; k++)
			{
				num4 += (double)(this.pathNodes[k] - this.pathNodes[k - 1]).Length();
			}
			double num5 = num4;
			num4 *= this.GetPathTime();
			int num6 = 0;
			for (int l = 0; l < this.pathNodes.Length - 1; l++)
			{
				float num7 = (this.pathNodes[num6] - this.pathNodes[num6 + 1]).Length();
				if (num4 > (double)num7)
				{
					num4 -= (double)num7;
					num6++;
				}
			}
			if (num6 < this.pathNodes.Length - 1)
			{
				float num8 = (float)(num4 / (double)(this.pathNodes[num6] - this.pathNodes[num6 + 1]).Length());
				Vector2 vector3 = this.pathNodes[num6] * (1f - num8) + this.pathNodes[num6 + 1] * num8;
				if (map.transInFrame >= 1f)
				{
					this.pathLoc = vector3;
				}
				else
				{
					this.pathLoc += (vector3 - this.pathLoc) * gameTime * 10f * (float)this.speed;
				}
			}
			if (this.loopType == 2 && num5 * (1.0 - this.GetPathTime()) < Math.Min(2000.0 * this.speed, 100.0))
			{
				this.alpha = Math.Max(this.alpha - gameTime * 4f, 0f);
			}
			else if (this.alpha < 1f)
			{
				this.alpha = Math.Min(this.alpha + gameTime * 4f, 1f);
			}
			float num9 = MathHelper.Clamp((this.GetLoc().X - this.prevLoc.X) / 8f, -1f, 1f);
			this.clothRot += (num9 - this.clothRot) * gameTime * 4f;
			if (Game1.longSkipFrame <= 3)
			{
				return;
			}
			Vector2 vector4 = this.pathLoc * Game1.worldScale - Game1.Scroll;
			if (!(vector4.X > -400f) || !(vector4.X < (float)(Game1.screenWidth + 800)) || !(vector4.Y > -200f) || !(vector4.Y < (float)(Game1.screenHeight + 400)))
			{
				return;
			}
			int num10 = this.segments * 240;
			bool flag3 = false;
			ColType colType = this.collisionType;
			this.collisionType = (ColType)map.CheckCol(this.pathLoc + new Vector2(0f, -20f));
			if (colType != this.collisionType && colType != ColType.Lava)
			{
				_ = this.collisionType;
				_ = 4;
			}
			ColType colType2 = this.collisionType;
			if (colType2 == ColType.Lava)
			{
				float num11 = (this.GetLoc().X - this.prevLoc.X) * 100f;
				for (int m = 0; m < this.segments; m++)
				{
					Game1.pManager.AddEmitLava(this.pathLoc + new Vector2((float)(120 - num10 / 2 + m * 240) + Rand.GetRandomFloat(-100f, 100f), -20f), Vector2.Zero, 4f, colliding: false, 3, 6);
					for (int n = 0; n < 4; n++)
					{
						Game1.pManager.AddEmitLava(this.pathLoc + new Vector2(120 - num10 / 2 + m * 240, -20f) + Rand.GetRandomVector2(-120f, 120f, -50f, 50f), Rand.GetRandomVector2(-50f + num11, 50f + num11, -600f, -400f), 1f, colliding: true, 1, 6);
					}
				}
			}
			else
			{
				flag3 = true;
			}
			if (flag3)
			{
				for (int num12 = 0; num12 < this.segments; num12++)
				{
					Game1.pManager.AddGlowSpark(this.pathLoc + new Vector2(120 - num10 / 2 + num12 * 240, -20f) + Rand.GetRandomVector2(-120f, 120f, -50f, 50f), new Vector2(0f, Rand.GetRandomFloat(4f, 40f)), 1f, 1f, 1.5f, 5);
				}
			}
		}

		public void UpdateCollision(float gameTime, Map map, DestructableManager dMan, Character c)
		{
			if (c.State != 0)
			{
				return;
			}
			Vector2 loc = this.GetLoc();
			if (c.Location.Y != this.prevLoc.Y)
			{
				if (dMan.CheckPlatformBounds(c.Location, loc, this.segments * 240) && (loc.Y < this.prevLoc.Y || c.ledgeAttach == -1))
				{
					c.Location.Y = loc.Y;
					c.ledgeAttach = -1;
					c.CanFallThrough = true;
				}
				return;
			}
			if (c.Location.X > this.prevLoc.X && c.Location.X < this.prevLoc.X + (float)(this.segments * 240))
			{
				Vector2 trajectory = c.Trajectory;
				Vector2 vector = loc - this.prevLoc;
				c.Trajectory += vector;
				c.Location += vector;
				if (map.CheckCol(c.Location + new Vector2(0f, -32f)) > 0 || map.CheckCol(c.Location - new Vector2(0f, c.Height - 40)) > 0)
				{
					c.Location.X = c.PLoc.X;
				}
				if (c.Trajectory.Y >= 0f)
				{
					c.MapCollision(Game1.map, Game1.character);
				}
				c.Location.Y = loc.Y;
				c.Trajectory = trajectory;
			}
			if (loc.Y >= this.prevLoc.Y)
			{
				if (map.CheckCol(c.Location) > 0)
				{
					c.Location.Y = (int)(c.Location.Y / 64f) * 64;
					c.CanFallThrough = false;
				}
				return;
			}
			int num = map.CheckCol(c.Location - new Vector2(0f, c.DefaultHeight));
			if (num > 0 || Game1.dManager.CheckColUpper(c.ID, c.DefaultHeight - 32))
			{
				c.FallOff(Game1.character, fallThrough: true);
				if (num > 2)
				{
					HitManager.CheckWallHazard(Game1.character, c.ID, Game1.pManager, 100, (ColType)num);
					c.Trajectory = new Vector2(0f, 100f);
				}
			}
		}

		public void Draw(SpriteBatch sprite, Texture2D[] particleTex)
		{
			Vector2 vector = (this.GetLoc() + new Vector2(120f, 0f)) * Game1.worldScale - Game1.Scroll;
			float y = 60f - 60f * (1f - this.alpha);
			for (int i = 0; i < this.segments; i++)
			{
				Rectangle value;
				int num;
				SpriteEffects spriteEffects;
				if (this.segments > 1 && (i == 0 || i == this.segments - 1))
				{
					value = new Rectangle(800, 3066, 258, 208);
					if (i == 0)
					{
						num = 136;
						spriteEffects = SpriteEffects.None;
					}
					else
					{
						num = 120;
						spriteEffects = SpriteEffects.FlipHorizontally;
					}
					float rotation = this.clothRot + (float)Math.Cos(Game1.map.MapSegFrameLocked * 40f + (float)i + 1f) * 0.2f;
					sprite.Draw(particleTex[2], vector + new Vector2(240 * i, y) * Game1.worldScale, new Rectangle(740, 3380, 28, 99), Color.White * this.alpha, rotation, new Vector2(16f, 24f), new Vector2(1f, this.alpha) * Game1.worldScale, SpriteEffects.None, 0f);
					sprite.Draw(particleTex[2], vector + new Vector2(240 * i + 60 * ((spriteEffects != 0) ? 1 : (-1)), 20f) * Game1.worldScale, new Rectangle(715, 3380, 25, 86), Color.White * this.alpha, rotation, new Vector2(12f, 24f), new Vector2(1f, this.alpha) * Game1.worldScale, SpriteEffects.None, 0f);
				}
				else
				{
					num = 122;
					value = new Rectangle(1058, 3066, 245, 250);
					spriteEffects = SpriteEffects.None;
				}
				sprite.Draw(particleTex[2], vector + new Vector2((float)(240 * i) * Game1.worldScale, 0f), value, Color.White * this.alpha, 0f, new Vector2(num, 76f), new Vector2(1f, this.alpha) * Game1.worldScale, spriteEffects, 0f);
			}
			if (this.segments > 1)
			{
				for (int j = 0; j < this.segments - 1; j++)
				{
					float rotation = this.clothRot + (float)Math.Cos(Game1.map.MapSegFrameLocked * 40f + (float)j) * 0.2f;
					sprite.Draw(particleTex[2], vector + new Vector2(120 + 240 * j, y) * Game1.worldScale, new Rectangle(674, 3380, 41, 130), Color.White * this.alpha, rotation, new Vector2(18f, 24f), new Vector2(1f, this.alpha) * Game1.worldScale, SpriteEffects.None, 0f);
				}
			}
			else
			{
				for (int k = 0; k < 2; k++)
				{
					float rotation = this.clothRot + (float)Math.Cos(Game1.map.MapSegFrameLocked * 40f + (float)k) * 0.2f;
					sprite.Draw(particleTex[2], vector + new Vector2(-120 + 240 * k, y) * Game1.worldScale, new Rectangle(674, 3380, 41, 130), Color.White * this.alpha, rotation, new Vector2(18f, 24f), new Vector2(1f, this.alpha) * Game1.worldScale, SpriteEffects.None, 0f);
				}
			}
		}
	}
}
