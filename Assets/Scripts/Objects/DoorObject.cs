using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorObject : InteractableBase
{
    [SerializeField] List<GameFlags> condiciones;
    [SerializeField] GameFlags eventoPuerta;
    private Animator animator;
    private Collider trigger;
    public AudioClip openSfx;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        trigger = GetComponent<Collider>();
    }

    private void Start()
    {
        if (GameEventManager.Instance.HasFlag(eventoPuerta))
        {
            animator.SetBool("IsOpen", true);
            DeactivateTrigger();
        }
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
            animator.SetBool("IsOpen", true);
            GameEventManager.Instance.SetFlag(eventoPuerta);
            DeactivateTrigger();
            player.ObjectOutOfRange(this);
            AudioManager.Instance.PlaySFX(openSfx);
        }
        else
        {
            // TODO puerta no se abre
        }
    }

    private void DeactivateTrigger()
    {
        trigger.enabled = false;
    }
}
