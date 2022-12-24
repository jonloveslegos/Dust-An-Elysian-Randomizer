using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Avalanche : Particle
	{
		private Rectangle sRect;

		private Vector2[] segLoc;

		private Vector2[] segPloc;

		private Vector2[] segTraj;

		private Vector2[] segScale;

		private CharState[] segState;

		private float[] segDelay;

		private float[] segRot;

		private float[] segRotTarg;

		private int[] segLedge;

		private int segments;

		private int width;

		private Rectangle renderRect;

		private Vector2 audioLoc;

		private int origX;

		private int offsetX;

		public Avalanche(Vector2 loc, CharDir direction, int width, int segments, int i, int l)
		{
			this.Reset(loc, direction, width, segments, i, l);
		}

		public void Reset(Vector2 loc, CharDir direction, int _width, int _segments, int i, int l)
		{
			base.exists = Exists.Init;
			this.width = _width;
			this.segments = _segments;
			this.segLoc = new Vector2[this.segments];
			this.segPloc = new Vector2[this.segments];
			this.segTraj = new Vector2[this.segments];
			this.segScale = new Vector2[this.segments];
			this.segState = new CharState[this.segments];
			this.segDelay = new float[this.segments];
			this.segRot = new float[this.segments];
			this.segRotTarg = new float[this.segments];
			this.segLedge = new int[this.segments];
			for (int j = 0; j < this.segLoc.Length; j++)
			{
				ref Vector2 reference = ref this.segLoc[j];
				reference = (this.segPloc[j] = loc);
				ref Vector2 reference2 = ref this.segTraj[j];
				reference2 = new Vector2(Rand.GetRandomInt(1200, 1600) * ((direction != 0) ? 1 : (-1)), 400f);
				ref Vector2 reference3 = ref this.segScale[j];
				reference3 = Rand.GetRandomVector2(1.8f, 2.2f, 1f, 2.5f);
				this.segState[j] = CharState.Air;
				this.segLedge[j] = -1;
				this.segDelay[j] = (float)j * (0f - (float)this.width / (float)Rand.GetRandomInt(2600, 2800));
				this.segRot[j] = (this.segRotTarg[j] = 0f);
				if ((float)j > (float)this.segLoc.Length * 0.75f)
				{
					this.segScale[j] *= 0.75f;
				}
			}
			if (direction == CharDir.Right)
			{
				this.offsetX = -200;
				this.origX = 340;
			}
			else
			{
				this.origX = 135;
				this.offsetX = 200;
			}
			this.audioLoc = loc;
			this.sRect = new Rectangle(2084, 3310, 475, 238);
			base.owner = Game1.character.Length - 1;
			base.renderState = RenderState.Normal;
			this.renderRect = new Rectangle(-20, -100, Game1.screenWidth + 40, Game1.screenHeight + 200);
			Sound.PlayFollowParticleCue("avalanche_slide", i, l);
			base.exists = Exists.Exists;
		}

		private void UpdateCollision(Map map, int i, ParticleManager pMan)
		{
			int num = map.CheckCol(this.segLoc[i]);
			if (this.segState[i] == CharState.Air)
			{
				if (num > 0 && num < 3)
				{
					this.segLoc[i].Y = (int)(this.segLoc[i].Y / 64f) * 64;
					this.segLedge[i] = -1;
					this.segState[i] = CharState.Grounded;
				}
				for (int j = 0; j < map.maxPlayerLedges; j++)
				{
					if (!map.GetLedgeMinMax(j, this.segLoc[i].X))
					{
						continue;
					}
					int ledgeSec = map.GetLedgeSec(j, this.segPloc[i].X);
					int ledgeSec2 = map.GetLedgeSec(j, this.segLoc[i].X);
					if (ledgeSec > -1 && ledgeSec2 > -1)
					{
						float ledgeYLoc = map.GetLedgeYLoc(j, ledgeSec, this.segPloc[i].X);
						float ledgeYLoc2 = map.GetLedgeYLoc(j, ledgeSec2, this.segLoc[i].X);
						if (this.segPloc[i].Y <= ledgeYLoc + 30f && this.segLoc[i].Y >= ledgeYLoc2)
						{
							this.segLoc[i].Y = ledgeYLoc2;
							this.segLedge[i] = j;
							this.segState[i] = CharState.Grounded;
							break;
						}
					}
				}
			}
			else
			{
				if (this.segState[i] != 0)
				{
					return;
				}
				if (this.segLedge[i] != -1)
				{
					if (num == 0)
					{
						int ledgeSec3 = map.GetLedgeSec(this.segLedge[i], this.segLoc[i].X);
						if (ledgeSec3 == -1)
						{
							this.segState[i] = CharState.Air;
						}
						else
						{
							this.segLoc[i].Y = map.GetLedgeYLoc(this.segLedge[i], ledgeSec3, this.segLoc[i].X);
						}
					}
					else
					{
						this.segLoc[i].Y = (int)(this.segLoc[i].Y / 64f) * 64;
						this.segLedge[i] = -1;
					}
					return;
				}
				for (int k = 0; k < map.maxPlayerLedges; k++)
				{
					if (!map.GetLedgeMinMax(k, this.segLoc[i].X))
					{
						continue;
					}
					int ledgeSec4 = map.GetLedgeSec(k, this.segPloc[i].X);
					int ledgeSec5 = map.GetLedgeSec(k, this.segLoc[i].X);
					if (ledgeSec4 > -1 && ledgeSec5 > -1)
					{
						float ledgeYLoc3 = map.GetLedgeYLoc(k, ledgeSec4, this.segPloc[i].X);
						float ledgeYLoc4 = map.GetLedgeYLoc(k, ledgeSec5, this.segLoc[i].X);
						if (this.segPloc[i].Y <= ledgeYLoc3 + 20f && this.segLoc[i].Y >= ledgeYLoc4 - 2f)
						{
							this.segLoc[i].Y = ledgeYLoc4;
							this.segLedge[i] = k;
							break;
						}
					}
				}
				if (num == 0 && this.segLedge[i] == -1)
				{
					this.segState[i] = CharState.Air;
					if (this.CanRender(i))
					{
						pMan.AddSpray(this.segLoc[i] + Rand.GetRandomVector2(-this.width, this.width, -this.width, this.width) / 2f, Rand.GetRandomVector2(-100f, 100f, 400f, 500f), Rand.GetRandomFloat(0.5f, 1.5f), 1, 2, 7);
					}
				}
			}
		}

		private bool CanRender(int i)
		{
			Vector2 vector = this.segLoc[i] * Game1.worldScale - Game1.Scroll;
			if (vector.X > (float)this.renderRect.X && vector.X < (float)this.renderRect.Width && vector.Y > (float)this.renderRect.Y && vector.Y < (float)this.renderRect.Height)
			{
				return true;
			}
			return false;
		}

		public override void GetAudio(ref Vector2 loc, ref Vector2 traj, ref bool _exists)
		{
			loc = (this.audioLoc + Game1.character[0].Location) / 2f;
			_exists = base.exists != Exists.Dead;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			float num = 1000000f;
			float num2 = num;
			float num3 = 0f;
			bool flag = false;
			for (int i = 0; i < this.segLoc.Length; i++)
			{
				this.segDelay[i] += gameTime;
				if (this.segDelay[i] < 0f)
				{
					flag = true;
				}
				else
				{
					ref Vector2 reference = ref this.segPloc[i];
					reference = this.segLoc[i];
					this.segLoc[i].X += this.segTraj[i].X * gameTime;
					if (this.segState[i] == CharState.Grounded)
					{
						this.segTraj[i].Y = 0f;
					}
					else
					{
						this.segTraj[i].Y = Math.Min(this.segTraj[i].Y + gameTime * 3000f, 2000f);
						this.segLoc[i].Y += this.segTraj[i].Y * gameTime;
					}
					this.UpdateCollision(map, i, pMan);
					if (this.segTraj[i].X > 0f)
					{
						this.segRotTarg[i] = GlobalFunctions.GetAngle(this.segPloc[i], this.segLoc[i]) - 3.14f;
					}
					else
					{
						this.segRotTarg[i] = GlobalFunctions.GetAngle(this.segLoc[i], this.segPloc[i]) - 3.14f;
					}
					this.segRot[i] += (this.segRotTarg[i] - this.segRot[i]) * gameTime * 6f;
					if (i % 3 == 0)
					{
						Rectangle hitRect = new Rectangle((int)this.segLoc[i].X - this.width / 2, (int)this.segLoc[i].Y - this.width / 2, this.width, this.width / 2);
						for (int j = 0; j < c.Length; j++)
						{
							if (HitManager.CheckHitPossible(this, c, j, hitRect))
							{
								HitManager.CheckWallHazard(c, j, pMan, 200, ColType.Solid);
							}
						}
					}
					float num4 = Math.Min(500f / (c[0].Location - this.segLoc[i]).Length(), 1f);
					if (num4 > num3)
					{
						num3 = num4;
					}
					if (this.CanRender(i))
					{
						pMan.AddSpray(this.segLoc[i] + Rand.GetRandomVector2(-this.width, this.width, -this.width, this.width * 2) / 2f, Rand.GetRandomVector2(-200f, 200f, -400f, 0f) + this.segTraj[i] / 2f, Rand.GetRandomFloat(1f, 2f), 1, 2, Rand.GetRandomInt(5, 7));
					}
					if (Game1.longSkipFrame == 1)
					{
						num2 = (this.segLoc[i] - c[0].Location).Length();
						if (num2 < num)
						{
							num = num2;
							this.audioLoc = this.segLoc[i];
						}
					}
				}
				VibrationManager.ScreenShake.value = num3;
				VibrationManager.Rumble(Game1.currentGamePad, num3 / 4f);
				if (this.segLoc[i].Y < map.bottomEdge + (float)this.width)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				base.Reset();
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			for (int i = 0; i < this.segLoc.Length; i++)
			{
				if (this.segDelay[i] > 0f)
				{
					Vector2 vector = this.segLoc[i] * worldScale - Game1.Scroll;
					sprite.Draw(Game1.map.GetTexture(12), vector, this.sRect, new Color(1f, 1f, 1f, this.segDelay[i] * 2f), this.segRot[i], new Vector2(this.origX, 200f), this.segScale[i] * worldScale, (i % 2 == 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
					sprite.Draw(Game1.map.GetTexture(12), vector + new Vector2(this.offsetX, 80f) * worldScale, this.sRect, new Color(1f, 1f, 1f, this.segDelay[i] * 2f), this.segRot[i], new Vector2(this.origX, 200f), this.segScale[i] * worldScale, SpriteEffects.FlipHorizontally, 1f);
				}
			}
		}
	}
}
