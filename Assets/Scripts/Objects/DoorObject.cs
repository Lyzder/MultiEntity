using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorObject : InteractableBase
{
    [SerializeField] List<GameFlags> condiciones;
    [SerializeField] GameFlags eventoPuerta;
    private Animator animator;
    private Collider trigger;
    private bool abierta;
    public AudioClip openSfx;
    public bool autoOpen;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        trigger = GetComponent<Collider>();
        abierta = false;
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
        if (autoOpen && !abierta && RevisarCondiciones())
            AbrirPuerta();
    }

    public override void Interact(PlayerController player)
    {
        if (RevisarCondiciones())
        {
            // TODO abrir puerta
            DeactivateTrigger();
            player.ObjectOutOfRange(this);
            AbrirPuerta();
        }
        else
        {
            // TODO puerta no se abre
        }
    }

    private bool RevisarCondiciones()
    {
        bool cumple = true;

        foreach (GameFlags cond in condiciones)
        {
            if (!GameEventManager.Instance.HasFlag(cond))
            {
                cumple = false;
                break;
            }
        }

        return cumple;
    }

    private void AbrirPuerta()
    {
        animator.SetBool("IsOpen", true);
        GameEventManager.Instance.SetFlag(eventoPuerta);
        AudioManager.Instance.PlaySFX(openSfx);
        abierta = true;
    }

    private void DeactivateTrigger()
    {
        trigger.enabled = false;
    }
}
