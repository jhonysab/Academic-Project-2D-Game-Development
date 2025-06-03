using UnityEngine;
using System.Collections; // Necessário para IEnumerator se você usar corrotinas no futuro
using System.Collections.Generic; // Necessário para List se você usar listas no futuro

public class EnemyCombinedMovement : MonoBehaviour
{
    public enum BehaviorState
    {
        PathFollowing,
        ChasingPlayer,
        ReturningToPath,
        Attacking
    }

    [Header("Referências de Componentes")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    private Enemy_Combat enemyCombat; // Para dano melee
    public EnemyShooting enemyShootingHandler; // Para ataque à distância

    [Header("Atributos de Movimento no Caminho")]
    [SerializeField] private float pathFollowSpeed = 1.5f;
    private Transform currentWaypoint;
    private int currentPathIndex = 0;

    [Header("Atributos de Perseguição")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float chaseDetectionRange = 7f; // Distância para começar a perseguir
    private Transform playerTransform;

    [Header("Atributos de Ataque Gerais")]
    [SerializeField] private float attackRange = 1.5f; // Para melee: distância de ataque. Para ranged: distância ideal de tiro.
    [SerializeField] private float attackCooldown = 2.0f;
    private float timeSinceLastAttack = 0f;
    private bool isPerformingAttack = false; // Controla se a animação/ação de ataque está em progresso

    [Header("Configurações Específicas de Ataque")]
    public bool isRangedUnit = false;
    [SerializeField] private string meleeAttackAnimationTrigger = "AttackTrigger";
    [SerializeField] private string rangedAttackAnimationTrigger = "RangedAttackTrigger";

    [Header("Configurações Gerais")]
    public BehaviorState currentState { get; private set; }
    private int facingDirection = 1;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
        
        if (!isRangedUnit) // Só pega Enemy_Combat se for melee
        {
            enemyCombat = GetComponent<Enemy_Combat>();
            if (enemyCombat == null)
            {
                Debug.LogWarning("Componente Enemy_Combat não encontrado em " + gameObject.name + " (necessário para melee).");
            }
        }
        
        if (isRangedUnit && enemyShootingHandler == null)
        {
            enemyShootingHandler = GetComponent<EnemyShooting>();
            if (enemyShootingHandler == null)
            {
                Debug.LogError("Componente EnemyShooting não encontrado ou não atribuído em " + gameObject.name + " (necessário para ranged).");
            }
        }
    }

    void Start()
    {
        SwitchToPathFollowing();
        timeSinceLastAttack = attackCooldown; // Permite atacar assim que possível
    }

    void Update()
    {
        if (isPerformingAttack) return; 

        if (timeSinceLastAttack < attackCooldown)
        {
            timeSinceLastAttack += Time.deltaTime;
        }

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
                // Se o jogador saiu do alcance enquanto o inimigo estava neste estado (mas não na animação 'isPerformingAttack')
                if (playerTransform == null || Vector2.Distance(transform.position, playerTransform.position) > (isRangedUnit ? chaseDetectionRange : attackRange * 1.2f) )
                {
                    SwitchToReturningToPath();
                }
                break;
        }
        UpdateAnimationStates();
    }

    void FixedUpdate()
    {
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
                    // Para ranged, attackRange é a distância de tiro. Para melee, é a distância de engajamento.
                    float currentEngagementRange = attackRange; 

                    if (distanceToPlayer > currentEngagementRange)
                    {
                        MoveTowards(playerTransform, chaseSpeed);
                    }
                    else // Dentro ou no alcance de engajamento/ataque
                    {
                        if (rb != null) rb.linearVelocity = Vector2.zero; // Para de se mover
                        // A lógica em UpdateChasingPlayerState decide se ataca
                    }
                }
                else
                {
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
        // Lógica para encontrar o waypoint mais próximo ou o próximo no caminho
        if (LevelManager.main != null && LevelManager.main.path.Length > 0)
        {
            // Se currentPathIndex for inválido ou muito distante, recalcula o mais próximo
            bool findClosest = true;
            if (currentPathIndex >= 0 && currentPathIndex < LevelManager.main.path.Length && currentWaypoint != null) {
                if (Vector2.Distance(transform.position, LevelManager.main.path[currentPathIndex].position) < chaseDetectionRange * 2) { // Heurística
                    findClosest = false; // Mantém o índice atual se estivermos razoavelmente perto do caminho
                }
            }

            if (findClosest) {
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
            }
            currentPathIndex = Mathf.Clamp(currentPathIndex, 0, LevelManager.main.path.Length - 1);
            currentWaypoint = LevelManager.main.path[currentPathIndex];
        }
        else
        {
            Debug.LogError(gameObject.name + ": LevelManager ou seu caminho (path) não está configurado!");
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
        SwitchToPathFollowing(); // Reutiliza a lógica de encontrar o waypoint para o retorno
    }
    
    // ESTE É O ÚNICO MÉTODO SwitchToAttackingState
    void SwitchToAttackingState()
    {
        currentState = BehaviorState.Attacking;
        timeSinceLastAttack = 0f;
        isPerformingAttack = true; 
        
        if (rb != null) rb.linearVelocity = Vector2.zero; 

        if (isRangedUnit)
        {
            if (anim != null && !string.IsNullOrEmpty(rangedAttackAnimationTrigger))
            {
                anim.SetTrigger(rangedAttackAnimationTrigger);
            }
            else if (enemyShootingHandler != null) // Fallback se não houver trigger de animação, mas isso não é ideal
            {
                // O ideal é que AnimationEvent_FireRangedProjectile seja chamado pela animação.
                // Se atirar imediatamente, pode não sincronizar com a animação (se houver).
                Debug.LogWarning(gameObject.name + ": Inimigo ranged tentando atacar sem trigger de animação configurado. Disparo deve ser por Animation Event.");
                 // enemyShootingHandler.PerformShoot(); // Removido para forçar o uso do Animation Event
            }
            else
            {
                Debug.LogError(gameObject.name + ": EnemyShootingHandler não configurado para unidade ranged.");
                isPerformingAttack = false; // Falha ao tentar atacar
            }
        }
        else // Unidade Melee
        {
            if (anim != null && !string.IsNullOrEmpty(meleeAttackAnimationTrigger))
            {
                anim.SetTrigger(meleeAttackAnimationTrigger);
            }
            else // Se não tiver Animator ou trigger, não pode atacar visualmente
            {
                 Debug.LogWarning(gameObject.name + ": Inimigo melee tentando atacar sem trigger de animação melee ou Animator.");
                 isPerformingAttack = false; // Falha ao tentar atacar
            }
        }
    }

    void UpdatePathFollowingState()
    {
        if (currentWaypoint == null)
        {
            if (LevelManager.main == null || LevelManager.main.path.Length == 0) {
                Debug.LogError(gameObject.name + ": Impossível seguir caminho, LevelManager ou path não configurado.");
                return; // Não pode fazer nada se não há caminho
            }
            // Tenta pegar o primeiro waypoint se currentWaypoint for nulo por algum motivo
            currentPathIndex = 0;
            currentWaypoint = LevelManager.main.path[currentPathIndex];
            if (currentWaypoint == null) { // Ainda nulo, problema sério no path
                Debug.LogError(gameObject.name + ": Waypoint inicial do caminho é nulo.");
                return;
            }
        }

        float distanceToWaypointSqr = (transform.position - currentWaypoint.position).sqrMagnitude; // Usar SqrMagnitude é mais eficiente
        if (distanceToWaypointSqr <= 0.2f * 0.2f) // Comparar com o quadrado da distância
        {
            currentPathIndex++; 
            if (currentPathIndex >= LevelManager.main.path.Length) 
            {
                if (EnemySpawner.onEnemyDestroy != null) EnemySpawner.onEnemyDestroy.Invoke(); 
                Destroy(gameObject); 
                return;
            }
            currentWaypoint = LevelManager.main.path[currentPathIndex]; 
            if (currentWaypoint == null) {
                 Debug.LogError(gameObject.name + ": Próximo waypoint no caminho é nulo. PathIndex: " + currentPathIndex);
                 // Decide o que fazer, talvez destruir ou ir para o início. Por ora, paramos.
                 currentState = BehaviorState.ReturningToPath; // Tenta se achar
                 return;
            }
        }
    }

    void UpdateChasingPlayerState()
    {
        if (playerTransform == null) 
        {
            SwitchToReturningToPath(); 
            return;
        }

        // Lógica de virar o sprite para encarar o jogador
        if ((playerTransform.position.x > transform.position.x && facingDirection == -1) || 
            (playerTransform.position.x < transform.position.x && facingDirection == 1)) 
        {
            FlipSprite(); 
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        // Verifica se pode atacar (está no alcance, cooldown pronto, e não está já executando um ataque)
        if (distanceToPlayer <= attackRange && timeSinceLastAttack >= attackCooldown && !isPerformingAttack)
        {
            SwitchToAttackingState();
        }
        // Se o jogador fugiu para além do alcance de detecção (chaseDetectionRange)
        else if (distanceToPlayer > chaseDetectionRange && !isPerformingAttack) 
        {
            SwitchToReturningToPath();
        }
        // Se estiver no alcance de ataque mas em cooldown, o FixedUpdate já está parando o movimento.
    }

    void UpdateReturningToPathState()
    {
        // A lógica de retornar ao caminho agora é principalmente gerenciada por SwitchToPathFollowing ao final de SwitchToReturningToPath
        // Mas precisamos de uma condição para de fato seguir o currentWaypoint definido
        if (currentWaypoint == null) 
        {
            SwitchToPathFollowing(); // Tenta redefinir se currentWaypoint ficou nulo
            return;
        }

        float distanceToWaypointSqr = (transform.position - currentWaypoint.position).sqrMagnitude; 
        if (distanceToWaypointSqr <= 0.1f * 0.1f) 
        {
            SwitchToPathFollowing(); // Chegou ao waypoint de retorno, volta a seguir o caminho normalmente
        }
        // O movimento em si é feito no FixedUpdate, case ReturningToPath
    }

    void MoveTowards(Transform target, float speed)
    {
        if (target == null || rb == null) return; 

        Vector2 direction = ((Vector2)target.position - rb.position).normalized; 
        rb.linearVelocity = direction * speed; 

        // Lógica de Flip baseada na direção do movimento
        if (Mathf.Abs(direction.x) > 0.01f) // Só vira se houver movimento horizontal significativo
        {
            if (direction.x > 0 && facingDirection == -1) FlipSprite();
            else if (direction.x < 0 && facingDirection == 1) FlipSprite();
        }
        // Debug.DrawLine(transform.position, target.position, Color.red); 
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            // Atualiza a referência do jogador e muda para perseguição se não estiver já atacando ou perseguindo ativamente.
            playerTransform = collision.transform; 
            if (currentState == BehaviorState.PathFollowing || currentState == BehaviorState.ReturningToPath) 
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
                // Só volta ao caminho se o jogador que saiu é o que estava sendo perseguido
                // E não está no meio de um ciclo de ataque (isPerformingAttack cobre a animação em si)
                // ou se o jogador saiu do chaseDetectionRange (verificado em UpdateChasingPlayerState)
                // Esta verificação aqui é mais para o trigger físico.
                // Se o chaseDetectionRange for maior que o trigger, o inimigo continuaria perseguindo.
                // Para simplificar, vamos assumir que o trigger é o limite de "ver" o jogador.
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

        bool isMoving = rb.linearVelocity.sqrMagnitude > 0.05f && !isPerformingAttack && currentState != BehaviorState.Attacking; 
        anim.SetBool("isMoving", isMoving); 
        
        // Para "isChasing", poderia ser (currentState == BehaviorState.ChasingPlayer || (currentState == BehaviorState.Attacking && !isPerformingAttack))
        // Mas se Attacking tem sua própria animação via trigger, "isChasing" pode ser só para o estado de perseguição.
        anim.SetBool("isChasing", currentState == BehaviorState.ChasingPlayer && !isPerformingAttack);
    }

    // Animation Event para o golpe do ataque Melee
    public void AnimationEvent_MeleeHit()
    {
        if (isRangedUnit || currentState != BehaviorState.Attacking || !isPerformingAttack || playerTransform == null) return;
        // Certifique-se que enemyCombat não é nulo (verificado no Awake para não-ranged)
        if (enemyCombat == null && !isRangedUnit) {
            Debug.LogError(gameObject.name + ": EnemyCombat é nulo para unidade melee em AnimationEvent_MeleeHit.");
            return;
        }


        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        // Aumentei um pouco a margem do attackRange aqui para compensar pequenas variações de posição durante a animação.
        if (distanceToPlayer <= attackRange * 1.2f) 
        {
            Player_Health playerHealth = playerTransform.GetComponent<Player_Health>();
            if (playerHealth != null)
            {
                enemyCombat.ApplyConfiguredDamage(playerHealth);
            }
        }
    }

    // Animation Event para disparar o projétil (Ranged Attack)
    public void AnimationEvent_FireRangedProjectile()
    {
        if (!isRangedUnit || currentState != BehaviorState.Attacking || !isPerformingAttack) return;
        // Certifique-se que enemyShootingHandler não é nulo (verificado no Awake para ranged)
         if (enemyShootingHandler == null && isRangedUnit) {
            Debug.LogError(gameObject.name + ": EnemyShootingHandler é nulo para unidade ranged em AnimationEvent_FireRangedProjectile.");
            return;
        }

        enemyShootingHandler.PerformShoot();
    }

    // Animation Event chamado ao final de qualquer animação de ataque (melee ou ranged)
    public void AnimationEvent_AttackFinished()
    {
        isPerformingAttack = false; // Permite que o Update volte a rodar normalmente

        // Decide o que fazer depois do ataque
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            // Se ainda estiver no alcance de ataque e o cooldown já tiver passado (improvável, mas para segurança)
            // ou se estiver dentro do alcance de perseguição mais amplo.
            if (distanceToPlayer <= attackRange && timeSinceLastAttack >= attackCooldown) 
            {
                SwitchToAttackingState(); // Tenta atacar de novo se puder
            }
            else if (distanceToPlayer <= chaseDetectionRange) 
            {
                SwitchToChasingPlayer(playerTransform); // Volta a perseguir
            }
            else
            {
                SwitchToReturningToPath(); // Jogador fugiu
            }
        }
        else
        {
            SwitchToReturningToPath(); // Jogador desapareceu
        }
    }
}