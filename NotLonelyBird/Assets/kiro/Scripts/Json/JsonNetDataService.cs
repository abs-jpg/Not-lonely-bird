using UnityEngine;
using System.IO;
using Newtonsoft.Json;

/// <summary>
/// JSON 持久化工具类，依赖 Newtonsoft.Json (Json.NET)
/// </summary>
public static class JsonNetDataService
{
    /// <summary>
    /// 保存数据到 JSON 文件
    /// </summary>
    public static void SaveData<T>(string fileName, T data)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(path, json);
        Debug.Log($"数据已保存到: {path}");
    }

    /// <summary>
    /// 从 JSON 文件加载数据
    /// </summary>
    public static T LoadData<T>(string fileName) where T : new()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path))
        {
            Debug.Log($"文件不存在，返回默认值: {path}");
            return new T();
        }

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(json);
    }

    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    public static bool FileExists(string fileName)
    {
        return File.Exists(Path.Combine(Application.persistentDataPath, fileName));
    }
}
