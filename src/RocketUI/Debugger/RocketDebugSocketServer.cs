using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RocketUI.Debugger.Exceptions;
using RocketUI.Debugger.Models;
using RocketUI.Serialization.Json.Converters;
using RocketUI.Serialization.Xaml;
using WebSocketSharp;
using WebSocketSharp.Net.WebSockets;
using WebSocketSharp.Server;

namespace RocketUI.Debugger
{
    public class RocketDebugSocketServer : IRocketDebugSocket, IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;

        public class NoTypeConverterJsonConverter<T> : JsonConverter
        {
            private static readonly IContractResolver Resolver = new NoTypeConverterContractResolver();

            class NoTypeConverterContractResolver : CamelCasePropertyNamesContractResolver
            {
                protected override JsonContract CreateContract(Type objectType)
                {
                    if (typeof(T).IsAssignableFrom(objectType))
                    {
                        var contract = this.CreateObjectContract(objectType);
                        contract.Converter = null; // Also null out the converter to prevent infinite recursion.
                        return contract;
                    }

                    return base.CreateContract(objectType);
                }
            }

            public override bool CanConvert(Type objectType)
            {
                return typeof(T).IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer                         serializer)
            {
                return JsonSerializer.CreateDefault(new JsonSerializerSettings()
                {
                    ContractResolver = Resolver
                }).Deserialize(reader, objectType);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                JsonSerializer.CreateDefault(new JsonSerializerSettings()
                {
                    ContractResolver = Resolver
                }).Serialize(writer, value);
            }
        }

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            Culture = CultureInfo.InvariantCulture,
            MaxDepth = 64,
            NullValueHandling = NullValueHandling.Include,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>()
            {
                //new StringEnumConverter(),
                new JavaScriptDateTimeConverter(),
                new SanitizedElementDetailPropertyJsonConverter(),
                new ColorJsonConverter(),
                new NoTypeConverterJsonConverter<Rectangle>(),
                new NoTypeConverterJsonConverter<Size>(),
                new NoTypeConverterJsonConverter<Vector2>(),
                new NoTypeConverterJsonConverter<Vector3>(),
                new NoTypeConverterJsonConverter<Thickness>(),
                new NoTypeConverterJsonConverter<Point>(),
            }
        };

        private static readonly JsonSerializerSettings DeserializerSettings = new JsonSerializerSettings()
        {
            Culture = CultureInfo.InvariantCulture,
//            MaxDepth = 128, // no max depth pls3
            NullValueHandling = NullValueHandling.Include,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>()
            {
                new StringEnumConverter(),
                new JavaScriptDateTimeConverter(),
                new SanitizedElementDetailPropertyJsonConverter(),
                new ColorJsonConverter(),
                new NoTypeConverterJsonConverter<Rectangle>(),
                new NoTypeConverterJsonConverter<Size>(),
                new NoTypeConverterJsonConverter<Vector2>(),
                new NoTypeConverterJsonConverter<Vector3>(),
                new NoTypeConverterJsonConverter<Thickness>(),
                new NoTypeConverterJsonConverter<Point>(),
            }
        };

        private GuiManager GuiManager => _serviceProvider.GetRequiredService<GuiManager>();

        private readonly WebSocketServer _webSocket;

        public RocketDebugSocketServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _webSocket = new WebSocketServer("ws://localhost:2345");
            _webSocket.AddWebSocketService<RocketDebugWebsocketService>("/",
                () => new RocketDebugWebsocketService(this));
        }

        internal object HandleMessage(Message msg)
        {
            switch (msg.Command)
            {
                case "GetRoot":
                {
                    var screens = GuiManager.Screens.ToArray();
                    return screens.Select(root => new SanitizedElementDetail(root)).ToArray();
                }

                case "GetChildren":
                {
                    var elementId = Guid.Parse(msg.Arguments[0]);
                    var element   = FindElementById(elementId);
                    return element.ChildElements.Select(x => new SanitizedElementDetail(x));
                }

                case "GetProperties":
                {
                    var elementId = Guid.Parse(msg.Arguments[0]);
                    var element   = FindElementById(elementId);
                    return new SanitizedElementDetail(element).Properties;
                }

                case "SelectElement":
                {
                    var elementId = Guid.Parse(msg.Arguments[0]);
                    var element   = FindElementById(elementId);
                    GuiManager.DebugHelper.Enabled = true;
                    GuiManager.DebugHelper.HighlightedElement = element;
                    break;
                }

                case "SetPropertyValue":
                {
                    var elementId = Guid.Parse(msg.Arguments[0]);
                    var element   = FindElementById(elementId);
                    var propertyInfo = element.GetType()
                        .GetProperty(msg.Arguments[1], BindingFlags.Instance | BindingFlags.Public);
                    if (propertyInfo == null)
                        throw new PropertyNotFoundException(elementId, msg.Arguments[1]);

                    if (!propertyInfo.CanWrite)
                        throw new PropertyNotEditableException(elementId, msg.Arguments[1]);
                    
                    var value = TypeDescriptor.GetConverter(propertyInfo.PropertyType)
                        .ConvertFromString(msg.Arguments[2]);
                    
                    propertyInfo.SetValue(element, value);

                    return true;
                }

                case "GetObjectAsXaml":
                {
                    var elementId = Guid.Parse(msg.Arguments[0]);
                    var element   = FindElementById(elementId);
                    var xaml      = RocketXamlSaver.SaveToXaml(element);
                    return xaml;
                }
            }

            return null;
        }

        //private void Send(WebSocketContext target, Response response)
        // {
        //     target.WebSocket.Send(JsonSerialize(response));
        // }

        private RocketElement FindElementById(Guid id)
        {
            foreach (var screen in GuiManager.Screens.ToArray())
            {
                if (screen.Id == id) return screen;
                
                if (screen.TryFindDeepestChild(guiElement => guiElement.Id == id, out var element))
                {
                    return element as RocketElement;
                }
            }

            throw new ElementNotFoundException(id);
        }

        internal static string JsonSerialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, SerializerSettings);
        }

        internal static T JsonDeserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, DeserializerSettings);
        }

        public void Dispose()
        {
            if(_webSocket is IDisposable disposableWebSocket)
                disposableWebSocket?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _webSocket.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _webSocket.Stop();
            return Task.CompletedTask;
        }
    }

    public class RocketDebugWebsocketService : WebSocketBehavior
    {
        private RocketDebugSocketServer _server;

        public RocketDebugWebsocketService(RocketDebugSocketServer server) : base()
        {
            _server = server;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                var msg = RocketDebugSocketServer.JsonDeserialize<Message>(e.Data);
                try
                {
                    var response = _server.HandleMessage(msg);
                    var json = RocketDebugSocketServer.JsonSerialize(new Response(msg.Id, response));
                    Send(json);
                }
                catch (Exception ex)
                {
                    Send(RocketDebugSocketServer.JsonSerialize(new ErrorResponse(msg.Id, ex)));
                    Console.WriteLine("Exception while handling websocket message: {0}", ex.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while parsing websocket message: {0}", ex.ToString());
            }
        }
    }

    class SanitizedElementDetail
    {
        public Guid   Id      { get; set; }
        public string Name    { get; set; }
        public string Type    { get; set; }
        public string Display { get; set; }

        [JsonConverter(typeof(SanitizedElementDetailPropertyJsonConverter))]
        public SanitizedElementDetailProperty[] Properties { get; set; }

        [JsonConverter(typeof(SanitizedElementDetailPropertyTypeJsonConverter))]
        public SanitizedElementDetailPropertyType[] Schema { get; set; }

        public List<SanitizedElementDetail> Children { get; set; }

        public SanitizedElementDetail()
        {
        }

        public SanitizedElementDetail(IGuiElement element)
        {
            Id = element.Id;
            Name = element.Name;
            Type = element.GetType().Name;
            Display = $"<{Type}>";
            GetProperties(element, out var propertyValues, out var propertyTypes);
            Properties = propertyValues;
            Schema = propertyTypes;


            Children = GetChildren(element).ToList();
        }

        private static IEnumerable<SanitizedElementDetail> GetChildren(IGuiElement element)
        {
            foreach (var child in element.ChildElements)
            {
                yield return new SanitizedElementDetail(child);
            }
        }

        private static void GetProperties(IGuiElement element,
            out SanitizedElementDetailProperty[]      propertyValues,
            out SanitizedElementDetailPropertyType[]  propertyTypes)
        {
            element.Properties.Initialize();

            var elementType = element.GetType();
            var props       = element.Properties.ToArray();
            propertyValues = new SanitizedElementDetailProperty[props.Length];
            propertyTypes = new SanitizedElementDetailPropertyType[props.Length];


            for (var i = 0; i < props.Length; i++)
            {
                var prop = props[i];

                propertyTypes[i] = new SanitizedElementDetailPropertyType(elementType.GetProperty(prop.Key.ToString()));
                propertyValues[i] = new SanitizedElementDetailProperty(prop.Key.ToString(), prop.Value);
            }
        }
    }

    [JsonConverter(typeof(SanitizedElementDetailPropertyJsonConverter))]
    class SanitizedElementDetailProperty
    {
        public string Key   { get; set; }
        public object Value { get; set; }

        public SanitizedElementDetailProperty()
        {
        }

        public SanitizedElementDetailProperty(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }

    class SanitizedElementDetailPropertyJsonConverter : JsonConverter<IEnumerable<SanitizedElementDetailProperty>>
    {
        public override void WriteJson(JsonWriter writer, IEnumerable<SanitizedElementDetailProperty> list,
            JsonSerializer                        serializer)
        {
            writer.WriteStartObject();

            if (list != null)
            {
                foreach (var value in list)
                {
                    writer.WritePropertyName(value.Key, true);
                    serializer.Serialize(writer, value.Value);
                }
            }

            writer.WriteEndObject();
        }

        public override IEnumerable<SanitizedElementDetailProperty> ReadJson(JsonReader reader, Type objectType,
            IEnumerable<SanitizedElementDetailProperty> existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    [JsonConverter(typeof(SanitizedElementDetailPropertyTypeJsonConverter))]
    class SanitizedElementDetailPropertyType
    {
        public string Key { get; set; }

        public bool CanEdit   { get; set; }
        public bool IsFlags   { get; set; }
        public Type ValueType { get; set; }

        public KeyValuePair<string, object>[] Values { get; set; } = Array.Empty<KeyValuePair<string, object>>();

        public SanitizedElementDetailPropertyType()
        {
        }

        public SanitizedElementDetailPropertyType(PropertyInfo propertyInfo)
        {
            Key = propertyInfo.Name;
            CanEdit = propertyInfo.CanWrite;
            ValueType = propertyInfo.PropertyType;

            if (ValueType.IsEnum)
            {
                IsFlags = ValueType.GetCustomAttribute<FlagsAttribute>() != null;
                PopulateEnumValues();
            }
        }

        private void PopulateEnumValues()
        {
            var values = Enum.GetValues(ValueType);
            Values = new KeyValuePair<string, object>[values.Length];

            for (var i = 0; i < values.Length; i++)
            {
                var value = values.GetValue(i);
                var name  = Enum.GetName(ValueType, value);
                Values[i] = new KeyValuePair<string, object>(name, value);
            }
        }
    }

    class SanitizedElementDetailPropertyTypeJsonConverter : JsonConverter<
        IEnumerable<SanitizedElementDetailPropertyType>>
    {
        public override void WriteJson(JsonWriter writer, IEnumerable<SanitizedElementDetailPropertyType> list,
            JsonSerializer                        serializer)
        {
            writer.WriteStartObject();

            if (list != null)
            {
                foreach (var value in list)
                {
                    writer.WritePropertyName(value.Key, true);

                    writer.WriteStartObject();

                    writer.WritePropertyName("type");
                    writer.WriteValue(value.ValueType.FullName);

                    writer.WritePropertyName("editable");
                    writer.WriteValue(value.CanEdit);

                    if (value.IsFlags)
                    {
                        writer.WritePropertyName("isFlags");
                        writer.WriteValue(true);
                    }

                    if (value.Values.Any())
                    {
                        writer.WritePropertyName("enumValues");
                        serializer.Serialize(writer, value.Values);
                    }

                    writer.WriteEndObject();
                }
            }

            writer.WriteEndObject();
        }

        public override IEnumerable<SanitizedElementDetailPropertyType> ReadJson(JsonReader reader, Type objectType,
            IEnumerable<SanitizedElementDetailPropertyType> existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}