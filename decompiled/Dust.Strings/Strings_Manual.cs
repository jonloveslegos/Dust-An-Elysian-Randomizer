using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Dust.Strings
{
	[CompilerGenerated]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	internal class Strings_Manual
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Strings_Manual.resourceMan, null))
				{
					ResourceManager resourceManager = (Strings_Manual.resourceMan = new ResourceManager("Dust.Strings.Strings_Manual", typeof(Strings_Manual).Assembly));
				}
				return Strings_Manual.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Strings_Manual.resourceCulture;
			}
			set
			{
				Strings_Manual.resourceCulture = value;
			}
		}

		internal static string ContentControls => Strings_Manual.ResourceManager.GetString("ContentControls", Strings_Manual.resourceCulture);

		internal static string ContentTitle => Strings_Manual.ResourceManager.GetString("ContentTitle", Strings_Manual.resourceCulture);

		internal static string Controls => Strings_Manual.ResourceManager.GetString("Controls", Strings_Manual.resourceCulture);

		internal static string Manual => Strings_Manual.ResourceManager.GetString("Manual", Strings_Manual.resourceCulture);

		internal Strings_Manual()
		{
		}
	}
}
