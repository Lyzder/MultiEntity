using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : InteractableBase
{
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
        if (player.personaActiva != 1)
        {
            
        }
        else
        {
            //TODO
            player.StartPush(gameObject);
        }
    }
}
