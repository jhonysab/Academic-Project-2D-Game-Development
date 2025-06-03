using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    public float force = 5f;
    public float damage = 20f; //Coloquei para ser configurável no Inspector

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

        Destroy(gameObject, 10f); // Destruir automaticamente após 10s se não atingir o Player
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

    public class EnemyShooting : MonoBehaviour
{
    public GameObject bulletPrefab;    // Arraste seu prefab de projétil aqui
    public Transform bulletSpawnPoint; // Ponto de onde o projétil será disparado

    // Este método será chamado pelo EnemyCombinedMovement através de um Animation Event
    public void PerformShoot()
    {
        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            // Instancia o projétil na posição e rotação do bulletSpawnPoint.
            // A rotação inicial do projétil em si pode ser Quaternion.identity
            // se o próprio script do projétil (EnemyBulletScript) já o orienta para o jogador.
            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        }
        else
        {
            Debug.LogError("Bullet Prefab ou Bullet Spawn Point não configurado no EnemyShooting de " + gameObject.name);
        }
    }
}
}
