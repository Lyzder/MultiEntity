using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxAudio, musicAudio;
    [SerializeField] private AudioMixer Master;

    [Header("Música de Fondo")]
    public AudioClip mainMenuMusic;
    public AudioClip GeneralMusic;

    [Header("Efectos de sonido")]
    public AudioClip jumpSound;
    public AudioClip Collision_Bala_and_Obstacle_Sound;
    public AudioClip VictorySound;
    public AudioClip DefeatSound;
    public AudioClip Walking;


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
        sfxAudio = transform.GetChild(0).GetComponent<AudioSource>();
        musicAudio = transform.GetChild(1).GetComponent<AudioSource>();

    }


    public void PlaySFX(AudioClip clip)
    {
        sfxAudio.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicAudio.clip == clip) return; // Evita reiniciar la misma música

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
}
