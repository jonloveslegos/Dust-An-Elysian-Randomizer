using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class CannonFireBall : Particle
	{
		private byte animFrame;

		private float animFrameTime;

		private float size;

		private float rotation;

		private Vector2 ploc = Vector2.Zero;

		private Vector2 followLoc = Vector2.Zero;

		public CannonFireBall(Vector2 loc, Vector2 traj, float size, int strength, int _owner)
		{
			this.Reset(loc, traj, size, strength, _owner);
		}

		public void Reset(Vector2 loc, Vector2 traj, float _size, int _strength, int _owner)
		{
			base.exists = Exists.Init;
			this.animFrame = (byte)Rand.GetRandomInt(0, 20);
			this.animFrameTime = 0f;
			base.trajectory = traj;
			base.owner = _owner;
			base.location = (this.ploc = (this.followLoc = loc));
			this.size = _size;
			base.strength = _strength;
			this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) + 1.57f;
			base.exists = Exists.Exists;
		}

		private bool CheckCreatures(Character[] c, Map map, Vector2 gameLoc, int l)
		{
			int num = (int)(100f * this.size);
			Rectangle bounds = new Rectangle((int)base.location.X - num, (int)base.location.Y - num, num * 2, num * 2);
			for (int i = 0; i < c.Length; i++)
			{
				if (c[i].Exists == CharExists.Exists && c[i].Ethereal != EtherealState.Ethereal && c[i].DyingFrame == -1f && c[i].InRectBounds(bounds))
				{
					return true;
				}
			}
			return false;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.GameLocation(l);
			if (!new Rectangle(-1000, -200, Game1.screenWidth + 2000, Game1.screenHeight + 400).Contains((int)vector.X, (int)vector.Y))
			{
				base.Reset();
			}
			else if (base.trajectory.Y > 0f && (this.CheckCreatures(c, map, base.location, l) || map.CheckPCol(base.location, this.ploc, canFallThrough: false, init: false) > 0f))
			{
				Sound.PlayCue("cannon_close_impact", this.ploc, (c[0].Location - this.ploc).Length() / 4f);
				Game1.pManager.AddShockRing(this.followLoc, 1f * this.size, 5);
				VibrationManager.SetBlast(1f * this.size, this.followLoc);
				for (int i = 0; i < 20; i++)
				{
					pMan.AddExplosion(this.ploc + Rand.GetRandomVector2(-300f, 300f, -300f, 300f) * this.size, Rand.GetRandomFloat(1.5f, 2.5f) * this.size, (Rand.GetRandomInt(0, 5) == 0) ? true : false, Rand.GetRandomInt(5, 7));
				}
				if (!Game1.events.anyEvent)
				{
					for (int j = 0; j < c.Length; j++)
					{
						if (j != base.owner && c[j].NPC == NPCType.None && c[j].Definition.charType != CharacterType.Gaius && (c[j].Location - this.ploc).Length() < 450f * this.size)
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
				}
				base.Reset();
			}
			this.animFrameTime += gameTime * 24f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 23)
				{
					this.animFrame = 0;
				}
				this.animFrameTime = 0f;
			}
			base.location += base.trajectory * gameTime;
			this.followLoc = this.ploc;
			this.ploc = base.location;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			sprite.Draw(particlesTex[2], base.GameLocation(l), new Rectangle(540 + 100 * this.animFrame, 1660, 100, 198), Color.White, this.rotation, new Vector2(50f, 180f), new Vector2(0.8f, 4f) * this.size * worldScale, SpriteEffects.None, 1f);
		}
	}
}
