using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class Node
{
    public string id;
    public string smiles;
}

[System.Serializable]
public class Edge
{
    public string id;
    public string from;
    public string to;
}

[System.Serializable]
public class DataGraph
{
    public List<Node> nodes;
    public List<Edge> edges;
}

[System.Serializable]
public class Root
{
    public DataGraph dataGraph;
    public DataGraph dispGraph;
    public int version;
}

public class NetworkVisualizer : MonoBehaviour
{
    public NodeCreator nodeCreator;
    public EdgeCreator edgeCreator;

    public Dictionary<string, GameObject> nodeLookup = new Dictionary<string, GameObject>();
    Dictionary<string, Node> nodeDataLookup = new Dictionary<string, Node>();
    Dictionary<string, List<string>> adjacencyList = new Dictionary<string, List<string>>();

    // Start is called before the first frame update
    void Start()
    {
        InitializeData();
    }

    public void InitializeData()
    {
        string path = "Assets/Resources/JSON/minimum_network.json";
        Root root = LoadJsonData(path);

        foreach (Node node in root.dataGraph.nodes)
        {
            nodeDataLookup.Add(node.id, node);
            adjacencyList.Add(node.id, new List<string>());
        }

        foreach (Edge edge in root.dataGraph.edges)
        {
            adjacencyList[edge.from].Add(edge.to);
        }

        string rootId = root.dataGraph.nodes[0].id; // Assuming the first node is the root
        Debug.Log(rootId);
        PositionNodes(rootId, 0, 0);

        foreach (Node node in root.dataGraph.nodes)
        {
            GameObject nodeObj = nodeCreator.CreateNode(node.id);
            nodeLookup.Add(node.id, nodeObj);
        }

        foreach (Edge edge in root.dataGraph.edges)
        {
            GameObject source = nodeLookup[edge.from];
            GameObject target = nodeLookup[edge.to];
            edgeCreator.CreateEdge(source, target);
        }
    }

    private Root LoadJsonData(string filePath)
    {
        string jsonData = File.ReadAllText(filePath);
        Root root = JsonUtility.FromJson<Root>(jsonData);
        return root;
    }

    private void PositionNodes(string nodeId, float x, float y)
    {
        GameObject nodeObj = nodeCreator.CreateNode(nodeId);
        nodeObj.transform.position = new Vector3(x, y, 0);

        int childCount = adjacencyList[nodeId].Count;
        for (int i = 0; i < childCount; i++)
        {
            PositionNodes(
                adjacencyList[nodeId][i],
                x + i - childCount / 2f,
                y - 1);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
