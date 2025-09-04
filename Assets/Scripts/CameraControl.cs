using System;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    private Board board;
    public float cameraOffset;
    public float aspectRatio = 0.5625f;
    public float padding = 2f;
    public float Yoffset = 1f;
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        
        board = FindFirstObjectByType<Board>();
        if (board!=null)
        {
            RepositionCamera(board.width-1, board.height-1);
            
        }
    }

    void RepositionCamera(float x, float y)
    {
        Vector3 tempPos = new Vector3(x / 2f, y / 2f + Yoffset, cameraOffset);
        transform.position = tempPos;

        if (board.width >= board.height || board.width >= 8)
        {
            Camera.main.orthographicSize = (board.width / 2f + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = board.height / 2f + padding;
        }

        
    }

}
