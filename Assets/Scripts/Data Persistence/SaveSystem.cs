using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string SaveFolder => Path.Combine(Application.persistentDataPath, "saves");

    public static void Save(string fileName, GamedData data)
    {
        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);

        string path = Path.Combine(SaveFolder, fileName);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log($"SaveSystem: Saved to {path}");
    }

    public static GamedData Load(string fileName)
    {
        string path = Path.Combine(SaveFolder, fileName);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"SaveSystem: Save file not found: {path}");
            return null;
        }

        string json = File.ReadAllText(path);
        try
        {
            return JsonUtility.FromJson<GamedData>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SaveSystem: Failed to parse save file: {ex.Message}");
            return null;
        }
    }

    public static string[] ListSaves()
    {
        if (!Directory.Exists(SaveFolder)) return new string[0];
        return Directory.GetFiles(SaveFolder);
    }

    public static bool DeleteSave(string fileName)
    {
        string path = Path.Combine(SaveFolder, fileName);
        if (!File.Exists(path)) return false;
        File.Delete(path);
        return true;
    }
}