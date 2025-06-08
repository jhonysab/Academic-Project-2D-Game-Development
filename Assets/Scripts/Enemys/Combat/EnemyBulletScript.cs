using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    public float force = 5f;
    public int damage = 20; //Coloquei para ser configurável no Inspector

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Vector3 direction = player.transform.position - transform.position;

            rb.linearVelocity = direction.normalized * force;

            float rot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rot + 90);
        }

        Destroy(gameObject, 1f); // Destruir automaticamente após 10s se não atingir o Player
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player_Health playerHealth = other.GetComponent<Player_Health>();
            if (playerHealth != null)
            {
                playerHealth.ChangeHealth(-damage); // Aplica o dano 
            }

            Destroy(gameObject); // Destroi a bala após atingir
        }
    }
}
