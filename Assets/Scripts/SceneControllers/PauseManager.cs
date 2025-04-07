using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public GameObject menuPause;
    public bool pausedGame = false;


    void Update()
    {
      PressKeySpaceBar();
    }

    void TogglePause()
    {
        pausedGame = !pausedGame;
        Time.timeScale = pausedGame ? 0 : 1;
    }


    public void ExitPause()

    {
        menuPause.SetActive(false);
        Time.timeScale = 1;
        pausedGame = false;
    }

    public void PressKeySpaceBar()// función para pausar el juego con la barra espaciadora

    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Pause();
        }
    }

    public void Pause()
    {
        menuPause.SetActive(true);
        Time.timeScale = 0;
        pausedGame = true;
    }

    public void LoadScene(string name)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(name);
    }

    public void ReloadScene()
    {
        Time.timeScale = 1;
        GameManager.Instance.ReloadCurrentScene();
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Nivel1");
    }
}

