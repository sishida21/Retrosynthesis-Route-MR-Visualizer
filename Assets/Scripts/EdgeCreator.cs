using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCreator : MonoBehaviour
{
    public GameObject CreateEdge(GameObject source, GameObject target)
    {
        GameObject edge = new GameObject("Edge_" + source.name + "_" + target.name);
        edge.transform.position = source.transform.position;

        LineRenderer lr = edge.AddComponent<LineRenderer>();
        lr.SetPosition(0, source.transform.position);
        lr.SetPosition(1, target.transform.position);

        return edge;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
