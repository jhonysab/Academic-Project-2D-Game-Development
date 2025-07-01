using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public EnemyShooting enemyShootingHandler; // Para ataque à distância (se houver)

    [Header("Atributos de Movimento no Caminho")]
    [SerializeField] private float pathFollowSpeed = 1.5f;
    private Transform[] activePath; // MODIFICADO: Armazena o caminho atual do inimigo
    private Transform currentWaypoint;
    private int currentPathIndex = 0;

    [Header("Atributos de Perseguição")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float chaseDetectionRange = 7f;
    private Transform playerTransform;

    [Header("Atributos de Ataque Gerais")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2.0f;
    private float timeSinceLastAttack = 0f;
    private bool isPerformingAttack = false;

    [Header("Configurações Específicas de Ataque")]
    public bool isRangedUnit = false;
    [SerializeField] private string meleeAttackAnimationTrigger = "AttackTrigger"; // Nome do trigger no Animator
    [SerializeField] private string rangedAttackAnimationTrigger = "RangedAttackTrigger"; // Nome do trigger no Animator

    [Header("Configurações Gerais")]
    public BehaviorState currentState { get; private set; }
    private int facingDirection = 1;

    // MÉTODO PÚBLICO PARA O SPAWNER CONFIGURAR O CAMINHO
    public void SetPath(Transform[] Path)
    {
        activePath = Path;
        currentPathIndex = 0; // Começa do início do novo caminho
        if (activePath != null && activePath.Length > 0)
        {
            currentWaypoint = activePath[0];
            // Se o inimigo já está na cena e não está em combate, inicia o seguimento do caminho
            if (currentState != BehaviorState.ChasingPlayer && currentState != BehaviorState.Attacking)
            {
                SwitchToPathFollowing();
            }
        }
        else
        {
            Debug.LogError(gameObject.name + ": Tentativa de definir um caminho (path) nulo ou vazio via SetPath(). O inimigo pode não se mover.");
            currentWaypoint = null;
        }
    }

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
        
        if (!isRangedUnit)
        {
            enemyCombat = GetComponent<Enemy_Combat>();
            if (enemyCombat == null)
            {
                Debug.LogWarning("Componente Enemy_Combat não encontrado em " + gameObject.name + " (necessário para melee).");
            }
        }
        
        if (isRangedUnit)
        {
            enemyShootingHandler = GetComponent<EnemyShooting>(); // Pega se não arrastado no Inspector
            if (enemyShootingHandler == null)
            {
                Debug.LogError("Componente EnemyShooting não encontrado ou não atribuído em " + gameObject.name + " (necessário para ranged).");
            }
        }
    }

    void Start()
    {
        // O caminho idealmente é definido por SetPath() logo após a instanciação pelo EnemySpawner.
        // Se 'activePath' não foi definido, o inimigo não terá um caminho para seguir inicialmente.
        if (activePath == null || activePath.Length == 0)
        {
            Debug.LogWarning(gameObject.name + ": Caminho (activePath) não definido no Start. Chame SetPath() após instanciar. O inimigo ficará parado ou em estado de erro.");
            // Poderia definir um estado de erro ou Idle aqui. Por enquanto, tentará SwitchToPathFollowing que falhará graciosamente.
        }
        SwitchToPathFollowing(); // Tenta iniciar no caminho (que deve ter sido setado)
        timeSinceLastAttack = attackCooldown; // Permite atacar assim que possível
    }

    void Update()
    {
        if (isPerformingAttack) return; // Se está na animação de ataque, não processa outros updates de estado

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
                // Se o jogador saiu do alcance ENQUANTO o inimigo estava neste ESTADO LÓGICO de ataque 
                // (mas NÃO durante a execução da animação 'isPerformingAttack')
                if (playerTransform == null || 
                    Vector2.Distance(GetAttackOriginPoint(), playerTransform.position) > (isRangedUnit ? chaseDetectionRange : attackRange * 1.2f) )
                {
                    SwitchToReturningToPath();
                }
                // A decisão de re-atacar ou não é feita em AnimationEvent_AttackFinished
                break;
        }
        UpdateAnimationStates();
    }

    void FixedUpdate()
    {
        if (currentState == BehaviorState.Attacking || isPerformingAttack)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero; // CORRIGIDO para rb.linearVelocity
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
                    float distanceToPlayer = Vector2.Distance(GetAttackOriginPoint(), playerTransform.position);
                    float currentEngagementRange = attackRange; 

                    if (distanceToPlayer > currentEngagementRange)
                    {
                        MoveTowards(playerTransform, chaseSpeed);
                    }
                    else 
                    {
                        if (rb != null) rb.linearVelocity = Vector2.zero; // CORRIGIDO para rb.linearVelocity
                        // A decisão de atacar é feita no Update (UpdateChasingPlayerState)
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
    
    Vector3 GetAttackOriginPoint() {
        // Usa o attackPoint do Enemy_Combat se configurado, senão o centro do inimigo
        if (enemyCombat != null && enemyCombat.attackPoint != null) {
            return enemyCombat.attackPoint.position;
        }
        return transform.position;
    }


    void SwitchToPathFollowing()
    {
        currentState = BehaviorState.PathFollowing;
        isPerformingAttack = false;
        playerTransform = null;

        if (activePath != null && activePath.Length > 0)
        {
            // Tenta encontrar o waypoint mais próximo no activePath ou continuar do atual
            bool findClosest = true;
            if (currentPathIndex >= 0 && currentPathIndex < activePath.Length && currentWaypoint != null)
            {
                // Verifica se currentWaypoint realmente pertence ao activePath atual
                bool waypointInCurrentPath = false;
                for(int i=0; i < activePath.Length; ++i) { if(activePath[i] == currentWaypoint) { waypointInCurrentPath = true; break;}}

                if (waypointInCurrentPath && Vector2.Distance(transform.position, currentWaypoint.position) < chaseDetectionRange * 0.5f) { // Heurística
                    findClosest = false; 
                }
            }

            if (findClosest || currentWaypoint == null) { // Se precisa encontrar o mais próximo ou currentWaypoint é nulo/inválido
                 float shortestDistance = Mathf.Infinity;
                int closestPathIndex = 0;
                for (int i = 0; i < activePath.Length; i++)
                {
                    if (activePath[i] == null) continue;
                    float distance = Vector2.Distance(transform.position, activePath[i].position);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        closestPathIndex = i;
                    }
                }
                currentPathIndex = closestPathIndex;
            }
            currentPathIndex = Mathf.Clamp(currentPathIndex, 0, activePath.Length - 1);
            currentWaypoint = activePath[currentPathIndex];
        }
        else
        {
            // Debug.LogWarning(gameObject.name + ": activePath não está configurado para SwitchToPathFollowing!");
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
        // A lógica de encontrar o waypoint para o retorno está em SwitchToPathFollowing
        SwitchToPathFollowing();
    }
    
    void SwitchToAttackingState()
    {
        currentState = BehaviorState.Attacking;
        timeSinceLastAttack = 0f; // Reseta o cooldown para o PRÓXIMO ataque começar a contar DEPOIS deste
        isPerformingAttack = true; // Indica que a animação de ataque começou
        
        if (rb != null) rb.linearVelocity = Vector2.zero; // CORRIGIDO para rb.linearVelocity

        string triggerToUse = isRangedUnit ? rangedAttackAnimationTrigger : meleeAttackAnimationTrigger;
        if (anim != null && !string.IsNullOrEmpty(triggerToUse))
        {
            anim.SetTrigger(triggerToUse);
        }
        else
        {
             Debug.LogWarning(gameObject.name + ": Tentando atacar mas Animator ou Trigger de animação não configurado. Tipo: " + (isRangedUnit ? "Ranged" : "Melee"));
             isPerformingAttack = false; // Falha ao tentar iniciar o ataque visual
             currentState = BehaviorState.ChasingPlayer; // Volta a perseguir se não pode animar o ataque
        }
    }

    void UpdatePathFollowingState()
    {
        if (currentWaypoint == null)
        {
            if (activePath == null || activePath.Length == 0) {
                // Debug.LogWarning(gameObject.name + ": Impossível seguir caminho em Update, activePath não configurado.");
                return; 
            }
            // Tenta se recuperar caso currentWaypoint tenha se perdido mas o caminho existe
            currentPathIndex = Mathf.Clamp(currentPathIndex, 0, activePath.Length -1);
            currentWaypoint = activePath[currentPathIndex];
            if (currentWaypoint == null) { 
                Debug.LogError(gameObject.name + ": Waypoint no activePath é nulo. PathIndex: " + currentPathIndex);
                //currentState = BehaviorState.ReturningToPath; // Ou alguma outra forma de erro
                return;
            }
        }

        float distanceToWaypointSqr = (transform.position - currentWaypoint.position).sqrMagnitude;
        if (distanceToWaypointSqr <= 0.2f * 0.2f) 
        {
            currentPathIndex++; 

            if (currentPathIndex >= activePath.Length)
            {
                // 🔴 CHEGOU NO FINAL DA ROTA
                // 1. Perde vida do jogador
                PlayerLivesManager vidas = FindFirstObjectByType<PlayerLivesManager>();
                if (vidas != null)
                {
                    vidas.PerderVida();
                }

                // 2. Encerra a contagem da onda
                FindFirstObjectByType<EnemySpawner>().OnEnemyDied();

                // 3. Destroi o inimigo
                Destroy(gameObject);
                return;
            }

            currentWaypoint = activePath[currentPathIndex]; 
            if (currentWaypoint == null) {
                 Debug.LogError(gameObject.name + ": Próximo waypoint no activePath é nulo. PathIndex: " + currentPathIndex);
                 //currentState = BehaviorState.ReturningToPath;
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

        if ((playerTransform.position.x > transform.position.x && facingDirection == -1) || 
            (playerTransform.position.x < transform.position.x && facingDirection == 1)) 
        {
            FlipSprite(); 
        }

        float distanceToPlayer = Vector2.Distance(GetAttackOriginPoint(), playerTransform.position);
        if (distanceToPlayer <= attackRange && timeSinceLastAttack >= attackCooldown && !isPerformingAttack)
        {
            SwitchToAttackingState();
        }
        else if (distanceToPlayer > chaseDetectionRange && !isPerformingAttack) 
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
        float distanceToWaypointSqr = (transform.position - currentWaypoint.position).sqrMagnitude; 
        if (distanceToWaypointSqr <= 0.1f * 0.1f) 
        {
            SwitchToPathFollowing(); 
        }
    }

    void MoveTowards(Transform target, float speed)
    {
        if (target == null || rb == null) return; 

        Vector2 direction = ((Vector2)target.position - rb.position).normalized; 
        rb.linearVelocity = direction * speed; // CORRIGIDO para rb.linearVelocity

        if (Mathf.Abs(direction.x) > 0.05f) // Ajustada pequena tolerância para flip
        {
            if (direction.x > 0 && facingDirection == -1) FlipSprite();
            else if (direction.x < 0 && facingDirection == 1) FlipSprite();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
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
            // Se o jogador que saiu é o que estava sendo perseguido E não estamos no meio da animação de um ataque
            if (playerTransform == collision.transform && !isPerformingAttack && 
                (currentState == BehaviorState.ChasingPlayer || currentState == BehaviorState.Attacking))
            {
                 SwitchToReturningToPath(); 
            }
        }
    }

    void FlipSprite()
    {
        facingDirection *= -1; 
        Vector3 currentScale = transform.localScale;
        currentScale.x = Mathf.Abs(currentScale.x) * facingDirection;
        transform.localScale = currentScale; 
    }

    void UpdateAnimationStates()
    {
        if (anim == null) return; 

        // 'isMoving' é verdadeiro se há velocidade E não está no meio de uma animação de ataque E não está no estado de ataque lógico
        bool isActuallyMoving = (rb != null ? rb.linearVelocity.sqrMagnitude > 0.05f : false) && !isPerformingAttack && currentState != BehaviorState.Attacking;
        anim.SetBool("isMoving", isActuallyMoving); 
        
        // 'isChasing' pode ser usado para uma animação de corrida mais agressiva se diferente de 'isMoving' normal
        anim.SetBool("isChasing", currentState == BehaviorState.ChasingPlayer && isActuallyMoving);
        
        // A animação de ataque em si é disparada por um Trigger (meleeAttackAnimationTrigger ou rangedAttackAnimationTrigger).
        // Não precisamos de um booleano "isAttacking" contínuo se os triggers controlam o início
        // e 'AnimationEvent_AttackFinished' controla o fim do 'isPerformingAttack'.
        // Se o seu Animator State Machine *precisa* de um bool "isAttacking" para o estado de ataque,
        // você pode definir: anim.SetBool("isAttacking", currentState == BehaviorState.Attacking || isPerformingAttack);
    }

    // Animation Event para o golpe do ataque Melee
    public void AnimationEvent_MeleeHit()
    {
        if (isRangedUnit || currentState != BehaviorState.Attacking || !isPerformingAttack || playerTransform == null) return;
        if (enemyCombat == null) {
            Debug.LogError(gameObject.name + ": EnemyCombat é nulo para unidade melee em AnimationEvent_MeleeHit.");
            return;
        }
        enemyCombat.ApplyConfiguredDamage(); // Chama o método do Enemy_Combat
    }

    // Animation Event para disparar o projétil (Ranged Attack)
    public void AnimationEvent_FireRangedProjectile()
    {
        if (!isRangedUnit || currentState != BehaviorState.Attacking || !isPerformingAttack) return;
        if (enemyShootingHandler == null) {
            Debug.LogError(gameObject.name + ": EnemyShootingHandler é nulo para unidade ranged em AnimationEvent_FireRangedProjectile.");
            return;
        }
        enemyShootingHandler.PerformShoot();
    }

    // Animation Event chamado ao final de QUALQUER animação de ataque (melee ou ranged)
    public void AnimationEvent_AttackFinished()
    {
        isPerformingAttack = false; // Libera o Update para mudar de estado
        timeSinceLastAttack = 0f; // Reseta o cooldown aqui, pois o ataque acabou de terminar.

        // Decide o que fazer depois do ataque
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(GetAttackOriginPoint(), playerTransform.position);
            
            // Verifica se pode atacar novamente (se estiver no alcance E cooldown permitir)
            // A variável timeSinceLastAttack foi resetada para 0, então >= attackCooldown será falso
            // a menos que attackCooldown seja 0. O cooldown efetivamente começa AGORA.
            // Então, após um ataque, ele SEMPRE tentará perseguir ou retornar ao caminho primeiro.
            // A decisão de um NOVO ataque ocorrerá no UpdateChasingPlayerState quando o cooldown terminar.

            if (distanceToPlayer <= chaseDetectionRange) // Se o jogador ainda está detectável
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