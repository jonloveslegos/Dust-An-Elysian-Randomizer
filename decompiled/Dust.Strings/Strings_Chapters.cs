using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Dust.Strings
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Strings_Chapters
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Strings_Chapters.resourceMan, null))
				{
					ResourceManager resourceManager = (Strings_Chapters.resourceMan = new ResourceManager("Dust.Strings.Strings_Chapters", typeof(Strings_Chapters).Assembly));
				}
				return Strings_Chapters.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Strings_Chapters.resourceCulture;
			}
			set
			{
				Strings_Chapters.resourceCulture = value;
			}
		}

		internal static string Chapter_0 => Strings_Chapters.ResourceManager.GetString("Chapter_0", Strings_Chapters.resourceCulture);

		internal static string Chapter_1 => Strings_Chapters.ResourceManager.GetString("Chapter_1", Strings_Chapters.resourceCulture);

		internal static string Chapter_2 => Strings_Chapters.ResourceManager.GetString("Chapter_2", Strings_Chapters.resourceCulture);

		internal static string Chapter_3 => Strings_Chapters.ResourceManager.GetString("Chapter_3", Strings_Chapters.resourceCulture);

		internal static string Chapter_4 => Strings_Chapters.ResourceManager.GetString("Chapter_4", Strings_Chapters.resourceCulture);

		internal static string Chapter_5 => Strings_Chapters.ResourceManager.GetString("Chapter_5", Strings_Chapters.resourceCulture);

		internal static string Title => Strings_Chapters.ResourceManager.GetString("Title", Strings_Chapters.resourceCulture);

		internal Strings_Chapters()
		{
		}
	}
}
