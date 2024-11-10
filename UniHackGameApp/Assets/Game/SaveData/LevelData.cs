using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SFB; // StandaloneFileBrowser namespace

[Serializable]
public class LevelData
{
    public List<TileData> tiles;
    public PlayerData player;

    private static readonly string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CheesyProgramming");

    // Load level data from a JSON file in the AppData directory
    public static LevelData LoadFromFile(string fileName = "LevelData.json")
    {
        string path = Path.Combine(appDataPath, fileName);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"File not found at path: {path}");
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);
            Debug.Log("Level data loaded successfully.");
            return levelData;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load level data: " + e.Message);
            return null;
        }
    }

    // Save level data to a JSON file in the AppData directory
    public static void SaveToFile(LevelData levelData, string fileName = "LevelData.json")
    {
        // Ensure the AppData directory exists
        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }

        string path = Path.Combine(appDataPath, fileName);

        try
        {
            string json = JsonUtility.ToJson(levelData, true);
            File.WriteAllText(path, json);
            Debug.Log("Level data saved successfully to " + path);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save level data: " + e.Message);
        }
    }

    // Open a file dialog to select a JSON file and load it into LevelData
    public static LevelData OpenFileDialog()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "json", false);

        if (paths.Length > 0)
        {
            string filePath = paths[0];
            Debug.Log("File selected: " + filePath);
            try
            {
                string json = File.ReadAllText(filePath);
                LevelData levelData = JsonUtility.FromJson<LevelData>(json);
                Debug.Log("File loaded successfully.");
                return levelData;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load file: " + e.Message);
            }
        }
        return null;
    }

    // Open a save file dialog to save LevelData as a JSON file
    public static void SaveFileDialog(LevelData levelData)
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "LevelData.json", "json");

        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                string json = JsonUtility.ToJson(levelData, true);
                File.WriteAllText(path, json);
                Debug.Log("File saved at: " + path);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save file: " + e.Message);
            }
        }
    }
}
