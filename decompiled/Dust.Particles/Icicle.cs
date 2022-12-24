using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Icicle : Particle
	{
		private Vector2 pLoc;

		private float frame;

		private float size;

		public Icicle(Vector2 loc, Vector2 traj, float size, int type)
		{
			this.Reset(loc, traj, size, type);
		}

		public void Reset(Vector2 loc, Vector2 traj, float _size, int type)
		{
			base.exists = Exists.Init;
			base.background = false;
			base.trajectory = traj;
			this.size = _size;
			base.location = (this.pLoc = loc);
			this.frame = -0.5f;
			base.renderState = RenderState.AllEffects;
			base.owner = 99;
			base.flag = type;
			base.strength = (int)((float)Game1.character[0].MaxHP * Game1.stats.bonusHealth * 0.3f);
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.frame += gameTime * 2f;
			bool flag = HitManager.CheckHazard(this, c, pMan, canEvade: true);
			if (this.frame > 0.5f || (this.frame > 0f && flag))
			{
				Vector2 vector = base.GameLocation(l);
				if (!new Rectangle(0, -3000, Game1.screenWidth, Game1.screenHeight + 3400).Contains((int)vector.X, (int)vector.Y))
				{
					base.Reset();
					return;
				}
				if (map.CheckPCol(base.location, this.pLoc, canFallThrough: false, init: false) > 0f || flag)
				{
					Sound.PlayCue("icicle_hit", base.location, (base.location - c[0].Location).Length());
					base.Reset();
					pMan.AddSpray(base.location, new Vector2(0f, Rand.GetRandomFloat(-600f, -300f)), 1.2f, 2, 6, 5);
					pMan.AddSpray(base.location, new Vector2(0f, Rand.GetRandomFloat(-400f, -300f)), 0.5f, 1, 6, 5);
				}
			}
			if (base.trajectory.Y < 1400f)
			{
				base.trajectory.Y += gameTime * 2000f;
			}
			base.location += base.trajectory * gameTime;
			this.pLoc = base.location;
		}

		private void DrawCorona(SpriteBatch sprite, Texture2D[] particlesTex, Vector2 gameLoc, float alpha, int l)
		{
			Vector2 vector = new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2);
			Vector2 vector2 = gameLoc - vector;
			float num = Math.Abs(vector2.X + vector2.Y) / 2000f;
			if (!(num < 1f))
			{
				return;
			}
			Color color = new Color(1f, 1f, 1f, 1f - num);
			for (int i = 0; i < 8; i++)
			{
				Vector2 position = vector - vector2 * ((float)i / 2f - 1.75f);
				Rectangle value;
				Vector2 origin;
				Vector2 vector3;
				float rotation;
				switch (i)
				{
				case 1:
					value = new Rectangle(3879, 328, 99, 99);
					origin = new Vector2(45f, 45f);
					vector3 = new Vector2(1f, 1f);
					rotation = 0f;
					break;
				case 2:
					value = new Rectangle(3780, 328, 99, 99);
					color = new Color(1f, 1f, 1f, 0.5f - num);
					origin = new Vector2(45f, 45f);
					vector3 = new Vector2(0.5f, 0.5f);
					rotation = 0f;
					break;
				case 3:
					value = new Rectangle(3879, 328, 99, 99);
					origin = new Vector2(45f, 45f);
					vector3 = new Vector2(0.25f, 0.25f);
					rotation = 0f;
					break;
				case 4:
					value = new Rectangle(3780, 328, 99, 99);
					color = new Color(1f, 1f, 1f, 0.5f - num);
					origin = new Vector2(45f, 45f);
					vector3 = new Vector2(0.5f, 0.5f);
					rotation = 0f;
					break;
				default:
					value = new Rectangle(3978, 328, 99, 99);
					origin = new Vector2(45f, 45f);
					vector3 = new Vector2(1f, 1f);
					rotation = 0f;
					break;
				}
				sprite.Draw(particlesTex[2], position, value, color * alpha, rotation, origin, vector3 * Game1.hiDefScaleOffset, SpriteEffects.None, 1f);
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 vector = base.GameLocation(l);
			sprite.Draw(particlesTex[2], vector, new Rectangle(2292, 1000, 47, 240), new Color(1f, 1f, 1f, this.frame), 0f, new Vector2(24f, 200f), this.size * worldScale, SpriteEffects.None, 1f);
			if (Game1.pManager.renderingAdditive)
			{
				this.DrawCorona(sprite, particlesTex, vector - new Vector2(0f, 50f), Rand.GetRandomFloat(0.1f, 0.5f), l);
			}
		}
	}
}
