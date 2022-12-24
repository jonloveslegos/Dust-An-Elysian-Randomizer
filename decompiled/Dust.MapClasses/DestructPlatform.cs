using System;
using Dust.Audio;
using Dust.CharClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.MapClasses
{
	public class DestructPlatform
	{
		public Vector2 location;

		private byte ID;

		private int debrisType;

		private float lifeSpan;

		public bool[] Exists;

		private byte[] segmentType;

		private byte[] segmentFlip;

		private float[] lifeTime;

		private float[] shakeTime;

		public DestructPlatform(Vector2 loc, int width, int _ID)
		{
			this.ID = (byte)_ID;
			this.location = loc;
			this.lifeSpan = 2f - (float)(int)Game1.stats.gameDifficulty * 0.25f;
			this.lifeSpan = 1.2f;
			this.Exists = new bool[width / 240];
			this.segmentType = new byte[this.Exists.Length];
			this.segmentFlip = new byte[this.Exists.Length];
			this.lifeTime = new float[this.Exists.Length];
			this.shakeTime = new float[this.Exists.Length];
			for (int i = 0; i < this.Exists.Length; i++)
			{
				this.Exists[i] = true;
				this.segmentType[i] = (byte)Rand.GetRandomInt(0, 6);
				this.segmentFlip[i] = (byte)Rand.GetRandomInt(0, 2);
				this.lifeTime[i] = this.lifeSpan;
				this.shakeTime[i] = 0f;
			}
			this.debrisType = 1010 + Game1.dManager.TextureType(Game1.map.path);
		}

		public void Update(float gameTime, Map map, DestructableManager dMan, Character[] c)
		{
			for (int i = 0; i < this.Exists.Length; i++)
			{
				if (this.lifeTime[i] > this.lifeSpan)
				{
					this.lifeTime[i] = Math.Max(this.lifeTime[i] - gameTime * 4f, this.lifeSpan);
				}
				else if (this.lifeTime[i] < this.lifeSpan)
				{
					this.shakeTime[i] = this.shakeTime[i] + gameTime * 0.4f;
					bool flag = false;
					if (this.lifeTime[i] > 0f)
					{
						Vector2 vector = (this.location + new Vector2(120 + i * 240, 30f)) * Game1.worldScale - Game1.Scroll;
						if (new Rectangle(-100, -1000, Game1.screenWidth + 200, Game1.screenHeight + 1200).Contains((int)vector.X, (int)vector.Y))
						{
							flag = true;
							if (Rand.GetRandomInt(0, (int)(10f / (this.lifeSpan / this.lifeTime[i]))) == 0)
							{
								Game1.pManager.AddBlood(this.location + new Vector2(i * 240, 0f) + Rand.GetRandomVector2(-40f, 280f, 0f, 90f), Rand.GetRandomVector2(-40f, 40f, -20f, 20f), 1f, 1f, 1f, 1f, Rand.GetRandomFloat(0.6f, 1.5f), (CharacterType)this.debrisType, 0, 6);
							}
						}
					}
					float num = this.lifeTime[i];
					this.lifeTime[i] -= gameTime;
					if (this.lifeTime[i] < 0.8f && num >= 0.8f)
					{
						for (int j = Math.Max(i - 1, 0); j < Math.Min(i + 2, this.Exists.Length); j++)
						{
							this.lifeTime[j] = Math.Min(this.lifeSpan - gameTime, this.lifeTime[j]);
							Sound.PlayCue("platform_rock_crumble", this.location, (this.location - c[0].Location).Length());
						}
					}
					if (this.lifeTime[i] < 0f && num >= 0f)
					{
						this.Exists[i] = false;
						if (flag)
						{
							Vector2 vector2 = this.location + new Vector2(i * 240, 0f);
							Game1.pManager.AddShockRing(this.location + new Vector2(120 + i * 240, 30f), 0.2f, 5);
							for (int k = 0; k < 30; k++)
							{
								Game1.pManager.AddBlood(vector2 + Rand.GetRandomVector2(-40f, 280f, -40f, 60f), Rand.GetRandomVector2(-200f, 200f, -300f, 0f), 1f, 1f, 1f, 1f, 2.5f, (CharacterType)this.debrisType, 0, 5);
							}
							Sound.PlayCue("platform_rock_destroy", vector2, (vector2 - c[0].Location).Length());
						}
					}
					if (this.lifeTime[i] < -5f && num >= -5f)
					{
						this.lifeTime[i] = this.lifeSpan + 1f;
						this.shakeTime[i] = 0f;
						this.Exists[i] = true;
						Vector2 vector3 = this.location + new Vector2(i * 240, 0f);
						Sound.PlayCue("platform_grow", vector3, (vector3 - c[0].Location).Length());
					}
				}
				else if (c[0].State == CharState.Grounded && c[0].Location.Y == this.location.Y && c[0].Location.X > this.location.X + (float)(i * 240) - 30f && c[0].Location.X < this.location.X + (float)(i * 240) + 270f && map.GetTransVal() <= 0f)
				{
					this.lifeTime[i] = Math.Min(this.lifeSpan - gameTime, this.lifeTime[i]);
					Vector2 vector4 = this.location + new Vector2(i * 240, 0f);
					Sound.PlayCue("platform_rock_crumble", vector4, (vector4 - c[0].Location).Length());
				}
			}
		}

		public void Draw(SpriteBatch sprite, Texture2D destTex)
		{
			if (destTex != null && !destTex.IsDisposed)
			{
				if (destTex.Height <= 900 || !((this.location.X + (float)(240 * this.Exists.Length)) * Game1.worldScale - Game1.Scroll.X > -50f) || !(this.location.X * Game1.worldScale - Game1.Scroll.X < (float)(Game1.screenWidth + 50)) || !(this.location.Y * Game1.worldScale - Game1.Scroll.Y > -120f * Game1.hiDefScaleOffset) || !(this.location.Y * Game1.worldScale - Game1.Scroll.Y < (float)Game1.screenHeight + 50f * Game1.hiDefScaleOffset))
				{
					return;
				}
				for (int i = 0; i < this.Exists.Length; i++)
				{
					if (this.Exists[i])
					{
						Vector2 vector = (this.location + new Vector2(120 + i * 240, 20f)) * Game1.worldScale - Game1.Scroll;
						if (this.shakeTime[i] > 0f)
						{
							vector += Rand.GetRandomVector2(-50f, 50f, -20f, 20f) * this.shakeTime[i] * Game1.FrameTime * 20f;
						}
						SpriteEffects spriteEffects = (SpriteEffects)this.segmentFlip[i];
						if (i > 0 && this.Exists[i - 1])
						{
							float num = (this.shakeTime[i] + this.shakeTime[i - 1]) / 2f;
							float num2 = Rand.GetRandomFloat(-0.2f, 0.2f) * num + num * (float)((spriteEffects == SpriteEffects.None) ? 1 : (-1));
							float num3 = Math.Min(1f - ((this.lifeTime[i] + this.lifeTime[i - 1]) / 2f - this.lifeSpan), 1f);
							sprite.Draw(destTex, vector + new Vector2(-120f, num * 80f) * Game1.worldScale, new Rectangle((int)this.segmentType[i] / 2 * 200, 900, 200, 100), new Color(1f, 1f, 1f, num3), (0f - num2) / 2f, new Vector2(100f, 40f), new Vector2(1.2f, 2f * num3) * Game1.worldScale, spriteEffects, 0f);
						}
					}
				}
				for (int j = 0; j < this.Exists.Length; j++)
				{
					if (this.Exists[j])
					{
						Vector2 vector = (this.location + new Vector2(120 + j * 240, 30f)) * Game1.worldScale - Game1.Scroll;
						if (this.shakeTime[j] > 0f)
						{
							vector += Rand.GetRandomVector2(-50f, 50f, -20f, 20f) * this.shakeTime[j] * Game1.FrameTime * 20f;
						}
						SpriteEffects spriteEffects = (SpriteEffects)this.segmentFlip[j];
						float num2 = Rand.GetRandomFloat(-0.2f, 0.2f) * this.shakeTime[j] + this.shakeTime[j] * (float)((spriteEffects == SpriteEffects.None) ? 1 : (-1));
						float num3 = Math.Min(1f - (this.lifeTime[j] - this.lifeSpan), 1f);
						sprite.Draw(destTex, vector, new Rectangle((int)this.segmentType[j] / 2 * 200, 900, 200, 100), new Color(1f, 1f, 1f, num3), num2 / 4f, new Vector2(100f, 40f), 1.5f * new Vector2(1f, num3) * Game1.worldScale, spriteEffects, 0f);
					}
				}
			}
			else if (Game1.longSkipFrame > 3 && Game1.map.GetTransVal() <= 0f)
			{
				Game1.dManager.SetDestTextures();
			}
		}
	}
}
