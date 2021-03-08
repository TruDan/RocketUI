using System;
using Newtonsoft.Json;

namespace RocketUI.Serialization.Json.Converters
{
    public class ThicknessJsonConverter : JsonConverter<Thickness>
    {
        public override void WriteJson(JsonWriter writer, Thickness value, JsonSerializer serializer)
        {
            if ((value.Top == value.Bottom) && (value.Left == value.Right))
            {
                if (value.Top == value.Right)
                {
                    serializer.Serialize(writer, value.Top);
                }
                else
                {
                    serializer.Serialize(writer, new [] { value.Top , value.Right});
                }
            }
            else
            {
                serializer.Serialize(writer, new [] { value.Top , value.Right, value.Bottom, value.Left });
            }
        }

        public override Thickness ReadJson(JsonReader reader, Type objectType, Thickness existingValue, bool hasExistingValue,
            JsonSerializer            serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var arr = serializer.Deserialize<int[]>(reader);
                if (arr.Length == 1)
                {
                    return new Thickness(arr[0]);
                }
                else if (arr.Length == 2)
                {
                    return new Thickness(arr[0], arr[1]);
                }
                else if (arr.Length == 3)
                {
                    return new Thickness(arr[1], arr[0], arr[1], arr[2]);
                }
                else if (arr.Length == 4)
                {
                    return new Thickness(arr[3], arr[0], arr[1], arr[2]);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(reader.Path, arr.Length, "Expected array length of 2, 3 or 4 items");
                }
            }
            else if (reader.TokenType == JsonToken.Integer)
            {
                var val = reader.ReadAsInt32();
                return new Thickness(val.Value);
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}