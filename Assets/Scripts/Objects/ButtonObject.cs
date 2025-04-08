using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonObject : InteractableBase
{
    [SerializeField] GameFlags flag;
    public AudioClip clickSfx;

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
        AudioManager.Instance.PlaySFX(clickSfx);
        GameEventManager.Instance.SetFlag(flag);
    }
}
