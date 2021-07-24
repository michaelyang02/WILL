using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

public class UnindentedJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(List<int>) ||
        objectType == typeof(int[]) ||
        objectType == typeof(List<StoryData.OutcomeIndices>) ||
        objectType == typeof(List<RearrangementPlayerData.TextboxIndices>) ||
        objectType == typeof(List<List<OutcomeCondition>>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {

        if (objectType == typeof(List<int>))
        {
            return JArray.Load(reader).ToObject<List<int>>(JsonSerializer);
        }
        else if (objectType == typeof(int[]))
        {
            return JArray.Load(reader).ToObject<int[]>(JsonSerializer);
        }
        else if (objectType == typeof(List<StoryData.OutcomeIndices>))
        {
            return JArray.Load(reader).ToObject<List<StoryData.OutcomeIndices>>(JsonSerializer);
        }
        else if (objectType == typeof(List<RearrangementPlayerData.TextboxIndices>))
        {
            return JArray.Load(reader).ToObject<List<RearrangementPlayerData.TextboxIndices>>(JsonSerializer);
        }
        else if (objectType == typeof(List<List<OutcomeCondition>>))
        {
            return JArray.Load(reader).ToObject<List<List<OutcomeCondition>>>(JsonSerializer);
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
        JsonSerializerSettings.Converters.Add(new OutcomeConditionListJsonConverter());
        JsonSerializer = JsonSerializer.Create(JsonSerializerSettings);
    }
}