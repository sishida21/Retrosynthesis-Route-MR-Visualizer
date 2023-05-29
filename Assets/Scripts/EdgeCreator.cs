using System.Collections.Generic;
using UnityEngine;

public class EdgeCreator : MonoBehaviour
{
    private Material edgeMaterial;
    private Dictionary<GameObject, LineRenderer> lineRenderers = new Dictionary<GameObject, LineRenderer>();


    private void Awake()
    {
        edgeMaterial = new Material(Shader.Find("Standard"));
        edgeMaterial.color = new Color(0.6f, 0.4f, 0.3f);
    }

    public GameObject CreateEdge(GameObject source, GameObject target)
    {
        GameObject edge = new GameObject("Edge" + source.name + "_" + target.name);
        LineRenderer line = edge.AddComponent<LineRenderer>();
        line.receiveShadows = false;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        edge.transform.localScale = Vector3.one * 0.001f;

        line.material = edgeMaterial;
        line.startWidth = 0.001f;
        line.endWidth = 0.001f;
        line.positionCount = 2;

        line.SetPosition(0, source.transform.position);
        line.SetPosition(1, target.transform.position);

        lineRenderers[edge] = line;

        return edge;
    }

    public void UpdateEdgePosition(GameObject edge, GameObject source, GameObject target)
    {
        // LineRenderer line = edge.GetComponent<LineRenderer>();
        LineRenderer line;

        if (lineRenderers.TryGetValue(edge, out line))
        {

            float sourceRadius;
            float targetRadius;
            // Source
            if (source.tag == "MolContainer")
            {
                GameObject sourceSphere = source.transform.Find("NodeSpherePrefab(Clone)").gameObject;
                sourceRadius = sourceSphere.transform.lossyScale.x / 2.0f;
            } 
            else  // RxnContainer
            {
                sourceRadius = source.transform.lossyScale.x / 2.0f;
            }
            // Target
            if (target.tag == "MolContainer")
            {
                GameObject targetSphere = target.transform.Find("NodeSpherePrefab(Clone)").gameObject;
                targetRadius = targetSphere.transform.lossyScale.x / 2.0f;
            }
            else
            {
                targetRadius = target.transform.lossyScale.x / 2.0f;
            }

            Vector3 directionToTarget = (target.transform.position - source.transform.position).normalized;
            Vector3 edgeStartPos = source.transform.position + directionToTarget * sourceRadius;
            Vector3 edgeEndPos = target.transform.position - directionToTarget * targetRadius;

            line.SetPosition(0, edgeStartPos);
            line.SetPosition(1, edgeEndPos);
        }
        else
        {
            Debug.LogWarning("Edge does not have a cached LineRenderer.");
        }
    }
}

