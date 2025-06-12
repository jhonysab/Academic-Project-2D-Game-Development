using UnityEngine;

public class TowerShooting : MonoBehaviour
{
    [Header("Configurações do Disparo")]
    [Tooltip("O prefab da flecha que será disparado.")]
    [SerializeField] private GameObject projectilePrefab;
    
    [Tooltip("O Transform (objeto filho) de onde a flecha efetivamente sai.")]
    [SerializeField] private Transform firePoint;

    // Método público chamado pelo NpcAttackController
    public void PerformShoot(Quaternion shotRotation)
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // Usa a posição do firePoint, mas a ROTAÇÃO que foi passada pelo NpcAttackController
            Instantiate(projectilePrefab, firePoint.position, shotRotation);
        }
        else
        {
            Debug.LogWarning("Prefab do projétil ou FirePoint não configurado no TowerShooting.");
        }
    }
}