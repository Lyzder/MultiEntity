using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class OptionMenu : MonoBehaviour
{
    
    //Estos son los sliders que se agregan en el inspector
    [SerializeField] public Slider sliderMusic, sliderSFX;

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
        InitializeToggleState();
    }
    //Este metodo se encargan de Actualizar el volumen de la música
    public void UpdateMusicVolume()
    {
        if (sliderSFX != null)
        {
            AudioManager.Instance.SFXVolumeControl(sliderSFX.value);
        }
    }

    //Este metodo se encargan de Actualizar el volumen de los efectos de sonido
    public void UpdateSFXVolume()
    {
        if (sliderMusic != null)
        {
            AudioManager.Instance.MusicVolumeControl(sliderMusic.value);
        }
    }
    void InitializeToggleState()
    {
        //muteToggle.isOn = false;
        if (muteToggle.isOn)
        {
            AudioManager.Instance.MusicVolumeControl(-80f);
            AudioManager.Instance.SFXVolumeControl(-80f);
        }
        if (muteToggle.isOn == false)
        {
            AudioManager.Instance.MusicVolumeControl(previousVolumeMusic);
            AudioManager.Instance.SFXVolumeControl(previousVolumeSFX);
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
