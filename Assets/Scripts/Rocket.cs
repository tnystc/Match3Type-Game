using UnityEngine;

public class Rocket : MonoBehaviour
{
    public bool isVertical; // Set this when spawning the rocket
    public GameObject rocketPartPrefab; // Assign this in the Inspector

    private Board board;

    private MoveManager moveManager;

    private void Start()
    {
        board = FindFirstObjectByType<Board>();
        moveManager = FindFirstObjectByType<MoveManager>();
    }

    private void OnMouseDown()
    {
        //Debug.Log("Rocket clicked");
        Explode();
        moveManager.SpendMove();
    }

    public void Explode()
    {
        Vector2Int dir = isVertical ? Vector2Int.up : Vector2Int.right;

        SpawnPart(dir);
        SpawnPart(-dir);
        board.cubeGrid[(int)transform.position.x, (int)transform.position.y] = null;
        Destroy(gameObject);
    }

    private void SpawnPart(Vector2Int direction)
    {
        GameObject part = Instantiate(rocketPartPrefab, transform.position, Quaternion.identity);
        RocketPart rocketPart = part.GetComponent<RocketPart>();
        if (rocketPart != null)
        {
            rocketPart.Launch(direction);
        }
    }
}

