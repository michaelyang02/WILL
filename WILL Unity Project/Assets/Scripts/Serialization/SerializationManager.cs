using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Newtonsoft.Json;

public class SerializationManager
{

    public static bool Save(string saveName, object saveData)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(Path.Combine(Application.streamingAssetsPath, "Data")))
        {
            Directory.CreateDirectory(Path.Combine(Application.streamingAssetsPath, "Data"));
        }

        string path = Path.Combine(Application.streamingAssetsPath, "Data", saveName + ".data");

        using (FileStream stream = File.Create(path))
        {
            formatter.Serialize(stream, saveData);
        }
        return true;
    }

    public static T Load<T>(string saveName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Data", saveName + ".data");

        if (!File.Exists(path))
        {
            return default(T);
        }

        BinaryFormatter formatter = GetBinaryFormatter();

        try
        {
            using (FileStream stream = File.OpenRead(path))
            {

                object save = formatter.Deserialize(stream);
                return (T)save;
            }
        }
        catch
        {
            Debug.LogErrorFormat("Failed to load file at {0}", path);
            return default(T);
        }
    }

    public static bool SaveJSON(string saveName, object saveData)
    {
        string json = JsonConvert.SerializeObject(saveData, GetJsonSerializerSettings());

        if (!Directory.Exists(Path.Combine(Application.streamingAssetsPath, "Data")))
        {
            Directory.CreateDirectory(Path.Combine(Application.streamingAssetsPath, "Data"));
        }

        string path = Path.Combine(Application.streamingAssetsPath, "Data", saveName + ".json");
        
        File.WriteAllText(path, json);

        return true;
    }

    public static T LoadJSON<T>(string saveName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Data", saveName + ".json");

        if (!File.Exists(path))
        {
            return default(T);
        }

        string json = File.ReadAllText(path);

        return JsonConvert.DeserializeObject<T>(json, GetJsonSerializerSettings());
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        SurrogateSelector selector = new SurrogateSelector();

        Vector2IntSerializationSurrogate vector2IntSurrogate = new Vector2IntSerializationSurrogate();

        selector.AddSurrogate(typeof(Vector2Int), new StreamingContext(StreamingContextStates.All), vector2IntSurrogate);
        selector.AddSurrogate(typeof(StoryData.Character), new StreamingContext(StreamingContextStates.All), new EnumSerializationSurrogate());
        selector.AddSurrogate(typeof(StoryData.LineFlags), new StreamingContext(StreamingContextStates.All), new EnumSerializationSurrogate());

        formatter.SurrogateSelector = selector;

        return formatter;
    }

    public static JsonSerializerSettings GetJsonSerializerSettings()
    {
        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
        jsonSerializerSettings.Formatting = Formatting.Indented;
        jsonSerializerSettings.Converters.Add(new Vector2IntJsonConverter());
        jsonSerializerSettings.Converters.Add(new EnumConverter());
        jsonSerializerSettings.Converters.Add(new StringJsonConverter());
        jsonSerializerSettings.Converters.Add(new TextboxIndicesJsonConverter());
        jsonSerializerSettings.Converters.Add(new OutcomeConditionListJsonConverter());
        jsonSerializerSettings.Converters.Add(new OutcomeIndicesJsonConverter());
        jsonSerializerSettings.Converters.Add(new UnindentedJsonConverter());
        return jsonSerializerSettings;
    }

    public static bool Backup(string saveName, object saveData)
    {
        string json = JsonConvert.SerializeObject(saveData, GetJsonSerializerSettings());

        string backupDirectory = Path.Combine(Application.dataPath, "BackupData");

        if (!Directory.Exists(backupDirectory))
        {
            Directory.CreateDirectory(backupDirectory);
        }

        int index = 0;

        while (File.Exists(Path.Combine(backupDirectory, saveName + "_backup_" + index + ".json")))
        {
            index++;
        }

        File.WriteAllText(Path.Combine(backupDirectory, saveName + "_backup_" + index + ".json"), json);
        
        return true;
    }
}