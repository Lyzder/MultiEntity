using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }
    private bool isPaused;
    [SerializeField] GameObject playerPrefab;
    public int playerLives;
    private Vector3 playerSpawn;
    private short spawnPersona;
    public static event Action PlayerLost;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnEnable()
    {
        PlayerController.PlayerDied += LoseLive;
    }

    private void OnDisable()
    {
        PlayerController.PlayerDied -= LoseLive;
    }

    // nameScene: Nombre de la escena
    public void LoadSceneByName(string nameScene)
    {
        GameEventManager.Instance.ResetHighlight();
        SceneManager.LoadScene(nameScene);
    }

    public void ReloadCurrentScene()
    {
        GameEventManager.Instance.ResetHighlight();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

   

    // Carga la siguiente escena por index
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Verifica si hay una siguiente escena
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No hay más escenas. ¡Has completado el juego!");
            ReloadCurrentScene(); // Recargar la primera escena
        }
    }

    public void PauseGame()
    {
        GameEventManager.Instance.ResetHighlight();
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }
    public void OpenOptionsMenu()
    {
        GameEventManager.Instance.ResetHighlight();
        SceneManager.LoadScene("OptionMenu", LoadSceneMode.Additive);
    }

    public void OpenCreditsMenu()
    {
        GameEventManager.Instance.ResetHighlight();
        SceneManager.LoadScene("CreditsMenu", LoadSceneMode.Additive);
    }

    public void TransitionPoint(string sceneName, Vector3 spawnPosition, PlayerController player)
    {
        GameEventManager.Instance.ResetHighlight();
        spawnPersona = player.personaActiva;
        playerSpawn = spawnPosition;
        StartCoroutine(LoadSceneAndSpawnPlayer(sceneName, spawnPosition));
    }

    private IEnumerator LoadSceneAndSpawnPlayer(string sceneName, Vector3 spawnPosition)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // After the scene is loaded
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            player = Instantiate(playerPrefab);

        player.transform.position = spawnPosition;
        player.GetComponent<PlayerController>().ForceTransition(spawnPersona);
    }

    private void LoseLive()
    {
        playerLives -= 1;
        if (playerLives > 0)
        {
            StartCoroutine(LoadSceneAndSpawnPlayer(SceneManager.GetActiveScene().name, playerSpawn));
        }
        else
        {
            PlayerLost?.Invoke();
        }
    }
}
/*Forma de utilizar funciones en otros scripts, llamar escenas por nombres
GameManager.instance.LoadSceneByName("Menu") */