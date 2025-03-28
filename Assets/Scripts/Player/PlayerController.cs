using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using System.Collections;


public class PlayerController : MonoBehaviour
{
    // Físicas y colisiones
    private Rigidbody rb;
    [Header("Colission")]
    [SerializeField] float rangeSweepCast;

    // Estado del jugador
    private bool isAlive;
    //private short personaActiva;

    // Movement variables
    private Vector2 moveInput;
    private bool isSprint;
    [Header("Movement speed")]
    public float moveSpeed = 4f;
    public float sprintMultiplier = 2.1f;

    // Input
    private InputSystem_Actions inputActions;
    private Renderer playerRenderer;
    private Color originalColor;

    private Color currentKeyColor;
    private bool hasKeyColor = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //personaActiva = 0;
        // Initialize Input Actions
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        // Subscribe to input events
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid leakages
        inputActions.Disable();
        inputActions.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled -= ctx => moveInput = Vector2.zero;
        inputActions.Player.Sprint.performed -= OnSprint;
        inputActions.Player.Sprint.canceled -= OnSprint;
    }

    // Start is called before the first frame update
    void Start()
    {
        EnableControl();
        playerRenderer = GetComponent<Renderer>();
        originalColor = playerRenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
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
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        float currentSpeed = isSprint ? moveSpeed * sprintMultiplier : moveSpeed;

        Vector3 desiredVelocity = moveDirection * currentSpeed;
        desiredVelocity.y = rb.velocity.y; // Preserve vertical movement

        // Check if movement will result in a collision
        if (rb.SweepTest(moveDirection, out RaycastHit hit, rangeSweepCast))
        {
            // If collision detected, slide along the obstacle's surface
            Vector3 slideDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal);
            desiredVelocity = slideDirection.normalized * currentSpeed;
            desiredVelocity.y = rb.velocity.y; // Preserve vertical movement
        }

        rb.velocity = desiredVelocity;
    }


    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSprint = true; // Sprint key is held down
        }
        else if (context.canceled)
        {
            isSprint = false; // Sprint key is released
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Llaves"))
        {
            Renderer llaveRenderer = other.gameObject.GetComponent<Renderer>();
            if (llaveRenderer != null)
            {
                currentKeyColor = llaveRenderer.material.color;
                playerRenderer.material.color = currentKeyColor;
                hasKeyColor = true;
            }
        }

        if (other.gameObject.CompareTag("Puerta"))
        {
            if (hasKeyColor && playerRenderer.material.color == currentKeyColor)
            {
                Renderer puertaRenderer = other.gameObject.GetComponent<Renderer>();
                if (puertaRenderer != null)
                {
                    StartCoroutine(CambiarColorPuertaTemporal(puertaRenderer, currentKeyColor, 5f));
                }
            }
        }
    }

    



    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Llaves"))
        {   StartCoroutine(CambiarColorTemporal(5f)); // 5 segundos
            
            
        }
        
    }
    private IEnumerator CambiarColorPuertaTemporal(Renderer puertaRenderer, Color nuevoColor, float duracion)
    {
        Color colorOriginal = puertaRenderer.material.color;
        puertaRenderer.material.color = nuevoColor;
        yield return new WaitForSeconds(duracion);
        puertaRenderer.material.color = colorOriginal;
    }



    private IEnumerator CambiarColorTemporal(float duracion)
    {
        
        yield return new WaitForSeconds(duracion);
        playerRenderer.material.color = originalColor;
    }
}

