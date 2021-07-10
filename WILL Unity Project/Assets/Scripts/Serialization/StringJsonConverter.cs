using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class StringJsonConverter : JsonConverter
{

    private static string[] types;
    private static string[] codes;

    private static string ChangeColor(string str, bool typeToCode)
    {
        // init
        if (types == null || codes == null)
        {
            types = Enum.GetNames(typeof(StoryData.HighlightType)).Select(s => s.ToLowerInvariant()).ToArray();
            codes = Enum.GetValues(typeof(StoryData.HighlightType)).Cast<StoryData.HighlightType>().Select(e => ((int)e).ToString("X")).ToArray();
        }

        StringBuilder stringBuilder = new StringBuilder(str);

        for (int i = 0; i < types.Length; i++)
        {
            stringBuilder.Replace(typeToCode ? "=" + types[i] : "=#" + codes[i], 
            !typeToCode ? "=" + types[i] : "=#" + codes[i]);
        }
        return stringBuilder.ToString();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(ChangeColor((string)value, false));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return ChangeColor((string)reader.Value, true);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(string);
    }
}