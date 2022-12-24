using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Dust.Strings
{
	[DebuggerNonUserCode]
	[CompilerGenerated]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	internal class Strings_PC
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Strings_PC.resourceMan, null))
				{
					ResourceManager resourceManager = (Strings_PC.resourceMan = new ResourceManager("Dust.Strings.Strings_PC", typeof(Strings_PC).Assembly));
				}
				return Strings_PC.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Strings_PC.resourceCulture;
			}
			set
			{
				Strings_PC.resourceCulture = value;
			}
		}

		internal static string AchievementLocal => Strings_PC.ResourceManager.GetString("AchievementLocal", Strings_PC.resourceCulture);

		internal static string AchievementSecret => Strings_PC.ResourceManager.GetString("AchievementSecret", Strings_PC.resourceCulture);

		internal static string Begin => Strings_PC.ResourceManager.GetString("Begin", Strings_PC.resourceCulture);

		internal static string CopyConfirm => Strings_PC.ResourceManager.GetString("CopyConfirm", Strings_PC.resourceCulture);

		internal static string DeleteConfirm => Strings_PC.ResourceManager.GetString("DeleteConfirm", Strings_PC.resourceCulture);

		internal static string HighScores => Strings_PC.ResourceManager.GetString("HighScores", Strings_PC.resourceCulture);

		internal static string Legend => Strings_PC.ResourceManager.GetString("Legend", Strings_PC.resourceCulture);

		internal static string LoadConfirm => Strings_PC.ResourceManager.GetString("LoadConfirm", Strings_PC.resourceCulture);

		internal static string LoadControls => Strings_PC.ResourceManager.GetString("LoadControls", Strings_PC.resourceCulture);

		internal static string Main5Desc => Strings_PC.ResourceManager.GetString("Main5Desc", Strings_PC.resourceCulture);

		internal static string MouseButtonLeft => Strings_PC.ResourceManager.GetString("MouseButtonLeft", Strings_PC.resourceCulture);

		internal static string MouseButtonMid => Strings_PC.ResourceManager.GetString("MouseButtonMid", Strings_PC.resourceCulture);

		internal static string MouseButtonRight => Strings_PC.ResourceManager.GetString("MouseButtonRight", Strings_PC.resourceCulture);

		internal static string QuestControls => Strings_PC.ResourceManager.GetString("QuestControls", Strings_PC.resourceCulture);

		internal static string ShopBuy => Strings_PC.ResourceManager.GetString("ShopBuy", Strings_PC.resourceCulture);

		internal static string ShopPageSwitch => Strings_PC.ResourceManager.GetString("ShopPageSwitch", Strings_PC.resourceCulture);

		internal static string ShopPurchaseMaterial => Strings_PC.ResourceManager.GetString("ShopPurchaseMaterial", Strings_PC.resourceCulture);

		internal static string ShopSell => Strings_PC.ResourceManager.GetString("ShopSell", Strings_PC.resourceCulture);

		internal static string UpgradeComplete => Strings_PC.ResourceManager.GetString("UpgradeComplete", Strings_PC.resourceCulture);

		internal Strings_PC()
		{
		}
	}
}
