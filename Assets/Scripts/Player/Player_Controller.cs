using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // --- NOVO: Referência ao Player_Health ---
    [Header("Referências de Stats")]
    public Player_Health playerHealthRef; // << ARRASTE O GAMEOBJECT DO PLAYER AQUI NO INSPECTOR
    // --- FIM NOVO ---

    [Header("Movimento")]
    // REMOVIDO: public float baseMoveSpeed = 5; // << REMOVIDO: A VELOCIDADE FINAL VIRÁ DE PLAYER_HEALTH
    public Rigidbody2D rb;
    private int facingDirection = 1;

    [Header("Animação")]
    public Animator anim;

    [Header("Ataque Normal")]
    [SerializeField] private KeyCode attackKey = KeyCode.J; // Tecla para ataque normal (ex: J)
    // REMOVIDO: [SerializeField] private float baseAttackDamage = 1f; // << REMOVIDO: O DANO FINAL VIRÁ DE PLAYER_HEALTH
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private string playerAttackAnimationTrigger = "PlayerAttack";
    private float timeSinceLastAttack = 0f;

    [Header("Ataque Forte")]
    [SerializeField] private KeyCode strongAttackKey = KeyCode.K; // Tecla para ataque forte (ex: K)
    // REMOVIDO: [SerializeField] private float baseStrongAttackDamage = 2.5f; // << REMOVIDO: O DANO FINAL VIRÁ DE PLAYER_HEALTH
    [SerializeField] private float strongAttackCooldown = 1.5f; // Cooldown maior para o ataque forte
    [SerializeField] private string playerStrongAttackAnimationTrigger = "PlayerStrongAttack"; // Trigger diferente para animação de ataque forte
    private float timeSinceLastStrongAttack = 0f;


    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
        // Garante que os cooldowns permitam o primeiro ataque
        timeSinceLastAttack = attackCooldown;
        timeSinceLastStrongAttack = strongAttackCooldown;

        // --- NOVO: Obtém a referência ao Player_Health se não estiver atribuída ---
        if (playerHealthRef == null)
        {
            playerHealthRef = GetComponent<Player_Health>(); // Tenta pegar no próprio GameObject
            if (playerHealthRef == null)
            {
                Debug.LogError("PlayerController: Player_Health não atribuído ou encontrado no GameObject do Player!", this);
            }
        }
        // --- FIM NOVO ---
    }

    void Update()
    {
        // Cooldowns
        timeSinceLastAttack += Time.deltaTime;
        timeSinceLastStrongAttack += Time.deltaTime;

        // --- NOVO: Verifica se playerHealthRef é válido antes de prosseguir ---
        if (playerHealthRef == null) return; 
        // --- FIM NOVO ---

        // Inputs de Ataque
        if (Input.GetKeyDown(attackKey) && timeSinceLastAttack >= attackCooldown)
        {
            PerformNormalAttack();
        }
        else if (Input.GetKeyDown(strongAttackKey) && timeSinceLastStrongAttack >= strongAttackCooldown)
        {
            PerformStrongAttack();
        }

        // Movimento
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        HandleMovement(horizontal, vertical);
        HandleAnimation(horizontal, vertical);
        HandleFlip(horizontal);
    }

    void FixedUpdate()
    {
        // A velocidade é definida em HandleMovement, chamada de Update
    }

    void HandleMovement(float horizontal, float vertical)
    {
        // Se rb ou playerHealthRef não existirem, o movimento não ocorrerá
        if (rb == null || playerHealthRef == null) return; 
        Vector2 movement = new Vector2(horizontal, vertical).normalized;

        // --- MUDANÇA CRÍTICA: USA CurrentMoveSpeed de Player_Health ---
        float totalSpeed = playerHealthRef.CurrentMoveSpeed; // << PEGA A VELOCIDADE FINAL DO PLAYER_HEALTH (BASE + BÔNUS POÇÃO)
        rb.linearVelocity = movement * totalSpeed;
        // --- FIM MUDANÇA ---
    }

    void HandleAnimation(float horizontal, float vertical)
    {
        if (anim == null) return;
        anim.SetFloat("horizontal", Mathf.Abs(horizontal));
        anim.SetFloat("vertical", Mathf.Abs(vertical));
    }

    void HandleFlip(float horizontalInput)
    {
        if (horizontalInput > 0 && facingDirection == -1)
        {
            Flip();
        }
        else if (horizontalInput < 0 && facingDirection == 1) 
        {
            Flip();
        }
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    void PerformNormalAttack()
    {
        if (anim != null && !string.IsNullOrEmpty(playerAttackAnimationTrigger))
        {
            anim.SetTrigger(playerAttackAnimationTrigger);
        }

        // --- MUDANÇA CRÍTICA: USA CurrentDamage de Player_Health ---
        float finalAttackDamage = playerHealthRef.CurrentDamage; // << PEGA O DANO FINAL DO PLAYER_HEALTH (BASE + BÔNUS POÇÃO)
        // --- FIM MUDANÇA ---

        timeSinceLastAttack = 0f;

        ApplyDamageToEnemies(finalAttackDamage);
    }

    void PerformStrongAttack()
    {
        if (anim != null && !string.IsNullOrEmpty(playerStrongAttackAnimationTrigger))
        {
            anim.SetTrigger(playerStrongAttackAnimationTrigger);
        }
        timeSinceLastStrongAttack = 0f;

        // --- MUDANÇA CRÍTICA: USA CurrentDamage de Player_Health ---
        // Se Strong Attack tem um multiplicador diferente, aplique-o aqui:
        float strongAttackMultiplier = 1.5f; // Exemplo: ataque forte causa 1.5x o dano normal
        float finalStrongAttackDamage = playerHealthRef.CurrentDamage * strongAttackMultiplier; // << PEGA O DANO FINAL E APLICA MULTIPLICADOR
        // --- FIM MUDANÇA ---

        ApplyDamageToEnemies(finalStrongAttackDamage); 
    }

    void ApplyDamageToEnemies(float damageToApply)
    {
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint não está atribuído no PlayerController!");
            return;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayerMask);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            EnemyHealth enemyHealth = enemyCollider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                Debug.Log("Jogador acertou: " + enemyCollider.name + " com dano: " + damageToApply);
                enemyHealth.TakeDamage(damageToApply);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}