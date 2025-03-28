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
    public short estadoJugador { get; private set; }

    // Movement variables
    private Vector2 moveInput;
    private bool isSprint;
    private bool isSneak;
    private bool isObserve;
    private bool lookLeft;
    [Header("Movement speed")]
    public float moveSpeed = 4f;
    public float sprintMultiplier = 2f;
    public float sneakMultiplier = 0.65f;

    // Input
    private InputSystem_Actions inputActions;

    // Entorno
    [Header("Interaccion")]
    [SerializeField] BoxCollider frontalTrigger;
    [SerializeField] SpriteRenderer topSprite;
    private List<InteractableBase> objectsInRange;

    // Efectos de sonido
    [Header("Efectos de sonido")]
    public AudioClip stepSfx;

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
        topSprite.enabled = false;
        objectsInRange = new List<InteractableBase>();

        // Initialize Input Actions
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        // Subscribe to input events
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        inputActions.Player.Sprint.performed += OnSkill;
        inputActions.Player.Sprint.canceled += OnSkill;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid leakages
        inputActions.Disable();
        inputActions.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled -= ctx => moveInput = Vector2.zero;
        inputActions.Player.Sprint.performed -= OnSkill;
        inputActions.Player.Sprint.canceled -= OnSkill;
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

        if (isSprint)
            currentSpeed = moveSpeed * sprintMultiplier;
        else if (isSneak)
            currentSpeed = moveSpeed * sneakMultiplier;
        else
            currentSpeed = moveSpeed;

        Vector3 desiredVelocity = moveDirection * currentSpeed;
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
    }

    private void OnInteract(InputAction.CallbackContext context)
    {

    }

    public void ChangePersona(int personaId)
    {
        Debug.Log("Cambio");
        personaActiva = (short)personaId;
        animator.SetInteger("PersonaActiva", personaId);
    }

    private void RotateSprite()
    {
        spriteRenderer.flipX = !lookLeft;
        if (lookLeft)
        {
            frontalTrigger.center = new Vector3(-0.7f, 0f, -0.3f);
            return;
        }
        frontalTrigger.center = new Vector3(0.7f, 0f, -0.3f);
    }

    private void ShowCanInteract()
    {
        if (objectsInRange.Count > 0) 
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
        objectsInRange.Add(obj);
    }

    public void ObjectOutOfRange(InteractableBase obj)
    {
        objectsInRange.Remove(obj);
    }

    private void StartPush(GameObject obj)
    {
        obj.transform.SetParent(transform);
    }
}
