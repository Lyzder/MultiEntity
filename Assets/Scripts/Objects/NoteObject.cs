using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteObject : InteractableBase
{
    [SerializeField] bool tieneSecreto;
    [SerializeField] Sprite mensaje;
    [SerializeField] Sprite mensajeSecreto;
    [SerializeField] GameObject canvas;
    [SerializeField] Image image;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact(PlayerController player)
    {
        if (tieneSecreto && player.personaActiva == 2)
        {
            // TODO leer como inteligente
            Debug.Log("Leer como inteligente");
            image.sprite = mensajeSecreto;
        }
        else
        {
            // TODO leer normal
            Debug.Log("Leer la nota");
            image.sprite = mensaje;
        }
        image.SetNativeSize();
        canvas.SetActive(true);
        player.StartReading(this);
    }

    public void CloseNote()
    {
        canvas.SetActive(false);
    }
}
