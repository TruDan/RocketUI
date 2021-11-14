using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Portable.Xaml;
using RocketUI.Serialization.Xaml.Types;
using RocketUI.Utilities.Converters;

namespace RocketUI.Serialization.Xaml
{
    public class RocketXamlSchemaContext : XamlSchemaContext
    {
	    public const string RocketNamespace = "http://schema.trudan.ninja/netfx/2021/xaml/ui";

        private readonly Dictionary<Type, XamlType> _typeCache = new Dictionary<Type, XamlType>();

        public bool DesignMode { get; set; }

		private static readonly Assembly RocketAssembly = typeof(RocketUI.Platform).GetTypeInfo().Assembly;

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

			if (info.IsAssignableFrom(typeof(Color)))
			{
				xamlType = new RocketCustomConverterXamlType(type, typeof(ColorTypeConverter), this);
				_typeCache.Add(type, xamlType);
				return xamlType;
			}
			else if (info.IsAssignableFrom(typeof(GuiTexture2D)))
			{
				xamlType = new RocketCustomConverterXamlType(type, typeof(GuiTexture2DTypeConverter), this);
				_typeCache.Add(type, xamlType);
				return xamlType;
			}
			else if (info.IsAssignableFrom(typeof(Size)))
			{
				xamlType = new RocketCustomConverterXamlType(type, typeof(SizeConverter), this);
				_typeCache.Add(type, xamlType);
				return xamlType;
			}
			else if (info.IsAssignableFrom(typeof(Thickness)))
			{
				xamlType = new RocketCustomConverterXamlType(type, typeof(ThicknessConverter), this);
				_typeCache.Add(type, xamlType);
				return xamlType;
			}

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
}