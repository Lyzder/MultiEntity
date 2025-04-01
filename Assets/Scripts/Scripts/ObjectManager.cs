using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using System.Collections;
using UnityEngine.UI;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }
    public bool PlayerCercano_a_Nota1 = false;
    public bool PlayerCercano_a_Nota2 = false;
    public bool tieneNota1 = false;
    public bool tieneNota2 = false;
    public bool hasKey = false;


    [SerializeField] public Toggle ToggleNota1, ToggleNota2;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    { Debug.Log(tieneNota1);
        ToggleNota1.isOn = tieneNota1;
        ToggleNota2.isOn = tieneNota2;
        ToggleNota1.interactable = false;
        ToggleNota2.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(tieneNota1 == true)
        { ToggleNota1.isOn = true; }
        if (tieneNota1 == false)
        { ToggleNota1.isOn = false; }


        if (tieneNota2 == true)
        { ToggleNota2.isOn = true; }
        if(tieneNota2 == false)
        { ToggleNota2.isOn = false; }
    }
}
