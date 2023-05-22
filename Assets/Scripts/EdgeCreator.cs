using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
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
}

