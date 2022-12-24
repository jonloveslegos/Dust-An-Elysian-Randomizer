using System;
using Dust.PCClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Dust.Vibration
{
	internal class Rumble
	{
		private Vector2 rumbleValue = Vector2.Zero;

		private PlayerIndex playerIndex;

		public float Left
		{
			get
			{
				return this.rumbleValue.X;
			}
			set
			{
				this.rumbleValue.X = value;
			}
		}

		public float Right
		{
			get
			{
				return this.rumbleValue.Y;
			}
			set
			{
				this.rumbleValue.Y = value;
			}
		}

		public Rumble(int gamePad)
		{
			this.playerIndex = (PlayerIndex)gamePad;
		}

		public void Update()
		{
			if (this.rumbleValue.X > 0f)
			{
				this.rumbleValue.X -= Game1.HudTime;
				if (this.rumbleValue.X < 0f)
				{
					this.rumbleValue.X = 0f;
				}
			}
			if (this.rumbleValue.Y > 0f)
			{
				this.rumbleValue.Y -= Game1.HudTime;
				if (this.rumbleValue.Y < 0f)
				{
					this.rumbleValue.Y = 0f;
				}
			}
			try
			{
				if (Game1.pcManager.inputDevice == InputDevice.GamePad)
				{
					GamePad.SetVibration(this.playerIndex, this.rumbleValue.X, this.rumbleValue.Y);
				}
				else
				{
					GamePad.SetVibration(this.playerIndex, 0f, 0f);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
