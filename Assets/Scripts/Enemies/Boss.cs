using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Boss : MonoBehaviour, IDamageable
{
    public int health;
    [SerializeField] Collider search, searchSmall;
    [SerializeField] GameObject agroRange;
    [SerializeField] GameObject atkHitbox;
    [SerializeField] float atkCooldown;
    private float cooldownTimer;
    private bool isAgro, isAlive, isAttacking, isRage;
    private NavMeshAgent agent;
    private List<GameObject> searchNodes;
    private GameObject currentNode;
    private float patrolTimer, rageTreshold;
    private PlayerController player;

    [Header("Sprite")]
    [SerializeField] SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Search")]
    public float minWait;
    public float maxWait;

    [Header("Evento")]
    [SerializeField] GameFlags gameFlag;

    [Header("Efectos de sonido")]
    private AudioSource audioSource;
    public AudioClip stepSfx1, stepSfx2;

    // Start is called before the first frame update
    void Start()
    {
        searchNodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("Search Node"));
        isAlive = true;
        isAgro = false;
        isAttacking = false;
        rageTreshold = health * 2 / 3;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.isStopped = false;
        patrolTimer = 0;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        SetNewDestination();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
            return;
        if (isAttacking)
        {
            CooldownCount();
            return;
        }
        animator.SetFloat("Velocidad", agent.velocity.magnitude);
        if (isAgro)
        {
            ChasePlayer();
            if (agent.remainingDistance <= 2)
            {
                FacePlayer();
                StartAttack();
                return;
            }
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            SearchWait();
        }
        FlipSprite();
    }

    void OnAnimatorMove()
    {
        transform.position = agent.nextPosition;
    }

    public bool IsInvulnerable()
    {
        return false;
    }

    public void TakeDamage()
    {
        health -= 1;
        if (health <= 0)
        {
            Die();
        }
        else if (!isRage && health <= rageTreshold)
        {
            Enrage();
        }
    }

    private void SetNewDestination()
    {
        int i;
        List<GameObject> list;
        if (currentNode == null)
        {
            i = Random.Range(0, searchNodes.Count);
            currentNode = searchNodes[i];
        }
        else
        {
            i = Random.Range(0, searchNodes.Count - 1);
            list = new List<GameObject>(searchNodes);
            list.Remove(currentNode);
            currentNode = list[i];
            list.Clear();
        }
        agent.destination = currentNode.transform.position;
        agent.isStopped = false;
    }

    private void SearchWait()
    {
        if (patrolTimer > 0)
        {
            patrolTimer -= Time.deltaTime;
        }
        else
        {
            if (minWait >= maxWait)
                patrolTimer = minWait;
            else
                patrolTimer = Random.Range(minWait, maxWait);
            SetNewDestination() ;
        }
    }

    private void FlipSprite()
    {
        if (agent.velocity.x < 0)
        {
            spriteRenderer.flipX = true;
            atkHitbox.GetComponent<BoxCollider>().center = new Vector3(-1f, -0.1f, 0);
        }
        else if (agent.velocity.x > 0)
        {
            spriteRenderer.flipX = false;
            atkHitbox.GetComponent<BoxCollider>().center = new Vector3(1f, -0.1f, 0);
        }
    }

    public void TriggerAgro(PlayerController target)
    {
        isAgro = true;
        player = target;
        agroRange.SetActive(true);
        patrolTimer = 0;
    }

    public void StopAgro()
    {
        isAgro = false;
        player = null;
        agroRange.SetActive(false);
        agent.ResetPath();
        patrolTimer = 2f;
    }

    private void Die()
    {
        isAlive = false;
        agent.isStopped = true;
        animator.SetBool("Muerto", true);
        DeactivateHitbox();
        StartCoroutine(DeathSequence());
    }

    private void CheckPlayer()
    {
        if (!player.isAlive)
        {
            agent.isStopped = true;
        }
    }

    private IEnumerator DeathSequence()
    {
        float timer = 0;
        while (timer < 3)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        GameEventManager.Instance.SetFlag(gameFlag);
        Destroy(gameObject);
    }

    private void StartAttack()
    {
        agent.isStopped = true;
        animator.ResetTrigger("Atacar");
        animator.SetTrigger("Atacar");
        isAttacking = true;
        cooldownTimer = 0;
    }

    private void CooldownCount()
    {
        if (cooldownTimer < atkCooldown)
        {
            cooldownTimer += Time.deltaTime;
        }
        else
        {
            isAttacking = false;
            agent.isStopped = false;
            DeactivateHitbox();
        }
    }

    private void ChasePlayer()
    {
        agent.destination = player.transform.position;
    }

    public void ActivateHitbox()
    {
        atkHitbox.SetActive(true);
    }

    public void DeactivateHitbox()
    {
        atkHitbox.SetActive(false);
    }

    private void FacePlayer()
    {
        float dir = player.transform.position.x - transform.position.x;
        if (dir < 0)
        {
            spriteRenderer.flipX = true;
            atkHitbox.GetComponent<BoxCollider>().center = new Vector3(-1f, -0.1f, 0);
        }
        else if (dir > 0)
        {
            spriteRenderer.flipX = false;
            atkHitbox.GetComponent<BoxCollider>().center = new Vector3(1f, -0.1f, 0);
        }
    }

    private void Enrage()
    {
        isRage = true;
        agent.speed *= 4 / 3;
    }

    public void PlaySfx(int index)
    {
        switch (index)
        {
            default:
                break;
        }
    }
}
