using System.Runtime.Serialization;
using UnityEngine;
using System;

public class EnumSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Enum enumValue = (Enum) obj;
        info.AddValue(enumValue.GetType().Name, enumValue.ToString());
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Enum enumValue = (Enum) obj;
        obj = Enum.Parse(enumValue.GetType(), info.GetString(enumValue.GetType().Name));
        return obj;
    }
}