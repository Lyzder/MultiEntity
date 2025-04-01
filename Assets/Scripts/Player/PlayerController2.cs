using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerController2 : MonoBehaviour
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
        if (Keyboard.current.eKey.wasPressedThisFrame && ObjectManager.Instance.PlayerCercano_a_Nota1)
        {
            ObjectManager.Instance.tieneNota1 = true;
            ObjectManager.Instance.ToggleNota1.isOn = true;
            

        }
        if (Keyboard.current.eKey.wasPressedThisFrame && ObjectManager.Instance.PlayerCercano_a_Nota2)
        {
            ObjectManager.Instance.tieneNota2 = true;
            ObjectManager.Instance.ToggleNota2.isOn = true;


        }
        originalColor = playerRenderer.material.color;

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
                ObjectManager.Instance.hasKey = true;
            }
        }

        if (other.gameObject.CompareTag("Nota1"))
        {
            //Debug.Log("Entraste");
            ObjectManager.Instance.PlayerCercano_a_Nota1 = true;
        }
        if (other.gameObject.CompareTag("Nota2"))
        {
            
            ObjectManager.Instance.PlayerCercano_a_Nota2 = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Nota1"))
        {
            //Debug.Log("Saliste");
            ObjectManager.Instance.PlayerCercano_a_Nota1 = false;
        }
        if (other.gameObject.CompareTag("Nota2"))
        {
            //Debug.Log("Saliste");
            ObjectManager.Instance.PlayerCercano_a_Nota2 = false;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Puerta"))
        {
           

            if (ObjectManager.Instance.hasKey && ObjectManager.Instance.tieneNota1 && ObjectManager.Instance.tieneNota2)
            {
                Renderer puertaRenderer = collision.gameObject.GetComponent<Renderer>();
                if (puertaRenderer != null)
                { 
                    SceneManager.LoadScene(6);

                    puertaRenderer.material.color = currentKeyColor;
                }
            }
        }

        if(collision.gameObject.CompareTag("Puerta2"))
        {
            Renderer puertaRenderer = collision.gameObject.GetComponent<Renderer>();
            if (ObjectManager.Instance.hasKey && ObjectManager.Instance.tieneNota1 && ObjectManager.Instance.tieneNota2)
            {
                playerRenderer.material.color = originalColor;
                puertaRenderer.material.color = originalColor;
            }
        }
        





    }




    

}

