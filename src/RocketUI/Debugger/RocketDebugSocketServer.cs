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
using RocketUI.Debugger.Models;
using RocketUI.Serialization.Json.Converters;
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
            static readonly IContractResolver resolver = new NoTypeConverterContractResolver(); 
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

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return JsonSerializer.CreateDefault(new JsonSerializerSettings()
                {
                    ContractResolver = resolver
                }).Deserialize(reader, objectType);
                
                var prevResolver = serializer.ContractResolver;
                try
                {
                    serializer.ContractResolver = resolver;
                    return serializer.Deserialize(reader, objectType);
                }
                finally
                {
                    serializer.ContractResolver = prevResolver;
                }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                JsonSerializer.CreateDefault(new JsonSerializerSettings()
                {
                    ContractResolver = resolver
                }).Serialize(writer, value);
                return;
                var prevResolver = serializer.ContractResolver;
                try
                {
                    serializer.ContractResolver = resolver;
                    serializer.Serialize(writer, value);
                }
                finally
                {
                    serializer.ContractResolver = prevResolver;
                }
            }
        }
        
        private static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            Culture = CultureInfo.InvariantCulture,
            MaxDepth = 64,
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

        private static JsonSerializerSettings _deserializerSettings = new JsonSerializerSettings()
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

        private GuiManager _guiManager => _serviceProvider.GetRequiredService<GuiManager>();

        private WebSocketServer _webSocket;

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
                    var screens = _guiManager.Screens.ToArray();
                    return screens.Select(root => new SanitizedElementDetail(root)).ToArray();
                }
                    break;

                case "GetChildren":
                {
                    var elementId = Guid.Parse(msg.Arguments[0]);
                    var element   = FindElementById(elementId);
                    return element.ChildElements.Select(x => new SanitizedElementDetail(x));
                }
                    break;

                case "GetProperties":
                {
                    var elementId = Guid.Parse(msg.Arguments[0]);
                    var element   = FindElementById(elementId);
                    return new SanitizedElementDetail(element).Properties;
                }
                    break;

                case "SelectElement":
                {
                    var elementId = Guid.Parse(msg.Arguments[0]);
                    var element   = FindElementById(elementId);
                    _guiManager.DebugHelper.Enabled = true;
                    _guiManager.DebugHelper.HighlightedElement = element;
                    break;
                }

                case "SetPropertyValue":
                {
                    var elementId = Guid.Parse(msg.Arguments[0]);
                    var element   = FindElementById(elementId);
                    var t         = element.GetType().GetMember(msg.Arguments[1], BindingFlags.Instance | BindingFlags.Public);
                    if (t.Length == 0)
                        return false;

                    foreach (var member in t)
                    {
                        if (member is PropertyInfo propertyInfo)
                        {
                            var value = TypeDescriptor.GetConverter(propertyInfo.PropertyType).ConvertFromString(msg.Arguments[2]);
                            propertyInfo.SetValue(element, value);
                            return true;
                        }
                        else if (member is FieldInfo fieldInfo)
                        {
                            var value = TypeDescriptor.GetConverter(fieldInfo.FieldType)
                                .ConvertToString(msg.Arguments[2]);
                            fieldInfo.SetValue(element, value);
                            return true;
                        }
                    }

                    return false;
                    
                    break;
                }
                
                default:

                    break;
            }

            return null;
        }

        //private void Send(WebSocketContext target, Response response)
        // {
        //     target.WebSocket.Send(JsonSerialize(response));
        // }

        private RocketElement FindElementById(Guid id)
        {
            foreach (var screens in _guiManager.Screens.ToArray())
            {
                if (screens.TryFindDeepestChild(guiElement => guiElement.Id == id, out var element))
                {
                    return element as RocketElement;
                }
            }

            return null;
        }

        internal static string JsonSerialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, _serializerSettings);
        }

        internal static T JsonDeserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, _deserializerSettings);
        }

        public void Dispose()
        {
            ((IDisposable) _webSocket)?.Dispose();
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
                    var json = RocketDebugSocketServer.JsonSerialize(new Response()
                    {
                        Id = msg.Id,
                        Data = response
                    });
                    Send(json);
                }
                catch (Exception ex)
                {
                    Send(RocketDebugSocketServer.JsonSerialize(new Response()
                    {
                        Id = msg.Id,
                        Data = ex.Message
                    }));
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
        public List<SanitizedElementDetailProperty> Properties { get; set; }

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
            Properties = GetProperties(element).ToList();
            Children = GetChildren(element).ToList();
        }

        private static IEnumerable<SanitizedElementDetail> GetChildren(IGuiElement element)
        {
            foreach (var child in element.ChildElements)
            {
                yield return new SanitizedElementDetail(child);
            }
        }

        private static IEnumerable<SanitizedElementDetailProperty> GetProperties(IGuiElement element)
        {
            element.Properties.Initialize();
            foreach (var prop in element.Properties.ToArray())
            {
                yield return new SanitizedElementDetailProperty(prop.Key.ToString(), prop.Value);
            }
        }
    }

    [JsonConverter(typeof(SanitizedElementDetailPropertyJsonConverter))]
    class SanitizedElementDetailProperty
    {
        public string Key       { get; set; }
        public object Value     { get; set; }
        public Type   ValueType { get; set; }

        public SanitizedElementDetailProperty()
        {
        }

        public SanitizedElementDetailProperty(string key, object value)
        {
            Key = key;
            Value = value;
            ValueType = value?.GetType();
        }
    }

    class SanitizedElementDetailPropertyJsonConverter : JsonConverter<IEnumerable<SanitizedElementDetailProperty>>
    {
        public override void WriteJson(JsonWriter writer, IEnumerable<SanitizedElementDetailProperty>? list,
            JsonSerializer                        serializer)
        {
            if (list == null)
            {
                serializer.Serialize(writer, new List<SanitizedElementDetailProperty>());
                return;
            }

            writer.WriteStartObject();

            foreach (var value in list)
            {
                writer.WritePropertyName(value.Key, true);

                writer.WriteStartObject();

                writer.WritePropertyName("value");
                serializer.Serialize(writer, value.Value);

                writer.WritePropertyName("type");
                writer.WriteValue(value.ValueType.FullName);

                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }

        public override IEnumerable<SanitizedElementDetailProperty>? ReadJson(JsonReader reader, Type objectType,
            IEnumerable<SanitizedElementDetailProperty>? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}