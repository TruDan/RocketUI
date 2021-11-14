using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Portable.Xaml;

namespace RocketUI.Serialization.Xaml
{

	public static class RocketXamlLoader
	{

		public static readonly string RocketNamespace = RocketXamlSchemaContext.RocketNamespace;


		private static Stream GetStream(Type type)
		{
			return GetStream(type, type.FullName)
			       ?? GetStream(type, type.FullName + ".xaml")
				   ?? GetStream(type, type.Name + ".xaml")
				   ?? throw new InvalidOperationException($"Embedded resource '{type.FullName}.xaml' not found in assembly '{type.Assembly}'");
		}

		private static Stream GetStream(string path)
		{
			if (File.Exists(path))
				return new FileStream(path, FileMode.Open, FileAccess.Read);
			
			throw new InvalidOperationException($"File resource '{path}' not found");
		}

		private static Stream GetStream(Type type, string resourceName)
		{
			Assembly searchAssembly = type.Assembly;
			
			if (!type.Assembly.GetManifestResourceNames().Contains(resourceName))
			{
				searchAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x =>
				{
					try
					{
						var names = x.GetManifestResourceNames();
						return names.Contains(resourceName);
					}
					catch
					{
						return false;
					}
				});
			}

			if (searchAssembly == null) return null;
			return searchAssembly.GetManifestResourceStream(resourceName);
		}

		/// <summary>
		/// Loads the specified type from a xaml of the same name
		/// </summary>
		/// <remarks>
		/// If your class name is MyNamespace.MyType, then this will attempt to load MyNamespace.MyType.xaml
		/// for the xaml definition in the same assembly.
		/// 
		/// If you want to specify a different xaml, use <see cref="Load{T}(Stream)"/>
		/// </remarks>
		/// <typeparam name="T">Type of object to load from xaml</typeparam>
		/// <returns>A new instance of the specified type with the contents loaded from xaml</returns>
		public static T Load<T>()
			where T : new()
		{
			using (var stream = GetStream(typeof(T)))
			{
				return Load<T>(stream, default(T));
			}
		}
		/// <summary>
		/// Loads the specified type from the specified xaml stream
		/// </summary>
		/// <remarks>
		/// If your class name is MyNamespace.MyType, then this will attempt to load MyNamespace.MyType.xaml
		/// for the xaml definition in the same assembly.
		/// </remarks>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="stream">Xaml content to load (e.g. from resources)</param>
		/// <returns>A new instance of the specified type with the contents loaded from the xaml stream</returns>
		public static T Load<T>(Stream stream)
			where T : new()
		{
			return Load<T>(stream, default(T));
		}
		/// <summary>
		/// Loads the specified instance with xaml of the same name
		/// </summary>
		/// <remarks>
		/// If your class name is MyNamespace.MyType, then this will attempt to load MyNamespace.MyType.xaml
		/// for the xaml definition in the same assembly.
		/// 
		/// If you want to specify a different xaml, use <see cref="Load{T}(Stream, T)"/>
		/// </remarks>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="instance">Instance to use as the starting object</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static void Load<T>(T instance)
		{
			if (Equals(instance, null))
				throw new ArgumentNullException(nameof(instance));

			using (var stream = GetStream(typeof(T)))
			{
				Load<T>(stream, instance);
			}
		}

		/// <summary>
		/// Loads the specified instance with a specified fully qualified xaml embedded resource
		/// </summary>
		/// <remarks>
		/// This will load the embedded resource from the same assembly as <paramref name="instance"/> with the 
		/// specified <paramref name="resourceName"/> embedded resource.
		/// 
		/// If you want to specify a different xaml, use <see cref="Load{T}(Stream, T)"/>
		/// </remarks>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="instance">Instance to use as the starting object</param>
		/// <param name="resourceName">Fully qualified name of the embedded resource to load.</param>
		/// <returns>An existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static void Load<T>(T instance, string resourceName)
		{
			if (Equals(instance, null))
				throw new ArgumentNullException(nameof(instance));

			using (var stream = GetStream(typeof(T), resourceName))
			{
				if (stream == null)
					throw new ArgumentException(nameof(resourceName), $"Embedded resource '{resourceName}' not found in assembly '{typeof(T).Assembly}'");

				Load<T>(stream, instance);
			}
		}
		/// <summary>
		/// Loads the specified instance with a specified fully qualified xaml embedded resource
		/// </summary>
		/// <remarks>
		/// This will load the embedded resource from the same assembly as <paramref name="instance"/> with the 
		/// specified <paramref name="resourceName"/> embedded resource.
		/// 
		/// If you want to specify a different xaml, use <see cref="Load{T}(Stream, T)"/>
		/// </remarks>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="instance">Instance to use as the starting object</param>
		/// <param name="resourceName">Fully qualified name of the embedded resource to load.</param>
		/// <returns>An existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static void LoadFromFile<T>(T instance, string filePath)
		{
			if (Equals(instance, null))
				throw new ArgumentNullException(nameof(instance));

			if (!Path.GetExtension(filePath).Equals("xaml", StringComparison.InvariantCultureIgnoreCase))
			{
				filePath = Path.ChangeExtension(filePath, "xaml");
			}

			if (!Path.IsPathFullyQualified(filePath))
			{
				filePath = Path.GetFullPath(filePath);
			}
			
			using (var stream = GetStream(filePath))
			{
				if (stream == null)
					throw new InvalidOperationException($"File resource '{filePath}' not found");

				Load<T>(stream, instance);
			}
		}

		internal static readonly RocketXamlSchemaContext Context = new RocketXamlSchemaContext();

		/// <summary>
		/// Gets or sets a value indicating that the reader is used in design mode
		/// </summary>
		/// <remarks>
		/// In Design mode, events are not wired up and will not cause exceptions due to missing methods to wire up to.
		/// </remarks>
		public static bool DesignMode
		{
			get { return Context.DesignMode; }
			set { Context.DesignMode = value; }
		}

		/// <summary>
		/// Loads the specified type from the specified xaml stream
		/// </summary>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="stream">Xaml content to load (e.g. from resources)</param>
		/// <param name="instance">Instance to use as the starting object, or null to create a new instance</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static T Load<T>(Stream stream, T instance)
		{
			var readerSettings = new XamlXmlReaderSettings();
			if (!DesignMode)
				readerSettings.LocalAssembly = typeof(T).Assembly;
			using var xamlXmlReader = new XamlXmlReader(stream, Context, readerSettings);
			return Load<T>(xamlXmlReader, instance);
		}

		/// <summary>
		/// Loads the specified type from the specified text <paramref name="reader"/>.
		/// </summary>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="reader">Reader to read the Xaml content</param>
		/// <param name="instance">Instance to use as the starting object, or null to create a new instance</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static T Load<T>(TextReader reader, T instance)
		{
			var readerSettings = new XamlXmlReaderSettings();
			if (!DesignMode)
				readerSettings.LocalAssembly = typeof(T).Assembly;
			using var xamlXmlReader = new XamlXmlReader(reader, Context, readerSettings);
			return Load<T>(xamlXmlReader, instance);
		}

		/// <summary>
		/// Loads the specified type from the specified XML <paramref name="reader"/>.
		/// </summary>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="reader">XmlReader to read the Xaml content</param>
		/// <param name="instance">Instance to use as the starting object, or null to create a new instance</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static T Load<T>(XmlReader reader, T instance)
		{
			var readerSettings = new XamlXmlReaderSettings();
			if (!DesignMode)
				readerSettings.LocalAssembly = typeof(T).Assembly;
			using var xamlXmlReader = new XamlXmlReader(reader, Context, readerSettings);
			return Load<T>(xamlXmlReader, instance);
		}

		static T Load<T>(XamlXmlReader reader, T instance)
		{
			var writerSettings = new XamlObjectWriterSettings();
			writerSettings.ExternalNameScope = new RocketNameScope { Instance = instance };
			writerSettings.RegisterNamesOnExternalNamescope = true;
			writerSettings.RootObjectInstance = instance;
			using var writer = new XamlObjectWriter(Context, writerSettings);

			XamlServices.Transform(reader, writer);
			return (T)writer.Result;
		}
		
		public static TBaseInstance Load<TBaseInstance, TXamlType>(TBaseInstance instance)
		{
			return Load<TBaseInstance>(GetStream(typeof(TXamlType)), instance);
		}
	}
}