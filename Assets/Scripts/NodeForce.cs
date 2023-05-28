using System.Collections.Generic;
using UnityEngine;

public class NodeForce : MonoBehaviour
{
    public float forceMultiplier = 0.5f;
    public float maximumDistance = 0.3f;
    public float stopDistance = 0.05f;
    public float highDrag = 3.0f;
    public float lowDrag = 0.5f;

    private Rigidbody rb;
    private List<GameObject> nodes;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        nodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("MolContainer"));

        bool shouldMove = false;

        foreach (GameObject node in nodes)
        {
            if (node == this.gameObject)
            { continue; }

            Vector3 direction = this.transform.position - node.transform.position;
            float distance = direction.magnitude;

            if (distance <= stopDistance)
            {
                shouldMove = true;
            }
            if (distance > maximumDistance)
            {
                continue;
            }
            Vector3 force = direction.normalized * (forceMultiplier / (distance * distance));
            rb.AddForce(force);
        }

        rb.drag = shouldMove ? lowDrag : highDrag;
        
    }
}
