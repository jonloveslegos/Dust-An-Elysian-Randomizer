using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Lightning : Particle
	{
		private Vector2[] boltNodes;

		private byte[] boltType;

		private float frame;

		private float boltWidth;

		private int boltLength;

		public Lightning(Vector2 loc, bool master, float lifeSpan, int _boltCount, int _boltLength, float _boltWidth, int l)
		{
			this.Reset(loc, master, lifeSpan, _boltCount, _boltLength, _boltWidth, l);
		}

		public void Reset(Vector2 loc, bool master, float lifeSpan, int _boltCount, int _boltLength, float _boltWidth, int l)
		{
			base.exists = Exists.Init;
			base.location = loc;
			this.frame = lifeSpan;
			base.flag = ((!master) ? 1 : 0);
			this.BuildBolt(_boltCount);
			this.boltLength = _boltLength;
			this.boltWidth = _boltWidth;
			base.renderState = RenderState.AdditiveOnly;
			base.exists = Exists.Exists;
		}

		private void BuildBolt(int _boltCount)
		{
			this.boltNodes = new Vector2[_boltCount];
			this.boltType = new byte[_boltCount - 1];
			ref Vector2 reference = ref this.boltNodes[0];
			reference = base.location;
			for (int i = 0; i < this.boltType.Length; i++)
			{
				this.boltType[i] = (byte)Rand.GetRandomInt(0, 8);
			}
			this.boltType[this.boltType.Length - 1] = 3;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.frame -= gameTime;
			if (this.frame < 0f)
			{
				base.Reset();
			}
			for (int i = 1; i < this.boltNodes.Length; i++)
			{
				if (this.boltNodes[i] == Vector2.Zero && this.boltNodes[i - 1] != Vector2.Zero && Rand.GetRandomInt(0, 10) < 5)
				{
					ref Vector2 reference = ref this.boltNodes[i];
					reference = this.boltNodes[i - 1] + Rand.GetRandomVector2(-60f, 60f, 20f, 100f) * Game1.hiDefScaleOffset;
					if (Game1.drawState == Game1.DrawState.None && Rand.GetRandomInt(0, 10) < 4)
					{
						pMan.AddLightning(this.boltNodes[i], master: false, this.frame, Math.Max(this.boltNodes.Length / 2, 2), this.boltLength, this.boltWidth, l);
					}
				}
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (base.flag == 0)
			{
				sprite.Draw(particlesTex[2], this.boltNodes[0] + Game1.wManager.mapBackPos, new Rectangle(964, 3960, 504, 136), Color.White * this.frame * Rand.GetRandomFloat(1f, 4f), 0f, new Vector2(250f, 70f), Game1.hiDefScaleOffset, SpriteEffects.FlipHorizontally, 0f);
			}
			Rectangle value = new Rectangle(2940, 1860, 497, 150);
			if (this.boltNodes == null)
			{
				return;
			}
			int num = this.boltNodes.Length;
			for (int i = 0; i < num - 1; i++)
			{
				if (this.boltNodes[i + 1] != Vector2.Zero)
				{
					float num2 = ((float)(num - i) / (float)num + 0.25f) * 2f * Game1.hiDefScaleOffset;
					float num3 = (this.boltNodes[i + 1] - this.boltNodes[i]).Length();
					float num4 = GlobalFunctions.GetAngle(this.boltNodes[i], this.boltNodes[i + 1]) + 1.57f;
					Rectangle value2;
					switch (this.boltType[i])
					{
					default:
						value2 = new Rectangle(2132, 1000, 40, 120);
						break;
					case 1:
					case 5:
						value2 = new Rectangle(2172, 1000, 40, 120);
						break;
					case 2:
					case 6:
						value2 = new Rectangle(2132, 1120, 40, 120);
						break;
					case 3:
					case 7:
						value2 = new Rectangle(2172, 1120, 40, 120);
						break;
					}
					sprite.Draw(particlesTex[2], this.boltNodes[i] + Game1.wManager.mapBackPos, value2, Color.White * this.frame * 4f, num4, new Vector2(20f, 10f), new Vector2(Rand.GetRandomFloat(0.8f, 1f) * this.boltWidth * num2, num3 / 110f), (this.boltType[i] >= 4) ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
					if (Rand.GetRandomInt(0, 30) == 0)
					{
						sprite.Draw(particlesTex[2], (this.boltNodes[i] + this.boltNodes[i + 1]) / 2f + Game1.wManager.mapBackPos, value, Color.White * this.frame * 4f, num4 + 1.57f, new Vector2(248f, 75f), 0.2f * Game1.hiDefScaleOffset, SpriteEffects.None, 1f);
					}
				}
			}
		}
	}
}
