using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Dust.Strings
{
	[DebuggerNonUserCode]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[CompilerGenerated]
	internal class Strings_Shop
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Strings_Shop.resourceMan, null))
				{
					ResourceManager resourceManager = (Strings_Shop.resourceMan = new ResourceManager("Dust.Strings.Strings_Shop", typeof(Strings_Shop).Assembly));
				}
				return Strings_Shop.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Strings_Shop.resourceCulture;
			}
			set
			{
				Strings_Shop.resourceCulture = value;
			}
		}

		internal static string Amount => Strings_Shop.ResourceManager.GetString("Amount", Strings_Shop.resourceCulture);

		internal static string ConfirmBuild => Strings_Shop.ResourceManager.GetString("ConfirmBuild", Strings_Shop.resourceCulture);

		internal static string ConfirmCraft => Strings_Shop.ResourceManager.GetString("ConfirmCraft", Strings_Shop.resourceCulture);

		internal static string ConfirmPurchase => Strings_Shop.ResourceManager.GetString("ConfirmPurchase", Strings_Shop.resourceCulture);

		internal static string ConfirmSell => Strings_Shop.ResourceManager.GetString("ConfirmSell", Strings_Shop.resourceCulture);

		internal static string Cost => Strings_Shop.ResourceManager.GetString("Cost", Strings_Shop.resourceCulture);

		internal static string ItemEquipped => Strings_Shop.ResourceManager.GetString("ItemEquipped", Strings_Shop.resourceCulture);

		internal static string ItemPurchased => Strings_Shop.ResourceManager.GetString("ItemPurchased", Strings_Shop.resourceCulture);

		internal static string ItemStocked => Strings_Shop.ResourceManager.GetString("ItemStocked", Strings_Shop.resourceCulture);

		internal static string ItemUnavailableMe => Strings_Shop.ResourceManager.GetString("ItemUnavailableMe", Strings_Shop.resourceCulture);

		internal static string ItemUnavailableShop => Strings_Shop.ResourceManager.GetString("ItemUnavailableShop", Strings_Shop.resourceCulture);

		internal static string MaterialCatalogued => Strings_Shop.ResourceManager.GetString("MaterialCatalogued", Strings_Shop.resourceCulture);

		internal static string MaterialInStock => Strings_Shop.ResourceManager.GetString("MaterialInStock", Strings_Shop.resourceCulture);

		internal static string MyGold => Strings_Shop.ResourceManager.GetString("MyGold", Strings_Shop.resourceCulture);

		internal static string MyStock => Strings_Shop.ResourceManager.GetString("MyStock", Strings_Shop.resourceCulture);

		internal static string Profit => Strings_Shop.ResourceManager.GetString("Profit", Strings_Shop.resourceCulture);

		internal static string PurchaseModeBuy => Strings_Shop.ResourceManager.GetString("PurchaseModeBuy", Strings_Shop.resourceCulture);

		internal static string PurchaseModeSell => Strings_Shop.ResourceManager.GetString("PurchaseModeSell", Strings_Shop.resourceCulture);

		internal static string ResourcesMissing => Strings_Shop.ResourceManager.GetString("ResourcesMissing", Strings_Shop.resourceCulture);

		internal static string ShopControlsBuy => Strings_Shop.ResourceManager.GetString("ShopControlsBuy", Strings_Shop.resourceCulture);

		internal static string ShopControlsConfirm => Strings_Shop.ResourceManager.GetString("ShopControlsConfirm", Strings_Shop.resourceCulture);

		internal static string ShopControlsSell => Strings_Shop.ResourceManager.GetString("ShopControlsSell", Strings_Shop.resourceCulture);

		internal static string ShopControlsSort => Strings_Shop.ResourceManager.GetString("ShopControlsSort", Strings_Shop.resourceCulture);

		internal static string ShopControlsSwitch => Strings_Shop.ResourceManager.GetString("ShopControlsSwitch", Strings_Shop.resourceCulture);

		internal static string ShopMaterial => Strings_Shop.ResourceManager.GetString("ShopMaterial", Strings_Shop.resourceCulture);

		internal static string Total => Strings_Shop.ResourceManager.GetString("Total", Strings_Shop.resourceCulture);

		internal Strings_Shop()
		{
		}
	}
}
