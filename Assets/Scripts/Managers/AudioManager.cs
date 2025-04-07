using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] public AudioSource sfxAudio, musicAudio;
    [SerializeField] public AudioMixer Master;

    [Header("Música de Fondo")]
    public AudioClip mainMenuMusic;
    public AudioClip GeneralMusic;

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
    }

    private void Start()
    {
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0f);
        float savedMaster = PlayerPrefs.GetFloat("GeneralVolume", 0f);

        MusicVolumeControl(savedMusic);
        SFXVolumeControl(savedSFX);
        GeneralVolumeControl(savedMaster);
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
}
