using UnityEngine;
using System.Linq;

public class NpcAttackController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private TowerShooting shootingHandler;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("Atributos de Ataque")]
    [SerializeField] private float attackRange = 7f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private LayerMask enemyLayer;

    private Transform currentTarget;
    private float currentCooldownTimer = 0f;

    void Awake()
    {
        if (shootingHandler == null) shootingHandler = GetComponent<TowerShooting>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (anim == null) Debug.LogError("Animator não encontrado no NPC!");
        if (spriteRenderer == null) Debug.LogError("SpriteRenderer não encontrado no NPC!");
    }

    void Update()
    {
        if (currentCooldownTimer > 0)
        {
            currentCooldownTimer -= Time.deltaTime;
        }

        FindNewTarget();

        if (currentTarget != null)
        {
            AimAtTarget(); // Esta função agora SÓ envia informações para o Animator.

            if (currentCooldownTimer <= 0f)
            {
                Attack();
            }
        }
    }

    void FindNewTarget()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        currentTarget = enemiesInRange.OrderBy(enemy => Vector2.Distance(transform.position, enemy.transform.position))
                                      .FirstOrDefault()?.transform;
    }

    // MÉTODO MODIFICADO:
    void AimAtTarget()
    {
        if (currentTarget == null) return;

        Vector2 direction = (Vector2)currentTarget.position - (Vector2)transform.position;

        // --- LÓGICA DE ROTAÇÃO FOI REMOVIDA DAQUI ---
        // A linha "transform.rotation = Quaternion.Euler(...);" foi deletada.
        // O Animator agora é 100% responsável pela direção visual.

        // --- LÓGICA DE FLIP VISUAL (Ainda necessária para esquerda/direita) ---
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true; // Vira a imagem para a esquerda
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false; // Mantém a imagem para a direita
        }
        
        // --- ENVIANDO PARÂMETROS PARA O BLEND TREE ---
        // A única tarefa desta função agora é dizer ao Animator para onde olhar.
        if (anim != null)
        {
            Vector2 normalizedDirection = direction.normalized;
            // Usamos Mathf.Abs(X) porque o flipX já cuida da direção.
            // O Blend Tree só precisa saber se é para frente, diagonal ou para cima/baixo.
            anim.SetFloat("DirectionX", Mathf.Abs(normalizedDirection.x));
            anim.SetFloat("DirectionY", normalizedDirection.y);
        }
    }

    void Attack()
    {
        currentCooldownTimer = attackCooldown;
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }
    }

    // Este método é chamado pelo evento na sua animação
    public void AnimationEvent_FireProjectile()
    {
        // Precisamos garantir que o PontoDeDisparo gire junto com a animação
        // (que agora não gira mais). Portanto, a lógica de tiro precisa ser ajustada.
        if (shootingHandler != null)
        {
            // Precisamos calcular a rotação no momento do disparo.
            if(currentTarget != null) {
                Vector2 direction = (Vector2)currentTarget.position - (Vector2)transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                shootingHandler.PerformShoot(Quaternion.Euler(0, 0, angle));
            }
        }
    }
}