using UnityEngine;

// [RequireComponent] garante que este objeto sempre terá um Rigidbody2D
[RequireComponent(typeof(Rigidbody2D))]
public class AlliedProjectile : MonoBehaviour
{
    [Header("Configurações do Projétil")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifetime = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        // Pega a referência do Rigidbody2D no início
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Destrói a flecha após 'lifetime' segundos para limpar a cena
        Destroy(gameObject, lifetime);

        // Define a velocidade do projétil UMA VEZ.
        // 'transform.right' é um vetor que aponta para a direita LOCAL do objeto.
        // Como a torre já rotacionou a flecha ao criá-la, 'transform.right' agora
        // aponta exatamente na direção do alvo.
        if (rb != null)
        {
            rb.linearVelocity = transform.right * speed;
        }
    }

    // O método Update() não é mais necessário para o movimento,
    // pois a física está cuidando disso.

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Tenta pegar o script de saúde do objeto com o qual colidiu
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        // Se o objeto atingido tem o script EnemyHealth, então é um inimigo
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); // Destrói a flecha ao atingir um inimigo
        }
        // Opcional: Se quiser que a flecha também seja destruída ao atingir o cenário
        else if (other.gameObject.layer == LayerMask.NameToLayer("Cenario")) // Substitua "Cenario" pelo nome da sua camada de cenário
        {
            Destroy(gameObject);
        }
    }
}