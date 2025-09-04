using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoveManager : MonoBehaviour
{
    public int moveCount;
    public TextMeshProUGUI moveText;

    public void SetMoves(int count)
    {
        moveCount = count;
        UpdateUI();
    }

    public void SpendMove()
    {
        moveCount--;
        UpdateUI();

        if (moveCount <= 0)
        {
            GameManager.Instance.FailLevel();
            
        }
    }

    void UpdateUI()
    {
        moveText.text = moveCount.ToString();
    }
}