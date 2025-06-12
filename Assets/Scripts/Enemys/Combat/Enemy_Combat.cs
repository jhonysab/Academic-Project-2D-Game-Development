using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    [Tooltip("Dano que o ataque causa. Aumente este valor para o Boss!")]
    public int damageAmount = 5; 
    [Tooltip("O 'ponto de contato' do ataque. Crie um objeto filho no Boss e arraste aqui.")]
    public Transform attackPoint;
    [Tooltip("O raio de alcance do golpe a partir do Attack Point.")]
    public float weaponRange = 1.0f;
    [Tooltip("A camada (Layer) do jogador.")]
    public LayerMask playerLayer;

    // Método público que será chamado pelo BossAI através de um Animation Event
    public void ApplyConfiguredDamage()
    {
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint não configurado em " + gameObject.name + ". O ataque não causará dano.");
            return;
        }

        // Detecta todos os colisores do jogador dentro da área de ataque
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);

        // Causa dano a todos os jogadores encontrados (geralmente será apenas um)
        foreach (Collider2D hitPlayerCollider in hits)
        {
            Player_Health playerHealth = hitPlayerCollider.GetComponent<Player_Health>();
            if (playerHealth != null)
            {
                Debug.Log(gameObject.name + " acertou " + hitPlayerCollider.name + " com dano: " + damageAmount);
                // Dano é passado como um número negativo para o método do jogador
                playerHealth.ChangeHealth(-damageAmount);
            }
        }
    }
}