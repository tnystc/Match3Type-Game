using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections.Generic;

public enum GameState
{
    wait,
    move
}

public enum TileKind
{
    normal,
    stone,
    box,
    vase
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{

    public int width;
    public int height;

    public int offset = 20;

    public GameState currentState = GameState.move;

    private FindMatches findMatches;

    public TileType[] boardLayout;

    public GameObject[,] cubeGrid;
    public GameObject[] cubes;

    public GameObject[] destroyEffects;

    public Cube currentCube;

    //Ony for stone and box
    public Tiles[,] obstacleTiles;
    public GameObject boxTilePrefab;
    public GameObject stoneTilePrefab;
    public GameObject vaseTilePrefab;

    private GoalManager goalManager;

    public int vaseCount;
    public int boxCount;
    public int stoneCount;

    public int currentLevel;
    
    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("LastLevel", 1);
        if (currentLevel == 0 || currentLevel > 10)
        {
            PlayerPrefs.SetInt("LastLevel", 1);
            PlayerPrefs.Save();
            currentLevel = 1;
        }

        findMatches = FindFirstObjectByType<FindMatches>();
        goalManager = FindFirstObjectByType<GoalManager>();
        Loader loader = FindFirstObjectByType<Loader>();
        LevelData level = loader.LoadLevel(currentLevel);
        LoadLevelData(level);
        FindFirstObjectByType<MoveManager>().SetMoves(level.move_count);
        CalculateObstacleGoals();
    }

    public void LoadLevelData(LevelData data)
    {
        width = data.grid_width;
        height = data.grid_height;
        cubeGrid = new GameObject[width, height];
        obstacleTiles = new Tiles[width, height];

        for (int i = 0; i < width*height; i++)
        {
            int x = i % width;
            int y = i / width;
            string tileKey = data.grid[i];

            Vector2 tempPos = new Vector2(x, y);

            switch (tileKey)
            {
                case "r":
                case "g":
                case "b":
                case "y":
                    GameObject cube = Instantiate(GetCubeByColor(tileKey), tempPos, Quaternion.identity) as GameObject;
                    cube.GetComponent<Cube>().y = y;
                    cube.GetComponent<Cube>().x = x;
                    cube.transform.parent = this.transform;
                    cube.name = "( " + x + ", " + y + " )";
                    cubeGrid[x, y] = cube;
                    break;
                case "rand":
                    GameObject randomCube = Instantiate(GetRandomCube(), tempPos, Quaternion.identity) as GameObject;
                    randomCube.GetComponent<Cube>().y = y;
                    randomCube.GetComponent<Cube>().x = x;
                    randomCube.transform.parent = this.transform;
                    randomCube.name = "( " + x + ", " + y + " )";
                    cubeGrid[x, y] = randomCube;
                    break;
                case "bo":
                    GameObject box = Instantiate(boxTilePrefab, tempPos, Quaternion.identity);
                    Tiles boxTile = box.GetComponent<Tiles>();
                    boxTile.x = x;
                    boxTile.y = y;
                    boxTile.kind = TileKind.box;
                    boxTile.hp = 1;
                    obstacleTiles[x, y] = boxTile;
                    break;
                case "s":
                    GameObject stone = Instantiate(stoneTilePrefab, tempPos, Quaternion.identity);
                    Tiles stoneTile = stone.GetComponent<Tiles>();
                    stoneTile.x = x;
                    stoneTile.y = y;
                    stoneTile.kind = TileKind.stone;
                    stoneTile.hp = 1;
                    obstacleTiles[x, y] = stoneTile;
                    break;
                case "v":
                    GameObject vase = Instantiate(vaseTilePrefab, tempPos, Quaternion.identity);
                    Cube vaseCube = vase.GetComponent<Cube>();
                    vaseCube.x = x;
                    vaseCube.y = y;
                    cubeGrid[x, y] = vase;
                    break;
                case "hro":
                    GameObject hro = Instantiate(GetCubeByColor("r"), tempPos, Quaternion.identity) as GameObject;
                    Cube hroComp = hro.GetComponent<Cube>();
                    hroComp.y = y;
                    hroComp.x = x;
                    hroComp.board = this;
                    hroComp.MakeHorizontalRocket();
                    break;
                case "vro":
                    GameObject vro = Instantiate(GetCubeByColor("r"), tempPos, Quaternion.identity) as GameObject;
                    Cube vroComp = vro.GetComponent<Cube>();
                    vroComp.y = y;
                    vroComp.x = x;
                    vroComp.board = this;
                    vroComp.MakeVerticalRocket();
                    break;
                default:
                    Debug.LogError("Unknown tile type: " + tileKey);
                    break;         
                }
        }
    }

    private GameObject GetCubeByColor(string color)
    {
        switch (color)
        {
            case "b":
                return cubes[0];
            case "g":
                return cubes[1];
            case "y":
                return cubes[2];
            case "r":
                return cubes[3];
            default:
                return null;
        }
    }

    private GameObject GetRandomCube()
    {
        return cubes[Random.Range(0, cubes.Length)];
    }

    public void CalculateObstacleGoals()
    {
        boxCount = 0;
        stoneCount = 0;
        vaseCount = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (obstacleTiles[i, j] != null)
                {
                    if (obstacleTiles[i, j].kind == TileKind.box)
                    {
                        boxCount++;
                    }
                    else if (obstacleTiles[i, j].kind == TileKind.stone)
                    {
                        stoneCount++;
                    }
                }
                else if (cubeGrid[i, j] != null && cubeGrid[i, j].CompareTag("Vase"))
                {
                    vaseCount++;
                }
            }
        }
        goalManager.SetGoals(vaseCount, boxCount, stoneCount);
    }


    private bool MatchesAt(int x, int y, GameObject piece)
    {
        if (x > 1 && y > 1)
        {
            if (cubeGrid[x - 1, y] != null && cubeGrid[x-2,y]!=null)
            {
                if (cubeGrid[x - 1, y].tag == piece.tag && cubeGrid[x - 2, y].tag == piece.tag)
                {
                    return true;
                }
            }
            if (cubeGrid[x, y - 1] != null && cubeGrid[x, y - 2] != null)
            {
                if (cubeGrid[x, y - 1].tag == piece.tag && cubeGrid[x, y - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if (x <= 1 || y <= 1)
        {
            if (y > 1)
            {
                if (cubeGrid[x, y - 1] != null && cubeGrid[x, y - 2] != null)
                {
                    if (cubeGrid[x, y - 1].tag == piece.tag && cubeGrid[x, y - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (x > 1)
            {
                if (cubeGrid[x - 1, y] != null && cubeGrid[x - 2, y] != null)
                {
                    if (cubeGrid[x - 1, y].tag == piece.tag && cubeGrid[x - 2, y].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
 

    private void DestroyMatchesAt(int x, int y)
    {
        if (cubeGrid[x,y].GetComponent<Rocket>() != null) return;
        if (cubeGrid[x, y].GetComponent<Cube>().isMatched)
        {
            //how many elements are in the matched pieces list from FindMatches
            if (findMatches.currentMatches.Count >= 4)
            {
                findMatches.CheckRockets();
            }

            DamageAdjacentObstacles(x, y);
            DamageAdjacentVases(x, y);

            //particle effects
            string cubeTag = cubeGrid[x, y].tag;
            GameObject effectToInstantiate = null;
            for (int i = 0; i < cubes.Length; i++)
            {
                if (destroyEffects[i].tag == cubeTag)
                {
                    effectToInstantiate = destroyEffects[i];
                    break;
                }
            }
            if (effectToInstantiate != null)
            {
                GameObject particle = Instantiate(effectToInstantiate, cubeGrid[x, y].transform.position, Quaternion.identity);
                particle.GetComponent<ParticleSystem>().Play();
                Destroy(particle, .6f);
            }
            
            Destroy(cubeGrid[x, y]);
            cubeGrid[x, y] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (cubeGrid[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowObstacle());
    }


    private IEnumerator DiagonalSlide()
    {
        yield return new WaitForSeconds(0.1f); // Wait for vertical gravity to settle

        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 1; y--) // from top to bottom
            {
                // If this cell is an obstacle
                if (obstacleTiles[x, y] == null) continue;

                int targetY = y - 1;

                // Go down the column under the obstacle
                while (targetY >= 0)
                {
                    // If the cell is already filled or has an obstacle, skip
                    if (cubeGrid[x, targetY] != null || obstacleTiles[x, targetY] != null)
                        break;

                    bool filled = false;

                    // Try slide from LEFT
                    if (x > 0 && cubeGrid[x - 1, targetY + 1] != null && obstacleTiles[x - 1, targetY + 1] == null)
                    {
                        GameObject cube = cubeGrid[x - 1, targetY + 1];
                        cubeGrid[x - 1, targetY + 1] = null;
                        cubeGrid[x, targetY] = cube;

                        Cube cubeComp = cube.GetComponent<Cube>();
                        cubeComp.x = x;
                        cubeComp.y = targetY;

                        filled = true;
                    }
                    // Try slide from RIGHT
                    else if (x < width - 1 && cubeGrid[x + 1, targetY + 1] != null && obstacleTiles[x + 1, targetY + 1] == null)
                    {
                        GameObject cube = cubeGrid[x + 1, targetY + 1];
                        cubeGrid[x + 1, targetY + 1] = null;
                        cubeGrid[x, targetY] = cube;

                        Cube cubeComp = cube.GetComponent<Cube>();
                        cubeComp.x = x;
                        cubeComp.y = targetY;

                        filled = true;
                    }

                    // If no cube was moved, stop checking lower rows
                    if (!filled)
                        break;

                    // Otherwise continue downward
                    targetY--;
                }
            }
        }

        yield return new WaitForSeconds(0.2f);
        StartCoroutine(FillBoard());
    }


    
    public IEnumerator DecreaseRowObstacle()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (obstacleTiles[i,j] != null) continue;
                if (cubeGrid[i, j] != null) continue;

                for (int k = j + 1; k < height; k++)
                {
                    if(obstacleTiles[i,k] != null) break;

                    if (cubeGrid[i, k] != null)
                    {
                        cubeGrid[i, j] = cubeGrid[i, k];
                        cubeGrid[i, k] = null;
                        cubeGrid[i, j].GetComponent<Cube>().y = j;
                        break;
                    }
                    
                }
            }   
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(DiagonalSlide());
    }

    private bool CheckForObstacle(int x, int y)
    {
        for (int i = y + 1; i < height; i++)
        {
            if (obstacleTiles[x, i] != null)
            {
                return true;
            }
        }
        return false;
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++) // Loop through each column
        {
            for (int j = 0; j < height; j++) // Loop through each row in the column
            {
                // Skip if there is an obstacle above this cell
                if (CheckForObstacle(i, j)) continue;

                // If the current cell is empty and not blocked by a box or stone
                if (cubeGrid[i, j] == null && obstacleTiles[i,j]==null)
                {
                    // Look for a cube above to move down
                    bool cubeMoved = false;
                    for (int k = j + 1; k < height; k++)
                    {
                        // Skip if there's an obstacle above
                        if (obstacleTiles[i, k] != null) break;

                        // If a cube is found above, move it down
                        if (cubeGrid[i, k] != null)
                        {
                            cubeGrid[i, k].GetComponent<Cube>().y = j;
                            cubeGrid[i, j] = cubeGrid[i, k];
                            cubeGrid[i, k] = null;
                            cubeMoved = true;
                            break;
                        }
                    }

                    // If no cube was moved and no obstacle is above, instantiate a new cube
                    if (!cubeMoved)
                    {
                        Vector2 tempPosition = new Vector2(i, j + offset);
                        int cubeToUse = Random.Range(0, cubes.Length);
                        GameObject piece = Instantiate(cubes[cubeToUse], tempPosition, Quaternion.identity);
                        cubeGrid[i, j] = piece;
                        piece.GetComponent<Cube>().x = i;
                        piece.GetComponent<Cube>().y = j;
                    }
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (cubeGrid[i, j] != null)
                {
                    if (cubeGrid[i, j].GetComponent<Cube>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoard()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
            FindFirstObjectByType<FindMatches>().CheckRockets();
        }
        findMatches.currentMatches.Clear();
        currentCube = null;
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
    }

    private void DamageAdjacentObstacles(int x, int y)
    {
        Vector2Int[] directions = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        for (int i =0; i < 4; i++)
        {

            int adjX = x + directions[i].x;
            int adjY = y + directions[i].y;

            if (adjX >= 0 && adjX < width && adjY >= 0 && adjY < height)
            {
                Tiles obs = obstacleTiles[adjX, adjY];
                if (obs != null &&  obs.kind != TileKind.stone)
                {
                    obs.Damage(1);
                }
            }
        }
    }

    private void DamageAdjacentVases(int x, int y)
    {
        List<GameObject> alreadyDamaged = new List<GameObject>();

        Vector2Int[] directions = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int dir in directions)
        {
            int adjX = x + dir.x;
            int adjY = y + dir.y;

            if (adjX >= 0 && adjX < width && adjY >= 0 && adjY < height)
            {
                GameObject obj = cubeGrid[adjX, adjY];
                if (obj != null && obj.CompareTag("Vase") && !alreadyDamaged.Contains(obj))
                {
                    obj.GetComponent<Vase>().Damage(1);
                    alreadyDamaged.Add(obj);
                }
            }
        }
    }

}



