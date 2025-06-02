using UnityEngine;

public class EnemyCombinedMovement : MonoBehaviour
{
    public enum BehaviorState
    {
        PathFollowing,
        ChasingPlayer,
        ReturningToPath,
        Attacking // Estado para o ataque
    }

    [Header("Referências de Componentes")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    private Enemy_Combat enemyCombat; // Referência ao script de combate

    [Header("Atributos de Movimento no Caminho")]
    [SerializeField] private float pathFollowSpeed = 1.5f;
    private Transform currentWaypoint;
    private int currentPathIndex = 0;

    [Header("Atributos de Perseguição")]
    [SerializeField] private float chaseSpeed = 4f;
    private Transform playerTransform;
    [SerializeField] private float chaseDetectionRange = 7f; // Adicione e ajuste este valor no Inspector

    [Header("Atributos de Ataque")]
    [SerializeField] private float attackRange = 1.5f;      // Distância para iniciar um ataque
    [SerializeField] private float attackCooldown = 2.0f;   // Tempo entre ataques
    [SerializeField] private string attackAnimationTrigger = "AttackTrigger"; // Nome do trigger no Animator
    private float timeSinceLastAttack = 0f;
    private bool isPerformingAttack = false; // Flag para controlar se a animação de ataque está em progresso

    [Header("Configurações Gerais")]
    public BehaviorState currentState { get; private set; }
    private int facingDirection = 1;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
        enemyCombat = GetComponent<Enemy_Combat>();
        if (enemyCombat == null)
        {
            Debug.LogError("Componente Enemy_Combat não encontrado em " + gameObject.name);
        }
    }

    void Start()
    {
        SwitchToPathFollowing();
        timeSinceLastAttack = attackCooldown; // Permite atacar assim que possível
    }

    void Update()
    {
        if (isPerformingAttack) return; // Não faz outras lógicas de estado enquanto a animação de ataque ocorre

        timeSinceLastAttack += Time.deltaTime;

        switch (currentState)
        {
            case BehaviorState.PathFollowing:
                UpdatePathFollowingState();
                break;
            case BehaviorState.ChasingPlayer:
                UpdateChasingPlayerState();
                break;
            case BehaviorState.ReturningToPath:
                UpdateReturningToPathState();
                break;
            case BehaviorState.Attacking:
                if (playerTransform == null || Vector2.Distance(transform.position, playerTransform.position) > attackRange * 1.2f) // 1.2f é uma margem
                {
                    // Se o jogador saiu do alcance durante a preparação (raro, pois isPerformingAttack deveria cobrir a animação em si)
                    // ou se o estado de ataque foi setado mas a animação ainda não começou e o jogador sumiu.
                    SwitchToReturningToPath();
                }
                break;
        }
        UpdateAnimationStates();
    }

    void FixedUpdate()
    {
        // Só move se não estiver atacando ou durante a preparação do ataque
        if (currentState == BehaviorState.Attacking || isPerformingAttack)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        switch (currentState)
        {
            case BehaviorState.PathFollowing:
                MoveTowards(currentWaypoint, pathFollowSpeed);
                break;

            case BehaviorState.ChasingPlayer:
                if (playerTransform != null) 
                {
                    float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

                    // Se o inimigo estiver FORA do alcance de ataque, ele se move em direção ao jogador.
                    if (distanceToPlayer > attackRange)
                    {
                        MoveTowards(playerTransform, chaseSpeed);
                    }
                    // Se o inimigo estiver DENTRO ou NO alcance de ataque:
                    else
                    {
                        // Para de se mover para não "grudar" no jogador.
                        // A lógica em UpdateChasingPlayerState() cuidará de iniciar um ataque se o cooldown permitir.
                        if (rb != null) rb.linearVelocity = Vector2.zero;
                    }
                }
                else
                {
                    // Se playerTransform se tornou nulo 
                    SwitchToReturningToPath();
                }
                break;

            case BehaviorState.ReturningToPath:
                MoveTowards(currentWaypoint, pathFollowSpeed);
                break;
        }
    }

    void SwitchToPathFollowing()
    {
        currentState = BehaviorState.PathFollowing;
        isPerformingAttack = false;
        if (LevelManager.main != null && LevelManager.main.path.Length > 0)
        {
            currentPathIndex = Mathf.Clamp(currentPathIndex, 0, LevelManager.main.path.Length - 1);
            currentWaypoint = LevelManager.main.path[currentPathIndex];
        }
        else
        {
            Debug.LogError("LevelManager ou seu caminho (path) não está configurado!");
            currentWaypoint = null;
        }
    }

    void SwitchToChasingPlayer(Transform targetPlayer)
    {
        currentState = BehaviorState.ChasingPlayer;
        playerTransform = targetPlayer;
        isPerformingAttack = false;
    }

    void SwitchToReturningToPath()
    {
        currentState = BehaviorState.ReturningToPath;
        playerTransform = null;
        isPerformingAttack = false;

        if (LevelManager.main != null && LevelManager.main.path.Length > 0)
        {
            float shortestDistance = Mathf.Infinity;
            int closestPathIndex = 0;

            for (int i = 0; i < LevelManager.main.path.Length; i++)
            {
                if (LevelManager.main.path[i] == null) continue;
                float distance = Vector2.Distance(transform.position, LevelManager.main.path[i].position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestPathIndex = i;
                }
            }
            currentPathIndex = closestPathIndex;
            currentWaypoint = LevelManager.main.path[currentPathIndex];
        }
        else
        {
            Debug.LogError("Não é possível retornar ao caminho: LevelManager ou caminho não disponível.");
            currentPathIndex = 0;
            currentWaypoint = (LevelManager.main != null && LevelManager.main.path.Length > 0) ? LevelManager.main.path[0] : null;
            if(currentWaypoint == null) SwitchToPathFollowing();
        }
    }
    
    void SwitchToAttackingState()
    {
        currentState = BehaviorState.Attacking;
        timeSinceLastAttack = 0f;
        isPerformingAttack = true; 
        
        if (rb != null) rb.linearVelocity = Vector2.zero; 
        if (anim != null) anim.SetTrigger(attackAnimationTrigger);
    }

    void UpdatePathFollowingState()
    {
        if (currentWaypoint == null)
        {
            if (LevelManager.main != null && LevelManager.main.path.Length > 0)
            {
                 currentWaypoint = LevelManager.main.path[Mathf.Clamp(currentPathIndex, 0, LevelManager.main.path.Length - 1)];
            } else {
                Debug.LogError("Caminho ou LevelManager não definido em UpdatePathFollowingState.");
                return;
            }
             if (currentWaypoint == null) return; // Ainda nulo após tentativa de correção
        }

        float distanceToWaypoint = (transform.position - currentWaypoint.position).sqrMagnitude;
        if (distanceToWaypoint <= 0.2f) 
        {
            currentPathIndex++; 
            if (currentPathIndex >= LevelManager.main.path.Length) 
            {
                if (EnemySpawner.onEnemyDestroy != null) EnemySpawner.onEnemyDestroy.Invoke(); 
                Destroy(gameObject); 
                return;
            }
            currentWaypoint = LevelManager.main.path[currentPathIndex]; 
        }
    }

    void UpdateChasingPlayerState()
    {
        if (playerTransform == null) 
        {
            SwitchToReturningToPath(); 
            return;
        }

        if ((playerTransform.position.x > transform.position.x && facingDirection == -1) || 
            (playerTransform.position.x < transform.position.x && facingDirection == 1)) 
        {
            FlipSprite(); 
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackRange && timeSinceLastAttack >= attackCooldown && !isPerformingAttack)
        {
            SwitchToAttackingState();
        }
        else if (distanceToPlayer > chaseDetectionRange && !isPerformingAttack) // Verifica se saiu do range de chase
        {
            SwitchToReturningToPath();
        }
    }

    void UpdateReturningToPathState()
    {
        if (currentWaypoint == null) 
        {
            SwitchToPathFollowing(); 
            return;
        }

        float distanceToWaypoint = (transform.position - currentWaypoint.position).sqrMagnitude; 
        if (distanceToWaypoint <= 0.1f) // Aumentei um pouco para garantir a transição
        {
            SwitchToPathFollowing(); 
        }
    }

    void MoveTowards(Transform target, float speed)
    {
        if (target == null || rb == null) return; 

        Vector2 direction = ((Vector2)target.position - rb.position).normalized; 
        rb.linearVelocity = direction * speed; 

        if (direction.x > 0.01f && facingDirection == -1) FlipSprite(); // Adicionado threshold para evitar flips rápidos com movimento mínimo
        else if (direction.x < -0.01f && facingDirection == 1) FlipSprite();

        Debug.DrawLine(transform.position, target.position, Color.red); 
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            playerTransform = collision.transform; 
            if (currentState != BehaviorState.ChasingPlayer && currentState != BehaviorState.Attacking) 
            {
                 SwitchToChasingPlayer(collision.transform); 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            if (playerTransform == collision.transform && currentState == BehaviorState.ChasingPlayer && !isPerformingAttack) 
            {
                // Se o jogador que saiu é o que estava sendo perseguido E não estamos no meio de um ataque
                SwitchToReturningToPath(); 
            }
        }
    }

    void FlipSprite()
    {
        facingDirection *= -1; 
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * facingDirection, transform.localScale.y, transform.localScale.z); 
    }

    void UpdateAnimationStates()
    {
        if (anim == null) return; 

        bool isMoving = rb.linearVelocity.sqrMagnitude > 0.05f && !isPerformingAttack; 
        anim.SetBool("isMoving", isMoving); 
        anim.SetBool("isChasing", currentState == BehaviorState.ChasingPlayer && !isPerformingAttack); 
    }

    public void AnimationEvent_Hit()
    {
        if (currentState == BehaviorState.Attacking && playerTransform != null && enemyCombat != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackRange * 1.1f) // Uma pequena margem extra para garantir o acerto
            {
                Player_Health playerHealth = playerTransform.GetComponent<Player_Health>();
                if (playerHealth != null)
                {
                    enemyCombat.ApplyConfiguredDamage(playerHealth);
                }
            }
        }
    }

    public void AnimationEvent_AttackFinished()
    {
        isPerformingAttack = false; 

        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackRange && timeSinceLastAttack >= attackCooldown) 
            {
                SwitchToAttackingState();
            }
            // Verifica o chaseDetectionRange explicitamente aqui para decidir se volta a perseguir
            else if (distanceToPlayer <= chaseDetectionRange) 
            {
                SwitchToChasingPlayer(playerTransform);
            }
            else
            {
                SwitchToReturningToPath();
            }
        }
        else
        {
            SwitchToReturningToPath();
        }
    }
}