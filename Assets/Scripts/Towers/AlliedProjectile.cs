using UnityEngine;

public class AlliedProjectile : MonoBehaviour
{
    [Header("Configurações do Projétil")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifetime = 3f; // Tempo em segundos para a flecha se autodestruir se não atingir nada

    void Start()
    {
        // Agenda a destruição da flecha após 'lifetime' segundos para não poluir a cena
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move a flecha para a "frente" (direita no seu espaço local)
        // A rotação correta será dada pela torre no momento do disparo
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    // Este método é chamado automaticamente porque o nosso Collider2D está marcado como "Is Trigger"
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Tenta pegar o script de saúde do objeto com o qual colidiu
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        // Se o objeto atingido tem o script EnemyHealth, então é um inimigo
        if (enemy != null)
        {
            // Causa dano ao inimigo
            enemy.TakeDamage(damage);

            // Destrói a flecha imediatamente após atingir um inimigo
            Destroy(gameObject);
        }
    }
}