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
            image.sprite = mensajeSecreto;
        }
        else
        {
            image.sprite = mensaje;
        }
        OpenPanel();
        player.StartReading(this);
    }

    public override void OpenPanel()
    {
        image.SetNativeSize();
        canvas.SetActive(true);
    }

    public override void ClosePanel()
    {
        canvas.SetActive(false);
    }
}
