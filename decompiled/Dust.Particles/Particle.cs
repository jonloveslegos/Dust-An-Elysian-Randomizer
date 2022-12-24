using Dust.CharClasses;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	public class Particle
	{
		public RenderState renderState;

		public StatusEffects statusType;

		public Vector2 location;

		public Vector2 trajectory;

		public float isSpun;

		public float maskGlow;

		public int flag = -1;

		public int owner;

		public int strength = 1;

		public Exists exists;

		public bool background;

		public Vector2 GameLocation(int l)
		{
			float num = 1f;
			switch (l)
			{
			case 0:
				num = 0.2f;
				break;
			case 1:
				num = 0.3f;
				break;
			case 2:
				return this.location + Game1.wManager.mapBackPos;
			case 3:
				num = 0.85f;
				break;
			case 4:
				num = ((Game1.map.backScroll != 1) ? 0.9f : 0.85f);
				break;
			case 7:
				num = 1.1f;
				break;
			case 8:
				num = 1.25f;
				break;
			case 9:
				num = 1f;
				return this.location;
			case 10:
				num = 1.5f;
				break;
			case 11:
				num = 1.75f;
				break;
			}
			if (num == 1f)
			{
				return this.location * Game1.worldScale - Game1.Scroll;
			}
			return this.location * Game1.worldScale - Game1.Scroll * num + new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f * (1f - num) * (Game1.hiDefScaleOffset - Game1.worldScale) / Game1.hiDefScaleOffset;
		}

		public Vector2 GameLocation(int l, Vector2 srcLoc)
		{
			float num = 1f;
			switch (l)
			{
			case 0:
				num = 0.2f;
				break;
			case 1:
				num = 0.3f;
				break;
			case 2:
				return srcLoc + Game1.wManager.mapBackPos;
			case 3:
				num = 0.85f;
				break;
			case 4:
				num = ((Game1.map.backScroll != 1) ? 0.9f : 0.85f);
				break;
			case 7:
				num = 1.1f;
				break;
			case 8:
				num = 1.25f;
				break;
			case 9:
				num = 1f;
				return srcLoc;
			case 10:
				num = 1.5f;
				break;
			case 11:
				num = 1.75f;
				break;
			}
			if (num == 1f)
			{
				return srcLoc * Game1.worldScale - Game1.Scroll;
			}
			return srcLoc * Game1.worldScale - Game1.Scroll * num + new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f * (1f - num) * (Game1.hiDefScaleOffset - Game1.worldScale) / Game1.hiDefScaleOffset;
		}

		public Vector2 PlayerLayerLoc(Vector2 locSrc, int l)
		{
			float num = this.LayerScale(l);
			Vector2 vector = locSrc * Game1.worldScale - Game1.Scroll * num + new Vector2(Game1.screenWidth / 2, Game1.screenHeight / 2) * (1f - num) * (1f - Game1.worldScale);
			return (vector + Game1.Scroll) / Game1.worldScale;
		}

		public float LayerScale(int l)
		{
			switch (l)
			{
				case 0: return 0.2f;
				case 1: return 0.3f;
				case 2: return 0.5f;
				case 3: return 0.85f;
				case 4: return 0.9f;
				case 7: return 1.1f;
				case 8: return 1.25f;
				case 10: return 1.5f;
				case 11: return 1.75f;
			}
			return 1f;
		}

		public void Reset()
		{
			if (this.exists != Exists.Init)
			{
				this.exists = Exists.Dead;
				this.flag = -1;
				this.owner = 0;
				this.maskGlow = 0f;
				this.isSpun = 0f;
				this.statusType = StatusEffects.Normal;
				this.renderState = RenderState.Normal;
				this.background = false;
			}
		}

		public bool CanPrecipitate(Vector2 loc, int l)
		{
			int count = Game1.map.noPrecipRegion.Count;
			if (count > 0)
			{
				loc /= this.LayerScale(l);
				for (int i = 0; i < Game1.map.noPrecipRegion.Count; i++)
				{
					_ = Game1.map.noPrecipRegion[i];
					if (Game1.map.noPrecipRegion[i].Contains((int)loc.X, (int)loc.Y))
					{
						return false;
					}
				}
			}
			return true;
		}

		public virtual void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
		}

		public virtual void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
		}

		public virtual void GetInfo(ref int intVar, ref float floatVar)
		{
		}

		public virtual void GetAudio(ref Vector2 loc, ref Vector2 traj, ref bool _exists)
		{
			loc = this.location;
			traj = this.trajectory;
			_exists = this.exists != Exists.Dead;
		}

		public virtual bool InitAction(int actionType)
		{
			return false;
		}

		public virtual void Relocate(Vector2 loc)
		{
			this.location = loc;
		}
	}
}
