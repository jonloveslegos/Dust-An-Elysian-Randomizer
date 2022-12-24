using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.MapClasses
{
	public class BrightRegion
	{
		private Rectangle region;

		public Rectangle Region => this.region;

		public BrightRegion(Rectangle _region)
		{
			this.region = _region;
		}

		public void Draw(SpriteBatch sprite, Texture2D particleTex)
		{
			Vector2 position = (new Vector2(this.region.X + this.region.Width / 2, this.region.Y + this.region.Height / 2) * Game1.worldScale - Game1.Scroll) / 4f;
			sprite.Draw(particleTex, position, new Rectangle(4000, 2796, 95, 95), Color.White, 0f, new Vector2(48f, 48f), new Vector2((float)(this.region.Width + 100) / 95f, (float)(this.region.Height + 100) / 95f) * Game1.worldScale / 4f, SpriteEffects.None, 0f);
		}
	}
}
