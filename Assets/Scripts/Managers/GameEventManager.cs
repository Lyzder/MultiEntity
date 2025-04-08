using UnityEngine;


/*
 *  Clase para almacenar y controlar eventos y banderas para el flujo del juego. Las banderas pueden ser eventos que ocurran u objetos que se consigan
 */
[System.Flags]
public enum GameFlags
{
    None = 0,
    HasNote1Room1 = 1 << 0,  // 1
    HasNote2Room1 = 1 << 1,  // 2
    HasKeyRoom1 = 1 << 2,   // 4
    OpenDoorRoom1 = 1 << 3,  // 8
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

        originalGlowColor = highlightMaterial.GetColor("_GlowColor");

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
}