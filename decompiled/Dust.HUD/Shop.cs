// Dust.HUD.Shop
using System;
using System.Collections.Generic;
using Dust;
using Dust.Audio;
using Dust.CharClasses;
using Dust.HUD;
using Dust.MapClasses;
using Dust.Particles;
using Dust.PCClasses;
using Dust.Strings;
using Lotus.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Shop
{
	private static SpriteBatch sprite;

	private static Texture2D[] particlesTex;

	private static Texture2D[] hudTex;

	private static Texture2D shopTex;

	private static Texture2D portraitTex;

	private static Texture2D nullTex;

	private static Texture2D numbersTex;

	public bool shopLoaded;

	private static LoadState portraitLoaded;

	private static PurchaseMode purchaseMode;

	private static ShopListType listType;

	private static bool canTransact;

	private static bool confirming;

	private static float confirmColor;

	private float categoryHighlight;

	private static int switchState = 0;

	private static float switchTime = 0f;

	private static string switchText = string.Empty;

	private static string itemInfoDisplay = string.Empty;

	private static string itemStats = string.Empty;

	private static List<Vector3> equipList = new List<Vector3>();

	private static List<Vector3> shopItemList = new List<Vector3>();

	private static int maxCategory;

	private static int shopCategory;

	private static int finalItem;

	private static int shopItem;

	private static int playerStock;

	public int multiplier;

	private static Vector2 shopItemListPos;

	public bool canInput = true;

	private static float shopAlpha;

	private static float portraitAlpha;

	private static int portraitID;

	private static int animFrame = 0;

	private static float animFrameTime = 0f;

	private static GamePadState curState = default(GamePadState);

	private static GamePadState prevState = default(GamePadState);

	private Character[] character;

	private Map map;

	private ScoreDraw scoreDraw;

	private static int coinAnimFrame;

	private static float coinAnimFrameTime = 0f;

	private static float itemAlpha = 0f;

	private static float itemIconPos;

	private Vector2 mousePos;

	private float holdMoveTime;

	private bool KeyLeft;

	private bool KeyRight;

	private bool KeyUp;

	private bool KeyDown;

	public bool KeySelect;

	private bool KeyCancel;

	private bool KeyX;

	private bool KeyY;

	private bool KeyRightBumper;

	private bool KeyLeftBumper;

	public Shop(SpriteBatch _sprite, Texture2D[] _particlesTex, Texture2D _nullTex, Texture2D[] _hudTex, Texture2D _numbersTex, Character[] _character, Map _map)
	{
		Shop.sprite = _sprite;
		Shop.particlesTex = _particlesTex;
		Shop.hudTex = _hudTex;
		this.character = _character;
		this.map = _map;
		Shop.nullTex = _nullTex;
		Shop.numbersTex = _numbersTex;
		this.scoreDraw = new ScoreDraw(Shop.sprite, Shop.numbersTex);
		Game1.inventoryManager.itemName = string.Empty;
		Game1.inventoryManager.itemInfo = string.Empty;
		Game1.inventoryManager.itemStats = string.Empty;
		Game1.inventoryManager.itemCost = 0;
	}

	private void DoInput(int index)
	{
		Shop.curState = GamePad.GetState((PlayerIndex)index);
		this.KeyLeft = false;
		this.KeyRight = false;
		this.KeySelect = false;
		this.KeyUp = false;
		this.KeyDown = false;
		this.KeyX = false;
		this.KeyY = false;
		this.KeyCancel = false;
		this.KeyRightBumper = false;
		this.KeyLeftBumper = false;
		if (Shop.switchState > 0 || Shop.shopAlpha < 1f)
		{
			return;
		}
		if (Game1.isPCBuild)
		{
			Game1.hud.mousePos = (this.mousePos = Game1.pcManager.GetMouseLoc());
			Game1.pcManager.UpdateShopInput(ref this.KeyLeft, ref this.KeyRight, ref this.KeyUp, ref this.KeyDown, ref this.KeySelect, ref this.KeyCancel, ref this.KeyX, ref this.KeyY, ref this.KeyLeftBumper, ref this.KeyRightBumper);
		}
		if (Shop.curState.ThumbSticks.Left.X != 0f || Shop.curState.ThumbSticks.Left.Y != 0f || Shop.curState.DPad.Up == ButtonState.Pressed || Shop.curState.DPad.Down == ButtonState.Pressed || Shop.curState.DPad.Right == ButtonState.Pressed || Shop.curState.DPad.Left == ButtonState.Pressed || Shop.curState.Buttons.LeftShoulder == ButtonState.Pressed || Shop.curState.Buttons.RightShoulder == ButtonState.Pressed)
		{
			if (this.holdMoveTime > 0f)
			{
				this.holdMoveTime -= Game1.HudTime;
				if (this.holdMoveTime < 0f)
				{
					this.holdMoveTime = 0f;
				}
			}
			else if (Game1.longSkipFrame > 3)
			{
				if (Shop.curState.ThumbSticks.Left.Y > 0f || Shop.curState.DPad.Up == ButtonState.Pressed)
				{
					this.KeyUp = true;
				}
				else if (Shop.curState.ThumbSticks.Left.Y < 0f || Shop.curState.DPad.Down == ButtonState.Pressed)
				{
					this.KeyDown = true;
				}
				if (Shop.curState.ThumbSticks.Left.X < 0f || Shop.curState.DPad.Left == ButtonState.Pressed)
				{
					this.KeyLeft = true;
				}
				else if (Shop.curState.ThumbSticks.Left.X > 0f || Shop.curState.DPad.Right == ButtonState.Pressed)
				{
					this.KeyRight = true;
				}
				if (Shop.curState.Buttons.LeftShoulder == ButtonState.Pressed)
				{
					this.KeyLeftBumper = true;
				}
				else if (Shop.curState.Buttons.RightShoulder == ButtonState.Pressed)
				{
					this.KeyRightBumper = true;
				}
			}
		}
		else
		{
			this.holdMoveTime = 0.5f;
		}
		if ((Shop.curState.ThumbSticks.Left.X < -0.2f && Shop.prevState.ThumbSticks.Left.X > -0.2f) || (Shop.curState.DPad.Left == ButtonState.Pressed && Shop.prevState.DPad.Left == ButtonState.Released))
		{
			this.KeyLeft = true;
		}
		else if ((Shop.curState.ThumbSticks.Left.X > 0.2f && Shop.prevState.ThumbSticks.Left.X < 0.2f) || (Shop.curState.DPad.Right == ButtonState.Pressed && Shop.prevState.DPad.Right == ButtonState.Released))
		{
			this.KeyRight = true;
		}
		if ((Shop.prevState.ThumbSticks.Left.Y < 0.15f && Shop.curState.ThumbSticks.Left.Y > 0.15f) || (Shop.curState.DPad.Up == ButtonState.Pressed && Shop.prevState.DPad.Up == ButtonState.Released))
		{
			this.KeyUp = true;
		}
		if ((Shop.prevState.ThumbSticks.Left.Y > -0.15f && Shop.curState.ThumbSticks.Left.Y < -0.15f) || (Shop.curState.DPad.Down == ButtonState.Pressed && Shop.prevState.DPad.Down == ButtonState.Released))
		{
			this.KeyDown = true;
		}
		if (Shop.curState.Buttons.A == ButtonState.Pressed && Shop.prevState.Buttons.A == ButtonState.Released)
		{
			if (Game1.inventoryManager.itemCost > 0)
			{
				this.KeySelect = true;
				if (!Shop.confirming)
				{
					this.multiplier = 1;
				}
			}
			else
			{
				Sound.PlayCue("shop_fail");
			}
		}
		if (Shop.curState.Buttons.B == ButtonState.Pressed && Shop.prevState.Buttons.B == ButtonState.Released)
		{
			this.KeyCancel = true;
		}
		if (Shop.curState.Buttons.Y == ButtonState.Pressed && Shop.prevState.Buttons.Y == ButtonState.Released)
		{
			this.KeyY = true;
		}
		if (Shop.curState.Buttons.X == ButtonState.Pressed && Shop.prevState.Buttons.X == ButtonState.Released)
		{
			this.KeyX = true;
		}
		if (Shop.curState.Buttons.LeftShoulder == ButtonState.Pressed && Shop.prevState.Buttons.LeftShoulder == ButtonState.Released)
		{
			this.KeyLeftBumper = true;
		}
		if (Shop.curState.Buttons.RightShoulder == ButtonState.Pressed && Shop.prevState.Buttons.RightShoulder == ButtonState.Released)
		{
			this.KeyRightBumper = true;
		}
		Shop.prevState = Shop.curState;
	}

	private void SetTextures()
	{
		this.shopLoaded = false;
		Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadTextures), new TaskFinishedDelegate(LoadingFinished)));
	}

	private void LoadTextures()
	{
		Shop.shopTex = Game1.GetLargeContent().Load<Texture2D>("gfx/ui/shop_elements");
	}

	private void LoadingFinished(int taskId)
	{
		this.shopLoaded = true;
		Game1.gameMode = Game1.GameModes.Game;
	}

	private void SetPortraitTexture()
	{
		Shop.portraitLoaded = LoadState.Loading;
		Game1.loadingThread.AddTask(new ThreadTask(new ThreadTaskDelegate(LoadPortraitTexture), new TaskFinishedDelegate(LoadingPortraitFinished)));
	}

	private void LoadPortraitTexture()
	{
		Shop.portraitTex = Game1.GetLargeContent().Load<Texture2D>("gfx/portraits/portrait_" + $"{Shop.portraitID:D2}");
	}

	private void LoadingPortraitFinished(int taskId)
	{
		Shop.portraitLoaded = LoadState.Loaded;
		Game1.gameMode = Game1.GameModes.Game;
	}

	public void UnloadTextures()
	{
		Game1.GetLargeContent().Unload();
		this.shopLoaded = false;
		Shop.portraitLoaded = LoadState.NotLoaded;
	}

	private void PopulateItemList()
	{
		Shop.shopItemList.Clear();
		if (Shop.listType == ShopListType.Materials)
		{
			Game1.hud.materialList.Clear();
			for (int i = 0; i < Game1.stats.Material.Length; i++)
			{
				if (Game1.stats.Material[i] > -1)
				{
					Game1.hud.materialList.Add(new Vector3(i, Game1.stats.Material[i], 0f));
				}
			}
			for (int j = 0; j < Game1.stats.shopMaterial.Length; j++)
			{
				if (Game1.stats.shopMaterial[j] > -1)
				{
					Shop.shopItemList.Add(new Vector3(j, Game1.stats.shopMaterial[j], 0f));
				}
			}
			return;
		}
		Shop.equipList.Clear();
		int categoryID = this.GetCategoryID();
		for (int k = categoryID * Game1.inventoryManager.invSelMax; k < (categoryID + 1) * Game1.inventoryManager.invSelMax; k++)
		{
			if (Game1.inventoryManager.equipItem[k] == null)
			{
				continue;
			}
			if (k < 320 && Game1.stats.Equipment[k] > 0)
			{
				Shop.equipList.Add(new Vector3(k, (int)Game1.stats.Equipment[k], 0f));
			}
			if (k < 320)
			{
				if (Game1.hud.shopType == ShopType.Shop && ((Shop.purchaseMode == PurchaseMode.Buy) ? Game1.stats.shopEquipment[k] : Game1.stats.Equipment[k]) > 0)
				{
					Shop.shopItemList.Add(new Vector3(k, (int)((Shop.purchaseMode == PurchaseMode.Buy) ? Game1.stats.shopEquipment[k] : Game1.stats.Equipment[k]), 0f));
				}
				if (k >= Game1.inventoryManager.invSelMax && Game1.hud.shopType != ShopType.Shop && Game1.stats.EquipBluePrint[k - Game1.inventoryManager.invSelMax] > 0)
				{
					Shop.shopItemList.Add(new Vector3(k, (int)Game1.stats.EquipBluePrint[k - Game1.inventoryManager.invSelMax], 1f));
				}
			}
		}
	}

	private void PopulateDescription()
	{
		Shop.canTransact = false;
		Game1.inventoryManager.itemControls = Game1.smallText.WordWrap(this.PopulateControls(), 0.7f, 380f, Game1.inventoryManager.itemControlsButtonList, TextAlign.LeftAndCenter);
		Shop.playerStock = 0;
		Shop.finalItem = this.GetItemID(Shop.shopItem);
		if (Shop.finalItem < 0)
		{
			return;
		}
		if (Shop.listType == ShopListType.Equipment)
		{
			Game1.inventoryManager.PopulateDescription(isShopping: true, Shop.finalItem);
			Shop.playerStock = Game1.stats.Equipment[Shop.finalItem];
			if (Shop.purchaseMode == PurchaseMode.Buy)
			{
				if (Game1.hud.shopType != ShopType.Shop)
				{
					bool flag = true;
					if (Game1.inventoryManager.equipItem[Shop.finalItem] != null)
					{
						int num = 0;
						for (int i = 0; i < Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq.Length; i++)
						{
							if (Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[i] > -1)
							{
								num++;
							}
						}
						for (int j = 0; j < num; j++)
						{
							int num2 = Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[j];
							if (num2 > -1 && Game1.stats.Material[num2] < Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReqAmt[j])
							{
								flag = false;
							}
						}
					}
					else
					{
						flag = false;
					}
					Shop.canTransact = flag && this.CanTransact(0);
					if (this.CheckVisible(Shop.shopItem) == 0)
					{
						Game1.inventoryManager.itemName = (Game1.inventoryManager.itemStats = (Game1.inventoryManager.itemInfo = string.Empty));
						Game1.inventoryManager.itemCost = 0;
					}
				}
				else
				{
					Shop.canTransact = this.CanTransact(0);
				}
			}
			else
			{
				Shop.canTransact = this.CanTransact(0);
			}
			Shop.itemInfoDisplay = Game1.smallText.WordWrap(Game1.inventoryManager.itemInfo, 0.8f, 380f, TextAlign.Left);
			Shop.itemStats = Game1.smallText.WordWrap(Game1.inventoryManager.itemStats, 0.8f, 380f, TextAlign.Left);
			if (this.CheckEquipped(Shop.finalItem))
			{
				Shop.itemStats = Game1.smallText.WordWrap(Game1.inventoryManager.itemStats + "[N]" + Strings_Shop.ItemEquipped, 0.8f, 380f, TextAlign.Left);
			}
			return;
		}
		Shop.playerStock = 0;
		Shop.itemInfoDisplay = (Game1.inventoryManager.itemName = (Shop.itemStats = (Game1.inventoryManager.itemInfo = string.Empty)));
		Game1.inventoryManager.itemCost = 0;
		if (this.ItemList().Count > 0 && Game1.inventoryManager.material[(int)this.ItemList()[Shop.shopItem].X] != null)
		{
			Shop.playerStock = Game1.stats.Material[(int)this.ItemList()[Shop.shopItem].X];
			if ((int)this.ItemList()[Shop.shopItem].Y > -1 && Game1.inventoryManager.material[(int)this.ItemList()[Shop.shopItem].X] != null)
			{
				Game1.inventoryManager.itemName = Game1.inventoryManager.material[(int)this.ItemList()[Shop.shopItem].X].name;
				Shop.itemInfoDisplay = Game1.smallText.WordWrap(Game1.inventoryManager.material[(int)this.ItemList()[Shop.shopItem].X].getDescription, 0.8f, 380f, TextAlign.Left);
				Game1.inventoryManager.itemCost = Game1.inventoryManager.material[(int)this.ItemList()[Shop.shopItem].X].value;
				if (Game1.stats.shopMaterial[(int)this.ItemList()[Shop.shopItem].X] > -1)
				{
					Shop.itemStats = Game1.smallText.WordWrap(Strings_Shop.MaterialCatalogued, 0.8f, 380f, TextAlign.Left);
				}
				if (Game1.stats.shopMaterial[(int)this.ItemList()[Shop.shopItem].X] > 0)
				{
					Shop.itemStats = Game1.smallText.WordWrap(Shop.itemStats + " " + Strings_Shop.MaterialInStock, 0.8f, 380f, TextAlign.Left);
				}
			}
		}
		Shop.canTransact = this.CanTransact(0);
	}

	private string PopulateControls()
	{
		string text;
		if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
		{
			text = ((Shop.purchaseMode != 0) ? Strings_PC.ShopBuy : Strings_PC.ShopSell);
		}
		else
		{
			if (Shop.purchaseMode == PurchaseMode.Buy)
			{
				text = Strings_Shop.ShopControlsBuy;
				if (Game1.hud.shopType == ShopType.Shop)
				{
					text = Strings_Shop.ShopControlsSwitch + "[N]" + text;
				}
			}
			else
			{
				text = Strings_Shop.ShopControlsSell;
			}
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardOnly)
			{
				text = Strings_PC.ShopPageSwitch + "\n" + text;
			}
		}
		return text;
	}

	private List<Vector3> ItemList()
	{
		if (Shop.purchaseMode == PurchaseMode.Buy)
		{
			return Shop.shopItemList;
		}
		if (Shop.listType == ShopListType.Materials)
		{
			return Game1.hud.materialList;
		}
		return Shop.equipList;
	}

	private int GetCategoryID()
	{
		if (Game1.hud.shopType == ShopType.Blacksmith)
		{
			if (Shop.shopCategory == 4)
			{
				return 0;
			}
			return Shop.shopCategory + 1;
		}
		return Shop.shopCategory;
	}

	private int GetItemID(int selection)
	{
		if (Shop.purchaseMode == PurchaseMode.Buy)
		{
			if (Shop.shopItemList.Count < 1 || selection >= Shop.shopItemList.Count)
			{
				return -1;
			}
			return (int)Shop.shopItemList[selection].X;
		}
		if (Shop.listType == ShopListType.Materials)
		{
			if (Game1.hud.materialList.Count < 1 || selection >= Game1.hud.materialList.Count)
			{
				return -1;
			}
			return (int)Game1.hud.materialList[selection].X;
		}
		if (Shop.equipList.Count < 1 || selection >= Shop.equipList.Count)
		{
			return -1;
		}
		return (int)Shop.equipList[selection].X;
	}

	private bool CanTransact(int amount)
	{
		int num = 250;
		if (Shop.listType == ShopListType.Equipment)
		{
			if (Shop.purchaseMode == PurchaseMode.Sell)
			{
				if (amount < Game1.stats.Equipment[Shop.finalItem])
				{
					return true;
				}
			}
			else if (amount < num - Game1.stats.Equipment[Shop.finalItem] && (Game1.hud.shopType == ShopType.Blacksmith || amount < Game1.stats.shopEquipment[Shop.finalItem]) && Game1.stats.Gold >= Game1.inventoryManager.itemCost * (amount + 1) / ((Game1.hud.shopType != ShopType.Blacksmith) ? 1 : 4))
			{
				return true;
			}
		}
		else if (this.ItemList().Count > 0)
		{
			num = 9999;
			if (Shop.purchaseMode == PurchaseMode.Sell)
			{
				num = 10000000;
			}
			int num2 = (int)this.ItemList()[Shop.shopItem].X;
			if (Game1.inventoryManager.material[num2] != null)
			{
				if (Shop.purchaseMode == PurchaseMode.Sell)
				{
					if (amount < Game1.stats.Material[num2] && amount < num - Game1.stats.shopMaterial[num2])
					{
						return true;
					}
				}
				else if (amount < num - Game1.stats.Material[num2] && amount < Game1.stats.shopMaterial[num2] && Game1.stats.Gold >= Game1.inventoryManager.itemCost * (amount + 1))
				{
					return true;
				}
			}
		}
		return false;
	}

	private int CheckVisible(int selection)
	{
		if (Game1.hud.shopType == ShopType.Shop || Shop.listType == ShopListType.Materials)
		{
			return 1;
		}
		int itemID = this.GetItemID(selection);
		if (itemID < 0)
		{
			return 0;
		}
		if (Game1.hud.shopType == ShopType.Shop && Game1.stats.Equipment[itemID] > 0)
		{
			return 1;
		}
		if (itemID >= Game1.inventoryManager.invSelMax && Game1.stats.EquipBluePrint[itemID - Game1.inventoryManager.invSelMax] > 0)
		{
			return 2;
		}
		return 0;
	}

	private bool CheckEquipped(int i)
	{
		switch (Shop.shopCategory)
		{
		case 0:
			if (i == Game1.stats.currentItem)
			{
				return true;
			}
			break;
		case 1:
			if (i == Game1.stats.currentPendant)
			{
				return true;
			}
			break;
		case 2:
			if (i == Game1.stats.currentAugment)
			{
				return true;
			}
			break;
		case 3:
			if (i == Game1.stats.currentArmor)
			{
				return true;
			}
			break;
		case 4:
			if (i == Game1.stats.currentRingLeft)
			{
				return true;
			}
			if (i == Game1.stats.currentRingRight)
			{
				return true;
			}
			break;
		}
		return false;
	}

	private void BuyItem(ParticleManager pMan)
	{
		if (Game1.hud.shopType == ShopType.Blacksmith && Shop.listType == ShopListType.Equipment)
		{
			for (int i = 0; i < Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq.Length; i++)
			{
				if (Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[i] > -1)
				{
					Game1.stats.Material[Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[i]] -= Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReqAmt[i];
				}
			}
			this.multiplier = 1;
			Game1.stats.EquipBluePrint[Shop.finalItem - Game1.inventoryManager.invSelMax]--;
			Game1.awardsManager.EarnAchievement(Achievement.BlueprintCraft, forceCheck: false);
		}
		if (Shop.listType == ShopListType.Equipment)
		{
			if (Game1.hud.shopType == ShopType.Shop && Game1.stats.ReturnChestItems("Shop " + Game1.inventoryManager.itemName + " " + ((int)Game1.stats.shopEquipGotten[Shop.finalItem]).ToString()).Count > 0)
			{
				this.multiplier = 1;
				Game1.stats.GetChestFromFileNoText("Shop " + Game1.inventoryManager.itemName + " " + ((int)Game1.stats.shopEquipGotten[Shop.finalItem]).ToString());
			}
			else
			{
				Game1.stats.Equipment[Shop.finalItem] += (byte)this.multiplier;
			}
			Game1.stats.shopEquipGotten[Shop.finalItem] += (byte)this.multiplier;
			if (Game1.stats.shopEquipment[Shop.finalItem] < byte.MaxValue && Game1.hud.shopType == ShopType.Shop)
			{
				Game1.stats.shopEquipment[Shop.finalItem] -= (byte)this.multiplier;
			}
		}
		else
		{
			Game1.stats.Material[Shop.finalItem] = Math.Max(Game1.stats.Material[Shop.finalItem], 0);
			Game1.stats.Material[Shop.finalItem] += this.multiplier;
			Game1.stats.shopMaterial[Shop.finalItem] -= this.multiplier;
		}
		this.KeySelect = false;
		Sound.PlayCue("shop_buy");
		if (Game1.hud.shopType == ShopType.Blacksmith && Shop.listType == ShopListType.Equipment)
		{
			Game1.stats.Gold -= Game1.inventoryManager.itemCost / 4 * this.multiplier;
		}
		else
		{
			Game1.stats.Gold -= Game1.inventoryManager.itemCost * this.multiplier;
		}
		this.PopulateItemList();
		Shop.shopItem = (int)MathHelper.Clamp(max: ((Shop.listType != 0) ? ((Shop.purchaseMode == PurchaseMode.Buy) ? Shop.shopItemList.Count : Game1.hud.materialList.Count) : ((Shop.purchaseMode == PurchaseMode.Buy) ? Shop.shopItemList.Count : Shop.equipList.Count)) - 1, value: Shop.shopItem, min: 0f);
		this.PopulateDescription();
		Shop.SpawnEffect(pMan);
		if (Game1.hud.shopType == ShopType.Blacksmith && Shop.listType == ShopListType.Equipment && Game1.hud.inventoryState == InventoryState.Equipment)
		{
			this.ExitShop();
		}
	}

	private void SellItem(ParticleManager pMan)
	{
		int val = 250;
		if (Shop.listType == ShopListType.Equipment)
		{
			Game1.inventoryManager.RemoveEquip(Shop.shopCategory, Shop.finalItem, this.multiplier);
			Game1.stats.Equipment[Shop.finalItem] = (byte)Math.Max(Game1.stats.Equipment[Shop.finalItem] - this.multiplier, 0);
			if (Game1.stats.shopEquipment[Shop.finalItem] < byte.MaxValue)
			{
				Game1.stats.shopEquipment[Shop.finalItem] = (byte)Math.Min(Game1.stats.shopEquipment[Shop.finalItem] + this.multiplier, val);
			}
		}
		else
		{
			val = 9999;
			if (Game1.stats.shopMaterial[Shop.finalItem] < 0)
			{
				Shop.switchState = 1;
				Shop.switchTime = 0.1f;
				Shop.switchText = Strings_Shop.ItemStocked;
			}
			Game1.stats.Material[Shop.finalItem] -= this.multiplier;
			if (Game1.stats.shopMaterial[Shop.finalItem] < 1)
			{
				Game1.questManager.SetQuestStage(5, 1);
			}
			Game1.stats.shopMaterial[Shop.finalItem] = Math.Min(Math.Max(Game1.stats.shopMaterial[Shop.finalItem], 0) + this.multiplier, val);
		}
		this.KeySelect = false;
		Sound.PlayCue("shop_buy");
		Game1.stats.Gold += Game1.inventoryManager.itemCost / 4 * this.multiplier;
		this.PopulateItemList();
		Shop.shopItem = (int)MathHelper.Clamp(max: ((Shop.listType != 0) ? ((Shop.purchaseMode == PurchaseMode.Buy) ? Shop.shopItemList.Count : Game1.hud.materialList.Count) : ((Shop.purchaseMode == PurchaseMode.Buy) ? Shop.shopItemList.Count : Shop.equipList.Count)) - 1, value: Shop.shopItem, min: 0f);
		this.PopulateDescription();
		Shop.SpawnEffect(pMan);
	}

	private void SwitchBuySell()
	{
		if (!Shop.confirming && Shop.switchState == 0)
		{
			Shop.switchState = 1;
			Shop.switchTime = 0.1f;
			if (Shop.purchaseMode == PurchaseMode.Buy)
			{
				Shop.switchText = Strings_Shop.PurchaseModeSell;
				Shop.purchaseMode = PurchaseMode.Sell;
			}
			else
			{
				Shop.switchText = Strings_Shop.PurchaseModeBuy;
				Shop.purchaseMode = PurchaseMode.Buy;
			}
			Shop.shopItem = 0;
			Shop.shopItemListPos.Y = Game1.screenHeight / 2 - 115;
			Sound.PlayCue("menu_confirm");
			if ((Game1.hud.shopType == ShopType.Shop && Shop.shopCategory == 6) || (Game1.hud.shopType == ShopType.Blacksmith && Shop.shopCategory == 4))
			{
				Shop.listType = ShopListType.Materials;
			}
			else
			{
				Shop.listType = ShopListType.Equipment;
			}
			this.PopulateItemList();
		}
	}

	private static void SpawnEffect(ParticleManager pMan)
	{
		Vector2 vector = new Vector2(Game1.screenWidth / 2 - 50, Game1.screenHeight / 2 + 260);
		for (int i = 0; i < 10; i++)
		{
			pMan.AddHudCoin(vector, new Vector2(Rand.GetRandomInt(-200, 200), Rand.GetRandomInt(-600, -200)));
			pMan.AddSparkle(vector + Rand.GetRandomVector2(150f, 380f, 500f, 570f), 1f, 0.5f, 0.5f, 1f, 0.5f, Rand.GetRandomInt(30, 80), 9);
		}
	}

	public void InitShop(int shopID, int selectedCategory, int selectedItem, bool blueprint)
	{
		switch (shopID)
		{
		case 100:
			if (Game1.hud.inventoryState == InventoryState.None)
			{
				Shop.portraitID = 70;
			}
			else
			{
				Shop.portraitID = 73;
			}
			Game1.hud.shopType = ShopType.Blacksmith;
			Shop.maxCategory = 5;
			Sound.PlayCue("shop_craft_item");
			break;
		case 101:
			Shop.portraitID = 42;
			Game1.hud.shopType = ShopType.Architect;
			Shop.maxCategory = 5;
			Sound.PlayCue("shop_craft_building");
			break;
		default:
			switch (shopID)
			{
			default:
				Shop.portraitID = 42;
				break;
			case 110:
				Shop.portraitID = 51;
				break;
			case 111:
				Shop.portraitID = 51;
				break;
			}
			Game1.hud.shopType = ShopType.Shop;
			Shop.maxCategory = 7;
			break;
		}
		Shop.purchaseMode = PurchaseMode.Buy;
		Shop.listType = ShopListType.Equipment;
		if (selectedItem > -1)
		{
			Shop.shopCategory = selectedCategory - 1;
			this.PopulateItemList();
			Shop.shopItem = selectedItem;
			for (int i = 0; i < Shop.shopItemList.Count; i++)
			{
				if (Shop.shopItemList[i].X == (float)selectedItem)
				{
					Shop.shopItem = i;
				}
			}
			this.KeySelect = true;
		}
		else
		{
			Shop.shopCategory = 0;
			this.PopulateItemList();
		}
		this.PopulateDescription();
		Shop.shopAlpha = 0f;
		Game1.hud.tempGold = Game1.stats.Gold;
		Shop.portraitLoaded = LoadState.NotLoaded;
		if (!this.shopLoaded)
		{
			this.SetTextures();
		}
	}

	private void ExitShop()
	{
		if (!Shop.confirming)
		{
			Shop.shopCategory = 0;
			Shop.shopItem = 0;
			Shop.confirmColor = 0f;
			Shop.shopAlpha = 0f;
			Shop.purchaseMode = PurchaseMode.Buy;
			this.shopLoaded = false;
			Shop.shopItemList.Clear();
			Game1.hud.materialList.Clear();
			Game1.hud.hudDim = 1f;
			this.UnloadTextures();
			GC.Collect();
		}
		else
		{
			this.PopulateDescription();
		}
		Sound.PlayCue("menu_cancel");
		Shop.confirming = false;
		Game1.hud.ExitShop();
	}

	public void UpdateShop(ParticleManager pMan, Character[] c)
	{
		this.canInput = false;
		if (!this.shopLoaded)
		{
			return;
		}
		Game1.events.anyEvent = false;
		Game1.BlurScene(1f);
		if (!Shop.confirming)
		{
			bool flag = false;
			int num = ((Shop.listType != 0) ? ((Shop.purchaseMode == PurchaseMode.Buy) ? Shop.shopItemList.Count : Game1.hud.materialList.Count) : ((Shop.purchaseMode == PurchaseMode.Buy) ? Shop.shopItemList.Count : Shop.equipList.Count));
			int num2 = 3;
			int num3 = Shop.shopItem;
			if (this.KeyLeft)
			{
				Shop.shopItem--;
			}
			if (this.KeyRight)
			{
				Shop.shopItem++;
			}
			if (this.KeyUp)
			{
				Shop.shopItem -= num2;
			}
			if (this.KeyDown)
			{
				Shop.shopItem += num2;
			}
			Shop.shopItem = (int)MathHelper.Clamp(Shop.shopItem, 0f, num - 1);
			if (num3 != Shop.shopItem)
			{
				Sound.PlayCue("menu_click");
				Shop.itemAlpha = 0f;
			}
			if ((Game1.pcManager.inputDevice == InputDevice.GamePad && this.KeyLeftBumper) || Game1.pcManager.KeyInvLeft)
			{
				this.KeyLeftBumper = (Game1.pcManager.KeyInvLeft = false);
				flag = true;
				Shop.shopCategory--;
				if (Shop.shopCategory < 0)
				{
					Shop.shopCategory = Shop.maxCategory - 1;
				}
			}
			else if ((Game1.pcManager.inputDevice == InputDevice.GamePad && this.KeyRightBumper) || Game1.pcManager.KeyInvRight)
			{
				this.KeyRightBumper = (Game1.pcManager.KeyInvRight = false);
				flag = true;
				Shop.shopCategory++;
				if (Shop.shopCategory >= Shop.maxCategory)
				{
					Shop.shopCategory = 0;
				}
			}
			if (flag)
			{
				Sound.PlayCue("menu_page_turn");
				Shop.shopItem = 0;
				Shop.shopItemListPos.Y = Game1.screenHeight / 2 - 115;
				Shop.itemAlpha = 0f;
				if (Game1.hud.shopType == ShopType.Blacksmith && Shop.purchaseMode == PurchaseMode.Sell)
				{
					Shop.switchState = 1;
					Shop.switchTime = 0.1f;
					Shop.switchText = Strings_Shop.PurchaseModeBuy;
					Shop.purchaseMode = PurchaseMode.Buy;
				}
				if ((Game1.hud.shopType == ShopType.Shop && Shop.shopCategory == 6) || (Game1.hud.shopType == ShopType.Blacksmith && Shop.shopCategory == 4))
				{
					Shop.listType = ShopListType.Materials;
				}
				else
				{
					Shop.listType = ShopListType.Equipment;
				}
				this.PopulateItemList();
				this.PopulateDescription();
			}
		}
		if (Shop.confirming)
		{
			if (this.UpdateConfirm() && this.KeySelect)
			{
				if (Shop.purchaseMode == PurchaseMode.Buy)
				{
					this.BuyItem(pMan);
				}
				else
				{
					this.SellItem(pMan);
				}
			}
		}
		else if (this.KeySelect)
		{
			if (Shop.canTransact || this.CheckVisible(Shop.shopItem) == 2)
			{
				this.KeySelect = false;
				Sound.PlayCue("menu_confirm");
				Shop.confirming = true;
				this.multiplier = ((Game1.hud.shopType != ShopType.Blacksmith || Shop.listType != 0) ? 1 : 0);
				float size = 0.7f;
				if (Shop.canTransact)
				{
					Game1.inventoryManager.itemControls = Game1.smallText.WordWrap(Strings_Shop.ShopControlsConfirm, size, 1000f, Game1.inventoryManager.itemControlsButtonList, TextAlign.Left);
				}
				else
				{
					string text = Strings_HudInv.Return;
					if (Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[this.multiplier] > -1 && Game1.stats.Material[Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[this.multiplier]] < Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReqAmt[this.multiplier] && Game1.stats.shopMaterial[Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[this.multiplier]] > 0)
					{
						text = Strings_Shop.ShopMaterial + "     " + text;
					}
					Game1.inventoryManager.itemControls = Game1.smallText.WordWrap(text, size, 1000f, Game1.inventoryManager.itemControlsButtonList, TextAlign.Left);
				}
			}
			else
			{
				Sound.PlayCue("shop_fail");
			}
		}
		if (this.KeyX && Game1.hud.shopType == ShopType.Shop && Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
		{
			this.SwitchBuySell();
		}
		if (Shop.switchState > 0 && Shop.switchTime > 0f)
		{
			Shop.switchTime -= Game1.HudTime;
			if (Shop.switchTime < 0f)
			{
				Shop.switchState++;
				if (Shop.switchState == 2)
				{
					if (Shop.switchText == Strings_Shop.ItemStocked)
					{
						Shop.switchTime = 0.8f;
					}
					else
					{
						Shop.switchTime = 0.5f;
					}
				}
				else
				{
					Shop.switchTime = 0.1f;
				}
				if (Shop.switchState > 3)
				{
					Shop.switchState = 0;
				}
			}
		}
		if ((this.KeySelect || this.KeyY || this.KeyX || this.KeyUp || this.KeyDown || this.KeyLeft || this.KeyRight || this.KeyLeftBumper || this.KeyRightBumper) && !Shop.confirming)
		{
			this.PopulateDescription();
		}
		if (this.KeyCancel)
		{
			if (!Shop.confirming)
			{
				this.ExitShop();
			}
			else
			{
				Sound.PlayCue("menu_cancel");
				Shop.confirming = false;
				this.PopulateDescription();
			}
		}
		if (Shop.itemAlpha < 1f)
		{
			Shop.itemAlpha += Game1.HudTime * 4f;
			if (Shop.itemAlpha > 1f)
			{
				Shop.itemAlpha = 1f;
			}
		}
		if (Shop.confirmColor > 0f && !Shop.confirming)
		{
			Shop.confirmColor -= Game1.HudTime * 8f;
			if (Shop.confirmColor < 0f)
			{
				Shop.confirmColor = 0f;
			}
		}
		if (Shop.shopAlpha < 1f)
		{
			Shop.shopAlpha = Math.Min(Shop.shopAlpha + Game1.HudTime * 4f, 1f);
		}
		Shop.itemIconPos += Game1.HudTime * 6f;
		if (Shop.itemIconPos > 6.28f)
		{
			Shop.itemIconPos -= 6.28f;
		}
		Shop.coinAnimFrameTime += Game1.HudTime * 24f;
		if (Shop.coinAnimFrameTime > 1f)
		{
			Shop.coinAnimFrame++;
			if (Shop.coinAnimFrame > 23)
			{
				Shop.coinAnimFrame = 0;
			}
			Shop.coinAnimFrameTime = 0f;
		}
		Shop.animFrameTime += Game1.HudTime * 20f;
		if (Shop.animFrameTime > 1f)
		{
			Shop.animFrameTime -= 1f;
			Shop.animFrame++;
			if (Shop.animFrame > 31)
			{
				Shop.animFrame = 0;
			}
		}
		if (Shop.portraitLoaded == LoadState.Loaded)
		{
			Shop.portraitAlpha += Game1.HudTime * 4f;
		}
		else
		{
			Shop.portraitAlpha = 0f;
		}
		this.DoInput(Game1.currentGamePad);
	}

	private bool UpdateConfirm()
	{
		if (Game1.hud.shopType == ShopType.Shop || Shop.listType == ShopListType.Materials)
		{
			if ((this.KeyLeft || Game1.pcManager.mouseWheelValue > 0f) && this.multiplier > 1)
			{
				Sound.PlayCue("menu_click");
				this.multiplier--;
			}
			if ((this.KeyRight || Game1.pcManager.mouseWheelValue < 0f) && this.CanTransact(this.multiplier))
			{
				Sound.PlayCue("menu_click");
				this.multiplier++;
			}
			if (this.KeySelect)
			{
				Shop.confirming = false;
				return true;
			}
		}
		else
		{
			int num = this.multiplier;
			if (this.KeyLeft && this.multiplier > 0)
			{
				this.multiplier--;
			}
			if (this.KeyRight && this.multiplier < Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq.Length - 1 && Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[this.multiplier + 1] > -1)
			{
				this.multiplier++;
			}
			bool flag = Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[this.multiplier] > -1 && Game1.stats.Material[Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[this.multiplier]] < Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReqAmt[this.multiplier] && Game1.stats.shopMaterial[Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[this.multiplier]] > 0;
			if (num != this.multiplier)
			{
				Sound.PlayCue("menu_click");
				if (!Shop.canTransact)
				{
					string text = Strings_HudInv.Return;
					if (flag)
					{
						text = Strings_Shop.ShopMaterial + "     " + text;
					}
					Game1.inventoryManager.itemControls = Game1.smallText.WordWrap(text, 0.7f, 380f, Game1.inventoryManager.itemControlsButtonList, TextAlign.Left);
				}
			}
			if (this.KeySelect)
			{
				if (Shop.canTransact)
				{
					Shop.confirming = false;
					return true;
				}
				if (flag)
				{
					Shop.confirmColor = 0f;
					Shop.confirming = false;
					Shop.shopCategory = 4;
					Shop.listType = ShopListType.Materials;
					Shop.purchaseMode = PurchaseMode.Buy;
					this.PopulateItemList();
					for (int i = 0; i < Shop.shopItemList.Count; i++)
					{
						if (Shop.shopItemList[i].X == (float)Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[this.multiplier])
						{
							Shop.shopItem = i;
						}
					}
					this.PopulateDescription();
					if (this.CanTransact(0))
					{
						this.multiplier = 1;
						Shop.confirming = true;
						Game1.inventoryManager.itemControls = Game1.smallText.WordWrap(Strings_Shop.ShopControlsConfirm, 0.7f, 1000f, Game1.inventoryManager.itemControlsButtonList, TextAlign.Left);
					}
					return false;
				}
			}
		}
		if (Shop.confirming && Shop.confirmColor < 1f)
		{
			Shop.confirmColor += Game1.HudTime * 4f;
			if (Shop.confirmColor > 1f)
			{
				Shop.confirmColor = 1f;
			}
		}
		return false;
	}

	public void DrawShop()
	{
		bool flag = Game1.hud.shopType == ShopType.Blacksmith && Shop.listType == ShopListType.Equipment;
		float num = (float)Math.Abs(Math.Cos(Shop.itemIconPos * 0.5f) * 20.0);
		Color color = new Color(1f, 1f, 1f, Shop.shopAlpha);
		if (this.shopLoaded && Shop.shopAlpha > 0f)
		{
			Vector2 vector = new Vector2(Game1.screenWidth / 2 - 465, (float)(Game1.screenHeight / 2 - 315) + (Shop.shopAlpha - 1f) * 20f);
			Rectangle value = new Rectangle(576 + Shop.coinAnimFrame * 32, 128, 32, 32);
			if (Shop.shopTex.IsDisposed || Shop.shopTex == null)
			{
				this.SetTextures();
				return;
			}
			Shop.sprite.Draw(Shop.shopTex, vector, new Rectangle(0, 0, 930, 630), color);
			if (Shop.shopAlpha >= 1f)
			{
				this.DrawPortrait(vector, color);
			}
			Vector2 vector2 = vector + new Vector2(460f, 40f);
			this.DrawCategory(vector, num);
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse && !Shop.confirming)
			{
				Vector2 loc = vector + new Vector2(860f, 0f);
				if (Game1.pcManager.DrawMouseButton(loc, 0.8f, new Color(1f, 1f, 1f, Shop.shopAlpha - Shop.confirmColor), 0, draw: true))
				{
					this.KeyCancel = true;
				}
			}
			if (this.ItemList().Count > 0)
			{
				int num2 = 90;
				int num3 = 3;
				int num4 = 2;
				int num5 = (int)vector2.Y + 120;
				int num6 = num5 + num2 * 3 + 55;
				Vector2 vector3 = vector2 + new Vector2(100f, 160f);
				vector3.Y -= Math.Max(Shop.shopItem / num3 - num4, 0) * num2;
				Shop.shopItemListPos.X = vector3.X;
				if (Shop.shopAlpha < 1f)
				{
					Shop.shopItemListPos.Y = vector3.Y;
				}
				else if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse || Shop.confirming)
				{
					Shop.shopItemListPos.Y += (vector3.Y - Shop.shopItemListPos.Y) * Game1.HudTime * 20f;
				}
				int num7 = this.GetCategoryID();
				int num8;
				int num9;
				if (Shop.listType == ShopListType.Equipment)
				{
					num8 = 4;
					num9 = Game1.inventoryManager.invSelMax;
				}
				else
				{
					num8 = 5;
					num9 = 12;
				}
				if (!Shop.confirming && this.ItemList().Count > num3 * 3)
				{
					if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
					{
						int num10 = this.ItemList().Count / num3 * num2;
						int num11 = (int)(vector2.Y + 160f);
						float persistFloat = 0f - Shop.shopItemListPos.Y + (float)num11;
						Game1.hud.DrawScrollBarMouse(vector2 + new Vector2(400f, 140f), ref persistFloat, 300, num10, num2, Shop.shopAlpha);
						Game1.pcManager.MouseWheelAdjust(ref persistFloat, num2, 0f, num10);
						Shop.shopItemListPos.Y = 0f - persistFloat + (float)num11;
					}
					else
					{
						Game1.hud.DrawScrollBar(vector2 + new Vector2(400f, 140f), (float)(Shop.shopItem / num3) / (float)Math.Max(this.ItemList().Count / num3, 4), 300f, Shop.shopAlpha);
					}
				}
				for (int i = 0; i < this.ItemList().Count; i++)
				{
					int num12 = this.CheckVisible(i);
					if (num12 <= 0)
					{
						continue;
					}
					float num13 = 0f;
					float num14 = 1f;
					float num15 = 0f;
					float num16 = 1f;
					Vector3 vector4 = this.ItemList()[i];
					float num17 = Shop.shopItemListPos.Y + (float)(i / num3 * num2);
					if (num17 < (float)num5 || num17 > (float)num6)
					{
						num16 = 0f;
					}
					float num18 = -(i / num3 * num3 * num2) + i / num3 * 10;
					num18 += (Shop.shopItemListPos.Y - 260f) * 0.11f;
					float num19 = 0.5f;
					if (i == Shop.shopItem)
					{
						num14 = MathHelper.Clamp(0.5f + Shop.itemAlpha * 2f, 1f, 1.5f);
						num13 = num;
						num15 = (float)Math.Sin(Shop.itemIconPos) / 10f;
						num19 = MathHelper.Clamp(6.28f / num, 0f, 0.5f) * Shop.shopAlpha;
						if (num12 == 1 && flag)
						{
							Game1.inventoryManager.itemCost = 0;
							Game1.bigText.Color = new Color(0f, 1f, 1f, Shop.itemAlpha);
							Game1.bigText.DrawOutlineText(vector + new Vector2(430f, 440f), Strings_Shop.ItemPurchased, 0.9f, 1000, TextAlign.Right, fullOutline: true);
						}
					}
					if (num12 == 2 && i != Shop.shopItem)
					{
						num14 *= 0.75f;
						num15 *= 0.5f;
					}
					if (Shop.listType == ShopListType.Equipment && this.CheckEquipped((int)vector4.X))
					{
						Color color2 = new Color(1f, 1f - num / 40f, num / 40f, num16);
						Shop.sprite.Draw(Shop.shopTex, Shop.shopItemListPos + new Vector2((float)(-1 + i * num2) + num18, 1 + Math.Abs(i / num3) * num2), new Rectangle(468, 630, 78, 78), color2, 0f, new Vector2(38f, 38f), 1f, SpriteEffects.None, 0f);
					}
					if (Shop.listType == ShopListType.Materials)
					{
						num7 = (int)vector4.X / num9;
					}
					Rectangle value2 = new Rectangle(num7 * 60, ((int)vector4.X - num7 * num9) * 60, 60, 60);
					if (num12 == 2)
					{
						Shop.sprite.Draw(Shop.particlesTex[1], Shop.shopItemListPos + new Vector2((float)(-10 + i * 90) + num18 + num13 / 2f, 10 + Math.Abs(i / 3) * 90), new Rectangle(0, 0, 102, 128), Color.Black * MathHelper.Clamp(6.28f / num13 / 2f, 0f, 0.2f) * Shop.shopAlpha * num16, 0f, new Vector2(30f, 30f), num14 * new Vector2(1f, 0.5f), SpriteEffects.None, 0f);
						Shop.sprite.Draw(Shop.particlesTex[1], Shop.shopItemListPos + new Vector2((float)(i * 90) + num18, (float)(Math.Abs(i / 3) * 90) - num13), new Rectangle(0, 0, 102, 128), new Color(1f, 1f, 1f, (num14 - 0.1f) * Shop.shopAlpha * num16), num15, new Vector2(52f, 60f), num14, SpriteEffects.None, 0f);
					}
					else
					{
						Shop.sprite.Draw(Shop.particlesTex[num8], Shop.shopItemListPos + new Vector2((float)(10 + i * num2) + num13 / 2f + num18, 17 + i / num3 * num2), value2, new Color(0f, 0f, 0f, num19 * num16), num15, new Vector2(30f, 30f), num14 * new Vector2(1f, 0.5f), SpriteEffects.None, 0f);
					}
					Shop.sprite.Draw(Shop.particlesTex[num8], Shop.shopItemListPos + new Vector2((float)(i * num2) + num18, (float)(i / num3 * num2) - num13), value2, color * num16, num15, new Vector2(30f, 30f), num14, SpriteEffects.None, 0f);
					if (vector4.Y == 255f && Shop.listType == ShopListType.Equipment)
					{
						Shop.sprite.Draw(Shop.shopTex, Shop.shopItemListPos + new Vector2((float)(20 + i * num2) + num18, 25 + Math.Abs(i / num3) * num2), new Rectangle(392, 630, 40, 20), color * num16, 0f, new Vector2(20f, 10f), 0.8f, SpriteEffects.None, 0f);
					}
					else
					{
						Color color3 = color;
						if ((int)vector4.Y < 1)
						{
							color3 = Color.Red;
						}
						this.scoreDraw.Draw((int)vector4.Y, Shop.shopItemListPos + new Vector2((float)(19 + i * num2) + num18, 15 + Math.Abs(i / num3) * num2), 0.8f, color3 * num16, ScoreDraw.Justify.Center, 0);
					}
					if (Shop.purchaseMode == PurchaseMode.Sell && Shop.listType == ShopListType.Materials && Game1.stats.shopMaterial[(int)vector4.X] > -1)
					{
						Shop.sprite.Draw(Shop.shopTex, Shop.shopItemListPos + new Vector2((float)(30 + i * num2) + num18, -30 + Math.Abs(i / num3) * num2), new Rectangle(392, 650, 40, 30), color * num16, 0f, new Vector2(20f, 15f), 1f, SpriteEffects.None, 0f);
					}
					if (i == Shop.shopItem && (int)vector4.Y < 1)
					{
						Game1.inventoryManager.itemCost = 0;
						Game1.bigText.Color = Color.Red * Shop.itemAlpha;
						if (Shop.purchaseMode == PurchaseMode.Buy)
						{
							Game1.bigText.DrawOutlineText(vector + new Vector2(430f, 440f), Strings_Shop.ItemUnavailableShop, 0.9f, 1000, TextAlign.Right, fullOutline: true);
						}
						else
						{
							Game1.bigText.DrawOutlineText(vector + new Vector2(430f, 440f), Strings_Shop.ItemUnavailableMe, 0.9f, 1000, TextAlign.Right, fullOutline: true);
						}
					}
					if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
					{
						Vector2 vector5 = Shop.shopItemListPos + new Vector2((float)(i * num2) + num18 - 40f, i / num3 * num2 - 40);
						if (!Shop.confirming && num16 > 0f && new Rectangle((int)vector5.X, (int)vector5.Y, 80, 80).Contains((int)this.mousePos.X, (int)this.mousePos.Y))
						{
							if (Shop.shopItem != i)
							{
								Shop.shopItem = i;
								Sound.PlayCue("menu_click");
								Shop.itemAlpha = 0f;
								this.PopulateDescription();
							}
							if (Game1.pcManager.leftMouseClicked)
							{
								Game1.pcManager.leftMouseClicked = false;
								this.KeySelect = true;
							}
						}
					}
					else if (i == Shop.shopItem)
					{
						Vector2 vector6 = vector3 + new Vector2(-40 + i * num2 - i / num3 * num3 * num2 + i / num3 * 10, -40 + i / num3 * num2);
						vector6.X -= Math.Max(Shop.shopItem / num3 - num4, 0) * 10;
						if (Shop.shopAlpha < 1f)
						{
							Game1.hud.cursorPos = vector6;
						}
						else
						{
							Game1.hud.cursorPos += (vector6 - Game1.hud.cursorPos) * Game1.HudTime * 20f * (1f - Shop.confirmColor);
						}
						Game1.hud.DrawCursor(Game1.hud.cursorPos, 0.75f, color, flip: false);
					}
				}
			}
			Game1.smallText.Color = color;
			Game1.smallText.DrawText(vector + new Vector2(90f, 515f), Strings_Shop.MyStock, 0.7f);
			Game1.smallText.DrawText(vector + new Vector2(300f, 515f), Strings_Shop.MyGold, 0.7f);
			this.scoreDraw.Draw(Shop.playerStock, new Vector2(vector.X + 90f, vector.Y + 545f), 1f, new Color(1f, 1f, 1f, Shop.itemAlpha * Shop.shopAlpha), ScoreDraw.Justify.Left, 1);
			this.scoreDraw.Draw(Game1.hud.tempGold, new Vector2(vector.X + 370f, vector.Y + 545f), new Vector2(0.8f, 1f), color, ScoreDraw.Justify.Right, 1);
			Shop.sprite.Draw(Shop.particlesTex[1], new Vector2(vector.X + 400f, vector.Y + 545f), value, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			if (!Shop.confirming)
			{
				if (Game1.isPCBuild)
				{
					Game1.inventoryManager.itemControls = Game1.smallText.WordWrap(this.PopulateControls(), 0.7f, 410f, Game1.inventoryManager.itemControlsButtonList, TextAlign.LeftAndCenter);
				}
				if (Game1.pcManager.inputDevice != InputDevice.KeyboardAndMouse)
				{
					int num20 = (int)(Game1.smallFont.MeasureString(Game1.inventoryManager.itemControls).X * 0.7f) + 80;
					int num21 = (int)(Game1.smallFont.MeasureString(Game1.inventoryManager.itemControls).Y * 0.7f) + 10;
					Vector2 vector7 = vector2 + new Vector2(15f, 530 - num21 / 2);
					Game1.hud.DrawMiniBorder(vector7 + new Vector2(205 - num20 / 2, -5f), num20, num21, Color.White, 1f);
					Game1.smallText.DrawButtonText(vector7, Game1.inventoryManager.itemControls, 0.7f, Game1.inventoryManager.itemControlsButtonList, bounce: false, 410f, TextAlign.Center);
				}
				else if (Game1.hud.shopType == ShopType.Shop && Game1.pcManager.DrawMouseButton(vector2 + new Vector2(220f, 520f), 0.8f, new Color(1f, 1f, 1f, Shop.shopAlpha - Shop.confirmColor), 1, draw: true, Game1.inventoryManager.itemControls, TextAlign.Center))
				{
					this.SwitchBuySell();
					this.PopulateDescription();
				}
			}
			float num22 = 0.8f;
			if (this.ItemList().Count > 0)
			{
				bool flag2 = false;
				if (Shop.purchaseMode == PurchaseMode.Buy && (Shop.listType == ShopListType.Equipment || (Shop.listType == ShopListType.Materials && Shop.shopItem < this.ItemList().Count && this.ItemList()[Shop.shopItem].Y > -1f)))
				{
					flag2 = true;
				}
				if (Shop.purchaseMode == PurchaseMode.Sell && Shop.shopItem < this.ItemList().Count && ((Shop.listType == ShopListType.Equipment && Game1.stats.Equipment[(int)this.ItemList()[Shop.shopItem].X] > 0) || (Shop.listType == ShopListType.Materials && Shop.shopItem < this.ItemList().Count && this.ItemList()[Shop.shopItem].Y > -1f)))
				{
					flag2 = true;
				}
				if (flag2)
				{
					Game1.smallText.Color = new Color(1f, 1f, 1f, Shop.itemAlpha * Shop.shopAlpha);
					Vector2 vector8 = vector + new Vector2(60f, 210f);
					string trueItemName = "";
					if (Shop.purchaseMode == PurchaseMode.Buy && Shop.listType == ShopListType.Equipment && Game1.hud.shopType == ShopType.Shop)
					{
						if (Game1.stats.shopEquipment[this.GetItemID(Shop.shopItem)] < byte.MaxValue || Game1.stats.ReturnChestItems("Shop " + Game1.inventoryManager.itemName + " " + ((int)Game1.stats.shopEquipGotten[this.GetItemID(Shop.shopItem)]).ToString()).Count > 0)
						{
							Console.WriteLine("Shop " + Game1.inventoryManager.itemName + " " + ((int)Game1.stats.shopEquipGotten[this.GetItemID(Shop.shopItem)]).ToString() + ":" + this.GetItemID(Shop.shopItem).ToString() + ",");
						}
						if (Game1.stats.ReturnChestItems("Shop " + Game1.inventoryManager.itemName + " " + ((int)Game1.stats.shopEquipGotten[this.GetItemID(Shop.shopItem)]).ToString()).Count == 0)
						{
							trueItemName = Game1.inventoryManager.itemName;
						}
						else 
						{
							var trueId = Game1.stats.ReturnChestItems("Shop " + Game1.inventoryManager.itemName + " " + ((int)Game1.stats.shopEquipGotten[this.GetItemID(Shop.shopItem)]).ToString())[0];
							if (trueId.Contains("~"))
							{
								switch (int.Parse(trueId.Replace("~", "")))
								{
									case 0:
										trueItemName = "DUST STORM";
										break;
									case 1:
										trueItemName = "FIDGET PROJECTILE";
										break;
									case 2:
										trueItemName = "FIDGET PROJECTILE";
										break;
									case 3:
										trueItemName = "FIDGET PROJECTILE";
										break;
									case 4:
										trueItemName = "SLASH";
										break;
									case 5:
										trueItemName = "JUMP";
										break;
									case 10:
										trueItemName = "DASH";
										break;
									case 12:
										trueItemName = "AERIAL DUST STORM";
										break;
									case 14:
										trueItemName = "CROUCH SLIDE";
										break;
									case 15:
										trueItemName = "DOUBLE JUMP";
										break;
									case 16:
										trueItemName = "IRON GRIP";
										break;
									case 17:
										trueItemName = "BOOST JUMP";
										break;
									default:
										trueItemName = "UNKNOWN ITEM";
										break;
								}
								Shop.itemInfoDisplay = "Ability.";
								Shop.itemStats = "";
							}
							else if (trueId == "!")
							{
								trueItemName = "Skill Gem";
								Shop.itemInfoDisplay = "";
								Shop.itemStats = "";
							}
							else if (trueId.Contains("-")
							{
								Shop.itemInfoDisplay = Game1.smallText.WordWrap(Game1.inventoryManager.equipItem[int.Parse(trueId.Replace("-", ""))].Description, 0.8f, 380f, TextAlign.Left);
								Shop.itemStats = Game1.smallText.WordWrap(Game1.inventoryManager.equipItem[int.Parse(trueId.Replace("-", ""))].StatInfo, 0.8f, 380f, TextAlign.Left);
								if (this.CheckEquipped(Shop.finalItem))
								{
									Shop.itemStats = Game1.smallText.WordWrap(Game1.inventoryManager.equipItem[int.Parse(trueId.Replace("-", ""))].StatInfo + "[N]" + Strings_Shop.ItemEquipped, 0.8f, 380f, TextAlign.Left);
								}
								trueItemName = Game1.inventoryManager.equipItem[int.Parse(trueId.Replace("-", ""))].Name+ " Blueprint";
							}
							else
							{
								Shop.itemInfoDisplay = Game1.smallText.WordWrap(Game1.inventoryManager.equipItem[int.Parse(trueId)].Description, 0.8f, 380f, TextAlign.Left);
								Shop.itemStats = Game1.smallText.WordWrap(Game1.inventoryManager.equipItem[int.Parse(trueId)].StatInfo, 0.8f, 380f, TextAlign.Left);
								if (this.CheckEquipped(Shop.finalItem))
								{
									Shop.itemStats = Game1.smallText.WordWrap(Game1.inventoryManager.equipItem[int.Parse(trueId)].StatInfo + "[N]" + Strings_Shop.ItemEquipped, 0.8f, 380f, TextAlign.Left);
								}
								trueItemName = Game1.inventoryManager.equipItem[int.Parse(trueId)].Name;
							} 
						}
					}
                    else
					{
						trueItemName = Game1.inventoryManager.itemName;
					}
					Game1.smallText.DrawText(vector8, trueItemName, num22, 0f, TextAlign.Left);
					vector8.Y += Game1.smallFont.MeasureString(trueItemName).Y * num22;
					Shop.sprite.Draw(Shop.hudTex[2], vector8 + new Vector2(140f, -3f), new Rectangle(0, 502, 326, 18), Game1.smallText.Color, 0f, new Vector2(163f, 0f), new Vector2(0.8f, 0.5f), SpriteEffects.None, 0f);
					vector8.Y += 5f;
					num22 = 0.7f;
					Game1.smallText.DrawText(vector8, Shop.itemInfoDisplay, num22, 290f, TextAlign.Left);
					vector8.Y += Game1.smallFont.MeasureString(Shop.itemInfoDisplay).Y * num22 + 20f;
					Game1.smallText.Color = new Color(0f, 1f, 0f, Shop.itemAlpha * Shop.shopAlpha);
					Game1.smallText.DrawText(vector8, Shop.itemStats, num22, 0f, TextAlign.Left);
					int num23 = Game1.inventoryManager.itemCost;
					if (num23 > 0)
					{
						if (Shop.purchaseMode == PurchaseMode.Sell)
						{
							num23 /= 4;
						}
						else if (flag)
						{
							num23 /= 4;
						}
						Color color4 = ((Game1.stats.Gold >= num23 || Shop.purchaseMode != 0) ? new Color(1f, 1f, 1f, Shop.itemAlpha * Shop.shopAlpha) : new Color(1f, 0f, 0f, Shop.itemAlpha * Shop.shopAlpha));
						Game1.smallText.Color = color;
						if (Shop.purchaseMode == PurchaseMode.Buy)
						{
							Game1.smallText.DrawText(vector + new Vector2(300f, 420f), Strings_Shop.Cost, num22);
						}
						else
						{
							Game1.smallText.DrawText(vector + new Vector2(300f, 420f), Strings_Shop.Profit, num22);
						}
						this.scoreDraw.Draw(num23, new Vector2(vector.X + 370f, vector.Y + 450f), 1f, color4, ScoreDraw.Justify.Right, 1);
						Shop.sprite.Draw(Shop.particlesTex[1], new Vector2(vector.X + 400f, vector.Y + 450f), value, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
					}
				}
			}
			if (Shop.listType == ShopListType.Materials)
			{
				Vector2 vector9 = vector + new Vector2(60f, 360f);
				Shop.sprite.Draw(Shop.hudTex[2], vector9 + new Vector2(180f, -10f), new Rectangle(0, 502, 326, 18), color, 0f, new Vector2(163f, 0f), new Vector2(1.2f, 0.5f), SpriteEffects.None, 0f);
				num22 = 0.7f;
				Game1.smallText.Color = new Color(1f, 1f, 1f, Shop.shopAlpha);
				string s = Game1.smallText.WordWrap(Strings_HudInv.MaterialShopCatalogue, num22, 300f, TextAlign.Left);
				Game1.smallText.DrawText(vector9, s, num22);
				vector9.Y += Game1.smallFont.MeasureString(Strings_HudInv.MaterialShopCatalogue).Y * num22;
				Game1.smallText.Color = new Color(0f, 1f, 0f, Shop.shopAlpha);
				int num24 = 0;
				for (int j = 0; j < Game1.stats.shopMaterial.Length; j++)
				{
					if (Game1.stats.shopMaterial[j] > -1)
					{
						num24++;
					}
				}
				Game1.smallText.DrawText(vector9, Strings_HudInv.InvCat4 + " (" + num24 + "/" + Game1.stats.maxMaterials + ")", num22);
			}
		}
		if (Shop.confirmColor > 0f)
		{
			this.DrawConfirm(flag, num, 1f);
		}
		if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
		{
			Game1.pcManager.DrawCursor(Shop.sprite, 0.8f, color);
		}
		if (Shop.switchState > 0)
		{
			this.DrawSwitch();
		}
	}

	private void DrawCategory(Vector2 shopLoc, float posInvY)
	{
		Vector2 vector = shopLoc + new Vector2(555f, 79f);
		Game1.bigText.Color = Color.White;
		float num;
		if (Game1.hud.shopType == ShopType.Shop)
		{
			Shop.sprite.Draw(Shop.shopTex, vector - new Vector2(75f, 7f), new Rectangle(0, 630, 392, 60), Color.White * Shop.shopAlpha);
			num = 54.6f;
		}
		else
		{
			num = 76f;
		}
		if (Shop.listType == ShopListType.Equipment)
		{
			Game1.bigText.DrawOutlineText(shopLoc + new Vector2(478f, 38f), Strings_HudInv.ResourceManager.GetString("EquipCategory" + this.GetCategoryID()), 0.8f, 396, TextAlign.Center, fullOutline: true);
		}
		else
		{
			Game1.bigText.DrawOutlineText(shopLoc + new Vector2(478f, 38f), Strings_HudInv.InvCat4, 0.8f, 396, TextAlign.Center, fullOutline: true);
		}
		this.categoryHighlight += ((float)Shop.shopCategory - this.categoryHighlight) * Game1.HudTime * 20f;
		Shop.sprite.Draw(Shop.nullTex, new Rectangle((int)vector.X - 70, (int)vector.Y, (int)(this.categoryHighlight * num), 46), new Color(0f, 0f, 0f, 0.75f));
		Shop.sprite.Draw(Shop.nullTex, new Rectangle((int)(vector.X - 70f + (this.categoryHighlight + 1f) * num), (int)vector.Y, (int)(((float)(Shop.maxCategory - 1) - this.categoryHighlight) * num) + 1, 46), new Color(0f, 0f, 0f, 0.75f));
		Shop.sprite.Draw(Shop.shopTex, vector + new Vector2(-76f + this.categoryHighlight * num, -5f), new Rectangle(432, 630, 18, 54), Color.White * Shop.shopAlpha);
		Shop.sprite.Draw(Shop.shopTex, vector + new Vector2(-86f + this.categoryHighlight * num + num, -5f), new Rectangle(450, 630, 18, 54), Color.White * Shop.shopAlpha);
		if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
		{
			if (!(Shop.confirmColor <= 0f) || Shop.switchState != 0)
			{
				return;
			}
			for (int i = 0; i < Shop.maxCategory; i++)
			{
				if (!new Rectangle((int)(vector.X - 70f + (float)i * num), (int)vector.Y, (int)num, 46).Contains((int)this.mousePos.X, (int)this.mousePos.Y) || !Game1.pcManager.leftMouseClicked)
				{
					continue;
				}
				Game1.pcManager.leftMouseClicked = false;
				if (Shop.shopCategory != i)
				{
					Shop.shopCategory = i;
					Sound.PlayCue("menu_page_turn");
					Shop.shopItem = 0;
					Shop.shopItemListPos.Y = Game1.screenHeight / 2 - 115;
					Shop.itemAlpha = 0f;
					if (Game1.hud.shopType == ShopType.Blacksmith && Shop.purchaseMode == PurchaseMode.Sell)
					{
						Shop.switchState = 1;
						Shop.switchTime = 0.1f;
						Shop.switchText = Strings_Shop.PurchaseModeBuy;
						Shop.purchaseMode = PurchaseMode.Buy;
					}
					if ((Game1.hud.shopType == ShopType.Shop && Shop.shopCategory == 6) || (Game1.hud.shopType == ShopType.Blacksmith && Shop.shopCategory == 4))
					{
						Shop.listType = ShopListType.Materials;
					}
					else
					{
						Shop.listType = ShopListType.Equipment;
					}
					this.PopulateItemList();
					this.PopulateDescription();
				}
			}
		}
		else if (Game1.pcManager.inputDevice == InputDevice.GamePad)
		{
			Shop.sprite.Draw(Shop.hudTex[0], vector + new Vector2(-54f, -12f - posInvY / 8f), new Rectangle(150, 140, 50, 45), Color.White * Shop.shopAlpha, 0f, new Vector2(25f, 20f), 0.8f, SpriteEffects.None, 0f);
			Shop.sprite.Draw(Shop.hudTex[0], vector + new Vector2(293f, -12f - posInvY / 8f), new Rectangle(0, 140, 50, 45), Color.White * Shop.shopAlpha, 0f, new Vector2(25f, 20f), 0.8f, SpriteEffects.None, 0f);
		}
	}

	private void DrawConfirm(bool blueprint, float posInvY, float textSize)
	{
		Shop.sprite.Draw(Shop.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, Shop.confirmColor / 1.5f));
		Color color = new Color(1f, 1f, 1f, Shop.confirmColor);
		Game1.smallText.Color = color;
		int num = 360;
		int dividerLoc = 0;
		int num2 = Game1.screenWidth / 2 - 380;
		int num3 = (int)((float)((Game1.screenHeight - num) / 2) - Shop.confirmColor * 20f);
		int num4 = num2 + 760;
		Game1.hud.DrawBorder(new Vector2(num2, num3), 760, num, color, 0.9f, dividerLoc);
		Color color2 = new Color(0f, 0f, 0f, 0.5f * Shop.confirmColor);
		Shop.sprite.Draw(Shop.hudTex[1], new Vector2(Game1.screenWidth / 2, num3 + 68), new Rectangle(887, 20, 234, 180), color2, 0f, Vector2.Zero, new Vector2(1.2f, 1f), SpriteEffects.None, 0f);
		Shop.sprite.Draw(Shop.hudTex[1], new Vector2(Game1.screenWidth / 2, num3 + 68), new Rectangle(887, 20, 234, 180), color2, 0f, new Vector2(234f, 0f), new Vector2(1.2f, 1f), SpriteEffects.FlipHorizontally, 0f);
		Shop.sprite.Draw(Shop.hudTex[2], new Vector2(Game1.screenWidth / 2, num3 + 60), new Rectangle(0, 502, 326, 18), color, 0f, new Vector2(163f, 0f), new Vector2(1.5f, 1f), SpriteEffects.None, 0f);
		Game1.bigText.Color = color;
		if (Shop.finalItem > -1)
		{
			Game1.bigText.DrawOutlineText(new Vector2(num2, num3 + 15), (Shop.purchaseMode != 0) ? Strings_Shop.ConfirmSell : ((!blueprint) ? Strings_Shop.ConfirmPurchase : (Shop.canTransact ? Strings_Shop.ConfirmCraft : ((Game1.hud.shopType == ShopType.Blacksmith && Game1.stats.Equipment[Shop.finalItem] == 250) ? Strings_Hud.Mini_InvFull : Strings_Shop.ResourcesMissing))), 1f, num4 - num2, TextAlign.Center, fullOutline: true);
		}
		float rotation = (float)Math.Sin(Shop.itemIconPos) / 10f;
		int num5 = ((Shop.listType == ShopListType.Equipment) ? 4 : 5);
		Rectangle value = new Rectangle(-1, -1, 1, 1);
		if (Shop.listType == ShopListType.Equipment)
		{
			int categoryID = this.GetCategoryID();
			value = new Rectangle(categoryID * 60, (Shop.finalItem - categoryID * Game1.inventoryManager.invSelMax) * 60, 60, 60);
		}
		else if (this.ItemList().Count > 0)
		{
			int num6 = Shop.finalItem / 12;
			value = new Rectangle(num6 * 60, (Shop.finalItem - num6 * 12) * 60, 60, 60);
		}
		Vector2 vector = new Vector2(num2 + 140, num3 + 180);
		Game1.hud.DrawMiniBorder(vector - new Vector2(75f, 80f), 150, 140, Color.White * Shop.confirmColor * 0.6f, 1f);
		Shop.sprite.Draw(Shop.particlesTex[num5], vector + new Vector2(-20f + posInvY / 2f, 20f), value, new Color(0f, 0f, 0f, MathHelper.Clamp(Shop.itemAlpha * (6.28f / posInvY) / 2f, 0f, 0.5f) * Shop.confirmColor), rotation, new Vector2(30f, 30f), 2f * new Vector2(1f, 0.5f), SpriteEffects.None, 0f);
		Shop.sprite.Draw(Shop.particlesTex[num5], vector - new Vector2(0f, posInvY), value, color, rotation, new Vector2(30f, 30f), 2f, SpriteEffects.None, 0f);
		int num7 = Math.Max((int)(Game1.smallFont.MeasureString(Game1.inventoryManager.itemName).X * 0.8f) + 20, 120);
		Game1.hud.DrawMiniBorder(new Vector2(num2 + 140 - num7 / 2, vector.Y + 80f), num7, 30, Color.White * Shop.confirmColor * 0.6f, 1f);
		Game1.smallText.DrawText(new Vector2(num2, vector.Y + 84f), Game1.inventoryManager.itemName, 0.8f, 280f, TextAlign.Center);
		int num8 = Game1.inventoryManager.itemCost;
		if (Shop.purchaseMode == PurchaseMode.Sell)
		{
			num8 /= 4;
		}
		else if (blueprint)
		{
			num8 /= 4;
		}
		Game1.bigText.DrawOutlineText(new Vector2(num2 + 310, num3 + 95), (Shop.purchaseMode == PurchaseMode.Buy) ? Strings_Shop.Cost : Strings_Shop.Profit, textSize, num4 - num2, TextAlign.Left, fullOutline: true);
		Color color3 = color;
		if (blueprint && num8 > Game1.stats.Gold)
		{
			color3 = Color.Red * Shop.confirmColor;
		}
		this.scoreDraw.Draw(num8, new Vector2(num2 + 610, num3 + 100), 1.2f, color3, ScoreDraw.Justify.Right, 1);
		Rectangle value2 = new Rectangle(576 + Shop.coinAnimFrame * 32, 128, 32, 32);
		Shop.sprite.Draw(Shop.particlesTex[1], new Vector2(num2 + 650, num3 + 102), value2, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
		if (!blueprint)
		{
			Game1.bigText.DrawOutlineText(new Vector2(num2 + 310, num3 + 155), Strings_Shop.Amount, textSize, num4 - num2, TextAlign.Left, fullOutline: true);
			float num9 = Game1.bigFont.MeasureString(this.multiplier.ToString()).X * textSize;
			Vector2 position = new Vector2((float)(num2 + 600) - num9 - posInvY / 4f, num3 + 175);
			Vector2 position2 = new Vector2((float)(num2 + 660) + posInvY / 4f, num3 + 175);
			float scale = 0.6f;
			float scale2 = 0.6f;
			if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
			{
				if (new Rectangle((int)position.X - 30, (int)position.Y - 20, 60, 40).Contains((int)this.mousePos.X, (int)this.mousePos.Y))
				{
					scale = 1f;
					if (Game1.pcManager.leftMouseClicked)
					{
						Game1.pcManager.leftMouseClicked = false;
						this.KeyLeft = true;
					}
				}
				if (new Rectangle((int)position2.X - 30, (int)position2.Y - 20, 70, 40).Contains((int)this.mousePos.X, (int)this.mousePos.Y))
				{
					scale2 = 1f;
					if (Game1.pcManager.leftMouseClicked)
					{
						Game1.pcManager.leftMouseClicked = false;
						this.KeyRight = true;
					}
				}
			}
			Shop.sprite.Draw(Game1.navManager.NavTex, position, new Rectangle(752, 0, 60, 48), color, 4.71f, new Vector2(30f, 24f), scale, SpriteEffects.None, 0f);
			Shop.sprite.Draw(Game1.navManager.NavTex, position2, new Rectangle(752, 0, 60, 48), color, 1.57f, new Vector2(30f, 24f), scale2, SpriteEffects.None, 0f);
			this.scoreDraw.Draw(this.multiplier, new Vector2(num2 + 610, num3 + 160), 1.2f, color, ScoreDraw.Justify.Right, 1);
			Shop.sprite.Draw(Shop.hudTex[2], new Vector2(num2 + 500, num3 + 210), new Rectangle(0, 502, 326, 18), color, 0f, new Vector2(163f, 0f), new Vector2(1f, 0.5f), SpriteEffects.None, 0f);
			Game1.bigText.DrawOutlineText(new Vector2(num2 + 310, num3 + 235), Strings_Shop.Total, textSize, num4 - num2, TextAlign.Left, fullOutline: true);
			this.scoreDraw.Draw(num8 * this.multiplier, new Vector2(num2 + 610, num3 + 240), 1.2f, color, ScoreDraw.Justify.Right, 1);
			Shop.sprite.Draw(Shop.particlesTex[1], new Vector2(num2 + 650, num3 + 242), value2, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
		}
		else if (Shop.finalItem > -1)
		{
			int num10 = 0;
			for (int i = 0; i < Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq.Length; i++)
			{
				if (Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[i] > -1)
				{
					num10++;
				}
			}
			if (num10 > 0)
			{
				Game1.bigText.DrawOutlineText(new Vector2(num2 + 310, num3 + 155), Strings_HudInv.InvCat4, textSize, num4 - num2, TextAlign.Left, fullOutline: true);
				Shop.sprite.Draw(Shop.hudTex[2], new Vector2(num2 + 500, num3 + 210), new Rectangle(0, 502, 326, 18), color, 0f, new Vector2(163f, 0f), new Vector2(1f, 0.5f), SpriteEffects.None, 0f);
				Shop.sprite.Draw(Shop.hudTex[2], new Vector2(num2 + 500, num3 + 310), new Rectangle(0, 502, 326, 18), color, 0f, new Vector2(163f, 0f), new Vector2(1f, 0.5f), SpriteEffects.None, 0f);
				int num11 = Game1.hud.DrawMaterialsRequired(Shop.confirmColor, new Vector2(num2 + 220, num3 + 135), Shop.finalItem, this.multiplier, alignName: true);
				if (num11 > -1)
				{
					if (this.multiplier != num11)
					{
						this.multiplier = num11;
						Sound.PlayCue("menu_click");
					}
					if (Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[this.multiplier] > -1 && Game1.stats.Material[Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[this.multiplier]] < Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReqAmt[this.multiplier] && Game1.stats.shopMaterial[Game1.inventoryManager.equipItem[Shop.finalItem].MaterialReq[this.multiplier]] > 0)
					{
						int num12 = (int)(Game1.smallFont.MeasureString(Strings_PC.ShopPurchaseMaterial).X * 0.7f) + 30;
						Vector2 vector2 = ((Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse) ? new Vector2(Math.Min(this.mousePos.X + 50f, Game1.screenWidth - num12), this.mousePos.Y + 50f) : (this.mousePos + new Vector2(-num12 / 2 - 20, -60f)));
						Game1.hud.DrawMiniBorder(vector2, num12, 10, color, 0.85f);
						Game1.smallText.DrawText(vector2 + new Vector2(0f, 5f), Strings_PC.ShopPurchaseMaterial, 0.7f, num12, TextAlign.Center);
						if (Game1.pcManager.leftMouseClicked)
						{
							Game1.pcManager.leftMouseClicked = false;
							this.KeySelect = true;
						}
					}
				}
			}
		}
		if (Game1.pcManager.inputDevice == InputDevice.KeyboardAndMouse)
		{
			Vector2 loc = new Vector2(num4 - 70, num3 + num - 70);
			Vector2 loc2 = new Vector2(num4 - 20, num3 - 30);
			if (Shop.canTransact && Game1.pcManager.DrawMouseButton(loc, 0.8f, color, 2, draw: true))
			{
				this.KeySelect = true;
			}
			if (Game1.pcManager.DrawMouseButton(loc2, 0.8f, color, 0, draw: true))
			{
				this.KeyCancel = true;
			}
		}
		else if (Shop.confirming)
		{
			int num13 = (int)(Game1.smallFont.MeasureString(Game1.inventoryManager.itemControls).X * 0.7f) + 20;
			Game1.hud.DrawMiniBorder(new Vector2(num2 + 380 - num13 / 2, num3 + num - 15), num13, 30, color, 1f);
			Game1.smallText.DrawButtonText(new Vector2(num2, num3 + num - 10), Game1.inventoryManager.itemControls, 0.7f, Game1.inventoryManager.itemControlsButtonList, bounce: false, num4 - num2, TextAlign.Center);
		}
	}

	private void DrawSwitch()
	{
		Color color = new Color(1f, 1f, 1f, 1f);
		float num = 0f;
		if (Shop.switchState == 1)
		{
			color = new Color(1f, 1f, 1f, 1f - Shop.switchTime * 5f);
			num = Shop.switchTime * 5f * 20f;
		}
		else if (Shop.switchState == 3)
		{
			color = new Color(1f, 1f, 1f, Shop.switchTime * 5f);
			num = (1f - Shop.switchTime * 5f) * -20f;
		}
		Shop.sprite.Draw(Shop.nullTex, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), new Color(0f, 0f, 0f, (float)(int)color.A / 255f / 2f));
		int num2 = (int)Game1.bigFont.MeasureString(Shop.switchText).X + 100;
		int num3 = Game1.screenWidth / 2 - num2 / 2;
		int num4 = Game1.screenHeight / 2 - 80 - (int)num;
		Game1.hud.DrawMiniBorder(new Vector2(num3, num4), num2, 80, color, 0.85f);
		Game1.bigText.Color = color;
		Game1.bigText.DrawOutlineText(new Vector2(num3, num4 + 20), Shop.switchText, 1f, num2, TextAlign.Center, fullOutline: true);
	}

	private void DrawPortrait(Vector2 shopScreenOffset, Color color)
	{
		if (Shop.portraitLoaded == LoadState.Loaded && Shop.portraitTex != null && !Shop.portraitTex.IsDisposed)
		{
			Shop.sprite.Draw(Shop.portraitTex, shopScreenOffset + new Vector2(45f, 183f), new Rectangle(40 + (500 * Shop.animFrame - 4000 * (Shop.animFrame / 8)), 480 * (Shop.animFrame / 8), 419, 250), color * Shop.portraitAlpha, 0f, new Vector2(0f, 250f), new Vector2(1.125f, 1.5f) * 0.75f, SpriteEffects.None, 0f);
		}
		else if (Shop.portraitLoaded == LoadState.NotLoaded)
		{
			this.SetPortraitTexture();
		}
	}
}
