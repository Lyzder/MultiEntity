using UnityEngine;
using UnityEngine.UI;

public class MenuManagement : MonoBehaviour
{  
    
    void Start()
    {

        //Reemplzar la siguiente linea de código la palabra GeneralMusic por la cancion que se desea reproducir
        //AudioManager.Instance.PlayMusic(AudioManager.Instance.GeneralMusic);

    }

    public void clicSound()
    {
        //Reemplzar la siguiente linea de código la palabra GeneralMusic por la cancion que se desea reproducir
        //AudioManager.Instance.PlayMusic(AudioManager.Instance.GeneralMusic);
    }

    public void OpenOptions() 
    {
        GameManager.Instance.OpenOptionsMenu();
    }
  

  

    

    public void OpenCredits()
    {
        GameManager.Instance.OpenCreditsMenu();
    }

    public void NextLevel()
    {
        GameManager.Instance.LoadNextScene();
    }

    public void GoToLevel(string nivel)
    {
        GameManager.Instance.LoadSceneByName(nivel);
    }

}
