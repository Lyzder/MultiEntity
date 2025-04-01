using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManagement : MonoBehaviour
{
    public void exitCredits()
    {
        PlayerPrefs.Save();
        SceneManager.UnloadSceneAsync("CreditsMenu");
    }
    public void clicSound()
    {   
        //Reemplzar la siguiente linea de código la palabra GeneralMusic por la cancion que se desea reproducir
        //AudioManager.Instance.PlayMusic(AudioManager.Instance.GeneralMusic);
    }
}
