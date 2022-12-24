using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class LadySphere : Particle
	{
		private float frame;

		private float size;

		private byte stage;

		public LadySphere(Vector2 loc, float size, int strength, int _owner)
		{
			this.Reset(loc, size, strength, _owner);
		}

		public void Reset(Vector2 loc, float _size, int _strength, int _owner)
		{
			base.exists = Exists.Init;
			base.owner = _owner;
			base.location = loc;
			this.size = _size;
			base.strength = _strength;
			this.stage = 0;
			this.frame = 1f;
			base.renderState = RenderState.Refract;
			Sound.PlayCue("lady_sphere", base.location, (Game1.character[0].Location - base.location).Length());
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			switch (this.stage)
			{
			case 0:
			{
				this.frame -= gameTime;
				float randomFloat = Rand.GetRandomFloat(0f, 6.28f);
				Vector2 loc = new Vector2((float)(Math.Cos(randomFloat) * 200.0 * (double)this.frame), (float)(Math.Sin(randomFloat) * 200.0 * (double)this.frame)) + base.location;
				pMan.AddElectricBolt(loc, -1, 0.3f, 1000, 100, 0.4f, 6);
				if (!(this.frame < 0f))
				{
					break;
				}
				this.stage++;
				this.frame = 0f;
				base.renderState = RenderState.AdditiveOnly;
				Game1.pManager.AddShockRing(base.location, 1f * this.size, 5);
				VibrationManager.SetBlast(1f * this.size, base.location);
				for (int i = 0; i < 20; i++)
				{
					Vector2 loc2 = base.location + Rand.GetRandomVector2(-100f, 100f, -100f, 100f);
					Game1.pManager.AddElectricBolt(loc2, -1, Rand.GetRandomFloat(0.2f, 0.8f), 1000, 100, 1f, 5);
					Game1.pManager.AddBounceSpark(loc2, Rand.GetRandomVector2(-800f, 800f, -500f, 10f), 0.5f, 6);
				}
				if (Game1.events.anyEvent)
				{
					break;
				}
				for (int j = 0; j < c.Length; j++)
				{
					if (j != base.owner && c[j].NPC == NPCType.None && (c[j].Location - base.location).Length() < 200f * this.size)
					{
						if (j > 0)
						{
							c[j].Ethereal = EtherealState.Normal;
						}
						else if (!c[0].AnimName.StartsWith("hurt") && c[0].CanHurtFrame < 3f)
						{
							c[0].Ethereal = EtherealState.Normal;
						}
						c[j].Face = ((base.trajectory.X < 0f) ? CharDir.Right : CharDir.Left);
						HitManager.CheckWallHazard(c, j, pMan, base.strength, ColType.Hazard);
						if (this.size == 1f)
						{
							c[j].Slide(-1500f);
						}
					}
				}
				break;
			}
			case 1:
				this.frame += gameTime * 6f;
				if (this.frame > 1f)
				{
					base.Reset();
				}
				break;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			switch (this.stage)
			{
			case 0:
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(1640, 2060, 50, 50), Color.White * (4f - this.frame * 4f), 0f, new Vector2(25f, 25f), this.size * 20f * this.frame * worldScale, SpriteEffects.None, 1f);
				break;
			case 1:
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(4000, 2796, 96, 96), Color.White * (4f - this.frame * 4f), 0f, new Vector2(48f, 48f), this.size * 10f * this.frame * worldScale, SpriteEffects.None, 1f);
				break;
			}
		}
	}
}
