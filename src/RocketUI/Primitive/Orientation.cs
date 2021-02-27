using System.ComponentModel;
using RocketUI.Utilities.Converters;

namespace RocketUI
{
	[TypeConverter(typeof(EnumTypeConverter<Orientation>))]
	public enum Orientation
	{
		Vertical,
		Horizontal
	}
}
