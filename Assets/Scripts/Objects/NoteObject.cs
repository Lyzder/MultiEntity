using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : InteractableBase
{
    [SerializeField] bool tieneSecreto;

    // Start is called before the first frame update
    void Start()
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
        }
        else
        {
            // TODO leer normal
            Debug.Log("Leer la nota");
        }
    }
}
