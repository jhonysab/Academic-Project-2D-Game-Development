using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public int damageAmount = 1;
    public Transform attackPoint; // Ponto de onde o "golpe" do ataque se origina
    public float weaponRange = 0.5f; // Alcance do golpe a partir do attackPoint
    public LayerMask playerLayer;

    // Método público para realizar a checagem de acerto e aplicar dano
    public void ApplyConfiguredDamage() // Renomeei de PerformActualHit para consistência com o que o EnemyCombinedMovement está chamando
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("AttackPoint não configurado em " + gameObject.name + " (Enemy_Combat).");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);

        foreach (Collider2D hitPlayerCollider in hits)
        {
            Player_Health playerHealth = hitPlayerCollider.GetComponent<Player_Health>();
            if (playerHealth != null)
            {
                Debug.Log(gameObject.name + " acertou " + hitPlayerCollider.name + " com dano: " + damageAmount);
                playerHealth.ChangeHealth(-damageAmount);
                return; // Aplica dano ao primeiro jogador encontrado e sai
            }
        }
    }
}