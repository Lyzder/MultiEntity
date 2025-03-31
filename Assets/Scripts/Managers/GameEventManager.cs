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
    }

    public void SetFlag(GameFlags flag)
    {
        flags |= flag;
    }

    public bool HasFlag(GameFlags flag)
    {
        return (flags & flag) == flag;
    }

    public void ClearFlag(GameFlags flag)
    {
        flags &= ~flag;
    }
}