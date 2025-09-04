using System.Security;
using UnityEngine;

public class Tiles : MonoBehaviour
{

    public int hp;
    public TileKind kind;

    public int x, y;

    private Board board;
    private GoalManager goalManager;

    public GameObject stoneParticles;
    public GameObject boxParticles;

    void Start()
    {
        board = FindFirstObjectByType<Board>();
        goalManager = FindFirstObjectByType<GoalManager>();
    }

    private void Update()
    {
        if (hp <= 0)
        {
            board.obstacleTiles[x, y] = null;
            SpawnParticles();
            Destroy(this.gameObject);
            if (kind == TileKind.box)
            {
                goalManager.SubtractBox();
            }
            else if (kind == TileKind.stone)
            {
                goalManager.SubtractStone();
            }

        }
    }

    public void Damage(int damage)
    {
        hp -= damage;
    }

    private void SpawnParticles()
    {
            GameObject effectToInstantiate = null;
            switch (kind)
            {
                case TileKind.stone:
                    effectToInstantiate = stoneParticles;
                    break;
                case TileKind.box:
                    effectToInstantiate = boxParticles;
                    break;
            }
            if (effectToInstantiate != null)
            {                
                GameObject particle = Instantiate(effectToInstantiate, transform.position, Quaternion.identity);
                ParticleSystem[] bundle = particle.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem x in bundle)
                {
                    x.Play();
                }
                Destroy(particle, 0.6f);
            
            }
    }
}
