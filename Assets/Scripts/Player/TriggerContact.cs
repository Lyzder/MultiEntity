using UnityEngine;

public class TriggerContact : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] bool frontal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interact"))
        {
            player.ObjecInRange(other.GetComponent<InteractableBase>());
        }
        else if (frontal && other.CompareTag("Push"))
        {
            player.ObjecInRange(other.GetComponent<InteractableBase>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interact") || other.CompareTag("Push"))
        {
            player.ObjectOutOfRange(other.GetComponent<InteractableBase>());
        }
        else if (frontal && other.CompareTag("Push"))
        {
            player.ObjectOutOfRange(other.GetComponent<InteractableBase>());
        }
    }
}
