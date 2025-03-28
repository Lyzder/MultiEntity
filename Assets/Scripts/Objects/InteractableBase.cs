using UnityEngine;


/*
 * Esta clase está diseñada para servir como base de los distintos objetos con los que el jugador puede interactuar
 */

public class InteractableBase : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Interact()
    {
        Debug.Log("Interactuado");
    }
}
