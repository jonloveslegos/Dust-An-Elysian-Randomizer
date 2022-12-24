using Microsoft.Xna.Framework;

namespace Dust.Vibration
{
	internal class VibrationManager
	{
		public static Rumble[] Rumbles = new Rumble[4];

		public static ScreenShake ScreenShake;

		public static Blast Blast;

		public static void Init()
		{
			VibrationManager.ScreenShake = new ScreenShake();
			for (int i = 0; i < VibrationManager.Rumbles.Length; i++)
			{
				VibrationManager.Rumbles[i] = new Rumble(i);
			}
			VibrationManager.Blast = new Blast();
		}

		public static void Reset()
		{
			VibrationManager.Blast.value = 0f;
			VibrationManager.ScreenShake.value = 0f;
			VibrationManager.StopRumble();
		}

		public static void Update()
		{
			VibrationManager.ScreenShake.Update();
			for (int i = 0; i < VibrationManager.Rumbles.Length; i++)
			{
				VibrationManager.Rumbles[i].Update();
			}
			VibrationManager.Blast.Update();
		}

		public static void SetBlast(float value, Vector2 center)
		{
			if (VibrationManager.Blast.value < value)
			{
				VibrationManager.Blast.value = value;
			}
			VibrationManager.Blast.center = new Vector2(MathHelper.Clamp(center.X, Game1.Scroll.X / Game1.worldScale, (Game1.Scroll.X + (float)Game1.screenWidth) / Game1.worldScale), MathHelper.Clamp(center.Y, Game1.Scroll.Y / Game1.worldScale, (Game1.Scroll.Y + (float)Game1.screenHeight) / Game1.worldScale));
		}

		public static void SetScreenShake(float val)
		{
			if (VibrationManager.ScreenShake.value < val)
			{
				VibrationManager.ScreenShake.value = val;
				if (Game1.settings.Rumble)
				{
					VibrationManager.Rumbles[Game1.currentGamePad].Left = MathHelper.Clamp(val, 0f, 1f);
					VibrationManager.Rumbles[Game1.currentGamePad].Right = MathHelper.Clamp(val, 0f, 1f);
				}
			}
		}

		public static void Rumble(int gamePad, float val)
		{
			if (gamePad > -1 && Game1.settings.Rumble && val >= 0f)
			{
				VibrationManager.Rumbles[gamePad].Left = val;
				VibrationManager.Rumbles[gamePad].Right = val;
			}
		}

		public static void SetRumble(int gamePad, int motor, float val)
		{
			if (gamePad > -1 && Game1.settings.Rumble && val >= 0f)
			{
				if (motor == 0)
				{
					VibrationManager.Rumbles[gamePad].Left = val;
				}
				else
				{
					VibrationManager.Rumbles[gamePad].Right = val;
				}
			}
		}

		public static void StopRumble()
		{
			for (int i = 0; i < 4; i++)
			{
				VibrationManager.Rumble(i, 0f);
			}
		}
	}
}
