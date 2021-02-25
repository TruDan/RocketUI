using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Portable.Xaml;
using Portable.Xaml.Markup;
using Portable.Xaml.Schema;

namespace RocketUI.Serialization.Xaml
{
    public class RocketXamlSchemaContext : XamlSchemaContext
    {
	    public const string RocketNamespace = "http://schema.trudan.ninja/netfx/2021/xaml/ui";

        private readonly Dictionary<Type, XamlType> _typeCache = new Dictionary<Type, XamlType>();

        public bool DesignMode { get; set; }

		private static readonly Assembly RocketAssembly = typeof(Platform).GetTypeInfo().Assembly;


		protected override XamlType GetXamlType(string xamlNamespace, string name, params XamlType[] typeArguments)
		{
			XamlType type = null;
			try
			{
				type = base.GetXamlType(xamlNamespace, name, typeArguments);
			}
			catch
			{
				if (!DesignMode || type != null)
					throw;
			}
			return type;
		}

		public override XamlType GetXamlType(Type type)
		{
			if (_typeCache.TryGetValue(type, out var xamlType))
				return xamlType;

			var info = type.GetTypeInfo();

			if (
				info.IsSubclassOf(typeof(RocketElement))
				|| info.Assembly == RocketAssembly // struct
				|| (
					   // nullable struct
					   info.IsGenericType
					   && info.GetGenericTypeDefinition() == typeof(Nullable<>)
					   && Nullable.GetUnderlyingType(type).GetTypeInfo().Assembly == RocketAssembly
				   ))
			{
				xamlType = new RocketXamlType(type, this);
				_typeCache.Add(type, xamlType);
				return xamlType;
			}

			return base.GetXamlType(type);
		}

		bool _isInResourceMember;
		PropertyInfo _resourceMember;

		internal bool IsResourceMember(PropertyInfo member)
		{
			if (member == null)
				return false;
			if (_resourceMember == null)
			{
				if (_isInResourceMember)
					return false;
				_isInResourceMember = true;
				try
				{
					_resourceMember = typeof(RocketElement).GetRuntimeProperty("Properties");
				}
				finally
				{
					_isInResourceMember = false;
				}
			}

			return member.DeclaringType == _resourceMember.DeclaringType
				   && member.Name == _resourceMember.Name;
		}

		class PropertiesXamlMember : XamlMember
		{
			public PropertiesXamlMember(PropertyInfo propertyInfo, XamlSchemaContext context)
				: base(propertyInfo, context)
			{
			}

			protected override bool LookupIsAmbient() => true;
		}

		protected override XamlMember GetProperty(PropertyInfo propertyInfo)
		{
			if (IsResourceMember(propertyInfo))
			{
				return new PropertiesXamlMember(propertyInfo, this);
			}

			return base.GetProperty(propertyInfo);
		}

		protected override XamlMember GetEvent(EventInfo eventInfo)
		{
			if (DesignMode)
			{
				// in design mode, ignore wiring up events
				return new EmptyXamlMember(eventInfo, this);
			}

			return base.GetEvent(eventInfo);
		}

    }
    
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

	class RocketXamlType : XamlType
	{
		public RocketXamlType(Type underlyingType, XamlSchemaContext schemaContext)
			: base(underlyingType, schemaContext)
		{

		}


		T GetCustomAttribute<T>(bool inherit = true)
			where T : Attribute
		{
			return UnderlyingType.GetTypeInfo().GetCustomAttribute<T>(inherit);
		}

		XamlValueConverter<TypeConverter> _typeConverter;
		bool                                 _gotTypeConverter;

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