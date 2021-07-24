using System;
using Newtonsoft.Json;

public class TextboxIndicesJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(RearrangementPlayerData.TextboxIndices);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        RearrangementPlayerData.TextboxIndices textboxIndices = new RearrangementPlayerData.TextboxIndices();
        string[] indicesString = ((string)reader.Value).Split('.');
        textboxIndices.storyIndex = int.Parse(indicesString[0]);
        textboxIndices.textboxIndex = int.Parse(indicesString[1]);
        return textboxIndices;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        RearrangementPlayerData.TextboxIndices textboxIndices = (RearrangementPlayerData.TextboxIndices) value;
        writer.WriteValue(textboxIndices.storyIndex + "." + textboxIndices.textboxIndex);
    }
}