using UnityEngine;
using Newtonsoft.Json;
using System;

public class Vector2IntJsonConverter : JsonConverter
{

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Vector2Int v2 = (Vector2Int)value;
        writer.WriteStartArray();
        writer.WriteValue(v2.x);
        writer.WriteValue(v2.y);
        writer.WriteEnd();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        Vector2Int v2 = new Vector2Int();
        v2.x = (int)reader.ReadAsInt32();
        v2.y = (int)reader.ReadAsInt32();
        reader.Read();
        return v2;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector2Int);
    }
}