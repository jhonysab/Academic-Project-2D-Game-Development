// Arquivo: ShopButtonInfo.cs

using UnityEngine;

// Coloque este script em cada BOTÃO da sua loja na cena.
public class ShopButtonInfo : MonoBehaviour
{
    [Header("Informações para Compra")]
    [Tooltip("Arraste o Asset da Torre (TowerBlueprint) que este botão irá comprar.")]
    public TowerBlueprint towerBlueprint;

    [Tooltip("Arraste o GameObject da CENA que serve como local de construção para esta torre.")]
    public Transform spawnPoint;

    // Este método será chamado pelo OnClick() do botão.
    public void OnPurchaseButtonPressed()
    {
        if (towerBlueprint == null || spawnPoint == null)
        {
            Debug.LogError("Botão da loja não configurado corretamente! Verifique o Blueprint e o Spawn Point.", this.gameObject);
            return;
        }

        // Encontra o ShopManager na cena e chama o método de compra,
        // passando a si mesmo como fonte de informação.
        ShopManager.main.PurchaseTower(this);
    }
}