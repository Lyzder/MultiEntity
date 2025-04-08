using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }
    private bool isPaused;
    [SerializeField] GameObject playerPrefab;

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
    // nameScene: Nombre de la escena
    public void LoadSceneByName(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }

    public void ReloadCurrentScene()
    {
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
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }
    public void OpenOptionsMenu()
    {
        SceneManager.LoadScene("OptionMenu", LoadSceneMode.Additive);
    }

    public void OpenCreditsMenu()
    {
        SceneManager.LoadScene("CreditsMenu", LoadSceneMode.Additive);
    }

    public void TransitionPoint(string sceneName, Vector3 spawnPosition, PlayerController player)
    {
        StartCoroutine(LoadSceneAndSpawnPlayer(sceneName, spawnPosition, player));
    }

    private IEnumerator LoadSceneAndSpawnPlayer(string sceneName, Vector3 spawnPosition, PlayerController oldPlayer)
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
        player.GetComponent<PlayerController>().ForceTransition(oldPlayer.personaActiva);
    }

}
/*Forma de utilizar funciones en otros scripts, llamar escenas por nombres
GameManager.instance.LoadSceneByName("Menu") */