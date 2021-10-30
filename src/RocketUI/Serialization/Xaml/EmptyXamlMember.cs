using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Portable.Xaml;
using Portable.Xaml.Schema;

namespace RocketUI.Serialization.Xaml
{
    class EmptyXamlMember : XamlMember
    {
        public EmptyXamlMember(EventInfo eventInfo, XamlSchemaContext context)
            : base(eventInfo, context)
        {

        }

        class EmptyConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => true;

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) => null;
        }

        protected override XamlValueConverter<TypeConverter> LookupTypeConverter()
        {
            return new XamlValueConverter<TypeConverter>(typeof(EmptyConverter), Type);
        }
    }
}