using System;
using System.Collections.Generic;
using System.IO;
using Dust.Audio;
using Dust.CharClasses;
using Dust.Dialogue;
using Dust.HUD;
using Dust.Strings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dust.PCClasses
{
	public class PCManager
	{
		public InputDevice inputDevice;

		private SpriteBatch sprite;

		private Texture2D[] hudTex;

		private AlternateKeyboardLayouts curHudKeyboardState = new AlternateKeyboardLayouts(Keyboard.GetState());

		private AlternateKeyboardLayouts prevHudKeyboardState;

		private MouseState curHudMouseState;

		private MouseState prevHudMouseState;

		private AlternateKeyboardLayouts curPlayerKeyboardState = new AlternateKeyboardLayouts(Keyboard.GetState());

		private AlternateKeyboardLayouts prevPlayerKeyboardState;

		private MouseState curPlayerMouseState;

		private MouseState prevPlayerMouseState;

		private AlternateKeyboardLayouts curShopKeyboardState = new AlternateKeyboardLayouts(Keyboard.GetState());

		private AlternateKeyboardLayouts prevShopKeyboardState;

		private MouseState curShopMouseState;

		private MouseState prevShopMouseState;

		private AlternateKeyboardLayouts curWorldMapKeyboardState = new AlternateKeyboardLayouts(Keyboard.GetState());

		private AlternateKeyboardLayouts prevWorldMapKeyboardState;

		private MouseState curWorldMapMouseState;

		private MouseState prevWorldMapMouseState;

		public bool inMenu;

		public bool mouseInBounds;

		public bool leftMouseClicked;

		public bool doubleClicked;

		public Vector2 mouseDiff;

		private float doubleClickTime;

		public float mouseWheelValue;

		private static Texture2D[] particlesTex;

		private InputKey mappedKeyLeft = new InputKey(Strings_ManualGraphics.KeyboardLeft, Keys.A, Keys.Left, MouseButton.None);

		private InputKey mappedKeyRight = new InputKey(Strings_ManualGraphics.KeyboardRight, Keys.D, Keys.Right, MouseButton.None);

		private InputKey mappedKeyUp = new InputKey(Strings_ManualGraphics.KeyboardUp, Keys.W, Keys.Up, MouseButton.None);

		private InputKey mappedKeyDown = new InputKey(Strings_ManualGraphics.KeyboardDown, Keys.S, Keys.Down, MouseButton.None);

		private InputKey mappedKeyA = new InputKey(Strings_ManualGraphics.KeyboardJumpConfirm, Keys.Space, Keys.Enter, MouseButton.None);

		private InputKey mappedKeyB = new InputKey(Strings_ManualGraphics.ButtonB, Keys.L, Keys.Escape, MouseButton.MidButton);

		private InputKey mappedKeyX = new InputKey(Strings_ManualGraphics.ButtonX, Keys.J, Keys.None, MouseButton.LeftButton);

		private InputKey mappedKeyY = new InputKey(Strings_ManualGraphics.ButtonY, Keys.K, Keys.None, MouseButton.RightButton);

		private InputKey mappedKeyRB = new InputKey(Strings_ManualGraphics.ButtonRB, Keys.F, Keys.None, MouseButton.None);

		private InputKey mappedKeyLB = new InputKey(Strings_ManualGraphics.ButtonLB, Keys.H, Keys.None, MouseButton.None);

		private InputKey mappedKeyRT = new InputKey(Strings_ManualGraphics.ButtonRT, Keys.E, Keys.None, MouseButton.None);

		private InputKey mappedKeyLT = new InputKey(Strings_ManualGraphics.ButtonLT, Keys.Q, Keys.None, MouseButton.None);

		private InputKey mappedKeyRClick = new InputKey(Strings_ManualGraphics.KeyboardMap, Keys.M, Keys.None, MouseButton.None);

		private InputKey mappedKeyBack = new InputKey(Strings_ManualGraphics.ButtonBack, Keys.Tab, Keys.None, MouseButton.None);

		private InputKey mappedKeyStart = new InputKey(Strings_ManualGraphics.ButtonStart, Keys.Escape, Keys.Escape, MouseButton.None);

		private InputKey mappedKeyInvRight = new InputKey(Strings_ManualGraphics.KeyboardInvRight, Keys.PageDown, Keys.None, MouseButton.None);

		private InputKey mappedKeyInvLeft = new InputKey(Strings_ManualGraphics.KeyboardInvLeft, Keys.PageUp, Keys.None, MouseButton.None);

		private InputKey mappedKeyInvSubRight = new InputKey(Strings_ManualGraphics.KeyboardInvSubRight, Keys.End, Keys.None, MouseButton.None);

		private InputKey mappedKeyInvSubLeft = new InputKey(Strings_ManualGraphics.KeyboardInvSubLeft, Keys.Home, Keys.None, MouseButton.None);

		public Dictionary<string, InputKey> inputKeyList = new Dictionary<string, InputKey>();

		public bool KeyInvLeft;

		public bool KeyInvRight;

		public bool KeyInvSubLeft;

		public bool KeyInvSubRight;

		public PCManager(SpriteBatch _sprite, Texture2D[] _particleTex, Texture2D[] _hudTex)
		{
			this.sprite = _sprite;
			PCManager.particlesTex = _particleTex;
			this.hudTex = _hudTex;
			this.inputDevice = InputDevice.GamePad;
			this.inputKeyList.Clear();
			this.inputKeyList.Add("left", this.mappedKeyLeft);
			this.inputKeyList.Add("right", this.mappedKeyRight);
			this.inputKeyList.Add("up", this.mappedKeyUp);
			this.inputKeyList.Add("down", this.mappedKeyDown);
			this.inputKeyList.Add("a", this.mappedKeyA);
			this.inputKeyList.Add("b", this.mappedKeyB);
			this.inputKeyList.Add("x", this.mappedKeyX);
			this.inputKeyList.Add("y", this.mappedKeyY);
			this.inputKeyList.Add("rb", this.mappedKeyRB);
			this.inputKeyList.Add("lb", this.mappedKeyLB);
			this.inputKeyList.Add("rt", this.mappedKeyRT);
			this.inputKeyList.Add("lt", this.mappedKeyLT);
			this.inputKeyList.Add("rclick", this.mappedKeyRClick);
			this.inputKeyList.Add("back", this.mappedKeyBack);
			this.inputKeyList.Add("start", this.mappedKeyStart);
			this.inputKeyList.Add("invright", this.mappedKeyInvRight);
			this.inputKeyList.Add("invleft", this.mappedKeyInvLeft);
			this.inputKeyList.Add("invsubright", this.mappedKeyInvSubRight);
			this.inputKeyList.Add("invsubleft", this.mappedKeyInvSubLeft);
		}

		public void ResetMapping()
		{
			foreach (KeyValuePair<string, InputKey> inputKey in this.inputKeyList)
			{
				inputKey.Value.ResetMapping();
			}
		}

		public void ClearMapping()
		{
			foreach (KeyValuePair<string, InputKey> inputKey in this.inputKeyList)
			{
				inputKey.Value.ClearMapping();
			}
		}

		public void WriteMapping(BinaryWriter writer)
		{
			foreach (KeyValuePair<string, InputKey> inputKey in this.inputKeyList)
			{
				inputKey.Value.WriteKey(writer);
			}
		}

		public void ReadMapping(BinaryReader reader)
		{
			foreach (KeyValuePair<string, InputKey> inputKey in this.inputKeyList)
			{
				inputKey.Value.ReadKey(reader);
			}
		}

		public bool IsHudClicked(InputKey mappedKey)
		{
			if (mappedKey.IsPressed(this.curHudKeyboardState, this.curHudMouseState) && !mappedKey.IsPressed(this.prevHudKeyboardState, this.prevHudMouseState))
			{
				return true;
			}
			return false;
		}

		private bool IsHudHeld(InputKey mappedKey)
		{
			if (mappedKey.IsPressed(this.curHudKeyboardState, this.curHudMouseState))
			{
				return true;
			}
			return false;
		}

		public bool IsHudHeld(string buttonName)
		{
			switch (buttonName)
			{
			case "KeyUp":
				if (this.IsHudHeld(this.mappedKeyUp))
				{
					return true;
				}
				break;
			case "KeyDown":
				if (this.IsHudHeld(this.mappedKeyDown))
				{
					return true;
				}
				break;
			case "KeyLeft":
				if (this.IsHudHeld(this.mappedKeyLeft))
				{
					return true;
				}
				break;
			case "KeyRight":
				if (this.IsHudHeld(this.mappedKeyRight))
				{
					return true;
				}
				break;
			case "KeySelect":
				if (this.IsHudHeld(this.mappedKeyA))
				{
					return true;
				}
				break;
			case "KeyCancel":
				if (this.IsHudHeld(this.mappedKeyB))
				{
					return true;
				}
				break;
			}
			return false;
		}

		private bool IsShopClicked(InputKey mappedKey)
		{
			if (mappedKey.IsPressed(this.curShopKeyboardState, this.curShopMouseState) && !mappedKey.IsPressed(this.prevShopKeyboardState, this.prevShopMouseState))
			{
				return true;
			}
			return false;
		}

		private bool IsShopHeld(InputKey mappedKey)
		{
			if (mappedKey.IsPressed(this.curShopKeyboardState, this.curShopMouseState))
			{
				return true;
			}
			return false;
		}

		private bool IsWorldMapClicked(InputKey mappedKey)
		{
			if (mappedKey.IsPressed(this.curWorldMapKeyboardState, this.curWorldMapMouseState) && !mappedKey.IsPressed(this.prevWorldMapKeyboardState, this.prevWorldMapMouseState))
			{
				return true;
			}
			return false;
		}

		private bool IsWorldMapHeld(InputKey mappedKey)
		{
			if (mappedKey.IsPressed(this.curWorldMapKeyboardState, this.curWorldMapMouseState))
			{
				return true;
			}
			return false;
		}

		private bool IsPlayerClicked(InputKey mappedKey)
		{
			if (mappedKey.IsPressed(this.curPlayerKeyboardState, this.curPlayerMouseState) && !mappedKey.IsPressed(this.prevPlayerKeyboardState, this.prevPlayerMouseState))
			{
				return true;
			}
			return false;
		}

		private bool IsPlayerHeld(InputKey mappedKey)
		{
			if (mappedKey.IsPressed(this.curPlayerKeyboardState, this.curPlayerMouseState))
			{
				return true;
			}
			return false;
		}

		public bool IsPlayerHeld(string buttonName)
		{
			switch (buttonName)
			{
			case "KeyJump":
				if (this.IsPlayerHeld(this.mappedKeyA))
				{
					return true;
				}
				break;
			case "KeyUp":
				if (this.IsPlayerHeld(this.mappedKeyUp))
				{
					return true;
				}
				break;
			case "KeyDown":
				if (this.IsPlayerHeld(this.mappedKeyDown))
				{
					return true;
				}
				break;
			}
			return false;
		}

		public bool CheckInitInput()
		{
			if (Keyboard.GetState().GetPressedKeys().Length > 0)
			{
				this.inputDevice = InputDevice.KeyboardOnly;
				return true;
			}
			if (this.IsMouseLeftHeld() || this.IsMouseRightHeld())
			{
				this.inputDevice = InputDevice.KeyboardAndMouse;
				return true;
			}
			return false;
		}

		public void UpdateInMenu()
		{
			this.inMenu = Game1.hud.inventoryState != 0 || Game1.hud.shopType != 0 || Game1.menu.prompt != promptDialogue.None || Game1.hud.isPaused || Game1.hud.dialogue != null || Game1.gameMode != Game1.GameModes.Game || Game1.cManager.challengeMode == ChallengeManager.ChallengeMode.ViewScore || Game1.cManager.challengeMode == ChallengeManager.ChallengeMode.TallyScore;
		}

		public void Update(GraphicsDeviceManager graphics)
		{
			this.UpdateInMenu();
			if (this.doubleClickTime > 0f)
			{
				this.doubleClickTime -= Game1.HudTime;
			}
			this.mouseInBounds = new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight).Contains(this.curHudMouseState.X, this.curHudMouseState.Y);
			if (this.mouseInBounds && Math.Abs(this.mouseDiff.X + this.mouseDiff.Y) > 60f)
			{
				this.inputDevice = InputDevice.KeyboardAndMouse;
			}
			bool flag = false;
			if (this.curHudMouseState.LeftButton == ButtonState.Pressed || this.curHudMouseState.RightButton == ButtonState.Pressed || this.curHudMouseState.MiddleButton == ButtonState.Pressed)
			{
				if (this.mouseInBounds)
				{
					this.inputDevice = InputDevice.KeyboardAndMouse;
				}
				else
				{
					flag = true;
				}
			}
			if (!Game1.CheckIsActive || flag)
			{
				if (Game1.stats.playerLifeState != 0 || Game1.hud.inventoryState != 0 || Game1.gameMode != Game1.GameModes.Game || Game1.menu.prompt != promptDialogue.None || Game1.hud.shopType != 0)
				{
					return;
				}
				if (Game1.events.anyEvent)
				{
					if (Game1.menu.prompt != promptDialogue.SkipEvent && (Game1.hud.dialogueState != DialogueState.Active || Game1.hud.dialogue.responseLocalList.Count < 2 || Game1.cutscene.SceneType == CutsceneType.Video))
					{
						Sound.PlayCue("menu_confirm");
						Game1.menu.prompt = promptDialogue.SkipEvent;
						Game1.menu.ClearPrompt();
						if (Game1.events.skippable != SkipType.Unskippable && (Game1.hud.dialogueState == DialogueState.Active || Game1.cutscene.SceneType == CutsceneType.Video))
						{
							Game1.menu.curMenuOption = 1;
							Game1.menu.PopulatePrompt();
						}
						VoiceManager.PauseVoice();
					}
				}
				else if (Game1.events.regionIntroStage == 0 && Game1.hud.unlockState == 0 && Game1.map.warpStage == 0 && Game1.map.doorStage == 0 && (Game1.cManager.challengeMode == ChallengeManager.ChallengeMode.None || Game1.cManager.challengeMode == ChallengeManager.ChallengeMode.InChallenge) && Game1.map.GetTransVal() <= 0f)
				{
					Game1.hud.Pause();
				}
			}
			else if (Game1.CheckIsActive && Game1.settings.FullScreen)
			{
				if (this.curHudMouseState.X < 20)
				{
					Mouse.SetPosition(20, this.curHudMouseState.Y);
				}
				if (this.curHudMouseState.X > Game1.screenWidth - 20)
				{
					Mouse.SetPosition(Game1.screenWidth - 20, this.curHudMouseState.Y);
				}
				if (this.curHudMouseState.Y < 20)
				{
					Mouse.SetPosition(this.curHudMouseState.X, 20);
				}
				if (this.curHudMouseState.Y > Game1.screenHeight - 20)
				{
					Mouse.SetPosition(this.curHudMouseState.X, Game1.screenHeight - 20);
				}
			}
		}

		public void UpdateHudInput(ref bool KeyLeft, ref bool KeyRight, ref bool KeyUp, ref bool KeyDown, ref bool KeyUpOpen, ref bool KeyThumb_Left, ref bool KeyThumb_Right, ref bool KeySelect, ref bool KeyCancel, ref bool KeyX, ref bool KeyY, ref bool KeyPause, ref bool OpenMenuButton, ref bool KeyLeftTrigger, ref bool KeyRightTrigger, ref bool KeyLeftBumper, ref bool KeyRightBumper)
		{
			this.curHudKeyboardState = new AlternateKeyboardLayouts(Keyboard.GetState());
			this.curHudMouseState = Mouse.GetState();
			this.doubleClicked = false;
			if (!Game1.CheckIsActive)
			{
				return;
			}
			if (this.curHudKeyboardState.IsKeyDown(Keys.Enter, isLocalKey: true) && this.curHudKeyboardState.IsKeyDown(Keys.RightAlt, isLocalKey: true))
			{
				Game1.graphics.ToggleFullScreen();
				Game1.settings.FullScreen = Game1.graphics.IsFullScreen;
				if (Game1.hud.inventoryState != 0 && Game1.hud.LeavePage(Game1.hud.inventoryState))
				{
					Game1.hud.ExitInventory();
				}
			}
			if (this.IsHudClicked(this.mappedKeyA))
			{
				KeySelect = true;
			}
			if (this.IsHudClicked(this.mappedKeyStart))
			{
				if (this.inMenu)
				{
					KeyCancel = true;
				}
				else
				{
					KeyPause = true;
				}
			}
			if (this.IsHudClicked(this.mappedKeyB))
			{
				KeyCancel = true;
			}
			if (this.IsHudClicked(this.mappedKeyUp))
			{
				KeyUp = (KeyUpOpen = true);
			}
			if (this.IsHudClicked(this.mappedKeyDown))
			{
				KeyDown = true;
			}
			if (this.IsHudClicked(this.mappedKeyLeft))
			{
				KeyLeft = true;
			}
			if (this.IsHudClicked(this.mappedKeyRight))
			{
				KeyRight = true;
			}
			if (this.IsHudClicked(this.mappedKeyBack))
			{
				OpenMenuButton = true;
			}
			if (Game1.hud.inventoryState != 0 || Game1.hud.shopType != 0)
			{
				this.KeyInvLeft = this.IsHudClicked(this.mappedKeyInvLeft);
				this.KeyInvRight = this.IsHudClicked(this.mappedKeyInvRight);
				this.KeyInvSubLeft = this.IsHudClicked(this.mappedKeyInvSubLeft);
				this.KeyInvSubRight = this.IsHudClicked(this.mappedKeyInvSubRight);
			}
			if (Game1.stats.playerLifeState == 0 || Game1.menu.menuMode != 0)
			{
				if (this.IsHudClicked(this.mappedKeyLB))
				{
					KeyLeftBumper = true;
				}
				if (this.IsHudClicked(this.mappedKeyRB))
				{
					KeyRightBumper = true;
				}
				if (this.IsHudClicked(this.mappedKeyRClick))
				{
					KeyThumb_Right = true;
				}
				if (this.IsHudClicked(this.mappedKeyY))
				{
					KeyY = true;
				}
				if (this.IsHudClicked(this.mappedKeyX))
				{
					KeyX = true;
				}
			}
			if (this.mouseInBounds && this.curHudMouseState.LeftButton == ButtonState.Pressed && this.prevHudMouseState.LeftButton == ButtonState.Released)
			{
				this.leftMouseClicked = true;
				this.inputDevice = InputDevice.KeyboardAndMouse;
				if (this.doubleClickTime > 0f)
				{
					this.doubleClicked = true;
				}
				this.doubleClickTime = 0.3f;
			}
			this.mouseWheelValue = this.curHudMouseState.ScrollWheelValue - this.prevHudMouseState.ScrollWheelValue;
			this.mouseDiff = new Vector2(this.curHudMouseState.X - this.prevHudMouseState.X, this.curHudMouseState.Y - this.prevHudMouseState.Y);
			if (this.inMenu && (KeyLeft || KeyRight || KeyUp || KeyDown || KeyThumb_Left || KeyThumb_Right || KeyLeftTrigger || KeyRightTrigger || KeyLeftBumper || KeyRightBumper || KeySelect || KeyCancel) && Game1.menu.configuringControls < 2)
			{
				this.inputDevice = InputDevice.KeyboardOnly;
			}
			if (Math.Abs(this.mouseDiff.X + this.mouseDiff.Y) > 4f)
			{
				this.doubleClickTime = -1f;
				this.doubleClicked = false;
			}
			this.prevHudKeyboardState = this.curHudKeyboardState;
			this.prevHudMouseState = this.curHudMouseState;
		}

		public void UpdateShopInput(ref bool KeyLeft, ref bool KeyRight, ref bool KeyUp, ref bool KeyDown, ref bool KeySelect, ref bool KeyCancel, ref bool KeyX, ref bool KeyY, ref bool KeyLeftBumper, ref bool KeyRightBumper)
		{
			this.curShopKeyboardState = new AlternateKeyboardLayouts(Keyboard.GetState());
			this.curShopMouseState = Mouse.GetState();
			if (Game1.CheckIsActive)
			{
				if (this.IsShopClicked(this.mappedKeyA))
				{
					KeySelect = true;
				}
				if (this.IsShopClicked(this.mappedKeyStart))
				{
					KeyCancel = true;
				}
				if (this.IsShopClicked(this.mappedKeyB))
				{
					KeyCancel = true;
				}
				if (this.IsShopClicked(this.mappedKeyUp))
				{
					KeyUp = true;
				}
				if (this.IsShopClicked(this.mappedKeyDown))
				{
					KeyDown = true;
				}
				if (this.IsShopClicked(this.mappedKeyLeft))
				{
					KeyLeft = true;
				}
				if (this.IsShopClicked(this.mappedKeyRight))
				{
					KeyRight = true;
				}
				if (this.IsShopClicked(this.mappedKeyLB))
				{
					KeyLeftBumper = true;
				}
				if (this.IsShopClicked(this.mappedKeyRB))
				{
					KeyRightBumper = true;
				}
				if (this.IsShopClicked(this.mappedKeyY))
				{
					KeyY = true;
				}
				if (this.IsShopClicked(this.mappedKeyX))
				{
					KeyX = true;
				}
				if (KeyLeft || KeyRight || KeyUp || KeyDown || KeyX || KeyY || KeyLeftBumper || KeyRightBumper)
				{
					this.inputDevice = InputDevice.KeyboardOnly;
				}
				this.prevShopKeyboardState = this.curShopKeyboardState;
				this.prevShopMouseState = this.curShopMouseState;
			}
		}

		public void UpdateWorldMapInput(ref bool KeyLeft, ref bool KeyRight, ref bool KeyUp, ref bool KeyDown, ref bool KeySelect, ref bool KeyCancel, ref bool KeyX, ref bool KeyY)
		{
			this.curWorldMapKeyboardState = new AlternateKeyboardLayouts(Keyboard.GetState());
			this.curWorldMapMouseState = Mouse.GetState();
			if (Game1.CheckIsActive)
			{
				if (this.IsWorldMapClicked(this.mappedKeyA))
				{
					KeySelect = true;
				}
				if (this.IsWorldMapClicked(this.mappedKeyB) || this.IsWorldMapClicked(this.mappedKeyStart))
				{
					KeyCancel = true;
				}
				if (this.IsWorldMapClicked(this.mappedKeyX))
				{
					KeyX = true;
				}
				if (this.IsWorldMapClicked(this.mappedKeyY))
				{
					KeyY = true;
				}
				if (this.IsWorldMapClicked(this.mappedKeyUp))
				{
					KeyUp = true;
				}
				if (this.IsWorldMapClicked(this.mappedKeyDown))
				{
					KeyDown = true;
				}
				if (this.IsWorldMapClicked(this.mappedKeyLeft))
				{
					KeyLeft = true;
				}
				if (this.IsWorldMapClicked(this.mappedKeyRight))
				{
					KeyRight = true;
				}
				if (KeyLeft || KeyRight || KeyUp || KeyDown)
				{
					this.inputDevice = InputDevice.KeyboardOnly;
				}
				this.prevWorldMapKeyboardState = this.curWorldMapKeyboardState;
				this.prevWorldMapMouseState = this.curWorldMapMouseState;
			}
		}

		public void UpdatePlayerInput(ref bool KeyLeft, ref bool KeyRight, ref bool KeyUp, ref bool KeyDown, ref bool KeyJump, ref bool KeyThrow, ref bool KeyAttack, ref bool KeySecondary, ref bool KeyEvade, Character c)
		{
			this.curPlayerKeyboardState = new AlternateKeyboardLayouts(Keyboard.GetState());
			this.curPlayerMouseState = Mouse.GetState();
			if (!Game1.CheckIsActive)
			{
				return;
			}
			if (this.IsPlayerClicked(this.mappedKeyA))
			{
				KeyJump = true;
			}
			if (!c.Holding)
			{
				if (this.IsPlayerHeld(this.mappedKeyLeft))
				{
					KeyLeft = true;
				}
				else if (this.IsPlayerHeld(this.mappedKeyRight))
				{
					KeyRight = true;
				}
				if (this.IsPlayerHeld(this.mappedKeyDown))
				{
					KeyDown = true;
				}
				else if (this.IsPlayerHeld(this.mappedKeyUp))
				{
					KeyUp = true;
				}
				if (this.IsPlayerClicked(this.mappedKeyA))
				{
					KeyJump = true;
				}
				if (this.IsPlayerClicked(this.mappedKeyRT))
				{
					if (c.CheckEvade())
					{
						c.Face = CharDir.Left;
					}
				}
				else if (this.IsPlayerClicked(this.mappedKeyLT) && c.CheckEvade())
				{
					c.Face = CharDir.Right;
				}
			}
			if (Game1.stats.gameDifficulty == 0 || Game1.settings.AutoCombo)
			{
				if (this.IsPlayerHeld(this.mappedKeyX))
				{
					KeyAttack = true;
					if (c.AnimName.StartsWith("attack"))
					{
						c.Defense = DefenseStates.Parrying;
						c.CanHurtFrame = 0f;
					}
				}
				if (this.IsPlayerHeld(this.mappedKeyY))
				{
					if (c.State == CharState.Grounded || c.AnimName == "airspin")
					{
						KeySecondary = true;
					}
					else if (!this.mappedKeyY.IsPressed(this.prevPlayerKeyboardState, this.prevPlayerMouseState))
					{
						KeySecondary = true;
					}
				}
				if (this.IsPlayerHeld(this.mappedKeyB) && Game1.longSkipFrame == 1)
				{
					KeyThrow = true;
				}
			}
			else
			{
				if (this.IsPlayerHeld(this.mappedKeyX))
				{
					if (c.AnimName.StartsWith("attack"))
					{
						c.Defense = DefenseStates.Parrying;
						c.CanHurtFrame = 0f;
					}
					if (!this.mappedKeyX.IsPressed(this.prevPlayerKeyboardState, this.prevPlayerMouseState))
					{
						KeyAttack = true;
					}
				}
				if (this.IsPlayerHeld(this.mappedKeyY))
				{
					if (c.AnimName.StartsWith("attack") || c.State == CharState.Air)
					{
						if (c.AnimName == "attack01" || c.AnimName == "airspin")
						{
							KeySecondary = true;
						}
						else if (!this.mappedKeyY.IsPressed(this.prevPlayerKeyboardState, this.prevPlayerMouseState))
						{
							KeySecondary = true;
						}
					}
					else
					{
						KeySecondary = true;
					}
				}
				if (this.IsPlayerClicked(this.mappedKeyB))
				{
					KeyThrow = true;
				}
			}
			if ((KeyLeft || KeyRight || KeyUp || KeyDown || KeyJump || KeyThrow || KeyAttack || KeySecondary || KeyEvade || this.IsHudHeld(this.mappedKeyStart)) && this.inputDevice == InputDevice.GamePad)
			{
				this.inputDevice = InputDevice.KeyboardOnly;
			}
			this.prevPlayerKeyboardState = this.curPlayerKeyboardState;
			this.prevPlayerMouseState = this.curPlayerMouseState;
		}

		public void ClearMouseInput()
		{
			this.leftMouseClicked = false;
		}

		public Vector2 GetMouseLoc()
		{
			return new Vector2(this.curHudMouseState.X, this.curHudMouseState.Y);
		}

		public bool IsMouseLeftHeld()
		{
			return this.curHudMouseState.LeftButton == ButtonState.Pressed;
		}

		public bool IsMouseRightHeld()
		{
			return this.curHudMouseState.RightButton == ButtonState.Pressed;
		}

		public void ResetDoubleClick()
		{
			this.doubleClicked = false;
			this.doubleClickTime = -1f;
		}

		public void MouseWheelAdjust(ref float scrollValue, float lockHeight, float minValue, float maxValue)
		{
			if (this.mouseWheelValue < 0f)
			{
				scrollValue = MathHelper.Clamp(scrollValue + lockHeight, minValue, maxValue);
			}
			else if (this.mouseWheelValue > 0f)
			{
				scrollValue = MathHelper.Clamp(scrollValue - lockHeight, minValue, maxValue);
			}
		}

		public Vector2 DrawCursor(SpriteBatch sprite, float scale, Color color)
		{
			Vector2 vector = new Vector2(this.curHudMouseState.X, this.curHudMouseState.Y);
			if (Game1.settings.FullScreen)
			{
				vector.X = MathHelper.Clamp(vector.X, 20f, Game1.screenWidth - 20);
				vector.Y = MathHelper.Clamp(vector.Y, 20f, Game1.screenHeight - 20);
			}
			sprite.Draw(PCManager.particlesTex[2], vector, new Rectangle(80 * Game1.hud.cursorFrame, 2920, 80, 50), new Color(0f, 0f, 0f, (float)(int)color.A / 255f * 0.3f), -2.2f, new Vector2(77f, 25f), scale, SpriteEffects.FlipVertically, 0f);
			sprite.Draw(PCManager.particlesTex[2], vector, new Rectangle(80 * Game1.hud.cursorFrame, 2920, 80, 50), color, -2.35f, new Vector2(77f, 25f), scale, SpriteEffects.FlipVertically, 0f);
			return vector;
		}

		public bool DrawMouseButton(Vector2 loc, float scale, Color color, int buttonType, bool draw)
		{
			if (draw)
			{
				this.sprite.Draw(this.hudTex[1], loc, new Rectangle(1190, buttonType * 70, 67, 70), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			}
			if (new Rectangle((int)loc.X, (int)loc.Y, (int)(60f * scale), (int)(60f * scale)).Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y))
			{
				float num = (float)Math.Abs(Math.Sin(Game1.hud.pulse));
				if (draw)
				{
					this.sprite.Draw(this.hudTex[1], loc, new Rectangle(880, 200, 57, 56), new Color(1f, 1f, 1f, (float)(int)color.A / 255f * num), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
				}
				if (this.leftMouseClicked)
				{
					this.leftMouseClicked = false;
					return true;
				}
			}
			return false;
		}

		public bool DrawMouseButton(Vector2 loc, float scale, Color color, int buttonType, bool draw, string text, TextAlign align)
		{
			float num = 0.7f;
			int num2 = (int)(Game1.smallFont.MeasureString(text).X * num) + 80;
			int height = (int)(Game1.smallFont.MeasureString(text).Y * num) + 10;
			switch (align)
			{
			case TextAlign.Left:
				loc.X += num2 + 20;
				break;
			case TextAlign.Center:
				loc.X += num2 / 2 - 40;
				break;
			}
			float num3 = (float)Math.Abs(Math.Sin(Game1.hud.pulse));
			bool flag = new Rectangle((int)loc.X - num2 + 20, (int)loc.Y, (int)(60f * scale) + num2 - 20, (int)(60f * scale)).Contains((int)Game1.hud.mousePos.X, (int)Game1.hud.mousePos.Y);
			if (draw)
			{
				Game1.hud.DrawMiniBorder(loc + new Vector2(40 - num2, 7f), num2, height, color, 1f);
				if (flag)
				{
					this.sprite.Draw(this.hudTex[1], loc + new Vector2(35 - num2, 5f), new Rectangle(886, 206, 50, 49), new Color(1f, 1f, 1f, (float)(int)color.A / 255f * num3 / 2f), 0f, Vector2.Zero, new Vector2((float)num2 / 48f, scale), SpriteEffects.None, 0f);
				}
				Game1.hud.DrawMiniBorder(loc + new Vector2(40 - num2, 7f), num2, height, color, 0f);
				Game1.smallText.Color = color;
				Game1.smallText.DrawText(loc + new Vector2(40 - num2, 12f), text, num, num2, TextAlign.Center);
			}
			if (flag && this.leftMouseClicked)
			{
				this.leftMouseClicked = false;
				return true;
			}
			return false;
		}

		public void DrawDebug(SpriteBatch sprite)
		{
			Game1.smallText.DrawText(new Vector2(20f, 300f), "X: " + this.mouseDiff.X + "\nY: " + this.mouseDiff.Y, 0.6f);
			Vector2 vector = new Vector2(300f, 10f);
			Game1.smallText.Color = Color.White;
			Game1.smallText.DrawText(vector, this.inputDevice.ToString(), 0.6f);
			float num = 0f;
			foreach (KeyValuePair<string, InputKey> inputKey in this.inputKeyList)
			{
				num += inputKey.Value.DrawTestKey(sprite, vector + new Vector2(0f, 15f + num), 0.6f);
			}
		}
	}
}
