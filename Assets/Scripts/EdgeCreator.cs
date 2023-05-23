using UnityEngine;

public class EdgeCreator : MonoBehaviour
{
    public GameObject CreateEdge(GameObject source, GameObject target)
    {
        GameObject edge = new GameObject("Edge" + source.name + "_" + target.name);
        edge.transform.SetParent(transform);
        LineRenderer line = edge.AddComponent<LineRenderer>();
        edge.transform.localScale = Vector3.one * 0.1f;

        Material edgeMaterial = new Material(Shader.Find("Standard"));
        edgeMaterial.color = Color.gray;

        line.material = edgeMaterial;
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;
        line.positionCount = 2;

        line.SetPosition(0, source.transform.position);
        line.SetPosition(1, target.transform.position);

        return edge;
    }

    public void UpdateEdgePosition(GameObject edge, GameObject source, GameObject target)
    {
        LineRenderer line = edge.GetComponent<LineRenderer>();

        float sourceRadius;
        float targetRadius;
        // Source
        if (source.tag == "MolContainer")
        {
            GameObject sourceSphere = source.transform.Find("NodeSpherePrefab(Clone)").gameObject;
            sourceRadius = sourceSphere.transform.localScale.x / 2.0f;
        } 
        else  // RxnContainer
        {
            sourceRadius = source.transform.localScale.x / 2.0f;
        }
        // Target
        if (target.tag == "MolContainer")
        {
            GameObject targetSphere = target.transform.Find("NodeSpherePrefab(Clone)").gameObject;
            targetRadius = targetSphere.transform.localScale.x / 2.0f;
        }
        else
        {
            targetRadius = target.transform.localScale.x / 2.0f;
        }

        Vector3 directionToTarget = (target.transform.position - source.transform.position).normalized;
        Vector3 edgeStartPos = source.transform.position + directionToTarget * sourceRadius;
        Vector3 edgeEndPos = target.transform.position - directionToTarget * targetRadius;

        line.SetPosition(0, edgeStartPos);
        line.SetPosition(1, edgeEndPos);
    }
}

