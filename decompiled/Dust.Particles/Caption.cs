using System;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Caption : Particle
	{
		private float frame;

		private float size;

		private float maxSize;

		private string text;

		private Color color;

		public Caption(Vector2 loc, string text, float size, Color color, float lifeSpan)
		{
			this.Reset(loc, text, size, color, lifeSpan);
		}

		public void Reset(Vector2 loc, string _text, float _maxSize, Color _color, float lifeSpan)
		{
			base.exists = Exists.Init;
			base.location = loc;
			this.text = _text;
			this.size = 0.1f;
			this.maxSize = _maxSize;
			this.color = _color;
			this.frame = lifeSpan;
			base.renderState = RenderState.Normal;
			base.exists = Exists.Exists;
		}

		public override bool InitAction(int actionType)
		{
			base.Reset();
			return true;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.size < this.maxSize)
			{
				this.size = Math.Min(this.size + Game1.HudTime * (this.maxSize - this.size) * 20f, this.maxSize);
				if (Game1.stats.playerLifeState > 0)
				{
					this.frame = 0f;
				}
			}
			if (gameTime == 0f || Game1.events.anyEvent)
			{
				this.frame = 0f;
			}
			this.frame -= gameTime;
			if (this.frame <= 0f)
			{
				base.Reset();
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (!Game1.hud.isPaused && Game1.hud.inventoryState == InventoryState.None && Game1.menu.prompt == promptDialogue.None && Game1.stats.playerLifeState == 0)
			{
				Game1.bigText.Color = new Color((float)(int)this.color.R / 255f, (float)(int)this.color.G / 255f, (float)(int)this.color.B / 255f, this.frame * 6f);
				Game1.bigText.DrawShadowText(base.location - new Vector2(400f, (0f - this.frame + 2.5f) * 20f), this.text, this.size, 800, TextAlign.Center, outline: true);
			}
		}
	}
}
