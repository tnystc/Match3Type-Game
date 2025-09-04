using UnityEngine;

public class Vase : MonoBehaviour
{
    public int hp = 2;
    public Sprite fullHpSprite;
    public Sprite halfHpSprite;

    private SpriteRenderer spriteRenderer;

    private Board board;
    private GoalManager goalManager;

    public GameObject vaseParticles;

    [System.Obsolete]
    void Start()
    {
        board = FindFirstObjectByType<Board>();
        goalManager = FindFirstObjectByType<GoalManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    public void Damage(int amount)
    {
        
        hp -= amount;
        UpdateSprite();

        if (hp <= 0)
        {
            board.obstacleTiles[(int)transform.position.x, (int)transform.position.y] = null;
            SpawnParticles();
            Destroy(gameObject);
            goalManager.SubtractVase();
        }
    }

    private void UpdateSprite()
    {
        if (spriteRenderer == null) return;

        if (hp == 2)
        {
            spriteRenderer.sprite = fullHpSprite;
        }
        else if (hp == 1)
        {
            spriteRenderer.sprite = halfHpSprite;
        }
    }

    private void SpawnParticles()
    {
        if (vaseParticles != null)
        {
            GameObject particle = Instantiate(vaseParticles, transform.position, Quaternion.identity);
            ParticleSystem[] bundle = particle.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem x in bundle)
            {
                x.Play();
            }
            Destroy(particle, 0.6f);
        }
    }
}
