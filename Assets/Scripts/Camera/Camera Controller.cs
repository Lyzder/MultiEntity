using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Poisición y movimiento
    [Header("Position Offset")]
    public Vector3 cameraOffset;
    private Transform player;
    private Transform[] playerCheckPoints;
    private bool checkpointsAssigned = false;
    private List<Renderer> currentObstructions = new List<Renderer>();
    private HashSet<Renderer> lastObstructions = new HashSet<Renderer>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
            if (player == null) return;
        }
        if (player != null && !checkpointsAssigned)
        {
            AssignCheckPoints();
        }
        FollowPlayer();
        HandleObstructions();
    }

    private void FollowPlayer()
    {
        transform.position = player.position + cameraOffset;
    }


    private void HandleObstructions()
    {
        // Step 1: Assign checkpoints if not done
        if (player != null && !checkpointsAssigned)
        {
            AssignCheckPoints();
            if (playerCheckPoints.Length == 0) return;
        }

        // Step 2: Reset previous obstructions
        foreach (var rend in lastObstructions)
        {
            if (rend != null)
            {
                foreach (var mat in rend.materials)
                {
                    if (mat.HasProperty("_Alpha"))
                        mat.SetFloat("_Alpha", 1.0f);
                }
            }
        }

        currentObstructions.Clear();

        // Step 3: Count how many player points are blocked by each object
        Dictionary<Renderer, int> hitCounts = new Dictionary<Renderer, int>();

        foreach (var point in playerCheckPoints)
        {
            Vector3 dir = point.position - transform.position;
            float dist = dir.magnitude;

            RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, dist);
            foreach (var hit in hits)
            {
                if (hit.collider.isTrigger) continue; // Skip trigger colliders
                if (hit.collider.CompareTag("Player")) continue;

                Renderer rend = hit.collider.GetComponentInChildren<Renderer>();
                if (rend != null)
                {
                    if (!hitCounts.ContainsKey(rend))
                        hitCounts[rend] = 0;

                    hitCounts[rend]++;
                }
            }
        }

        // Step 4: Apply transparency only if object blocks enough check points
        foreach (var kvp in hitCounts)
        {
            Renderer rend = kvp.Key;
            int count = kvp.Value;

            if (count >= 2) // ? Adjust this number if you want to be more or less strict
            {
                foreach (var mat in rend.materials)
                {
                    if (mat.HasProperty("_Alpha"))
                        mat.SetFloat("_Alpha", 0.3f); // Your desired transparency
                }
                currentObstructions.Add(rend);
            }
        }

        lastObstructions = new HashSet<Renderer>(currentObstructions);
    }


    private void AssignCheckPoints()
    {
        // Assuming the check points are children named "Check_Head", "Check_Torso", etc.
        List<Transform> foundPoints = new List<Transform>();

        foreach (Transform child in player.GetComponentsInChildren<Transform>())
        {
            if (child.name.StartsWith("Check_"))
            {
                foundPoints.Add(child);
            }
        }

        playerCheckPoints = foundPoints.ToArray();
        checkpointsAssigned = true;
    }
}