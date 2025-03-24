using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    // Movement variables
    private Vector2 moveInput;
    private bool isSprint;
    [Header("Movement speed")]
    public float moveSpeed = 4f;
    public float sprintMultiplier = 2.1f;

    // Input
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        inputActions.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled -= ctx => moveInput = Vector2.zero;
        inputActions.Player.Sprint.performed -= OnSprint;
        inputActions.Player.Sprint.canceled -= OnSprint;
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
        rb.velocity = isSprint ? moveDirection * moveSpeed * sprintMultiplier : moveDirection * moveSpeed;
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
}
