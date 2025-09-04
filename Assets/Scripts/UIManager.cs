using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public GameObject WinPanel;
    public GameObject LosePanel;

    public Button nextLevelButton;
    public TextMeshProUGUI nextLevelButtonText;

    public void ShowWin()
    {
        WinPanel.SetActive(true);
        nextLevelButton.gameObject.SetActive(true);
        int lastLevel = PlayerPrefs.GetInt("LastLevel", 1);
        if (lastLevel > 10)
        {
            nextLevelButton.interactable = false;
            nextLevelButtonText.text = "Finished";
        }
        else
        {
            nextLevelButton.interactable = true;
            nextLevelButtonText.text = "Next Level";
        }

    }

    public void ShowFail()
    {
        LosePanel.SetActive(true);
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        nextLevelButton.gameObject.SetActive(false);

    }

    public void BackToMain()
    {
        SceneManager.LoadScene("MainScene");
        nextLevelButton.gameObject.SetActive(false);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene("LevelScene");
        nextLevelButton.gameObject.SetActive(false);
    }
}
