using System.ComponentModel;
using RocketUI.Utilities.Converters;

namespace RocketUI
{
	[TypeConverter(typeof(EnumTypeConverter<HorizontalAlignment>))]
    public enum HorizontalAlignment
	{
		None		= Alignment.None,

		Left		= Alignment.MinX,
		Center		= Alignment.CenterX,
		Right		= Alignment.MaxX,

		FillParent	= Alignment.FillX,

	}
}
