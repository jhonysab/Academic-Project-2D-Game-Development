using UnityEngine;

public class TowerShooting : MonoBehaviour
{
    [Header("Configurações do Disparo")]
    
    // O atributo [SerializeField] faz com que variáveis privadas apareçam no Inspector do Unity.
    // É aqui que você vai arrastar o prefab da sua flecha.
    [SerializeField] private GameObject projectilePrefab;
    
    // Este é o Transform (objeto filho) de onde a flecha será criada.
    [SerializeField] private Transform firePoint;

    // Método público que será chamado pelo script principal da torre (NpcAttackController)
    public void PerformShoot()
    {
        // Uma verificação para garantir que tudo foi configurado antes de tentar atirar
        if (projectilePrefab != null && firePoint != null)
        {
            // Cria uma instância do prefab do projétil na posição e rotação do firePoint
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.LogWarning("Prefab do projétil ou FirePoint não configurado no componente TowerShooting em " + gameObject.name);
        }
    }
}