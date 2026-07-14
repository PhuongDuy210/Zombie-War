using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CameraObstruction : MonoBehaviour
{
    public Transform player;
    public LayerMask obstructionMask;

    private List<Renderer> currentObstructions = new List<Renderer>();

    void Update()
    {
        // Reset previous obstructions
        foreach (var r in currentObstructions)
            ResetMaterial(r);
        currentObstructions.Clear();

        // Cast ray from camera to player
        Vector3 direction = player.position - transform.position;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, direction.magnitude, obstructionMask);

        foreach (var hit in hits)
        {
            List<Renderer> renderers = hit.collider.GetComponentsInChildren<Renderer>().ToList();
            renderers.Add(hit.collider.GetComponent<Renderer>());
            foreach (Renderer r in renderers)
            {
                SetTransparent(r, 0.2f);
                currentObstructions.Add(r);
            }
        }
    }

    void SetTransparent(Renderer r, float alpha)
    {
        foreach (Material m in r.materials)
        {
            Color c = m.color;
            c.a = alpha;
            m.color = c;

            // Force transparent mode
            m.SetFloat("_Surface", 1f);
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite", 0);
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
    }

    void ResetMaterial(Renderer r)
    {
        foreach (Material m in r.materials)
        {
            Color c = m.color;
            c.a = 1f;
            m.color = c;

            // Restore opaque mode
            m.SetFloat("_Surface", 0f);
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            m.SetInt("_ZWrite", 1);
            m.DisableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
        }
    }
}
