using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    // Control de sprites
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Físicas y colisiones
    private Rigidbody rb;
    private BoxCollider cldr;
    [Header("Colission")]
    [SerializeField] float rangeSweepCast;

    // Estado del jugador
    private bool isAlive;
    public short personaActiva { get; private set; } // 0: Default. 1: Dep. 2: Int
    public enum Estados : ushort
    {
        Defecto = 0,
        Daño = 1,
        Cambio = 2,
        Sigilo = 3,
        Empujar = 4,
        Leer = 5,
    }
    public Estados estadoJugador { get; private set; }
    public delegate void HighlightToggle(bool isHighlighted);
    public static event HighlightToggle OnHighlightToggle;

    // Movement variables
    private Vector2 moveInput;
    private bool isSprint;
    private bool isSneak;
    private bool isObserve;
    private bool lookLeft;
    [Header("Movement speed")]
    public float moveSpeed = 4f;
    public float sprintMultiplier = 2f;
    public float sneakMultiplier = 0.5f;
    public float pushMultiplier = 0.5f;

    // Input
    private InputSystem_Actions inputActions;

    // Entorno
    [Header("Interaccion")]
    [SerializeField] BoxCollider frontalTrigger;
    [SerializeField] SpriteRenderer topSprite;
    private List<InteractableBase> objectsInRange;
    private GameObject pushingObj;
    private NoteObject notaAbierta;

    // Efectos de sonido
    [Header("Efectos de sonido")]
    public AudioClip stepSfx;
    public AudioClip stepSoftSfx;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();
        cldr = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
        personaActiva = 0;
        isSprint = false;
        isSneak = false;
        isObserve = false;
        lookLeft = false;
        estadoJugador = 0;
        topSprite.enabled = false;
        objectsInRange = new List<InteractableBase>();

        // Initialize Input Actions
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        // Subscribe to input events
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Sprint.performed += OnSkill;
        inputActions.Player.Sprint.canceled += OnSkill;
        inputActions.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid leakages
        inputActions.Disable();
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Sprint.performed -= OnSkill;
        inputActions.Player.Sprint.canceled -= OnSkill;
        inputActions.Player.Interact.performed -= OnInteract;
    }

    // Start is called before the first frame update
    void Start()
    {
        EnableControl();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        ShowCanInteract();
    }

    public void EnableControl()
    {
        inputActions.Enable();
    }

    public void DisableControl()
    {
        inputActions.Disable();
    }

    private void Move()
    {
        float currentSpeed;
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        // Rotates sprite if there's horizontal movement
        if (moveDirection.x != 0)
        {
            lookLeft = moveDirection.x < 0;
            RotateSprite();
        }

        if (estadoJugador == Estados.Empujar)
            currentSpeed = moveSpeed * pushMultiplier;
        else if (isSprint)
        {
            currentSpeed = moveSpeed * sprintMultiplier;
            animator.SetBool("Habilidad", true);
        }
        else if (isSneak)
        {
            currentSpeed = moveSpeed * sneakMultiplier;
            animator.SetBool("Habilidad", true);
        }
        else
        {
            currentSpeed = moveSpeed;
            animator.SetBool("Habilidad", false);
        }

        Vector3 desiredVelocity = moveDirection * currentSpeed;
        animator.SetFloat("Velocidad", desiredVelocity.magnitude);
        desiredVelocity.y = rb.velocity.y; // Preserve vertical movement

        // Check if movement will result in a collision
        if (rb.SweepTest(moveDirection, out RaycastHit hit, rangeSweepCast) && !hit.collider.isTrigger)
        {
            // If collision detected, slide along the obstacle's surface
            Vector3 slideDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal);
            desiredVelocity = slideDirection.normalized * currentSpeed;
            desiredVelocity.y = rb.velocity.y; // Preserve vertical movement
        }

        rb.velocity = desiredVelocity;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed && !(estadoJugador == Estados.Daño || estadoJugador == Estados.Leer || estadoJugador == Estados.Cambio))
        {
            moveInput = context.ReadValue<Vector2>();
        }
        else
            moveInput = Vector2.zero;
    }

    private void OnSkill(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (personaActiva)
            {
                case 1:
                    isSprint = true;
                    isSneak = false;
                    isObserve = false;
                    break;
                case 2:
                    isSprint = false;
                    isSneak = false;
                    isObserve = true;
                    break;
                default:
                    isSprint = false;
                    isSneak = true;
                    isObserve = false;
                    break;
            }
        }
        else if (context.canceled)
        {
            isSprint = false;
            isSneak = false;
            isObserve = false;
        }
        UpdateAnimator();
        if (personaActiva == 2)
            OnHighlightToggle?.Invoke(isObserve);
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        InteractableBase obj;

        if (estadoJugador == Estados.Defecto)
        {
            obj = GetCloserObject();
            if (obj != null)
                obj.Interact(this);
        }
        else if (estadoJugador == Estados.Empujar)
        {
            StopPush();
        }
        else if (estadoJugador == Estados.Leer)
        {
            StopReading();
        }
    }

    public void ChangePersona(int personaId)
    {
        Debug.Log("Cambio");
        personaActiva = (short)personaId;
        animator.SetInteger("PersonaActiva", personaId);
    }

    private void RotateSprite()
    {
        spriteRenderer.flipX = lookLeft;
        if (lookLeft)
        {
            frontalTrigger.center = new Vector3(-0.7f, 0f, -0.3f);
            return;
        }
        frontalTrigger.center = new Vector3(0.7f, 0f, -0.3f);
    }

    private void ShowCanInteract()
    {
        if (estadoJugador == Estados.Daño || estadoJugador == Estados.Empujar)
        {
            topSprite.enabled = false;
        }
        else if (objectsInRange.Count > 0) 
        { 
            topSprite.enabled = true;
        }
        else
        {
            topSprite.enabled = false;
        }
    }

    public void ObjecInRange(InteractableBase obj)
    {
        if (objectsInRange.IndexOf(obj) < 0)
            objectsInRange.Add(obj);
    }

    public void ObjectOutOfRange(InteractableBase obj)
    {
        objectsInRange.Remove(obj);
    }

    public void StartPush(GameObject obj)
    {
        obj.transform.SetParent(transform);
        pushingObj = obj;
        estadoJugador = Estados.Empujar;
    }

    private void StopPush()
    {
        pushingObj.transform.SetParent(null);
        objectsInRange.Remove(pushingObj.GetComponent<InteractableBase>());
        pushingObj = null;
        estadoJugador = Estados.Defecto;
    }

    private InteractableBase GetCloserObject()
    {
        InteractableBase closeObj = null;
        float dist = 50f;
        foreach(InteractableBase obj in objectsInRange)
        {
            if (Vector3.Distance(obj.transform.position, transform.position) < dist)
            {
                dist = Vector3.Distance(obj.transform.position, transform.position);
                closeObj = obj;
            }
        }
        return closeObj;
    }

    public void PlaySound(int index)
    {
        switch (index)
        {
            case 0: // Reproduce paso
                AudioManager.Instance.PlaySFX(stepSfx);
                break;
            case 1: // Reproduce paso suave
                AudioManager.Instance.PlaySFX(stepSoftSfx);
                break;
            default:
                break;
        }
    }

    private void UpdateAnimator()
    {
        animator.SetInteger("PersonaActiva", personaActiva);
        animator.SetBool("Habilidad", isSneak || isSprint || isObserve);
    }

    public void StartReading(NoteObject nota)
    {
        estadoJugador = Estados.Leer;
        notaAbierta = nota;
    }

    private void StopReading()
    {
        if(estadoJugador == Estados.Leer)
        {
            estadoJugador = Estados.Defecto;
        }
        if (notaAbierta != null)
        {
            notaAbierta.CloseNote();
            notaAbierta = null;
        }
    }
}
