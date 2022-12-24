using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class HudDust : Particle
	{
		private SpriteEffects flip;

		private byte animFrame;

		private byte frameRate;

		private int sRectY;

		private float frame;

		private Color color;

		private float a;

		private float size;

		private float rotation;

		public HudDust(Vector2 loc, Vector2 traj, float size, float rot, Color _color, float alpha, int type)
		{
			this.Reset(loc, traj, size, rot, _color, alpha, type);
		}

		public void Reset(Vector2 loc, Vector2 traj, float size, float rot, Color _color, float alpha, int type)
		{
			base.exists = Exists.Init;
			this.animFrame = 5;
			this.frame = 0f;
			this.frameRate = (byte)Rand.GetRandomInt(20, 28);
			base.flag = type;
			this.size = size * Rand.GetRandomFloat(0.9f, 1.1f);
			base.renderState = RenderState.AdditiveOnly;
			this.color = _color;
			this.a = alpha * Rand.GetRandomFloat(0.9f, 1.1f);
			if (base.flag == 0)
			{
				base.location = loc;
				base.trajectory = traj;
				this.rotation = rot;
				this.sRectY = 600;
				if (Rand.GetRandomInt(0, 2) == 0)
				{
					this.flip = SpriteEffects.FlipHorizontally;
				}
				else
				{
					this.flip = SpriteEffects.None;
				}
			}
			else
			{
				base.background = true;
				base.trajectory = traj;
				if (traj.X > 0f)
				{
					this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) - 3.14f;
					this.flip = SpriteEffects.None;
				}
				else
				{
					this.rotation = GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory);
					this.flip = SpriteEffects.FlipHorizontally;
				}
				base.location = loc + new Vector2(0f, 100f * size);
				this.sRectY = 800;
			}
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			this.frame += Game1.HudTime * (float)(int)this.frameRate;
			if (this.frame > 1f)
			{
				this.frame = 0f;
				this.animFrame++;
				if (this.animFrame > 19)
				{
					this.animFrame = 19;
					base.Reset();
				}
			}
			if (Game1.gameMode == Game1.GameModes.Game && Game1.hud.inventoryState == InventoryState.None && !Game1.events.anyEvent && Game1.hud.unlockState == 0)
			{
				base.Reset();
			}
			base.location += base.trajectory * Game1.HudTime;
			this.size += gameTime / 3f;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (Game1.menu.prompt == promptDialogue.None)
			{
				if (base.flag == 0)
				{
					sprite.Draw(particlesTex[2], base.location, new Rectangle(200 * this.animFrame, this.sRectY, 200, 200), this.color * this.a, this.rotation, new Vector2(100f, 190f), new Vector2(this.size, this.size), this.flip, 1f);
				}
				else
				{
					sprite.Draw(particlesTex[2], base.location, new Rectangle(200 * this.animFrame, this.sRectY, 200, 200), this.color * this.a, this.rotation, new Vector2(100f, 200f), this.size, this.flip, 1f);
				}
			}
		}
	}
}
