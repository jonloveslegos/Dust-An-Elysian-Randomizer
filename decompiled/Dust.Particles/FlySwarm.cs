using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	public class FlySwarm : Particle
	{
		private Vector2[] flyLoc = new Vector2[20];

		private Vector2[] flyTraj = new Vector2[20];

		private Rectangle renderRect;

		private float alpha;

		public FlySwarm(Vector2 loc, int audioID, int l)
		{
			this.Reset(loc, audioID, l);
		}

		public void Reset(Vector2 loc, int audioID, int l)
		{
			base.exists = Exists.Init;
			Game1.map.bugCount++;
			base.trajectory = (base.location = loc);
			for (int i = 0; i < this.flyLoc.Length; i++)
			{
				ref Vector2 reference = ref this.flyLoc[i];
				reference = Rand.GetRandomVector2(-400f, 400f, -400f, 400f);
				ref Vector2 reference2 = ref this.flyTraj[i];
				reference2 = Rand.GetRandomVector2(-2000f, 2000f, -1000f, 1000f);
			}
			base.isSpun = Rand.GetRandomFloat(4f, 20f);
			base.owner = 1;
			this.alpha = 0f;
			this.renderRect = new Rectangle(-200, -200, Game1.screenWidth + 400, Game1.screenHeight + 400);
			base.flag = audioID;
			Sound.PlayFollowParticleCue("swarm", base.flag, l);
			base.exists = Exists.Exists;
		}

		public override void GetAudio(ref Vector2 loc, ref Vector2 traj, ref bool _exists)
		{
			loc = base.location + (base.location - Game1.character[0].Location) / 2f;
			_exists = base.exists != Exists.Dead;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (!this.renderRect.Contains((int)vector.X, (int)vector.Y))
			{
				map.bugCount--;
				base.Reset();
			}
			for (int i = 0; i < this.flyLoc.Length; i++)
			{
				float x = Math.Abs(this.flyLoc[i].X) * Rand.GetRandomFloat(0.5f, 4f);
				float num = Math.Abs(this.flyLoc[i].Y) * Rand.GetRandomFloat(0.5f, 4f);
				if (this.flyLoc[i].X < 0f)
				{
					this.flyTraj[i] += new Vector2(x, 0f);
				}
				else
				{
					this.flyTraj[i] -= new Vector2(x, 0f);
				}
				if (this.flyLoc[i].Y < 0f)
				{
					this.flyTraj[i] += new Vector2(0f, num * 0.75f);
				}
				else
				{
					this.flyTraj[i] -= new Vector2(0f, num);
				}
				this.flyTraj[i].X = MathHelper.Clamp(this.flyTraj[i].X, -4000f, 4000f);
				this.flyTraj[i].Y = MathHelper.Clamp(this.flyTraj[i].Y, -1000f, 1000f);
				if (Rand.GetRandomInt(0, 100) == 0)
				{
					if (Rand.GetRandomInt(0, 3) == 0)
					{
						this.flyTraj[i] *= 10f;
					}
					else
					{
						this.flyTraj[i].X *= 0.5f;
						this.flyTraj[i].Y *= 20f;
					}
				}
				this.flyLoc[i] += this.flyTraj[i] * gameTime * 0.25f;
			}
			if (this.alpha < 1f)
			{
				this.alpha += gameTime;
			}
			base.isSpun -= gameTime;
			if (base.isSpun < 0f)
			{
				base.isSpun = Rand.GetRandomFloat(4f, 20f);
				base.owner = Rand.GetRandomInt(0, 4);
				if (base.owner > 0)
				{
					base.trajectory = c[0].Location + Rand.GetRandomVector2(-1000f, 1000f, -700f, 200f);
				}
			}
			if (base.owner == 0)
			{
				base.trajectory = c[0].Location - new Vector2(0f, 200f);
			}
			float num2 = (float)Math.Cos((double)map.MapSegFrame * 20.0);
			float num3 = (float)Math.Sin((double)map.MapSegFrame * 30.0);
			base.location += (base.trajectory + new Vector2(num2 * 100f, num3 * 100f) - base.location) * gameTime * 0.7f;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			for (int i = 0; i < this.flyLoc.Length; i++)
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l) + this.flyLoc[i] * worldScale * base.LayerScale(l), new Rectangle(2048 + Game1.skipFrame * 64, 2724, 64, 64), Color.Black * this.alpha, GlobalFunctions.GetAngle(Vector2.Zero, this.flyTraj[i]), new Vector2(32f, 32f), (0.05f + (float)i * 0.01f) * worldScale, SpriteEffects.None, 0f);
			}
		}
	}
}
