using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Poisición y movimiento
    [Header("Position Offset")]
    public Vector3 cameraOffset;
    private Transform player;

    // Oclusion de objetos
    [Header("Object Occlusion")]
    public LayerMask occlusionLayer;
    private List<Renderer> hiddenObjects = new List<Renderer>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;
        FollowPlayer();
        ClearHiddenObjects();
        OccludeObjects();
    }

    private void FollowPlayer()
    {
        transform.position = player.position + cameraOffset;
    }

    private void OccludeObjects()
    {
        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, occlusionLayer);
        foreach (var hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {

                SetTransparency(renderer, 0.3f); // Make it semi-transparent
                hiddenObjects.Add(renderer);
            }
        }
    }

    private void ClearHiddenObjects()
    {
        foreach (var obj in hiddenObjects)
        {
            if (obj != null) RestoreTransparency(obj);
        }
        hiddenObjects.Clear();
    }

    private void SetTransparency(Renderer renderer, float alpha)
    {
        foreach (Material mat in renderer.materials)
        {
            if (mat.HasProperty("_BaseColor"))
            {
                // Ensure material is set to Transparent mode
                mat.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent
                mat.SetFloat("_Blend", 0);   // 0 = Alpha, 1 = Premultiplied, 2 = Additive
                mat.SetFloat("_AlphaClip", 0); // Disable alpha clipping
                mat.renderQueue = 3000; // Render after opaque objects

                // Modify transparency
                Color color = mat.GetColor("_BaseColor");
                color.a = alpha;
                mat.SetColor("_BaseColor", color);
            }
        }
    }

    private void RestoreTransparency(Renderer renderer)
    {
        foreach (Material mat in renderer.materials)
        {
            if (mat.HasProperty("_BaseColor"))
            {
                // Restore material settings
                mat.SetFloat("_Surface", 0); // 0 = Opaque, 1 = Transparent
                mat.SetFloat("_Blend", 0);   // 0 = Alpha, 1 = Premultiplied, 2 = Additive
                mat.SetFloat("_AlphaClip", 0);
                mat.renderQueue = 2000; // Default for opaque objects

                // Reset alpha to fully opaque
                Color color = mat.GetColor("_BaseColor");
                color.a = 1f;
                mat.SetColor("_BaseColor", color);
            }
        }
    }

}