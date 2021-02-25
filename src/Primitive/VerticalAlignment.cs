using System.ComponentModel;
using RocketUI.Utilities.Converters;

namespace RocketUI
{
	[TypeConverter(typeof(EnumTypeConverter<VerticalAlignment>))]
	public enum VerticalAlignment
	{
		None		= Alignment.NoneY,
		Top			= Alignment.MinY,
		Center		= Alignment.CenterY,
		Bottom		= Alignment.MaxY,
		FillParent	= Alignment.FillY,
	}
}
