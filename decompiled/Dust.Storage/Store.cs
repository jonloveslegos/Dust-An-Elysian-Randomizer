using System;
using System.IO;
using Dust.Audio;
using Dust.HUD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

namespace Dust.Storage
{
	public class Store
	{
		public const int STORE_SETTINGS = 0;

		public const int STORE_GAME = 1;

		public StoreResult storeResult;

		public StorageDevice device;

		private IAsyncResult deviceResult;

		public bool pendingDevice;

		public StorageContainer container;

		private bool containerOpen;

		public void GetDevice(PlayerIndex player)
		{
			if (this.container != null)
			{
				this.container.Dispose();
			}
			this.containerOpen = false;
			this.container = null;
			this.device = null;
			this.pendingDevice = true;
			if (!Game1.GamerServices || !Guide.IsVisible)
			{
				this.deviceResult = StorageDevice.BeginShowSelector(player, null, null);
			}
		}

		public void RemoveDevice()
		{
			if (!this.pendingDevice)
			{
				if (this.container != null)
				{
					this.container.Dispose();
				}
				this.containerOpen = false;
				this.container = null;
				this.device = null;
				this.pendingDevice = false;
				for (int i = 0; i < Game1.menu.fileManage.Length; i++)
				{
					Game1.menu.fileManage[i] = FileManage.StorageDeviceNull;
				}
			}
		}

		public void Update()
		{
			if ((!Game1.isPCBuild || Game1.gameMode == Game1.GameModes.MainMenu || Game1.hud.isPaused) && this.device != null && !this.device.IsConnected)
			{
				if (Game1.menu.menuMode == MenuMode.FileManage)
				{
					Game1.menu.confirming = false;
					for (int i = 0; i < Game1.menu.fileManage.Length; i++)
					{
						Game1.menu.fileManage[i] = FileManage.StorageDeviceNull;
					}
					Game1.menu.SelectStorage();
				}
				else if (Game1.gameMode == Game1.GameModes.MainMenu)
				{
					if (Game1.menu.menuMode == MenuMode.LeaderBoards)
					{
						Game1.menu.menuMode = MenuMode.None;
					}
					Sound.PlayCue("menu_cancel");
					Game1.menu.curMenuPage = 0;
					Game1.menu.curMenuOption = 0;
					Game1.menu.PopulateMenu();
					Game1.menu.SelectStorage();
				}
				else if (Game1.gameMode == Game1.GameModes.Game && Game1.menu.prompt == promptDialogue.None && !Game1.events.anyEvent && Game1.hud.inventoryState == InventoryState.None && Game1.hud.unlockState == 0 && Game1.map.warpStage == 0 && Game1.map.doorStage == 0 && Game1.menu.menuMode != MenuMode.Loading && Game1.menu.menuMode != MenuMode.Quitting)
				{
					if (Game1.cManager.challengeMode != 0 && Game1.cManager.challengeMode != ChallengeManager.ChallengeMode.InChallenge)
					{
						Game1.cManager.ExitScoreBoard(canRestart: false);
					}
					Game1.gameMode = Game1.GameModes.Game;
					Game1.menu.prompt = promptDialogue.StorageDisconnected;
					Game1.menu.ClearPrompt();
				}
			}
			if (this.pendingDevice)
			{
				while (this.deviceResult == null)
				{
				}
				if (this.deviceResult.IsCompleted)
				{
					this.device = StorageDevice.EndShowSelector(this.deviceResult);
					this.pendingDevice = false;
					this.Read(0);
				}
			}
		}

		private bool CheckDeviceFail()
		{
			if (this.pendingDevice)
			{
				return true;
			}
			if (Game1.GamerServices && Gamer.SignedInGamers[LogicalGamer.GetPlayerIndex(LogicalGamerIndex.One)] == null)
			{
				if (Game1.gameMode != 0)
				{
					Game1.hud.isPaused = false;
					Game1.gameMode = Game1.GameModes.Game;
					Game1.menu.menuMode = MenuMode.None;
					Game1.menu.prompt = promptDialogue.SignedOut;
					Game1.menu.ClearPrompt();
				}
				return true;
			}
			if (this.device == null)
			{
				if (Game1.gameMode != 0)
				{
					if (Game1.menu.prompt != promptDialogue.Failed && Game1.menu.menuMode != MenuMode.FileManage && Game1.menu.menuMode != MenuMode.Quitting)
					{
						Game1.hud.isPaused = false;
						Game1.gameMode = Game1.GameModes.Game;
						Game1.menu.menuMode = MenuMode.None;
						Game1.menu.prompt = promptDialogue.Failed;
						Game1.menu.ClearPrompt();
					}
				}
				else
				{
					for (int i = 0; i < Game1.menu.fileManage.Length; i++)
					{
						Game1.menu.fileManage[i] = FileManage.StorageDeviceNull;
					}
				}
				return true;
			}
			if (!this.device.IsConnected)
			{
				if (Game1.gameMode != 0)
				{
					if (Game1.menu.prompt != promptDialogue.StorageDisconnected)
					{
						Game1.hud.isPaused = false;
						Game1.gameMode = Game1.GameModes.Game;
						Game1.menu.prompt = promptDialogue.StorageDisconnected;
						Game1.menu.ClearPrompt();
					}
				}
				else
				{
					Game1.menu.menuMode = MenuMode.None;
					Game1.menu.curMenuPage = 5;
					Game1.menu.curMenuOption = 0;
					Game1.menu.PopulateMenu();
				}
				return true;
			}
			return false;
		}

		private void OpenContainer()
		{
			if (this.device != null && !this.containerOpen)
			{
				this.container = null;
				IAsyncResult asyncResult = this.device.BeginOpenContainer("Save Files", null, null);
				asyncResult.AsyncWaitHandle.WaitOne();
				this.container = this.device.EndOpenContainer(asyncResult);
				asyncResult.AsyncWaitHandle.Close();
				if (this.container != null)
				{
					this.containerOpen = true;
				}
			}
		}

		private string GetStoreString(int type, int slot)
		{
			string[] array = new string[2]
			{
				"Settings.sav",
				"Game Save " + slot + ".sav"
			};
			return array[type];
		}

		public void Write(int type, int saveSlot)
		{
			int num = 0;
			while (Game1.questManager.updatingQuests)
			{
				num++;
				if (num > 1000)
				{
					break;
				}
			}
			if (this.CheckDeviceFail())
			{
				this.storeResult = StoreResult.StorageNull;
				return;
			}
			this.OpenContainer();
			try
			{
				Stream stream = this.container.OpenFile(this.GetStoreString(type, Math.Max(saveSlot, 0)), FileMode.OpenOrCreate, FileAccess.Write);
				BinaryWriter writer = new BinaryWriter(stream);
				switch (type)
				{
				case 0:
					Game1.settings.Write(writer);
					break;
				case 1:
					Game1.stats.saveSlot = (byte)saveSlot;
					Game1.savegame.Write(writer);
					break;
				}
				stream.Close();
				this.storeResult = StoreResult.Saved;
			}
			catch (Exception)
			{
				this.storeResult = StoreResult.StorageFull;
			}
		}

		public void Read(int type)
		{
			int num = 0;
			while (Game1.questManager.updatingQuests)
			{
				num++;
				if (num > 1000)
				{
					break;
				}
			}
			if (this.CheckDeviceFail())
			{
				this.storeResult = StoreResult.StorageNull;
				return;
			}
			Game1.stats.ResetTimers();
			this.OpenContainer();
			try
			{
				Stream stream = this.container.OpenFile(this.GetStoreString(type, Game1.stats.saveSlot), FileMode.Open, FileAccess.Read);
				BinaryReader reader = new BinaryReader(stream);
				switch (type)
				{
				case 0:
					Game1.settings.Read(reader);
					break;
				case 1:
					if (!Game1.savegame.Read(reader))
					{
						this.storeResult = StoreResult.Corrupt;
						stream.Close();
						Game1.hud.SignOut(Game1.pManager);
						Game1.events.ReturnToTitle();
						return;
					}
					break;
				}
				this.storeResult = StoreResult.Loaded;
				stream.Close();
			}
			catch (Exception)
			{
				this.storeResult = StoreResult.Corrupt;
			}
		}

		public void Delete(int slot)
		{
			if (this.CheckDeviceFail())
			{
				this.storeResult = StoreResult.StorageNull;
				return;
			}
			this.OpenContainer();
			this.container.DeleteFile(this.GetStoreString(1, slot));
			this.storeResult = StoreResult.Deleted;
		}

		public void Copy(int source, int dest)
		{
			if (this.CheckDeviceFail())
			{
				this.storeResult = StoreResult.StorageNull;
				return;
			}
			this.OpenContainer();
			try
			{
				Stream stream = this.container.OpenFile(this.GetStoreString(1, source), FileMode.Open, FileAccess.Read);
				Stream stream2 = this.container.OpenFile(this.GetStoreString(1, dest), FileMode.OpenOrCreate, FileAccess.Write);
				byte[] array = new byte[4096];
				int count;
				while ((count = stream.Read(array, 0, array.Length)) > 0)
				{
					stream2.Write(array, 0, count);
				}
				stream.Close();
				stream2.Close();
				this.storeResult = StoreResult.Copied;
			}
			catch (Exception)
			{
				this.storeResult = StoreResult.StorageNull;
			}
		}

		public int Check(int slot)
		{
			if (this.CheckDeviceFail())
			{
				return -1;
			}
			this.OpenContainer();
			try
			{
				if (!this.container.FileExists(this.GetStoreString(1, slot)))
				{
					return 0;
				}
				Stream stream = this.container.OpenFile(this.GetStoreString(1, slot), FileMode.Open);
				BinaryReader reader = new BinaryReader(stream);
				Game1.savegame.CheckMemory(reader, slot);
				stream.Close();
			}
			catch (Exception)
			{
				this.storeResult = StoreResult.Corrupt;
				return -2;
			}
			return 1;
		}

		public void DisposeContainer()
		{
			this.containerOpen = false;
			if (this.container != null)
			{
				this.container.Dispose();
				this.container = null;
			}
		}
	}
}
