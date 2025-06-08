using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main; // Instância Singleton

    private TowerBlueprint torreParaConstruir; // A planta da torre que o jogador selecionou na loja
    

    void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método que a UI da loja chamará quando o jogador escolher uma torre
    public void SelecionarTorreParaConstruir(TowerBlueprint planta)
    {
        torreParaConstruir = planta;
        Debug.Log("Torre selecionada para construir: " + planta.nomeDaTorre);
        // Aqui você pode adicionar lógica para mudar o cursor do mouse, por exemplo
    }
    public bool TemTorreSelecionada()
{
    return torreParaConstruir != null;
}
    
    // Método que o TowerSlot chama quando é clicado
    public void ConstruirTorreNesteSlot(TowerSlot slot)
    {
        if (torreParaConstruir == null)
        {
            Debug.Log("Nenhuma torre selecionada para construir.");
            return;
        }

        // Verifica se o jogador tem dinheiro suficiente (requer um sistema de moeda)
        if (PlayerCurrency.main.PodePagar(torreParaConstruir.custo))
        {
            // Gasta o dinheiro
            PlayerCurrency.main.GastarMoeda(torreParaConstruir.custo);

            // Manda o slot construir a torre
            slot.ConstruirTorre(torreParaConstruir);

            // Desseleciona a torre para que o jogador não construa várias sem querer
            torreParaConstruir = null;
        }
        else
        {
            Debug.Log("Dinheiro insuficiente! Custo: " + torreParaConstruir.custo);
        }
    }
}