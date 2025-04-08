using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Windows;
using System;

public class PlayerController : MonoBehaviour, IDamageable
{
    // Control de sprites
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField] Material highlightMaterial;
    private static Color originalGlowColor;
    public Color highlightGlowColor = new(128f, 122f, 0f);
    [SerializeField] private float highlightDuration = 0.5f;
    private Coroutine highlightCoroutine;

    // Físicas y colisiones
    private Rigidbody rb;
    private BoxCollider cldr;
    [Header("Colission")]
    [SerializeField] float rangeSweepCast;

    // Estado del jugador
    public bool isAlive { get; private set; }
    [SerializeField] ushort health;
    [SerializeField] ushort lives;
    public short personaActiva { get; private set; } // 0: Default. 1: Dep. 2: Int
    public enum Estados : ushort
    {
        Defecto = 0,
        Daño = 1,
        Cambio = 2,
        Sigilo = 3,
        Empujar = 4,
        Leer = 5,
        Ataque = 6,
        JuegoTerminado = 7,
    }
    public Estados estadoJugador { get; private set; }

    // Movement variables
    private Vector2 moveInput;
    private float speed;
    private bool isSprint;
    public bool isSneak { get; private set; }
    private bool isObserve;
    private bool lookLeft;
    [Header("Movement speed")]
    public float moveSpeed = 4f;
    public float sprintMultiplier = 2f;
    public float sneakMultiplier = 0.5f;
    public float pushMultiplier = 0.5f;

    // Input
    private InputSystem_Actions inputActions;
    private PlayerInput playerInput;

    // Entorno
    [Header("Interaccion")]
    [SerializeField] BoxCollider frontalTrigger;
    [SerializeField] SpriteRenderer topSprite;
    [SerializeField] GameObject atkHitbox;
    [SerializeField] float iFrames;
    private List<InteractableBase> objectsInRange;
    private GameObject pushingObj;
    private InteractableBase notaAbierta;
    private bool invulnerable;
    private float iFramesTimer;
    public Vector3 spanwPoint { get; private set; }

    // Efectos de sonido
    [Header("Efectos de sonido")]
    public AudioClip stepSfx;
    public AudioClip stepSoftSfx;
    public AudioClip hurtSfx;
    public AudioClip pushSfx;
    public AudioClip changeSfx;

    // Eventos
    public delegate void HighlightToggle(bool isHighlighted);
    public static event HighlightToggle OnHighlightToggle;
    public static event Action PlayerDied;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();
        cldr = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
        isAlive = true;
        health = 3;
        speed = 0;
        personaActiva = 0;
        iFramesTimer = 0;
        isSprint = false;
        isSneak = false;
        isObserve = false;
        lookLeft = false;
        estadoJugador = 0;
        topSprite.enabled = false;
        objectsInRange = new List<InteractableBase>();
        originalGlowColor = new Color(0, 0, 0);
    }

    private void OnEnable()
    {
        if (inputActions == null)
            inputActions = new InputSystem_Actions();

        inputActions.Enable();
        // Subscribe to input events
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Sprint.performed += OnSkill;
        inputActions.Player.Sprint.canceled += OnSkill;
        inputActions.Player.Interact.performed += OnInteract;
        inputActions.Player.Attack.performed += OnAttack;
        TransitionNotifier.OnAttackExit += FinishAttack;
        TransitionNotifier.OnChangeExit += ChangeFinish;
        TransitionNotifier.OnDamageExit += ExitDamage;
    }

    private void OnDisable()
    {
        TransitionNotifier.OnAttackExit -= FinishAttack;
        TransitionNotifier.OnChangeExit -= ChangeFinish;
        TransitionNotifier.OnDamageExit -= ExitDamage;
        // Unsubscribe to avoid leakages
        if (inputActions == null)
            return;
        inputActions.Disable();
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Sprint.performed -= OnSkill;
        inputActions.Player.Sprint.canceled -= OnSkill;
        inputActions.Player.Interact.performed -= OnInteract;
        inputActions.Player.Attack.performed -= OnAttack;
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetHighlight();
        EnableControl();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive || estadoJugador == Estados.JuegoTerminado)
            return;
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
        speed = desiredVelocity.magnitude;
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
        if (context.performed && estadoJugador != Estados.Daño && estadoJugador != Estados.Leer && estadoJugador != Estados.Cambio)
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
        if (estadoJugador == Estados.Daño || estadoJugador == Estados.Ataque)
            return;
        if (estadoJugador == Estados.Empujar)
            StopPush();
        estadoJugador = Estados.Cambio;
        GameEventManager.Instance.ResetHighlight();
        moveInput = Vector2.zero;
        isSprint = false;
        isSneak = false;
        isObserve = false;
        invulnerable = true;
        personaActiva = (short)personaId;
        animator.SetInteger("PersonaActiva", personaId);
        AudioManager.Instance.PlaySFX(changeSfx);
    }

    public void ChangeFinish()
    {
        estadoJugador = Estados.Defecto;
        invulnerable = false;
    }

    private void RotateSprite()
    {
        if (estadoJugador == Estados.Empujar)
            return;
        spriteRenderer.flipX = lookLeft;
        if (lookLeft)
        {
            frontalTrigger.center = new Vector3(-0.7f, 0f, -0.3f);
            atkHitbox.GetComponent<BoxCollider>().center = new Vector3(-0.7f, 0, 0);
            return;
        }
        frontalTrigger.center = new Vector3(0.7f, 0f, -0.3f);
        atkHitbox.GetComponent<BoxCollider>().center = new Vector3(0.7f, 0, 0);
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
        animator.SetBool("Empujar", true);
        StartCoroutine(PlayEffectLoop());
    }

    private void StopPush()
    {
        pushingObj.transform.SetParent(null);
        objectsInRange.Remove(pushingObj.GetComponent<InteractableBase>());
        pushingObj = null;
        animator.SetBool("Empujar", false);
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

    public void StartReading(InteractableBase nota)
    {
        estadoJugador = Estados.Leer;
        moveInput = Vector2.zero;
        notaAbierta = nota;
    }

    public void StopReading()
    {
        if(estadoJugador == Estados.Leer)
        {
            estadoJugador = Estados.Defecto;
        }
        if (notaAbierta != null)
        {
            notaAbierta.ClosePanel();
            notaAbierta = null;
        }
    }

    private System.Collections.IEnumerator PlayEffectLoop()
    {
        while (estadoJugador == Estados.Empujar)
        {
            if (speed > 0)
            {
                AudioManager.Instance.PlaySFX(pushSfx);
                yield return new WaitForSeconds(pushSfx.length);
            }
            else
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (personaActiva != 1 || estadoJugador == Estados.Ataque || estadoJugador == Estados.Leer || estadoJugador == Estados.Empujar || estadoJugador == Estados.Daño)
            return;
        estadoJugador = Estados.Ataque;
        animator.SetTrigger("Atacar");
        atkHitbox.SetActive(true);
    }

    private void FinishAttack()
    {
        animator.ResetTrigger("Atacar");
        estadoJugador = Estados.Defecto;
        atkHitbox.SetActive(false);
    }

    public void TakeDamage()
    {
        if (invulnerable)
            return;
        health -= 1;
        estadoJugador = Estados.Daño;
        animator.ResetTrigger("Daño");
        animator.SetTrigger("Daño");
        AudioManager.Instance.PlaySFX(hurtSfx);
        if (health <= 0)
        {
            isAlive = false;
            animator.SetBool("Muerto", true);
            DisableControl();
            moveInput = Vector2.zero;
            StartCoroutine(DeadSequence());
        }
        else
        {
            // TODO jugador sigue vivo
            invulnerable = true;
            StartCoroutine(InvulnerableTimer());
        }
    }

    public bool IsInvulnerable()
    {
        return invulnerable || !isAlive;
    }

    private IEnumerator InvulnerableTimer()
    {
        iFramesTimer = 0;
        while (iFramesTimer < iFrames)
        {
            iFramesTimer += Time.deltaTime;
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return null;
        }
        spriteRenderer.enabled = true;
        invulnerable = false;
    }

    private IEnumerator DeadSequence()
    {
        yield return new WaitForSeconds(3f);
        PlayerDied?.Invoke();
    }

    public void SetSpawnPoint(Vector3 spawnPoint)
    {
        this.spanwPoint = spawnPoint;
    }

    public void SetHighlight()
    {
        if (highlightCoroutine != null)
            StopCoroutine(highlightCoroutine);

        highlightCoroutine = StartCoroutine(TransitionHighlightColor());
    }

    private IEnumerator TransitionHighlightColor()
    {
        Color startColor = highlightMaterial.GetColor("_GlowColor");
        Color targetColor = highlightGlowColor;
        float time = 0f;

        while (time < highlightDuration)
        {
            time += Time.deltaTime;
            Color newColor = Color.Lerp(startColor, targetColor, time / highlightDuration);
            highlightMaterial.SetColor("_GlowColor", newColor);
            yield return null;
        }

        // Ensure it's set exactly at the end
        highlightMaterial.SetColor("_GlowColor", targetColor);
    }

    public void ResetHighlight()
    {
        if (highlightCoroutine != null)
            StopCoroutine(highlightCoroutine);

        highlightCoroutine = StartCoroutine(TransitionOriginalColor());
    }

    private IEnumerator TransitionOriginalColor()
    {
        Color startColor = highlightMaterial.GetColor("_GlowColor");
        Color targetColor = originalGlowColor;
        float time = 0f;

        while (time < highlightDuration)
        {
            time += Time.deltaTime;
            Color newColor = Color.Lerp(startColor, targetColor, time / highlightDuration);
            highlightMaterial.SetColor("_GlowColor", newColor);
            yield return null;
        }

        // Ensure it's set exactly at the end
        highlightMaterial.SetColor("_GlowColor", targetColor);
    }

    private void ExitDamage()
    {
        estadoJugador = Estados.Defecto;
        if (!isAlive)
            animator.Play("Die");
    }

    public void ForceTransition(int personaId)
    {

        switch (personaId)
        {
            case 1:
                animator.Play("Idle Dep");
                animator.SetInteger("PersonaActiva", 1);
                break;
            case 2:
                animator.Play("Idle Int");
                animator.SetInteger("PersonaActiva", 2);
                break;
            default:
                animator.Play("Idle Default");
                animator.SetInteger("PersonaActiva", 0);
                break;
        }
        personaActiva = (short)personaId;
    }
}
