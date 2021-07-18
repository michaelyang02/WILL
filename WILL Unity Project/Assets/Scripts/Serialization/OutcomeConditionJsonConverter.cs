using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class OutcomeConditionJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(OutcomeCondition).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        OutcomeCondition outcomeCondition = OutcomeCondition.FromString((string)reader.Value);
        return outcomeCondition;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        OutcomeCondition outcomeCondition = (OutcomeCondition) value;
        writer.WriteValue(outcomeCondition.ToString());
    }
}