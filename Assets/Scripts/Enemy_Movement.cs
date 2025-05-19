using System.Numerics;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform player;

    private bool isChasing; // Variável para controlar se o inimigo está perseguindo o jogador
    public float speed = 4f; // Velocidade do inimigo
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isChasing == true)
        {
            UnityEngine.Vector2 direction = player.position - transform.position;
            rb.linearVelocity = direction.normalized * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (player == null)
            { player = collision.transform; } // Armazena a referência do jogador
            else
            { player = collision.transform; } // Atualiza a referência do jogador
            isChasing = true; // Inicia a perseguição quando o inimigo colide com o jogador
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rb.linearVelocity = UnityEngine.Vector2.zero; // Para o movimento do inimigo quando o jogador sai da colisão
            isChasing = false; // Para a perseguição quando o inimigo sai da colisão com o jogador
        }
    }

}
