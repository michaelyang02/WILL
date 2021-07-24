using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class OutcomeConditionListJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(List<OutcomeCondition>).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        List<OutcomeCondition> outcomeConditions = OutcomeCondition.FromString((string)reader.Value);
        return outcomeConditions;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        List<OutcomeCondition> outcomeConditions = (List<OutcomeCondition>) value;
        writer.WriteValue(outcomeConditions.OutcomeConditionListString());
    }
}