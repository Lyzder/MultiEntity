using UnityEngine;

public class PickableObject : InteractableBase
{
    [SerializeField] GameFlags itemFlag;

    // Start is called before the first frame update
    void Start()
    {
        if (GameEventManager.Instance.HasFlag(itemFlag))
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameEventManager.Instance.HasFlag(itemFlag))
            Destroy(gameObject);
    }

    public override void Interact(PlayerController player)
    {
        //TODO
        GameEventManager.Instance.SetFlag(itemFlag);
        player.ObjectOutOfRange(this);
        Destroy(gameObject);
    }
}
