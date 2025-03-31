using UnityEngine;

[System.Flags]
public enum GameFlags
{
    None = 0,
    HasNote1Room1 = 1 << 0,  // 1
    HasNote2Room1 = 1 << 1,  // 2
    HasKeyRoom1 = 1 << 1,   // 4
}

public class GameEventManager : MonoBehaviour
{
    [SerializeField] Material highlightMaterial;
    private Color originalGlowColor;
    public Color highlightGlowColor = Color.yellow;

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

    private void SetHighlight(bool isHighlighted)
    {
        // Change the glow color in the shader
        highlightMaterial.SetColor("_GlowColor", isHighlighted ? highlightGlowColor : originalGlowColor);
    }
}