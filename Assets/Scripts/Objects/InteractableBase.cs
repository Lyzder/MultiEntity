using UnityEngine;


/*
 * Esta clase está diseñada para servir como base de los distintos objetos con los que el jugador puede interactuar
 */

public class InteractableBase : MonoBehaviour
{

    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Interact(PlayerController player)
    {
        Debug.Log("Interactuado");
    }

    public virtual void OpenPanel()
    {

    }

    public virtual void ClosePanel()
    {

    }
}
