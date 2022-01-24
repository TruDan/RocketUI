using System.IO;
using System.Text;
using System.Xml;
using Portable.Xaml;

namespace RocketUI.Serialization.Xaml
{
    public static class RocketXamlSaver
    {
        public static string SaveToXaml<T>(T instance)
        {
            var       sb        = new StringBuilder();
            using var writer    = new StringWriter(sb);
            using var xmlWriter = new XmlTextWriter(writer);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.Indentation = 2;
            SaveToXaml(instance, xmlWriter);
            return sb.ToString();
        }
		
        public static void SaveToXaml<T>(T instance, XmlWriter xmlWriter)
        {
            var       writerSettings = new XamlXmlWriterSettings();
            writerSettings.AssumeValidInput = true;
            using var xamlXmlWriter  = new XamlXmlWriter(xmlWriter, RocketXamlLoader.Context, writerSettings);
            SaveToXaml(instance, xamlXmlWriter);
        }
		
        public static void SaveToXaml<T>(T instance, XamlXmlWriter xamlXmlWriter)
        {
            // var readerSettings = new XamlObjectReaderSettings();
            // readerSettings.IgnoreDefaultValues = true;
            // readerSettings.LocalAssembly = typeof(T).Assembly;
            // readerSettings.UseIgnoreDataMemberAttribute = true;

            // using var reader = new XamlObjectReader(instance, RocketXamlLoader.Context, readerSettings);
            // XamlServices.Transform(reader, xamlXmlWriter);
            XamlServices.Save(xamlXmlWriter, instance);
        }
    }
}