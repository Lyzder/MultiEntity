using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorObject : InteractableBase
{
    [SerializeField] List<GameFlags> condiciones;

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
        bool cumple = true;

        foreach(GameFlags cond in condiciones)
        {
            if (!GameEventManager.Instance.HasFlag(cond))
            {
                cumple = false;
                break;
            }
        }
        if (cumple)
        {
            // TODO abrir puerta
        }
        else
        {
            // TODO puerta no se abre
        }
    }
}
