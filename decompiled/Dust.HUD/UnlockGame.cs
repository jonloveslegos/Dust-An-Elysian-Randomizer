using System;
using Dust.Audio;
using Dust.CharClasses;
using Dust.Particles;
using Dust.PCClasses;
using Dust.Vibration;
using Lotus.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dust.HUD
{
	internal class UnlockGame
	{
		private int[] unlockSequence;

		private float[] unlockTab;

		private float fadeTime;

		private float timer;

		private float maxTime;

		private float targetRot;

		private float outerRot;

		private float innerRot;

		private float outerSpeed;

		private float innerSpeed;

		private float shakeTime;

		private float fallRot;

		private int returnType;

		private Texture2D unlockTex;

		private bool KeyY;

		private bool KeyCancel;

		private bool KeySelect;

		private bool KeyX;

		private bool KeyUp;

		private bool KeyRight;

		private bool KeyDown;

		private bool KeyLeft;

		public UnlockGame(int lockType, Character[] c, float lockCenter)
		{
			Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadUnlockTextures)));
			this.unlockSequence = new int[((lockType == 0) ? 3 : 5) + Game1.stats.gameDifficulty];
			this.unlockTab = new float[this.unlockSequence.Length];
			for (int i = 0; i < this.unlockSequence.Length; i++)
			{
				this.unlockSequence[i] = Rand.GetRandomInt(1, 5);
				this.unlockTab[i] = 0f;
			}
			this.targetRot = (float)this.unlockSequence[0] * 1.57f - 1.57f;
			this.innerRot = this.targetRot + 2.4f;
			this.outerRot = this.targetRot - 2.4f;
			this.innerSpeed = (this.outerSpeed = 2.4f);
			this.timer = 0f;
			this.maxTime = this.unlockSequence.Length * 2 - Game1.stats.gameDifficulty;
			Sound.PlayCue("unlock_begin");
			Game1.hud.InitHelp("unlocking", restart: true, -1);
			if (c[0].AnimName.StartsWith("attack") && c[0].State == CharState.Grounded)
			{
				c[0].SetAnim("runend", 0, 2);
			}
			c[0].Trajectory.X = 0f;
			CharDir face = c[0].Face;
			if (c[0].Location.X < lockCenter)
			{
				c[0].Face = CharDir.Right;
			}
			else
			{
				c[0].Face = CharDir.Left;
			}
			Game1.camera.camOffset.X = 100 * ((c[0].Face != CharDir.Right) ? 1 : (-1));
			if (c[0].Face != face)
			{
				c[0].SetAnim("idle00", 0, 0);
				c[0].SetAnim("runend", 0, 2);
			}
			this.ClearInput();
		}

		private void LoadUnlockTextures()
		{
			try
			{
				this.unlockTex = Game1.GetInventoryContent().Load<Texture2D>("gfx/ui/unlock_elements");
			}
			catch (Exception)
			{
			}
		}

		public void ExitUnlocking()
		{
			Sound.StopPersistCue("unlock_quick_slide");
			Sound.StopPersistCue("unlock_slow_slide");
			Game1.hud.ClearHelp();
			Game1.character[0].Ethereal = EtherealState.Normal;
			Game1.GetInventoryContent().Unload();
			this.ClearInput();
		}

		public void UpdateInput(HUD hud)
		{
			this.KeyY = hud.KeyY;
			this.KeyCancel = hud.KeyCancel;
			this.KeySelect = hud.KeySelect;
			this.KeyX = hud.KeyX;
			this.KeyUp = hud.KeyUp;
			this.KeyRight = hud.KeyRight;
			this.KeyDown = hud.KeyDown;
			this.KeyLeft = hud.KeyLeft;
		}

		public void ClearInput()
		{
			this.KeyY = (this.KeyCancel = (this.KeySelect = (this.KeyX = false)));
			this.KeyUp = (this.KeyRight = (this.KeyDown = (this.KeyLeft = false)));
		}

		public int Update(ParticleManager pMan, Character[] c, float hudTime, HUD hud)
		{
			Game1.BlurScene(1f);
			hud.LimitInput();
			hud.canInput = false;
			this.shakeTime = Math.Max(this.shakeTime - hudTime, 0f);
			if (hud.unlockState > 2)
			{
				float num = 3f * hudTime;
				if (this.innerRot < this.targetRot)
				{
					this.innerRot += num * this.innerSpeed;
					if (this.innerRot > this.targetRot)
					{
						this.innerRot = this.targetRot;
						Sound.PlayCue("unlock_button_stop");
					}
				}
				else if (this.innerRot > this.targetRot)
				{
					this.innerRot -= num * this.innerSpeed;
					if (this.innerRot < this.targetRot)
					{
						this.innerRot = this.targetRot;
						Sound.PlayCue("unlock_button_stop");
					}
				}
				if (this.outerRot < this.targetRot)
				{
					this.outerRot += num * this.outerSpeed;
					if (this.outerRot > this.targetRot)
					{
						this.outerRot = this.targetRot;
					}
				}
				else if (this.outerRot > this.targetRot)
				{
					this.outerRot -= num * this.outerSpeed;
					if (this.outerRot < this.targetRot)
					{
						this.outerRot = this.targetRot;
					}
				}
			}
			if (hud.unlockState > 4 && this.returnType == 2)
			{
				this.fallRot += this.timer * hudTime;
			}
			switch (hud.unlockState)
			{
			case 1:
				this.fadeTime += hudTime * 2f;
				if (this.fadeTime > 2f)
				{
					hud.unlockState++;
					Sound.PlayPersistCue("unlock_quick_slide", new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f);
				}
				break;
			case 2:
				this.timer += hudTime * this.maxTime * 5f;
				this.shakeTime = 0.2f;
				if (this.timer > this.maxTime)
				{
					Sound.StopPersistCue("unlock_quick_slide");
					Sound.PlayCue("unlock_clamp_open");
					this.timer = this.maxTime;
					hud.unlockState++;
				}
				break;
			case 3:
			{
				if (this.unlockSequence[0] < 0)
				{
					if (this.timer == this.maxTime)
					{
						Sound.PlayPersistCue("unlock_slow_slide", new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f);
					}
					this.timer -= hudTime;
				}
				int num3 = 0;
				for (int j = 0; j < this.unlockSequence.Length; j++)
				{
					if (this.unlockSequence[j] > 0)
					{
						num3 = j;
						break;
					}
				}
				bool flag = false;
				if (Game1.pcManager.inputDevice == InputDevice.GamePad)
				{
					switch (Math.Abs(this.unlockSequence[num3]))
					{
					default:
						if (this.KeyY)
						{
							flag = true;
						}
						break;
					case 2:
						if (this.KeyCancel)
						{
							flag = true;
						}
						break;
					case 3:
						if (this.KeySelect)
						{
							flag = true;
						}
						break;
					case 4:
						if (this.KeyX)
						{
							flag = true;
						}
						break;
					}
				}
				else
				{
					switch (Math.Abs(this.unlockSequence[num3]))
					{
					default:
						if (this.KeyUp)
						{
							flag = true;
						}
						break;
					case 2:
						if (this.KeyRight)
						{
							flag = true;
						}
						break;
					case 3:
						if (this.KeyDown)
						{
							flag = true;
						}
						break;
					case 4:
						if (this.KeyLeft)
						{
							flag = true;
						}
						break;
					}
				}
				if (flag)
				{
					if (num3 < this.unlockSequence.Length - 1)
					{
						this.targetRot = (float)this.unlockSequence[num3 + 1] * 1.57f - 1.57f;
						if (this.targetRot > this.innerRot)
						{
							this.outerRot += 6.28f;
						}
						else
						{
							this.outerRot -= 6.28f;
						}
						if (this.unlockSequence[num3 + 1] == this.unlockSequence[num3])
						{
							int num4 = ((Rand.GetRandomInt(0, 2) != 0) ? 1 : (-1));
							this.innerRot = this.targetRot + 6.28f * (float)num4;
							this.outerRot = this.targetRot - 6.28f * (float)num4;
						}
						else if (this.innerRot - this.targetRot > 3.14f)
						{
							this.innerRot -= 6.28f;
							this.outerRot = this.innerRot + 6.28f;
						}
						else if (this.targetRot - this.innerRot > 3.14f)
						{
							this.innerRot += 6.28f;
							this.outerRot = this.innerRot - 6.28f;
						}
						this.outerSpeed = Math.Abs(this.targetRot - this.outerRot);
						this.innerSpeed = Math.Abs(this.targetRot - this.innerRot);
						this.shakeTime = 0.2f;
						Sound.PlayCue("unlock_button_success");
						Sound.PlayCue("unlock_button_slide");
						VibrationManager.Rumble(Game1.currentGamePad, 0.4f);
					}
					this.unlockSequence[num3] *= -1;
					if (num3 == this.unlockSequence.Length - 1)
					{
						Sound.StopPersistCue("unlock_slow_slide");
						Sound.PlayCue("unlock_success");
						this.returnType = 2;
						hud.unlockState = 5;
					}
					break;
				}
				bool flag2 = ((Game1.pcManager.inputDevice == InputDevice.GamePad) & (this.KeyY || this.KeyCancel || this.KeySelect || this.KeyX)) || (Game1.pcManager.inputDevice != 0 && (this.KeyUp || this.KeyRight || this.KeyDown || this.KeyLeft));
				if (!flag2 && c[0].State != CharState.Air && !(this.timer < this.maxTime * 0.4f))
				{
					break;
				}
				bool flag3 = false;
				if (flag2)
				{
					if (Game1.stats.gameDifficulty > 0)
					{
						flag3 = true;
					}
					Sound.PlayCue("shop_fail");
				}
				else
				{
					flag3 = true;
				}
				if (flag3)
				{
					Sound.StopPersistCue("unlock_slow_slide");
					Sound.PlayPersistCue("unlock_quick_slide", new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f);
					this.returnType = 1;
					hud.unlockState = 4;
				}
				break;
			}
			case 4:
			{
				float num2 = this.timer;
				this.timer -= hudTime * 80f;
				if (this.timer < 0f && num2 >= 0f)
				{
					this.shakeTime = 0.4f;
					Sound.StopPersistCue("unlock_quick_slide");
					Sound.PlayCue("unlock_clamp_shut");
					Vector2 vector = new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f;
					for (int i = 0; i < 20; i++)
					{
						pMan.AddBounceSpark(vector + new Vector2(0f, Rand.GetRandomFloat(-150f, 150f)), Rand.GetRandomVector2(-400f, 400f, -200f, 10f), 0.2f, 9);
					}
				}
				if (this.timer < -6f)
				{
					hud.unlockState = 6;
				}
				break;
			}
			case 5:
				this.timer += (25f - this.timer) * hudTime * 4f;
				if (25f * Math.Max(this.timer, 0f) > 300f)
				{
					hud.unlockState++;
				}
				break;
			case 6:
				if (this.returnType == 2)
				{
					this.timer += (25f - this.timer) * hudTime * 4f;
				}
				this.fadeTime = Math.Min(this.fadeTime - hudTime * 2f, 1f);
				if ((double)this.fadeTime < -0.2)
				{
					this.ExitUnlocking();
					return this.returnType;
				}
				break;
			}
			return 0;
		}

		public void Draw(SpriteBatch sprite, ParticleManager pMan, Texture2D nullTex)
		{
			float num = Math.Min(this.fadeTime, 1f);
			if (num < 0.05f)
			{
				return;
			}
			sprite.Draw(nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.Black * 0.75f * num);
			Color color = new Color(1f, 1f, 1f, num);
			Color color2 = new Color(1f, 1f, 1f, num * 4f);
			float num2 = this.fallRot / 40f;
			Vector2 vector = new Vector2(Game1.screenWidth, Game1.screenHeight) / 2f;
			if (this.returnType < 2)
			{
				vector += new Vector2(0f, 200f * (0.4f - Math.Min(num, 0.4f)));
			}
			Vector2 vector2 = vector + new Vector2(250f / this.maxTime * Math.Max(this.timer, 0f), num2 * 400f) + this.shakeTime * Rand.GetRandomVector2(-20f, 20f, -20f, 20f);
			if (this.unlockTex == null || this.unlockTex.IsDisposed)
			{
				return;
			}
			try
			{
				Vector2 vector3 = vector + new Vector2(-250f / this.maxTime * Math.Max(this.timer, 0f), num2 * 400f) + this.shakeTime * Rand.GetRandomVector2(-20f, 20f, -20f, 20f);
				Vector2 vector4 = Vector2.Zero;
				if (this.unlockSequence[0] < 0)
				{
					vector4 = Rand.GetRandomVector2(-1f, 1f, -1f, 1f) * MathHelper.Clamp(this.timer, 0f, 1f);
				}
				sprite.Draw(this.unlockTex, vector2 + vector4, new Rectangle(300, 300, 710, 64), color, num2, new Vector2(650f, 120f), 1f, SpriteEffects.None, 0f);
				sprite.Draw(this.unlockTex, vector2 - vector4, new Rectangle(300, 364, 710, 64), color, num2, new Vector2(650f, -56f), 1f, SpriteEffects.None, 0f);
				int num3 = (int)Math.Min((vector2.X - vector3.X - 80f) / 0.75f, 710f);
				sprite.Draw(this.unlockTex, vector3 - vector4, new Rectangle(1010 - num3, 300, num3, 64), color, 0f - num2, new Vector2(-20f, 100f), new Vector2(0.75f, 0.5f), SpriteEffects.FlipHorizontally, 0f);
				sprite.Draw(this.unlockTex, vector3 + vector4, new Rectangle(1010 - num3, 364, num3, 64), color, 0f - num2, new Vector2(-20f, -36f), new Vector2(0.75f, 0.5f), SpriteEffects.FlipHorizontally, 0f);
				Vector2 vector5 = new Vector2((float)(Math.Cos(num2) * 40.0), (float)(Math.Sin(num2) * 40.0)) + vector2;
				for (int i = 0; i < this.unlockTab.Length; i++)
				{
					float num4 = num2 + 0.62f + 2.4f / (float)this.unlockTab.Length * (float)i;
					if (this.returnType == 1)
					{
						this.unlockTab[i] = Math.Max(this.unlockTab[i] - Game1.HudTime * 12f, 0f);
					}
					else if (this.unlockSequence[i] < 0)
					{
						if (this.unlockTab[i] == 0f)
						{
							Vector2 vector6 = new Vector2((float)((0.0 - Math.Sin(num2 + num4)) * -120.0), (float)(Math.Cos(num2 + num4) * -120.0)) + vector5;
							Vector2 traj = new Vector2((float)((0.0 - Math.Sin(num2 + num4)) * -180.0), (float)(Math.Cos(num2 + num4) * -180.0)) + vector5 - vector6;
							pMan.AddHudDust(vector6, traj, 0.2f, num4, Color.White, 0.75f, 0, 9);
						}
						this.unlockTab[i] = Math.Min(this.unlockTab[i] + Game1.HudTime * 12f, 1f);
					}
					sprite.Draw(this.unlockTex, vector5, new Rectangle(250, 0, 50, 60), color, num4, new Vector2(30f, 180f - 40f * (1f - this.unlockTab[i])), 1f, SpriteEffects.None, 0f);
				}
				float num5 = vector3.X / 16.5f;
				Vector2 position = new Vector2((float)(Math.Cos(0f - num2) * 55.0 - Math.Sin(0f - num2) * -50.0), (float)(Math.Cos(0f - num2) * -50.0 + Math.Sin(0f - num2) * 55.0)) + vector3;
				sprite.Draw(this.unlockTex, position, new Rectangle(1010, 300, 80, 80), color, 0f - num2 + (0f - num5), new Vector2(40f, 40f), 1f, SpriteEffects.None, 0f);
				position = new Vector2((float)(Math.Cos(0f - num2) * 55.0 - Math.Sin(0f - num2) * 50.0), (float)(Math.Cos(0f - num2) * 50.0 + Math.Sin(0f - num2) * 55.0)) + vector3;
				sprite.Draw(this.unlockTex, position, new Rectangle(1010, 300, 80, 80), color, 0f - num2 + num5, new Vector2(40f, 40f), 1f, SpriteEffects.None, 0f);
				position = new Vector2((float)(Math.Cos(0f - num2) * -80.0 - Math.Sin(0f - num2) * -50.0), (float)(Math.Cos(0f - num2) * -50.0 + Math.Sin(0f - num2) * -80.0)) + vector3;
				sprite.Draw(this.unlockTex, position, new Rectangle(1010, 300, 80, 80), color, 0f - num2 + (0f - num5), new Vector2(40f, 40f), 1f, SpriteEffects.None, 0f);
				position = new Vector2((float)(Math.Cos(0f - num2) * -80.0 - Math.Sin(0f - num2) * 50.0), (float)(Math.Cos(0f - num2) * 50.0 + Math.Sin(0f - num2) * -80.0)) + vector3;
				sprite.Draw(this.unlockTex, position, new Rectangle(1010, 300, 80, 80), color, 0f - num2 + num5, new Vector2(40f, 40f), 1f, SpriteEffects.None, 0f);
				position = new Vector2((float)(Math.Cos(0f - num2) * -50.0), (float)(Math.Sin(0f - num2) * -50.0)) + vector3;
				sprite.Draw(this.unlockTex, position, new Rectangle(300, 428, 140, 140), color, 0f - num2 + num5, new Vector2(70f, 70f), 1f, SpriteEffects.None, 0f);
				position = new Vector2((float)(Math.Cos(num2) * -60.0), (float)(Math.Sin(num2) * -60.0)) + vector2;
				sprite.Draw(this.unlockTex, position, new Rectangle(300, 428, 140, 140), color, num2 + (0f - num5), new Vector2(70f, 70f), 1f, SpriteEffects.None, 0f);
				Rectangle value = ((Game1.pcManager.inputDevice == InputDevice.GamePad) ? new Rectangle(0, 0, 250, 300) : new Rectangle(440, 428, 250, 272));
				sprite.Draw(this.unlockTex, vector2, value, color, num2, new Vector2(123f, 150f), 1f, SpriteEffects.None, 0f);
				float num6 = (float)Math.Abs(Math.Cos(Game1.hud.pulse * 4f) * 1.0);
				sprite.End();
				sprite.Begin(SpriteSortMode.Immediate, BlendState.Additive);
				sprite.Draw(this.unlockTex, vector2, value, color * num6, num2, new Vector2(123f, 150f), 1f, SpriteEffects.None, 0f);
				sprite.End();
				sprite.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				sprite.Draw(this.unlockTex, vector2, new Rectangle(900, 0, 300, 300), color2, this.innerRot + num2, new Vector2(150f, 150f), 1f, SpriteEffects.None, 0f);
				sprite.Draw(this.unlockTex, vector2, new Rectangle(600, 0, 300, 300), color2, this.outerRot + num2, new Vector2(150f, 150f), 1f, SpriteEffects.None, 0f);
				sprite.Draw(this.unlockTex, vector2, new Rectangle(300, 0, 300, 300), color2, num2, new Vector2(123f, 150f), 1f, SpriteEffects.None, 0f);
				sprite.Draw(this.unlockTex, vector3, new Rectangle(0, 300, 300, 360), color2, 0f - num2, new Vector2(200f, 170f), 1f, SpriteEffects.None, 0f);
				if (Game1.hud.unlockState > 1)
				{
					if (this.outerRot != this.targetRot && Rand.GetRandomInt(0, 10) == 0)
					{
						pMan.AddBounceSpark(vector2 + Rand.GetRandomVector2(-100f, 100f, -100f, 100f), Rand.GetRandomVector2(-200f, 200f, -100f, 100f), 0.2f, 9);
					}
					if (Game1.hud.unlockState > 4)
					{
						pMan.AddSparkle(vector3 + Rand.GetRandomVector2(-100f, 200f, -100f, 100f), 1f, 1f, 1f, 0.5f, 0.8f, 70, 9);
						pMan.AddSparkle(vector2 + Rand.GetRandomVector2(-200f, 100f, -100f, 100f), 1f, 1f, 1f, 0.5f, 0.8f, 70, 9);
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
