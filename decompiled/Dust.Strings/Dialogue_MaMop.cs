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
	internal class Dialogue_MaMop
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Dialogue_MaMop.resourceMan, null))
				{
					ResourceManager resourceManager = (Dialogue_MaMop.resourceMan = new ResourceManager("Dust.Strings.Dialogue_MaMop", typeof(Dialogue_MaMop).Assembly));
				}
				return Dialogue_MaMop.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Dialogue_MaMop.resourceCulture;
			}
			set
			{
				Dialogue_MaMop.resourceCulture = value;
			}
		}

		internal static string _000_000 => Dialogue_MaMop.ResourceManager.GetString("000_000", Dialogue_MaMop.resourceCulture);

		internal static string _001_000 => Dialogue_MaMop.ResourceManager.GetString("001_000", Dialogue_MaMop.resourceCulture);

		internal Dialogue_MaMop()
		{
		}
	}
}
