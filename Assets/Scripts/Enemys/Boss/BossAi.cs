using UnityEngine;
using System.Linq;

public class BossAI : MonoBehaviour
{
    public enum BossState { FollowingPath, ChasingPlayer, Attacking }

    [Header("Referências")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private Enemy_Combat enemyCombat;

    [Header("Atributos de Movimento e Percepção")]
    [SerializeField] private float pathFollowSpeed = 1.0f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float chaseDetectionRange = 10f;
    
    [Header("Atributos de Combate")]
    [SerializeField] private float attackRange = 2.0f;
    [SerializeField] private float attackCooldown = 3.0f;
    [SerializeField] private string attackAnimationTrigger = "BossAttack";
    [SerializeField] private int damageOnPathEnd = 10;
    
    // --- Variáveis de Controle Interno ---
    public BossState currentState { get; private set; }
    private Transform[] activePath;
    private Transform currentWaypoint;
    private int currentPathIndex = 0;
    private Transform playerTransform;
    private float timeSinceLastAttack;
    private bool isPerformingAttack = false;
    private Vector2 lastLookDirection = Vector2.down; // Guarda a última direção

    // --- SETUP ---
    public void SetPath(Transform[] newPath)
    {
        activePath = newPath;
        currentPathIndex = 0;
        if (activePath != null && activePath.Length > 0)
        {
            currentWaypoint = activePath[0];
            SwitchToState(BossState.FollowingPath);
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyCombat = GetComponent<Enemy_Combat>();
    }

    void Start()
    {
        timeSinceLastAttack = attackCooldown;
        SwitchToState(BossState.FollowingPath);
    }
    
    // --- LÓGICA PRINCIPAL ---
    void Update()
    {
        if (isPerformingAttack) return; // Pausa a lógica se a animação de ataque está tocando
        
        timeSinceLastAttack += Time.deltaTime;
        
        // A máquina de estados agora é mais simples e direta
        FindPlayerInRange();
        
        if (playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) <= chaseDetectionRange)
        {
            // Se encontrou um jogador e ele está no alcance de perseguição
            if (Vector2.Distance(GetAttackOriginPoint(), playerTransform.position) <= attackRange && timeSinceLastAttack >= attackCooldown)
            {
                SwitchToState(BossState.Attacking);
            }
            else
            {
                SwitchToState(BossState.ChasingPlayer);
            }
        }
        else
        {
            // Se não há jogador ou ele está muito longe
            SwitchToState(BossState.FollowingPath);
        }

        UpdateAnimationParameters();
    }

    void FixedUpdate()
    {
        if (currentState == BossState.Attacking || isPerformingAttack)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Transform target = null;
        float speed = 0;
        switch (currentState)
        {
            case BossState.FollowingPath:
                target = currentWaypoint;
                speed = pathFollowSpeed;
                UpdatePathFollowingLogic();
                break;
            case BossState.ChasingPlayer:
                target = playerTransform;
                speed = chaseSpeed;
                break;
        }
        
        MoveTowards(target, speed);
    }

    // --- AÇÕES E TRANSIÇÕES DE ESTADO ---
    void SwitchToState(BossState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        
        if (newState == BossState.Attacking)
        {
            Attack();
        } else {
            isPerformingAttack = false;
        }
    }

    void Attack()
    {
        if (isPerformingAttack) return;
        timeSinceLastAttack = 0f;
        isPerformingAttack = true;
        anim.SetTrigger(attackAnimationTrigger);
    }
    
    // --- ATUALIZAÇÃO DE ANIMAÇÃO (A CORREÇÃO PRINCIPAL) ---
    void UpdateAnimationParameters()
    {
        if (anim == null) return;
        
        bool isMoving = rb.linearVelocity.sqrMagnitude > 0.05f;
        anim.SetBool("isBossMoving", isMoving);

        // A direção de olhar é sempre a prioridade.
        // Se há um jogador, olha para ele. Se não, olha para onde está andando.
        // Se estiver totalmente parado, mantém a última direção.
        if (playerTransform != null)
        {
            // Zona morta para evitar giros quando muito perto
            if (Vector2.Distance(transform.position, playerTransform.position) > 0.1f)
            {
                lastLookDirection = (playerTransform.position - transform.position).normalized;
            }
        }
        else if (isMoving)
        {
            lastLookDirection = rb.linearVelocity.normalized;
        }
        
        // Envia a direção correta e consistente para TODOS os Blend Trees
        anim.SetFloat("BossMoveX", lastLookDirection.x);
        anim.SetFloat("BossMoveY", lastLookDirection.y);
    }

    // --- Funções Auxiliares e de Eventos ---
    private void UpdatePathFollowingLogic() { if (currentWaypoint == null) { FindClosestWaypointOnPath(); return; } if ((transform.position - currentWaypoint.position).sqrMagnitude < 0.1f) { currentPathIndex++; if (currentPathIndex >= activePath.Length) { if(PlayerLivesManager.main != null) PlayerLivesManager.main.PerderVida(damageOnPathEnd); Destroy(gameObject); } else { currentWaypoint = activePath[currentPathIndex]; } } }
    private void MoveTowards(Transform target, float speed) { if (target != null && rb != null) { rb.linearVelocity = ((Vector2)target.position - rb.position).normalized * speed; } else if (rb != null) { rb.linearVelocity = Vector2.zero; } }
    private void FindPlayerInRange() { var hits = Physics2D.OverlapCircleAll(transform.position, chaseDetectionRange, LayerMask.GetMask("Player")); playerTransform = hits.OrderBy(h => Vector2.Distance(transform.position, h.transform.position)).FirstOrDefault()?.transform; }
    private void FindClosestWaypointOnPath() { if (activePath == null || activePath.Length == 0) { currentWaypoint = null; return; } float shortestDistance = float.MaxValue; int closestIndex = 0; for (int i = 0; i < activePath.Length; i++) { if (activePath[i] == null) continue; float dist = Vector2.Distance(transform.position, activePath[i].position); if (dist < shortestDistance) { shortestDistance = dist; closestIndex = i; } } currentPathIndex = closestIndex; currentWaypoint = activePath[closestIndex]; }
    private Vector3 GetAttackOriginPoint() { if (enemyCombat != null && enemyCombat.attackPoint != null) return enemyCombat.attackPoint.position; return transform.position; }
    public void AnimationEvent_MeleeHit() { if (enemyCombat != null) enemyCombat.ApplyConfiguredDamage(); }
    public void AnimationEvent_AttackFinished() { isPerformingAttack = false; }
    // As funções de Trigger e Habilidade Especial não precisam de mudanças
    private void UseSpecialAbility() { /* ... */ }
}