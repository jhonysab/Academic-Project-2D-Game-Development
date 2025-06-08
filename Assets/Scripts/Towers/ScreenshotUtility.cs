using UnityEngine;
using System.IO; // Necessário para manipulação de arquivos

public class ScreenshotUtility : MonoBehaviour
{
    // O atributo [ContextMenu(...)] adiciona um botão no menu de contexto do componente no Inspector.
    [ContextMenu("Capturar Screenshot Transparente")]
    public void TakeTransparentScreenshot()
    {
        // Pega a câmera principal
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Nenhuma câmera principal encontrada na cena!");
            return;
        }

        // Cria uma RenderTexture com as mesmas dimensões da tela e com canal alfa
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;

        // Cria uma textura 2D para receber os pixels
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);

        // Renderiza a câmera para a RenderTexture
        cam.Render();

        // Ativa a RenderTexture, lê os pixels e depois desativa
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        // Limpa as referências da câmera e da RenderTexture
        cam.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);

        // Converte a textura para um array de bytes em formato PNG
        byte[] bytes = screenShot.EncodeToPNG();

        // Define o caminho e o nome do arquivo
        string filename = Path.Combine(Application.dataPath, "MinhaTorre.png");

        // Salva o arquivo
        File.WriteAllBytes(filename, bytes);

        Debug.Log("Screenshot transparente salvo em: " + filename);
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh(); // Atualiza o editor para mostrar o novo arquivo
        #endif
    }
}