// PlayerController.cs
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5;
    public Rigidbody2D rb;
    private int facingDirection = 1;

    [Header("Animação")]
    public Animator anim;

    [Header("Ataque Normal")]
    [SerializeField] private KeyCode attackKey = KeyCode.J; // Tecla para ataque normal (ex: J)
    [SerializeField] private float attackDamage = 1f;
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private string playerAttackAnimationTrigger = "PlayerAttack";
    private float timeSinceLastAttack = 0f;

    [Header("Ataque Forte")]
    [SerializeField] private KeyCode strongAttackKey = KeyCode.K; // Tecla para ataque forte (ex: K)
    [SerializeField] private float strongAttackDamage = 2.5f; // Dano maior para o ataque forte
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
    }

    void Update()
    {
        // Cooldowns
        if (timeSinceLastAttack < attackCooldown)
        {
            timeSinceLastAttack += Time.deltaTime;
        }
        if (timeSinceLastStrongAttack < strongAttackCooldown)
        {
            timeSinceLastStrongAttack += Time.deltaTime;
        }

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
        if (rb == null) return;
        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        rb.linearVelocity = movement * speed;
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
        timeSinceLastAttack = 0f;

        ApplyDamageToEnemies(attackDamage);
    }

    void PerformStrongAttack()
    {
        if (anim != null && !string.IsNullOrEmpty(playerStrongAttackAnimationTrigger))
        {
            anim.SetTrigger(playerStrongAttackAnimationTrigger);
        }
        timeSinceLastStrongAttack = 0f;

        ApplyDamageToEnemies(strongAttackDamage); // Usa o dano forte
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