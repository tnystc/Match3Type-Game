using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    
    private Board board;
    public float hintTime;
    private float hintClock;
    public GameObject BlueHintPrefab;
    public GameObject RedHintPrefab;
    public GameObject GreenHintPrefab;
    public GameObject YellowHintPrefab;
    public GameObject hintParent;

    void Start()
    {
        board = FindFirstObjectByType<Board>();
        hintClock = hintTime;
    }

    
    void Update()
    {
        
    }


    
}
