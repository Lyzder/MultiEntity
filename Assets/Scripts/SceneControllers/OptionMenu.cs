using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;
public class OptionMenu : MonoBehaviour
{
    
    //Estos son los sliders que se agregan en el inspector
    [SerializeField] public Slider sliderMusic, sliderSFX, sliderGeneral;

    [SerializeField] private Toggle muteToggle;         // Toggle para silenciar/desilenciar
    private float previousVolumeMusic;
    private float previousVolumeSFX;
    void Awake()
    {
        
    }
    void Start()
    {
        SyncSlidersWithMixer(); // 👈 muy importante: sincroniza sin disparar eventos
                                // ✅ Restaurar estado del muteToggle sin disparar el evento
        bool wasMuted = PlayerPrefs.GetInt("MuteToggle", 0) == 1;
        muteToggle.SetIsOnWithoutNotify(wasMuted);
        OnMuteToggleChanged(wasMuted); // Aplicar lógica de mute manualment

        sliderMusic.onValueChanged.AddListener(UpdateMusicVolume);
        sliderSFX.onValueChanged.AddListener(UpdateSFXVolume);
        sliderGeneral.onValueChanged.AddListener(UpdateGeneralVolume);
        muteToggle.onValueChanged.AddListener(OnMuteToggleChanged);
    }
    void SyncSlidersWithMixer()
    {
        float musicVol, sfxVol, masterVol;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Master.GetFloat("Musica", out musicVol);
            AudioManager.Instance.Master.GetFloat("SFX", out sfxVol);
            AudioManager.Instance.Master.GetFloat("Master", out masterVol);

            sliderMusic.SetValueWithoutNotify(musicVol);
            sliderSFX.SetValueWithoutNotify(sfxVol);
            sliderGeneral.SetValueWithoutNotify(masterVol);

            // También actualizamos los valores "previos" por si mute está activo
            previousVolumeMusic = musicVol;
            previousVolumeSFX = sfxVol;
        }
    }

    public void OnMuteToggleChanged(bool isMuted)
         
    {PlayerPrefs.SetInt("MuteToggle", isMuted ? 1 : 0); // ✅ Guardar como 1 o 0
        if (isMuted)
        {
            // Guardamos los valores actuales antes de silenciar
            previousVolumeMusic = sliderMusic.value;
            previousVolumeSFX = sliderSFX.value;

            // Silenciamos
            AudioManager.Instance.MusicVolumeControl(-80f);
            AudioManager.Instance.SFXVolumeControl(-80f);
            sliderMusic.interactable = false;
            sliderSFX.interactable = false;
        }
        else
        {
            // Restauramos volúmenes anteriores
            AudioManager.Instance.MusicVolumeControl(previousVolumeMusic);
            AudioManager.Instance.SFXVolumeControl(previousVolumeSFX);
            sliderMusic.value = previousVolumeMusic;
            sliderSFX.value = previousVolumeSFX;
            sliderMusic.interactable = true;
            sliderSFX.interactable = true;
        }
    }
    public void UpdateGeneralVolume(float value)
    {
        if (sliderGeneral != null)
        {
            AudioManager.Instance.GeneralVolumeControl(value);
            PlayerPrefs.SetFloat("GeneralVolume", value);
        }
    }
    //Este metodo se encargan de Actualizar el volumen de la música
    public void UpdateMusicVolume(float value)
    {
        
        if (sliderMusic != null)
        {
            AudioManager.Instance.MusicVolumeControl(value); // Actualizamos el volumen de la música
            PlayerPrefs.SetFloat("MusicVolume", value);
        }
    }

    //Este metodo se encargan de Actualizar el volumen de los efectos de sonido
    public void UpdateSFXVolume(float value)
    {
        if (sliderSFX != null)
        {
            AudioManager.Instance.SFXVolumeControl(value);
            PlayerPrefs.SetFloat("SFXVolume", value);

        }
    }
 

    
   
  
    public void clicSound()
    {
        //Reemplzar la siguiente linea de código la palabra GeneralMusic por la cancion que se desea reproducir
        //AudioManager.Instance.PlayMusic(AudioManager.Instance.GeneralMusic);
    }

    public void exitMenu()
    {
        PlayerPrefs.Save();
        SceneManager.UnloadSceneAsync("OptionMenu");
    }

}
