using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class Score : Particle
	{
		private bool stopped;

		private int startPos;

		private float frame;

		public Score(Vector2 loc, int amount)
		{
			this.Reset(loc, amount);
		}

		public void Reset(Vector2 loc, int amount)
		{
			base.exists = Exists.Init;
			if (amount > 999)
			{
				loc.X -= 40f;
			}
			else if (amount > 99)
			{
				loc.X -= 20f;
			}
			else if (amount > 9)
			{
				loc.X -= 10f;
			}
			base.location = loc;
			this.startPos = (int)(loc.Y - 50f);
			this.stopped = false;
			base.trajectory = new Vector2(0f, -600f);
			base.flag = amount;
			this.frame = 4f;
			base.renderState = RenderState.Normal;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (this.stopped)
			{
				this.frame -= gameTime * 4f;
				if (this.frame < 0f)
				{
					base.Reset();
				}
				return;
			}
			base.trajectory.Y += gameTime * 2000f;
			if (base.location.Y > (float)this.startPos)
			{
				if (base.trajectory.Y > 0f && base.trajectory.Y < 100f)
				{
					base.trajectory = Vector2.Zero;
					this.stopped = true;
				}
				else if (base.trajectory.Y > 100f)
				{
					base.trajectory.Y = (0f - base.trajectory.Y) / 3f;
				}
			}
			base.location += base.trajectory * gameTime;
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			Vector2 vector = base.location * Game1.worldScale - Game1.Scroll;
			Color color = new Color(1f, 1f, 1f, this.frame * Game1.hud.hideHudDetails);
			int type;
			if (base.flag < 0)
			{
				sprite.Draw(Game1.hud.NumbersTex, vector + new Vector2(-10f, 20f), new Rectangle(392, 50, 48, 14), color, 0f, new Vector2(24f, 7f), 0.8f, SpriteEffects.None, 0f);
				type = 3;
			}
			else
			{
				sprite.Draw(Game1.hud.NumbersTex, vector + new Vector2(-10f, 20f), new Rectangle(392, 0, 48, 50), color, 0f, new Vector2(24f, 25f), 0.6f, SpriteEffects.None, 0f);
				type = 2;
			}
			Game1.hud.scoreDraw.Draw(base.flag, vector, 0.6f, color, ScoreDraw.Justify.Left, type);
		}
	}
}
