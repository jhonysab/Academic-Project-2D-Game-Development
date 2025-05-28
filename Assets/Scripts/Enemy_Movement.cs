using System.Numerics;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.XR;

public class Enemy_Movement : MonoBehaviour
{
    private Rigidbody2D rb; // Referência ao Rigidbody2D do inimigo
    private Transform player;  // Referência ao Transform do jogador
    private Animator anim;
    private EnemyState enemyState;

    public float speed = 4f; // Velocidade do inimigo

    private int facingDirection = -1; // Direção de movimento do inimigo, 1 para direita, -1 para esquerda
    void Start() // Inicializa o Rigidbody2D do inimigo
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // Obtém o componente Animator do inimigo
        ChangeState(EnemyState.Idle); // Define o estado inicial do inimigo como Idle
    }

    void Update() // Atualiza a posição do inimigo
    {
        if (enemyState == EnemyState.Chase)
        {
            if (player.position.x > transform.position.x && facingDirection == -1 ||
                player.position.x < transform.position.x && facingDirection == 1)
            {
                FlipTool();
            }
            
            UnityEngine.Vector2 direction = player.position - transform.position;
            rb.linearVelocity = direction.normalized * speed;
        }
    }

    void FlipTool() // Inverte a direção do inimigo
    {
    facingDirection *= -1; // Inverte a direção de movimento
    UnityEngine.Vector3 scale = transform.localScale; // Obtém a escala atual do inimigo
    scale.x = Mathf.Abs(scale.x) * facingDirection; // Garante que só inverta a direção sem alterar o tamanho
    transform.localScale = scale; // Aplica a nova escala ao inimigo
    }
    private void OnTriggerEnter2D(Collider2D collision) // Detecta a colisão com o jogador
    {
        if (collision.gameObject.tag == "Player")
        {
            if (player == null)
            {
                player = collision.transform; // Armazena a referência do jogador
            }
            else
            {
                player = collision.transform; // Atualiza a referência do jogador
            }
            ChangeState(EnemyState.Chase); // Altera o estado do inimigo para Chase
        }
    }

    void ChangeState(EnemyState newState) // Altera o estado do inimigo
    {
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", false); // Desativa a animação de Idle
        else if (enemyState == EnemyState.Chase)
            anim.SetBool("isChase", false);

        enemyState = newState; // Atualiza o estado do inimigo

        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", true); // Ativa a animação de Idle
        else if (enemyState == EnemyState.Chase)
            anim.SetBool("isChase", true); // Ativa a animação de Chase
    }

    private void OnTriggerExit2D(Collider2D collision) // Detecta quando o jogador sai da colisão com o inimigo
    {
        if (collision.gameObject.tag == "Player")
        {
            rb.linearVelocity = UnityEngine.Vector2.zero; // Para o movimento do inimigo quando o jogador sai da colisão
            ChangeState(EnemyState.Idle); // Altera o estado do inimigo para Idle
        }
    }
}

public enum EnemyState // Define os estados do inimigo
{
    Idle, // Inimigo parado
    Chase, // Inimigo perseguindo o jogador
    Attack // Inimigo atacando o jogador
}