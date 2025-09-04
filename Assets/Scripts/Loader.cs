using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public int grid_width;
    public int grid_height;
    public int move_count;
    public List<String> grid;
}

public class Loader : MonoBehaviour
{
    public LevelData LoadLevel(int levelNumber)
    {
        string path = "Levels/Level" + levelNumber;
        TextAsset jsonFile = Resources.Load<TextAsset>(path);
        if (jsonFile == null)
        {
            Debug.LogError("Level file not found: " + path);
            return null;
        }
        LevelData levelData = JsonUtility.FromJson<LevelData>(jsonFile.text);
        if (levelData == null)
        {
            Debug.LogError("Failed to parse level data from: " + path);
            return null;
        }
        return levelData;
    }
}
