using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using RocketUI.Utilities.Helpers;

namespace RocketUI.Serialization.Json.Converters
{
    public class ColorJsonConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToHexString());
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue,
            JsonSerializer                        serializer)
        {
            var color = serializer.Deserialize<string>(reader);
            return ColorHelper.HexToColor(color);
        }
    }
}