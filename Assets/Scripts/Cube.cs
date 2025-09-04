using System.Collections;
using System.Data;
using UnityEngine;

public class Cube : MonoBehaviour
{

    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    public float angle = 0;
    public float dontSwipe = 1f;
    public int x;
    public int y;
    public int targetX;
    public int targetY;
    public GameObject otherCube;
    public Board board;
    private Vector2 tempPos;
    
    public bool isMatched = false;
    public int previousX;
    public int previousY;

    private FindMatches findMatches;
    private MoveManager moveManager;


    public bool isVerticalRocket;
    public bool isHorizontalRocket;
    public GameObject VerticalRocket;
    public GameObject HorizontalRocket;

    void Start()
    {
        isVerticalRocket = false;
        isHorizontalRocket = false;

        board = Object.FindFirstObjectByType<Board>();
        findMatches = Object.FindFirstObjectByType<FindMatches>();
        moveManager = Object.FindFirstObjectByType<MoveManager>();
    }

    void Update()
    {
        targetX = x;
        targetY = y;
        //Movement on X axis
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .2f);
            if (board.cubeGrid[x, y] != this.gameObject)
            {
                board.cubeGrid[x, y] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
            board.cubeGrid[x, y] = this.gameObject;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, .2f);
            if (board.cubeGrid[x, y] != this.gameObject)
            {
                board.cubeGrid[x, y] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }
    }

    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)
        {
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        //DEBUG PURPOSES
        if (Input.GetMouseButtonDown(1))
        {
            MakeVerticalRocket();
        }
        
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    private void CalculateAngle()
    {

        if (Mathf.Abs(finalTouchPos.y - firstTouchPos.y) > dontSwipe || Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > dontSwipe)
        {
            board.currentState = GameState.wait;
            angle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentCube = this;
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void MovePiecesHelper(Vector2 direction)
    {
        otherCube = board.cubeGrid[x + (int)direction.x, y + (int)direction.y];
        previousX = x;
        previousY = y;
        if (otherCube == null || otherCube.CompareTag("Vase") || otherCube.CompareTag("Rocket"))
        {
            board.currentState = GameState.move;
            return;
        }
        otherCube.GetComponent<Cube>().x += -1 * (int)direction.x;
        otherCube.GetComponent<Cube>().y += -1 * (int)direction.y;
        x += (int)direction.x;
        y += (int)direction.y;
        StartCoroutine(CheckMove());
    }
    void MovePieces()
    {
        //Right swipe
        if (angle > -45 && angle <= 45 && x < board.width - 1)
        {
            MovePiecesHelper(Vector2.right);
        }
        //Up swipe
        else if (angle > 45 && angle <= 135 && y < board.height - 1)
        {
            MovePiecesHelper(Vector2.up);
        }
        //Left swipe
        else if (angle > 135 || angle <= -135 && x > 0)
        {
            MovePiecesHelper(Vector2.left);
        }
        //Down swipe
        else if (angle < -45 && angle >= -135 && y > 0)
        {
            MovePiecesHelper(Vector2.down);
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    public IEnumerator CheckMove()
    {
        yield return new WaitForSeconds(.5f);
        if (otherCube != null)
        {
            if (!isMatched && !otherCube.GetComponent<Cube>().isMatched)
            {
                otherCube.GetComponent<Cube>().x = x;
                otherCube.GetComponent<Cube>().y = y;
                x = previousX;
                y = previousY;
                yield return new WaitForSeconds(.2f);
                board.currentCube = null;
                board.currentState = GameState.move;
            }
            else
            {
               board.DestroyMatches();
               moveManager.SpendMove();
            }
        }
    }


    public void MakeHorizontalRocket()
    {
        isHorizontalRocket = true;

        GameObject rocket = Instantiate(HorizontalRocket, transform.position, Quaternion.identity);
        Rocket rocketComp = rocket.GetComponent<Rocket>();
        if (rocketComp != null)
        {
            rocketComp.isVertical = false;
        }
        Cube cubeComp = rocket.GetComponent<Cube>();
        if (cubeComp != null)
        {
            cubeComp.isHorizontalRocket = true;
            cubeComp.x = (int)transform.position.x;
            cubeComp.y = (int)transform.position.y;
        }
        board.cubeGrid[x, y] = rocket;
        Destroy(gameObject);
    }


    public void MakeVerticalRocket()
    {
        isVerticalRocket = true;

        GameObject rocket = Instantiate(VerticalRocket, transform.position, Quaternion.identity);
        Rocket rocketComp= rocket.GetComponent<Rocket>();
        if (rocketComp != null)
        {
            rocketComp.isVertical = true;
        }
        Cube cubeComp = rocket.GetComponent<Cube>();
        if (cubeComp != null)
        {
            cubeComp.isVerticalRocket = true;
            cubeComp.x = (int)transform.position.x;
            cubeComp.y = (int)transform.position.y;
        }
        board.cubeGrid[x, y] = rocket;
        Destroy(gameObject);
    }



}
