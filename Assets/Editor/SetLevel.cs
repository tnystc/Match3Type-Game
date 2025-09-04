using UnityEditor;
using UnityEngine;

public class SetLevel : EditorWindow
{
    private int level = 1;

    [MenuItem("Tools/Set Last Played Level")]
    public static void ShowWindow()
    {
        GetWindow<SetLevel>("Set Last Level");
    }

    void OnGUI()
    {
        GUILayout.Label("Set Last Played Level", EditorStyles.boldLabel);
        level = EditorGUILayout.IntField("Level Number (1-10):", level);

        if (GUILayout.Button("Set Level"))
        {
            if (level >= 1 && level <= 10)
            {
                PlayerPrefs.SetInt("LastLevel", level);
                PlayerPrefs.Save();
                Debug.Log($"LastLevel set to {level}");
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid Level", "Please enter a value between 1 and 10.", "OK");
            }
        }
    }
}
