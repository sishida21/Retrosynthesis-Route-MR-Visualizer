using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NodeCreator : MonoBehaviour
{
    public Material nodeMaterial;

    public GameObject CreateNode(string nodeId)
    {
        GameObject node = new GameObject(nodeId);

        MeshFilter meshFilter = node.AddComponent<MeshFilter>();
        meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");

        MeshRenderer meshRenderer = node.AddComponent<MeshRenderer>();
        meshRenderer.material = nodeMaterial;

        SphereCollider sphereCollider = node.AddComponent<SphereCollider>();
        sphereCollider.radius = 0.5f;

        return node;
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
