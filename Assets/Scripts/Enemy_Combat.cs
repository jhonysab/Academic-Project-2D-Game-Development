using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public float damage = 0.5f; // Dano que o inimigo causa

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player_Health player = collision.gameObject.GetComponent<Player_Health>();
        if (player != null)
        {
            player.ChangeHealth(-damage);
        }
    }
}
