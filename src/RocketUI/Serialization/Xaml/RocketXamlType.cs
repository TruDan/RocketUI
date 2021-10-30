using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Portable.Xaml;
using Portable.Xaml.Markup;
using Portable.Xaml.Schema;

namespace RocketUI.Serialization.Xaml
{
    public class RocketXamlType : XamlType
    {
        public RocketXamlType(Type underlyingType, XamlSchemaContext schemaContext)
            : base(underlyingType, schemaContext)
        {

        }

        public RocketXamlType(Type underlyingType, XamlSchemaContext schemaContext, XamlTypeInvoker invoker) : base(underlyingType, schemaContext, invoker)
        {
        }

        public RocketXamlType(string unknownTypeNamespace, string unknownTypeName, IList<XamlType> typeArguments, XamlSchemaContext schemaContext) : base(unknownTypeNamespace, unknownTypeName, typeArguments, schemaContext)
        {
        }

        protected RocketXamlType(string typeName, IList<XamlType> typeArguments, XamlSchemaContext schemaContext) : base(typeName, typeArguments, schemaContext)
        {
        }


        T GetCustomAttribute<T>(bool inherit = true)
            where T : Attribute
        {
            return UnderlyingType.GetTypeInfo().GetCustomAttribute<T>(inherit);
        }

        XamlValueConverter<TypeConverter> _typeConverter;
        bool                              _gotTypeConverter;

        protected override XamlValueConverter<TypeConverter> LookupTypeConverter()
        {
            if (_gotTypeConverter)
                return _typeConverter;

            _gotTypeConverter = true;

            // convert from Eto.TypeConverter to Portable.Xaml.ComponentModel.TypeConverter
            var typeConverterAttrib = GetCustomAttribute<TypeConverterAttribute>();

            if (typeConverterAttrib == null
                && UnderlyingType.GetTypeInfo().IsGenericType
                && UnderlyingType.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                typeConverterAttrib = Nullable.GetUnderlyingType(UnderlyingType).GetTypeInfo().GetCustomAttribute<TypeConverterAttribute>();
            }

            //if (typeConverterAttrib != null)
            //{
            //	var converterType = Type.GetType(typeConverterAttrib.ConverterTypeName);
            //	if (converterType != null)
            //		typeConverter = new ValueConverter(converterType, this);
            //}
            if (typeof(MulticastDelegate).GetTypeInfo().IsAssignableFrom(UnderlyingType.GetTypeInfo()))
            {
                var context = SchemaContext as RocketXamlSchemaContext;
                if (context.DesignMode)
                {
                    return null;
                }
            }

            if (_typeConverter == null)
                _typeConverter = base.LookupTypeConverter();
            return _typeConverter;
        }

        protected override XamlValueConverter<ValueSerializer> LookupValueSerializer()
        {
            return base.LookupValueSerializer();
        }

        protected override bool LookupIsAmbient()
        {
            if (UnderlyingType != null && UnderlyingType == typeof(PropertyStore))
                return true;
            return base.LookupIsAmbient();
        }

        bool       _gotContentProperty;
        XamlMember _contentProperty;

        protected override XamlMember LookupContentProperty()
        {
            if (_gotContentProperty)
                return _contentProperty;
            _gotContentProperty = true;
            var pXamlcontentAttribute = GetCustomAttribute<ContentPropertyAttribute>();
            if (pXamlcontentAttribute == null || pXamlcontentAttribute.Name == null)
                _contentProperty = base.LookupContentProperty();
            else
                _contentProperty = GetMember(pXamlcontentAttribute.Name);
            return _contentProperty;
        }

        XamlMember _nameAliasedProperty;

        protected override XamlMember LookupAliasedProperty(XamlDirective directive)
        {
            if (directive == XamlLanguage.Name)
            {
                if (_nameAliasedProperty != null)
                    return _nameAliasedProperty;

                var nameAttribute = GetCustomAttribute<RuntimeNamePropertyAttribute>();
                if (nameAttribute != null && nameAttribute.Name != null)
                {
                    _nameAliasedProperty = GetMember(nameAttribute.Name);
                    return _nameAliasedProperty;
                }

            }
            return base.LookupAliasedProperty(directive);
        }
		
    }
}