using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;

namespace Dust.PCClasses
{
	public class AlternateKeyboardLayouts
	{
		internal enum MAPVK : uint
		{
			VK_TO_VSC,
			VSC_TO_VK,
			VK_TO_CHAR
		}

		public struct KeyboardLayout : IDisposable
		{
			public readonly IntPtr Handle;

			public static KeyboardLayout US_English = new KeyboardLayout("00000409");

			public bool IsDisposed { get; private set; }

			public static KeyboardLayout Active => new KeyboardLayout(AlternateKeyboardLayouts.GetKeyboardLayout(IntPtr.Zero));

			public KeyboardLayout(IntPtr handle)
			{
				this = default(KeyboardLayout);
				this.Handle = handle;
			}

			public KeyboardLayout(string keyboardLayoutID)
				: this(AlternateKeyboardLayouts.LoadKeyboardLayout(keyboardLayoutID, 128u))
			{
			}

			public void Dispose()
			{
				if (!this.IsDisposed)
				{
					AlternateKeyboardLayouts.UnloadKeyboardLayout(this.Handle);
					this.IsDisposed = true;
				}
			}
		}

		internal const uint KLF_NOTELLSHELL = 128u;

		public readonly KeyboardState nativeState;

		public AlternateKeyboardLayouts(KeyboardState keyboardState)
		{
			this.nativeState = keyboardState;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern uint MapVirtualKeyEx(uint key, MAPVK mappingType, IntPtr keyboardLayout);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr LoadKeyboardLayout(string keyboardLayoutID, uint flags);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool UnloadKeyboardLayout(IntPtr handle);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr GetKeyboardLayout(IntPtr threadId);

		public bool IsKeyDown(Keys key, bool isLocalKey)
		{
			if (!isLocalKey)
			{
				key = AlternateKeyboardLayouts.USEnglishToLocal(key);
			}
			return this.nativeState.IsKeyDown(key);
		}

		public bool IsKeyUp(Keys key, bool isLocalKey)
		{
			if (!isLocalKey)
			{
				key = AlternateKeyboardLayouts.USEnglishToLocal(key);
			}
			return this.nativeState.IsKeyDown(key);
		}

		public bool IsKeyDown(Keys key)
		{
			return this.IsKeyDown(key, isLocalKey: false);
		}

		public bool IsKeyUp(Keys key)
		{
			return this.IsKeyDown(key, isLocalKey: false);
		}

		public static Keys USEnglishToLocal(Keys key)
		{
			uint key2 = AlternateKeyboardLayouts.MapVirtualKeyEx((uint)key, MAPVK.VK_TO_VSC, KeyboardLayout.US_English.Handle);
			return (Keys)AlternateKeyboardLayouts.MapVirtualKeyEx(key2, MAPVK.VSC_TO_VK, KeyboardLayout.Active.Handle);
		}
	}
}
