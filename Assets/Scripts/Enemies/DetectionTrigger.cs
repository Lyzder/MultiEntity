using UnityEngine;

public class DetectionTrigger : MonoBehaviour

{
    [SerializeField] Boss owner;
    public bool canStealth;

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
        PlayerController player;

        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerController>();
            if (canStealth && player.isSneak)
                return;
            owner.TriggerAgro(player);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerController player;

        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerController>();
            if (canStealth && player.isSneak)
                return;
            owner.TriggerAgro(player);
        }
    }
}
