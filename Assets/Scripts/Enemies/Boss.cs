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
    [SerializeField] BoxCollider atkHitbox;
    private bool isAgro, isAlive, isAttacking;
    private NavMeshAgent agent;
    private List<GameObject> searchNodes;
    private GameObject currentNode;
    private float patrolTimer;
    private PlayerController player;

    [Header("Sprite")]
    [SerializeField] SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Search")]
    public float minWait;
    public float maxWait;

    [Header("Evento")]
    [SerializeField] GameFlags gameFlag;

    // Start is called before the first frame update
    void Start()
    {
        isAlive = true;
        isAgro = false;
        isAttacking = false;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.destination = transform.position;
        patrolTimer = 0;
        searchNodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("Search Node"));
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
            return;
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            SearchWait();
        }
        FlipSprite();
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
    }

    private void SetNewDestination()
    {
        int i;
        List<GameObject> list;
        if (currentNode == null)
        {
            i = Random.Range(0, searchNodes.Count - 1);
            currentNode = searchNodes[i];
        }
        else
        {
            i = Random.Range(0, searchNodes.Count - 2);
            list = new List<GameObject>(searchNodes);
            list.Remove(currentNode);
            currentNode = list[i];
            list.Clear();
        }
        agent.destination = currentNode.transform.position;
        Debug.Log(currentNode);
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
            atkHitbox.center = new Vector3(-0.7f, -0.1f, 0);
        }
        else if (agent.velocity.x > 0)
        {
            spriteRenderer.flipX = false;
            atkHitbox.center = new Vector3(0.7f, -0.1f, 0);
        }
    }

    public void TriggerAgro(PlayerController target)
    {
        isAgro = true;
        player = target;
        agroRange.SetActive(true);
        Debug.Log("Is agro");
    }

    public void StopAgro()
    {
        isAgro = false;
        player = null;
        agroRange.SetActive(false);
        Debug.Log("No agro");
    }

    private void Die()
    {
        isAlive = false;
        agent.isStopped = true;
        animator.SetBool("Muerto", true);
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
}
