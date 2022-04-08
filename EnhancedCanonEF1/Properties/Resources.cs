using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ASCOM.CanonEF.Properties
{
	[DebuggerNonUserCode]
	[CompilerGenerated]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(resourceMan, null))
				{
					ResourceManager resourceManager = (resourceMan = new ResourceManager("ASCOM.CanonEF.Properties.Resources", typeof(Resources).Assembly));
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static Bitmap aperture2
		{
			get
			{
				object @object = ResourceManager.GetObject("aperture2", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap ASCOM
		{
			get
			{
				object @object = ResourceManager.GetObject("ASCOM", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap canonef_logo
		{
			get
			{
				object @object = ResourceManager.GetObject("canonef_logo", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Icon DefaultIcon
		{
			get
			{
				object @object = ResourceManager.GetObject("DefaultIcon", resourceCulture);
				return (Icon)@object;
			}
		}

		internal Resources()
		{
		}
	}
}
