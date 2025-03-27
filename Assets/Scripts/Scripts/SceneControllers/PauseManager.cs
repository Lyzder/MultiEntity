using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject menuPause;
    public bool pausedGame = false;

    private void Update()

    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausedGame)

            {
                Restart();
            }
            
            else
            {
                Pause();
            }
        }

    }

    public void Restart()

    {
        menuPause.SetActive(false);
        Time.timeScale = 1;
        pausedGame = false;
    }

     public void Pause()

    {
        menuPause.SetActive(true);
        Time.timeScale = 0;
        pausedGame = true;
    }

}

