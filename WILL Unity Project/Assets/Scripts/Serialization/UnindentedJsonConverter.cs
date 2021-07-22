using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class UnindentedJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(List<int>) ||
        objectType == typeof(List<StoryData.OutcomeIndices>) ||
        objectType == typeof(List<RearrangementData.TextboxIndices>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {

        if (objectType == typeof(List<int>))
        {
            return JArray.Load(reader).ToObject<List<int>>(JsonSerializer);
        }
        else if (objectType == typeof(List<StoryData.OutcomeIndices>))
        {
            return JArray.Load(reader).ToObject<List<StoryData.OutcomeIndices>>(JsonSerializer);
        }
        else if (objectType == typeof(List<RearrangementData.TextboxIndices>))
        {
            return JArray.Load(reader).ToObject<List<RearrangementData.TextboxIndices>>(JsonSerializer);
        }
        return null;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteRawValue(JsonConvert.SerializeObject(value, JsonSerializerSettings));
    }

    static JsonSerializerSettings JsonSerializerSettings;
    static JsonSerializer JsonSerializer;

    static UnindentedJsonConverter()
    {
        JsonSerializerSettings = new JsonSerializerSettings();
        JsonSerializerSettings.Formatting = Formatting.None;
        JsonSerializerSettings.Converters.Add(new TextboxIndicesJsonConverter());
        JsonSerializerSettings.Converters.Add(new OutcomeIndicesJsonConverter());
        JsonSerializer = JsonSerializer.Create(JsonSerializerSettings);
    }
}