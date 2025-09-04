using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class RocketPart : MonoBehaviour
{
    private Board board;
    private Vector2Int direction;

    public ParticleSystem smokeParticles;
    public ParticleSystem starParticles;

    public float speed = 0.1f;

    private void Start()
    {
        
    }

    public void Launch(Vector2Int dir)
    {
        board = FindFirstObjectByType<Board>();
        direction = dir;

        // Rotate to face correct direction
        if (dir == Vector2Int.up)
            transform.rotation = Quaternion.Euler(0, 0, 90);
        else if (dir == Vector2Int.down)
            transform.rotation = Quaternion.Euler(0, 0, -90);
        else if (dir == Vector2Int.left)
            transform.rotation = Quaternion.Euler(0, 0, 180);
        else if (dir == Vector2Int.right)
            transform.rotation = Quaternion.Euler(0, 0, 0);

        StartCoroutine(MoveAndDamage());
    }

    private IEnumerator MoveAndDamage()
    {
        Vector2 currentPos = transform.position;

        while (true)
        {
            currentPos += (Vector2)direction;

            Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(currentPos.x), Mathf.FloorToInt(currentPos.y));

            // Out of bounds?
            if (gridPos.x < 0 || gridPos.x >= board.width || gridPos.y < 0 || gridPos.y >= board.height)
            {   
                break;
            }

            //Particle Effects
            if (smokeParticles != null)
            {
                ParticleSystem smoke = Instantiate(smokeParticles, transform.position, transform.rotation);
                smoke.Play();
                Destroy(smoke.gameObject, 1f);
            }
            if (starParticles != null)
            {
                ParticleSystem star = Instantiate(starParticles, transform.position, transform.rotation);
                star.Play();
                Destroy(star.gameObject, 1f);
            }

            GameObject target = board.cubeGrid[gridPos.x, gridPos.y];

            Tiles obs = board.obstacleTiles[gridPos.x, gridPos.y];
            if (obs != null)
            {
                obs.Damage(1);
            }

            bool rocketHit = false;

            if (target != null)
            {
                // Vases
                if (target.CompareTag("Vase"))
                {
                    Vase vase = target.GetComponent<Vase>();
                    if (vase != null)
                    {
                        vase.Damage(1);
                    }
                }
                // Rockets
                else if (target.GetComponent<Rocket>())
                {
                    Rocket rocket = target.GetComponent<Rocket>();
                    if (rocket != null)
                    {
                        rocket.Explode();
                        rocketHit = true;

                    }
                }
                // Other cubes
                else if (target.GetComponent<Cube>() != null)
                {
                    //particle effects
                    string cubeTag = board.cubeGrid[gridPos.x, gridPos.y].tag;
                    GameObject effectToInstantiate = null;
                    for (int i = 0; i < board.cubes.Length; i++)
                    {
                        if (board.destroyEffects[i].tag == cubeTag)
                        {
                            effectToInstantiate = board.destroyEffects[i];
                            break;
                        }
                    }
                    if (effectToInstantiate != null)
                    {
                        GameObject particle = Instantiate(effectToInstantiate, board.cubeGrid[gridPos.x, gridPos.y].transform.position, Quaternion.identity);
                        particle.GetComponent<ParticleSystem>().Play();
                        Destroy(particle, .6f);
                    }
                    Destroy(target);
                }

                // Clear from grid
                if (!rocketHit)
                {
                    board.cubeGrid[gridPos.x, gridPos.y] = null;
                }
            }

            transform.position = currentPos;

            yield return new WaitForSeconds(speed*1.5f);
        }

        Destroy(gameObject); // Remove the rocket part
        board.StartCoroutine(board.DecreaseRowObstacle());

    }
}
