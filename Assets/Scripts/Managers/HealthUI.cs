using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    private Image[] heartImages;

void Awake()
{
    heartImages = GetComponentsInChildren<Image>();

    // Filtro pras pegar as imagens, só pega o que tem heart
    heartImages = System.Array.FindAll(heartImages, img => img.name.StartsWith("Heart"));
}


    public void UpdateHearts(float currentHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            float heartValue = Mathf.Clamp(currentHealth - i, 0f, 1f);

            if (heartValue >= 1f)
            {
                heartImages[i].sprite = fullHeart;
            }
            else if (heartValue >= 0.5f)
            {
                heartImages[i].sprite = halfHeart;
            }
            else
            {
                heartImages[i].sprite = emptyHeart;
            }
        }
    }
}
