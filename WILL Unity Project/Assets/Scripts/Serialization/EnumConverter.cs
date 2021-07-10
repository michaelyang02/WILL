using Newtonsoft.Json;
using System;

public class EnumConverter<T> : JsonConverter where T : Enum
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        T enumValue = (T)value;
        writer.WriteValue(enumValue.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return Enum.Parse(typeof(T), (string)reader.Value);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(T);
    }

}