using UnityEngine;
using TMPro;

public class GoalManager : MonoBehaviour
{

    public int targetVase = 0;
    public int targetBox = 0;
    public int targetStone = 0;


    public TextMeshProUGUI vaseText;
    public TextMeshProUGUI boxText;
    public TextMeshProUGUI stoneText;

    private int remainingVase;
    private int remainingBox;
    private int remainingStone;

    public GameObject vaseCheck;
    public GameObject boxCheck;
    public GameObject stoneCheck;

    void Start()
    {
        
        remainingVase = targetVase;
        remainingBox = targetBox;
        remainingStone = targetStone;

        UpdateUI();
    }

    public void SetGoals(int vase, int box, int stone)
    {
        targetVase = vase;
        targetBox = box;
        targetStone = stone;

        remainingVase = vase;
        remainingBox = box;
        remainingStone = stone;

        UpdateUI();
    }

    public void SubtractVase()
    {
        if (remainingVase > 0)
            remainingVase--;

        UpdateUI();
        CheckGoalCompletion();
    }

    public void SubtractBox()
    {
        if (remainingBox > 0)
            remainingBox--;

        UpdateUI();
        CheckGoalCompletion();
    }

    public void SubtractStone()
    {
        if (remainingStone > 0)
            remainingStone--;

        UpdateUI();
        CheckGoalCompletion();
    }

    private void UpdateUI()
    {
        if (vaseText != null)
            if (remainingVase <= 0)
            {
                vaseText.gameObject.SetActive(false);
                vaseCheck.SetActive(true);
            }
            else
            {
                vaseText.gameObject.SetActive(true);
                vaseText.text = remainingVase.ToString();
                vaseCheck.SetActive(false);
            }

        if (boxText != null)
            if (remainingBox <= 0)
            {
                boxText.gameObject.SetActive(false);
                boxCheck.SetActive(true);
            }
            else
            {
                boxText.gameObject.SetActive(true);
                boxText.text = remainingBox.ToString();
                boxCheck.SetActive(false);
            }

        if (stoneText != null)
            if (remainingStone <= 0)
            {
                stoneText.gameObject.SetActive(false);
                stoneCheck.SetActive(true);
            }
            else
            {
                stoneText.gameObject.SetActive(true);
                stoneText.text = remainingStone.ToString();
                stoneCheck.SetActive(false);
            }
    }

    private void CheckGoalCompletion()
    {
        if (remainingVase <= 0 && remainingBox <= 0 && remainingStone <= 0)
        {
            Debug.Log("All goals completed!");
            GameManager.Instance.WinLevel();
        }
    }
}
