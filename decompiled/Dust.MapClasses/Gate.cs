using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.Vibration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.MapClasses
{
	public class Gate
	{
		private int Id;

		public Vector2 location;

		public int lockType;

		private float openAmount;

		public int openStage;

		private float openTime;

		private float shakeTime;

		private float glowFrame;

		public Gate(int _Id, Vector2 loc, int _lockType)
		{
			this.Id = _Id;
			this.location = loc;
			this.lockType = Math.Min(_lockType, 12);
		}

		private Color GetColor()
		{
			Color color = Color.White;
			switch (this.lockType)
			{
			case 0:
				color = new Color(0.5f, 0.5f, 1f, 1f);
				break;
			case 1:
				color = new Color(1f, 0.5f, 0.5f, 1f);
				break;
			case 2:
				color = new Color(0.2f, 1f, 0.2f, 1f);
				break;
			case 3:
				color = new Color(1f, 1f, 0f, 1f);
				break;
			case 4:
				color = new Color(1f, 1f, 1f, 1f);
				break;
			case 5:
				color = new Color(1f, 0f, 1f, 1f);
				break;
			case 6:
				color = new Color(0.2f, 1f, 1f, 1f);
				break;
			case 7:
			{
				float num = Game1.stats.gameClock * 4f;
				color = new Color(Math.Abs((float)Math.Sin(num)), Math.Abs((float)Math.Sin(num + 1f)), Math.Abs((float)Math.Sin(num + 2f)), 1f);
				break;
			}
			}
			return color * this.glowFrame * Rand.GetRandomFloat(0.8f, 1.2f);
		}

		public void Update(float gameTime, Map map, DestructableManager dMan, Character[] c)
		{
			int val = this.openStage;
			Sound.SetInteractiveStage(this.location, "gate_hum", Math.Min(val, 2));
			if (this.openStage == 0)
			{
				this.glowFrame = Math.Max(this.glowFrame - gameTime * 2f, 0f);
				if (new Rectangle((int)this.location.X - 380, (int)this.location.Y - 300, 760, 450).Contains((int)c[0].Location.X, (int)c[0].Location.Y))
				{
					this.openStage++;
					this.openTime = 0.8f;
				}
			}
			else
			{
				this.glowFrame = Math.Min(this.glowFrame + gameTime, 1f);
				if (this.openStage == 1)
				{
					if (Game1.stats.Equipment[this.lockType + 321] > 0)
					{
						if (this.openTime < 0.4f)
						{
							this.shakeTime = 0.15f;
						}
						if (this.openTime == 0f)
						{
							Sound.PlayCue("gate_open", this.location, (this.location - c[0].Location).Length() / 2f);
							VibrationManager.SetScreenShake(0.5f);
							for (int i = 0; i < 3; i++)
							{
								Game1.pManager.AddGroundDust(this.location + new Vector2(0f, -180 + 100 * i), Vector2.Zero, 0.5f, 0.5f, 0, 5);
							}
							this.openStage++;
						}
					}
					else
					{
						if (!new Rectangle((int)this.location.X - 380, (int)this.location.Y - 300, 760, 450).Contains((int)c[0].Location.X, (int)c[0].Location.Y))
						{
							this.openStage = 0;
						}
						Game1.events.InitEvent(44, isSideEvent: true);
					}
				}
				else if (this.openStage == 2)
				{
					this.shakeTime = 0.1f;
					this.openAmount += (340f - this.openAmount) * gameTime * 10f;
					if (this.openAmount > 339f)
					{
						this.openStage++;
					}
					else if (this.openAmount < 320f)
					{
						Game1.pManager.AddBlood(this.location + new Vector2(Rand.GetRandomFloat(-200f, 200f), -200f - this.openAmount), Rand.GetRandomVector2(-200f, 200f, -200f, 0f), 1f, 1f, 1f, 1f, 0.3f, (CharacterType)1000, 0, 5);
					}
				}
				else if (this.openStage == 3)
				{
					if (Game1.longSkipFrame == 1)
					{
						bool flag = true;
						for (int j = 0; j < c.Length; j++)
						{
							if (c[j].Exists == CharExists.Exists && new Rectangle((int)this.location.X - 400, (int)this.location.Y - 820, 800, 990).Contains((int)c[j].Location.X, (int)c[j].Location.Y))
							{
								flag = false;
							}
						}
						if (flag)
						{
							this.openTime = 1f;
							Sound.PlayCue("gate_close", this.location, (this.location - c[0].Location).Length() / 2f);
							this.openStage++;
						}
						else if (Rand.GetRandomInt(0, 10) == 0)
						{
							this.shakeTime = 0.1f;
						}
					}
				}
				else if (this.openStage == 4)
				{
					this.shakeTime = 0.2f;
					this.openAmount -= 80f * (1f - this.openTime);
					if (this.openAmount < 0f)
					{
						for (int k = 0; k < 16; k++)
						{
							Game1.pManager.AddBlood(this.location + Rand.GetRandomVector2(-200f, 200f, -260f, -220f), new Vector2(Rand.GetRandomFloat(-500f, 500f), Rand.GetRandomFloat(-400f, -100f)), 1f, 1f, 1f, 1f, 0.3f, (CharacterType)1000, 0, 5);
						}
						VibrationManager.SetScreenShake(1f);
						Sound.PlayCue("gate_close_slam", this.location, (this.location - c[0].Location).Length() / 4f);
						this.openAmount = 0f;
						this.openStage++;
						this.openAmount = 0f;
					}
				}
				else if (this.openStage == 5)
				{
					this.shakeTime = 0.2f;
					this.openAmount += (20f - this.openAmount) * gameTime * 20f;
					if (this.openAmount > 19f)
					{
						this.openStage++;
						this.openTime = 1f;
					}
				}
				else if (this.openStage == 6)
				{
					this.openAmount -= 20f * (1f - this.openTime);
					if (this.openAmount < 0f)
					{
						this.openAmount = 0f;
						this.openStage = 0;
					}
				}
				this.openTime = Math.Max(this.openTime - gameTime, 0f);
			}
			if (this.shakeTime > 0f)
			{
				this.shakeTime = Math.Max(this.shakeTime - gameTime, 0f);
				VibrationManager.Rumble(Game1.currentGamePad, this.shakeTime);
			}
			if (this.glowFrame > 0f && Game1.settings.ColorBlind && Game1.hud.fidgetPrompt < FidgetPrompt.GateBlue)
			{
				Game1.hud.InitFidgetPrompt((FidgetPrompt)(32 + this.lockType));
			}
		}

		public void Draw(SpriteBatch sprite, Texture2D[] particlesTex)
		{
			int num = (int)(this.openAmount * 1.3f);
			Vector2 vector = this.location * Game1.worldScale - Game1.Scroll;
			Vector2 vector2 = vector;
			if (this.shakeTime > 0f)
			{
				vector += Rand.GetRandomVector2(-80f, 80f, -50f, 50f) * this.shakeTime * Game1.FrameTime * 20f;
			}
			sprite.Draw(particlesTex[2], vector2 + new Vector2(-221f, -9f) * Game1.worldScale, new Rectangle(546, 3870, 418, 90), Color.White, 0f, Vector2.Zero, Game1.worldScale, SpriteEffects.None, 0f);
			sprite.Draw(particlesTex[2], vector + new Vector2(0f, 470f + this.openAmount) * Game1.worldScale, new Rectangle(400, 2970, 400, (int)Math.Min(410f - this.openAmount, 410f)), Color.White, 0f, new Vector2(200f, 790f), Game1.worldScale, SpriteEffects.None, 0f);
			sprite.Draw(particlesTex[2], vector, new Rectangle(0, 2970 + num, 400, Math.Min(760 - num, 760)), Color.White, 0f, new Vector2(200f, 790f), Game1.worldScale, SpriteEffects.None, 0f);
			sprite.Draw(particlesTex[2], vector + new Vector2(62f, 490f + this.openAmount) * Game1.worldScale, new Rectangle(400, 3380, 274, (int)Math.Min(410f - this.openAmount, 180f)), Color.White, 0f, new Vector2(200f, 790f), Game1.worldScale, SpriteEffects.None, 0f);
			int num2 = Math.Max(3730, 3290 + num);
			sprite.Draw(particlesTex[2], vector + new Vector2(48f, 440 - num + (num2 - 3730)) * Game1.worldScale, new Rectangle(0, num2, 291, 366), Color.White, 0f, new Vector2(200f, 790f), Game1.worldScale, SpriteEffects.None, 0f);
			sprite.Draw(particlesTex[2], vector2 + new Vector2(-346f, 0f) * Game1.worldScale, new Rectangle(292, 3960, 672, 136), Color.White, 0f, Vector2.Zero, Game1.worldScale, SpriteEffects.None, 0f);
			if (this.glowFrame > -1f)
			{
				sprite.End();
				sprite.Begin(SpriteSortMode.Deferred, BlendState.Additive);
				Color color = this.GetColor();
				sprite.Draw(particlesTex[2], vector + new Vector2(126f, 551 - num) * Game1.worldScale, new Rectangle(400, 3686, 146, 274), color, 0f, new Vector2(200f, 790f), Game1.worldScale, SpriteEffects.None, 0f);
				sprite.Draw(particlesTex[2], vector + new Vector2(-23f, 675 - num) * Game1.worldScale, new Rectangle(400, 3570, 96, 116), color, 0f, new Vector2(200f, 790f), Game1.worldScale, SpriteEffects.None, 0f);
				sprite.Draw(particlesTex[2], vector + new Vector2(324f, 675 - num) * Game1.worldScale, new Rectangle(496, 3570, 98, 116), color, 0f, new Vector2(200f, 790f), Game1.worldScale, SpriteEffects.None, 0f);
				int num3 = Math.Max(3730, 3434 + num);
				sprite.Draw(particlesTex[2], vector + new Vector2(17f, 296 - num + (num3 - 3730)) * Game1.worldScale, new Rectangle(292, num3, 108, 114 - (num3 - 3730)), color, 0f, new Vector2(200f, 790f), Game1.worldScale, SpriteEffects.None, 0f);
				int num4 = Math.Max(3844, 3550 + num);
				sprite.Draw(particlesTex[2], vector + new Vector2(275f, 294 - num + (num4 - 3844)) * Game1.worldScale, new Rectangle(292, num4, 108, 116 - (num4 - 3844)), color, 0f, new Vector2(200f, 790f), Game1.worldScale, SpriteEffects.None, 0f);
				sprite.End();
				sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			}
		}
	}
}
