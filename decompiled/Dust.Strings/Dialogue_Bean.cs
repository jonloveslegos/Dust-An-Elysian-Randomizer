using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Dust.Strings
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[CompilerGenerated]
	[DebuggerNonUserCode]
	internal class Dialogue_Bean
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Dialogue_Bean.resourceMan, null))
				{
					ResourceManager resourceManager = (Dialogue_Bean.resourceMan = new ResourceManager("Dust.Strings.Dialogue_Bean", typeof(Dialogue_Bean).Assembly));
				}
				return Dialogue_Bean.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Dialogue_Bean.resourceCulture;
			}
			set
			{
				Dialogue_Bean.resourceCulture = value;
			}
		}

		internal static string _000_000 => Dialogue_Bean.ResourceManager.GetString("000_000", Dialogue_Bean.resourceCulture);

		internal static string _000_010 => Dialogue_Bean.ResourceManager.GetString("000_010", Dialogue_Bean.resourceCulture);

		internal static string _000_020 => Dialogue_Bean.ResourceManager.GetString("000_020", Dialogue_Bean.resourceCulture);

		internal static string _001_000 => Dialogue_Bean.ResourceManager.GetString("001_000", Dialogue_Bean.resourceCulture);

		internal static string _001_010 => Dialogue_Bean.ResourceManager.GetString("001_010", Dialogue_Bean.resourceCulture);

		internal static string _002_000 => Dialogue_Bean.ResourceManager.GetString("002_000", Dialogue_Bean.resourceCulture);

		internal Dialogue_Bean()
		{
		}
	}
}
