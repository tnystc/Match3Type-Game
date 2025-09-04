using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private UIManager ui;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ui = FindFirstObjectByType<UIManager>();
        if (ui == null)
        {
            Debug.LogError("UIManager not found");
        }
    }

    public void WinLevel()
    {
        PlayerPrefs.SetInt("LastLevel", PlayerPrefs.GetInt("LastLevel", 1) + 1);
        PlayerPrefs.Save();

        if (ui != null)
            ui.ShowWin();
    }

    public void FailLevel()
    {
        if (ui != null)
            ui.ShowFail();
    }
}

