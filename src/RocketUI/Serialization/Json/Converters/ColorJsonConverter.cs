using System;
using Newtonsoft.Json;
using RocketUI.Utilities.Helpers;

namespace RocketUI.Serialization.Json.Converters
{
    public class ColorJsonConverter : JsonConverter<RgbaColor>
    {
        public override void WriteJson(JsonWriter writer, RgbaColor value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToHexString());
        }

        public override RgbaColor ReadJson(JsonReader reader, Type objectType, RgbaColor existingValue, bool hasExistingValue,
            JsonSerializer                        serializer)
        {
            var color = serializer.Deserialize<string>(reader);
            return ColorHelper.HexToColor(color);
        }
    }
}