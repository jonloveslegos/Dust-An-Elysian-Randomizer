using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Dust.Strings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.Particles
{
	internal class HP : Particle
	{
		private StatusEffects statusEffect;

		private bool stopped;

		private bool critical;

		private float frame;

		private float size;

		private int startPos;

		public HP(Vector2 loc, int amount, bool _critical, StatusEffects _statusEffect)
		{
			this.Reset(loc, amount, _critical, _statusEffect);
		}

		public void Reset(Vector2 loc, int amount, bool _critical, StatusEffects _statusEffect)
		{
			base.exists = Exists.Init;
			base.location = loc;
			this.startPos = (int)loc.Y - 200;
			this.stopped = false;
			base.trajectory = new Vector2(Rand.GetRandomFloat(-100f, 100f), -1000f);
			this.size = MathHelper.Clamp(Game1.hud.hudScale, 1f, 20f);
			base.flag = amount;
			this.critical = _critical;
			this.statusEffect = _statusEffect;
			if (this.critical || this.statusEffect != 0)
			{
				this.frame = 4f;
			}
			else
			{
				this.frame = 1f;
			}
			base.renderState = RenderState.Normal;
			base.exists = Exists.Exists;
		}

		public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c, int l)
		{
			if (map.transOutFrame > 0.5f)
			{
				base.Reset();
			}
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
			base.trajectory.X *= 0.999f;
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
			float num = this.frame * Game1.hud.hideHudDetails;
			if (base.flag < 0)
			{
				Game1.smallText.Color = new Color(1f, 1f, 0f, num);
			}
			else if (base.flag > 0)
			{
				Game1.smallText.Color = new Color(0f, 1f, 0f, num);
			}
			if (base.flag != 0)
			{
				Game1.hud.scoreDraw.Draw(base.flag, vector, 0.8f * this.size, Game1.smallText.Color, ScoreDraw.Justify.Left, 0);
			}
			if (this.critical)
			{
				Game1.smallText.Color = this.GetTextColor(num);
				Game1.smallText.DrawOutlineText(vector - new Vector2(90f, 25f) * this.size, Strings_Hud.Status_Critical, 0.8f * this.size, 200, TextAlign.Center, fullOutline: false);
				return;
			}
			switch (this.statusEffect)
			{
			case StatusEffects.Poison:
				Game1.smallText.Color = this.GetTextColor(num);
				Game1.smallText.DrawOutlineText(vector - new Vector2(90f, 25f) * this.size, Strings_Hud.Status_Poison, 0.8f * this.size, 200, TextAlign.Center, fullOutline: true);
				break;
			case StatusEffects.Burning:
				Game1.smallText.Color = this.GetTextColor(num);
				Game1.smallText.DrawOutlineText(vector - new Vector2(90f, 25f) * this.size, Strings_Hud.Status_Burned, 0.8f * this.size, 200, TextAlign.Center, fullOutline: true);
				break;
			case StatusEffects.Silent:
				Game1.smallText.Color = this.GetTextColor(num);
				Game1.smallText.DrawOutlineText(vector - new Vector2(90f, 25f) * this.size, Strings_Hud.Status_Silenced, 0.8f * this.size, 200, TextAlign.Center, fullOutline: true);
				break;
			}
		}

		private Color GetTextColor(float alpha)
		{
			if (Game1.longSkipFrame == 3)
			{
				return new Color(1f, 0.2f, 0f, alpha);
			}
			return new Color(1f, 1f, 1f, alpha);
		}
	}
}
