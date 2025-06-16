// Arquivo: ShopManager.cs

using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager main;

    private void Awake()
    {
        main = this;
    }

    // MÉTODO MODIFICADO: Agora ele recebe as informações do botão.
    public void PurchaseTower(ShopButtonInfo buttonInfo)
    {
        Debug.LogWarning("--- O MÉTODO PurchaseTower FOI CHAMADO! ---");
        // Pega as informações de dentro do botão
        TowerBlueprint planta = buttonInfo.towerBlueprint;
        Transform pontoDeSpawn = buttonInfo.spawnPoint;

        if (planta == null)
        {
            Debug.LogError("Tentativa de comprar um item com uma planta nula!");
            return;
        }

        if (pontoDeSpawn == null)
        {
            Debug.LogError("O botão da loja não tem um 'Ponto De Spawn' configurado!");
            return;
        }

        // A checagem de ocupado continua a mesma
        if (pontoDeSpawn.childCount > 0)
        {
            Debug.Log("O local para a " + planta.nomeDaTorre + " já está ocupado!");
            return;
        }

        // A verificação de dinheiro continua a mesma
        if (PlayerCurrency.main.PodePagar(planta.custo))
        {
            PlayerCurrency.main.GastarMoeda(planta.custo);

            GameObject torreInstanciada = Instantiate(planta.prefabDaTorre, pontoDeSpawn.position, pontoDeSpawn.rotation);
            torreInstanciada.transform.SetParent(pontoDeSpawn);

            Debug.Log(planta.nomeDaTorre + " comprada e construída com sucesso!");

            // --- IMPLEMENTAÇÃO ADICIONADA ---
            // Fecha a janela da loja após a compra ser bem-sucedida.
            ShopController.main.FecharLoja();
        }
        else
        {
            Debug.Log("Dinheiro insuficiente para comprar " + planta.nomeDaTorre);
        }
        
    }
    
}