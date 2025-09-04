using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainScene : MonoBehaviour
{
    public Button playButton;
    public TextMeshProUGUI playButtonText;

    void Start()
    {
        int lastLevel = PlayerPrefs.GetInt("LastLevel", 1);

        if (lastLevel > 10)
        {
            playButtonText.text = "Finished";
            playButton.interactable = false;
        }
        else
        {
            playButtonText.text = $"Level {lastLevel}";
            playButton.interactable = true;
        }

    }

    public void OnPlayButtonClicked()
    {
        
        int lastLevel = PlayerPrefs.GetInt("LastLevel", 1);

        if (lastLevel <= 10)
        {
            SceneManager.LoadScene("LevelScene");
        }
    }

    [ContextMenu("Reset Level Progress")]
    public void ResetProgress()
    {
        PlayerPrefs.SetInt("LastLevel", 1);
        PlayerPrefs.Save();
        Debug.Log("Progress reset to level 1");
    }

}
