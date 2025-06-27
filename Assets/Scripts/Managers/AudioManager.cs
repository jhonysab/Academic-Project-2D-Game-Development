// Arquivo: AudioManager.cs
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Padrão Singleton: Garante que só exista um AudioManager no jogo inteiro.
    public static AudioManager instance;

    private AudioSource musicSource;

    void Awake()
    {
        // Lógica do Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // A MÁGICA ACONTECE AQUI! Não destrói ao carregar nova cena.
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Se outro já existe, este é destruído.
        }

        // Pega o componente AudioSource para tocarmos a música
        musicSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Ao iniciar, carrega as configurações salvas pelo jogador
        LoadAudioSettings();
    }

    // --- Funções para serem chamadas pelo Menu de Opções ---

    public void ChangeMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void ToggleMusic(bool isMuted)
    {
        musicSource.mute = isMuted;
    }

    private void LoadAudioSettings()
    {
        // Carrega o volume salvo. Se não houver nada salvo, usa 1 (volume máximo) como padrão.
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        musicSource.volume = musicVolume;

        // Carrega o estado de mute. Se não houver, usa 0 (desmutado) como padrão. 1 = true, 0 = false.
        bool musicMuted = PlayerPrefs.GetInt("musicMuted", 0) == 1;
        musicSource.mute = musicMuted;
    }
}