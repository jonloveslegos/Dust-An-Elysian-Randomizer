using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class ElectricBolt : Particle
	{
		private Vector2[] boltNodes;

		private Vector2 destLoc;

		private float frame;

		private byte nodeCount;

		private byte drawingMode;

		private float boltWidth;

		private int boltLength;

		public ElectricBolt(Vector2 loc, int _owner, float lifeSpan, int range, int _boltLength, float _boltWidth, int l)
		{
			this.Reset(loc, _owner, lifeSpan, range, _boltLength, _boltWidth, l);
		}

		public void Reset(Vector2 loc, int _owner, float lifeSpan, int range, int _boltLength, float _boltWidth, int l)
		{
			base.exists = Exists.Init;
			base.location = loc;
			base.owner = _owner;
			if (base.owner == 101)
			{
				this.destLoc = loc;
			}
			else if (!this.FindDestination(range))
			{
				base.exists = Exists.Dead;
				base.Reset();
				return;
			}
			base.maskGlow = 0.5f;
			this.frame = lifeSpan;
			this.boltLength = _boltLength;
			this.boltWidth = _boltWidth;
			Vector2 vector = (this.destLoc + base.location) / 2f;
			if (this.boltWidth < 0.8f && lifeSpan > 0.2f)
			{
				Sound.PlayCue("fidgetproj_03_contact", vector, (vector - Game1.character[0].Location).Length() * 1.5f);
			}
			this.drawingMode = 0;
			this.nodeCount = 0;
			base.renderState = RenderState.AdditiveOnly;
			base.exists = Exists.Exists;
		}

		private bool FindDestination(int range)
		{
			int num = 0;
			while (num < 3)
			{
				num++;
				float randomFloat = Rand.GetRandomFloat(0f, 6.28f);
				Vector2 vector = new Vector2((float)(Math.Cos(randomFloat) * (double)range - Math.Sin(randomFloat) * 0.0), (float)(Math.Cos(randomFloat) * 0.0 + Math.Sin(randomFloat) * (double)range)) + base.location;
				this.destLoc = base.location;
				float num2 = 0.2f;
				while (num2 <= 1f)
				{
					Vector2 pLoc = this.destLoc;
					this.destLoc = base.location * (1f - num2) + vector * num2;
					num2 += 0.2f;
					float y;
					if ((y = Game1.map.CheckPCol(this.destLoc, pLoc, canFallThrough: false, init: false)) > 0f)
					{
						this.destLoc.Y = y;
						return true;
					}
				}
			}
			return false;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.frame -= gameTime;
			if (this.frame < 0f)
			{
				base.Reset();
			}
			if (Game1.skipFrame <= 1 || this.drawingMode != 0)
			{
				return;
			}
			this.drawingMode = 1;
			int num = (int)((float)this.boltLength * Game1.worldScale);
			int num2 = base.owner;
			switch (num2)
			{
			case 100:
			case 101:
				base.location = Game1.pManager.GetFidgetLoc(accomodateScroll: false);
				break;
			default:
				if (num2 < c.Length && c[num2].Exists == CharExists.Exists)
				{
					base.location = c[num2].Location - new Vector2(0f, c[num2].Height / 2) + Rand.GetRandomVector2(-50f, 50f, -50f, 50f);
				}
				break;
			case -1:
				break;
			}
			float num3 = (this.destLoc - base.location).Length();
			int num4 = (int)Math.Max(50f, 10f * num3 / 300f);
			byte b = this.nodeCount;
			this.nodeCount = (byte)Math.Min(num3 / Math.Min(num, num3), 250f);
			if (this.nodeCount != b)
			{
				this.boltNodes = new Vector2[this.nodeCount];
			}
			float num5 = GlobalFunctions.GetAngle(base.location, this.destLoc) + 1.57f;
			for (int i = 0; i < this.boltNodes.Length; i++)
			{
				float num6 = (float)i / (float)this.boltNodes.Length;
				float num7 = (float)Math.Sin(6.28 * (double)(num6 / 2f / 1f)) * Game1.worldScale;
				ref Vector2 reference = ref this.boltNodes[i];
				reference = this.destLoc * num6 + base.location * (1f - num6) + new Vector2((Rand.GetRandomFloat(-num4, num4) + base.trajectory.Y) * (float)Math.Cos(num5), (Rand.GetRandomFloat(-num4, num4) + base.trajectory.Y * Game1.worldScale) * (float)Math.Sin(num5)) * num7;
			}
			if (Rand.GetRandomInt(0, 10) == 0)
			{
				base.trajectory.Y = 0f;
				base.trajectory.X = Rand.GetRandomFloat(-80f, 80f) * (float)(int)this.nodeCount;
			}
			base.trajectory.Y += (base.trajectory.X - base.trajectory.Y) * gameTime * 4f * this.boltWidth;
			this.drawingMode = 0;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (this.drawingMode != 0)
			{
				return;
			}
			this.drawingMode = 2;
			Rectangle value = new Rectangle(2940, 1860, 497, 150);
			if (this.boltNodes != null)
			{
				for (int i = 0; i < this.nodeCount; i++)
				{
					float num;
					float rotation;
					if (i < this.boltNodes.Length - 1)
					{
						num = (this.boltNodes[i + 1] - this.boltNodes[i]).Length();
						rotation = GlobalFunctions.GetAngle(this.boltNodes[i], this.boltNodes[i + 1]) + 1.57f;
					}
					else
					{
						num = (this.destLoc - this.boltNodes[i]).Length();
						rotation = GlobalFunctions.GetAngle(this.boltNodes[i], this.destLoc) + 1.57f;
					}
					Vector2 position = this.boltNodes[i] * worldScale - Game1.Scroll;
					sprite.Draw(particlesTex[2], position, new Rectangle(2132 + Rand.GetRandomInt(0, 2) * 40, 1000 + Rand.GetRandomInt(0, 2) * 120, 40, 120), Color.White * this.frame * 4f, rotation, new Vector2(20f, 10f), new Vector2(Rand.GetRandomFloat(0.6f, 2f) * this.boltWidth, num / 110f) * worldScale, (Rand.GetRandomInt(0, 2) != 0) ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
					if (Rand.GetRandomFloat(0f, 8f) < this.boltWidth)
					{
						sprite.Draw(particlesTex[2], position, value, Color.White * this.frame * 4f, 0f, new Vector2(248f, 75f), worldScale, SpriteEffects.None, 1f);
					}
				}
			}
			this.drawingMode = 0;
		}
	}
}
