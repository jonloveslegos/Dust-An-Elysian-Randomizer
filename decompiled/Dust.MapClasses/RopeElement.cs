using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.MapClasses
{
	public class RopeElement
	{
		private int ropeID;

		private byte ropeType;

		private byte startSeg;

		private byte layer;

		private Vector2[] segLoc;

		private float[] segRot;

		private float speed;

		private float windStrength;

		private float undulateTime;

		private float undulateAmount;

		private float undulateTarget;

		private int boosting = -1;

		private float ripple;

		private float alpha;

		private float sourceRot;

		private float sourceScale;

		private int maxSegHeight;

		private Vector2 sourceLoc;

		private Vector2 orig;

		public RopeElement(int _ropeID, int _layer, byte _ropeType, MapSegment mapSeg, int segments, Rectangle sRect, float _speed, Map map)
		{
			this.ropeID = _ropeID;
			this.speed = _speed * mapSeg.Scale.Y / mapSeg.Scale.X;
			this.sourceLoc = mapSeg.Location;
			this.sourceRot = mapSeg.Rotation;
			this.maxSegHeight = sRect.Height / segments * 2;
			this.ropeType = _ropeType;
			if (this.ropeType == 0)
			{
				this.startSeg = 0;
			}
			else
			{
				this.startSeg = (byte)Math.Max((float)segments * 0.2f, 1f);
			}
			this.orig = new Vector2(sRect.Width / 2, 0f);
			this.segLoc = new Vector2[segments];
			this.segRot = new float[segments];
			for (int i = 0; i < segments; i++)
			{
				ref Vector2 reference = ref this.segLoc[i];
				reference = new Vector2(0f, i * 10);
				this.segRot[i] = this.sourceRot;
			}
			float num = 1f;
			this.layer = (byte)_layer;
			switch (this.layer)
			{
			case 0:
				num = 0.2f;
				break;
			case 1:
				num = 0.3f;
				break;
			case 2:
				num = 0.5f;
				break;
			case 3:
				num = 0.85f;
				break;
			case 4:
				num = ((map.backScroll != 1) ? 0.9f : 0.85f);
				break;
			case 7:
				num = 1.1f;
				break;
			case 8:
				num = 1.25f;
				break;
			}
			this.sourceScale = num;
			sRect = new Rectangle((int)((mapSeg.Location.X - (float)sRect.Width * mapSeg.Scale.X / 2f) / num), (int)((mapSeg.Location.Y - (float)sRect.Height * mapSeg.Scale.Y / 2f) / num), (int)((float)sRect.Width * mapSeg.Scale.X * num), (int)((float)sRect.Height * mapSeg.Scale.Y * num));
			for (int j = 0; j < map.boostRegions.Count; j++)
			{
				if (map.boostRegions[j] != null && map.boostRegions[j].Region.Intersects(sRect))
				{
					this.boosting = map.boostRegions[j].Direction;
					this.speed = 1f;
				}
			}
		}

		public void Update(float frameTime)
		{
			this.undulateTime -= frameTime;
			if (this.undulateTime < 0f)
			{
				this.undulateTime = Rand.GetRandomFloat(1f, 4f) * 2f;
				this.undulateTarget = Rand.GetRandomFloat(0.1f, 2f);
			}
			this.windStrength = (this.ripple = MathHelper.Clamp(Game1.wManager.windStrength / 100f, -1f, 1f));
			this.ripple = 0f;
			if (this.boosting > -1)
			{
				this.ripple = this.undulateAmount * 2f;
				switch (this.boosting)
				{
				case 0:
					this.windStrength = 3.14f;
					break;
				case 3:
					this.windStrength = 1f;
					break;
				case 9:
					this.windStrength = -1f;
					break;
				}
				this.windStrength += (float)Math.Cos(Game1.map.MapSegFrameLocked * 60f + (float)this.ropeID) * 2f * this.undulateAmount;
			}
			else if (Game1.stats.isSpinning)
			{
				this.ripple = 1f;
				this.windStrength = (float)Math.Cos(Game1.map.MapSegFrameLocked * 50f + (float)this.ropeID) * this.undulateAmount;
				this.windStrength += MathHelper.Clamp((Game1.character[0].Location - this.sourceLoc / this.sourceScale).X / 1000f, -2f, 2f);
			}
			else if (Game1.wManager.windStrength == 0f)
			{
				if (this.windStrength > this.sourceRot)
				{
					this.windStrength = Math.Max(this.windStrength - frameTime * 2f, this.sourceRot);
				}
				else if (this.windStrength < this.sourceRot)
				{
					this.windStrength = Math.Min(this.windStrength + frameTime * 2f, this.sourceRot);
				}
			}
			else
			{
				this.ripple *= this.undulateAmount;
				if (Math.Abs(Game1.wManager.windStrength) > 200f)
				{
					this.ripple = 1f;
				}
				else
				{
					this.windStrength += (float)Math.Cos(Game1.map.MapSegFrameLocked * 30f + (float)this.ropeID) * MathHelper.Clamp(Game1.wManager.windStrength / 200f * this.undulateAmount, -1f, 1f);
				}
			}
		}

		public void Draw(SpriteBatch sprite, Texture2D texture, MapSegment mapSeg, Vector2 pos, Rectangle sRect, Vector2 dimen, float scale, float worldScale, float frameTime)
		{
			this.undulateAmount += (this.undulateTarget - this.undulateAmount) * frameTime * this.speed;
			float num = (float)Math.Sin((double)Game1.map.MapSegFrame * 10.0 + (double)this.ropeID) * 0.1f;
			this.segRot[(this.boosting < 0) ? this.startSeg : 0] += (0f - this.windStrength + num - this.segRot[0]) * frameTime * this.speed;
			if (this.startSeg > 0 && this.boosting < 0)
			{
				this.segRot[0] = this.sourceRot;
				this.segRot[this.startSeg - 1] = (this.segRot[this.startSeg] + this.sourceRot) / 2f;
			}
			Vector2 zero = Vector2.Zero;
			Vector2 vector = new Vector2((float)(0.0 - Math.Sin(this.sourceRot)) * (float)sRect.Height / 2f, (float)Math.Cos(this.sourceRot) * (float)sRect.Height / 2f);
			int num2 = sRect.Height / this.segLoc.Length;
			bool flag = false;
			if (Math.Abs(this.ripple) > 0.4f)
			{
				flag = true;
			}
			float num3 = 0f;
			for (int i = 0; i < this.segLoc.Length; i++)
			{
				if (i > 0)
				{
					if (flag)
					{
						num3 = this.ripple * (float)Math.Cos((Game1.map.MapSegFrameLocked + (float)i + (float)this.ropeID) * 100f) * this.speed / 20f;
					}
					this.segRot[i] += (this.segRot[i - 1] + num3 - this.segRot[i]) * frameTime * 10f;
					zero += new Vector2((float)(0.0 - Math.Sin(this.segRot[i - 1])) * (float)num2, (float)Math.Cos(this.segRot[i - 1]) * (float)num2);
					this.segLoc[i] = zero;
				}
				if (i < this.segLoc.Length - 1)
				{
					sRect.Height = (int)((float)num2 + Math.Abs(this.segRot[i + 1] - this.segRot[i]) * (float)num2 / 2f * (float)sRect.Width / (float)num2 * (dimen.X / dimen.Y)) + 1;
				}
				else
				{
					sRect.Height = num2;
					switch (this.ropeType)
					{
					case 2:
						if (Rand.GetRandomInt(0, 100) == 0)
						{
							Vector2 vector2 = mapSeg.Location + (this.segLoc[i] - vector + new Vector2(0f, num2 / 2)) * dimen.Y * scale;
							Vector2 vector3 = vector2 + Rand.GetRandomVector2(-5f, 5f, 200f, 300f);
							Vector2 vector4 = new Vector2((float)(Math.Cos(this.segRot[i]) * (double)vector3.X - Math.Sin(this.segRot[i]) * (double)vector3.Y), (float)(Math.Cos(this.segRot[i]) * (double)vector3.Y + Math.Sin(this.segRot[i]) * (double)vector3.X)) - new Vector2((float)(Math.Cos(this.segRot[i]) * (double)vector2.X - Math.Sin(this.segRot[i]) * (double)vector2.Y), (float)(Math.Cos(this.segRot[i]) * (double)vector2.Y + Math.Sin(this.segRot[i]) * (double)vector2.X));
							Game1.pManager.AddLavaDrip(vector2, vector4, Rand.GetRandomFloat(0.5f, 1.2f), this.layer);
							for (int j = 0; j < 3; j++)
							{
								Game1.pManager.AddLavaDrip(vector2, vector4 / 2f, Rand.GetRandomFloat(0.2f, 0.4f), this.layer);
							}
						}
						break;
					case 3:
					{
						Vector2 position = pos + (this.segLoc[i] - vector) * dimen.Y * scale * worldScale;
						if (new Rectangle(-100, -200, Game1.screenWidth + 400, Game1.screenHeight + 200).Contains((int)position.X, (int)position.Y))
						{
							this.alpha = Math.Min(this.alpha + frameTime * 4f, 1f);
						}
						else
						{
							this.alpha = Math.Max(this.alpha - frameTime * 4f, 0f);
						}
						if (this.alpha > 0f)
						{
							sprite.End();
							sprite.Begin(SpriteSortMode.Immediate, BlendState.Additive);
							sprite.Draw(texture, position, new Rectangle(578, 2248, 169, 136), new Color(1f, 1f, 1f, Rand.GetRandomFloat(0.46f, 0.5f) * this.alpha), this.segRot[i], new Vector2(84f, 2f), dimen * scale * worldScale * new Vector2(3f, 4f), mapSeg.Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
							sprite.End();
							sprite.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
						}
						break;
					}
					}
				}
				sprite.Draw(texture, pos + (this.segLoc[i] - vector) * dimen.Y * scale * worldScale, sRect, mapSeg.color, this.segRot[i], this.orig, dimen * scale * worldScale, mapSeg.Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
				sRect.Y += num2;
			}
		}
	}
}
