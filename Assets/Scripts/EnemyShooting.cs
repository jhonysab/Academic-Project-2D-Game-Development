using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject bulletPrefab;    // Arraste seu prefab de projétil aqui
    public Transform bulletSpawnPoint; // Ponto de onde o projétil será disparado

    // Este método é público e deve ser chamado pelo EnemyCombinedMovement
    public void PerformShoot()
    {
        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            // Instancia o projétil na posição e rotação do bulletSpawnPoint.
            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        }
        else
        {
            Debug.LogError("Bullet Prefab ou Bullet Spawn Point não configurado no EnemyShooting de " + gameObject.name);
        }
    }
}