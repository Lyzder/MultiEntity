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
        previousVolumeMusic = sliderMusic.value;
        previousVolumeSFX = sliderSFX.value;
        sliderMusic.onValueChanged.AddListener(UpdateMusicVolume);
        sliderSFX.onValueChanged.AddListener(UpdateSFXVolume);
        sliderGeneral.onValueChanged.AddListener(UpdateGeneralVolume);
        muteToggle.onValueChanged.AddListener(OnMuteToggleChanged);


    }
    public void OnMuteToggleChanged(bool isMuted)
    {
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
        }
    }
    //Este metodo se encargan de Actualizar el volumen de la música
    public void UpdateMusicVolume(float value)
    {
        
        if (sliderMusic != null)
        {
            AudioManager.Instance.MusicVolumeControl(value); // Actualizamos el volumen de la música
        }
    }

    //Este metodo se encargan de Actualizar el volumen de los efectos de sonido
    public void UpdateSFXVolume(float value)
    {
        if (sliderSFX != null)
        {
            AudioManager.Instance.SFXVolumeControl(value);
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
