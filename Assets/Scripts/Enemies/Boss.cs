using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour, IDamageable
{
    public int health;
    [SerializeField] Collider search, searchSmall;
    [SerializeField] GameObject atkHitbox;
    private bool isAgro, isAlive, isAttacking;
    private NavMeshAgent agent;
    private List<GameObject> searchNodes;
    private GameObject currentNode;
    private float patrolTimer;

    [Header("Sprite")]
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("Search")]
    public float minWait, maxWait;

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
    }

    // Update is called once per frame
    void Update()
    {
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
            Debug.Log(patrolTimer);
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
            spriteRenderer.flipX = true;
        else if (agent.velocity.x > 0)
            spriteRenderer.flipX = false;
    }
}
