using System;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Spray : Particle
	{
		private Rectangle[] sRect;

		private Vector2[] loc;

		private Vector2[] pLoc;

		private Vector2[] traj;

		private bool[] sExists;

		private float frame;

		private float size;

		private float[] rotation;

		public Spray(Vector2 loc, Vector2 _traj, float size, int type, byte count)
		{
			this.Reset(loc, _traj, size, type, count);
		}

		public void Reset(Vector2 _loc, Vector2 _traj, float _size, int type, byte count)
		{
			base.exists = Exists.Init;
			base.location = _loc;
			float angle = GlobalFunctions.GetAngle(Vector2.Zero, _traj);
			if (this.loc == null || this.loc.Length != count)
			{
				this.loc = new Vector2[count];
				this.pLoc = new Vector2[count];
				this.traj = new Vector2[count];
				this.rotation = new float[count];
				this.sExists = new bool[count];
				this.sRect = new Rectangle[count];
			}
			for (int i = 0; i < this.loc.Length; i++)
			{
				ref Vector2 reference = ref this.loc[i];
				ref Vector2 reference2 = ref this.pLoc[i];
				reference = (reference2 = _loc + new Vector2(Rand.GetRandomFloat(-100f, 100f), 0f) * this.size);
				float num = angle + Rand.GetRandomFloat(-0.4f, 0.4f);
				ref Vector2 reference3 = ref this.traj[i];
				reference3 = new Vector2((float)Math.Cos(num), (float)Math.Sin(num)) * (0f - _traj.Length()) * Rand.GetRandomFloat(0.7f, 1.5f);
				this.rotation[i] = Rand.GetRandomFloat(0f, 6.28f);
				this.sExists[i] = true;
			}
			base.flag = type;
			base.owner = 600;
			base.renderState = RenderState.AdditiveOnly;
			switch (type)
			{
			case 0:
			{
				for (int k = 0; k < this.sRect.Length; k++)
				{
					ref Rectangle reference5 = ref this.sRect[k];
					reference5 = new Rectangle(1040, 2110, 10, 10);
				}
				this.size = Rand.GetRandomFloat(0.9f, 1.1f) * _size;
				this.frame = Rand.GetRandomFloat(0.8f, 1.2f);
				break;
			}
			case 1:
			{
				for (int l = 0; l < this.sRect.Length; l++)
				{
					ref Rectangle reference6 = ref this.sRect[l];
					reference6 = new Rectangle(2212, 1000 + (byte)Rand.GetRandomInt(0, 3) * 80, 80, 80);
				}
				this.size = Rand.GetRandomFloat(0.9f, 1.4f) * _size;
				this.frame = Rand.GetRandomFloat(0.4f, 1f);
				for (int m = 0; m < this.loc.Length; m++)
				{
					this.traj[m].Y /= 2f;
				}
				break;
			}
			case 2:
			{
				this.size = Rand.GetRandomFloat(0.9f, 1.1f) * _size;
				this.frame = 5f;
				base.owner = 2000;
				for (int n = 0; n < this.loc.Length; n++)
				{
					ref Vector2 reference7 = ref this.loc[n];
					ref Vector2 reference8 = ref this.pLoc[n];
					reference7 = (reference8 = _loc + new Vector2(0f, Rand.GetRandomFloat(-100f, 0f)) * this.size);
					ref Rectangle reference9 = ref this.sRect[n];
					reference9 = new Rectangle(546 + Rand.GetRandomInt(0, 4) * 24, 3686, 24, 64);
					if (this.traj[n].Y == 0f)
					{
						this.rotation[n] = 0f;
					}
				}
				break;
			}
			case 3:
			{
				for (int j = 0; j < this.loc.Length; j++)
				{
					ref Rectangle reference4 = ref this.sRect[j];
					reference4 = new Rectangle(964 + 120 * Rand.GetRandomInt(0, 3), 3840, 120, 120);
				}
				this.size = Rand.GetRandomFloat(0.9f, 1.1f) * _size;
				this.frame = Rand.GetRandomFloat(0.8f, 1.2f);
				base.renderState = RenderState.Normal;
				break;
			}
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			bool flag = false;
			for (int i = 0; i < this.loc.Length; i++)
			{
				if (this.sExists[i])
				{
					flag = true;
					this.traj[i].Y += gameTime * (float)base.owner;
					if (this.traj[i].X > 0f)
					{
						this.rotation[i] += gameTime * 2f;
					}
					else
					{
						this.rotation[i] -= gameTime * 2f;
					}
					if (this.traj[i].Y > 0f && l < 7 && map.CheckPCol(this.loc[i], this.pLoc[i], canFallThrough: false, init: false) > 0f)
					{
						this.sExists[i] = false;
					}
					ref Vector2 reference = ref this.pLoc[i];
					reference = this.loc[i];
					this.loc[i] += this.traj[i] * gameTime;
				}
			}
			this.frame -= gameTime;
			if (this.frame < 0f || !flag)
			{
				base.Reset();
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			for (int i = 0; i < this.loc.Length; i++)
			{
				if (this.sExists[i])
				{
					Vector2 position = ((l < 5) ? base.GameLocation(l, this.loc[i]) : (this.loc[i] * Game1.worldScale - Game1.Scroll));
					sprite.Draw(particlesTex[2], position, this.sRect[i], new Color(1f, 1f, 1f, this.frame), this.rotation[i], new Vector2(this.sRect[i].Width, this.sRect[i].Height) / 2f, this.size * worldScale, SpriteEffects.None, 1f);
				}
			}
		}
	}
}
