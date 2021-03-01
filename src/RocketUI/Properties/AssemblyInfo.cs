using System;
using System.Resources;
#if NETFRAMEWORK || NET472
using System.Xaml;
using System.Xaml.Markup;
#else
using Portable.Xaml;
using Portable.Xaml.Markup;
#endif

[assembly: NeutralResourcesLanguage("en-US")]
//[assembly: CLSCompliant(true)]

[assembly: XmlnsDefinition(RocketUI.Serialization.Xaml.RocketXamlSchemaContext.RocketNamespace, "RocketUI", AssemblyName = "RocketUI")]
[assembly: XmlnsDefinition(RocketUI.Serialization.Xaml.RocketXamlSchemaContext.RocketNamespace, "RocketUI")]
[assembly: XmlnsPrefix(RocketUI.Serialization.Xaml.RocketXamlSchemaContext.RocketNamespace, "")]