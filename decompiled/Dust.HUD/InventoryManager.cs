using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using Dust.Strings;
using Microsoft.Xna.Framework;

namespace Dust.HUD
{
	public class InventoryManager
	{
		public int invSelection;

		public byte invCatMax = 6;

		public byte invSelMax = 60;

		public EquipItem[] equipItem;

		public Material[] material;

		public string categoryName = "Items";

		public string itemName = string.Empty;

		public string itemInfo = string.Empty;

		public string itemStats = string.Empty;

		public string itemControls = string.Empty;

		public int itemCost;

		public Dictionary<Vector3, string> itemControlsButtonList = new Dictionary<Vector3, string>();

		public InventoryManager()
		{
			this.equipItem = new EquipItem[this.invCatMax * this.invSelMax];
			this.material = new Material[Game1.stats.Material.Length];
			this.LoadEquipment();
			this.LoadMaterials(this.material);
			this.ResetInventoryManager();
		}

		public void ResetInventoryManager()
		{
			this.invSelection = 0;
			this.UpdateEquipStats(0, updateEquip: true, string.Empty);
			GC.Collect();
		}

		public void PopulateDescription(bool isShopping, int selection)
		{
			int num = selection / (int)this.invSelMax;
			string empty = string.Empty;
			if (Game1.hud.invSubStage == 0)
			{
				empty = ((num != 5 && num != 6) ? Strings_HudInv.EquipCategoryControls0 : Strings_HudInv.EquipCategoryControls4);
			}
			else
			{
				this.categoryName = Strings_HudInv.ResourceManager.GetString("EquipCategory" + num);
				switch (num)
				{
				default:
					empty = ((Game1.hud.invSubStage <= 1) ? ((!Game1.hud.CheckBluePrint(selection)) ? Strings_HudInv.EquipCategoryControls2 : ((Game1.stats.Equipment[320] <= 0) ? Strings_HudInv.EquipCategoryControls3a : Strings_HudInv.EquipCategoryControls3b)) : ((Game1.stats.Equipment[320] <= 0) ? Strings_HudInv.Return : Strings_HudInv.EquipCategoryControls3c));
					break;
				case 5:
				case 6:
					empty = Strings_HudInv.Return;
					break;
				case 0:
					empty = Strings_HudInv.EquipCategoryControls1;
					break;
				}
			}
			if (selection < 0 || selection > this.equipItem.Length || this.equipItem[selection] == null)
			{
				this.itemName = (this.itemInfo = (this.itemStats = string.Empty));
				this.itemCost = 0;
			}
			else
			{
				this.itemName = this.equipItem[selection].Name;
				this.itemInfo = this.equipItem[selection].Description;
				this.itemStats = this.equipItem[selection].StatInfo;
				this.itemCost = this.equipItem[selection].Value;
			}
			float size = 0.7f;
			if (!isShopping)
			{
				this.itemInfo = Game1.smallText.WordWrap(this.itemInfo, size, 455f, TextAlign.Left);
				this.itemControls = Game1.smallText.WordWrap(empty, size, 1000f, this.itemControlsButtonList, TextAlign.LeftAndCenter);
				Game1.hud.CheckItemDiff();
				if (selection < 0 || this.equipItem[selection] == null || (Game1.stats.Equipment[selection] < 1 && (num == 0 || (selection >= this.invSelMax && selection < this.invSelMax * 5 && Game1.stats.EquipBluePrint[selection - this.invSelMax] == 0))))
				{
					this.itemName = (this.itemInfo = (this.itemStats = string.Empty));
				}
			}
		}

		public void RemoveEquip(int category, int item, int removing)
		{
			if (category == 4)
			{
				if (item == Game1.stats.currentRingLeft && item == Game1.stats.currentRingRight)
				{
					removing++;
				}
				if (Game1.stats.Equipment[item] <= removing)
				{
					if (item == Game1.stats.currentRingLeft)
					{
						Game1.stats.currentRingLeft = -1;
					}
					if (item == Game1.stats.currentRingRight)
					{
						Game1.stats.currentRingRight = -1;
					}
				}
			}
			else if (Game1.stats.Equipment[item] <= removing)
			{
				if (item == Game1.stats.currentPendant)
				{
					Game1.stats.currentPendant = -1;
				}
				if (item == Game1.stats.currentAugment)
				{
					Game1.stats.currentAugment = -1;
				}
				if (item == Game1.stats.currentArmor)
				{
					Game1.stats.currentArmor = -1;
				}
			}
		}

		public bool ItemEquipped(int item, int category)
		{
			if (item == Game1.stats.currentItem)
			{
				return true;
			}
			if (item == Game1.stats.currentPendant)
			{
				return true;
			}
			if (item == Game1.stats.currentAugment)
			{
				return true;
			}
			if (item == Game1.stats.currentArmor)
			{
				return true;
			}
			if (item == Game1.stats.currentRingLeft)
			{
				return true;
			}
			if (item == Game1.stats.currentRingRight)
			{
				return true;
			}
			return false;
		}

		public int UpdateEquipStats(int category, bool updateEquip, string stat)
		{
			float attackEquip = 0f;
			float attackMult = 1f;
			float defenseEquip = 0f;
			float defenseMult = 1f;
			float fidgetMult = 1f;
			int luckEquip = 0;
			int regenEquip = 0;
			if ((!updateEquip && category == 1) || Game1.stats.currentPendant > -1)
			{
				int equip = ((updateEquip || category != 1) ? Game1.stats.currentPendant : this.invSelection);
				this.UpdateStats(ref attackEquip, ref attackMult, ref defenseEquip, ref defenseMult, ref fidgetMult, ref luckEquip, ref regenEquip, equip);
			}
			if ((!updateEquip && category == 2) || Game1.stats.currentAugment > -1)
			{
				int equip = ((updateEquip || category != 2) ? Game1.stats.currentAugment : this.invSelection);
				this.UpdateStats(ref attackEquip, ref attackMult, ref defenseEquip, ref defenseMult, ref fidgetMult, ref luckEquip, ref regenEquip, equip);
			}
			if ((!updateEquip && category == 3) || Game1.stats.currentArmor > -1)
			{
				int equip = ((updateEquip || category != 3) ? Game1.stats.currentArmor : this.invSelection);
				this.UpdateStats(ref attackEquip, ref attackMult, ref defenseEquip, ref defenseMult, ref fidgetMult, ref luckEquip, ref regenEquip, equip);
			}
			if ((!updateEquip && category == 4 && Game1.hud.equipSelection == 4) || Game1.stats.currentRingLeft > -1)
			{
				int equip = (((!updateEquip || Game1.stats.currentRingLeft <= -1) && (Game1.hud.equipSelection != 6 || Game1.stats.currentRingLeft <= -1) && (updateEquip || Game1.stats.currentRingLeft <= -1 || Game1.hud.equipSelection == 4)) ? this.invSelection : Game1.stats.currentRingLeft);
				this.UpdateStats(ref attackEquip, ref attackMult, ref defenseEquip, ref defenseMult, ref fidgetMult, ref luckEquip, ref regenEquip, equip);
			}
			if ((!updateEquip && category == 4 && Game1.hud.equipSelection == 6) || Game1.stats.currentRingRight > -1)
			{
				int equip = (((!updateEquip || Game1.stats.currentRingRight <= -1) && (Game1.hud.equipSelection != 4 || Game1.stats.currentRingRight <= -1) && (updateEquip || Game1.stats.currentRingRight <= -1 || Game1.hud.equipSelection == 6)) ? this.invSelection : Game1.stats.currentRingRight);
				this.UpdateStats(ref attackEquip, ref attackMult, ref defenseEquip, ref defenseMult, ref fidgetMult, ref luckEquip, ref regenEquip, equip);
			}
			if (updateEquip)
			{
				Game1.stats.attackEquip = (int)(((float)Game1.stats.attack + attackEquip) * attackMult);
				Game1.stats.defenseEquip = (int)(((float)Game1.stats.defense + defenseEquip) * defenseMult);
				Game1.stats.newFidgetEquip = (int)((float)Game1.stats.fidget * fidgetMult);
				Game1.stats.luckEquip = (int)MathHelper.Clamp(Game1.stats.luck + luckEquip, 0f, 100f);
				Game1.stats.newHealtRegen = regenEquip;
				return 0;
			}
			switch (stat)
			{
				case "attack": return (int)(((float)Game1.stats.attack + attackEquip) * attackMult);
				case "defense": return (int)(((float)Game1.stats.defense + defenseEquip) * defenseMult);
				case "fidget": return (int)((float)Game1.stats.fidget * fidgetMult);
				case "luck": return (int)MathHelper.Clamp(Game1.stats.luck + luckEquip, 0f, 100f);
				case "regen": return regenEquip;
			}
			return 0;
		}

		private void UpdateStats(ref float attackEquip, ref float attackMult, ref float defenseEquip, ref float defenseMult, ref float fidgetMult, ref int luckEquip, ref int regenEquip, int equip)
		{
			if (this.equipItem[equip] != null)
			{
				attackEquip += this.equipItem[equip].AttackAdd;
				attackMult *= this.equipItem[equip].AttackMult;
				defenseEquip += this.equipItem[equip].DefenseAdd;
				defenseMult *= this.equipItem[equip].DefenseMult;
				fidgetMult *= this.equipItem[equip].FidgetMult;
				luckEquip += this.equipItem[equip].Luck;
				regenEquip += this.equipItem[equip].Regen;
			}
		}

		private void LoadEquipment()
		{
			for (int i = 0; i < this.equipItem.Length; i++)
			{
				this.equipItem[i] = null;
			}
			for (int j = 0; j < Game1.stats.Equipment.Length; j++)
			{
				this.AddEquipmentList((EquipType)j, 0);
			}
			this.AddEquipmentList(EquipType.FeebleFruit, 80, 0, 20, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.LoutaNut, 160, 0, 40, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.CupCake, 240, 0, 60, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.Mushroom, 200, 1, 20, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.WallChicken, 300, 0, 80, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.CinnamonBun, 400, 0, 100, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.Chaldulbagi, 480, 0, 120, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.WarmPretzel, 480, 0, 120, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.KimBap, 560, 0, 140, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.DeadlyDelight, 600, 3, 1, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.HotCocoa, 640, 0, 160, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.HotDog, 720, 0, 180, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.Doenjang, 1000, 10, 200, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.Champong, 1000, 11, 200, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.IceCream, 600, 2, 100, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.BirthdayCake, 1000, 0, 250, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.BuffaloBurger, 1400, 11, 300, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.Lasagna, 1200, 0, 320, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.KingCrabLegs, 1400, 0, 350, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.Galbi, 1800, 10, 400, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.FilthyPendant, 200, 0, 0, 0, 1, 1, 1f, 1, 1f, 1f, new MaterialType[1] { MaterialType.Junk }, new byte[1] { 2 });
			this.AddEquipmentList(EquipType.WarpedPendant, 400, 0, 0, 0, 2, 2, 1f, 0, 1f, 1f, new MaterialType[2]
			{
				MaterialType.ScrapMetal,
				MaterialType.ImpClaw
			}, new byte[2] { 2, 2 });
			this.AddEquipmentList(EquipType.SimplePendant, 800, 0, 0, 0, 1, 2, 1f, 4, 1f, 1f, new MaterialType[2]
			{
				MaterialType.ScrapMetal,
				MaterialType.BeastSpear
			}, new byte[2] { 1, 4 });
			this.AddEquipmentList(EquipType.ChildPendant, 1200, 0, 0, 0, 1, 0, 1f, 0, 1.1f, 1f, new MaterialType[3]
			{
				MaterialType.Cloth,
				MaterialType.Thread,
				MaterialType.Glue
			}, new byte[3] { 3, 3, 2 });
			this.AddEquipmentList(EquipType.OrnamentalPendant, 1200, 0, 0, 0, 2, 0, 1f, 10, 1f, 1f, new MaterialType[3]
			{
				MaterialType.AveeClaw,
				MaterialType.BeastLeather,
				MaterialType.ImpHide
			}, new byte[3] { 4, 4, 4 });
			this.AddEquipmentList(EquipType.CreaturePendant, 1600, 0, 0, 0, 3, 6, 1f, 0, 1f, 1f, new MaterialType[3]
			{
				MaterialType.SlimySpike,
				MaterialType.GiantCore,
				MaterialType.FlornSpark
			}, new byte[3] { 3, 2, 4 });
			this.AddEquipmentList(EquipType.HalmeoniPendant, 2000, 0, 0, 0, 4, 2, 1f, 12, 1f, 1f, new MaterialType[3]
			{
				MaterialType.Nails,
				MaterialType.HoundHide,
				MaterialType.StoneCarapace
			}, new byte[3] { 4, 4, 2 });
			this.AddEquipmentList(EquipType.HarabeojiPendant, 2000, 0, 0, 0, 6, 10, 1f, 4, 1f, 1f, new MaterialType[3]
			{
				MaterialType.Nails,
				MaterialType.SquirtArm,
				MaterialType.StoneFeeler
			}, new byte[3] { 4, 4, 2 });
			this.AddEquipmentList(EquipType.PoorMansPendant, 800, 0, 0, 0, 6, 6, 1f, 6, 1f, 1f, new MaterialType[1] { MaterialType.Wire }, new byte[1] { 10 });
			this.AddEquipmentList(EquipType.UglyPendant, 1000, 0, 0, 0, 0, 0, 0.1f, 0, 0.1f, 0.1f, new MaterialType[4]
			{
				MaterialType.ImpHide,
				MaterialType.SlimyCoat,
				MaterialType.HoundHide,
				MaterialType.HoundTeeth
			}, new byte[4] { 4, 2, 1, 1 });
			this.AddEquipmentList(EquipType.BeautyPendant, 3000, 0, 0, 1, 8, 60, 1f, 40, 1f, 1f, new MaterialType[4]
			{
				MaterialType.Cloth,
				MaterialType.Thread,
				MaterialType.Dye,
				MaterialType.Bone
			}, new byte[4] { 2, 4, 6, 2 });
			this.AddEquipmentList(EquipType.SaintPendant, 4000, 0, 0, 4, 10, 0, 1f, 100, 1f, 1f, new MaterialType[4]
			{
				MaterialType.ScrapMetal,
				MaterialType.Cotton,
				MaterialType.Paper,
				MaterialType.LostSoul
			}, new byte[4] { 4, 10, 10, 2 });
			this.AddEquipmentList(EquipType.CowardsPendant, 5000, 0, 0, 2, 20, 0, 0.5f, 0, 1f, 1f, new MaterialType[1] { MaterialType.Maggot }, new byte[1] { 20 });
			this.AddEquipmentList(EquipType.MadManPendant, 5000, 0, 0, 0, 12, 80, 1f, 0, 1f, 1f, new MaterialType[3]
			{
				MaterialType.Nails,
				MaterialType.LostSoul,
				MaterialType.TrolkFinger
			}, new byte[3] { 10, 2, 5 });
			this.AddEquipmentList(EquipType.MarshPendant, 6000, 0, 0, 4, 14, 100, 1f, 80, 1f, 1f, new MaterialType[3]
			{
				MaterialType.Maggot,
				MaterialType.Bone,
				MaterialType.KushPelt
			}, new byte[3] { 20, 10, 4 });
			this.AddEquipmentList(EquipType.WarriorsPendant, 7000, 0, 0, 0, 16, 0, 1.2f, 0, 1.2f, 1.2f, new MaterialType[2]
			{
				MaterialType.WolfPelt,
				MaterialType.KushPelt
			}, new byte[2] { 6, 4 });
			this.AddEquipmentList(EquipType.WisdomPendant, 8000, 0, 0, 12, 18, 0, 1f, 0, 2f, 1.2f, new MaterialType[3]
			{
				MaterialType.HollowShard,
				MaterialType.LostSoul,
				MaterialType.Ectoplasm
			}, new byte[3] { 6, 10, 10 });
			this.AddEquipmentList(EquipType.BattleMastersPendant, 10000, 0, 0, 0, 20, 0, 2f, 0, 1f, 1.5f, new MaterialType[3]
			{
				MaterialType.ToughMetal,
				MaterialType.DogTag,
				MaterialType.KushPelt
			}, new byte[3] { 10, 20, 6 });
			this.AddEquipmentList(EquipType.EggShellAug, 40, 0, 0, 0, 0, 1, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.SandPaperAug, 80, 0, 0, 0, 0, 2, 1f, 0, 1f, 1f, new MaterialType[1] { MaterialType.Paper }, new byte[1] { 2 });
			this.AddEquipmentList(EquipType.DullWhetAug, 200, 0, 0, 0, 0, 6, 1f, 0, 1f, 1f, new MaterialType[1] { MaterialType.ScrapMetal }, new byte[1] { 1 });
			this.AddEquipmentList(EquipType.SmoothWhetAug, 300, 0, 0, 0, 0, 8, 1f, 0, 1f, 1f, new MaterialType[2]
			{
				MaterialType.ScrapMetal,
				MaterialType.BeastLeather
			}, new byte[2] { 3, 1 });
			this.AddEquipmentList(EquipType.ImpAug, 400, 0, 0, 0, 0, 10, 1f, 0, 1f, 1.1f, new MaterialType[3]
			{
				MaterialType.ImpHide,
				MaterialType.ImpClaw,
				MaterialType.Bottle
			}, new byte[3] { 4, 4, 1 });
			this.AddEquipmentList(EquipType.SpearAug, 600, 0, 0, 0, 0, 20, 1f, 0, 1f, 1.1f, new MaterialType[2]
			{
				MaterialType.Nails,
				MaterialType.BeastSpear
			}, new byte[2] { 4, 4 });
			this.AddEquipmentList(EquipType.GiantAug, 600, 0, 0, 0, 0, 30, 1f, 0, 1f, 1.1f, new MaterialType[3]
			{
				MaterialType.GiantCore,
				MaterialType.GiantRock,
				MaterialType.Bottle
			}, new byte[3] { 2, 2, 1 });
			this.AddEquipmentList(EquipType.BrutalAug, 800, 0, 0, 0, 0, 40, 1f, 0, 1f, 1.1f, new MaterialType[2]
			{
				MaterialType.Lumber,
				MaterialType.Nails
			}, new byte[2] { 2, 6 });
			this.AddEquipmentList(EquipType.ScrapAug, 1000, 0, 0, 0, 0, 50, 1f, 0, 1f, 1.2f, new MaterialType[1] { MaterialType.ScrapMetal }, new byte[1] { 10 });
			this.AddEquipmentList(EquipType.ElectricAug, 1200, 0, 0, 0, 0, 60, 1f, 0, 1f, 1.2f, new MaterialType[3]
			{
				MaterialType.FlornSpark,
				MaterialType.FlornTentacle,
				MaterialType.ScrapMetal
			}, new byte[3] { 4, 4, 1 });
			this.AddEquipmentList(EquipType.TeethAug, 1600, 0, 0, 0, 0, 80, 1f, 0, 1f, 1.2f, new MaterialType[3]
			{
				MaterialType.HoundTeeth,
				MaterialType.HoundHide,
				MaterialType.Thread
			}, new byte[3] { 2, 1, 1 });
			this.AddEquipmentList(EquipType.TrolkFistAug, 2400, 0, 0, 0, 0, 120, 1f, 0, 1f, 1.2f, new MaterialType[3]
			{
				MaterialType.TrolkFinger,
				MaterialType.TrolkShell,
				MaterialType.Wire
			}, new byte[3] { 1, 4, 4 });
			this.AddEquipmentList(EquipType.FleshAug, 3000, 0, 0, 0, 0, 150, 1f, 0, 1f, 1.3f, new MaterialType[3]
			{
				MaterialType.FleshChunk,
				MaterialType.Bone,
				MaterialType.Wire
			}, new byte[3] { 20, 4, 4 });
			this.AddEquipmentList(EquipType.SickAug, 4400, 0, 0, 0, 0, 220, 1f, 0, 1f, 1.4f, new MaterialType[4]
			{
				MaterialType.SlimyCoat,
				MaterialType.Ectoplasm,
				MaterialType.Maggot,
				MaterialType.Bottle
			}, new byte[4] { 2, 1, 20, 1 });
			this.AddEquipmentList(EquipType.WickedAug, 5000, 0, 0, 0, 0, 250, 1f, 0, 1f, 1.5f, new MaterialType[3]
			{
				MaterialType.LostSoul,
				MaterialType.Ectoplasm,
				MaterialType.SquirtEye
			}, new byte[3] { 6, 10, 2 });
			this.AddEquipmentList(EquipType.MountaingAug, 5600, 0, 0, 0, 0, 280, 1f, 0, 1f, 1.6f, new MaterialType[3]
			{
				MaterialType.WolfPelt,
				MaterialType.Bone,
				MaterialType.Junk
			}, new byte[3] { 2, 10, 10 });
			this.AddEquipmentList(EquipType.FrozenAug, 6400, 0, 0, 0, 0, 320, 1f, 0, 1f, 1.7f, new MaterialType[3]
			{
				MaterialType.HollowShard,
				MaterialType.LostSoul,
				MaterialType.ToughMetal
			}, new byte[3] { 4, 2, 1 });
			this.AddEquipmentList(EquipType.MetalAug, 7200, 0, 0, 0, 0, 360, 1f, 0, 1f, 1.8f, new MaterialType[2]
			{
				MaterialType.ToughMetal,
				MaterialType.ScrapMetal
			}, new byte[2] { 4, 20 });
			this.AddEquipmentList(EquipType.SoldierAug, 8000, 0, 0, 0, 0, 400, 1f, 0, 1f, 1.9f, new MaterialType[3]
			{
				MaterialType.DogTag,
				MaterialType.ToughMetal,
				MaterialType.Lumber
			}, new byte[3] { 10, 10, 20 });
			this.AddEquipmentList(EquipType.MithrarinAug, 12000, 0, 0, 0, 0, 800, 1f, 0, 1f, 2f, new MaterialType[4]
			{
				MaterialType.LostSoul,
				MaterialType.HollowShard,
				MaterialType.ToughMetal,
				MaterialType.DogTag
			}, new byte[4] { 10, 10, 15, 20 });
			this.AddEquipmentList(EquipType.LeafArmor, 40, 0, 0, 0, 0, 0, 1f, 1, 1f, 1f, new MaterialType[0], new byte[0]);
			MaterialType[] matReq = new MaterialType[1];
			this.AddEquipmentList(EquipType.BarkArmor, 80, 0, 0, 0, 0, 0, 1f, 2, 1f, 1f, matReq, new byte[1] { 4 });
			this.AddEquipmentList(EquipType.CottonArmor, 160, 0, 0, 0, 1, 0, 1f, 4, 1f, 1f, new MaterialType[2]
			{
				MaterialType.Cotton,
				MaterialType.Thread
			}, new byte[2] { 4, 1 });
			this.AddEquipmentList(EquipType.LightChain, 600, 0, 0, 0, 1, 0, 1f, 10, 1f, 1f, new MaterialType[3]
			{
				MaterialType.Nails,
				MaterialType.FlornSpark,
				MaterialType.ScrapMetal
			}, new byte[3] { 5, 2, 2 });
			this.AddEquipmentList(EquipType.SlimeArmor, 900, 0, 0, 0, 2, 0, 1f, 12, 1f, 1f, new MaterialType[2]
			{
				MaterialType.SlimyCoat,
				MaterialType.SlimySpike
			}, new byte[2] { 4, 4 });
			this.AddEquipmentList(EquipType.LightStoneArmor, 1200, 0, 0, 0, 2, 0, 1f, 15, 1f, 1f, new MaterialType[2]
			{
				MaterialType.GiantRock,
				MaterialType.Wire
			}, new byte[2] { 4, 10 });
			this.AddEquipmentList(EquipType.ElectricVest, 2000, 0, 0, 0, 2, 0, 1f, 20, 1f, 1f, new MaterialType[3]
			{
				MaterialType.FlornSpark,
				MaterialType.FlornTentacle,
				MaterialType.Cloth
			}, new byte[3] { 2, 2, 5 });
			this.AddEquipmentList(EquipType.WetSuit, 2500, 0, 0, 0, 4, 0, 1f, 25, 1f, 1f, new MaterialType[3]
			{
				MaterialType.HoundHide,
				MaterialType.SlimyCoat,
				MaterialType.Cotton
			}, new byte[3] { 4, 2, 10 });
			this.AddEquipmentList(EquipType.MinerGear, 3500, 0, 0, 0, 4, 0, 1f, 35, 1f, 1f, new MaterialType[4]
			{
				MaterialType.FlornSpark,
				MaterialType.SquirtEye,
				MaterialType.SquirtArm,
				MaterialType.TrolkShell
			}, new byte[4] { 2, 2, 2, 1 });
			this.AddEquipmentList(EquipType.TrolkArmor, 4000, 0, 0, 0, 4, 0, 1f, 40, 1f, 1f, new MaterialType[3]
			{
				MaterialType.TrolkFinger,
				MaterialType.TrolkShell,
				MaterialType.Cloth
			}, new byte[3] { 4, 6, 10 });
			this.AddEquipmentList(EquipType.DetectiveCoat, 8000, 0, 0, 0, 6, 0, 1f, 80, 1f, 1f, new MaterialType[3]
			{
				MaterialType.Cloth,
				MaterialType.Thread,
				MaterialType.Ectoplasm
			}, new byte[3] { 20, 20, 4 });
			this.AddEquipmentList(EquipType.WrongVest, 12000, 0, 0, 0, 6, 0, 1f, 120, 1f, 1f, new MaterialType[4]
			{
				MaterialType.FleshChunk,
				MaterialType.Bone,
				MaterialType.Maggot,
				MaterialType.Thread
			}, new byte[4] { 20, 4, 20, 20 });
			this.AddEquipmentList(EquipType.SpectralVest, 15000, 0, 0, 2, 6, 0, 1f, 150, 1f, 1f, new MaterialType[4]
			{
				MaterialType.LostSoul,
				MaterialType.Ectoplasm,
				MaterialType.Cloth,
				MaterialType.FlornSpark
			}, new byte[4] { 6, 10, 10, 1 });
			this.AddEquipmentList(EquipType.WarmSweater, 20000, 0, 0, 1, 8, 0, 1f, 180, 1f, 1f, new MaterialType[4]
			{
				MaterialType.WolfPelt,
				MaterialType.Cloth,
				MaterialType.Cotton,
				MaterialType.Thread
			}, new byte[4] { 4, 10, 10, 20 });
			this.AddEquipmentList(EquipType.SnowSuit, 25000, 0, 0, 2, 8, 0, 1f, 200, 1f, 1f, new MaterialType[3]
			{
				MaterialType.WolfPelt,
				MaterialType.HollowShard,
				MaterialType.Cotton
			}, new byte[3] { 10, 5, 20 });
			this.AddEquipmentList(EquipType.MountainGear, 30000, 0, 0, 4, 8, 0, 1f, 220, 1f, 1f, new MaterialType[5]
			{
				MaterialType.KushPelt,
				MaterialType.WolfPelt,
				MaterialType.Wire,
				MaterialType.ScrapMetal,
				MaterialType.HollowShard
			}, new byte[5] { 4, 6, 20, 20, 2 });
			this.AddEquipmentList(EquipType.LavaGear, 35000, 0, 0, 6, 10, 0, 1f, 250, 1f, 1f, new MaterialType[4]
			{
				MaterialType.ToughMetal,
				MaterialType.WolfPelt,
				MaterialType.Glue,
				MaterialType.Cotton
			}, new byte[4] { 2, 4, 20, 20 });
			this.AddEquipmentList(EquipType.SoldierArmor, 40000, 0, 0, 8, 10, 0, 1f, 280, 1f, 1f, new MaterialType[4]
			{
				MaterialType.DogTag,
				MaterialType.ToughMetal,
				MaterialType.KushPelt,
				MaterialType.Cloth
			}, new byte[4] { 20, 10, 5, 40 });
			this.AddEquipmentList(EquipType.MithrarinRobe, 60000, 0, 0, 10, 10, 0, 1f, 300, 1f, 1f, new MaterialType[6]
			{
				MaterialType.KushPelt,
				MaterialType.LostSoul,
				MaterialType.HollowShard,
				MaterialType.WolfPelt,
				MaterialType.Cotton,
				MaterialType.Dye
			}, new byte[6] { 10, 10, 6, 16, 40, 20 });
			this.AddEquipmentList(EquipType.DirtyRing, 20, 0, 0, 0, 2, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.DullRing, 100, 0, 0, 0, 2, 0, 1f, 1, 1f, 1f, new MaterialType[0], new byte[0]);
			this.AddEquipmentList(EquipType.SmoothRing, 600, 0, 0, 0, 2, 1, 1f, 4, 1f, 1f, new MaterialType[1] { MaterialType.ScrapMetal }, new byte[0]);
			this.AddEquipmentList(EquipType.LightRing, 2000, 0, 0, 0, 0, 4, 1f, 4, 1f, 1f, new MaterialType[2]
			{
				MaterialType.FlornSpark,
				MaterialType.Wire
			}, new byte[0]);
			this.AddEquipmentList(EquipType.ScavengerRing, 1000, 30, 0, 0, 4, 2, 1f, 4, 1f, 1f, new MaterialType[1] { MaterialType.Junk }, new byte[1] { 10 });
			this.AddEquipmentList(EquipType.VigilanceRing, 2000, 31, 0, 0, 6, 2, 1f, 4, 1f, 1f, new MaterialType[2]
			{
				MaterialType.Paper,
				MaterialType.ScrapMetal
			}, new byte[2] { 10, 10 });
			this.AddEquipmentList(EquipType.MinerRing, 2000, 20, 0, 0, 6, 10, 1f, 0, 1f, 1f, new MaterialType[2]
			{
				MaterialType.ImpClaw,
				MaterialType.TrolkFinger
			}, new byte[2] { 6, 2 });
			this.AddEquipmentList(EquipType.WealthRing, 5000, 21, 0, 1, 8, 20, 1f, 0, 1f, 1f, new MaterialType[3]
			{
				MaterialType.SquirtArm,
				MaterialType.Paper,
				MaterialType.Dye
			}, new byte[3] { 4, 10, 10 });
			this.AddEquipmentList(EquipType.ApprenticeRing, 4000, 40, 0, 1, 8, 10, 1f, 0, 1f, 1f, new MaterialType[3]
			{
				MaterialType.AveeWing,
				MaterialType.FlornTentacle,
				MaterialType.StoneFeeler
			}, new byte[3] { 4, 4, 4 });
			this.AddEquipmentList(EquipType.ArtisanRing, 12000, 41, 0, 2, 10, 20, 1f, 0, 1f, 1f, new MaterialType[3]
			{
				MaterialType.Paper,
				MaterialType.Dye,
				MaterialType.Thread
			}, new byte[3] { 20, 20, 20 });
			this.AddEquipmentList(EquipType.PatienceRing, 200, 0, 0, 4, 10, 0, 1f, 10, 1f, 1.2f, new MaterialType[2]
			{
				MaterialType.LostSoul,
				MaterialType.Maggot
			}, new byte[2] { 2, 20 });
			this.AddEquipmentList(EquipType.BrokenRing, 1000, 0, 0, 8, 12, 2, 1f, 12, 1f, 1.5f, new MaterialType[3]
			{
				MaterialType.ScrapMetal,
				MaterialType.Junk,
				MaterialType.Bone
			}, new byte[3] { 20, 50, 20 });
			this.AddEquipmentList(EquipType.StudyRing, 4000, 0, 0, 8, 12, 4, 1f, 14, 1f, 2f, new MaterialType[2]
			{
				MaterialType.Bottle,
				MaterialType.Ectoplasm
			}, new byte[2] { 10, 20 });
			this.AddEquipmentList(EquipType.FocusRing, 10000, 0, 0, 10, 14, 8, 1f, 18, 1f, 2.5f, new MaterialType[2]
			{
				MaterialType.HollowShard,
				MaterialType.SquirtEye
			}, new byte[2] { 4, 20 });
			this.AddEquipmentList(EquipType.DevastationRing, 20000, 0, 0, 10, 16, 10, 1f, 20, 1f, 3f, new MaterialType[3]
			{
				MaterialType.LostSoul,
				MaterialType.ToughMetal,
				MaterialType.Nails
			}, new byte[3] { 10, 10, 100 });
			this.AddEquipmentList(EquipType.WeddingRing, 40000, 50, 0, 20, 20, 20, 1f, 20, 1f, 2f, new MaterialType[4]
			{
				MaterialType.HollowShard,
				MaterialType.LostSoul,
				MaterialType.Paper,
				MaterialType.Dye
			}, new byte[4] { 20, 20, 100, 100 });
			this.AddEquipmentList(EquipType.TeleportStone, 400);
			this.AddEquipmentList(EquipType.RevivalStone, 8000);
			this.AddEquipmentList(EquipType.TreasureKey, 3000);
			this.AddEquipmentList(EquipType.RedOrb, 6000);
			this.AddEquipmentList(EquipType.FidgetDoll, 2);
			this.AddEquipmentList(EquipType.Cooler, 4000);
		}

		private void AddEquipmentList(EquipType equipType, int _value)
		{
			this.AddEquipmentList(equipType, _value, 0, 0, 0, 0, 0, 1f, 0, 1f, 1f, new MaterialType[0], new byte[0]);
		}

		private void AddEquipmentList(EquipType equipType, int _value, int _flag, int _hpGain, int _regen, int _luck, int _augAdd, float _augMult, int _armAdd, float _armMult, float _fidget, MaterialType[] matReq, byte[] matReqAmt)
		{
			string text = $"{(int)equipType:D3}";
			string text2 = string.Empty;
			string desc = string.Empty;
			string empty = string.Empty;
			int[] array = new int[6];
			byte[] array2 = new byte[6];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = -1;
				array2[i] = 1;
				if (matReq.Length > i)
				{
					array[i] = (int)matReq[i];
				}
				if (matReqAmt.Length > i)
				{
					array2[i] = matReqAmt[i];
				}
			}
			ResourceManager resourceManager = Strings_Equipment.ResourceManager;
			ResourceSet resourceSet = resourceManager.GetResourceSet(CultureInfo.InvariantCulture, createIfNotExists: true, tryParents: true);
			if (resourceSet != null)
			{
				foreach (DictionaryEntry item in resourceSet)
				{
					string text3 = (string)item.Key;
					string @string = resourceManager.GetString(text3);
					if (!text3.StartsWith(text))
					{
						continue;
					}
					if (text3 == text)
					{
						text2 = @string;
						continue;
					}
					string[] array3 = text3.Split('_');
					if (array3.Length > 1)
					{
						desc = @string;
					}
				}
			}
			if (text2 != string.Empty)
			{
				empty = this.PopulateEquipStats(_flag, _hpGain, _regen, _luck, _augAdd, _augMult, _armAdd, _armMult, _fidget);
				if (equipType >= EquipType.BlueKey && equipType < EquipType.CoraQuest01)
				{
					desc = Strings_Equipment._321_Desc;
				}
				this.equipItem[(int)equipType] = new EquipItem(text2, desc, empty, _value, _flag, _hpGain, _regen, _luck, _augAdd, _augMult, _armAdd, _armMult, _fidget, array, array2);
			}
		}

		private string PopulateEquipStats(int _flag, int _hpGain, int _regen, int _luck, int _augAdd, float _augMult, int _armAdd, float _armMult, float _fidget)
		{
			List<string> list = new List<string>();
			if (_hpGain > 0)
			{
				list.Add(Strings_EquipDetails.SymbolAddition + _hpGain + " " + Strings_EquipDetails.Health);
			}
			if (_regen != 0)
			{
				list.Add((_regen < 0) ? Strings_EquipDetails.SymbolSubtraction : (Strings_EquipDetails.SymbolAddition + _regen + " " + Strings_EquipDetails.Regen));
			}
			if (_augAdd != 0)
			{
				list.Add((_augAdd < 0) ? Strings_EquipDetails.SymbolSubtraction : (Strings_EquipDetails.SymbolAddition + _augAdd + " " + Strings_EquipDetails.Attack));
			}
			if (_armAdd != 0)
			{
				list.Add((_armAdd < 0) ? Strings_EquipDetails.SymbolSubtraction : (Strings_EquipDetails.SymbolAddition + _armAdd + " " + Strings_EquipDetails.Defense));
			}
			if (_augMult != 1f)
			{
				list.Add(Strings_EquipDetails.SymbolMultiplication + _augMult + " " + Strings_EquipDetails.Attack);
			}
			if (_armMult != 1f)
			{
				list.Add(Strings_EquipDetails.SymbolMultiplication + _armMult + " " + Strings_EquipDetails.Defense);
			}
			if (_fidget != 1f)
			{
				list.Add(Strings_EquipDetails.SymbolMultiplication + _fidget + " " + Strings_EquipDetails.Fidget);
			}
			if (_flag > 0)
			{
				list.Add(Strings_EquipDetails.ResourceManager.GetString("Flag" + $"{_flag:D2}"));
			}
			if (_luck > 0)
			{
				list.Add(Strings_EquipDetails.SymbolAddition + _luck + " " + Strings_EquipDetails.Luck);
			}
			string text = string.Empty;
			for (int i = 0; i < list.Count; i++)
			{
				if (text != string.Empty)
				{
					text += ", ";
				}
				text += list[i];
			}
			return text;
		}

		private void LoadMaterials(Material[] material)
		{
			Game1.stats.maxMaterials = 0;
			ResourceSet resourceSet = Strings_Materials.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, createIfNotExists: true, tryParents: true);
			if (resourceSet == null)
			{
				return;
			}
			int num = -1;
			int value = 0;
			string empty = string.Empty;
			string empty2 = string.Empty;
			for (int i = 0; i < material.Length; i++)
			{
				string value2 = $"{i:D3}";
				num = i;
				empty = (empty2 = string.Empty);
				foreach (DictionaryEntry item in resourceSet)
				{
					string text = (string)item.Key;
					if (text.StartsWith(value2))
					{
						if (!text.Contains("_"))
						{
							empty = Strings_Materials.ResourceManager.GetString(text);
						}
						else
						{
							empty2 = text;
							string[] array = text.Split('_');
							value = short.Parse(array[2]);
						}
					}
					if (empty != string.Empty && empty2 != string.Empty)
					{
						break;
					}
				}
				if (empty != string.Empty)
				{
					material[num] = new Material(empty, empty2, value);
					Game1.stats.maxMaterials++;
				}
			}
		}
	}
}
