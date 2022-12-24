using System;
using System.IO;
using Dust.HUD;
using Dust.Strings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dust.PCClasses
{
	public class InputKey
	{
		private Keys mappedKey;

		private Keys defaultKey;

		private Keys hardLockedKey;

		private MouseButton mappedMouseButton;

		private MouseButton defaultMouseButton;

		public string friendlyName;

		public InputKey(string _name, Keys key, Keys hardKey, MouseButton _mouseButton)
		{
			this.friendlyName = _name;
			this.mappedKey = (this.defaultKey = AlternateKeyboardLayouts.USEnglishToLocal(key));
			this.mappedMouseButton = (this.defaultMouseButton = _mouseButton);
			if (hardKey != 0)
			{
				this.hardLockedKey = AlternateKeyboardLayouts.USEnglishToLocal(hardKey);
			}
		}

		public void WriteKey(BinaryWriter writer)
		{
			writer.Write(Convert.ToInt32(this.mappedKey));
			writer.Write(Convert.ToInt32(this.mappedMouseButton));
		}

		public void ReadKey(BinaryReader reader)
		{
			this.mappedKey = (Keys)reader.ReadInt32();
			this.mappedMouseButton = (MouseButton)reader.ReadInt32();
		}

		public bool IsPressed(AlternateKeyboardLayouts keyboardState, MouseState mouseState)
		{
			if (keyboardState == null)
            {
				return false;
			}
			if (mouseState == null)
			{
				return false;
			}
			if (this.hardLockedKey != 0 && (Game1.pcManager.inMenu || (this.hardLockedKey == Keys.Escape && this.defaultKey != Keys.L)) && keyboardState.IsKeyDown(this.hardLockedKey, isLocalKey: true))
			{
				return true;
			}
			if (this.mappedKey != 0 && keyboardState.IsKeyDown(this.mappedKey, isLocalKey: true))
			{
				return true;
			}
			if (!Game1.pcManager.inMenu && Game1.pcManager.mouseInBounds && Game1.CheckIsActive)
			{
				switch (this.mappedMouseButton)
				{
				case MouseButton.LeftButton:
					if (mouseState.LeftButton == ButtonState.Pressed)
					{
						return true;
					}
					break;
				case MouseButton.MidButton:
					if (mouseState.MiddleButton == ButtonState.Pressed)
					{
						return true;
					}
					break;
				case MouseButton.RightButton:
					if (mouseState.RightButton == ButtonState.Pressed)
					{
						return true;
					}
					break;
				}
				switch (this.defaultMouseButton)
				{
				case MouseButton.LeftButton:
					if (mouseState.LeftButton == ButtonState.Pressed)
					{
						return true;
					}
					break;
				case MouseButton.MidButton:
					if (mouseState.MiddleButton == ButtonState.Pressed)
					{
						return true;
					}
					break;
				case MouseButton.RightButton:
					if (mouseState.RightButton == ButtonState.Pressed)
					{
						return true;
					}
					break;
				}
			}
			return false;
		}

		public bool AssignFunction()
		{
			Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
			if (pressedKeys.Length > 0)
			{
				this.mappedKey = pressedKeys[0];
				return true;
			}
			MouseState state = Mouse.GetState();
			if (state.LeftButton == ButtonState.Pressed)
			{
				this.mappedMouseButton = MouseButton.LeftButton;
				return true;
			}
			if (state.MiddleButton == ButtonState.Pressed)
			{
				this.mappedMouseButton = MouseButton.MidButton;
				return true;
			}
			if (state.RightButton == ButtonState.Pressed)
			{
				this.mappedMouseButton = MouseButton.RightButton;
				return true;
			}
			return false;
		}

		public bool ClearMapping()
		{
			bool result = this.mappedKey != 0 || this.mappedMouseButton != MouseButton.None;
			this.mappedKey = Keys.None;
			this.mappedMouseButton = MouseButton.None;
			return result;
		}

		public void ResetMapping()
		{
			this.mappedKey = this.defaultKey;
			this.mappedMouseButton = this.defaultMouseButton;
		}

		public float DrawTestKey(SpriteBatch sprite, Vector2 loc, float size)
		{
			string text = this.friendlyName + "\n      Key: (" + this.mappedKey.ToString().ToUpper() + ")";
			if (this.mappedMouseButton.ToString() != "None")
			{
				text = text + ",   Mouse:" + this.mappedMouseButton;
			}
			Game1.smallText.DrawOutlineText(loc, text, size, 0, TextAlign.Left, fullOutline: true);
			return Game1.smallFont.MeasureString(text).Y * size;
		}

		public string MappedKeyString()
		{
			if (this.mappedKey.ToString() != "None")
			{
				return this.mappedKey.ToString().ToUpper();
			}
			return "";
		}

		public string MappedMouseString()
		{
			if (this.mappedMouseButton.ToString() != "None")
			{
				switch (this.mappedMouseButton)
				{
					case MouseButton.RightButton: return "[MOUSER]: " + Strings_PC.MouseButtonRight;
					case MouseButton.MidButton: return "[MOUSEM]: " + Strings_PC.MouseButtonMid;
				}
				return "[MOUSEL]: " + Strings_PC.MouseButtonLeft;
			}
			return "";
		}

		public string MappedUsingString(bool parantheses)
		{
			Game1.pcManager.UpdateInMenu();
			if (Game1.menu.menuMode == MenuMode.FileManage && Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				return "";
			}
			if (this.hardLockedKey.ToString() != "None" && Game1.pcManager.inMenu && Game1.menu.menuMode != MenuMode.Help && (Game1.hud.dialogue == null || Game1.menu.prompt != promptDialogue.None || Game1.hud.shopType != 0))
			{
				if (parantheses)
				{
					return "'" + this.hardLockedKey.ToString().ToUpper() + "'";
				}
				return this.hardLockedKey.ToString().ToUpper();
			}
			if (this.mappedMouseButton.ToString() != "None" && Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				switch (this.mappedMouseButton)
				{
					case MouseButton.RightButton: return "[MOUSER]";
					case MouseButton.MidButton: return "[MOUSEM]";
				}
				return "[MOUSEL]"; 
			}
			if (this.mappedKey.ToString() != "None")
			{
				if (parantheses)
				{
					return "'" + this.mappedKey.ToString().ToUpper() + "'";
				}
				return this.mappedKey.ToString().ToUpper();
			}
			return "";
		}

		public string MappedUsingStringOld(bool parantheses)
		{
			Game1.pcManager.UpdateInMenu();
			if (Game1.menu.menuMode == MenuMode.FileManage && Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				return "";
			}
			if (this.hardLockedKey.ToString() != "None" && Game1.pcManager.inMenu && Game1.menu.menuMode != MenuMode.Help && (Game1.hud.dialogue == null || Game1.menu.prompt != promptDialogue.None || Game1.hud.shopType != 0))
			{
				if (parantheses)
				{
					return "'" + this.hardLockedKey.ToString().ToUpper() + "'";
				}
				return this.hardLockedKey.ToString().ToUpper();
			}
			if (this.mappedMouseButton.ToString() != "None" && Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				switch (this.mappedMouseButton)
				{
					case MouseButton.RightButton: return "[MOUSER]";
					case MouseButton.MidButton: return "[MOUSEM]";
				}
				return "[MOUSEL]";
			}
			if (this.mappedKey.ToString() != "None")
			{
				if (parantheses)
				{
					return "'" + this.mappedKey.ToString().ToUpper() + "'";
				}
				return this.mappedKey.ToString().ToUpper();
			}
			return "";
		}
	}
}
