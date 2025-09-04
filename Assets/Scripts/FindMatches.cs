using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    MoveManager moveManager;

    void Start()
    {
        board = FindFirstObjectByType<Board>();
        moveManager = FindFirstObjectByType<MoveManager>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private void AddToListAndMatch(GameObject cube)
    {
        if (!currentMatches.Contains(cube))
        {
            currentMatches.Add(cube);
        }
        cube.GetComponent<Cube>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject cube1, GameObject cube2, GameObject cube3)
    {
        AddToListAndMatch(cube1);
        AddToListAndMatch(cube2);
        AddToListAndMatch(cube3);
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentCube = board.cubeGrid[i, j];
                if (currentCube != null)
                {
                    Cube currentCubeComp = currentCube.GetComponent<Cube>();

                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftCube = board.cubeGrid[i - 1, j];
                        GameObject rightCube = board.cubeGrid[i + 1, j];

                        if (leftCube != null && rightCube != null)
                        {
                            Cube leftCubeComp = leftCube.GetComponent<Cube>();
                            Cube rightCubeComp = rightCube.GetComponent<Cube>();

                            if (leftCube.tag == currentCube.tag && rightCube.tag == currentCube.tag
                                && leftCubeComp != null && rightCubeComp != null && currentCubeComp != null
                                && !leftCube.CompareTag("Vase") && !rightCube.CompareTag("Vase")  && !currentCube.CompareTag("Vase")
                                && !leftCube.CompareTag("Rocket") && !rightCube.CompareTag("Rocket")  && !currentCube.CompareTag("Rocket"))
                            {   
                                GetNearbyPieces(leftCube, currentCube, rightCube);
                                
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upCube = board.cubeGrid[i, j + 1];              
                        GameObject downCube = board.cubeGrid[i, j - 1];

                        if (upCube != null && downCube != null)
                        {
                            Cube upCubeComp = upCube.GetComponent<Cube>();
                            Cube downCubeComp = downCube.GetComponent<Cube>();

                            if (upCube.tag == currentCube.tag && downCube.tag == currentCube.tag
                                && upCubeComp != null && downCubeComp != null && currentCubeComp != null
                                && !upCube.CompareTag("Vase") && !downCube.CompareTag("Vase")  && !currentCube.CompareTag("Vase")
                                && !upCube.CompareTag("Rocket") && !downCube.CompareTag("Rocket")  && !currentCube.CompareTag("Rocket"))
                            {
                                GetNearbyPieces(upCube, currentCube, downCube);
                            }
                        }
                    }
                }
            }
        }
    }

    private void CreateRocket(Cube cube)
    {
        int rand = Random.Range(0, 100);
        if (rand < 50)
        {
            cube.MakeHorizontalRocket();
        }
        else
        {
            cube.MakeVerticalRocket();
        }
    }

    public void CheckRockets()
    {
        if (board.currentCube != null && board.currentCube.isMatched)
        {
            board.currentCube.isMatched = false;
            CreateRocket(board.currentCube);
        }
        else if (board.currentCube?.otherCube != null)
        {
            Cube other = board.currentCube.otherCube.GetComponent<Cube>();
            if (other != null && other.isMatched)
            {
                other.isMatched = false;
                CreateRocket(other);
            }
        }
    }

}

