using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public float damageAmount = 0.5f; 

    // Novo método para ser chamado pelo EnemyCombinedMovement (via Animation Event)
    public void ApplyConfiguredDamage(Player_Health playerHealthTarget)
    {
        if (playerHealthTarget != null)
        {
            playerHealthTarget.ChangeHealth(-damageAmount); //
        }
    }
}