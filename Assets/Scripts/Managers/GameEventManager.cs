using UnityEngine;


/*
 *  Clase para almacenar y controlar eventos y banderas para el flujo del juego. Las banderas pueden ser eventos que ocurran u objetos que se consigan
 */
[System.Flags]
public enum GameFlags
{
    None = 0,
    HasKeyRoom1 = 1 << 0,   // 1
    OpenDoorRoom1 = 1 << 1,  // 2
    ButtonRoom2 = 1 << 2, // 4
    CodeRoom2 = 1 << 3, // 8
    OpenDoor1Room2 = 1 << 4, // 16
    OpenDoor2Room2 = 1 << 5, // 32
    OpenDoorRoom3 = 1 << 6, // 64
    CodeRoom3 = 1 << 7, // 128
    EnemyRoom3 = 1 << 8, // 256
}

public class GameEventManager : MonoBehaviour
{
    [SerializeField] Material highlightMaterial;
    private static Color originalGlowColor;

    public Color highlightGlowColor = new(128f, 122f, 0f);

    public static GameEventManager Instance { get; private set; }
    private GameFlags flags;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        originalGlowColor = new(0, 0, 0);

        // Subscribe to the highlight event
        PlayerController.OnHighlightToggle += SetHighlight;
    }

    private void OnDestroy()
    {
        PlayerController.OnHighlightToggle -= SetHighlight;
    }

    /// <summary>
    /// Levantar bandera
    /// </summary>
    /// <param name="flag"></param>
    public void SetFlag(GameFlags flag)
    {
        flags |= flag;
    }

    /// <summary>
    /// Revisar si bandera está levantada
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    public bool HasFlag(GameFlags flag)
    {
        return (flags & flag) == flag;
    }

    /// <summary>
    /// Bajar bandera
    /// </summary>
    /// <param name="flag"></param>
    public void ClearFlag(GameFlags flag)
    {
        flags &= ~flag;
    }

    /// <summary>
    /// Bajar todas las banderas
    /// </summary>
    public void ClearAllFlags()
    {
        flags = GameFlags.None;
    }

    private void SetHighlight(bool isHighlighted)
    {
        highlightMaterial.SetColor("_GlowColor", isHighlighted ? highlightGlowColor : originalGlowColor);
    }

    public void ResetHighlight()
    {
        highlightMaterial.SetColor("_GlowColor", originalGlowColor);
    }
}