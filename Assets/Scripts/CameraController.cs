using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public NetworkVisualizer networkVisualier;
    public float distanceFromCenter = 2.0f;

    void LateUpdate()
    {
        Vector3 networkCenter = CalculateNetworkCenter();
        transform.position = networkCenter - transform.forward * distanceFromCenter;
        transform.LookAt(networkCenter);
    }

    private Vector3 CalculateNetworkCenter()
    {
        Vector3 center = Vector3.zero;
        int count = 0;

        foreach(GameObject node in networkVisualier.nodeLookup.Values)
        {
            Debug.Log(node.transform.position);
            center += node.transform.position;
            count++;
        }

        if (count > 0)
        {
            center /= count;
        }

        return center;
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
