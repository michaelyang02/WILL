using System;
using Newtonsoft.Json;

public class OutcomeIndicesJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(StoryData.OutcomeIndices);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        StoryData.OutcomeIndices outcomeIndices = new StoryData.OutcomeIndices();
        string[] indicesString = ((string)reader.Value).Split('.');
        outcomeIndices.storyIndex = int.Parse(indicesString[0]);
        outcomeIndices.outcomeIndex = int.Parse(indicesString[1]);
        return outcomeIndices;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        StoryData.OutcomeIndices outcomeIndices = (StoryData.OutcomeIndices) value;
        writer.WriteValue(outcomeIndices.storyIndex + "." + outcomeIndices.outcomeIndex);
    }
}