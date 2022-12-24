using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class XP : Particle
	{
		private bool stopped;

		private int startPos;

		private float alpha;

		private float alpha2;

		private float glowPos;

		private float glowTime;

		private float frame;

		private float chaseTime;

		private float size;

		public XP(Vector2 loc, int amount, bool bonus)
		{
			this.Reset(loc, amount, bonus);
		}

		public void Reset(Vector2 loc, int amount, bool bonus)
		{
			base.exists = Exists.Init;
			base.location = loc;
			this.startPos = (int)loc.Y - 200;
			this.chaseTime = 2f;
			base.trajectory = new Vector2(0f, -1000f);
			this.size = MathHelper.Clamp(Game1.hud.hudScale, 1f, 20f) * 0.8f;
			base.owner = amount;
			this.alpha = 0f;
			this.alpha2 = 0.75f;
			this.stopped = false;
			base.flag = (bonus ? 1 : 0);
			this.frame = ((!bonus) ? 1 : 2);
			base.renderState = RenderState.Normal;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (Game1.events.anyEvent || Game1.hud.hideHudDetails < 1f)
			{
				return;
			}
			if (map.transOutFrame > 0.5f)
			{
				base.Reset();
			}
			if (this.stopped)
			{
				_ = base.location * Game1.worldScale - Game1.Scroll;
				if (this.frame > 0f)
				{
					if (this.stopped)
					{
						this.frame -= gameTime;
					}
					if (this.frame < 0f)
					{
						base.trajectory = (base.location - c[0].Location) * 4f;
					}
				}
				else
				{
					this.chaseTime -= gameTime;
					this.alpha2 -= gameTime * 4f;
					Vector2 vector = c[0].Location - new Vector2(0f, c[0].Height - 80);
					if (this.chaseTime > 1.7f)
					{
						base.trajectory += -(base.location - vector) / 1.5f;
					}
					else
					{
						float num = (base.location - vector).Length();
						base.trajectory = -(base.location - vector) / num * 4000f;
					}
					if (c[0].InHitBounds(base.location) || this.chaseTime < 0f)
					{
						base.Reset();
						if (this.chaseTime > 0f)
						{
							pMan.AddLenseFlare(base.location, 0.4f, 1, 5);
							Sound.PlayCue("xp");
						}
					}
				}
			}
			else
			{
				if (this.alpha < 1f)
				{
					this.alpha += gameTime * 2f;
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
			}
			base.location += base.trajectory * gameTime;
			this.glowPos += gameTime * 400f;
			this.glowTime += gameTime;
			if (this.glowTime > 1f)
			{
				this.glowPos = 0f;
				this.glowTime -= 1f;
			}
		}

		public override void Draw(SpriteBatch sprite, Texture2D[] particlesTex, float worldScale, int l)
		{
			if (Game1.events.anyEvent)
			{
				return;
			}
			Vector2 vector = base.location * Game1.worldScale - Game1.Scroll;
			if (this.alpha2 > 0f)
			{
				Rectangle value = ((base.flag == 0) ? new Rectangle(200, 266, 100, 40) : new Rectangle(300, 260, 164, 40));
				sprite.Draw(Game1.hud.HudTex[0], vector, value, new Color(1f, 1f, 1f, this.alpha2 * this.alpha * Game1.hud.hideHudDetails), 0f, new Vector2(28f, 8f), this.size, SpriteEffects.None, 0f);
				if (this.glowPos < (float)value.Width)
				{
					Rectangle value2 = ((base.flag == 0) ? new Rectangle(200 + (int)this.glowPos, 306, (int)this.glowPos, 40) : new Rectangle(300 + (int)this.glowPos, 300, (int)this.glowPos, 40));
					if (value2.Width > value.Width - (int)this.glowPos)
					{
						value2.Width -= value2.Width - (value.Width - (int)this.glowPos);
					}
					sprite.Draw(Game1.hud.HudTex[0], vector + new Vector2(this.glowPos * this.size, 0f), value2, new Color(1f, 1f, 1f, this.alpha2 * this.alpha * Game1.hud.hideHudDetails), 0f, new Vector2(28f, 8f), this.size, SpriteEffects.None, 0f);
				}
			}
			float num = 1f - this.glowPos / 100f;
			Game1.hud.scoreDraw.Draw(base.owner, vector, this.size, new Color(num * 4f, 1f, 1f, this.alpha * this.chaseTime * Game1.hud.hideHudDetails), ScoreDraw.Justify.Right, 0);
		}
	}
}
