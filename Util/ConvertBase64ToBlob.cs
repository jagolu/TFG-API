using Newtonsoft.Json;
using System;

namespace API.Util
{
    public class ConvertBase64ToBlob : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value != null) {
                string[] words = reader.Value.ToString().Split("data:image/jpeg;base64,");
                return Convert.FromBase64String(words[words.Length - 1]);
            }
            return null;
        }

        //Because we are never writing out as Base64, we don't need this. 
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(Convert.ToBase64String((byte[])value));
        }
    }
}
