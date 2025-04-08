using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] public AudioSource sfxAudio, musicAudio;
    [SerializeField] public AudioMixer Master;

    [Header("Música de Fondo")]
    public AudioClip mainMenuMusic;
    public AudioClip GeneralMusic, nivel2, nivel3;

    /*
     * Evitar cargar el manager con efectos de sonido. El manager debe encargarse de reproducir lo que le mandan, por eso los métodos son públicos.
     * Colocar los efectos de sonido aquí dificulta reproducir lo que cada objeto necesita reproducir y carga el script con demasiadas variables
     * - Santiago
     */


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        TriggerNivel.AdvanceLevel += (flag) => AdvanceMusic(flag);
    }

    private void OnDisable()
    {
        
    }

    private void Start()
    {
        InitialValues();
    }


    /// <summary>
    /// Reproducir efecto de sonido sin ubicación espacial
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySFX(AudioClip clip)
    {
        sfxAudio.PlayOneShot(clip);
    }

    /// <summary>
    /// Reproducir efecto de sonido con ubicación espacial. Requiere que el objeto tenga su propia fuente de sonido configurada en 3D
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="source"></param>
    public void PlaySFXDirectional(AudioClip clip, AudioSource source)
    {
        source.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicAudio.clip == clip) return; // Evita reiniciar la misma música

        // 🔥 REAPLICAR los valores guardados del volumen
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 20f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0f);
        float savedMaster = PlayerPrefs.GetFloat("GeneralVolume", 0f);

        MusicVolumeControl(savedMusic);
        SFXVolumeControl(savedSFX);
        GeneralVolumeControl(savedMaster);

        // 🎵 Reproducir la música
        musicAudio.Stop();
        musicAudio.clip = clip;
        musicAudio.Play();
        musicAudio.loop = true;
    }



    public void MusicVolumeControl(float volume)
    {
        Master.SetFloat("Musica", volume);
    }

    public void SFXVolumeControl(float volume)
    {
        Master.SetFloat("SFX", volume);
    }
    public void GeneralVolumeControl(float volume)
    {
        Master.SetFloat("Master", volume);
    }

    private void PlayBGM(int index)
    {
        switch (index)
        {
            case 1:
                PlayMusic(mainMenuMusic);
                break;
            case 2:
                PlayMusic(nivel2);
                break;
            case 3:
                PlayMusic(nivel3);
                break;
            default:
                PlayMusic(mainMenuMusic);
                break;
        }
    }

    public void ReiniciarBGM()
    {
        PlayBGM(0);
    }

    private void AdvanceMusic(GameFlags flag)
    {
        if (flag == GameFlags.EnterLevel3)
            PlayBGM(3);
        else if (flag == GameFlags.EnterLevel2)
            PlayBGM(2);
        else
            PlayBGM(1);
    }

    private void InitialValues()
    {
        GeneralVolumeControl(0);
        PlayerPrefs.SetFloat("GeneralVolume", 0);
        MusicVolumeControl(0);
        PlayerPrefs.SetFloat("MusicVolume", 0);
        SFXVolumeControl(0);
        PlayerPrefs.SetFloat("SFXVolume", 0);
    }
}
