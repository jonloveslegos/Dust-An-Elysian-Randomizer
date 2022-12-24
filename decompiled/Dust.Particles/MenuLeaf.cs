using System;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class MenuLeaf : Particle
	{
		private byte animFrame;

		private float animFrameTime;

		private float lifeSpan;

		private float frame;

		private float size;

		private float rotation;

		public MenuLeaf(Vector2 loc, Vector2 traj, float rot, float size)
		{
			this.Reset(loc, traj, rot, size);
		}

		public void Reset(Vector2 loc, Vector2 traj, float rot, float size)
		{
			base.exists = Exists.Init;
			this.lifeSpan = 6f * Rand.GetRandomFloat(0.75f, 1.25f);
			this.animFrame = 0;
			this.animFrameTime = 0f;
			base.location = loc;
			base.trajectory = traj;
			this.rotation = rot;
			this.size = size;
			this.frame = this.lifeSpan;
			base.renderState = RenderState.Additive;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			Vector2 vector = base.location;
			if (Game1.navManager.worldMap != null)
			{
				vector = base.location + Game1.navManager.worldMap.WorldMapPos(background: false);
			}
			if (!new Rectangle(-50, -200, Game1.screenWidth + 100, Game1.screenHeight + 220).Contains((int)vector.X, (int)vector.Y) || this.frame < 0f || (Game1.gameMode == Game1.GameModes.Game && Game1.hud.inventoryState == InventoryState.None))
			{
				base.Reset();
			}
			this.animFrameTime += Game1.HudTime * 24f;
			if (this.animFrameTime > 1f)
			{
				this.animFrame++;
				if (this.animFrame > 47)
				{
					this.animFrame = 0;
				}
				this.animFrameTime = 0f;
			}
			base.trajectory.X = (float)Math.Cos((double)map.MapSegFrame + (double)this.frame) * 100f;
			base.location += base.trajectory * Game1.HudTime;
			this.frame -= Game1.HudTime;
			if (Game1.menu.menuMode == MenuMode.Loading || Game1.menu.menuMode == MenuMode.Quitting)
			{
				this.frame /= 1.1f;
				base.trajectory.X *= 1.2f;
			}
			this.rotation = 2f * GlobalFunctions.GetAngle(Vector2.Zero, base.trajectory) - 135f;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			float num = (float)Math.Sin(6.28 * (double)(this.frame / 2f / this.lifeSpan));
			Vector2 position = base.location;
			if (Game1.navManager.worldMap != null)
			{
				position = base.location + Game1.navManager.worldMap.WorldMapPos(background: false);
			}
			sprite.Draw(particlesTex[2], position, new Rectangle(1740 + this.animFrame * 32, 2011, 32, 31), new Color(1f, 1f, 1f, num * 2f), this.rotation, new Vector2(16f, 16f), this.size, SpriteEffects.None, 1f);
		}
	}
}
