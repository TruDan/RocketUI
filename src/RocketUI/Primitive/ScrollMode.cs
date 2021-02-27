using System.ComponentModel;
using RocketUI.Utilities.Converters;

namespace RocketUI
{
	[TypeConverter(typeof(EnumTypeConverter<ScrollMode>))]
	public enum ScrollMode
	{
		Hidden,
		Auto,
		Visible
	}
}
