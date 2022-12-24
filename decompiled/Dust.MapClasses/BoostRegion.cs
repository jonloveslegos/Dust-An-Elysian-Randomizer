using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.MapClasses
{
	public class BoostRegion
	{
		public bool renderable;

		private Rectangle region;

		private int direction;

		private Vector2[] location;

		private Vector2[] trajectory;

		private float[] alpha;

		private byte[] type;

		private byte[] animFrame;

		private float[] animFrameTime;

		private float[] lifeSpan;

		private float[] frame;

		private float[] size;

		private float[] rotation;

		private RenderState[] renderState;

		public Rectangle Region => this.region;

		public int Direction => this.direction;

		public BoostRegion(Rectangle _region, int _direction)
		{
			this.region = _region;
			this.direction = _direction;
			int num = this.region.Width * this.region.Height / 12000;
			this.location = new Vector2[num];
			this.trajectory = new Vector2[num];
			this.alpha = new float[num];
			this.type = new byte[num];
			this.animFrame = new byte[num];
			this.animFrameTime = new float[num];
			this.lifeSpan = new float[num];
			this.frame = new float[num];
			this.size = new float[num];
			this.rotation = new float[num];
			this.renderState = new RenderState[num];
			for (int i = 0; i < num; i++)
			{
				this.type[i] = (byte)Math.Max(Rand.GetRandomInt(0, 2), 0);
				int randomInt;
				int randomInt2;
				if (this.type[i] == 0)
				{
					if (Rand.GetRandomInt(0, 4) == 0)
					{
						this.renderState[i] = RenderState.Refract;
						this.alpha[i] = 0.6f;
					}
					else
					{
						this.renderState[i] = RenderState.Additive;
						this.alpha[i] = 1f;
					}
					randomInt = Rand.GetRandomInt(1500, 3000);
					randomInt2 = Rand.GetRandomInt(-100, 100);
					this.size[i] = 2f;
				}
				else
				{
					this.rotation[i] = Rand.GetRandomFloat(0f, 6.28f);
					this.size[i] = 1f;
					this.alpha[i] = 1f;
					randomInt = Rand.GetRandomInt(400, 1800);
					randomInt2 = Rand.GetRandomInt(-200, 200);
					switch (Game1.map.regionName)
					{
					default:
						this.type[i] = 1;
						break;
					case "cave":
						this.type[i] = 2;
						this.renderState[i] = RenderState.Additive;
						break;
					case "trial":
					case "grave":
						this.type[i] = 3;
						break;
					case "snow":
						this.type[i] = 4;
						break;
					case "lava":
						this.type[i] = 5;
						break;
					}
				}
				switch (this.direction)
				{
				default:
				{
					ref Vector2 reference4 = ref this.trajectory[i];
					reference4 = new Vector2(randomInt2, -randomInt);
					break;
				}
				case 3:
				{
					ref Vector2 reference3 = ref this.trajectory[i];
					reference3 = new Vector2(randomInt, randomInt2);
					break;
				}
				case 6:
				{
					ref Vector2 reference2 = ref this.trajectory[i];
					reference2 = new Vector2(randomInt2, randomInt);
					break;
				}
				case 9:
				{
					ref Vector2 reference = ref this.trajectory[i];
					reference = new Vector2(-randomInt, randomInt2);
					break;
				}
				}
				if (this.type[i] == 0)
				{
					this.rotation[i] = GlobalFunctions.GetAngle(Vector2.Zero, this.trajectory[i]) + 1.57f;
				}
				this.ResetPos(i, ref this.type[i], init: true);
			}
		}

		private void ResetPos(int i, ref byte type, bool init)
		{
			if (type == 0)
			{
				this.frame[i] = (this.lifeSpan[i] = Rand.GetRandomFloat(0.8f, 1.2f));
			}
			else
			{
				this.frame[i] = (this.lifeSpan[i] = Rand.GetRandomFloat(1.4f, 1.8f) * 2f);
				this.animFrame[i] = (byte)Rand.GetRandomInt(0, 45);
				this.animFrameTime[i] = 0f;
			}
			switch (this.direction)
			{
			default:
			{
				ref Vector2 reference4 = ref this.location[i];
				reference4 = Rand.GetRandomVector2(this.region.X, this.region.X + this.region.Width, this.region.Y + Math.Min(2000, this.region.Height), this.region.Y + this.region.Height);
				break;
			}
			case 3:
			{
				ref Vector2 reference3 = ref this.location[i];
				reference3 = Rand.GetRandomVector2(this.region.X, this.region.X + Math.Max(this.region.Width - 3000, 0), this.region.Y, this.region.Y + this.region.Height);
				break;
			}
			case 6:
			{
				ref Vector2 reference2 = ref this.location[i];
				reference2 = Rand.GetRandomVector2(this.region.X, this.region.X + this.region.Width, this.region.Y, this.region.Y + Math.Max(this.region.Height - 3000, 0));
				break;
			}
			case 9:
			{
				ref Vector2 reference = ref this.location[i];
				reference = Rand.GetRandomVector2(this.region.X + Math.Min(3000, this.region.Width), this.region.X + this.region.Width, this.region.Y, this.region.Y + this.region.Height);
				break;
			}
			}
		}

		public void Update(float gameTime, Map map)
		{
			for (int i = 0; i < this.location.Length; i++)
			{
				if (!this.region.Contains((int)this.location[i].X, (int)this.location[i].Y))
				{
					this.frame[i] -= gameTime * 20f;
				}
				this.frame[i] -= gameTime;
				if (this.frame[i] < 0f)
				{
					this.ResetPos(i, ref this.type[i], init: false);
				}
				this.location[i] += this.trajectory[i] * gameTime;
				if (this.type[i] <= 0)
				{
					continue;
				}
				this.animFrameTime[i] += gameTime * 24f;
				if (this.animFrameTime[i] > 1f)
				{
					this.animFrame[i]++;
					if (this.animFrame[i] > 47)
					{
						this.animFrame[i] = 0;
					}
					this.animFrameTime[i] = 0f;
				}
			}
		}

		public void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, bool refractive, bool additive)
		{
			for (int i = 0; i < this.location.Length; i++)
			{
				if ((!refractive || this.renderState[i] != RenderState.Refract) && (!additive || this.renderState[i] != RenderState.Additive) && (refractive || this.renderState[i] != 0))
				{
					continue;
				}
				float num = (float)Math.Sin(this.frame[i] / 2f / this.lifeSpan[i]);
				Vector2 position = this.location[i] * worldScale - Game1.Scroll;
				if (this.type[i] == 0)
				{
					sprite.Draw(particlesTex[2], position, new Rectangle(1856, 2596, 128, 128), new Color(1f, 1f, 1f, Math.Min(this.alpha[i] * num * 2f, 0.08f)), this.rotation[i], new Vector2(64f, 96f), new Vector2(2f, this.size[i] * 2f) * worldScale, SpriteEffects.None, 1f);
					continue;
				}
				switch (this.type[i])
				{
				case 1:
					sprite.Draw(particlesTex[2], position, new Rectangle(1740 + this.animFrame[i] * 32, 2011, 32, 31), Color.White * num * 2f, this.rotation[i], new Vector2(16f, 16f), worldScale, SpriteEffects.None, 1f);
					break;
				case 2:
					sprite.Draw(particlesTex[2], position, new Rectangle(1740 + this.animFrame[i] * 32, 2042, 32, 32), Color.White * num * 2f, this.rotation[i], new Vector2(16f, 16f), worldScale, SpriteEffects.None, 1f);
					break;
				case 3:
					sprite.Draw(particlesTex[2], position, new Rectangle(1468 + this.animFrame[i] * 32, 4064, 32, 32), Color.White * num * 4f, this.rotation[i], new Vector2(16f, 16f), 1.5f * worldScale, SpriteEffects.None, 1f);
					break;
				case 5:
					sprite.Draw(particlesTex[2], position, new Rectangle(1468 + this.animFrame[i] * 32, 4032, 32, 32), Color.White * num * 4f, this.rotation[i], new Vector2(16f, 16f), 0.8f * worldScale, SpriteEffects.None, 1f);
					break;
				}
			}
		}
	}
}
