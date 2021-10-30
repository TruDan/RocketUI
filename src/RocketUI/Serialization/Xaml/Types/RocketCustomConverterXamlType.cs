using System;
using System.Collections.Generic;
using System.ComponentModel;
using Portable.Xaml;
using Portable.Xaml.Schema;

namespace RocketUI.Serialization.Xaml.Types
{
    public class RocketCustomConverterXamlType : RocketXamlType
    {
        private readonly Type _typeConverterType;

        public RocketCustomConverterXamlType(Type type, Type converterType, XamlSchemaContext schemaContext) : base(
            type, schemaContext)
        {
            _typeConverterType = converterType;
        }

        public RocketCustomConverterXamlType(Type type, Type converterType, XamlSchemaContext schemaContext,
            XamlTypeInvoker                       invoker) : base(type, schemaContext, invoker)
        {
            _typeConverterType = converterType;
        }

        protected override XamlValueConverter<TypeConverter> LookupTypeConverter()
        {
            var t = new XamlValueConverter<TypeConverter>(_typeConverterType, this);
            return t;
        }
    }

    public class RocketCustomConverterXamlType<TType, TConverter> : RocketCustomConverterXamlType
        where TConverter : TypeConverter
    {
        public RocketCustomConverterXamlType(XamlSchemaContext schemaContext) : base(typeof(TType), typeof(TConverter),
            schemaContext)
        {
        }

        public RocketCustomConverterXamlType(XamlSchemaContext schemaContext, XamlTypeInvoker invoker) : base(
            typeof(TType), typeof(TConverter), schemaContext, invoker)
        {
        }
    }
}