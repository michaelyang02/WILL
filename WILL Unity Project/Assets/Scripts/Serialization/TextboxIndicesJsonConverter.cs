using System;
using Newtonsoft.Json;

public class TextboxIndicesJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(RearrangementData.TextboxIndices);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        RearrangementData.TextboxIndices textboxIndices = new RearrangementData.TextboxIndices();
        string[] indicesString = ((string)reader.Value).Split('.');
        textboxIndices.storyIndex = int.Parse(indicesString[0]);
        textboxIndices.textboxIndex = int.Parse(indicesString[1]);
        return textboxIndices;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        RearrangementData.TextboxIndices textboxIndices = (RearrangementData.TextboxIndices) value;
        writer.WriteValue(textboxIndices.storyIndex + "." + textboxIndices.textboxIndex);
    }
}