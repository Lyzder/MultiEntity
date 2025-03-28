using System.Collections.Generic;
using UnityEngine;

public class CameraOcclusion : MonoBehaviour
{
    public Transform player;
    public LayerMask occlusionLayer;
    private List<Renderer> hiddenObjects = new List<Renderer>();

    void Update()
    {
        ClearHiddenObjects();

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

    void ClearHiddenObjects()
    {
        foreach (var obj in hiddenObjects)
        {
            if (obj != null) SetTransparency(obj, 1f); // Restore original opacity
        }
        hiddenObjects.Clear();
    }

    void SetTransparency(Renderer renderer, float alpha)
    {
        foreach (Material mat in renderer.materials)
        {
            if (mat.HasProperty("_Color"))
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
                mat.SetInt("_Surface", 1); // Ensure it's transparent
                mat.SetInt("_Blend", 1);
                mat.renderQueue = 3000; // Render after opaque objects
            }
        }
    }
}
