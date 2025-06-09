using UnityEngine;
using System.Linq; // Usado para ordenar os inimigos por distância

public class NpcAttackController : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("A parte do NPC que gira para mirar (geralmente contém o sprite).")]
    [SerializeField] private Transform rotationPoint;
    [Tooltip("Referência ao script que lida com o disparo.")]
    [SerializeField] private TowerShooting shootingHandler;

    [Header("Atributos de Ataque")]
    [Tooltip("O raio de detecção de inimigos a partir do centro do NPC.")]
    [SerializeField] private float attackRange = 7f;
    [Tooltip("O tempo em segundos entre cada disparo.")]
    [SerializeField] private float attackCooldown = 1f;
    [Tooltip("A camada (Layer) onde os inimigos estão.")]
    [SerializeField] private LayerMask enemyLayer;

    private Transform currentTarget;
    private float currentCooldownTimer = 0f;

    void Awake()
    {
        // Pega o TowerShooting se não for atribuído no Inspector
        if (shootingHandler == null)
        {
            shootingHandler = GetComponent<TowerShooting>();
        }
    }

    void Update()
    {
        if (currentTarget == null)
        {
            FindNewTarget(); // Procura um novo alvo se não tiver um
        }
        else
        {
            // Se o alvo saiu do alcance ou foi destruído, procura um novo
            if (Vector2.Distance(transform.position, currentTarget.position) > attackRange)
            {
                currentTarget = null;
            }
            else
            {
                // Se tem um alvo válido, mira e atira
                AimAtTarget();

                if (currentCooldownTimer <= 0f)
                {
                    Shoot();
                }
            }
        }

        if (currentCooldownTimer > 0)
        {
            currentCooldownTimer -= Time.deltaTime;
        }
    }

    void FindNewTarget()
    {
        // Cria um círculo de detecção e pega todos os inimigos dentro dele
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        if (enemiesInRange.Length > 0)
        {
            // Ordena os inimigos pela distância e pega o mais próximo como novo alvo
            currentTarget = enemiesInRange.OrderBy(enemy => 
                Vector2.Distance(transform.position, enemy.transform.position)
            ).FirstOrDefault()?.transform;
        }
    }

    void AimAtTarget()
    {
        if (rotationPoint == null || currentTarget == null) return;

        Vector2 direction = (Vector2)currentTarget.position - (Vector2)rotationPoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplica a rotação ao ponto de rotação para mirar no alvo
        rotationPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Shoot()
    {
        if (shootingHandler != null)
        {
            shootingHandler.PerformShoot();
            currentCooldownTimer = attackCooldown; // Reseta o cooldown
        }
    }

    // Desenha o raio de ataque no editor para fácil visualização
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}