using Dust.Audio;
using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Blood : Particle
	{
		private CharacterType bloodType;

		private int animXOffset;

		private byte frameRate;

		private byte animFrame;

		private Rectangle sRect;

		private SpriteEffects flip;

		private float frame;

		private float r;

		private float g;

		private float b;

		private float a;

		private float size;

		private float sizeY;

		private float rotation;

		private Vector2 ploc = Vector2.Zero;

		public Blood(Vector2 loc, Vector2 traj, float r, float g, float b, float a, float size, CharacterType bloodType, int skin)
		{
			this.Reset(loc, traj, r, g, b, a, size, bloodType, skin);
		}

		public void Reset(Vector2 loc, Vector2 traj, float _r, float _g, float _b, float _a, float _size, CharacterType _bloodType, int skin)
		{
			base.exists = Exists.Init;
			this.animXOffset = 0;
			this.frameRate = (byte)Rand.GetRandomInt(24, 36);
			this.animFrame = 0;
			this.frame = 0f;
			this.sizeY = 0f;
			this.flip = SpriteEffects.None;
			base.location = (this.ploc = loc);
			base.trajectory = traj;
			this.bloodType = _bloodType;
			this.a = _a;
			if (_bloodType >= (CharacterType)1000)
			{
				if (Game1.map.CheckCol(base.location) > 0)
				{
					base.exists = Exists.Dead;
					base.Reset();
					return;
				}
				base.trajectory.Y *= 2f;
				this.rotation = Rand.GetRandomFloat(0f, 6.28f);
				base.renderState = RenderState.Normal;
				this.r = _r;
				this.g = _g;
				this.b = _b;
				if (_bloodType >= (CharacterType)1010)
				{
					this.size = (this.sizeY = Rand.GetRandomFloat(0.2f, 1f) * _size);
					this.sRect = new Rectangle((int)(_bloodType - 1010) * 64 + Rand.GetRandomInt(0, 2) * 32 + 2368, 2724, 32, 32);
				}
				else
				{
					this.size = Rand.GetRandomFloat(0.1f, 1f) * _size;
					this.sizeY = Rand.GetRandomFloat(0.2f, 1f) * this.size;
					this.sRect = new Rectangle((int)(_bloodType - 1000) * 192 + Rand.GetRandomInt(0, 3) * 64 + 1984, 2660, 64, 64);
				}
			}
			else
			{
				switch (_bloodType)
				{
				case CharacterType.Slime:
					this.r = _r / 255f;
					this.g = _g / 255f;
					this.b = _b / 255f;
					this.rotation = Rand.GetRandomFloat(0f, 6.28f);
					this.size = Rand.GetRandomFloat(0.4f, 1f) * _size;
					this.sizeY = this.size * 2f;
					base.renderState = RenderState.Refract;
					this.sRect = new Rectangle(Rand.GetRandomInt(0, 3) * 64 + 2620, 2596, 64, 64);
					break;
				case CharacterType.Giant:
				case CharacterType.Trolk:
					base.trajectory.X /= Rand.GetRandomInt(1, 3);
					this.rotation = Rand.GetRandomFloat(0f, 6.28f);
					this.size = Rand.GetRandomFloat(0.2f, 0.6f) * _size;
					this.sizeY = this.size;
					this.r = _r;
					this.g = _g;
					this.b = _b;
					base.renderState = RenderState.Normal;
					switch (_bloodType)
					{
					case CharacterType.Giant:
						this.sRect = new Rectangle(Rand.GetRandomInt(0, 3) * 64 + skin * 192 + 1984, 2596, 64, 64);
						break;
					case CharacterType.Trolk:
						this.sRect = new Rectangle(Rand.GetRandomInt(0, 3) * 64 + 1984, 2596, 64, 64);
						break;
					}
					break;
				default:
				{
					this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) - 1.57f;
					this.size = _size * Rand.GetRandomFloat(0.5f, 1.5f);
					base.renderState = RenderState.AdditiveOnly;
					this.r = _r * Rand.GetRandomFloat(0.25f, 1f);
					this.g = _g * Rand.GetRandomFloat(0.25f, 1f);
					this.b = _b * Rand.GetRandomFloat(0.25f, 1f);
					this.sRect = new Rectangle(0, 0, 145, 300);
					int randomInt = Rand.GetRandomInt(0, 4);
					switch (randomInt)
					{
					case 1:
						this.animXOffset = 1885;
						break;
					case 2:
						this.sRect.Y = 300;
						break;
					case 3:
						this.animXOffset = 1885;
						this.sRect.Y = 300;
						break;
					}
					if (Rand.GetRandomInt(0, 3) == 0 && randomInt > 1)
					{
						base.renderState = RenderState.Refract;
					}
					if (Rand.GetRandomInt(0, 2) == 0)
					{
						this.flip = SpriteEffects.FlipHorizontally;
					}
					break;
				}
				}
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.animFrame < 0)
			{
				this.KillBlood(map, pMan);
				return;
			}
			if (this.bloodType == CharacterType.Slime)
			{
				if (map.CheckPCol(base.location, this.ploc, canFallThrough: false, init: false) > 0f || this.a < 0f)
				{
					this.KillBlood(map, pMan);
				}
				base.trajectory.Y += gameTime * 2000f;
				if (this.sizeY > this.size)
				{
					this.sizeY -= gameTime * 4f;
				}
				this.a -= gameTime;
				this.ploc = base.location;
				base.location += base.trajectory * gameTime;
				this.rotation += base.trajectory.X * gameTime * 0.01f;
				return;
			}
			if (this.bloodType == CharacterType.Giant || this.bloodType == CharacterType.Trolk || this.bloodType >= (CharacterType)1000)
			{
				base.trajectory.Y += gameTime * 2000f;
				if (base.trajectory.X > 0f)
				{
					this.rotation += gameTime * 8f;
				}
				else
				{
					this.rotation -= gameTime * 8f;
				}
				if (this.bloodType >= (CharacterType)1000 && map.CheckCol(base.location) > 0)
				{
					base.location.X = this.ploc.X;
					base.trajectory.X = 0f - base.trajectory.X;
				}
				Vector2 vector = base.GameLocation(l);
				if (!new Rectangle(-100, 0, Game1.screenWidth + 200, Game1.screenHeight + 40).Contains((int)vector.X, (int)vector.Y))
				{
					this.KillBlood(map, pMan);
				}
				if (map.CheckPCol(base.location, this.ploc, canFallThrough: false, init: false) > 0f)
				{
					if (this.bloodType >= (CharacterType)1010)
					{
						this.KillBlood(map, pMan);
					}
					else if (this.bloodType >= (CharacterType)1000 && this.size > 0.5f)
					{
						if (Rand.GetRandomInt(0, 4) == 0)
						{
							Sound.PlayDropCue("destruct_debris", base.location, this.size * 200f);
						}
						float num = base.trajectory.Y * -0.5f;
						base.trajectory.Y = num;
						this.size /= 2f;
						this.sizeY /= 2f;
						pMan.AddBlood(base.location - new Vector2(0f, 20f), new Vector2(Rand.GetRandomFloat(0f - base.trajectory.X, base.trajectory.X), num / 1.5f), 1f, 1f, 1f, 1f, this.size, this.bloodType, 0, 5);
					}
					else
					{
						this.KillBlood(map, pMan);
						if (Rand.GetRandomInt(0, 2) == 0)
						{
							pMan.AddFootStep(base.location, 1f, 0.5f, 5);
						}
					}
				}
				this.ploc = base.location;
				base.location += base.trajectory * gameTime;
				return;
			}
			this.frame += gameTime * (float)(int)this.frameRate;
			if (this.frame > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 12)
				{
					this.animFrame = 12;
					this.KillBlood(map, pMan);
				}
				this.frame = 0f;
			}
		}

		private void KillBlood(Map map, ParticleManager pMan)
		{
			if (map.bloodCount > 0)
			{
				map.bloodCount--;
			}
			base.Reset();
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (this.bloodType == CharacterType.Giant || this.bloodType == CharacterType.Slime || this.bloodType == CharacterType.Trolk || this.bloodType >= (CharacterType)1000)
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, new Color(new Vector4(this.r, this.g, this.b, this.a)), this.rotation, new Vector2(this.sRect.Width, this.sRect.Height) / 2f, new Vector2(this.size, this.sizeY) * worldScale, SpriteEffects.None, 1f);
				return;
			}
			this.sRect.X = 145 * this.animFrame + this.animXOffset;
			if (!Game1.refractive)
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, new Color(new Vector4(this.r, this.g, this.b, this.a / 4f)), this.rotation, new Vector2(72.5f, 280f), new Vector2(this.size, this.size * 1.5f) * worldScale, this.flip, 1f);
			}
			else
			{
				sprite.Draw(particlesTex[2], base.GameLocation(l), this.sRect, new Color(new Vector4(1f, 1f, 1f, this.a / 4f)), this.rotation, new Vector2(72.5f, 280f), new Vector2(this.size * 1.2f, this.size * 1.8f) * worldScale, this.flip, 1f);
			}
		}
	}
}
