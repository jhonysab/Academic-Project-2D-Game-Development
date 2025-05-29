using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public float damage = 0.5f; // Dano que o inimigo causa
    public Transform attackPoint; // Ponto de ataque do inimigo
    public float wepaonRange;
    public LayerMask playerLayer; // Camada do jogador para detectar colisões

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player_Health player = collision.gameObject.GetComponent<Player_Health>();
        if (player != null)
        {
            player.ChangeHealth(-damage);
        }
    }

    private void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, wepaonRange, playerLayer);

        if (hits.Length > 0)
        {
            hits[0].GetComponent<Player_Health>().ChangeHealth(-damage); // Aplica dano ao jogador
        }
    }
}
