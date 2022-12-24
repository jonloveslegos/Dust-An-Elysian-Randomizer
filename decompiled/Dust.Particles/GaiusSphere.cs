using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class GaiusSphere : Particle
	{
		private Vector2 targLoc;

		private float frame;

		private float size;

		private byte stage;

		public GaiusSphere(Vector2 loc, Vector2 _targLoc, float size, int strength, int _owner)
		{
			this.Reset(loc, _targLoc, size, strength, _owner);
		}

		public void Reset(Vector2 loc, Vector2 _targLoc, float _size, int _strength, int _owner)
		{
			base.exists = Exists.Init;
			base.owner = _owner;
			base.location = loc;
			this.targLoc = _targLoc;
			this.size = _size;
			base.strength = _strength;
			this.stage = 0;
			this.frame = 2f;
			base.maskGlow = 0.8f;
			Sound.PlayCue("gaius_sphere_spawn", base.location, (Game1.character[0].Location - base.location).Length() / 4f);
			base.renderState = RenderState.Refract;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			switch (this.stage)
			{
			case 0:
			{
				base.location += (this.targLoc - base.location) * gameTime * 4f;
				this.frame -= gameTime;
				this.size += gameTime * 0.2f;
				float randomFloat = Rand.GetRandomFloat(0f, 6.28f);
				Vector2 loc = new Vector2((float)(Math.Cos(randomFloat) * 50.0 * (double)this.size), (float)(Math.Sin(randomFloat) * 50.0 * (double)this.size)) + base.location;
				pMan.AddElectricBolt(loc, -1, 0.2f, 1000, 100, 0.4f, 6);
				if (!(this.frame < 0f))
				{
					break;
				}
				this.stage++;
				this.frame = 0f;
				base.renderState = RenderState.AdditiveOnly;
				Sound.PlayCue("gaius_sphere_die", base.location, (c[0].Location - base.location).Length() / 2f);
				Game1.pManager.AddShockRing(base.location, 1f, 5);
				VibrationManager.SetBlast(0.5f, base.location);
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
					if (j != base.owner && c[j].NPC == NPCType.None && (c[j].Location - base.location).Length() < 300f * this.size)
					{
						if (j > 0)
						{
							c[j].KillMe(instantly: false);
						}
						else if (!c[0].AnimName.StartsWith("hurt") && c[0].CanHurtFrame < 3f)
						{
							c[0].Ethereal = EtherealState.Normal;
							c[j].Face = ((c[j].Location.X < base.location.X) ? CharDir.Right : CharDir.Left);
							HitManager.CheckWallHazard(c, j, pMan, base.strength, ColType.Hazard);
							HitManager.StatusHit(c, j, (Rand.GetRandomInt(0, 2) != 0) ? StatusEffects.Poison : StatusEffects.Silent, 0f);
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
				if (!Game1.refractive)
				{
					sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(3750, 2796, 125, 125), Color.White, this.frame * 8f, new Vector2(125f, 125f) / 2f, this.size * 0.8f * worldScale, SpriteEffects.None, 1f);
					break;
				}
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(3875, 2796, 125, 125), Color.White * 0.3f, (0f - this.frame) * 8f, new Vector2(125f, 125f) / 2f, this.size * worldScale, SpriteEffects.None, 1f);
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(3875, 2796, 125, 125), Color.White * 0.3f, this.frame * 4f, new Vector2(125f, 125f) / 2f, this.size * worldScale, SpriteEffects.FlipHorizontally, 1f);
				break;
			case 1:
				sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(4000, 2796, 96, 96), new Color(1f, 0.5f, 1f, 4f - this.frame * 4f), 0f, new Vector2(48f, 48f), this.size * 10f * this.frame * worldScale, SpriteEffects.None, 1f);
				break;
			}
		}
	}
}
