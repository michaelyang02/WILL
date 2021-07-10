using System.Runtime.Serialization;
using UnityEngine;

public class Vector2IntSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Vector2Int v2 = (Vector2Int) obj;
        info.AddValue("x", v2.x);
        info.AddValue("y", v2.y);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Vector2Int v2 = (Vector2Int) obj;
        v2.x = info.GetInt32("x");
        v2.y = info.GetInt32("y");
        obj = v2;
        return obj;
    }
}