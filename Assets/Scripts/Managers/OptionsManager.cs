// Arquivo: OptionsManager.cs
using UnityEngine;
using UnityEngine.UI; // Essencial para controlar Slider e Toggle

public class OptionsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider volumeSlider;
    public Toggle muteToggle;

    void Start()
    {
        // Configura o estado inicial da UI para refletir as configurações salvas
        LoadAndApplySettings();
    }

    // --- Funções para serem chamadas pelos eventos da UI ---

    public void OnVolumeSliderChanged()
    {
        // Pega o valor do slider e manda para o AudioManager
        float volume = volumeSlider.value;
        AudioManager.instance.ChangeMusicVolume(volume);

        // Salva a nova configuração
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void OnMuteToggleChanged()
    {
        // Pega o estado do toggle (marcado/desmarcado) e manda para o AudioManager
        bool isMuted = muteToggle.isOn;
        AudioManager.instance.ToggleMusic(isMuted);

        // Salva a nova configuração (1 para true, 0 para false)
        PlayerPrefs.SetInt("musicMuted", isMuted ? 1 : 0);
    }

    private void LoadAndApplySettings()
    {
        // Carrega as configurações salvas e atualiza os componentes da UI
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        volumeSlider.value = musicVolume;

        bool musicMuted = PlayerPrefs.GetInt("musicMuted", 0) == 1;
        muteToggle.isOn = musicMuted;
    }
}